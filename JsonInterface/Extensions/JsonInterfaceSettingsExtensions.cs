using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface.Extensions
{
    internal static class JsonInterfaceSettingsExtensions
    {
        internal static JsonInterfaceSettings DefaultIfNull(this JsonInterfaceSettings settings) =>
            settings ?? JsonInterfaceSettings.DefaultSettings;
    }
}
