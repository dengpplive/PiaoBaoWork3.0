using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Dal.ControlBase;
using DataBase.Data;
using PbProject.Logic.ControlBase;
using PbProject.Logic.User;
using System.Data;
using PbProject.WebCommon.Utility;

/// <summary>
/// 员工管理
/// </summary>
public partial class Company_UserList : BasePage
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
                BindUserPermissions();
                Curr = 1;
                SelWhere();
                AspNetPager1.PageSize = 20;
                //PageDataBind(); 默认不加载
            }
        }
        catch (Exception)
        {

        }
    }

    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    /// <summary>
    /// 绑定部门权限
    /// </summary>
    protected void BindUserPermissions()
    {
        List<User_Permissions> bPaseList = new User_PermissionsBLL().GetListByCpyNo(mUser.CpyNo);
        //ddlBM.Items.Add(new ListItem("-全部-", "0"));
        //for (int i = 0; i < bPaseList.Count; i++)
        //{
        //    ddlBM.Items.Add(new ListItem(bPaseList[i].DeptName, bPaseList[i].id.ToString()));
        //}
        ddlBM.Items.Insert(0, new ListItem("--选择--", "0"));
        ddlBM.DataSource = bPaseList;
        ddlBM.DataTextField = "DeptName";
        ddlBM.DataValueField = "id";
        ddlBM.DataBind();
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    private void PageDataBind()
    {
        //int TotalCount = 0;
        //IHashObject outParams = new HashObject();
        ////指定参数类型 第一个参数为out输出类型
        ////key 为参数索引从1开始 value为引用类型 out ref
        //outParams.Add("1", "out");
        //List<User_Employees> list = baseDataManage.CallMethod("User_Employees", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<User_Employees>;
        //TotalCount = outParams.GetValue<int>("1");
        //AspNetPager1.RecordCount = TotalCount;
        //AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        //Repeater.DataSource = list;
        //Repeater.DataBind();
        int num = 0;
        DataTable dt = baseDataManage.GetBasePagerDataTable("V_User_EmployeesList", out num, AspNetPager1.PageSize, Curr, "*", Con, "CreateTime desc");
        AspNetPager1.RecordCount = num;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        Repeater.DataSource = dt;
        Repeater.DataBind();
    }
    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SelButton_Click(object sender, EventArgs e)
    {
        SelWhere();
        PageDataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    public void SelWhere()
    {
        try
        {
            Curr = 1;
            StringBuilder strWhere = new StringBuilder();
            strWhere.Append(" IsAdmin <> 0 and id <> '" + mUser.id + "' and CpyNo='" + mCompany.UninCode + "'");
            if (WorkNum.Text != "")
            {
                strWhere.Append(" and WorkNum like '%" + CommonManage.TrimSQL(WorkNum.Text) + "%'");
            }
            if (UserName.Text != "")
            {
                strWhere.Append(" and UserName like '%" + CommonManage.TrimSQL(UserName.Text) + "%'");
            }
            if (Tel.Text != "")
            {
                strWhere.Append(" and Tel like '%" + CommonManage.TrimSQL(Tel.Text) + "%'");
            }
            if (CertificateNum.Text != "")
            {
                strWhere.Append(" and CertificateNum like '%" + CommonManage.TrimSQL(CertificateNum.Text) + "%'");
            }
            if (Sex.SelectedValue != "ALL")
            {
                strWhere.Append(" and Sex =" + Sex.SelectedValue);
            }
            if (Phone.Text != "")
            {
                strWhere.Append(" and Phone like '%" + CommonManage.TrimSQL(Phone.Text) + "%'");
            }
            //部门
            if (ddlBM.SelectedValue != "0")
            {
                strWhere.Append(" and DeptId = '" + ddlBM.SelectedValue +"'");
            }
            //状态
            if (ddlStatus.SelectedValue != "-1")
            {
                strWhere.Append(" and State =  " + ddlStatus.SelectedValue);
            }

            Con = strWhere.ToString();
        }
        catch (Exception)
        {
            
            throw;
        }
        
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddUser.aspx?currentuserid=" + this.currentuserid.Value.ToString());
    }


    /// <summary>
    /// 绑定
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        try
        {
            IHashObject paramter = new HashObject();
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Update")
            {
                string s = (e.Item.FindControl("UpDateButton") as LinkButton).Text;

                if (s.Equals("冻 结"))
                {
                    paramter.Add("State", 0);
                }
                else if (s.Equals("解 冻"))
                {
                    paramter.Add("State", 1);
                }
                paramter.Add("id", e.CommandArgument.ToString());
                msg = new PbProject.Logic.User.User_EmployeesBLL().UpdateById(paramter) == true ? s + " 成 功" : s + " 失 败";
            }
            else
            {
                paramter.Add("id", id);
                paramter.Add("LoginPassWord", PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5("888888"));
                msg = new PbProject.Logic.User.User_EmployeesBLL().UpdateById(paramter) == true ? "密码恢复成功,初始密码888888" : "恢复失败";
            }
            PageDataBind();
        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
       
    }

    /// <summary>
    /// OnError
    /// </summary>
    /// <param name="e"></param>
    protected override void OnError(EventArgs e)
    {
        //base.OnError(e);
        e.ToString();
    }
    /// <summary>
    /// Reset_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Reset_Click(object sender, EventArgs e)
    {
        WorkNum.Text = "";
        UserName.Text = "";
        Tel.Text = "";
        Phone.Text = "";
        Sex.SelectedIndex = 0;
        CertificateNum.Text = "";
        ddlBM.SelectedIndex = 0;
        ddlStatus.SelectedIndex = 0;
    }
    /// <summary>
    /// GetChildNames
    /// </summary>
    /// <param name="ChildId"></param>
    /// <returns></returns>
    public string GetChildNames(string ChildId)
    {
        return "";

    }
 
    /// <summary>
    /// 格式化时间
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns></returns>
    public string getTime(string time)
    {
        string setTime = "";
        try
        {
            if (time != "")
            {
                setTime = DateTime.Parse(time).ToString("yyyy-MM-dd");
            }
        }
        catch
        {
        }
        return setTime;
    }

    /// <summary>
    /// 格式化
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public string getStr(string str, int length)
    {
        if (str != null && str != "")
        {
            if (str.Length > length)
            {
                str = str.Substring(0, length) + "...";
            }
        }
        return str;
    }
    public string encodeName(string str)
    {
        return Server.UrlEncode(str);
    }
}