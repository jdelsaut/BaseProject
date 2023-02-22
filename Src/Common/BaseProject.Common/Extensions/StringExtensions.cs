using System.Globalization;

namespace BoxApi.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToStringWithDecimalSeparator(this double value, string separator)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalSeparator = ".";
            return value.ToString(nfi);
        }
    }
}
