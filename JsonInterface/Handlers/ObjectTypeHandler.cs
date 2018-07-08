using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;
using static JsonInterface.JsonInterfaceFactory;

namespace JsonInterface.Handlers
{
    public class ObjectTypeHandler<T> : IReadJsonTypeHandler<T> where T : class, IJsonObject
    {
        public T FromToken(JToken token) =>
            FacadeFor<T>.Create((JObject)token);

        public T GetPropertyValue(JObject jObject, string propertyName) =>
            FromToken(jObject.ForceGetObjectPropertyToken(propertyName));

        public JToken ToToken(T value) => value?.JsonObject ?? new JObject();
    }
}