using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;

public partial class SMS_SmsCpyGroup : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            BindCpy();
        }
    }
    /// <summary>
    /// 绑定下级商家
    /// </summary>
    protected void BindCpy()
    {

        List<User_Company> companylist = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { Query() }) as List<User_Company>;
        Repeater.DataSource = companylist;
        Repeater.DataBind();
       
    }
    protected string Query()
    {
        string strWhere = "";
        if (mCompany.RoleType == 2)
        {
            //运营商直属下级
            strWhere = "RoleType in (4,5) and len(UninCode)=18 and SUBSTRING(UninCode,1,12)='" + mCompany.UninCode + "'";
        }
        else if (mCompany.RoleType == 4)
        {
            //分销直属下级
            if (mCompany.UninCode.Length == 18)
            {
                strWhere = "RoleType in (4,5) and len(UninCode)=24 and SUBSTRING(UninCode,1,18)='" + mCompany.UninCode + "'";
            }
            else if (mCompany.UninCode.Length == 24)
            {
                //只有采购
                strWhere = "RoleType = 5 and len(UninCode)=30 and SUBSTRING(UninCode,1,24)='" + mCompany.UninCode + "'";
            }
        }
        else if (mCompany.RoleType == 3 || mCompany.RoleType == 5)
        {
            strWhere = " 1<>1";
        }
        else
        {
            //平台下级
            strWhere = "RoleType in (2,3)";
        }
        //常旅客姓名
        if (txtName.Text.Trim() != "")
        {
            strWhere += " and UninAllName like '%" + txtName.Text.Trim() + "%'";
        }
        //电话
        if (txtTel.Text.Trim() != "")
        {
            strWhere += " and ContactTel like '%" + txtTel.Text.Trim() + "%'";
        }
        return strWhere;
    }
    /// <summary>
    /// 确定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btok_Click(object sender, EventArgs e)
    {
        string tels = "";
        int count = this.Repeater.Items.Count;
        DateTime dt = DateTime.Now;
        for (var i = 0; i < count; i++)
        {
            if (((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.Repeater.Items[i].FindControl("cbItem")).Checked)
            {
                string id = string.Empty;
                string tel = ((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.Repeater.Items[i].FindControl("cbItem")).Value;
                if (tel.Length != 0)
                {
                    tels += tel + ",";
                }
            }
        }
        DateTime dt1 = DateTime.Now;
        if (tels.Length > 0)
        {
            tels = tels.Substring(0, tels.Length - 1);
            Response.Redirect("SmsSend.aspx?tels=" + tels + "&currentuserid=" + this.currentuserid.Value.ToString());
        }
        else
        {
            Response.Redirect("SmsSend.aspx?currentuserid=" + this.currentuserid.Value.ToString());
        }
    }
    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btback_Click(object sender, EventArgs e)
    {
        Response.Redirect("SmsSend.aspx?currentuserid=" + this.currentuserid.Value.ToString());
    }
    protected void btsel_Click(object sender, EventArgs e)
    {
        BindCpy();
    }
}