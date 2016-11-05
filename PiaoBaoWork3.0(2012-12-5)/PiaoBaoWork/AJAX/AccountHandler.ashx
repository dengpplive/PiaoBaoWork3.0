<%@ WebHandler Language="C#" Class="AccountHandler" %>

using System;
using System.Web;
using PbProject.Model;
using System.Collections.Generic;
using PbProject.Logic.ControlBase;

public class AccountHandler : HttpHandle{
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
        string result = string.Empty;
        string cpyNo = this.mCompany.UninCode;
        List<User_Company> objList = new BaseDataManage().CallMethod("User_Company", "GetList", null, new object[] { " UninCode='" + cpyNo + "'" }) as List<User_Company>;
        if (objList != null && objList.Count > 0)
        {
            result = objList[0].AccountMoney.ToString();
        }
        OutPut(result);
    }

}