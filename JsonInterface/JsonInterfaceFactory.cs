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

        public static class FacadeFor<T>
            where T : class, IJsonObject
        {
            private static T GetDynamicProxy(JObject jObject)
            {
                var propertyInterceptor = new JsonInterfacePropertyInterceptor<T>(jObject);
                var proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(propertyInterceptor);
                return proxy;
            }

            public static T Create() => Create(new JObject());
            public static T Create(Action<T> initializer)
            {
                var newValue = Create();
                initializer(newValue);
                return newValue;
            }

            public static T Create(JObject jObject) => GetDynamicProxy(jObject);
        }
    }
}