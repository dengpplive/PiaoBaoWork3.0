using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using PbProject.Logic;
public partial class TravelNumManage_TriplList : BasePage
{
    #region 属性
    private List<Bd_Base_Dictionary> TripNumList = new List<Bd_Base_Dictionary>();
    /// <summary>
    /// 过滤条件
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    /// <summary>
    /// 当前页索引
    /// </summary>
    protected int PageIndex
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    /// <summary>
    /// 用于排序
    /// </summary>
    protected string OrderBy
    {
        get { return (string)ViewState["OrderBy"]; }
        set { ViewState["OrderBy"] = value; }
    }
    /// <summary>
    /// 是否为行程单入库 true 是 false 否
    /// </summary>
    public bool IsImport
    {
        get
        {
            bool IsImport = false;
            if (Request["Import"] != null)
            {
                IsImport = true;
                Hid_Import.Value = "1";
            }
            return IsImport;
        }

    }
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

    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            this.currentuserid.Value = this.mUser.id.ToString();
        //if (this.SessionIsNull)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！','0');", true);
        //    return;
        //}
        try
        {
            //if (mCompany.RoleType < 4)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您没有权限访问该页面！','0');", true);
            //    return;
            //}
            if (!IsPostBack)
            {
                //角色
                Hid_RoleType.Value = mCompany.RoleType.ToString();
                OrderBy = " TripNum ";
                AspNetPager1.PageSize = int.Parse(Hid_PageSize.Value);
                AspNetPager1.CurrentPageIndex = 1;
                //初始化参数
                InitParam();
                //绑定行程单状态
                BindTripState();
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面加载异常！','0');", true);
        }
        //ajax修改数据
        UpdateCon();
    }

    public void InitParam()
    {
        bool IsUse = false;
        //领用下级公司
        if (Request["UseCpyNo"] != null)
        {
            Hid_UseCpyNo.Value = Request["UseCpyNo"].ToString();
            IsUse = true;
        }
        if (Request["OwnerCpyNo"] != null)
        {
            Hid_OwnerCpyNo.Value = Request["OwnerCpyNo"].ToString();
        }
        //显示入库按钮
        if (IsImport)
        {
            span_import.Visible = true;
            lblShow.Text = "行程单入库管理";
        }
        else
        {
            span_import.Visible = false;
        }
        //显示选择供应和落地运营商按钮
        if (mCompany.RoleType == 1)
        {
            if (!IsUse)
            {
                tr1.Visible = true;
                tr2.Visible = true;
                BindGY();
            }
            else
            {
                tr1.Visible = false;
                tr2.Visible = false;
            }
        }
        else
        {
            tr1.Visible = false;
            tr2.Visible = false;
        }
        //显示批量操作按钮
        if (mCompany.RoleType < 4 && !IsImport)
        {
            span1.Visible = true;
            span2.Visible = true;
            span3.Visible = true;
            span_Office.Visible = true;
        }
        else
        {
            span1.Visible = false;
            span2.Visible = false;
            span3.Visible = false;
            span_Office.Visible = false;
        }
    }

