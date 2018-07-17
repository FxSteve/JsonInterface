using JsonInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonInterface.PublicTests
{
    [TestClass]
    public class VariousTypeTests
    {
        static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public interface IPropTestInterface<T> : IJsonObject
        {
            T MyProperty { get; set; }
        }

        [TestMethod]
        public void GuidTest()
        {
            var guidString = "1CF7C42A-7D2B-401E-AF92-7443EA9B422F";
            var guidJson = JsonConvert.SerializeObject(new
            {
                MyProperty = guidString
            }, _jsonSettings);

            var guidInterface = JsonInterfaceFactory.Create<IPropTestInterface<Guid?>>(guidJson, _jsonSettings);

            Assert.AreEqual(new Guid(guidString), guidInterface.MyProperty);
        }

        [TestMethod]
        public void StringTest()
        {
            var stringJson = JsonConvert.SerializeObject(new
            {
                MyProperty = "anything"
            }, _jsonSettings);

            var stringInterface = JsonInterfaceFactory.Create<IPropTestInterface<string>>(stringJson, _jsonSettings);

            var result = stringInterface.MyProperty;
            Assert.AreEqual("anything", result);
        }

        public interface IInheritanceTest : IPropTestInterface<string>
        {
            Guid? MyGuid { get; set; }
        }

        [TestMethod]
        public void InheritedPropertyTest()
        {
            var guidString = "1CF7C42A-7D2B-401E-AF92-7443EA9B422F";

            var stringJson = JsonConvert.SerializeObject(new
            {
                MyProperty = "anything",
                MyGuid = guidString
            }, _jsonSettings);

            var inheritedInterface = JsonInterfaceFactory.Create<IInheritanceTest>(stringJson, _jsonSettings);

            var result = inheritedInterface.MyProperty;
            var result2 = inheritedInterface.MyGuid;

            Assert.AreEqual("anything", result);
            Assert.AreEqual(new Guid(guidString), result2);
        }

        public interface IMyType : IJsonObject
        {
            int? Id { get; set; }
            Guid? Id2 { get; set; }
            string Name { get; set; }
        }

        [TestMethod]
        public void SettingsTest()
        {
            var settings = new JsonInterfaceSettings().JsonSerializerSettings;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var serializer = JsonSerializer.Create(settings);

            var contract = serializer.ContractResolver.ResolveContract(typeof(IMyType));
            var contract2 = serializer.ContractResolver.ResolveContract(typeof(List<int>));

            var properties = ((JsonObjectContract)contract).Properties;

            var myProperties = properties
                .Select(v => new { v.PropertyName, v.UnderlyingName })
                .ToDictionary(v => v.UnderlyingName, v => v.PropertyName);

            var reverseDictionary = myProperties.ToDictionary(v => v.Value, v => v.Key);

        }
    }
}
