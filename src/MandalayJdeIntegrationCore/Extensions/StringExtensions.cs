using System;

namespace MandalayJdeIntegrationCore.Extensions
{
    public static class StringExtensions
    {
        public static int? ToInteger(this string s)
        {
            if (Int32.TryParse(s, out int val))
            {
                return val;
            };
            return null;
        }

        public static long? ToLong(this string s)
        {
            if (Int64.TryParse(s, out long val))
            {
                return val;
            };
            return null;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s?.Trim());
        }

        public static bool FilterAll(this string s)
        {
            if (!string.IsNullOrEmpty(s?.Trim()))
            {
                return s.Trim().ToUpper() == "ALL";
            }
            return false;
        }

        public static T ToEnum<T>(this string value, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch(Exception ex)
            {
                return default;
            }
        }
    }
}
