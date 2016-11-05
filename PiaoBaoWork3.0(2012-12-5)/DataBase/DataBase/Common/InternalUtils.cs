namespace DataBase.Common
{
    using DataBase;
    using DataBase.CustomException;
    using DataBase.Enums;
    using DataBase.LogCommon;
    using DataBase.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Web;

    internal static class InternalUtils
    {
        private static Hashtable assemblyInfoCache = Hashtable.Synchronized(new Hashtable());
        private static readonly string[] cMods;
        internal static readonly Encoding DefaultEncoding = Encoding.GetEncoding("gb2312");
        internal const string DefaultEncodingName = "gb2312";
        private static readonly long minModifiedTimeTicks;
        private static DateTime minVersionDateTime = new DateTime(0x7d0, 1, 1, 0, 0, 0);
        internal const string SysAssemblyName = "Carpa.Web";
        internal const string SysDir = "_Sys";

        static InternalUtils()
        {
            DateTime time = new DateTime(0x7d8, 5, 12, 0, 0, 0);
            minModifiedTimeTicks = time.Ticks;
            cMods = new string[] { "Ctrl", "Shift", "Alt" };
        }

        internal static void CheckDispose(object instance)
        {
            DbHelper.DisposeContextDbHelper();
        }

        internal static string DictionaryToQueryString(IDictionary<string, object> dict)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, object> pair in dict)
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }
                string key = pair.Key;
                if (!string.IsNullOrEmpty(key))
                {
                    builder.Append(HttpUtility.UrlEncodeUnicode(key)).Append('=');
                }
                if (pair.Value != null)
                {
                    builder.Append(HttpUtility.UrlEncodeUnicode(pair.Value.ToString()));
                }
            }
            return builder.ToString();
        }

        private static string ExtractConnectInfo(string connectionString)
        {
            if (connectionString.Contains("Data Source="))
            {
                string str4="";
                try
                {
                    object obj2;
                    object obj3;
                    DbConnectionStringBuilder builder = new DbConnectionStringBuilder {
                        ConnectionString = connectionString
                    };
                    if (!builder.TryGetValue("data source", out obj2) || !builder.TryGetValue("initial catalog", out obj3))
                    {
                        return connectionString.ToLower();
                    }
                    string str2 = (obj2 != null) ? obj2.ToString().ToLower() : null;
                    string str3 = (obj3 != null) ? obj3.ToString().ToLower() : null;
                    if (string.IsNullOrEmpty(str2) || string.IsNullOrEmpty(str3))
                    {
                        return connectionString.ToLower();
                    }
                    if ((str2 == "localhost") || (str2 == "127.0.0.1"))
                    {
                        str2 = "(local)";
                    }
                    str4 = str2 + "_" + str3;
                }
                catch (ArgumentException)
                {
                }
                return str4;
            }
            return connectionString.ToLower();
        }

        internal static DateTime GetAssemblyModifiedTime(Assembly assembly)
        {
            object obj2 = assemblyInfoCache[assembly];
            if (obj2 == null)
            {
                lock (assemblyInfoCache)
                {
                    obj2 = assemblyInfoCache[assembly];
                    if (obj2 == null)
                    {
                        DateTime time = LogAssemblyDateTime(assembly);
                        assemblyInfoCache[assembly] = time;
                        return time;
                    }
                    return (DateTime) obj2;
                }
            }
            return (DateTime) obj2;
        }

        internal static bool GetIsMessageException(Exception ex)
        {
            if (!(ex.GetType() == typeof(Exception)))
            {
                return (ex is MessageException);
            }
            return true;
        }

        internal static string GetPageParams(HttpRequest request)
        {
            string str = request.QueryString["__Params"];
            if (string.IsNullOrEmpty(str))
            {
                str = request.Form["__Params"];
            }
            return str;
        }

        internal static string GetProfileId(string profileKey)
        {
            if (!string.IsNullOrEmpty(profileKey))
            {
                profileKey = ExtractConnectInfo(profileKey);
                return StringUtils.GetHashCode(profileKey);
            }
            return null;
        }

        internal static string GetSessionVarStr(string varName)
        {
            if (string.IsNullOrEmpty(varName))
            {
                return null;
            }
            object obj2 = HttpContext.Current.Session[varName];
            if (obj2 == null)
            {
                return null;
            }
            return obj2.ToString();
        }

        internal static int GetVersionTag(DateTime modifiedTime)
        {
            return (int) ((modifiedTime.Ticks - minModifiedTimeTicks) / 0x989680);
        }

        internal static void InitSqlDebug(ref bool isDebuggingSql, ref bool isNoLoggingSql)
        {
            InitSqlDebug(HttpContext.Current, ref isDebuggingSql, ref isNoLoggingSql);
        }

        private static void InitSqlDebug(HttpContext context, ref bool isDebuggingSql, ref bool isNoLoggingSql)
        {
            if ((context != null) && (context.Session != null))
            {
                isDebuggingSql = context.Session["__DebugSql"] != null;
                isNoLoggingSql = context.Session["__NoLogSql"] != null;
            }
        }

        internal static bool IsDebuggingSql(HttpContext context)
        {
            bool isDebuggingSql = false;
            bool isNoLoggingSql = false;
            InitSqlDebug(context, ref isDebuggingSql, ref isNoLoggingSql);
            return isDebuggingSql;
        }

        internal static bool IsRequestNotModified(HttpContext context, DateTime lastModifiedTime)
        {
            string s = context.Request.Headers["If-Modified-Since"];
            if (s != null)
            {
                DateTime time;
                int index = s.IndexOf(';');
                if (index > 0)
                {
                    s = s.Substring(0, index);
                }
                if (DateTime.TryParse(s, out time) && (time >= lastModifiedTime))
                {
                    context.Response.StatusCode = 0x130;
                    return true;
                }
            }
            return false;
        }

        internal static DateTime LogAssemblyDateTime(Assembly assembly)
        {
            AssemblyName name = assembly.GetName();
            DateTime lastWriteTime = File.GetLastWriteTime(new Uri(name.CodeBase).LocalPath);
            DateTime time2 = new DateTime(lastWriteTime.Year, lastWriteTime.Month, lastWriteTime.Day, lastWriteTime.Hour, lastWriteTime.Minute, lastWriteTime.Second);
            string str = "文件时间 " + time2.ToString("yyyy-MM-dd HH:mm:ss");
            int build = name.Version.Build;
            if (build >= 0xc8a)
            {
                time2 = minVersionDateTime.AddDays((double) build).AddSeconds((double) (name.Version.Revision * 2));
                str = string.Concat(new object[] { "版本 ", name.Version, "，内部时间 ", time2.ToString("yyyy-MM-dd HH:mm:ss"), "，", str });
            }
            Log.Info(name.Name + " " + str);
            return time2;
        }

        internal static void LogError(string message, Exception ex, HttpRequest request)
        {
            Log.Error(request.UserHostAddress + " " + message + " " + request.Url.OriginalString + ((request.UrlReferrer != null) ? (" " + request.UrlReferrer.AbsoluteUri) : null), ex);
        }

        internal static bool NeedLog(Exception ex)
        {
            return !GetIsMessageException(ex);
        }

        internal static void SetCacheLastModified(HttpContext context, DateTime lastModifiedTime, bool neverExpires)
        {
            if (lastModifiedTime.ToUniversalTime() < DateTime.UtcNow)
            {
                HttpCachePolicy cache = context.Response.Cache;
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetLastModified(lastModifiedTime);
                if (neverExpires)
                {
                    cache.SetExpires(DateTime.Now.AddYears(1));
                    cache.SetValidUntilExpires(true);
                }
            }
        }

        internal static string ShortcutToString(Shortcut shortcut)
        {
            string template = shortcut.ToString();
            for (int i = 0; i < cMods.Length; i++)
            {
                string placeholder = cMods[i];
                template = StringUtils.ReplaceFirst(template, placeholder, placeholder + "+", StringComparison.OrdinalIgnoreCase);
            }
            return template;
        }
    }
}

