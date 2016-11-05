using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.ControlBase;
using PbProject.Model;
using DataBase.Data;
using PbProject.Logic.ControlBase;
using PbProject.Dal.Mapping;

/// <summary>
/// 页面权限管理
/// </summary>
public partial class Manager_Base_BasePageList : BasePage
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSel_Click(object sender, EventArgs e)
    {
        RepBasePageBind();
    }

    /// <summary>
    /// 查询
    /// </summary>
    protected void RepBasePageBind()
    {
        try
        {
            int roleType = int.Parse(ddlType.SelectedValue);
            List<Bd_Base_Page> bPaseList = new Bd_Base_PageBLL().GetListByRoleType(roleType);
            List<Bd_Base_Page> bPaseListNew = new List<Bd_Base_Page>();

            #region 筛选

            Bd_Base_Page[] modelArray = bPaseList.ToArray<Bd_Base_Page>();

            IQueryable<Bd_Base_Page> query = modelArray.AsQueryable();

            if (!string.IsNullOrEmpty(txtModuleIndex.Text))
            {
                query = query.Where(c => c.ModuleIndex == int.Parse(txtModuleIndex.Text));
            }
            if (!string.IsNullOrEmpty(txtModuleName.Text))
            {
                query = query.Where(c => c.ModuleName.Contains(txtModuleName.Text));
            }
            if (!string.IsNullOrEmpty(txtOneMenuIndex.Text))
            {
                query = query.Where(c => c.OneMenuIndex == int.Parse(txtOneMenuIndex.Text));
            }
            if (!string.IsNullOrEmpty(txtOneMenuName.Text))
            {
                query = query.Where(c => c.OneMenuName.Contains(txtOneMenuName.Text));
            }
            if (!string.IsNullOrEmpty(txtTwoMenuIndex.Text))
            {
                query = query.Where(c => c.TwoMenuIndex == int.Parse(txtTwoMenuIndex.Text));
            }
            if (!string.IsNullOrEmpty(txtTwoMenuName.Text))
            {
                query = query.Where(c => c.TwoMenuName.Contains(txtTwoMenuName.Text));
            }
            if (!string.IsNullOrEmpty(txtPageIndex.Text))
            {
                query = query.Where(c => c.PageIndex == int.Parse(txtPageIndex.Text));
            }
            if (!string.IsNullOrEmpty(txtPageName.Text))
            {
                query = query.Where(c => c.PageName.Contains(txtPageName.Text));
            }
            if (!string.IsNullOrEmpty(txtPageURL.Text))
            {
                query = query.Where(c => c.PageURL.Contains(txtPageURL.Text));
            }

            bPaseListNew = query.ToList<Bd_Base_Page>();


            #endregion

            #region 排序
            IEnumerable<Bd_Base_Page> querys = null;
            querys = from aa in bPaseListNew orderby aa.ModuleIndex, aa.OneMenuIndex, aa.TwoMenuIndex, aa.PageIndex select aa;
            bPaseListNew = querys.ToList<Bd_Base_Page>();

            #endregion

            RepBasePage.DataSource = bPaseListNew;
            RepBasePage.DataBind();
        }
        catch (Exception ex)
        {
            RepBasePage.DataSource = null;
            RepBasePage.DataBind();
        }
    }

    /// <summary>
    /// 操作
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RepBasePage_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        bool result = false;

        try
        {
            string id = e.CommandArgument.ToString(); //要修改的 id



            if (e.CommandName == "Update")
            {
                string strModuleIndex = (e.Item.FindControl("txt_ModuleIndex") as TextBox).Text.Trim();
                string strModuleName = (e.Item.FindControl("txt_ModuleName") as TextBox).Text.Trim();
                string strOneMenuIndex = (e.Item.FindControl("txt_OneMenuIndex") as TextBox).Text.Trim();
                string strOneMenuName = (e.Item.FindControl("txt_OneMenuName") as TextBox).Text.Trim();
                string strTwoMenuIndex = (e.Item.FindControl("txt_TwoMenuIndex") as TextBox).Text.Trim();
                string strTwoMenuName = (e.Item.FindControl("txt_TwoMenuName") as TextBox).Text.Trim();
                string strPageIndex = (e.Item.FindControl("txt_PageIndex") as TextBox).Text.Trim();
                string strPageName = (e.Item.FindControl("txt_PageName") as TextBox).Text.Trim();
                string strPageURL = (e.Item.FindControl("txt_PageURL") as TextBox).Text.Trim();
                string strRemark = (e.Item.FindControl("txt_Remark") as TextBox).Text.Trim();

                HashObject paramter = new HashObject();

                paramter.Add("id", id);
                paramter.Add("ModuleIndex", strModuleIndex);
                paramter.Add("ModuleName", strModuleName);
                paramter.Add("OneMenuIndex", strOneMenuIndex);
                paramter.Add("OneMenuName", strOneMenuName);
                paramter.Add("TwoMenuIndex", strTwoMenuIndex);
                paramter.Add("TwoMenuName", strTwoMenuName);
                paramter.Add("PageIndex", strPageIndex);
                paramter.Add("PageName", strPageName);
                paramter.Add("PageURL", strPageURL);
                paramter.Add("Remark", strRemark);

                result = (bool)baseDataManage.CallMethod("Bd_Base_Page", "Update", null, new Object[] { paramter });

                if (result)
                {
                    msg = "修改成功！";
                }
                else
                {
                    msg = "修改失败！";
                }

            }
            else if (e.CommandName == "Del")
            {
                result = (bool)baseDataManage.CallMethod("Bd_Base_Page", "DeleteById", null, new Object[] { id });

                if (result)
                {
                    msg = "删除成功！";
                }
                else
                {
                    msg = "删除失败";
                }
            }
        }
        catch (Exception)
        {
            msg = "操作失败";
        }

        if (result)
        {
            RepBasePageBind();
        }

        ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string msg = "";
        bool result = false;

        try
        {
            #region 验证数据

            if (string.IsNullOrEmpty(txtModuleIndex.Text))
            {
                msg = "数据不能为空！";
            }
            if (string.IsNullOrEmpty(txtModuleName.Text))
            {
                msg = "数据不能为空！";
            }
            if (string.IsNullOrEmpty(txtOneMenuIndex.Text))
            {
                msg = "数据不能为空！";
            }
            if (string.IsNullOrEmpty(txtOneMenuName.Text))
            {
                msg = "数据不能为空！";
            }
            if (string.IsNullOrEmpty(txtTwoMenuIndex.Text))
            {
                msg = "数据不能为空！";
            }
            if (string.IsNullOrEmpty(txtTwoMenuName.Text))
            {
                msg = "数据不能为空！";
            }
            //if (string.IsNullOrEmpty(txtPageIndex.Text))
            //{
            //    msg = "数据不能为空！";
            //}
            if (string.IsNullOrEmpty(txtPageName.Text))
            {
                msg = "数据不能为空！";
            }
            if (string.IsNullOrEmpty(txtPageURL.Text))
            {
                msg = "数据不能为空！";
            }
            if (string.IsNullOrEmpty(txtRemark.Text))
            {

            }
            #endregion

            if (msg == "")
            {
                /*
                HashObject paramter = new HashObject();

                paramter.Add("id", Guid.NewGuid());
                paramter.Add("ModuleIndex", txtModuleIndex.Text.Trim());
                paramter.Add("ModuleName", txtModuleName.Text.Trim());
                paramter.Add("OneMenuIndex", txtOneMenuIndex.Text.Trim());
                paramter.Add("OneMenuName", txtOneMenuName.Text.Trim());
                paramter.Add("TwoMenuIndex", txtTwoMenuIndex.Text.Trim());
                paramter.Add("TwoMenuName", txtTwoMenuName.Text.Trim());
                 // 页面编号 数据库自动生成
                //paramter.Add("PageIndex", txtPageIndex.Text.Trim());
                paramter.Add("PageName", txtPageName.Text.Trim());
                paramter.Add("PageURL", txtPageURL.Text.Trim());
                paramter.Add("Remark", txtRemark.Text.Trim());
                paramter.Add("RoleType", ddlType.SelectedValue);
                */

                Bd_Base_Page model = new Bd_Base_Page();
                int _ModuleIndex = 0;
                int _OneMenuIndex = 0;
                int _TwoMenuIndex = 0;
                int _PageIndex = 0;
                int _RoleType = 1;

                if (int.TryParse(txtTwoMenuIndex.Text.Trim(), out _TwoMenuIndex))
                {
                    model.TwoMenuIndex = _TwoMenuIndex;
                }
                else
                {
                    msg = "二级菜单编号输入错误！";
                }

                if (int.TryParse(txtOneMenuIndex.Text.Trim(), out _OneMenuIndex))
                {
                    model.OneMenuIndex = _OneMenuIndex;
                }
                else
                {
                    msg = "一级菜单编号输入错误！";
                }

                if (int.TryParse(txtModuleIndex.Text.Trim(), out _ModuleIndex))
                {
                    model.ModuleIndex = _ModuleIndex;
                }
                else
                {
                    msg = "模块编号输入错误！";
                }

                if (int.TryParse(ddlType.SelectedValue, out _RoleType))
                {
                    model.RoleType = _RoleType;
                }
                else
                {
                    msg = "请选择用户类型！";
                }


                if (msg == "")
                {
                    #region 添加

                    //获取最大id
                    _PageIndex = new PbProject.Logic.ControlBase.Bd_Base_PageBLL().GetPageIndexMaxByRoleType(_RoleType);
                    _PageIndex++;

                    model.PageIndex = _PageIndex;

                    //if (int.TryParse(txtPageIndex.Text.Trim(), out _PageIndex))
                    //{
                    //    model.PageIndex = _PageIndex;
                    //}
                    //else
                    //{
                    //    msg = "页面编号输入错误！";
                    //    return;
                    //}
                    model.id = Guid.NewGuid();
                    model.ModuleName = txtModuleName.Text.Trim();
                    model.OneMenuName = txtOneMenuName.Text.Trim();
                    model.TwoMenuName = txtTwoMenuName.Text.Trim();
                    model.PageName = txtPageName.Text.Trim();
                    model.PageURL = txtPageURL.Text.Trim();
                    model.Remark = txtRemark.Text.Trim();

                    result = (bool)baseDataManage.CallMethod("Bd_Base_Page", "Insert", null, new Object[] { model });

                    if (result)
                    {
                        msg = "添加成功！";
                        //清理缓存
                        //baseDataManage.CallMethod("Bd_Base_Page", "RefreshCache", null, new object[] { });
                    }
                    else
                    {
                        msg = "添加失败！";
                    }

                    #endregion
                }
            }
        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
        finally
        {
            if (result)
            {
                RepBasePageBind();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }

    /// <summary>
    /// 清空
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtModuleIndex.Text = "";
        txtModuleName.Text = "";
        txtOneMenuIndex.Text = "";
        txtOneMenuName.Text = "";
        txtTwoMenuIndex.Text = "";
        txtTwoMenuName.Text = "";
        txtPageIndex.Text = "";
        txtPageName.Text = "";
        txtPageURL.Text = "";
        txtRemark.Text = "";
        //ddlType.SelectedValue
    }

    /// <summary>
    /// 跟新用户页面权限
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateUserPage_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            int _RoleType = 1;
            if (int.TryParse(ddlType.SelectedValue, out _RoleType))
            {
                if (new PbProject.Logic.User.User_PermissionsBLL().Update_User_Permissions(_RoleType))
                {
                    msg = "更新成功！";
                }
                else
                {
                    msg = "更新失败！";
                }
            }
            else
            {
                msg = "操作失败！";
            }

        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
        finally
        {
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }
}