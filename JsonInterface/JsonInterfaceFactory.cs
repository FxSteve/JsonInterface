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
    public class JsonInterfaceFactory
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        private static T GetDynamicProxy<T>(JObject jObject)
            where T : class, IJsonObject
        {
            var propertyInterceptor = new JsonInterfacePropertyInterceptor<T>(jObject);
            return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(propertyInterceptor);
        }

        public static T Create<T>(string json)
            where T : class, IJsonObject => 
            Create<T>(JObject.Parse(json));

        public static T Create<T>()
            where T : class, IJsonObject => 
            Create<T>(new JObject());

        public static T Create<T>(JObject jObject)
            where T : class, IJsonObject => 
            GetDynamicProxy<T>(jObject);

        public static T Create<T>(Action<T> initializer)
            where T : class, IJsonObject
        {
            var newValue = Create<T>();
            initializer(newValue);
            return newValue;
        }
    }
}
