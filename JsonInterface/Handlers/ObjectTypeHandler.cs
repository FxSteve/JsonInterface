using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal class ObjectTypeHandler<T> : IReadJsonTypeHandler<T> where T : class, IJsonObject
    {
        public T FromToken(JToken token, JsonBase jsonBase) =>
            jsonBase.Factory.Create<T>(token.ToTokenTypeOrEmptyObject<JObject>());

        public T GetPropertyValue(JsonBase jsonBase, string propertyName) =>
            FromToken(jsonBase.ForceGetObjectPropertyToken(propertyName), jsonBase);

        public void ThrowIfFaulted() { }

        public JToken ToToken(T value, JsonBase jsonBase) => value?.JsonObject ?? new JObject();
    }
}