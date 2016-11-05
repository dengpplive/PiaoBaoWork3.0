namespace DataBase.Configuration
{
    using DataBase;
    using DataBase.DbInterface;
    using System;
    using System.Runtime.InteropServices;

    public sealed class Settings
    {
        private static IDbHelperCreator dbHelperCreator;
        private static int decimalIntDigits = 0;
        private static int decimalPrecision = 14;
        private static int decimalScale = 4;
        private static string logModuleConnectionString = null;

        private Settings()
        {
        }

        private static bool GetConfig(string key, out bool value)
        {
            value = false;
            string str = AppSettings.GetString(key);
            if (!string.IsNullOrEmpty(str))
            {
                value = string.Equals(str, "true", StringComparison.OrdinalIgnoreCase);
                return true;
            }
            return false;
        }

        public static IDbHelperCreator DbHelperCreator
        {
            get
            {
                return dbHelperCreator;
            }
            set
            {
                dbHelperCreator = value;
            }
        }

        public static int DecimalIntDigits
        {
            get
            {
                return decimalIntDigits;
            }
            set
            {
                decimalIntDigits = value;
            }
        }

        public static int DecimalPrecision
        {
            get
            {
                return decimalPrecision;
            }
            set
            {
                decimalPrecision = value;
            }
        }

        public static int DecimalScale
        {
            get
            {
                return decimalScale;
            }
            set
            {
                decimalScale = value;
            }
        }

        public static string LogModuleConnectionString
        {
            get
            {
                return logModuleConnectionString;
            }
            set
            {
                logModuleConnectionString = value;
            }
        }

        internal static int MaxIntLength
        {
            get
            {
                if (decimalIntDigits > 0)
                {
                    return decimalIntDigits;
                }
                int num = DecimalPrecision - DecimalScale;
                if (num <= 0)
                {
                    return 0;
                }
                return num;
            }
        }
    }
}

