using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SKYNET_server.Json;

public sealed class FlexibleUInt32JsonConverter : JsonConverter<uint>
{
    public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return 0;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetUInt32(out var value))
            {
                return value;
            }

            if (reader.TryGetInt64(out var signed) && signed >= 0 && signed <= uint.MaxValue)
            {
                return (uint)signed;
            }

            var numeric = reader.GetDouble();
            return numeric <= 0 ? 0 : Convert.ToUInt32(Math.Truncate(Math.Min(numeric, uint.MaxValue)));
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected UInt32 token {reader.TokenType}.");
        }

        var text = reader.GetString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        text = text.Trim();
        if (uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
        {
            return decimalValue <= 0 ? 0 : decimal.ToUInt32(decimal.Truncate(Math.Min(decimalValue, uint.MaxValue)));
        }

        throw new JsonException($"Unsupported UInt32 value '{text}'.");
    }

    public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public sealed class FlexibleUInt64JsonConverter : JsonConverter<ulong>
{
    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return 0;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetUInt64(out var value))
            {
                return value;
            }

            if (reader.TryGetInt64(out var signed) && signed >= 0)
            {
                return (ulong)signed;
            }

            var numeric = reader.GetDouble();
            return numeric <= 0 ? 0 : Convert.ToUInt64(Math.Truncate(Math.Min(numeric, ulong.MaxValue)));
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected UInt64 token {reader.TokenType}.");
        }

        var text = reader.GetString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        text = text.Trim();
        if (ulong.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
        {
            return decimalValue <= 0 ? 0 : decimal.ToUInt64(decimal.Truncate(Math.Min(decimalValue, ulong.MaxValue)));
        }

        throw new JsonException($"Unsupported UInt64 value '{text}'.");
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public sealed class FlexibleInt32JsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return 0;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out var value))
            {
                return value;
            }

            var numeric = reader.GetDouble();
            return Convert.ToInt32(Math.Truncate(Math.Clamp(numeric, int.MinValue, int.MaxValue)));
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected Int32 token {reader.TokenType}.");
        }

        var text = reader.GetString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        text = text.Trim();
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
        {
            return decimal.ToInt32(decimal.Truncate(Math.Clamp(decimalValue, int.MinValue, int.MaxValue)));
        }

        throw new JsonException($"Unsupported Int32 value '{text}'.");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public sealed class FlexibleInt64JsonConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return 0;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var value))
            {
                return value;
            }

            var numeric = reader.GetDouble();
            return Convert.ToInt64(Math.Truncate(Math.Clamp(numeric, long.MinValue, long.MaxValue)));
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected Int64 token {reader.TokenType}.");
        }

        var text = reader.GetString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        text = text.Trim();
        if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
        {
            return decimal.ToInt64(decimal.Truncate(Math.Clamp(decimalValue, long.MinValue, long.MaxValue)));
        }

        throw new JsonException($"Unsupported Int64 value '{text}'.");
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public sealed class FlexibleBooleanJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False or JsonTokenType.Null => false,
            JsonTokenType.Number => reader.TryGetInt64(out var value) && value != 0,
            JsonTokenType.String => ParseString(reader.GetString()),
            _ => throw new JsonException($"Unexpected Boolean token {reader.TokenType}.")
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }

    private static bool ParseString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.Trim();
        if (bool.TryParse(text, out var parsed))
        {
            return parsed;
        }

        if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric))
        {
            return numeric != 0;
        }

        return text.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
               text.Equals("y", StringComparison.OrdinalIgnoreCase) ||
               text.Equals("on", StringComparison.OrdinalIgnoreCase);
    }
}
