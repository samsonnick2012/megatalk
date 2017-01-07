using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Innostar.UI.Helpers
{
    public static class Extensions
    {
        public static List<int> ToListNotEmpty(this IEnumerable<int> source)
        {
            if (!source.Any())
            {
                return new List<int>() { 0 };
            }
            return source.ToList();
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> source)
        {
            return source.ToDelimitedString(x => x.ToString(), ","
                /*CultureInfo.CurrentCulture.TextInfo.ListSeparator*/) + " ";
        }

        public static string ToDelimitedString<T>(
            this IEnumerable<T> source, Func<T, string> converter)
        {
            return source.ToDelimitedString(converter,
                CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> source, string separator)
        {
            return source.ToDelimitedString(x => x.ToString(), separator);
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> source,
            Func<T, string> converter, string separator)
        {
            return string.Join(separator, source.Select(converter).ToArray());
        }

        public static List<int> ToIntListFromDelimitedString(this string source, bool zeroIsNull)
        {
            if (source == null)
            {
                return new List<int>();
            }
            string[] sValues = source.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var values = sValues.Select(int.Parse).ToList();
            if (values.Count == 1 && values.First() == 0)
            {
                return new List<int>();
            }
            return values;
        }

        public static List<string> ToStringListFromDelimitedString(this string source, bool zeroIsNull)
        {
            if (source == null)
            {
                return new List<string>();
            }
            string[] sValues = source.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var values = sValues.ToList();
            if (values.Count == 1 && values.First() == "0")
            {
                return new List<string>();
            }
            return values;
        }
    }
}