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
        public T FromToken(JToken token, JsonInterfaceSettings settings) =>
           new JArrayListWrapper<V>(token.ToTokenTypeOrEmptyObject<JArray>(), settings) as T;

        public JToken ToToken(T value, JsonInterfaceSettings settings) => (value as IJsonList)?.JsonArray ?? new JArray();

        public T GetPropertyValue(JsonBase jsonBase, string propertyName, JsonInterfaceSettings settings) =>
            FromToken(jsonBase.ForceGetArrayPropertyToken(propertyName), settings);

        public void ThrowIfFaulted() { }
    }
}
