using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pay_Pos_ChinaPnrPosNotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                OnErrorNew("Pay_Pos_ChinaPnrPosNotifyUrl", true);

                IsValidation();
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

            //GateId =U2 
            //MerPriv = 
            //TrxId =2013062589057816 
            //OrdId =30201020130625000010 
            //RetType =2 
            //OrdAmt =1.00 
            //Pid = 
            //ChkValue =9188B176EE0CEC769E077BC63ACE7ADBDAD0FA273A5C81DF9E14C67789F93FACFBF062F22A9EECCA4208CBCEDC4084505C4565E32BA20C9670FC83C775804520AC8C317A2B59BF1FE173F7AC58A80286A54DBA3A2B6C8CFC8559A8530D6E2A55CDF1286A6EE1169712C90212526EA8A0E954B0D3488A8BEEC025DB4AA702F6BD 
            //CmdId =Buy 
            //DivDetails =Agent:00000458605930201020:1.00 
            //CurCode =RMB 
            //RespCode =000000 
            //MerId =871997 

            String GateId, MerPriv, TrxId, OrdId, RetType, OrdAmt, Pid, ChkValue, CmdId, DivDetails, CurCode, RespCode, MerId;

            GateId = Request.Form["GateId"];				
            MerPriv = Request.Form["MerPriv"]; 	 		   
            TrxId = Request.Form["TrxId"];
            OrdId = Request.Form["OrdId"]; 			   
            RetType = Request.Form["RetType"]; 			
            OrdAmt = Request.Form["OrdAmt"];  		
            Pid = Request.Form["Pid"];
            ChkValue = Request.Form["ChkValue"];
            CmdId = Request.Form["CmdId"];
            DivDetails = Request.Form["DivDetails"];
            CurCode = Request.Form["CurCode"];  	
            RespCode = Request.Form["RespCode"];		
            MerId = Request.Form["MerId"];		

           

            string SignData = "";

            #region 验证签名使用

            PbProject.Logic.Pay.ChinaPnr _ChinaPnr = new PbProject.Logic.Pay.ChinaPnr();
            String MsgData;
            MsgData = CmdId + MerId + RespCode + TrxId + OrdAmt + CurCode + Pid + OrdId + MerPriv + RetType + DivDetails + GateId;  	//参数顺序不能错
            CHINAPNRLib.NetpayClient SignObject = new CHINAPNRLib.NetpayClientClass();
            SignData = SignObject.VeriSignMsg0(_ChinaPnr._PgKeyUrl, MsgData, MsgData.Length, ChkValue);           //请将此处改成你的私钥文件所在路径
            
            #endregion


            if (SignData == "0")
            {
                if (RespCode == "000000")
                {
                    #region 交易成功

                    if (CmdId == "Buy")
                    {
                       new PbProject.Logic.Pay.Bill().CreateLogMoneyDetail("", TrxId, OrdAmt, 12, OrdId.Substring(0, 8), "POS充值", "POS充值");
                    }
                    #endregion
                }
                else
                {
                    //交易失败
                    OnErrorNew("交易失败 OrdId=" + OrdId, false);
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