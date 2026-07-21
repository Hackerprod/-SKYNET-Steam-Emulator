using System.Text.Json;
using System.Text.Json.Serialization;

namespace SKYNET_server.Json;

public static class SkynetJsonSerializerOptions
{
    public static JsonSerializerOptions CreateCompatible()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        AddCompatibilityConverters(options);
        return options;
    }

    public static void AddCompatibilityConverters(JsonSerializerOptions options)
    {
        options.Converters.Add(new FlexibleDateTimeJsonConverter());
        options.Converters.Add(new FlexibleUInt32JsonConverter());
        options.Converters.Add(new FlexibleUInt64JsonConverter());
        options.Converters.Add(new FlexibleInt32JsonConverter());
        options.Converters.Add(new FlexibleInt64JsonConverter());
        options.Converters.Add(new FlexibleBooleanJsonConverter());
    }
}
