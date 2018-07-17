using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal class ObjectTypeHandler<T> : IReadJsonTypeHandler<T> where T : class, IJsonObject
    {
        public T FromToken(JToken token, JsonInterfaceSettings settings) =>
            JsonInterfaceFactory.Create<T>(token.ToTokenTypeOrEmptyObject<JObject>(), settings);

        public T GetPropertyValue(JsonBase jsonBase, string propertyName, JsonInterfaceSettings settings) =>
            FromToken(jsonBase.ForceGetObjectPropertyToken(propertyName), settings);

        public void ThrowIfFaulted() { }

        public JToken ToToken(T value, JsonInterfaceSettings settings) => value?.JsonObject ?? new JObject();
    }
}