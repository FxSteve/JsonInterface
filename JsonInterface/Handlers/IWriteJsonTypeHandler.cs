using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal interface IWriteJsonTypeHandler<T>
    {
        void SetPropertyValue(JsonBase jsonBase, string propertyName, T value, JsonInterfaceSettings settings);

    }
}
