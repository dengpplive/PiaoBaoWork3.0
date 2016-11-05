using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using PbProject.Model;
using PbProject.Logic.Pay.batch_trans;

/// <summary>
/// 退款处理：技术使用
/// </summary>
public partial class Pay_OrderTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 支付查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPaySel_Click(object sender, EventArgs e)
    {
        string msg = "";

        //支付状态查询
        string returnValue = "";


        for (int i = 1; i <= 3; i++)
        {
            returnValue += new PbProject.Logic.Pay.OperOnline().PaySel(i.ToString(), txtOldOrder.Text.Trim(), true, out msg);
            returnValue += "\r\r\n";

            //result = PaySel(i.ToString(), txtOldOrder.Text.Trim(), true, out msg);
            if (msg.Contains("交易成功"))
            {
                break;
            }
        }

        txtReturnValue.Text = returnValue;

        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('" + msg + "');", true);
    }

    /// <summary>
    /// 退款查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnRefundSel_Click(object sender, EventArgs e)
    {
        string msg = "";
        string returnValue = new PbProject.Logic.Pay.OperOnline().RefundSel(rblPayType.SelectedValue,
            txtOldOrder.Text.Trim(), txtNewOrder.Text.Trim(), txtOnlineNo.Text.Trim(), out msg);


        //returnValue += "\r\n" + msg;
        returnValue = returnValue.Replace("#", "\r\r\n");
        returnValue = returnValue.Replace("<br/>", "\r\r\n");

        txtReturnValue.Text = returnValue;
        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('" + msg + "');", true);
    }

    /// <summary>
    /// 退款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnRefund_Click(object sender, EventArgs e)
    {
        string msg = Refund(rblPayType.SelectedValue, txtOldOrder.Text.Trim(), txtNewOrder.Text.Trim(),
             txtOnlineNo.Text.Trim(), txtPrice.Text.Trim(), txtRate.Text.Trim(), txtAct.Text.Trim(), txtDetail.Text.Trim());
        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('" + msg + "');", true);
    }

    /// <summary>
    /// 退款处理
    /// </summary>
    /// <param name="payType">支付方式</param>
    /// <param name="oldOrderId">原支付订单号</param>
    /// <param name="newOrderId">退款订单号</param>
    /// <param name="onlineNo">在线交易号、退款流水号</param>
    /// <param name="price">订单金额</param>
    /// <param name="rate">手续费率</param>
    /// <param name="act">收款账号</param>
    /// <param name="detail">退款明细</param>
    /// <returns></returns>
    public string Refund(string payType, string oldOrderId, string newOrderId, string onlineNo, string price, string rate, string act, string detail)
    {
        string msg = "";
        try
        {

            string strOrderID = newOrderId;//订单编号(退、废)
            string strOrderOldID = oldOrderId;//原支付订单号
            string strOrderOnlineNo = onlineNo;//交易号
            string strPrice = price;// 退款金额
            string strAct = act; //收款账号
            string details = detail; //退款明细


            decimal strRate = (!string.IsNullOrEmpty(rate)) ? decimal.Parse(rate) : 0.001M; //费率
            bool result = false;
            msg = (string.IsNullOrEmpty(strPrice)) ? "请输入退款金额！" : msg;
            msg = (string.IsNullOrEmpty(strAct)) ? "请输入收款账号！" : msg;
            decimal total = (!string.IsNullOrEmpty(strPrice)) ? decimal.Parse(strPrice) : 0; //订单退款金额
            total = total != 0 ? FourToFiveNum(total, 2) : total;

            if (payType == "1")
            {
                #region 支付宝
                msg = (string.IsNullOrEmpty(strOrderOnlineNo)) ? "请输入交易号！" : msg;
                msg = (strOrderOnlineNo.Length != 16) ? "交易号格式错误！" : msg;

                if (msg == "")
                {
                    PbProject.Logic.Pay.AliPay alipay = new PbProject.Logic.Pay.AliPay(false);

                    #region 处理
                    decimal priceRate = total * strRate; //费率
                    priceRate = FourToFiveNum(priceRate, 2);
                    decimal priceGy = total - priceRate; //供应退款
                    priceGy = FourToFiveNum(priceGy, 2);

                    if (string.IsNullOrEmpty(details))
                        details = strOrderOnlineNo + "^" + total.ToString("f2") + "^退款|" + act + "^^" + alipay._serveremail + "^^" + priceGy.ToString("f2") + "^退款";

                    if (strOrderOnlineNo != null && strOrderOnlineNo != "")
                    {
                        string bno = DateTime.Now.ToString("yyyyMMddHHmmsss");

                        string[] strValueS = new string[3];
                        strValueS[0] = bno;
                        strValueS[1] = "1";
                        strValueS[2] = details;

                        //退款 
                        result = alipay.IsRefund(strValueS);

                        if (result == true)
                        {
                            msg = "数据提交成功,稍后请查询核对数据。。。";

                            #region 支付宝 查询退款
                            /*
                            if (msg == "")
                            {
                                string str = alipay.QueryRefundResultStr(strOrderOnlineNo);
                                string[] strs = str.Split('#');
                                for (int i = 0; i < strs.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(strs[i]))
                                    {
                                        string[] strss = strs[i].Split('$');

                                        if (!string.IsNullOrEmpty(strss[0]) && strss[0].Contains("SUCCESS"))
                                        {
                                            //退款成功
                                            msg += "退款成功！银行卡 2 - 7个工作日到账,信用卡 2 - 14 个工作日到账!";
                                            break;
                                        }
                                    }
                                }
                            }
                             * */
                            #endregion
                        }
                        else
                        {
                            msg = "数据提交失败,请检测数据格式是否有误！！！";
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else if (payType == "2")
            {
                #region 快钱
                msg = (string.IsNullOrEmpty(strOrderID)) ? "请输入订单号！" : msg;//订单编号(退、废)
                msg = (string.IsNullOrEmpty(strOrderOldID)) ? "请输入原订单号！" : msg;//原支付订单号

                if (msg == "")
                {
                    #region 处理

                    PbProject.Logic.Pay._99Bill bill = new PbProject.Logic.Pay._99Bill(false);

                    string dataTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    decimal priceRate = total * strRate; //费率
                    priceRate = FourToFiveNum(priceRate, 2);
                    decimal priceGy = total - priceRate; //供应退款
                    priceGy = FourToFiveNum(priceGy, 2);

                    priceGy = priceGy * 100; //供应退款
                    priceRate = priceRate * 100;//费率
                    total = total * 100; //订单退款

                    int p1 = int.Parse(priceGy.ToString().Split('.')[0]);//供应退款
                    int p2 = int.Parse(priceRate.ToString().Split('.')[0]); //费率
                    int p3 = int.Parse(total.ToString().Split('.')[0]); //订单退款  

                    //"1^kqcom06@sina.com^5000^爱的|1^kqcom02@sina.com^7000^分账1|1^kqcomsh@sina.com^8000^分账2";
                    if (string.IsNullOrEmpty(details))
                        details = "1^" + bill.LinkEmail + "^" + p2.ToString() + "^Refund|1^" + strAct + "^" + p1.ToString() + "^Refund";

                    string[] Details = new string[6];
                    Details[0] = strOrderOldID; // 订单编号(原订单号)
                    Details[1] = p3.ToString(); // 订单金额
                    Details[2] = "系统退款"; //备注
                    Details[3] = details;
                    Details[4] = strOrderID; // 订单编号(退废订单编号)
                    Details[5] = dataTime;  //退款流水号


                    string str = bill.Refund(Details);

                    result = bill.IsRefund(str);
                    //result = true;

                    if (result == true)
                    {
                        msg = "数据提交成功,稍后请查询核对数据。。。";
                    }
                    else
                    {
                        msg = "数据提交失败,请检测数据格式是否有误！！！";
                    }

                    #endregion
                }
                #endregion
            }
            else if (payType == "3")
            {
                #region 汇付

                msg = (string.IsNullOrEmpty(strOrderID)) ? "请输入订单号！" : "";//订单编号(退、废)
                msg = (string.IsNullOrEmpty(strOrderOldID)) ? "请输入原订单号！" : "";//原支付订单号

                if (msg == "")
                {
                    #region 处理

                    PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr(false);

                    string orderno = DateTime.Now.ToString("yyyyMMddHHmm");
                    string orderid = (strOrderID == strOrderOldID) ? orderno + "_" + strOrderOldID : strOrderID; //订单编号

                    decimal priceRate = total * strRate; //费率
                    priceRate = FourToFiveNum(priceRate, 2);
                    decimal priceGy = total - priceRate; //供应退款
                    priceGy = FourToFiveNum(priceGy, 2);

                    if (string.IsNullOrEmpty(details))
                        details = "Agent:" + act + ":" + priceGy.ToString("f2");

                    string[] Details = new string[4];
                    Details[0] = orderid;
                    Details[1] = strOrderOldID;
                    Details[2] = strPrice;
                    Details[3] = details;

                    string value = chinaPnr.Refund(Details[0], Details[1], Details[2], Details[3]);

                    if (value.Contains("RespCode=000000") && value.Contains("ErrMsg=成功"))
                    {
                        msg = "数据提交成功,稍后请查询核对数据。。。";
                    }
                    else
                    {
                        msg = "数据提交失败,请检测数据格式是否有误！！！";
                    }
                    #endregion
                }
                #endregion
            }
            else if (payType == "4")
            {
                #region 财付通


                #endregion
            }
            else
            {
                msg = "请选择支付方式";
            }

        }
        catch (Exception)
        {

        }

        return msg;
    }

    /// <summary>  
    /// 实现数据的四舍五入法, 保留小数
    /// </summary>  
    /// <param name="v">要进行处理的数据</param>  
    /// <param name="x">保留的小数位数</param>   
    /// <returns>四舍五入后的结果</returns>   
    public decimal FourToFiveNum(decimal v, int x)
    {
        decimal _del = Math.Round(v + 0.0000001M, x);// //四舍五入
        return _del;
    }

    /// <summary>
    /// 清空
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtReturnValue.Text = "";
    }

    /// <summary>
    /// 账号余额查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQueryBalance_Click(object sender, EventArgs e)
    {
        try
        {
            string msg = "";
            string strValue = new PbProject.Logic.Pay.OperOnline().QueryBalance(rblPayType.SelectedValue, txtAct.Text.Trim(), out msg);
            txtReturnValue.Text = strValue;
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('" + msg + "');", true);
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 账号签约查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuerySign_Click(object sender, EventArgs e)
    {
        try
        {
            string msg = "";
            string strAct = txtAct.Text.Trim();
            bool strValue = new PbProject.Logic.Pay.OperOnline().QuerySign(rblPayType.SelectedValue, strAct);

            if (strValue)
                msg = "签约成功！";
            else
                msg = "签约失败！";

            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('" + msg + "');", true);
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSign_Click(object sender, EventArgs e)
    {
        
        try
        {
            string msg = "";
            string strAct = txtAct.Text.Trim();
            string strValue = new PbProject.Logic.Pay.OperOnline().GetSign(rblPayType.SelectedValue, strAct);



            //Response.Write(strValue);
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "window.open ('"+strValue+"','newwindow','top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no')", true);
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 显示账单明细
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPayDetail_Click(object sender, EventArgs e)
    {
        try
        {
            string sqlWhere = " OrderId='" + txtOldOrder.Text.Trim() + "' ";
            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;


            List<string> strList = new PbProject.Logic.Pay.Bill().CreateOrderAndTicketPayDetailNew(mOrderList[0], PassengerList); //计算订单金额生成订单

            if (strList != null && strList.Count > 0)
            {
                foreach (string item in strList)
                {
                    txtReturnValue.Text += item + "\r\r\n";
                }

                txtReturnValue.Text += "PayMoney = " + mOrderList[0].PayMoney + "\r\r\n";
                txtReturnValue.Text += "OrderMoney = " + mOrderList[0].OrderMoney + "\r\r\n";
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('生成数据失败');", true);
            }
        }
        catch (Exception ex)
        {

        }
    }
    /// <summary>
    /// 计算订单金额
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPayMoney_Click(object sender, EventArgs e)
    {
        try
        {
            string sqlWhere = " OrderId='" + txtOldOrder.Text.Trim() + "' ";
            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;


            // PbProject.Logic.Pay.Data d = new PbProject.Logic.Pay.Data();
            //string PayMoney = d.CreateOrderPayMoney(mOrderList[0], PassengerList).ToString();
            //string OrderMoney = d.CreateOrderOrderMoney(mOrderList[0], PassengerList).ToString();

            PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
            bill.CreateOrderAndTicketPayDetailNew(mOrderList[0], PassengerList);
            decimal PayMoney = mOrderList[0].PayMoney;
            decimal OrderMoney = mOrderList[0].OrderMoney;


            txtReturnValue.Text += "PayMoney=" + PayMoney + ",OrderMoney=" + OrderMoney;
        }
        catch (Exception ex)
        {

        }
    }


    /// <summary>
    /// 调试申请退款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnShenQ_Click(object sender, EventArgs e)
    {
        try
        {
            // 请勿随便调试，请先注释  OperOrderTFG 方法里面的执行 事物语句
            if (true)
            {
                //string sqlWhere = " OrderId='" + txtOldOrder.Text.Trim() + "' ";
                //PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                //List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
                //List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;


                //List<User_Employees> UEmployees = baseDataManage.CallMethod("User_Employees", "GetList", null, new Object[] { "LoginName='pb_jianghui'" }) as List<User_Employees>;
                //List<User_Company> UCompany = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='100001'" }) as List<User_Company>;

                //PbProject.Logic.Order.Tb_Ticket_OrderBLL TTOrderBLL = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 调试处理退款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnTuik_Click(object sender, EventArgs e)
    {
        try
        {
            // 请勿随便调试，请先注释  OperOrderTFG 方法里面的执行 事物语句
            if (true)
            {
                //string sqlWhere = " OrderId='" + txtOldOrder.Text.Trim() + "' ";
                //PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                //List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
                //List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;


                //List<User_Employees> UEmployees = baseDataManage.CallMethod("User_Employees", "GetList", null, new Object[] { "LoginName='pb_jianghui'" }) as List<User_Employees>;
                //List<User_Company> UCompany = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='100001'" }) as List<User_Company>;

                //PbProject.Logic.Order.Tb_Ticket_OrderBLL TTOrderBLL = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();

                //TTOrderBLL.OperOrderTFG(mOrderList[0], PassengerList, UEmployees[0], UCompany[0], "");
            }
        }
        catch (Exception ex)
        {

        }
    }

    #region 自动代扣、支付、通知

    /// <summary>
    /// CAE 自动代扣
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCAEPay_Click(object sender, EventArgs e)
    {
        PbProject.Logic.Pay.AliPay alipay = new PbProject.Logic.Pay.AliPay();

        ///// <param name="type_code">代理业务编号</param>
        ///// <param name="out_order_no">商户订单号</param>
        ///// <param name="subject">支付标题：代扣</param>
        ///// <param name="amount">金额</param>
        ///// <param name="trans_account_out">转出支付宝账号</param>



        //string out_order_no = DateTime.Now.Ticks.ToString();//代理业务编号
        //string subject = "代扣"; //说明
        //string amount = "4.0"; // 订单金额
        //string trans_account_out = "money@mypb.cn";  //代扣账号
        //string trans_account_in = "jianghui520you@126.com";// 收款账号
        //string royalty_parameters = "2646798837@qq.com^3.00^测试代扣|zyl_go@126.com^1.00^测试代扣"; //分账明细，可以为空

        string out_order_no = DateTime.Now.Ticks.ToString();//代理业务编号
        string subject = "代扣测试"; //说明
        string amount = "40.0"; // 订单金额
        string trans_account_out = "zhifubaopb@mypb.cn";// 代扣账号
        string trans_account_in = "jianghui520you@126.com";  //收款账号
        string royalty_parameters = ""; //分账明细，可以为空


        string strValues = alipay.GetPayCAE(out_order_no, subject, amount, trans_account_out, trans_account_in, royalty_parameters);

        if (strValues.Contains("USER_PAY_TYPE_MISMATCH"))
        {
            //代扣账号没钱
        }

        //strValues = strValues.Replace("method='post'", "method='post' target='_blank'");

        Response.Write(strValues);
    }


    /// <summary>
    /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
    /// </summary>
    /// <returns>request回来的信息组成的数组</returns>
    public void GetRequestPost()
    {
        int i = 0;
        SortedDictionary<string, string> sPara = new SortedDictionary<string, string>();
        System.Collections.Specialized.NameValueCollection coll;
        //Load Form variables into NameValueCollection variable.
        coll = Request.Form;

        // Get names of all forms into a string array.
        String[] requestItem = coll.AllKeys;

        for (i = 0; i < requestItem.Length; i++)
        {
            sPara.Add(requestItem[i], Request.Form[requestItem[i]]);
        }

        if (sPara.Count > 0)//判断是否有带返回参数
        {
            Notify aliNotify = new Notify();
            bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

            if (verifyResult)//验证成功
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //请在这里加上商户的业务逻辑程序代码
                //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表
                string out_trade_no = Request.Form["out_trade_no"];//商户网站唯一订单号
                string trade_status = Request.Form["trade_status"]; //交易状态
                //具体参考开发文档
                //判断是否在商户网站中已经做过了这次通知返回的处理
                //如果没有做过处理，那么执行商户的业务程序
                //如果有做过处理，那么不执行商户的业务程序

                string notify_type = Request.Form["air_cae_charge_agent"]; // 自动代扣说明

                Response.Write("success");  //请不要修改或删除
                //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////


            }
            else//验证失败
            {
                Response.Write("fail");
            }
        }
        else
        {
            Response.Write("无通知参数");
        }
    }

    #endregion
   
}