using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace System
{
    public class ApplicationStorage
    {
        public string Get(string name)
        {
            try
            {
                return System.IO.File.ReadAllText("data/" + name);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public DateTime? GetVarDate(string name)
        {
            try
            {
                return System.IO.File.GetLastWriteTime("data/" + name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Set(string name, string data)
        {
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }
            System.IO.File.WriteAllText("data/" + name, data);
        }

        public void Append(string name, string data)
        {
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }
            string path = "data/" + name;
            System.IO.File.AppendAllLines(path, new string[] { data.Trim() });
        }
    }

    public class EnvironmentSettings
    {
        private Dictionary<string, string> storedValues;
        private string file;
        public string EnvironmentFile
        {
            get => file; set => file = value;
        }

        public EnvironmentSettings()
        {
            this.file = ".env";
        }
        public EnvironmentSettings(string filename)
        {
            this.file = filename;
        }

        public void Flush()
        {
            storedValues = this.FlushData();
        }

        public string? Get(string propertyName, string? defaultValue = null)
        {
            if(storedValues.ContainsKey(propertyName))
            {
                return storedValues[propertyName];
            } else
            {
                return defaultValue;
            }
        }

        public void Set(string propertyName, string propertyValue)
        {
            storedValues = this.FlushData(new Dictionary<string, string>() { { propertyName, propertyValue } });
        }

        private Dictionary<string, string> FlushData(Dictionary<string, string> updatingData = null)
        {
            //read properties
            Dictionary<string, string> values = new Dictionary<string, string>();

            if (!System.IO.File.Exists(file)) System.IO.File.Create(file);
            foreach (string line in System.IO.File.ReadLines(file))
            {
                if(line.StartsWith("#"))
                {
                    continue; // comment
                }
                try
                {
                    string propertyName = line.Substring(0, line.IndexOf("=")).Trim();
                    string propertyValue = line.Substring(line.IndexOf("=") + 1).Trim();

                    if((updatingData ?? new Dictionary<string, string>() { }).ContainsKey(propertyName))
                    {
                        propertyValue = updatingData[propertyName];
                    }

                    values.Add(propertyName, propertyValue);

                } catch (Exception)
                {
                    continue;
                }
            }

            foreach(KeyValuePair<string, string> updateData in updatingData ?? new Dictionary<string, string>() { })
            {
                if (values.ContainsKey(updateData.Key)) continue;
                values.Add(updateData.Key, updateData.Value);
                System.IO.File.AppendAllLines(file, new string[] { updateData.Key.Trim() + "=" + updateData.Value.Trim() });
            }

            return values;
        }
    }
}
