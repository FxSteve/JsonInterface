using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    public interface IReadJsonTypeHandler<T>
    {
        T GetPropertyValue(JObject jObject, string propertyName);

        T FromToken(JToken token);

        JToken ToToken(T value);

    }
}
