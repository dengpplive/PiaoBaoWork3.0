using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Cache;
namespace PbProject.WebCommon.Utility
{
    public class HttpSend
    {
        /// <summary>
        /// 发送请求数据
        /// </summary>
        /// <param name="target">请求Url</param>
        /// <param name="method">GET/POST</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeOut">请求超时（秒）</param>
        /// <param name="parameters">请求参数key=value</param>
        /// <returns></returns>
        public string SendRequest(string target, string method, System.Text.Encoding encoding, int timeOut, params string[] parameters)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Stream responseStream = null;
            string result = string.Empty;
            try
            {
                string str = "";
                if ((parameters != null) && (parameters.Length >= 1))
                {
                    str = string.Join("&", parameters);
                }
                byte[] bytes = encoding.GetBytes(str);
                string requestUriString = target;
                request = (HttpWebRequest)WebRequest.Create(requestUriString);
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.CachePolicy = policy;
                request.Timeout = timeOut * 0x3e8;
                request.KeepAlive = false;
                request.Method = method.ToString().ToUpper();
                bool flag = false;
                flag = request.Method.ToUpper() == "POST";
                if (flag)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;
                }
                else
                {
                    if (target.Contains("?"))
                    {
                        target = target.Trim(new char[] { '&' }) + "&" + str;
                    }
                    else
                    {
                        target = target.Trim(new char[] { '?' }) + "?" + str;
                    }
                }
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
                request.Headers.Add("Cache-Control", "no-cache");
                request.Accept = "*/*";
                request.Credentials = CredentialCache.DefaultCredentials;
                if (flag)
                {
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                List<byte> list = new List<byte>();
                for (int i = responseStream.ReadByte(); i != -1; i = responseStream.ReadByte())
                {
                    list.Add((byte)i);
                }
                Stream stream2 = new MemoryStream(list.ToArray());
                StreamReader sr = new StreamReader(stream2, encoding);
                result = sr.ReadToEnd();
                sr.Close();
                stream2.Close();
            }
            catch (WebException ex)
            {
            }
            catch (Exception ex1)
            {
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }
    }
}
