using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.Model;
using DataBase.Data;
using PbProject.WebCommon.Utility;

public partial class SMS_Payment : BasePage
{
    protected Tb_Sms_RateSet MRatesSet = new Tb_Sms_RateSet();
    protected string SmsRatesMoney = "";
    protected string SmsRatesCount = "";
    protected string SmsRatesUnitPrice = "";
    protected string SmsRatesRemark = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            if (Request["ratesetid"] != null)
            {
                ViewState["ratesetid"] = Request["ratesetid"].ToString();
                DataBindInfo();
                BindPayType();
            }
        }
    }
    /// <summary>
    /// 页面信息绑定
    /// </summary>
    private void DataBindInfo()
    {
        MRatesSet = baseDataManage.CallMethod("Tb_Sms_RateSet", "GetById", null, new object[] { ViewState["ratesetid"].ToString() }) as Tb_Sms_RateSet;
        SmsRatesMoney = MRatesSet.RatesMoney.ToString("f2");
        SmsRatesCount = MRatesSet.RatesCount.ToString();
        SmsRatesUnitPrice = MRatesSet.RatesUnitPrice.ToString("f2");
        SmsRatesRemark = MRatesSet.RatesRemark;
        lblPay.Text = MRatesSet.RatesMoney.ToString("f2");
        ViewState["MRatesSet"] = MRatesSet;
    }
   
    //生成短信充值订单
    /// <summary>
    /// 生成短信充值订单
    /// </summary>
    private void CreateOrder()
    {
        if (hidPayWay.Value!=null && hidPayWay.Value !="")
        {
            MRatesSet = (Tb_Sms_RateSet)ViewState["MRatesSet"];
            //短信充值记录--充值订单
            Tb_Sms_ReCharge MRecharge = new Tb_Sms_ReCharge();
            //短信参数信息
            if (MRatesSet != null && MRatesSet.RatesMoney > 0)
            {
                //生成订单号
                string orderid = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetNewOrderId("2");
                if (orderid != "")
                {
                    IHashObject parameter = new HashObject();
                    Random Rdm = new Random(); 
                    MRecharge.OrderId = orderid;
                    MRecharge.CpyNo = mCompany.UninCode;
                    MRecharge.InPayNo = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetIndexId();  //DateTime.Now.ToString("yyMMddHHmmss") + Rdm.Next(100, 999).ToString();
                    MRecharge.PayNo = "";  //DateTime.Now.ToString("yyMMddHHmmss");
                    MRecharge.ReChargeMoney = MRatesSet.RatesMoney;
                    MRecharge.ReChargeCount = MRatesSet.RatesCount;
                    MRecharge.ReChargeDate = DateTime.Now;
                    MRecharge.ReChargeState = 0;
                    MRecharge.PayType = int.Parse(hidPayWay.Value);
                    //生成订单
                    bool rsorder = (bool)baseDataManage.CallMethod("Tb_Sms_ReCharge", "Insert", null, new object[] { MRecharge });
                    List<Tb_Sms_User> listsmsUser = baseDataManage.CallMethod("Tb_Sms_User", "GetList", null, new Object[] { " CpyNo= '"+mCompany.UninCode+"'" }) as List<Tb_Sms_User>;
                    if (listsmsUser.Count<1)
                    {
                        Tb_Sms_User Msmsuser = new Tb_Sms_User();
                        Msmsuser.CpyNo = mCompany.UninCode;
                        Msmsuser.SmsCount = 0;
                        Msmsuser.SmsRemainCount = 0;
                        Msmsuser.SmsDate = Convert.ToDateTime(DateTime.Now);
                        Msmsuser.SmsUserType = mCompany.RoleType;
                        bool rssmsuser = (bool)baseDataManage.CallMethod("Tb_Sms_User", "Insert", null, new object[] { Msmsuser });
                    }
                    if (rsorder == true)
                    {
                            Session["SmsRecharge"] = MRecharge;

                            ////跳转支付页面
                            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "openUrl('" + orderid + "');", true);
                           
                    }
                    else
	                {
                        Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), "showdialogOne('支付失败');", true);
	                }
                }
            }
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), "showdialogOne('请选择支付方式');", true);
            return;
        }
        
    }
    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPay_Click(object sender, EventArgs e)
    {
        string pwd = CommonManage.TrimSQL(txtAccountPayPwd.Text.Trim());
        string payWay = hidPayWay.Value.ToString();
        pwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(pwd);//原支付密码

        //账户余额
        if (payWay == "14" && mCompany.AccountPwd != pwd)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), "showdialogOne('支付失败!支付密码错误！');", true);
        }
        else
        {
            CreateOrder();
        }
        
        DataBindInfo();
    }
    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Response.Redirect("SMSCompanyAccoutManage.aspx?currentuserid=" + this.currentuserid.Value.ToString());
    }
    private void BindPayType()
    {
        try
        {

            string gYcpyNo = mUser.CpyNo.Substring(0, 12);

            if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
            {
                gYcpyNo = mUser.CpyNo.Substring(0, 6); //取平台收款账号
            }
            else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
            {
                gYcpyNo = mUser.CpyNo.Substring(0, 12);//取供应运营收款账号
            }
            string wangYinZhangHao = PbProject.Model.definitionParam.paramsName.wangYinZhangHao;
            string wangYinLeiXing = PbProject.Model.definitionParam.paramsName.wangYinLeiXing;
            string sqlWhere = " CpyNo='" + gYcpyNo + "' and (SetName='" + wangYinZhangHao + "' or SetName='" + wangYinLeiXing + "')";
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
                string strGongYingKongZhiFenXiao = PbProject.WebCommon.Utility.BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
               
                //余额支付
                if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                {
                    if (strGongYingKongZhiFenXiao != null && !strGongYingKongZhiFenXiao.Contains("|10|"))//分销购买短信只能用账户余额
                    {
                         if (zhangHao != null && !string.IsNullOrEmpty(zhangHao.SetValue) && zhangHao.SetValue.Contains("|"))
                        {
                            string[] setValues = zhangHao.SetValue.Split('|');
                            string[] setValue0 = setValues[0].Split('^');
                            if (!string.IsNullOrEmpty(setValue0[1]))
                            {
                                //支付宝
                                rblPayType.Items.Add(new ListItem("支付宝", "1"));
                                temp += "5,";
                            }
                            string[] setValue1 = setValues[1].Split('^');
                            if (!string.IsNullOrEmpty(setValue1[1]))
                            {
                                //快钱
                                rblPayType.Items.Add(new ListItem("快钱", "2"));
                                temp += "6,";
                            }
                            string[] setValue2 = setValues[2].Split('^');
                            if (!string.IsNullOrEmpty(setValue2[1]))
                            {
                                //汇付
                                rblPayType.Items.Add(new ListItem("汇付", "3"));
                                temp += "7,";
                            }
                            string[] setValue3 = setValues[3].Split('^');
                            if (!string.IsNullOrEmpty(setValue3[1]))
                            {
                                //财付通
                                rblPayType.Items.Add(new ListItem("财付通", "4"));
                                temp += "8,";
                            }
                        }
                    }

                    //账户余额支付
                    if (strGongYingKongZhiFenXiao != null && strGongYingKongZhiFenXiao.Contains("|76|"))
                    {
                        rblPayType.Items.Add(new ListItem("账户余额支付", "14"));
                        temp += "14,";
                    }
                }
                else
                {
                    if (zhangHao != null && !string.IsNullOrEmpty(zhangHao.SetValue) && zhangHao.SetValue.Contains("|"))
                    {
                        string[] setValues = zhangHao.SetValue.Split('|');
                        string[] setValue0 = setValues[0].Split('^');
                        if (!string.IsNullOrEmpty(setValue0[1]))
                        {
                            //支付宝
                            rblPayType.Items.Add(new ListItem("支付宝", "1"));
                            temp += "5,";
                        }
                        string[] setValue1 = setValues[1].Split('^');
                        if (!string.IsNullOrEmpty(setValue1[1]))
                        {
                            //快钱
                            rblPayType.Items.Add(new ListItem("快钱", "2"));
                            temp += "6,";
                        }
                        string[] setValue2 = setValues[2].Split('^');
                        if (!string.IsNullOrEmpty(setValue2[1]))
                        {
                            //汇付
                            rblPayType.Items.Add(new ListItem("汇付", "3"));
                            temp += "7,";
                        }
                        string[] setValue3 = setValues[3].Split('^');
                        if (!string.IsNullOrEmpty(setValue3[1]))
                        {
                            //财付通
                            rblPayType.Items.Add(new ListItem("财付通", "4"));
                            temp += "8,";
                        }
                    }
                }
                //判断网银
                if (leiXing != null && !string.IsNullOrEmpty(leiXing.SetValue) && leiXing.SetValue != "0")
                {
                    if (temp.Contains(leiXing.SetValue))
                    {
                        rblPayType.Items.Insert(0, new ListItem("网银", leiXing.SetValue));

                        hidWangYingType.Value = leiXing.SetValue;
                    }
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
                    hidPayWay.Value = rblPayType.Items[0].Value; //支付方式
                    spanid.Visible = true;
                }
                else
                {
                    spanid.Visible = false;

                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('不能进入该页面!');history.go(-1);", true);
                }
            }
        }
        catch (Exception)
        {

        }
    }
}