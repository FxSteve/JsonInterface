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
    public class ConsistentApiTest
    {
        public interface IPrimitives : IJsonObject
        {
            int? Integer { get; set; }
            decimal? Decimal { get; set; }
            Guid? Guid { get; set; }
        }

        public interface IPrimitivesBad : IJsonObject
        {
            int Integer { get; set; }
            decimal Decimal { get; set; }
            Guid Guid { get; set; }
        }

        /// <summary>
        /// Changes to this test should be reviewed to determine if they represent breaking changes
        /// that should be reported and possibly bumping the major version
        /// </summary>
        [TestMethod]
        public void ApiIsConsistent()
        {
            // factory has 3 constructors
            var factory1 = new JsonInterfaceFactory();

            var factory2 = new JsonInterfaceFactory(new JsonInterfaceSettings
            {
                JsonSerializerSettings = new JsonSerializerSettings(),
                TrapExceptions = false
            });

            var factory3 = new JsonInterfaceFactory(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }, trapExceptions: true);

            Assert.IsFalse(factory1.Settings.TrapExceptions);
            Assert.IsFalse(factory2.Settings.TrapExceptions);
            Assert.IsTrue(factory3.Settings.TrapExceptions);

            var factoryPlain = factory1;
            var factoryCamelAndTrappedExceptions = factory3;


            // settings are utilized consistently throughout the object and child objects

            // Create returns a new object proxy and has 4 overloads
            var proxy1 = factoryPlain.Create<IPrimitives>();
            proxy1.Integer = 33;
            var proxy2 = factoryPlain.Create<IPrimitives>(JObject.FromObject(new { Integer = 33 }));
            var proxy3 = factoryPlain.Create<IPrimitives>("{ \"Integer\": \"33\" }");
            var proxy4 = factoryPlain.Create<IPrimitives>(v => v.Integer = 33);

            var proxies = ToList(proxy1, proxy2, proxy3, proxy4);

            proxies.ForEach(v =>
            {
                Assert.AreEqual(33, v.Integer);
                Assert.AreEqual(null, v.Decimal);
            });

            // all using the same serializer settings and having the same values, 
            // should output the same json string
            proxies.TrueForAll(v => v.ToString() == proxies[0].ToString());

            // CreateList returns a new list proxy and has 4 overloads
            var listProxy1 = factoryPlain.CreateList<IPrimitives>();
            listProxy1.AddNew().Integer = 33;

            var listProxy2 = factoryPlain.CreateList<IPrimitives>(JArray.FromObject(new[] { new { Integer = 33 } }));
            var listProxy3 = factoryPlain.CreateList<IPrimitives>("[{ \"Integer\": 33 }]");
            var listProxy4 = factoryPlain.CreateList<IPrimitives>(v => v.AddNew().Integer = 33);
            var listProxy5 = factoryPlain.CreateList<IPrimitives>(v => v.AddNewObject(x => x.Integer = 33));

            var listProxies = ToList(listProxy1, listProxy2, listProxy3, listProxy4, listProxy5);
            listProxies.ForEach(v => Assert.AreEqual(33, v.First().Integer));

            listProxies.TrueForAll(v => v.ToString() == listProxies[0].ToString());

            var listProxy7 = factoryPlain.CreateList<IJsonList<IPrimitives>>(v => v.AddNewList(x => x.AddNewObject(z => z.Integer = 33)));

            Assert.AreEqual(33, listProxy7.First().First().Integer);

            var listProxy8 = factoryPlain.CreateList<int?>();
            listProxy8.Add(8);
            var result = listProxy8.AddNew();

            Assert.AreEqual("[8,null]", listProxy8.ToString().RemoveWhitespace());
        }

        [TestMethod]
        public void ConsistentApiTestConstructors()
        {
            var factory1 = new JsonInterfaceFactory();
            AssertFactorySettings(factory1, trapExceptionsValue: false, contractResolverType: typeof(DefaultContractResolver));

            var factory2 = new JsonInterfaceFactory(new JsonInterfaceSettings
            {
                JsonSerializerSettings = new JsonSerializerSettings(),
                TrapExceptions = false
            });
            AssertFactorySettings(factory2, trapExceptionsValue: false, contractResolverType: null);

            var factory3 = new JsonInterfaceFactory(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }, trapExceptions: true);
            AssertFactorySettings(factory3, trapExceptionsValue: true, contractResolverType: typeof(CamelCasePropertyNamesContractResolver));

        }

        private void AssertFactorySettings(JsonInterfaceFactory factory, bool trapExceptionsValue, Type contractResolverType)
        {
            Assert.AreEqual(trapExceptionsValue, factory.Settings.TrapExceptions);

            var actualResolver = factory.Settings.JsonSerializerSettings.ContractResolver;
            if (contractResolverType == null)
            {
                Assert.IsNull(actualResolver);
            }
            else
            {
                Assert.IsInstanceOfType(actualResolver, contractResolverType);
            }
        }

        private List<T> ToList<T>(params T[] parameters) => parameters.ToList();
    }
}
