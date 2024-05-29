using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGenerator.Extensions
{
    internal static class StringExtensions
    {
        public static string RemovePeriods(this string value) => value.Replace(".", "");
    }
}
