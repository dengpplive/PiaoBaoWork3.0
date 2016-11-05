using System;
using System.Web;
using PbProject.WebCommon.Utility;

namespace PbProject.WebCommon.Web.Cookie
{
    public class SignPage : System.Web.UI.Page , ISignHandler
    {
        private bool _Init = false;
        private Core _Core = null;

        public CookieUserInfo UserInfoFromCookie { get { return _Core != null ? _Core.UserInfoFromCookie : null; } set { if (_Core != null) _Core.UserInfoFromCookie = value; } }

        public string Domain { get { return _Core != null ? _Core.Domain : ""; } set {  if (_Core != null) _Core.Domain = value; } }
        
        public SignPage()
        {
            InitConfig();
        }

        public string RemoteIP 
        {
            get
            {
                string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(result))
                {
                    //可能有代理 
                    if (result.IndexOf(".") != -1)     //没有“.”肯定是非IPv4格式 
                    {
                        if (result.IndexOf(",") != -1)
                        {
                            //有“,”，可能存在多个代理。取第一个不是内网的IP。 
                            result = result.Replace(" ", "").Replace("'", "");
                            string[] temparyip = result.Split(new Char[] { ',', ';' });
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                if (IPUtility.IsIPAddress(temparyip[i]) && !IPUtility.IsInnerIP(temparyip[i]))
                                {
                                    return temparyip[i];     //找到不是内网的地址 
                                }
                            }
                        }
                        else if (IPUtility.IsIPAddress(result)) //代理即是IP格式 
                            return result;
                    }
                }

                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.UserHostAddress;

                return result;
            }
        }

        protected void InitConfig()
        {
            if (_Init)
                return;

            try
            {
                _Core = new Core();
                _Core.RegistContext(this.Context);

                _Init = true;
            }
            catch { }
            finally { }

        }

        public void SaveSignCookie()
        {
            _Core.SaveSignCookie();
        }

        public void ClearSignCookie()
        {
            _Core.ClearSignCookie();
        }

        public bool CheckSignCookie()
        {
            bool bResult = _Core.CheckSignCookie();
            return bResult;
        }
    }
}
