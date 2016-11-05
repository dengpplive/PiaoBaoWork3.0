using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using PbProject.Model;

public partial class DiscountSet_DiscountSetDetail : BasePage
{
    string msg = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            hid_groupid.Value = Request["gid"] == null ? "" : Request["gid"].ToString();//组id
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            BindDiscountList();
           
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
    /// 绑定扣点组详情信息
    /// </summary>
    public void BindDiscountList()
    {
        try
        {
            DataTable dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_StrategyGroup", Con + " order by OperTime desc");
            AspNetPager1.RecordCount = dt.Rows.Count;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            repList.DataSource = dt;
            repList.DataBind();
        }
        catch (Exception)
        {
            throw;
        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        try
        {
            StringBuilder strWhere = new StringBuilder();
            strWhere.Append("CpyNo='" + mCompany.UninCode + "' and GroupId='" + hid_groupid.Value + "'");
            if (txtGroupName.Text != "")
            {
                strWhere.Append(" and GroupName like '%" + txtGroupName.Text + "%'");
            }
            Con = strWhere.ToString();
            Curr = 1;
        }
        catch (Exception)
        {
            
            throw;
        }
        
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindDiscountList();
    }
    /// <summary>
    /// 列表操作
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
       
        try
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Del")
            {
                Tb_Ticket_TakeOffDetail Mtakeoffdetail = baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetById", null, new object[] { id }) as Tb_Ticket_TakeOffDetail;
                msg = (bool)baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "DeleteById", null, new Object[] { id }) == true ? "删除成功" : "删除失败";
                if (msg == "删除成功")
                {
                    //日志
                    Log_Operation logoper = new Log_Operation();
                    logoper.ModuleName = "扣点组管理";
                    logoper.LoginName = mUser.LoginName;
                    logoper.UserName = mUser.UserName;
                    logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
                    logoper.CpyNo = mCompany.UninCode;
                    logoper.OperateType = "删除扣点组明细";
                    logoper.OptContent = "删除扣点组明细id:"+id+",出发城市:" + Mtakeoffdetail.FromCityCode + ",到达城市:" + Mtakeoffdetail.ToCityCode + ",承运人:" + Mtakeoffdetail.CarryCode + ",扣点范围:" + Mtakeoffdetail.PointScope;
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加日志
                }
            }
            
        }
        catch (Exception)
        {
            msg = "操作异常";
            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDel_Click(object sender, EventArgs e)
    {
        try
        {
            int count = this.repList.Items.Count;
            string ids = "";
            for (var i = 0; i < count; i++)
            {
                if (((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.repList.Items[i].FindControl("cbItem")).Checked)
                {
                    string id = ((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.repList.Items[i].FindControl("cbItem")).Value;
                    if (id.Length != 0)
                    {
                        ids += "'" + id + "',";
                    }
                }
            }
            ids = ids.TrimEnd(',');
            if (ids.Length>0)
            {
                List<Tb_Ticket_TakeOffDetail> list = this.baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new object[] { "id in (" + ids + ")" }) as List<Tb_Ticket_TakeOffDetail>;
                string sql = "delete from Tb_Ticket_TakeOffDetail where id in (" + ids + ")";
                msg = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo(sql) == true ? "删除成功" : "删除失败";
                if (msg == "删除成功")
                {
                    string contents = "";
                    for (int i = 0; i < list.Count; i++)
                    {
                        contents += "id:" + list[i].id + ",出发城市:" + list[i].FromCityCode + ",到达城市:" + list[i].ToCityCode + ",承运人:" + list[i].CarryCode + ",扣点范围:" + list[i].PointScope+"###";
                    }
                    //日志
                    Log_Operation logoper = new Log_Operation();
                    logoper.ModuleName = "扣点组管理";
                    logoper.LoginName = mUser.LoginName;
                    logoper.UserName = mUser.UserName;
                    logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
                    logoper.CpyNo = mCompany.UninCode;
                    logoper.OperateType = "删除多条扣点组明细";
                    logoper.OptContent = "删除多条扣点组明细|" + contents;
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加日志
                    BindDiscountList();
                }
            }
            else
            {
                msg = "请选择要删除的选项";
            }
           
        }
        catch (Exception)
        {
            
            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Query();
        Curr = e.NewPageIndex;
        BindDiscountList();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("DiscountSet.aspx?addgroupid=" + hid_groupid.Value + "&currentuserid=" + this.currentuserid.Value.ToString());
    }
}