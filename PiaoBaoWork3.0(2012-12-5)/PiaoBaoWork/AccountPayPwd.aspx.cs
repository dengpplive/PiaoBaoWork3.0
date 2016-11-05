using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;

/// <summary>
/// 修改密码
/// </summary>
public partial class AccountPayPwd : BasePage
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
                if (string.IsNullOrEmpty(mCompany.AccountPwd))
                {
                    //新设置
                    trOldPwd.Visible = false;
                    OldPWD.Text = "******";
                }
                else
                {
                    //修改 ,暂时未处理
                }

                Bind();
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// Bind
    /// </summary>
    public void Bind()
    {
        try
        {
            UserId.Text = mUser.LoginName;
            UserName.Text = mUser.UserName;
        }
        catch (Exception ex)
        {
            
        }
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnPwd_Click(object sender, EventArgs e)
    {
        string msg = "";

        try
        {

            bool retulst = false;

            string oldPwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(OldPWD.Text.Trim());//原支付密码
            string newPwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(NewPwd.Text.Trim()); //新支付密码

            if (NewPwd.Text.Trim() != RNewPWD.Text.Trim())
                msg = "两次输入的密码不一致!";

            if (string.IsNullOrEmpty(mCompany.AccountPwd))
            {
                //新设置
            }
            else
            {
                //修改 ,暂时未处理
                if (mCompany.AccountPwd != oldPwd)
                    msg = "原支付密码错误!";
                else if (oldPwd == newPwd)
                    msg = "原支付密码和新支付密码不能一样！";
                
            }


            if (msg == "")
            {
                HashObject parameter = new HashObject();

                parameter.Add("id", mCompany.id);
                parameter.Add("AccountPwd", newPwd);

                retulst = new PbProject.Logic.User.User_CompanyBLL().UpdateById(parameter);

                if (retulst)
                    msg = "密码修改成功!请重新登录!";
                else
                    msg = "密码修改失败!";
            }
        }
        catch (Exception)
        {

        }

        ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }
}