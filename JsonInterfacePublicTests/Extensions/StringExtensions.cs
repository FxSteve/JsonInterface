using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonInterface.PublicTests
{
    public static class StringExtensions
    {
        public static string RemoveWhitespace(this string input) =>
            new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());

    }
}