    /// <summary>
    /// 绑定列表
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            int TotalCount = 0;
            IHashObject outParams = new HashObject();
            //指定参数类型 第一个参数为out输出类型
            //key 为参数索引从1开始 value为引用类型 out ref
            outParams.Add("1", "out");
            List<Tb_TripDistribution> list = baseDataManage.CallMethod("Tb_TripDistribution", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, PageIndex, "*", Con, OrderBy }) as List<Tb_TripDistribution>;
            TotalCount = outParams.GetValue<int>("1");
            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = PageIndex;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + PageIndex + "</font> / " + AspNetPager1.PageCount;
            repList.DataSource = list;
            repList.DataBind();
        }
        catch (Exception ex)
        {
            DataBase.LogCommon.Log.Error("ApplyTravel.aspx:PageDataBind()", ex);
        }
    }
    //分页事件
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        PageIndex = e.NewPageIndex;
        PageDataBind();
    }
    /// <summary>
    /// 绑定行程单状态
    /// </summary>
    public void BindTripState()
    {
        if (TripNumList.Count == 0)
        {
            string sqlWhere = "ParentID=34 order by childid";
            TripNumList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }
        int RoleType = int.Parse(Hid_RoleType.Value);
        ddlTripState.Items.Clear();
        ddlTripState.Items.Add(new ListItem("所有", "-1"));
        foreach (Bd_Base_Dictionary item in TripNumList)
        {
            if (RoleType > 3 && item.ChildID == 1)
            {
                continue;
            }
            ddlTripState.Items.Add(new ListItem(item.ChildName, item.ChildID.ToString()));
        }
    }
    /// <summary>
    /// 获取行程单状态
    /// </summary>
    /// <returns></returns>
    public string GetDicName(string statusCode)
    {
        string result = "";
        if (TripNumList.Count == 0)
        {
            string sqlWhere = "ParentID=34 ";
            TripNumList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }
        var query = from Bd_Base_Dictionary d in TripNumList
                    where d.ChildID.ToString() == statusCode
                    select d;
        if (query.Count<Bd_Base_Dictionary>() > 0)
        {
            List<Bd_Base_Dictionary> List = query.ToList<Bd_Base_Dictionary>();
            result = List[0].ChildName;
        }
        return result;
    }

    /// <summary>
    /// 页面显示数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string ShowText(int type, object obj)
    {
        string result = "";
        if (type == 0)
        {
            //行程单状态
            result = GetDicName(obj.ToString());
            if (result != "")
            {
                result = result.Replace(",", "<br />").Replace("，", "<br />");
                //状态显示颜色设置
                //未分配
                if (obj.ToString().Trim() == "1" || obj.ToString().Trim() == "8")
                {
                    result = "<font class=\"colorFP\">" + result + "</font>";
                }
                //未使用
                else if (obj.ToString().Trim() == "2" || obj.ToString().Trim() == "11")
                {
                    result = "<font class=\"colorGreen\">" + result + "</font>";
                }
                //作废
                else if (obj.ToString().Trim() == "6" || obj.ToString().Trim() == "7" || obj.ToString().Trim() == "10")
                {
                    result = "<font class=\"colorYellow\">" + result + "</font>";
                }
                //已创建
                else if (obj.ToString().Trim() == "9")
                {
                    result = "<font class=\"colorRed\">" + result + "</font>";
                }
            }
        }
        else if (type == 1)
        {
            //拒绝理由
            if (obj != null && obj != DBNull.Value)
            {
                result = obj.ToString();
            }
        }
        else if (type == 2)
        {
            if (IsImport)
            {
                result = "hide";
            }
            else
            {
                //更具不同角色隐藏页面数据
                if (mCompany.RoleType > 3)
                {
                    result = "hide";
                }
                else
                {
                    result = "show";
                }
            }
        }
        else if (type == 3)
        {
            //是否显示作废行程单审核和拒绝审核
            result = "hide";
            if (obj != null && obj != DBNull.Value)
            {
                if (obj.ToString().Trim() == "6")
                {
                    result = "show";
                }
            }
        }
        else if (type == 4)
        {
            //是否显示空白行程单
            result = "hide";
            if (obj != null && obj != DBNull.Value)
            {
                if (obj.ToString().Trim() == "2")
                {
                    result = "show";
                }
            }
        }
        else if (type == 5)
        {
            //是否显示 空白行程单 作废行程单审核和拒绝审核
            result = "hide";
            if (obj != null && obj != DBNull.Value)
            {
                if (obj.ToString().Trim() == "6" || obj.ToString().Trim() == "2")
                {
                    result = "show";
                }
            }
        }
        return result;
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
                //if (dr["RoleType"] != DBNull.Value && dr["RoleType"].ToString() == "2")
                //{
                ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString() + "@@" + dr["UninAllName"].ToString() + "@@" + dr["LoginName"].ToString() + "@@" + dr["UserName"].ToString());
                ddlGY.Items.Add(li);
                //}
            }
        }
    }
    /// <summary>
    /// 获取SQL条件
    /// </summary>
    /// <returns></returns>
    public string Query()
    {
        StringBuilder sbWhere = new StringBuilder();
        if (mCompany.RoleType > 3)
        {
            //分销或者采购
            sbWhere.Append(string.Format("  UseCpyNo='{0}' and OwnerCpyNo='{1}' and TripStatus>1 ", mCompany.UninCode, mSupCompany.UninCode));
        }
        else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
        {
            if (!IsImport)
            {
                //行程单详情
                //供应商和落地运营商
                string UseCpyNo = Hid_UseCpyNo.Value;
                sbWhere.Append(string.Format("  UseCpyNo='{0}' and OwnerCpyNo='{1}' ", UseCpyNo, mCompany.UninCode));
            }
            else
            {
                sbWhere.Append(string.Format("   OwnerCpyNo='{0}' ", mCompany.UninCode));
            }
        }
        else
        {
            if (!IsImport)
            {
                //行程单详情
                //管理员
                string UseCpyNo = Hid_UseCpyNo.Value;
                string OwnerCpyNo = Hid_OwnerCpyNo.Value;
                sbWhere.Append(string.Format("  UseCpyNo='{0}' and OwnerCpyNo='{1}' ", UseCpyNo, OwnerCpyNo));
            }
            else
            {
                string[] selVal = ddlGY.SelectedValue.Split(new string[] { "@@" }, StringSplitOptions.None);
                if (selVal.Length == 4)
                {
                    //行程单入库
                    sbWhere.Append(string.Format("   OwnerCpyNo='{0}' ", selVal[0]));
                }
            }
        }
        //行程单号
        if (txtTripStart.Value.Trim() != "" && txtTripEnd.Value.Trim() != "")
        {
            sbWhere.Append(" and TripNum between '" + txtTripStart.Value.Trim().Replace("\'", "") + "' and '" + txtTripEnd.Value.Trim().Replace("\'", "") + "' ");
        }
        //票号
        if (txtTicketStartNum.Value.Trim() != "" && txtTicketEndNum.Value.Trim() != "")
        {
            sbWhere.AppendFormat(" and replace(A1,'-','') between '{0}' and '{1}'", txtTicketStartNum.Value.Trim().Replace("-", "").Replace("\'", ""), txtTicketEndNum.Value.Trim().Replace("-", "").Replace("\'", ""));
        }
        //发放时间
        if (txtAddStart.Value.Trim() != "" && txtAddEnd.Value.Trim() != "")
        {
            sbWhere.Append(" and UseTime between '" + txtAddStart.Value.Trim().Replace("\'", "") + " 00:00:00' and '" + txtAddEnd.Value.Trim().Replace("\'", "") + " 23:59:59' ");
        }
        //创建打印时间
        if (txtCreateStart.Value.Trim() != "" && txtCreateEnd.Value.Trim() != "")
        {
            sbWhere.Append("and PrintTime >= '" + txtCreateStart.Value.Trim().Replace("\'", "") + " 00:00:00' and PrintTime <= '" + txtCreateEnd.Value.Trim().Replace("\'", "") + " 23:59:59'");
        }
        //作废时间
        if (txtVoidStart.Value.Trim() != "" && txtVoidEnd.Value.Trim() != "")
        {
            sbWhere.Append(" and InvalidTime between '" + txtVoidStart.Value.Trim().Replace("\'", "") + " 00:00:00' and '" + txtVoidEnd.Value.Trim().Replace("\'", "") + " 23:59:59' ");
        }
        //行程单状态
        if (ddlTripState.SelectedValue != "" && ddlTripState.SelectedValue.Trim() != "-1")
        {
            sbWhere.AppendFormat(" and TripStatus={0} ", ddlTripState.SelectedValue);
        }
        //使用的Office
        if (txtOffice.Value.Trim() != "")
        {
            sbWhere.AppendFormat(" and CreateOffice ='{0}' ", txtOffice.Value.Trim());
        }
        return sbWhere.ToString();
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        PageIndex = 1;
        Con = Query();
        PageDataBind();
    }


    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="DefaultVal"></param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }
    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Context.Response.ContentType = "text/plain";
        Context.Response.Clear();
        Context.Response.Write(result);
        Context.Response.Flush();
        Context.Response.End();
    }
    #region 修改数据
    public void UpdateCon()
    {
        if (Request["type"] != null && Request["type"].ToString() != "")
        {
            string type = Request["type"].ToString();
            string Ids = GetVal("IDs", "");
            string[] strArr = Ids.Split(',');
            List<string> idsList = new List<string>();
            idsList.AddRange(strArr);
            string ErrMsg = "0@@失败";
            try
            {
                if (type == "0")//修改选中的Office
                {
                    string Office = GetVal("Office", "").Replace("'", "").ToUpper();
                    bool issuc = (bool)this.baseDataManage.CallMethod("Tb_TripDistribution", "UpdateByIds", null, new object[] { idsList, "CreateOffice='" + Office + "'" });
                    if (issuc)
                    {
                        ErrMsg = "1@@修改Office成功";
                    }
                    else
                    {
                        ErrMsg = "0@@修改Office失败";
                    }
                }
                else
                {
                    bool issuc = false;
                    string UpdateFileds = "";
                    if (type == "1")//空白回收
                    {
                        UpdateFileds = " TripStatus=8 ";//修改为空白回收,未分配 
                    }
                    else if (type == "2")//审核作废行程单
                    {
                        UpdateFileds = " TripStatus=10 ";//修改为 已作废行程单,审核成功 
                    }
                    else if (type == "3")//拒绝作废行程单
                    {
                        UpdateFileds = " TripStatus=7 ";//修改为 拒绝作废行程单,审核失败
                    }
                    if (UpdateFileds.Trim() != "")
                    {
                        //修改状态
                        issuc = (bool)this.baseDataManage.CallMethod("Tb_TripDistribution", "UpdateByIds", null, new object[] { idsList, UpdateFileds });
                    }
                    if (issuc)
                    {
                        ErrMsg = "1@@修改Office成功";
                    }
                    else
                    {
                        ErrMsg = "0@@修改Office失败";
                    }
                }
            }
            catch (Exception ex)
            {
                DataBase.LogCommon.Log.Error("行程单修改Office", ex);
            }
            finally
            {
                OutPut(ErrMsg);
            }
        }
    }

    #endregion
    //行程单入库
    protected void btImport_Click(object sender, EventArgs e)
    {
        string OwnerCpyNo = "";
        string OwnerCpyName = "";
        string AddLoginName = "";
        string AddUserName = "";
        string[] selVal = ddlGY.SelectedValue.Split(new string[] { "@@" }, StringSplitOptions.None);
        if (selVal.Length == 4)
        {
            OwnerCpyNo = selVal[0];
            OwnerCpyName = selVal[1];
            AddLoginName = selVal[2];
            AddUserName = selVal[3];
        }
        string url = string.Format("OwnerCpyNo={0}&OwnerCpyName={1}&AddLoginName={2}&AddUserName={3}&currentuserid={4}", OwnerCpyNo, OwnerCpyName, AddLoginName, AddUserName, this.currentuserid.Value.ToString());
        Response.Redirect("AddTripNum.aspx?Import=1&" + url);
    }
}