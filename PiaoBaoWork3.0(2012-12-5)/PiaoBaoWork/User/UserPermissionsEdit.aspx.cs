using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using PbProject.Logic.User;
using System.Collections;
using DataBase.Data;

/// <summary>
/// 页面权限 修改、添加
/// </summary>
public partial class User_UserPermissionsEdit : BasePage
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
                TreeDataBind();
                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"].ToString())
                   && Request.QueryString["name"] != null && !string.IsNullOrEmpty(Request.QueryString["name"].ToString())
                    )
                {

                    string id = Request.QueryString["id"].ToString();

                    hidId.Value = id;

                    txtDeptName.Text = Request.QueryString["name"].ToString();

                    GetPostDataBind(id);
                    txtDeptName.Enabled = false;
                }

                if (Request.QueryString["Url"] != null)
                {
                    Hid_GoUrl.Value = Request.QueryString["Url"].ToString() + "?currentuserid=" + this.currentuserid.Value.ToString();//返回  
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 修改时，绑定原来已有的页面权限
    /// </summary>
    /// <param name="id">当前ID</param>
    private void GetPostDataBind(string id)
    {
        try
        {
            #region 权限集合

            List<User_Permissions> UPermissions = new User_PermissionsBLL().GetListById(Guid.Parse(id));//mUser.DeptId
            User_Permissions mPost = UPermissions[0];

            //权限集合
            if (mPost != null)
            {
                this.txtRemark.Text = mPost.Remark;
                string[] value = mPost.Permissions.Split(',');

                //遍历权限
                foreach (TreeNode childnodes in trvPagePower.Nodes)
                {
                    foreach (TreeNode childnodes2 in childnodes.ChildNodes)
                    {
                        foreach (TreeNode childnodes3 in childnodes2.ChildNodes)
                        {
                            for (int i = 0; i < value.Length; i++)
                            {
                                if (value[i] == childnodes3.Value)
                                {

                                    childnodes.Checked = true;
                                    childnodes2.Checked = true;
                                    childnodes.Expanded = true;
                                    childnodes2.Expanded = true;
                                    childnodes3.Expanded = true;
                                    childnodes3.Checked = true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion 
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 绑定页面
    /// </summary>
    private void TreeDataBind()
    {
        try
        {
            int roleType = mCompany.RoleType;
            List<Bd_Base_Page> iPost = new Bd_Base_PageBLL().GetListByRoleType(roleType);

            #region 排序

            iPost = (from e in iPost
                     orderby e.ModuleIndex
                     select new Bd_Base_Page
                     {
                         id = e.id,
                         ModuleIndex = e.ModuleIndex,
                         ModuleName = e.ModuleName,
                         OneMenuIndex = e.OneMenuIndex,
                         OneMenuName = e.OneMenuName,
                         TwoMenuIndex = e.TwoMenuIndex,
                         TwoMenuName = e.TwoMenuName,
                         PageIndex = e.PageIndex,
                         PageName = e.PageName,
                         PageURL = e.PageURL,
                         RoleType = e.RoleType
                     }).ToList();

            #endregion 

            #region 菜单页面

            for (int i = 0; i < iPost.Count; i++)
            {
                int v = 0;
                int x = 0;
                foreach (TreeNode childnodes in trvPagePower.Nodes)
                {
                    if (childnodes.Value == iPost[i].ModuleIndex.ToString())
                    {
                        v++;
                        foreach (TreeNode childnodes2 in childnodes.ChildNodes)
                        {
                            if (iPost[i].OneMenuIndex.ToString() == childnodes2.Value)
                            {
                                x++;
                                if (iPost[i].TwoMenuIndex != 0)
                                {
                                    //添加第三级菜单
                                    TreeNode childnodey = new TreeNode(iPost[i].TwoMenuName.ToString(), iPost[i].PageIndex.ToString());

                                    childnodey.Expanded = false;
                                    childnodes2.ChildNodes.Add(childnodey);

                                }
                            }
                        }
                        if (x == 0)
                        {
                            if (iPost[i].TwoMenuIndex != 0)
                            {
                                //添加第二级菜单
                                TreeNode childnodex = new TreeNode(iPost[i].OneMenuName, iPost[i].OneMenuIndex.ToString());
                                childnodex.Expanded = false;
                                childnodes.ChildNodes.Add(childnodex);


                                //添加第三级菜单
                                TreeNode childnodey = new TreeNode(iPost[i].TwoMenuName.ToString(), iPost[i].PageIndex.ToString());
                                childnodey.Expanded = false;
                                childnodex.ChildNodes.Add(childnodey);
                            }
                        }
                        x = 0;
                    }
                }
                if (v == 0)
                {
                    if (iPost[i].ModuleName != "测试模块")
                    {
                        iPost[i].ModuleName = iPost[i].ModuleName == "报表管理" ? "机票" : iPost[i].ModuleName;
                        //添加第一级菜单
                        TreeNode regionalnode = new TreeNode(iPost[i].ModuleName.Replace("预订", "").Replace("管理", "").Replace("服务", ""), iPost[i].ModuleIndex.ToString());
                        regionalnode.Expanded = false;
                        //添加第二级菜单
                        TreeNode childnodex = new TreeNode(iPost[i].OneMenuName, iPost[i].OneMenuIndex.ToString());
                        childnodex.Expanded = false;
                        regionalnode.ChildNodes.Add(childnodex);
                        if (iPost[i].TwoMenuIndex != 0)
                        {
                            //添加第三级菜单
                            TreeNode childnodey = new TreeNode(iPost[i].TwoMenuName.ToString(), iPost[i].PageIndex.ToString());
                            childnodey.Expanded = false;
                            childnodex.ChildNodes.Add(childnodey);
                        }

                        trvPagePower.Nodes.Add(regionalnode);
                    }
                }
                v = 0;
            }

            #endregion

            #region 非菜单页面，隐藏页面

            iPost = (from e in iPost
                     where e.TwoMenuIndex == 0
                     orderby e.ModuleIndex
                     select new Bd_Base_Page
                     {
                         id = e.id,
                         ModuleIndex = e.ModuleIndex,
                         ModuleName = e.ModuleName,
                         OneMenuIndex = e.OneMenuIndex,
                         OneMenuName = e.OneMenuName,
                         TwoMenuIndex = e.TwoMenuIndex,
                         TwoMenuName = e.TwoMenuName,
                         PageIndex = e.PageIndex,
                         PageName = e.PageName,
                         PageURL = e.PageURL,
                         RoleType = e.RoleType
                     }).ToList();
            ViewState["NullPage"] = iPost;

            #endregion

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
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string msg = "";
        bool result = false;

        try
        {
            string strDeptName = txtDeptName.Text.Trim();

            if (string.IsNullOrEmpty(strDeptName))
            {
                msg = "该名称不能为空！";
            }

            List<User_Permissions> iPost = new User_PermissionsBLL().GetListByCpyNo(mUser.CpyNo);

            foreach (var item in iPost)
            {
                if (item.DeptName == strDeptName && hidId.Value == "")
                {
                    msg = "添加失败,该名称已经存在！";
                    break;
                }
                //else if (item.DeptName == strDeptName && hidId.Value != item.id.ToString())
                //{
                //    msg = "修改失败,该名称已经存在！";
                //    break;
                //}
            }

            if (msg == "")
            {
                if (hidId.Value == "")
                {
                    #region 添加

                    User_Permissions model = new User_Permissions();
                    model.id = Guid.NewGuid();
                    model.CpyNo = mUser.CpyNo;
                    model.DeptName = strDeptName;
                    model.ParentIndex = 1;
                    model.DeptIndex = 1;
                    model.Remark = txtRemark.Text.Trim();
                    model.Permissions = GetPermissions();
                    model.A1 = 1;
                    model.A2 = 0.00M;
                    model.A3 = DateTime.Now;
                    model.A4 = "";
                    model.A5 = "";

                    result = (bool)baseDataManage.CallMethod("User_Permissions", "Insert", null, new Object[] { model });

                    if (result)
                    {
                        msg = "添加成功！";
                    }
                    else
                    {
                        msg = "添加失败！";
                    }

                    #endregion
                }
                else
                {
                    #region 修改
                    IHashObject parameter = new HashObject();
                    //User_Permissions model = new User_Permissions();
                    parameter.Add("id", Guid.Parse(hidId.Value));
                    parameter.Add("DeptName", strDeptName);
                    parameter.Add("Permissions", GetPermissions());
                    parameter.Add("Remark", txtRemark.Text.Trim());
                    result = (bool)baseDataManage.CallMethod("User_Permissions", "Update", null, new Object[] { parameter });

                    if (result)
                    {
                        msg = "修改成功！";

                    }
                    else
                    {
                        msg = "修改失败！";
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
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }

    /// <summary>
    /// 获取选择的权限 (遍历权限)
    /// </summary>
    /// <returns></returns>
    public string GetPermissions()
    {
        string value = "";
        string tempvalue = "";

        try
        {
            ArrayList list = new ArrayList();
            IList<Bd_Base_Page> iPost = ViewState["NullPage"] as IList<Bd_Base_Page>; //非菜单页面

            #region 遍历权限

            foreach (TreeNode childnodes in trvPagePower.Nodes)
            {
                foreach (TreeNode childnodes2 in childnodes.ChildNodes)
                {
                    foreach (TreeNode childnodes3 in childnodes2.ChildNodes)
                    {
                        if (childnodes3.Checked)
                        {
                            value += childnodes3.Value + ",";

                            //非菜单页面
                            for (int i = 0; i < iPost.Count; i++)
                            {
                                if (childnodes2.Value == iPost[i].OneMenuIndex.ToString())
                                {
                                    if (!list.Contains(iPost[i].PageIndex.ToString()))
                                    {
                                        list.Add(iPost[i].PageIndex.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            for (int i = 0; i < list.Count; i++)
            {
                tempvalue += list[i].ToString() + ",";
            }
        }
        catch (Exception ex)
        {
        }
        return value + tempvalue;
    }
}