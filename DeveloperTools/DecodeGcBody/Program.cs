using System.Collections;
using System.Reflection;
using System.Text.Json;
using ProtoBuf;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: DecodeGcBody <messageType> <payload.bin>");
    return 2;
}

var messageType = uint.Parse(args[0]);
var path = Path.GetFullPath(args[1]);
var bytes = File.ReadAllBytes(path);
var type = Resolve(messageType);
using var stream = new MemoryStream(bytes);
var decoded = Serializer.NonGeneric.Deserialize(type, stream);
var json = ToJson(decoded);
Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true }));
return 0;

static Type Resolve(uint messageType)
{
    return messageType switch
    {
        21 => typeof(CMsgSOSingleObject),
        24 => typeof(CMsgSOCacheSubscribed),
        26 => typeof(CMsgSOMultipleObjects),
        7038 => typeof(CMsgPracticeLobbyCreate),
        7041 => typeof(CMsgPracticeLobbyLaunch),
        7055 => typeof(CMsgGenericResult),
        _ => throw new InvalidOperationException("Unsupported message type " + messageType)
    };
}

static object? ToJson(object? value, int? soType = null)
{
    if (value == null)
    {
        return null;
    }

    if (value is string or bool or int or uint or short or ushort or byte or sbyte or float or double or decimal)
    {
        return value;
    }

    if (value is long longValue)
    {
        return longValue.ToString();
    }

    if (value is ulong ulongValue)
    {
        return ulongValue.ToString();
    }

    if (value is Enum enumValue)
    {
        return new Dictionary<string, object?>
        {
            ["name"] = enumValue.ToString(),
            ["value"] = Convert.ToInt64(enumValue)
        };
    }

    if (value is byte[] bytes)
    {
        var result = new Dictionary<string, object?>
        {
            ["length"] = bytes.Length,
            ["base64"] = Convert.ToBase64String(bytes)
        };
        var decodedSo = DecodeSo(soType, bytes);
        if (decodedSo != null)
        {
            result["decoded"] = decodedSo;
        }

        return result;
    }

    if (value is IEnumerable enumerable && value is not string)
    {
        var array = new List<object?>();
        foreach (var item in enumerable)
        {
            array.Add(ToJson(item, soType));
        }

        return array;
    }

    return MessageToJson(value);
}

static object MessageToJson(object value)
{
    var result = new Dictionary<string, object?>
    {
        ["__type"] = value.GetType().Name
    };
    var localSoType = ReadIntProperty(value, "TypeId");
    foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(property => property.GetCustomAttribute<ProtoMemberAttribute>() != null))
    {
        if (!ShouldInclude(value, property))
        {
            continue;
        }

        var soType = property.Name.Contains("ObjectData", StringComparison.Ordinal) ? localSoType : null;
        result[JsonName(property)] = ToJson(property.GetValue(value), soType);
    }

    return result;
}

static object? DecodeSo(int? soType, byte[] bytes)
{
    var type = soType switch
    {
        2004 => typeof(CSODOTALobby),
        2013 => typeof(CSODOTALobbyInvite),
        2014 => typeof(CSODOTAStaticLobby),
        2015 => typeof(CSODOTAServerLobby),
        2016 => typeof(CSODOTAServerStaticLobby),
        _ => null
    };
    if (type == null)
    {
        return null;
    }

    try
    {
        using var stream = new MemoryStream(bytes);
        return ToJson(Serializer.NonGeneric.Deserialize(type, stream));
    }
    catch
    {
        return null;
    }
}

static bool ShouldInclude(object instance, PropertyInfo property)
{
    var shouldSerialize = instance.GetType().GetMethod("ShouldSerialize" + property.Name, Type.EmptyTypes);
    if (shouldSerialize != null && shouldSerialize.ReturnType == typeof(bool))
    {
        return (bool)shouldSerialize.Invoke(instance, null)!;
    }

    var value = property.GetValue(instance);
    return value switch
    {
        null => false,
        string text => text.Length > 0,
        Array array => array.Length > 0,
        ICollection collection => collection.Count > 0,
        _ => true
    };
}

static int? ReadIntProperty(object instance, string propertyName)
{
    var property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
    if (property == null || property.PropertyType != typeof(int))
    {
        return null;
    }

    var shouldSerialize = instance.GetType().GetMethod("ShouldSerialize" + property.Name, Type.EmptyTypes);
    if (shouldSerialize != null && shouldSerialize.ReturnType == typeof(bool) &&
        !(bool)shouldSerialize.Invoke(instance, null)!)
    {
        return null;
    }

    return (int)property.GetValue(instance)!;
}

static string JsonName(PropertyInfo property)
{
    var protoName = property.GetCustomAttribute<ProtoMemberAttribute>()?.Name;
    return string.IsNullOrWhiteSpace(protoName)
        ? char.ToLowerInvariant(property.Name[0]) + property.Name[1..]
        : protoName;
}
