using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

public partial class Financial_CpyBankAccountEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                GetCpyBankAccount();
            }
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    /// <summary>
    /// 获取要修改的信息
    /// </summary>
    protected void GetCpyBankAccount()
    {
        try
        {
            Tb_Company_BankAccount maccount = baseDataManage.CallMethod("Tb_Company_BankAccount", "GetById", null, new Object[] { ViewState["Id"].ToString() }) as Tb_Company_BankAccount;
            txtAccount.Text = maccount.Account;
            txtAccountBank.Text = maccount.AccountBank;
            txtAccountUserName.Text = maccount.OperUserName;
            txtBankName.Text = maccount.BankName;
            rblSignFlag.SelectedValue = maccount.SignFlag.ToString();
            ddlBankType.SelectedValue = maccount.BankType.ToString();
            ddlUseType.SelectedValue = maccount.UseType.ToString();
            trshow.Visible = maccount.BankType == 1 ? true : false;
        }
        catch (Exception)
        {
            throw;
        }

    }
    /// <summary>
    /// 账号类型改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlBankType_SelectedIndexChanged(object sender, EventArgs e)
    {
        trshow.Visible = ddlBankType.SelectedValue == "1" ? true : false;
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbsave_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            IHashObject parameter = new HashObject();
            parameter.Add("CpyNo", mCompany.UninCode);
            parameter.Add("CpyName",mCompany.UninAllName);
            parameter.Add("CpyType",mCompany.RoleType);
            parameter.Add("OperTime",DateTime.Now);
            parameter.Add("OperLoginName",mUser.LoginName);
            parameter.Add("OperUserName",mUser.UserName);
            parameter.Add("BankType",ddlBankType.SelectedValue);
            parameter.Add("UseType",ddlUseType.SelectedValue);
            parameter.Add("BankName", txtBankName.Text.Trim());
            parameter.Add("AccountBank", txtAccountBank.Text.Trim());
            parameter.Add("Account", txtAccount.Text.Trim());
            parameter.Add("AccountUserName", txtAccountUserName.Text.Trim());
            parameter.Add("SignFlag",rblSignFlag.SelectedValue);
            if (ViewState["Id"]!=null)
            {
                #region 修改
                parameter.Add("id", ViewState["Id"].ToString());
                msg = (bool)baseDataManage.CallMethod("Tb_Company_BankAccount", "Update", null, new object[] { parameter }) == true ? "更新成功" : "更新失败";
                #endregion
            }
            else
            {
                #region 添加
                List<Tb_Company_BankAccount> list = baseDataManage.CallMethod("Tb_Company_BankAccount", "GetList", null, new Object[] { "CpyNo='" + mCompany.UninCode + "' and BankType=" + int.Parse(ddlBankType.SelectedValue) + " and UseType=" + int.Parse(ddlUseType.SelectedValue) }) as List<Tb_Company_BankAccount>;
                if (list!=null&&list.Count>0)
                {
                    msg = "该公司已存在此账号类型和使用类型，添加失败!";
                }
                else
                {
                    msg = (bool)baseDataManage.CallMethod("Tb_Company_BankAccount", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
                }
                
                #endregion
            }
        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}