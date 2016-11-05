using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using System.Collections;
using System.Text;
using PbProject.WebCommon.Utility.Encoding;
public partial class Policy_AddDefaultPolicy : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            currentuserid.Value = mUser.id.ToString();
            GetDefaultPolicy();
            Hid_CpyNO.Value = mCompany.UninCode;
        }
    }
    /// <summary>
    /// 设置发布页面默认日期
    /// </summary>
    public void SetDefaultDate()
    {
        DateTime dt = DateTime.Now;
        DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);
        //设置日期默认值
        txtTicketStartDate.Value = dt.ToString("yyyy-MM-dd");
        txtTicketEndDate.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
        txtFlightStartDate.Value = dt.ToString("yyyy-MM-dd");
        txtFlightEndDate.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
    }

    public void GetDefaultPolicy()
    {
        string cpyNo = GetVal("cpyNo", "");
        string aircode = GetVal("aircode", "ALL");
        if (!string.IsNullOrEmpty(cpyNo))
        {
            string sqlWhere = string.Format("  CpyNo='{0}' and CarryCode like '%{1}%' and A1 in(1,2) ", cpyNo, aircode.Trim(new char[] { '/' }));
            List<Tb_Ticket_Policy> defaultList = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Policy>;
            if (defaultList != null && defaultList.Count > 0)
            {
                Tb_Ticket_Policy adult = defaultList.Find(delegate(Tb_Ticket_Policy tb_ticket_policy)
                {
                    return (tb_ticket_policy.A1 == 1 ? true : false);
                });
                Tb_Ticket_Policy child = defaultList.Find(delegate(Tb_Ticket_Policy tb_ticket_policy)
                {
                    return (tb_ticket_policy.A1 == 2 ? true : false);
                });
                if (adult != null)
                {
                    string cityData = JsonHelper.ObjToJson<Tb_Ticket_Policy>(adult);
                    Hid_AdultPolicy.Value = escape(cityData);
                    SelectAirCode1.Value = adult.CarryCode.Trim(new char[] { '/' });
                    Hid_AirCode.Value = adult.CarryCode.Trim(new char[] { '/' });
                }
                if (child != null)
                {
                    string cityData = JsonHelper.ObjToJson<Tb_Ticket_Policy>(child);
                    Hid_ChildPolicy.Value = escape(cityData);
                    SelectAirCode1.Value = child.CarryCode.Trim(new char[] { '/' });
                    Hid_AirCode.Value = child.CarryCode.Trim(new char[] { '/' });
                }
                string id = GetVal("id", "");
                Hid_IsEdit.Value = "1";
                Hid_id.Value = id;//列表中的数据id
                string currPage = GetVal("currPage", "1");
                Hid_currPage.Value = currPage;//来自列表第几页
                Hid_where.Value = Request["where"] != null && Request["where"].ToString() != "" ? Request["where"].ToString() : "";

                addAndNext.Value = "保存";
            }
            else
            {
                addAndNext.Value = "保存";
                Hid_IsEdit.Value = "0";
                SetDefaultDate();
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面初始化异常!');", true);
        }
    }

    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name">请求key名称</param>
    /// <param name="DefaultVal">数据默认值</param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null && Request[Name].ToString() != "")
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }

}