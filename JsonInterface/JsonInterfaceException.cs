using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface
{
    /// <summary>
    /// Exception from JsonInterface
    /// </summary>
    public class JsonInterfaceException : Exception
    {
        /// <summary>
        /// Construct a new JsonInterfaceException
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="tokenPath">The path in the json object to the affected token</param>
        /// <param name="innerException">The actual exception</param>
        public JsonInterfaceException(string message, string tokenPath, Exception innerException) :
            base(ExpandMessage(message, tokenPath), innerException)
        {
            TokenPath = tokenPath;
        }

        private static string ExpandMessage(string message, string tokenPath)
        {
            if (tokenPath == null) return message;

            if (message.Contains("{0}"))
            {
                return string.Format(message, tokenPath);
            }
            else
            {
                return message + " -> at tokenPath " + tokenPath;
            }
        }

        /// <summary>
        /// Path to token throwing exception
        /// </summary>
        public string TokenPath { get; set; }
    }
}
