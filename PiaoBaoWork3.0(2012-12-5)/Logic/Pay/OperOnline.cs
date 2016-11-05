using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using System.Xml.Linq;
using PbProject.Model.PayParam;

namespace PbProject.Logic.Pay
{
    /// <summary>
    /// 退款处理
    /// </summary>
    public class OperOnline
    {
        PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();

        /// <summary>
        /// 退款处理
        /// </summary>
        /// <param name="mOrder">订单 Model</param>
        /// <param name="uEmployees">用户 Model</param>
        /// <param name="uCompany">公司 Model</param>
        /// <returns></returns>
        public bool TitckOrderRefund(Tb_Ticket_Order mOrder, User_Employees uEmployees, User_Company uCompany, out string msg)
        {
            bool result = false;
            msg = "";

            try
            {
                if (mOrder.PayWay == 1 || mOrder.PayWay == 5)
                {
                    #region 支付宝

                    AliPay aliPay = new AliPay();
                    string strTime = DateTime.Now.ToString("yyyyMMdd");

                    string bno = strTime + mOrder.OrderId + DateTime.Now.ToString("HHmm");

                    string detail_data = "";

                    string[] Details = new string[3];
                    Details[0] = bno;//批次号规则
                    Details[1] = "1";//1.要退款的支付宝交易号
                    //2.退款参数
                    detail_data = mOrder.PayNo + "^" + mOrder.PayMoney.ToString("F2") + "^退款|" + "";

                    #region 分账信息

                    string tempSqlWhere = " OrderId='" + mOrder.OrderId + "'";
                    List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);

                    payDetailList = bill.OnLinePayDetails(payDetailList);

                    #endregion 分账信息

                    Tb_Order_PayDetail payDetail = null;
                    decimal realPayMoney = 0;

                    for (int i = 0; i < payDetailList.Count; i++)
                    {
                        payDetail = payDetailList[i];
                        realPayMoney = payDetail.RealPayMoney;

                        if (payDetail.PayType == "付款" || realPayMoney == 0)
                            continue;

                        detail_data += payDetail.PayAccount + "^^" + aliPay._serveremail + "^^" + realPayMoney.ToString("F2") + "^退" + payDetail.PayType + "|";
                    }
                    detail_data = detail_data.TrimEnd('|');
                    Details[2] = detail_data;

                    result = aliPay.IsRefund(Details);

                    if (result)
                    {
                        bill.CreateBillRefundFailedLog(uEmployees, mOrder.OrderId, "提交退款成功,等待退款......"); //退款失败
                    }
                    else
                    {
                        bill.CreateBillRefundFailedLog(uEmployees, mOrder.OrderId, "提交退款失败"); //退款失败
                    }

                    #endregion
                }
                else if (mOrder.PayWay == 2 || mOrder.PayWay == 6)
                {
                    #region 快钱

                    _99Bill _99bill = new _99Bill();
                    string dataTime = DateTime.Now.ToString("yyyyMMddHHmmss");

                    string detail_data = "";
                    decimal total = mOrder.PayMoney; //退款金额

                    int orderPrice = int.Parse((total * 100).ToString().Split('.')[0]);//退款金额 分为单位
                    int tempPrice = 0;

                    #region 分账信息

                    string tempSqlWhere = " OrderId='" + mOrder.OrderId + "'";
                    List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);

                    payDetailList = bill.OnLinePayDetails(payDetailList);

                    #endregion 分账信息

                    Tb_Order_PayDetail payDetail = null;
                    decimal realPayMoney = 0;
                    int allTempPrice = 0;

                    for (int i = 0; i < payDetailList.Count; i++)
                    {
                        payDetail = payDetailList[i];
                        realPayMoney = payDetail.RealPayMoney;

                        if (payDetail.PayType == "付款" || realPayMoney == 0)
                            continue;

                        tempPrice = int.Parse((realPayMoney * 100).ToString().Split('.')[0]);//退款金额 分为单位

                        //"1^kqcom06@sina.com^5000^爱的|1^kqcom02@sina.com^7000^分账1|1^kqcomsh@sina.com^8000^分账2";
                        // Details[3] = "1^" + LinkEmail + "^" + p2 + "^Refund|1^" + account + "^" + p1 + "^Refund";

                        detail_data += "1^" + payDetail.PayAccount + "^" + tempPrice + "^Refund|";

                        allTempPrice += tempPrice;
                    }
                    tempPrice = orderPrice - allTempPrice;
                    detail_data += "1^" + _99bill.LinkEmail + "^" + tempPrice + "^Refund|"; //主账号退款 , 用于退款平衡

                    detail_data = detail_data.TrimEnd('|');

                    string[] Details = new string[6];

                    string OldOrderId = (string.IsNullOrEmpty(mOrder.OldOrderId)) ? mOrder.OrderId : mOrder.OldOrderId;
                    Details[0] = OldOrderId; // 订单编号(原订单号)
                    Details[1] = orderPrice.ToString(); // 订单金额
                    Details[2] = "系统退款"; //备注
                    Details[3] = detail_data; //退款明细
                    Details[4] = mOrder.OrderId; // 订单编号(退废订单编号)
                    Details[5] = DateTime.Now.ToString("yyyyMMddHHmmss");  //退款流水号

                    string str = _99bill.Refund(Details);

                    //PbProject.WebCommon.Log.Log.RecordLog("RefundOper", str, false, null);//日志

                    result = _99bill.IsRefund(str);

                    if (result == true)
                    {
                        //退款成功，处理订单状态
                        string indexno = Details[5] + Details[4];  // 退款成功流水号
                        bill.CreateBillRefund(mOrder.OrderId, indexno, 2, "在线退款", "快钱退款", str);
                    }
                    else
                    {
                        //退款失败日志
                        bill.CreateBillRefundFailedLog(uEmployees, mOrder.OrderId, "退款失败，请检查:" + str); //退款失败
                    }

                    #endregion
                }
                else if (mOrder.PayWay == 3 || mOrder.PayWay == 7)
                {
                    #region 汇付

                    ChinaPnr chinaPnr = new ChinaPnr();

                    string dataTime = DateTime.Now.ToString("yyyyMMddHHmmss");

                    string detail_data = "";
                    decimal total = mOrder.PayMoney; //退款金额


                    #region 分账信息

                    string tempSqlWhere = " OrderId='" + mOrder.OrderId + "'";
                    List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);

