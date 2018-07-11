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

        internal static JArray ForceGetArrayPropertyToken(this JObject jObject, string propertyName)
        {
            propertyName = propertyName.ToCamelCase();

            if (jObject[propertyName].IsNullOrEmpty())
            {
                jObject[propertyName] = new JArray();
            }

            var propertyValueToken = jObject[propertyName];

            if (!(propertyValueToken.Type == JTokenType.Array))
            {
                throw new ArgumentException($"{propertyName} must be an array");
            }

            return (JArray)propertyValueToken;
        }


        internal static JObject ForceGetObjectPropertyToken(this JObject jObject, string propertyName)
        {
            propertyName = propertyName.ToCamelCase();

            if (jObject[propertyName].IsNullOrEmpty())
            {
                jObject[propertyName] = new JObject();
            }

            var propertyValueToken = jObject[propertyName];

            if (!(propertyValueToken.Type == JTokenType.Object))
            {
                throw new ArgumentException($"{propertyName} must be an object.");
            }

            return (JObject)propertyValueToken;
        }

        internal static JValue ForceGetValuePropertyToken(this JObject jObject, string propertyName)
        {
            propertyName = propertyName.ToCamelCase();

            if (jObject[propertyName].IsNullOrEmpty())
            {
                jObject[propertyName] = (JValue)null;
            }

            var propertyValueToken = jObject[propertyName];

            if (propertyValueToken.Type == JTokenType.Array || propertyValueToken.Type == JTokenType.Object)
            {
                throw new ArgumentException($"{propertyName} must be an primitive property.");
            }

            return (JValue)propertyValueToken;
        }
    }
}
