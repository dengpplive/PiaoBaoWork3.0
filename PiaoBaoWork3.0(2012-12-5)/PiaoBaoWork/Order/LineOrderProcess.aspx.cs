using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using PbProject.Dal.SQLEXDAL;
using System.Data;

public partial class Order_LineOrderProcess : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //if (this.SessionIsNull)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！');", true);
            //    return;
            //}
            if (!IsPostBack)
                this.currentuserid.Value = this.mUser.id.ToString();
            if (mCompany.RoleType > 3)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您没有权限访问该页面！');", true);
                return;
            }
            if (!IsPostBack)
            {
                //创建日期
                DateTime dt = DateTime.Now;
                //txtCreateTime1.Value = dt.AddMonths(-1).ToString("yyyy-MM-dd");
                txtCreateTime1.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
                txtCreateTime2.Value = dt.ToString("yyyy-MM-dd");
                InitPageData();
                //用于订单提醒查询
                showPrompt();
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面初始化异常!');", true);
        }
    }
    /// <summary>
    /// 用于 订单提醒查询
    /// </summary>
    public void showPrompt()
    {
        if (Request["prompt"] != null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "btnOk('" + Request["prompt"].ToString() + "');", true);
        }
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    public void InitPageData()
    {
        //分页大小
        AspNetPager1.PageSize = int.Parse(hid_PageSize.Value);
        OrderBy = "  CreateTime desc ";
        Curr = 1;
        Con = " 1=1 ";

        //绑定订单状态
        BindStatus(1);
        //绑定城市
        // BindCity();
        //用户角色
        Hid_RoleType.Value = mCompany.RoleType.ToString();        
        Hid_LoginName.Value = mUser.LoginName;//登录账号
        if (mCompany.RoleType == 1)
        {
            //供应运营商
            div_GY.Visible = true;
            ddlGY.Visible = true;
            BindGY();
        }
        else
        {
            div_GY.Visible = false;
            ddlGY.Visible = false;
        }
    }


    #region 属性
    private List<Bd_Air_AirPort> list = null;
    private List<Bd_Base_Dictionary> diclist = null;
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
    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Ticket_Order> list = baseDataManage.CallMethod("Tb_Ticket_Order", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, OrderBy }) as List<Tb_Ticket_Order>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        RepOrderList.DataSource = list;
        RepOrderList.DataBind();
    }
    /// <summary>
    /// 分页事件
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }

    /// <summary>
    /// 绑定订单状态
    /// </summary>
    public void BindStatus(int ParentID)
    {
        if (diclist == null)
        {
            diclist = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { "parentid=" + ParentID + " order by ChildID" }) as List<Bd_Base_Dictionary>;
        }
        ddlStatus.DataSource = diclist;
        ddlStatus.DataTextField = "ChildName";
        ddlStatus.DataValueField = "ChildID";
        ddlStatus.DataBind();
        ddlStatus.Items.Insert(0, new ListItem("--全部状态--", "0"));
    }
    /// <summary>
    /// 获取城市数据
    /// </summary>
    /// <param name="IsDomestic"></param>
    /// <returns></returns>
    public List<Bd_Air_AirPort> GetCity(string IsDomestic)
    {
        return this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=" + IsDomestic }) as List<Bd_Air_AirPort>;
    }
    public Bd_Air_AirPort GetCityInfo(string data)
    {
        if (list == null)
        {
            list = GetCity("1");
        }
        Bd_Air_AirPort cityinfo = list.Find(delegate(Bd_Air_AirPort item)
        {
            return (!string.IsNullOrEmpty(data) && (item.CityCodeWord.ToUpper() == data.ToUpper() || item.CityName.ToUpper() == data.ToUpper()));
        });

        return cityinfo;
    }
    /// <summary>
    /// 绑定城市
    /// </summary>
    //public void BindCity()
    //{
    //    if (list == null)
    //    {
    //        list = GetCity("1");
    //    }
    //    //出发城市
    //    ddlFromCity.DataSource = list;
    //    ddlFromCity.DataFiledText = "CityCodeWord-CityName";
    //    ddlFromCity.DataFiledValue = "CityCodeWord";
    //    ddlFromCity.DataBind();
    //    //到达城市
    //    ddlToCity.DataSource = list;
    //    ddlToCity.DataFiledText = "CityCodeWord-CityName";
    //    ddlToCity.DataFiledValue = "CityCodeWord";
    //    ddlToCity.DataBind();
    //}
    /// <summary>
    /// 绑定供应或者落地运营商
    /// </summary>
    public void BindGY()
    {
        SQLEXDAL_Base sqlexdal_base = new SQLEXDAL_Base();
        DataTable table = sqlexdal_base.GetGYEmpolyees();
        ddlGY.Items.Clear();
        ddlGY.Items.Add(new ListItem("---供应商或运营商---", ""));
        if (table != null && table.Rows.Count > 0)
        {
            foreach (DataRow dr in table.Rows)
            {
                ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString() + "@" + dr["LoginName"].ToString() + "@" + dr["UninAllName"].ToString() + "@" + dr["Uid"].ToString() + "@" + dr["Cid"].ToString());
                ddlGY.Items.Add(li);
            }
            ddlGY.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    public string Query()
    {
        StringBuilder sbWhere = new StringBuilder();
        if (mCompany.RoleType == 1)
        {
            string CpyNo = "";
            if (ddlGY.Value.Trim() != "")
            {
                CpyNo = ddlGY.Value.Split(new string[] { "@" }, StringSplitOptions.None)[0];
            }
            //该供应和落地运营上的订单 线下订单
            sbWhere.Append(string.Format(" LEFT(OwnerCpyNo,12)='{0}' and OrderSourceType=4 ", CpyNo));
        }
        else
        {
            //该供应和落地运营上的订单 线下订单
            sbWhere.Append(string.Format(" LEFT(OwnerCpyNo,12)='{0}' and OrderSourceType=4 ", mCompany.UninCode));
        }
        //订单号
        if (txtOrderId.Text.Trim() != "")
        {
            sbWhere.AppendFormat(" and OrderId='{0}' ", txtOrderId.Text.Trim().Replace("\'", ""));
        }
        //订单状态
        if (ddlStatus.SelectedValue != "" && ddlStatus.SelectedValue != "0")
        {
            sbWhere.Append(string.Format(" and  OrderStatusCode = '{0}' ", ddlStatus.SelectedValue.Trim()));
        }
        //乘机人姓名
        if (txtPassengerName.Text.Trim() != "")
        {
            sbWhere.AppendFormat(" and PassengerName like '%{0}%' ", txtPassengerName.Text.Trim().Replace("\'", ""));
        }
        //出发城市
        if (ddlFromCity.Value != "0" && ddlFromCity.Value != "")
        {
            sbWhere.Append(string.Format(" and  TravelCode  like '%{0}%' ", ddlFromCity.Value.Trim()));
        }
        //到达城市
        if (ddlToCity.Value != "0" && ddlToCity.Value != "")
        {
            sbWhere.Append(string.Format(" and  TravelCode  like '%{0}%' ", ddlToCity.Value.Trim()));
        }
        //乘机日期
        if (txtFromDate1.Value.Trim() != "" && txtFromDate2.Value.Trim() != "")
        {
            sbWhere.Append(string.Format(" and  (AirTime>='{0} 00:00:00' and  AirTime<='{1} 23:59:59') ", txtFromDate1.Value.Trim(), txtFromDate2.Value.Trim()));
        }
        //创建日期
        if (txtCreateTime1.Value.Trim() != "" && txtCreateTime2.Value.Trim() != "")
        {
            sbWhere.Append(string.Format(" and  (CreateTime>='{0} 00:00:00' and  CreateTime<= '{1} 23:59:59') ", txtCreateTime1.Value.Trim(), txtCreateTime2.Value.Trim()));
        }
        return sbWhere.ToString();
    }

    //页面显示数据
    public string ShowText(int opType, object objData)
    {
        string strReData = "";
        if (opType == 0)
        {
            //创建时间
            if (objData != null)
            {
                strReData = objData.ToString().Trim().Replace(" ", "<br />");
            }
        }
        else if (opType == 1)
        {
            //显示乘机人
            if (objData != null)
            {
                strReData = objData.ToString().Replace("|", "<br />");
            }
        }
        else if (opType == 2)
        {
            //起飞日期
            if (objData != null)
            {
                strReData = objData.ToString().Trim().Replace("/", "<br />");
            }
        }
        else if (opType == 3)
        {
            //行程
            if (objData != null)
            {
                if (objData.ToString().Trim() == "1")
                {
                    strReData = "单程";
                }
                else if (objData.ToString().Trim() == "2")
                {
                    strReData = "往返";
                }
                else if (objData.ToString().Trim() == "3")
                {
                    strReData = "联程";
                }
                else if (objData.ToString().Trim() == "4")
                {
                    strReData = "多程";
                }
            }
        }
        else if (opType == 4)
        {
            //订单状态
            if (objData != null)
            {
                if (diclist == null)
                {
                    diclist = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { "parentid=1" }) as List<Bd_Base_Dictionary>;
                }
                Bd_Base_Dictionary d = diclist.Find(delegate(Bd_Base_Dictionary _dic)
                  {
                      return _dic.ChildID.ToString() == objData.ToString().Trim();
                  });
                if (d != null)
                {
                    strReData = d.ChildName.Replace("，", ",").Replace(",", "<br />");
                }
            }
        }
        else if (opType == 5)
        {
            //航程
            if (objData != null)
            {
                strReData = objData.ToString().Trim().Replace("/", "<br />");
            }
        }
        else if (opType == 6)
        {
            //订单状态
            if (objData != null)
            {
                strReData = objData.ToString().Trim() == "27" ? "show" : "hide";
            }
        }
        return strReData;
    }
    /// <summary>
    /// 显示或者隐藏不算成人
    /// </summary>
    /// <param name="IsChdFlag"></param>
    /// <param name="HaveBabyFlag"></param>
    /// <returns></returns>
    public string ShowHand(string IsChdFlag, string HaveBabyFlag)
    {
        string result = "hide";
        if (IsChdFlag.Trim().ToUpper() == "TRUE" || IsChdFlag.Trim() == "1")
        {
            result = "hide";
        }
        else
        {
            if (HaveBabyFlag.Trim().ToUpper() == "TRUE" || HaveBabyFlag.Trim() == "1")
            {
                result = "show";
            }
        }
        return result;
    }

    //查询
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;
        Con = Query();
        PageDataBind();
    }
}