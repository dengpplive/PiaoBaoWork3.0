using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using PbProject.Dal.Mapping;

public partial class Pay_BatchPay_QueryBatch :BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_query_Click(object sender, EventArgs e)
    {
        this.Pager.CurrentPageIndex = 1;
        StringBuilder strWhere = new StringBuilder();
        string payNum = txt_payNum.Text;
        string payeeNum = txt_payeeNum.Text;
        string payState = drop_list.SelectedValue;
        string payStartTime = txt_StartTime.Text;
        string payEndTime = txt_EndTime.Text;
        strWhere.AppendFormat(" PayCompanyID='{0}'", this.mUser.CpyNo);
        if (!string.IsNullOrEmpty(payNum))
            strWhere.AppendFormat(" and PayAccount='{0}'", payNum);
        if (!string.IsNullOrEmpty(payeeNum))
            strWhere.AppendFormat(" and GetAccount='{0}'", payeeNum);
        if (!string.IsNullOrEmpty(payState))
            strWhere.AppendFormat(" and Result={0}", payState);
        if (!string.IsNullOrEmpty(payStartTime))
            strWhere.AppendFormat(" and DATEDIFF(N,PayTime,'{0}')", payStartTime);
        if (!string.IsNullOrEmpty(payEndTime))
            strWhere.AppendFormat(" and DATEDIFF(N,PayTime,'{0}')", payEndTime);
        ViewState["where"] = strWhere.ToString();
        string sqlcount = string.Format("select count(*) from TransferAccounts where {0}", strWhere);
        Pager.RecordCount = new SqlHelper().QueryCount(sqlcount);
        BindRepeater();
    }
    /// <summary>
    /// 绑定列表
    /// </summary>
    private void BindRepeater()
    {
        string where = " where ";
        object obj = ViewState["where"];
        if (obj != null)
            where += obj.ToString();
        else
            where += " CompanyID='" + this.mUser.CpyNo + "' ";

        string sql = string.Format("select RowNum,PayAccount,PayName,UserName,GetAccount,GetName,PayMoney,OutOrderID,Result=case Result when 0 then '等待付款'  when 1 then '付款成功' when 2 then '付款失败' end,PayTime,Remark from (select ROW_NUMBER() over(order by CreateTime Desc) as RowNum,* from TransferAccounts {0}) as TempTable where RowNum>{1} and RowNum <= {2}", where, (Pager.CurrentPageIndex - 1) * Pager.PageSize, Pager.CurrentPageIndex * Pager.PageSize);
        
        DataTable dt = new SqlHelper().GetDataTableBySql(sql);
        rp_list.DataSource = dt; 
        rp_list.DataBind();
        
    }
    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Pager_PageChanged(object sender, EventArgs e)
    {
        BindRepeater();
    }
}