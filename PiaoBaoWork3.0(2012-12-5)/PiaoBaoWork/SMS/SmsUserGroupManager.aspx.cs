using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using DataBase.Data;
using PbProject.Model;

public partial class SMS_SmsUserGroupManager : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            PageDataBind();
        }
    }
    /// <summary>
    /// 绑定常旅客
    /// </summary>
    private void PageDataBind()
    {
        List<User_Flyer> list = baseDataManage.CallMethod("User_Flyer", "GetList", null, new object[] { Query() }) as List<User_Flyer>;
        Repeater.DataSource = list;
        Repeater.DataBind();    
    }
    protected string Query()
    {
        StringBuilder SQLWhere = new StringBuilder();
        SQLWhere.AppendFormat(" CpyNo='{0}' ", mCompany.UninCode);
        //常旅客姓名
        if (txtName.Text.Trim() != "")
        {
            SQLWhere.AppendFormat(" and Name like '%{0}%' ", txtName.Text.Trim());
        }
        //电话
        if (txtTel.Text.Trim() != "")
        {
            SQLWhere.AppendFormat(" and Tel like '%{0}%' ", txtTel.Text.Trim());
        }
        return SQLWhere.ToString();
    }
   /// <summary>
   /// 确定
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e)
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
    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Redirect("SmsSend.aspx?currentuserid=" + this.currentuserid.Value.ToString());
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button3_Click(object sender, EventArgs e)
    {
        PageDataBind();
    }
}