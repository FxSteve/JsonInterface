using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JsonInterface;
using JsonInterface.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Tests
{
    [TestClass]
    public class ListOfListsTests
    {
        public interface IListOfValuesDefinedInJsonTest : IJsonObject
        {
            IJsonList<int?> Values { get; }
        }
        
        public interface IMyBaseList : IJsonObject
        {
            string Name { get; set; }
            string Version { get; }
            IJsonList<IMyBaseList> ReCurse { get; }

            IJsonList<IJsonList<IJsonList<IJsonList<IMyBaseList>>>> ListRecurseHell { get; }
        }
    }
}
