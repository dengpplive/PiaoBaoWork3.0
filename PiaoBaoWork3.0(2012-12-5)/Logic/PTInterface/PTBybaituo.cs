using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using System.Xml;
using System.Data;
using System.IO;
using System.Security.Cryptography;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    ///百拓接口操作
    /// </summary>
    public class PTBybaituo
    {
        #region 参数定义
        /// <summary>
        /// 百拓接口帐号
        /// </summary>
        public string _baiTuoAccout = "";
        /// <summary>
        /// 百拓接口密码
        /// </summary>
        public string _baiTuoPassword = "";
        /// <summary>
        /// 百拓接口安全码
        /// </summary>
        public string _baiTuoAg = "";


        /// <summary>
        /// 百拓接口参数
        /// </summary>
        public string PseudoCityCode = "DFW";
        public string ISOCountry = "US";
        public string ISOCurrency = "USD";
        public string AirlineVendorID = "AA";
        public string AirportCode = "IDA";
        public string Url = "http://provider1.org/OTAEngine";
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
        w_BTWebService.BaiTuoWebSoapClient _baiTuoService = new w_BTWebService.BaiTuoWebSoapClient();
        #endregion


        #region 实例赋值
        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public PTBybaituo(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.definitionParam.BaseSwitch BS)
        {

            _order = Order;
            List<PbProject.Model.User_Company> mtCom = new PbProject.Logic.ControlBase.BaseDataManage().
                 CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + Order.OwnerCpyNo.Substring(0, 12) + "'" }) as List<PbProject.Model.User_Company>;
            if (mtCom != null && mtCom.Count > 0)
            {
                _mTopcom = mtCom[0];
            }

            _baiTuoAccout = BS.JieKouZhangHao.Split('|')[2].Split('^')[0];

            _baiTuoPassword = BS.JieKouZhangHao.Split('|')[2].Split('^')[1];
            _baiTuoAg = BS.JieKouZhangHao.Split('|')[2].Split('^')[2];



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
                List<PbProject.Model.Tb_Ticket_SkyWay> SkyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + _order.OrderId + "'");
                //if (_QXValue.Contains("") && SkyList.Count <= 2)
                //{
                string StartDate = "";
                string SecondDate = "";
                string StartFlyNo = "";
                string SecondFlyNo = "";
                string StartSpace = "";
                string SecondSpace = "";
                string StartCity = "";
                string SecondCity = "";
                string TravelType = "1";

                StartDate = SkyList[0].FromDate.ToShortDateString() + "T" + SkyList[0].FromDate.ToShortTimeString();
                StartFlyNo = SkyList[0].CarryCode + SkyList[0].FlightCode;
                StartSpace = SkyList[0].Space;
                StartCity = SkyList[0].FromCityCode;
                SecondCity = SkyList[0].ToCityCode;

                if (SkyList.Count > 1)
                {
                    SecondDate = SkyList[1].FromDate.ToShortDateString() + "T" + SkyList[1].FromDate.ToShortTimeString();
                    SecondFlyNo = SkyList[1].CarryCode + SkyList[1].FlightCode;
                    SecondSpace = SkyList[1].Space;
                    TravelType = "2";
                }
                DataSet ds = new DataSet();
                try
                {
                    XmlElement xmlElement = BaiTuoSSPolicySend(StartDate, StartFlyNo, StartSpace, StartCity, SecondCity, TravelType, SecondFlyNo, SecondSpace, SecondDate);
                    string xmlNode = _baiTuoService.GetDomesticMatchNormalZRateStr(xmlElement.InnerXml);
                    StringReader rea = new StringReader(xmlNode);
                    XmlTextReader xmlReader = new XmlTextReader(rea);
                    ds.ReadXml(xmlReader);

                }
                catch (Exception e)
                {
                    //errorDataBaiTuo = "1";
                    //OnError(e.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetBaiTuoPol");
                }
                if (ds.Tables.Count > 0)
                {
                    try
                    {
                        StartDate = StartDate.Replace("/", "-");
                        SecondDate = SecondDate.Replace("/", "-");
                        mPolicyList = MergerBaiTuoDT(ds.Tables[0].Select("Effdate<='" + StartDate + "' and Expdate>='" + SecondDate + "'"), SkyList[0].FromCityCode, SkyList[0].ToCityCode, SkyList[0].CarryCode, SkyList[0].FlightCode + "/" + SkyList[SkyList.Count - 1].FlightCode, StartSpace + "/" + SecondSpace, SkyList[0].FromDate.ToShortDateString(), ChangePnr);
                        //updatebaituoInterFcae(ds);
                    }
                    catch (Exception ex)
                    {
                        //errorDataBaiTuo = "1";
                        //OnError(ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetBaiTuoPol");
                    }
                }
                //}

            }
            catch (Exception ex)
            {
                //errorDataBaiTuo = "1";
                //OnError("整体线程异常，" + ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.GetBaiTuoPol");
            }
            return mPolicyList;
        }

        /// <summary>
        /// 将百托政策合并到原始政策dt中去
        /// </summary>
        /// <param name="LocalDt">原始dt</param>
        /// <param name="DrBaiTuo">百托dt</param>
        private List<PbProject.Model.Tb_Ticket_Policy> MergerBaiTuoDT(DataRow[] DrBaiTuo, string FromCityCode, string ToCityCode, string Carrcode, string FlyNo, string space, string FromDate, bool ChangePnr)
        {
            try
            {
                List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
                for (int i = 0; i < DrBaiTuo.Length; i++)
                {



                    //如果不允许换编码并且政策是必须换编码出票的,则过滤掉,yyy 2013-6-7update
                    if (!ChangePnr && DrBaiTuo[i]["ChangePnr"].ToString() == "1")
                    {
                        continue;
                    }


                    PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                    mPolicy.CpyNo = "0" + PbProject.Model.definitionParam.PolicySourceParam.baiTuo.ToString() + _order.OwnerCpyNo.Substring(0, 12);
                    //mPolicy.CpyName = _order.o;
                    mPolicy.PolicyKind = 1;
                    mPolicy.GenerationType = 1;
                    mPolicy.CarryCode = Carrcode;
                    //if (DrBaiTuo[i]["TravelType"].ToString() == "1")
                    //{
                    mPolicy.TravelType = 1;
                    //}
                    //else if (DrBaiTuo[i]["TravelType"].ToString() == "3")
                    //{
                    //    mPolicy.TravelType = 2;
                    //}
                    //else if (DrBaiTuo[i]["TravelType"].ToString() == "2")
                    //{
                    //    mPolicy.TravelType = 3;
                    //}
                    //else
                    //{
                    //    mPolicy.TravelType = 4;
                    //}

                    if (DrBaiTuo[i]["PolicyType"].ToString() == "1")
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
                    mPolicy.ApplianceFlightType = 2;

                    mPolicy.ScheduleConstraints = "";
                    mPolicy.ShippingSpace = space;

                    mPolicy.FlightStartDate = Convert.ToDateTime(DrBaiTuo[i]["Effdate"]);
                    mPolicy.FlightEndDate = Convert.ToDateTime(DrBaiTuo[i]["Expdate"]);
                    mPolicy.PrintStartDate = Convert.ToDateTime(DrBaiTuo[i]["Effdate"]);
                    mPolicy.PrintEndDate = Convert.ToDateTime(DrBaiTuo[i]["Expdate"]);
                    mPolicy.AuditDate = DateTime.Now;
                    mPolicy.AuditType = 1;
                    string isChangePnr = "";
                    if (DrBaiTuo[i]["ChangePnr"].ToString() == "1")
                    {
                        isChangePnr = "须换编码出票.";
                    }

                    mPolicy.Remark = isChangePnr+" "+DrBaiTuo[i]["Remark"].ToString();
                    mPolicy.IsApplyToShareFlight = 0;
                    mPolicy.ShareAirCode = "";
                    mPolicy.IsLowerOpen = 0;
                    mPolicy.DownPoint = decimal.Parse(DrBaiTuo[i]["Rate"].ToString()) * 100;
                    mPolicy.InterPolicyId = DrBaiTuo[i]["Id"].ToString();

                    string[] times = DrBaiTuo[i]["ProviderWorkTime"].ToString().Split(',');
                    string[] times2 = DrBaiTuo[i]["VoidWorkTime"].ToString().Split(',');
                    int index = 0;
                    DayOfWeek date = DateTime.Parse(FromDate).DayOfWeek;
                    if (date == DayOfWeek.Tuesday)
                    {
                        index = 1;
                    }
                    if (date == DayOfWeek.Wednesday)
                    {
                        index = 2;
                    }
                    if (date == DayOfWeek.Thursday)
                    {
                        index = 3;
                    }
                    if (date == DayOfWeek.Friday)
                    {
                        index = 4;
                    }
                    if (date == DayOfWeek.Saturday)
                    {
                        index = 5;
                    }
                    if (date == DayOfWeek.Sunday)
                    {
                        index = 6;
                    }
                    if (DrBaiTuo[i]["ProviderWorkTime"].ToString() != "" && DrBaiTuo[i]["ProviderWorkTime"].ToString() != null)
                    {
                        mPolicy._WorkTime = WorkTimeConvert(times[index], _mTopcom.WorkTime);
                        mPolicy._FPGQTime = times2[index];
                    }
                    else
                    {
                        mPolicy._WorkTime = _mTopcom.WorkTime.Replace("/", "-");//"9:00-18:00";
                        mPolicy._FPGQTime = _mTopcom.BusinessTime.Replace("/", "-");//"9:00-17:30";
                    }
                    mPolicy.Office = DrBaiTuo[i]["Office"] == DBNull.Value ? "" : DrBaiTuo[i]["Office"].ToString();
                    mPolicyList.Add(mPolicy);



                    //DataRow dr = LocalDt.NewRow();
                    //string[] times = DrBaiTuo[i]["ProviderWorkTime"].ToString().Split(',');
                    //string[] times2 = DrBaiTuo[i]["VoidWorkTime"].ToString().Split(',');
                    //dr["CarryCode"] = Carrcode;
                    //dr["ApplianceFlight"] = FlyNo;
                    //dr["ScheduleConstraints"] = "1/2/3/4/5/6/7";
                    //dr["Space"] = "/" + space + "/";
                    //dr["OldPolicy"] = DrBaiTuo[i]["Rate"].ToString();
                    //dr["GYPolicy"] = DrBaiTuo[i]["Rate"].ToString();
                    //dr["FXPolicy"] = DrBaiTuo[i]["Rate"].ToString();
                    //dr["PolicySource"] = "2";
                    //dr["PolicyId"] = DrBaiTuo[i]["Id"];
                    //dr["PolicyType"] = DrBaiTuo[i]["PolicyType"];
                    //DayOfWeek date = DateTime.Parse(FromDate).DayOfWeek;
                    //int index = 0;
                    //if (date == DayOfWeek.Tuesday)
                    //{
                    //    index = 1;
                    //}
                    //if (date == DayOfWeek.Wednesday)
                    //{
                    //    index = 2;
                    //}
                    //if (date == DayOfWeek.Thursday)
                    //{
                    //    index = 3;
                    //}
                    //if (date == DayOfWeek.Friday)
                    //{
                    //    index = 4;
                    //}
                    //if (date == DayOfWeek.Saturday)
                    //{
                    //    index = 5;
                    //}
                    //if (date == DayOfWeek.Sunday)
                    //{
                    //    index = 6;
                    //}
                    //if (DrBaiTuo[i]["ProviderWorkTime"].ToString() != "" && DrBaiTuo[i]["ProviderWorkTime"].ToString() != null)
                    //{
                    //    dr["WorkTime"] = WorkTimeConvert(times[index], GYCompany.WorkTime);
                    //    dr["BusinessTime"] = times2[index];
                    //}
                    //else
                    //{
                    //    dr["WorkTime"] = GYCompany.WorkTime.Replace("/", "-");//"9:00-18:00";
                    //    dr["BusinessTime"] = GYCompany.BusinessTime.Replace("/", "-");//"9:00-17:30";
                    //}
                    //dr["SpecialType"] = "0";
                    //dr["IsPause"] = "0";
                    //dr["IsLowerOpen"] = "0";
                    //dr["Remark"] = DrBaiTuo[i]["Remark"];
                    //dr["PolOffice"] = DrBaiTuo[i]["Office"];
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

            //OnErrorNew(1, "开始生成百拓订单", "百拓生成订单");
            string xmlNode = CreateBaiTuoOrder();
            DataSet dsReson = new DataSet();
            StringReader rea = new StringReader("<BAITOUR_ORDER_CREATE_RS>" + xmlNode + "</BAITOUR_ORDER_CREATE_RS>");
            XmlTextReader xmlReader = new XmlTextReader(rea);
            dsReson.ReadXml(xmlReader);
            string mesBaituoCreate = "";
            for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                {
                    mesBaituoCreate = mesBaituoCreate + dsReson.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                }

                mesBaituoCreate = mesBaituoCreate + "|";
            }
            if (dsReson != null)
            {
                if (dsReson.Tables[0].TableName == "Errors")
                {
                    CreateLog(_order.OrderId, "预定", "百拓生成订单失败：" + mesBaituoCreate, 3);
                }
                if (dsReson.Tables[0].Rows[0]["forderformid"].ToString() != "" && dsReson.Tables[0].Rows[0]["shouldPay"].ToString() != "")
                {
                    CreateLog(_order.OrderId, "预定", "百拓生成订单成功", 3);
                    if (dsReson.Tables[0].Rows[0]["shouldPay"].ToString() == "")
                    {
                        dsReson.Tables[0].Rows[0]["shouldPay"] = "0";
                    }
                    OnPay(Convert.ToDecimal(dsReson.Tables[0].Rows[0]["shouldPay"].ToString()), dsReson.Tables[0].Rows[0]["forderformid"].ToString(), mesBaituoCreate);
                }
            }
            //}
        }

        public void OnPay(decimal outpayfee, string outorderid, string mesBaituoCreate)
        {
            //if (_QXValue.Contains("31"))//是否自动代付
            //{
            if (outpayfee != _order.OutOrderPayMoney)//如果百拓价格比本地高，则不支付
            {
                CreateLog(_order.OrderId, "预定", "百拓自动代付失败：平台订单价格和本地价格不符，不进行代付！", 3);
            }
            _order.OutOrderId = outorderid;
            _order.OutOrderPayMoney = outpayfee;

            string Message = "";
            try
            {

                string SendURL = BaiTuoPaySend(_order.OrderId, "1");
                if (SendURL != "")
                {
                    Message = _baiTuoService.GetUrlData(SendURL);
                }
            }
            catch (Exception ex)
            {
                CreateLog(_order.OrderId, "预定", "百拓自动代付失败", 3);
                CreateLog(_order.OrderId, "预定", "百拓自动代付失败:" + ex.ToString(), 1);
                Message = "";
            }
            if (Message != "")
            {
                if (Message.Substring(Message.IndexOf("<PaymentResult>") + "<PaymentResult>".Length, 1) == "T" || Message.Substring(Message.IndexOf("<PaymentResult>") + "<PaymentResult>".Length, 1) == "1")
                {
                    CreateLog(_order.OrderId, "预定", "百拓代付成功！", 3);
                    _order.OrderStatusCode = 3;
                    _order.PayStatus = 2;
                    _order.OutOrderPayFlag = true;
                }
                else
                {
                    CreateLog(_order.OrderId, "预定", "百拓自动代付失败", 3);
                    CreateLog(_order.OrderId, "预定", "百拓后台代付失败:" + Message.Substring(Message.IndexOf("<PaymentResult>")), 1);
                }
            }
            else
            {
                CreateLog(_order.OrderId, "预定", "百拓自动代付失败", 3);
                CreateLog(_order.OrderId, "预定", "百拓后台代付失败:Message为空", 1);
            }
            bool result = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new Object[] { _order });
            //}
        }

        public DataSet QueryOrder()
        {
            XmlElement xe = BaiTuoCpSend(_order.OutOrderId);
            System.Xml.Linq.XElement XNew = System.Xml.Linq.XElement.Parse(xe.InnerXml);
            System.Xml.Linq.XElement xml = _baiTuoService.getOrderInfoXml(XNew);
            DataSet ds = new DataSet();
            ds = BaiTuoCpReceive(xml);
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

            model.CpyName = _order.OwnerCpyName;
            model.CpyNo = _order.OwnerCpyNo;
            //model.CpyType = _mCom.RoleType;
            model.OperContent = OperContent;

            model.OperLoginName = _order.CreateLoginName;
            model.OperTime = DateTime.Now;
            model.OperType = OperType;
            model.OperUserName = _order.CreateUserName;
            model.WatchType = WatchType;
            #endregion

            bool result = (bool)baseDataManage.CallMethod("Log_Tb_AirOrder", "Insert", null, new Object[] { model });
        }
        #endregion


        #region 百拓基础方法

        /// <summary>
        /// 百拓生成订单接口调用
        /// </summary>
        /// <param name="OrderId">本地订单编号</param>
        private string CreateBaiTuoOrder()
        {
            string xmlNode = null;
            DataTable dtPassenger = StructPassenger();
            string[] OrderArray = StructOrder();

            XmlElement xmlElementCreateOrder = BaiTuoCreateOrderSend(OrderArray, dtPassenger);
            //OnErrorNew(1, xmlElementCreateOrder.InnerXml, "baituodataset");
            xmlNode = _baiTuoService.pnrCreateOrderExStr(xmlElementCreateOrder.InnerXml);
            return xmlNode;
        }


        /// <summary>
        /// 百拓实时获取政策
        /// </summary>
        /// <param name="DepartureDateTime">出发时间</param>
        /// <param name="FlightNumber">航班号</param>
        /// <param name="ResBookDesigCode">舱位</param>
        /// <param name="DepartureAirport">出发城市三字码</param>
        /// <param name="ArrivalAirport">到达城市三字码</param>
        /// <param name="TripType">行程类型：1，单程 2，往返</param>
        /// <param name="FlightNumberBack">回程航班号</param>
        /// <param name="ResBookDesigCodeBack">回程舱位</param>
        /// <param name="DepartureDateTimeBack">回程出发时间</param>
        /// <returns>返回处理好的XML节点</returns>
        public XmlElement BaiTuoSSPolicySend(string DepartureDateTime, string FlightNumber, string ResBookDesigCode, string DepartureAirport, string ArrivalAirport, string TripType, string FlightNumberBack, string ResBookDesigCodeBack, string DepartureDateTimeBack)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            XmlElement xe3 = doc.CreateElement("OTA_AirFareRQ");
            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", "DFW");
                xe2.SetAttribute("ISOCountry", "US");
                xe2.SetAttribute("ISOCurrency", "USD");
                xe2.SetAttribute("AirlineVendorID", "AA");
                xe2.SetAttribute("AirportCode", "IAD");
                XmlElement xe4 = doc.CreateElement("RequestorID");
                xe4.SetAttribute("URL", Url);
                xe4.SetAttribute("Type", "6");
                xe4.SetAttribute("ID", _baiTuoPassword);
                xe2.AppendChild(xe4);
                xe1.AppendChild(xe2);
                XmlElement xe5 = doc.CreateElement("FlightSegmentFares");
                XmlElement xe6 = doc.CreateElement("FlightSegment");
                xe6.SetAttribute("DepartureDateTime", ConvertDateTime(DepartureDateTime.Replace(' ', 'T')));
                xe6.SetAttribute("FlightNumber", FlightNumber);
                xe6.SetAttribute("ResBookDesigCode", ResBookDesigCode);
                xe6.SetAttribute("DepartureAirport", DepartureAirport);
                xe6.SetAttribute("ArrivalAirport", ArrivalAirport);
                xe6.SetAttribute("ReturnPolicyType", "2");
                if (TripType != "1")
                {
                    xe6.SetAttribute("TripType", TripType);
                    xe6.SetAttribute("FlightNumberBack", FlightNumberBack);
                    xe6.SetAttribute("ResBookDesigCodeBack", ResBookDesigCodeBack);
                    xe6.SetAttribute("DepartureDateTimeBack", ConvertDateTime(DepartureDateTimeBack.Replace(' ', 'T')));
                }
                xe5.AppendChild(xe6);
                xe3.AppendChild(xe1);
                xe3.AppendChild(xe5);
                xe3.InnerXml = "<OTA_AirFareRQ>" + xe3.InnerXml + "</OTA_AirFareRQ>";
            }
            catch
            {
                return xe3;
            }
            return xe3;
        }


        /// <summary>
        /// 百拓出票传入XML拼接
        /// </summary>
        /// <param name="OrderId">百拓机票订单编号</param>
        /// <returns>返回处理好的XML节点</returns>
        public XmlElement BaiTuoCpSend(string OrderId)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            XmlElement xe6 = doc.CreateElement("ORDER_INFO_RQ");

            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", PseudoCityCode);
                xe2.SetAttribute("ISOCountry", ISOCountry);
                xe2.SetAttribute("ISOCurrency", ISOCurrency);
                xe2.SetAttribute("AirlineVendorID", AirlineVendorID);
                xe2.SetAttribute("AirportCode", AirportCode);
                XmlElement xe3 = doc.CreateElement("RequestorID");
                xe3.SetAttribute("URL", Url);
                xe3.SetAttribute("Type", "11");
                xe3.SetAttribute("ID", _baiTuoPassword);
                //将节点附加到父节点中
                xe2.AppendChild(xe3);
                xe1.AppendChild(xe2);
                //创建父节点
                XmlElement xe4 = doc.CreateElement("OrderInfo");
                XmlElement xe5 = doc.CreateElement("OrderID");
                //在节点中添加属性与对应的值
                xe5.InnerText = OrderId;
                //将节点附加到父节点中
                xe4.AppendChild(xe5);
                //将节点附加到父节点中
                xe6.AppendChild(xe1);
                xe6.AppendChild(xe4);
            }
            catch
            {
                return xe6;
            }
            //返回匹配好的XML节点
            return xe6;
        }

        /// <summary>
        /// 百拓出票调用政策返回数据处理
        /// </summary>
        /// <returns>返回读取的DataSet集合对象</returns>
        public DataSet BaiTuoCpReceive(XmlNode xmlNode)
        {
            DataSet ds = new DataSet();
            try
            {
                StringReader rea = new StringReader("<OTA_Alter_AirFareRS>" + xmlNode.InnerXml + "</OTA_Alter_AirFareRS>");
                XmlTextReader xmlReader = new XmlTextReader(rea);
                ds.ReadXml(xmlReader);
            }
            catch
            {
                return ds;
            }
            return ds;
        }

        /// <summary>
        /// 百拓出票调用政策返回数据处理
        /// </summary>
        /// <returns>返回读取的DataSet集合对象</returns>
        public DataSet BaiTuoCpReceive(System.Xml.Linq.XElement xmlNode)
        {
            DataSet ds = new DataSet();
            try
            {
                StringReader rea = new StringReader("<OTA_Alter_AirFareRS>" + xmlNode.Value + "</OTA_Alter_AirFareRS>");
                XmlTextReader xmlReader = new XmlTextReader(rea);
                ds.ReadXml(xmlReader);
            }
            catch
            {
                return ds;
            }
            return ds;
        }

        /// <summary>
        /// 百拓订单取消、退票、废票传入XML拼接
        /// </summary>
        /// <param name="OrderId">百拓机票订单编号</param>
        /// <param name="BaiTuoOrderId">外部订单号</param>
        /// <param name="MessageApp">申请理由</param>
        /// <returns>返回处理好的XML节点</returns>
        public XmlElement BaiTuoQuitOrderSend(string OrderId, string BaiTuoOrderId, string MessageApp, int Type)
        {
            List<Tb_Ticket_SkyWay> SkyWayList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere(" OrderId='" + _order.OrderId + "'");
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            //返回匹配好的XML节点
            XmlElement xe8 = doc.CreateElement("Order_Refund_RQ");

            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", PseudoCityCode);
                xe2.SetAttribute("ISOCountry", ISOCountry);
                xe2.SetAttribute("ISOCurrency", ISOCurrency);
                xe2.SetAttribute("AirlineVendorID", AirlineVendorID);
                xe2.SetAttribute("AirportCode", AirportCode);
                XmlElement xe3 = doc.CreateElement("RequestorID");
                xe3.SetAttribute("URL", Url);
                xe3.SetAttribute("Type", "9");
                xe3.SetAttribute("ID", _baiTuoPassword);
                //创建父节点
                XmlElement xe4 = doc.CreateElement("RefundOrderInfo");
                xe4.SetAttribute("ForderformID", BaiTuoOrderId);
                xe4.SetAttribute("InternationalTicket", "0");
                xe4.SetAttribute("RefundType", Type.ToString());
                xe4.SetAttribute("UserFare", "0");

                xe4.SetAttribute("UserFetchInFare", "0");
                xe4.SetAttribute("RefundSrc", "0");
                XmlElement xe5 = doc.CreateElement("RefundFlightSegments");
                XmlElement xe6 = doc.CreateElement("RefundFlightSegment");
                //在节点中添加属性与对应的值
                xe6.SetAttribute("DepartureAirport", SkyWayList[0].FromCityCode);
                xe6.SetAttribute("ArrivalAirport", SkyWayList[0].ToCityCode);

                string[] PassengerName = _order.PassengerName.Split('/');
                for (int i = 0; i < PassengerName.Length; i++)
                {
                    if (PassengerName[i].ToString() != "")
                    {
                        XmlElement xe7 = doc.CreateElement("PersonInfo");
                        xe7.SetAttribute("PersonName", PassengerName[i].ToString());
                        xe6.AppendChild(xe7);
                    }
                }

                XmlElement xe9 = doc.CreateElement("Remark");
                xe9.InnerText = MessageApp;

                //将节点附加到父节点中
                xe5.AppendChild(xe6);
                xe4.AppendChild(xe5);
                xe4.AppendChild(xe9);
                xe2.AppendChild(xe3);
                xe1.AppendChild(xe2);
                xe8.AppendChild(xe1);
                xe8.AppendChild(xe4);
            }
            catch
            {
                return xe8;
            }
            return xe8;
        }

        /// <summary>
        /// 百拓订单取消、退票、废票解析返回信息
        /// </summary>
        /// <param name="xmlNode">返回的XML数据</param>
        /// <returns>返回解析好的DataSet集合对象</returns>
        public DataSet BaiTuoQuitOrderReceive(XmlNode xmlNode)
        {
            DataSet ds = new DataSet();
            try
            {
                StringReader rea = new StringReader("<Order_Refund_RS>" + xmlNode.InnerXml + "</Order_Refund_RS>");
                XmlTextReader xmlReader = new XmlTextReader(rea);
                ds.ReadXml(xmlReader);
            }
            catch
            {
                return ds;
            }
            return ds;
        }

        /// <summary>
        /// 创建百拓机票生成乘机人信息DataTable集合
        /// </summary>
        /// <returns>返回创建好的DataTable集合</returns>
        public DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            //乘机人姓名
            dt.Columns.Add("PassengerName");
            //PNR
            dt.Columns.Add("PNR");
            //乘客类型
            dt.Columns.Add("PassengerType");
            //证件类型
            dt.Columns.Add("CType");
            //证件号码
            dt.Columns.Add("Cid");
            //国籍
            dt.Columns.Add("NationalityId");
            //性别
            dt.Columns.Add("Gender");
            //出生日期
            dt.Columns.Add("Birthday");
            //购买保险标识
            dt.Columns.Add("Insurance");

            return dt;
        }

        /// <summary>
        /// 百拓订单生成传入参数XML配置
        /// </summary>
        /// <param name="values">数组对象1:政策编号2:行程类型 1单程 2往返3:是否有保险 0没有 1有4:本地订单编号5:出发时间6:到达时间7:航班号8:机型9:出发城市三字码10:到达城市三字码11:舱位12:票面价13:结算价14:基建费15:燃油费16:联系人17：联系电话</param>
        /// <param name="dt">乘机人信息DatatTable集合</param>
        /// <returns>返回拼接好的XMLElement对象</returns>
        public XmlElement BaiTuoCreateOrderSend(string[] values, DataTable dt)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            XmlElement xe14 = doc.CreateElement("ORDER_CREATE_RQ");
            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", "2U8");
                xe2.SetAttribute("ISOCountry", "CN");
                xe2.SetAttribute("ISOCurrency", "USD");
                xe2.SetAttribute("AirlineVendorID", "AA");
                xe2.SetAttribute("AirportCode", "IAD");
                XmlElement xe3 = doc.CreateElement("RequestorID");
                xe3.SetAttribute("URL", Url);
                xe3.SetAttribute("Type", "4");
                xe3.SetAttribute("ID", _baiTuoPassword);

                XmlElement xe4 = doc.CreateElement("OrderInfo");
                xe4.SetAttribute("PolicyId", values[0]);
                xe4.SetAttribute("InternationalTicket", "0");
                xe4.SetAttribute("TripTypeCode", values[1]);
                xe4.SetAttribute("OrderSrc", "1");
                xe4.SetAttribute("InsuranceType", values[2]);
                xe4.SetAttribute("LocalOrderID", values[3]);

                XmlElement xe5 = doc.CreateElement("FlightSegments");
                if (values[4].Split('/').Length == 2)
                {
                    if (values[8].Split('/')[1].ToString() != "")
                    {
                        for (int x = 0; x < values[8].Split('/').Length; x++)
                        {
                            XmlElement xe6 = doc.CreateElement("FlightInfo");
                            xe6.SetAttribute("DepartureDatetime", ConvertDateTime(values[4].Split('/')[x].ToString()));
                            xe6.SetAttribute("ArrivalDatetime", ConvertDateTime(values[5].Split('/')[x].ToString()));
                            xe6.SetAttribute("FlightNumber", values[6].Split('/')[x].ToString());
                            xe6.SetAttribute("PlaneStyle", values[7].Split('/')[x].ToString());

                            XmlElement xe7 = doc.CreateElement("DepartureAirport");
                            xe7.SetAttribute("LocationCode", values[8].Split('/')[x].ToString());
                            XmlElement xe8 = doc.CreateElement("ArrivalAirport");
                            xe8.SetAttribute("LocationCode", values[9].Split('/')[x].ToString());
                            XmlElement xe9 = doc.CreateElement("Cabin");
                            xe9.SetAttribute("Code", values[10].Split('/')[x].ToString());
                            xe9.SetAttribute("Price", Convert.ToString(values[11].Split('/')[x].ToString().Split('.')[x].ToString()));
                            xe9.SetAttribute("AgentPrice", Convert.ToString(Math.Round(double.Parse(values[12].Split('/')[x].ToString()), 2)));
                            xe9.SetAttribute("Tax", Convert.ToString(values[13].Split('/')[x].ToString().Split('.')[x].ToString()));
                            xe9.SetAttribute("YQTax", Convert.ToString(values[14].Split('/')[x].ToString().Split('.')[x].ToString()));
                            xe9.SetAttribute("SubCode", "");
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                XmlElement xe10 = doc.CreateElement("RPH");
                                xe10.SetAttribute("personName", dt.Rows[i]["PassengerName"].ToString());
                                xe10.SetAttribute("PNR", dt.Rows[i]["PNR"].ToString());
                                xe10.SetAttribute("Insurance", dt.Rows[i]["Insurance"].ToString());
                                xe10.SetAttribute("BigPNR", values[17].ToString());
                                xe9.AppendChild(xe10);
                            }
                            xe6.AppendChild(xe7);
                            xe6.AppendChild(xe8);
                            xe6.AppendChild(xe9);
                            xe5.AppendChild(xe6);
                        }
                    }
                    else
                    {
                        XmlElement xe6 = doc.CreateElement("FlightInfo");
                        xe6.SetAttribute("DepartureDatetime", ConvertDateTime(values[4].Split('/')[0].ToString()));
                        xe6.SetAttribute("ArrivalDatetime", ConvertDateTime(values[5].Split('/')[0].ToString()));
                        xe6.SetAttribute("FlightNumber", values[6].Split('/')[0].ToString());
                        xe6.SetAttribute("PlaneStyle", values[7].Split('/')[0].ToString());

                        XmlElement xe7 = doc.CreateElement("DepartureAirport");
                        xe7.SetAttribute("LocationCode", values[8].Split('/')[0].ToString());
                        XmlElement xe8 = doc.CreateElement("ArrivalAirport");
                        xe8.SetAttribute("LocationCode", values[9].Split('/')[0].ToString());
                        XmlElement xe9 = doc.CreateElement("Cabin");
                        xe9.SetAttribute("Code", values[10].Split('/')[0].ToString());
                        xe9.SetAttribute("Price", Convert.ToString(values[11].Split('/')[0].ToString().Split('.')[0].ToString()));
                        xe9.SetAttribute("AgentPrice", Convert.ToString(Math.Round(double.Parse(values[12].Split('/')[0].ToString()), 2)));
                        xe9.SetAttribute("Tax", Convert.ToString(values[13].Split('/')[0].ToString().Split('.')[0].ToString()));
                        xe9.SetAttribute("YQTax", Convert.ToString(values[14].Split('/')[0].ToString().Split('.')[0].ToString()));
                        xe9.SetAttribute("SubCode", "");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            XmlElement xe10 = doc.CreateElement("RPH");
                            xe10.SetAttribute("personName", dt.Rows[i]["PassengerName"].ToString());
                            xe10.SetAttribute("PNR", dt.Rows[i]["PNR"].ToString());
                            xe10.SetAttribute("Insurance", "0");
                            xe10.SetAttribute("BigPNR", values[17].ToString());
                            xe9.AppendChild(xe10);
                        }
                        xe6.AppendChild(xe7);
                        xe6.AppendChild(xe8);
                        xe6.AppendChild(xe9);
                        xe5.AppendChild(xe6);
                    }
                }
                else
                {
                    XmlElement xe6 = doc.CreateElement("FlightInfo");
                    xe6.SetAttribute("DepartureDatetime", ConvertDateTime(values[4].Split('/')[0].ToString()));
                    xe6.SetAttribute("ArrivalDatetime", ConvertDateTime(values[5].Split('/')[0].ToString()));
                    xe6.SetAttribute("FlightNumber", values[6].Split('/')[0].ToString());
                    xe6.SetAttribute("PlaneStyle", values[7].Split('/')[0].ToString());

                    XmlElement xe7 = doc.CreateElement("DepartureAirport");
                    xe7.SetAttribute("LocationCode", values[8].Split('/')[0].ToString());
                    XmlElement xe8 = doc.CreateElement("ArrivalAirport");
                    xe8.SetAttribute("LocationCode", values[9].Split('/')[0].ToString());
                    XmlElement xe9 = doc.CreateElement("Cabin");
                    xe9.SetAttribute("Code", values[10].Split('/')[0].ToString());
                    xe9.SetAttribute("Price", Convert.ToString(values[11].Split('/')[0].ToString().Split('.')[0].ToString()));
                    string AgentPrice = Convert.ToString(Math.Round(double.Parse(values[12].Split('/')[0].ToString()), 2));
                    if (AgentPrice.Split('.').Length < 2)
                    {
                        xe9.SetAttribute("AgentPrice", Convert.ToString(AgentPrice + ".00"));
                    }
                    else
                    {
                        if (AgentPrice.Split('.')[1].Length == 1)
                        {
                            AgentPrice = AgentPrice + "0";
                        }
                        xe9.SetAttribute("AgentPrice", AgentPrice);
                    }
                    xe9.SetAttribute("Tax", Convert.ToString(values[13].Split('/')[0].ToString().Split('.')[0].ToString()));
                    xe9.SetAttribute("YQTax", Convert.ToString(values[14].Split('/')[0].ToString().Split('.')[0].ToString()));
                    xe9.SetAttribute("SubCode", "");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        XmlElement xe10 = doc.CreateElement("RPH");
                        xe10.SetAttribute("personName", dt.Rows[i]["PassengerName"].ToString());
                        xe10.SetAttribute("PNR", dt.Rows[i]["PNR"].ToString());
                        xe10.SetAttribute("Insurance", "0");
                        xe10.SetAttribute("BigPNR", values[17].ToString());
                        xe9.AppendChild(xe10);
                    }
                    xe6.AppendChild(xe7);
                    xe6.AppendChild(xe8);
                    xe6.AppendChild(xe9);
                    xe5.AppendChild(xe6);
                }
                XmlElement xe11 = doc.CreateElement("TravelerInfo");
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    XmlElement xe12 = doc.CreateElement("personInfo");
                    XmlElement xe20 = doc.CreateElement("personName");
                    XmlElement xe21 = doc.CreateElement("personType");
                    XmlElement xe22 = doc.CreateElement("CertifyID");
                    XmlElement xe23 = doc.CreateElement("IdNumber");
                    XmlElement xe24 = doc.CreateElement("NationalityId");
                    XmlElement xe25 = doc.CreateElement("Gender");
                    XmlElement xe26 = doc.CreateElement("Birthday");
                    xe20.InnerText = dt.Rows[j]["PassengerName"].ToString();
                    xe21.InnerText = dt.Rows[j]["PassengerType"].ToString();
                    xe22.InnerText = dt.Rows[j]["CType"].ToString();
                    xe23.InnerText = dt.Rows[j]["Cid"].ToString();
                    xe24.InnerText = dt.Rows[j]["NationalityId"].ToString();
                    xe25.InnerText = dt.Rows[j]["Gender"].ToString();
                    xe26.InnerText = dt.Rows[j]["Birthday"].ToString();
                    xe12.AppendChild(xe20);
                    xe12.AppendChild(xe21);
                    xe12.AppendChild(xe22);
                    xe12.AppendChild(xe23);
                    xe12.AppendChild(xe24);
                    xe12.AppendChild(xe25);
                    xe12.AppendChild(xe26);
                    xe11.AppendChild(xe12);
                }
                XmlElement xe13 = doc.CreateElement("Remark");
                xe13.SetAttribute("SysRemark", "合作伙伴接口政策实时预定！");
                XmlElement xe15 = doc.CreateElement("LinkmanInfo");
                xe15.SetAttribute("Name", values[15].ToString());
                xe15.SetAttribute("MobilePhone", values[16].ToString());
                xe2.AppendChild(xe3);
                xe1.AppendChild(xe2);
                xe14.AppendChild(xe1);
                xe4.AppendChild(xe5);
                xe4.AppendChild(xe11);
                xe4.AppendChild(xe15);
                xe4.AppendChild(xe13);
                xe14.AppendChild(xe4);
            }
            catch
            {
                return xe14;
            }
            return xe14;
        }

        /// <summary>
        /// 根据订单编号构建数据对象兵赋值
        /// </summary>
        /// <param name="OrderId">订单编号</param>
        /// <returns>返回构建与赋值好的string类型数组对象</returns>
        public string[] StructOrder()
        {
            //根据订单编号获得行程信息、订单信息
            List<Tb_Ticket_SkyWay> SkyWayList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere(" OrderId='" + _order.OrderId + "'");
            List<Tb_Ticket_Passenger> PassengerList = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL().GetPasListBySQLWhere(" OrderId='" + _order.OrderId + "'");
            //声明数组对象参数信息
            string[] OrderArray = new string[18];
            //构造数组中参数的值
            OrderArray[0] = _order.PolicyId;
            OrderArray[1] = _order.TravelType.ToString();
            OrderArray[2] = "0";
            OrderArray[3] = _order.OrderId;

            #region 行程参数声明并赋值

            //起飞时间
            string FromTime = "";
            //到达时间
            string ToTime = "";
            //起飞城市三字码
            string FromCityCode = "";
            //到达城市三字码
            string ToCityCode = "";
            //航班号
            string FlightCode = "";
            //机型
            string AircraftModel = "";
            //舱位
            string Space = "";
            //销售价
            string PMFee = "";
            //本地结算价
            string JSFee = "";
            //基建费
            string ABFee = "";
            //燃油费
            string FuelFee = "";
            foreach (Tb_Ticket_SkyWay SkyWay in SkyWayList)
            {
                FromTime = FromTime + SkyWay.FromDate.ToString().Replace(' ', 'T') + "/";
                ToTime = ToTime + SkyWay.ToDate.ToString().Replace(' ', 'T') + "/";
                FromCityCode = FromCityCode + SkyWay.FromCityCode + "/";
                ToCityCode = ToCityCode + SkyWay.ToCityCode + "/";
                FlightCode = FlightCode + SkyWay.CarryCode + SkyWay.FlightCode + "/";
                AircraftModel = AircraftModel + SkyWay.Aircraft + "/";
                Space = Space + SkyWay.Space + "/";
                decimal PMFees = SkyWay.SpacePrice;
                string s = PMFees.ToString().Split('.')[0].ToString();
                int s1 = int.Parse(s.Substring(s.Length - 1, 1));
                if (s1 > 0 && s1 <= 4)
                {
                    PMFees = PMFees - s1;
                }
                else if (s1 >= 5)
                {
                    PMFees = PMFees - s1;
                    PMFees = PMFees + 10;
                }
                PMFee = PMFees.ToString() + "/";
                JSFee = JSFee + Convert.ToString(PMFees * (1 - _order.PolicyPoint)) + "/";
                ABFee = ABFee + (SkyWay.ABFee).ToString() + "/";
                FuelFee = FuelFee + (SkyWay.FuelFee).ToString() + "/";
            }

            #endregion

            OrderArray[4] = FromTime.Replace("/", "-").Substring(0, FromTime.Length - 1);
            OrderArray[5] = ToTime.Replace("/", "-").Substring(0, ToTime.Length - 1);
            OrderArray[6] = FlightCode.Substring(0, FlightCode.Length - 1);
            OrderArray[7] = AircraftModel.Substring(0, AircraftModel.Length - 1);
            OrderArray[8] = FromCityCode.Substring(0, FromCityCode.Length - 1);
            OrderArray[9] = ToCityCode.Substring(0, ToCityCode.Length - 1);
            OrderArray[10] = Space.Substring(0, Space.Length - 1);
            OrderArray[11] = PMFee.Substring(0, PMFee.Length - 1);
            OrderArray[12] = JSFee.Substring(0, JSFee.Length - 1);
            OrderArray[13] = ABFee.Substring(0, ABFee.Length - 1);
            OrderArray[14] = FuelFee.Substring(0, FuelFee.Length - 1);
            OrderArray[15] = _order.LinkMan;
            OrderArray[16] = _order.LinkManPhone;
            OrderArray[17] = _order.BigCode;

            return OrderArray;
        }

        /// <summary>
        /// 根据订单编号获取乘客信息并赋值到构造好的DataTable中
        /// </summary>
        /// <param name="OrderId">订单编号</param>
        /// <returns>返回赋值好的DataTable</returns>
        public DataTable StructPassenger()
        {
            //声明乘机人信息实体并根据订单编号获取乘机人信息集合

            List<Tb_Ticket_Passenger> PassengerList = new Order.Tb_Ticket_PassengerBLL().GetPasListBySQLWhere(" OrderId='" + _order.OrderId + "'");
            //实例化创建好的DataTable对象
            DataTable dt = CreateDataTable();

            //循环将乘机人信息赋值到构造好的空DataTable对象中
            foreach (Tb_Ticket_Passenger Passenger in PassengerList)
            {
                DataRow dr = dt.NewRow();
                //乘客姓名
                dr["PassengerName"] = Passenger.PassengerName;
                //乘客PNR
                dr["PNR"] = _order.PNR;
                //乘客类型
                dr["PassengerType"] = Passenger.PassengerType;
                //证件类型
                dr["CType"] = ReturnPassengerCType(Passenger.CType);
                //证件号码
                dr["Cid"] = Passenger.Cid;
                //国籍
                dr["NationalityId"] = "45";
                //性别
                dr["Gender"] = "0";
                //出生日期
                dr["Birthday"] = "1999-1-1";
                //购买保险标识
                dr["Insurance"] = "0";

                //添加到构造好的DataTable中
                dt.Rows.Add(dr);
            }

            //返回DataTable
            return dt;
        }

        /// <summary>
        /// 百拓机票订单生成返回字符窜解析并返回DataSet集合对象
        /// </summary>
        /// <param name="xmlNode">返回的XMLNode对象</param>
        /// <returns>返回处理好的DataSet集合</returns>
        public bool BaiTuoCreateOrderReceive(XmlNode xmlNode, int Id)
        {
            bool IsSuccess = false;
            DataSet ds = new DataSet();
            try
            {
                StringReader rea = new StringReader("<BAITOUR_ORDER_CREATE_RS>" + xmlNode.InnerXml + "</BAITOUR_ORDER_CREATE_RS>");
                XmlTextReader xmlReader = new XmlTextReader(rea);
                ds.ReadXml(xmlReader);
                //对解析出的XML信息进行判断
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {
                        //判断返回的信息
                        if (ds.Tables[i].Rows[j]["forderformid"].ToString() != "" && ds.Tables[i].Rows[j]["shouldPay"].ToString() != "")
                        {
                            //对外部生成的订单根据返回的信息进行赋值
                            //外部订单生成的订单编号
                            _order.OutOrderId = ds.Tables[i].Rows[j]["forderformid"].ToString();
                            //外部订单支付金额
                            _order.OutOrderPayMoney = decimal.Parse(ds.Tables[i].Rows[j]["shouldPay"].ToString());
                            //修改订单信息
                            bool result = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new Object[] { _order });
                            if (result)
                            {
                                IsSuccess = true;
                            }
                        }
                    }
                }
            }
            catch
            {
                return IsSuccess;
            }
            return IsSuccess;
        }

        /// <summary>
        /// 返回乘客的证件类型在百拓中相对应的证件类型
        /// </summary>
        /// <param name="Type">乘机人证件类型编号</param>
        /// <returns>返回处理后的证件类型编号</returns>
        private string ReturnPassengerCType(string Type)
        {
            string Message = "0";
            switch (Type)
            {
                case "1":
                    Message = "0";
                    break;
                case "2":
                    Message = "3";
                    break;
            }
            return Message;
        }

        /// <summary>
        /// 构造百拓支付订单信息接口页面
        /// </summary>
        /// <param name="OrderId">本地机票订单编号</param>
        /// <param name="Type">1，支付宝；2，快钱</param>
        /// <returns>返回构造好的GET提交方式页面信息</returns>
        public string BaiTuoPaySend(string OrderId, string Type)
        {
            string SendURL = "";
            try
            {
                //if (OrderList[0].Paid != 1)
                //{
                //    return "";
                //}
                decimal PayFee = _order.PayMoney;
                PayFee = Math.Round(PayFee, 2);
                string CoAgsinCode = GetMD5str(_baiTuoAg + _order.OutOrderId);
                string ReturnURL = "http://210.14.138.26:91/Pay/PTReturnPage/ReturnBaiTuoPay.aspx";
                if (Type == "1")
                {
                    SendURL = "http://co.baitour.com/copayment/PortFlightPayment.aspx?ForderId=" + _order.OutOrderId + "&CoUserName=" + _baiTuoAccout + "&CoAgentCode=" + CoAgsinCode + "&UserFares=" + _order.PMFee.ToString() + "&CoShouldPay=" + PayFee.ToString() + "&opUrl=" + ReturnURL + "&bankId=AUTOALIPAY";
                }
                else
                {
                    SendURL = "http://co.baitour.com/copayment/PortFlightPayment.aspx?ForderId=" + _order.OutOrderId + "&CoUserName=" + _baiTuoAccout + "&CoAgentCode=" + CoAgsinCode + "&UserFares=" + _order.PMFee.ToString() + "&CoShouldPay=" + PayFee.ToString() + "&opUrl=" + ReturnURL + "&bankId=AUTOBILLPAY";
                }
            }
            catch
            {
                SendURL = "";
            }
            if (SendURL != "")
            {
                return SendURL;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="oldstr">要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        private string GetMD5str(string oldstr)
        {
            ASCIIEncoding asc = new ASCIIEncoding();
            byte[] result = Encoding.Default.GetBytes(oldstr);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }

        /// <summary>
        /// 判断当前政策是否来源于百拓
        /// </summary>
        /// <param name="Id">订单主键编号</param>
        /// <returns>返回是否来源于百拓的标识</returns>
        public bool IsBaiTuoPolicySource(int Id)
        {
            //根据订单编号获取订单信息集合
            bool Issuccess = false;
            try
            {
                //判断是否为享受百拓政策接口，享受则生成百拓订单
                if (_order.PolicySource == 2)
                {
                    Issuccess = true;
                }
            }
            catch
            {
                Issuccess = false;
            }
            return Issuccess;
        }

        /// <summary>
        /// 百拓信息发送回调接口
        /// </summary>
        /// <param name="OutOrderId">外部订单编号</param>
        /// <param name="ProduceType">票据类型</param>
        /// <param name="MessageType">信息类型</param>
        /// <returns>返回构造好的XML信息</returns>
        public XmlElement BaiTuoMessageSend(string OutOrderId, int ProduceType, int MessageType)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            XmlElement xe6 = doc.CreateElement("OTA_CallBackSuccessRQ");

            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", PseudoCityCode);
                xe2.SetAttribute("ISOCountry", ISOCountry);
                xe2.SetAttribute("ISOCurrency", ISOCurrency);
                xe2.SetAttribute("AirlineVendorID", AirlineVendorID);
                xe2.SetAttribute("AirportCode", AirportCode);
                XmlElement xe3 = doc.CreateElement("RequestorID");
                xe3.SetAttribute("URL", Url);
                xe3.SetAttribute("Type", "20");
                xe3.SetAttribute("ID", _baiTuoPassword);
                //将节点附加到父节点中
                xe2.AppendChild(xe3);
                xe1.AppendChild(xe2);
                //创建父节点
                XmlElement xe4 = doc.CreateElement("ForderInfo");
                xe4.SetAttribute("OrderID", OutOrderId);
                xe4.SetAttribute("ProduceType", ProduceType.ToString());
                xe4.SetAttribute("MessageType", MessageType.ToString());
                //将节点附加到父节点中
                xe6.AppendChild(xe1);
                xe6.AppendChild(xe4);
            }
            catch
            {
                return xe6;
            }
            //返回匹配好的XML节点
            return xe6;
        }


        /// <summary>
        /// 百拓订单支付是否成功标识
        /// </summary>
        /// <param name="OutOrderId">外部订单号</param>
        /// <param name="ReturnStatus">返回状态</param>
        /// <param name="MessageType">消息类型</param>
        /// <param name="OrderId">订单号</param>
        /// <returns>返回是否成功标识</returns>
        public bool IsSuccessForBaiTuoQuitOrder(string OutOrderId, string ReturnStatus, string MessageType)
        {
            bool IsSuccess = false;
            if (ReturnStatus.ToUpper() == "Y" && MessageType == "3")
            {
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
            }
            //返回是否成功退款标识
            return IsSuccess;
        }

        /// <summary>
        /// 支付异步返回参数判断
        /// </summary>
        /// <param name="OutOrderId">外部订单编号</param>
        /// <param name="PaymentStatus">支付状态</param>
        /// <param name="AgentCode">接口用户编号</param>
        /// <param name="Pid">产品类型</param>
        /// <returns>返回是否成功标识</returns>
        public bool IsSuccessForBaiTuoPayOrder(string OutOrderId, string PaymentStatus, string PaymentMode, string messageType)
        {
            bool IsSuccess = false;
            if (PaymentStatus.ToUpper() == "Y" && messageType.ToString() == "1")
            {
                IsSuccess = true;
            }
            return IsSuccess;
        }

        /// <summary>
        /// 创建获取黑名单的信息
        /// </summary>
        /// <param name="UserName">百拓用户名</param>
        /// <param name="UserPwd">百拓用户密码</param>
        /// <param name="AgentCode">百拓用户编号</param>
        /// <returns>返回构建好的XML格式信息</returns>
        public XmlElement GetInvalidationProviderListSend(string UserName, string UserPwd, string AgentCode)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            XmlElement xe6 = doc.CreateElement("OTA_Invalid_ProviderRQ");

            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", "2U8");
                xe2.SetAttribute("ISOCountry", "CN");
                xe2.SetAttribute("ISOCurrency", "USD");
                XmlElement xe3 = doc.CreateElement("RequestorID");
                xe3.SetAttribute("URL", Url);
                xe3.SetAttribute("Type", "18");
                xe3.SetAttribute("ID", _baiTuoPassword);
                //将节点附加到父节点中
                xe2.AppendChild(xe3);
                xe1.AppendChild(xe2);
                xe6.AppendChild(xe1);
            }
            catch
            {
                return xe6;
            }

            return xe6;
        }

        /// <summary>
        /// 根据获取的黑名单信息进行处理
        /// </summary>
        /// <param name="xmlNode">返回的XML信息</param>
        /// <returns>返回解析后的DataSet集合</returns>
        public DataSet GetInvalidationProviderListRecive(XmlNode xmlNode)
        {
            DataSet ds = new DataSet();
            try
            {
                StringReader rea = new StringReader("<OTA_Invalid_ProviderRS>" + xmlNode.InnerXml + "</OTA_Invalid_ProviderRS>");
                XmlTextReader xmlReader = new XmlTextReader(rea);
                ds.ReadXml(xmlReader);
            }
            catch
            {
                return ds;
            }
            return ds;
        }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="Time">时间</param>
        /// <returns>返回处理后的时间</returns>
        public string ConvertDateTime(string Time)
        {
            string Message = "";
            bool IsMonth = false;
            bool IsDay = false;
            bool IsHour = false;
            bool IsMintue = false;
            bool IsSecond = false;
            if (Time.IndexOf("T") >= 0)
            {
                Time = Time.Replace("T", " ");
            }
            DateTime dt = DateTime.Parse(Time);
            int hour = dt.Hour;
            int Month = dt.Month;
            int Day = dt.Day;
            int Minute = dt.Minute;
            int Second = dt.Second;
            if (Month < 10)
            {
                IsMonth = true;
            }
            if (Day < 10)
            {
                IsDay = true;
            }
            if (hour < 10)
            {
                IsHour = true;
            }
            if (Minute < 10)
            {
                IsMintue = true;
            }
            if (Second < 10)
            {
                IsSecond = true;
            }
            Message = dt.Year + "-";
            if (IsMonth)
            {
                Message = Message + "0" + dt.Month + "-";
            }
            else
            {
                Message = Message + dt.Month + "-";
            }
            if (IsDay)
            {
                Message = Message + "0" + dt.Day + "T";
            }
            else
            {
                Message = Message + dt.Day + "T";
            }
            if (IsHour)
            {
                Message = Message + "0" + dt.Hour + ":";
            }
            else
            {
                Message = Message + dt.Hour + ":";
            }
            if (IsMintue)
            {
                Message = Message + "0" + dt.Minute + ":";
            }
            else
            {
                Message = Message + dt.Minute + ":";
            }
            if (IsSecond)
            {
                Message = Message + "0" + dt.Second;
            }
            else
            {
                Message = Message + dt.Second;
            }
            return Message;
        }
        #endregion
    }
}
