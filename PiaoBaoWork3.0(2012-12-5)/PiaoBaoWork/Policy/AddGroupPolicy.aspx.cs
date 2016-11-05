using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Text;
public partial class Policy_AddGroupPolicy : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindCity();
            BindClass("");
            //绑定起抵时间
            BindTime();
            //机型
            BindAircraft();
            if (Request["id"] != null && Request["id"].ToString() != "")
            {
                Hid_IsEdit.Value = "1";
                addAndNext.Text = "保存";
                Hid_id.Value = Request["id"].ToString();

                string currPage = GetVal("currPage", "1");
                Hid_currPage.Value = currPage;//来自列表第几页
                Hid_where.Value = Request["where"].ToString();

                InitData(Request["id"].ToString());
            }
        }
        Hid_CpyNo.Value = mCompany.UninCode;
        Hid_CpyName.Value = mCompany.UninAllName;
        Hid_UserName.Value = mUser.UserName;
        Hid_LoginName.Value = mUser.LoginName;
    }
    private List<Bd_Air_AirPort> list = null;
    /// <summary>
    /// 绑定城市
    /// </summary>
    public void BindCity()
    {
        string sqlWhere = " IsDomestic=1 ";
        List<Bd_Air_AirPort> defaultList = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { sqlWhere }) as List<Bd_Air_AirPort>;
        //出发城市
        ddlFromCity.DataSource = defaultList;
        ddlFromCity.DataFiledText = "CityCodeWord-CityName";
        ddlFromCity.DataFiledValue = "CityCodeWord";
        ddlFromCity.DataBind();
        //到达城市
        ddlToCity.DataSource = defaultList;
        ddlToCity.DataFiledText = "CityCodeWord-CityName";
        ddlToCity.DataFiledValue = "CityCodeWord";
        ddlToCity.DataBind();
    }
    //绑定仓位
    public void BindClass(string SelClass)
    {
        string strChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        List<char> strList = new List<char>();
        strList.AddRange(strChar.ToCharArray());
        ddlClass.DataSource = strList;
        ddlClass.DataBind();
    }
    //设置时间格式
    public void BindTime()
    {
        TimeCtrl1.SetTimeShow(true, true, false, true, true, false);

        DateTime dt = DateTime.Now;
        DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);

        txtStartDate.Value = dt.ToString("yyyy-MM-dd") + " 00:00";
        txtEndDate.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd") + " 00:00";


        txtTicketStartDate.Value = dt.ToString("yyyy-MM-dd");
        txtTicketEndDate.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// 绑定机型
    /// </summary>
    public void BindAircraft()
    {
        List<Bd_Air_Aircraft> defaultList = this.baseDataManage.CallMethod("Bd_Air_Aircraft", "GetList", null, new object[] { "" }) as List<Bd_Air_Aircraft>;
        ddlAircraft.DataSource = defaultList;
        ddlAircraft.DataFiledText = "Aircraft";
        ddlAircraft.DataFiledValue = "ABFeeN-ABFeeW";//国内和国外机建
        ddlAircraft.DataBind();
    }
    /// <summary>
    /// 获取所有城市信息
    /// </summary>
    /// <param name="IsDomestic"></param>
    /// <returns></returns>
    public List<Bd_Air_AirPort> GetCityList(string IsDomestic)
    {
        return this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=" + IsDomestic }) as List<Bd_Air_AirPort>;
    }
    /// <summary>
    /// 获取城市信息
    /// </summary>
    /// <param name="city"></param>
    /// <returns></returns>
    public Bd_Air_AirPort GetCity(string city)
    {
        if (list == null)
        {
            list = GetCityList("1");
        }
        Bd_Air_AirPort reModel = list.Find(delegate(Bd_Air_AirPort bd_air_airport)
         {
             if (bd_air_airport.CityName.ToUpper() == city.Trim().ToUpper() || bd_air_airport.CityCodeWord == city.Trim().ToUpper())
             {
                 return true;
             }
             else
             {
                 return false;
             }
         });
        return reModel;
    }

    /// <summary>
    /// 编辑数据
    /// </summary>
    /// <param name="id"></param>
    public void InitData(string id)
    {
        Tb_Ticket_UGroupPolicy tb_ticket_ugrouppolicy = baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "GetById", null, new object[] { id }) as Tb_Ticket_UGroupPolicy;
        if (tb_ticket_ugrouppolicy != null)
        {
            ddlAirCode.Value = tb_ticket_ugrouppolicy.AirCode;
            Hid_AirCode.Value = tb_ticket_ugrouppolicy.AirCode;

            ddlFromCity.Value = tb_ticket_ugrouppolicy.FromCityCode;
            Hid_fromCode.Value = tb_ticket_ugrouppolicy.FromCityCode;

            ddlToCity.Value = tb_ticket_ugrouppolicy.ToCityCode;
            Hid_toCode.Value = tb_ticket_ugrouppolicy.ToCityCode;

            txtFlightNo.Text = tb_ticket_ugrouppolicy.FlightNo;
            ddlClass.Value = tb_ticket_ugrouppolicy.Class;
            Hid_Seat.Value = tb_ticket_ugrouppolicy.Class;

            TimeCtrl1.Value = tb_ticket_ugrouppolicy.FlightTime;
            ddlAircraft.Text = tb_ticket_ugrouppolicy.PlaneType;
            Hid_PlaneType.Value = tb_ticket_ugrouppolicy.PlaneType;

            txtAircraftFare.Text = tb_ticket_ugrouppolicy.BuildPrice.ToString();
            txtRQFare.Text = tb_ticket_ugrouppolicy.OilPrice.ToString();

            Hid_PriceType.Value = tb_ticket_ugrouppolicy.PriceType ? "1" : "0";
            Hid_UPolicyType.Value = tb_ticket_ugrouppolicy.PolicyType.ToString();

            txtURebate.Text = tb_ticket_ugrouppolicy.Rebate.ToString();
            txtPrices.Text = tb_ticket_ugrouppolicy.Prices.ToString();
            txtDownRebate.Text = tb_ticket_ugrouppolicy.DownRebate.ToString();
            txtLaterRebate.Text = tb_ticket_ugrouppolicy.LaterRebate.ToString();
            txtShareRebate.Text = tb_ticket_ugrouppolicy.ShareRebate.ToString();
            txtAirRebate.Text = tb_ticket_ugrouppolicy.AirRebate.ToString();
            txtStartDate.Value = tb_ticket_ugrouppolicy.FlightStartDate.ToString("yyyy-MM-dd HH:mm");
            txtEndDate.Value = tb_ticket_ugrouppolicy.FlightEndDate.ToString("yyyy-MM-dd HH:mm");
            txtTicketStartDate.Value = tb_ticket_ugrouppolicy.PrintStartDate.ToString("yyyy-MM-dd");
            txtTicketEndDate.Value = tb_ticket_ugrouppolicy.PrintEndDate.ToString("yyyy-MM-dd");
            txtAdvanceDays.Text = tb_ticket_ugrouppolicy.AdvanceDays.ToString();
            txtSeatCount.Text = tb_ticket_ugrouppolicy.SeatCount.ToString();
            txtOfficeCode.Text = tb_ticket_ugrouppolicy.OfficeCode;
            txtUPolicyRemark.InnerText = tb_ticket_ugrouppolicy.Remarks;
        }
    }
    /*
    //添加
    protected void addAndNext_Click(object sender, EventArgs e)
    {
        AddAndEdit();
    }

    public void AddAndEdit()
    {
        if (mUser == null || mCompany == null)
        {
            return;
        }
        Tb_Ticket_UGroupPolicy tb_ticket_ugrouppolicy = null;
        if (Request["id"] != null && Request["id"].ToString() != "")
        {
            //编辑
            tb_ticket_ugrouppolicy = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "GetById", null, new object[] { Request["id"].ToString() }) as Tb_Ticket_UGroupPolicy;
        }
        else
        {
            //添加
            tb_ticket_ugrouppolicy = new Tb_Ticket_UGroupPolicy();
        }
        string errMsg = "";
        if (tb_ticket_ugrouppolicy != null)
        {
            if (txtOfficeCode.Text.Trim() == "")
            {
                errMsg = "输入出票Office不能为空";
            }
            tb_ticket_ugrouppolicy.CpyNo = mCompany.UninCode;
            tb_ticket_ugrouppolicy.CpyName = mCompany.UninAllName;
            tb_ticket_ugrouppolicy.CpyType = mCompany.RoleType;
            tb_ticket_ugrouppolicy.OperTime = System.DateTime.Now;
            tb_ticket_ugrouppolicy.OperLoginName = mUser.LoginName;
            tb_ticket_ugrouppolicy.OperUserName = mUser.UserName;
            //航空公司
            tb_ticket_ugrouppolicy.AirCode = ddlAirCode.Value;
            //出发城市
            string fromCode = ddlFromCity.Value;
            Bd_Air_AirPort CityInfo = GetCity(fromCode);
            tb_ticket_ugrouppolicy.FromCityCode = fromCode;
            tb_ticket_ugrouppolicy.FromCityName = CityInfo != null ? CityInfo.CityName : "";
            //到达城市
            string toCode = ddlToCity.Value;
            CityInfo = GetCity(toCode);
            tb_ticket_ugrouppolicy.ToCityCode = toCode;
            tb_ticket_ugrouppolicy.ToCityName = CityInfo != null ? CityInfo.CityName : "";
            //航班号
            tb_ticket_ugrouppolicy.FlightNo = txtFlightNo.Text.Trim();
            //舱位
            tb_ticket_ugrouppolicy.Class = ddlClass.Value;
            //起抵时间
            tb_ticket_ugrouppolicy.FlightTime = TimeCtrl1.Value;
            //机型
            tb_ticket_ugrouppolicy.PlaneType = ddlAircraft.Text;
            //政策类型
            tb_ticket_ugrouppolicy.PolicyType = int.Parse(Hid_UPolicyType.Value);
            //适用日期
            tb_ticket_ugrouppolicy.FlightStartDate = DateTime.Parse(txtStartDate.Value + ":00");
            tb_ticket_ugrouppolicy.FlightEndDate = DateTime.Parse(txtEndDate.Value + ":00");
            //出票日期
            tb_ticket_ugrouppolicy.PrintStartDate = DateTime.Parse(txtTicketStartDate.Value + " 00:00:00");
            tb_ticket_ugrouppolicy.PrintEndDate = DateTime.Parse(txtTicketStartDate.Value + " 23:59:59");
            //提前天数
            int AdvanceDays = 0;
            int.TryParse(txtAdvanceDays.Text.Trim(), out AdvanceDays);
            tb_ticket_ugrouppolicy.AdvanceDays = AdvanceDays;
            //座位数
            int SeatCount = 0;
            int.TryParse(txtSeatCount.Text.Trim(), out SeatCount);
            tb_ticket_ugrouppolicy.SeatCount = SeatCount;
            //优惠类型
            bool PriceType = false;
            if (Hid_PriceType.Value == "1")
            {
                PriceType = true;
                //价格
                decimal _Prices = 0m;
                if (!decimal.TryParse(txtPrices.Text.Trim(), out _Prices))
                {
                    errMsg = "输入价格必须为数字！";
                }
                if (txtPrices.Text.Trim() == "")
                {
                    errMsg = "输入价格不能为空";
                }
                tb_ticket_ugrouppolicy.Prices = _Prices;
            }
            else
            {
                PriceType = false;
                //折扣
                decimal _Rebate = 0m;
                if (!decimal.TryParse(txtURebate.Text.Trim(), out _Rebate))
                {
                    errMsg = "输入折扣必须为数字！";
                }
                if (txtURebate.Text.Trim() == "")
                {
                    errMsg = "输入折扣不能为空";
                }
                tb_ticket_ugrouppolicy.Rebate = _Rebate;
            }
            //
            tb_ticket_ugrouppolicy.PriceType = PriceType;
            //燃油
            decimal _OilPrice = 0m;
            if (!decimal.TryParse(txtRQFare.Text.Trim(), out _OilPrice))
            {
                errMsg = "输入燃油必须为数字！";
            }
            tb_ticket_ugrouppolicy.OilPrice = _OilPrice;
            //机建
            decimal _BuildPrice = 0m;
            if (!decimal.TryParse(txtAircraftFare.Text.Trim(), out _BuildPrice))
            {
                errMsg = "输入机建必须为数字！";
            }
            tb_ticket_ugrouppolicy.BuildPrice = _BuildPrice;
            //航空公司返点
            decimal _AirRebate = 0m;
            if (!decimal.TryParse(txtAirRebate.Text.Trim(), out _AirRebate))
            {
                errMsg = "输入航空公司返点必须为数字！";
            }
            tb_ticket_ugrouppolicy.AirRebate = _AirRebate;
            //下级返点
            decimal _DownRebate = 0m;
            if (!decimal.TryParse(txtDownRebate.Text.Trim(), out _DownRebate))
            {
                errMsg = "输入下级返点必须为数字！";
            }
            tb_ticket_ugrouppolicy.DownRebate = _DownRebate;
            //下级后返
            decimal _LaterRebate = 0m;
            if (!decimal.TryParse(txtLaterRebate.Text.Trim(), out _LaterRebate))
            {
                errMsg = "输入下级后返必须为数字！";
            }
            tb_ticket_ugrouppolicy.LaterRebate = _LaterRebate;
            //共享返点
            decimal _ShareRebate = 0m;
            if (!decimal.TryParse(txtShareRebate.Text.Trim(), out _ShareRebate))
            {
                errMsg = "输入共享返点必须为数字！";
            }
            tb_ticket_ugrouppolicy.ShareRebate = _ShareRebate;
            //出票Office号
            tb_ticket_ugrouppolicy.OfficeCode = txtOfficeCode.Text.Trim();
            //备注
            tb_ticket_ugrouppolicy.Remarks = txtUPolicyRemark.InnerText;

            if (txtStartDate.Value == "" || txtEndDate.Value == "" || txtTicketStartDate.Value == "" || txtTicketStartDate.Value == "")
            {
                errMsg = "输入日期不能为空";
            }
            if (txtSeatCount.Text.Trim() == "")
            {
                errMsg = "输入舱位个数不能为空";
            }
            if (txtRQFare.Text.Trim() == "")
            {
                errMsg = "输入燃油不能为空";
            }
            if (txtAircraftFare.Text.Trim() == "")
            {
                errMsg = "输入机建不能为空";
            }
            if (ddlAircraft.Value == "")
            {
                errMsg = "输入机型不能为空";
            }
            string[] strArr = TimeCtrl1.Value.Split('-');
            if (strArr.Length == 2)
            {
                if (strArr[0] == "00:00:00")
                {
                    errMsg = "请选择起飞时间！";
                }
                else if (strArr[1] == "00:00:00")
                {
                    errMsg = "请选择抵达时间！";
                }
            }
            if (TimeCtrl1.Value == "00:00:00-00:00:00")
            {
                errMsg = "请选择起飞抵达时间！";
            }
            if (ddlClass.Value == "")
            {
                errMsg = "输入舱位不能为空";
            }
            if (txtFlightNo.Text.Trim() == "")
            {
                errMsg = "输入航班号不能为空";
            }
            if (ddlFromCity.Value == ddlToCity.Value)
            {
                errMsg = "出发城市与到达城市不能一致！";
            }
            if (ddlToCity.Value == "")
            {
                errMsg = "到达城市不能为空";
            }
            if (ddlFromCity.Value == "")
            {
                errMsg = "出发城市不能为空";
            }
            if (ddlAirCode.Value == "")
            {
                errMsg = "航空公司不能为空";
            }
        }
        if (errMsg == "" && tb_ticket_ugrouppolicy != null)
        {
            if (Request["id"] != null && Request["id"].ToString() != "")
            {
                //更新
                bool UpdateSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "Update", null, new object[] { tb_ticket_ugrouppolicy });
                if (UpdateSuc)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('修改成功');", true);
                }
            }
            else
            {
                //添加
                bool InsertSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "Insert", null, new object[] { tb_ticket_ugrouppolicy });
                if (InsertSuc)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('添加成功');", true);
                }
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + errMsg + "');", true);
        }
    }
    */

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