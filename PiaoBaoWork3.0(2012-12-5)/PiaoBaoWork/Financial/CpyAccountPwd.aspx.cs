using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DataBase.Data;

public partial class Financial_CpyAccountPwd : BasePage
{
    public DataTable dt = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string type = Request["type"] == null ? "" : Request["type"].ToString();
            string cpyid = Request["cpyid"] == null ? "" : Request["cpyid"].ToString();
            dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_AccountInfo", "id='" + cpyid + "'");
            ViewState["dt"] = dt;
            ViewState["type"] = type;
            if (dt != null)
            {
                this.UserId.Text = dt.Rows[0]["LoginName"].ToString();
                this.UserName.Text = dt.Rows[0]["UninAllName"].ToString();
            }
            switch (type)
            {
                case "updatepwd":
                    this.troldpwd.Visible = true;
                    break;
                case "setpwd":
                    this.troldpwd.Visible = false;
                    break;
            }
        }
    }
    protected void lbtnPwd_Click(object sender, EventArgs e)
    {
        string msg = "";
        bool retulst = false;
        dt = (DataTable)ViewState["dt"];
        try
        {
            string oldPwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(OldPWD.Text.Trim());
            string newPwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(NewPwd.Text.Trim());
            if (NewPwd.Text.Trim() != RNewPWD.Text.Trim())
                msg = "两次输入的密码不一致!";
            HashObject parameter = new HashObject();
            switch (ViewState["type"].ToString())
            {
                case "updatepwd":
                    if (dt.Rows[0]["LoginPassWord"].ToString() != oldPwd)
                        msg = "原密码错误!";
                    else if (oldPwd == newPwd)
                        msg = "原密码和新密码不能一样！";
                    break;
            }
            if (msg == "")
            {
                parameter.Add("id", dt.Rows[0]["id"]);
                parameter.Add("AccountPwd", newPwd);
                retulst = new PbProject.Logic.User.User_CompanyBLL().UpdateById(parameter);
                if (retulst)
                {
                    msg = "成功!";
                    mCompany.AccountPwd = newPwd;
                }
                else
                    msg = "失败!";
            }
        }
        catch (Exception)
        {
            msg = "操作失败!";
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "456", "showdialog('" + msg + "');", true);
    }
}