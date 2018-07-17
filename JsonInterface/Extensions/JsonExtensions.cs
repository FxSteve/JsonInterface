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
    }
}
