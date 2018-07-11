using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JsonInterface;
using JsonInterface.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonInterface.PublicTests
{
    [TestClass]
    public class ListOfListsTests
    {
        public interface IListOfValuesDefinedInJsonTest : IJsonObject
        {
            IJsonList<Int32?> Values { get; }
        }

        [TestMethod]
        public void ListOfValuesDefinedInJsonTest()
        {
            var jsonObject = JObject.Parse("{\"values\": [1,2,3,4,5]}");

            var tester = JsonInterfaceFactory.Create<IListOfValuesDefinedInJsonTest>(jsonObject);

            var result = tester.Values.Sum();

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void ManipulateListOfValuesDefinedInJsonTest()
        {
            var jsonObject = JObject.Parse("{\"values\": [1,2,3,4,5]}");

            var tester = JsonInterfaceFactory.Create<IListOfValuesDefinedInJsonTest>(jsonObject);

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

            var tester = JsonInterfaceFactory.Create<IListOfValuesDefinedInJsonTest>(jsonObject);

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
            var tester = JsonInterfaceFactory.Create<IMyBaseList>(JObject.Parse("{ \"version\": \"1.2.3.4\" }"));

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