                    payDetailList = bill.OnLinePayDetails(payDetailList);

                    #endregion 分账信息

                    Tb_Order_PayDetail payDetail = null;
                    decimal realPayMoney = 0;

                    for (int i = 0; i < payDetailList.Count; i++)
                    {
                        payDetail = payDetailList[i];
                        realPayMoney = payDetail.RealPayMoney;

                        if (payDetail.PayType == "付款" || realPayMoney == 0)
                            continue;

                        detail_data += "Agent:" + payDetail.PayAccount + ":" + realPayMoney.ToString("f2") + ";";
                    }

                    detail_data = detail_data.TrimEnd(';');

                    string strOrderOldID = string.IsNullOrEmpty(mOrder.OldOrderId) ? mOrder.OrderId : mOrder.OldOrderId;

                    string[] Details = new string[4];
                    Details[0] = mOrder.OrderId; //订单编号
                    Details[1] = strOrderOldID; //原订单编号
                    Details[2] = total.ToString();//退款总金额
                    Details[3] = detail_data;//退分润数据集

                    string value = chinaPnr.Refund(Details[0], Details[1], Details[2], Details[3]);

                    if (value.Contains("RespCode=000000") && value.Contains("ErrMsg=成功"))
                    {
                        result = true;
                    }

                    if (result == true)
                    {
                        string indexno = mOrder.OrderId;  // 退款成功流水号
                        bill.CreateBillRefund(mOrder.OrderId, indexno, 3, "在线退款", "汇付退款", value);
                    }
                    else
                    {
                        bill.CreateBillRefundFailedLog(uEmployees, mOrder.OrderId, "退款失败，请检查:" + value); //退款失败
                    }

