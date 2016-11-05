using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
using PbProject.WebCommon.Utility;
using PbProject.Logic.Order;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Data;

/// <summary>
/// 订单综合查询
/// </summary>
public partial class Order_OrderList : BasePage
{

    private List<Bd_Base_Dictionary> dicList = new List<Bd_Base_Dictionary>();

    /// <summary>
    /// 
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }

    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                this.currentuserid.Value = this.mUser.id.ToString();
                OrderStatusCodeBind();
                Curr = 1;
                AspNetPager1.PageSize = 20;
                ViewState["orderBy"] = " CreateTime desc ";

                //当前时间
                DateTime dt = DateTime.Now;

                //每月一号时间
                //DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);
                //txtCreateTime1.Value = dt1.AddMonths(-1).ToString("yyyy-MM-dd");

                //乘机日期
                //txtFromDate1.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
                //txtFromDate2.Value = dt.ToString("yyyy-MM-dd");

                //创建日期
                //txtCreateTime1.Value = dt.AddMonths(-1).ToString("yyyy-MM-dd");
                txtCreateTime1.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
                txtCreateTime2.Value = dt.ToString("yyyy-MM-dd");

                thCpyName.Visible = false;
                tdCpyName.Visible = false;
                if (mCompany.RoleType == 1)
                {
                    thCpyName.Visible = true;
                    tdCpyName.Visible = true;
                }

                Con = SelWhere(false);
                //初始化数据
                GetDictionary();

            }
            //绑定落地和供应列表
            BindLDGY();
        }
        catch (Exception) { }
    }


    public bool IsShow()
    {
        try
        {
            if (mCompany.RoleType == 1 || mCompany.RoleType == 2 || mCompany.RoleType == 3)
                return true;
        }
        catch (Exception)
        {

        }
        return false;
    }
    public string ZFZ(string a8)
    {
        string result = string.Empty;
        if (!string.IsNullOrEmpty(a8))
        {
            if (a8 == "0")
                result = "<font class='red'>非自愿</font>";
            else if (a8 == "1")
                result = "<font class='red'>自愿</font>";
        }
        return result;
    }
    /// <summary>
    /// 是否显示客户名称
    /// </summary>
    /// <returns></returns>
    public bool IsShow2()
    {
        try
        {
            //只有平台和落地运营商可以看到
            if (mCompany.RoleType == 1 || mCompany.RoleType == 2)
                return true;
        }
        catch (Exception)
        {

        }
        return false;
    }

    /// <summary>
    /// 是否显示协调
    /// </summary>
    /// <returns></returns>
    public bool IsShow3()
    {
        try
        {
            //只有平台和（落地运营或供应开了协调权限）能操作
            if (mCompany.RoleType == 1 || ((mCompany.RoleType == 2 || mCompany.RoleType == 3) && this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|104|")))
                return true;
        }
        catch (Exception)
        {

        }
        return false;
    }




    /// <summary>
    /// 是否隐藏客户名称
    /// </summary>
    /// <param name="CPCpyNo"></param>
    /// <param name="OrderId"></param>
    /// <param name="OwnerCpyName"></param>
    /// <param name="PolicySource"></param>
    /// <returns></returns>
    public string IsSelfCustomer(string CPCpyNo, string OrderId, string OwnerCpyName, string PolicySource)
    {
        string sb = "";

        try
        {

            if (mCompany.RoleType == 1)//平台
            {
                sb = "<a href='#' onclick=\"return GetUserInfo('" + OrderId + "')\">" + OwnerCpyName + "</a>";
            }
            else if (mCompany.RoleType == 2)
            {
                if (PolicySource == "9")
                {
                    if (mCompany.UninCode.Substring(0, 12) != CPCpyNo.Substring(0, 12))
                    {
                        sb = "<a href='#' onclick=\"return GetUserInfo('" + OrderId + "')\">" + OwnerCpyName + "</a>";
                    }
                    else
                    {
                        sb = "异地采购商";
                    }
                }
                else
                {
                    sb = "<a href='#' onclick=\"return GetUserInfo('" + OrderId + "')\">" + OwnerCpyName + "</a>";
                }
            }
        }
        catch (Exception)
        {

        }
        return sb;
    }
    /// <summary>
    /// 订单绑定
    /// </summary>
    private void OrderListDataBind()
    {
        try
        {
            int TotalCount = 0;

            IHashObject outParams = new HashObject();
            //指定参数类型 第一个参数为out输出类型
            //key 为参数索引从1开始 value为引用类型 out ref
            outParams.Add("1", "out");
            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            List<Tb_Ticket_Order> list = baseDataManage.CallMethod("Tb_Ticket_Order", "GetBasePager1", outParams,
                new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*,dbo.GetCpyName(CPCpyNo) NewCpCpyName ", Con, ViewState["orderBy"].ToString() }) as List<Tb_Ticket_Order>;

            TotalCount = outParams.GetValue<int>("1");

            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;

            repOrderList.DataSource = list;
            repOrderList.DataBind();
        }
        catch (Exception)
        {

        }
    }
    /// <summary>
    /// 初始化字典表
    /// </summary>
    public void GetDictionary()
    {
        try
        {
            if (dicList.Count == 0)
            {
                dicList = new Bd_Base_DictionaryBLL().GetList();
            }

            if (mCompany.RoleType == 1)
            {
                spanUnLock.Visible = true;
                spanTKZ.Visible = true;
            }
        }
        catch (Exception)
        {

        }
    }
    /// <summary>
    /// 获取字典表名称
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="childId"></param>
    /// <returns></returns>
    public string GetDicName(int parentId, string childId)
    {
        string result = "";
        GetDictionary();
        var query = from Bd_Base_Dictionary d in dicList
                    where d.ParentID == parentId && d.ChildID == int.Parse(childId)
                    select d;
        if (query.Count<Bd_Base_Dictionary>() > 0)
        {
            List<Bd_Base_Dictionary> list = query.ToList<Bd_Base_Dictionary>();
            result = list[0].ChildName;
        }
        return result;
    }

    public string ShowAdult(object AssociationOrder, object IsChdFlag)
    {
        string ass = AssociationOrder.ToString();
        if (IsChdFlag != null && !string.IsNullOrEmpty(ass))
        {
            bool temp = Convert.ToBoolean(IsChdFlag);
            //儿童订单
            if (temp)
                return string.Format("<br/>{0}(成人)", ass);
        }
        return string.Empty;
    }
    //页面显示数据
    public string ShowText(int opType, params object[] objData)
    {
        string strReData = "";
        if (opType == 0)
        {
            //创建时间
            if (objData != null && objData.Length == 1)
            {
                strReData = objData[0].ToString().Trim().Replace(" ", "<br />");
            }
        }
        else if (opType == 1)
        {
            //显示乘机人
            if (objData != null && objData.Length == 1)
            {
                strReData = objData[0].ToString().Replace("|", "<br />");
            }
        }
        else if (opType == 2)
        {
            //起飞日期
            if (objData != null && objData.Length == 1)
            {
                //strReData = objData.ToString().Trim().Replace("/", "<br />");
                //strReData = DateTime.Parse(objData.ToString()).ToString("yyyy-MM-dd HH:mm");

                strReData = DateTime.Parse(objData[0].ToString()).ToString("yyyy-MM-dd") + "<br/>" + DateTime.Parse(objData[0].ToString()).ToString("HH:mm"); ;
            }
        }
        else if (opType == 3)
        {
            //行程
            if (objData != null && objData.Length == 1)
            {
                if (objData[0].ToString().Trim() == "1")
                {
                    strReData = "单程";
                }
                else if (objData[0].ToString().Trim() == "2")
                {
                    strReData = "往返";
                }
                else if (objData[0].ToString().Trim() == "3")
                {
                    strReData = "联程";
                }
                else if (objData[0].ToString().Trim() == "4")
                {
                    strReData = "多程";
                }
            }
        }
        else if (opType == 4)
        {
            if (objData != null && objData.Length == 1)
            {
                //航程
                strReData = objData[0].ToString().Trim().Replace("/", "<br />");
            }
        }
        else if (opType == 5)//当日航班显示红色
        {
            //tdbg
            if (objData != null && objData.Length == 1)
            {
                DateTime dt = DateTime.Parse("1901-01-01");
                if (DateTime.TryParse(objData[0].ToString(), out dt))
                {
                    //乘机日期为当天
                    if (dt.ToString("yyyy-MM-dd") == System.DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        strReData = "class=\"tdbg\"";
                    }
                }
            }
        }
        else if (opType == 6)//当日航班显示红色
        {
            //起飞日期
            if (objData != null && objData.Length == 1)
            {
                //strReData = objData.ToString().Trim().Replace("/", "<br />");

                strReData = DateTime.Parse(objData[0].ToString()).ToString("yyyy-MM-dd") + "<br/>" +
                    DateTime.Parse(objData[0].ToString()).ToString("HH:mm:ss");
            }
        }
        else if (opType == 7)//订单号 外部订单号 儿童出成人票标记 升舱标记
        {
            if (objData != null && objData.Length == 5)
            {
                string Identity = objData[2].ToString();
                string IsCHDETAdultTK = objData[3].ToString();
                string OrderSourceType = objData[4].ToString();
                if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                {
                    strReData = objData[0] != null ? objData[0].ToString() : "";
                }
                else
                {
                    if (objData != null && !string.IsNullOrEmpty(objData[0].ToString()) && objData[1] != null && !string.IsNullOrEmpty(objData[1].ToString()))
                    {
                        strReData = objData[0].ToString() + "<br/><span style='color:red;'>" + objData[1].ToString() + "</span>";
                    }
                    else
                    {
                        strReData = objData[0] != null ? objData[0].ToString() : "";
                    }
                }
                //儿童出成人票标记
                if (Identity.ToLower() == "true" || Identity.ToLower() == "1")
                {
                    strReData += string.Format("<br /><font class='red'>{0}</font>", IsCHDETAdultTK == "1" ? "儿童(成人价)" : "儿童");
                }
                //升舱换开
                if (OrderSourceType == "10")
                {
                    strReData += "<br /><font class='red'>升舱换开</font>";
                }
            }
        }
        else if (opType == 8)//8出票公司
        {
            strReData = "hide";
            if (mCompany.RoleType == 1)
            {
                strReData = "show";
            }
        }
        else if (opType == 9)//9政策来源
        {
            if (objData != null && objData.Length == 5)
            {
                string PolicySource = objData[0] != null ? objData[0].ToString() : "";
                string CPCpyNo = objData[1] != null ? objData[1].ToString() : "";
                string PolicyType = objData[2] != null ? objData[2].ToString() : "";
                string AutoPrintFlag = objData[3] != null ? objData[3].ToString() : "";
                string PolicyId = objData[4] != null ? objData[4].ToString() : "";
                if (PolicySource == "9")
                {
                    if (CPCpyNo == mUser.CpyNo)
                    {
                        if (PolicyType == "1")
                        {
                            strReData = "本地B2B";
                        }
                        else if (PolicyType == "2")
                        {
                            strReData = "本地BSP";
                        }
                    }
                    else
                    {
                        if (PolicySource != "")
                        {
                            strReData = GetDictionaryName("24", PolicySource);
                        }
                    }
                }
                else
                {
                    strReData = GetDictionaryName("24", PolicySource);
                }
                //自动出票标识
                if (AutoPrintFlag != "" && AutoPrintFlag != "0")//（0=手动，1=半自动，2=全自动）
                {
                    if (AutoPrintFlag == "1")
                    {
                        strReData += "<br /><font class='red'>半自动出票" + (PolicyId.Contains("b2bpolicy") ? "(air)" : "") + "</font>";
                    }
                    else if (AutoPrintFlag == "2")
                    {
                        strReData += "<br /><font class='red'>全自动出票" + (PolicyId.Contains("b2bpolicy") ? "(air)" : "") + "</font>";
                    }
                }
            }
        }
        else if (opType == 10)//10订单状态 和支付方式
        {
            if (objData != null && objData.Length == 2)
            {
                string data = objData[0] != null ? objData[0].ToString() : "";
                int Identity = 0;
                if (int.TryParse(data, out Identity) && Identity != 0)
                {
                    string ChildId = objData[1] != null ? objData[1].ToString() : "";
                    if (ChildId != "")
                    {
                        strReData = GetDicName(Identity, ChildId);
                        strReData = string.Join("<br />", strReData.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
                        if (Identity == 1 && (ChildId == "3" || ChildId == "15"))
                        {
                            strReData = "<font class='red'>" + strReData + " </font>";
                        }
                    }
                }
            }

        }
        else if (opType == 11)//查看授权
        {
            strReData = "";
            if (objData != null && objData.Length == 3)
            {
                //：1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享,10易行
                string PolicySource = objData[0] != null ? objData[0].ToString() : "";
                string PrintOffice = objData[1] != null ? objData[1].ToString() : "";
                string OrderSourceType = objData[2] != null ? objData[2].ToString() : "";
                //不是PNR内容导入 不是本地B2B和BSP PrintOffice不为空 mCompany.RoleType < 3 &&
                if (PolicySource != "1" && PolicySource != "2" && PrintOffice != "" && !"379".Contains(OrderSourceType))
                {
                    strReData = "<br /><a href=\"#\" onclick=\"return GetAuth('" + PrintOffice + "');\">查看授权</a>";
                }
            }
        }
        return strReData;
    }

    /// <summary>
    /// 绑定落地供应商信息
    /// </summary>
    public void BindLDGY()
    {
        DataTable table = this.baseDataManage.GetGYEmpolyees();
        ddlGYList.DataTableSource = table;
        ddlGYList.DataFiledText = "LoginName-UninAllName";
        ddlGYList.DataFiledValue = "CpyNo";
        ddlGYList.DataBind();
    }
    public string FlyCode(string CanyCode, string FlightCode)
    {
        string fullCode = string.Empty;
        string returnCode = string.Empty;
        if (!string.IsNullOrEmpty(CanyCode))
            returnCode = CanyCode.Split('/')[0].ToString();
        if (!string.IsNullOrEmpty(FlightCode))
        {
            FlightCode.Split('/').ToList().ForEach(x =>
            {
                fullCode += string.Format("{0}{1}<br/>", returnCode, x);
            });
        }
        return fullCode;
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string SelWhere(bool IsTKZ)
    {
        StringBuilder StrWhere = new StringBuilder(" A1=1 ");

        if (mCompany.RoleType == 1)
        {
            if (ddlGYList.Value != "")
            {
                StrWhere.AppendFormat(" and (CPCpyNo='{0}' or left(OwnerCpyNo,12)= '{0}' ) ", ddlGYList.Value); //可查询共享
            }
        }
        else if (mCompany.RoleType == 2)
        {
            StrWhere.Append(" and (CPCpyNo='" + mUser.CpyNo + "' or left(OwnerCpyNo,12)= '" + mUser.CpyNo + "' ) "); //可查询共享
        }
        else if (mCompany.RoleType == 3)
        {
            StrWhere.Append(" and CPCpyNo='" + mUser.CpyNo + "' ");
        }
        else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
        {
            StrWhere.Append(" and OwnerCpyNo='" + mUser.CpyNo + "' ");
        }
        try
        {
            //订单号或者票号
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtOrderId.Text.Trim())))
            {
                string OrderIdAndTicketNumber = CommonManage.TrimSQL(txtOrderId.Text.Replace("-", "").Trim());
                //string pattern = @"^\d{3,4}(\-?|\s+)\d{10}$";
                //if (Regex.Match(OrderIdAndTicketNumber, pattern, RegexOptions.IgnoreCase).Success)
                //{
                //票号               
                //StrWhere.AppendFormat("  and  dbo.GetTicketNumber(OrderId) like '%|{0}|%' ", OrderIdAndTicketNumber);

                //Kevin 2013-06-05 Edit 解决条件查询慢的问题
                //StrWhere.AppendFormat(" and (OrderId in(select distinct OrderId from Tb_Ticket_Passenger where TicketNumber like '%{0}%') or OrderId='{0}') ", OrderIdAndTicketNumber);
                //}
                //else
                //{
                //    //订单号
                //    StrWhere.AppendFormat(" and OrderId='{0}' ", OrderIdAndTicketNumber);
                //}

                //解决条件查询慢的问题 2013-6-14                
                if (OrderIdAndTicketNumber.Length == 19)
                {
                    StrWhere.AppendFormat(" and OrderId='{0}' ", OrderIdAndTicketNumber);
                }
                else if (OrderIdAndTicketNumber.Length == 13 || OrderIdAndTicketNumber.Length == 14 || OrderIdAndTicketNumber.Length == 10)
                {
                    StrWhere.AppendFormat(" and OrderId in(select distinct OrderId from Tb_Ticket_Passenger where replace(TicketNumber,'-','') ='{0}') ", OrderIdAndTicketNumber);
                }
                else
                {
                    StrWhere.AppendFormat(" and OrderId like '%{0}%' ", OrderIdAndTicketNumber);
                }
            }
            //pnr
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtPNR.Text.Trim())))
                StrWhere.Append(" and PNR='" + CommonManage.TrimSQL(txtPNR.Text.Trim()) + "' ");
            //乘机人
            if (!string.IsNullOrEmpty(txtPassengerName.Text.Trim()))
                StrWhere.Append(" and PassengerName like'%" + CommonManage.TrimSQL(txtPassengerName.Text.Trim()) + "%' ");
            //客户名称
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtCorporationName.Text.Trim())))
                StrWhere.Append(" and OwnerCpyName like'%" + CommonManage.TrimSQL(txtCorporationName.Text.Trim()) + "%' ");
            //航班号
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtFlightCode.Text.Trim())))
                StrWhere.Append(" and FlightCode ='" + CommonManage.TrimSQL(txtFlightCode.Text.Trim()) + "' ");
            //航空公司
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(SelectAirCode1.Value.Trim())))
                StrWhere.Append(" and CarryCode ='" + CommonManage.TrimSQL(SelectAirCode1.Value.Trim()) + "' ");

            if (IsTKZ)
            {
                StrWhere.Append(" and OrderStatusCode in(20,21,22,23) ");
            }
            else
            {
                //订单状态
                if (ddlStatus.SelectedValue != "" && ddlStatus.SelectedValue != "0")
                    StrWhere.Append(" and OrderStatusCode= " + CommonManage.TrimSQL(ddlStatus.SelectedValue));
            }

            //乘机日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtFromDate1.Value.Trim())))
                StrWhere.Append(" and AirTime >'" + CommonManage.TrimSQL(txtFromDate1.Value.Trim()) + " 00:00:00'");
            //乘机日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtFromDate2.Value.Trim())))
                StrWhere.Append(" and AirTime <'" + CommonManage.TrimSQL(txtFromDate2.Value.Trim()) + " 23:59:59'");

            //创建日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtCreateTime1.Value.Trim())))
                StrWhere.Append(" and CreateTime >'" + CommonManage.TrimSQL(txtCreateTime1.Value.Trim()) + " 00:00:00'");
            //创建日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtCreateTime2.Value.Trim())))
                StrWhere.Append(" and CreateTime <'" + CommonManage.TrimSQL(txtCreateTime2.Value.Trim()) + " 23:59:59'");

            ////城市控件
            //if (txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
            //    StrWhere.Append(" and Travel like '" + CommonManage.TrimSQL(txtFromCity.Value.Trim()) + "%'");
            //if (txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
            //    StrWhere.Append(" and Travel like '%" + CommonManage.TrimSQL(txtToCity.Value.Trim()) + "'");

            ////城市控件
            if (hidFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
                StrWhere.Append(" and TravelCode like '" + CommonManage.TrimSQL(hidFromCity.Value.Trim()) + "%'");
            if (hidToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
                StrWhere.Append(" and TravelCode like '%" + CommonManage.TrimSQL(hidToCity.Value.Trim()) + "'");

        }
        catch (Exception)
        {

        }
        return StrWhere.ToString();

    }

    #region 绑定事件

    /// <summary>
    /// 订单状态
    /// </summary>
    private void OrderStatusCodeBind()
    {
        try
        {
            // List<Bd_Base_Dictionary> bDictionaryList = GetDictionaryList("1");

            if (dicList.Count == 0)
            {
                GetDictionary();
            }

            List<Bd_Base_Dictionary> bDictionaryList = (from Bd_Base_Dictionary d in dicList
                                                        where d.ParentID == 1
                                                        orderby d.ChildID
                                                        select d).ToList<Bd_Base_Dictionary>();


            if (bDictionaryList != null && bDictionaryList.Count > 0)
            {
                //ViewState["DictionaryList"] = bDictionaryList;

                ddlStatus.DataSource = bDictionaryList;
                ddlStatus.DataTextField = "ChildName";
                ddlStatus.DataValueField = "ChildID";
                ddlStatus.DataBind();
            }

            ddlStatus.Items.Insert(0, new ListItem("全部状态", "0"));
            ddlStatus.Items[0].Selected = true;
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        try
        {
            Curr = e.NewPageIndex;
            OrderListDataBind();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// repCabinList_ItemCommand
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repOrderList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            string Id = e.CommandArgument.ToString();
            string msg = "";
            string result = "";

            if (e.CommandName == "PaySel")//支付状态查询
            {
                #region 支付状态查询

                for (int i = 1; i <= 4; i++)
                {
                    result = new PbProject.Logic.Pay.OperOnline().PaySel(i.ToString(), Id, true, out msg);
                    if (msg.Contains("交易成功"))
                        break;
                }
                if (msg.Contains("交易成功"))
                    OrderListDataBind();

                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
                #endregion
            }
            else if (e.CommandName == "RefundSel")//退款查询
            {
                #region 退款查询

                List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
                Tb_Ticket_Order Order = null; //订单

                if (mOrderList != null && mOrderList.Count > 0)
                    Order = mOrderList[0];
                if (Order != null && Order.PayWay > 0 && Order.PayWay < 13 &&
                    (Order.OrderStatusCode == 5 || Order.OrderStatusCode == 16 || Order.OrderStatusCode == 17 ||
                    Order.OrderStatusCode == 20 || Order.OrderStatusCode == 21 || Order.OrderStatusCode == 22 || Order.OrderStatusCode == 23))
                {
                    result = new PbProject.Logic.Pay.OperOnline().RefundSel(Order, out msg);
                }
                else
                {
                    msg = "订单状态有误，不能退款查询！";
                }

                result = "<textarea style='width:760px; height:360px; background-color:Black; color:Green;'>" + result + "</textarea>";
                string strValue = escape("<span style='color:Red; font-size:18px;'><b>" + msg + "</b></span>" + result);

                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialogNew('" + strValue + "');", true);
                #endregion
            }
            else if (e.CommandName == "Refund")//退款处理
            {
                #region 退款处理

                List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
                Tb_Ticket_Order Order = null; //订单

                if (mOrderList != null && mOrderList.Count > 0)
                    Order = mOrderList[0];
                if (Order != null && (Order.OrderStatusCode == 20 || Order.OrderStatusCode == 21 || Order.OrderStatusCode == 22 || Order.OrderStatusCode == 23))
                {
                    #region 拒绝出票退款处理

                    bool rs = new PbProject.Logic.Pay.OperOnline().TitckOrderRefund(Order, mUser, mCompany, out msg);

                    if (rs)
                    {
                        msg = "";
                    }
                    else
                    {
                        msg = "退款处理失败！" + msg;
                    }
                    #endregion
                }
                else
                {
                    msg = "订单状态有误，不能退款处理！";
                }

                if (msg == "")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "TitckOrderRefund();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
                }

                #endregion
            }
            else if (e.CommandName == "Detail")//订单详情
            {

                Response.Redirect("OrderDetail.aspx?Id=" + Id + "&Url=OrderList.aspx&currentuserid=" + this.currentuserid.Value.ToString());
            }
            else if (e.CommandName == "UpdateOrder") //修改订单
            {
                Response.Redirect("OrderUpdate.aspx?Id=" + Id + "&Url=OrderList.aspx&currentuserid=" + this.currentuserid.Value.ToString());
            }
            else if (e.CommandName == "unLock")//解锁
            {
                Tb_Ticket_OrderBLL service = new Tb_Ticket_OrderBLL();
                if (service.OrderUnLock(Id))
                {
                    msg = "此订单已解锁成功!";
                    OrderListDataBind();
                }
                else
                {
                    msg = "解锁失败，请稍后再试!";
                }
                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
            }
        }
        catch (Exception)
        {

        }
    }


    /// <summary>
    /// repOrderList_ItemDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void repOrderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemIndex != -1)
            {
                LinkButton lbtnPaySel = e.Item.FindControl("lbtnPaySel") as LinkButton; //支付状态查询
                LinkButton lbtnRefundSel = e.Item.FindControl("lbtnRefundSel") as LinkButton;//退款查询
                LinkButton lbtnRefund = e.Item.FindControl("lbtnRefund") as LinkButton;//退款处理
                LinkButton lbtnUpdateOrder = e.Item.FindControl("lbtnUpdateOrder") as LinkButton;//修改订单信息
                LinkButton lbtnUnLock = e.Item.FindControl("lbtnUnLock") as LinkButton;//解锁

                string orderStatusCode = (e.Item.FindControl("Hid_OrderStatusCode") as HiddenField).Value;//订单状态
                int payWay = int.Parse((e.Item.FindControl("Hid_PayWay") as HiddenField).Value);//支付方式
                string lockName = (e.Item.FindControl("Hid_Lock") as HiddenField).Value;//锁定人

                if (mCompany.RoleType == 1)
                {
                    lbtnUpdateOrder.Visible = true;//修改订单信息

                    // 支付状态查询
                    if (orderStatusCode == "1" || orderStatusCode == "9")
                        lbtnPaySel.Visible = true;

                    //退款查询
                    if ((payWay > 0 && payWay < 13) && (orderStatusCode == "5" || orderStatusCode == "6" || orderStatusCode == "7" ||
                        orderStatusCode == "20" || orderStatusCode == "21" || orderStatusCode == "22" || orderStatusCode == "23"
                        || orderStatusCode == "16" || orderStatusCode == "17" || orderStatusCode == "24"))
                        lbtnRefundSel.Visible = true;

                    //退款处理 ： 注意 只能是 取消出票 退款中的
                    if ((orderStatusCode == "20" || orderStatusCode == "21" || orderStatusCode == "22" || orderStatusCode == "23"))
                        lbtnRefund.Visible = true;
                }
                else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
                {
                    if (!string.IsNullOrEmpty(lockName))
                    {
                        lbtnUnLock.Visible = true;
                    }
                }
                #region 显示、隐藏

                //支付状态查询
                if (lbtnPaySel.Visible == true)
                    lbtnPaySel.Text += "<br/>";
                //退款查询
                if (lbtnRefundSel.Visible == true)
                    lbtnRefundSel.Text += "<br/>";
                //退款处理
                if (lbtnRefund.Visible == true)
                    lbtnRefund.Text += "<br/>";
                //修改订单信息
                if (lbtnUpdateOrder.Visible == true)
                    lbtnUpdateOrder.Text += "<br/>";
                if (lbtnUnLock.Visible == true)
                    lbtnUnLock.Text += "<br/>";

                ////订单详情
                //LinkButton lbtnDetail = e.Item.FindControl("lbtnDetail") as LinkButton;
                //if (lbtnDetail.Visible == true)
                //    lbtnDetail.Text += "<br/>";

                #endregion
            }
        }
        catch (Exception)
        {

        }
    }

    #endregion

    #region 按钮事件

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        try
        {
            Curr = 1;
            AspNetPager1.CurrentPageIndex = Curr;
            Con = SelWhere(false);
            OrderListDataBind();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtOrderId.Text = "";
        txtPNR.Text = "";
        txtFromCity.Value = "";
        txtToCity.Value = "";
        txtPassengerName.Text = "";
        txtCorporationName.Text = "";
        txtFlightCode.Text = "";
        txtFromDate1.Value = "";
        txtFromDate2.Value = "";
        SelectAirCode1.Value = "";
        ddlStatus.SelectedValue = "0";
        //当前时间
        DateTime dt = DateTime.Now;
        //每月一号时间
        DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);

        //创建日期
        txtCreateTime1.Value = dt1.AddMonths(-1).ToString("yyyy-MM-dd");
        txtCreateTime2.Value = dt.ToString("yyyy-MM-dd");

    }

    /// <summary>
    /// 获取选择的订单 格式: id##锁定账号
    /// </summary>
    /// <returns></returns>
    public List<string> GetSelId()
    {
        List<string> Idslist = new List<string>();
        string[] SelOrderIdArr = Hid_SelOrderId.Value.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
        if (SelOrderIdArr.Length > 0)
        {
            Idslist.AddRange(SelOrderIdArr);
        }
        return Idslist;
    }

    /// <summary>
    /// 订单锁定/解锁
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUnLock_Click(object sender, EventArgs e)
    {
        string ErrMsg = "";
        Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
        List<string> ids = GetSelId();
        List<string> LockIds = new List<string>();
        List<string> UnLockIds = new List<string>();
        foreach (string item in ids)
        {
            string[] strArr = item.Split(new string[] { "##" }, StringSplitOptions.None);
            if (strArr.Length == 2 && strArr[1].Trim() == "")
            {
                //锁定
                LockIds.Add(strArr[0]);
            }
            else
            {
                //解锁
                UnLockIds.Add(strArr[0]);
            }
        }

        bool reuslt = false;

        if (LockIds.Count > 0)
            reuslt = OrderManage.PatchLockOrder(true, LockIds, mUser.LoginName, mCompany.UninCode, out ErrMsg);

        if (UnLockIds.Count > 0)
            reuslt = OrderManage.PatchLockOrder(false, UnLockIds, mUser.LoginName, mCompany.UninCode, out ErrMsg);

        if (reuslt)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('锁定/解锁成功！',{type:0})", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('锁定/解锁失败！')", true);
        }
    }

    #endregion

    #region 数据处理

    /// <summary>
    /// 显示金额
    /// </summary>
    /// <param name="payMoney"></param>
    /// <param name="orderMoney"></param>
    /// <returns></returns>
    public string GetPrice(object payMoney, object orderMoney, object ticketStatus)
    {
        string Message = "";
        try
        {
            if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
            {
                Message = orderMoney != null ? orderMoney.ToString() : "0.00";
            }
            else
            {
                Message = payMoney != null ? payMoney.ToString() : "0.00";
            }

            if (Message != "0.00" && ticketStatus != null && (ticketStatus.ToString() == "3" || ticketStatus.ToString() == "4"))
            {
                Message = "-" + Message;
            }

            Message = "<span style='color:Red;'>" + Message + "</span>";
        }
        catch (Exception ex)
        {

        }
        return Message;
    }

    #endregion

    /// <summary>
    /// 退款中的订单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnTKZ_Click(object sender, EventArgs e)
    {
        try
        {
            Curr = 1;
            AspNetPager1.CurrentPageIndex = Curr;
            Con = SelWhere(true);
            OrderListDataBind();
        }
        catch (Exception)
        {

        }
    }
}