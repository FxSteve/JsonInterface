using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal class ObjectTypeHandler<T> : IReadJsonTypeHandler<T> where T : class, IJsonObject
    {
        public T FromToken(JToken token) =>
            JsonInterfaceFactory.Create<T>(token.ToTokenTypeOrEmptyObject<JObject>());

        public T GetPropertyValue(JObject jObject, string propertyName) =>
            FromToken(jObject.ForceGetObjectPropertyToken(propertyName));

        public JToken ToToken(T value) => value?.JsonObject ?? new JObject();
    }
}