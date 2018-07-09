using JsonInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterfaceTests
{
    [TestClass]
    public class VariousTypeTests
    {
        static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public interface IGuidInterface : IJsonObject
        {
            Guid? MyGuid { get; set; }
        }

        [TestMethod]
        public void GuidTest()
        {
            var guidJson = JsonConvert.SerializeObject(new
            {
                MyGuid = "1CF7C42A-7D2B-401E-AF92-7443EA9B422F"
            }, _jsonSettings);

            var guidInterface = JsonInterfaceFactory.Create<IGuidInterface>(guidJson);

            var result = guidInterface.MyGuid;
        }
    }
}
