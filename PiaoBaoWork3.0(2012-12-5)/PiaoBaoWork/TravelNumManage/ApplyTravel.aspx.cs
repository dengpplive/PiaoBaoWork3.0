using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using DataBase.Data;
using PbProject.Model;
using PbProject.Logic;
public partial class TravelNumManage_ApplyTravel : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (this.SessionIsNull)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！',{p:0});", true);
        //    return;
        //}
        try
        {
            if (!IsPostBack)
            {
                //默认按照申请时间排序
                OrderBy = " ApplyDate  desc";
                AspNetPager1.PageSize = int.Parse(hid_PageSize.Value);
                AspNetPager1.CurrentPageIndex = 1;
                //角色
                Hid_RoleType.Value = mCompany.RoleType.ToString();
                if (mCompany.RoleType < 4)
                {
                    lblShow.Text = "行程单审核";
                }
                if (mCompany.RoleType == 1)
                {
                    tr1.Visible = true;
                    tr2.Visible = true;
                    if (Request["CpyName"] != null)
                    {
                        Hid_CpyName.Value = Request["CpyName"].ToString();
                    }
                    if (Request["CpyNo"] != null)
                    {
                        Hid_CpyNo.Value = Request["CpyNo"].ToString();
                    }
                    if (Request["LoginName"] != null)
                    {
                        Hid_Account.Value = Request["LoginName"].ToString();
                    }
                    if (Request["UserName"] != null)
                    {
                        Hid_UserName.Value = Request["UserName"].ToString();
                    }
                    BindGY();
                }
                else
                {
                    tr1.Visible = false;
                    tr2.Visible = false;
                    if (mCompany != null)
                    {
                        //客户名称
                        Hid_CpyName.Value = mCompany.UninAllName;
                        //客户公司编号
                        Hid_CpyNo.Value = mCompany.UninCode.ToString();
                    }
                    if (mUser != null)
                    {
                        //申请人账号
                        Hid_Account.Value = mUser.LoginName;
                        //申请人姓名
                        Hid_UserName.Value = mUser.UserName;
                    }
                    if (mSupCompany != null)
                    {
                        Hid_AuditCpyNo.Value = mSupCompany.UninCode;
                    }
                }
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面加载异常！',{p:0});", true);
        }
        if (Request["optype"] != null && Request["optype"].ToString() != "")
        {
            string optype = Request["optype"].ToString();
            if (optype == "TripApply")
            {
                //行程单申请
                TripApply();
            }
        }
        //拒绝申请
        JuJueAppliay();
    }


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
                ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString() + "@@" + dr["UninAllName"].ToString() + "@@" + dr["LoginName"].ToString() + "@@" + dr["UserName"].ToString());
                ddlGY.Items.Add(li);
            }
        }
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    public string QueryWhere()
    {
        StringBuilder sbWhere = new StringBuilder();// 
        if (mCompany.RoleType < 4)
        {
            sbWhere.Append(string.Format(" AuditCpyNo='{0}' ", Hid_AuditCpyNo.Value.Trim()));
        }
        else
        {
            sbWhere.Append(string.Format(" ApplyCpyNo='{0}' and ApplyAccount='{1}' and AuditCpyNo='{2}' ", Hid_CpyNo.Value.Trim(), Hid_Account.Value.Trim(), Hid_AuditCpyNo.Value.Trim()));
        }
        if (txtApplyStartDate.Value.Trim() != "" && txtApplyEndDate.Value.Trim() != "")
        {
            sbWhere.AppendFormat("  and  ApplyDate between '{0} 00:00:00' and '{1} 23:59:59' ", txtApplyStartDate.Value.Trim(), txtApplyEndDate.Value.Trim());
        }
        if (ddlAdult.SelectedValue != "-1" && ddlAdult.SelectedValue != "")
        {
            sbWhere.AppendFormat(" and ApplyStatus={0} ", ddlAdult.SelectedValue.Trim());
        }
        return sbWhere.ToString();
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
            List<Tb_TripNumApply> list = baseDataManage.CallMethod("Tb_TripNumApply", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, PageIndex, "*", Con, OrderBy }) as List<Tb_TripNumApply>;
            TotalCount = outParams.GetValue<int>("1");
            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = PageIndex;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + PageIndex + "</font> / " + AspNetPager1.PageCount;
            repApplyList.DataSource = list;
            repApplyList.DataBind();
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
    //行程单查询
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Con = QueryWhere();
        PageIndex = 1;
        AspNetPager1.CurrentPageIndex = PageIndex;
        PageDataBind();
    }
    /// <summary>
    /// 截取字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public string SubStrStr(object Remark, int len)
    {
        string str = Remark == null || Remark == DBNull.Value ? "" : Remark.ToString();
        if (!string.IsNullOrEmpty(str))
        {
            if (str.Length > len)
            {
                str = str.Substring(0, len) + "...";
            }
        }
        return str;
    }


    /// <summary>
    /// 行程单申请
    /// </summary>
    public void TripApply()
    {
        string ApplyNum = GetVal("ApplyNum", "");
        string ApplyRemark = GetVal("ApplyRemark", "");
        string CpyName = GetVal("CpyName", ""); //公司名称
        string CpyNo = GetVal("CpyId", ""); //公司编号
        string Account = GetVal("Account", ""); //用户帐号
        string UserName = GetVal("UserName", ""); //用户姓名 
        string AuditCpyNo = GetVal("AuditCpyNo", ""); //用户姓名
        int IsAdd = 0;
        StringBuilder sbLog = new StringBuilder();
        sbLog.AppendFormat("ApplyNum:{0} ApplyRemark:{1} CpyName:{2} CpyId:{3} Account:{4} UserName:{5}", ApplyNum, ApplyRemark, CpyName, CpyNo, Account, UserName);
        try
        {
            Tb_TripNumApply tb_tripnumapply = new Tb_TripNumApply();
            tb_tripnumapply.ApplyAccount = Account;//申请人账号
            tb_tripnumapply.ApplyUserName = UserName;//申请人姓名
            tb_tripnumapply.ApplyCpyNo = CpyNo;//申请人公司编号
            tb_tripnumapply.ApplyCpyName = CpyName;//申请人公司名称
            tb_tripnumapply.ApplyCount = ApplyNum;//申请行程单数目 
            tb_tripnumapply.ApplyRemark = ApplyRemark;//申请行程单描述
            tb_tripnumapply.ApplyStatus = 3;//行程单状态
            tb_tripnumapply.ApplyDate = System.DateTime.Now;//申请日期
            tb_tripnumapply.AuditCpyNo = AuditCpyNo;//上级公司编号
            //添加
            bool IsSuc = (bool)this.baseDataManage.CallMethod("Tb_TripNumApply", "Insert", null, new object[] { tb_tripnumapply });
            if (IsSuc)
            {
                IsAdd = 1;
            }
        }
        catch (Exception ex)
        {
            DataBase.LogCommon.Log.Error("行程单申请参数:" + sbLog.ToString(), ex);
        }
        try
        {
            OutPut(IsAdd.ToString());
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 供应或者落地运营商 拒绝申请
    /// </summary>
    public void JuJueAppliay()
    {
        if (Request["optype"] != null && Request["optype"].ToString() == "jujue")
        {
            int IsAdd = 0;
            StringBuilder sbLog = new StringBuilder();
            try
            {
                string id = GetVal("Id", "");
                string AuditRemark = GetVal("AuditRemark", "");
                string AuditAccount = GetVal("AuditAccount", "");
                string AuditUserName = GetVal("AuditUserName", "");
                string AuditCpyNo = GetVal("AuditCpyNo", "");
                string AuditCpyName = GetVal("AuditCpyName", "");
                IHashObject param = new HashObject();
                param.Add("id", id);
                param.Add("AuditRemark", AuditRemark);
                param.Add("ApplyStatus", 5);
                param.Add("AuditDate", System.DateTime.Now);
                param.Add("AuditAccount", AuditAccount);
                param.Add("AuditUserName", AuditUserName);
                param.Add("AuditCpyNo", AuditCpyNo);
                param.Add("AuditCpyName", AuditCpyName);
                sbLog.AppendFormat("AuditRemark:{0} AuditAccount:{1} AuditUserName:{2} AuditCpyNo:{3} AuditCpyName:{4}",
                 AuditRemark, AuditAccount, AuditUserName, AuditCpyNo, AuditCpyName);
                bool IsSuc = (bool)this.baseDataManage.CallMethod("Tb_TripNumApply", "Update", null, new object[] { param });
                if (IsSuc)
                {
                    IsAdd = 1;
                }
            }
            catch (Exception ex)
            {
                DataBase.LogCommon.Log.Error("行程单申请参数:" + sbLog.ToString(), ex);
            }
            try
            {
                OutPut(IsAdd.ToString());
            }
            catch (Exception)
            {
            }
        }
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
    public string ShowText(int type, object obj)
    {
        string result = "";
        if (type == 0)
        {
            //行程单状态
            int ApplyStatus = (obj == DBNull.Value || obj == null) ? 3 : int.Parse(obj.ToString());
            //0未审核 1审核通过 2审核未通过
            result = "未审核";
            if (ApplyStatus == 3)
            {
                result = "未审核";
            }
            else if (ApplyStatus == 4)
            {
                result = "<font style=\"color:green;\">审核通过</font>";
            }
            else if (ApplyStatus == 5)
            {
                result = "<font style=\"color:red;\">审核未通过</font>";
            }
        }
        else if (type == 1)
        {
            result = "hide";
            //if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
            if (mCompany.RoleType < 4)
            {
                result = "show";
            }
        }
        else if (type == 2)
        {
            result = "hide";
            if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
            {
                result = "show";
            }
        }
        return result;
    }
}