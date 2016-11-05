using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using System.Data;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    ///51book接口操作
    /// </summary>
    public class PTBy51book
    {


        #region 参数定义
        /// <summary>
        /// 51book接口帐号
        /// </summary>
        public string _51bookAccout = "";
        /// <summary>
        /// 51book接口密码
        /// </summary>
        public string _51bookPassword = "";
        /// <summary>
        /// 51book接口安全码
        /// </summary>
        public string _51bookAg = "";
        /// <summary>
        /// 51book返回Url
        /// </summary>
        public string _51bookUrl = "";
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

        w_51bookService._51bookServiceSoapClient _51bookSerive = new w_51bookService._51bookServiceSoapClient();
        #endregion


        #region 实例赋值
        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public PTBy51book(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.User_Employees mUser, PbProject.Model.User_Company mTopcom, PbProject.Model.User_Company mCom)
        {

            _mTopcom = mTopcom;

            _mCom = mCom;

            _mUser = mUser;

            _order = Order;

            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();

            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(_mTopcom.UninCode.ToString());
            BS = WebCommon.Utility.BaseParams.getParams(ParList);

            _51bookAccout = BS.JieKouZhangHao.Split('|')[1].Split('^')[0];

            _51bookPassword = BS.JieKouZhangHao.Split('|')[1].Split('^')[1];

            _51bookAg = BS.JieKouZhangHao.Split('|')[1].Split('^')[2];

            _51bookUrl = BS.JieKouZhangHao.Split('|')[1].Split('^')[3];

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
                //if (_QXValue.Contains(""))
                //{
                List<PbProject.Model.Tb_Ticket_SkyWay> SkyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + _order.OrderId + "'");
                string CompanyCode = _51bookAccout;
                string CompanySign = _51bookAg;
                string travel = "OW";
                string time = SkyList[0].FromDate.ToShortDateString();
                if (SkyList.Count > 1)
                {
                    travel = "RT";
                    time = SkyList[1].FromDate.ToShortDateString();
                }
                DataSet ds51 = new DataSet();
                PnrAnalysis.FormatPNR ss = new PnrAnalysis.FormatPNR();
                string pnrTxt = ss.RemoveHideChar(SkyList[0].NewPnrContent).Replace("\r", "").Replace("\t", "").Replace("\n", "");

                string patTxt = ss.RemoveHideChar(SkyList[0].Pat).Replace("\r", "").Replace("\t", "").Replace("\n", "");
                //string pnrTxt = RepalaceChar(SkyList[0].Pat);
                int stau = 0;
                try
                {
                    ds51 = _51bookSerive.bookGetPolicyDataByPNR(CompanyCode, _order.PNR, CompanySign, pnrTxt, patTxt);
                }
                catch (Exception e)
                {
                    stau = 1;

                    //OnError(e.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.Get51BookPol");
                }
                if (stau == 1)
                {
                    try
                    {
                        ds51 = _51bookSerive.bookGetPolicyDataByPNR(CompanyCode, _order.PNR, CompanySign, pnrTxt, patTxt);
                    }
                    catch (Exception e)
                    {
                        stau = 1;

                        //OnError(e.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.Get51BookPol");
                    }
                }
                if (ds51.Tables.Count > 0)
                {
                    if (ds51.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            string where = "startDate<='" + SkyList[0].FromDate.ToShortDateString() + "' and expiredDate>='" + SkyList[0].ToDate.ToShortDateString() + "' ";
                            where += " and printTicketStartDate<='" + DateTime.Now.ToShortDateString() + "' and printTicketExpiredDate>='" + DateTime.Now.ToShortDateString() + "'";
                            where += " and routeType = '" + travel + "' and needSwitchPNR='false'";
                            mPolicyList = Merger51DT(ds51.Tables[0].Select(), travel, SkyList[0].FromDate.ToShortDateString(), time, _mTopcom, ChangePnr);
                            //updateInterFcae(ds51);
                        }
                        catch (Exception ex)
                        {
                            //errorData51book = "1";
                            //OnError(ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.Get51BookPol");
                        }
                    }
                }
                //}

            }
            catch (Exception ex)
            {
                //OnError("整体线程异常，" + ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.Get51BookPol");
            }
            return mPolicyList;
        }

        /// <summary>
        /// 将51政策合并到原始政策dt中去
        /// </summary>
        /// <param name="LocalDt">原始dt</param>
        /// <param name="Dt517">51dt</param>
        private List<PbProject.Model.Tb_Ticket_Policy> Merger51DT(DataRow[] Dr51, string travel, string FromDate, string time, User_Company GYCompany, bool ChangePnr)
        {
            try
            {
                List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
                for (int i = 0; i < Dr51.Length; i++)
                {
                    Dr51[i]["startDate"] = Dr51[i]["startDate"].ToString().Trim() == "" ? FromDate : Dr51[i]["startDate"].ToString();
                    Dr51[i]["expiredDate"] = Dr51[i]["expiredDate"].ToString().Trim() == "" ? time : Dr51[i]["expiredDate"].ToString();
                    Dr51[i]["printTicketStartDate"] = Dr51[i]["printTicketStartDate"].ToString().Trim() == "" ? DateTime.Now.ToShortDateString() : Dr51[i]["printTicketStartDate"].ToString();
                    Dr51[i]["printTicketExpiredDate"] = Dr51[i]["printTicketExpiredDate"].ToString().Trim() == "" ? DateTime.Now.ToShortDateString() : Dr51[i]["printTicketExpiredDate"].ToString();

                    if (DateTime.Parse(Dr51[i]["startDate"].ToString().Split(' ')[0]) <= DateTime.Parse(FromDate) && DateTime.Parse(Dr51[i]["expiredDate"].ToString().Split(' ')[0]) >= DateTime.Parse(time) && DateTime.Parse(Dr51[i]["printTicketStartDate"].ToString().Split(' ')[0]) <= DateTime.Parse(DateTime.Now.ToShortDateString()) && DateTime.Parse(Dr51[i]["printTicketExpiredDate"].ToString().Split(' ')[0]) >= DateTime.Parse(DateTime.Now.ToShortDateString()))
                    {

                        //如果不允许换编码并且政策是必须换编码出票的,则过滤掉,yyy 2013-6-7update
                        if (!ChangePnr && bool.Parse(Dr51[i]["needSwitchPNR"].ToString()))
                        {
                            continue;
                        }
                        if (travel == Dr51[i]["routeType"].ToString())
                        {
                            if (Dr51[i]["onWorking"].ToString().ToLower() == "true")
                            {
                                PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                                mPolicy.CpyNo = "0" + PbProject.Model.definitionParam.PolicySourceParam.b51book.ToString() + _mTopcom.UninCode;
                                mPolicy.CpyName = _mTopcom.UninName;
                                if (Dr51[i]["businessUnitType"].ToString() == "0")//是否是特殊政策
                                {
                                    mPolicy.PolicyKind = 0;
                                    mPolicy.GenerationType = 1;
                                }
                                else
                                {
                                    mPolicy.PolicyKind = 2;
                                    mPolicy.GenerationType = 2;
                                }
                                mPolicy.CarryCode = Dr51[i]["airlineCode"].ToString();
                                if (Dr51[i]["routeType"].ToString() == "OW")
                                {
                                    mPolicy.TravelType = 1;
                                }
                                else if (Dr51[i]["routeType"].ToString() == "RT")
                                {
                                    mPolicy.TravelType = 3;
                                }
                                else
                                {
                                    mPolicy.TravelType = 4;
                                }

                                if (Dr51[i]["policyType"].ToString() == "B2P")
                                {
                                    mPolicy.PolicyType = 2;
                                }
                                else
                                {
                                    mPolicy.PolicyType = 1;
                                }
                                mPolicy.TeamFlag = 0;
                                if (Dr51[i]["flightCourse"].ToString() == "999-999")
                                {
                                    mPolicy.StartCityNameCode = "ALL";
                                }
                                else
                                {
                                    mPolicy.StartCityNameCode = Dr51[i]["flightCourse"].ToString().Split('-')[0];
                                }
                                mPolicy.StartCityNameSame = 2;

                                if (Dr51[i]["flightCourse"].ToString() == "999-999")
                                {
                                    mPolicy.TargetCityNameCode = "ALL";
                                }
                                else
                                {
                                    mPolicy.TargetCityNameCode = Dr51[i]["flightCourse"].ToString().Split('-')[1];
                                }
                                mPolicy.TargetCityNameSame = 2;
                                mPolicy.ApplianceFlight = Dr51[i]["flightNoIncluding"].ToString();
                                mPolicy.UnApplianceFlight = Dr51[i]["flightNoExclude"].ToString();
                                mPolicy.ApplianceFlightType = 2;
                                mPolicy.ScheduleConstraints = Dr51[i]["flightCycle"].ToString();
                                mPolicy.ShippingSpace = Dr51[i]["seatClass"].ToString();

                                mPolicy.FlightStartDate = Convert.ToDateTime(Dr51[i]["startDate"]);
                                mPolicy.FlightEndDate = Convert.ToDateTime(Dr51[i]["expiredDate"]);
                                mPolicy.PrintStartDate = Convert.ToDateTime(Dr51[i]["printTicketStartDate"]);
                                mPolicy.PrintEndDate = Convert.ToDateTime(Dr51[i]["printTicketExpiredDate"]);
                                mPolicy.AuditDate = DateTime.Now;
                                mPolicy.AuditType = 1;
                                string isChangePnr = "";
                                if (bool.Parse(Dr51[i]["needSwitchPNR"].ToString()))
                                {
                                    isChangePnr = "须换编码出票.";
                                }
                                mPolicy.Remark = isChangePnr + " " + Dr51[i]["comment"].ToString();
                                mPolicy.IsApplyToShareFlight = 0;
                                mPolicy.ShareAirCode = "";
                                mPolicy.IsLowerOpen = 0;
                                mPolicy.DownPoint = decimal.Parse(Dr51[i]["Commission"].ToString());
                                mPolicy.InterPolicyId = Dr51[i]["Id"].ToString();
                                mPolicy._WorkTime = WorkTimeConvert(Dr51[i]["workTime"].ToString(), GYCompany.WorkTime);
                                mPolicy._FPGQTime = Dr51[i]["chooseOutWorkTime"].ToString();
                                mPolicy.Office = Dr51[i]["param2"] == DBNull.Value ? "" : Dr51[i]["param2"].ToString();
                                mPolicyList.Add(mPolicy);
                                //DataRow dr = LocalDt.NewRow();
                                //dr["CarryCode"] = Dr51[i]["airlineCode"];
                                //dr["ApplianceFlight"] = Dr51[i]["flightNoIncluding"];
                                //dr["UnApplianceFlight"] = Dr51[i]["flightNoExclude"];
                                //dr["ScheduleConstraints"] = Dr51[i]["flightCycle"];
                                //dr["Space"] = "/" + Dr51[i]["seatClass"] + "/";
                                //dr["OldPolicy"] = decimal.Parse(Dr51[i]["Commission"].ToString()) / 100;
                                //dr["GYPolicy"] = decimal.Parse(Dr51[i]["Commission"].ToString()) / 100;
                                //dr["FXPolicy"] = decimal.Parse(Dr51[i]["Commission"].ToString()) / 100;
                                //dr["PolicySource"] = "6";
                                //dr["PolicyId"] = Dr51[i]["Id"];
                                //dr["PolicyType"] = Dr51[i]["policyType"].ToString().ToUpper().Contains("B2B") ? "1" : "2";
                                //dr["WorkTime"] = WorkTimeConvert(Dr51[i]["workTime"].ToString(), GYCompany.WorkTime);
                                //dr["BusinessTime"] = Dr51[i]["chooseOutWorkTime"];
                                //dr["SpecialType"] = "0";
                                //dr["IsPause"] = "0";
                                //dr["IsLowerOpen"] = "0";
                                //dr["Remark"] = Dr51[i]["Comment"].ToString();
                                //dr["PolOffice"] = Dr51[i]["param2"];
                                //LocalDt.Rows.Add(dr);
                            }
                        }
                    }
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

            //OnErrorNew(1, "51book开始生成订单", "51book生成订单");
            DataSet dsReson = _51bookSerive.bookCreatePolicyOrderByPNR(_51bookAccout, _order.PNR, _order.PolicyId, _51bookUrl, _51bookUrl, _mUser.UserName, _51bookAg, RTContent, PATContent);

            string mes51bookCreate = "table's count:" + dsReson.Tables.Count + "&";

            for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                {
                    mes51bookCreate = mes51bookCreate + dsReson.Tables[0].Columns[i].ColumnName + ":" + dsReson.Tables[0].Rows[i][j].ToString() + "/";//DataTable转化成String类型
                }
            }
            mes51bookCreate = "table1's name:" + dsReson.Tables[0].TableName + "/table1's content:" + mes51bookCreate;

            if (dsReson.Tables[0].Columns.Contains("ErorrMessage"))
            {
                CreateLog(_order.OrderId, "预定", mes51bookCreate, 1);
                //OnErrorNew(1, "51book生成订单失败", "51book生成订单");
            }
            else
            {
                if (dsReson.Tables[0].Rows[0]["sequenceNo"].ToString() != "")
                {

                    CreateLog(_order.OrderId, "预定", "51book生成订单成功！", 3);
                    //OnErrorNew(1, "51book生成订单成功", "51book生成订单");
                    if (dsReson.Tables[0].Rows[0]["settlePrice"].ToString() == "")
                    {
                        dsReson.Tables[0].Rows[0]["settlePrice"] = "0";
                    }
                    OnPay(Convert.ToDecimal(dsReson.Tables[0].Rows[0]["settlePrice"].ToString()), dsReson.Tables[0].Rows[0]["sequenceNo"].ToString(), mes51bookCreate);
                }
            }
            //OnErrorNew(1, mesPMCreate, "PMdataset");
            //}
        }

        public void OnPay(decimal outpayfee, string outorderid, string mes51bookCreate)
        {
            //if (_QXValue.Contains("31"))//是否自动代付
            //{
            if (outpayfee != _order.OutOrderPayMoney)//如果百拓价格比本地高，则不支付
            {
                CreateLog(_order.OrderId, "预定", "51book自动代付失败：平台订单价格和本地价格不符，不进行代付！", 3);
            }
            _order.OutOrderId = outorderid;
            _order.OutOrderPayMoney = outpayfee;

            DataSet dsResonPay = _51bookSerive.bookPayPolicyOrderByPNR(_51bookAccout, _order.OutOrderId, "", "", _51bookAg);
            if (dsResonPay != null)
            {
                string mes51bookPay = "";

                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mes51bookPay = mes51bookPay + dsResonPay.Tables[0].Columns[i].ColumnName + ":" + dsResonPay.Tables[0].Rows[i][j].ToString() + "/";
                    }
                }
                mes51bookCreate = mes51bookCreate + "&51book代付：" + mes51bookPay;

                if (dsResonPay.Tables[0].Columns.Contains("ErorrMessage"))
                {
                    CreateLog(_order.OrderId, "预定", "51book代付失败：" + mes51bookPay, 3);
                }
                if (dsResonPay.Tables[0].Rows[0]["orderStatus"].ToString() == "2")
                {
                    CreateLog(_order.OrderId, "预定", "51book代付成功！", 3);
                    _order.OrderStatusCode = 3;
                    _order.PayStatus = 2;
                    _order.OutOrderPayFlag = true;
                }
                else
                {
                    CreateLog(_order.OrderId, "预定", "51book代付失败：" + mes51bookPay, 3);
                }
            }
            bool result = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new Object[] { _order });
            //}
        }

        public DataSet QueryOrder()
        {
            DataSet ds = _51bookSerive.bookgetPolicyOrderByOrderNo(_51bookAccout, _order.OutOrderId, "1", _51bookAg);
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
