using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Model;
using DataBase.Data;
using PbProject.WebCommon.Utility;

public partial class DiscountSet_DiscountList : BasePage
{
    public string msg = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                this.currentuserid.Value = this.mUser.id.ToString();
                btnAdd.PostBackUrl = string.Format("DiscountSet.aspx?currentuserid={0}", this.currentuserid.Value.ToString());
                Button2.PostBackUrl = string.Format("DiscountUserSet.aspx?currentuserid={0}", this.currentuserid.Value.ToString());
                
                Query();
                ViewState["orderBy"] = " OperTime desc";
                BindDiscountList();
            }
        }
        catch (Exception ex)
        {
            
        }
    }
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
  
    /// <summary>
    /// 绑定扣点组信息
    /// </summary>
    public void BindDiscountList()
    {
        try
        {
            List<Tb_Ticket_StrategyGroup> list = baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetList", null, new Object[] { Con + " order by OperTime desc" }) as List<Tb_Ticket_StrategyGroup>;
            repList.DataSource = list;
            repList.DataBind();
        }
        catch (Exception ex)
        {

        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append(mCompany.RoleType == 1 ? "1=1" : "CpyNo='" + mCompany.UninCode + "'");
        if (txtGroupName.Text != "")
        {
            strWhere.Append(" and GroupName like '%" + CommonManage.TrimSQL(txtGroupName.Text.Trim()) + "%'");
        }
        Con = strWhere.ToString();
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
                Tb_Ticket_StrategyGroup Mgroup = baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetById", null, new object[] { id }) as Tb_Ticket_StrategyGroup;
                string sql1 = "delete from Tb_Ticket_StrategyGroup where id='"+id+"'";
                string sql2 = "delete from Tb_Ticket_TakeOffDetail where GroupId ='" + id + "'";
                List<string> sqllist = new List<string>();
                sqllist.Add(sql1);
                sqllist.Add(sql2);
                string errormsg = "";
                msg = baseDataManage.ExecuteSqlTran(sqllist, out errormsg) == true ? "删除成功" : "删除失败";
                if (msg == "删除成功")
                {
                    //日志
                    Log_Operation logoper = new Log_Operation();
                    logoper.ModuleName = "扣点组管理";
                    logoper.LoginName = mUser.LoginName;
                    logoper.UserName = mUser.UserName;
                    logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
                    logoper.CpyNo = mCompany.UninCode;
                    logoper.OperateType = "删除扣点组";
                    logoper.OptContent = "删除扣点组id:" + id + ",组名:" + Mgroup.GroupName;
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加日志
                    BindDiscountList();
                }
               
            }
            else
            {
                Response.Redirect("DiscountSet.aspx?gid=" + id + "&currentuserid=" + this.currentuserid.Value.ToString());
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
                List<Tb_Ticket_StrategyGroup> list = this.baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetList", null, new object[] { "id in (" + ids + ")" }) as List<Tb_Ticket_StrategyGroup>;
                string sql1 = "delete from Tb_Ticket_StrategyGroup where id in (" + ids + ")";
                string sql2 = "delete from Tb_Ticket_TakeOffDetail where GroupId in (" + ids + ")";
                List<string> sqllist = new List<string>();
                sqllist.Add(sql1);
                sqllist.Add(sql2);
                string errormsg = "";
                msg = baseDataManage.ExecuteSqlTran(sqllist, out errormsg) == true ? "删除成功" : "删除失败";
                BindDiscountList();
                if (msg == "删除成功")
                {
                    string contents = "";
                    for (int i = 0; i < list.Count; i++)
                    {
                        contents += "id:" + list[i].id + ",组名:" + list[i].GroupName + "###";
                    }
                    Log_Operation logoper = new Log_Operation();
                    logoper.ModuleName = "扣点组管理";
                    logoper.LoginName = mUser.LoginName;
                    logoper.UserName = mUser.UserName;
                    logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
                    logoper.CpyNo = mCompany.UninCode;
                    logoper.OperateType = "删除多个扣点组";
                    logoper.OptContent = "删除多个扣点组|" + contents;
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加日志
                }
            }
            else
            {
                msg = "请选择要删除的选项";
            }
           
        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}