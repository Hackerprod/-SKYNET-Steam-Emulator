using System.Collections;
using System.Numerics;
using System.Reflection;
using ProtoBuf;
using TypeSharp.VM.Memory;

namespace SKYNET_server.Services;

public sealed class GameCoordinatorProtoCodec
{
    private readonly Dictionary<string, Type> _types = new(StringComparer.Ordinal);

    public GameCoordinatorProtoCodec()
    {
        RegisterGeneratedContracts(typeof(CMsgClientHello).Assembly);
    }

    public TsValue Decode(string typeName, byte[] payload)
    {
        var type = Resolve(typeName);
        using var stream = new MemoryStream(payload);
        var message = Serializer.NonGeneric.Deserialize(type, stream);
        return ToTsValue(message);
    }

    public byte[] Encode(string typeName, TsValue value)
    {
        var type = Resolve(typeName);
        var message = CreateFromTs(type, value);
        using var stream = new MemoryStream();
        Serializer.NonGeneric.Serialize(stream, message);
        return stream.ToArray();
    }

    private void RegisterGeneratedContracts(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<ProtoContractAttribute>() == null ||
                type.FullName?.StartsWith("Google.Protobuf.", StringComparison.Ordinal) == true)
            {
                continue;
            }

            Register(type);
        }
    }

    private void Register(Type type)
    {
        _types[type.Name] = type;
        var contract = type.GetCustomAttribute<ProtoContractAttribute>();
        if (!string.IsNullOrWhiteSpace(contract?.Name))
        {
            _types[contract.Name] = type;
        }

        foreach (var nested in type.GetNestedTypes(BindingFlags.Public))
        {
            RegisterNested(nested);
        }
    }

    private void RegisterNested(Type type)
    {
        _types[type.Name] = type;
        var contract = type.GetCustomAttribute<ProtoContractAttribute>();
        if (!string.IsNullOrWhiteSpace(contract?.Name))
        {
            _types[contract.Name] = type;
        }

        foreach (var nested in type.GetNestedTypes(BindingFlags.Public))
        {
            RegisterNested(nested);
        }
    }

    private Type Resolve(string typeName)
    {
        if (_types.TryGetValue(typeName, out var type))
        {
            return type;
        }

        throw new InvalidOperationException($"GC proto type is not registered: {typeName}");
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
            property.Name
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
