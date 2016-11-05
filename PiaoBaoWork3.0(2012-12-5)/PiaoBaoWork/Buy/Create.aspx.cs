using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Text;
using PnrAnalysis;
using PnrAnalysis.Model;
using PbProject.WebCommon.Utility;
using PbProject.Logic.Order;
using PbProject.Logic.Pay;
using PbProject.WebCommon.Utility.Encoding;
public partial class Buy_Create : BasePage
{
    #region 全局变量
    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
            }
            return result;
        }
    }
    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
            }
            return result;
        }
    }
    /// <summary>
    /// 常旅客列表
    /// </summary>
    private List<User_Flyer> flyList = new List<User_Flyer>();
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        string ErrMsg = "";
        LoadScript();
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            bool IsSuc = ConfigIsSet(out ErrMsg);
            if (IsSuc)
            {
                //绑定乘客类型
                BindPasType();
                //绑定证件类型
                BindCardType();
                //基本信息
                BindBaseInfo();
                //初始化参数
                FlightQueryParam FQP = InitValue();
                //显示航段信息
                ShowSkyWayInfo(FQP);
                //查询常旅客
                GetFlyUser();

                Hid_BigNum.Value = "9";
                //登录用户角色
                Hid_RoleType.Value = mCompany.RoleType.ToString();
                Hid_CurrTime.Value = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //关闭生成订单联系人默认值   生成订单时，联系人不需要默认值，让用户自己填写  
                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|55|"))
                {
                    txtLinkName.ReadOnly = false;
                    txtMobile.ReadOnly = false;
                }
                else
                {
                    txtLinkName.ReadOnly = true;
                    txtMobile.ReadOnly = true;
                }
                //关闭查询常旅客
                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|57|"))
                {
                    span_Flyer.Visible = false;
                }
                else
                {
                    span_Flyer.Visible = true;
                }
                //开启儿童编码必须关联成人编码或者成人订单号
                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|95|"))//开启儿童编码必须关联成人编码或者成人订单号
                {
                    Hid_CHDOPENAsAdultOrder.Value = "1";
                }
                else
                {
                    Hid_CHDOPENAsAdultOrder.Value = "0";
                }
                //开启儿童出成人票
                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|96|"))
                {
                    CHDToAdult.Visible = true;
                }
                else
                {
                    CHDToAdult.Visible = false;
                }
                //是否开启了抢票功能 
                if (KongZhiXiTong.Contains("|100|"))
                {
                    _IsRobTicketOrder.Visible = true;
                }
                else
                {
                    _IsRobTicketOrder.Visible = false;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), System.DateTime.Now.Ticks.ToString(), "showdialog('" + escape(ErrMsg) + "',2);", true);
            }
        }
    }
    /// <summary>
    /// 时时加载js
    /// </summary>
    public void LoadScript()
    {
        Script.Text = "<script type=\"text/javascript\" src=\"../js/js_Create.js?v=" + new Random().Next(0, 100000) + "\"></script>" +
        "<script type=\"text/javascript\" src=\"../js/js_CardValid.js?v=" + new Random().Next(0, 100000) + "\"></script>";
    }

    /// <summary>
    /// 查询该用户的常旅客
    /// </summary>
    public void GetFlyUser()
    {
        if (flyList.Count == 0)
        {
            string sqlWhere = string.Format(" CpyNo='{0}' ", mCompany.UninCode);
            //是否存在
            flyList = this.baseDataManage.CallMethod("User_Flyer", "GetList", null, new object[] { sqlWhere }) as List<User_Flyer>;
            if (flyList != null && flyList.Count > 0)
            {
                //用于页面输入查找
                Hid_FlyerList.Value = escape(JsonHelper.ObjToJson<List<User_Flyer>>(flyList));
            }
        }
    }
    /// <summary>
    /// 绑定证件号类型
    /// </summary>
    public void BindCardType()
    {
        string sqlWhere = " parentid=7 order by ChildID";
        List<Bd_Base_Dictionary> PasTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        if (PasTypeList != null && PasTypeList.Count > 0)
        {
            List<string> lstArr = new List<string>();
            foreach (Bd_Base_Dictionary item in PasTypeList)
            {
                ListItem lim = new ListItem();
                lim.Text = item.ChildName;
                lim.Value = item.ChildID.ToString();
                if (item.ChildName != "出生日期")
                {
                    SelCardType_0.Items.Add(lim);
                }
                lstArr.Add(item.ChildID.ToString() + "###" + item.ChildName.Replace("###", "").Replace("|", ""));
            }
            //证件类型字符串
            Hid_CardType.Value = escape(string.Join("|", lstArr.ToArray()));
        }
    }
    /// <summary>
    /// 绑定乘机人类型
    /// </summary>
    public void BindPasType()
    {
        string sqlWhere = " parentid=6  order by ChildID";
        List<Bd_Base_Dictionary> PasTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        if (PasTypeList != null && PasTypeList.Count > 0)
        {
            foreach (Bd_Base_Dictionary item in PasTypeList)
            {
                ListItem lim = new ListItem();
                lim.Text = item.ChildName;
                lim.Value = item.ChildID.ToString();
                if (item.ChildName.Contains("儿童"))//关闭预定儿童票
                {
                    if (GongYingKongZhiFenXiao == null || !GongYingKongZhiFenXiao.Contains("|90|"))
                    {
                        SelPasType_0.Items.Add(lim);
                    }
                }
                else
                {
                    SelPasType_0.Items.Add(lim);
                }
            }
        }
    }

    /// <summary>
    /// 绑定基本信息
    /// </summary>
    public void BindBaseInfo()
    {
        Hid_LoginAccount.Value = mUser != null ? mUser.LoginName : "";
        Hid_LoginID.Value = mUser != null ? mUser.id.ToString() : "";
        txtLinkName.Text = mCompany != null ? mCompany.ContactUser : "";
        txtMobile.Text = mCompany != null ? mCompany.ContactTel : "";
    }


    /// <summary>
    /// 初始化预订传过来的数据赋值
    /// </summary>
    private FlightQueryParam InitValue()
    {
        FlightQueryParam FQP = new FlightQueryParam();
        try
        {
            //承运人
            ViewState["Carryer"] = Request.Form["Carryer"].ToString();
            Hid_Carriy.Value = Request.Form["Carryer"].ToString();//CA^国航         3U^川航~3U^川航
            Hid_Space.Value = Request.Form["Cabin"].ToString(); //N~K
            //航班号
            ViewState["FlyNo"] = Request.Form["FlyNo"].ToString();//4193           8881~8548
            //机型
            ViewState["Aircraft"] = Request.Form["Aircraft"].ToString();//757         330~321
            //起止时间
            ViewState["Time"] = Request.Form["Time"].ToString();//2013-1-31=07:00=09:35       2013-1-07=07:30=10:05~2013-2-26=07:45=10:40
            //起止城市
            ViewState["City"] = Request.Form["City"].ToString();//CTU-PEK^成都-北京       CTU-PEK^成都-北京~PEK-CTU^北京-成都
            //经停
            // ViewState["IsStop"] = Request.Form["A6"].ToString();//                      ~
            //机建
            ViewState["ABFee"] = Request.Form["ABFee"].ToString();//50.00                  50.00~50.00
            //燃油
            ViewState["FuelAdultFee"] = Request.Form["FuelAdultFee"].ToString();//130             130~130
            //折扣
            ViewState["DiscountRate"] = Request.Form["DiscountRate"].ToString();//40              30~35
            //座位数
            ViewState["TickNum"] = Request.Form["TickNum"].ToString();//>9                  >9~>9
            //销售价 仓位价
            ViewState["XSFee"] = Request.Form["XSFee"].ToString();//580.00                430.00~500.00
            //政策
            // ViewState["Policy"] = Request.Form["A12"].ToString();//0.121                0.04~0.058
            //里程
            ViewState["Mileage"] = Request.Form["Mileage"].ToString();//1440.000000         1440.000000~1440.000000
            //舱位
            ViewState["Cabin"] = Request.Form["Cabin"].ToString();//V1                N~K
            //餐饮
            // ViewState["IsFood"] = Request.Form["A15"].ToString();//                 ~
            //票面价Y仓价格
            ViewState["FareFee"] = Request.Form["FareFee"].ToString();//1440.000000     1440.000000~1440.000000
            //现返
            //  ViewState["ReturnMoney"] = Request.Form["A17"].ToString();//0           0~0
            //客规
            ViewState["Reservation"] = Request.Form["Reservation"].ToString();//<br/><br/>      <br/><br/>~<br/><br/>
            //政策来源
            // ViewState["PolicySource"] = Request.Form["A19"].ToString();//1              5~5
            //政策类型、政策ID、原始政策、供应政策、分销政策
            //ViewState["PolicyInfo"] = Request.Form["A20"].ToString();//1/10927/0/0.121/0.121        1/ffffffff-fff1-7680-2881-201212121218/0.0400/0.040/0.040~2/ffffffff-fff1-3950-0232-201208161006/0.0580/0.058/0.058
            //特价类型 1动态特价 2固态特价 0为普通价格
            ViewState["SpecialType"] = Request.Form["SpecialType"].ToString();//0               0~0
            //客规(分开的)
            //ViewState["Reservation2"] = Request.Form["A22"].ToString();//1              undefined~1
            //ViewState["Reservation2"] = Request.Form["Reservation2"].ToString();//1              undefined~1

            //行程类型
            ViewState["TravelType"] = Request.Form["TravelType"].ToString();//1         2
            ViewState["Terminal"] = Request.Form["Terminal"].ToString();//T1航站楼


            FQP.Carryer = Request.Form["Carryer"].ToString();
            FQP.FlyNo = Request.Form["FlyNo"].ToString();
            FQP.Aircraft = Request.Form["Aircraft"].ToString();
            FQP.Time = Request.Form["Time"].ToString();
            FQP.City = Request.Form["City"].ToString();
            FQP.ABFee = Request.Form["ABFee"].ToString();
            FQP.FuelAdultFee = Request.Form["FuelAdultFee"].ToString();
            FQP.DiscountRate = Request.Form["DiscountRate"].ToString();
            FQP.TickNum = Request.Form["TickNum"].ToString();
            FQP.XSFee = Request.Form["XSFee"].ToString();
            FQP.Mileage = Request.Form["Mileage"].ToString();
            FQP.Cabin = Request.Form["Cabin"].ToString();
            FQP.FareFee = Request.Form["FareFee"].ToString();
            FQP.Reservation = Request.Form["Reservation"].ToString();
            FQP.SpecialType = Request.Form["SpecialType"].ToString();
            FQP.TravelType = Request.Form["TravelType"].ToString();
            FQP.Terminal = Request.Form["Terminal"].ToString();
            //权限
            FQP.KongZhiXiTong = this.KongZhiXiTong;
            FQP.GongYingKongZhiFenXiao = this.GongYingKongZhiFenXiao;

            //加入ViewState
            ViewState["FlightQueryParam"] = FQP;
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "InitValue");
            // DataBase.LogCommon.Log.Error("Create.aspx页面InitValue", ex);

            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: private FlightQueryParam InitValue()】================================================================\r\n 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return FQP;
    }



    /// <summary>
    /// 分割时间
    /// </summary>
    /// <param name="time"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private string SplitTime(string time, int index)
    {
        try
        {
            string[] tlist = time.Split('=');
            return tlist[index].ToString();
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "SplitTime");
            //DataBase.LogCommon.Log.Error("Create.aspx页面SplitTime", ex);

            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: private string SplitTime(string time, int index)】================================================================\r\n 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return "";
    }
    /// <summary>
    /// 获取时间
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private DateTime GetDateTime(string datetime, int index)
    {
        //2013-1-07=07:30=10:05
        DateTime reStrData = Convert.ToDateTime("1900-01-01");
        try
        {
            string[] strArr = datetime.Split('=');
            if (index == 0)
            {
                reStrData = DateTime.Parse(strArr[0] + " " + strArr[1] + ":00");
            }
            else if (index == 1)
            {
                reStrData = DateTime.Parse(strArr[0] + " " + strArr[2] + ":00");
            }
        }
        catch (Exception ex)
        {
            // DataBase.LogCommon.Log.Error("Create.aspx页面GetDateTime", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: private DateTime GetDateTime(string datetime, int index)】================================================================\r\n 参数:datetime=" + datetime + " index=" + index + " 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return reStrData;
    }
    /// <summary>
    /// 分割城市
    /// </summary>
    /// <param name="city"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private string SplitCity(string city, int index)
    {
        try
        {
            //ctu-hgh^成都-杭州
            string[] c1list = city.Split('^');
            string[] c2list = c1list[1].Split('-');
            return c2list[index].ToString();
        }
        catch (Exception ex)
        {
            // OnErrorNew(0, ex.ToString(), "SplitCity");
            //DataBase.LogCommon.Log.Error("Create.aspx页面SplitCity", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: private string SplitCity(string city, int index)】================================================================\r\n 参数:city=" + city + " index=" + index + " 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return "";
    }
    /// <summary>
    /// 获取航段字符串信息
    /// </summary>
    /// <param name="city"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private string GetStrSkyWay(string city, int index)
    {
        string reCity = "";
        try
        {
            //ctu-hgh^成都-杭州~hgh-xiy^杭州-西安
            string[] c1list = city.Split('~');
            List<string> list = new List<string>();
            for (int i = 0; i < c1list.Length; i++)
            {
                list.Add(c1list[i].Split('^')[index]);
            }
            reCity = string.Join("/", list.ToArray());
        }
        catch (Exception ex)
        {
            // OnErrorNew(0, ex.ToString(), "SplitCity");
            //DataBase.LogCommon.Log.Error("Create.aspx页面GetStrSkyWay", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: private string GetStrSkyWay(string city, int index)】================================================================\r\n 参数:city=" + city + " index=" + index + " 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return reCity;
    }
    /// <summary>
    /// 获取起飞时间
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetAirTime(string date)
    {
        //2013-1-07=07:30=10:05~2013-2-26=07:45=10:40
        string reData = "";
        try
        {
            List<string> dateList = new List<string>();
            string[] strArr = date.Replace("+1", "").Split('~');
            for (int i = 0; i < strArr.Length; i++)
            {
                string[] strArrTime = strArr[i].Split('=');
                if (strArrTime.Length == 3)
                {
                    dateList.Add(strArrTime[0] + " " + strArrTime[1]);
                }
            }
            reData = string.Join("/", dateList.ToArray());
        }
        catch (Exception ex)
        {
            //DataBase.LogCommon.Log.Error("Create.aspx页面GetAirTime", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: public string GetAirTime(string date)】================================================================\r\n 参数:date=" + date + " 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return reData;
    }
    /// <summary>
    /// 分割城市三字码
    /// </summary>
    /// <param name="city"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private string SplitCityCode(string city, int index)
    {
        try
        {
            //ctu-hgh^成都-杭州
            string[] c1list = city.Split('^');
            string[] c2list = c1list[0].Split('-');
            return c2list[index].ToString();
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "SplitCityCode");
            //DataBase.LogCommon.Log.Error("Create.aspx页面SplitCityCode", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: private string SplitCityCode(string city, int index)】================================================================\r\n 参数:city=" + city + " index=" + index + " 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return "";
    }
    /// <summary>
    /// 显示和构造航段信息
    /// </summary>
    public void ShowSkyWayInfo(FlightQueryParam FQP)
    {
        try
        {
            decimal priceall = Math.Round((decimal.Parse(FQP.XSFee.ToString().Split('~')[0])), 2) + decimal.Parse(FQP.ABFee.ToString().Split('~')[0]) + decimal.Parse(FQP.FuelAdultFee.ToString().Split('~')[0]);
            StringBuilder sbSkyWay = new StringBuilder();
            sbSkyWay.Append("<tr>");
            sbSkyWay.Append("<td style='font-weight:bold;color:#404040;width:250px;'>" + SplitCity(FQP.City.ToString().Split('~')[0], 0) + "(" + SplitCityCode(FQP.City.ToString().Split('~')[0], 0) + ")" + "-" + SplitCity(FQP.City.ToString().Split('~')[0], 1) + "(" + SplitCityCode(FQP.City.ToString().Split('~')[0], 1) + ")" + "</td>");
            sbSkyWay.Append("<td>" + SplitTime(FQP.Time.ToString().Split('~')[0], 0).Substring(0) + "</td>");
            sbSkyWay.Append("<td style='font-weight:bold;color:#404040;'>" + SplitTime(FQP.Time.ToString().Split('~')[0], 1) + " " + SplitTime(FQP.Time.ToString().Split('~')[0], 2) + "</td>");
            sbSkyWay.Append("<td>" + FQP.Carryer.ToString().Split('~')[0].Split('^')[0] + "</td>");
            sbSkyWay.Append("<td>" + FQP.FlyNo.ToString().Split('~')[0] + "</td>");
            sbSkyWay.Append("<td>" + FQP.Aircraft.ToString().Split('~')[0] + "</td>");
            sbSkyWay.Append("<td style='font-weight:bold;color:#404040;'>" + FQP.Cabin.ToString().Split('~')[0] + "</td>");
            sbSkyWay.Append("<td>" + decimal.Parse(FQP.XSFee.ToString().Split('~')[0]).ToString("f2") + "</td>");
            sbSkyWay.Append("<td>" + FQP.ABFee.ToString().Split('~')[0] + "</td>");
            sbSkyWay.Append("<td>" + FQP.FuelAdultFee.ToString().Split('~')[0] + "</td>");
            sbSkyWay.Append("<td style='font-weight:bold;color:#ff6600; font-size:14px;'>" + priceall.ToString("f2") + "</td>");
            sbSkyWay.Append("</tr>");


            if (FQP.TravelType.ToString() == "2" || FQP.TravelType.ToString() == "3")
            {
                decimal price2 = Math.Round((decimal.Parse(FQP.XSFee.ToString().Split('~')[1])), 2) + decimal.Parse(FQP.ABFee.ToString().Split('~')[1]) + decimal.Parse(FQP.FuelAdultFee.ToString().Split('~')[1]);
                sbSkyWay.Append("<tr>");
                sbSkyWay.Append("<td style='font-weight:bold;color:#404040;width:250px;'>" + SplitCity(FQP.City.ToString().Split('~')[1], 0) + "(" + SplitCityCode(FQP.City.ToString().Split('~')[1], 0) + ")" + "-" + SplitCity(FQP.City.ToString().Split('~')[1], 1) + "(" + SplitCityCode(FQP.City.ToString().Split('~')[1], 1) + ")" + "</td>");
                sbSkyWay.Append("<td>" + SplitTime(FQP.Time.ToString().Split('~')[1], 0).Substring(0) + "</td>");
                sbSkyWay.Append("<td style='font-weight:bold;color:#404040;'>" + SplitTime(FQP.Time.ToString().Split('~')[1], 1) + " " + SplitTime(FQP.Time.ToString().Split('~')[1], 2) + "</td>");
                sbSkyWay.Append("<td>" + FQP.Carryer.ToString().Split('~')[1].Split('^')[0] + "</td>");
                sbSkyWay.Append("<td>" + FQP.FlyNo.ToString().Split('~')[1] + "</td>");
                sbSkyWay.Append("<td>" + FQP.Aircraft.ToString().Split('~')[1] + "</td>");
                sbSkyWay.Append("<td style='font-weight:bold;color:#404040;'>" + FQP.Cabin.ToString().Split('~')[1] + "</td>");
                sbSkyWay.Append("<td>" + decimal.Parse(FQP.XSFee.ToString().Split('~')[1]).ToString("f2") + "</td>");
                sbSkyWay.Append("<td>" + FQP.ABFee.ToString().Split('~')[1] + "</td>");
                sbSkyWay.Append("<td>" + FQP.FuelAdultFee.ToString().Split('~')[1] + "</td>");
                sbSkyWay.Append("<td style='font-weight:bold;color:#ff6600; font-size:14px;'>" + price2.ToString("f2") + "</td>");
                sbSkyWay.Append("</tr>");
            }

            Hid_BigNum.Value = FQP.TickNum.ToString().Split('~')[0];
            if (Hid_BigNum.Value == ">9")
            {
                Hid_BigNum.Value = "9";
            }
            TrShow.Text = sbSkyWay.ToString();
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "ShowInfo");
            //DataBase.LogCommon.Log.Error("Create.aspx页面ShowSkyWayInfo", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: public void ShowSkyWayInfo(FlightQueryParam FQP)】================================================================\r\n 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
    }






    /// <summary>
    /// 构造乘机人列表
    /// </summary>
    /// <param name="FQP">航班查询数据</param>
    /// <param name="IsExistAdult">乘机人数据</param>
    /// <param name="IsExistAdult">是否儿童出成人票 true是 false否</param>
    /// <param name="IsExistAdult">乘机人中是否含有成人</param>
    /// <param name="IsContainChild">乘机人中是否含有儿童</param>
    /// <param name="IsContainChild">乘机人中是否含有婴儿</param>
    /// <returns></returns>
    private List<Tb_Ticket_Passenger> GetPasList(FlightQueryParam FQP, out bool IsExistAdult, out bool IsExistCHD, out bool IsExistINF)
    {
        IsExistAdult = false;
        IsExistCHD = false;
        IsExistINF = false;
        List<Tb_Ticket_Passenger> pass = new List<Tb_Ticket_Passenger>();
        string ErrMsg = "";
        //需要加入的常旅客SQL列表       
        List<string> AddFlyList = new List<string>();
        try
        {
            if (flyList.Count == 0)
            {
                GetFlyUser();
            }
            string[] flyerStr = FQP.PasData.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < flyerStr.Length; i++)
            {
                //乘客序号#####乘客姓名#####乘客类型#####证件号类型#####证件号码#####乘客手机#####是否常旅客#####常旅客备注#####儿童标识(日期)#####航空公司卡号
                string[] strFlylist = flyerStr[i].Split(new string[] { "#####" }, StringSplitOptions.None);
                if (strFlylist != null && strFlylist.Length >= 10)
                {
                    Tb_Ticket_Passenger tpasser = new Tb_Ticket_Passenger();
                    tpasser.PassengerName = strFlylist[1].Trim();
                    tpasser.CType = strFlylist[3];
                    tpasser.Cid = strFlylist[4];
                    tpasser.PassengerType = int.Parse(strFlylist[2]);
                    tpasser.A10 = strFlylist[5];//乘客手机号码
                    tpasser.PMFee = decimal.Parse(FQP.XSFee.ToString().Split('~')[0]);
                    tpasser.A8 = strFlylist[9];//航空公司卡号
                    //常旅客备注
                    tpasser.Remark = HttpUtility.UrlDecode(strFlylist[7]);
                    //Y舱价格
                    decimal YFare = decimal.Parse(FQP.FareFee.ToString().Split('~')[0]);
                    DataAction d = new DataAction();
                    if (tpasser.PassengerType == 1)
                    {
                        //存在成人
                        IsExistAdult = true;
                    }
                    //婴儿
                    if (tpasser.PassengerType == 3)
                    {
                        decimal _TempFare = (0.1m * YFare) / 10;
                        tpasser.PMFee = d.FourToFiveNum(_TempFare, 0) * 10;
                        tpasser.ABFee = 0m;
                        tpasser.FuelFee = 0m;
                        //存在婴儿
                        IsExistINF = true;
                    }
                    else
                    {
                        tpasser.ABFee = decimal.Parse(FQP.ABFee.ToString().Split('~')[0]);
                        tpasser.FuelFee = decimal.Parse(FQP.FuelAdultFee.ToString().Split('~')[0]);
                        //儿童
                        if (tpasser.PassengerType == 2)
                        {
                            //不是儿童出成人票的儿童
                            if (!FQP.IsCHDToAudltTK)
                            {
                                tpasser.ABFee = 0m;
                                tpasser.FuelFee = (0.5m) * decimal.Parse(FQP.FuelAdultFee.ToString().Split('~')[0]);
                                tpasser.FuelFee = d.FourToFiveNum(tpasser.FuelFee / 10, 0) * 10;
                                tpasser.PMFee = d.FourToFiveNum(((0.5m) * YFare) / 10, 0) * 10;
                            }
                            //儿童标识Chld
                            tpasser.A7 = strFlylist[8];
                            //存在儿童
                            IsExistCHD = true;
                        }
                    }


                    //加入
                    pass.Add(tpasser);
                    //排序
                    pass.Sort(delegate(Tb_Ticket_Passenger a, Tb_Ticket_Passenger b)
                    {
                        return (a.PassengerType - b.PassengerType);
                    });


                    //常旅客处理
                    var list = from u in flyList where u.Name == tpasser.PassengerName && tpasser.Cid == u.CertificateNum && u.CpyNo == mCompany.UninCode select u;
                    //不存在该常旅客就添加一条
                    if (list.ToList<User_Flyer>().Count == 0 && strFlylist[6] == "1")
                    {
                        User_Flyer Flyer = new User_Flyer();
                        Flyer.id = Guid.NewGuid();
                        Flyer.MemberAccount = mUser.LoginName;
                        Flyer.RemainWithId = mUser.id.ToString();
                        Flyer.CpyNo = mCompany.UninCode;
                        Flyer.Name = tpasser.PassengerName.Trim();
                        Flyer.CertificateNum = tpasser.Cid.Trim();
                        Flyer.CertificateType = int.Parse(tpasser.CType);
                        Flyer.Tel = tpasser.A10;
                        Flyer.Sex = 0;
                        Flyer.Flyertype = tpasser.PassengerType;
                        //航空公司卡号
                        Flyer.CpyandNo = FQP.Carryer.Split('^')[0] + "," + strFlylist[9];
                        Flyer.Remark = "";
                        ////常旅客SQL                    
                        AddFlyList.Add(PbProject.Dal.Mapping.MappingHelper<User_Flyer>.CreateInsertModelSql(Flyer));
                    }
                }
            }
            //常旅客单独事务处理
            if (AddFlyList != null && AddFlyList.Count > 0)
            {
                if (!this.baseDataManage.ExecuteSqlTran(AddFlyList, out ErrMsg))
                {
                    PnrAnalysis.LogText.LogWrite("Create.aspx页面GetPasList添加常旅客 时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + ErrMsg + "\r\n", "AddFly");
                }
            }
        }
        catch (Exception ex)
        {
            //DataBase.LogCommon.Log.Error("Create.aspx页面GetPasList", ep);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:private List<Tb_Ticket_Passenger> GetPasList(FlightQueryParam FQP, out bool IsExistAdult, out bool IsExistCHD, out bool IsExistINF)】================================================================\r\n 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return pass;
    }

    /// <summary>
    ///  构造机票订单实体 
    /// </summary>
    /// <param name="FQP">航班查询参数实体</param>
    /// <param name="IsChild">是否为儿童订单 true 是 false 不是</param>
    /// <param name="AllowChangePNRFlag">是否允许换编码 true 是 false 不是</param>
    /// <param name="IsETDZAudltTK">是否儿童出成人票 true 是 false 不是 </param>
    /// <param name="PasList">乘机人列表</param>
    /// <param name="SkyWayList">航段列表</param>
    /// <returns></returns>
    private Tb_Ticket_Order GetOrder(FlightQueryParam FQP, bool IsChild, List<Tb_Ticket_Passenger> PasList, List<Tb_Ticket_SkyWay> SkyWayList)
    {
        Tb_Ticket_Order to = new Tb_Ticket_Order();
        try
        {
            to.LinkMan = txtLinkName.Text.Trim().Replace("'", "") == "" ? mCompany.ContactUser : txtLinkName.Text.Trim().Replace("'", "");
            to.LinkManPhone = txtMobile.Text.Trim().Replace("'", "") == "" ? mCompany.ContactTel : txtMobile.Text.Trim().Replace("'", "");
            //白屏预订
            to.OrderSourceType = 1;
            to.OrderStatusCode = 1;//默认新订单等待支付
            to.PolicySource = 1;//默认b2b


            to.CreateCpyName = mCompany != null ? mCompany.UninAllName : "";
            to.CreateCpyNo = mCompany != null ? mCompany.UninCode : "";

            to.CreateLoginName = mUser.LoginName;
            to.CreateUserName = mUser.UserName;
            to.OwnerCpyNo = mCompany != null ? mCompany.UninCode : "";
            to.OwnerCpyName = mCompany != null ? mCompany.UninAllName : "";
            //是否允许换编码出票
            to.AllowChangePNRFlag = FQP.AllowChangePNRFlag;
            //是否为抢票订单
            to.RobTicketStatus = FQP.IsRobTicketOrder ? 1 : 0;
            //行程类型
            int _TravelType = 1;
            int.TryParse(FQP.TravelType.ToString(), out _TravelType);
            to.TravelType = _TravelType;
            to.CarryCode = GetStrSkyWay(FQP.Carryer.ToString(), 0);
            to.FlightCode = GetStrSkyWay(FQP.FlyNo.ToString(), 0);

            //to.AirTime = GetAirTime(ViewState["Time"].ToString());
            to.AirTime = DateTime.Parse(GetAirTime(FQP.Time.ToString()).Split('/')[0]);

            to.Travel = GetStrSkyWay(FQP.City.ToString(), 1);
            to.TravelCode = GetStrSkyWay(FQP.City.ToString(), 0);
            to.Space = GetStrSkyWay(FQP.Cabin.ToString(), 0);
            to.Discount = GetStrSkyWay(FQP.DiscountRate.ToString(), 0);
            if (to.Discount.Length > 10)
            {
                to.Discount = to.Discount.Substring(0, 10);
            }
            //乘客人数
            to.PassengerNumber = PasList.Count;
            //乘客姓名 已"|"分割
            List<string> PasNameList = new List<string>();
            foreach (Tb_Ticket_Passenger item in PasList)
            {
                PasNameList.Add(item.PassengerName);
            }
            to.PassengerName = string.Join("|", PasNameList.ToArray());
            //舱位价
            to.PMFee = decimal.Parse(FQP.XSFee.ToString().Split('~')[0]);
            //Y舱价格
            decimal YFare = decimal.Parse(FQP.FareFee.ToString().Split('~')[0]);
            //为儿童订单 且儿童不出成人票
            if (IsChild)
            {
                if (!FQP.IsCHDToAudltTK)
                {
                    to.ABFee = 0m;
                    to.FuelFee = (0.5m) * decimal.Parse(FQP.FuelAdultFee.ToString().Split('~')[0]);
                    if (FQP.Cabin.Contains("F") || FQP.Cabin.Contains("C") || FQP.Cabin.Contains("Y"))
                    {
                        to.Space = FQP.Cabin.Replace("~", "/");
                    }
                    else
                    {
                        to.Space = "Y";
                    }
                    to.Discount = "50";
                }
                to.IsChdFlag = true;//儿童
                ///是否儿童出成人票
                to.IsCHDETAdultTK = FQP.IsCHDToAudltTK ? 1 : 0;
            }
            else
            {
                to.ABFee = decimal.Parse(FQP.ABFee.ToString().Split('~')[0]);
                to.FuelFee = decimal.Parse(FQP.FuelAdultFee.ToString().Split('~')[0]);
                to.IsChdFlag = false;//成人  

                //婴儿价格        
                DataAction d = new DataAction();
                decimal _TempFare = (0.1m * YFare) / 10;
                to.BabyFee = d.FourToFiveNum(_TempFare, 0) * 10;
            }
            //客规
            to.KeGui = FQP.Reservation.ToString().Split('~')[0];
        }
        catch (Exception ex)
        {
            //DataBase.LogCommon.Log.Error("Create.aspx页面GetOrder", ep);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:private Tb_Ticket_Order GetOrder(FlightQueryParam FQP, bool IsChild, List<Tb_Ticket_Passenger> PasList, List<Tb_Ticket_SkyWay> SkyWayList)】================================================================\r\n 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return to;
    }

    /// <summary>
    /// 构造航段实体列表
    /// </summary>
    /// <returns></returns>
    private List<Tb_Ticket_SkyWay> GetSkyWay(FlightQueryParam FQP)
    {
        List<Tb_Ticket_SkyWay> listSky = new List<Tb_Ticket_SkyWay>();
        try
        {
            //去程
            Tb_Ticket_SkyWay FromSkyWay = new Tb_Ticket_SkyWay();
            //赋值
            FromSkyWay.CarryCode = FQP.Carryer.ToString().Split('~')[0].Split('^')[0];
            FromSkyWay.CarryName = FQP.Carryer.ToString().Split('~')[0].Split('^')[1];
            FromSkyWay.FlightCode = FQP.FlyNo.ToString().Split('~')[0];

            string StartHour = "1901-1-1 " + FQP.Time.ToString().Split('~')[0].Split('=')[1] + ":00";
            string EndHour = "1901-1-1 " + FQP.Time.ToString().Split('~')[0].Split('=')[2] + ":00";
            bool IsAddDay = false;
            if (DateTime.Compare(DateTime.Parse(EndHour), DateTime.Parse(StartHour)) < 0)
            {
                IsAddDay = true;
            }
            FromSkyWay.FromDate = GetDateTime(FQP.Time.ToString().Split('~')[0], 0);
            FromSkyWay.ToDate = GetDateTime(FQP.Time.ToString().Split('~')[0], 1);
            if (IsAddDay)
            {
                FromSkyWay.ToDate = FromSkyWay.ToDate.AddDays(1);
            }
            FromSkyWay.FromCityName = SplitCity(FQP.City.ToString().Split('~')[0], 0);
            FromSkyWay.FromCityCode = SplitCityCode(FQP.City.ToString().Split('~')[0], 0);
            FromSkyWay.ToCityName = SplitCity(FQP.City.ToString().Split('~')[0], 1); ;
            FromSkyWay.ToCityCode = SplitCityCode(FQP.City.ToString().Split('~')[0], 1);
            FromSkyWay.Space = FQP.Cabin.ToString().Split('~')[0];
            FromSkyWay.Discount = FQP.DiscountRate.ToString().Split('~')[0];
            FromSkyWay.Aircraft = FQP.Aircraft.ToString().Split('~')[0];
            FromSkyWay.Mileage = FQP.Mileage.ToString().Split('~')[0];
            decimal _ABFee = 0m, _FuelFee = 0m, _SeatPrice = 0m, _FareFee = 0m;
            decimal.TryParse(FQP.ABFee.ToString().Split('~')[0], out _ABFee);
            decimal.TryParse(FQP.FuelAdultFee.ToString().Split('~')[0], out _FuelFee);
            decimal.TryParse(FQP.XSFee.ToString().Split('~')[0], out _SeatPrice);
            decimal.TryParse(FQP.FareFee.ToString().Split('~')[0], out _FareFee);
            FromSkyWay.ABFee = _ABFee;
            FromSkyWay.FuelFee = _FuelFee;
            FromSkyWay.SpacePrice = _SeatPrice;
            FromSkyWay.FareFee = _FareFee;
            FromSkyWay.IsShareFlight = FQP.FlyNo.ToString().ToString().Split('~')[0].Contains("*") ? 1 : 0;
            FromSkyWay.Terminal = FQP.Terminal.ToString().Split('~')[0];
            //添加去程
            listSky.Add(FromSkyWay);
            if (FQP.TravelType.ToString() == "2" || FQP.TravelType.ToString() == "3")
            {
                //回程
                Tb_Ticket_SkyWay ReturnSkyWay = new Tb_Ticket_SkyWay();
                //赋值
                //....
                ReturnSkyWay.CarryCode = FQP.Carryer.ToString().Split('~')[1].Split('^')[0];
                ReturnSkyWay.CarryName = FQP.Carryer.ToString().Split('~')[1].Split('^')[1];
                ReturnSkyWay.FlightCode = FQP.FlyNo.ToString().Split('~')[1];

                StartHour = "1901-1-1 " + FQP.Time.ToString().Split('~')[1].Split('=')[1] + ":00";
                EndHour = "1901-1-1 " + FQP.Time.ToString().Split('~')[1].Split('=')[2] + ":00";
                IsAddDay = false;
                if (DateTime.Compare(DateTime.Parse(EndHour), DateTime.Parse(StartHour)) < 0)
                {
                    IsAddDay = true;
                }
                ReturnSkyWay.FromDate = GetDateTime(FQP.Time.ToString().Split('~')[1], 0);
                ReturnSkyWay.ToDate = GetDateTime(FQP.Time.ToString().Split('~')[1], 1);
                if (IsAddDay)
                {
                    ReturnSkyWay.ToDate = ReturnSkyWay.ToDate.AddDays(1);
                }

                ReturnSkyWay.FromCityName = SplitCity(FQP.City.ToString().Split('~')[1], 0);
                ReturnSkyWay.FromCityCode = SplitCityCode(FQP.City.ToString().Split('~')[1], 0);
                ReturnSkyWay.ToCityName = SplitCity(FQP.City.ToString().Split('~')[1], 1); ;
                ReturnSkyWay.ToCityCode = SplitCityCode(FQP.City.ToString().Split('~')[1], 1);
                ReturnSkyWay.Space = FQP.Cabin.ToString().Split('~')[1];
                ReturnSkyWay.Discount = FQP.DiscountRate.ToString().Split('~')[1];
                ReturnSkyWay.Aircraft = FQP.Aircraft.ToString().Split('~')[1];
                ReturnSkyWay.Mileage = FQP.Mileage.ToString().Split('~')[1];
                decimal _ABFee1 = 0m, _FuelFee1 = 0m, _SpacePrice1 = 0m, _FareFee1 = 0m;
                decimal.TryParse(FQP.ABFee.ToString().Split('~')[1], out _ABFee1);
                decimal.TryParse(FQP.FuelAdultFee.ToString().Split('~')[1], out _FuelFee1);
                decimal.TryParse(FQP.XSFee.ToString().Split('~')[1], out _SpacePrice1);
                decimal.TryParse(FQP.FareFee.ToString().Split('~')[0], out _FareFee1);
                ReturnSkyWay.ABFee = _ABFee1;
                ReturnSkyWay.FuelFee = _FuelFee1;
                ReturnSkyWay.SpacePrice = _SpacePrice1;
                ReturnSkyWay.FareFee = _FareFee1;
                ReturnSkyWay.IsShareFlight = FQP.FlyNo.ToString().ToString().Split('~')[1].Contains("*") ? 1 : 0;
                ReturnSkyWay.Terminal = FQP.Terminal.ToString().Split('~')[1];
                //添加回程
                listSky.Add(ReturnSkyWay);
            }
        }
        catch (Exception ex)
        {
            //DataBase.LogCommon.Log.Error("Create.aspx页面GetSkyWay()", ep);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:private List<Tb_Ticket_SkyWay> GetSkyWay(FlightQueryParam FQP)】================================================================\r\n 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return listSky;
    }

    /// <summary>
    /// 提示编码生成失败原因
    /// </summary>
    /// <param name="type"></param>
    /// <param name="yudingRecvData"></param>
    /// <returns></returns>
    public string ShowPnrFailInfo(int type, string yudingRecvData, RePnrObj PnrInfo)
    {
        string msg = "";
        if (yudingRecvData.ToUpper().Contains("PLS NM1XXXX/XXXXXX"))
        {
            //PLS NM1XXXX/XXXXXX
            msg = "乘客输入姓名格式错误！原因如下:<br />（" + yudingRecvData.ToUpper() + "）姓名中应加斜线(/),或斜线数量不正确!";
        }
        else if (yudingRecvData.ToUpper().Contains("UNABLE TO SELL.PLEASE") || yudingRecvData.ToUpper().Contains("SEATS"))
        {
            msg = "座位数不足或座位已销售完,请重新预定!";
        }
        else if (yudingRecvData.Contains("不支持的汉字"))
        {
            msg = "乘机姓名中存在航信不支持的汉字，请仔细检查！";
        }
        else if (yudingRecvData.IndexOf("地址无效") != -1 || yudingRecvData.IndexOf("无法从传输连接中读取数据") != -1 || yudingRecvData.IndexOf("无法连接") != -1 || yudingRecvData.IndexOf("强迫关闭") != -1 || yudingRecvData.IndexOf("由于") != -1)
        {
            msg = "与航信连接失败，请重新预订！<br />错误信息:" + yudingRecvData;
        }
        else if (yudingRecvData.IndexOf("超时！") != -1 || yudingRecvData.IndexOf("服务器忙") != -1)
        {
            msg = "系统繁忙,请稍后再试！";
        }
        else if (yudingRecvData.ToUpper().Contains("WSACancelBlockingCall") || yudingRecvData.ToUpper().Contains("TRANSACTION IN PROGRESS") || yudingRecvData.ToUpper().Contains(" FORMAT ") || yudingRecvData.ToUpper().Contains("NO PNR") || yudingRecvData.ToUpper().Contains("CHECK TKT DATE") || yudingRecvData.ToUpper().Contains("为空") || yudingRecvData.ToUpper().Contains("对象的实例"))
        {
            msg = (type == 1 ? "成人" : "儿童") + "编码生成失败！原因如下:<br />" + yudingRecvData;
        }
        else
        {
            string AdultYuDing = "", CHDYuDing = "";
            if (PnrInfo.AdultYudingList.Count > 0)
            {
                foreach (string key in PnrInfo.AdultYudingList.Keys)
                {
                    AdultYuDing = PnrInfo.AdultYudingList[key].ToString();
                }
            }
            if (PnrInfo.ChildYudingList.Count > 0)
            {
                foreach (string key in PnrInfo.ChildYudingList.Keys)
                {
                    CHDYuDing = PnrInfo.ChildYudingList[key].ToString();
                }
            }
            string strYuding = (type == 1 ? AdultYuDing : CHDYuDing);
            msg = (type == 1 ? "成人" : "儿童") + "编码生成失败,原因如下:<br />" + strYuding + (strYuding.ToUpper().Contains("FFP TOP TIER CODE INPUT ERROR") ? "<br />输入航空公司卡号错误!" : "");
        }
        return msg;
    }
    /// <summary>
    /// 生成订单
    /// </summary>
    /// <param name="PasData">航班查询数据实体</param>
    /// <param name="PasData">输入乘客数据</param>
    /// <param name="IsAsAdultOrder">是否关联成人订单号 true关联 false不关联</param>
    /// <param name="AllowChangePNRFlag">是否允许换编码出票 true允许 false不允许</param>
    /// <param name="IsETDZAudltTK">是否儿童出成人票 true是 false否</param>
    /// <param name="CHDAssociationAdultOrderId">关联成人订单号</param>
    /// <param name="msg">提示信息或者跳转</param>
    /// <returns></returns>
    public bool GenerationOrder(FlightQueryParam FQP, out string msg)
    {
        bool IsSuc = false;
        msg = "";
        //儿童备注关联成人编码
        string RmkAdultPnr = "";
        //是否为两个订单
        bool IsSecOrder = false;
        bool IsExistAdult = false;
        bool IsExistCHD = false;
        bool IsExistINF = false;
        try
        {
            Tb_Ticket_Order AdultOrder = null, ChildOrder = null;
            if (mCompany == null || baseParametersList == null)
            {
                msg = "mCompany公司信息丢失";
                return IsSuc;
            }
            //订单管理
            Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
            OrderInputParam OrderParam = new OrderInputParam();
            OrderMustParamModel ParamModel = new OrderMustParamModel();

            //构造生成订单需要的信息
            List<Tb_Ticket_SkyWay> SkyWay1 = GetSkyWay(FQP);
            //构造乘客信息            
            List<Tb_Ticket_Passenger> Paslist = GetPasList(FQP, out IsExistAdult, out IsExistCHD, out IsExistINF);
            if (Paslist == null || Paslist.Count == 0)
            {
                msg = "乘客信息错误";
                return IsSuc;
            }
            //订单日志记录
            Log_Tb_AirOrder logOrder = new Log_Tb_AirOrder();
            logOrder.OperTime = DateTime.Now;
            logOrder.OperType = "创建订单";
            logOrder.OperContent = mUser.LoginName + "于" + logOrder.OperTime + "创建订单。";
            logOrder.OperLoginName = mUser.LoginName;
            logOrder.OperUserName = mUser.UserName;
            logOrder.CpyNo = mCompany.UninCode;
            logOrder.CpyName = mCompany.UninName;
            logOrder.CpyType = mCompany.RoleType;
            logOrder.WatchType = 5;
            if (IsExistCHD)
            {
                //添加权限 是否可以预定儿童票 未加
                if (FQP.GongYingKongZhiFenXiao != null && FQP.GongYingKongZhiFenXiao.Contains("|90|"))
                {
                    msg = "目前暂时无法预订儿童票！";
                }
            }

            //关联成人订单号
            if (FQP.IsAsAdultOrder)
            {
                #region 关联成人订单号
                //开启儿童编码必须关联成人编码或者成人订单号
                if (FQP.KongZhiXiTong != null && FQP.KongZhiXiTong.Contains("|95|"))
                {
                    string sqlWhere = "";
                    //儿童订单关联成人订单号
                    if (FQP.CHDAssociationAdultOrderId == "")
                    {
                        msg = "关联成人订单不能为空！";
                    }
                    else
                    {
                        sqlWhere = string.Format(" OrderId='{0}' ", FQP.CHDAssociationAdultOrderId);
                        List<Tb_Ticket_Order> list = this.baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Order>;
                        if (list != null && list.Count > 0)
                        {
                            if (list[0].IsChdFlag)
                            {
                                msg = "该订单非成人订单!";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(list[0].PNR))
                                {
                                    msg = "该订单还未生成编码!";
                                }
                                else if (list[0].OrderStatusCode < 4)
                                {
                                    msg = "关联成人订单未出票！";
                                }
                                else
                                {
                                    RmkAdultPnr = list[0].PNR;
                                }
                                //添加权限是否验证 儿童航段与关联成人航段信息是否一致 还未加权限
                                if (FQP.KongZhiXiTong == null || !FQP.KongZhiXiTong.Contains("|91|"))
                                {
                                    if (msg == "" && !ValSkyWay(FQP.CHDAssociationAdultOrderId, SkyWay1))
                                    {
                                        msg = "成人订单航程与儿童订单航程信息不一致，无法预定！";
                                    }
                                }
                            }
                        }
                        else
                        {
                            msg = "关联成人订单不存在！";
                        }
                    }
                }
                #endregion
            }
            //关联订单号通过
            if (msg == "")
            {
                #region 生成编码记录编码信息
                //航空公司 出票Office 和儿童编码所备注的成人编码
                string defaultOffice = this.configparam.Office.Split('^')[0];
                string CpyNo = mCompany.UninCode, CarryCode = SkyWay1[0].CarryCode, Office = defaultOffice;
                string PrintOffice = GetPrintOffice(CpyNo, CarryCode);
                if (!string.IsNullOrEmpty(PrintOffice))
                {
                    Office = PrintOffice;
                }
                //生成编码
                RePnrObj PnrInfo = GetPnrInfo(FQP, CarryCode, Office, RmkAdultPnr, FQP.IsCHDToAudltTK, Paslist, SkyWay1);
                //记录指令
                SaveInsInfo(PnrInfo, mUser, mCompany);
                PnrInfo.PrintOffice = PrintOffice;
                //记录Pnr日志Id=
                List<string> pnrLogList = new List<string>();
                string AdultPnr = string.Empty;
                string childPnr = string.Empty;
                //成人预订信息编码记录            
                if (PnrInfo.AdultYudingList.Count > 0)
                {
                    AdultPnr = PnrInfo.AdultPnr;
                    if (string.IsNullOrEmpty(AdultPnr) || AdultPnr.Trim().Length != 6)
                    {
                        //提示pnr失败信息
                        string yudingRecvData = PnrInfo.AdultYudingList.Values[0];
                        msg = ShowPnrFailInfo(1, yudingRecvData, PnrInfo);
                        AdultPnr = "";
                    }
                    int AdultYuDingCount = PnrInfo.AdultYudingList.Keys.Count;
                    for (int i = 0; i < AdultYuDingCount; i++)
                    {
                        //记录编码日志
                        YuDingPnrLog(PnrInfo, PnrInfo.AdultYudingList.Keys[i], PnrInfo.AdultYudingList.Values[i], AdultPnr, PnrInfo.Office, ref pnrLogList);
                    }
                    if (AdultPnr.Length == 6)
                    {
                        if ((PnrInfo.PatModelList[0] == null || PnrInfo.PatModelList[0].PatList.Count == 0))
                        {
                            msg = "成人编码" + AdultPnr + "未能PAT取到价格，原因如下:<br />" + PnrInfo.PatList[0];
                        }
                        //婴儿PAT
                        if (IsExistINF && (PnrInfo.PatModelList[2] == null || PnrInfo.PatModelList[2].PatList.Count == 0))
                        {
                            msg = "成人编码" + AdultPnr + "未能PAT取到婴儿价格，原因如下:<br />" + PnrInfo.PatList[2];
                        }
                    }
                }
                //儿童预订信息编码记录
                if (PnrInfo.ChildYudingList.Count > 0)
                {
                    childPnr = PnrInfo.childPnr;
                    if (string.IsNullOrEmpty(childPnr) || childPnr.Trim().Length != 6)
                    {
                        //提示pnr失败信息
                        string yudingRecvData = PnrInfo.ChildYudingList.Values[0];
                        msg = ShowPnrFailInfo(2, yudingRecvData, PnrInfo);
                        childPnr = "";
                    }
                    int ChildYuDingCount = PnrInfo.ChildYudingList.Keys.Count;
                    for (int i = 0; i < ChildYuDingCount; i++)
                    {
                        //记录编码日志
                        YuDingPnrLog(PnrInfo, PnrInfo.ChildYudingList.Keys[i], PnrInfo.ChildYudingList.Values[i], childPnr, PnrInfo.Office, ref pnrLogList);
                    }
                    if (childPnr.Length == 6)
                    {
                        if ((PnrInfo.PatModelList[1] == null || PnrInfo.PatModelList[1].PatList.Count == 0))
                        {
                            msg = "儿童编码" + childPnr + "未能PAT取到价格，原因如下:<br />" + PnrInfo.PatList[1];
                        }
                        //是否儿童出成人票
                        if (FQP.IsCHDToAudltTK && (PnrInfo.CHDToAdultPat == null || PnrInfo.CHDToAdultPat.PatList.Count == 0))
                        {
                            msg = "儿童编码" + childPnr + "未能PAT取到价格，原因如下:<br />" + PnrInfo.CHDToAdultPatCon.Trim();
                        }
                    }
                }
                #endregion

                #region 组合生成订单所需要的实体数据
                //成人+婴儿 成人+成人 儿童+备注成人订单号  只生成一个订单
                //成人+儿童且没有备注订单号  成人+儿童+婴儿   生成两个订单
                //存在儿童 也存在成人
                if (IsExistCHD && IsExistAdult)
                {
                    if (!FQP.IsAsAdultOrder)
                    {
                        IsSecOrder = true;
                        //生成儿童订单
                        List<Tb_Ticket_SkyWay> SkyWay2 = GetSkyWay(FQP);
                        //儿童乘客列表
                        List<Tb_Ticket_Passenger> ChildList = new List<Tb_Ticket_Passenger>();
                        foreach (Tb_Ticket_Passenger pas in Paslist)
                        {
                            if (pas.PassengerType == 2)
                            {
                                ChildList.Add(pas);
                            }
                        }
                        ChildOrder = GetOrder(FQP, true, ChildList, SkyWay2);
                        Log_Tb_AirOrder logOrder1 = new Log_Tb_AirOrder();
                        logOrder1.OperTime = DateTime.Now;
                        logOrder1.OperType = "创建订单";
                        logOrder1.OperContent = mUser.LoginName + "于" + logOrder.OperTime + "创建订单。";
                        logOrder1.OperLoginName = mUser.LoginName;
                        logOrder1.OperUserName = mUser.UserName;
                        logOrder1.CpyNo = mCompany.UninCode;
                        logOrder1.CpyName = mCompany.UninName;
                        logOrder1.CpyType = mCompany.RoleType;
                        logOrder1.WatchType = 5;

                        //加入参数
                        OrderMustParamModel ParamModel1 = new OrderMustParamModel();
                        OrderParam.PnrInfo = PnrInfo;
                        ParamModel1.PasList = ChildList;
                        ParamModel1.SkyList = SkyWay2;
                        ParamModel1.Order = ChildOrder;
                        ParamModel1.LogOrder = logOrder1;
                        //加入集合
                        OrderParam.OrderParamModel.Add(ParamModel1);
                    }
                }
                //为两个订单时
                if (IsSecOrder)
                {
                    //排除儿童乘客
                    List<Tb_Ticket_Passenger> NotCHDList = new List<Tb_Ticket_Passenger>();
                    foreach (Tb_Ticket_Passenger item in Paslist)
                    {
                        if (item.PassengerType != 2)
                        {
                            NotCHDList.Add(item);
                        }
                    }
                    Paslist = NotCHDList;
                    AdultOrder = GetOrder(FQP, false, Paslist, SkyWay1);
                }
                else
                {
                    //为一个订单时
                    AdultOrder = GetOrder(FQP, IsExistCHD, Paslist, SkyWay1);
                    if (FQP.IsAsAdultOrder)
                    {
                        AdultOrder.PNR = RmkAdultPnr;
                        AdultOrder.AssociationOrder = FQP.CHDAssociationAdultOrderId;
                    }
                }
                //  是否有婴儿
                AdultOrder.HaveBabyFlag = IsExistINF;
                //
                OrderParam.PnrInfo = PnrInfo;
                ParamModel.PasList = Paslist;
                ParamModel.SkyList = SkyWay1;
                ParamModel.Order = AdultOrder;
                ParamModel.LogOrder = logOrder;
                //加入集合
                OrderParam.OrderParamModel.Add(ParamModel);
                #endregion
                if (pnrLogList.Count > 0 && !(AdultPnr == "" && childPnr == ""))
                {
                    string UpdatePnrLogSQL = string.Format(" update Log_Pnr set OrderFlag=1 where id in({0}) ", string.Join(",", pnrLogList.ToArray()));
                    OrderParam.ExecSQLList.Add(UpdatePnrLogSQL);
                }
            }
            //前面都通过
            if (msg == "")
            {
                string ErrMsg = "";
                //生成订单                
                IsSuc = OrderManage.CreateOrder(ref OrderParam, out ErrMsg);
                List<string> Paramlist = new List<string>();
                //两个订单url处理
                foreach (OrderMustParamModel item in OrderParam.OrderParamModel)
                {
                    if (item.Order.IsChdFlag)
                    {
                        Paramlist.Add("ChildOrderId=" + item.Order.OrderId);
                    }
                    else
                    {
                        Paramlist.Add("AdultOrderId=" + item.Order.OrderId);
                    }
                }
                if (IsSuc)
                {
                    //调转页面参数
                    msg = "Confirmation.aspx?" + string.Join("&", Paramlist.ToArray()) + "&currentuserid=" + mUser.id.ToString();
                }
                else
                {
                    //出错信息
                    msg = ErrMsg;
                }
            }
        }
        catch (Exception ex)
        {
            //DataBase.LogCommon.Log.Error("Create.aspx页面GenerationOrder", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:public bool GenerationOrder(FlightQueryParam FQP, out string msg)】================================================================\r\n 异常信息:" + msg + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
            //异常信息
            msg = ex.Message;
            IsSuc = false;
        }
        return IsSuc;
    }

    /// <summary>
    /// 保存指令信息到数据库
    /// </summary>
    /// <returns></returns>
    public bool SaveInsInfo(RePnrObj PnrInfo, User_Employees m_user, User_Company m_company)
    {
        bool IsSuc = true;
        string errMsg = "";
        try
        {
            List<string> sqlList = new List<string>();
            if (PnrInfo != null && PnrInfo.InsList.Count > 0)
            {
                //一组指令ID
                string GroupID = System.DateTime.Now.Ticks.ToString();
                DateTime _sendtime = Convert.ToDateTime("1900-01-01");
                DateTime _recvtime = Convert.ToDateTime("1900-01-01");
                string UserAccount = mUser.LoginName, CpyNo = mCompany.UninCode, serverIPPort = PnrInfo.ServerIP + ":" + PnrInfo.ServerPort, Office = PnrInfo.Office;
                string[] strArr = null;
                List<string> Removelist = new List<string>();
                Removelist.Add("id");
                foreach (KeyValuePair<string, string> KV in PnrInfo.InsList)
                {
                    strArr = KV.Key.Split(new string[] { PnrInfo.SplitChar }, StringSplitOptions.None);
                    if (strArr.Length == 4)
                    {
                        Tb_SendInsData ins = new Tb_SendInsData();
                        ins.SendIns = strArr[0];
                        if (DateTime.TryParse(strArr[1], out _sendtime))
                        {
                            ins.SendTime = _sendtime;
                        }
                        if (DateTime.TryParse(strArr[2], out _recvtime))
                        {
                            ins.RecvTime = _recvtime;
                        }
                        if (strArr[3] != "")
                        {
                            ins.Office = strArr[3];
                        }
                        ins.RecvData = KV.Value;
                        ins.Office = Office;
                        ins.ServerIPAndPort = serverIPPort + "|" + GroupID;
                        ins.UserAccount = UserAccount;
                        ins.CpyNo = CpyNo;
                        sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(ins, Removelist));
                    }
                }
                if (sqlList.Count > 0)
                {
                    IsSuc = this.baseDataManage.ExecuteSqlTran(sqlList, out errMsg);
                }
            }
        }
        catch (Exception ex)
        {
            IsSuc = false;
            errMsg = ex.Message + ex.StackTrace.ToString();
            // DataBase.LogCommon.Log.Error("Create.aspx页面SaveInsInfo", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:public bool SaveInsInfo(RePnrObj PnrInfo, User_Employees m_user, User_Company m_company)】================================================================\r\n 异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return IsSuc;
    }
    /// <summary>
    /// 预订编码日志
    /// </summary>
    /// <param name="SSContent"></param>
    /// <param name="ResultContent"></param>
    /// <param name="Pnr"></param>
    /// <param name="pnrLogList"></param>
    /// <returns></returns>
    public bool YuDingPnrLog(RePnrObj PnrInfo, string SSContent, string ResultContent, string Pnr, string Office, ref List<string> pnrLogList)
    {
        bool Insert = false;
        Log_Pnr logpnr = new Log_Pnr();
        try
        {
            logpnr.CpyNo = mCompany != null ? mCompany.UninCode : "为空";
            logpnr.CpyName = mCompany != null ? mCompany.UninAllName : "为空";
            logpnr.CpyType = mCompany != null ? mCompany.RoleType : 4;
            logpnr.OperTime = System.DateTime.Now;
            logpnr.OperLoginName = mUser != null ? mUser.LoginName : "为空";
            logpnr.OperUserName = mUser != null ? mUser.UserName : "为空";
            logpnr.SSContent = SSContent;
            logpnr.ResultContent = ResultContent;
            logpnr.PNR = Pnr;
            logpnr.OfficeCode = Office;
            logpnr.A7 = PnrInfo.ServerIP + "|" + PnrInfo.ServerPort;//IP和端口
            Insert = (bool)this.baseDataManage.CallMethod("Log_Pnr", "Insert", null, new object[] { logpnr });
            if (Insert)
            {
                pnrLogList.Add("'" + logpnr.id.ToString() + "'");
            }
        }
        catch (Exception ex)
        {
            // DataBase.LogCommon.Log.Error("Create.aspx页面YuDingPnrLog", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:public bool YuDingPnrLog(RePnrObj PnrInfo, string SSContent, string ResultContent, string Pnr, string Office, ref List<string> pnrLogList)】================================================================\r\n实体数据：" + logpnr.ToString() + "  异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
        }
        return Insert;
    }

    /// <summary>
    /// 生成编码
    /// </summary>
    /// <param name="CarryCode">航空公司二字码</param>
    /// <param name="Office">预订编码Office</param>
    /// <param name="AdultPnr">儿童订单中儿童编码备注的成人编码</param>
    /// <param name="IsChdETDZAudltTK">是否儿童出成人票</param>
    /// <param name="pList">乘机人列表</param>
    /// <param name="skywaylist">航段列表</param>
    /// <returns></returns>
    public RePnrObj GetPnrInfo(FlightQueryParam FQP, string CarryCode, string Office, string AdultPnr, bool IsChdETDZAudltTK, List<Tb_Ticket_Passenger> pList, List<Tb_Ticket_SkyWay> skywaylist)
    {
        List<IPassenger> pasList = new List<IPassenger>();
        List<ISkyLeg> skyList = new List<ISkyLeg>();
        SendNewPID pid = new SendNewPID();
        PnrParamObj PnrParam = new PnrParamObj();
        //必填项 是否开启新版PID发送指令 
        PnrParam.UsePIDChannel = FQP.KongZhiXiTong != null && FQP.KongZhiXiTong.Contains("|48|") ? 2 : 0;  //2;
        PnrParam.ServerIP = this.configparam.WhiteScreenIP;
        PnrParam.ServerPort = int.Parse(string.IsNullOrEmpty(this.configparam.WhiteScreenPort) ? "0" : this.configparam.WhiteScreenPort);
        PnrParam.Office = Office;
        PnrParam.CarryCode = CarryCode;
        PnrParam.PasList = pasList;
        PnrParam.SkyList = skyList;
        //只是儿童时需要备注的成人编码 
        PnrParam.AdultPnr = AdultPnr;
        //是否儿童出成人票
        PnrParam.ChildIsAdultEtdz = IsChdETDZAudltTK ? "1" : "0";
        //可选项
        PnrParam.UserName = mUser != null ? mUser.LoginName : "";

        //输入的手机号码 预订编码CT项电话
        if (FQP.KongZhiXiTong == null || !FQP.KongZhiXiTong.Contains("|19|"))
        {
            PnrParam.CTTel = mUser != null ? mUser.Tel : "";
            PnrParam.CTCTPhone = txtMobile.Text.Trim().Replace("'", "") != "" ? txtMobile.Text.Trim().Replace("'", "") : (mCompany != null && mCompany.ContactTel.Trim() != "" ? mCompany.ContactTel.Trim() : "");
        }
        else
        {
            PnrParam.CTTel = mSupCompany.Tel != null ? mSupCompany.Tel : "";
            PnrParam.CTCTPhone = txtMobile.Text.Trim().Replace("'", "") != "" ? txtMobile.Text.Trim().Replace("'", "") : (mSupCompany != null && mSupCompany.ContactTel.Trim() != "" ? mSupCompany.ContactTel.Trim() : "");
        }

        //关闭生成订单联系人默认值   生成订单时，联系人不需要默认值，让用户自己填写  
        if (FQP.KongZhiXiTong != null && FQP.KongZhiXiTong.Contains("|55|"))
        {
            PnrParam.CTTel = mUser != null ? mUser.Tel : "";
            PnrParam.CTCTPhone = txtMobile.Text.Trim().Replace("'", "");
        }
        //关闭落地运营商CTCT电话号码
        if (FQP.KongZhiXiTong == null || !FQP.KongZhiXiTong.Contains("|102|"))
        {
            //落地运营商公司电话号码
            PnrParam.LuoDiCTTel = mSupCompany.Tel != null ? mSupCompany.Tel : "";
            //落地运营商手机号码
            PnrParam.LuoDiCTCTPhone = (mSupCompany != null && mSupCompany.ContactTel.Trim() != "" ? mSupCompany.ContactTel.Trim() : "");
        }

        PnrParam.PID = this.configparam.Pid;
        PnrParam.KeyNo = this.configparam.KeyNo;
        //乘机人
        foreach (Tb_Ticket_Passenger pas in pList)
        {
            IPassenger p1 = new IPassenger();
            pasList.Add(p1);
            p1.PassengerName = pas.PassengerName;
            p1.PassengerType = pas.PassengerType;
            p1.PasSsrCardID = pas.Cid;
            p1.ChdBirthday = pas.A7;
            p1.CpyandNo = pas.A8;
        }
        //航段
        foreach (Tb_Ticket_SkyWay skyway in skywaylist)
        {
            ISkyLeg leg1 = new ISkyLeg();
            skyList.Add(leg1);
            leg1.CarryCode = skyway.CarryCode;
            leg1.FlightCode = skyway.FlightCode;
            leg1.FlyStartTime = skyway.FromDate.ToString("HHmm");
            leg1.FlyEndTime = skyway.ToDate.ToString("HHmm");
            leg1.FlyStartDate = skyway.FromDate.ToString("yyyy-MM-dd");
            leg1.fromCode = skyway.FromCityCode;
            leg1.toCode = skyway.ToCityCode;
            leg1.Space = skyway.Space;
            leg1.Discount = skyway.Discount;
        }
        RePnrObj pObj = pid.ISendIns(PnrParam);
        return pObj;
    }
    /// <summary>
    /// 获取航空公司出票Office号 
    /// </summary>
    /// <param name="CarryCode"></param>
    /// <param name="defaultOffice"></param>
    /// <returns></returns>
    public string GetPrintOffice(string CpyNo, string CarryCode)
    {
        string PrintOffice = "";
        string sqlWhere = string.Format(" CpyNo='{0}' and AirCode='{1}' ", CpyNo, CarryCode);
        List<Tb_Ticket_PrintOffice> list = this.baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_PrintOffice>;
        if (list != null && list.Count > 0)
        {
            if (!string.IsNullOrEmpty(list[0].OfficeCode))
            {
                PrintOffice = list[0].OfficeCode;
            }
        }
        return PrintOffice;
    }

    /// <summary>
    /// 验证儿童与成人航线是否一样
    /// </summary>
    /// <param name="AdtOrderId"></param>
    /// <param name="CHDSkyWay"></param>
    /// <returns></returns>
    public bool ValSkyWay(string AdtOrderId, List<Tb_Ticket_SkyWay> CHDSkyWay)
    {
        bool IsSuc = false;
        try
        {
            string sqlWhere = string.Format(" OrderId ='{0}'", AdtOrderId.Replace("\'", ""));
            List<Tb_Ticket_SkyWay> AdultSkyWay = this.baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_SkyWay>;
            if (CHDSkyWay != null && AdultSkyWay != null && CHDSkyWay.Count <= AdultSkyWay.Count)
            {
                //排序
                CHDSkyWay.Sort(delegate(Tb_Ticket_SkyWay _skyway0, Tb_Ticket_SkyWay _skyway1)
                {
                    return _skyway0.FromDate.CompareTo(_skyway1.FromDate) > 0 ? 1 : 0;
                });
                AdultSkyWay.Sort(delegate(Tb_Ticket_SkyWay _skyway0, Tb_Ticket_SkyWay _skyway1)
                {
                    return _skyway0.FromDate.CompareTo(_skyway1.FromDate) > 0 ? 1 : 0;
                });
                //               
                for (int i = 0; i < AdultSkyWay.Count; i++)
                {
                    if (CHDSkyWay.Count > 1 && CHDSkyWay.Count == AdultSkyWay.Count)
                    {
                        //多程必须每程都一样
                        if (CHDSkyWay[i].CarryCode.ToUpper() == AdultSkyWay[i].CarryCode.ToUpper() &&
                            CHDSkyWay[i].FlightCode == AdultSkyWay[i].FlightCode &&
                            (CHDSkyWay[i].FromCityCode == AdultSkyWay[i].FromCityCode || CHDSkyWay[i].FromCityCode == AdultSkyWay[i].FromCityName) &&
                            (CHDSkyWay[i].ToCityCode == AdultSkyWay[i].ToCityCode || CHDSkyWay[i].ToCityCode == AdultSkyWay[i].ToCityName) &&
                            CHDSkyWay[i].FromDate == AdultSkyWay[i].FromDate &&
                            CHDSkyWay[i].ToDate == AdultSkyWay[i].ToDate)
                        {
                            IsSuc = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //单程
                        if (CHDSkyWay[i].CarryCode.ToUpper() == AdultSkyWay[i].CarryCode.ToUpper() &&
                            CHDSkyWay[i].FlightCode == AdultSkyWay[i].FlightCode &&
                            (CHDSkyWay[i].FromCityCode == AdultSkyWay[i].FromCityCode || CHDSkyWay[i].FromCityCode == AdultSkyWay[i].FromCityName) &&
                            (CHDSkyWay[i].ToCityCode == AdultSkyWay[i].ToCityCode || CHDSkyWay[i].ToCityCode == AdultSkyWay[i].ToCityName) &&
                            CHDSkyWay[i].FromDate == AdultSkyWay[i].FromDate &&
                            CHDSkyWay[i].ToDate == AdultSkyWay[i].ToDate)
                        {
                            IsSuc = true;
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //DataBase.LogCommon.Log.Error("Create.aspx页面ValSkyWay", ex);
            PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:public bool ValSkyWay(string AdtOrderId, List<Tb_Ticket_SkyWay> CHDSkyWay)】================================================================\r\n参数：AdtOrderId=" + AdtOrderId + "  异常信息:" + ex.Message + ex.StackTrace + "\r\n", "Buy\\Create");
            IsSuc = false;
        }
        return IsSuc;
    }

    //生成订单
    protected void btnSub_Click(object sender, EventArgs e)
    {
        if (mSupCompany == null || mCompany == null || mUser == null || this.configparam == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + escape("该网页已失效,请刷新页面或者重新登录后再操作!") + "');", true);
            return;
        }
        string msg = "";
        bool IsAsAdultOrder = Hid_IsAsAdultOrder.Value == "1" ? true : false;
        FlightQueryParam FQP = ViewState["FlightQueryParam"] as FlightQueryParam;
        if (FQP != null)
        {
            //输入乘客数据
            FQP.PasData = Hid_PasData.Value.Replace("'", "");
            FQP.IsAsAdultOrder = IsAsAdultOrder;
            FQP.AllowChangePNRFlag = chkChangePnr.Checked;
            FQP.IsCHDToAudltTK = ckIsETDZAudltTK.Checked;
            FQP.IsRobTicketOrder = ckIsRobTicketOrder.Checked;
            FQP.CHDAssociationAdultOrderId = txtAdultOrder.Text.Trim().Replace("'", "");
            //生成订单
            bool IsSuc = GenerationOrder(FQP, out msg);
            if (IsSuc)
            {
                //清除页面视图
                Hid_ViewState.Value = "";
                //PnrAnalysis.LogText.LogWrite("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "跳转URl:" + msg + "\r\n", "GoConfirmation");
                //跳到确认页面            
                Response.Redirect(msg + "&SpecialType=" + FQP.SpecialType, false);
            }
            else
            {
                //PnrAnalysis.LogText.LogWrite("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "预订提示:" + msg + "\r\n", "GoConfirmation");
                if (msg.Contains("update Log_Pnr"))
                {
                    msg = "生成订单失败！";
                }
                else if (msg.Contains("Uq_OrderId_Cid_PassengerName"))
                {
                    msg = "乘客数据重复,生成订单失败！";
                }
                ScriptManager.RegisterStartupScript(this, GetType(), System.DateTime.Now.Ticks.ToString(), "showdialog('" + escape(msg) + "',1);", true);
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), System.DateTime.Now.Ticks.ToString(), "showdialog('" + escape("航班查询数据丢失，请刷新页面！") + "',1);", true);
        }
    }

    /// <summary>
    /// 判断预订编码落地运营商和供应是否设置配置信息
    /// </summary>
    /// <returns></returns>
    public bool ConfigIsSet(out string Msg)
    {
        bool IsSet = true;
        Msg = "";
        try
        {
            PbProject.Model.ConfigParam config = this.configparam;
            string UserCpyNo = mUser.CpyNo;
            if (config == null)
            {
                Msg = "编码预订没有找到可用的配置信息,请联系运营商设置";
                IsSet = false;
                return IsSet;
            }
            if (string.IsNullOrEmpty(config.WhiteScreenIP))
            {
                Msg = "预订配置信息没有设置IP地址,请联系运营商设置";
                IsSet = false;
            }
            else if (string.IsNullOrEmpty(config.WhiteScreenPort))
            {
                Msg = "预订配置信息没有设置端口号,请联系运营商设置";
                IsSet = false;
            }
            else if (string.IsNullOrEmpty(config.Office))
            {
                Msg = "预订配置信息可用Office号,请联系运营商设置";
                IsSet = false;
            }
            if (Msg == "")
            {
                Hid_IP.Value = config.WhiteScreenIP;
                Hid_Port.Value = config.WhiteScreenPort;
                Hid_Office.Value = config.Office;
            }
        }
        catch (Exception ex)
        {
            Msg = "预订编码配置信息异常,无法预订！";
            IsSet = false;
        }
        return IsSet;
    }
}
