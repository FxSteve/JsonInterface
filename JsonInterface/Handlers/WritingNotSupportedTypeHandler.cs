using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal class WritingNotSupportedTypeHandler<T> : IWriteJsonTypeHandler<T>
    {
        static readonly string NotSupportedErrorMessage = $"Writes are not supported for objects of type {typeof(T).Name}";

        public void SetPropertyValue(JsonBase jsonBase, string propertyName, T value,JsonInterfaceSettings settings)
        {
            throw new NotImplementedException(NotSupportedErrorMessage);
        }

        public JToken ToToken(T value, JsonInterfaceSettings settings)
        {
            throw new NotImplementedException(NotSupportedErrorMessage);
        }
    }
}
