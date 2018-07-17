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
    public class JsonInterfaceFactory
    {
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static readonly ProxyGenerationOptions generationOptions = new ProxyGenerationOptions
        {
            BaseTypeForInterfaceProxy = typeof(JsonBase)
        };

        private T GetDynamicProxy<T>(JObject jObject)
            where T : class, IJsonObject
        {
            _settings = _settings.DefaultIfNull();

            var propertyInterceptor = new JsonInterfacePropertyInterceptor<T>(_settings);
            var proxy = proxyGenerator
                .CreateInterfaceProxyWithoutTarget<T>(generationOptions, propertyInterceptor);

            var jsonBase = proxy as JsonBase;
            jsonBase.Factory = this;
            jsonBase.JsonObject = jObject;
            jsonBase.JsonInterfaceSettings = _settings;
            jsonBase.ObjectPropertyNameToJsonPropertyName = propertyInterceptor.ObjectPropertyNameToJsonPropertyName;
            return proxy;
        }
        
        /// <summary>
        /// New interface factory with all the defaults
        /// </summary>
        public JsonInterfaceFactory() => Initialize(null);

        /// <summary>
        /// New interface factory with specific serializer and exception settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="trapExceptions"></param>
        public JsonInterfaceFactory(
            JsonSerializerSettings settings,
            bool trapExceptions = false)
        {
            var interfaceSettings = new JsonInterfaceSettings
            {
                JsonSerializerSettings = settings
            };

            Initialize(interfaceSettings);
        }

        /// <summary>
        /// New interface factory with settings 
        /// </summary>
        /// <param name="jsonInterfaceSettings"></param>
        public JsonInterfaceFactory(JsonInterfaceSettings jsonInterfaceSettings) =>
            Initialize(jsonInterfaceSettings);

        private void Initialize(JsonInterfaceSettings jsonInterfaceSettings)
        {
            _settings = jsonInterfaceSettings.DefaultIfNull();
        }

        private JsonInterfaceSettings _settings;
        
        /// <summary>
        /// Create a new json interface, using a json string to initialize the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Create<T>(string json)
            where T : class, IJsonObject
        {
            _settings = _settings.DefaultIfNull();

            return Create<T>(JsonConvert.DeserializeObject<JObject>(
                    json,
                    _settings.JsonSerializerSettings));
        }

        /// <summary>
        /// Create a new json interface, with an empty json object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>()
            where T : class, IJsonObject =>
            Create<T>(new JObject());

        /// <summary>
        /// Create a new json interface from an existing JObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public T Create<T>(JObject jObject)
            where T : class, IJsonObject =>
            GetDynamicProxy<T>(jObject);

        /// <summary>
        /// Create a new json interface from an empty object, but execute the 
        /// initializer on it before returning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public T Create<T>(Action<T> initializer)
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
        public IJsonList<T> CreateList<T>(string json) =>
            CreateList<T>(JsonConvert.DeserializeObject<JArray>(
                json,
                _settings.DefaultIfNull().JsonSerializerSettings));

        /// <summary>
        /// Create a new json interface, with an empty json object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IJsonList<T> CreateList<T>() =>
            CreateList<T>(new JArray());

        /// <summary>
        /// Create a new json interface from an existing JObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jArray"></param>
        /// <returns></returns>
        public IJsonList<T> CreateList<T>(JArray jArray) =>
            new JsonArrayListWrapper<T>(jArray, new JsonBase
            {
                 JsonInterfaceSettings= _settings,
                 Factory = this
            });

        /// <summary>
        /// Create a new json interface from an empty object, but execute the 
        /// initializer on it before returning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public IJsonList<T> CreateList<T>(Action<IJsonList<T>> initializer)
        {
            var newValue = CreateList<T>();
            initializer(newValue);
            return newValue;
        }
    }
}
