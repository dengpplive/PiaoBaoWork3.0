using System;

namespace PbProject.WebCommon.Web.Cookie
{
    public class SignMasterPage : System.Web.UI.MasterPage,ISignHandler
    {
        private bool _Init = false;
        private Core _Core = null;

        public CookieUserInfo UserInfoFromCookie { get { return _Core != null ? _Core.UserInfoFromCookie : null; } set { if (_Core != null) _Core.UserInfoFromCookie = value; } }
        
        public string Domain { get { return _Core != null ? _Core.Domain : ""; } set { if (_Core != null) _Core.Domain = value; } }

        public SignMasterPage()
        {
            InitConfig();
        }

        private void InitConfig()
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
            return _Core.CheckSignCookie();
        }
    }
}
