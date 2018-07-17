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

        public T FromToken(JToken token, JsonBase jsonBase) =>
            TrapOperation(() => _readJsonTypeHandler.FromToken(token, jsonBase));

        public T GetPropertyValue(JsonBase jsonBase, string propertyName) =>
            TrapOperation(() => _readJsonTypeHandler.GetPropertyValue(jsonBase, propertyName));

        public JToken ToToken(T value, JsonBase jsonBase) =>
            TrapOperation(() => _readJsonTypeHandler.ToToken(value, jsonBase));

        public void SetPropertyValue(JsonBase jsonBase, string propertyName, T value ) =>
            TrapOperation(() => _writeJsonTypeHandler.SetPropertyValue(jsonBase, propertyName, value));

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
