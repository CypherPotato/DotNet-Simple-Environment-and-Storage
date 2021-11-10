using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectPrincipium.Exceptions
{
    /// <summary>
    /// Represents errors that occour during Environment INI parser process.
    /// </summary>
    public class INIParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INIParserException"/> class with the given message.
        /// </summary>
        /// <param name="message">The error's message.</param>
        public INIParserException(string message) : base(message)
        {
            
        }
    }
}
