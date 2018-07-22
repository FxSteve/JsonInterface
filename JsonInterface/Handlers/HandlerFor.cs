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

                    // is array
                    _readJsonTypeHandler = (IReadJsonTypeHandler<T>)Activator
                        .CreateInstance(typeof(ArrayTypeHandler<,>)
                        .MakeGenericType(arrayType, arrayElementType)
                        );

                    _writeJsonTypeHandler = new WritingNotSupportedTypeHandler<T>();
                }
                else if (typeof(IJsonObject).IsAssignableFrom(typeof(T)))
                {
                    // is object
                    _readJsonTypeHandler = (IReadJsonTypeHandler<T>)Activator.CreateInstance(typeof(ObjectTypeHandler<>).MakeGenericType(typeof(T)));
                    _writeJsonTypeHandler = new WritingNotSupportedTypeHandler<T>();
                }
                else
                {
                    // use primitive handlers (verify elsewhere that this is a valid primitive type)
                    var typeHandler = new PrimitiveTypeHandler<T>();
                    _readJsonTypeHandler = typeHandler;
                    _writeJsonTypeHandler = typeHandler;
                }
            }
            catch (Exception ex)
            {
                var faultException = new FaultedTypeHander<T>.TypeHandlerFaultException(ex.Message, ex);
                var typeHandler = new FaultedTypeHander<T>(faultException);

                _readJsonTypeHandler = typeHandler;
                _writeJsonTypeHandler = typeHandler;
            }

            _exceptionCatchingHandler = new ExceptionCatchingHandler<T>(
                _readJsonTypeHandler,
                _writeJsonTypeHandler);
        }

        private static ExceptionCatchingHandler<T> _exceptionCatchingHandler;
        private static IReadJsonTypeHandler<T> _readJsonTypeHandler;
        private static IWriteJsonTypeHandler<T> _writeJsonTypeHandler;

        public static IReadJsonTypeHandler<T> GetReadJsonTypeHandler(JsonInterfaceSettings settings) =>
            settings.TrapExceptions ? _exceptionCatchingHandler : _readJsonTypeHandler;

        public static IWriteJsonTypeHandler<T> GetWriteJsonTypeHandler(JsonInterfaceSettings settings) =>
            settings.TrapExceptions ? _exceptionCatchingHandler : _writeJsonTypeHandler;

        public static T GetPropertyValue(JsonBase jsonBase, string propertyName) =>
            GetReadJsonTypeHandler(jsonBase.JsonInterfaceSettings).GetPropertyValue(jsonBase, propertyName);

        public static void SetPropertyValue(JsonBase jsonBase, string propertyName, T value) =>
            GetWriteJsonTypeHandler(jsonBase.JsonInterfaceSettings).SetPropertyValue(jsonBase, propertyName, value);
    }
}
