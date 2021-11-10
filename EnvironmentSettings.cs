using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

#nullable enable

namespace ProjectPrincipium
{
    /// <summary>
    /// Provides an initial configuration file handling class for the application.
    /// </summary>
    public static class EnvironmentSettings
    {
        private static Dictionary<string, string> storedValues = new Dictionary<string, string>();
        private static string file = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Environment.ini";
        private static bool flushed = false;

        /// <summary>
        /// Get or sets the name of the initial settings file.
        /// </summary>
        public static string EnvironmentFile
        {
            get => file;
            set
            {
                if (!ApplicationStorage.isPathNameValid(value)) throw new InvalidDataException("Environment file cannot contain invalid path chars.");
                file = value;
            }
        }

        /// <summary>
        /// Gets an object with the environment variables as properties.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static dynamic GetDynamicObject()
        {
            if (!flushed)
            {
                throw new Exception("INI Container not flushed or initialized.");
            }

            var newObject = new ExpandoObject() as IDictionary<string, object>;

            foreach (var keyValuePair in storedValues)
            {
                newObject.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return newObject as dynamic;
        }

        /// <summary>
        /// Reads environment variables from file and stores them in the application memory.
        /// </summary>
        public static void Flush()
        {
            storedValues = FlushData();
        }

        /// <summary>
        /// Reads environment variables from file and stores them in the application memory and creates the Environment file if it doens't exists.
        /// </summary>
        /// <param name="createEnvironmentFileIfDontExists">Creates the Environment file if it doens't exists.</param>
        public static void Flush(bool createEnvironmentFileIfDontExists)
        {
            storedValues = FlushData();
            if(createEnvironmentFileIfDontExists)
            {
                if (!System.IO.File.Exists(file))
                {
                    System.IO.File.Create(file).Close();
                }
            }
        }

        /// <summary>
        /// Gets an variable from the environment file.
        /// </summary>
        /// <param name="propertyName">The variable name.</param>
        /// <param name="defaultValue">The default value to return if the variable don't exists.</param>
        /// <returns></returns>
        public static string? Get(string propertyName, string? defaultValue = null)
        {
            if (!flushed)
            {
                throw new Exception("INI Container not flushed or initialized.");
            }
            string? v = storedValues.Where((i) => i.Key.ToLower() == propertyName.ToLower()).FirstOrDefault().Value;
            return v ?? defaultValue;
        }

        private static Dictionary<string, string> FlushData()
        {
            //read properties
            Dictionary<string, string> values = new Dictionary<string, string>();

            flushed = true;

            if (!System.IO.File.Exists(file)) return values;

            foreach (string line in System.IO.File.ReadLines(file))
            {
                if (line.StartsWith("#") || line.StartsWith(";"))
                {
                    continue; // comment
                }
                if (line.StartsWith("["))
                {
                    continue; // group
                }
                try
                {
                    string propertyName = line.Substring(0, line.IndexOf("=")).Trim();
                    string propertyValue = line.Substring(line.IndexOf("=") + 1).Trim();

                    if(propertyValue.StartsWith("\""))
                    {
                        if(!propertyValue.EndsWith("\""))
                        {
                            throw new Exceptions.INIParserException("Quoted variables must end with quotes.");
                        }
                        propertyValue = propertyValue.Substring(1, propertyValue.Length - 2);
                    }

                    if (values.ContainsKey(propertyName))
                    {
                        throw new Exceptions.INIParserException("A key with this name already exists in this INI collection: " + propertyName);
                    }

                    values.Add(propertyName, propertyValue);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.INIParserException("Internal error on the parser: " + ex.Message);
                }
            }

            return values;
        }
    }
}
