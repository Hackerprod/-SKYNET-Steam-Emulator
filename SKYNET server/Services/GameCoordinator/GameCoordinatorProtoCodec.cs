using System.Collections;
using System.Numerics;
using System.Reflection;
using ProtoBuf;
using TypeSharp.VM.Memory;

namespace SKYNET_server.Services;

public sealed class GameCoordinatorProtoCodec
{
    private readonly object _sync = new();
    private readonly Dictionary<uint, ProtoRegistry> _registries = new();

    public void ConfigureApp(GameCoordinatorAppDefinition app, Assembly defaultAssembly)
    {
        var fingerprint = BuildFingerprint(app, defaultAssembly);
        lock (_sync)
        {
            if (_registries.TryGetValue(app.AppId, out var existing) && existing.Fingerprint == fingerprint)
            {
                return;
            }

            _registries[app.AppId] = BuildRegistry(app, defaultAssembly, fingerprint);
        }
    }

    private static string BuildFingerprint(GameCoordinatorAppDefinition app, Assembly defaultAssembly)
    {
        var sourceIdentities = app.ProtoContracts.Sources.Select(source =>
        {
            var assembly = GameCoordinatorAppCatalog.ResolveContractAssembly(app, source, defaultAssembly);
            return string.Join(':',
                source.Assembly ?? string.Empty,
                assembly.FullName ?? assembly.GetName().Name ?? string.Empty,
                AssemblyFileIdentity(assembly));
        });

        return string.Join('|', new[] { app.RuntimeCacheKey }.Concat(sourceIdentities));
    }

    private static string AssemblyFileIdentity(Assembly assembly)
    {
        if (assembly.IsDynamic || string.IsNullOrWhiteSpace(assembly.Location) || !File.Exists(assembly.Location))
        {
            return "dynamic";
        }

        var file = new FileInfo(assembly.Location);
        return string.Join(':', Path.GetFullPath(file.FullName), file.Length, file.LastWriteTimeUtc.Ticks);
    }

    public TsValue Decode(uint appId, string typeName, byte[] payload)
    {
        var type = Resolve(appId, typeName);
        using var stream = new MemoryStream(payload);
        var message = Serializer.NonGeneric.Deserialize(type, stream);
        return ToTsValue(message);
    }

    public byte[] Encode(uint appId, string typeName, TsValue value)
    {
        var type = Resolve(appId, typeName);
        var message = CreateFromTs(type, value);
        using var stream = new MemoryStream();
        Serializer.NonGeneric.Serialize(stream, message);
        return stream.ToArray();
    }

    private ProtoRegistry BuildRegistry(GameCoordinatorAppDefinition app, Assembly defaultAssembly, string fingerprint)
    {
        var contracts = new List<Type>();
        foreach (var source in app.ProtoContracts.Sources)
        {
            var assembly = GameCoordinatorAppCatalog.ResolveContractAssembly(app, source, defaultAssembly);
            contracts.AddRange(GetLoadableTypes(assembly)
                .Where(IsProtoContract)
                .Where(type => type.FullName?.StartsWith("Google.Protobuf.", StringComparison.Ordinal) != true)
                .Where(type => MatchesSource(type, source)));
        }

        var names = new Dictionary<string, List<Type>>(StringComparer.Ordinal);
        foreach (var type in contracts.Distinct().OrderBy(type => GetCanonicalRuntimeName(type), StringComparer.Ordinal))
        {
            foreach (var name in GetRuntimeNames(type))
            {
                if (!names.TryGetValue(name, out var candidates))
                {
                    candidates = new List<Type>();
                    names[name] = candidates;
                }

                if (!candidates.Contains(type))
                {
                    candidates.Add(type);
                }
            }
        }

        var resolved = new Dictionary<string, Type>(StringComparer.Ordinal);
        var ambiguous = new Dictionary<string, IReadOnlyList<Type>>(StringComparer.Ordinal);
        foreach (var (name, candidates) in names)
        {
            if (candidates.Count == 1)
            {
                resolved[name] = candidates[0];
                continue;
            }

            ambiguous[name] = candidates
                .OrderBy(type => GetCanonicalRuntimeName(type), StringComparer.Ordinal)
                .ToList();
        }

        return new ProtoRegistry(fingerprint, resolved, ambiguous, contracts.Count);
    }

