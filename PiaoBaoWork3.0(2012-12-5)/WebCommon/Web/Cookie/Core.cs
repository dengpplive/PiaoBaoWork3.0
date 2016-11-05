using System;
using System.Web;
using PbProject.WebCommon.Utility.Encryption;

namespace PbProject.WebCommon.Web.Cookie
{
    public class Core
    {
        const string X_SIGN_KEYNAME_S1 = "sk1";
        const string X_SIGN_KEYNAME_S2 = "sk2";

        const string X_SIGN_KEY_S1 = "gHacMdbjVhY3fNdh13kMndh6#@3klpOkc7HcmdjhIgd4ndye";
        const string X_SIGN_KEY_S2 = "h1jsdG2mNv8aBdhke6G7J3Jsmdjh&klpvd7NdpRgivcndOye";

        const string X_DOMAINNAME = "51cbc.com";

        private RC4Encrypt _objEnc = new RC4Encrypt();
        private HashEncrypt _objHash = new HashEncrypt();

        public string _strError = "";

        public CookieUserInfo UserInfoFromCookie = null;

        public HttpContext Context { get; set; }

        public HttpRequest Request { get { return Context != null ? Context.Request : null; } }
        public HttpResponse Response { get { return Context != null ? Context.Response : null; } }

        public string Domain { get; set; }

        public Core()
        {
            this.UserInfoFromCookie = new CookieUserInfo();
            this.Domain = "";
        }

        private void AppendCookieItem(string strInput, ref string strBuffer)
        {
            if (strBuffer.Length == 0)
                strBuffer = strInput;
            else
                strBuffer = strBuffer + ";" + strInput;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string Encrypt(string strInput)
        {
            return _objEnc.Encrypt(strInput, X_SIGN_KEY_S1, Utility.Encryption.RC4Encrypt.EncoderMode.Base64Encoder);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string Decrypt(string strInput)
        {
            return _objEnc.Decrypt(strInput, X_SIGN_KEY_S1, Utility.Encryption.RC4Encrypt.EncoderMode.Base64Encoder);
        }

        private string SHA1Sign(string strInput)
        {
            return _objHash.SHA1Encrypt(strInput + X_SIGN_KEY_S2);
        }

        /// <summary>
        /// 检验Sign Cookie是否有效
        /// </summary>
        /// <returns>True=SignCookie有效</returns>
        public bool CheckSignCookie()
        {
            string strBuffer = "";
            string strEncBuffer = "";
            string strContent = "";
            string strSHA1Sign = "", strShA1Temp = "";

            try
            {
                if (Context == null)
                    return false;

                strBuffer = Request.Cookies.Get(X_SIGN_KEYNAME_S1).Value;
                if (strBuffer.Length < 40)
                    return false;
                //  取出签名和密文
                strSHA1Sign = strBuffer.Substring(0, 40);
                strEncBuffer = strBuffer.Substring(40);
                //  签名校验
                strShA1Temp = SHA1Sign(HttpUtility.UrlDecode(strEncBuffer).Trim());
                if (strSHA1Sign != strShA1Temp)
                    return false;
                strEncBuffer = HttpUtility.UrlDecode(strEncBuffer);
                //  还原成明文
                strContent = Decrypt(strEncBuffer);
                if (strContent.Length == 0)
                    return false;
                //  从明文中取出各个字段值
                char[] delimiterChars = { ';' };
                string[] arTemp = strContent.Split(delimiterChars);
                if (arTemp.Length >= 1)
                {
                    this.UserInfoFromCookie.ID = Convert.ToInt32(arTemp[0]);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void RegistContext(HttpContext context)
        {
            if (context == null)
                throw new Exception("HttpContext cannot be null.");

            Context = context;
            if (this.Context.Application["Domain"] != null)
            {
                this.Domain = this.Context.Application["Domain"].ToString().Trim();
            }
        }

        /// <summary>
        /// 写入Sign Cookie
        /// </summary>
        public void SaveSignCookie()
        {
            string strBuffer = "";
            string strEncBuffer = "";
            string strSHA1Sign = "";

            if (Context == null)
                return ;

            AppendCookieItem(this.UserInfoFromCookie.ID.ToString(), ref strBuffer);

            strEncBuffer = Encrypt(strBuffer).Trim();
            strSHA1Sign = SHA1Sign(strEncBuffer);
            strEncBuffer = HttpUtility.UrlEncode(strEncBuffer);
            strEncBuffer = strSHA1Sign + strEncBuffer;
            HttpCookie cookie = new HttpCookie(X_SIGN_KEYNAME_S1, strEncBuffer);
            //cookie.Domain = Domain;
            Response.Cookies.Add(cookie);
        }

        public void ClearSignCookie()
        {
            if (Context == null)
                return;

            HttpCookie cookie = new HttpCookie(X_SIGN_KEYNAME_S1, "");
            cookie.Expires = DateTime.Now.AddDays(-1d);
            cookie.Domain = X_DOMAINNAME;
            Response.Cookies.Add(cookie);
        }
    }
}
