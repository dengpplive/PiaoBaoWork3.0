using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using PbProject.WebCommon.Utility;

public partial class Financial_PosList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnadd.PostBackUrl = string.Format("PosEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " OperTime desc ";
            //BindPosInfo();
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
    /// 绑定pos机信息
    /// </summary>
    protected void BindPosInfo()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_PosInfo> list = baseDataManage.CallMethod("Tb_PosInfo", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_PosInfo>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repPosList.DataSource = list;
        repPosList.DataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append(mCompany.RoleType == 1 ? " 1=1 " : " (CpyNo='" + mCompany.UninCode + "' or OperCpyNo='" + mCompany.UninCode + "')");
        if (txtCopName.Text != "")
        {
            strWhere.Append(" and CpyName like '%" + CommonManage.TrimSQL(txtCopName.Text.Trim()) + "%'");
        }
        if (txtTerminalNo.Text != "")
        {
            strWhere.Append(" and PosNo like '%" + CommonManage.TrimSQL(txtTerminalNo.Text.Trim()) + "%'");
        }
        Con = strWhere.ToString();
        Curr = 1;
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindPosInfo();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindPosInfo();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnreset_Click(object sender, EventArgs e)
    {
        txtCopName.Text = "";
        txtTerminalNo.Text = "";
    }
    /// <summary>
    /// 获取支付类型
    /// </summary>
    /// <param name="ChildID"></param>
    /// <returns></returns>
    protected string ReturnType(string ChildID)
    {
        
        return GetDictionaryName("4", ChildID);
    }
    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repPosList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();
        string msg = "";
        if ((bool)baseDataManage.CallMethod("Tb_PosInfo", "DeleteById", null, new Object[] { id }))
        {
            msg="删除成功";

            #region 操作日志
            Log_Operation logoper = new Log_Operation();
            logoper.ModuleName = "pos机管理";
            logoper.LoginName = mUser.LoginName;
            logoper.UserName = mUser.UserName;
            logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
            logoper.CpyNo = mCompany.UninCode;
            logoper.OperateType = "删除Pos机";
            logoper.OptContent = "id=" +id;
            new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);
            #endregion
        }
        else
	    {
            msg="删除失败";
	    }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
        BindPosInfo();
    }
}