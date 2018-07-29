using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface.PublicTests
{
    [TestClass]
    public class AlternateBaseTypeTests
    {
        public class JsonBasePlus : JsonBase
        {
            public string Tag { get => this.JsonObject.TryGetValue("tag", out var value) ? value.ToString() : null; }
        }

        public interface IMyJson : IJsonObject
        {
            string Name { get; set; }
        }

        [TestMethod]
        public void ProxyGeneratedWithAlternateBaseTypeTest()
        {
            var factory = new JsonInterfaceFactory(new JsonInterfaceSettings
            {
                BaseType = typeof(JsonBasePlus),
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            var result = factory.Create<IMyJson>("{ \"name\": \"Simon\", \"tag\": \"tag-yer-it\" }");

            Assert.IsInstanceOfType(result, typeof(JsonBasePlus));
            Assert.AreEqual("Simon", result.Name);
            Assert.AreEqual("tag-yer-it", ((JsonBasePlus)result).Tag);
        }
    }
}
