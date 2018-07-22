using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonInterfaceExtensions
    {
        /// <summary>
        /// Add a new object to the list of objects and initialize it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonList"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T AddNewObject<T>(this IJsonList<T> jsonList, Action<T> action) where T : IJsonObject
        {
            var result = ((JsonArrayListWrapper<T>)jsonList).AddNew();
            action(result);
            return result;
        }

        /// <summary>
        /// Add a new object to the list of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonList"></param>
        /// <returns></returns>
        public static T AddNewObject<T>(this IJsonList<T> jsonList) where T : IJsonObject =>
             ((JsonArrayListWrapper<T>)jsonList).AddNew();

        /// <summary>
        /// Add a new list to the list of lists and initialize it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonList"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T AddNewList<T>(this IJsonList<T> jsonList, Action<T> action) where T : IJsonList
        {
            var result = ((JsonArrayListWrapper<T>)jsonList).AddNew();
            action(result);
            return result;
        }

        /// <summary>
        /// Add a new list to the list of lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonList"></param>
        /// <returns></returns>
        public static T AddNewList<T>(this IJsonList<T> jsonList) where T : IJsonList =>
            ((JsonArrayListWrapper<T>)jsonList).AddNew();
    }
}
