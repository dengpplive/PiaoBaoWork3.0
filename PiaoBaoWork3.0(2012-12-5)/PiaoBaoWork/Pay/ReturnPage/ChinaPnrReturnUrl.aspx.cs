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
/// 汇付返回页面
/// </summary>
public partial class Pay_ReturnPage_ChinaPnrReturnUrl : System.Web.UI.Page
{

    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("Pay_ReturnPage_ChinaPnrReturnUrl", true);

        if (Request.Form["CmdId"] != null)
        {
            string value= Validation();
            if (!string.IsNullOrEmpty(value))
            {
                Response.Redirect(value);
            }
        }
    }

    /// <summary>
    /// Validation
    /// </summary>
    private string Validation()
    {
        string val = "";
        try
        {
            String CmdId, MerId, RespCode, TrxId, OrdAmt, CurCode, Pid, OrdId, MerPriv, RetType, DivDetails, GateId, ChkValue;

            CmdId = Request.Form["CmdId"];				//消息类型
            MerId = Request.Form["MerId"]; 	 		    //商户号
            RespCode = Request.Form["RespCode"]; 		//应答返回码
            TrxId = Request.Form["TrxId"]; 			    //钱管家交易唯一标识
            OrdAmt = Request.Form["OrdAmt"]; 			//金额
            CurCode = Request.Form["CurCode"];  		//币种
            Pid = Request.Form["Pid"];  				//商品编号
            OrdId = Request.Form["OrdId"];  			//订单号
            MerPriv = Request.Form["MerPriv"]; 		    //商户私有域
            RetType = Request.Form["RetType"];  		//返回类型
            DivDetails = Request.Form["DivDetails"];  	//分账明细
            GateId = Request.Form["GateId"];			//银行ID
            ChkValue = Request.Form["ChkValue"];		//签名信息

            PbProject.Logic.Pay.ChinaPnr _ChinaPnr = new PbProject.Logic.Pay.ChinaPnr();

            //验证签名
            String MsgData, SignData;
            MsgData = CmdId + MerId + RespCode + TrxId + OrdAmt + CurCode + Pid + OrdId + MerPriv + RetType + DivDetails + GateId;
            CHINAPNRLib.NetpayClient SignObject = new CHINAPNRLib.NetpayClientClass();

            SignData = SignObject.VeriSignMsg0(_ChinaPnr._PgKeyUrl, MsgData, MsgData.Length, ChkValue);       //请将此处改成你的私钥文件所在路径

            if (SignData == "0")
            {
                if (RespCode == "000000")
                {
                    #region 交易成功

                    if (CmdId == "Buy")
                    {
                        #region 支付
                        #endregion
                    }
                    else if (CmdId == "Refund")
                    {
                        #region 退款
                        #endregion
                    }

                    #endregion

                    val = "Sucess.aspx?PayType=3&ReturnType=1&OrderId=" + OrdId + "&Price=" + OrdAmt + "&OnLineNo=" + OrdId;
                }
                else
                {
                    //交易失败
                    OnErrorNew("交易失败", false);
                }
            }
            else
            {
                OnErrorNew("验证失败", false);
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("catch：" + ex, false);
        }

        return val;
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