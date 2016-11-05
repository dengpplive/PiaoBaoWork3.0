using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Dal.ControlBase;
using DataBase.Data;
using System.Text;
using PbProject.Logic.ControlBase;
using System.Data;
using PbProject.WebCommon.Utility;

public partial class User_ComPanyList : BasePage
{   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnAdd.PostBackUrl = string.Format("OpenAccount.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Curr = 1;
            Con = Query();
            AspNetPager1.PageSize = 20;
           // PageDataBind();
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
    /// 绑定数据
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            int num = 0;
            DataTable dt = baseDataManage.GetBasePagerDataTable("V_AccountInfo",out num, AspNetPager1.PageSize, Curr, "*", Con, "CreateTime desc");
            AspNetPager1.RecordCount = num;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            Repeater.DataSource = dt;
            Repeater.DataBind();
        }
        catch (Exception)
        {
            
            throw;
        }
        
    }
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
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
            if (e.CommandName == "Update")
            {
                string s = (e.Item.FindControl("UpDateButton") as LinkButton).Text;
                string id = e.CommandArgument.ToString();
                if (s.Equals("冻 结"))
                {
                    paramter.Add("AccountState", 0);
                }
                else if (s.Equals("解 冻"))
                {
                    paramter.Add("AccountState", 1);
                }
                paramter.Add("id", e.CommandArgument.ToString());
                msg = new PbProject.Logic.User.User_CompanyBLL().UpdateById(paramter) == true ? s+" 成 功" : s+" 失 败";
            }
            else if (e.CommandName == "UpDatePwd")
            {
                List<User_Employees> list = new PbProject.Logic.User.User_EmployeesBLL().GetBySQLList("CpyNo='" + e.CommandArgument.ToString() + "' and IsAdmin=0");
                paramter.Add("id", list[0].id.ToString());
                paramter.Add("LoginPassWord", PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5("888888"));
                msg =  new PbProject.Logic.User.User_EmployeesBLL().UpdateById(paramter) == true ? "密码恢复成功,初始密码888888" : "恢复失败";
            }
            PageDataBind();
        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    private string Query()
    {
        StringBuilder strWhere = new StringBuilder();
        
        try
        {
            if (mCompany.RoleType == 1)
            {
                if (!string.IsNullOrEmpty(Request["isfx"]))
                {
                    //分销（可设置独立分销）
                    strWhere.Append(" RoleType =4");
                }
                else
                {
                    //平台直属下级
                    strWhere.Append(" RoleType in (2,3) and len(UninCode)=12 and SUBSTRING(UninCode,1,6)='" + mCompany.UninCode + "' ");
                }
                
            }
            else if (mCompany.RoleType == 2)
            {
                //运营商直属下级
                strWhere.Append(" RoleType in (4,5) and len(UninCode)=18 and SUBSTRING(UninCode,1,12)='" + mCompany.UninCode + "' ");
            }
            else if (mCompany.RoleType == 4)
            {
                //分销直属下级
                if (mCompany.UninCode.Length == 18)
                {
                    strWhere.Append(" RoleType in (4,5) and len(UninCode)=24 and SUBSTRING(UninCode,1,18)='" + mCompany.UninCode + "' ");
                }
                else if (mCompany.UninCode.Length == 24)
                {
                    //只有采购
                    strWhere.Append(" RoleType = 5 and len(UninCode)=30 and SUBSTRING(UninCode,1,24)='" + mCompany.UninCode + "' ");
                }
            }
            else
            {
                strWhere.Append(" 1=1");
            }
            //账号
            strWhere.Append(txtUnitCode.Text != "" ? " and LoginName like '%" + CommonManage.TrimSQL(txtUnitCode.Text.Trim()) + "%'" : "");
            //单位全称
            strWhere.Append(txtUnitName.Text != "" ? " and UninAllName like '%" + CommonManage.TrimSQL(txtUnitName.Text.Trim()) + "%'" : "");
            //省
            if (Request.Form["province"] != null && Request.Form["province"] != "--省份--")
            {
                strWhere.Append(" and Provice = '" + Request.Form["province"] + "'");
            }
            //市
            if (Request.Form["city"] != null && Request.Form["city"] != "--城市--" && Request.Form["city"] != "")
            {
                strWhere.Append(" and City = '" + Request.Form["city"] + "'");
            }
            strWhere.Append(StartTimeSta.Value != "" ? " and CreateTime >= convert(DateTime, '" + StartTimeSta.Value + " 00:00:00')" : "");
            //时间结束
            strWhere.Append(StartTimeStp.Value != "" ? " and CreateTime <= convert(DateTime, '" + StartTimeStp.Value + " 23:59:59')" : "");

            //账号状态
            strWhere.Append(int.Parse(rblISDJ.SelectedItem.Value) != -1 ? " and AccountState = '" + rblISDJ.SelectedItem.Value + "'" : "");

            //联系人
            if (this.txtContactUser.Text != "")
            {
                strWhere.Append(" and ContactUser like '%" + CommonManage.TrimSQL(txtContactUser.Text.Trim()) + "%'");
            }
            //联系电话
            if (this.txtUserPhone.Text != "")
            {
                strWhere.Append(" and ContactTel = '" + CommonManage.TrimSQL(this.txtUserPhone.Text.Trim()) + "'");
            }
            strWhere.Append(" and  IsAdmin=0 ");
            return strWhere.ToString();
        }
        catch (Exception)
        {

            return strWhere.ToString();
        }

    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtUnitCode.Text = "";
        txtUnitName.Text = "";
        StartTimeSta.Value = "";
        StartTimeStp.Value = "";
        rblISDJ.SelectedValue = "-1";
        this.province.SelectedValue = "--省份--";
        this.city.SelectedIndex = 0;
        txtUserPhone.Text = "";
        txtContactUser.Text = "";
    }
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Con = Query();
        PageDataBind();
    }
    /// <summary>
    /// 平台管理分销和公司管理区别
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["isfx"] != null ? Request["isfx"].ToString() : ""))
        {
            e.Item.FindControl("Div2").Visible = true;
        }
        else
        {
            e.Item.FindControl("Div1").Visible = true;
        }
        
    }
}