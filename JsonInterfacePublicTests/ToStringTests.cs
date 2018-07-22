using JsonInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface.PublicTests
{
    [TestClass]
    public class ToStringTests
    {
        public interface IMyObject : IJsonObject
        {
            int? MyNumber { get; set; }
            string MyName { get; set; }

            IMyEmbeddedObject MyEmbeddedObject { get; set; }

            IJsonList<IMyEmbeddedObject> MyEmbeddedObjects { get; set; }
        }

        public interface IMyEmbeddedObject : IJsonObject
        {
            Guid? MyGuid { get; set; }
        }

        [TestMethod]
        public void ToStringReturnsValidJsonTest()
        {
            var testGuid = Guid.NewGuid();
            var myJson = (new JsonInterfaceFactory()).Create<IMyObject>(v => v.MyName = "Earl");

            myJson.MyNumber = 99;
            myJson.MyEmbeddedObject.MyGuid = testGuid;

            var newObj = myJson.MyEmbeddedObjects.AddNew();
            newObj.MyGuid = testGuid;

            Assert.AreEqual(myJson.JsonObject.ToString(), myJson.ToString());
            var jsonString = myJson.ToString();

            var result = JsonConvert.DeserializeAnonymousType(jsonString, new { MyName = "", MyNumber = 0 });
            Assert.AreEqual("Earl", result.MyName);
            Assert.AreEqual(99, result.MyNumber);
        }
    }
}
