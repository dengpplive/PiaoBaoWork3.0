namespace DataBase.LogCommon
{
    using log4net;
    using log4net.Config;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class Log
    {
        private static bool initialized = false;
        private static readonly ILog log = LogManager.GetLogger(typeof(DataBase.LogCommon.Log));

        static Log()
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }
        }

        public static void Debug(object message)
        {
            log.Debug(message);
        }

        public static void Debug(object message, Exception exception)
        {
            log.Debug(message, exception);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        public static void Error(object message)
        {
            log.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            log.ErrorFormat(format, args);
        }

        public static void Fatal(object message)
        {
            log.Fatal(message);
        }

        public static void Fatal(object message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            log.FatalFormat(format, args);
        }

        public static void Info(object message)
        {
            log.Info(message);
        }

        public static void Info(object message, Exception exception)
        {
            log.Info(message, exception);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            log.InfoFormat(format, args);
        }

        private static void Initialize()
        {
            FileInfo configFile = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            if (configFile.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(configFile);
            }
            if (!LogManager.GetRepository().Configured)
            {
                configFile = new FileInfo(Path.ChangeExtension(new Uri(Assembly.GetCallingAssembly().GetName().CodeBase).LocalPath, ".config"));
                if (configFile.Exists)
                {
                    XmlConfigurator.ConfigureAndWatch(configFile);
                }
                else
                {
                    Trace.TraceWarning("配置文件 {0} 不存在", new object[] { configFile.FullName });
                }
            }
        }

        public static void Warn(object message)
        {
            log.Warn(message);
        }

        public static void Warn(object message, Exception exception)
        {
            log.Warn(message, exception);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            log.WarnFormat(format, args);
        }

        public static bool IsDebugEnabled
        {
            get
            {
                return log.IsDebugEnabled;
            }
        }

        public static bool IsErrorEnabled
        {
            get
            {
                return log.IsErrorEnabled;
            }
        }

        public static bool IsFatalEnabled
        {
            get
            {
                return log.IsFatalEnabled;
            }
        }

        public static bool IsInfoEnabled
        {
            get
            {
                return log.IsInfoEnabled;
            }
        }

        public static bool IsWarnEnabled
        {
            get
            {
                return log.IsWarnEnabled;
            }
        }
    }
}

