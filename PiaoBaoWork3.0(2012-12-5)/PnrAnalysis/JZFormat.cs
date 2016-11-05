using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PnrAnalysis
{
    /// <summary>
    /// 进制转化显示
    /// </summary>
    public class JZFormat
    {
        /// <summary>
        /// 普通字符串转换为十六进制字符串
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string strToHxString(string strData)
        {
            List<string> lstArr = new List<string>();
            if (!string.IsNullOrEmpty(strData))
            {
                char[] strChArr = strData.ToCharArray();
                foreach (char ch in strChArr)
                {
                    int value = Convert.ToInt32(ch);
                    lstArr.Add(string.Format("{0:X}", value));
                }
            }
            return string.Join(" ", lstArr.ToArray());
        }
        /// <summary>
        /// 十六进制字符串转换为普通字符串
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string strHxStringToString(string strData)
        {
            StringBuilder sbChar = new StringBuilder();
            if (!string.IsNullOrEmpty(strData))
            {
                string[] strChArr = strData.Split(' ');
                if (strChArr.Length > 0)
                {
                    List<string> lstArr = new List<string>();
                    foreach (string ch in strChArr)
                    {
                        sbChar.Append(Char.ConvertFromUtf32(Convert.ToInt32(ch, 16)));
                    }
                }
            }
            return sbChar.ToString();
        }
        /// <summary>
        /// 将含有汉字的字符串转换为Unicode编码
        /// </summary>
        /// <param name="str">要编码的汉字字符串</param>
        /// <returns>Unicode编码的的字符串</returns>
        public static string ToUnicode(string strDATA)
        {
            byte[] bts = Encoding.Unicode.GetBytes(strDATA);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2)
            {
                r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');
            }
            return r;
        }
        /// <summary>
        /// 将含有Unicode编码的字符串转换为普通字符串
        /// </summary>
        /// <param name="str">Unicode编码字符串</param>
        /// <returns>汉字字符串</returns>
        public static string ToGB2312(string strDATA)
        {
            MatchEvaluator me = new MatchEvaluator(delegate(Match m)
            {
                string strVal = "";
                if (m.Success)
                {
                    byte[] bts = new byte[2];
                    bts[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                    bts[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                    strVal = Encoding.Unicode.GetString(bts);
                }
                return strVal;
            });
            return Regex.Replace(strDATA, @"\\u([\w]{2})([\w]{2})", me, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

    }
}
