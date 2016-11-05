using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using System.Data;
using PnrAnalysis;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    ///517接口操作
    /// </summary>
    public class PTBy517
    {
        #region 参数定义
        /// <summary>
        /// 517接口帐号
        /// </summary>
        public string _517Accout = "";
        /// <summary>
        /// 517接口密码
        /// </summary>
        public string _517Password = "";
        /// <summary>
        /// 517接口安全码
        /// </summary>
        public string _517Ag = "";
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

        w_517WebService._517WebServiceSoapClient _517Service;
        #endregion

        #region 实例赋值
        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public PTBy517(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.User_Employees mUser, PbProject.Model.User_Company mTopcom, PbProject.Model.User_Company mCom)
        {

            _mTopcom = mTopcom;

            _mCom = mCom;

            _order = Order;

            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();
            _517Service = new w_517WebService._517WebServiceSoapClient();

            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(_mTopcom.UninCode.ToString());
            BS = WebCommon.Utility.BaseParams.getParams(ParList);

            _517Accout = BS.JieKouZhangHao.Split('|')[0].Split('^')[0];

            _517Password = BS.JieKouZhangHao.Split('|')[0].Split('^')[1];
            _517Ag = BS.JieKouZhangHao.Split('|')[0].Split('^')[2];

            //_517Accout = "cdqmkjt";

            //_517Password = "kjt962992";
            //_517Ag = "4b9e9902f1c34ed08cefd84f2388e7e0";



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
                //if (_QXValue.Contains(""))//控台517开关是否打开
                //{
                    List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + _order.OrderId + "'");
                    string StartDate = "";
                    string SecondDate = "";
                    StartDate = skyList[0].FromDate.ToShortDateString();
                    SecondDate = skyList[0].FromDate.ToShortDateString();
                    PnrAnalysis.FormatPNR ss = new FormatPNR();
                    string RTContent = ss.RemoveHideChar(skyList[0].NewPnrContent).Replace("\r", "").Replace("\t", "").Replace("\n", "");
                    string PATContent = ss.RemoveHideChar(skyList[0].Pat).Replace("\r", "").Replace("\t", "").Replace("\n", "");
                    if (skyList.Count > 1)
                    {
                        SecondDate = skyList[1].FromDate.ToShortDateString();
                    }
                    //获取517政策
                    DataSet ds517 = new DataSet();
                    try
                    {
                        ds517 = _517Service.GetBenefitDataPnrContent(_517Accout, _517Password, _517Ag, RTContent, PATContent, _order.PNR);
                    }
                    catch (Exception e)
                    {
                        
                    }

                    if (ds517.Tables.Count > 0)
                    {
                        try
                        {
                            DataTable NewDt = ds517.Tables[0].Clone();
                            NewDt.Columns["EffectDate"].DataType = typeof(DateTime);
                            NewDt.Columns["ExpirationDate"].DataType = typeof(DateTime);
                            foreach (DataRow dr in ds517.Tables[0].Rows)
                            {
                                DataRow NewDr = NewDt.NewRow();
                                for (int i = 0; i < ds517.Tables[0].Columns.Count; i++)
                                {
                                    NewDr[i] = dr[i].ToString();
                                }
                                NewDt.Rows.Add(NewDr);
                            }
                            mPolicyList = Merger517DT(NewDt.Select("EffectDate<='" + StartDate + " 00:00:00' and ExpirationDate>='" + SecondDate + " 23:59:59'"), _mTopcom, ChangePnr);
                        }
                        catch (Exception ex)
                        {
                            //errorData517 = "1";
                            //OnError(ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.Get517Pol");
                        }
                    }
                }
            //}
            catch (Exception ex)
            {
                //errorData517 = "1";
                //OnError("整体线程异常，" + ex.ToString(), "PiaoBao.BLLLogic.Policy.InterFacePol.Get517Pol");
            }
            return mPolicyList;
        }

        /// <summary>
        /// 将517政策合并到原始政策dt中去
        /// </summary>
        /// <param name="LocalDt">原始dt</param>
        /// <param name="Dt517">517dt</param>
        private List<PbProject.Model.Tb_Ticket_Policy> Merger517DT(DataRow[] Dr517, User_Company GYCompany, bool ChangePnr)
        {
            try
            {
                List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
                for (int i = 0; i < Dr517.Length; i++)
                {
                    //如果不允许换编码并且政策是必须换编码出票的,则过滤掉,yyy 2013-6-7update
                    if (!ChangePnr && bool.Parse(Dr517[i]["IsChangePNRCP"].ToString()))
                    {
                        continue;
                    }
                        //DataRow dr = LocalDt.NewRow();
                        PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                        mPolicy.CpyNo = "0" + PbProject.Model.definitionParam.PolicySourceParam.b517na.ToString() + _mTopcom.UninCode;
                        mPolicy.CpyName = _mTopcom.UninName;
                        if (Dr517[i]["IsSp"].ToString() == "0")//是否是特殊政策
                        {
                            mPolicy.PolicyKind = 0;
                            mPolicy.GenerationType = 1;
                        }
                        else
                        {
                            mPolicy.PolicyKind = 2;
                            mPolicy.GenerationType = 2;
                        }
                        mPolicy.CarryCode = Dr517[i]["CarryCode"].ToString();
                        if (Dr517[i]["TravelType"].ToString() == "单程")
                        {
                            mPolicy.TravelType = 1;
                        }
                        else if (Dr517[i]["TravelType"].ToString() == "单程/往返")
                        {
                            mPolicy.TravelType = 2;
                        }
                        else if (Dr517[i]["TravelType"].ToString() == "往返")
                        {
                            mPolicy.TravelType = 3;
                        }
                        else
                        {
                            mPolicy.TravelType = 4;
                        }

                        if (Dr517[i]["PolicyType"].ToString() == "1")
                        {
                            mPolicy.PolicyType = 2;
                        }
                        else
                        {
                            mPolicy.PolicyType = 1;
                        }
                        mPolicy.TeamFlag = 0;
                        if (Dr517[i]["FromCity"].ToString() == "")
                        {
                            mPolicy.StartCityNameCode = "ALL";
                        }
                        else
                        {
                            mPolicy.StartCityNameCode = Dr517[i]["FromCity"].ToString();
                        }
                        mPolicy.StartCityNameSame = 2;

                        if (Dr517[i]["ToCity"].ToString() == "")
                        {
                            mPolicy.TargetCityNameCode = "ALL";
                        }
                        else
                        {
                            mPolicy.TargetCityNameCode = Dr517[i]["FromCity"].ToString();
                        }
                        mPolicy.TargetCityNameSame = 2;
                        if (Dr517[i]["FlightType"].ToString() == "1")
                        {
                            mPolicy.ApplianceFlight = Dr517[i]["Flight"].ToString();
                        }
                        if (Dr517[i]["FlightType"].ToString() == "2")
                        {
                            mPolicy.UnApplianceFlight = Dr517[i]["Flight"].ToString();
                        }
                        if (Dr517[i]["FlightType"].ToString() == "0")
                        {
                            mPolicy.ApplianceFlightType = 1;
                        }
                        else if (Dr517[i]["FlightType"].ToString() == "1")
                        {
                            mPolicy.ApplianceFlightType = 2;
                        }
                        else
                        {
                            mPolicy.ApplianceFlightType = 3;
                        }

                        mPolicy.ScheduleConstraints = Dr517[i]["ScheduleConstraints"].ToString();
                        mPolicy.ShippingSpace = Dr517[i]["Space"].ToString();

                        mPolicy.SpacePrice = Convert.ToDecimal(Dr517[i]["PMFee"]);
                        mPolicy.FlightStartDate = Convert.ToDateTime(Dr517[i]["EffectDate"]);
                        mPolicy.FlightEndDate = Convert.ToDateTime(Dr517[i]["ExpirationDate"]);
                        mPolicy.PrintStartDate = Convert.ToDateTime(Dr517[i]["EffectDate"]);
                        mPolicy.PrintEndDate = Convert.ToDateTime(Dr517[i]["ExpirationDate"]);
                        mPolicy.AuditDate = DateTime.Now;
                        mPolicy.AuditType = 1;
                        string isChangePnr = "";
                        if (bool.Parse(Dr517[i]["IsChangePNRCP"].ToString()))
                        {
                            isChangePnr = "须换编码出票.";
                        }
                        mPolicy.Remark = isChangePnr+" "+Dr517[i]["Remark"].ToString();
                        mPolicy.IsApplyToShareFlight = 0;
                        mPolicy.ShareAirCode = "";
                        mPolicy.IsLowerOpen = 0;
                        mPolicy.DownPoint = decimal.Parse(Dr517[i]["Policy"].ToString());
                        mPolicy.InterPolicyId = Dr517[i]["PolicyID"].ToString() + "~" + Dr517[i]["PolicyChildID"].ToString();
                        mPolicy._WorkTime = WorkTimeConvert(Dr517[i]["GYOnlineTime"].ToString(), GYCompany.WorkTime);
                        mPolicy._FPGQTime = Dr517[i]["GYFPTime"].ToString();
                        mPolicy.Office=Dr517[i]["Office"]==DBNull.Value?"":Dr517[i]["Office"].ToString();

                        mPolicyList.Add(mPolicy);
                        //dr["CarryCode"] = Dr517[i]["CarryCode"];
                        //if (Dr517[i]["FlightType"].ToString() == "1")
                        //{
                        //    dr["ApplianceFlight"] = Dr517[i]["Flight"];
                        //}
                        //if (Dr517[i]["FlightType"].ToString() == "2")
                        //{
                        //    dr["UnApplianceFlight"] = Dr517[i]["Flight"];
                        //}
                        //dr["ScheduleConstraints"] = Dr517[i]["ScheduleConstraints"];
                        //dr["Space"] = "/" + Dr517[i]["Space"] + "/";
                        //dr["OldPolicy"] = decimal.Parse(Dr517[i]["Policy"].ToString()) / 100;
                        //dr["GYPolicy"] = decimal.Parse(Dr517[i]["Policy"].ToString()) / 100;
                        //dr["FXPolicy"] = decimal.Parse(Dr517[i]["Policy"].ToString()) / 100;
                        //dr["PolicySource"] = "3";
                        //dr["PolicyId"] = Dr517[i]["PolicyID"] + "~" + Dr517[i]["PolicyChildID"];
                        //dr["PolicyType"] = Dr517[i]["PolicyType"];
                        //if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                        //{
                        //    dr["WorkTime"] = WorkTimeConvert(Dr517[i]["GYOnlineTime"].ToString(), GYCompany.WorkTime);
                        //    dr["BusinessTime"] = Dr517[i]["GYFPTime"];
                        //}
                        //else
                        //{
                        //    dr["WorkTime"] = Dr517[i]["GYOutlineTime"];
                        //    dr["BusinessTime"] = Dr517[i]["GYFPTimeNew"];
                        //}
                        //dr["SpecialType"] = "0";
                        //dr["IsPause"] = "0";
                        //dr["IsLowerOpen"] = "0";
                        //dr["Remark"] = Dr517[i]["Remark"];
                        //dr["PolOffice"] = Dr517[i]["Office"];
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
            catch (Exception e) {
                //OnError("类：PolicyMatching中方法WorkTimeConvert报错：" + e.ToString(), "WorkTimeConvert");
            }
            return NewTimeList[0] + "-" + NewTimeList[1];
        }

        public void OnCreateOrder()
        {
            //if (_QXValue.Contains("|62|"))//控制是否自动生成订单
            //{

                List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + _order.OrderId + "'");
                PnrAnalysis.FormatPNR ss = new PnrAnalysis.FormatPNR();
                string RTContent = skyList[0].NewPnrContent.Replace("\r", "").Replace("\t", "").Replace("\n", "");
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
                        PatInfo patFirst = sss.PatList[0];
                        PatInfo patLast = sss.PatList[sss.PatList.Count - 1];
                        //if (_QXValue.Contains("|60|"))
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


                //OnErrorNew(1, "517开始生成订单", "517生成订单");
                DataSet dsReson;
                if (_order.PolicyId.Split('~')[1].ToString() != "")//判断有无子政策ID
                {
                    dsReson = _517Service.CreateOrderByPnrAndPAT(_517Accout, _517Password, _517Ag, RTContent, _order.BigCode, Convert.ToInt32(_order.PolicyId.Split('~')[0].ToString()), _order.LinkMan, _order.LinkManPhone, _order.PolicyId.Split('~')[1].ToString(), PATContent, _order.PNR);
                }
                else
                {
                    dsReson = _517Service.CreateOrderByPnrAndPAT(_517Accout, _517Password, _517Ag, RTContent, _order.BigCode, Convert.ToInt32(_order.PolicyId.Split('~')[0].ToString()), _order.LinkMan, _order.LinkManPhone, "", PATContent, _order.PNR);
                }

                string mes517Create = "";

                for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                    {
                        mes517Create = mes517Create + dsReson.Tables[0].Columns[i].ColumnName + ":" + dsReson.Tables[0].Rows[i][j].ToString() + "/";//DataTable转化成String类型
                    }
                }
                mes517Create = "table's name:" + dsReson.Tables[0].TableName + "/table's content:" + mes517Create;
                CreateLog(_order.OrderId, "预定", mes517Create, 1);
                if (dsReson.Tables[0].TableName == "error")//生成订单失败，记录日志
                {
                    CreateLog(_order.OrderId, "预定", mes517Create, 3);
                    //OnErrorNew(1, "517生成订单失败", "517生成订单");
                }
                else
                {
                    if (dsReson.Tables[0].Rows[0]["OrderId"].ToString() != "")
                    {
                        //OnErrorNew(1, "517生成订单成功", "517生成订单");
                        CreateLog(_order.OrderId, "预定", "517生成订单成功！", 3);
                        if (dsReson.Tables[0].Rows[0]["TotlePirce"].ToString() == "")
                        {
                            dsReson.Tables[0].Rows[0]["TotlePirce"] = "0";
                        }
                        OnPay(Convert.ToDecimal(dsReson.Tables[0].Rows[0]["TotlePirce"].ToString()), dsReson.Tables[0].Rows[0]["OrderId"].ToString(), mes517Create);
                    }
                    else
                    {
                        CreateLog(_order.OrderId, "预定", "517生成订单失败：" + mes517Create, 3);
                    }
                }
            //}
        }

        public void OnPay(decimal outpayfee, string outorderid, string mes517Create)
        {
            //if (_QXValue.Contains("31"))//是否自动代付
            //{
                DataSet dsResonPay;
                if (outpayfee != _order.OutOrderPayMoney)//如果百拓价格比本地高，则不支付
                {
                    CreateLog(_order.OrderId, "预定", "517自动代付失败：平台订单价格和本地价格不符，不进行代付！", 3);
                    return;
                }
                _order.OutOrderId = outorderid;
                _order.OutOrderPayMoney = outpayfee;

                //if (_QXValue.Contains("54"))//开启517接口预存款支付
                //{
                    //CreateLog(_order.OrderId, "预定", "517进行预存款支付", 1);
                    //OnErrorNew(1, "预存款支付", "517支付订单");
                    //dsResonPay = _517Service.OrderPay(_517Accout, _517Password, _517Ag, "预存款账号", "预存款密码", _order.OutOrderId, outpayfee, "", _order.PNR);
                //}
                //else
                //{
                CreateLog(_order.OrderId, "预定", "517进行支付宝支付", 1);
                //    //OnErrorNew(1, "支付宝支付", "517支付订单");
                dsResonPay = _517Service.OrderNoPwdPay(_517Accout, _517Password, _order.OutOrderId, outpayfee, _517Ag);
                //}
                if (dsResonPay != null)
                {
                    string mes517Pay = "";

                    for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                    {
                        for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                        {
                            mes517Pay = mes517Pay + dsResonPay.Tables[0].Columns[i].ColumnName + ":" + dsResonPay.Tables[0].Rows[i][j].ToString() + "/";
                        }
                    }
                    mes517Create = mes517Create + "&517代付：" + mes517Pay;

                    if (mes517Pay == "False%%%/|")//代付失败，可能为余额不足
                    {
                        CreateLog(_order.OrderId, "预定", "517代付失败：请检查自动代付支付宝余额！", 3);
                    }
                    if (dsResonPay.Tables[0].TableName == "error")//代付失败，记录日志
                    {
                        CreateLog(_order.OrderId, "预定", "517代付失败：" + mes517Pay, 3);
                    }
                    if (dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString() == "True")
                    {
                        CreateLog(_order.OrderId, "预定", "517代付成功！", 3);
                        _order.OrderStatusCode = 3;
                        _order.PayStatus = 2;
                        _order.OutOrderPayFlag = true;
                    }
                    else
                    {
                        CreateLog(_order.OrderId, "预定", "517代付失败：" + mes517Pay, 3);
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
            DataSet ds = _517Service.GetOrderInfo(_517Accout, _517Password, _517Ag, _order.OutOrderId, _order.PNR.ToString());
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
