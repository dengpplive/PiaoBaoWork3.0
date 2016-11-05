using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Model;
using System.Data;
using System.Data.SqlClient;
using DataBase.Data;
using PbProject.WebCommon.Utility;

public partial class Air_Order_HangOrderList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (mCompany.RoleType > 3)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您无权访问该页面',{op:1});", true);
            return;
        }
        if (!IsPostBack)
        {
            Curr = 1;
            Con = Query();
            AspNetPager1.PageSize = 20;
            this.txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            this.txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
            //绑定城市
            GetCity();
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
    /// 页面数据信息绑定
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
            List<Tb_Ticket_Order> OrderList = this.baseDataManage.CallMethod("Tb_Ticket_Order", "GetBasePager1", outParams,
                new object[] { TotalCount, AspNetPager1.PageSize, Curr, "dbo.GetPasSuppendStatus(OrderId,'###') SuppendStatus,*", Con, " CreateTime desc" }) as List<Tb_Ticket_Order>;

            TotalCount = outParams.GetValue<int>("1");
            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            if (OrderList.Count == 0)
            {
                Show.Visible = true;
                repList.Visible = false;
                AspNetPager1.Visible = false;
            }
            else
            {
                Show.Visible = false;
                repList.Visible = true;
                AspNetPager1.Visible = true;
            }
            repList.DataSource = OrderList;
            repList.DataBind();
        }
        catch
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面出错，请从新点击链接!');", true);
        }
    }

    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    List<Bd_Air_AirPort> cityList = null;
    public List<Bd_Air_AirPort> GetCity()
    {
        if (cityList == null)
        {
            string sqlWhere = " IsDomestic=1 ";
            cityList = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { sqlWhere }) as List<Bd_Air_AirPort>;
        }
        return cityList;
    }
    /// <summary>
    /// 查询条件拼接
    /// </summary>
    /// <returns>返回拼接好的字符串</returns>
    private string Query()
    {
        //本地和共享 已经出票
        StringBuilder sb = new StringBuilder();
        try
        {
            sb.Append(" policysource in(1,2,9)  and OrderStatusCode=4 ");
            if (mCompany.RoleType == 1)
            {
                sb.AppendFormat(" and left(OwnerCpyNo,6)='{0}'", mCompany.UninCode);
            }
            else
            {
                sb.AppendFormat(" and left(OwnerCpyNo,12)='{0}'", mCompany.UninCode);
            }
            if (CommonManage.TrimSQL(txtOrderId.Text.Trim()) != "")
            {
                sb.Append(" and OrderId like '%" + CommonManage.TrimSQL(txtOrderId.Text.Trim()) + "%'");
            }
            //航空公司
            if (SelectAirCode1.Value != "" && SelectAirCode1.Value != "0")
            {
                sb.Append(" and CarryCode = '" + CommonManage.TrimSQL(SelectAirCode1.Value) + "'");
            }
            //乘客
            if (CommonManage.TrimSQL(txtPassengerName.Text.Trim()) != "")
            {
                sb.Append(" and PassengerName like '%" + CommonManage.TrimSQL(txtPassengerName.Text.Trim()) + "%'");
            }
            //编码
            if (CommonManage.TrimSQL(txtPNR.Text.Trim()) != "")
            {
                sb.Append(" and PNR like '%" + CommonManage.TrimSQL(txtPNR.Text.Trim()) + "%'");
            }
            //订单生成时间
            if (CommonManage.TrimSQL(txtCreateTime1.Value.Trim()) != "")
            {
                sb.Append(" and CreateTime >= '" + CommonManage.TrimSQL(txtCreateTime1.Value.Trim()) + " 00:00:00'");
            }
            if (CommonManage.TrimSQL(txtCreateTime2.Value.Trim()) != "")
            {
                sb.Append(" and CreateTime <= '" + CommonManage.TrimSQL(txtCreateTime2.Value.Trim()) + " 23:59:59'");
            }
            //乘机时间
            if (CommonManage.TrimSQL(txtFromDate1.Value.Trim()) != "")
            {
                sb.Append(" and AirTime <= '" + CommonManage.TrimSQL(txtFromDate1.Value.Trim()) + " 00:00:00'");
            }
            if (CommonManage.TrimSQL(txtFromDate2.Value.Trim()) != "")
            {
                sb.Append(" and AirTime >= '" + CommonManage.TrimSQL(txtFromDate2.Value.Trim()) + " 23:59:59'");
            }
            //行程类型
            if (rbtlTravelType.SelectedValue != "0" && rbtlTravelType.SelectedValue != "")
            {
                sb.Append(" and TravelType = " + rbtlTravelType.SelectedValue.ToString());
            }
            if (txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
            {
                sb.Append(" and Travel like '" + txtFromCity.Value.Trim() + "%'");
            }
            if (txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
            {
                sb.Append(" and Travel like '%" + txtToCity.Value.Trim() + "'");
            }
            if (CommonManage.TrimSQL(txtFlightCode.Text.Trim()) != "")
            {
                sb.Append(" and FlightCode like '%" + CommonManage.TrimSQL(txtFlightCode.Text.Trim()) + "%'");
            }
        }
        catch
        {
            return sb.ToString();
        }
        return sb.ToString();
    }


    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;
        Con = Query();
        AspNetPager1.CurrentPageIndex = Curr;
        PageDataBind();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
        txtFromCity.Value = "";
        txtFromDate1.Value = "";
        txtFromDate2.Value = "";
        txtOrderId.Text = "";
        txtPassengerName.Text = "";
        txtPNR.Text = "";
        txtToCity.Value = "";
        rbtlTravelType.SelectedValue = "0";
        SelectAirCode1.Value = "0";
    }




    //----------------------------------------------------------------------

    /// <summary>
    /// 页面数据显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="objArr"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] objArr)
    {
        string result = "";
        if (type == 1)
        {
            //显示乘机人
            if (objArr != null && objArr.Length == 1)//程敏|0###程思思|0###李佳佳|0###
            {
                List<string> lstPas = new List<string>();
                string tempStr = "";
                string strPasStatus = objArr[0].ToString();
                string[] strPas = strPasStatus.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strPas.Length; i++)
                {
                    tempStr = "";
                    string[] strArr = strPas[i].Split('|');//乘客机票状态 格式：乘客姓名|机票状态
                    if (strArr.Length == 2)
                    {
                        if (strArr[1] == "1")//已挂
                        {
                            tempStr = strArr[0] + "&nbsp;&nbsp;<font class=\"red\">已挂</font>";
                        }
                        else
                        {
                            //未挂
                            tempStr = strArr[0] + "&nbsp;&nbsp;<font class=\"green\">未挂</font>";
                        }
                        lstPas.Add(tempStr);
                    }
                }
                result = string.Join("<br />", lstPas.ToArray());
            }
        }
        else if (type == 2)
        {
            //起飞日期
            if (objArr != null && objArr.Length == 1)
            {
                result = objArr[0].ToString().Trim().Replace("/", "<br />");
            }
        }
        else if (type == 4)
        {
            //航程
            if (objArr != null && objArr.Length == 1)
            {
                result = objArr[0].ToString().Trim().Replace("/", "<br />");
            }
        }
        return result;
    }


}