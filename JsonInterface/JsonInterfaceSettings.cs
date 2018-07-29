using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface
{
    /// <summary>
    /// Settings for JsonInterface
    /// </summary>
    public class JsonInterfaceSettings
    {
        private static readonly DefaultContractResolver _defaultContractResolver = new DefaultContractResolver();

        /// <summary>
        /// When true, trap exceptions instead of throwing on conversion failures
        /// resulting in non-compliant properties appearing null.
        /// When false, throw exceptions on conversion failures
        /// </summary>
        public bool TrapExceptions { get; set; } = false;

        /// <summary>
        /// JsonSerializerSettings to use when converting values
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = GetJsonSerializerSettings();

        private Type _baseType = typeof(JsonBase);

        /// <summary>
        /// Base type for proxies.  Must inherit from JsonBase
        /// </summary>
        public Type BaseType
        {
            get => _baseType;
            set
            {
                if (typeof(JsonBase).IsAssignableFrom(value))
                {
                    _baseType = value;
                }
                else
                {
                    throw new ArgumentException(nameof(BaseType), $"Type must inherit from {nameof(JsonBase)}");
                }
            }
        }

        /// <summary>
        /// Default JsonInterface settings
        /// </summary>
        public static JsonInterfaceSettings DefaultSettings => new JsonInterfaceSettings();

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            JsonSerializerSettings settings;

            if (JsonConvert.DefaultSettings == null)
            {
                settings = new JsonSerializerSettings();
            }
            else
            {
                settings = JsonConvert.DefaultSettings();
            }

            if (settings.ContractResolver == null) settings.ContractResolver = _defaultContractResolver;

            return settings;
        }
    }
}
