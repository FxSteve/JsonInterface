using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JsonInterface;
using JsonInterface.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonInterface.PublicTests
{
    [TestClass]
    public class ListOfListsTests
    {
        static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public interface IListOfValuesDefinedInJsonTest : IJsonObject
        {
            IJsonList<Int32?> Values { get; }
        }

        [TestMethod]
        public void ListOfValuesDefinedInJsonTest()
        {
            var jsonObject = JObject.Parse("{\"values\": [1,2,3,4,5]}");

            var tester = JsonInterfaceFactory.Create<IListOfValuesDefinedInJsonTest>(jsonObject, _jsonSettings);

            var result = tester.Values.Sum();

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void ManipulateListOfValuesDefinedInJsonTest()
        {
            var jsonObject = JObject.Parse("{\"values\": [1,2,3,4,5]}");

            var tester = JsonInterfaceFactory.Create<IListOfValuesDefinedInJsonTest>(jsonObject, _jsonSettings);

            tester.Values.RemoveAt(2); // number at index 2 is "3"
            tester.Values.Remove(5);
            tester.Values.Add(14);

            var result = tester.Values.Sum();

            Assert.AreEqual(21, result);
        }

        [TestMethod]
        public void IndexOfTest()
        {
            var jsonObject = JObject.Parse("{\"values\": [1,2,3,4,5]}");

            var tester = JsonInterfaceFactory.Create<IListOfValuesDefinedInJsonTest>(jsonObject, _jsonSettings);

            var result = tester.Values.IndexOf(3);

            Assert.AreEqual(2, result);
        }

        public interface IMyBaseList : IJsonObject
        {
            string Name { get; set; }
            string Version { get; }
            IJsonList<IMyBaseList> ReCurse { get; }

            IJsonList<IJsonList<IJsonList<IJsonList<IMyBaseList>>>> ListRecurseHell { get; }
        }

        [TestMethod]
        public void ListOfListsOfListsTest()
        {
            var tester = JsonInterfaceFactory.Create<IMyBaseList>(JObject.Parse("{ \"version\": \"1.2.3.4\" }"), _jsonSettings);

            var baseList = tester.ReCurse;

            baseList.Add(null);

            var childList = baseList.First().ReCurse;
            childList.Add(null);

            var grandchildList = childList.First().ReCurse;
            grandchildList.Add(JsonInterfaceFactory.Create<IMyBaseList>());

            var ggcl = grandchildList.First().ReCurse;
            ggcl.Add(JsonInterfaceFactory.Create<IMyBaseList>());

            Assert.AreEqual("1.2.3.4", tester.Version);
            Assert.IsNull(ggcl.First().Version);

            var x = tester.ListRecurseHell.AddNew();
            var y = x.AddNew();
            var z = y.AddNew();
            var a = z.AddNew();
            a.Name = "One of those things";
            a = z.AddNew();
            a.Name = "Another of those things";

            Debug.Print(tester.JsonObject.ToString());

        }

        [TestMethod]
        public void ListSerializesCorrectlyTest()
        {
            var myList = JsonInterfaceFactory.CreateList<int?>(v =>
            {
                v.AddNew();
                v.AddNew();
                v.Add(44);
                v.Add(null);
            });

            Assert.AreEqual(myList.ToString().RemoveWhitespace(), "[null,null,44,null]");
        }

        [TestMethod]
        public void ListForEachValueTest()
        {
            var myList = JsonInterfaceFactory.CreateList<int?>(v =>
            {
                v.AddNew();
                v.AddNew();
                v.Add(null);
                v.Add(null);
            });

            foreach (var item in myList)
            {
                Assert.IsNull(item, $"List of int? should successfully return null for items in {nameof(ListForEachValueTest)}");
            }
        }

        [TestMethod]
        public void ListForEachObjectTest()
        {
            var myList = JsonInterfaceFactory.CreateList<IMyBaseList>(v =>
            {
                v.AddNew();
                v.AddNew();
                v.Add(null);
                v.Add(null);
            });

            foreach (var item in myList)
            {
                Assert.IsInstanceOfType(item, typeof(IMyBaseList), $"List of object should successfully return null for items in {nameof(ListForEachObjectTest)}");
            }
        }

        [TestMethod]
        public void ListForEachListTest()
        {
            var myList = JsonInterfaceFactory.CreateList<IJsonList<int?>>(v =>
            {
                v.AddNew();
                v.AddNew();
                v.Add(null);
                v.Add(null);
            });

            foreach (var item in myList)
            {
                Assert.IsInstanceOfType(item, typeof(IJsonList<int?>), $"List of int? should successfully return null for items in {nameof(ListForEachListTest)}");
            }
        }

        public interface IListItem : IJsonObject
        {
            Guid? Id { get; set; }
            string Version { get; set; }
        }

        [TestMethod]
        public void ParseListTest()
        {
            var json = "[{ \"id\": \"6f2572e3-faed-437a-b465-86ec126a9001\", " +
                " \"version\": \"string\", " +
                " } ]";

            var list = JsonInterfaceFactory.CreateList<IListItem>(json);

            foreach (var item in list)
            {
                Assert.IsInstanceOfType(item, typeof(IListItem));
            }
        }

        [TestMethod]
        public void ListAddNewTests()
        {
            var myList = JsonInterfaceFactory.CreateList<int?>();

            myList.AddNew();
            myList.Add(33);

            Assert.AreEqual(1, myList.Count(v => v == null));
            Assert.AreEqual(1, myList.Count(v => v == 33));
        }

        [TestMethod]
        public void ListFailsForBadTypes()
        {
            var wasCaught = false;
            try
            {
                var mylist = JsonInterfaceFactory.CreateList<int>();
            }
            catch (ArgumentException)
            {
                wasCaught = true;
            }

            Assert.IsTrue(wasCaught);
        }
    }
}
