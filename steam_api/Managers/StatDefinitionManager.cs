using System;
using System.Collections.Generic;
using System.Linq;

namespace SKYNET.Managers
{
    /// <summary>
    /// Stores the app's immutable stat schema delivered with the authenticated
    /// session. Per-user values continue to live in StateCache and steam.db.
    /// </summary>
    public static class StatDefinitionManager
    {
        private static readonly object Gate = new object();
        private static readonly Dictionary<string, APIClient.StatDefinitionDto> Definitions =
            new Dictionary<string, APIClient.StatDefinitionDto>(StringComparer.Ordinal);

        public static void Apply(
            uint appId,
            IEnumerable<APIClient.StatDefinitionDto> definitions)
        {
            lock (Gate)
            {
                Definitions.Clear();
                foreach (var definition in definitions ?? Enumerable.Empty<APIClient.StatDefinitionDto>())
                {
                    var name = definition?.Name?.Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        Definitions[name] = definition;
                    }
                }
            }
        }

        public static bool TryGetIntDefault(string name, out int value)
        {
            lock (Gate)
            {
                var normalizedName = name?.Trim() ?? string.Empty;
                if (Definitions.TryGetValue(normalizedName, out var definition) &&
                    string.Equals(definition.Type, "int", StringComparison.OrdinalIgnoreCase))
                {
                    value = definition.DefaultInt;
                    return true;
                }
            }

            value = 0;
            return false;
        }

        public static int Count
        {
            get
            {
                lock (Gate)
                {
                    return Definitions.Count;
                }
            }
        }

        public static bool TryGetFloatDefault(string name, out float value)
        {
            lock (Gate)
            {
                var normalizedName = name?.Trim() ?? string.Empty;
                if (Definitions.TryGetValue(normalizedName, out var definition) &&
                    (string.Equals(definition.Type, "float", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(definition.Type, "avgrate", StringComparison.OrdinalIgnoreCase)))
                {
                    value = definition.DefaultFloat;
                    return true;
                }
            }

            value = 0;
            return false;
        }
    }
}
