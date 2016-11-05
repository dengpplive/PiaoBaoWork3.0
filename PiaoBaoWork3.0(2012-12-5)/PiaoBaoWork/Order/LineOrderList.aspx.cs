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
using System.Collections;
using PbProject.WebCommon.Utility;
public partial class Order_LineOrderList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (dicList == null)
            {
                dicList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { "ParentID=1" }) as List<Bd_Base_Dictionary>;
            }
            //分页大小
            AspNetPager1.PageSize = int.Parse(hid_PageSize.Value);
            OrderBy = " CreateTime desc ";
            Curr = 1;
            Con = " OrderSourceType=4 ";
            //初始化页面数据
            InitData();
            //绑定城市
            //BindCity();
            //订单订单状态
            BindOrderStatus();
        }
        //直接查询
        if (Request["go"] != null && Request["go"].ToString() != "")
        {
            CommitQuery();
        }
    }

    #region 属性
    private List<Bd_Air_AirPort> list = null;
    private List<Bd_Base_Dictionary> diclist = null;
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
    /// <summary>
    /// 机票状态字典表
    /// </summary>
    public List<Bd_Base_Dictionary> dicList
    {
        get { return ViewState["Dic"] as List<Bd_Base_Dictionary>; }
        set { ViewState["Dic"] = value; }
    }
    #endregion

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
    /// 获取机票状态
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public string GetDicName(int childId)
    {
        string result = "";
        foreach (Bd_Base_Dictionary dic in dicList)
        {
            if (dic.ChildID == childId)
            {
                result = dic.ChildName;
                break;
            }
        }
        return result;
    }
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
        repList.DataSource = list;
        repList.DataBind();
    }
    /// <summary>
    /// 显示绑定数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Data"></param>
    /// <returns></returns>
    public string ShowText(int type, object Data)
    {
        string result = "";
        if (type == 1)
        {
            //乘机人
            if (Data != null && Data != DBNull.Value)
            {
                result = Data.ToString().Replace("|", "<br />");
            }
        }
        else if (type == 2)
        {
            //航程
            if (Data != null && Data != DBNull.Value)
            {
                result = Data.ToString().Replace("/", "<br />");
            }
        }
        else if (type == 3)
        {
            //订单状态
            if (Data != null && Data != DBNull.Value && Data.ToString() != "")
            {
                result = GetDicName(int.Parse(Data.ToString()));
                result = result.Replace("，", ",").Replace(",", "<br />");
            }
        }
        else if (type == 4)
        {
            //起飞日期
            if (Data != null && Data != DBNull.Value && Data.ToString() != "")
            {
                result = Data.ToString().Replace("/", "<br />");
            }
        }
        else if (type == 5)
        {
            //订单取消显示和隐藏
            if (Data != null && Data != DBNull.Value && Data.ToString() != "")
            {
                //为”线下订单申请,等待处理“ ”新订单，等待支付“显示
                if (Data.ToString().Trim() == "1" || Data.ToString().Trim() == "27")
                {
                    result = "show";
                }
                else
                {
                    result = "hide";
                }
            }
        }
        return result;
    }
    /// <summary>
    /// 获取查询字符串
    /// </summary>
    /// <returns></returns>
    public string Query()
    {
        StringBuilder sbWhere = new StringBuilder();
        sbWhere.Append(string.Format(" OwnerCpyNo='{0}' and OrderSourceType=4 ", mCompany.UninCode));
        //订单号
        if (CommonManage.TrimSQL(txtOrderId.Text.Trim()) != "")
        {
            sbWhere.AppendFormat(" and  OrderId='{0}' ", CommonManage.TrimSQL(txtOrderId.Text.Trim()));
        }
        //航空公司
        if (SelectAirCode1.Value != "" && SelectAirCode1.Value != "0")
        {
            sbWhere.AppendFormat(" and  CarryCode like'%{0}%' ", CommonManage.TrimSQL(SelectAirCode1.Value.Trim()));
        }
        //航班号
        if (CommonManage.TrimSQL(txtFlightCode.Text.Trim()) != "")
        {
            sbWhere.AppendFormat(" and  FlightCode like'%{0}%' ", CommonManage.TrimSQL(txtFlightCode.Text.Trim()));
        }
        //编码
        if (CommonManage.TrimSQL(txtPNR.Text.Trim()) != "")
        {
            sbWhere.AppendFormat(" and  PNR='{0}' ", txtPNR.Text.Trim());
        }
        //出发城市
        if (Hid_fromCode.Value != "0" && Hid_fromCode.Value != "")
        {
            sbWhere.Append(string.Format(" and  TravelCode  like '%{0}%' ", Hid_fromCode.Value.Trim()));
        }
        //到达城市
        if (Hid_toCode.Value != "0" && Hid_toCode.Value != "")
        {
            sbWhere.Append(string.Format(" and  TravelCode  like '%{0}%' ", Hid_toCode.Value.Trim()));
        }
        //订单状态
        if (ddlStatus.SelectedValue != "" && ddlStatus.SelectedValue != "0")
        {
            sbWhere.Append(string.Format(" and  OrderStatusCode = '{0}' ", ddlStatus.SelectedValue.Trim()));
        }
        //乘机人姓名
        if (CommonManage.TrimSQL(txtPassengerName.Text.Trim()) != "")
        {
            sbWhere.AppendFormat(" and PassengerName like '%{0}%' ", txtPassengerName.Text.Trim().Replace("\'", ""));
        }
        //乘机日期
        if (CommonManage.TrimSQL(txtFromDate1.Value.Trim()) != "" && CommonManage.TrimSQL(txtFromDate2.Value.Trim()) != "")
        {
            sbWhere.Append(string.Format(" and  (AirTime>='{0} 00:00:00' and  AirTime<='{1} 23:59:59') ", CommonManage.TrimSQL(txtFromDate1.Value.Trim()), CommonManage.TrimSQL(txtFromDate2.Value.Trim())));
        }
        //创建日期
        if (CommonManage.TrimSQL(txtCreateTime1.Value.Trim()) != "" && CommonManage.TrimSQL(txtCreateTime2.Value.Trim()) != "")
        {
            sbWhere.Append(string.Format(" and  (CreateTime>='{0} 00:00:00' and  CreateTime<= '{1} 23:59:59') ", CommonManage.TrimSQL(txtCreateTime1.Value.Trim()), CommonManage.TrimSQL(txtCreateTime2.Value.Trim())));
        }
        return sbWhere.ToString();
    }
    /// <summary>
    /// 绑定订单状态
    /// </summary>
    public void BindOrderStatus()
    {
        if (dicList != null)
        {
            ddlStatus.Items.Clear();
            foreach (Bd_Base_Dictionary dicitem in dicList)
            {
                ddlStatus.Items.Add(new ListItem(dicitem.ChildName, dicitem.ChildID.ToString()));
            }
            ddlStatus.Items.Insert(0, new ListItem("--全部状态--", "0"));
        }
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
    /// 初始化页面数据
    /// </summary>
    public void InitData()
    {
        //登录账号
        Hid_LoginName.Value = mUser.LoginName;
        Hid_CpyNo.Value = mCompany.UninCode;
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        CommitQuery();
    }
    public void CommitQuery()
    {
        Curr = 1;
        Con = Query();
        PageDataBind();
    }


}