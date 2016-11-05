using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis
{
    /// <summary>
    /// 文本日志
    /// </summary>
    public class LogText
    {
        public static object root = new object();
        public static void LogWrite(string Con, string dir)
        {
            lock (root)
            {
                try
                {
                    if (string.IsNullOrEmpty(Con))
                    {
                        Con = "日志数据为空";
                    }
                    string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log";
                    if (!string.IsNullOrEmpty(dir))
                    {
                        path = path + "\\" + dir.Trim(new char[] { '\\' });
                    }
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    if (!path.EndsWith("\\"))
                    {
                        path = path + "\\";
                    }
                    System.IO.File.AppendAllText(path + System.DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt", Con);
                }
                catch
                {
                }
            }
        }
    }
}
