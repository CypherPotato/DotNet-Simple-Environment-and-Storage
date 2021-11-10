using System;
using System.IO;
using System.Linq;

#nullable enable

namespace ProjectPrincipium
{
    /// <summary>
    /// Provides a solid file storage of variables and data.
    /// </summary>
    public class ApplicationStorage
    {
        private string _prefix = "";

        internal static bool isPathNameValid(string value)
        {
            char[] invalidChars = System.IO.Path.GetInvalidPathChars();
            foreach (char c in value)
            {
                if (invalidChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool isFileNameValid(string value)
        {
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in value)
            {
                if (invalidChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        private static void initStorage(ApplicationStorage applicationStorage)
        {
            if (!Directory.Exists(applicationStorage.Prefix))
            {
                Directory.CreateDirectory(applicationStorage.Prefix);
            }
        }

        /// <summary>
        /// Gets or sets the directory where the data will be stored.
        /// </summary>
        public string Prefix
        {
            get => _prefix;
            set
            {
                if (!isPathNameValid(value)) throw new InvalidDataException("Storage prefixes cannot contain invalid path chars.");
                _prefix = value;
            }
        }

        /// <summary>
        /// Creates an new <see cref="System.ApplicationStorage"/> instance with the default storage prefix.
        /// </summary>
        public ApplicationStorage()
        {
            this.Prefix = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        }

        /// <summary>
        /// Creates an new <see cref="System.ApplicationStorage"/> instance with the provided storage prefix.
        /// </summary>
        /// <param name="storagePrefix">The directory where the data will be stored.</param>
        public ApplicationStorage(string storagePrefix)
        {
            this.Prefix = storagePrefix;
        }

        /// <summary>
        /// Creates an new <see cref="System.ApplicationStorage"/> instance with an new storage prefix inside this instance's prefix.
        /// </summary>
        /// <param name="subStoragePrefix">The subdirectory prefix for the new instance.</param>
        /// <returns></returns>
        public ApplicationStorage GetSubStorage(string subStoragePrefix)
        {
            return new ApplicationStorage(Path.Combine(Prefix, subStoragePrefix));
        }

        /// <summary>
        /// Gets an variable value inside this storage.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="defaultValue">Optional. If the variable is not found, this value is returned.</param>
        /// <returns></returns>
        public string? Get(string name, string? defaultValue = null)
        {
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            try
            {
                return System.IO.File.ReadAllText(System.IO.Path.Combine(Prefix, name));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public string[] GetVariables()
        {
            return System.IO.Directory.GetFiles(Prefix);
        }

        /// <summary>
        /// Gets the modification date of a variable. If it does not exist, nothing is returned.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <returns></returns>
        public DateTime? GetVarDate(string name)
        {
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            try
            {
                return System.IO.File.GetLastWriteTime(System.IO.Path.Combine(Prefix, name));
            } 
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Sets an variable and stores the value on it.
        /// </summary>
        /// <param name="name">The variable name. It must be an valid name and cannot have invalid path chars.</param>
        /// <param name="data">The string data to store in the variable. If nothing is provided, the variable is deleted.</param>
        public void Set(string name, string? data)
        {
            initStorage(this);
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            string path = System.IO.Path.Combine(Prefix, name);
            if (data == null)
            {
                System.IO.File.Delete(path);
                return;
            }
            System.IO.File.WriteAllText(path, data);
        }

        /// <summary>
        /// Appends values to an variable. If it doens't exist, it will be created.
        /// </summary>
        /// <param name="name">The variable name. It must be an valid name and cannot have invalid path chars.</param>
        /// <param name="data">The string data to store in the variable.</param>
        public void Append(string name, string data)
        {
            initStorage(this);
            if (!isFileNameValid(name)) throw new InvalidDataException("Variables names cannot contain invalid path chars.");
            string path = System.IO.Path.Combine(Prefix, name);
            System.IO.File.AppendAllLines(path, new string[] { data.Trim() });
        }
    }
}
