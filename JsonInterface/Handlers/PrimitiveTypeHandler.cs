using System;
using System.Collections.Generic;
using System.Text;
using JsonInterface.Extensions;
using Newtonsoft.Json.Linq;

namespace JsonInterface.Handlers
{
    internal class PrimitiveTypeHandler<T> : IReadJsonTypeHandler<T>, IWriteJsonTypeHandler<T>
    {
        private readonly T nullValue = default(T);
        private readonly static bool IsFaulted = false;
        private readonly static Exception FaultException;

        static PrimitiveTypeHandler()
        {
            if (default(T) != null)
            {
                // throw an exception if the type used is not nullable
                // there is not a constraint that permits class+nullable but
                // rejects non-nullables, so this performs the restriction
                // at runtime
                IsFaulted = true;
                FaultException = new ArgumentException($"Generic Argument T must be a nullable type.");
            }
        }

        public T FromToken(JToken token)
        {
            if (IsFaulted) throw FaultException;

            return token == null ? nullValue : token.ToObject<T>();
        }

        public T GetPropertyValue(JObject jObject, string propertyName) =>
            FromToken(jObject[propertyName.ToCamelCase()]);

        public void SetPropertyValue(JObject jObject, string propertyName, T value) =>
            jObject.ForceGetValuePropertyToken(propertyName.ToCamelCase()).Value = value;

        public JToken ToToken(T value) =>
            new JValue(value);
    }
}
