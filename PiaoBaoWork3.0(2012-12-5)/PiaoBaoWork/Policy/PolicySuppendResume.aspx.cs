using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Text;
using System.Data;
using PbProject.Logic;
using DataBase.Data;
public partial class Policy_PolicySuppendResume : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (this.SessionIsNull)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！',{op:0});", true);
        //    return;
        //}
        if (!IsPostBack)
        {
            BindAirList();
            if (mCompany.RoleType == 1)
            {
                BindGY();
            }
            ViewState["orderBy"] = "CreateTime desc ";
            Curr = 1;
            Con = string.Format(" CpyNo='{0}' and ModuleName='政策挂起解挂' ", mCompany.UninCode);
            //供应和落地运营商公司编号
            Hid_CpyNo.Value = mCompany.UninCode;
            Hid_LoginName.Value = mUser.LoginName;
            Hid_UserName.Value = mUser.UserName;
            Hid_UserRoleType.Value = mCompany.RoleType.ToString();
            //绑定操作日志
            BindOPLog();
        }
    }

    #region 属性
    List<Bd_Air_Carrier> m_AirList = null;
    /// <summary>
    /// 航空公司属性
    /// </summary>
    public List<Bd_Air_Carrier> AirList
    {
        get
        {
            return m_AirList;
        }
        set
        {
            m_AirList = value;
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
    /// <summary>
    /// 查询条件
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    /// <summary>
    /// 当前分页第几页
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    /// <summary>
    /// 排序字段和升降序
    /// </summary>
    public string OrderBy
    {
        get { return (string)ViewState["orderBy"]; }
        set { ViewState["orderBy"] = value; }
    }
    #endregion
    /// <summary>
    /// 获取航空公司信息
    /// </summary>
    /// <returns></returns>
    public List<Bd_Air_Carrier> GetAirList()
    {
        List<Bd_Air_Carrier> list = new List<Bd_Air_Carrier>();
        list = this.baseDataManage.CallMethod("Bd_Air_Carrier", "GetList", null, new object[] { "" }) as List<Bd_Air_Carrier>;
        this.AirList = list;
        return AirList;
    }

    /// <summary>
    /// 绑定列表
    /// </summary>
    public void BindAirList()
    {
        if (m_AirList == null)
        {
            GetAirList();
        }
        StringBuilder sbB2B = new StringBuilder();
        StringBuilder sbBSP = new StringBuilder();

        sbB2B.Append("<div id=\"div_b2b\" class=\"green\">");
        sbBSP.Append("<div id=\"div_bsp\" class=\"green\">");
        //每行10个
        int Len = 10;
        if (m_AirList != null)
        {
            for (int i = 0; i < m_AirList.Count; i++)
            {
                if ((i + 1) % Len == 0)
                {
                    sbB2B.AppendFormat("<label for=\"b2b_air_{0}\" style=\"cursor:pointer;\"><input type=\"checkbox\" name=\"b2b_air\" id=\"b2b_air_{0}\" value=\"{1}\">{2}</label><br />", i, m_AirList[i].Code, (m_AirList[i].ShortName + "[" + m_AirList[i].Code + "]").PadRight(10, ' '));
                    sbBSP.AppendFormat("<label for=\"bsp_air_{0}\" style=\"cursor:pointer;\"><input type=\"checkbox\" name=\"bsp_air\" id=\"bsp_air_{0}\" value=\"{1}\">{2}</label><br />", i, m_AirList[i].Code, (m_AirList[i].ShortName + "[" + m_AirList[i].Code + "]").PadRight(10, ' '));
                }
                else
                {
                    sbB2B.AppendFormat("<label for=\"b2b_air_{0}\" style=\"cursor:pointer;\"><input type=\"checkbox\" name=\"b2b_air\" id=\"b2b_air_{0}\" value=\"{1}\">{2}</label>", i, m_AirList[i].Code, (m_AirList[i].ShortName + "[" + m_AirList[i].Code + "]").PadRight(10, ' '));
                    sbBSP.AppendFormat("<label for=\"bsp_air_{0}\" style=\"cursor:pointer;\"><input type=\"checkbox\" name=\"bsp_air\" id=\"bsp_air_{0}\" value=\"{1}\">{2}</label>", i, m_AirList[i].Code, (m_AirList[i].ShortName + "[" + m_AirList[i].Code + "]").PadRight(10, ' '));
                }
            }
        }
        sbB2B.Append("</div>");
        sbBSP.Append("</div>");
        B2BSuppendBox.Text = sbB2B.ToString();
        BSPSuppendBox.Text = sbBSP.ToString();
    }

    /// <summary>
    /// 绑定供应和落地运营
    /// </summary>
    public void BindGY()
    {
        TRGY.Visible = true;
        span_msg.Visible = true;
        DataTable table = this.baseDataManage.GetGYEmpolyees();
        StringBuilder sbLog = new StringBuilder();
        sbLog.Append("<select class=\"wd250\" onchange=\"SetHid(this)\" id=\"SelGY\">");
        string val = "", text = "", IsSelected = "";
        foreach (DataRow dr in table.Rows)
        {
            if (Hid_CpyNo.Value.Trim() == "")
            {
                Hid_CpyNo.Value = dr["UninCode"].ToString();
            }
            if (Hid_CpyNo.Value.Trim() == dr["UninCode"].ToString())
            {
                IsSelected = " selected=true ";
            }
            else
            {
                IsSelected = "";
            }
            text = dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString();
            val = dr["UninCode"].ToString() + "@@" + dr["LoginName"].ToString() + "@@" + dr["UserName"].ToString();
            sbLog.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", val, IsSelected, text);
        }
        sbLog.Append("</select>");
        literGY.Text = sbLog.ToString();
    }

    /// <summary>
    /// 分页事件
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindOPLog();
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    public string Query()
    {
        StringBuilder sbSQlWhere = new StringBuilder();
        sbSQlWhere.AppendFormat(" CpyNo='{0}' and ModuleName='政策挂起解挂' ", Hid_CpyNo.Value.Trim());
        //账号
        if (txtLoginName.Text.Trim() != "")
        {
            sbSQlWhere.AppendFormat(" and LoginName='{0}' ", txtLoginName.Text.Trim());
        }
        //姓名
        if (txtUserName.Text.Trim() != "")
        {
            sbSQlWhere.AppendFormat(" and UserName='{0}' ", txtUserName.Text.Trim());
        }
        //挂起还是解挂
        if (ddlopType.SelectedValue != "")
        {
            sbSQlWhere.AppendFormat(" and OperateType='{0}' ", ddlopType.SelectedValue);
        }
        //日期
        if (txtStart.Value.Trim() != "" && txtEnd.Value.Trim() != "")
        {
            sbSQlWhere.AppendFormat(" and CreateTime between '{0}'  and '{1}'", txtStart.Value.Trim(), txtEnd.Value.Trim());
        }
        return sbSQlWhere.ToString();
    }
    /// <summary>
    /// 绑定政策挂起解挂日志
    /// </summary>
    public void BindOPLog()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Log_Operation> list = baseDataManage.CallMethod("Log_Operation", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, OrderBy }) as List<Log_Operation>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        Repeater.DataSource = list;
        Repeater.DataBind();
    }
    //查询日志
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Con = Query();
        Curr = 1;
        BindOPLog();
    }
    //清空日志
    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtEnd.Value = "";
        txtStart.Value = "";
        txtLoginName.Text = "";
        txtUserName.Text = "";
        ddlopType.SelectedIndex = -1;
    }
    /// <summary>
    /// 页面日志显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string ShowText(int type, object obj)
    {
        string result = "";
        if (type == 0)
        {
            if (obj != null && obj != DBNull.Value)
            {
                if (obj.ToString().Contains("解挂"))
                {
                    result = "<span class=\"green\">" + obj.ToString() + "</span>";
                }
                else
                {
                    result = "<span class=\"red\">" + obj.ToString() + "</span>";
                }
            }
        }
        return result;
    }
    /// <summary>
    /// 显示部分字符
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Len"></param>
    /// <returns></returns>
    public string SubChar(object Str, int Len, string replaceSchar)
    {
        string reStr = "";
        if (Str == null) return reStr;
        reStr = Str.ToString();
        if (!string.IsNullOrEmpty(reStr))
        {
            if (reStr.Length > Len)
            {
                reStr = reStr.Substring(0, Len) + " " + replaceSchar;
            }
        }
        else
        {
            reStr = "";
        }
        return reStr;
    }
}