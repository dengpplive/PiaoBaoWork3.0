using System;

namespace PbProject.WebCommon.Web.Cookie
{
    public class CookieUserInfo
    {
        private int _ID;

        public int ID { get { return _ID; } set { _ID = value > 0 ? value : 0; } }

        private void InitConfig()
        {
            this.ID = 0;
        }

        public CookieUserInfo()
        {
            InitConfig();
        }
    }
}
