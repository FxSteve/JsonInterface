using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface
{
    public interface IJsonObject
    {
        JObject JsonObject { get; set; }
    }
}
