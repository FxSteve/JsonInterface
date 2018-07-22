using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface
{
    /// <summary>
    /// Base interface for list types.
    /// </summary>
    public interface IJsonList
    {
        /// <summary>
        /// The JArray token that this type is a wrapper for
        /// </summary>
        JArray JsonArray { get; set; }
    }

    /// <summary>
    /// Interface for list types.  T should be a valid nullable primitive type
    /// or a type that inherits from IJsonObject
    /// or a type that inherits from IJsonList&gt;T&lt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJsonList<T> : IJsonList, IList<T>
    {
        /// <summary>
        /// Add a new item to the list.
        /// Null for primitive types, 
        /// and empty object for object types
        /// and an empty list for list types
        /// </summary>
        /// <returns></returns>
        T AddNew();
    }
}
