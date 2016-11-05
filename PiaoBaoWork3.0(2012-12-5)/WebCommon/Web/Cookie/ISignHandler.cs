using System;

namespace PbProject.WebCommon.Web.Cookie
{
    public interface ISignHandler
    {
        CookieUserInfo UserInfoFromCookie { get; set; }

        void SaveSignCookie();
        void ClearSignCookie();
        bool CheckSignCookie();
    }
}
