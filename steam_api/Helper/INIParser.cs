using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{
    public class INIParser
    {
        public ConcurrentDictionary<string, Section> Sections;

        public event EventHandler<string> OnMessageError;

        private string _filePath;
        private Section currentSection;
        private Setting currentSetting;
        private List<string> CommentsAwaitingSetting;
        private int CurrentNumber;
        public INIParser()
        {
            Sections = new ConcurrentDictionary<string, Section>();
            CommentsAwaitingSetting = new List<string>();
            CurrentNumber = 1;
        }

        public Section this[string sectionName]
        {
            get
            {
                if (!Sections.ContainsKey(sectionName))
                {
                    Sections.TryAdd(sectionName, new Section() { Name = sectionName, Number = CurrentNumber });
                    CurrentNumber++;
                }
                return Sections[sectionName];
            }
        }
        public ConcurrentDictionary<string, string> this[Section section]
        {
            get
            {
                ConcurrentDictionary<string, string> response = new ConcurrentDictionary<string, string>();
                if (section != null)
                {
                    foreach (var setting in section.Settings)
                    {
                        response.TryAdd(setting.Key, setting.Value.ToString());
                    }
                }
                return response;
            }
            set
            {
                if (section != null)
                {
                    for (int i = 0; i < section.Settings.Count; i++)
                    {
                        Setting s = section.Settings[i];
                        if (!value.ContainsKey(s.Key))
                        {
                            section.Settings.Remove(s);
                        }
                    }

                    foreach (var KV in value)
                    {
                        Setting setting = section.Settings.Find(s => s.Key == KV.Key);
                        if (setting == null)
                        {
                            setting = new Setting()
                            {
                                Key = KV.Key,
                                Value = KV.Value
                            };
                            section.Settings.Add(setting);
                        }
                        else
                        {
                            setting.Key = KV.Key;
                            setting.Value = KV.Value;
                        }
                    }
                }
            }
        }

        public void Load(string filePath)
        {
            _filePath = filePath;

            if (!File.Exists(filePath))
            {
                InvokeError($"File {filePath} not found");
                return;
            }
            List<string> lines = File.ReadAllLines(_filePath).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.StartsWith("["))
                {
                    if (currentSection != null)
                    {
                        if (CommentsAwaitingSetting.Any())
                        {
                            currentSection.Comments.AddRange(CommentsAwaitingSetting);
                        }
                        Sections.TryAdd(currentSection.Name, currentSection);
                        currentSection = null;
                    }

                    currentSection = new Section();
                    currentSection.Name = line.Replace("[", "").Replace("]", "");
                    currentSection.Number = CurrentNumber;
                    CurrentNumber++;

                    CommentsAwaitingSetting.Clear();
                    currentSetting = null;

                    VerifyLastLine(i == lines.Count - 1);
                }
                else if (line.StartsWith("#") || line.StartsWith(";"))
                {

                    CommentsAwaitingSetting.Add(line);

                    VerifyLastLine(i == lines.Count - 1);
                }
                else
                {
                    if (line.Contains("="))
                    {
                        line = line.Replace(" =", "=").Replace("= ", "=");
                        currentSetting = new Setting();
                        currentSetting.Comments.AddRange(CommentsAwaitingSetting);
                        CommentsAwaitingSetting.Clear();

                        string[] settingParts = line.Split('=');
                        if (settingParts.Length > 1)
                        {
                            currentSetting.Key = settingParts[0];
                            for (int s = 0; s < settingParts.Length; s++)
                            {
                                if (s != 0)
                                {
                                    currentSetting.Value += (s == settingParts.Length - 1) ? settingParts[s] : settingParts[s] + " ";
                                }
                            }
                            currentSection.Settings.Add(currentSetting);
                            currentSetting = null;
                        }
                    }

                    VerifyLastLine(i == lines.Count - 1);
                }
            }
        }

        private void VerifyLastLine(bool v)
        {
            if (v)
            {
                if (CommentsAwaitingSetting.Any())
                {
                    currentSection.ExtraComments.AddRange(CommentsAwaitingSetting);
                }
                Sections.TryAdd(currentSection.Name, currentSection);
            }
        }

        private void InvokeError(string msg)
        {
            OnMessageError?.Invoke(this, msg);
        }

        public void Save()
        {
            List<Section> _Sections = new List<Section>();
            foreach (var keyValue in Sections) _Sections.Add(keyValue.Value);

            _Sections.Sort((s1, s2) => s1.Number.CompareTo(s2.Number));

            StringBuilder Data = new StringBuilder();
            foreach (Section section in _Sections)
            {
                Data.AppendLine($"[{section.Name}]");

                if (section.Comments.Any())
                {
                    foreach (var Comment in section.Comments)
                    {
                        Data.AppendLine($"{Comment}");
                    }
                }

                if (section.Settings.Any())
                {
                    foreach (var Setting in section.Settings)
                    {
                        if (Setting.Comments.Any())
                        {
                            foreach (var Comment in Setting.Comments)
                            {
                                Data.AppendLine(Comment);
                            }
                        }
                        Data.AppendLine($"{Setting.Key} = {Setting.Value}");
                    }
                    if (section.ExtraComments.Any())
                    {
                        Data.AppendLine();
                    }
                }

                if (section.ExtraComments.Any())
                {
                    foreach (var Comment in section.ExtraComments)
                    {
                        Data.AppendLine($"{Comment}");
                    }
                }

                Data.AppendLine();
            }
            File.WriteAllText(_filePath, Data.ToString());
        }

        public void SetProperties(string sectionName, ConcurrentDictionary<string, string> heyValue)
        {
            List<Setting> Settings = new List<Setting>();

            foreach (var KV in heyValue)
            {
                Settings.Add(new Setting()
                {
                    Key = KV.Key,
                    Value = KV.Value
                });
            }

            Section section = Sections[sectionName];
            if (section == null)
            {
                section = new Section() { Name = sectionName, Number = CurrentNumber };
                CurrentNumber++;
                Sections.TryAdd(sectionName, section);
            }

            for (int i = 0; i < Sections[sectionName].Settings.Count; i++)
            {
                Setting s = Sections[sectionName].Settings[i];
                if (!heyValue.ContainsKey(s.Key))
                {
                    Sections[sectionName].Settings.Remove(s);
                }
            }

            foreach (var setting in Settings)
            {
                Sections[sectionName][setting.Key] = setting.Value;
            }

        }
    }
    public class Section
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public List<Setting> Settings { get; set; }
        public List<string> Comments { get; set; }
        public List<string> ExtraComments { get; set; }
        public Section()
        {
            Settings = new List<Setting>();
            Comments = new List<string>();
            ExtraComments = new List<string>();
        }
        public object this[string keyName]
        {
            get
            {
                Setting setting = Settings.Find(s => s.Key == keyName);
                if (setting != null)
                {
                    return MakeType(setting.Value.ToString());
                }
                return null;
            }
            set
            {
                Setting setting = Settings.Find(s => s.Key == keyName);
                if (setting != null)
                {
                    setting.Value = value;
                }
                else
                {
                    setting = new Setting()
                    {
                        Key = keyName,
                        Value = value
                    };
                    Settings.Add(setting);
                }
            }
        }

        private object MakeType(string value)
        {
            if (bool.TryParse(value, out bool b))
            {
                return b;
            }
            if (int.TryParse(value, out int i))
            {
                return i;
            }
            if (ulong.TryParse(value, out ulong u))
            {
                return u;
            }
            return value.ToString();
        }
    }
    public class Setting
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public List<string> Comments { get; set; }
        public Setting()
        {
            Comments = new List<string>();
        }
    }

}
