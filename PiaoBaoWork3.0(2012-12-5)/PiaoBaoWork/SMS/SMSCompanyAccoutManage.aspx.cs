using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.Model;

public partial class SMS_SMSCompanyAccoutManage : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            if (mCompany.RoleType == 1)
            {
                Trsmscount.Visible = false;
                Tbsmstemplate.Visible = false;
            }
            BindrblSmsTemplate();
        }
        BindSmsCountandSmsSendCounts();
    }

    /// <summary>
    /// 短信剩余条数和短信发送条数
    /// </summary>
    private void BindSmsCountandSmsSendCounts()
    {
        List<Tb_Sms_User> listSmsUser = baseDataManage.CallMethod("Tb_Sms_User", "GetList", null, new Object[] { "CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Sms_User>;
        //List<Tb_Sms_SendInfo> listSmsSendInfo = baseDataManage.CallMethod("Tb_Sms_SendInfo", "GetList", null, new Object[] { "CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Sms_SendInfo>;
        DataTable dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteStrSQL("select sum(SmsUserCount) sendcount from Tb_Sms_SendInfo where CpyNo = '"+mCompany.UninCode+"'");
        if (listSmsUser != null && listSmsUser.Count > 0 )
        {
            lbsmsCount.Text = listSmsUser[0].SmsRemainCount.ToString();
        }
        if (dt!=null && dt.Rows.Count>0)
        {
            lbsmsSendCount.Text = dt.Rows[0]["sendcount"].ToString();
        }
    }

    /// <summary>
    /// 绑定短信购买模板
    /// </summary>
    private void BindrblSmsTemplate()
    {
        string cpyno = "";
        if (mCompany.RoleType==2 || mCompany.RoleType==3)
        {
            cpyno = mCompany.UninCode.Substring(0, 6);
        }
        else if (mCompany.RoleType==4 || mCompany.RoleType==5)
        {
            cpyno = mSupCompany.UninCode;
        }
        List<Tb_Sms_RateSet> listRateset = baseDataManage.CallMethod("Tb_Sms_RateSet", "GetList", null, new Object[] { "RatesState=1 and CpyNo='"+cpyno+"' order by RatesCount asc" }) as List<Tb_Sms_RateSet>;
        if (listRateset != null && listRateset.Count > 0)
        {
            for (int i = 0; i < listRateset.Count; i++)
            {
                rblSmsTemplate.Items.Insert(i, new ListItem(listRateset[i].RatesCount.ToString() + "条/" + float.Parse(listRateset[i].RatesMoney.ToString()).ToString() + "元  &nbsp;&nbsp;&nbsp;", listRateset[i].id.ToString()));
            }
            rblSmsTemplate.Items[0].Selected = true;
        }
    }
    /// <summary>
    /// 查看充值明细
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnRechargeDetail_Click(object sender, EventArgs e)
    {
        Response.Redirect("Transaction.aspx?cpyno=" + mCompany.UninCode + "&flag=0&currentuserid=" + this.currentuserid.Value.ToString());
    }
    /// <summary>
    /// 查看发送明细
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSendDetail_Click(object sender, EventArgs e)
    {
        Response.Redirect("SmsSendInfo.aspx?cpyno=" + mCompany.UninCode + "&flag=0&currentuserid=" + this.currentuserid.Value.ToString());
    }
    /// <summary>
    /// 购买短信
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBuy_Click(object sender, EventArgs e)
    {
        
        if (rblSmsTemplate.SelectedValue == "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('请选择充值项！')</script>", false);
        }
        else
        {
            //选择的短信条数
            Tb_Sms_RateSet MRatesSet = baseDataManage.CallMethod("Tb_Sms_RateSet", "GetById", null, new object[] { rblSmsTemplate.SelectedValue.ToString() }) as Tb_Sms_RateSet;
            if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
            {
                //运营商短信
                List<Tb_Sms_User> listSmsUser = baseDataManage.CallMethod("Tb_Sms_User", "GetList", null, new Object[] { "CpyNo='" + mSupCompany.UninCode + "'" }) as List<Tb_Sms_User>;
                if (listSmsUser!=null&&listSmsUser.Count>0)
                {
                    if (listSmsUser[0].SmsRemainCount>=MRatesSet.RatesCount)
                    {
                        Response.Redirect("Payment.aspx?ratesetid=" + rblSmsTemplate.SelectedValue + "&currentuserid=" + this.currentuserid.Value.ToString());
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('运营商短信条数不足！')</script>", false);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('短信商不存在！')</script>", false);
                }
            }
            else
            {
                Response.Redirect("Payment.aspx?ratesetid=" + rblSmsTemplate.SelectedValue + "&currentuserid=" + this.currentuserid.Value.ToString());
            }
        }
    }
}