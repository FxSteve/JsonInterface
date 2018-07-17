using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface.PublicTests
{
    [TestClass]
    public class EqualityTests
    {
        public interface IEqualityTest : IJsonObject
        {

        }

        [TestMethod]
        public void EqualIfSameJObjectButDifferentProxy()
        {
            var j = new JObject();

            var left = JsonInterfaceFactory.Create<IEqualityTest>(j);
            var right = JsonInterfaceFactory.Create<IEqualityTest>(j);

            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(right.Equals(left));
        }

        [TestMethod]
        public void EqualIfJObjectComparedToProxy()
        {
            var jsonString = "{ \"prop\": 445 }";
            var leftJObject = JObject.Parse(jsonString);
            var rightJObject = JObject.Parse(jsonString);

            var left = JsonInterfaceFactory.Create<IEqualityTest>(leftJObject);
            var right = JsonInterfaceFactory.Create<IEqualityTest>(rightJObject);

            // proxies refer to different JObjects, so they are not equal
            Assert.IsFalse(right.Equals(left));
            Assert.IsFalse(left.Equals(right));

            // proxies came from same initial json, so DeepEquals should be true
            Assert.IsTrue(JToken.DeepEquals(left.JsonObject, right.JsonObject));
        }
    }
}
