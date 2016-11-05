using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
namespace PbProject.WebCommon.Log
{
    /// <summary>
    /// 操作日志记录
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 记录日志(记事本)
        /// </summary>
        /// <param name="PageName">页面名</param>
        /// <param name="logContents">日志内容</param>
        /// <param name="bRecordRequest">是否记录参数</param>
        /// <param name="request">请求值</param>
        public static void RecordLog(string PageName, string logContents, bool bRecordRequest, HttpRequest request)
        {
            StreamWriter fs = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 记录文本日志

                sb.AppendFormat("记录时间：" + DateTime.Now.ToString() + "\r\n");
                sb.AppendFormat("      IP：" + System.Web.HttpContext.Current.Request.UserHostAddress + "\r\n");
                sb.AppendFormat("内    容: " + logContents + "\r\n");
                if (bRecordRequest)
                {
                    #region 记录 Request 参数
                    try
                    {
                        sb.AppendFormat("  Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");
                        if (HttpContext.Current.Request != null)
                        {
                            if (HttpContext.Current.Request.HttpMethod == "POST")
                            {
                                #region POST 提交
                                if (HttpContext.Current.Request.Form.Count != 0)
                                {
                                    //__VIEWSTATE
                                    //__EVENTVALIDATION 
                                    System.Collections.Specialized.NameValueCollection nv = HttpContext.Current.Request.Form;
                                    foreach (string key in nv.Keys)
                                    {
                                        if (key == "__VIEWSTATE" || key == "__EVENTVALIDATION")
                                        {
                                            continue;
                                        }
                                        sb.AppendFormat("{0} ={1} \r\n", key, (nv[key] != null ? nv[key].ToString() : ""));
                                    }
                                }
                                else
                                {
                                    sb.AppendFormat(" HttpContext.Current.Request.Form.Count = 0 \r\n");
                                }

                                #endregion
                            }
                            else if (HttpContext.Current.Request.HttpMethod == "GET")
                            {
                                #region GET 提交

                                if (HttpContext.Current.Request.QueryString.Count != 0)
                                {
                                    System.Collections.Specialized.NameValueCollection nv = HttpContext.Current.Request.QueryString;
                                    foreach (string key in nv.Keys)
                                    {
                                        sb.AppendFormat("{0}={1} \r\n", key, nv[key]);
                                    }
                                }
                                else
                                {
                                    sb.AppendFormat(" HttpContext.Current.QueryString.Form.Count = 0 \r\n");
                                }

                                #endregion
                            }
                            else
                            {
                                #region 不是 GET 和 POST

                                sb.AppendFormat("  不是 GET 和 POST, Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");

                                System.Collections.Specialized.NameValueCollection nv = request.Params;
                                foreach (string key in nv.Keys)
                                {
                                    if (key == "__VIEWSTATE" || key == "__EVENTVALIDATION")
                                    {
                                        continue;
                                    }
                                    sb.AppendFormat("{0}={1} \r\n", key, nv[key]);
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            sb.AppendFormat("  HttpContext.Current.Request=null \r\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendFormat("  异常内容: " + ex + "\r\n");
                        sb.AppendFormat("----------------------------------------------------------------------------------------------------\r\n\r\n");
                        AgainWrite(sb, PageName);
                    }

                    #endregion
                }
                sb.AppendFormat("----------------------------------------------------------------------------------------------------\r\n\r\n");
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logs\\" + PageName + "\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
                fs.WriteLine(sb.ToString());

                #endregion
            }
            catch (Exception ex)
            {

                AgainWrite(sb, PageName);
                throw ex;

            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

        }

        /// <summary>
        /// 再次记录
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="PageName"></param>
        protected static void AgainWrite(StringBuilder sb, string PageName)
        {
            StreamWriter fs = null;
            try
            {
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\" + PageName + "\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + "again" + ".txt", true, System.Text.Encoding.Default);
                fs.WriteLine(sb.ToString());
            }
            catch (Exception)
            {

            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
    }
}
