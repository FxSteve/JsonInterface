using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface
{
    public class JsonBase : IJsonObject
    {
        public JObject JsonObject { get; set; }

        public override bool Equals(object obj) => JsonObject.Equals(obj);

        public override int GetHashCode() => JsonObject.GetHashCode();

        public override string ToString() => JsonObject?.ToString();
    }
}
