using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using System.Data;
using System.IO;
using System.Xml;
using PnrAnalysis;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    ///易行接口操作
    /// </summary>
    public class PTByYeeXing
    {
        #region 参数定义
        /// <summary>
        /// 易行接口帐号
        /// </summary>
        public string _yeeXingAccout = "";
        /// <summary>
        /// 易行接口子帐号
        /// </summary>
        public string _yeeXingAccout2 = "";
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

        w_YeeXingService.YeeXingSerivceSoapClient _yeeXingService;
        #endregion


        #region 实例赋值
        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public PTByYeeXing(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.User_Employees mUser, PbProject.Model.User_Company mTopcom, PbProject.Model.User_Company mCom)
        {

            _mTopcom = mTopcom;

            _mCom = mCom;

            _mUser = mUser;

            _order = Order;

            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();

            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(_mTopcom.UninCode.ToString());
            BS = WebCommon.Utility.BaseParams.getParams(ParList);

            _yeeXingService = new w_YeeXingService.YeeXingSerivceSoapClient();

            _yeeXingAccout = BS.JieKouZhangHao.Split('|')[6].Split('^')[0];

            _yeeXingAccout2 = BS.JieKouZhangHao.Split('|')[6].Split('^')[1];

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
                DataSet dsYeeXing = new DataSet();
                try
                {
                    dsYeeXing = _yeeXingService.ParsePnrMatchAirpContract(_yeeXingAccout, _yeeXingAccout2, _order.PNR, RTContent, PATContent);
                }
                catch (Exception e)
                {

                }

                if (dsYeeXing.Tables.Count > 0)
                {
                    try
                    {
                        DataTable NewDt = dsYeeXing.Tables[5].Clone();
                        NewDt.Columns.Add("startTime", typeof(DateTime));
                        NewDt.Columns.Add("endTime", typeof(DateTime));
                        NewDt.Columns.Add("airComp", typeof(String));
                        NewDt.Columns.Add("airSeg", typeof(int));
                        NewDt.Columns.Add("orgCity", typeof(String));
                        NewDt.Columns.Add("dstCity", typeof(String));
                        NewDt.Columns.Add("flight", typeof(String));
                        NewDt.Columns.Add("cabin", typeof(String));

                        foreach (DataRow dr in dsYeeXing.Tables[5].Rows)
                        {
                            DataRow NewDr = NewDt.NewRow();
                            for (int i = 0; i < dsYeeXing.Tables[5].Columns.Count; i++)
                            {
                                NewDr[i] = dr[i].ToString();
                            }
                            NewDr["startTime"] = dsYeeXing.Tables[2].Rows[0]["startTime"];
                            NewDr["endTime"] = dsYeeXing.Tables[2].Rows[0]["endTime"];
                            NewDr["airComp"] = dsYeeXing.Tables["lineinfo"].Rows[0]["airComp"];
                            NewDr["orgCity"] = dsYeeXing.Tables["lineinfo"].Rows[0]["orgCity"];
                            NewDr["dstCity"] = dsYeeXing.Tables["lineinfo"].Rows[0]["dstCity"];
                            NewDr["flight"] = "";
                            NewDr["cabin"] = dsYeeXing.Tables["lineinfo"].Rows[0]["cabin"];
                            NewDr["airSeg"] = dsYeeXing.Tables["lineinfos"].Rows[0]["airSeg"];
                            NewDt.Rows.Add(NewDr);
                        }
                        mPolicyList = MergerYeeXingDT(NewDt.Select("1=1"), _mTopcom, ChangePnr);
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
        private List<PbProject.Model.Tb_Ticket_Policy> MergerYeeXingDT(DataRow[] DrYeeXing, User_Company GYCompany, bool ChangePnr)
        {
            try
            {
                List<PbProject.Model.Tb_Ticket_Policy> mPolicyList = new List<Tb_Ticket_Policy>();
                for (int i = 0; i < DrYeeXing.Length; i++)
                {


                    //如果不允许换编码并且政策是必须换编码出票的,则过滤掉,yyy 2013-6-7update
                    if (!ChangePnr && DrYeeXing[i]["changePnr"].ToString()=="1")
                    {
                        continue;
                    }


                    //DataRow dr = LocalDt.NewRow();
                    PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                    mPolicy.CpyNo = PbProject.Model.definitionParam.PolicySourceParam.bYeeXing.ToString() + _mTopcom.UninCode;
                    mPolicy.CpyName = _mTopcom.UninName;

                    //屏蔽易行特殊政策性额
                    if (DrYeeXing[i]["isSphigh"].ToString() == "1")//是否是特殊政策
                    {
                        continue;
                    }

                    if (!DrYeeXing[i]["payType"].ToString().Contains("1"))//屏蔽不支持支付宝支付的政策
                    {
                        continue;
                    }


                    if (DrYeeXing[i]["isSphigh"].ToString() == "0")//是否是特殊政策
                    {
                        mPolicy.PolicyKind = 0;
                        mPolicy.GenerationType = 1;
                    }
                    else
                    {
                        mPolicy.PolicyKind = 2;
                        mPolicy.GenerationType = 2;
                    }
                    mPolicy.CarryCode = DrYeeXing[i]["airComp"].ToString();
                    if (DrYeeXing[i]["airSeg"].ToString() == "1")
                    {
                        mPolicy.TravelType = 1;
                    }
                    else if (DrYeeXing[i]["airSeg"].ToString() == "2")
                    {
                        mPolicy.TravelType = 3;
                    }
                    else
                    {
                        mPolicy.TravelType = 4;
                    }

                    if (DrYeeXing[i]["tickType"].ToString() == "1")
                    {
                        mPolicy.PolicyType = 1;
                    }
                    else
                    {
                        mPolicy.PolicyType = 1;
                    }
                    mPolicy.TeamFlag = 0;
                    mPolicy.StartCityNameCode = DrYeeXing[i]["orgCity"].ToString();
                    mPolicy.StartCityNameSame = 2;
                    mPolicy.TargetCityNameCode = DrYeeXing[i]["dstCity"].ToString();
                    mPolicy.TargetCityNameSame = 2;
                    mPolicy.ApplianceFlight = DrYeeXing[i]["flight"].ToString();
                    mPolicy.UnApplianceFlight = "";
                    mPolicy.ApplianceFlightType = 1;

                    mPolicy.ShippingSpace = DrYeeXing[i]["cabin"].ToString();

                    mPolicy.SpacePrice = Convert.ToDecimal(DrYeeXing[i]["ibePrice"]);
                    mPolicy.FlightStartDate = Convert.ToDateTime(DrYeeXing[i]["startTime"]);
                    mPolicy.FlightEndDate = Convert.ToDateTime(DrYeeXing[i]["endTime"]);
                    mPolicy.PrintStartDate = Convert.ToDateTime(DrYeeXing[i]["startTime"]);
                    mPolicy.PrintEndDate = Convert.ToDateTime(DrYeeXing[i]["endTime"]);
                    mPolicy.AuditDate = DateTime.Now;
                    mPolicy.AuditType = 1;
                    string isChangePnr = "";
                    if (DrYeeXing[i]["changePnr"].ToString()=="1")
                    {
                        isChangePnr = "须换编码出票.";
                    }

                    mPolicy.Remark =isChangePnr+" "+DrYeeXing[i]["memo"].ToString();
                    mPolicy.IsApplyToShareFlight = 0;
                    mPolicy.ShareAirCode = "";
                    mPolicy.IsLowerOpen = 0;
                    mPolicy.DownPoint = decimal.Parse(DrYeeXing[i]["disc"].ToString());
                    mPolicy.InterPolicyId = DrYeeXing[i]["plcid"].ToString();
                    mPolicy._WorkTime = WorkTimeConvert(DrYeeXing[i]["workTime"].ToString(), GYCompany.WorkTime);
                    mPolicy._FPGQTime = WorkTimeConvert(DrYeeXing[i]["workReturnTime"].ToString(),GYCompany.BusinessTime);
                    mPolicy._returnMoney = DrYeeXing[i]["extReward"].ToString(); ;

                    mPolicyList.Add(mPolicy);
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


            dsReson = _yeeXingService.ParsePnrBookContract(_yeeXingAccout, _yeeXingAccout2, _order.PNR, RTContent, PATContent, _order.PolicyId, _order.PMFee.ToString(), _order.OrderId, _order.PolicyPoint.ToString(), _order.ReturnMoney.ToString());

            string mesYeeXingCreate = "";

            for (int i = 0; i < dsReson.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsReson.Tables[0].Columns.Count; j++)
                {
                    mesYeeXingCreate = mesYeeXingCreate + dsReson.Tables[0].Columns[i].ColumnName + ":" + dsReson.Tables[0].Rows[i][j].ToString() + "/";//DataTable转化成String类型
                }
            }
            mesYeeXingCreate = "table's name:" + dsReson.Tables[0].TableName + "/table's content:" + mesYeeXingCreate;
            CreateLog(_order.OrderId, "预定", mesYeeXingCreate, 1);
            if (dsReson.Tables[0].Rows[0]["is_success"].ToString() == "F")//生成订单失败，记录日志
            {
                CreateLog(_order.OrderId, "预定", mesYeeXingCreate, 3);
            }
            else
            {
                CreateLog(_order.OrderId, "预定", "易行生成订单成功！", 3);
                if (dsReson.Tables[0].Rows[0]["totalPrice"].ToString() == "")
                {
                    dsReson.Tables[0].Rows[0]["totalPrice"] = "0";
                }
                OnPay(Convert.ToDecimal(dsReson.Tables[0].Rows[0]["totalPrice"].ToString()), dsReson.Tables[0].Rows[0]["orderid"].ToString(), mesYeeXingCreate);
            }
            //}
        }

        public void OnPay(decimal outpayfee, string outorderid, string mesYeeXingCreate)
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
            dsResonPay = _yeeXingService.PayOutContract(_yeeXingAccout, _yeeXingAccout2, _order.OutOrderId, outpayfee.ToString(), "1", "", "");
            //}
            if (dsResonPay != null)
            {
                string mesYeeXingPay = "";

                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mesYeeXingPay = mesYeeXingPay + dsResonPay.Tables[0].Columns[i].ColumnName + ":" + dsResonPay.Tables[0].Rows[i][j].ToString() + "/";
                    }
                }
                mesYeeXingCreate = mesYeeXingCreate + "&易行代付：" + mesYeeXingPay;
                if (dsResonPay.Tables[0].Rows[0]["is_success"].ToString() == "T")
                {
                    CreateLog(_order.OrderId, "预定", "易行代付成功！", 3);
                    _order.OrderStatusCode = 3;
                    _order.PayStatus = 2;
                    _order.OutOrderPayFlag = true;
                }
                else
                {
                    CreateLog(_order.OrderId, "预定", "易行代付失败：" + dsResonPay.Tables[0].Rows[0]["error"].ToString() + ":" + mesYeeXingPay, 3);
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
            DataSet ds = _yeeXingService.OrderQueryContract(_yeeXingAccout, _yeeXingAccout2, _order.OutOrderId, _order.OrderId);
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
