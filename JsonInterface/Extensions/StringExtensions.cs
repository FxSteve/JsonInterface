using System;
using System.Collections.Generic;
using System.Text;

namespace JsonInterface.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string value) =>
            (value?.Length).GetValueOrDefault() > 1 ?
            char.ToLowerInvariant(value[0]) + value.Substring(1) :
            value;
    }
}