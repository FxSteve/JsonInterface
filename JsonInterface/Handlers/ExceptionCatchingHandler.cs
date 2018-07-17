using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal class ExceptionCatchingHandler<T> : IReadJsonTypeHandler<T>, IWriteJsonTypeHandler<T>
    {
        IReadJsonTypeHandler<T> _readJsonTypeHandler;
        IWriteJsonTypeHandler<T> _writeJsonTypeHandler;

        public ExceptionCatchingHandler(IReadJsonTypeHandler<T> readJsonTypeHandler,
            IWriteJsonTypeHandler<T> writeJsonTypeHandler)
        {
            _readJsonTypeHandler = readJsonTypeHandler ?? throw new ArgumentNullException(nameof(readJsonTypeHandler));
            _writeJsonTypeHandler = writeJsonTypeHandler; // null is acceptable here
        }

        public T FromToken(JToken token, JsonInterfaceSettings settings) =>
            TrapOperation(() => _readJsonTypeHandler.FromToken(token, settings));

        public T GetPropertyValue(JsonBase jsonBase, string propertyName, JsonInterfaceSettings settings) =>
            TrapOperation(() => _readJsonTypeHandler.GetPropertyValue(jsonBase, propertyName, settings));

        public JToken ToToken(T value, JsonInterfaceSettings settings) =>
            TrapOperation(() => _readJsonTypeHandler.ToToken(value, settings));

        public void SetPropertyValue(JsonBase jsonBase, string propertyName, T value, JsonInterfaceSettings settings) =>
            TrapOperation(() => _writeJsonTypeHandler.SetPropertyValue(jsonBase, propertyName, value, settings));

        private TReturn TrapOperation<TReturn>(Func<TReturn> func)
        {
            try { return func(); }
            catch (Exception ex) { LastError = ex; }

            return default(TReturn);
        }

        private void TrapOperation(Action action)
        {
            try { action(); }
            catch (Exception ex) { LastError = ex; }
        }

        public void ThrowIfFaulted() { }

        public Func<T> DefaultValue { get; set; } = () => default(T);

        public Exception LastError { get; set; }
    }
}
