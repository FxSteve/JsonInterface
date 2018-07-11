using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal static class HandlerFor<T>
    {
        static HandlerFor()
        {
            try
            {
                if (typeof(IJsonList).IsAssignableFrom(typeof(T)))
                {
                    var arrayType = typeof(T);
                    if (!arrayType.IsGenericType || arrayType.GetGenericTypeDefinition() != typeof(IJsonList<>))
                    {
                        throw new Exception($"All types that implement {typeof(IJsonList).Name} should also implement {typeof(IJsonList<>).Name.Replace("~1", "")}<T> also");
                    }
                    var arrayElementType = arrayType.GetGenericArguments()[0];

                    var handlers = GetHandlersForType(arrayElementType);

                    var elementReadJsonTypeHandler = handlers[0];
                    var elementWriteJsonTypeHandler = handlers[1];

                    // is array
                    ReadJsonTypeHandler = (IReadJsonTypeHandler<T>)Activator
                        .CreateInstance(typeof(ArrayTypeHandler<,>)
                        .MakeGenericType(arrayType, arrayElementType),
                        elementReadJsonTypeHandler,
                        elementWriteJsonTypeHandler
                        );
                    WriteJsonTypeHandler = new WritingNotSupportedTypeHandler<T>();
                }
                else if (typeof(IJsonObject).IsAssignableFrom(typeof(T)))
                {
                    // is object
                    ReadJsonTypeHandler = (IReadJsonTypeHandler<T>)Activator.CreateInstance(typeof(ObjectTypeHandler<>).MakeGenericType(typeof(T)));
                    WriteJsonTypeHandler = new WritingNotSupportedTypeHandler<T>();
                }
                else
                {
                    // use primitive handlers (verify elsewhere that this is a valid primitive type)
                    var typeHandler = new PrimitiveTypeHandler<T>();
                    ReadJsonTypeHandler = typeHandler;
                    WriteJsonTypeHandler = typeHandler;
                }
            }
            catch (Exception ex)
            {
                var faultException = new FaultedTypeHander<T>.TypeHandlerFaultException(ex.Message, ex);
                var typeHandler = new FaultedTypeHander<T>(faultException);
                ReadJsonTypeHandler = typeHandler;
                WriteJsonTypeHandler = typeHandler;
            }
        }

        private static object[] GetHandlersForType(Type type)
        {
            var handlerType = typeof(HandlerFor<>)
                 .MakeGenericType(type);

            var readHandler = handlerType.GetProperty(nameof(ReadJsonTypeHandler), BindingFlags.Static | BindingFlags.Public)
                .GetValue(null);

            var writeHandler = handlerType.GetProperty(nameof(WriteJsonTypeHandler), BindingFlags.Static | BindingFlags.Public)
                .GetValue(null);

            return new[] { readHandler, writeHandler };
        }

        public static IReadJsonTypeHandler<T> ReadJsonTypeHandler { get; }
        public static IWriteJsonTypeHandler<T> WriteJsonTypeHandler { get; }
        public static T GetPropertyValue(JObject jObject, string propertyName) => ReadJsonTypeHandler.GetPropertyValue(jObject, propertyName);
        public static void SetPropertyValue(JObject jObject, string propertyName, T value) => WriteJsonTypeHandler.SetPropertyValue(jObject, propertyName, value);
    }
}
