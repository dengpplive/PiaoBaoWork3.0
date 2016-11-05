namespace DataBase
{
    using DataBase.Utils;
    using System;
    using System.Configuration;

    public static class AppSettings
    {
        public static string DecryptGetString(string keyName)
        {
            string hex = GetString(keyName);
            if (hex == null)
            {
                return string.Empty;
            }
            return StringUtils.Decrypt(hex);
        }

        public static bool GetBool(string keyName)
        {
            return GetBool(keyName, false);
        }

        public static bool GetBool(string keyName, bool defaultValue)
        {
            string str = GetString(keyName);
            if (!string.IsNullOrEmpty(str))
            {
                return bool.Parse(str);
            }
            return defaultValue;
        }

        public static DateTime GetDateTime(string keyName)
        {
            return GetDateTime(keyName, DateTime.MinValue);
        }

        public static DateTime GetDateTime(string keyName, DateTime defaultValue)
        {
            string str = GetString(keyName);
            if (!string.IsNullOrEmpty(str))
            {
                return DateTime.Parse(str);
            }
            return defaultValue;
        }

        public static double GetDouble(string keyName)
        {
            return GetDouble(keyName, 0.0);
        }

        public static double GetDouble(string keyName, double defaultValue)
        {
            string str = GetString(keyName);
            if (!string.IsNullOrEmpty(str))
            {
                return double.Parse(str);
            }
            return defaultValue;
        }

        public static int GetInt(string keyName)
        {
            return GetInt(keyName, 0);
        }

        public static int GetInt(string keyName, int defaultValue)
        {
            string str = GetString(keyName);
            if (!string.IsNullOrEmpty(str))
            {
                return int.Parse(str);
            }
            return defaultValue;
        }

        public static string GetString(string keyName)
        {
            string str = ConfigurationManager.AppSettings[keyName];
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str;
        }

        public static string GetString(string keyName, string defaultValue)
        {
            string str = GetString(keyName);
            if (str == null)
            {
                return defaultValue;
            }
            return str;
        }
    }
}

