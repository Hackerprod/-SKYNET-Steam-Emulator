using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace SKYNET.INI
{
    public class INISerializer
    {
        public static string Serialize(object obj)
        {
            var Properties = new List<Property>();

            Type type = obj.GetType();
            foreach (var PropertyInfo in type.GetProperties())
            {
                if (PropertyInfo.IsDefined(typeof(SectionAttribute)))
                {
                    var Property = new Property();
                    var Comment = "";

                    var SectionAttribute = PropertyInfo.GetCustomAttribute<SectionAttribute>();

                    if (PropertyInfo.IsDefined(typeof(CommentAttribute)))
                    {
                        var CommentAttribute = PropertyInfo.GetCustomAttribute<CommentAttribute>();
                        Comment = CommentAttribute.Comment;
                    }

                    Property.Section = SectionAttribute?.Name;
                    Property.Name = PropertyInfo.Name;
                    Property.Value = PropertyInfo.GetValue(obj, null);
                    Property.Comment = Comment;

                    Properties.Add(Property);
                }
            }

            StringBuilder builder = new StringBuilder();
            string currentSection = "";

            foreach (var Property in Properties)
            {
                if (Property.Section != currentSection)
                {
                    if (!string.IsNullOrEmpty(currentSection)) builder.AppendLine();
                    builder.AppendLine($"[{Property.Section}]");
                    currentSection = Property.Section;
                }

                if (!string.IsNullOrEmpty(Property.Comment))
                {
                    builder.AppendLine($"# {Property.Comment}");
                }
                builder.AppendLine($"{Property.Name} = {Property.Value}");
            }

            return builder.ToString(); ;
        }

        public static void SerializeToFile(object obj, string filePath)
        {
            try
            {
                string content = Serialize(obj);
                File.WriteAllText(filePath, content);
            }
            catch
            {
            }
        }

        public static T Deserialize<T>(string content)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));

            Dictionary<string, string> StringProperties = GetStringProperties(content);

            Type type = obj.GetType();
            foreach (var PropertyInfo in type.GetProperties())
            {
                if (PropertyInfo.IsDefined(typeof(SectionAttribute)))
                {
                    if (StringProperties.ContainsKey(PropertyInfo.Name))
                    {
                        var value = StringProperties[PropertyInfo.Name];

                        Type PropertyType = PropertyInfo.PropertyType;

                        if (PropertyType == typeof(String))
                        {
                            PropertyInfo.SetValue(obj, value);
                        }
                        else if (PropertyType == typeof(DateTime))
                        {
                            if (DateTime.TryParse(value, out var Time))
                            {
                                PropertyInfo.SetValue(obj, Time);
                            }
                        }
                        else if (PropertyType == typeof(IPAddress))
                        {
                            if (IPAddress.TryParse(value, out var Address))
                            {
                                PropertyInfo.SetValue(obj, Address);
                            }
                        }
                        else if (PropertyType.IsEnum)
                        {
                            PropertyInfo.SetValue(obj, Enum.Parse(PropertyType, value, false));
                        }
                        else if (PropertyType == typeof(object))
                        {
                            PropertyInfo.SetValue(obj, value);
                        }
                        else if (PropertyType == typeof(double))
                        {
                            value = value.Replace(",", ".");
                            if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result))
                            {
                                PropertyInfo.SetValue(obj, result);
                            }
                        }
                        else if (PropertyType == typeof(float))
                        {
                            if (float.TryParse(value, out var result))
                            {
                                PropertyInfo.SetValue(obj, result);
                            }
                        }
                        else if (PropertyType == typeof(decimal))
                        {
                            if (decimal.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                            {
                                PropertyInfo.SetValue(obj, result);
                            }
                        }
                        else if (PropertyType.IsPrimitive)
                        {
                            var result = Convert.ChangeType(value, PropertyType, System.Globalization.CultureInfo.InvariantCulture);
                            PropertyInfo.SetValue(obj, result);
                        }
                        else
                        {
                            Console.WriteLine($"Not found type for {PropertyInfo.Name} {PropertyType}");
                        }
                    }
                }
            }
            return obj;
        }

        public static T DeserializeFromFile<T>(string fileName)
        {
            try
            {
                string content = File.ReadAllText(fileName);
                T deserialized = (T)Deserialize<T>(content);
                return deserialized;
            }
            catch
            {
                return default;
            }
        }

        private static Dictionary<string, string> GetStringProperties(string content)
        {
            Dictionary<string, string> StringProperties = new Dictionary<string, string>();

            using (var reader = new StringReader(content))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (line.Contains("="))
                    {
                        line = line.Replace(" =", "=").Replace("= ", "=");
                        string[] settingParts = line.Split('=');
                        if (settingParts.Length > 1)
                        {
                            var Key = settingParts[0];
                            var Value = "";
                            for (int s = 0; s < settingParts.Length; s++)
                            {
                                if (s != 0)
                                {
                                    Value += (s == settingParts.Length - 1) ? settingParts[s] : settingParts[s] + " ";
                                }
                            }
                            StringProperties.Add(Key, Value);
                        }
                    }
                }
            }

            return StringProperties;
        }

        private class Property
        {
            public string Section { get; set; }
            public string Name { get; set; }
            public object Value { get; set; }
            public string Comment { get; set; }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SectionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public SectionAttribute(string Name) { this.Name = Name; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; set; }
        public CommentAttribute(string Comment) { this.Comment = Comment; }
    }
}
