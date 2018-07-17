using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal interface IReadJsonTypeHandler<T>
    {
        T GetPropertyValue(JsonBase jsonBase, string propertyName);

        T FromToken(JToken token, JsonBase jsonBase);

        JToken ToToken(T value, JsonBase jsonBase);

        void ThrowIfFaulted();
    }
}
