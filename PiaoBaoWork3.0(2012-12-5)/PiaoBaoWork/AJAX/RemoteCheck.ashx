<%@ WebHandler Language="C#" Class="RemoteCheck" %>

using System;
using System.Web;

public class RemoteCheck : HttpHandle {
    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Ajax_Context.Response.ContentType = "text/plain";
        Ajax_Context.Response.Clear();
        Ajax_Context.Response.Write(result);
        Ajax_Context.Response.Flush();
    }
    /// <summary>
    /// 请求数据处理，重写
    /// </summary>
    public override void DoHandleWork()
    {
        //result为0通过，为1验证重复
        string result = "0";
       string account = this.Ajax_Request.Params["account"] != null ?Server.UrlDecode(Ajax_Request.Params["account"].ToString()) : string.Empty;
        if (account.Length > 0)
        {
            var listuser = new PbProject.Logic.User.User_EmployeesBLL().GetListByLoginName(account);
            if (listuser != null && listuser.Count > 0)
            {
                result ="1";
            }
        }
        OutPut(result);
    }

}