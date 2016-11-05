using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.Logic;
/// <summary>
/// 查询航班
/// </summary>
public partial class Buy_List : BasePage
{

    public string oneWayJson;
    public string connAndReturnJson;
    public int weeknum = 0;
    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //this.loading.Style.Value = "display: none;background: url(../images/" + mCompany.UninAllName + "/mainbg.gif) no-repeat;";
            if (!IsPostBack)
            {
               
                //隐藏政策
                hidePolicy.Value = mUser.UserPower.Contains("|2|").ToString();
                hidRoleType.Value = mCompany.RoleType.ToString();
                hidTime.Value = DateTime.Now.ToString("yyyy-MM-dd");
                PbProject.Model.definitionParam.BaseSwitch baseParams = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                Hid_GroupId.Value = mCompany.GroupId;//获取扣点组ID
                if (baseParams.KongZhiXiTong.Contains("|74|"))//开启共享航班开关
                {
                    this.divIsShowShare.Style["display"] = "block";
                }
                cbIsShowShare.Checked = true;//默认不显示共享
              
            }
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "Air_Buy_List_Page_Load");
        }
    }
    /// <summary>
    /// btnQuery_Click 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        try
        {
            string msg = "";
            string travelType = Hid_travel.Value.Trim();
            if (travelType == "1")
            {
                if (txtFromCityCode.Value.Trim() == "" || txtFromCityCode.Value.Trim() == "中文/英文")
                {
                    msg = "请输入出发城市！";
                }
                if (txtToCityCode.Value.Trim() == "" || txtToCityCode.Value.Trim() == "中文/英文")
                {
                    msg = "请输入到达城市！";
                }
                if (txtFromDate.Value.Trim() == "")
                {
                    msg = "请输入出发日期！";
                }
            }
            else if (travelType == "2")
            {
                if (txtFromCityCode.Value.Trim() == "" || txtFromCityCode.Value.Trim() == "中文/英文")
                {
                    msg = "请输入出发城市！";
                }
                if (txtToCityCode.Value.Trim() == "" || txtToCityCode.Value.Trim() == "中文/英文")
                {
                    msg = "请输入到达城市！";
                }
                //if (DateTime.Compare(DateTime.Parse(txtReturnDate.Value.Trim()), DateTime.Parse(txtFromDate.Value.Trim())) <= 0)
                //{
                //    msg = "回程日期必须大于出发日期！";
                //}
            }
            else if (travelType == "3")
            {
                if (txtFromCityCode.Value.Trim() == "" || txtFromCityCode.Value.Trim() == "中文/英文")
                {
                    msg = "请输入出发城市！";
                }
                if (txtMidCityCode.Value.Trim() == "" || txtMidCityCode.Value.Trim() == "中文/英文")
                {
                    msg = "请输入中转城市！";
                }
                if (txtToCityCode.Value.Trim() == "" || txtToCityCode.Value.Trim() == "中文/英文")
                {
                    msg = "请输入到达城市！";
                }
                //if (DateTime.Compare(DateTime.Parse(txtReturnDate.Value.Trim()), DateTime.Parse(txtFromDate.Value.Trim())) <= 0)
                //{
                //    msg = "中转日期必须大于出发日期！";
                //}

            }
            if (msg != "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('" + msg + "');", true);
            }
            else
            {
                ForValue();
                ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "setTravelRadio(" + ViewState["TripType"] + ");", true);
                AirQueryStr();
            }
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "Air_Buy_List_btnQuery_Click");
        }
    }
    /// <summary>
    /// 查询航班数据
    /// </summary>
    private void AirQueryStr()
    {
        string os = "";
        string osBack = "";
        try
        {

            int num = 0;

            string fromCityCode = hidFCityCode.Value.Trim();
            string fromCity = txtFromCityCode.Value == "" ? ViewState["FCity"].ToString() : txtFromCityCode.Value;
            string toCityCode = hidTCityCode.Value.Trim();
            string toCity = txtToCityCode.Value == "" ? ViewState["TCity"].ToString() : txtToCityCode.Value;
            string midCityCode = hidMCityCode.Value.Trim();
            string midCity = txtMidCityCode.Value.Trim();


            lblSky.Text = fromCityCode + fromCity + "-" + toCityCode + toCity;

            PbProject.Logic.Buy.AirQurey a = new PbProject.Logic.Buy.AirQurey();
            //PiaoBao.BLLLogic.Interface.AirQurey a = new PiaoBao.BLLLogic.Interface.AirQurey();

            //行程类型
            int travelType = int.Parse(Hid_travel.Value);
            if (travelType == 2)
            {
                //往返
                travelType = 3;
            }
            else if (travelType == 3)
            {
                //联程
                travelType = 4;
                //string mcode = midCityCode;
                //midCityCode = toCityCode;
                //toCityCode = mcode;
            }
            string begintime = DateTime.Now.ToString("mm:ss:fff");
            PbProject.Model.definitionParam.SelectCityParams selectCityParams = new PbProject.Model.definitionParam.SelectCityParams();
            selectCityParams.fcity = fromCityCode;
            selectCityParams.mcity = midCityCode;
            selectCityParams.tcity = toCityCode;
            selectCityParams.time = txtFromDate.Value;
            selectCityParams.Totime = txtReturnDate.Value;
            selectCityParams.cairry = ViewState["Carrier"].ToString();
            selectCityParams.TravelType = travelType;
            selectCityParams.num = num;
            selectCityParams.mEmployees = mUser;
            selectCityParams.mCompany = mCompany;
            selectCityParams.IsShowGX = cbIsShowShare.Checked;//true 不显示,false 显示

            //PiaoBao.BLLLogic.Interface.Statistics statistics = new PiaoBao.BLLLogic.Interface.Statistics();
            //statistics.AddStatisticsData(mCompany, mUser, 1);

            //os = a.Start(fromCityCode, midCityCode, toCityCode, txtFromDate.Value, txtReturnDate.Value, ViewState["Carrier"].ToString(), 1, travelType, ref num, mUser, mCompany, cbIsShowShare.Checked);
            if (travelType == 4)
            {
                selectCityParams.fcity = fromCityCode;
                selectCityParams.tcity = midCityCode;
            }
            os = a.Start(selectCityParams);
            if (travelType == 3 || travelType == 4)//往返联成的情况需要读取两次IBE数据
            {
                if (travelType==3)
                {
                    selectCityParams.fcity = toCityCode;
                    selectCityParams.tcity = fromCityCode;
                } 
                if (travelType==4)
                {
                    selectCityParams.fcity = midCityCode;
                    selectCityParams.tcity = toCityCode;
                }
          
                selectCityParams.time = txtReturnDate.Value;
                PbProject.Logic.Buy.AirQurey aa = new PbProject.Logic.Buy.AirQurey();
                osBack = aa.Start(selectCityParams);
            }
            //用于记录 处理航班查询的时间
            //OnErrorNew(0, "查询返回数据长度：" + os.Length + ",处理总共用时:" + begintime + "-" + DateTime.Now.ToString("mm:ss:fff"), "AirQueryStr 查询航班");

            //os = tif.OutString(mCompany, tra, int.Parse(ViewState["tratype"].ToString()), ref num, hiTarget.Value);
            lblnumt.Text = selectCityParams.num.ToString();
            DateBind();
            //ScriptManager.RegisterStartupScript(this, GetType(), "", "showts2(" + weeknum + ")", true);
            string script = "";
            string scriptBack = "";
            if (os == "")
            {
                script = "<script language=\"javascript\">flyType('" + Hid_travel.Value + "');</script>";
            }
            else
            {
                if (travelType == 1)
                {
                    script = "<script language=\"javascript\">flyType('" + Hid_travel.Value + "');getAirInfo('','showAirInfo');</script>";
                }
                if ((travelType == 3 || travelType == 4) && osBack!="")
                {
                    script = "<script language=\"javascript\">flyType('" + Hid_travel.Value + "');getAirInfoBack('','showAirInfo');</script>";
                    scriptBack = "<script language=\"javascript\">flyType('" + Hid_travel.Value + "');getAirInfoBack('','showAirInfoBack');</script>";
                }
            }
            this.h31.Style["display"] = "block";
            this.h32.Style["display"] = "none";
            oneWayJson = os;
            connAndReturnJson = osBack;
            showDiv.InnerHtml = script;
            showDivBack.InnerHtml = scriptBack;
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "AirQueryStr 查询航班");
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), ";showdialogmsg('查询失败！请重新查询')", true);
        }
    }
    /// <summary>
    /// 赋值  获取查询航班的数据
    /// </summary>
    private void ForValue()
    {
        //行程类型
        ViewState["TripType"] = Hid_travel.Value.Trim();
        //出发时间
        ViewState["BeginTime"] = txtFromDate.Value.Trim();
        //往返时间（中转时间）
        ViewState["ReturnTime"] = txtReturnDate.Value.Trim();
        //出发城市三字码
        ViewState["FCityCode"] = hidFCityCode.Value.Trim();
        //中转城市三字码
        ViewState["MCityCode"] = hidMCityCode.Value.Trim();
        //到达城市三字码
        ViewState["TCityCode"] = hidTCityCode.Value.Trim();
        //出发城市
        ViewState["FCity"] = txtFromCityCode.Value.Trim();
        //中转城市
        ViewState["MCity"] = txtMidCityCode.Value.Trim();
        //到达城市
        ViewState["TCity"] = txtToCityCode.Value.Trim();
        //承运人
        ViewState["Carrier"] = ddlCarrier.Value.Trim();
        //是否显示共享航班
        ViewState["IsShowShare"] = cbIsShowShare.Checked ? "true" : "false";
    }

    #region 最近7天查询

    /// <summary>
    /// DateBind
    /// </summary>
    private void DateBind()
    {

        lbl1.ForeColor = System.Drawing.Color.Black;
        lbl2.ForeColor = System.Drawing.Color.Black;
        lbl3.ForeColor = System.Drawing.Color.Black;
        lbl4.ForeColor = System.Drawing.Color.Black;
        lbl5.ForeColor = System.Drawing.Color.Black;
        lbl6.ForeColor = System.Drawing.Color.Black;
        lbl7.ForeColor = System.Drawing.Color.Black;

        //查询日期
        DateTime fromDt = DateTime.Parse(txtFromDate.Value);
        int fromWeek = (int)fromDt.DayOfWeek;
        int fromDay = fromDt.Day;//查询日期在查询日期中的第几天
        int fromMonth = fromDt.Month;

        //当天日期
        DateTime nowDt = DateTime.Now;
        int nowWeek = (int)nowDt.DayOfWeek;
        int nowDay = nowDt.Day; //当天在当月中的第几天
        int nowMonth = nowDt.Month;

        if (fromDay - nowDay >= 6 || fromMonth != nowMonth)
        {
            int val = 0;

            if (fromWeek == 0 || fromWeek == 7)
            {
                val = 7;
            }

            //以前
            lbl1.Text = "星期一";
            lbtn1.Text = fromDt.AddDays(1 - fromWeek - val).ToString("yyyy-MM-dd");

            lbl2.Text = "星期二";
            lbtn2.Text = fromDt.AddDays(2 - fromWeek - val).ToString("yyyy-MM-dd");

            lbl3.Text = "星期三";
            lbtn3.Text = fromDt.AddDays(3 - fromWeek - val).ToString("yyyy-MM-dd");

            lbl4.Text = "星期四";
            lbtn4.Text = fromDt.AddDays(4 - fromWeek - val).ToString("yyyy-MM-dd");

            lbl5.Text = "星期五";
            lbtn5.Text = fromDt.AddDays(5 - fromWeek - val).ToString("yyyy-MM-dd");

            lbl6.Text = "星期六";
            lbtn6.Text = fromDt.AddDays(6 - fromWeek - val).ToString("yyyy-MM-dd");

            lbl7.Text = "星期日";
            lbtn7.Text = fromDt.AddDays(7 - fromWeek - val).ToString("yyyy-MM-dd");

            if (fromWeek == 1)
            {
                lbl1.ForeColor = System.Drawing.Color.Red;
            }
            else if (fromWeek == 2)
            {
                lbl2.ForeColor = System.Drawing.Color.Red;
            }
            else if (fromWeek == 3)
            {
                lbl3.ForeColor = System.Drawing.Color.Red;
            }
            else if (fromWeek == 4)
            {
                lbl4.ForeColor = System.Drawing.Color.Red;
            }
            else if (fromWeek == 5)
            {
                lbl5.ForeColor = System.Drawing.Color.Red;
            }
            else if (fromWeek == 6)
            {
                lbl6.ForeColor = System.Drawing.Color.Red;
            }
            else if (fromWeek == 0 || fromWeek == 7)
            {
                lbl7.ForeColor = System.Drawing.Color.Red;
            }
        }
        else
        {
            //现在
            int week = (nowWeek + 0) % 7;
            lbl1.Text = getWeek(week);
            lbtn1.Text = nowDt.ToString("yyyy-MM-dd");
            if (week == fromWeek)
            {
                lbl1.ForeColor = System.Drawing.Color.Red;
            }

            week = (nowWeek + 1) % 7;
            lbl2.Text = getWeek(week);
            lbtn2.Text = nowDt.AddDays(1).ToString("yyyy-MM-dd");
            if (week == fromWeek)
            {
                lbl2.ForeColor = System.Drawing.Color.Red;
            }

            week = (nowWeek + 2) % 7;
            lbl3.Text = getWeek(week);
            lbtn3.Text = nowDt.AddDays(2).ToString("yyyy-MM-dd");
            if (week == fromWeek)
            {
                lbl3.ForeColor = System.Drawing.Color.Red;
            }

            week = (nowWeek + 3) % 7;
            lbl4.Text = getWeek(week);
            lbtn4.Text = nowDt.AddDays(3).ToString("yyyy-MM-dd");
            if (week == fromWeek)
            {
                lbl4.ForeColor = System.Drawing.Color.Red;
            }

            week = (nowWeek + 4) % 7;
            lbl5.Text = getWeek(week);
            lbtn5.Text = nowDt.AddDays(4).ToString("yyyy-MM-dd");
            if (week == fromWeek)
            {
                lbl5.ForeColor = System.Drawing.Color.Red;
            }

            week = (nowWeek + 5) % 7;
            lbl6.Text = getWeek(week);
            lbtn6.Text = nowDt.AddDays(5).ToString("yyyy-MM-dd");
            if (week == fromWeek)
            {
                lbl6.ForeColor = System.Drawing.Color.Red;
            }

            week = (nowWeek + 6) % 7;
            lbl7.Text = getWeek(week);
            lbtn7.Text = nowDt.AddDays(6).ToString("yyyy-MM-dd");
            if (week == fromWeek)
            {
                lbl7.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    /// <summary>
    /// 返回星期
    /// </summary>
    /// <param name="week"></param>
    /// <returns></returns>
    public string getWeek(int week)
    {
        if (week == 1)
        {
            return "星期一";
        }
        else if (week == 2)
        {
            return "星期二";
        }
        else if (week == 3)
        {
            return "星期三";
        }
        else if (week == 4)
        {
            return "星期四";
        }
        else if (week == 5)
        {
            return "星期五";
        }
        else if (week == 6)
        {
            return "星期六";
        }
        else if (week == 7 || week == 0)
        {
            return "星期日";
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// lbtn1_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn1_Click(object sender, EventArgs e)
    {
        ForValue();
        weeknum = 1;
        ViewState["BeginTime"] = lbtn1.Text;
        txtFromDate.Value = ViewState["BeginTime"].ToString();
        AirQueryStr();
    }
    /// <summary>
    /// lbtn2_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn2_Click(object sender, EventArgs e)
    {
        ForValue();
        weeknum = 2;
        ViewState["BeginTime"] = lbtn2.Text;
        txtFromDate.Value = ViewState["BeginTime"].ToString();
        AirQueryStr();
    }
    /// <summary>
    /// lbtn3_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn3_Click(object sender, EventArgs e)
    {
        ForValue();
        weeknum = 3;
        ViewState["BeginTime"] = lbtn3.Text;
        txtFromDate.Value = ViewState["BeginTime"].ToString();
        AirQueryStr();
    }
    /// <summary>
    /// lbtn4_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn4_Click(object sender, EventArgs e)
    {
        ForValue();
        weeknum = 4;
        ViewState["BeginTime"] = lbtn4.Text;
        txtFromDate.Value = ViewState["BeginTime"].ToString();
        AirQueryStr();
    }
    /// <summary>
    /// lbtn5_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn5_Click(object sender, EventArgs e)
    {
        ForValue();
        weeknum = 5;
        ViewState["BeginTime"] = lbtn5.Text;
        txtFromDate.Value = ViewState["BeginTime"].ToString();
        AirQueryStr();
    }
    /// <summary>
    /// lbtn6_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn6_Click(object sender, EventArgs e)
    {
        ForValue();
        weeknum = 6;
        ViewState["BeginTime"] = lbtn6.Text;
        txtFromDate.Value = ViewState["BeginTime"].ToString();
        AirQueryStr();
    }
    /// <summary>
    /// lbtn7_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn7_Click(object sender, EventArgs e)
    {
        ForValue();
        weeknum = 7;
        ViewState["BeginTime"] = lbtn7.Text;
        txtFromDate.Value = ViewState["BeginTime"].ToString();
        AirQueryStr();
    }
    #endregion
    protected void btnQueryTeam_Click(object sender, EventArgs e)
    {
        ForValue();
        string fromCityCode = hidFCityCode.Value.Trim();
        string fromCity = txtFromCityCode.Value == "" ? ViewState["FCity"].ToString() : txtFromCityCode.Value;
        string toCityCode = hidTCityCode.Value.Trim();
        string toCity = txtToCityCode.Value == "" ? ViewState["TCity"].ToString() : txtToCityCode.Value;
        //string midCityCode = hidMCityCode.Value.Trim();
        //string midCity = txtMidCityCode.Value.Trim();
        var plpu = new PbProject.Logic.Policy.UGroupPolicy();

        string os = PbProject.WebCommon.Utility.Encoding.JsonHelper.ObjToJson(plpu.getTb_Ticket_UGroupPolicy(ViewState["Carrier"].ToString(), fromCityCode, toCityCode, txtFromDate.Value));
        string script = "<script language=\"javascript\">getUGroupPolicyInfo('" + os + "','showGroupInfo');</script>";
        showDiv.InnerHtml = script;
    }
}