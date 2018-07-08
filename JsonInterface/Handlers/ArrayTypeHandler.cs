using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    public class ArrayTypeHandler<T, V> : IReadJsonTypeHandler<T>
        where T : class
    {
        readonly IReadJsonTypeHandler<V> _readJsonTypeHandler;
        readonly IWriteJsonTypeHandler<V> _writeJsonTypeHandler;

        public ArrayTypeHandler(IReadJsonTypeHandler<V> readJsonTypeHandler, IWriteJsonTypeHandler<V> writeJsonTypeHandler)
        {
            _readJsonTypeHandler = readJsonTypeHandler ?? throw new ArgumentNullException(nameof(readJsonTypeHandler));
            _writeJsonTypeHandler = writeJsonTypeHandler;
        }

        public T FromToken(JToken token) =>
           new JArrayListWrapper<V>((JArray)token, _readJsonTypeHandler, _writeJsonTypeHandler) as T;

        public JToken ToToken(T value) => (value as IJsonList)?.JsonArray ?? new JArray();

        public T GetPropertyValue(JObject jObject, string propertyName) =>
            FromToken(jObject.ForceGetArrayPropertyToken(propertyName));
    }
}
