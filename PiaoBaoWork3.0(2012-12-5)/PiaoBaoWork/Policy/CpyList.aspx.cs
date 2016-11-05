using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;

public partial class Policy_CpyList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            BindCpyinfo();
            ViewState["policytype"] = Request["policytype"].ToString();
        }
    }
    protected void BindCpyinfo()
    {
        List<User_Company> list = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "RoleType in (2,3)" }) as List<User_Company>;
        Repeater.DataSource = list;
        Repeater.DataBind();
    }
    /// <summary>
    /// 确实
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btsure_Click(object sender, EventArgs e)
    {
        try
        {
            string names = "";
            int count = this.Repeater.Items.Count;
            for (var i = 0; i < count; i++)
            {
                if (((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.Repeater.Items[i].FindControl("cbItem")).Checked)
                {
                    string name = ((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.Repeater.Items[i].FindControl("cbItem")).Value;
                    if (name.Length != 0)
                    {
                        names += name + ",";
                    }
                }
            }
            names = names.TrimEnd(',');

            if (names.Length > 0)
            {
                Session["Cpynames"] = names;
                if (ViewState["policytype"].ToString()=="bd")
                {
                    Response.Redirect("PolicySupplyEdit.aspx?currentuserid=" + this.currentuserid.Value.ToString());
                }
                else
                {
                    Response.Redirect("PolicyShareTakeoffEdit.aspx?names=" + Server.HtmlEncode(names) + "&currentuserid=" + this.currentuserid.Value.ToString());

                }
            }
            else
            {
                if (ViewState["policytype"].ToString() == "bd")
                {
                    Response.Redirect("PolicySupplyEdit.aspx?currentuserid=" + this.currentuserid.Value.ToString(),false);
                }
                else
                {
                    Response.Redirect("PolicyShareTakeoffEdit.aspx?currentuserid=" + this.currentuserid.Value.ToString(),false);
                }
            }
        }
        catch (Exception ex)
        {
            
            throw ex;
        }
        
    }
    protected void btback_Click(object sender, EventArgs e)
    {
        if (ViewState["policytype"].ToString() == "bd")
        {
            Response.Redirect("PolicySupplyEdit.aspx?currentuserid=" + this.currentuserid.Value.ToString());
        }
        else
        {
            Response.Redirect("PolicyShareTakeoffEdit.aspx?currentuserid=" + this.currentuserid.Value.ToString());
        }
    }
}