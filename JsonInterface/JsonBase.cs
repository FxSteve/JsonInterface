using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
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
        /// This is used to avoid null references
        /// </summary>
        static IReadOnlyDictionary<string, string> _emptyDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Interface settings, including serializer settings
        /// </summary>
        public JsonInterfaceSettings JsonInterfaceSettings { get; set; }

        /// <summary>
        /// The Newtonsoft json object.  The proxy is a wrapper around this object
        /// </summary>
        public JObject JsonObject { get; set; }

        internal IReadOnlyDictionary<string, string> ObjectPropertyNameToJsonPropertyName { get; set; }

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
            (ObjectPropertyNameToJsonPropertyName ?? _emptyDictionary).TryGetValue(propertyName, out var value) ? value : propertyName;


        internal JArray ForceGetArrayPropertyToken(string propertyName)
        {
            var jsonPropertyName = GetJsonPropertyNameFromPropertyName(propertyName);

            var jObject = JsonObject;

            if (jObject[jsonPropertyName].IsNullOrEmpty())
            {
                jObject[jsonPropertyName] = new JArray();
            }

            var propertyValueToken = jObject[jsonPropertyName];

            if (!(propertyValueToken.Type == JTokenType.Array))
            {
                throw new ArgumentException($"{jsonPropertyName} must be an array");
            }

            return (JArray)propertyValueToken;
        }

        internal JObject ForceGetObjectPropertyToken(string propertyName)
        {
            var jsonPropertyName = GetJsonPropertyNameFromPropertyName(propertyName);

            var jObject = JsonObject;

            if (jObject[jsonPropertyName].IsNullOrEmpty())
            {
                jObject[jsonPropertyName] = new JObject();
            }

            var propertyValueToken = jObject[jsonPropertyName];

            if (!(propertyValueToken.Type == JTokenType.Object))
            {
                throw new ArgumentException($"{jsonPropertyName} must be an object.");
            }

            return (JObject)propertyValueToken;
        }

        internal JValue ForceGetValuePropertyToken(string propertyName)
        {
            var jsonPropertyName = GetJsonPropertyNameFromPropertyName(propertyName);

            var jObject = JsonObject;

            if (jObject[jsonPropertyName].IsNullOrEmpty())
            {
                jObject[jsonPropertyName] = JValue.CreateNull();
            }

            var propertyValueToken = jObject[jsonPropertyName];

            if (propertyValueToken.Type == JTokenType.Array || propertyValueToken.Type == JTokenType.Object)
            {
                throw new ArgumentException($"{jsonPropertyName} must be an primitive property.");
            }

            return (JValue)propertyValueToken;
        }

        internal JsonInterfaceFactory Factory { get; set; }
    }
}
