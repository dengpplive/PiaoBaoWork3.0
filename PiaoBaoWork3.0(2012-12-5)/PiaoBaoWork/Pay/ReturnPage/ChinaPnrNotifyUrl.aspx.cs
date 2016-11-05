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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using PnrAnalysis;
using PbProject.Logic;

/// <summary>
/// 汇付
/// </summary>
public partial class Pay_ReturnPage_ChinaPnrNotifyUrl : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("Pay_ReturnPage_ChinaPnrNotifyUrl", true);
        if (Request.Form["CmdId"] != null)
        {
            IsValidation();
        }
    }
    /// <summary>
    /// IsValidation
    /// </summary>
    private void IsValidation()
    {
        try
        {
            bool reuslt = false;

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

            #region 旧方法

            /*
             
            String MsgData, SignData;
            MsgData = CmdId + MerId + RespCode + TrxId + OrdAmt + CurCode + Pid + OrdId + MerPriv + RetType + DivDetails + GateId;  	//参数顺序不能错
            CHINAPNRLib.NetpayClient SignObject = new CHINAPNRLib.NetpayClientClass();
            SignData = SignObject.VeriSignMsg0(_ChinaPnr._PgKeyUrl, MsgData, MsgData.Length, ChkValue);           //请将此处改成你的私钥文件所在路径
             
            */

            #endregion 

            #region 新方法：汇付退款订单 暂时不使用签名 出现乱码错误

            //验证签名
            String MsgData = "", SignData = "";

            if (CmdId == "Refund")
            {
                SignData = "0";
            }
            else
            {
                MsgData = CmdId + MerId + RespCode + TrxId + OrdAmt + CurCode + Pid + OrdId + MerPriv + RetType + DivDetails + GateId;  	//参数顺序不能错
                CHINAPNRLib.NetpayClient SignObject = new CHINAPNRLib.NetpayClientClass();
                SignData = SignObject.VeriSignMsg0(_ChinaPnr._PgKeyUrl, MsgData, MsgData.Length, ChkValue);           //请将此处改成你的私钥文件所在路径
            }

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
                            #region 机票

                            PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
                            reuslt = bill.CreateBillPayBill(OrdId, TrxId, 3, MerPriv, "在线支付", "汇付支付", "");

                            //Kevin 2013-04-19 Edit
                            //如果接收到多次通知，则略过
                            if (!reuslt)
                            {
                                OnErrorNew("订单号：" + OrdId + "，汇付交易流水号：" + TrxId + "，收到多次通知，略过不做处理...", false);
                                return ;
                            }

                            PbProject.Model.Tb_Ticket_Order Order = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(OrdId);


                            List<PbProject.Model.Bd_Base_Parameters> mBP = new PbProject.Logic.ControlBase.BaseDataManage().
                 CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + Order.OwnerCpyNo.Substring(0, 12) + "'" }) as List<PbProject.Model.Bd_Base_Parameters>;

                            if (Order != null && Order.OrderStatusCode == 3)
                            {

                                //Login(Order);
                                if (Order.PolicySource == 3)
                                {
                                    PayBy517(Order, mBP);//517
                                }
                                else if (Order.PolicySource == 4)
                                {
                                    BaiTuoPay(Order, mBP);//百拓
                                }
                                else if (Order.PolicySource == 5)
                                {
                                    //  8000Y
                                    PayFor8000Y(Order, mBP);
                                }
                                else if (Order.PolicySource == 6)
                                {
                                    //  今日
                                    PayForToday(Order, mBP);
                                }
                                else if (Order.PolicySource == 7)
                                {
                                    PMPay(Order, mBP);//票盟
                                }
                                else if (Order.PolicySource == 8)
                                {
                                    bookPay(Order, mBP);//51book
                                }
                                else if (Order.PolicySource == 10)
                                {
                                    PayByYeeXing(Order, mBP);//易行
                                }
                            }

                            #endregion
                        }
                        else if (OrdId.Substring(0, 1) == "1")
                        {
                            #region 充值

                            PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
                            reuslt = bill.CreateLogMoneyDetail(OrdId, TrxId, OrdAmt, 3, MerPriv, "在线支付", "在线充值");

                            #endregion
                        }
                        else if (OrdId.Substring(0, 1) == "2")
                        {
                            #region 短信
                            new PbProject.Logic.Pay.Bill().MakeSMS(OrdId, OrdId, 3);
                            #endregion
                        }
                    }
                    else if (CmdId == "Refund")
                    {
                        //退款

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
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), errContent, IsRecordRequest, System.Web.HttpContext.Current.Request);
        }
        catch (Exception)
        {

        }

    }

    #region 接口方法

    #region 易行
    /// <summary>
    /// 易行订单并支付
    /// </summary>
    private bool PayByYeeXing(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsYeeXingna = true;
        DataSet dsReson = new DataSet();
        DataSet dsResonPay = new DataSet();
        string sql = " update Tb_Ticket_Order set ";
        try
        {

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);

            string yeeXingAccout = BS.JieKouZhangHao.Split('|')[6].Split('^')[0];
            string yeeXingAccout2 = BS.JieKouZhangHao.Split('|')[6].Split('^')[1];

            w_YeeXingService.YeeXingSerivce ServiceByYeeXing = new w_YeeXingService.YeeXingSerivce();
            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            if (Order.OutOrderId == "")
            {
                #region 无订单号：生成订单+支付
                FormatPNR ss = new FormatPNR();
                string PNRContent = "";
                PNRContent = skyList[0].PnrContent.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
                string PATContent = skyList[0].Pat.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
                if (Order.OrderSourceType == 1 && !Order.Space.Contains("1"))
                {

                    string bb = "";
                    PatModel sss = ss.GetPATInfo(skyList[0].Pat, out bb);
                    if (sss.ChildPat != null)
                    {
                        for (int i = 0; i < sss.PatList.Count; i++)
                        {
                            if (sss.PatList[i].SeatGroup == sss.ChildPat.SeatGroup)
                            {
                                sss.PatList.Remove(sss.PatList[i]);
                                break;
                            }
                        }
                        PatInfo patFirst = sss.PatList[0];
                        PatInfo patLast = sss.PatList[sss.PatList.Count - 1];
                        if (BS.KongZhiXiTong.Contains("|60|"))
                        {
                            PATContent = ss.NewPatData(patFirst);
                        }
                        else
                        {
                            PATContent = ss.NewPatData(patLast);
                        }
                        bool IsOnePrice = false;
                        PNRContent = ss.RemoveChildSeat(PNRContent, out IsOnePrice);
                    }
                }
                OnErrorNew("开始生成易行订单，本地订单号："+Order.OrderId, false);
                dsReson = ServiceByYeeXing.ParsePnrBookContract(yeeXingAccout, yeeXingAccout2, Order.PNR, PNRContent, PATContent, Order.PolicyId, Order.PMFee.ToString(), Order.OrderId, Order.OldPolicyPoint.ToString(), Order.OldReturnMoney.ToString());
                if (dsReson != null)
                {
                    string mesYeeXingCreate = "";
                    for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                    {
                        for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                        {
                            mesYeeXingCreate = mesYeeXingCreate + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                        }

                        mesYeeXingCreate = mesYeeXingCreate + "|";
                    }
                    OnErrorNew(mesYeeXingCreate, false);
                    if (dsReson.Tables[0].Rows[0]["is_success"].ToString() == "F")//生成订单失败，记录日志
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "于 " + DateTime.Now + " 易行生成失败：" + dsReson.Tables[0].Rows[0][0].ToString() + "," + dsReson.Tables[0].Rows[0][1].ToString();
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                        return false;
                    }
                    if (dsReson.Tables[0].Rows[0]["is_success"].ToString() == "T")
                    {
                        OnErrorNew("易行生成订单成功，本地订单号："+Order.OrderId, false);
                        sql += " OutOrderId='" + dsReson.Tables[1].Rows[0]["orderid"].ToString() + "'";
                        Order.OutOrderId = dsReson.Tables[1].Rows[0]["orderid"].ToString();
                        if (dsReson.Tables[6].Rows[0]["ibePrice"].ToString() == "")
                        {
                            sql += " ,OutOrderPayMoney=0";
                            Order.OutOrderPayMoney = 0;
                        }
                        else
                        {
                            sql += " ,OutOrderPayMoney=" + (Convert.ToDecimal(dsReson.Tables[6].Rows[0]["ibePrice"].ToString()) +
                                Convert.ToDecimal(dsReson.Tables[6].Rows[0]["buildFee"].ToString()) + Convert.ToDecimal(dsReson.Tables[6].Rows[0]["oilFee"].ToString())
                                - Convert.ToDecimal(dsReson.Tables[6].Rows[0]["profits"].ToString()));
                            Order.OutOrderPayMoney = (Convert.ToDecimal(dsReson.Tables[6].Rows[0]["ibePrice"].ToString()) +
                                Convert.ToDecimal(dsReson.Tables[6].Rows[0]["buildFee"].ToString()) + Convert.ToDecimal(dsReson.Tables[6].Rows[0]["oilFee"].ToString())
                                - Convert.ToDecimal(dsReson.Tables[6].Rows[0]["profits"].ToString()));
                        }
                        sql += " where OrderId='" + Order.OrderId + "'";
                        sqlbase.ExecuteNonQuerySQLInfo(sql);
                        if (BS.KongZhiXiTong.Contains("31"))//如果自动支付开关开启就调用接口支付
                        {
                            sql = " update Tb_Ticket_Order set ";
                            OnErrorNew("易行开始自动支付，本地订单号："+Order.OrderId, false);

                            //如果易行价格比本地高，则不支付
                            if (Convert.ToDecimal(dsReson.Tables["price"].Rows[0]["ibeprice"].ToString())
                                + Convert.ToDecimal(dsReson.Tables["price"].Rows[0]["buildFee"].ToString())
                                + Convert.ToDecimal(dsReson.Tables["price"].Rows[0]["oilFee"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                            {
                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "于 " + DateTime.Now + " 易行代付失败：平台订单价格和本地价格不符，不进行代付！易行平台价格："+
					dsReson.Tables["price"].Rows[0]["ibeprice"].ToString()+"|"+dsReson.Tables["price"].Rows[0]["buildFee"].ToString()+
					"|"+dsReson.Tables["price"].Rows[0]["oilFee"].ToString();
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                                OnErrorNew("易行平台订单价格和本地价格不符，不进行代付，本地订单号："+Order.OrderId, false);
                                return false;
                            }

                            string ReturnURL = "http://210.14.138.26:91/Pay/PTReturnPage/YeeXingNotifyUrl.aspx";
                            dsResonPay = ServiceByYeeXing.PayOutContract(yeeXingAccout, yeeXingAccout2, Order.OutOrderId, Order.OutOrderPayMoney.ToString(), "1", ReturnURL,ReturnURL);
                            if (dsResonPay != null)
                            {
                                string mesYeeXing = "";
                                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                                {
                                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                                    {
                                        mesYeeXing = mesYeeXing + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                                    }

                                    mesYeeXing = mesYeeXing + "|";
                                }
                                OnErrorNew(mesYeeXing, false);
                                if (dsResonPay.Tables[0].Rows[0]["is_success"].ToString() == "F")
                                {
                                    #region 记录操作日志
                                    //添加操作订单的内容
                                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                    OrderLog.id = Guid.NewGuid();
                                    OrderLog.OrderId = Order.OrderId;
                                    OrderLog.OperType = "修改";
                                    OrderLog.OperTime = DateTime.Now;
                                    OrderLog.OperContent = "于 " + DateTime.Now + " 易行代付失败：" + dsResonPay.Tables[0].Rows[0][0].ToString() + "," + dsResonPay.Tables[0].Rows[0][1].ToString();
                                    OrderLog.WatchType = 2;
                                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                    #endregion
                                    return false;
                                }
                                if (dsResonPay.Tables[0].Rows[0]["is_success"].ToString() == "T")
                                {
                                    OnErrorNew("is_success:" + dsResonPay.Tables[0].Rows[0]["is_success"].ToString(), false);

                                    OnErrorNew("易行自动代付成功，本地订单号："+Order.OrderId, false);
                                    sql += " OutOrderPayFlag=1,PayStatus=1";
                                    sql += " where OrderId='" + Order.OrderId + "'";
                                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                                    #region 记录操作日志
                                    //添加操作订单的内容
                                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                    OrderLog.id = Guid.NewGuid();
                                    OrderLog.OrderId = Order.OrderId;
                                    OrderLog.OperType = "修改";
                                    OrderLog.OperTime = DateTime.Now;
                                    OrderLog.OperContent = "易行自动代付成功!";
                                    OrderLog.WatchType = 2;
                                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                    #endregion
                                }

                            }
                        }
                    }
                    else
                    {
                        IsYeeXingna = false;
                    }
                }
                #endregion
            }
            else
            {
                #region 有订单号：单独支付
                if (BS.KongZhiXiTong.Contains("31"))//如果自动支付开关开启就调用接口支付
                {
                    sql = " update Tb_Ticket_Order set ";
                    OnErrorNew("易行开始自动支付，本地订单号："+Order.OrderId, false);

                    //如果易行价格比本地高，则不支付
                    if (Convert.ToDecimal(dsReson.Tables["price"].Rows[0]["ibeprice"].ToString())
                        + Convert.ToDecimal(dsReson.Tables["price"].Rows[0]["buildFee"].ToString())
                        + Convert.ToDecimal(dsReson.Tables["price"].Rows[0]["oilFee"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "于 " + DateTime.Now + " 易行代付失败：平台订单价格和本地价格不符，不进行代付！";
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                        OnErrorNew("易行平台订单价格和本地价格不符，不进行代付，本地订单号："+Order.OrderId, false);
                        return false;
                    }

                    string ReturnURL = "http://210.14.138.26:91/Pay/PTReturnPage/YeeXingNotifyUrl.aspx";
                    dsResonPay = ServiceByYeeXing.PayOutContract(yeeXingAccout, yeeXingAccout2, Order.OutOrderId, Order.OutOrderPayMoney.ToString(), "1", ReturnURL,ReturnURL);
                    if (dsResonPay != null)
                    {
                        string mesYeeXing = "";
                        for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                            {
                                mesYeeXing = mesYeeXing + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                            }

                            mesYeeXing = mesYeeXing + "|";
                        }
                        OnErrorNew(mesYeeXing, false);
                        if (dsResonPay.Tables[0].Rows[0]["is_success"].ToString() == "F")
                        {
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 易行代付失败：" + dsResonPay.Tables[0].Rows[0][0].ToString() + "," + dsResonPay.Tables[0].Rows[0][1].ToString();
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            return false;
                        }
                        if (dsResonPay.Tables[0].Rows[0]["is_success"].ToString() == "T")
                        {
                            OnErrorNew("is_success:" + dsResonPay.Tables[0].Rows[0]["is_success"].ToString(), false);

                            OnErrorNew("易行自动代付成功，本地订单号："+Order.OrderId, false);
                            sql += " OutOrderPayFlag=1,PayStatus=1";
                            sql += " where OrderId='" + Order.OrderId + "'";
                            sqlbase.ExecuteNonQuerySQLInfo(sql);

                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "易行自动代付成功!";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                        }

                    }

                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("易行线下异常:" + ex.Message+"，本地订单号："+Order.OrderId, false);
            IsYeeXingna = false;

            #region 记录操作日志
            //添加操作订单的内容
            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

            OrderLog.id = Guid.NewGuid();
            OrderLog.OrderId = Order.OrderId;
            OrderLog.OperType = "修改";
            OrderLog.OperTime = DateTime.Now;
            OrderLog.OperContent = "易行线下异常:" + ex.Message;
            OrderLog.WatchType = 2;
            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
            #endregion
        }
        return IsYeeXingna;
    }
    #endregion

    #region 517
    /// <summary>
    /// 517订单并支付
    /// </summary>
    private bool PayBy517(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool Is517na = true;
        DataSet dsReson = new DataSet();
        DataSet dsResonPay = new DataSet();
        string sql = " update Tb_Ticket_Order set ";
        try
        {

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();


            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            string Accout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[0];
            string Password517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[1];
            string Ag517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[2];
            string PayAccout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[3];
            string PayPassword517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[4];


            w_517WebService._517WebService ServiceBy517 = new w_517WebService._517WebService();
            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            if (Order.OutOrderId == "")
            {
                #region 无订单号：生成订单+支付
                FormatPNR ss = new FormatPNR();
                string PNRContent = "";
                PNRContent = skyList[0].PnrContent.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
                string PATContent = skyList[0].Pat.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
                if (Order.OrderSourceType == 1 && !Order.Space.Contains("1"))
                {

                    string bb = "";
                    PatModel sss = ss.GetPATInfo(skyList[0].Pat, out bb);
                    if (sss.ChildPat != null)
                    {
                        for (int i = 0; i < sss.PatList.Count; i++)
                        {
                            if (sss.PatList[i].SeatGroup == sss.ChildPat.SeatGroup)
                            {
                                sss.PatList.Remove(sss.PatList[i]);
                                break;
                            }
                        }
                        PatInfo patFirst = sss.PatList[0];
                        PatInfo patLast = sss.PatList[sss.PatList.Count - 1];
                        if (BS.KongZhiXiTong.Contains("|60|"))
                        {
                            PATContent = ss.NewPatData(patFirst);
                        }
                        else
                        {
                            PATContent = ss.NewPatData(patLast);
                        }
                        bool IsOnePrice = false;
                        PNRContent = ss.RemoveChildSeat(PNRContent, out IsOnePrice);
                    }
                }
                OnErrorNew("开始生成517订单，本地订单号："+Order.OrderId, false);

                if (Order.PolicyId.Split('~')[1].ToString() != "")//判断有无子政策ID
                {
                    dsReson = ServiceBy517.CreateOrderByPnrAndPAT(Accout517, Password517, Ag517, PNRContent, Order.BigCode, Convert.ToInt32(Order.PolicyId.Split('~')[0].ToString()), Order.LinkMan, Order.LinkManPhone, Order.PolicyId.Split('~')[1].ToString(), PATContent, Order.PNR);
                }
                else
                {
                    dsReson = ServiceBy517.CreateOrderByPnrAndPAT(Accout517, Password517, Ag517, PNRContent, Order.BigCode, Convert.ToInt32(Order.PolicyId.Split('~')[0].ToString()), Order.LinkMan, Order.LinkManPhone, "", PATContent, Order.PNR);
                }
                if (dsReson != null)
                {
                    string mes517Create = "";
                    for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                    {
                        for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                        {
                            mes517Create = mes517Create + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                        }

                        mes517Create = mes517Create + "|";
                    }
                    OnErrorNew(mes517Create, false);
                    if (dsReson.Tables[0].TableName == "error")//生成订单失败，记录日志
                    {
                        if (Order.OutOrderId == "")
                        {
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 517生成失败：" + dsReson.Tables[0].Rows[0][0].ToString() + "," + dsReson.Tables[0].Rows[0][1].ToString();
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            return false;
                        }
                    }
                    if (dsReson.Tables[0].Rows[0]["OrderId"].ToString() != "")
                    {
                        OnErrorNew("517生成订单成功，本地订单号："+Order.OrderId, false);
                        sql += " OutOrderId='" + dsReson.Tables[0].Rows[0]["OrderId"].ToString() + "'";
                        Order.OutOrderId = dsReson.Tables[0].Rows[0]["OrderId"].ToString();
                        if (dsReson.Tables[0].Rows[0]["TotlePirce"].ToString() == "")
                        {
                            sql += " ,OutOrderPayMoney=0";
                            Order.OutOrderPayMoney = 0;
                        }
                        else
                        {
                            sql += " ,OutOrderPayMoney=" + Convert.ToDecimal(dsReson.Tables[0].Rows[0]["TotlePirce"].ToString());
                            Order.OutOrderPayMoney = Convert.ToDecimal(dsReson.Tables[0].Rows[0]["TotlePirce"].ToString());
                        }
                        sql += " where OrderId='" + Order.OrderId + "'";
                        sqlbase.ExecuteNonQuerySQLInfo(sql);
                        if (BS.KongZhiXiTong.Contains("31"))//如果自动支付开关开启就调用接口支付
                        {
                            sql = " update Tb_Ticket_Order set ";
                            OnErrorNew("517开始自动支付，本地订单号："+Order.OrderId, false);

                            //如果517价格比本地高，则不支付
                            if ((Convert.ToDecimal(dsReson.Tables[0].Rows[0]["TotlePirce"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                                || (Convert.ToDecimal(dsReson.Tables["FlightInfo"].Rows[0]["FaceValue"].ToString())*Order.PassengerNumber != Order.PMFee)
                                || (Convert.ToDecimal(dsReson.Tables["FlightInfo"].Rows[0]["BenefitRate"].ToString()) < Order.OldPolicyPoint))
                            {
                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "于 " + DateTime.Now + " 517代付失败：平台订单价格和本地价格不符，不进行代付！";
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                                OnErrorNew("517平台订单价格和本地价格不符，不进行代付，平台票面价：" + dsReson.Tables["FlightInfo"].Rows[0]["FaceValue"].ToString()
                                    + "，平台返点：" + dsReson.Tables["FlightInfo"].Rows[0]["BenefitRate"].ToString() + "，本地订单号：" + Order.OrderId, false);
                                return false;
                            }
                            if (BS.KongZhiXiTong.Contains("54"))//开启517接口预存款支付
                            {
                                dsResonPay = ServiceBy517.OrderPay(Accout517, Password517, Ag517, PayAccout517, PayPassword517, Order.OutOrderId, Order.OutOrderPayMoney, "", Order.PNR);
                            }
                            else
                            {
                                dsResonPay = ServiceBy517.OrderNoPwdPay(Accout517, Password517, Order.OutOrderId, Order.OutOrderPayMoney, Ag517);
                            }
                            if (dsResonPay != null)
                            {
                                string mes517 = "";
                                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                                {
                                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                                    {
                                        mes517 = mes517 + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                                    }

                                    mes517 = mes517 + "|";
                                }
                                OnErrorNew(mes517, false);
                                if (mes517 == "False%%%/|")//代付失败，可能为余额不足
                                {

                                    #region 记录操作日志
                                    //添加操作订单的内容
                                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                    OrderLog.id = Guid.NewGuid();
                                    OrderLog.OrderId = Order.OrderId;
                                    OrderLog.OperType = "修改";
                                    OrderLog.OperTime = DateTime.Now;
                                    OrderLog.OperContent = "于 " + DateTime.Now + " 517代付失败：请检查自动代付支付宝余额";
                                    OrderLog.WatchType = 2;
                                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                    #endregion
                                    return false;
                                }
                                if (dsResonPay.Tables[0].TableName == "error")//代付失败，记录日志
                                {
                                    #region 记录操作日志
                                    //添加操作订单的内容
                                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                    OrderLog.id = Guid.NewGuid();
                                    OrderLog.OrderId = Order.OrderId;
                                    OrderLog.OperType = "修改";
                                    OrderLog.OperTime = DateTime.Now;
                                    OrderLog.OperContent = "于 " + DateTime.Now + " 517代付失败：" + dsResonPay.Tables[0].Rows[0][0].ToString() + "," + dsResonPay.Tables[0].Rows[0][1].ToString();
                                    OrderLog.WatchType = 2;
                                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                    #endregion
                                    return false;
                                }
                                if (dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString() == "True")
                                {
                                    OnErrorNew("PaySuccess:" + dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString(), false);

                                    OnErrorNew("517自动代付成功，本地订单号："+Order.OrderId, false);
                                    sql += " OutOrderPayFlag=1,PayStatus=1";
                                    sql += " where OrderId='" + Order.OrderId + "'";
                                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                                    #region 记录操作日志
                                    //添加操作订单的内容
                                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                    OrderLog.id = Guid.NewGuid();
                                    OrderLog.OrderId = Order.OrderId;
                                    OrderLog.OperType = "修改";
                                    OrderLog.OperTime = DateTime.Now;
                                    OrderLog.OperContent = "517自动代付成功!";
                                    OrderLog.WatchType = 2;
                                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                    #endregion
                                }

                            }

                        }
                    }
                    else
                    {
                        Is517na = false;
                    }
                }
                #endregion
            }
            else
            {
                #region 有订单号：单独支付
                if (BS.KongZhiXiTong.Contains("31"))//如果自动支付开关开启就调用接口支付
                {
                    sql = " update Tb_Ticket_Order set ";
                    OnErrorNew("517开始自动支付，本地订单号："+Order.OrderId, false);

                    //如果517价格比本地高，则不支付
                    if ((Convert.ToDecimal(dsReson.Tables[0].Rows[0]["TotlePirce"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                        || (Convert.ToDecimal(dsReson.Tables["FlightInfo"].Rows[0]["FaceValue"].ToString())*Order.PassengerNumber != Order.PMFee)
                        || (Convert.ToDecimal(dsReson.Tables["FlightInfo"].Rows[0]["BenefitRate"].ToString()) < Order.OldPolicyPoint))
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "于 " + DateTime.Now + " 517代付失败：平台订单价格和本地价格不符，不进行代付！";
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                        OnErrorNew("517平台订单价格和本地价格不符，不进行代付，平台票面价：" + dsReson.Tables["FlightInfo"].Rows[0]["FaceValue"].ToString()
                            + "，平台返点：" + dsReson.Tables["FlightInfo"].Rows[0]["BenefitRate"].ToString() + "，本地订单号：" + Order.OrderId, false);
                        return false;
                    }
                    if (BS.KongZhiXiTong.Contains("54"))//开启517接口预存款支付
                    {
                        dsResonPay = ServiceBy517.OrderPay(Accout517, Password517, Ag517, PayAccout517, PayPassword517, Order.OutOrderId, Order.OutOrderPayMoney, "", Order.PNR);
                    }
                    else
                    {
                        dsResonPay = ServiceBy517.OrderNoPwdPay(Accout517, Password517, Order.OutOrderId, Order.OutOrderPayMoney, Ag517);
                    }
                    if (dsResonPay != null)
                    {
                        string mes517 = "";
                        for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                            {
                                mes517 = mes517 + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                            }

                            mes517 = mes517 + "|";
                        }
                        OnErrorNew(mes517, false);
                        if (mes517 == "False%%%/|")//代付失败，可能为余额不足
                        {

                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 517代付失败：请检查自动代付支付宝余额";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            return false;
                        }
                        if (dsResonPay.Tables[0].TableName == "error")//代付失败，记录日志
                        {
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 517代付失败：" + dsResonPay.Tables[0].Rows[0][0].ToString() + "," + dsResonPay.Tables[0].Rows[0][1].ToString();
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            return false;
                        }
                        if (dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString() == "True")
                        {
                            OnErrorNew("PaySuccess:" + dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString(), false);

                            OnErrorNew("517自动代付成功，本地订单号："+Order.OrderId, false);
                            sql += " OutOrderPayFlag=1,PayStatus=1";
                            sql += " where OrderId='" + Order.OrderId + "'";
                            sqlbase.ExecuteNonQuerySQLInfo(sql);

                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "517自动代付成功!";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                        }

                    }

                }

                #endregion
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("517线下异常:" + ex.Message+"，本地订单号："+Order.OrderId, false);
            Is517na = false;

            #region 记录操作日志
            //添加操作订单的内容
            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

            OrderLog.id = Guid.NewGuid();
            OrderLog.OrderId = Order.OrderId;
            OrderLog.OperType = "修改";
            OrderLog.OperTime = DateTime.Now;
            OrderLog.OperContent = "517线下异常:" + ex.Message;
            OrderLog.WatchType = 2;
            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
            #endregion
        }
        return Is517na;
    }
    #endregion

    #region 百拓

    /// <summary>
    /// 百拓生成订单接口调用
    /// </summary>
    /// <param name="OrderId">本地订单编号</param>
    private XmlNode CreateBaiTuoOrder(PbProject.Model.Tb_Ticket_Order Order, PbProject.Logic.PTInterface.PTBybaituo OrderbaiTuoManager)
    {

        XmlNode xmlNode = null;
        DataTable dtPassenger = OrderbaiTuoManager.StructPassenger();
        string[] OrderArray = OrderbaiTuoManager.StructOrder();
        w_BTWebService.BaiTuoWeb BaiTuoService = new w_BTWebService.BaiTuoWeb();
        //BaiTuoWebService.BaiTuoWeb BaiTuoService = new BaiTuoWebService.BaiTuoWeb();

        XmlElement xmlElementCreateOrder = OrderbaiTuoManager.BaiTuoCreateOrderSend(OrderArray, dtPassenger);
        OnErrorNew(xmlElementCreateOrder.InnerXml, false);
        xmlNode = BaiTuoService.pnrCreateOrderEx(xmlElementCreateOrder);
        return xmlNode;
    }
    /// <summary>
    /// 调用百拓支付接口
    /// </summary>
    /// <param name="OrderId">订单编号</param>
    private bool BaiTuoPay(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsOk = true;
        try
        {
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();

            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            PbProject.Logic.PTInterface.PTBybaituo OrderbaiTuoManager = new PbProject.Logic.PTInterface.PTBybaituo(Order, BS);
            string sql = " update Tb_Ticket_Order set ";
            //if (ComModel.A66 == "1")
            //{
            OnErrorNew("开始生成百拓订单，本地订单号："+Order.OrderId, false);
            XmlNode xmlNode = CreateBaiTuoOrder(Order, OrderbaiTuoManager);
            DataSet dsReson = new DataSet();
            StringReader rea = new StringReader("<BAITOUR_ORDER_CREATE_RS>" + xmlNode.InnerXml + "</BAITOUR_ORDER_CREATE_RS>");
            XmlTextReader xmlReader = new XmlTextReader(rea);
            dsReson.ReadXml(xmlReader);

            if (dsReson != null)
            {
                string mesBaituoCreate = "";
                for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                    {
                        mesBaituoCreate = mesBaituoCreate + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mesBaituoCreate = mesBaituoCreate + "|";
                }
                OnErrorNew(mesBaituoCreate, false);
                if (dsReson.Tables[0].TableName == "Errors")
                {
                    if (Order.OutOrderId == "")
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "于 " + DateTime.Now + " 生成百拓订单失败，失败原因：" + dsReson.Tables[0].Rows[0][0].ToString() + "," + dsReson.Tables[0].Rows[0][1].ToString();
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                        return false;
                    }
                }
                if (dsReson.Tables[0].Rows[0]["forderformid"].ToString() != "" && dsReson.Tables[0].Rows[0]["shouldPay"].ToString() != "")
                {
                    OnErrorNew("百拓生成订单成功，本地订单号："+Order.OrderId, false);
                    sql += " OutOrderId='" + dsReson.Tables[0].Rows[0]["forderformid"].ToString() + "'";
                    Order.OutOrderId = dsReson.Tables[0].Rows[0]["forderformid"].ToString();
                    if (dsReson.Tables[0].Rows[0]["shouldPay"].ToString() == "")
                    {
                        sql += " ,OutOrderPayMoney=0";
                        Order.OutOrderPayMoney = 0;
                    }
                    else
                    {
                        sql += " ,OutOrderPayMoney=" + Convert.ToDecimal(dsReson.Tables[0].Rows[0]["shouldPay"].ToString());
                        Order.OutOrderPayMoney = Convert.ToDecimal(dsReson.Tables[0].Rows[0]["shouldPay"].ToString());
                    }
                    sql += " where OrderId='" + Order.OrderId + "'";

                    sqlbase.ExecuteNonQuerySQLInfo(sql);
                    if (BS.KongZhiXiTong.Contains("31"))
                    {
                        sql = " update Tb_Ticket_Order set ";
                        OnErrorNew("百拓开始自动支付，本地订单号："+Order.OrderId, false);

                        //如果百拓价格比本地高，则不支付
                        if (Convert.ToDecimal(dsReson.Tables[0].Rows[0]["shouldPay"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                        {
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 百拓自动代付失败：平台订单价格和本地价格不符，不进行代付！";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            OnErrorNew("百拓平台订单价格和本地价格不符，不进行代付，本地订单号："+Order.OrderId, false);
                            return false;
                        }
                        string Message = "";
                        try
                        {
                            string SendURL = OrderbaiTuoManager.BaiTuoPaySend(Order.OrderId, "1");
                            w_BTWebService.BaiTuoWeb BaiTuoService = new w_BTWebService.BaiTuoWeb();
                            if (SendURL != "")
                            {
                                OnErrorNew(SendURL, false);
                                Message = HttpUtility.UrlDecode(BaiTuoService.GetUrlData(SendURL));
                                OnErrorNew("收到订单号：" + Order.OrderId + "，百拓平台自动代付返回结果：" + Message, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            OnErrorNew("百拓自动支付失败:" + ex.ToString()+",本地订单号："+Order.OrderId, false);
                            Message = "";

                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "百拓自动支付失败:" + ex.Message;
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                        }
                        if (Message != "")
                        {
                            if (Message.Substring(Message.IndexOf("<PaymentResult>") + "<PaymentResult>".Length, 1) == "T" || Message.Substring(Message.IndexOf("<PaymentResult>") + "<PaymentResult>".Length, 1) == "1")
                            {
                                sql += " OutOrderPayFlag=1,PayStatus=1";
                                sql += " where OrderId='" + Order.OrderId + "'";
                                sqlbase.ExecuteNonQuerySQLInfo(sql);

                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "百拓自动代付成功!";
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                            }
                            else
                            {
                                OnErrorNew("百拓后台代付失败:" + Message+"，本地订单号："+Order.OrderId, false);

                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "百拓后台代付失败:" + Message;
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                            }
                        }
                        else
                        {
                            OnErrorNew("百拓自动支付失败:Message为空，本地订单号："+Order.OrderId, false);
                            IsOk = false;
                        }
                    }

                }
            }
            //}
        }
        catch (Exception ex)
        {
            OnErrorNew("百拓后台代付失败:" + ex.Message+"，本地订单号："+Order.OrderId, false);
            IsOk = false;

            #region 记录操作日志
            //添加操作订单的内容
            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

            OrderLog.id = Guid.NewGuid();
            OrderLog.OrderId = Order.OrderId;
            OrderLog.OperType = "修改";
            OrderLog.OperTime = DateTime.Now;
            OrderLog.OperContent = "百拓后台代付失败:" + ex.Message;
            OrderLog.WatchType = 2;
            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
            #endregion
        }
        return IsOk;
    }

    #endregion

    #region 今日
    /// <summary>
    /// 调用今日支付接口
    /// </summary>
    /// <returns></returns>
    private bool PayForToday(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsToday = true;
        try
        {
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);

            string todayAccout = BS.JieKouZhangHao.Split('|')[4].Split('^')[0];

            string todayAccout2 = BS.JieKouZhangHao.Split('|')[4].Split('^')[1];
            string sql = " update Tb_Ticket_Order set ";


            w_TodayService.WTodayService WSvcToday = new w_TodayService.WTodayService();
            IList<PbProject.Model.Tb_Ticket_Passenger> passengerList = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL().GetPasListByOrderID(Order.OrderId);
            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            string PNRContent = skyList[0].PnrContent.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            string PATContent = skyList[0].Pat.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            OnErrorNew("今日开始生成订单，本地订单号："+Order.OrderId, false);
            string PNRinfo = "O|P|" + Order.PNR + "^F^" + Order.BigCode + "|" + skyList[0].FromDate.ToString("yyyy-MM-dd") + "|" + skyList[0].FromCityCode + "|" + skyList[0].FromCityName + "|" + skyList[0].ToCityCode + "|" + skyList[0].ToCityName + "|" + skyList[0].CarryCode + skyList[0].FlightCode + "^N||" + skyList[0].FromDate.ToShortTimeString() + "|" + skyList[0].ToDate.ToShortTimeString() + "|" + skyList[0].Space + "|" + skyList[0].Discount + "||" + passengerList[0].PMFee + "|" + (skyList[0].ABFee + skyList[0].FuelFee) + "|" + Order.PassengerName.Split('/').Length + "|" + Order.PassengerName.Replace("/", "@");
            DataSet dsReson = WSvcToday.CreateOrderByPNR(todayAccout2, Order.PNR, Order.JinriGYCode, (Order.OldPolicyPoint * 100).ToString(), Order.PolicyId, PNRinfo, "0");
            string mesTodayCreate = "";
            for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                {
                    mesTodayCreate = mesTodayCreate + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                }

                mesTodayCreate = mesTodayCreate + "|";
            }
            OnErrorNew(mesTodayCreate, false);
            if (dsReson != null && dsReson.Tables.Count > 0 && dsReson.Tables[0].Rows[0]["OrderNo"].ToString() != "")
            {
                //  生成订单成功
                OnErrorNew("今日生成订单成功，本地订单号："+Order.OrderId, false);
                sql += " OutOrderId='" + dsReson.Tables[0].Rows[0]["OrderNo"].ToString() + "'";

                Order.OutOrderId = dsReson.Tables[0].Rows[0]["OrderNo"].ToString();
                if (dsReson.Tables[0].Rows[0]["PayMoney"].ToString() == "")
                {
                    sql += " ,OutOrderPayMoney=0";
                    Order.OutOrderPayMoney = 0;
                }
                else
                {
                    sql += " ,OutOrderPayMoney=" + Convert.ToDecimal(dsReson.Tables[0].Rows[0]["PayMoney"].ToString());
                    Order.OutOrderPayMoney = Convert.ToDecimal(dsReson.Tables[0].Rows[0]["PayMoney"].ToString());
                }
                sql += " where OrderId='" + Order.OrderId + "'";
                sqlbase.ExecuteNonQuerySQLInfo(sql);
                if (BS.KongZhiXiTong.Contains("31"))
                {
                    sql = " update Tb_Ticket_Order set ";
                    OnErrorNew("今日开始自动支付，本地订单号："+Order.OrderId, false);
                    //  若今日价格比本地高，则不支付
                    if (Convert.ToDecimal(dsReson.Tables[0].Rows[0]["PayMoney"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "于 " + DateTime.Now + " 今日自动代付失败：平台订单价格和本地价格不符，不进行代付！";
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                        OnErrorNew("今日平台订单价格和本地价格不符，不进行代付，本地订单号："+Order.OrderId, false);
                        return false;
                    }
                    DataSet dsResonPay = WSvcToday.AutoPayOrder(todayAccout2, Order.OutOrderId);
                    string mesTodayPay = "";
                    for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                    {
                        for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                        {
                            mesTodayPay = mesTodayPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                        }

                        mesTodayPay = mesTodayPay + "|";
                    }
                    OnErrorNew(mesTodayPay, false);
                    if (dsResonPay != null && dsResonPay.Tables.Count > 0)
                    {
                        if (dsResonPay.Tables[0].Rows.Count > 0 && dsResonPay.Tables[0].Rows[0]["Result"].ToString() == "T")
                        {
                            //  支付成功
                            OnErrorNew("今日自动代付成功，本订单号："+Order.OrderId, false);
                            sql += " OutOrderPayFlag=1,PayStatus=1";
                            sql += " where OrderId='" + Order.OrderId + "'";
                            sqlbase.ExecuteNonQuerySQLInfo(sql);

                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "今日自动代付成功!";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                        }
                        else
                        {
                            //  支付失败
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 今日代付失败：请检查自动代付支付宝余额";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            OnErrorNew("今日线下异常:" + ex.Message+"，本地订单号："+Order.OrderId, false);
            IsToday = false;

            #region 记录操作日志
            //添加操作订单的内容
            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

            OrderLog.id = Guid.NewGuid();
            OrderLog.OrderId = Order.OrderId;
            OrderLog.OperType = "修改";
            OrderLog.OperTime = DateTime.Now;
            OrderLog.OperContent = "今日线下异常:" + ex.Message;
            OrderLog.WatchType = 2;
            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
            #endregion
        }
        return IsToday;
    }
    #endregion

    #region 八千翼
    /// <summary>
    /// 调用8000Y支付接口
    /// </summary>
    /// <returns></returns>
    private bool PayFor8000Y(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool Is8000Y = true;
        string sql = " update Tb_Ticket_Order set ";

        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        OnErrorNew("开始Session", false);
        BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
        OnErrorNew("结束Session", false);
        //落地运营商和供应商公司参数信息

        string Accout8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[0];

        string Password8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[1];
        string Alipaycode8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[2];
        try
        {
            w_8000YService.W8000YService WSvc8000Y = new w_8000YService.W8000YService();

            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            FormatPNR ss = new FormatPNR();
            string PNRContent = skyList[0].PnrContent.Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            string PATContent = skyList[0].Pat.Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            OnErrorNew("8000Y开始生成订单，本地订单号："+Order.OrderId, false);
            string reqPNRContent = "";

            reqPNRContent = skyList[0].PnrContent.Replace("", "").Replace("", "").Replace("", "").Replace("", "");

            //string reqPNRContent = skyList[0].A9;// (new PnrAnalysis.FormatPNR()).SplitPnrAutoLine(skyList[0].A7);
            DataSet dsReson = null;// WSvc8000Y.CreatOrderNewByPNRNote(ComModel.A61, ComModel.A62, Order.PNR, Order.PolicyId, reqPNRContent);
            if (Order.TravelType == 1 || Order.TravelType == 2)
            {
                OnErrorNew("开始调用接口生成，本地订单号："+Order.OrderId, false);
                dsReson = WSvc8000Y.CreatOrderNewByPNRNote(Accout8000yi, Password8000yi, Order.PNR, Order.PolicyId, reqPNRContent);
                OnErrorNew("接口生成完成，本地订单号："+Order.OrderId, false);
            }
            else
            {

                #region 记录操作日志
                //添加操作订单的内容
                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = Order.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperContent = "于 " + DateTime.Now + " 八千翼不支持联程";
                OrderLog.WatchType = 2;
                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                #endregion
            }
            try
            {
                string mes8000YCreate = "";
                for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                    {
                        mes8000YCreate = mes8000YCreate + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mes8000YCreate = mes8000YCreate + "|";
                }
                OnErrorNew(mes8000YCreate, false);
                if (dsReson != null && dsReson.Tables.Count > 0 && dsReson.Tables[0].Rows[0]["OrderID"].ToString() != "")
                {
                    //  生成订单成功
                    OnErrorNew("8000Y生成订单成功，本地订单号："+Order.OrderId, false);
                    sql += " OutOrderId='" + dsReson.Tables[0].Rows[0]["OrderID"].ToString() + "'";

                    Order.OutOrderId = dsReson.Tables[0].Rows[0]["OrderID"].ToString();
                    if (dsReson.Tables[0].Rows[0]["CWZongJia"].ToString() == "")
                    {
                        sql += " ,OutOrderPayMoney=0";
                        Order.OutOrderPayMoney = 0;
                    }
                    else
                    {
                        sql += " ,OutOrderPayMoney=" + Convert.ToDecimal(dsReson.Tables[0].Rows[0]["CWZongJia"].ToString());
                        Order.OutOrderPayMoney = Convert.ToDecimal(dsReson.Tables[0].Rows[0]["CWZongJia"].ToString());
                    }

                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);
                    if (BS.KongZhiXiTong.Contains("31"))
                    {
                        sql = " update Tb_Ticket_Order set ";
                        OnErrorNew("8000Y开始自动支付，本地订单号："+Order.OrderId, false);

                        //  若8000Y价格比本地高，则不支付
                        if ((Convert.ToDecimal(dsReson.Tables[0].Rows[0]["CGShiFu"].ToString()) != Order.PMFee)//票面价合计
                            || (Convert.ToDecimal(dsReson.Tables[0].Rows[0]["FanDian"].ToString()) < Order.OldPolicyPoint))//返点
                        {
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 8000Y自动代付失败：平台订单价格和本地价格不符，不进行代付！";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            OnErrorNew("8000Y平台订单价格和本地价格不符，平台票面价合计：" + dsReson.Tables[0].Rows[0]["CGShiFu"].ToString()
                                + "，平台返点：" + dsReson.Tables[0].Rows[0]["FanDian"].ToString() + "不进行代付，本地订单号：" + Order.OrderId, false);
                            return false;
                        }
                        DataSet dsResonPay = WSvc8000Y.AutomatismPay(Accout8000yi, Password8000yi, Order.OutOrderId, Alipaycode8000yi);
                        OnErrorNew("8000Y自动支付完成，本地订单号："+Order.OrderId, false);
                        string mes8000YPay = "";
                        for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                            {
                                mes8000YPay = mes8000YPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                            }

                            mes8000YPay = mes8000YPay + "|";
                        }
                        OnErrorNew(mes8000YPay, false);
                        if (dsResonPay != null && dsResonPay.Tables.Count > 0)
                        {
                            try
                            {
                                //  支付失败
                                dsResonPay.Tables[0].Rows[0]["is_success"].ToString();
                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "于 " + DateTime.Now + " 8000Y代付失败："+mes8000YPay;
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                            }
                            catch
                            {
                                //  支付成功
                                OnErrorNew("8000Y自动代付成功，本地订单号："+Order.OrderId, false);
                                sql += " OutOrderPayFlag=1,PayStatus=1";
                                sql += " where OrderId='" + Order.OrderId + "'";
                                sqlbase.ExecuteNonQuerySQLInfo(sql);

                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "8000Y自动代付成功!";
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string errMsg = "";
                try
                {
                    errMsg = ex.ToString();
                    OnErrorNew("8000Y下单失败，错误信息：" + errMsg+"|"+ex.Message+"，本地订单号："+Order.OrderId, false);
                }
                catch { }
                //  生成订单失败

                #region 记录操作日志
                //添加操作订单的内容
                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = Order.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperContent = "于 " + DateTime.Now + " 8000Y代付失败，请检查代付支付账户余额!";//错误信息：" + errMsg+"|"+ex.Message;
                OrderLog.WatchType = 2;
                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                #endregion
                return false;
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("8000Y线下异常:" + ex.Message+"，本地订单号："+Order.OrderId, false);
            Is8000Y = false;

            #region 记录操作日志
            //添加操作订单的内容
            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

            OrderLog.id = Guid.NewGuid();
            OrderLog.OrderId = Order.OrderId;
            OrderLog.OperType = "修改";
            OrderLog.OperTime = DateTime.Now;
            OrderLog.OperContent = "8000Y线下异常:" + ex.Message;
            OrderLog.WatchType = 2;
            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
            #endregion
        }
        return Is8000Y;
    }
    #endregion

    #region 51book
    /// <summary>
    /// 调用51book支付接口
    /// </summary>
    /// <param name="OrderId">订单编号</param>
    private bool bookPay(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool Isbook = true;

        string sql = " update Tb_Ticket_Order set ";

        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);

        string Accout51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[0];

        string Password51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[1];

        string Ag51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[2];

        string Url51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[3];
        try
        {
            w_51bookService._51bookService bookService = new w_51bookService._51bookService();

            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");

            string PNRContent = skyList[0].PnrContent.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            string PATContent = skyList[0].Pat.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            OnErrorNew("51book开始生成订单，本地订单号："+Order.OrderId, false);
            DataSet dsReson = bookService.bookCreatePolicyOrderByPNR(Accout51book, Order.PNR, Order.PolicyId, Url51book, Url51book, "票宝", Ag51book, PNRContent, PATContent);
            if (dsReson != null)
            {
                string mes51bookCreate = "";
                for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                    {
                        mes51bookCreate = mes51bookCreate + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mes51bookCreate = mes51bookCreate + "|";
                }
                OnErrorNew(mes51bookCreate, false);
                if (dsReson.Tables[0].Columns.Contains("ErorrMessage"))//生成订单失败，记录日志
                {
                    if (Order.OutOrderId == "")
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "于 " + DateTime.Now + " 51book生成失败：" + dsReson.Tables[0].Rows[0]["ErorrMessage"].ToString();
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                        return false;
                    }
                }
                if (dsReson.Tables[0].Rows[0]["sequenceNo"].ToString() != "")
                {
                    OnErrorNew("51book生成订单成功，本地订单号："+Order.OrderId, false);
                    sql += "OutOrderId='" + dsReson.Tables[0].Rows[0]["sequenceNo"].ToString() + "'";

                    Order.OutOrderId = dsReson.Tables[0].Rows[0]["sequenceNo"].ToString();
                    if (dsReson.Tables[0].Rows[0]["settlePrice"].ToString() == "")
                    {
                        sql += " ,OutOrderPayMoney=0";
                        Order.OutOrderPayMoney = 0;
                    }
                    else
                    {
                        sql += " ,OutOrderPayMoney=" + Convert.ToDecimal(dsReson.Tables[0].Rows[0]["settlePrice"].ToString());
                        Order.OutOrderPayMoney = Convert.ToDecimal(dsReson.Tables[0].Rows[0]["settlePrice"].ToString());
                    }

                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);
                    if (BS.KongZhiXiTong.Contains("31"))
                    {
                        sql = " update Tb_Ticket_Order set ";
                        OnErrorNew("51book开始自动支付，本地订单号："+Order.OrderId, false);
                        //如果51book价格比本地高，则不支付
                        if (Convert.ToDecimal(dsReson.Tables[0].Rows[0]["settlePrice"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee-Order.PolicyMoney))
                        {
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 51book自动代付失败：平台订单价格和本地价格不符，不进行代付！";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            OnErrorNew("51book平台订单价格和本地价格不符，不进行代付，本地订单号："+Order.OrderId, false);
                            return false;
                        }
                        DataSet dsResonPay = bookService.bookPayPolicyOrderByPNR(Accout51book, Order.OutOrderId, Accout51book, Password51book, Ag51book);
                        if (dsResonPay != null)
                        {
                            string mes51bookPay = "";
                            for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                            {
                                for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                                {
                                    mes51bookPay = mes51bookPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                                }

                                mes51bookPay = mes51bookPay + "|";
                            }
                            OnErrorNew(mes51bookPay, false);
                            if (mes51bookPay.IndexOf("1%%%/F") > 0)//代付失败，记录日志
                            {
                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "于 " + DateTime.Now + " 51book代付失败：" + dsResonPay.Tables[0].Rows[0]["ErorrMessage"].ToString();
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                                return false;
                            }
                            if (dsResonPay.Tables[0].Rows[0]["orderStatus"].ToString() == "2")
                            {
                                OnErrorNew("51book自动代付成功，本地订单号："+Order.OrderId, false);
                                sql += " OutOrderPayFlag=1,PayStatus=1";
                                sql += " where OrderId='" + Order.OrderId + "'";
                                sqlbase.ExecuteNonQuerySQLInfo(sql);

                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "51book自动代付成功!";
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                            }
                        }
                    }
                }
                else
                {
                    Isbook = false;
                }
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("51book线下异常:" + ex.Message+"，本地订单号："+Order.OrderId, false);
            Isbook = false;

            #region 记录操作日志
            //添加操作订单的内容
            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

            OrderLog.id = Guid.NewGuid();
            OrderLog.OrderId = Order.OrderId;
            OrderLog.OperType = "修改";
            OrderLog.OperTime = DateTime.Now;
            OrderLog.OperContent = "51book线下异常:" + ex.Message;
            OrderLog.WatchType = 2;
            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
            #endregion
        }
        return Isbook;
    }
    #endregion

    #region 票盟
    /// <summary>
    /// 调用票盟支付接口
    /// </summary>
    /// <param name="OrderId">订单编号</param>
    private bool PMPay(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsPm = true;
        string sql = " update Tb_Ticket_Order set ";
        try
        {
            PMService.PMService PMService = new PMService.PMService();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            string pmAccout = BS.JieKouZhangHao.Split('|')[3].Split('^')[0];

            string pmPassword = BS.JieKouZhangHao.Split('|')[3].Split('^')[1];
            string pmAg = BS.JieKouZhangHao.Split('|')[3].Split('^')[2];

            OnErrorNew("票盟开始生成订单，本地订单号："+Order.OrderId, false);
            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            string PATContent = skyList[0].Pat.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            string RTContent = skyList[0].PnrContent.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            if (Order.OrderSourceType == 1 && !Order.Space.Contains("1"))
            {
                FormatPNR ss = new FormatPNR();
                string bb = "";
                PatModel sss = ss.GetPATInfo(skyList[0].Pat, out bb);
                if (sss.ChildPat != null)
                {
                    for (int i = 0; i < sss.PatList.Count; i++)
                    {
                        if (sss.PatList[i].SeatGroup == sss.ChildPat.SeatGroup)
                        {
                            sss.PatList.Remove(sss.PatList[i]);
                            break;
                        }
                    }
                    PatInfo patFirst = sss.PatList[0];
                    PatInfo patLast = sss.PatList[sss.PatList.Count - 1];
                    if (BS.KongZhiXiTong.Contains("|60|"))
                    {
                        PATContent = ss.NewPatData(patFirst);
                    }
                    else
                    {
                        PATContent = ss.NewPatData(patLast);
                    }
                    bool IsOnePrice = false;
                    RTContent = ss.RemoveChildSeat(RTContent, out IsOnePrice);
                }
            }
            DataSet dsReson = PMService.CreateOrderByPAT(Order.PolicyId, Order.BigCode, RTContent, PATContent, "0", pmAccout, pmAg);
            if (dsReson.Tables.Count > 1)
            {
                string mesPMCreate = "";
                for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                    {
                        mesPMCreate = mesPMCreate + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mesPMCreate = mesPMCreate + "|";
                }
                for (int i = 0; i < dsReson.Tables[1].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[1].Columns.Count; j++)
                    {
                        mesPMCreate = mesPMCreate + dsReson.Tables[1].Rows[i][j].ToString() + "&&&/";
                    }
                    mesPMCreate = mesPMCreate + "|";
                }
                OnErrorNew(mesPMCreate, false);
                if (dsReson.Tables[0].Rows[0]["statuscode"].ToString() == "0")
                {
                    OnErrorNew("票盟生成订单成功，本地订单号："+Order.OrderId, false);
                    sql += " OutOrderId = '" + dsReson.Tables[1].Rows[0]["orderid"].ToString() + "'";

                    Order.OutOrderId = dsReson.Tables[1].Rows[0]["orderid"].ToString();
                    if (dsReson.Tables[1].Rows[0]["payfee"].ToString() == "")
                    {
                        sql += " ,OutOrderPayMoney=0";
                        Order.OutOrderPayMoney = 0;
                    }
                    else
                    {
                        sql += " ,OutOrderPayMoney=" + Convert.ToDecimal(dsReson.Tables[1].Rows[0]["payfee"].ToString());
                        Order.OutOrderPayMoney = Convert.ToDecimal(dsReson.Tables[1].Rows[0]["payfee"].ToString());
                    }

                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);
                    if (BS.KongZhiXiTong.Contains("31"))
                    {
                        sql = " update Tb_Ticket_Order set ";

                        //如果票盟价格比本地高，则不支付
                        if ((Convert.ToDecimal(dsReson.Tables[1].Rows[0]["payfee"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                            || (Convert.ToDecimal(dsReson.Tables[1].Rows[0]["ticketfee"].ToString())*Order.PassengerNumber != Order.PMFee)
                            || (Convert.ToDecimal(dsReson.Tables[1].Rows[0]["rate"].ToString()) < Order.OldPolicyPoint))
                        {
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "于 " + DateTime.Now + " 票盟自动代付失败：平台订单价格和本地价格不符，不进行代付！";
                            OrderLog.WatchType = 2;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                            #endregion
                            OnErrorNew("票盟平台订单价格和本地价格不符，平台票面价：" + dsReson.Tables[1].Rows[0]["ticketfee"].ToString()
                                + "，平台返点：" + dsReson.Tables[1].Rows[0]["rate"].ToString() + "，不进行代付，本地订单号：" + Order.OrderId, false);
                            return false;
                        }
                        DataSet dsResonPay = PMService.PMPay(Order.OutOrderId, pmAccout, pmAg);
                        if (dsResonPay != null)
                        {
                            string mesPMPay = "";
                            for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                            {
                                for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                                {
                                    mesPMPay = mesPMPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                                }

                                mesPMPay = mesPMPay + "|";
                            }
                            OnErrorNew(mesPMPay, false);
                            if (dsResonPay.Tables[0].Rows[0]["statuscode"].ToString() == "0")
                            {
                                sql += " OutOrderPayFlag=1,PayStatus=1";
                                sql += " where OrderId='" + Order.OrderId + "'";
                                sqlbase.ExecuteNonQuerySQLInfo(sql);

                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "票盟自动代付成功!";
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                            }
                            else
                            {
                                #region 记录操作日志
                                //添加操作订单的内容
                                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OrderId = Order.OrderId;
                                OrderLog.OperType = "修改";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperContent = "于 " + DateTime.Now + " 票盟代付失败：" + dsResonPay.Tables[0].Rows[0]["statuscode"].ToString() + ":" + dsResonPay.Tables[0].Rows[0]["resp_Text"].ToString();
                                OrderLog.WatchType = 2;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                                #endregion
                                return false;
                            }

                        }
                    }

                }
                else
                {
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 票盟生成失败：" + dsReson.Tables[0].Rows[0]["statuscode"].ToString() + ":" + dsReson.Tables[0].Rows[0]["resp_Text"].ToString();
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("票盟线下异常:" + ex.Message+"，本地订单号："+Order.OrderId, false);
            IsPm = false;

            #region 记录操作日志
            //添加操作订单的内容
            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

            OrderLog.id = Guid.NewGuid();
            OrderLog.OrderId = Order.OrderId;
            OrderLog.OperType = "修改";
            OrderLog.OperTime = DateTime.Now;
            OrderLog.OperContent = "票盟线下异常:" + ex.Message;
            OrderLog.WatchType = 2;
            string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            sqlbase.ExecuteNonQuerySQLInfo(tempSql);
            #endregion
        }
        return IsPm;

    }
    #endregion

    #endregion
}