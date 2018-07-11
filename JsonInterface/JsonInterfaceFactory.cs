using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("JsonInterfaceTests")]

namespace JsonInterface
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonInterfaceFactory
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static ProxyGenerationOptions generationOptions = new ProxyGenerationOptions
        {
            BaseTypeForInterfaceProxy = typeof(JsonBase)
        };

        private static T GetDynamicProxy<T>(JObject jObject)
            where T : class, IJsonObject
        {
            var propertyInterceptor = new JsonInterfacePropertyInterceptor<T>(jObject);
            var proxy = proxyGenerator
                .CreateInterfaceProxyWithoutTarget<T>(generationOptions, propertyInterceptor);

            proxy.JsonObject = jObject;
            return proxy;
        }

        /// <summary>
        /// Create a new json interface, using a json string to initialize the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Create<T>(string json)
            where T : class, IJsonObject =>
            Create<T>(JObject.Parse(json));

        /// <summary>
        /// Create a new json interface, with an empty json object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>()
            where T : class, IJsonObject =>
            Create<T>(new JObject());

        /// <summary>
        /// Create a new json interface from an existing JObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public static T Create<T>(JObject jObject)
            where T : class, IJsonObject =>
            GetDynamicProxy<T>(jObject);

        /// <summary>
        /// Create a new json interface from an empty object, but execute the 
        /// initializer on it before returning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public static T Create<T>(Action<T> initializer)
            where T : class, IJsonObject
        {
            var newValue = Create<T>();
            initializer(newValue);
            return newValue;
        }

        /// <summary>
        /// Create a new json list interface, using a json string to initialize the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>(string json) =>
            CreateList<T>(JArray.Parse(json));

        /// <summary>
        /// Create a new json interface, with an empty json object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>() =>
            CreateList<T>(new JArray());

        /// <summary>
        /// Create a new json interface from an existing JObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jArray"></param>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>(JArray jArray) =>
            new JArrayListWrapper<T>(jArray);

        /// <summary>
        /// Create a new json interface from an empty object, but execute the 
        /// initializer on it before returning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>(Action<IJsonList<T>> initializer)
        {
            var newValue = CreateList<T>();
            initializer(newValue);
            return newValue;
        }

    }
}
