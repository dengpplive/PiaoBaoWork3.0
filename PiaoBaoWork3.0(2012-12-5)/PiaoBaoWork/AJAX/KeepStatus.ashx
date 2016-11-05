<%@ WebHandler Language="C#" Class="KeepStatus" %>

using System;
using System.Web;

public class KeepStatus : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string result = "2";//默认2,校验码过期
        try
        {
            if (HttpContext.Current.Request["currentuserid"] != null)
            {
                string CheckCode = "";
                PbProject.WebCommon.Web.Cookie.SiteCookie siteCookie = new PbProject.WebCommon.Web.Cookie.SiteCookie();
                string keyName = HttpContext.Current.Request["currentuserid"].ToString() + "oneUserLoginCookies";
                CheckCode = siteCookie.GetCookie(keyName);
                if (HttpContext.Current.Application != null
                    && HttpContext.Current.Application[keyName] != null
                    )
                {
                    if (CheckCode == HttpContext.Current.Application[keyName].ToString())
                    {
                        result = "1";
                    }
                    else
                    {
                        result = "0";
                    }
                }
                else
                {
                    result = "2";
                }
            }
        }
        catch (Exception ex)
        {
            result = "2";
        }
        context.Response.Write(result);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}