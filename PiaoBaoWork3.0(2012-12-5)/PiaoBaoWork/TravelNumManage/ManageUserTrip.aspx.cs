using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.Logic;

public partial class TravelNumManage_ManageUserTrip : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.currentuserid.Value = this.mUser.id.ToString();

        //if (this.SessionIsNull)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！','0');", true);
        //    return;
        //}
        if (mCompany.RoleType > 3)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您没有权限访问该页面！','0');", true);
            return;
        }
        try
        {
            if (!IsPostBack)
            {
                Curr = 1;
                Con = " 1=1 ";
                AspNetPager1.PageSize = int.Parse(Hid_PageSize.Value);
                //初始化参数
                InitParam();
                //绑定显示数据              
                BindData();
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面加载异常！','0');", true);
        }
    }

    public void InitParam()
    {
        //登录用户角色
        Hid_RoleType.Value = mCompany.RoleType.ToString();
        if (mCompany.RoleType == 1)
        {
            tr1.Visible = true;
            tr2.Visible = true;
            tr3.Visible = true;
            //绑定供应
            BindGY();
        }
        else
        {
            tr1.Visible = false;
            tr2.Visible = false;
            tr3.Visible = false;
        }
    }



    #region 属性
    /// <summary>
    /// 检查Session是否丢失
    /// </summary>
    //public bool SessionIsNull
    //{
    //    get
    //    {
    //        bool isSuc = false;
    //        if (Session[new SessionContent().USERLOGIN] == null)
    //        {
    //            isSuc = true;
    //        }
    //        return isSuc;
    //    }
    //}
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
    #endregion

    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindData();
    }


    public string Query()
    {
        StringBuilder sbSQL = new StringBuilder(" 1=1 ");
        //哪家供应的行程单
        if (mCompany.RoleType == 1)
        {
            if (ddlGY.SelectedValue != "")
            {
                string[] strArr = ddlGY.SelectedValue.Split(new string[] { "@@" }, StringSplitOptions.None);
                sbSQL.AppendFormat(" and OwnerCpyNo='{0}' ", strArr[0]);
            }
        }
        else
        {
            sbSQL.AppendFormat(" and OwnerCpyNo='{0}' ", mCompany.UninCode);
        }
        //登录帐号
        if (txtAccount.Text.Trim() != "")
        {
            sbSQL.Append(" and LoginName like '%" + txtAccount.Text.Trim().Replace("\'", "") + "%' ");
        }
        //公司名称
        if (txtCompanyName.Text.Trim() != "")
        {
            sbSQL.Append(" and UninAllName like '%" + txtCompanyName.Text.Trim().Replace("\'", "") + "%' ");
        }
        //用于查看含有行程单的用户
        if (ckFenPei.Checked)
        {
            sbSQL.Append(" and Total >0 ");
        }        
        return sbSQL.ToString();
    }

    public void BindData()
    {
        try
        {
            Con = Query();
            int TotalCount = 0;
            StringBuilder TableSQL = new StringBuilder();
            TableSQL.Append("select LoginName,UserName,CpyNo, C.UninAllName UninAllName,C.RoleType,  ");
            TableSQL.Append("left(CpyNo,12) OwnerCpyNo,  ");
            TableSQL.Append("(select UninAllName from User_Company where UninCode=left(CpyNo,12)) OwnerCpyName,  ");
            TableSQL.Append("(select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  not in(1,3,4,5)) Total,  ");
            TableSQL.Append("(select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(2,8,11)) NoUse ,  ");
            TableSQL.Append("(select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  not in(2,8,11,1)) IsUse,  ");
            TableSQL.Append("(select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(10)) IsVoid,  ");
            TableSQL.Append("(select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(6)) IsNotVoid,  ");
            TableSQL.Append("(select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(11)) TotalBack  ");
            TableSQL.Append(" from User_Employees,User_Company C where CpyNo=C.UninCode  and C.RoleType>3 and  IsAdmin=0 Group by LoginName,UserName,CpyNo, UninAllName,C.RoleType");
            DataTable table = this.baseDataManage.ExecProcPager("proc_TripNumPager", Curr, AspNetPager1.PageSize, TableSQL.ToString(), Con, " CpyNo", out TotalCount);
            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            repList.DataSource = table;
            repList.DataBind();
        }
        catch (Exception ex)
        {
        }
    }
    public void ExportToExcel()
    {
        try
        {
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(" select * from ( ");
            sbSQL.Append(" select LoginName,UserName,CpyNo, C.UninAllName UninAllName,C.RoleType, ");
            sbSQL.Append(" left(CpyNo,12) OwnerCpyNo, ");
            sbSQL.Append(" (select UninAllName from User_Company where UninCode=left(CpyNo,12)) OwnerCpyName, ");
            sbSQL.Append(" (select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  not in(1,3,4,5)) Total, ");
            sbSQL.Append(" (select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(2,8,11)) NoUse , ");
            sbSQL.Append(" (select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  not in(2,8,11,1)) IsUse, ");
            sbSQL.Append(" (select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(10)) IsVoid, ");
            sbSQL.Append(" (select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(6)) IsNotVoid, ");
            sbSQL.Append(" (select COUNT(id)  from Tb_TripDistribution where useCpyNo=CpyNo and TripStatus  in(11)) TotalBack ");
            sbSQL.Append(" from User_Employees,User_Company C where CpyNo=C.UninCode  and C.RoleType>3 and  IsAdmin=0 Group by LoginName,UserName,CpyNo, UninAllName,C.RoleType)  as A ");
            sbSQL.Append(" where " + Query());

            DataTable dtxcd = this.baseDataManage.GetDataTableBySql(sbSQL.ToString());
            #region excel 数据
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("客户名称"));
            dt.Columns.Add(new DataColumn("客户账号"));
            dt.Columns.Add(new DataColumn("所有"));
            dt.Columns.Add(new DataColumn("已使用"));
            dt.Columns.Add(new DataColumn("未使用"));
            dt.Columns.Add(new DataColumn("作废已审核"));
            dt.Columns.Add(new DataColumn("作废未审核"));
            dt.Columns.Add(new DataColumn("空白回收"));

            for (int i = 0; i < dtxcd.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["客户名称"] = dtxcd.Rows[i]["LoginName"];
                dr["客户账号"] = dtxcd.Rows[i]["UninAllName"];
                dr["所有"] = dtxcd.Rows[i]["Total"];
                dr["已使用"] = dtxcd.Rows[i]["IsUse"];
                dr["未使用"] = dtxcd.Rows[i]["NoUse"];
                dr["作废已审核"] = dtxcd.Rows[i]["IsVoid"];
                dr["作废未审核"] = dtxcd.Rows[i]["IsNotVoid"];
                dr["空白回收"] = dtxcd.Rows[i]["TotalBack"];
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            dt = dv.ToTable();
            DataRow drnul = dt.NewRow();
            for (int i = 0; i < 80; i++)
            {
                dt.Rows.Add(drnul[1]);
            }
            #endregion
            new ExcelData().SaveToExcel(dt, "行程单报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 绑定供应或落地运营
    /// </summary>
    public void BindGY()
    {
        DataTable table = this.baseDataManage.GetGYEmpolyees();
        ddlGY.Items.Clear();
        ddlGY.Items.Add(new ListItem("---供应或落地运营商---", ""));
        if (table != null && table.Rows.Count > 0)
        {
            foreach (DataRow dr in table.Rows)
            {
                ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString() + "@@" + dr["LoginName"].ToString() + "@@" + dr["UserName"].ToString());
                ddlGY.Items.Add(li);
            }
        }
    }

    protected void repList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string[] strArr = e.CommandArgument.ToString().Split('|');
        string url = "";
        if (strArr.Length == 6)
        {
            string LoginName = strArr[0];
            string UserName = strArr[1];
            string UseCpyNo = strArr[2];
            string UninAllName = strArr[3];
            string OwnerCpyNo = strArr[4];
            string OwnerCpyName = strArr[5];
            url = string.Format("&UseCpyNo={0}&UseCpyName={1}&OwnerCpyNo={2}&OwnerCpyName={3}", UseCpyNo, UninAllName, OwnerCpyNo, OwnerCpyName);
        }
        if (e.CommandName == "Detail")
        {
            Response.Redirect("~/TravelNumManage/TriplList.aspx?currentuserid=" + this.currentuserid.Value.ToString() + url);
        }
        else if (e.CommandName == "TripSend")
        {
            if (mCompany.RoleType == 1)
            {
                strArr = ddlGY.SelectedValue.Split(new string[] { "@@" }, StringSplitOptions.None);
                if (strArr.Length >= 2)
                {
                    url = string.Format(url + "&AddLoginName={0}&AddUserName={1}", strArr[1], strArr[2]);
                }
            }
            Response.Redirect("~/TravelNumManage/AddTripNum.aspx?currentuserid=" + this.currentuserid.Value.ToString() + url);
        }
    }
    //查询
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;      
        AspNetPager1.CurrentPageIndex = Curr;
        BindData();
    }
    /// <summary>
    /// 显示数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] obj)
    {
        string result = "";
        if (type == 0)
        {
            if (obj != null && obj.Length > 0)
            {
                foreach (object item in obj)
                {
                    if (item != null)
                    {
                        result += item.ToString() + "|";
                    }
                }
                result = result.Trim(new char[] { '|' });
            }
        }
        return result;
    }
    /// <summary>
    /// 导出
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        ExportToExcel();
    }
}