    private Type Resolve(uint appId, string typeName)
    {
        lock (_sync)
        {
            if (!_registries.TryGetValue(appId, out var registry))
            {
                throw new InvalidOperationException($"GC protobuf registry is not configured for app {appId}");
            }

            return registry.Resolve(appId, typeName);
        }
    }

    private static IReadOnlyList<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(type => type != null).Cast<Type>().ToList();
        }
    }

    private static bool IsProtoContract(Type type)
    {
        return type.GetCustomAttribute<ProtoContractAttribute>() != null;
    }

    private static bool MatchesSource(Type type, GameCoordinatorProtoContractSource source)
    {
        var aliases = GetTypeAliases(type).ToList();
        return MatchesExactFilter(aliases, source.TypeNames)
            && MatchesPrefixFilter(aliases, source.TypeNamePrefixes)
            && MatchesPrefixFilter(GetContractNames(type), source.ContractNamePrefixes);
    }

    private static bool MatchesExactFilter(IReadOnlyList<string> aliases, IReadOnlyList<string> filters)
    {
        return filters.Count == 0 || aliases.Any(alias => filters.Contains(alias, StringComparer.Ordinal));
    }

    private static bool MatchesPrefixFilter(IEnumerable<string> aliases, IReadOnlyList<string> prefixes)
    {
        return prefixes.Count == 0 || aliases.Any(alias => prefixes.Any(prefix => alias.StartsWith(prefix, StringComparison.Ordinal)));
    }

    private static IEnumerable<string> GetRuntimeNames(Type type)
    {
        yield return GetCanonicalRuntimeName(type);

        var dotted = GetCanonicalRuntimeName(type).Replace('+', '.');
        if (!string.Equals(dotted, GetCanonicalRuntimeName(type), StringComparison.Ordinal))
        {
            yield return dotted;
        }

        yield return type.Name;

        foreach (var contractName in GetContractNames(type))
        {
            yield return contractName;
        }
    }

    private static IEnumerable<string> GetTypeAliases(Type type)
    {
        yield return GetCanonicalRuntimeName(type);
        yield return GetCanonicalRuntimeName(type).Replace('+', '.');
        yield return type.Name;
        if (!string.IsNullOrWhiteSpace(type.FullName))
        {
            yield return type.FullName!;
        }

        foreach (var contractName in GetContractNames(type))
        {
            yield return contractName;
        }
    }

    private static IEnumerable<string> GetContractNames(Type type)
    {
        var contract = type.GetCustomAttribute<ProtoContractAttribute>();
        if (!string.IsNullOrWhiteSpace(contract?.Name))
        {
            yield return contract.Name!;
        }
    }

    private static string GetCanonicalRuntimeName(Type type)
    {
        return type.FullName ?? type.AssemblyQualifiedName ?? type.Name;
    }

    private sealed class ProtoRegistry
    {
        private readonly IReadOnlyDictionary<string, Type> _types;
        private readonly IReadOnlyDictionary<string, IReadOnlyList<Type>> _ambiguous;
        private readonly int _contractCount;

        public ProtoRegistry(
            string fingerprint,
            IReadOnlyDictionary<string, Type> types,
            IReadOnlyDictionary<string, IReadOnlyList<Type>> ambiguous,
            int contractCount)
        {
            Fingerprint = fingerprint;
            _types = types;
            _ambiguous = ambiguous;
            _contractCount = contractCount;
        }

        public string Fingerprint { get; }

        public Type Resolve(uint appId, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException($"GC protobuf type name is empty for app {appId}");
            }

            if (_types.TryGetValue(typeName, out var type))
            {
                return type;
            }

            if (_ambiguous.TryGetValue(typeName, out var candidates))
            {
                var examples = string.Join(", ", candidates.Take(6).Select(GetCanonicalRuntimeName));
                throw new InvalidOperationException(
                    $"GC protobuf type '{typeName}' is ambiguous for app {appId}; use a canonical runtime name. Candidates: {examples}");
            }

            if (_contractCount == 0)
            {
                throw new InvalidOperationException($"GC app {appId} has no protobuf contract sources configured");
            }

            throw new InvalidOperationException($"GC protobuf type is not registered for app {appId}: {typeName}");
        }
    }
    private object CreateFromTs(Type type, TsValue value)
    {
        if (TryConvertScalar(value, type, out var scalar))
        {
            return scalar!;
        }

        if (type == typeof(byte[]))
        {
            return ToByteArray(value);
        }

        if (type.IsArray)
        {
            return CreateArrayFromTs(type, value);
        }

        var instance = Activator.CreateInstance(type)
            ?? throw new InvalidOperationException($"Cannot create protobuf message {type.Name}");

        if (value is not TsObjectValue objectValue)
        {
            throw new InvalidOperationException($"Expected object for protobuf message {type.Name}");
        }

        var fields = objectValue.Value.Fields;
        var consumed = new HashSet<string>(StringComparer.Ordinal);

        foreach (var property in GetProtoProperties(type))
        {
            var aliases = GetFieldAliases(property);
            var matchedName = aliases.FirstOrDefault(alias => fields.ContainsKey(alias));
            if (matchedName == null)
            {
                continue;
            }

            var fieldValue = fields[matchedName];
            foreach (var alias in aliases)
            {
                consumed.Add(alias);
            }

            if (fieldValue is TsVoid or TsNull)
            {
                continue;
            }

            try
            {
                ApplyProperty(instance, property, fieldValue);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to encode field {type.Name}.{property.Name} "
                    + $"({property.PropertyType.FullName}) from {fieldValue.ValueType}/{fieldValue.RawValue?.GetType().FullName ?? "null"}",
                    ex);
            }
        }

        foreach (var supplied in fields.Keys)
        {
            if (fields[supplied] is TsVoid)
            {
                continue;
            }

            if (!consumed.Contains(supplied))
            {
                throw new InvalidOperationException(
                    $"Unknown field '{supplied}' for protobuf message {type.Name}");
            }
        }

        return instance;
    }

    private void ApplyProperty(object instance, PropertyInfo property, TsValue value)
    {
        var propertyType = property.PropertyType;
        if (propertyType == typeof(byte[]))
        {
            if (!property.CanWrite)
            {
                throw new InvalidOperationException(
                    $"Protobuf property {property.DeclaringType?.Name}.{property.Name} is not writable");
            }

            property.SetValue(instance, ToByteArray(value));
            return;
        }

        if (propertyType.IsArray)
        {
            if (!property.CanWrite)
            {
                throw new InvalidOperationException(
                    $"Protobuf property {property.DeclaringType?.Name}.{property.Name} is not writable");
            }

            property.SetValue(instance, CreateArrayFromTs(propertyType, value));
            return;
        }

        if (typeof(IList).IsAssignableFrom(propertyType) && property.GetValue(instance) is IList list)
        {
            list.Clear();
            var itemType = propertyType.IsGenericType
                ? propertyType.GetGenericArguments()[0]
                : typeof(object);

            foreach (var item in EnumerateArray(value))
            {
                list.Add(CreateFromTs(itemType, item));
            }

            return;
        }

        if (!property.CanWrite)
        {
            throw new InvalidOperationException(
                $"Protobuf property {property.DeclaringType?.Name}.{property.Name} is not writable");
        }

        property.SetValue(instance, CreateFromTs(propertyType, value));
    }

    private object CreateArrayFromTs(Type arrayType, TsValue value)
    {
        var itemType = arrayType.GetElementType()
            ?? throw new InvalidOperationException($"Cannot determine array element type for {arrayType.FullName}");
        var items = EnumerateArray(value).ToList();
        var array = Array.CreateInstance(itemType, items.Count);
        for (var i = 0; i < items.Count; i++)
        {
            array.SetValue(CreateFromTs(itemType, items[i]), i);
        }

        return array;
    }

    private static IEnumerable<TsValue> EnumerateArray(TsValue value)
    {
        if (value is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException("Expected array value");
        }

        for (var i = 0; i < arrayValue.Value.Count; i++)
        {
            yield return arrayValue.Value.Get(i);
        }
    }

    private static byte[] ToByteArray(TsValue value)
    {
        if (value is TsStringValue stringValue)
        {
            return Convert.FromBase64String(stringValue.Value);
        }

        if (value is TsUint8ArrayValue bytesValue)
        {
            var copy = new byte[bytesValue.Length];
            Array.Copy(bytesValue.Value, copy, copy.Length);
            return copy;
        }

        if (value is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException("Expected Uint8Array, byte array, or base64 string");
        }

        var bytes = new byte[arrayValue.Value.Count];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(ReadNumber(arrayValue.Value.Get(i)));
        }

        return bytes;
    }

    private static bool TryConvertScalar(TsValue value, Type targetType, out object? result)
    {
        var nullableType = Nullable.GetUnderlyingType(targetType);
        if (nullableType != null)
        {
            if (value is TsNull or TsVoid)
            {
                result = null;
                return true;
            }

            return TryConvertScalar(value, nullableType, out result);
        }

        if (targetType.IsEnum)
        {
            var underlyingType = Enum.GetUnderlyingType(targetType);
            var enumValue = ConvertInteger(ReadInteger(value), underlyingType);
            result = Enum.ToObject(targetType, enumValue);
            return true;
        }

        if (targetType == typeof(string))
        {
            result = value is TsStringValue stringValue ? stringValue.Value : value.ToString() ?? string.Empty;
            return true;
        }

        if (targetType == typeof(bool))
        {
            result = value is TsBoolValue boolValue
                ? boolValue.Value
                : ReadNumber(value) != 0;
            return true;
        }

        if (targetType == typeof(uint))
        {
            result = ConvertInteger(ReadInteger(value), targetType);
            return true;
        }

        if (targetType == typeof(int))
        {
            result = ConvertInteger(ReadInteger(value), targetType);
            return true;
        }

        if (targetType == typeof(ulong))
        {
            result = ConvertInteger(ReadInteger(value), targetType);
            return true;
        }

        if (targetType == typeof(long))
        {
            result = ConvertInteger(ReadInteger(value), targetType);
            return true;
        }

        if (targetType == typeof(float))
        {
            result = Convert.ToSingle(ReadNumber(value));
            return true;
        }

        if (targetType == typeof(double))
        {
            result = Convert.ToDouble(ReadNumber(value));
            return true;
        }

        result = null;
        return false;
    }

    private static decimal ReadNumber(TsValue value)
    {
        return value switch
        {
            TsInt32Value int32Value => int32Value.Value,
            TsInt64Value int64Value => int64Value.Value,
            TsUInt64Value uint64Value => uint64Value.Value,
            TsBigIntValue bigIntValue => (decimal)bigIntValue.Value,
            TsFloat32Value float32Value => (decimal)float32Value.Value,
            TsFloat64Value float64Value => (decimal)float64Value.Value,
            TsDecimalValue decimalValue => decimalValue.Value,
            TsStringValue stringValue when decimal.TryParse(stringValue.Value, out var parsed) => parsed,
            _ => throw new InvalidOperationException($"Expected numeric value, got {value.ValueType}")
        };
    }

    private static BigInteger ReadInteger(TsValue value)
    {
        return value switch
        {
            TsInt32Value int32Value => int32Value.Value,
            TsInt64Value int64Value => int64Value.Value,
            TsUInt64Value uint64Value => uint64Value.Value,
            TsBigIntValue bigIntValue => bigIntValue.Value,
            TsFloat32Value float32Value => new BigInteger(float32Value.Value),
            TsFloat64Value float64Value => new BigInteger(float64Value.Value),
            TsDecimalValue decimalValue => new BigInteger(decimalValue.Value),
            TsStringValue stringValue when BigInteger.TryParse(stringValue.Value, out var parsed) => parsed,
            _ => throw new InvalidOperationException($"Expected integer value, got {value.ValueType}")
        };
    }

    private static object ConvertInteger(BigInteger value, Type targetType)
    {
        if (targetType == typeof(byte)) return CheckedInteger<byte>(value, byte.MinValue, byte.MaxValue);
        if (targetType == typeof(sbyte)) return CheckedInteger<sbyte>(value, sbyte.MinValue, sbyte.MaxValue);
        if (targetType == typeof(short)) return CheckedInteger<short>(value, short.MinValue, short.MaxValue);
        if (targetType == typeof(ushort)) return CheckedInteger<ushort>(value, ushort.MinValue, ushort.MaxValue);
        if (targetType == typeof(int)) return CheckedInteger<int>(value, int.MinValue, int.MaxValue);
        if (targetType == typeof(uint)) return CheckedInteger<uint>(value, uint.MinValue, uint.MaxValue);
        if (targetType == typeof(long)) return CheckedInteger<long>(value, long.MinValue, long.MaxValue);
        if (targetType == typeof(ulong)) return CheckedInteger<ulong>(value, ulong.MinValue, ulong.MaxValue);
        throw new InvalidOperationException($"Unsupported integer target type {targetType.FullName}");
    }

    private static T CheckedInteger<T>(BigInteger value, BigInteger min, BigInteger max)
        where T : struct, IConvertible
    {
        if (value < min || value > max)
            throw new OverflowException($"Integer value {value} is outside the range of {typeof(T).Name}");

        return (T)Convert.ChangeType(value.ToString(), typeof(T), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static TsValue ToTsValue(object? value)
    {
        if (value == null)
        {
            return TsValue.Null;
        }

        if (value is byte[] bytes)
        {
            var copy = new byte[bytes.Length];
            Array.Copy(bytes, copy, copy.Length);
            return new TsUint8ArrayValue(copy);
        }

        if (value is string stringValue)
        {
            return TsValue.FromString(stringValue);
        }

        if (value is bool boolValue)
        {
            return TsValue.FromBool(boolValue);
        }

        if (value is uint uintValue)
        {
            return uintValue <= int.MaxValue
                ? TsValue.FromInt32((int)uintValue)
                : TsValue.FromInt64(uintValue);
        }

        if (value is int intValue)
        {
            return TsValue.FromInt32(intValue);
        }

        if (value is ulong ulongValue)
        {
            return TsValue.FromUInt64(ulongValue);
        }

        if (value is long longValue)
        {
            return TsValue.FromInt64(longValue);
        }

        if (value is float floatValue)
        {
            return TsValue.FromFloat32(floatValue);
        }

        if (value is double doubleValue)
        {
            return TsValue.FromFloat64(doubleValue);
        }

        if (value is Enum enumValue)
        {
            return TsValue.FromInt32(Convert.ToInt32(enumValue));
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var array = new TsArray();
            foreach (var item in enumerable)
            {
                array.Add(ToTsValue(item));
            }

            return new TsArrayValue(array);
        }

        var objectValue = new TsObject(value.GetType().Name);
        foreach (var property in GetProtoProperties(value.GetType()))
        {
            var propertyValue = ToTsValue(property.GetValue(value));
            foreach (var alias in GetFieldAliases(property))
            {
                objectValue.SetField(alias, propertyValue);
            }
        }

        return new TsObjectValue(objectValue);
    }

    private static IEnumerable<PropertyInfo> GetProtoProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttribute<ProtoMemberAttribute>() != null);
    }

    private static string GetFieldName(PropertyInfo property)
    {
        var protoName = property.GetCustomAttribute<ProtoMemberAttribute>()?.Name;
        if (!string.IsNullOrWhiteSpace(protoName))
        {
            return SnakeToCamel(protoName);
        }

        return char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
    }

    private static IReadOnlyList<string> GetFieldAliases(PropertyInfo property)
    {
        var aliases = new List<string>
        {
            GetFieldName(property),
            char.ToLowerInvariant(property.Name[0]) + property.Name[1..],
            property.Name,
            SnakeToCamel(property.Name)
        };
        return aliases.Distinct(StringComparer.Ordinal).ToList();
    }

    private static string SnakeToCamel(string value)
    {
        var result = new char[value.Length];
        var index = 0;
        var upperNext = false;
        foreach (var c in value)
        {
            if (c == '_')
            {
                upperNext = true;
                continue;
            }

            result[index++] = upperNext ? char.ToUpperInvariant(c) : c;
            upperNext = false;
        }

        return new string(result, 0, index);
    }
}
