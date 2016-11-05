using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using DataBase.Data;
using PbProject.Logic.ControlBase;

public partial class AJAX_OrderPrompt : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string result = GetOrderPrompt();
        OutPut(result);
    }
    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Response.ContentType = "text/plain";
        Response.Clear();
        Response.Write(result);
        Response.Flush();
        Response.End();
    }
    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name">请求key名称</param>
    /// <param name="DefaultVal">数据默认值</param>   
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), System.Text.Encoding.Default).Replace("'", "");
        }
        return DefaultVal;
    }
    /// <summary>
    /// 获取网站url根目录
    /// </summary>
    /// <returns></returns>
    public string getRootURL()
    {
        string AppPath = "";
        HttpContext HttpCurrent = HttpContext.Current;
        HttpRequest Req;
        if (HttpCurrent != null)
        {
            Req = HttpCurrent.Request;
            string UrlAuthority = Req.Url.GetLeftPart(UriPartial.Authority);
            if (Req.ApplicationPath == null || Req.ApplicationPath == "/")
            {
                AppPath = UrlAuthority;
            }
            else
            {
                AppPath = UrlAuthority + Req.ApplicationPath;
            }
            if (!AppPath.EndsWith("/"))
            {
                AppPath += "/";
            }
        }
        return AppPath;
    }
    /// <summary>
    /// 获取订单提醒数据
    /// </summary>
    /// <returns></returns>
    public string GetOrderPrompt()
    {
        StringBuilder sbPromptData = new StringBuilder();
        string result = "";
        string CpyNo = GetVal("CpyNo", "");
        string RoleType = GetVal("RoleType", "");
        string currentuserid = GetVal("currentuserid", "");
        string LoginName = GetVal("cudspeb", "");
        string LoginPwd = GetVal("cpdwpdb", "");
        string CurUrl = getRootURL();

        string ReUrl = CurUrl + "Login.aspx?cudspeb=" + LoginName + "&cpdwpdb=" + LoginPwd + "&ctdyppbe=cydepsb&OrderPrompt=1";

        HashObject hash = new HashObject();
        hash.Add("CpyNo", CpyNo);
        BaseDataManage baseDataManage = new BaseDataManage();
        DataTable table = baseDataManage.EexcProc("GetOrderPrompt", hash);
        int Num = 0;
        if (table != null && table.Rows.Count > 0)
        {
            DataRow dr = table.Rows[0];
            string param = "&currentuserid=" + currentuserid;
            string url = "";
            if (dr["待出票订单数"] != DBNull.Value && dr["待出票订单数"].ToString() != "0")
            {
                //待出票订单数
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderProcessList.aspx?prompt=1" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["待出票订单数"].ToString() + "</strong></a>张机票等待出票 </td></tr>");
                Num++;
            }
            if (dr["申请改签订单数"] != DBNull.Value && dr["申请改签订单数"].ToString() != "0")
            {
                //申请改签订单数
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderTGQList.aspx?prompt=4" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["申请改签订单数"].ToString() + "</strong></a>张机票申请改签 </td></tr>");
                Num++;
            }
            if (dr["申请退票订单数"] != DBNull.Value && dr["申请退票订单数"].ToString() != "0")
            {
                //申请退票订单数
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderTGQList.aspx?prompt=2" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["申请退票订单数"].ToString() + "</strong></a>张机票申请退票 </td></tr>");
                Num++;
            }
            if (dr["申请废票订单数"] != DBNull.Value && dr["申请废票订单数"].ToString() != "0")
            {
                //申请废票订单数
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderTGQList.aspx?prompt=3" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["申请废票订单数"].ToString() + "</strong></a>张机票申请废票 </td></tr>");
                Num++;
            }
            if (dr["异地退废改签订单数"] != DBNull.Value && dr["异地退废改签订单数"].ToString() != "0")
            {
                //异地退废改签订单数
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderTGQList.aspx?prompt=8" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["异地退废改签订单数"].ToString() + "</strong></a>张异地退废改签订单</td></tr>");
                Num++;
            }
            if (dr["退款中的订单"] != DBNull.Value && dr["退款中的订单"].ToString() != "0")
            {
                //退款中的订单
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderTGQList.aspx?prompt=9" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["退款中的订单"].ToString() + "</strong></a>张退款中的订单</td></tr>");
                Num++;
            }

            if (dr["待收银订单数"] != DBNull.Value && dr["待收银订单数"].ToString() != "0")
            {
                //显示数据
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderCashierList.aspx?prompt=1" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["待收银订单数"].ToString() + "</strong></a>张待收银订单</td></tr>");
                Num++;
            }
            if (dr["审核中的订单数"] != DBNull.Value && dr["审核中的订单数"].ToString() != "0")
            {
                //审核中的订单数
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderTGQList.aspx?prompt=10" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["审核中的订单数"].ToString() + "</strong></a>张审核中的订单</td></tr>");
                Num++;
            }
            if (dr["审核通过待退款"] != DBNull.Value && dr["审核通过待退款"].ToString() != "0")
            {
                //审核通过待退款
                url = HttpUtility.UrlEncode(CurUrl + "Order/OrderTGQList.aspx?prompt=5" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["审核通过待退款"].ToString() + "</strong></a>张审核通过，待退款订单</td></tr>");
                Num++;
            }
            if (dr["线下订单申请"] != DBNull.Value && dr["线下订单申请"].ToString() != "0")
            {
                //线下订单申请
                url = HttpUtility.UrlEncode(CurUrl + "Order/LineOrderProcess.aspx?prompt=5" + param);
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + ReUrl + "&ourl=" + url + "\"  target=\"_blank\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["线下订单申请"].ToString() + "</strong></a>张线下订单,等待处理</td></tr>");
                Num++;
            }
        }
        StringBuilder PromptUI = new StringBuilder();
        if (sbPromptData.ToString() != "")
        {
            int x1 = 224;
            int x2 = 230;
            x2 = Num * 28;
            x1 = Num * 28;
            if (x2 < 150)
            {
                x2 = 150;
                x1 = 170;
            }
            PromptUI.Append("<table>");
            PromptUI.Append(sbPromptData.ToString());
            PromptUI.Append("</table>");
        }
        result = PromptUI.ToString();
        return result;
    }
}