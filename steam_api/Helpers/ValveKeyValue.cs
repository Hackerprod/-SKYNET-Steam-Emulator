using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SKYNET.Helpers
{
    internal sealed class ValveKeyValue
    {
        private readonly List<ValveKeyValue> children = new List<ValveKeyValue>();

        private ValveKeyValue(string name, string value)
        {
            Name = name ?? string.Empty;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
        public IReadOnlyList<ValveKeyValue> Children => children;
        public bool IsObject => Value == null;

        public ValveKeyValue Child(string name)
        {
            return children.FirstOrDefault(child =>
                string.Equals(child.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<ValveKeyValue> ChildrenNamed(string name)
        {
            return children.Where(child =>
                string.Equals(child.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public string GetValue(string name, string fallback = "")
        {
            var child = Child(name);
            return child?.Value ?? fallback;
        }

        public IEnumerable<ValveKeyValue> Descendants()
        {
            foreach (var child in children)
            {
                yield return child;
                foreach (var descendant in child.Descendants())
                {
                    yield return descendant;
                }
            }
        }

        public static ValveKeyValue ParseFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A KeyValues path is required.", nameof(path));
            }

            return Parse(File.ReadAllText(path));
        }

        public static ValveKeyValue Parse(string text)
        {
            var tokens = Tokenize(text ?? string.Empty);
            var root = new ValveKeyValue(string.Empty, null);
            ParseChildren(root, tokens, false);
            return root;
        }

        private static void ParseChildren(ValveKeyValue parent, Queue<string> tokens, bool stopAtBrace)
        {
            while (tokens.Count > 0)
            {
                var name = tokens.Dequeue();
                if (name == "}")
                {
                    if (!stopAtBrace)
                    {
                        throw new FormatException("Unexpected closing brace in Valve KeyValues data.");
                    }

                    return;
                }

                if (name == "{")
                {
                    throw new FormatException("Unexpected opening brace in Valve KeyValues data.");
                }

                if (tokens.Count == 0)
                {
                    throw new FormatException($"Missing value for Valve KeyValues key '{name}'.");
                }

                var value = tokens.Dequeue();
                if (value == "{")
                {
                    var child = new ValveKeyValue(name, null);
                    parent.children.Add(child);
                    ParseChildren(child, tokens, true);
                    continue;
                }

                if (value == "}")
                {
                    throw new FormatException($"Missing value for Valve KeyValues key '{name}'.");
                }

                parent.children.Add(new ValveKeyValue(name, value));
            }

            if (stopAtBrace)
            {
                throw new FormatException("Unterminated object in Valve KeyValues data.");
            }
        }

        private static Queue<string> Tokenize(string text)
        {
            var tokens = new Queue<string>();
            var index = 0;
            while (index < text.Length)
            {
                SkipTrivia(text, ref index);
                if (index >= text.Length)
                {
                    break;
                }

                var current = text[index];
                if (current == '{' || current == '}')
                {
                    tokens.Enqueue(current.ToString());
                    index++;
                    continue;
                }

                if (current == '"')
                {
                    tokens.Enqueue(ReadQuoted(text, ref index));
                    continue;
                }

                var start = index;
                while (index < text.Length &&
                       !char.IsWhiteSpace(text[index]) &&
                       text[index] != '{' &&
                       text[index] != '}')
                {
                    index++;
                }

                if (index > start)
                {
                    tokens.Enqueue(text.Substring(start, index - start));
                }
            }

            return tokens;
        }

        private static void SkipTrivia(string text, ref int index)
        {
            while (index < text.Length)
            {
                if (char.IsWhiteSpace(text[index]))
                {
                    index++;
                    continue;
                }

                if (index + 1 < text.Length && text[index] == '/' && text[index + 1] == '/')
                {
                    index += 2;
                    while (index < text.Length && text[index] != '\r' && text[index] != '\n')
                    {
                        index++;
                    }
                    continue;
                }

                break;
            }
        }

        private static string ReadQuoted(string text, ref int index)
        {
            index++;
            var value = new StringBuilder();
            while (index < text.Length)
            {
                var current = text[index++];
                if (current == '"')
                {
                    return value.ToString();
                }

                if (current == '\\' && index < text.Length)
                {
                    var escaped = text[index++];
                    switch (escaped)
                    {
                        case 'n':
                            value.Append('\n');
                            break;
                        case 'r':
                            value.Append('\r');
                            break;
                        case 't':
                            value.Append('\t');
                            break;
                        case '"':
                        case '\\':
                            value.Append(escaped);
                            break;
                        default:
                            value.Append('\\').Append(escaped);
                            break;
                    }
                    continue;
                }

                value.Append(current);
            }

            throw new FormatException("Unterminated quoted string in Valve KeyValues data.");
        }
    }
}
