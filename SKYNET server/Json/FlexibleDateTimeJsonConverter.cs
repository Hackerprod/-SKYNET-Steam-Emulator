using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SKYNET_server.Json;

public sealed class FlexibleDateTimeJsonConverter : JsonConverter<DateTime>
{
    private static readonly string[] LegacyFormats =
    {
        "M/d/yyyy h:mm:ss tt",
        "M/d/yyyy h:mm tt",
        "M/d/yyyy H:mm:ss",
        "M/d/yyyy H:mm",
        "MM/dd/yyyy HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
        "O"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return DateTime.MinValue;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var unix))
            {
                return FromUnix(unix);
            }

            throw new JsonException("DateTime number must be a Unix timestamp.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected DateTime token {reader.TokenType}.");
        }

        var text = reader.GetString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return DateTime.MinValue;
        }

        text = text.Trim();
        if (TryParseMicrosoftJsonDate(text, out var microsoftDate))
        {
            return microsoftDate;
        }

        if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unixText))
        {
            return FromUnix(unixText);
        }

        if (DateTimeOffset.TryParse(
                text,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var offset))
        {
            return offset.UtcDateTime;
        }

        if (DateTime.TryParseExact(
                text,
                LegacyFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var exact))
        {
            return DateTime.SpecifyKind(exact, DateTimeKind.Utc);
        }

        throw new JsonException($"Unsupported DateTime value '{text}'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
    }

    private static DateTime FromUnix(long value)
    {
        var milliseconds = Math.Abs(value) > 9_999_999_999L;
        var offset = milliseconds
            ? DateTimeOffset.FromUnixTimeMilliseconds(value)
            : DateTimeOffset.FromUnixTimeSeconds(value);
        return offset.UtcDateTime;
    }

    private static bool TryParseMicrosoftJsonDate(string text, out DateTime value)
    {
        value = default;
        if (!text.StartsWith("/Date(", StringComparison.Ordinal) ||
            !text.EndsWith(")/", StringComparison.Ordinal))
        {
            return false;
        }

        var body = text.Substring(6, text.Length - 8);
        var signIndex = body.IndexOfAny(new[] { '+', '-' }, 1);
        if (signIndex > 0)
        {
            body = body.Substring(0, signIndex);
        }

        if (!long.TryParse(body, NumberStyles.Integer, CultureInfo.InvariantCulture, out var milliseconds))
        {
            return false;
        }

        value = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
        return true;
    }
}
