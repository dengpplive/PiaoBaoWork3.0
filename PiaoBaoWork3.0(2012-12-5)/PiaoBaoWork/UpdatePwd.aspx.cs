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
public partial class UpdatePwd : BasePage
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
                Bind();
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// Bind
    /// </summary>
    public void Bind()
    {
        UserId.Text = mUser.LoginName;
        UserName.Text = mUser.UserName;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnPwd_Click(object sender, EventArgs e)
    {
        string msg = "";
        bool retulst = false;

        try
        {
            string oldPwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(OldPWD.Text.Trim());
            string newPwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(NewPwd.Text.Trim());

            if (mUser.LoginPassWord != oldPwd)
                msg = "原密码错误!";
            else if (oldPwd == newPwd)
                msg = "原密码和新密码不能一样！";
            else if (NewPwd.Text.Trim() != RNewPWD.Text.Trim())
                msg = "两次输入的密码不一致!";

            if (msg == "")
            {
                HashObject parameter = new HashObject();
                parameter.Add("id", mUser.id);
                parameter.Add("LoginPassWord", newPwd);
                retulst = new PbProject.Logic.User.User_EmployeesBLL().UpdateById(parameter);
                if (retulst)
                    msg = "密码修改成功!请重新登录!";
                else
                    msg = "密码修改失败!";
            }
        }
        catch (Exception)
        {
            msg = "密码修改失败!";
        }
        
        ScriptManager.RegisterStartupScript(this, GetType(), "456", "showdialog('" + msg + "');", true);
    }
}