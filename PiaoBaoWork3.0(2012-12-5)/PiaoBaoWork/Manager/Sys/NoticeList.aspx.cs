using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DataBase.Data;
using PbProject.Model;
using PbProject.WebCommon.Utility;
public partial class Sys_NoticeList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            btnAdd.PostBackUrl = string.Format("NoticeEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
        if (mCompany.RoleType > 3)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您无权访问该页面！',{op:2});", true);
            return;
        }
        if (!IsPostBack)
        {
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " ReleaseTime desc ";
            PageDataBind();
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
    /// 绑定公告列表
    /// </summary>
    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Bd_Base_Notice> list = baseDataManage.CallMethod("Bd_Base_Notice", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Bd_Base_Notice>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        Repeater.DataSource = list;
        Repeater.DataBind();
    }
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
        Query();
        PageDataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        try
        {
            Curr = 1;
            StringBuilder WhereStr = new StringBuilder(" 1=1");
            if (mCompany.UninCode.Length == 6)
            {
                WhereStr.Append(" and (ReleaseCpyNo='" + mCompany.UninCode + "' or (len(ReleaseCpyNo)=12 and ReleaseCpyNo like '" + mCompany.UninCode + "%'))");
            }
            else
            {
                WhereStr.Append(" and ReleaseCpyNo='" + mCompany.UninCode + "'");
            }
            if (NoticeTitle.Text != "")
            {
                WhereStr.Append(" and Title like'%" + CommonManage.TrimSQL(NoticeTitle.Text.Trim()) + "%'");
            }
            if (ReleaseName.Text != "")
            {
                WhereStr.Append(" and ReleaseName like'%" + CommonManage.TrimSQL(ReleaseName.Text.Trim()) + "%'");
            }
            if (CallboardType.SelectedValue != "==请选择状态==")
            {
                WhereStr.Append(" and CallboardType =" + CallboardType.SelectedValue + "");
            }
            if (ReleaseDateSta.Text != "")
            {
                WhereStr.Append(" and ReleaseTime >='" + CommonManage.TrimSQL(ReleaseDateSta.Text.Trim()) + "'");
            }
            if (ReleaseDateStp.Text != "")
            {
                WhereStr.Append(" and ReleaseTime <='" + CommonManage.TrimSQL(ReleaseDateStp.Text.Trim()) + "'");
            }
            Con = WhereStr.ToString();
        }
        catch (Exception)
        {

            throw;
        }
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Reset_Click(object sender, EventArgs e)
    {
        NoticeTitle.Text = "";
        ReleaseName.Text = "";
        CallboardType.SelectedIndex = 0;
        ReleaseDateSta.Text = "";
        ReleaseDateStp.Text = "";
    }
    /// <summary>
    /// 删除审核操作
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        try
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Del")
            {
                msg = (bool)baseDataManage.CallMethod("Bd_Base_Notice", "DeleteById", null, new Object[] { id }) == true ? "删除成功" : "删除失败";
            }
            else
            {
                IHashObject paramter = new HashObject();
                string s = (e.Item.FindControl("UpDateButton") as LinkButton).Text;
                if (s.Equals("审 核"))
                {
                    paramter.Add("CallboardType", 1);
                }
                else if (s.Equals("撤 审"))
                {
                    paramter.Add("CallboardType", 2);
                }
                paramter.Add("id", id);
                msg = (bool)baseDataManage.CallMethod("Bd_Base_Notice", "Update", null, new Object[] { paramter }) == true ? "更新成功" : "更新失败";
            }
            PageDataBind();
        }
        catch (Exception)
        {

            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 批量操作
    /// </summary>
    /// <param name="type"></param>
    /// <param name="IdsList"></param>
    /// <returns></returns>
    public bool PatchUpdate(int type)
    {
        bool PatchUpdate = false;
        string ids = hid_ids.Value.Trim();
        if (ids == "")
        {
            return PatchUpdate;
        }
        string[] strArr = ids.Split(new string[] { "#####" }, StringSplitOptions.RemoveEmptyEntries);
        List<string> listIds = new List<string>();
        listIds.AddRange(strArr);
        if (listIds.Count > 0)
        {
            string UpdateFileds = "";
            if (type == 1)//审核
            {
                UpdateFileds = " CallboardType=1 ";
            }
            else if (type == 0)//撤审
            {
                UpdateFileds = " CallboardType=2 ";
            }
            if (UpdateFileds != "")
            {
                PatchUpdate = (bool)baseDataManage.CallMethod("Bd_Base_Notice", "UpdateByIds", null, new Object[] { listIds, UpdateFileds });
            }
        }
        hid_ids.Value = "";
        return PatchUpdate;
    }
    /// <summary>
    /// 批量审核
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdate1_Click(object sender, EventArgs e)
    {
        bool flag = PatchUpdate(1);
        string Msg = "";
        if (flag)
        {
            Msg = "批量审核成功";
            btnUpdate1.Enabled = true;
            PageDataBind();
        }
        else
        {
            Msg = "批量审核失败";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + Msg + "',1);", true);
    
        
    }
    /// <summary>
    /// 批量撤审
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdate0_Click(object sender, EventArgs e)
    {

        bool flag = PatchUpdate(0);
        string Msg = "";
        if (flag)
        {
            Msg = "批量撤审成功";
            btnUpdate0.Enabled = true;
            PageDataBind();
        }
        else
        {
            Msg = "批量撤审失败";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + Msg + "',1);", true);
    }
  
}