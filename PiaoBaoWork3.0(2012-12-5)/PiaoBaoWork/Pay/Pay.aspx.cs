using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using PbProject.Model;
using PbProject.Logic.Order;
using PbProject.Model.PayParam;
using PbProject.Logic.PID;
using PbProject.Logic.Pay;

/// <summary>
/// pay 支付页面
/// </summary>
public partial class Pay_Pay : BasePage
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                #region
                //string type = (Request.QueryString["type"] != null && !string.IsNullOrEmpty(Request.QueryString["type"].ToString()))
                //    ? Request.QueryString["type"].ToString() : "";
                //if (type=="0")
                //{
                //    TicketOrderPay(id, payWay, code); //机票订单支付
                //}
                //else if(type=="1")
                //{
                //    SmsOrderPay(id, payWay, code);//短信订单支付
                //}
                //else if (type=="2")
                //{
                //    OnlineRepaymentOrderPay(id, payWay, code);//在线充值订单支付
                //}
                #endregion

                if (Request.QueryString["param"] != null)
                {
                    string param = Request.QueryString["param"].ToString(); //没有中文不用 反编码
                    string[] vals = param.Split('&');

                    #region 参数取值

                    string type = ""; //1.类型
                    string id = "";//2.订单Id
                    string payWay = "";//3.支付方式：
                    string code = "";//4.网银银行代码
                    string price = "";//5.支付金额

                    for (int i = 0; i < vals.Length; i++)
                    {
                        if (vals[i] != "")
                        {
                            string[] temp = vals[i].Split('=');

                            switch (temp[0])
                            {
                                case "type":
                                    type = temp[1];
                                    break;
                                case "id":
                                    id = temp[1];
                                    break;
                                case "payway":
                                    payWay = temp[1];
                                    break;
                                case "code":
                                    code = temp[1];
                                    break;
                                case "price":
                                    price = temp[1];
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    #endregion

                    if (type == "0")//机票支付
                        TicketOrderPay(id, payWay, code);
                    else if (type == "1")//在线充值
                        OnlineRepaymentOrderPay(id, payWay, code, price);
                    else if (type == "2")//短信支付
                        SmsOrderPay(id, payWay, code, price);
                }
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('系统繁忙,请稍候再试！');", true);
        }
    }

    /// <summary>
    /// 机票支付
    /// </summary>
    /// <param name="id">订单</param>
    /// <param name="payWay">支付方式</param>
    /// <param name="code">网银银行代码</param>
    private void TicketOrderPay(string id, string payWay, string code)
    {
        string msgShow = "";
        string url = string.Empty;
        try
        {
            bool result = false;
            string payDetails = "";//分账明细

            #region 获取订单信息

            //PayWay 支付方式：1支付宝、2快钱、3汇付、4财付通、5支付宝网银、6快钱网银 、7汇付网银、8财付通网银、
            //9支付宝pos、10快钱pos、11汇付pos、12财付通pos、13易宝pos、14账户支付、15收银
            //string tempSqlWhere = " OrderId='" + id + "'";

            string tempSqlWhere = " id='" + id + "'";
            List<Tb_Ticket_Order> bParametersList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere(tempSqlWhere);
            Tb_Ticket_Order mOrder = null;

            if (bParametersList != null && bParametersList.Count == 1)
            {
                mOrder = bParametersList[0];

                #region 判断PNR状态是否取消等，取消就不能支付
                if (mOrder.OrderStatusCode != 1 && mOrder.OrderStatusCode != 9)
                    msgShow = "订单异常,不能支付!";
                #endregion

                if (this.KongZhiXiTong.Contains("|101|"))
                {
                    //超过 1 小时后的订单能支付
                }
                else
                {
                    #region  超过1个小时不能支付
                    if (mOrder.OrderStatusCode == 1)
                    {
                        DateTime dtTime = DateTime.Now;
                        if (dtTime.CompareTo(mOrder.CreateTime.AddHours(1)) > 0)
                        {
                            // 超过1个小时不能支付
                            msgShow = "超过1个小时的订单不能支付,请重新生成订单进行支付!";
                        }
                    }
                    #endregion
                }

                #region 判断PNR状态是否取消等，取消就不能支付

                //扩展参数
                ParamEx pe = new ParamEx();
                pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                SendInsManage sendins = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, this.configparam);
                string ErrMsg;

                if (mOrder.OrderSourceType == 1 && mOrder.A9 != "1")
                {
                    PnrAnalysis.PnrModel PnrModel = sendins.GetPnr(mOrder.PNR, mOrder.Office, out ErrMsg);

                    if (ErrMsg.IndexOf("已被取消") != -1)
                    {
                        msgShow = "此PNR（" + mOrder.PNR + "）状态为异常,请检查PNR状态后再试！";
                    }
                    else if (PnrModel != null && ErrMsg == "")
                    {
                        if (PnrModel.PassengerNameIsCorrent)
                        {
                            if (KongZhiXiTong != null && KongZhiXiTong.Contains("|23|"))
                            {
                                if ((PnrModel._OldPnrContent.Contains("THIS PNR WAS ENTIRELY CANCELLED") && PnrModel._OldPnrContent.ToUpper().Contains(mOrder.PNR.ToUpper())))
                                {
                                    msgShow = "此PNR（" + mOrder.PNR + "）状态为异常,请检查PNR状态后再试！";
                                }
                                else
                                {
                                    string PnrStatus = PnrModel.PnrStatus;
                                    if (string.IsNullOrEmpty(PnrStatus))
                                    {
                                        msgShow = "此PNR（" + mOrder.PNR + "）数据异常,请检查PNR状态后再试！";
                                    }
                                    else
                                    {
                                        if (!PnrStatus.Contains("HK") && !PnrStatus.Contains("DK") && !PnrStatus.Contains("RR") && !PnrStatus.Contains("KK"))
                                        {
                                            msgShow = "此PNR（" + mOrder.PNR + "）状态为：" + PnrStatus + "，请检查PNR状态后再试！";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            msgShow = "此PNR（" + mOrder.PNR + "）中乘机人名字：" + string.Join(",", PnrModel.ErrorPassengerNameList.ToArray()) + " 不正确！";
                        }
                    }
                }

                #endregion 判断PNR状态是否取消等，取消就不能支付

                if (msgShow == "")
                {

                    mOrder.PayWay = int.Parse(payWay);

                    // 切换支付方式：修改订单账单明细数据
                    result = new PbProject.Logic.Pay.Bill().UpdateOrderAndTicketPayDetail(mOrder);

                    if (result == true)
                    {
                        //获取分账明细
                         payDetails = new PbProject.Logic.Pay.Bill().PayDetails(payWay, mOrder.OrderId);
                        result = string.IsNullOrEmpty(payDetails) ? false : result;
                    }
                }
            }

            #endregion

            if (result)
            {
                #region 支付

                if (payWay == "1" || payWay == "5")//支付宝
                {
                    #region 支付参数赋值

                    AliPayParam aliPayParam = new AliPayParam();

                    aliPayParam.Body = "机票订单";
                    aliPayParam.DefaultBank = code;
                    aliPayParam.Extra_common_param = mUser.id.ToString(); //自定义：操作人id
                    aliPayParam.Out_trade_no = mOrder.OrderId;
                    aliPayParam.Royalty_parameters = payDetails; //分账明细
                    aliPayParam.Subject = "机票订单";
                    aliPayParam.Total_fee = mOrder.PayMoney.ToString("f2");


                    if ((!string.IsNullOrEmpty(mOrder.A9) && mOrder.A9 == "1") || string.IsNullOrEmpty(mOrder.PNR))
                    {
                        //不检查编码

                        OnErrorNew("不检查编码,PNR:" + mOrder.PNR, false);
                    }
                    else
                    {
                        //需要检查编码
                        #region 判断PNR

                        tempSqlWhere = " PNR='" + mOrder.PNR + "' and OrderStatusCode=4 and (PayWay=1 or PayWay=5)";
                        List<Tb_Ticket_Order> orderListNew = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere(tempSqlWhere);

                        if (orderListNew == null || orderListNew.Count == 0)
                        {
                            //先看该订单支付已经支付过
                            aliPayParam.Extend_param = "PNR^" + mOrder.PNR;
                        }
                        else
                        {
                            OnErrorNew("该编码已经有支付记录,PNR:" + mOrder.PNR, false);
                        }

                        #endregion
                    }

                    #endregion

                    msgShow = new PbProject.Logic.Pay.AliPay().GetPay(aliPayParam);
                }
                else if (payWay == "2" || payWay == "6")//快钱
                {
                    PbProject.Logic.Pay.DataAction d = new PbProject.Logic.Pay.DataAction();
                    decimal tempMoney = d.FourToFiveNum(mOrder.PayMoney * 0.001M, 2) * 100; //手续费

                    #region 支付参数赋值
                    _99BillParam billParam = new _99BillParam();
                    billParam.Bankcode = code;
                    billParam.Detail = payDetails;//分账明细
                    billParam.Ext = mUser.id.ToString();//操作人id
                    billParam.Money = tempMoney.ToString("f0"); //供应商收款
                    billParam.Orderid = mOrder.OrderId;
                    //billParam.Payid = "1076090377@qq.com";
                    billParam.Payid = "";
                    billParam.Paytype = (string.IsNullOrEmpty(code)) ? "00" : "10";
                    billParam.Pname = "机票订单";
                    billParam.Price = (mOrder.PayMoney * 100).ToString("f0"); //供应商收款
                    billParam.Ptype = "1";

                    #endregion

                    msgShow = new PbProject.Logic.Pay._99Bill().GetPay(billParam);
                }
                else if (payWay == "3" || payWay == "7")//汇付天下
                {
                    #region 支付参数赋值

                    ChinaPnrParam chinaPnrParam = new ChinaPnrParam();
                    chinaPnrParam.Orderid = mOrder.OrderId;//订单号
                    chinaPnrParam.Price = mOrder.PayMoney.ToString("f2");//订单总价
                    chinaPnrParam.Merpriv = mUser.id.ToString(); //自定义字段
                    chinaPnrParam.Details = payDetails; //分账明细
                    chinaPnrParam.Pnr = mOrder.PNR;

                    #endregion

                    msgShow = new PbProject.Logic.Pay.ChinaPnr().Buy(chinaPnrParam);
                }
                else if (payWay == "4" || payWay == "8" || payWay=="40")// 财付通
                {
                    string queryParam = string.Format("orderid={0}&total_tee={1}&userhostaddress={2}&attach={3}&busargs={4}&busdesc={5}&banktype={6}",
                        mOrder.OrderId, (mOrder.PayMoney * 100).ToString("F0"), Request.UserHostAddress, mUser.id.ToString(), payDetails, string.Format("{0}^{1}^{2}^{3}^{4}^{5}", mOrder.PNR, mOrder.Travel, mOrder.PassengerNumber, mUser.id.ToString(), mOrder.CreateUserName, "13800000000"), code);
                    url = string.Format("http://lzh.mypb.cn/Pay/TenPay.aspx?{0}", queryParam);
                   // url = string.Format("TenPay.aspx?{0}", queryParam);
                  
                }

                #endregion
            }
            else
            {
                OnErrorNew("支付错误:" + payDetails, false);

                msgShow = string.IsNullOrEmpty(msgShow) ? "支付错误" : msgShow;
            }
        }
        catch (Exception ex)
        {
            msgShow = "支付异常";

            OnErrorNew(msgShow + ex.ToString(), false);
        }
        if (!string.IsNullOrEmpty(url))
        {
            Response.Redirect(url);
        }
        else
        {
            OnErrorNew(msgShow, false);
            Response.Write(msgShow);
        }
    }

    /// <summary>
    /// 短信支付
    /// </summary>
    /// <param name="id">订单id</param>
    /// <param name="payWay">支付方式</param>
    /// <param name="code">网银银行代码</param>
    private void SmsOrderPay(string id, string payWay, string code, string prices)
    {
        string url = string.Empty;
        string strFromValue = "";
        PbProject.Logic.Pay.DataAction data = new PbProject.Logic.Pay.DataAction();
        Tb_Ticket_OrderBLL ticket_Order = new Tb_Ticket_OrderBLL();

        string orderId = id;//订单编号
        string price = prices;//订单金额

        //string payCpyNo = mCompany.UninCode; //付款公司编号
        string payId = mUser != null ? mUser.id.ToString() : ""; //付款公司编号
        string act = UseAct(payWay,mUser.CpyNo); //收款账号
        try
        {
            //PayWay 支付方式：1支付宝、2快钱、3汇付、4财付通、5支付宝网银、6快钱网银 、7汇付网银、8财付通网银、
            //9支付宝pos、10快钱pos、11汇付pos、12财付通pos、13易宝pos、14账户支付、15收银
            if (payWay == "1" || payWay == "5")//支付宝
            {
                #region 支付宝

                #region 参数赋值

                decimal total = decimal.Parse(price); //订单支付金额
                decimal supperates = 0.001M; //支付费率
                decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                decimal actPrice = total - paySXF; //收款账号金额
                string detail = act + "^" + actPrice.ToString("F2") + "^充值收款";
                #endregion

                AliPayParam aliPayParam = new AliPayParam();
                aliPayParam.Out_trade_no = orderId;  //内部订单号
                aliPayParam.Subject = "在线充值";//商品名称
                aliPayParam.Body = "在线充值";//商品描述
                aliPayParam.Total_fee = total.ToString();  //订单总价
                aliPayParam.Royalty_parameters = detail;//分润参数
                aliPayParam.Extend_param = ""; //公用业务扩展参数，支付宝用于 显示 PNR （格式：参数名1^参数值1|参数名2^参数值2|......）
                aliPayParam.Extra_common_param = payId;//自定义字段
                aliPayParam.DefaultBank = code; //网银标示

                strFromValue = new PbProject.Logic.Pay.AliPay().GetPay(aliPayParam);
                #endregion
            }
            else if (payWay == "2" || payWay == "6")//快钱
            {
                #region 快钱

                #region 参数赋值
                decimal total = decimal.Parse(price); //订单支付金额
                decimal supperates = 0.001M; //支付费率
                decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                decimal actPrice = total - paySXF; //收款账号金额

                int totalInt = int.Parse((total * 100).ToString().Split('.')[0]);
                int paySXFInt = int.Parse((paySXF * 100).ToString().Split('.')[0]);
                int actPriceInt = int.Parse((actPrice * 100).ToString().Split('.')[0]);

                string detail = "1^" + act + "^" + actPriceInt.ToString() + "^0^充值收款";
                #endregion

                _99BillParam billParam = new _99BillParam();
                billParam.Orderid = orderId;//0.订单编号
                billParam.Price = totalInt.ToString();//1.订单金额，单位“分”
                billParam.Money = paySXFInt.ToString(); ;//2.手续费，单位“分”
                billParam.Pname = "在线充值";//3.商品名称
                billParam.Ext = payId;//4.自定义字段
                billParam.Detail = detail;//5.分润数据集
                billParam.Ptype = "1";//6.分润类别，1 立刻分润 0 异步分润
                billParam.Payid = "1076090377@qq.com"; ;//7.付款帐户
                billParam.Paytype = (string.IsNullOrEmpty(code)) ? "00" : "10";//8.  00：组合支付，10：银行卡支付
                billParam.Bankcode = code;//9: 银行代码
                strFromValue = new PbProject.Logic.Pay._99Bill().GetPay(billParam);
                #endregion
            }
            else if (payWay == "3" || payWay == "7")//汇付天下
            {
                #region 汇付天下

                #region 参数赋值
                decimal total = decimal.Parse(price); //订单支付金额
                decimal supperates = 0.001M; //支付费率
                decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                decimal actPrice = total - paySXF; //收款账号金额
                string detail = "Agent:" + act + ":" + actPrice;
                #endregion

                ChinaPnrParam chinaPnrParam = new ChinaPnrParam();
                chinaPnrParam.Orderid = orderId;//订单号
                chinaPnrParam.Price = total.ToString("f2");//订单总价
                chinaPnrParam.Merpriv = payId; //自定义字段
                chinaPnrParam.Details = detail;
                strFromValue = new PbProject.Logic.Pay.ChinaPnr().Buy(chinaPnrParam);

                #endregion
            }
            else if (payWay == "4" || payWay == "8")// 财付通
            {
                #region 财付通

                #region 参数赋值
                string actXSF = UseAct(payWay, mUser.CpyNo.Substring(0, 12)); //手续费收款账号
                decimal total = decimal.Parse(price); //订单支付金额
                decimal supperates = 0.001M; //支付费率
                decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                decimal actPrice = total - paySXF; //收款账号金额
                total = total * 100;
                string busdesc=string.Empty;
                if (paySXF > 0)
                    busdesc = string.Format("{0}^{1}^1|{2}^{3}^2", act, (actPrice * 100).ToString("F0"), actXSF, (paySXF * 100).ToString("F0"));
                else
                    busdesc = string.Format("{0}^{1}^1", act, (actPrice * 100).ToString("F0"));
                //TenPayParam tenPayParam = new TenPayParam();
                //tenPayParam.Bus_Args = act + "^" + actPrice + "^1|";
                //// tenPayParam.Bus_Desc = "12345^深圳-上海^1^fady^庄^13800138000";///业务描述，特定格式的字符串，格式为：PNR^航程^机票张数^机票销售商在机票平台的id^联系人姓名^联系电话
                //tenPayParam.Bus_Desc = "测试";
                //tenPayParam.Desc = "在线充值";
                //tenPayParam.Orderid = orderId;
                //tenPayParam.Total_Tee = total.ToString("F0");
                //tenPayParam.UserHostAddress = Page.Request.UserHostAddress;

                //strFromValue = new PbProject.Logic.Pay.TenPay().SplitPayRequest(tenPayParam);

                string queryParam = string.Format("orderid={0}&total_tee={1}&userhostaddress={2}&attach={3}&busargs={4}&busdesc={5}",
                        orderId, total.ToString("F0"), Request.UserHostAddress, mUser.id.ToString(),busdesc, "短信支付");//,string.Format("{0}^{1}^{2}^{3}^{4}^{5}", mOrder.PNR, mOrder.Travel, mOrder.PassengerNumber, mUser.id.ToString(), mOrder.CreateUserName, "13800000000"));
                url = string.Format("http://lzh.mypb.cn/Pay/TenPay.aspx?{0}", queryParam);

                #endregion

                #endregion
            }
            else if (payWay == "14")//账户余额
            {
                #region 账户余额
                if (mCompany != null)
                {
                    List<User_Company> listSmsUser = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + mCompany.UninCode + "'" }) as List<User_Company>;
                    decimal maxMoney = listSmsUser[0].MaxDebtMoney;//最大欠款额度
                    decimal oldAccountMoney = listSmsUser[0].AccountMoney; // 支付前
                    decimal newAccountMoney = oldAccountMoney + maxMoney - decimal.Parse(prices);// 支付后
                   
                    if (newAccountMoney >= 0)
                    {
                        PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
                        strFromValue = bill.MakeSMS(id, "", 14) == true ? "充值成功" : "充值失败";
                    }
                    else
                    {
                        strFromValue = "账户余额不足";
                    }
                }
                #endregion
            }
        }
        catch (Exception)
        {
            strFromValue = "支付异常";
        }

        if (!string.IsNullOrEmpty(url))
            Response.Redirect(url);
        if (!string.IsNullOrEmpty(strFromValue))
        {
            OnErrorNew(strFromValue, false);
        }

        Response.Write(strFromValue);

    }

    /// <summary>
    /// 在线充值
    /// </summary>
    /// <param name="id">订单id</param>
    /// <param name="payWay">支付方式</param>
    /// <param name="code">网银银行代码</param>
    /// <param name="price">充值金额</param>
    private void OnlineRepaymentOrderPay(string id, string payWay, string code, string price)
    {
        string strFromValue = "";
        var url = string.Empty;
        try
        {
            PbProject.Logic.Pay.DataAction data = new PbProject.Logic.Pay.DataAction();
            Tb_Ticket_OrderBLL ticket_Order = new Tb_Ticket_OrderBLL();
            //string indexId = ticket_Order.GetIndexId();//内部流水号
            string orderId = ticket_Order.GetOrderId("1");//订单编号
            //string payCpyNo = mCompany.UninCode; //付款公司编号

            string payId = mUser != null ? mUser.id.ToString() : ""; //付款公司编号
            string act = UseAct(payWay,mUser.CpyNo); //收款账号

            if (!string.IsNullOrEmpty(payId) && !string.IsNullOrEmpty(act))
            {
                //添加交易日志
                //bool reuslt = new PbProject.Logic.Pay.Bill().InsertLogMoneyDetail(orderId, indexId, price, payWay, mUser, mCompany);

                if (payWay == "1" || payWay == "5")//支付宝
                {
                    #region 支付宝

                    #region 参数赋值

                    decimal total = decimal.Parse(price); //订单支付金额
                    decimal supperates = 0.001M; //支付费率
                    decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                    decimal actPrice = total - paySXF; //收款账号金额
                    string detail = act + "^" + actPrice.ToString("F2") + "^充值收款";
                    #endregion

                    AliPayParam aliPayParam = new AliPayParam();
                    aliPayParam.Out_trade_no = orderId;  //内部订单号
                    aliPayParam.Subject = "在线充值";//商品名称
                    aliPayParam.Body = "在线充值";//商品描述
                    aliPayParam.Total_fee = total.ToString();  //订单总价
                    aliPayParam.Royalty_parameters = detail;//分润参数
                    aliPayParam.Extend_param = ""; //公用业务扩展参数，支付宝用于 显示 PNR （格式：参数名1^参数值1|参数名2^参数值2|......）
                    aliPayParam.Extra_common_param = payId;//自定义字段
                    aliPayParam.DefaultBank = code; //网银标示

                    strFromValue = new PbProject.Logic.Pay.AliPay().GetPay(aliPayParam);
                    #endregion
                }
                else if (payWay == "2" || payWay == "6")//快钱
                {
                    #region 快钱

                    #region 参数赋值
                    decimal total = decimal.Parse(price); //订单支付金额
                    decimal supperates = 0.001M; //支付费率
                    decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                    decimal actPrice = total - paySXF; //收款账号金额

                    int totalInt = int.Parse((total * 100).ToString().Split('.')[0]);
                    int paySXFInt = int.Parse((paySXF * 100).ToString().Split('.')[0]);
                    int actPriceInt = int.Parse((actPrice * 100).ToString().Split('.')[0]);

                    string detail = "1^" + act + "^" + actPriceInt.ToString() + "^0^充值收款";
                    #endregion

                    _99BillParam billParam = new _99BillParam();
                    billParam.Orderid = orderId;//0.订单编号
                    billParam.Price = totalInt.ToString();//1.订单金额，单位“分”
                    billParam.Money = paySXFInt.ToString(); ;//2.手续费，单位“分”
                    billParam.Pname = "在线充值";//3.商品名称
                    billParam.Ext = payId;//4.自定义字段
                    billParam.Detail = detail;//5.分润数据集
                    billParam.Ptype = "1";//6.分润类别，1 立刻分润 0 异步分润
                    billParam.Payid = "1076090377@qq.com"; ;//7.付款帐户
                    billParam.Paytype = (string.IsNullOrEmpty(code)) ? "00" : "10";//8.  00：组合支付，10：银行卡支付
                    billParam.Bankcode = code;//9: 银行代码
                    strFromValue = new PbProject.Logic.Pay._99Bill().GetPay(billParam);
                    #endregion
                }
                else if (payWay == "3" || payWay == "7")//汇付天下
                {
                    #region 汇付天下

                    #region 参数赋值
                    decimal total = decimal.Parse(price); //订单支付金额
                    decimal supperates = 0.001M; //支付费率
                    decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                    decimal actPrice = total - paySXF; //收款账号金额
                    string detail = "Agent:" + act + ":" + actPrice;
                    #endregion

                    ChinaPnrParam chinaPnrParam = new ChinaPnrParam();
                    chinaPnrParam.Orderid = orderId;//订单号
                    chinaPnrParam.Price = total.ToString("f2");//订单总价
                    chinaPnrParam.Merpriv = payId; //自定义字段
                    chinaPnrParam.Details = detail;
                    strFromValue = new PbProject.Logic.Pay.ChinaPnr().Buy(chinaPnrParam);

                    #endregion
                }
                else if (payWay == "4" || payWay == "8" || payWay=="40")// 财付通
                {
                    #region 财付通

                    #region 参数赋值
                    
                    string actXSF = UseAct(payWay, mUser.CpyNo.Substring(0, 12)); //手续费收款账号
                    decimal total = decimal.Parse(price); //订单支付金额
                    decimal supperates = 0.001M; //支付费率
                    decimal paySXF = data.FourToFiveNum(total * supperates, 2); //支付手续费
                    decimal actPrice = total - paySXF; //收款账号金额

                    total = total * 100;

                    //TenPayParam tenPayParam = new TenPayParam();
                    //tenPayParam.Bus_Args = string.Format("{0}^{1}^1|{2}^{3}^2", act, actPrice, actXSF, paySXF);
                    ////tenPayParam.Bus_Desc = "12345^深圳-上海^1^fady^庄^13800138000";///业务描述，特定格式的字符串，格式为：PNR^航程^机票张数^机票销售商在机票平台的id^联系人姓名^联系电话
                    //tenPayParam.Bus_Desc = "机票充值";
                    //tenPayParam.Desc = "在线充值";
                    //tenPayParam.Orderid = orderId;
                    //tenPayParam.Total_Tee = total.ToString("F0");
                    //tenPayParam.UserHostAddress = Page.Request.UserHostAddress;

                    string queryParam = string.Format("orderid={0}&total_tee={1}&userhostaddress={2}&attach={3}&busargs={4}&busdesc={5}&banktype={6}",
                        orderId, total.ToString("F0"), Request.UserHostAddress, mUser.id.ToString(), string.Format("{0}^{1}^1|{2}^{3}^2", act, (actPrice * 100).ToString("F0"), actXSF, (paySXF * 100).ToString("F0")), "在线充值", code);//,string.Format("{0}^{1}^{2}^{3}^{4}^{5}", mOrder.PNR, mOrder.Travel, mOrder.PassengerNumber, mUser.id.ToString(), mOrder.CreateUserName, "13800000000"));
                    url = string.Format("http://lzh.mypb.cn/Pay/TenPay.aspx?{0}", queryParam);

                    //strFromValue = new PbProject.Logic.Pay.TenPay().SplitPayRequest(tenPayParam);

                    #endregion

                    #endregion
                }
            }
            else
            {
                strFromValue = "收款方账号错误!";
            }
        }
        catch (Exception)
        {
            strFromValue = "收款方账号异常!";
        }

        if (!string.IsNullOrEmpty(url))
        {
            Response.Redirect(url);
        }
        if (!string.IsNullOrEmpty(strFromValue))
        {
            OnErrorNew(strFromValue, false);
        }
        Response.Write(strFromValue);
    }

    /// <summary>
    /// 获取充值收款账号
    /// </summary>
    /// <param name="payWay">支付方式:</param>
    /// <returns></returns>
    private string UseAct(string payWay, string mUserCpyNo)
    {
        string act = "";
        try
        {
              string gYcpyNo="";

              if (mUser != null && !string.IsNullOrEmpty(mUser.CpyNo))
              {
                  if (mUserCpyNo.Length == 12)
                  {
                      gYcpyNo = mUserCpyNo.Substring(0, 6);
                  }
                  else
                  {
                      gYcpyNo = mUserCpyNo.Substring(0, 12);
                  }
              }


            
            string wangYinZhangHao = PbProject.Model.definitionParam.paramsName.wangYinZhangHao;
            string wangYinLeiXing = PbProject.Model.definitionParam.paramsName.wangYinLeiXing;
            string sqlWhere = " CpyNo='" + gYcpyNo + "' and SetName='" + wangYinZhangHao + "'";
            List<Bd_Base_Parameters> bParametersList = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL().GetParametersListByWhere(sqlWhere);

            #region 测试数据

            /*List<Bd_Base_Parameters> bParametersList = new List<Bd_Base_Parameters>();
            Bd_Base_Parameters ts = new Bd_Base_Parameters();
            ts.SetName = wangYinZhangHao;
            ts.SetValue = "jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|";
            bParametersList.Add(ts);

            Bd_Base_Parameters ts1 = new Bd_Base_Parameters();
            ts1.SetName = wangYinLeiXing;
            ts1.SetValue = "6";
            bParametersList.Add(ts1);
             */
            #endregion

            if ((bParametersList != null && bParametersList.Count == 1)
                && !string.IsNullOrEmpty(bParametersList[0].SetValue) && bParametersList[0].SetValue.Contains("|"))
            {
                string[] setValues = bParametersList[0].SetValue.Split('|');

                if (setValues.Length > 3)
                {
                    int temp = -1;
                    switch (payWay)
                    {
                        #region 支付方式
                        case "1":
                        case "5":
                            temp = 0;
                            break;
                        case "2":
                        case "6":
                            temp = 1;
                            break;
                        case "3":
                        case "7":
                            temp = 2;
                            break;
                        case "4":
                        case "8":
                        case "40":
                            temp = 3;
                            break;
                        default:
                            break;
                        #endregion
                    }
                    if (temp != -1)
                    {
                        string[] setValuesNewS = setValues[temp].Split('^');
                        if (setValuesNewS.Length != 0 && !string.IsNullOrEmpty(setValuesNewS[1]))
                            act = setValuesNewS[1];
                    }
                }
            }
        }
        catch (Exception)
        {

        }
        return act.Trim();
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

            #region 记录文本日志

            /*    
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("记录时间：" + DateTime.Now.ToString() + "\r\n");
            sb.AppendFormat("  IP ：" + Page.Request.UserHostAddress + "\r\n");
            sb.AppendFormat("  Content : " + errContent + "\r\n");

            if (IsRecordRequest)
            {
                #region 记录 Request 参数
                try
                {
                    sb.AppendFormat("  Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");

                    if (HttpContext.Current.Request != null)
                    {
                        if (HttpContext.Current.Request.HttpMethod == "POST")
                        {
                            #region POST 提交
                            if (HttpContext.Current.Request.Form.Count != 0)
                            {
                                for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
                                {
                                    sb.AppendFormat(HttpContext.Current.Request.Form.Keys[i].ToString() + " = " + HttpContext.Current.Request.Form[i].ToString() + "\r\n");
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" HttpContext.Current.Request.Form.Count = 0 \r\n");
                            }

                            #endregion
                        }
                        else if (HttpContext.Current.Request.HttpMethod == "GET")
                        {
                            #region GET 提交

                            if (HttpContext.Current.Request.QueryString.Count != 0)
                            {
                                for (int i = 0; i < HttpContext.Current.Request.QueryString.Count; i++)
                                {
                                    sb.AppendFormat(HttpContext.Current.Request.QueryString.Keys[i].ToString() + " = " + HttpContext.Current.Request.QueryString[i].ToString() + "\r\n");
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" HttpContext.Current.QueryString.Form.Count = 0 \r\n");
                            }

                            #endregion
                        }
                        else
                        {
                            #region 不是 GET 和 POST

                            sb.AppendFormat("  不是 GET 和 POST, Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");

                            System.Collections.Specialized.NameValueCollection nv = Request.Params;
                            foreach (string key in nv.Keys)
                            {
                                sb.AppendFormat("{0}={1} \r\n", key, nv[key]);
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        sb.AppendFormat("  HttpContext.Current.Request=null \r\n");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendFormat("  catch: " + ex + "\r\n");
                }

                #endregion
            }

            sb.AppendFormat("----------------------------------------------------------------------------------------------------\r\n");
            sb.AppendFormat("----------------------------------------------------------------------------------------------------");

            string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\" + Page + "\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
            fs.WriteLine(sb.ToString());
            fs.Close();
             */

            #endregion
        }
        catch (Exception)
        {

        }
    }
}