using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PbProject.WebCommon.Utility;
using PbProject.WebCommon.Utility.Encryption;
using PbProject.WebCommon.Utility.Encoding;

namespace PbProject.WebCommon.Web.Cookie
{
    public class SiteCookie
    {
        const string Domain = "51cbc.com";

        const string X_LOCAL_KEYNAME = "lk";
        const string X_LOCAL_KEY1 = "gHacMdbjVhY3fddh13kMndh6#@3ksdEkc7HcmdhhIgd4ndye";
        const string X_LOCAL_KEY2 = "h1jsdG2mNv8aBdhke6G7J3Jsmdjh&klpvd7NdpRgivcndOye";

        private PbProject.WebCommon.Utility.Encryption.RC4Encrypt _objEnc = new RC4Encrypt();
        private PbProject.WebCommon.Utility.Encryption.HashEncrypt _objHash = new HashEncrypt();

        public string _strError = "";

        private Hashtable CookieInfo = Hashtable.Synchronized(new Hashtable(100));

        public SiteCookie()
        {

        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string Encrypt(string strInput)
        {
            return _objEnc.Encrypt(strInput, X_LOCAL_KEY1, PbProject.WebCommon.Utility.Encryption.RC4Encrypt.EncoderMode.Base64Encoder);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string Decrypt(string strInput)
        {
            return _objEnc.Decrypt(strInput, X_LOCAL_KEY1, PbProject.WebCommon.Utility.Encryption.RC4Encrypt.EncoderMode.Base64Encoder);
        }

        /// <summary>
        /// 登陆密码加密
        /// </summary>
        /// <param name="dataStr">加密字符串</param>
        /// <returns>返回加密字符串</returns>
        public static string GetMD5(string dataStr)
        {
            string codeType = "utf-8";
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(codeType).GetBytes(dataStr));
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// SHA1Sign
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string SHA1Sign(string strInput)
        {
            return _objHash.SHA1Encrypt(strInput + X_LOCAL_KEY2);
        }

        /// <summary>
        /// 保存cookie
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveCookie(string key, string value)
        {
            string strBuffer = "";
            string strEncBuffer = "";
            string strSHA1Sign = "";

            strBuffer = value;

            strEncBuffer = Encrypt(strBuffer).Trim();
            strSHA1Sign = SHA1Sign(strEncBuffer);
            strEncBuffer = HttpUtility.UrlEncode(strEncBuffer);
            strEncBuffer = strSHA1Sign + strEncBuffer;
            HttpCookie cookie = new HttpCookie(key, strEncBuffer);
            cookie.Path = "/";
            cookie.Expires = DateTime.Now.AddYears(100);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <returns></returns>
        public string GetCookie(string key)
        {
            string strBuffer = "";
            string strEncBuffer = "";
            string strContent = "";
            string strSHA1Sign = "", strShA1Temp = "";

            try
            {
                strBuffer = HttpContext.Current.Request.Cookies.Get(key).Value;
                if (strBuffer.Length < 40)
                    return strContent;
                //  取出签名和密文
                strSHA1Sign = strBuffer.Substring(0, 40);
                strEncBuffer = strBuffer.Substring(40);
                //  签名校验
                strShA1Temp = SHA1Sign(HttpUtility.UrlDecode(strEncBuffer).Trim());
                if (strSHA1Sign != strShA1Temp)
                    return strContent;
                strEncBuffer = HttpUtility.UrlDecode(strEncBuffer);
                //  还原成明文
                strContent = Decrypt(strEncBuffer);
                if (strContent.Length == 0)
                    return strContent;

                return strContent;
            }
            catch
            {
                return strContent;
            }
        }
    }
}
