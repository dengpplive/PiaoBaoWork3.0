using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using System.Data;
using PnrAnalysis;
using System.Text.RegularExpressions;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    ///8000Yi接口操作
    /// </summary>
    public class PTBy8000yi
    {
        #region 参数定义
        /// <summary>
        /// 8000yi接口帐号
        /// </summary>
        public string _8000yiAccout = "";
        /// <summary>
        /// 8000yi接口二级帐号
        /// </summary>
        public string _8000yiPassword = "";
        /// <summary>
        /// 8000yi接口支付宝签约帐号
        /// </summary>
        public string _8000yiAlipaycode = "";
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

        w_8000YService.W8000YServiceSoapClient _8000yiService = new w_8000YService.W8000YServiceSoapClient();
        #endregion


        #region 实例赋值
        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public PTBy8000yi(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.User_Employees mUser, PbProject.Model.User_Company mTopcom, PbProject.Model.User_Company mCom)
        {

            _mTopcom = mTopcom;

            _mCom = mCom;

            _order = Order;

            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();

            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(_mTopcom.UninCode.ToString());
            BS = WebCommon.Utility.BaseParams.getParams(ParList);

            _8000yiAccout = BS.JieKouZhangHao.Split('|')[5].Split('^')[0];

            _8000yiPassword = BS.JieKouZhangHao.Split('|')[5].Split('^')[1];
            _8000yiAlipaycode = BS.JieKouZhangHao.Split('|')[5].Split('^')[2];



            _QXValue = BS.KongZhiXiTong;
            //BS.GongYingKongZhiFenXiao
        }
        #endregion


        #region 基本方法
        public List<PbProject.Model.Tb_Ticket_Policy> GetPolicy(bool ChangePnr)
        {
            string errMsg = "";
            List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
            try
            {
                List<PbProject.Model.Tb_Ticket_SkyWay> SkyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + _order.OrderId + "'");
                //if (_QXValue.Contains("") && SkyList.Count == 1)
                //{
                FormatPNR ss = new FormatPNR();
                string StartDate = SkyList[0].FromDate.ToShortDateString();
                string SecondDate = SkyList[0].FromDate.ToShortDateString();
                string RTContent = ss.RemoveHideChar(SkyList[0].NewPnrContent).Replace("\t", "").Replace("\n", "");
                string PATContent = ss.RemoveHideChar(SkyList[0].Pat).Replace("\r", "").Replace("\t", "").Replace("\n", "");
                bool IsOnePrice = false;
                if (!SkyList[0].Space.Contains("1"))
                {
                    RTContent = ss.RemoveChildSeat(RTContent, out IsOnePrice);
                }
                DataSet ds8000Y = _8000yiService.SPbyPNRNote(_8000yiAccout, _8000yiPassword, _order.PNR, 0, RTContent);
                if (ds8000Y.Tables.Count > 0)
                {
                    if (ds8000Y.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            //  对方接口报错
                            errMsg = ds8000Y.Tables[0].Rows[0]["errInfo"].ToString();
                        }
                        catch
                        {
                            //  返回正常
                            mPolicyList = Merger8000YDT(ds8000Y.Tables[0].Select("A10<='" + StartDate + " 00:00:00' and A11>='" + SecondDate + " 00:00:00'"), _mTopcom, ChangePnr);
                            //Update8000YInterFcae(ds8000Y);
                        }
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                //errorData51book = "1";
                //OnError("整体线程异常，" + ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.Get8000YPol");
            }
            if (errMsg != "")
            {
                //errorData51book = "1";
            }
            return mPolicyList;
        }

        private List<PbProject.Model.Tb_Ticket_Policy> Merger8000YDT(DataRow[] Dr8000Y, User_Company GYCompany, bool ChangePnr)
        {
            try
            {
                List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
                for (int i = 0; i < Dr8000Y.Length; i++)
                {
                    PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                    mPolicy.CpyNo = "0" + PbProject.Model.definitionParam.PolicySourceParam.b8000yi.ToString() + _mTopcom.UninCode;
                    mPolicy.CpyName = _mTopcom.UninName;

                  

                    //yyy 2013年6月7日
                    //过滤掉换编码出票的政策
                    if (!ChangePnr)
                    {
                        if (Dr8000Y[i]["A17"].ToString().Contains("换编码出票"))
                        {
                            continue;
                        }
                    }


                    if (Dr8000Y[i]["A22"].ToString() == "0")//是否是特殊政策
                    {
                        mPolicy.PolicyKind = 0;
                        mPolicy.GenerationType = 1;
                    }
                    else
                    {
                        mPolicy.PolicyKind = 2;
                        mPolicy.GenerationType = 2;
                    }
                    mPolicy.CarryCode = Dr8000Y[i]["A4"].ToString();
                    if (Dr8000Y[i]["A7"].ToString() == "1")
                    {
                        mPolicy.TravelType = 1;
                    }
                    else if (Dr8000Y[i]["A7"].ToString() == "3")
                    {
                        mPolicy.TravelType = 2;
                    }
                    else if (Dr8000Y[i]["A7"].ToString() == "2")
                    {
                        mPolicy.TravelType = 3;
                    }
                    else
                    {
                        mPolicy.TravelType = 4;
                    }

                    if (Dr8000Y[i]["A16"].ToString() == "BSP")
                    {
                        mPolicy.PolicyType = 2;
                    }
                    else
                    {
                        mPolicy.PolicyType = 1;
                    }
                    mPolicy.TeamFlag = 0;
                    if (Dr8000Y[i]["A2"].ToString() == "All")
                    {
                        mPolicy.StartCityNameCode = "ALL";
                    }
                    else
                    {
                        mPolicy.StartCityNameCode = Dr8000Y[i]["A2"].ToString();
                    }
                    mPolicy.StartCityNameSame = 2;

                    if (Dr8000Y[i]["A3"].ToString() == "All")
                    {
                        mPolicy.TargetCityNameCode = "ALL";
                    }
                    else
                    {
                        mPolicy.TargetCityNameCode = Dr8000Y[i]["A3"].ToString();
                    }
                    mPolicy.TargetCityNameSame = 2;

                    mPolicy.ApplianceFlight = Dr8000Y[i]["A5"].ToString();
                    mPolicy.UnApplianceFlight = Dr8000Y[i]["A6"].ToString();
                    mPolicy.ApplianceFlightType = 2;

                    mPolicy.ScheduleConstraints = Dr8000Y[i]["A21"].ToString();
                    Regex reg = new Regex(@"/^\d+$/");
                    if (!reg.IsMatch(Dr8000Y[i]["A9"].ToString()))
                    {
                        mPolicy.ShippingSpace = Dr8000Y[i]["A9"].ToString().Replace("#", "");
                    }

                    mPolicy.SpacePrice = Convert.ToDecimal(Dr8000Y[i]["A24"]);
                    mPolicy.FlightStartDate = Convert.ToDateTime(Dr8000Y[i]["A10"]);
                    mPolicy.FlightEndDate = Convert.ToDateTime(Dr8000Y[i]["A11"]);
                    mPolicy.PrintStartDate = Convert.ToDateTime(Dr8000Y[i]["A10"]);
                    mPolicy.PrintEndDate = Convert.ToDateTime(Dr8000Y[i]["A11"]);
                    mPolicy.AuditDate = DateTime.Now;
                    mPolicy.AuditType = 1;
                    mPolicy.Remark = Dr8000Y[i]["A17"].ToString();
                    mPolicy.IsApplyToShareFlight = 0;
                    mPolicy.ShareAirCode = "";
                    mPolicy.IsLowerOpen = 0;
                    mPolicy.DownPoint = decimal.Parse(Dr8000Y[i]["A8"].ToString());
                    mPolicy.InterPolicyId = Dr8000Y[i]["A1"].ToString();
                    mPolicy._WorkTime = WorkTimeConvert(Dr8000Y[i]["A12"].ToString(), GYCompany.WorkTime);
                    if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                    {
                        mPolicy._FPGQTime = Dr8000Y[i]["A19"].ToString().Replace("|", "-");
                    }
                    else
                    {
                        mPolicy._FPGQTime = Dr8000Y[i]["A20"].ToString().Replace("|", "-");
                    }

                    mPolicy.Office = Dr8000Y[i]["A26"] == DBNull.Value ? "" : Dr8000Y[i]["A26"].ToString();
                    mPolicyList.Add(mPolicy);



                    //DataRow dr = LocalDt.NewRow();
                    //dr["CarryCode"] = Dr8000Y[i]["A4"];
                    //dr["ApplianceFlight"] = Dr8000Y[i]["A5"];
                    //dr["UnApplianceFlight"] = Dr8000Y[i]["A6"];
                    //dr["ScheduleConstraints"] = Dr8000Y[i]["A21"];
                    //Regex reg = new Regex(@"/^\d+$/");
                    //if (!reg.IsMatch(Dr8000Y[i]["A9"].ToString()))
                    //{
                    //    dr["Space"] = "/" + Dr8000Y[i]["A9"].ToString().Replace("#", "") + "/";
                    //}
                    //dr["OldPolicy"] = decimal.Parse(Dr8000Y[i]["A8"].ToString()) / 100;
                    //dr["GYPolicy"] = decimal.Parse(Dr8000Y[i]["A8"].ToString()) / 100;
                    //dr["FXPolicy"] = decimal.Parse(Dr8000Y[i]["A8"].ToString()) / 100;
                    //dr["PolicySource"] = "8";
                    //dr["PolicyId"] = Dr8000Y[i]["A1"];
                    //dr["PolicyType"] = Dr8000Y[i]["A16"].ToString().ToUpper().Contains("B2B") ? "1" : "2";
                    //dr["WorkTime"] = WorkTimeConvert(Dr8000Y[i]["A12"].ToString(), GYCompany.WorkTime);
                    //if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                    //{
                    //    dr["BusinessTime"] = Dr8000Y[i]["A19"].ToString().Replace("|", "-");
                    //}
                    //else
                    //{
                    //    dr["BusinessTime"] = Dr8000Y[i]["A20"].ToString().Replace("|", "-");
                    //}
                    //dr["SpecialType"] = "0";
                    //dr["IsPause"] = "0";
                    //dr["IsLowerOpen"] = "0";
                    //dr["Remark"] = Dr8000Y[i]["A17"].ToString();
                    //dr["PolOffice"] = Dr8000Y[i]["A26"];
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
            string PATContent = skyList[0].Pat.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            RTContent = (new PnrAnalysis.FormatPNR()).SplitPnrAutoLine(new PnrAnalysis.FormatPNR().RemoveHideChar(RTContent));
            DataSet dsReson;
            if (_order.TravelType == 1)
            {
                dsReson = _8000yiService.CreatOrderNewByPNRNote(_8000yiAccout, _8000yiPassword, _order.PNR, _order.PolicyId, RTContent);
                string mes8000yiCreate = "table's count:" + dsReson.Tables.Count + "&";

                for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                    {
                        mes8000yiCreate = mes8000yiCreate + dsReson.Tables[0].Columns[i].ColumnName + ":" + dsReson.Tables[0].Rows[i][j].ToString() + "/";//DataTable转化成String类型
                    }
                }
                mes8000yiCreate = "table's name:" + dsReson.Tables[0].TableName + "/table's content:" + mes8000yiCreate;
                CreateLog(_order.OrderId, "预定", mes8000yiCreate, 1);
                if (dsReson != null)
                {
                    try
                    {
                        string orderid = dsReson.Tables[0].Rows[0]["OrderID"].ToString();
                        //OnErrorNew(1, "8000Y生成订单成功", "8000Y生成订单");
                        CreateLog(_order.OrderId, "预定", "8000Y生成订单成功", 3);
                        _order.OutOrderId = dsReson.Tables[0].Rows[0]["OrderID"].ToString();
                        if (dsReson.Tables[0].Rows[0]["CWZongJia"].ToString() == "")
                        {
                            dsReson.Tables[0].Rows[0]["CWZongJia"] = "0";
                        }
                        OnPay(Convert.ToDecimal(dsReson.Tables[0].Rows[0]["CWZongJia"].ToString()), dsReson.Tables[0].Rows[0]["OrderID"].ToString(), mes8000yiCreate);
                    }
                    catch (Exception)
                    {
                        CreateLog(_order.OrderId, "预定", "8000Y生成订单失败：" + dsReson.Tables[0].Rows[0]["ErrInfo"].ToString(), 3);
                    }
                }
            }
            else
            {
                CreateLog(_order.OrderId, "预定", "8000yi只支持单程票", 3);
            }
            //}
        }

        public void OnPay(decimal outpayfee, string outorderid, string mes8000yiCreate)
        {
            //if (_QXValue.Contains("31"))//是否自动代付
            //{
            DataSet dsResonPay;
            if (outpayfee != _order.OutOrderPayMoney)//如果百拓价格比本地高，则不支付
            {
                CreateLog(_order.OrderId, "预定", "8000yi自动代付失败：平台订单价格和本地价格不符，不进行代付！", 3);
            }
            _order.OutOrderId = outorderid;
            _order.OutOrderPayMoney = outpayfee;
            dsResonPay = _8000yiService.AutomatismPay(_8000yiAccout, _8000yiPassword, _order.OutOrderId, _8000yiAlipaycode);
            if (dsResonPay != null)
            {
                string mes8000yiPay = "";

                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mes8000yiPay = mes8000yiPay + dsResonPay.Tables[0].Columns[i].ColumnName + ":" + dsResonPay.Tables[0].Rows[i][j].ToString() + "/";
                    }
                }
                mes8000yiCreate = mes8000yiCreate + "&8000yi代付：" + mes8000yiPay;


                if (dsResonPay != null && dsResonPay.Tables.Count > 0)
                {
                    try
                    {
                        //  支付失败
                        dsResonPay.Tables[0].Rows[0]["is_success"].ToString();
                        CreateLog(_order.OrderId, "预定", "8000Y代付失败：" + mes8000yiPay, 3);
                    }
                    catch
                    {
                        //  支付成功
                        CreateLog(_order.OrderId, "预定", "8000Y自动支付成功", 3);
                        _order.OrderStatusCode = 3;
                        _order.PayStatus = 2;
                        _order.OutOrderPayFlag = true;
                    }
                }
            }
            bool result = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new Object[] { _order });
            //}
        }

        public DataSet QueryOrder()
        {
            DataSet ds = _8000yiService.OrderPayOutTicketAndPly(_8000yiAccout, _8000yiPassword, _order.CreateTime.ToString().Replace("/", "-"));
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
