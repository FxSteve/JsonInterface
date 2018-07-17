using System;
using Castle.DynamicProxy;
using JsonInterface.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("JsonInterfaceTests")]

namespace JsonInterface
{
    /// <summary>
    /// Factory for JsonInterface proxies 
    /// </summary>
    public static class JsonInterfaceFactory
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static ProxyGenerationOptions generationOptions = new ProxyGenerationOptions
        {
            BaseTypeForInterfaceProxy = typeof(JsonBase)
        };

        private static T GetDynamicProxy<T>(JObject jObject, JsonInterfaceSettings settings)
            where T : class, IJsonObject
        {
            settings = settings.DefaultIfNull();

            var propertyInterceptor = new JsonInterfacePropertyInterceptor<T>(settings);
            var proxy = proxyGenerator
                .CreateInterfaceProxyWithoutTarget<T>(generationOptions, propertyInterceptor);

            var jsonBase = proxy as JsonBase;
            jsonBase.JsonObject = jObject;
            jsonBase.JsonInterfaceSettings = settings;
            jsonBase.ObjectPropertyNameToJsonPropertyName = propertyInterceptor.ObjectPropertyNameToJsonPropertyName;
            return proxy;
        }

        /// <summary>
        /// Create a new json interface, using a json string to initialize the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T Create<T>(string json, JsonInterfaceSettings settings = null)
            where T : class, IJsonObject
        {
            settings = settings.DefaultIfNull();

            return Create<T>(JsonConvert.DeserializeObject<JObject>(
                    json,
                    settings.JsonSerializerSettings),
                    settings);
        }

        /// <summary>
        /// Create a new json interface, with an empty json object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>(JsonInterfaceSettings settings = null)
            where T : class, IJsonObject =>
            Create<T>(new JObject(), settings);

        /// <summary>
        /// Create a new json interface from an existing JObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jObject"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T Create<T>(JObject jObject, JsonInterfaceSettings settings = null)
            where T : class, IJsonObject =>
            GetDynamicProxy<T>(jObject, settings);

        /// <summary>
        /// Create a new json interface from an empty object, but execute the 
        /// initializer on it before returning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T Create<T>(Action<T> initializer, JsonInterfaceSettings settings = null)
            where T : class, IJsonObject
        {
            var newValue = Create<T>(settings);
            initializer(newValue);
            return newValue;
        }

        /// <summary>
        /// Create a new json list interface, using a json string to initialize the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>(string json, JsonInterfaceSettings settings = null) =>
            CreateList<T>(JsonConvert.DeserializeObject<JArray>(
                json,
                settings.DefaultIfNull().JsonSerializerSettings));

        /// <summary>
        /// Create a new json interface, with an empty json object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>(JsonInterfaceSettings settings = null) =>
            CreateList<T>(new JArray(), settings);

        /// <summary>
        /// Create a new json interface from an existing JObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jArray"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>(JArray jArray, JsonInterfaceSettings settings = null) =>
            new JArrayListWrapper<T>(jArray, settings.DefaultIfNull());

        /// <summary>
        /// Create a new json interface from an empty object, but execute the 
        /// initializer on it before returning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IJsonList<T> CreateList<T>(Action<IJsonList<T>> initializer, JsonInterfaceSettings settings = null)
        {
            var newValue = CreateList<T>(settings);
            initializer(newValue);
            return newValue;
        }
    }
}
