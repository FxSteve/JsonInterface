using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface
{
    public interface IJsonList
    {
        JArray JsonArray { get; set; }
    }

    public interface IJsonList<T> : IJsonList, IList<T>
    {
        T FromJToken(JToken token);
        JToken ToJToken(T value);

        T AddNew();
        T AddNew(Action<T> initializer);
    }
}
