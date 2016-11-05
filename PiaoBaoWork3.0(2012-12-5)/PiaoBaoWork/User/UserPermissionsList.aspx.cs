using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Logic.User;
using PbProject.Logic.ControlBase;

/// <summary>
/// 页面权限管理
/// </summary>
public partial class User_UserPermissionsList : BasePage
{
    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                this.currentuserid.Value = this.mUser.id.ToString();
                btnAddSel.PostBackUrl = string.Format("UserPermissionsEdit.aspx?Url=UserPermissionsList.aspx&currentuserid={0}", this.currentuserid.Value.ToString());
                int roleType = mCompany.RoleType;
                List<Bd_Base_Page> iPost = new Bd_Base_PageBLL().GetListByRoleType(roleType);

                ViewState["basePage"] = iPost;

                BindUserPermissions();
            }
        }
        catch (Exception)
        {

        }
        
    }

    /// <summary>
    /// 绑定数据
    /// </summary>
    public void BindUserPermissions()
    {
        try
        {
            List<User_Permissions> iPost = new User_PermissionsBLL().GetListByCpyNo(mUser.CpyNo);
            repUserPermissions.DataSource = iPost;
            repUserPermissions.DataBind();
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 显示功能页面
    /// </summary>
    /// <param name="permissionsId"></param>
    /// <returns></returns>
    public string BindPermissionsName(string permissionsId)
    {
        string msg = "";
        try
        {
            if (ViewState["basePage"] != null && permissionsId != null)
            {
                permissionsId+=",";
                string temp = "";
                int count = 0;

                List<Bd_Base_Page> iPost = ViewState["basePage"] as List<Bd_Base_Page>;

                foreach (var item in iPost)
                {
                    temp = "," + item.PageIndex + ",";
                    if (permissionsId.Contains(temp) && !string.IsNullOrEmpty(item.TwoMenuName))
                    {
                        count++;
                        msg += item.TwoMenuName + ",";

                        if (count % 10 == 0)
                            msg += "<br/>";
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }

        return msg;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void repUserPermissions_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemIndex != -1)
            {
                LinkButton lbtn = e.Item.FindControl("UpDateButton") as LinkButton;
                lbtn.PostBackUrl = "~/User/UserPermissionsEdit.aspx?id=" + lbtn.CommandArgument + "+&name=" + lbtn.CommandName + "&Url=UserPermissionsList.aspx&currentuserid=" + this.currentuserid.Value.ToString();
            }
        }
        catch (Exception ex)
        {

        }
    }
}