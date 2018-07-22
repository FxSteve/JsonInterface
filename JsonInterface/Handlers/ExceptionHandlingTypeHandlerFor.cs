using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface.Handlers
{
    internal class ExceptionHandlingTypeHandlerFor<T>
    {
        static readonly JsonInterfaceSettings jsonInterfaceSettingsNoTrap = new JsonInterfaceSettings
        {
            TrapExceptions = false
        };

        static readonly IReadJsonTypeHandler<T> _readJsonTypeHandler = HandlerFor<T>.GetReadJsonTypeHandler(jsonInterfaceSettingsNoTrap);
        static readonly IWriteJsonTypeHandler<T> _writeJsonTypeHandler = HandlerFor<T>.GetWriteJsonTypeHandler(jsonInterfaceSettingsNoTrap);

        static readonly ExceptionCatchingHandler<T> _exceptionCatchingHandler =
            new ExceptionCatchingHandler<T>(_readJsonTypeHandler, _writeJsonTypeHandler);

        public static IReadJsonTypeHandler<T> ReadJsonTypeHandler { get; } = _exceptionCatchingHandler;
        public static IWriteJsonTypeHandler<T> WriteJsonTypeHandler { get; } = _exceptionCatchingHandler;

        public static T GetPropertyValue(JsonBase jsonBase, string propertyName) => ReadJsonTypeHandler.GetPropertyValue(jsonBase, propertyName);
        public static void SetPropertyValue(JsonBase jsonBase, string propertyName, T value) => WriteJsonTypeHandler.SetPropertyValue(jsonBase, propertyName, value);

    }
}
