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

        internal static readonly Dictionary<MethodInfo, Func<JObject, object>> Getters = new Dictionary<MethodInfo, Func<JObject, object>>();
        private static readonly Dictionary<MethodInfo, Action<JObject, object>> Setters = new Dictionary<MethodInfo, Action<JObject, object>>();

        private static bool IsFaulted = false;
        public static readonly List<string> FaultMessages = new List<string>();

        private static void AddGetter(MethodInfo methodInfo, Func<JObject, object> getter)
        {
            if (methodInfo != null) Getters.Add(methodInfo, getter);
        }

        private static void AddSetter(MethodInfo methodInfo, Action<JObject, object> setter)
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
                    var propertyName = property.Name.ToCamelCase();
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

        public JsonInterfacePropertyInterceptor(JObject jObject)
        {
            if (IsFaulted) throw new Exception($"Fault creating facade object. \n{string.Join("\n", FaultMessages)}");

         var   _targetJObject = jObject ?? throw new ArgumentNullException(nameof(jObject));
        }

        private static Func<JObject, object> GetGetterFunc(string propertyName, Type type)
        {
            var genericTypeParameter = type;

            var method = typeof(HandlerFor<>)
                .MakeGenericType(genericTypeParameter)
                .GetMethod(nameof(HandlerFor<object>.GetPropertyValue), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            var propertyNameExpression = Expression.Constant(propertyName);
            var jObjectParameterExpression = Expression.Parameter(typeof(JObject));

            var expression = Expression.Lambda<Func<JObject, object>>(
                Expression.Convert(Expression.Call(null, method, jObjectParameterExpression, propertyNameExpression), typeof(object)),
                jObjectParameterExpression);

            var result = expression.Compile();
            return result;
        }

        private static Action<JObject, object> GetSetterFunc(string propertyName, Type type)
        {
            var method = typeof(HandlerFor<>)
                .MakeGenericType(type)
                .GetMethod(nameof(HandlerFor<object>.SetPropertyValue), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            var propertyNameExpression = Expression.Constant(propertyName);
            var jObjectParameterExpression = Expression.Parameter(typeof(JObject));
            var valueExpression = Expression.Parameter(typeof(object));

            var expression = Expression.Lambda<Action<JObject, object>>(
                Expression.Call(null, method, jObjectParameterExpression, propertyNameExpression, Expression.Convert(valueExpression, type)),
                jObjectParameterExpression,
                valueExpression);

            return expression.Compile();
        }

        public void Intercept(IInvocation invocation)
        {
            var targetJObject = (invocation.Proxy as JsonBase).JsonObject;

            if (Getters.TryGetValue(invocation.Method, out var getter))
            {
                invocation.ReturnValue = getter(targetJObject);
                return;
            }

            if (Setters.TryGetValue(invocation.Method, out var setter))
            {
                setter(targetJObject, invocation.Arguments[0]);
                return;
            }

            var methodName = invocation.Method.Name;
            switch (methodName)
            {
                case "set_" + nameof(IJsonObject.JsonObject):
                    var value = (JObject)invocation.Arguments[0];
                    (invocation.Proxy as JsonBase).JsonObject = value;
                    return;
                case "get_" + nameof(IJsonObject.JsonObject):
                    invocation.ReturnValue = targetJObject;
                    return;
            }
        }
    }

}
