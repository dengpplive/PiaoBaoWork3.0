using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using PbProject.Logic.ControlBase;
using System.Text;
using PbProject.Logic;
using System.Web.Security;
/// <summary>
///HttpHandle 的摘要说明
/// </summary>
public class HttpHandle : BasePage, IHttpHandler, IRequiresSessionState
{
    public HttpHandle() { }

    public HttpRequest Ajax_Request { get; set; }
    public HttpResponse Ajax_Response { get; set; }
    public HttpContext Ajax_Context { get; set; }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="context"></param>
    public void Ajax_Init(HttpContext context)
    {
        Ajax_Request = context.Request;
        Ajax_Request.ContentEncoding = System.Text.Encoding.Default;
        Ajax_Response = context.Response;
        Ajax_Response.ContentEncoding = System.Text.Encoding.Default;
        Ajax_Context = context;
        //加载Session
        LoadSession();
    }

    /// <summary>
    /// 重设编码方式 默认Default
    /// </summary>
    /// <param name="encoding"></param>
    public void ResetEncoding(System.Text.Encoding encoding)
    {
        Ajax_Request.ContentEncoding = encoding;
        Ajax_Response.ContentEncoding = encoding;
    }
    /// <summary>
    /// 请求
    /// </summary>
    /// <param name="context"></param>
    //void IHttpHandler.ProcessRequest(HttpContext context)
    //{
    //    Ajax_Init(context);
    //    DoHandleWork();
    //}
    public override void ProcessRequest(HttpContext context)
    {
        base.ProcessRequest(context);
        Ajax_Init(context);
        DoHandleWork();

    }

    public virtual void DoHandleWork()
    {

    }
    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string ClientIP
    {
        get
        {
            string ip = "";
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            return ip;
        }
    }
    /// <summary>
    /// 客户端主机名
    /// </summary>
    public string HostName
    {
        get
        {
            return Request.ServerVariables.Get("Remote_Host").ToString();
        }
    }
    bool IHttpHandler.IsReusable
    {
        get
        {
            return false;
        }
    }
}