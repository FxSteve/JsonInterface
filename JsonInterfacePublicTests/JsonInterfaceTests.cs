using System;
using System.Diagnostics;
using System.Linq;
using JsonInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonInterface.PublicTests
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
        public void BasicTypesTest()
        {
            var jsonObject = JObject.Parse("{}");

            var tester = JsonInterfaceFactory.Create<ITestInterface>(jsonObject);

            tester.Name = "Morris";

            Assert.AreEqual("Morris", tester.Name);

            tester.Ages.Clear();
            new[] { 5, 6, 7, 8 }
                .ToList()
                .ForEach(v => tester.Ages.Add(v));

            tester.Names.Add("Yo");
            tester.Names.Add("Ho");
            tester.Names.Add("Go");

            tester.Elements.Add(JsonInterfaceFactory.Create<ITestElement>(v =>
           {
               v.Something = "some value";
               v.Value = 88;
           }));

            tester.ListOfElements.Add(null);
            tester.ListOfElements.Add(null);

            Assert.AreEqual(26, tester.Ages.Sum());
            Debug.WriteLine(tester.JsonObject.ToString());
        }

        public interface IHaveNonNullableValueTypes : IJsonObject
        {
            int BadType { get; set; }
        }

        [TestMethod]
        public void InterfaceDisallowsNonNullableValueTypes()
        {
            var wasCaught = false;

            try
            {
                var badType = JsonInterfaceFactory.Create<IHaveNonNullableValueTypes>();
                var badTypePropertyResult = badType.BadType;
            }
            catch (JsonInterfaceException)
            {
                wasCaught = true;
            }

            Assert.IsTrue(wasCaught);
        }

        public interface IHasPath : IJsonObject
        {
            IHaveNonNullableValueTypes HaveNonNullableValueTypes { get; set; }
            IHasPath Child { get; set; }
        }

        [TestMethod]
        public void ExceptionIncludesPath()
        {
            var wasCaught = false;

            try
            {
                var badType = JsonInterfaceFactory.Create<IHasPath>();
                var badTypePropertyResult = badType.Child.Child.Child.HaveNonNullableValueTypes.BadType;
            }
            catch (JsonInterfaceException ex)
            {
                Assert.AreEqual("child.child.child.haveNonNullableValueTypes.badType", ex.TokenPath);
                wasCaught = true;
            }

            Assert.IsTrue(wasCaught);
        }

        public interface IHaveDisallowedTypes : IJsonObject
        {
            PocoType PocoType { get; set; }
        }

        public class PocoType
        {

        }
    }
}
