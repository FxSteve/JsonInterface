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

        public static bool IsFaulted { get; set; } = false;
        public void ThrowIfFaulted() { if (IsFaulted) throw FaultException; }

        public T FromToken(JToken token, JsonInterfaceSettings settings)
        {
            if (IsFaulted) throw FaultException;

            return token == null ? nullValue : token.ToObject<T>();
        }

        public T GetPropertyValue(JsonBase jsonBase, string propertyName, JsonInterfaceSettings settings) =>
            FromToken(jsonBase.JsonObject[jsonBase.GetJsonPropertyNameFromPropertyName(propertyName)], settings);

        public void SetPropertyValue(JsonBase jsonBase, string propertyName, T value, JsonInterfaceSettings settings) =>
            jsonBase.ForceGetValuePropertyToken(propertyName).Value = value;

        public JToken ToToken(T value, JsonInterfaceSettings settings) =>
            new JValue(value);
    }
}
