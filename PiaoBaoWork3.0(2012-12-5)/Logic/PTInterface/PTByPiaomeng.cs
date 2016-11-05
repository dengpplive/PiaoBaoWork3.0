using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using PbProject.Logic.ControlBase;
using PbProject.Model;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    ///票盟接口操作
    /// </summary>
    public class PTByPiaomeng
    {

        #region 参数定义
        /// <summary>
        /// 票盟接口帐号
        /// </summary>
        public string _pmAccout = "";
        /// <summary>
        /// 票盟接口密码
        /// </summary>
        public string _pmPassword = "";
        /// <summary>
        /// 票盟接口安全码
        /// </summary>
        public string _pmAg = "";
        /// <summary>
        /// 平台控制供应开关
        /// </summary>
        public string _QXValue = "";

        /// <summary>
        /// 供应公司
        /// </summary>
        public PbProject.Model.User_Company _mTopcom;

        /// <summary>
        /// 采购公司（支付公司）
        /// </summary>
        public PbProject.Model.User_Company _mCom;

        /// <summary>
        /// 采购员（支付员）
        /// </summary>
        public PbProject.Model.User_Employees _mUser;

        /// <summary>
        /// Order
        /// </summary>
        public PbProject.Model.Tb_Ticket_Order _order;

        public BaseDataManage baseDataManage = new BaseDataManage();

        w_PMWebService.PMServiceSoapClient _pmService;
        #endregion


        #region 实例赋值
        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public PTByPiaomeng(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.User_Employees mUser, PbProject.Model.User_Company mTopcom, PbProject.Model.User_Company mCom)
        {

            _mTopcom = mTopcom;

            _mCom = new PbProject.Logic.User.User_CompanyBLL().GetCompany(mUser.CpyNo);

            _order = Order;

            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();

            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(_mTopcom.UninCode.ToString());
            BS = WebCommon.Utility.BaseParams.getParams(ParList);

            _pmAccout = BS.JieKouZhangHao.Split('|')[3].Split('^')[0];

            _pmPassword = BS.JieKouZhangHao.Split('|')[3].Split('^')[1];
            _pmAg = BS.JieKouZhangHao.Split('|')[3].Split('^')[2];

            _pmService = new w_PMWebService.PMServiceSoapClient();

            _QXValue = BS.KongZhiXiTong;
            //BS.GongYingKongZhiFenXiao
        }
        #endregion

        #region 基础方法

        public List<PbProject.Model.Tb_Ticket_Policy> GetPolicy(bool ChangePnr)
        {
            List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
            try
            {
                List<PbProject.Model.Tb_Ticket_SkyWay> SkyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere(" OrderId='" + _order.OrderId + "'");
                //if (_QXValue.Contains(""))
                //{
                string StartDate = "";
                string SecondDate = "";
                string StartFlyNo = "";
                string SecondFlyNo = "";
                string StartSpace = "";
                string SecondSpace = "";
                string StartCity = "";
                string SecondCity = "";
                string ThirdCity = "";
                string TravelType = "0";
                string uid = _pmAccout;
                string pwd = _pmAg;

                StartDate = SkyList[0].FromDate.ToShortDateString();
                StartFlyNo = SkyList[0].CarryCode + SkyList[0].FlightCode;
                StartSpace = SkyList[0].Space;
                StartCity = SkyList[0].FromCityCode;
                SecondCity = SkyList[0].ToCityCode;

                if (SkyList.Count > 1)
                {
                    TravelType = "1";
                    SecondDate = SkyList[1].FromDate.ToShortDateString();
                    SecondFlyNo = SkyList[1].CarryCode + SkyList[1].FlightCode;
                    SecondSpace = SkyList[1].Space;
                    if (SkyList[0].FromCityCode != SkyList[1].ToCityCode)
                    {
                        TravelType = "2";
                        ThirdCity = SkyList[1].ToCityCode;
                    }
                }
                DataSet dsPM = new DataSet();
                bool IfTry = true;
                try
                {
                    if (TravelType == "0")
                    {
                        dsPM = _pmService.GetPolicyDataByDC(StartDate, StartFlyNo, StartCity, SecondCity, "", StartSpace, uid, pwd);
                    }
                    else
                    {
                        string citytemp = "";
                        //往返
                        if (TravelType == "1")
                        {
                            citytemp = StartCity;
                        }
                        //联程
                        else
                        {
                            citytemp = ThirdCity;
                        }
                        dsPM = _pmService.GetPolicyDataByWFLC(TravelType, StartDate, SecondDate, StartFlyNo, SecondFlyNo, StartCity, SecondCity, SecondCity, citytemp, "", StartSpace, SecondSpace, uid, pwd);
                    }
                }
                catch (Exception e)
                {
                    //errorDataPM = "1";
                    IfTry = false;
                    //OnError(e.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetPMPol");
                }
                if (IfTry == false)
                {
                    try
                    {
                        if (TravelType == "0")
                        {
                            dsPM = _pmService.GetPolicyDataByDC(StartDate, StartFlyNo, StartCity, SecondCity, "", StartSpace, uid, pwd);
                        }
                        else
                        {
                            string citytemp = "";
                            //往返
                            if (TravelType == "1")
                            {
                                citytemp = StartCity;
                            }
                            //联程
                            else
                            {
                                citytemp = ThirdCity;
                            }
                            dsPM = _pmService.GetPolicyDataByWFLC(TravelType, StartDate, SecondDate, StartFlyNo, SecondFlyNo, StartCity, SecondCity, SecondCity, citytemp, "", StartSpace, SecondSpace, uid, pwd);
                        }
                    }
                    catch (Exception e)
                    {
                        //errorDataPM = "1";
                        IfTry = false;
                        //OnError(e.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetPMPol");
                    }
                }
                if (dsPM.Tables.Count > 1 && dsPM.Tables[1].Rows.Count > 0)
                {
                    try
                    {
                        mPolicyList = MergerPMDT(dsPM.Tables[1].Select(), SkyList[0].FromCityCode, SkyList[0].ToCityCode, SkyList[0].CarryCode, SkyList[0].FlightCode + "/" + SkyList[SkyList.Count - 1].FlightCode, _mTopcom, StartDate.Replace("/", "-"), SecondDate.Replace("/", "-"), ChangePnr);
                        //updatePMInterFcae(dsPM);
                    }
                    catch (Exception ex)
                    {
                        //errorDataPM = "1";
                        //OnError(ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetPMPol");
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                //OnError("整体线程异常，" + ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetPMPol");
            }
            //DataPMOk = true;
            return mPolicyList;
        }

        /// <summary>
        /// 将票盟政策合并到原始政策dt中去
        /// </summary>
        /// <param name="LocalDt">原始dt</param>
        /// <param name="DrPM">票盟dt</param>
        private List<PbProject.Model.Tb_Ticket_Policy> MergerPMDT(DataRow[] DrPM, string FromCityCode, string ToCityCode, string CarrCode, string FlyNo, PbProject.Model.User_Company GYCompany, string
            StartDate, string SecondDate, bool ChangePnr)
        {
            try
            {
                List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
                for (int i = 0; i < DrPM.Length; i++)
                {
                    if (SecondDate == "")
                    {
                        if (DateTime.Parse(DrPM[i]["fromtime"].ToString()) > DateTime.Parse(StartDate))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (DateTime.Parse(DrPM[i]["fromtime"].ToString()) > DateTime.Parse(StartDate) || DateTime.Parse(DrPM[i]["totime"].ToString()) < DateTime.Parse(SecondDate))
                        {
                            continue;
                        }
                    }




                    //如果不允许换编码并且政策是必须换编码出票的,则过滤掉,yyy 2013-6-7update
                    if (!ChangePnr && DrPM[i]["changerecord"].ToString() == "1")
                    {
                        continue;
                    }
                    PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                    mPolicy.CpyNo = "0" + PbProject.Model.definitionParam.PolicySourceParam.bPiaoMeng.ToString() + _mTopcom.UninCode;
                    mPolicy.CpyName = _mTopcom.UninName;
                    if (DrPM[i]["isspecmark"].ToString() == "0")//是否是特殊政策
                    {
                        mPolicy.PolicyKind = 0;
                        mPolicy.GenerationType = 1;
                    }
                    else
                    {
                        mPolicy.PolicyKind = 2;
                        mPolicy.GenerationType = 2;
                    }
                    mPolicy.CarryCode = CarrCode;
                    mPolicy.TravelType = 1;
                    if (DrPM[i]["policytype"].ToString().ToUpper().Contains("B2P"))
                    {
                        mPolicy.PolicyType = 2;
                    }
                    else
                    {
                        mPolicy.PolicyType = 1;
                    }
                    mPolicy.TeamFlag = 0;
                    mPolicy.StartCityNameCode = FromCityCode;
                    mPolicy.StartCityNameSame = 2;
                    mPolicy.TargetCityNameCode = ToCityCode;
                    mPolicy.TargetCityNameSame = 2;
                    mPolicy.ApplianceFlight = FlyNo;
                    mPolicy.UnApplianceFlight = "";
                    mPolicy.ApplianceFlightType = 2;

                    mPolicy.ScheduleConstraints = "";
                    mPolicy.ShippingSpace = DrPM[i]["applyclass"].ToString().Replace(",", "/");

                    mPolicy.FlightStartDate = Convert.ToDateTime(DrPM[i]["fromtime"]);
                    mPolicy.FlightEndDate = Convert.ToDateTime(DrPM[i]["totime"]);
                    mPolicy.PrintStartDate = Convert.ToDateTime(DrPM[i]["fromtime"]);
                    mPolicy.PrintEndDate = Convert.ToDateTime(DrPM[i]["totime"]);
                    mPolicy.AuditDate = DateTime.Now;
                    mPolicy.AuditType = 1;
                    string isChangePnr = "";
                    if (DrPM[i]["changerecord"].ToString() == "1")
                    {
                        isChangePnr = "须换编码出票.";
                    }
                    mPolicy.Remark = isChangePnr+" "+DrPM[i]["note"].ToString();
                    mPolicy.IsApplyToShareFlight = 0;
                    mPolicy.ShareAirCode = "";
                    mPolicy.IsLowerOpen = 0;
                    mPolicy.DownPoint = decimal.Parse(DrPM[i]["rate"].ToString());
                    mPolicy.InterPolicyId = DrPM[i]["id"].ToString();
                    mPolicy._WorkTime = WorkTimeConvert(TimeTemp(DrPM[i]["worktime"].ToString(), 0), GYCompany.WorkTime);
                    mPolicy._FPGQTime = TimeTemp(DrPM[i]["worktime"].ToString().Substring(0, 4) + "-" + DrPM[i]["RefundWorkTimeTo"].ToString(), 0);
                    mPolicy.Office = DrPM[i]["officeid"] == DBNull.Value ? "" : DrPM[i]["officeid"].ToString();

                    mPolicyList.Add(mPolicy);



                    //DataRow dr = LocalDt.NewRow();
                    //dr["CarryCode"] = CarrCode;
                    //dr["ApplianceFlight"] = FlyNo;
                    //dr["ScheduleConstraints"] = "1/2/3/4/5/6/7";
                    //dr["Space"] = "/" + DrPM[i]["applyclass"].ToString().Replace(",", "/") + "/";
                    //dr["OldPolicy"] = decimal.Parse(DrPM[i]["rate"].ToString()) / 100;
                    //dr["GYPolicy"] = decimal.Parse(DrPM[i]["rate"].ToString()) / 100;
                    //dr["FXPolicy"] = decimal.Parse(DrPM[i]["rate"].ToString()) / 100;
                    //dr["PolicySource"] = "5";
                    //dr["PolicyId"] = DrPM[i]["id"];
                    //dr["PolicyType"] = DrPM[i]["policytype"].ToString().ToUpper().Contains("B2B") ? "1" : "2";
                    //dr["WorkTime"] = WorkTimeConvert(TimeTemp(DrPM[i]["worktime"].ToString(), 0), GYCompany.WorkTime);
                    ////dr["BusinessTime"] = TimeTemp(DrPM[i]["worktime"].ToString(), -1);
                    //dr["BusinessTime"] = TimeTemp(DrPM[i]["worktime"].ToString().Substring(0, 4) + "-" + DrPM[i]["RefundWorkTimeTo"].ToString(), 0);

                    //dr["SpecialType"] = "0";
                    //dr["IsPause"] = "0";
                    //dr["IsLowerOpen"] = "0";
                    //dr["Remark"] = DrPM[i]["note"].ToString();
                    //dr["PolOffice"] = DrPM[i]["officeid"];
                    //LocalDt.Rows.Add(dr);
                }

                return mPolicyList;
            }
            finally
            { }
        }

        /// <summary>
        /// 时间格式调整
        /// </summary>
        /// <param name="timStr"></param>
        /// <param name="OutTime"></param>
        /// <returns></returns>
        private string TimeTemp(string timStr, double OutTime)
        {
            string[] times = timStr.Split('-');
            string timeNew = "";
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i].Length == 2)
                {
                    times[i] = times[i].ToString() + "00";
                }
                if (times[i].Length == 3)
                {
                    times[i] = "0" + times[i];
                }
                times[i] = times[i].Insert(2, ":");
                if (i == times.Length - 1)
                {
                    times[i] = DateTime.Parse(times[i].ToString()).AddHours(OutTime).ToShortTimeString();
                }
                if (times[i].ToString() == "0:00")
                {
                    times[i] = "00:00";
                }
                timeNew = timeNew + times[i] + "-";
            }
            return timeNew.Substring(0, timeNew.Length - 1);
        }

        /// <summary>
        /// 工作时间转换
        /// </summary>
        /// <param name="OldTime">原始时间</param>
        /// <param name="GYTime">供应时间</param>
        /// <returns></returns>
        public string WorkTimeConvert(string OldTime, string GYTime)
        {
            string[] symbols = new string[]{
                "/",
                "^",
                ".",
                "~",
                "#",
                "&",
                "*",
                "|"
            };
            if (GYTime == null || GYTime == "")
            {
                GYTime = "08:00-23:00";
            }
            foreach (string symbol in symbols)
            {
                OldTime = OldTime.Replace(symbol, "-");
                GYTime = GYTime.Replace(symbol, "-");
            }
            string[] OldTimeList = OldTime.Split('-');
            string[] GYTimeList = GYTime.Split('-');
            string[] NewTimeList = new string[2];
            try
            {
                if (OldTimeList[0].IndexOf(":") < 0)
                {
                    OldTimeList[0] = OldTimeList[0].Substring(0, 2) + ":" + OldTimeList[0].Substring(2, 2);
                }
                if (GYTimeList[0].IndexOf(":") < 0)
                {
                    GYTimeList[0] = GYTimeList[0].Substring(0, 2) + ":" + GYTimeList[0].Substring(2, 2);
                }
                if (OldTimeList[1].IndexOf(":") < 0)
                {
                    OldTimeList[1] = OldTimeList[1].Substring(0, 2) + ":" + OldTimeList[1].Substring(2, 2);
                }
                if (GYTimeList[1].IndexOf(":") < 0)
                {
                    GYTimeList[1] = GYTimeList[1].Substring(0, 2) + ":" + GYTimeList[1].Substring(2, 2);
                }
                if (DateTime.Compare(DateTime.Parse(OldTimeList[0]), DateTime.Parse(GYTimeList[0])) <= 0)
                {
                    NewTimeList[0] = GYTimeList[0];
                }
                else
                {
                    NewTimeList[0] = OldTimeList[0];
                }
                if (DateTime.Compare(DateTime.Parse(OldTimeList[1]), DateTime.Parse(GYTimeList[1])) >= 0)
                {
                    NewTimeList[1] = GYTimeList[1];
                }
                else
                {
                    NewTimeList[1] = OldTimeList[1];
                }
            }
            catch (Exception e)
            {
                //OnError("类：PolicyMatching中方法WorkTimeConvert报错：" + e.ToString(), "WorkTimeConvert");
            }
            return NewTimeList[0] + "-" + NewTimeList[1];
        }

        public void OnCreateOrder()
        {
            //if (_QXValue.Contains("|62|"))//控制是否自动生成订单
            //{

            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + _order.OrderId + "'");
            string RTContent = skyList[0].NewPnrContent.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            PnrAnalysis.FormatPNR ss = new PnrAnalysis.FormatPNR();
            string PATContent = ss.RemoveHideChar(skyList[0].Pat).Replace("\r", "").Replace("\t", "").Replace("\n", "");
            #region 处理高低开
            if (!_order.Space.Contains("1"))//不为子舱位
            {
                string bb = "";
                PnrAnalysis.PatModel sss = ss.GetPATInfo(skyList[0].NewPnrContent, out bb);
                if (sss.ChildPat != null)
                {
                    for (int i = 0; i < sss.PatList.Count; i++)
                    {
                        if (sss.PatList[i].SeatGroup == sss.ChildPat.SeatGroup)
                        {
                            sss.PatList.Remove(sss.PatList[i]);
                            break;
                        }
                    }
                    PnrAnalysis.PatInfo patFirst = sss.PatList[0];
                    PnrAnalysis.PatInfo patLast = sss.PatList[sss.PatList.Count - 1];
                    //if (_QXValue.Contains("|60|"))//低开开关是否打开
                    //{
                    PATContent = ss.NewPatData(patFirst);
                    //}
                    //else
                    //{
                    //    PATContent = ss.NewPatData(patLast);
                    //}
                    bool IsOnePrice = false;
                    RTContent = ss.RemoveChildSeat(RTContent, out IsOnePrice);
                }
            }
            #endregion

            //OnErrorNew(1, "票盟开始生成订单", "票盟生成订单");
            DataSet dsReson = _pmService.CreateOrderByPAT(_order.PolicyId, _order.BigCode, HttpUtility.UrlEncode(RTContent), HttpUtility.UrlEncode(PATContent), "0", _pmAccout, _pmAg);

            string mesPMCreate = "table's count:" + dsReson.Tables.Count + "&";

            if (dsReson.Tables.Count <= 1)
            {
                for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                    {
                        mesPMCreate = mesPMCreate + dsReson.Tables[0].Columns[i].ColumnName + ":" + dsReson.Tables[0].Rows[i][j].ToString() + "/";//DataTable转化成String类型
                    }
                }
                mesPMCreate = "table1's name:" + dsReson.Tables[0].TableName + "/table1's content:" + mesPMCreate;
                CreateLog(_order.OrderId, "预定", mesPMCreate, 1);
                //OnErrorNew(1, "票盟生成订单失败", "票盟生成订单");
            }
            else
            {
                mesPMCreate = mesPMCreate + "&";
                for (int i = 0; i < dsReson.Tables[1].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[1].Columns.Count; j++)
                    {
                        mesPMCreate = mesPMCreate + dsReson.Tables[1].Columns[i].ColumnName + ":" + dsReson.Tables[1].Rows[i][j].ToString() + "/";
                    }
                }
                mesPMCreate = "table2's name:" + dsReson.Tables[0].TableName + "/table2's content:" + mesPMCreate;

                if (dsReson.Tables[0].Rows[0]["statuscode"].ToString() == "0")
                {
                    CreateLog(_order.OrderId, "预定", "票盟生成订单成功！", 3);
                    OnPay(Convert.ToDecimal(dsReson.Tables[1].Rows[0]["payfee"].ToString()), dsReson.Tables[1].Rows[0]["orderid"].ToString(), mesPMCreate);
                    //OnErrorNew(1, "票盟生成订单成功", "票盟生成订单");
                }
                else
                {
                    //票盟生成订单失败
                    //OnErrorNew(1, 票盟生成订单失败, "票盟生成订单");
                    CreateLog(_order.OrderId, "预定", "票盟生成订单失败：" + mesPMCreate, 3);
                }
            }
            //OnErrorNew(1, mesPMCreate, "PMdataset");
            //}
        }

        public void OnPay(decimal outpayfee, string outorderid, string mesPMCreate)
        {
            //if (_QXValue.Contains("31"))//是否自动代付
            //{
            if (outpayfee != _order.OutOrderPayMoney)//如果百拓价格比本地高，则不支付
            {
                CreateLog(_order.OrderId, "预定", "票盟自动代付失败：平台订单价格和本地价格不符，不进行代付！", 3);
            }
            _order.OutOrderId = outorderid;
            _order.OutOrderPayMoney = outpayfee;
            DataSet dsResonPay = _pmService.PMPay(_order.OutOrderId, _pmAccout, _pmAg);
            if (dsResonPay != null)
            {
                string mesPMPay = "";

                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mesPMPay = mesPMPay + dsResonPay.Tables[0].Columns[i].ColumnName + ":" + dsResonPay.Tables[0].Rows[i][j].ToString() + "/";
                    }
                }
                mesPMCreate = mesPMCreate + "&票盟代付：" + mesPMPay;
                if (dsResonPay.Tables[0].Rows[0]["statuscode"].ToString() == "0")
                {
                    _order.OrderStatusCode = 3;
                    _order.PayStatus = 2;
                    _order.OutOrderPayFlag = true;
                    CreateLog(_order.OrderId, "预定", "票盟代付成功！", 3);
                }
                else
                {
                    CreateLog(_order.OrderId, "预定", "票盟代付失败：" + mesPMPay, 3);
                }
            }
            bool result = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new Object[] { _order });
            //}
        }

        /// <summary>
        /// 外部订单查询
        /// </summary>
        /// <param name="outorderid">外部订单号</param>
        /// <returns></returns>
        public DataSet QueryOrder(string outorderid)
        {
            DataSet ds = _pmService.PMOrderQuery(outorderid, _pmAccout, _pmAg);
            return ds;
        }

        /// <summary>
        /// 添加订单操作日志
        /// </summary>
        /// <param name="OrderId">订单编号</param>
        /// <param name="OperType">操作类型</param>
        /// <param name="OperContent">操作内容</param>
        /// <param name="WatchType">查看权限</param>
        private void CreateLog(string OrderId, string OperType, string OperContent, int WatchType)
        {

            Model.Log_Tb_AirOrder model = new Model.Log_Tb_AirOrder();

            #region 参数赋值
            //model.id = Guid.NewGuid();
            model.OrderId = OrderId;
            model.CpyName = _mCom.UninName;
            model.CpyNo = _mCom.UninCode;
            model.CpyType = _mCom.RoleType;
            model.OperContent = OperContent;
            model.OperLoginName = _mUser.LoginName;
            model.OperTime = DateTime.Now;
            model.OperType = OperType;
            model.OperUserName = _mUser.UserName;
            model.WatchType = WatchType;
            #endregion

            bool result = (bool)baseDataManage.CallMethod("Log_Tb_AirOrder", "Insert", null, new Object[] { model });
        }
        #endregion
    }
}
