using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PbProject.WebCommon.Utility
{
    /// <summary>
    /// 公共静态方法
    /// </summary>
    public class CommonManage
    {
        /// <summary>
        /// 清除SQL特殊字符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TrimSQL(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                data = "";
            }
            else
            {
                string[] strChar = new string[]{
                "'"
                };
                foreach (string item in strChar)
                {
                    data = data.Replace(item, "");
                }
            }
            return data;
        }
        /// <summary>
        /// 字符转换到&特殊符号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ReplaceXMLToChar(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                data = "";
            }
            else
            {
                data = data.Replace(" ", "&nbsp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;");
            }
            return data;
        }
        /// <summary>
        /// &特殊符号换到字符转
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ReplaceCharToXML(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                data = "";
            }
            else
            {
                data = data.Replace("&nbsp;", " ").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&apos;", "'").Replace("&quot;", "\"");
            }
            return data;
        }

        /// <summary>
        /// 移除标记
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string StripHTML(string strHtml)
        {
            string TagName = "[A-Za-z]{1,10}";
            string StartPattern = string.Format(@"<{0}[^>]*?>", TagName);
            string EndPattern = string.Format(@"</{0}>", TagName);

            strHtml = Regex.Replace(strHtml.Replace("\r", "").Replace("\n", ""), StartPattern, "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            strHtml = Regex.Replace(strHtml.Replace("\r", "").Replace("\n", ""), EndPattern, "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            string pattern = string.Format(@"<{0}[^>]*\/>", TagName);
            strHtml = Regex.Replace(strHtml.Replace("\r", "").Replace("\n", ""), pattern, "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return strHtml;
        }


        /// <summary>
        /// 简单编码 用于js解码函数 unescape()
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Escape(string s)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byteArr = System.Text.Encoding.Unicode.GetBytes(s);

            for (int i = 0; i < byteArr.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(byteArr[i + 1].ToString("X2"));//把字节转换为十六进制的字符串表现形式
                sb.Append(byteArr[i].ToString("X2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 字符串用10进制显示
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public string strToOct(string strData)
        {
            if (!string.IsNullOrEmpty(strData))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strData);
                List<string> listArr = new List<string>();
                foreach (byte ch in bytes)
                {
                    listArr.Add(ch.ToString());
                }
                strData = string.Join(" ", listArr.ToArray());
            }
            return strData;
        }
        /// <summary>
        /// 10进制字符串还原显示
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public string octTostr(string strData)
        {
            if (!string.IsNullOrEmpty(strData))
            {
                string[] strArray = strData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                List<string> list = new List<string>();
                list.AddRange(strArray);
                if (list.Count > 0)
                {
                    byte[] buffer = new byte[list.Count];
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        buffer[j] = byte.Parse(list[j].ToString());
                    }
                    strData = System.Text.Encoding.UTF8.GetString(buffer);
                }
            }
            return strData;
        }
    }
}
