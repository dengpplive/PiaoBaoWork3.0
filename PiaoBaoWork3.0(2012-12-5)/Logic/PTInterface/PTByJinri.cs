using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using System.Data;
using System.IO;
using System.Xml;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    ///今日天下通接口操作
    /// </summary>
    public class PTByJinri
    {
        #region 参数定义
        /// <summary>
        /// 今日接口帐号
        /// </summary>
        public string _todayAccout = "";
        /// <summary>
        /// 今日接口密码
        /// </summary>
        public string _todayAccout2 = "";
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

        w_TodayService.WTodayServiceSoapClient _todayService = new w_TodayService.WTodayServiceSoapClient();
        #endregion


        #region 实例赋值
        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public PTByJinri(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.User_Employees mUser, PbProject.Model.User_Company mTopcom, PbProject.Model.User_Company mCom)
        {

            _mTopcom = mTopcom;

            _mCom = mCom;

            _mUser = mUser;

            _order = Order;

            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();

            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(_mTopcom.UninCode.ToString());
            BS = WebCommon.Utility.BaseParams.getParams(ParList);

            _todayAccout = BS.JieKouZhangHao.Split('|')[4].Split('^')[0];

            _todayAccout2 = BS.JieKouZhangHao.Split('|')[4].Split('^')[1];

            _QXValue = BS.KongZhiXiTong;
            //BS.GongYingKongZhiFenXiao
        }
        #endregion


        #region 基础方法

        public List<PbProject.Model.Tb_Ticket_Policy> GetPolicy()
        {
            List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
            try
            {
                List<PbProject.Model.Tb_Ticket_SkyWay> SkyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere(" OrderId='" + _order.OrderId + "'");
                //if (_QXValue.Contains("|9|") && SkyList.Count < 2)
                //{
                string StartDate = SkyList[0].FromDate.ToShortDateString();
                string SecondDate = SkyList[0].FromDate.ToShortDateString();
                List<PbProject.Model.Tb_Ticket_Passenger> passengerList = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL().GetPasListBySQLWhere(" OrderId='" + _order.OrderId + "'");

                string PnrInfo = "O|P|" + _order.PNR + "^F^" + _order.BigCode + "|" + SkyList[0].FromDate.ToShortDateString() + "|" + SkyList[0].FromCityCode + "|" + SkyList[0].FromCityName + "|" + SkyList[0].ToCityCode + "|" + SkyList[0].ToCityName + "|" + SkyList[0].CarryCode + SkyList[0].FlightCode + "^N||" + SkyList[0].FromDate.ToShortTimeString() + "|" + SkyList[0].ToDate.ToShortTimeString() + "|" + SkyList[0].Space + "|" + SkyList[0].Discount + "||" + _order.PMFee / _order.PassengerName.Split('/').Length + "|" + (SkyList[0].ABFee + SkyList[0].FuelFee) + "|" + _order.PassengerNumber + "|" + _order.PassengerName.Replace("/", "@");
                DataSet dsToday = new DataSet();
                string sss = "";
                try
                {

                    sss = _todayService.GetRateListByPNR(_todayAccout, _order.PNR, PnrInfo);
                    StringReader sr = new StringReader(sss);
                    XmlTextReader xtr = new XmlTextReader(sr);
                    dsToday = new DataSet();
                    dsToday.ReadXml(xtr);
                }
                catch (Exception)
                {
                    //errorDataToday = "1";
                    //OnError("WebService异常，重新调用", "PiaoBao.BLLLogic.Policy.InterFacePol.GetTodayPol");
                    //dsToday = wsvcToday.GetRateListByPNR(Company.A63, PnrCode, PnrInfo);
                    sss = _todayService.GetRateListByPNR(_todayAccout, _order.PNR, PnrInfo);
                    StringReader sr = new StringReader(sss);
                    XmlTextReader xtr = new XmlTextReader(sr);
                    dsToday = new DataSet();
                    dsToday.ReadXml(xtr);
                }
                if (dsToday.Tables.Count > 1)
                {
                    if (dsToday.Tables[1].Rows.Count > 0)
                    {
                        if (dsToday.Tables[1].TableName == "Response")
                        {
                            DateTime dt1;
                            DateTime dt2;
                            if (DateTime.TryParse(StartDate.Replace("/", "-"), out dt1)
                                && DateTime.TryParse(SecondDate.Replace("/", "-"), out dt2)
                                )
                            {
                                string sqlwhere = "Sdate<='" + dt1.ToString("yyyy-MM-dd") + " 00:00:00' and Edate>='" + dt2.ToString("yyyy-MM-dd") + " 23:59:59'";
                                //  返回正常
                                mPolicyList = MergerTodayDT(dsToday.Tables[1].Select(sqlwhere), _mTopcom, SkyList[0].CarryCode, SkyList[0].FromCityCode, SkyList[0].ToCityCode);
                                //UpdateTodayInterFcae(dsToday);
                            }
                        }
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                //errorDataToday = "1";
                //OnError("整体线程异常，" + ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetTodayPol");
            }
            return mPolicyList;
        }

        private List<PbProject.Model.Tb_Ticket_Policy> MergerTodayDT(DataRow[] DrToday, User_Company GYCompany, string CarryCode, string FromCityCode, string ToCityCode)
        {
            try
            {
                List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
                for (int i = 0; i < DrToday.Length; i++)
                {
                    PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                    mPolicy.CpyNo = "0" + PbProject.Model.definitionParam.PolicySourceParam.bToday.ToString() + _mTopcom.UninCode;
                    mPolicy.CpyName = _mTopcom.UninName;
                    if (DrToday[i]["RateType"].ToString() == "0")//是否是特殊政策
                    {
                        mPolicy.PolicyKind = 0;
                        mPolicy.GenerationType = 1;
                    }
                    else
                    {
                        mPolicy.PolicyKind = 2;
                        mPolicy.GenerationType = 2;
                    }
                    mPolicy.CarryCode = CarryCode;
                    if (DrToday[i]["VoyageType"].ToString() == "0")
                    {
                        mPolicy.TravelType = 1;
                    }
                    else if (DrToday[i]["VoyageType"].ToString() == "2")
                    {
                        mPolicy.TravelType = 2;
                    }
                    else if (DrToday[i]["VoyageType"].ToString() == "1")
                    {
                        mPolicy.TravelType = 3;
                    }
                    else
                    {
                        mPolicy.TravelType = 4;
                    }
                    if (DrToday[i]["RateType"].ToString().ToUpper().Contains("B2P"))
                    {
                        mPolicy.PolicyType = 2;
                    }
                    else
                    {
                        mPolicy.PolicyType = 1;
                    }
                    if (DrToday[i]["UserType"].ToString() == "0")
                    {
                        mPolicy.TeamFlag = 0;
                    }
                    else
                    {
                        mPolicy.TeamFlag = 1;
                    }
                    mPolicy.StartCityNameCode = FromCityCode;
                    mPolicy.StartCityNameSame = 2;

                    mPolicy.TargetCityNameCode = ToCityCode;
                    mPolicy.TargetCityNameSame = 2;
                    //适用的航空公司
                    mPolicy.ApplianceFlight = DrToday[i]["AirComE"].ToString();
                    //不适用的航空公司
                    mPolicy.UnApplianceFlight = DrToday[i]["NoAirComE"].ToString();
                    mPolicy.ApplianceFlightType = 1;

                    mPolicy.ScheduleConstraints = "";
                    mPolicy.ShippingSpace = DrToday[i]["Cabin"].ToString();

                    mPolicy.FlightStartDate = Convert.ToDateTime(DrToday[i]["Sdate"]);
                    mPolicy.FlightEndDate = Convert.ToDateTime(DrToday[i]["Edate"]);
                    mPolicy.PrintStartDate = Convert.ToDateTime(DrToday[i]["Sdate"]);
                    mPolicy.PrintEndDate = Convert.ToDateTime(DrToday[i]["Edate"]);
                    mPolicy.AuditDate = DateTime.Now;
                    mPolicy.AuditType = 1;
                    mPolicy.Remark = DrToday[i]["Remark"].ToString();
                    mPolicy.IsApplyToShareFlight = 0;
                    mPolicy.ShareAirCode = "";
                    mPolicy.IsLowerOpen = 0;
                    mPolicy.DownPoint = decimal.Parse(DrToday[i]["Discounts"].ToString());
                    mPolicy.InterPolicyId = DrToday[i]["PolicyId"].ToString();
                    string workTime = "{0}-{1}";
                    workTime = string.Format(workTime, DrToday[i]["WorkTimeBegin"].ToString(), DrToday[i]["WorkTimeEnd"].ToString());
                    mPolicy._WorkTime = WorkTimeConvert(workTime, GYCompany.WorkTime);
                    mPolicy._FPGQTime = DrToday[i]["RefundTimeBegin"].ToString() + "-" + DrToday[i]["RefundTimeEnd"].ToString();
                    //Office
                    mPolicy.Office = (DrToday[i]["OfficeNum"] == DBNull.Value ? "" : DrToday[i]["OfficeNum"].ToString());// +"^" + (DrToday[i]["RateId"] == DBNull.Value ? "" : DrToday[i]["RateId"].ToString());
                    //供应商ID
                    mPolicy.A16 = DrToday[i]["RateId"].ToString();

                    mPolicyList.Add(mPolicy);

                    //DataRow dr = LocalDt.NewRow();
                    //dr["CarryCode"] = CarryCode;
                    //dr["ApplianceFlight"] = DrToday[i]["AirComE"].ToString().Replace(CarryCode, "");
                    //dr["UnApplianceFlight"] = DrToday[i]["NoAirComE"].ToString().Replace(CarryCode, "");
                    //dr["ScheduleConstraints"] = "1/2/3/4/5/6/7";
                    //dr["Space"] = "/" + DrToday[i]["Cabin"].ToString() + "/";
                    //dr["OldPolicy"] = decimal.Parse(DrToday[i]["Discounts"].ToString()) / 100;
                    //dr["GYPolicy"] = decimal.Parse(DrToday[i]["Discounts"].ToString()) / 100;
                    //dr["FXPolicy"] = decimal.Parse(DrToday[i]["Discounts"].ToString()) / 100;
                    //dr["PolicySource"] = "4";
                    //dr["PolicyId"] = DrToday[i]["PolicyId"];
                    //dr["PolicyType"] = DrToday[i]["RateType"].ToString().ToUpper().Contains("B2B") ? "1" : "2";

                    //workTime = string.Format(workTime, DrToday[i]["WorkTimeBegin"].ToString(), DrToday[i]["WorkTimeEnd"].ToString());
                    //dr["WorkTime"] = WorkTimeConvert(workTime, GYCompany.WorkTime);
                    //dr["BusinessTime"] = DrToday[i]["RefundTimeBegin"].ToString() + "-" + DrToday[i]["RefundTimeEnd"].ToString();
                    //dr["SpecialType"] = "0";
                    //dr["IsPause"] = "0";
                    //dr["IsLowerOpen"] = "0";
                    //dr["Remark"] = DrToday[i]["Remark"].ToString();
                    //dr["PolOffice"] = DrToday[i]["OfficeNum"] + "^" + DrToday[i]["RateId"];
                    //LocalDt.Rows.Add(dr);
                }
                return mPolicyList;
            }
            finally
            { }
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
            //string RTContent = RepalaceChar(skyList[0].Pat);

            string PNRinfo = "O|P|" + _order.PNR + "^F^" + _order.BigCode + "|" + skyList[0].FromDate.ToString("yyyy-MM-dd") + "|" + skyList[0].FromCityCode + "|" + skyList[0].FromCityName + "|" + skyList[0].ToCityCode + "|" + skyList[0].ToCityName + "|" + skyList[0].CarryCode + skyList[0].FlightCode + "^N||" + skyList[0].FromDate.ToShortTimeString() + "|" + skyList[0].ToDate.ToShortTimeString() + "|" + skyList[0].Space + "|" + skyList[0].Discount + "||" + _order.PMFee / _order.PassengerName.Split('/').Length + "|" + (skyList[0].ABFee + skyList[0].FuelFee) + "|" + _order.PassengerName.Split('/').Length + "|" + _order.PassengerName.Replace("/", "@");
            DataSet dsReson = _todayService.CreateOrderByPNR(_todayAccout2, _order.PNR, _order.JinriGYCode, (_order.PolicyPoint * 100).ToString(), _order.PolicyId, PNRinfo, "0");
            string mestodayCreate = "table's count:" + dsReson.Tables.Count + "&";

            for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                {
                    mestodayCreate = mestodayCreate + dsReson.Tables[0].Columns[i].ColumnName + ":" + dsReson.Tables[0].Rows[i][j].ToString() + "/";//DataTable转化成String类型
                }
            }
            mestodayCreate = "table1's name:" + dsReson.Tables[0].TableName + "/table1's content:" + mestodayCreate;
            if (dsReson != null)
            {
                if (dsReson.Tables[0].Rows[0]["OrderNo"].ToString() != "")
                {
                    CreateLog(_order.OrderId, "预定", "今日生成订单成功", 3);
                    if (dsReson.Tables[0].Rows[0]["PayMoney"].ToString() == "")
                    {
                        dsReson.Tables[0].Rows[0]["PayMoney"] = "0";
                    }
                    OnPay(Convert.ToDecimal(dsReson.Tables[0].Rows[0]["PayMoney"].ToString()), dsReson.Tables[0].Rows[0]["OrderNo"].ToString(), mestodayCreate);
                }
                else
                {
                    CreateLog(_order.OrderId, "预定", "今日生成订单失败", 3);
                    CreateLog(_order.OrderId, "预定", "今日生成订单失败:" + mestodayCreate, 1);
                }
            }
            //}
        }

        public void OnPay(decimal outpayfee, string outorderid, string mestodayCreate)
        {
            //if (_QXValue.Contains("31"))//是否自动代付
            //{
            if (outpayfee != _order.OutOrderPayMoney)//如果百拓价格比本地高，则不支付
            {
                CreateLog(_order.OrderId, "预定", "今日自动代付失败：平台订单价格和本地价格不符，不进行代付！", 3);
            }
            _order.OutOrderId = outorderid;
            _order.OutOrderPayMoney = outpayfee;
            DataSet dsResonPay = _todayService.AutoPayOrder(_todayAccout2, _order.OutOrderId);

            string mestodayPay = "";

            for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                {
                    mestodayPay = mestodayPay + dsResonPay.Tables[0].Columns[i].ColumnName + ":" + dsResonPay.Tables[0].Rows[i][j].ToString() + "/";
                }
            }
            mestodayCreate = mestodayCreate + "&51book代付：" + mestodayPay;

            if (dsResonPay != null && dsResonPay.Tables.Count > 0)
            {
                if (dsResonPay.Tables[0].Rows.Count > 0 && dsResonPay.Tables[0].Rows[0]["Result"].ToString() == "T")
                {
                    CreateLog(_order.OrderId, "预定", "今日代付成功！", 3);
                    _order.OrderStatusCode = 3;
                    _order.PayStatus = 2;
                    _order.OutOrderPayFlag = true;
                }
                else
                {
                    CreateLog(_order.OrderId, "预定", "今日代付失败：" + mestodayPay, 3);
                }
            }
            bool result = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new Object[] { _order });
            //}
        }

        public DataSet QueryOrder()
        {
            DataSet ds = _todayService.GetOrderInfo(_todayAccout2, _order.OutOrderId);
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
