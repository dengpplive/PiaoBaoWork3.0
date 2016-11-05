using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using System.Data;
public partial class Financial_AccountChange : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["payUnincode"] = Request.QueryString["unincode"];
            ViewState["uninAllNAME"] = Request.QueryString["uninAllNAME"];//操作公司名称
            ViewState["MaxDebtMoney"] = Request.QueryString["MaxDebtMoney"];
            ViewState["AccountMoney"] = Request.QueryString["AccountMoney"];
            ViewState["MaxDebtDays"] = Request.QueryString["MaxDebtDays"];
            ViewState["OperUserName"] = mUser.UserName;//操作人名称
            ViewState["recUnincode"] = mCompany.UninCode;
            ViewState["RecCpyType"] = mCompany.RoleType;//收款方公司类型
            ViewState["RecCpyName"] = mCompany.UninAllName;//收款方公司名称
            ViewState["OperLoginName"] = mUser.LoginName;//操作人登录名
            ViewState["OperUserName"] = mUser.UserName;//操作人名称


            if (ViewState["AccountMoney"].ToString().Trim() == "")
            {
                ViewState["AccountMoney"] = "0";
            }
            lblBeforehandFund.Text = ViewState["AccountMoney"].ToString();//可用预存款
            lblUninAllName.Text = Server.UrlDecode(ViewState["uninAllNAME"].ToString());//下级公司名称
            lblMaxDebtMoney.Text = ViewState["MaxDebtMoney"].ToString();//最大欠款额度
            lblMaxdays.Text = ViewState["MaxDebtDays"].ToString();
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        try
        {


            if (txtFee.Text.Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调整金额不能为空');", true);
                return;
            }
            if (decimal.Parse(txtFee.Text.Trim())<= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调整金额不能为负数,如果是扣款,请选择调整方式为代扣');", true);
                return;
            }
            if (ViewState["payUnincode"] != null && ViewState["payUnincode"].ToString().Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调账账号失效,请重新登录');", true);
                return;
            }
            if (ViewState["payUnincode"] != null && ViewState["recUnincode"].ToString().Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('登录账号失效,请重新登录');", true);
                return;
            }


            string orderId = "";
            HashObject hparamsOrder = new HashObject();
            hparamsOrder.Add("Number", "3");
            DataTable dt = baseDataManage.EexcProc("GetNewOrderId", hparamsOrder);
            orderId = dt.Rows[0][0].ToString();

            HashObject hparams = new HashObject();
            hparams.Add("recUnincode", ViewState["recUnincode"].ToString().Trim());//收款方账号
            hparams.Add("payUnincode", ViewState["payUnincode"].ToString().Trim());//支付方账号
            hparams.Add("type", rblState.SelectedValue);//0充值,1代扣
            hparams.Add("PayMoney", txtFee.Text.Trim());
            hparams.Add("OperReason", txtReason.Text.Trim());
            hparams.Add("RecCpyType", ViewState["RecCpyType"].ToString());
            hparams.Add("RecCpyName", ViewState["RecCpyName"].ToString());
            hparams.Add("OperLoginName", ViewState["OperLoginName"].ToString());
            hparams.Add("OperUserName", ViewState["OperUserName"].ToString());
            hparams.Add("orderid", orderId);
            decimal decimalBeforehandFund = 0.0M;
            if (rblState.SelectedValue == "0")
            {
                decimalBeforehandFund = decimal.Parse(lblBeforehandFund.Text) + decimal.Parse(txtFee.Text.Trim());
            }
            if (rblState.SelectedValue == "1")
            {
                decimalBeforehandFund = decimal.Parse(lblBeforehandFund.Text) - decimal.Parse(txtFee.Text.Trim());
            }
            int count = baseDataManage.ExecuteNonQueryProcedure("proc_AccountChange", hparams);
            if (count > 0)
            {
                txtFee.Text = "";
                txtReason.Text = "";
                txtMaxDebtMoney.Text = "";
                rblState.SelectedIndex = 0;
                lblBeforehandFund.Text = decimalBeforehandFund.ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调账成功');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调账失败');", true);
            }
        }
        catch (Exception)
        {

            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('未知错误,请联系管理员');", true);
        }
    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtMaxDebtMoney.Text.Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('最大欠款不能为空');", true);
                return;
            }
            if (ViewState["payUnincode"] != null && ViewState["payUnincode"].ToString().Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调账账号失效,请重新登录');", true);
                return;
            }
            if (decimal.Parse(txtMaxDebtMoney.Text.Trim()) < 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('最大欠款额度不能为负数');", true);
                return;
            }
            string sqlwhere = " 1=1";
            sqlwhere += " and uninCode = '" + ViewState["payUnincode"].ToString().Trim() + "' ";

            List<User_Company> objList = baseDataManage.CallMethod("User_Company", "GetList", null, new object[] { sqlwhere }) as List<User_Company>;
            string MaxDebtMoney = "";
            string MaxDebtDays = txtMaxDebtDays.Text.Trim();
            if (objList == null || objList.Count <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('未能正确读取调账公司账号,请重新登录');", true);
                return;
            }
            else
            {
                //如果支付公司已经欠款,则欠款的绝对值不能大于最大欠款的
                if (objList[0].AccountMoney < 0)
                {
                    if (Math.Abs(objList[0].AccountMoney) >decimal.Parse(txtMaxDebtMoney.Text) )
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('最大欠款度不能低于当前公司已经欠款额度');", true);
                        return;
                    }
                }
            }
            MaxDebtMoney = txtMaxDebtMoney.Text;
            int count = baseDataManage.UPDATEsql("update User_Company set MaxDebtMoney=" + MaxDebtMoney + ",MaxDebtDays="+MaxDebtDays+" where unincode='" + ViewState["payUnincode"].ToString().Trim() + "'");
            if (count > 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调整成功');", true);
                txtFee.Text = "";
                txtReason.Text = "";
                txtMaxDebtMoney.Text = "";
                lblMaxDebtMoney.Text = MaxDebtMoney;
                txtMaxDebtDays.Text = "";
                lblMaxdays.Text = MaxDebtDays;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('调整失败');", true);
            }
        }
        catch (Exception)
        {

            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('未知错误,请联系管理员');", true);
        }
    }
}