                    #endregion
                }
                else if (mOrder.PayWay == 4 || mOrder.PayWay == 8 || mOrder.PayWay==40)
                {
                    #region 财付通
                    // 财付通 暂时不处理
                    TenPayParam tenPayParam = new TenPayParam();
                    //内部订单号
                    tenPayParam.Orderid = mOrder.OrderId;
                    //旧订单号
                    if (!String.IsNullOrEmpty(mOrder.OldOrderId))
                        tenPayParam.OldOrderid = mOrder.OldOrderId;
                    else
                        tenPayParam.OldOrderid = mOrder.OrderId;
                    //财付通交易号
                    tenPayParam.PayNo = mOrder.PayNo;
                    //总金额
                    Tb_Ticket_Order tbTicketOrder = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(tenPayParam.OldOrderid);
                    if (tbTicketOrder != null)
                    {
                        tenPayParam.Total_Tee = (tbTicketOrder.PayMoney * 100).ToString("F0");
                    }
                    else
                    {
                        tenPayParam.Total_Tee = (mOrder.PayMoney * 100).ToString("F0");
                    }
                    tenPayParam.Date = (mOrder.PayMoney * 100).ToString("F0");
                    /*---------------------分账信息----------------------------*/
                    string detail_data = string.Format("{0}|", (mOrder.PayMoney * 100).ToString("F0"));
                    string tempSqlWhere = " OrderId='" + mOrder.OrderId + "'";
                    Tb_Order_PayDetail payDetail = null;
                    decimal realPayMoney = 0;
                    List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);
                    /*
                    for (int i = 0; i < payDetailList.Count; i++)
                    {
                        payDetail = payDetailList[i];
                        if (payDetail.PayType == "付款")
                        {
                            tenPayParam.BackState = payDetail.A1;
                        }
                        if (payDetail.PayType == "手续费")
                        {
                            realPayMoney = payDetail.BuyPoundage;
                            detail_data += string.Format("{0}^{1}|", payDetail.PayAccount, (realPayMoney * 100).ToString("F0"));
                        }
                    }
                    */
                    //
                    payDetailList = bill.OnLinePayDetails(payDetailList);
                    for (int i = 0; i < payDetailList.Count; i++)
                    {
                        payDetail = payDetailList[i];
                        realPayMoney = payDetail.RealPayMoney;
                        
                        if (payDetail.PayType == "付款")
                        {
                            tenPayParam.BackState = payDetail.A1;
                            continue;
                        }
                        detail_data += string.Format("{0}^{1}|", payDetail.PayAccount, (realPayMoney * 100).ToString("F0"));
                            
                    }


                    detail_data = detail_data.TrimEnd('|');



                    //1000|(帐号^退款金额)|



