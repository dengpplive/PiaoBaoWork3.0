using System;
using System.Web;
using PbProject.WebCommon.Utility;

namespace PbProject.WebCommon.Web.Cookie
{
    public class SignHttpHandle : IHttpHandler,ISignHandler
    {
        private Core _Core = null;
        private HttpContext _HttpContext = null;

        public HttpContext Context { get { return this._HttpContext; } set { this._HttpContext = value; } }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }

        public CookieUserInfo UserInfoFromCookie { get { return _Core != null ? _Core.UserInfoFromCookie : null; } set { if (_Core != null) _Core.UserInfoFromCookie = value; } }

        public SignHttpHandle()
        { }

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

        private void InitConfig(HttpContext context)
        {
            try
            {
                _Core = new Core();

                _Core.RegistContext(context);

                Request = _Core.Request;
                Response = _Core.Response;
                Context = _Core.Context;
            }
            catch { }
            finally { }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
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
            return _Core.CheckSignCookie();
        }

        public void ProcessRequest(HttpContext context)
        {
            InitConfig(context);
            DoHandleWork();
        }

        public virtual void DoHandleWork()
        { }
    }
}
