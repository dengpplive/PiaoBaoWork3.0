using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Net;

/// <summary>
/// 支付宝返回
/// </summary>
public partial class Pay_ReturnPage_AliPayReturnUrl : System.Web.UI.Page
{
    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("Pay_ReturnPage_AliPayReturnUrl", true);
        if (IsDataUrl())
        {
            string value = Success();

            if (!string.IsNullOrEmpty(value))
            {
                Response.Redirect(value);
            }
        }
    }

    /// <summary>
    /// 验证签约
    /// </summary>
    /// <returns></returns>
    private bool IsDataUrl()
    {
        if (Request.QueryString["sign"] != null && Request.QueryString["notify_id"] != null)
        {
            PbProject.Logic.Pay.AliPay alipay = new PbProject.Logic.Pay.AliPay();
             //接收地址
            string alipayNotifyURL = alipay._alipayreurl + "notify_id=" + Request.QueryString["notify_id"].ToString() + "&partner=" + alipay._partner;
            string key = alipay._code;

            //获取支付宝ATN返回结果，true是正确的订单信息，false 是无效的
            string responseTxt = alipay.Get_Http(alipayNotifyURL, 120000);
            //解析签名
            int i;
            NameValueCollection coll;
            coll = Request.QueryString;
            String[] requestarr = coll.AllKeys;
            //进行排序
            string[] Sortedstr = alipay.BubbleSort(requestarr);
            //构造待md5摘要字符串
            string prestr = "";
            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (Request.QueryString[Sortedstr[i]] != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr = prestr + Sortedstr[i] + "=" + Request.QueryString[Sortedstr[i]];
                    }
                    else
                    {
                        prestr = prestr + Sortedstr[i] + "=" + Request.QueryString[Sortedstr[i]] + "&";
                    }
                }

            }
            prestr = prestr + key;
            string mysign = alipay.GetMD5(prestr, "");
            string sign = Request.QueryString["sign"];

            //验证支付发过来的消息，签名是否正确
            if (mysign == sign && responseTxt == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string Success()
    {
        string value = "";
        try
        {
            PbProject.Logic.Pay.AliPay alipay = new PbProject.Logic.Pay.AliPay();
            int type = alipay.ReturnType(Request.QueryString["notify_type"].ToString());
            if (type == 1)
            {
                //1.支付
                if (Request.QueryString["out_trade_no"] != null
                    && Request.QueryString["trade_no"] != null
                    && Request.QueryString["total_fee"] != null)
                {
                    string orderid = Request.QueryString["out_trade_no"].ToString();
                    string onLineNo = Request.QueryString["trade_no"].ToString();
                    string price = Request.QueryString["total_fee"].ToString();
                    value = "Sucess.aspx?PayType=1&ReturnType=1&OrderId=" + orderid + "&Price=" + price + "&OnLineNo=" + onLineNo;
                }
            }
            else if (type == 2)
            {
                //2.退款
            }
        }
        catch (Exception)
        {

        }
        return value;
    }

    /// <summary>
    /// 记录文本日志
    /// </summary>
    /// <param name="content">记录内容</param>
    /// <param name="IsPostBack">是否记录 Request 参数</param>
    private void OnErrorNew(string errContent, bool IsRecordRequest)
    {
        try
        {
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), errContent, IsRecordRequest, System.Web.HttpContext.Current.Request);
        }
        catch (Exception)
        {

        }
         
    }
}