                    /*----------------------------------------------------------*/
                    //退款分账
                    tenPayParam.Bus_Args = detail_data;
                    //分账退款
                    TenPay tenPay = new TenPay();
                    result = tenPay.ClientSplitRollback(tenPayParam);
                    #endregion
                }
                else if (mOrder.PayWay == 14)
                {
                    #region 账户支付

                    try
                    {
                        result = bill.CreateVirtualRefundBill(mOrder, uEmployees, uCompany, out msg);

                        if (!result)
                        {
                            //退款失败记录日志
                            bill.CreateBillRefundFailedLog(uEmployees, mOrder.OrderId, msg);
                        }
                    }
                    catch (Exception)
                    {
                        msg = "退款异常！";
                    }

                    #endregion
                }
                else if (mOrder.PayWay == 15)
                {
                    // 收银 暂时不处理
                    result = true;
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

        /// <summary>
        /// 支付查询
        /// </summary>
        /// <param name="payType">支付方式</param>
        /// <param name="oldOrderId">订单号</param>
        ///  <param name="IsOperOrder">true：更新订单等信息 、false：不跟新订单等信息</param>
        /// <param name="oldOrderId">out：提示消息</param>
        /// <returns></returns>
        public string PaySel(string payType, string oldOrderId, bool IsOperOrder, out string msgShow)
        {
            string strValue = "";
            msgShow = "";

            try
            {
                string PayNo = ""; //交易号
                decimal PayMoney = 0; //交易金额

                PbProject.Logic.Pay.Bill payBill = new PbProject.Logic.Pay.Bill();

                if (payType == "1")
                {
                    msgShow += "支付宝:";

                    #region 支付宝

                    PbProject.Logic.Pay.AliPay aliPay = new PbProject.Logic.Pay.AliPay();

                    strValue = aliPay.TradeQuery(oldOrderId);

                    if (strValue.Contains("<error>TRADE_NOT_EXIST</error>"))
                    {
                        msgShow += "查询失败";
                    }
                    else if (strValue.Contains("<trade_status>TRADE_SUCCESS</trade_status>"))
                    {
                        msgShow += "交易成功";

                        XElement root = XElement.Parse(strValue);

                        if (root.Element("response") != null)
                        {
                            XElement xe = root.Element("response");

                            if (xe.Element("trade") != null)
                            {
                                XElement xe1 = xe.Element("trade");

                                //交易号
                                if (xe1.Element("trade_no") != null)
                                {
                                    PayNo = xe1.Element("trade_no").Value;
                                    msgShow += " , 交易号：" + PayNo;
                                }

                                //交易金额
                                if (xe1.Element("total_fee") != null)
                                {
                                    PayMoney = decimal.Parse(xe1.Element("total_fee").Value);
                                    msgShow += " , 订单支付金额:" + PayMoney.ToString("f2");
                                }
                            }
                        }


                        if (IsOperOrder && msgShow.Contains("交易成功"))
                            payBill.CreateBillPayBill(oldOrderId, PayNo, 1, "", "在线支付", "支付宝支付",""); // 支付
                    }
                    else
                    {
                        msgShow += "支付宝 未付";
                    }



                    #endregion

                    strValue = "支付宝查询结果:" + strValue;
                }
                else if (payType == "2")
                {
                    msgShow += "快钱:";

                    #region 快钱

                    PbProject.Logic.Pay._99Bill bill = new PbProject.Logic.Pay._99Bill();
                    strValue = bill.GetPayReturn(oldOrderId);

                    if (strValue.Contains("<payResult>10</payResult>"))
                    {
                        msgShow += "交易成功";
                        XElement root = XElement.Parse(strValue);
                        //交易号
                        if (root.Element("dealId") != null)
                        {
                            PayNo = root.Element("dealId").Value;
                            msgShow += " , 交易号：" + PayNo;
                        }
                        //支付金额
                        if (root.Element("payAmount") != null)
                        {
                            PayMoney = decimal.Parse(root.Element("payAmount").Value);
                            PayMoney = PayMoney / 100;
                            msgShow += " , 订单支付金额:" + PayMoney.ToString("f2");
                        }
                        if (IsOperOrder && msgShow.Contains("交易成功"))
                            payBill.CreateBillPayBill(oldOrderId, PayNo, 2, "", "在线支付", "快钱支付",""); // 支付


                        if (strValue.Contains("<sharingStatus>10</sharingStatus>"))
                        {
                            msgShow += ",分账成功";
                        }
                        else
                        {
                            msgShow += ",分账失败";
                        }
                    }
                    else
                    {
                        msgShow += "快钱 未付";
                    }
                    #endregion

                    strValue = "快钱查询结果:" + strValue;
                }
                else if (payType == "3")
                {
                    msgShow += "汇付:";

                    #region 汇付

                    PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                    strValue = chinaPnr.QueryStatus(oldOrderId);
                    if (strValue.Contains("CmdId=QueryStatus") && strValue.Contains("RespCode=000000") && strValue.Contains("已支付"))
                    {
                        msgShow += "交易成功";
                        msgShow += " , 交易号：" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        msgShow += " , 订单支付金额:见订单";

                        if (IsOperOrder && msgShow.Contains("交易成功"))
                            payBill.CreateBillPayBill(oldOrderId, PayNo, 3, "", "在线支付", "汇付支付",""); // 支付
                    }
                    else 
                    {
                        msgShow += "汇付 未付";
                    }

                    #endregion

                    strValue = "汇付查询结果:" + strValue;
                }
                else if (payType == "4")
                {
                    msgShow += "财付通:";

                    #region 财付通
                    msgShow += "财付通暂时不支付查询";
                    #endregion

                    strValue = "汇付查询结果:" + msgShow;
                }
                else
                {
                    msgShow = "请选择支付方式";
                }
            }
            catch (Exception)
            {

            }
            return strValue;
        }

        /// <summary>
        /// 账号余额查询
        /// </summary>
        /// <param name="payType"></param>
        /// <param name="payAct"></param>
        /// <returns></returns>
        public string QueryBalance(string payType, string payAct, out string msgShow)
        {
            msgShow = "";
            string result = "";

            try
            {
                if (payType == "3")
                {
                    PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                    result = chinaPnr.QueryBalance(payAct);
                    string[] strValues = result.Split(new string[] { "<br/>", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string item in strValues)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (item.Contains("AcctBal"))
                            {
                                msgShow = "账户（ " + payAct + " ）余额 : " + item.Split('=')[1] + " 元 ";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    msgShow = "暂时只支持汇付！";
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// 账号签约查询
        /// </summary>
        /// <param name="payType"></param>
        /// <param name="payAct"></param>
        /// <returns></returns>
        public bool QuerySign(string payType, string payAct)
        {
            bool result = false;
            try
            {
                if (payType == "1")
                {
                    PbProject.Logic.Pay.AliPay aliPay = new PbProject.Logic.Pay.AliPay();
                    result = aliPay.IsUserSign(payAct);
                }
                else if (payType == "2")
                {
                    //快钱
                }
                else if (payType == "3")
                {
                    PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                    result = chinaPnr.QuerySign(payAct, payAct);
                }
                else if (payType == "4")
                {
                    //财付通
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// 账号签约查询
        /// </summary>
        /// <param name="payType"></param>
        /// <param name="payAct"></param>
        /// <returns></returns>
        public string GetSign(string payType, string payAct)
        {
            string result = "";
            try
            {
                if (payType == "1")
                {
                    PbProject.Logic.Pay.AliPay aliPay = new PbProject.Logic.Pay.AliPay();
                    result = aliPay.GetUserSign(payAct);
                }
                else if (payType == "2")
                {
                    //快钱
                }
                else if (payType == "3")
                {
                    PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                    result = chinaPnr.Sign(payAct, payAct);
                }
                else if (payType == "4")
                {
                    //财付通
                }
                else
                {
                    result = "";
                }
            }
            catch (Exception)
            {
            }
            return result;
        }


        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="mOrder"></param>
        /// <returns></returns>
        public string RefundSel(Tb_Ticket_Order mOrder, out string msgShow)
        {
            string returnValue = "";
            msgShow = "";

            try
            {
                string payType = mOrder.PayWay.ToString();
                string oldOrderId = string.IsNullOrEmpty(mOrder.OldOrderId) ? mOrder.OrderId : mOrder.OldOrderId;
                string newOrderId = mOrder.OrderId;

                string onlineNo = mOrder.PayNo;

                returnValue = RefundSel(payType, oldOrderId, newOrderId, onlineNo, out msgShow);
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="payType">支付方式</param>
        /// <param name="oldOrderId">原支付订单号</param>
        /// <param name="newOrderId">退款订单号</param>
        /// <param name="onlineNo">在线交易号 、退款流水号</param>
        /// <returns></returns>
        public string RefundSel(string payType, string oldOrderId, string newOrderId, string onlineNo, out string msgShow)
        {
            string returnValue = "" ;
            msgShow = "";
            try
            {
                if (payType == "1" || payType == "5")
                {
                    #region 支付宝

                    PbProject.Logic.Pay.AliPay alipay = new PbProject.Logic.Pay.AliPay();
                    msgShow = (string.IsNullOrEmpty(onlineNo) || onlineNo.Length != 16) ? "请正确输入交易号！" : msgShow;

                    if (msgShow == "")
                    {
                        string str = alipay.QueryRefundResultStr(onlineNo);
                        string[] strs = str.Split('#');
                        for (int i = 0; i < strs.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(strs[i]))
                            {
                                string[] strss = strs[i].Split('$');

                                if (!string.IsNullOrEmpty(strss[0]) && strss[0].Contains("SUCCESS"))
                                {
                                    //退款成功
                                    msgShow = "退款成功！银行卡 2 - 7个工作日到账,信用卡 2 - 14 个工作日到账!";
                                    break;
                                }
                            }
                        }
                        returnValue = str + "<br/>" + msgShow + "<br/>";
                    }

                    #endregion
                }
                else if (payType == "2" || payType == "6")
                {
                    #region 快钱

                    msgShow = (string.IsNullOrEmpty(newOrderId)) ? "请输入订单号！" : msgShow;
                    msgShow = (string.IsNullOrEmpty(oldOrderId)) ? "请输入原订单号！" : msgShow;
                    msgShow = (string.IsNullOrEmpty(onlineNo)) ? "请输入退款流水号！" : msgShow;

                    if (msgShow == "")
                    {
                        PbProject.Logic.Pay._99Bill bill = new PbProject.Logic.Pay._99Bill();

                        string[] strOrderNoS = onlineNo.Split('|');


                        for (int i = 0; i < strOrderNoS.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(strOrderNoS[i]))
                            {
                                // strOrderNoS[i]; //快钱退款流水号

                                string str = bill.GetRefund(oldOrderId, strOrderNoS[i]);

                                if (str != null && str.Contains("<result>10</result>"))
                                {
                                    msgShow = "退款成功！银行卡 2 - 7个工作日到账,信用卡 2 - 14 个工作日到账！";
                                    returnValue += str + "<br/>" + msgShow + "<br/>";
                                    break;
                                }
                                else if (str != null && (str.Contains("<result>00</result>") && str.Contains("<errCode></errCode>")))
                                {
                                    msgShow = "退款进行中!需等待快钱审核！银行卡 2 - 7个工作日到账,信用卡 2 - 14 个工作日到账！";
                                    returnValue += str + "<br/>" + msgShow + "<br/>";
                                    break;
                                }
                                else
                                {
                                    msgShow = "退款失败！";
                                    returnValue += str + "<br/>" + msgShow + "<br/>";
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (payType == "3" || payType == "7")
                {
                    #region 汇付

                    PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();

                    msgShow = (string.IsNullOrEmpty(onlineNo)) ? "请输入退款流水号！" : msgShow;

                    if (msgShow == "")
                    {
                        string[] strOrderNoS = onlineNo.Split('|');

                        for (int i = 0; i < strOrderNoS.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(strOrderNoS[i]))
                            {
                                // strOrderNoS[i]; //汇付 退款订单号
                                //string str = chinaPnr.QueryRefundStatus(strOrderNoS[i]).Replace("#", "<br/>\n");
                                string str = chinaPnr.QueryRefundStatus(strOrderNoS[i]);

                                if (str != null && str.Contains("RespCode=000000") && str.Contains("ErrMsg=退款交易已成功"))
                                {
                                    msgShow = " 退款成功！银行卡 2 - 7个工作日到账,信用卡 2 - 14 个工作日到账！";
                                    returnValue += str + "<br/>" + msgShow + "<br/>";
                                    break;
                                }
                                else if (str != null && str.Contains("RespCode=000000") && str.Contains("ErrMsg=退款交易，已扣款，系统处理中"))
                                {
                                    msgShow = " 退款成功，退款中！银行卡 2 - 7个工作日到账,信用卡 2 - 14 个工作日到账！";
                                    returnValue += str + "<br/>" + msgShow + "<br/>";
                                    break;
                                }
                                else
                                {
                                    msgShow = " 退款失败!";
                                    returnValue += str + "<br/>" + msgShow + "<br/>";
                                }
                            }
                        }

                    }
                    #endregion
                }
                else if (payType == "4" || payType == "8" || payType=="40")
                {
                    #region 财付通
                    TenPayParam tenPayParam = new TenPayParam();
                    tenPayParam.Orderid = newOrderId;
                    tenPayParam.OldOrderid = oldOrderId;
                    tenPayParam.PayNo = onlineNo;
                    tenPayParam.Date = DateTime.Now.ToString("yyyyMMdd");
                    returnValue=new TenPay().ClientSplitInquire(tenPayParam);
                    #endregion
                }
                else
                {
                    msgShow = "请选择支付方式";
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }
    }
}
