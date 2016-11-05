using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Logic.Order;
using PbProject.WebCommon.Utility;
using PbProject.Logic.ControlBase;
using PbProject.Logic.PID;
using System.Data;
using PnrAnalysis;
using PbProject.Logic;
using System.Xml;
using System.IO;
using System.Xml.Linq;
/// <summary>
/// 订单支付页面
/// </summary>
public partial class Buy_PayMent : BasePage
{

    #region 属性
    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            return BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
        }
    }
    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            return BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
        }
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                CFTBankBind();
                this.currentuserid.Value = this.mUser.id.ToString();
                lbtnPwd.PostBackUrl = string.Format("~/AccountPayPwd.aspx?currentuserid={0}", this.currentuserid.Value.ToString());
                if ((Request.QueryString["id"] != null))
                {
                    Hid_id.Value = Request.QueryString["id"].ToString(); //订单id
                    BindOrder(); //绑定订单信息
                    BindPayType(); //绑定支付方式

                    //隐藏政策
                    Hid_IsPolicy.Value = mUser.UserPower.Contains("|2|") ? "1" : "0";
                }
            }

            if (Request["Method"] != null && Request["Method"].ToString() == "AjacPay")
            {
                AjacPay();
            }
        }
        catch (Exception ex)
        {

        }
    }

    private void CFTBankBind()
    {
        string path = Server.MapPath("~/banklist.xml");
        XElement root = XElement.Load(path);
        var list = from x in root.Descendants("bank")
                   select new
                   {
                       ID = x.Attribute("num").Value,
                       Name = x.Attribute("name").Value,
                       Code = x.Attribute("code").Value
                   };
        r_bank_type40.DataSource = list;
        r_bank_type40.DataBind();

    }
    /// <summary>
    /// PNR
    /// </summary>
    public string PNR
    {
        get
        {
            return (string)ViewState["pnr"];
        }
    }

    /// <summary>
    /// 政策和佣金显示
    /// </summary>
    public string PolicyAndYongJing
    {
        get
        {
            return (string)ViewState["Policy_XF_YJ"];
        }
    }

    /// <summary>
    /// 页面绑定订单信息
    /// </summary>
    private void BindOrder()
    {
        try
        {
            string tempSqlWhere = "";
            tempSqlWhere = "id='" + Hid_id.Value + "'";
            List<Tb_Ticket_Order> ticketOrderList = new Tb_Ticket_OrderBLL().GetListBySqlWhere(tempSqlWhere);

            if (ticketOrderList != null && ticketOrderList.Count > 0)
            {
                Tb_Ticket_Order mOrder = ticketOrderList[0];

                ViewState["order"] = mOrder; // order 

                //显示编码
                ViewState["pnr"] = mOrder.PNR; //pnr
                //政策 现返 佣金显示
                //ViewState["Policy_XF_YJ"] = (mOrder.ReturnPoint) + "%" + (mOrder.ReturnMoney > 0 ? (mOrder.ReturnMoney == 0 ? "" : (mOrder.ReturnMoney > 0 ? ("+" + mOrder.ReturnMoney.ToString()) : mOrder.ReturnMoney.ToString())) : "") + "/" + mOrder.PolicyMoney;
                ViewState["Policy_XF_YJ"] = mOrder.ReturnPoint.ToString();
                LinkMan.Text = mOrder.LinkMan; //联系人
                LinkPhone.Text = mOrder.LinkManPhone;//联系电话
                if (mUser.UserPower.Contains("|2|"))
                    lblPay.Text = (mOrder.ABFee + mOrder.FuelFee + mOrder.PMFee).ToString("F2");
                else
                    lblPay.Text = mOrder.PayMoney.ToString("F2"); //订单支付金额
                lblOrderId.Text = mOrder.OrderId;

                #region 乘机人信息
                tempSqlWhere = " OrderId='" + mOrder.OrderId + "'";
                List<Tb_Ticket_Passenger> ticketPassengerList = new Tb_Ticket_PassengerBLL().GetPasListBySQLWhere(tempSqlWhere);
                if (ticketPassengerList != null && ticketPassengerList.Count > 0)
                {

                    string PassengerMessage = "";
                    string tempPassengerType = "";

                    foreach (Tb_Ticket_Passenger item in ticketPassengerList)
                    {
                        if (item.PassengerType == 1)
                        {
                            tempPassengerType = "成人";
                        }
                        else if (item.PassengerType == 2)
                        {
                            tempPassengerType = "儿童";
                        }
                        else if (item.PassengerType == 3)
                        {
                            tempPassengerType = "婴儿";
                        }

                        PassengerMessage += item.PassengerName + "&nbsp;&nbsp;&nbsp;(" + tempPassengerType + ")&nbsp;&nbsp;&nbsp;" + item.Cid;
                    }

                    lblPassenger.Text = PassengerMessage;
                    lblPassengerNum.Text = ticketPassengerList.Count.ToString();
                }
                #endregion

                #region 航段信息
                List<Tb_Ticket_SkyWay> ticketSkyWayList = new Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere(tempSqlWhere);
                if (ticketSkyWayList != null && ticketSkyWayList.Count > 0)
                {
                    repTicketSkyWay.DataSource = ticketSkyWayList;
                    repTicketSkyWay.DataBind();
                }
                #endregion
            }
            else
            {
                //没有订单信息不能支付
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// BindPayType
    /// </summary>
    private void BindPayType()
    {
        try
        {
            Tb_Ticket_Order mOrder = ViewState["order"] as Tb_Ticket_Order; // order 

            if (mOrder.PayMoney == 0)
            {
                // 没有支付方式
                spanBtnPay.Visible = false;
                spanNoPay.Visible = true;
                return;
            }

            //rblPayType.Items.Add(new ListItem("账户余额支付", "14"));
            //rblPayType.Items.Add(new ListItem("收银", "15"));

            string wangYinZhangHao = PbProject.Model.definitionParam.paramsName.wangYinZhangHao;
            string wangYinLeiXing = PbProject.Model.definitionParam.paramsName.wangYinLeiXing;

            if (mOrder != null)
            {
                string gYcpyNo = mOrder.CPCpyNo.Substring(0, 12);
                List<Bd_Base_Parameters> bParametersList = null;

                if (gYcpyNo == mUser.CpyNo.Substring(0, 12))
                {
                    bParametersList = supBaseParametersList;
                }
                else
                {
                    string sqlWhere = " CpyNo='" + gYcpyNo + "' and (SetName='" + wangYinZhangHao + "' or SetName='" + wangYinLeiXing + "')";
                    bParametersList = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL().GetParametersListByWhere(sqlWhere);
                }

                #region 测试数据

                //List<Bd_Base_Parameters> bParametersList = new List<Bd_Base_Parameters>();

                //Bd_Base_Parameters ts = new Bd_Base_Parameters();
                //ts.SetName = wangYinZhangHao;
                //ts.SetValue = "jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|";
                //bParametersList.Add(ts);

                //Bd_Base_Parameters ts1 = new Bd_Base_Parameters();
                //ts1.SetName = wangYinLeiXing;
                //ts1.SetValue = "6";
                //bParametersList.Add(ts1);

                //

                #endregion

                Bd_Base_Parameters zhangHao = null;
                Bd_Base_Parameters leiXing = null;

                if (bParametersList != null && bParametersList.Count > 1)
                {
                    foreach (var item in bParametersList)
                    {
                        if (item.SetName == wangYinZhangHao)
                        {
                            zhangHao = item;
                        }
                        else if (item.SetName == wangYinLeiXing)
                        {
                            leiXing = item;
                        }
                    }
                    string temp = "";

                    #region 在线支付功能

                    if (zhangHao != null && !string.IsNullOrEmpty(zhangHao.SetValue) && zhangHao.SetValue.Contains("|"))
                    {
                        string[] setValues = zhangHao.SetValue.Split('|');
                        string[] setValue0 = setValues[0].Split('^');
                        if (!string.IsNullOrEmpty(setValue0[0]))
                        {
                            //支付宝
                            rblPayType.Items.Add(new ListItem("支付宝", "1"));
                            temp += "5,";
                        }
                        string[] setValue1 = setValues[1].Split('^');
                        if (!string.IsNullOrEmpty(setValue1[0]))
                        {
                            //快钱
                            rblPayType.Items.Add(new ListItem("快钱", "2"));
                            temp += "6,";
                        }
                        string[] setValue2 = setValues[2].Split('^');
                        if (!string.IsNullOrEmpty(setValue2[0]))
                        {
                            //汇付
                            rblPayType.Items.Add(new ListItem("汇付", "3"));
                            temp += "7,";
                        }
                        string[] setValue3 = setValues[3].Split('^');
                        if (!string.IsNullOrEmpty(setValue3[0]))
                        {
                            //财付通
                            rblPayType.Items.Add(new ListItem("财付通", "4"));
                            temp += "8,";

                            rblPayType.Items.Add(new ListItem("信用卡大额支付", "40"));
                        }

                    }

                    //判断网银
                    if (leiXing != null && !string.IsNullOrEmpty(leiXing.SetValue) && leiXing.SetValue != "0")
                    {
                        if (temp.Contains(leiXing.SetValue))
                        {
                            rblPayType.Items.Insert(0, new ListItem("网银", leiXing.SetValue));
                            Hid_payType.Value = leiXing.SetValue;
                        }
                    }

                    #endregion


                    if (gYcpyNo == mUser.CpyNo.Substring(0, 12))
                    {
                        #region 线下支付功能

                        if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                        {
                            string strGongYingKongZhiFenXiao = PbProject.WebCommon.Utility.BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;

                            //账户余额支付
                            if (strGongYingKongZhiFenXiao != null && strGongYingKongZhiFenXiao.Contains("|76|"))
                            {
                                rblPayType.Items.Add(new ListItem("账户余额支付", "14"));
                                //账户余额支付
                            }

                            if (mOrder.PolicySource == 1 || mOrder.PolicySource == 2)
                            {
                                // 不能收银
                                //收银
                                if (strGongYingKongZhiFenXiao != null && strGongYingKongZhiFenXiao.Contains("|79|"))
                                {
                                    rblPayType.Items.Add(new ListItem("收银", "15"));
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        //关闭线下支付
                    }

                    if (string.IsNullOrEmpty(mCompany.AccountPwd))
                    {
                        //未设置密码，显示设置密码
                        trbtnPwdUrl.Visible = true;
                        trbtnPwd.Visible = false;
                    }
                    else
                    {
                        //有设置
                        trbtnPwdUrl.Visible = false;
                        trbtnPwd.Visible = true;
                    }

                    if (rblPayType.Items.Count > 0)
                    {
                        rblPayType.Items[0].Selected = true;
                        Hid_payWay.Value = rblPayType.Items[0].Value; //支付方式

                        spanBtnPay.Visible = true;
                        spanNoPay.Visible = false;
                    }
                    else
                    {
                        // 没有支付方式
                        spanBtnPay.Visible = false;
                        spanNoPay.Visible = true;
                    }
                }
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    ///  本地支付
    /// </summary>
    protected void AjacPay()
    {
        string msgShow = "";
        bool result = false;

        try
        {
            // 判断密码是否正确
            string pwd = Request["AccountPayPwd"].ToString();
            string id = Request["Hidid"].ToString(); //Hid_id.Value
            string payWay = Request["HidpayWay"].ToString(); //  Hid_payWay.Value;

            pwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(pwd);//原支付密码

            //有收银的情况
            if (payWay == "14" && mCompany.AccountPwd != pwd)
                msgShow = "支付失败!支付密码错误！";

            if (msgShow == "")
            {
                string tempSqlWhere = "";
                tempSqlWhere = "id='" + id + "'";

                List<Tb_Ticket_Order> reList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere(tempSqlWhere);
                Tb_Ticket_Order mOrder = (reList != null && reList.Count > 0) ? reList[0] : null;

                if (mOrder != null)
                {
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
                        if (PnrModel != null && ErrMsg == "")
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

                    if (mOrder.PayMoney < mOrder.OrderMoney)
                    {
                        // 订单金额有误
                        msgShow = "生成订单数据有误,请重新导入订单进行支付！";
                    }

                    if (msgShow == "")
                    {
                        #region

                        PbProject.Logic.Pay.VirtualPay virtualPay = new PbProject.Logic.Pay.VirtualPay();

                        if (payWay == "14")
                        {
                            decimal payDebtsMoney = 0;
                            result = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().AutomaticRemind(mCompany, out payDebtsMoney);

                            if (result)
                            {
                                result = false;

                                // 有未还清的账单，请
                                msgShow = "对不起不能支付您账单周期有未还清的账单,请还清账单周期内的欠款";
                            }
                            else
                            {
                                result = false;

                                mOrder.PayWay = 14;
                                result = new PbProject.Logic.Pay.Bill().UpdateOrderAndTicketPayDetail(mOrder);
                                if (result)
                                    result = virtualPay.DepositPay(mOrder, mUser, out msgShow);//账号余额支付
                            }
                        }
                        else if (payWay == "15")
                        {
                            //收银
                            mOrder.PayWay = 15;
                            result = new PbProject.Logic.Pay.Bill().UpdateOrderAndTicketPayDetail(mOrder);

                            if (result)
                                result = virtualPay.CashRegisterPay(mOrder, mUser, mCompany, out msgShow);
                        }
                        OnErrorNew("开始接口：" + result + "&&&" + mOrder.OrderStatusCode + "&&" + mOrder.PolicySource, false);


                        if (result)
                        {
                            msgShow = "支付成功!";
                            PbProject.Model.Tb_Ticket_Order NewOrder = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(mOrder.OrderId);

                            if (NewOrder.OrderStatusCode == 3)
                            {
                                #region 接口订单处理

                                //Login(mOrder);

                                if (NewOrder.PolicySource == 3)
                                {
                                    PayBy517(NewOrder);//517
                                }
                                else if (NewOrder.PolicySource == 4)
                                {
                                    BaiTuoPay(NewOrder);//百拓
                                }
                                else if (NewOrder.PolicySource == 5)
                                {
                                    //  8000Y
                                    PayFor8000Y(NewOrder);
                                }
                                else if (NewOrder.PolicySource == 6)
                                {
                                    //  今日
                                    PayForToday(NewOrder);
                                }
                                else if (NewOrder.PolicySource == 7)
                                {

                                    PMPay(NewOrder);//票盟
                                }
                                else if (NewOrder.PolicySource == 8)
                                {
                                    bookPay(NewOrder);//51book
                                }
                                else if (NewOrder.PolicySource == 10)
                                {
                                    PayByYeeXing(NewOrder);//易行
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            msgShow = "支付失败!" + msgShow;
                        }


                        #endregion
                    }
                }
                else
                {
                    msgShow = "支付失败，获取订单信息失败!";
                }
            }
        }
        catch (Exception ex)
        {
            msgShow = "支付错误!";
        }

        Response.Clear();
        Response.Write(msgShow);
        Response.End();
    }

    /// <summary>
    /// 数据处理
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="value">数据</param>
    /// <returns></returns>
    public string SkyWayMsg(int type, string value)
    {
        string result = "";
        try
        {
            result = value;

            switch (type)
            {
                case 1:

                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    result = (ViewState["pnr"] != null) ? ViewState["pnr"].ToString() : "";
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                default:
                    break;
            }
        }
        catch (Exception)
        {

            result = value;
        }
        return result;
    }

    #region 接口方法

    #region 517
    /// <summary>
    /// 517订单并支付
    /// </summary>
    private bool PayBy517(PbProject.Model.Tb_Ticket_Order Order)
    {
        bool Is517na = true;
        DataSet dsReson = new DataSet();
        DataSet dsResonPay = new DataSet();
        string sql = " update Tb_Ticket_Order set ";
        try
        {

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
            string Accout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[0];
            string Password517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[1];
            string Ag517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[2];
            string PayAccout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[3];
            string PayPassword517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[4];


            w_517WebService._517WebService ServiceBy517 = new w_517WebService._517WebService();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
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
                OnErrorNew("开始生成517订单，本地订单号：" + Order.OrderId, false);

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

                            //AddOrderLog(Order.OrderId, "系统", "系统", mCompany.UninCode, mCompany.RoleType, mCompany.UninAllName, "修改", "于 " + DateTime.Now + " 517生成失败：" + dsReson.Tables[0].Rows[0][0].ToString() + "," + dsReson.Tables[0].Rows[0][1].ToString());

                            return false;
                        }
                    }
                    if (dsReson.Tables[0].Rows[0]["OrderId"].ToString() != "")
                    {
                        OnErrorNew("517生成订单成功，本地订单号：" + Order.OrderId, false);
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
                            OnErrorNew("517开始自动支付，本地订单号：" + Order.OrderId, false);

                            //如果517价格比本地高，则不支付
                            if ((Convert.ToDecimal(dsReson.Tables[0].Rows[0]["TotlePirce"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                                || (Convert.ToDecimal(dsReson.Tables["FlightInfo"].Rows[0]["FaceValue"].ToString()) * Order.PassengerNumber != Order.PMFee)
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
                                    OnErrorNew("PaySuccess:" + dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString() + "，本地订单号：" + Order.OrderId, false);

                                    OnErrorNew("517自动支付成功，本地订单号：" + Order.OrderId, false);
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
                                    OrderLog.OperContent = "517代付成功!";
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
                    OnErrorNew("517开始自动支付，本地订单号：" + Order.OrderId, false);

                    //如果517价格比本地高，则不支付
                    if ((Convert.ToDecimal(dsReson.Tables[0].Rows[0]["TotlePirce"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee))
                        || (Convert.ToDecimal(dsReson.Tables["FlightInfo"].Rows[0]["FaceValue"].ToString()) * Order.PassengerNumber != Order.PMFee)
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
                            OnErrorNew("PaySuccess:" + dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString() + "，本地订单号：" + Order.OrderId, false);

                            OnErrorNew("517自动支付成功，本地订单号：" + Order.OrderId, false);
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
                            OrderLog.OperContent = "517代付成功!";
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
            OnErrorNew("517线下异常:" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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

    #region 易行
    /// <summary>
    /// 易行订单并支付
    /// </summary>
    private bool PayByYeeXing(PbProject.Model.Tb_Ticket_Order Order)
    {
        bool IsYeeXingna = true;
        DataSet dsReson = new DataSet();
        DataSet dsResonPay = new DataSet();
        string sql = " update Tb_Ticket_Order set ";
        try
        {

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
            string yeeXingAccout = BS.JieKouZhangHao.Split('|')[6].Split('^')[0];
            string yeeXingAccout2 = BS.JieKouZhangHao.Split('|')[6].Split('^')[1];

            w_YeeXingService.YeeXingSerivce ServiceByYeeXing = new w_YeeXingService.YeeXingSerivce();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
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
                OnErrorNew("开始生成易行订单，本地订单号：" + Order.OrderId, false);
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
                        OnErrorNew("易行生成订单成功，本地订单号：" + Order.OrderId, false);
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
                            OnErrorNew("易行开始自动支付，本地订单号：" + Order.OrderId, false);

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
                                OnErrorNew("易行平台订单价格和本地价格不符，不进行代付，本地订单号：" + Order.OrderId, false);
                                return false;
                            }

                            string ReturnURL = "http://210.14.138.26:91/Pay/PTReturnPage/YeeXingNotifyUrl.aspx";
                            dsResonPay = ServiceByYeeXing.PayOutContract(yeeXingAccout, yeeXingAccout2, Order.OutOrderId, Order.OutOrderPayMoney.ToString(), "1", ReturnURL, ReturnURL);
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

                                    OnErrorNew("易行自动支付成功，本地订单号：" + Order.OrderId, false);
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
                                    OrderLog.OperContent = "易行代付成功!";
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
                    OnErrorNew("易行开始自动支付，本地订单号：" + Order.OrderId, false);
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
                        OrderLog.OperContent = "于 " + DateTime.Now + " 易行代付失败：平台订单价格和本地价格不符，不进行代付！易行平台价格信息："+
				dsReson.Tables["price"].Rows[0]["ibeprice"].ToString()+"|"+dsReson.Tables["price"].Rows[0]["buildFee"].ToString()+
				"|"+dsReson.Tables["price"].Rows[0]["oilFee"].ToString();
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                        OnErrorNew("易行平台订单价格和本地价格不符，不进行代付，本地订单号：" + Order.OrderId, false);
                        return false;
                    }

                    string ReturnURL = "http://210.14.138.26:91/Pay/PTReturnPage/YeeXingNotifyUrl.aspx";
                    dsResonPay = ServiceByYeeXing.PayOutContract(yeeXingAccout, yeeXingAccout2, Order.OutOrderId, Order.OutOrderPayMoney.ToString(), "1", ReturnURL, ReturnURL);
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

                            OnErrorNew("易行自动支付成功，本地订单号：" + Order.OrderId, false);
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
                            OrderLog.OperContent = "易行代付成功!";
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
            OnErrorNew("易行线下异常:" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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

        XmlElement xmlElementCreateOrder = OrderbaiTuoManager.BaiTuoCreateOrderSend(OrderArray, dtPassenger);
        OnErrorNew(xmlElementCreateOrder.InnerXml, false);
        xmlNode = BaiTuoService.pnrCreateOrderEx(xmlElementCreateOrder);
        OnErrorNew("订单生成完成，本地订单号：" + Order.OrderId, false);
        return xmlNode;
    }
    /// <summary>
    /// 调用百拓支付接口
    /// </summary>
    /// <param name="OrderId">订单编号</param>
    private bool BaiTuoPay(PbProject.Model.Tb_Ticket_Order Order)
    {
        bool IsOk = true;
        try
        {
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

            PbProject.Logic.PTInterface.PTBybaituo OrderbaiTuoManager = new PbProject.Logic.PTInterface.PTBybaituo(Order, BS);
            string sql = " update Tb_Ticket_Order set ";
            //if (ComModel.A66 == "1")
            //{
            OnErrorNew("开始生成百拓订单，本地订单号：" + Order.OrderId, false);
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
                    OnErrorNew("百拓生成订单成功，本地订单号：" + Order.OrderId, false);
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
                        OnErrorNew("百拓开始自动支付，本地订单号：" + Order.OrderId, false);
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
                            OnErrorNew("百拓平台订单价格和本地价格不符，不进行代付，本地订单号：" + Order.OrderId, false);
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
                                OnErrorNew("收到订单号："+Order.OrderId+"，百拓平台自动代付返回结果："+Message, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            OnErrorNew("百拓自动支付失败:" + ex.ToString() + "，本地订单号：" + Order.OrderId, false);
                            Message = "";
                            #region 记录操作日志
                            //添加操作订单的内容
                            PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = Order.OrderId;
                            OrderLog.OperType = "修改";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperContent = "百拓自动代付失败:" + ex.Message;
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
                                OnErrorNew("百拓后台代付失败:" + Message + "，本地订单号：" + Order.OrderId, false);

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
                            OnErrorNew("百拓自动支付失败:Message为空，本地订单号：" + Order.OrderId, false);
                            IsOk = false;
                        }
                    }
                }
            }
            //}
        }
        catch (Exception ex)
        {
            OnErrorNew("百拓后台代付失败:" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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
    private bool PayForToday(PbProject.Model.Tb_Ticket_Order Order)
    {
        bool IsToday = true;
        try
        {
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

            string todayAccout = BS.JieKouZhangHao.Split('|')[4].Split('^')[0];

            string todayAccout2 = BS.JieKouZhangHao.Split('|')[4].Split('^')[1];
            string sql = " update Tb_Ticket_Order set ";


            w_TodayService.WTodayService WSvcToday = new w_TodayService.WTodayService();
            IList<PbProject.Model.Tb_Ticket_Passenger> passengerList = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL().GetPasListByOrderID(Order.OrderId);
            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            string PNRContent = skyList[0].PnrContent.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            string PATContent = skyList[0].Pat.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            OnErrorNew("今日开始生成订单，本地订单号：" + Order.OrderId, false);
            string PNRinfo = "O|P|" + Order.PNR + "^F^" + Order.BigCode + "|" + skyList[0].FromDate.ToString("yyyy-MM-dd") + "|" + skyList[0].FromCityCode + "|" + skyList[0].FromCityName + "|" + skyList[0].ToCityCode + "|" + skyList[0].ToCityName + "|" + skyList[0].CarryCode + skyList[0].FlightCode + "^N||" + skyList[0].FromDate.ToShortTimeString() + "|" + skyList[0].ToDate.ToShortTimeString() + "|" + skyList[0].Space + "|" + skyList[0].Discount + "||" + passengerList[0].PMFee + "|" + (skyList[0].ABFee + skyList[0].FuelFee) + "|" + Order.PassengerName.Split('/').Length + "|" + Order.PassengerName.Replace("/", "@");
            DataSet dsReson = WSvcToday.CreateOrderByPNR(todayAccout2, Order.PNR, Order.JinriGYCode, (Order.OldPolicyPoint).ToString(), Order.PolicyId, PNRinfo, "0");
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
                OnErrorNew("今日生成订单成功，本地订单号：" + Order.OrderId, false);
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
                    OnErrorNew("今日开始自动支付，本地订单号：" + Order.OrderId, false);
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
                        OnErrorNew("今日平台订单价格和本地价格不符，不进行代付，本地订单号：" + Order.OrderId, false);
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
                            OnErrorNew("今日自动代付成功，本地订单号：" + Order.OrderId, false);
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
                            OrderLog.OperContent = " 今日代付成功!";
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
            OnErrorNew("今日线下异常:" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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
    private bool PayFor8000Y(PbProject.Model.Tb_Ticket_Order Order)
    {
        bool Is8000Y = true;
        string sql = " update Tb_Ticket_Order set ";

        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
        BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

        string Accout8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[0];

        string Password8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[1];
        string Alipaycode8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[2];
        try
        {
            w_8000YService.W8000YService WSvc8000Y = new w_8000YService.W8000YService();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();

            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            FormatPNR ss = new FormatPNR();
            string PNRContent = skyList[0].PnrContent.Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            string PATContent = skyList[0].Pat.Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            OnErrorNew("8000Y开始生成订单，本地订单号：" + Order.OrderId, false);
            string reqPNRContent = "";

            reqPNRContent = skyList[0].PnrContent.Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            //if (skyList[0].A9 != "")
            //{
            //    reqPNRContent = skyList[0].A9.Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            //}
            //else
            //{
            //    reqPNRContent = (new PnrAnalysis.FormatPNR()).SplitPnrAutoLine(skyList[0].A7.Replace("", "").Replace("", "").Replace("", "").Replace("", ""));
            //}
            //string reqPNRContent = skyList[0].A9;// (new PnrAnalysis.FormatPNR()).SplitPnrAutoLine(skyList[0].A7);
            DataSet dsReson = null;// WSvc8000Y.CreatOrderNewByPNRNote(ComModel.A61, ComModel.A62, Order.PNR, Order.PolicyId, reqPNRContent);
            if (Order.TravelType == 1 || Order.TravelType == 2)
            {
                dsReson = WSvc8000Y.CreatOrderNewByPNRNote(Accout8000yi, Password8000yi, Order.PNR, Order.PolicyId, reqPNRContent);
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
                    OnErrorNew("8000Y生成订单成功，本地订单号：" + Order.OrderId, false);
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
                        OnErrorNew("8000Y开始自动支付，本地订单号：" + Order.OrderId, false);
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
                                OnErrorNew("8000Y自动代付成功，本地订单号：" + Order.OrderId, false);
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
                    errMsg = dsReson.Tables[0].Rows[0]["ErrInfo"].ToString();
                    OnErrorNew("8000Y下单失败，错误信息：" + errMsg + "|" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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
            OnErrorNew("8000Y线下异常:" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
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
    private bool bookPay(PbProject.Model.Tb_Ticket_Order Order)
    {
        bool Isbook = true;

        string sql = " update Tb_Ticket_Order set ";

        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
        BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

        string Accout51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[0];

        string Password51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[1];

        string Ag51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[2];

        string Url51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[3];
        try
        {
            w_51bookService._51bookService bookService = new w_51bookService._51bookService();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();

            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");

            string PNRContent = skyList[0].PnrContent.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            string PATContent = skyList[0].Pat.Replace("\n", "").Replace("\r", "").Replace("", "").Replace("", "").Replace("", "").Replace("", "");
            OnErrorNew("51book开始生成订单，本地订单号：" + Order.OrderId, false);
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
                    OnErrorNew("51book生成订单成功，本地订单号：" + Order.OrderId, false);
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
                        OnErrorNew("51book开始自动支付，本地订单号：" + Order.OrderId, false);
                        //如果51book价格比本地高，则不支付
                        if (Convert.ToDecimal(dsReson.Tables[0].Rows[0]["settlePrice"].ToString()) > (Order.PMFee + Order.ABFee + Order.FuelFee-Order.PolicyMoney))
                        //if (Math.Abs(Convert.ToDecimal(dsReson.Tables[0].Rows[0]["settlePrice"].ToString())-(Order.PMFee*Convert.ToDecimal(1-Convert.ToDouble(Order.OldPolicyPoint)*0.01) + Order.ABFee + Order.FuelFee))>1
                        //    || (Convert.ToDecimal(dsReson.Tables[0].Rows[0]["CommisionInfo"].ToString().Substring(0, dsReson.Tables[0].Rows[0]["CommisionInfo"].ToString().IndexOf("%"))) < Order.OldPolicyPoint))
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
                            OnErrorNew("51book平台订单价格和本地价格不符，不进行代付，本地订单号：" + Order.OrderId, false);
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
                                OnErrorNew("51book自动代付成功，本地订单号：" + Order.OrderId, false);
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
            OnErrorNew("51book线下异常:" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
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
    private bool PMPay(PbProject.Model.Tb_Ticket_Order Order)
    {
        bool IsPm = true;
        string sql = " update Tb_Ticket_Order set ";
        try
        {
            PMService.PMService PMService = new PMService.PMService();

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
            string pmAccout = BS.JieKouZhangHao.Split('|')[3].Split('^')[0];

            string pmPassword = BS.JieKouZhangHao.Split('|')[3].Split('^')[1];
            string pmAg = BS.JieKouZhangHao.Split('|')[3].Split('^')[2];

            OnErrorNew("票盟开始生成订单，本地订单号：" + Order.OrderId, false);
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
                    OnErrorNew("票盟生成订单成功，本地订单号：" + Order.OrderId, false);
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
                            || (Convert.ToDecimal(dsReson.Tables[1].Rows[0]["ticketfee"].ToString()) * Order.PassengerNumber != Order.PMFee)
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
            OnErrorNew("票盟线下异常:" + ex.Message + "，本地订单号：" + Order.OrderId, false);
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

    ///// <summary>
    ///// 添加订单日志
    ///// </summary>
    ///// <param name="OrderId">订单编号</param>
    ///// <param name="LoginName">登录帐号</param>
    ///// <param name="UserName">登陆名称</param>
    ///// <param name="UninCode">公司编号</param>
    ///// <param name="RoleType">角色类型</param>
    ///// <param name="UninAllName">公司全名</param>
    ///// <param name="OperType">操作类型：预订、支付、出票、修改</param>
    ///// <param name="Content">描述内容</param>
    //private void AddOrderLog(string OrderId, string LoginName, string UserName, string UninCode, int RoleType, string UninAllName,string OperType,string Content)
    //{
    //    #region 2.添加订单日志

    //    //2	OrderId	varchar	50	0	订单编号
    //    //3	OperType	varchar	10	0	操作类型：预订、支付、出票、修改等。
    //    //4	OperTime	datetime	23	3	操作时间
    //    //5	OperLoginName	varchar	50	0	操作员登录名
    //    //6	OperUserName	varchar	100	0	操作员名称
    //    //7	CpyNo	varchar	50	0	公司编号
    //    //8	CpyType	int	4	0	公司类型
    //    //9	CpyName	varchar	100	0	公司名称
    //    //10	OperContent	text	4	0	操作内容描述
    //    //11	WatchType	int	4	0	查看权限（1.平台 2.运营 3.供应 4.分销 5.采购）

    //    Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
    //    OrderLog.id = Guid.NewGuid();
    //    OrderLog.OrderId = OrderId;
    //    OrderLog.OperType = OperType;
    //    OrderLog.OperTime = DateTime.Now;
    //    OrderLog.OperLoginName = LoginName;
    //    OrderLog.OperUserName = UserName;
    //    OrderLog.CpyNo = UninCode;
    //    OrderLog.CpyType = RoleType;
    //    OrderLog.CpyName = UninAllName;
    //    OrderLog.OperContent = Content;
    //    OrderLog.WatchType = 2;

    //    #endregion
    //    string sql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
    //    PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
    //    sqlbase.ExecuteNonQuerySQLInfo(sql);
    //}
}