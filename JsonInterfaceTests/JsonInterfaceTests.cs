using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JsonInterface;
using JsonInterface.Extensions;
using JsonInterface.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonInterface.Tests
{
    [TestClass]
    public class JsonInterfaceTests
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public interface ITestInterface : IJsonObject
        {
            string Name { get; set; }
            int? Count { get; set; }
            IJsonList<int?> Ages { get; }
            IJsonList<string> Names { get; }
            IJsonList<ITestElement> Elements { get; }
            IJsonList<IJsonList<ITestElement>> ListOfElements { get; }
        }

        public interface ITestElement : IJsonObject
        {
            string Something { get; set; }
            int? Value { get; set; }
        }

        [TestMethod]
        public void ForceGetArrayPropertyTest()
        {
            var jsonBase = new JsonBase { JsonObject = JObject.Parse("{}") };

            var result = jsonBase.ForceGetArrayPropertyToken("myArray");

            Assert.IsTrue(result.Type == JTokenType.Array);
            result.Add(JToken.FromObject("Hi There"));

            Debug.WriteLine(result.ToString());
        }


        [TestMethod]
        public void TestGenericStuff()
        {
            var noCamelSerializerSettings = new JsonSerializerSettings();

            var myObject = new
            {
                Age = (int?)27,
                AgeNull = (int?)null,
                Name = "David",
                NameNull = (string)null,
                MyProfile = new { Address = "1234 Any Street" },
                MyValues = new[] { new { Storms = "Norm" }, new { Storms = "Strange" } }
            };
            var myJson = JsonConvert.SerializeObject(myObject, noCamelSerializerSettings);

            var jsonBase = new JsonBase { JsonObject = JObject.Parse(myJson) };

            var resultAge = new PrimitiveTypeHandler<int?>().GetPropertyValue(jsonBase, "Age");
            var resultNull = new PrimitiveTypeHandler<int?>().GetPropertyValue(jsonBase, "AgeNull");
            var nameResult = new PrimitiveTypeHandler<string>().GetPropertyValue(jsonBase, "Name");
            var nameNullResult = new PrimitiveTypeHandler<string>().GetPropertyValue(jsonBase, "NameNull");

            Assert.AreEqual(27, resultAge);
            Assert.AreEqual(null, resultNull);
            Assert.AreEqual("David", nameResult);
            Assert.AreEqual(null, nameNullResult);

            var valueToken = jsonBase.ForceGetValuePropertyToken("Hokey");
            Assert.IsNotNull(valueToken);
            Assert.IsNull(valueToken.Value<string>());
        }

        [TestMethod]
        public void HandlerDisallowsNonNullableValueTypes()
        {
            var wasCaught = false;
            try
            {
                new PrimitiveTypeHandler<int>().FromToken(null, null);
            }
            catch (ArgumentException)
            {
                wasCaught = true;
            }
            Assert.IsTrue(wasCaught);
        }

        public interface IHaveNonNullableValueTypes : IJsonObject
        {
            int BadType { get; set; }
        }

        public interface IHaveDisallowedTypes : IJsonObject
        {
            PocoType PocoType { get; set; }
        }

        public class PocoType
        {

        }

        [TestMethod]
        public void InterfaceDisallowsInvalidTypes()
        {
            var wasCaught = false;

            try
            {
                var badType = (new JsonInterfaceFactory()).Create<IHaveDisallowedTypes>();
                var badTypePropertyResult = badType.PocoType;
            }
            catch (Exception ex) when (ex.Message.Contains(JsonInterfacePropertyInterceptor<object>.InterceptorFaultMessagePattern.Replace("{0}", "")))
            {
                wasCaught = true;
            }

            Assert.IsTrue(wasCaught);
        }

        [TestMethod]
        public void StringTypeHandlerTest()
        {
            var myObject = new
            {
                Age = (int?)27,
                AgeNull = (int?)null,
                Name = "David",
                NameNull = (string)null,
                MyProfile = new { Address = "1234 Any Street" },
                MyValues = new[] { new { Storms = "Norm" }, new { Storms = "Strange" } }
            };

            var myJson = JsonConvert.SerializeObject(myObject);
            var jsonBase = new JsonBase { JsonObject = JObject.Parse(myJson) };

            var stringTypeHandler = new PrimitiveTypeHandler<string>();

            var nameValue = stringTypeHandler.GetPropertyValue(jsonBase, nameof(myObject.Name));
            var nameNullValue = stringTypeHandler.GetPropertyValue(jsonBase, nameof(myObject.NameNull));

            Assert.AreEqual("David", nameValue);
            Assert.AreEqual(null, nameNullValue);

            stringTypeHandler.SetPropertyValue(jsonBase, jsonBase.GetJsonPropertyNameFromPropertyName(nameof(myObject.Name)), "Seth");
            stringTypeHandler.SetPropertyValue(jsonBase, jsonBase.GetJsonPropertyNameFromPropertyName(nameof(myObject.NameNull)), "SethNotNull");

            nameValue = stringTypeHandler.GetPropertyValue(jsonBase, jsonBase.GetJsonPropertyNameFromPropertyName(nameof(myObject.Name)));
            nameNullValue = stringTypeHandler.GetPropertyValue(jsonBase, jsonBase.GetJsonPropertyNameFromPropertyName(nameof(myObject.NameNull)));

            Assert.AreEqual("Seth", nameValue);
            Assert.AreEqual("SethNotNull", nameNullValue);
        }

        [TestMethod]
        public void ObjectTypeHandlerTest()
        {
            var myObject = new
            {
                Age = (int?)27,
                AgeNull = (int?)null,
                Name = "David",
                NameNull = (string)null,
                MyProfile = new { Address = "1234 Any Street" },
                MyValues = new[] { new { Storms = "Norm" }, new { Storms = "Strange" } }
            };

            var myJson = JsonConvert.SerializeObject(myObject, _serializerSettings);

            var obj = (new JsonInterfaceFactory(_serializerSettings)).Create<IMyObject>(myJson);
            var profile = obj.MyProfile;

            Assert.AreEqual("1234 Any Street", profile.Address);
        }

        public interface IMyObject : IJsonObject
        {
            string Name { get; set; }
            IAddress MyProfile { get; }
            IJsonList<IMyValue> MyValues { get; }
        }

        public interface IAddress : IJsonObject
        {
            string Address { get; set; }
        }

        public interface IMyValue : IJsonObject
        {
            string Storms { get; set; }
        }

    }
}
