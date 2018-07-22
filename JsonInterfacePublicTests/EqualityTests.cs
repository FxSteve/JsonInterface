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

            var factory = new JsonInterfaceFactory();

            var left = factory.Create<IEqualityTest>(j);
            var right = factory.Create<IEqualityTest>(j);

            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(right.Equals(left));
        }

        [TestMethod]
        public void EqualIfJObjectComparedToProxy()
        {
            var factory = new JsonInterfaceFactory();

            var jsonString = "{ \"prop\": 445 }";
            var leftJObject = JObject.Parse(jsonString);
            var rightJObject = JObject.Parse(jsonString);

            var left = factory.Create<IEqualityTest>(leftJObject);
            var right = factory.Create<IEqualityTest>(rightJObject);

            // proxies refer to different JObjects, so they are not equal
            Assert.IsFalse(right.Equals(left));
            Assert.IsFalse(left.Equals(right));

            // proxies came from same initial json, so DeepEquals should be true
            Assert.IsTrue(JToken.DeepEquals(left.JsonObject, right.JsonObject));
        }
    }
}
