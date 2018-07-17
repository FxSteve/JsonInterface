using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal interface IReadJsonTypeHandler<T>
    {
        T GetPropertyValue(JsonBase jsonBase, string propertyName, JsonInterfaceSettings settings);

        T FromToken(JToken token, JsonInterfaceSettings settings);

        JToken ToToken(T value, JsonInterfaceSettings settings);

        void ThrowIfFaulted();
    }
}
