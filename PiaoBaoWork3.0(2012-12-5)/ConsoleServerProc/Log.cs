using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConsoleServerProc
{
    /// <summary>
    /// Log 的摘要说明。
    /// </summary>
    public class Log
    {
        //锁客户端对象
        public static object lockclient = new object();

        //锁服务端对象
        public static object lockserver = new object();

        //日志文件存储路径
        public static string filepath = "c:\\PidLog";

        public static void Record(string filename,System.Exception ex,string functionname)
        {
            //Kevin 2009-08-30 更改
            //在文件名前加日期和时间，以免日志文件过大
            string tmpdir = filepath+ "\\log";
            string tmpdate=DateTime.Now.ToString("yyyy-MM-dd HH");
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            tmpdir += "\\" + tmpdate.Substring(0, 10);
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            string file = tmpdir +"\\"+tmpdate+"-"+ filename;
            try
            {
                if(filename.ToLower()=="client.log")
                {
                    lock (lockclient)
                    {
                        System.IO.StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine("#######################################");
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        sw.WriteLine("出错函数名称：" + functionname);
                        sw.WriteLine("message:" + ex.Message);
                        sw.WriteLine("Source:" + ex.Source);
                        sw.WriteLine("StackTrace:" + ex.StackTrace);
                        sw.WriteLine("InnerException:" + ex.InnerException);
                        sw.WriteLine("#######################################");
                        sw.Close();
                    }
                }
                else
                {
                    lock(lockserver)
                    {
                        System.IO.StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine("#######################################");
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        sw.WriteLine("message:" + ex.Message);
                        sw.WriteLine("Source:" + ex.Source);
                        sw.WriteLine("StackTrace:" + ex.StackTrace);
                        sw.WriteLine("InnerException:" + ex.InnerException);
                        sw.WriteLine("#######################################");
                        sw.Close();
                    }
                }
            }
            catch
            {
            }
        }

        public static void Record(string filename,string head,byte[] buf,int count)
        {
            string tmpdir = filepath+ "\\log";
            string tmpdate = DateTime.Now.ToString("yyyy-MM-dd HH");
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            tmpdir += "\\" + tmpdate.Substring(0, 10);
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            string file = tmpdir + "\\" + tmpdate + "-" + filename;
            try
            {
                if(filename.ToLower()=="client.log")
                {
                    lock (lockclient)
                    {
                        System.IO.StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine("#######################################");
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                        string rstr = "";
                        for (int i = 0; i < count; i++)
                        {
                            if (i == 0)
                            {
                                rstr = "0x" + buf[i].ToString("X");
                            }
                            else
                            {
                                rstr += (", 0x" + buf[i].ToString("X"));
                            }
                        }

                        sw.WriteLine("message:" + head + rstr);
                        sw.WriteLine("#######################################");
                        sw.Close();
                    }
                }
                else
                {
                    lock (lockserver)
                    {
                        System.IO.StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine("#######################################");
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                        string rstr = "";
                        for (int i = 0; i < count; i++)
                        {
                            if (i == 0)
                            {
                                rstr = "0x" + buf[i].ToString("X");
                            }
                            else
                            {
                                rstr += (", 0x" + buf[i].ToString("X"));
                            }
                        }

                        sw.WriteLine("message:" + head + rstr);
                        sw.WriteLine("#######################################");
                        sw.Close();
                    }
                }
                
            }
            catch
            {
            }
        }

        public static void Record(string filename,string message)
        {
            string tmpdir = filepath + "\\log";
            string tmpdate = DateTime.Now.ToString("yyyy-MM-dd HH");
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            tmpdir += "\\" + tmpdate.Substring(0, 10);
            if (!Directory.Exists(tmpdir))
            {
                Directory.CreateDirectory(tmpdir);
            }

            string file = tmpdir + "\\" + tmpdate + "-" + filename;
            try
            {
                if(filename.ToLower()=="client.log")
                {
                    lock (lockclient)
                    {
                        System.IO.StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine("#######################################");
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        sw.WriteLine("message:" + message);
                        sw.WriteLine("#######################################");
                        sw.Close();
                    }
                }
                else
                {
                    lock (lockserver)
                    {
                        System.IO.StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine("#######################################");
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        sw.WriteLine("message:" + message);
                        sw.WriteLine("#######################################");
                        sw.Close();
                    }
                }

            }
            catch
            {
            }
        }
    }
}
