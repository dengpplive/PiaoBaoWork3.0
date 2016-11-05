using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;

public partial class SMS_SMSRateSetEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnback.PostBackUrl = string.Format("SMSRateSetManage.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            if (Request["id"] != null)
            {
                ViewState["id"] = Request["id"];
                GetOneSetInfo();
            }
        }
    }
    protected void GetOneSetInfo()
    {
        Tb_Sms_RateSet Mreteset = baseDataManage.CallMethod("Tb_Sms_RateSet", "GetById", null, new object[] { ViewState["id"].ToString() }) as Tb_Sms_RateSet;
        SmsRatesCount.Text = Mreteset.RatesCount.ToString();
        //SmsRatesMoney.Text = Mreteset.RatesMoney.ToString();
        SmsRatesRemark.Text = Mreteset.RatesRemark;
        SmsRatesUnitPrice.Text = Mreteset.RatesUnitPrice.ToString();
        rblSmsRatesState.SelectedValue = Mreteset.RatesState.ToString();
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string msg = "";
        IHashObject parameter = new HashObject();
        try
        {
            parameter.Add("CpyNo",mCompany.UninCode);
            //parameter.Add("RatesMoney",SmsRatesMoney.Text.Trim().Trim());
            parameter.Add("RatesMoney", int.Parse(SmsRatesCount.Text.Trim()) * float.Parse(SmsRatesUnitPrice.Text.Trim()));
            parameter.Add("RatesCount",SmsRatesCount.Text.Trim());
            parameter.Add("RatesUnitPrice",SmsRatesUnitPrice.Text.Trim());
            parameter.Add("RatesRemark",SmsRatesRemark.Text.Trim());
            parameter.Add("RatesState",rblSmsRatesState.SelectedValue);
            parameter.Add("RatesDateTime",DateTime.Now);
            if (ViewState["id"] != null)
            {
                 parameter.Add("id",ViewState["id"].ToString());
                 msg = (bool)baseDataManage.CallMethod("Tb_Sms_RateSet", "Update", null, new object[] { parameter }) == true ? "修改成功" : "修改失败";
            }
            else
            {
                 msg = (bool)baseDataManage.CallMethod("Tb_Sms_RateSet", "Insert", null, new object[] { parameter }) == true ? "添加成功" : "添加失败";
            }
        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}