using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal class ArrayTypeHandler<T, V> : IReadJsonTypeHandler<T>
        where T : class
    {
        public T FromToken(JToken token, JsonBase jsonBase) =>
           new JsonArrayListWrapper<V>(token.ToTokenTypeOrEmptyObject<JArray>(), jsonBase) as T;

        public JToken ToToken(T value, JsonBase jsonBase) => (value as IJsonList)?.JsonArray ?? new JArray();

        public T GetPropertyValue(JsonBase jsonBase, string propertyName) =>
            FromToken(jsonBase.ForceGetArrayPropertyToken(propertyName), jsonBase);

        public void ThrowIfFaulted() { }
    }
}
