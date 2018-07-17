using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Extensions
{
    internal static class JsonExtensions
    {
        internal static bool IsNullOrEmpty(this JToken token) =>
            token == null ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && string.IsNullOrEmpty(token.ToString())) ||
                   (token.Type == JTokenType.Null);

        internal static T ToTokenTypeOrEmptyObject<T>(this JToken token)
            where T : JToken, new() =>
            token.Type == JTokenType.Null ? new T() : (T)token;

        internal static JArray ForceGetArrayPropertyToken(this JsonBase jsonBase, string propertyName)
        {
           var jsonPropertyName = jsonBase.GetJsonPropertyNameFromPropertyName(propertyName);

            var jObject = jsonBase.JsonObject;

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

        internal static JObject ForceGetObjectPropertyToken(this JsonBase jsonBase, string propertyName)
        {
          var  jsonPropertyName = jsonBase.GetJsonPropertyNameFromPropertyName(propertyName);

            var jObject = jsonBase.JsonObject;

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

        internal static JValue ForceGetValuePropertyToken(this JsonBase jsonBase, string propertyName)
        {
            var jsonPropertyName = jsonBase.GetJsonPropertyNameFromPropertyName(propertyName);

            var jObject = jsonBase.JsonObject;

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
    }
}
