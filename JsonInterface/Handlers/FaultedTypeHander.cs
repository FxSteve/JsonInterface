using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    public class FaultedTypeHander<T> : IReadJsonTypeHandler<T>, IWriteJsonTypeHandler<T>
    {
        public class TypeHandlerFaultException : Exception
        {
            public TypeHandlerFaultException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        private readonly Exception _faultException;

        public FaultedTypeHander(Exception ex)
        {
            _faultException = ex ?? throw new Exception("No exception specified for fault exception");
        }
        public T FromToken(JToken token) => throw _faultException;

        public T GetPropertyValue(JObject jObject, string propertyName) => throw _faultException;

        public void SetPropertyValue(JObject jObject, string propertyName, T value) => throw _faultException;

        public JToken ToToken(T value) => throw _faultException;
    }
}
