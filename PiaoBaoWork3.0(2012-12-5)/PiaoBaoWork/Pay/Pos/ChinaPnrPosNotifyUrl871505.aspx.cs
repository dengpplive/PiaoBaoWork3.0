using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pay_Pos_ChinaPnrPosNotifyUrl871505 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                OnErrorNew("Pay_Pos_ChinaPnrPosNotifyUrl", true);
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// IsValidation
    /// </summary>
    private void IsValidation()
    {
        try
        {

            String CmdId, MerId, RespCode, TrxId, OrdAmt, CurCode, Pid, OrdId, MerPriv, RetType, DivDetails, GateId, ChkValue;
            String MsgData = "", SignData = "";

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

            #region 旧方法

            /*
             
            String MsgData, SignData;
            MsgData = CmdId + MerId + RespCode + TrxId + OrdAmt + CurCode + Pid + OrdId + MerPriv + RetType + DivDetails + GateId;  	//参数顺序不能错
            CHINAPNRLib.NetpayClient SignObject = new CHINAPNRLib.NetpayClientClass();
            SignData = SignObject.VeriSignMsg0(_ChinaPnr._PgKeyUrl, MsgData, MsgData.Length, ChkValue);           //请将此处改成你的私钥文件所在路径
             
            */

            #endregion


            if (SignData == "0")
            {
                if (RespCode == "000000")
                {
                    #region 交易成功

                    if (CmdId == "Buy")
                    {
                        //支付
                        if (OrdId.Substring(0, 1) == "0")
                        {

                        }
                    }
                    #endregion
                }
                else
                {
                    //交易失败
                    OnErrorNew("交易失败", false);
                }
                Response.Write("RECV_ORD_ID_" + OrdId);
            }
            else
            {
                //验签失败
                OnErrorNew("验签失败", false);
            }
        }
        catch (Exception ex)
        {

        }
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
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), errContent, IsRecordRequest, null);
        }
        catch (Exception)
        {

        }
    }
}