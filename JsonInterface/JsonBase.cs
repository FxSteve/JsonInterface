using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonInterface
{
    /// <summary>
    /// Base class for JsonInterface proxies
    /// </summary>
    public class JsonBase : IJsonObject
    {
        /// <summary>
        /// Interface settings, including serializer settings
        /// </summary>
        public JsonInterfaceSettings JsonInterfaceSettings { get; set; }

        /// <summary>
        /// The Newtonsoft json object.  The proxy is a wrapper around this object
        /// </summary>
        public JObject JsonObject { get; set; }
        internal Dictionary<string, string> ObjectPropertyNameToJsonPropertyName { get; set; }

        /// <summary>
        /// Compare with other object.  Returns true if both references refer to the same JObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) =>
          JsonObject.Equals((obj as JsonBase)?.JsonObject);

        /// <summary>
        /// Return the hashcode from the JObject
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => JsonObject.GetHashCode();

        /// <summary>
        /// Return the json string for this object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => JsonObject?.ToString();

        internal string GetJsonPropertyNameFromPropertyName(string propertyName) =>
            ObjectPropertyNameToJsonPropertyName.TryGetValue(propertyName, out var value) ? value : propertyName;
    }
}
