using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using JsonInterface.Extensions;
using JsonInterface.Handlers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonInterface
{
    internal class JsonInterfacePropertyInterceptor<T> : IInterceptor
    {
        internal const string InterceptorFaultMessagePattern = "{0} is not a valid IJsonList<>, IJsonObject, or an acceptable Newtonsoft.Json Primitive type";

        internal static readonly Dictionary<MethodInfo, Func<JsonBase, JsonInterfaceSettings, object>> Getters = new Dictionary<MethodInfo, Func<JsonBase, JsonInterfaceSettings, object>>();
        private static readonly Dictionary<MethodInfo, Action<JsonBase, JsonInterfaceSettings, object>> Setters = new Dictionary<MethodInfo, Action<JsonBase, JsonInterfaceSettings, object>>();

        private static bool IsFaulted = false;
        public static readonly List<string> FaultMessages = new List<string>();

        private static void AddGetter(MethodInfo methodInfo, Func<JsonBase, JsonInterfaceSettings, object> getter)
        {
            if (methodInfo != null) Getters.Add(methodInfo, getter);
        }

        private static void AddSetter(MethodInfo methodInfo, Action<JsonBase, JsonInterfaceSettings, object> setter)
        {
            if (methodInfo != null) Setters.Add(methodInfo, setter);
        }

        static JsonInterfacePropertyInterceptor()
        {
            var type = typeof(T);
            try
            {
                var interfaceList = type
                    .GetInterfaces()
                    .Where(v => v != typeof(IJsonObject))
                    .ToList();

                interfaceList.Add(type);

                var properties = interfaceList.SelectMany(v => v.GetProperties());

                var contractResolver = new DefaultContractResolver();

                foreach (var property in properties)
                {
                    var getter = property.GetGetMethod();
                    var setter = property.GetSetMethod();
                    var propertyName = property.Name;
                    var propertyType = property.PropertyType;

                    AddGetter(getter, GetGetterFunc(propertyName, propertyType));
                    AddSetter(setter, GetSetterFunc(propertyName, propertyType));

                    if (!IsValidJsonInterfaceType(propertyType, contractResolver))
                    {
                        IsFaulted = true;
                        FaultMessages.Add(string.Format(InterceptorFaultMessagePattern, propertyType.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                IsFaulted = true;
                FaultMessages.Add($"Exception: {ex.Message}, Type: {typeof(T).Name}");
            }
        }

        /// <summary>
        /// Returns true if type is a valid jsonInterface type
        /// Type must be an IJsonList&lt;&gt;, IJsonObject, or a valid primitive type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contractResolver"></param>
        /// <returns></returns>
        private static bool IsValidJsonInterfaceType(Type type, IContractResolver contractResolver) =>
            (type.IsInterface && typeof(IJsonList).IsAssignableFrom(type)) ||
            (type.IsInterface && typeof(IJsonObject).IsAssignableFrom(type)) ||
            contractResolver.ResolveContract(type) is JsonPrimitiveContract;

        private static Func<JsonBase, JsonInterfaceSettings, object> GetGetterFunc(string propertyName, Type type)
        {
            var genericTypeParameter = type;

            var method = typeof(HandlerFor<>)
                .MakeGenericType(genericTypeParameter)
                .GetMethod(nameof(HandlerFor<object>.GetPropertyValue), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            var propertyNameExpression = Expression.Constant(propertyName);
            var jsonBaseParameterExpression = Expression.Parameter(typeof(JsonBase));
            var jsonInterfaceSettingsExpression = Expression.Parameter(typeof(JsonInterfaceSettings));

            var expression = Expression.Lambda<Func<JsonBase, JsonInterfaceSettings, object>>(
                Expression.Convert(Expression.Call(null, method,
                    jsonBaseParameterExpression,
                    propertyNameExpression,
                    jsonInterfaceSettingsExpression), typeof(object)),
                jsonBaseParameterExpression,
                jsonInterfaceSettingsExpression);

            var result = expression.Compile();
            return result;
        }

        private static Action<JsonBase, JsonInterfaceSettings, object> GetSetterFunc(string propertyName, Type type)
        {
            var method = typeof(HandlerFor<>)
                .MakeGenericType(type)
                .GetMethod(nameof(HandlerFor<object>.SetPropertyValue), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            var propertyNameExpression = Expression.Constant(propertyName);
            var jsonBaseParameterExpression = Expression.Parameter(typeof(JsonBase));
            var jsonInterfaceSettingsExpression = Expression.Parameter(typeof(JsonInterfaceSettings));
            var valueExpression = Expression.Parameter(typeof(object));

            var expression = Expression.Lambda<Action<JsonBase, JsonInterfaceSettings, object>>(
                    Expression.Call(null, method,
                        jsonBaseParameterExpression,
                        propertyNameExpression,
                        Expression.Convert(valueExpression, type),
                        jsonInterfaceSettingsExpression),
                    jsonBaseParameterExpression,
                    jsonInterfaceSettingsExpression,
                    valueExpression
                    );

            return expression.Compile();
        }

        public JsonInterfacePropertyInterceptor(JsonInterfaceSettings settings)
        {
            if (IsFaulted) throw new Exception($"Fault creating facade object. \n{string.Join("\n", FaultMessages)}");

            var contract = (JsonObjectContract)settings.JsonSerializerSettings.ContractResolver.ResolveContract(typeof(T));

            ObjectPropertyNameToJsonPropertyName = contract.Properties.ToDictionary(v => v.UnderlyingName, v => v.PropertyName);
        }

        internal readonly Dictionary<string, string> ObjectPropertyNameToJsonPropertyName = new Dictionary<string, string>();

        public void Intercept(IInvocation invocation)
        {
            var jsonBase = (invocation.Proxy as JsonBase);

            try
            {
                if (Getters.TryGetValue(invocation.Method, out var getter))
                {
                    invocation.ReturnValue = getter(jsonBase, jsonBase.JsonInterfaceSettings);
                    return;
                }

                if (Setters.TryGetValue(invocation.Method, out var setter))
                {
                    setter(jsonBase, jsonBase.JsonInterfaceSettings, invocation.Arguments[0]);
                    return;
                }
            }
            catch (Exception ex)
            {
                var methodName = invocation.Method.Name;
                if (methodName.IndexOf("_") == 3)
                {
                    methodName = jsonBase.GetJsonPropertyNameFromPropertyName(methodName.Substring(4));
                }

                throw new JsonInterfaceException(ex.Message, jsonBase.JsonObject.Path + "." + methodName, ex);
            }

            switch (invocation.Method.Name)
            {
                case "set_" + nameof(IJsonObject.JsonObject):
                    var value = (JObject)invocation.Arguments[0];
                    jsonBase.JsonObject = value;
                    return;
                case "get_" + nameof(IJsonObject.JsonObject):
                    invocation.ReturnValue = jsonBase.JsonObject;
                    return;
            }
        }
    }

}
