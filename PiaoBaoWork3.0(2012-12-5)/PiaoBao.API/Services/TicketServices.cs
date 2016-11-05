using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using PbProject.Logic.Order;
using PbProject.Model;
using DataBase.Data;
using PbProject.Logic.PID;
using PbProject.WebCommon.Utility;
using System.Text;
using PiaoBao.API.Common;

namespace PiaoBao.API.Services
{
    //pasList乘机人格式
    //id,id,id
    //
    //skywayList 航段格式
    //SkyId@@@@FromCityName@@@@ToCityName@@@@FromCityCode@@@@ToCityCode@@@@
    //FromDate@@@@ToDate@@@@CarryCode@@@@FlightCode@@@@Space@@@@IsFP^
    //
    //OrderID 订单ID
    //ApplayType 退改签类型 3.退票，4.废票，5.改签
    //TGQRemark  退改签备注
    //quitReasonType  退票理由类型，1自愿，2非自愿
    //reason 退改签理由
    public class TicketServices : BaseServices
    {
        /// <summary>
        /// 获取控制系统权限
        /// </summary>
        public string KongZhiXiTong { get; set; }
        /// <summary>
        /// 公共操作
        /// </summary>
        public PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        /// <summary>
        /// 取票
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="parames"></param>
        public override void Query(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            string json;
            PbProject.Logic.Order.Tb_Ticket_OrderBLL orderBll = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
            string sqlWhere = " OrderId='" + CommonMethod.GetFomartString(parames["OrderID"]) + "' ";

            List<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            Tb_Ticket_Order Order = OrderList != null && OrderList.Count > 0 ? OrderList[0] : null;
            if (Order == null)
            {
                writer.WriteEx(546, "order num err", "订单编号错误");
            }
            else
            {
                sqlWhere = " OrderId='" + Order.OrderId + "' ";
                List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;
                json = ListToJSon(PassengerList, Order.OrderId);
                writer.Write(json);
            }
        }

        /// <summary>
        /// 退废改签
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="parames"></param>
        public override void Update(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            UserLoginInfo userInfo = AuthLogin.GetUserInfo(Username);
            KongZhiXiTong = BaseParams.getParams(userInfo.SupParameters).KongZhiXiTong;

            PbProject.Logic.Order.Tb_Ticket_OrderBLL orderBll = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
            string sqlWhere = " OrderID='" + CommonMethod.GetFomartString(parames["OrderID"]) + "' ";

            List<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            Tb_Ticket_Order Order = OrderList != null && OrderList.Count > 0 ? OrderList[0] : null;
            if (Order == null)
            {
                writer.WriteEx(546, "order num err", "订单编号错误");
            }
            else
            {
                sqlWhere = " OrderId='" + Order.OrderId + "' ";
                List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;
                List<Tb_Ticket_SkyWay> NewSkyWayList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { sqlWhere + " order by FromDate " }) as List<Tb_Ticket_SkyWay>;
                //提示信息
                string msg = "";

                try
                {
                    //申请类型3退票申请 4废票申请 5改签申请
                    string ApplayType = CommonMethod.GetFomartString(parames["ApplayType"]);
                    //扩展参数
                    ParamEx pe = new ParamEx();
                    pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                    //发送指令管理类
                    SendInsManage SendIns = new SendInsManage(userInfo.User.LoginName, userInfo.Company.UninCode, pe, userInfo.Configparam);
                    //订单日志
                    StringBuilder orderLog = new StringBuilder();

                    #region

                    //选择的乘客信息
                    List<HashObject> pasList = GetPassengerInfo(Order, PassengerList, CommonMethod.GetFomartString(parames["pasList"]));
                    //选中的 成人 儿童 婴儿
                    int AdultCount = 0, CHDCount = 0, INFCount = 0;
                    //没选中的 成人 儿童 婴儿
                    int NoSelAdultCount = 0, NoSelCHDCount = 0, NoSelINFCount = 0;
                    //选择的乘机人
                    List<HashObject> selList = new List<HashObject>();
                    foreach (HashObject param in pasList)
                    {
                        if (param["pasIsBack"].ToString() == "0") //获取没有提交的用户
                        {
                            //选择的乘客数
                            if (param["IsChecked"].ToString() == "1")
                            {
                                if (param["pasPType"].ToString() == "1")
                                {
                                    AdultCount++;
                                }
                                else if (param["pasPType"].ToString() == "2")
                                {
                                    CHDCount++;
                                }
                                else if (param["pasPType"].ToString() == "3")
                                {
                                    INFCount++;
                                }
                                selList.Add(param);
                            }
                            else
                            {
                                if (param["pasPType"].ToString() == "1")
                                {
                                    NoSelAdultCount++;
                                }
                                else if (param["pasPType"].ToString() == "2")
                                {
                                    NoSelCHDCount++;
                                }
                                else if (param["pasPType"].ToString() == "3")
                                {
                                    NoSelINFCount++;
                                }
                            }
                        }
                    }

                    #endregion

                    //航段信息
                    List<HashObject> skyList = GetSkyInfo(Order, NewSkyWayList, CommonMethod.GetFomartString(parames["skywayList"]));

                    #region
                    if (ApplayType != "5")//退废票
                    {
                        //行程单状态判断                           
                        if (!KongZhiXiTong.Contains("|68|"))
                        {
                            if (string.IsNullOrEmpty(msg))
                            {
                                //行程单状态判断
                                if (!TripIsVoid(userInfo, pasList, SendIns, out msg))
                                    msg = (string.IsNullOrEmpty(msg)) ? "不能提交,行程单状态异常！" : msg;
                            }
                        }
                        if (string.IsNullOrEmpty(msg))
                        {
                            //票号状态判断
                            if (!KongZhiXiTong.Contains("|69|"))
                            {
                                if (string.IsNullOrEmpty(msg))
                                {
                                    //票号状态判断
                                    if (!PassengerTicketIsOpen(pasList, skyList, SendIns, out msg))
                                        msg = (string.IsNullOrEmpty(msg)) ? "不能提交,票号状态异常！" : msg;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(msg))
                        {
                            //废票 飞机起飞两个半小时内不能提交废票操作，只能走退票通道
                            if (ApplayType == "4" && skyList.Count > 0)
                            {
                                //是否可以提交废票 1可以 0不可以
                                HashObject strArr = skyList[0] as HashObject;

                                if (strArr.Count == 11 && strArr["IsFP"].ToString() == "0")
                                {
                                    if (KongZhiXiTong != null && !KongZhiXiTong.Contains("|16|"))
                                        msg = "飞机起飞两个半小时内不能提交废票操作，只能走退票通道";
                                }
                            }
                        }
                    }

                    #endregion

                    if (string.IsNullOrEmpty(msg))
                        msg = TimeIsCommit(ApplayType, Order);

                    if (string.IsNullOrEmpty(msg))
                    {
                        //含有没有提交的婴儿并且没有成人时 不可以提交
                        if (NoSelINFCount > 0 && NoSelAdultCount == 0)
                        {
                            msg = "婴儿订单必须有成人陪同！";
                            orderLog.Append("成人带婴儿订单,需要手动处理编码！");
                        }
                    }

                    if (string.IsNullOrEmpty(msg))
                    {
                        string divMsg = "";
                        if (Order.OrderSourceType == 1)//白屏预订
                        {
                            //退费票且没有婴儿
                            if (ApplayType != "5" && (AdultCount > 0 || CHDCount > 0))
                            {
                                //开启退废票（分离、取消）编码
                                if (KongZhiXiTong.Contains("|32|"))
                                {
                                    ////退废处理 判断 分离编码 和 取消编码
                                    divMsg = PnrHandle(Order, SendIns, orderLog, selList, INFCount);
                                }
                            }
                        }
                        if (divMsg != "" && INFCount == 0)
                        {

                        }
                        else
                        {
                            //处理退改签
                            msg = TGQ(userInfo, Order, NewSkyWayList, skyList, PassengerList, pasList,
                                 CommonMethod.GetFomartString(parames["remark"]),
                                 CommonMethod.GetFomartString(parames["quitReasonType"]),
                                 CommonMethod.GetFomartString(parames["reason"]));
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    //解锁
                    orderBll.LockOrder(false, Order.id.ToString(), userInfo.User, userInfo.Company);
                    writer.Write(msg);
                }
            }
        }

        /// <summary>
        /// 处理退改签 处理
        /// </summary>
        private string TGQ(UserLoginInfo userInfo, Tb_Ticket_Order Order, List<Tb_Ticket_SkyWay> OldSkyWayList, List<HashObject> newSkyWay, List<Tb_Ticket_Passenger> oldPassengerList, List<HashObject> newPasList, string TGQRemark, string quitReasonType, string reason)
        {
            bool result = false;
            string msg = "";
            //选择申请类型
            int ApplayType = int.Parse(CommonMethod.GetFomartString(parames["ApplayType"]));
            try
            {
                //订单管理类
                Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();

                List<Tb_Ticket_Passenger> tempPassengerList = new List<Tb_Ticket_Passenger>();

                //记录出票订单id
                List<string> passengerId = new List<string>();
                List<Tb_Ticket_Passenger> NewPassengerList = new List<Tb_Ticket_Passenger>();
                Tb_Ticket_Passenger Passenger = null;
                //新订单号
                string NewOrderId = OrderManage.GetOrderId("0");
                int TicketNum = 0;

                #region 处理乘机人
                string strId = "", IsChecked = "0";
                for (int i = 0; i < newPasList.Count; i++)
                {
                    Passenger = new Tb_Ticket_Passenger();
                    //strId = newPasList[i].GetValue<string>("id");
                    strId = newPasList[i]["id"].ToString();
                    //IsChecked = newPasList[i].GetValue<string>("IsChecked");
                    IsChecked = newPasList[i]["IsChecked"].ToString();
                    Guid id = Guid.Parse(strId);

                    if (oldPassengerList != null && oldPassengerList.Count > 0)
                        tempPassengerList = oldPassengerList.Where(w => w.id == id).ToList<Tb_Ticket_Passenger>();

                    if (tempPassengerList != null && tempPassengerList.Count > 0)
                        Passenger = tempPassengerList[0];

                    //选中的乘客
                    if (IsChecked == "1")
                    {
                        //原乘客id
                        //passengerId.Add(newPasList[i].GetValue<string>("id"));
                        passengerId.Add(newPasList[i]["id"].ToString());
                        Passenger.id = Guid.NewGuid();
                        NewPassengerList.Add(Passenger);
                        TicketNum++;
                    }
                }

                #endregion

                #region 处理航段表

                //改签用到航班号和舱位
                List<string> FlightCodeList = new List<string>();
                List<string> SpaceList = new List<string>();
                //临时日期
                DateTime TempDate = DateTime.Parse("1901-01-01");
                DateTime fromDate = DateTime.Parse("1901-01-01");
                for (int i = 0; i < OldSkyWayList.Count; i++)
                {
                    //处理改签航段
                    if (ApplayType == 5)
                    {
                        foreach (HashObject skyParam in newSkyWay)
                        {
                            //找到相同航段id
                            if (skyParam.GetValue<string>("SkyId") == OldSkyWayList[i].id.ToString())
                            {
                                if (DateTime.TryParse(skyParam.GetValue<string>("FromDate"), out fromDate))
                                {
                                    OldSkyWayList[i].FromDate = fromDate;
                                }
                                if (TempDate.ToString("yyyy-MM-dd") == "1901-01-01")
                                {
                                    TempDate = OldSkyWayList[i].FromDate;
                                }
                                if (DateTime.TryParse(skyParam.GetValue<string>("ToDate"), out fromDate))
                                {
                                    OldSkyWayList[i].ToDate = fromDate;
                                }
                                OldSkyWayList[i].FlightCode = skyParam.GetValue<string>("FlightCode");
                                OldSkyWayList[i].Space = skyParam.GetValue<string>("Space");
                                //航班号和舱位
                                FlightCodeList.Add(OldSkyWayList[i].FlightCode);
                                SpaceList.Add(OldSkyWayList[i].Space);
                            }
                        }
                    }
                    //新id
                    OldSkyWayList[i].id = Guid.NewGuid();
                }

                #endregion

                if (TicketNum == 0)
                {
                    msg = "请选择乘客!";
                }
                else
                {

                    string PassengerNameS = "";
                    foreach (Tb_Ticket_Passenger item in NewPassengerList)
                    {
                        PassengerNameS += item.PassengerName + "|";
                    }
                    PassengerNameS = PassengerNameS.TrimEnd('|');


                    //新id
                    Order.id = Guid.NewGuid();
                    Order.PassengerName = PassengerNameS;
                    Order.PassengerNumber = NewPassengerList.Count;
                    Order.OldOrderId = Order.OrderId; //原订单号
                    Order.OrderId = NewOrderId; //新订单号
                    //Order.PayNo = "";
                    Order.InPayNo = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetIndexId();
                    Order.TicketStatus = ApplayType; //机票状态
                    Order.CreateTime = DateTime.Now;
                    Order.YDRemark = TGQRemark;  //退改签申请备注
                    Order.PayDebtsMoney = 0;
                    Order.LockLoginName = "";
                    Order.LockCpyNo = "";
                    Order.LockTime = DateTime.Parse("1900-01-01 00:00:00.000");

                    string strType = "";

                    if (ApplayType == 3)//退票
                    {
                        Order.OrderStatusCode = 7;
                        Order.A8 = quitReasonType;

                        if (quitReasonType == "1")
                        {
                            strType = "自愿申请";
                            Order.TGQApplyReason = reason + "--自愿申请";  //申请退票理由:自愿

                        }
                        else
                        {
                            strType = "非自愿申请";
                            Order.TGQApplyReason = reason + "--非自愿申请"; //申请退票理由：非自愿
                        }
                    }
                    else if (ApplayType == 4)//废票
                    {
                        Order.OrderStatusCode = 8;
                        Order.TGQApplyReason = reason; //申请废票理由 
                    }
                    else if (ApplayType == 5)//改签
                    {
                        Order.OrderStatusCode = 6;
                        Order.AirTime = TempDate;
                        Order.FlightCode = string.Join("/", FlightCodeList.ToArray());
                        Order.Space = string.Join("/", SpaceList.ToArray());
                        //改签申请理由  
                        Order.TGQApplyReason = reason;

                        //改签处理其它参数
                        Order.PayWay = 0; //默认支付方式
                        Order.PayNo = "";
                    }

                    string isContent = ""; //是否保留编码日志

                    //生成订单
                    result = OrderManage.CreateOrderTFG(Order, NewPassengerList, passengerId, OldSkyWayList, userInfo.User, userInfo.Company, strType, isContent, out msg);
                }
            }
            catch (Exception ex)
            {

            }
            string values = "";

            if (result)
            {
                values = "订单申请成功!";
            }
            else
            {
                if (string.IsNullOrEmpty(msg))
                {
                    if (ApplayType == 3)
                        values = "订单申请退票失败!";
                    else if (ApplayType == 4)
                        values = "订单申请废票失败!";
                    else if (ApplayType == 5)
                        values = "订单申请改签失败!";
                }
                else
                {
                    values = msg;
                }
            }

            return values;
        }

        /// <summary>
        /// 获取乘机人信息
        /// </summary>
        /// <returns></returns>
        public List<HashObject> GetPassengerInfo(Tb_Ticket_Order Order, List<Tb_Ticket_Passenger> PassengerList, string passIDList)
        {
            List<HashObject> list = new List<HashObject>();
            try
            {
                HashObject parameter = null;
                string[] strArr = passIDList.Split(',');

                foreach (Tb_Ticket_Passenger passenger in PassengerList)
                {
                    parameter = new HashObject();
                    parameter.Add("IsChecked", "0");//是否选中1是0否
                    foreach (string pasModel in strArr)
                    {
                        if (pasModel.Trim().Equals(passenger.id.ToString()))
                        {
                            parameter["IsChecked"] = "1";//是否选中1是0否
                            break;
                        }
                    }
                    parameter.Add("id", passenger.id);
                    parameter.Add("pasName", passenger.PassengerName);
                    parameter.Add("pasPType", passenger.PassengerType);
                    parameter.Add("pasCType", passenger.CType);
                    parameter.Add("pasCid", passenger.Cid);
                    parameter.Add("pasTravelNumber", passenger.TravelNumber);
                    parameter.Add("pasTicketNum", passenger.TicketNumber);
                    parameter.Add("pasIsBack", passenger.IsBack);
                    parameter.Add("office", Order.Office);
                    list.Add(parameter);
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        /// <summary>
        /// 获取航段信息
        /// </summary>
        /// <returns></returns>
        private List<HashObject> GetSkyInfo(Tb_Ticket_Order Order, List<Tb_Ticket_SkyWay> listsky, string skyStr)
        {
            List<HashObject> list = new List<HashObject>();
            string IsFP = "1";
            if (Order != null && listsky != null && listsky.Count > 0)
            {
                string TravelCode = Order.TravelCode.Split('/')[0];
                string Travel = Order.Travel.Split('/')[0];
                string fromcode = TravelCode.Split('-')[0];
                string tocode = TravelCode.Split('-')[1];
                string FromCity = Travel.Split('-')[0];
                string ToCity = Travel.Split('-')[1];
                string FromDate = "";
                //查找第一程航段
                Tb_Ticket_SkyWay sky = listsky.Find(delegate(Tb_Ticket_SkyWay _sky)
                {
                    return ((_sky.FromCityCode.ToUpper() == fromcode.ToUpper() || _sky.FromCityName == FromCity) && (_sky.ToCityCode.ToUpper() == tocode.ToUpper() || _sky.ToCityName == ToCity));
                });
                if (sky != null)
                {
                    FromDate = sky.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
                    //坐了飞机以后还能提交退票 增加一个废票的限制，飞机起飞两个半小时内不能提交废票操作，只能走退票通道 1能提交废票 0不能提交废票
                    double Minutes = (sky.FromDate - System.DateTime.Now).TotalMinutes;
                    IsFP = Minutes < 150 ? "0" : "1";
                }
            }
            HashObject parameter = null;
            //航段信息
            string[] strArr = skyStr.Split('^');
            foreach (string PasModel in strArr)
            {
                parameter = new HashObject();
                string[] _strArr = PasModel.Split(new string[] { "@@@@" }, StringSplitOptions.None);
                if (_strArr != null && _strArr.Length == 10)
                {
                    parameter.Add("SkyId", _strArr[0]);
                    parameter.Add("FromCityName", _strArr[1]);
                    parameter.Add("ToCityName", _strArr[2]);
                    parameter.Add("FromCityCode", _strArr[3]);
                    parameter.Add("ToCityCode", _strArr[4]);
                    parameter.Add("FromDate", _strArr[5]);
                    parameter.Add("ToDate", _strArr[6]);
                    parameter.Add("CarryCode", _strArr[7]);
                    parameter.Add("FlightCode", _strArr[8]);
                    parameter.Add("Space", _strArr[9]);
                    parameter.Add("IsFP", IsFP);//是否可以废票
                    list.Add(parameter);
                }
            }
            return list;
        }

        /// <summary>
        /// 该乘机人票号 提示对应的行程单是否作废
        /// </summary>
        /// <param name="pas"></param>
        /// <returns></returns>
        private bool TripIsVoid(UserLoginInfo userInfo, List<HashObject> strList, SendInsManage SendIns, out string msg)
        {
            bool Isvoid = true;
            msg = "";
            try
            {
                StringBuilder sbLog = new StringBuilder();
                string[] OfficeNum = userInfo.Configparam.Office.Split(new string[] { "|", " ", "/", ",", "，", "\\", "#" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (HashObject param in strList)
                {
                    if (param.Count == 9 && param["IsChecked"].ToString() == "1")
                    {
                        string PasName = param["pasName"].ToString().Trim();//乘机人姓名
                        string PasA8 = param["pasTravelNumber"].ToString().Trim();//行程单号                  
                        string PasTicketNumber = param["pasTicketNum"].ToString().Trim();//票号
                        //if (PasA8 != "")
                        //{
                        //Office
                        string office = param["office"].ToString().Trim();
                        //真实状态
                        string strInstruction = string.Format("DETR:TN/{0},F", PasTicketNumber);
                        string recvData = SendIns.Send(strInstruction, ref office, 0);
                        List<string> lst = new List<string>();
                        lst.AddRange(new string[]{
                    "没有权限",
                    "NOT EXIST",
                    "TICKET NUMBER",
                    "NOT FOUND",
                    "NO RECORD",
                    "超时"    ,
                    "无法从传输连接中读取数据"
                    });
                        bool isContain = false;
                        foreach (string item in lst)
                        {
                            if (recvData.ToUpper().Contains(item))
                            {
                                isContain = true;
                                break;
                            }
                        }
                        if (recvData != "" && !isContain && recvData.ToUpper().Contains("RP" + PasA8))
                        {
                            Isvoid = false;
                            sbLog.AppendFormat("乘机人:{0} 票号{1}所关联的行程单号未作废<br />请先作废行程单后再操作！", PasName, PasTicketNumber);
                        }
                        else
                        {
                            if (recvData.Contains("授权"))
                            {
                                Isvoid = false;
                                sbLog.AppendFormat("乘机人:{0} 票号{1}验证行程单状态失败,需授权,授权指令:RMK TJ AUTH " + (office == "" ? OfficeNum[0] : office.Trim(new char[] { '$' })), PasName, PasTicketNumber);
                            }
                        }
                        // }
                    }
                }
                msg = sbLog.ToString();
            }
            catch (Exception)
            {
            }
            return Isvoid;
        }

        /// <summary>
        /// 票号状态是否为Open for Use 状态 true为Open for Use  否则不是
        /// </summary>
        /// <param name="TicketNumber"></param>
        /// <param name="Office"></param>
        /// <returns></returns>
        private bool PassengerTicketIsOpen(List<HashObject> pasList, List<HashObject> skyList, SendInsManage SendIns, out string errMsg)
        {
            bool IsOpen = false;
            errMsg = "";
            //第一航段
            HashObject skyParam = skyList[0];
            StringBuilder sbLog = new StringBuilder();
            //格式化pnr类
            PnrAnalysis.FormatPNR format = new PnrAnalysis.FormatPNR();
            string TicketNumber = "", Office = "", Status = "", PassengerName = "", FlyDate = "";
            bool IsSuc = false;
            if (pasList != null && pasList.Count > 0)
            {
                foreach (HashObject param in pasList)
                {
                    if (param.Count == 9 && param["IsChecked"].ToString() == "1")
                    {
                        PassengerName = param["pasName"].ToString().Trim();//乘机人姓名
                        TicketNumber = param["pasTicketNum"].ToString().Trim();//票号
                        Office = param["office"].ToString().Trim();//Office
                        FlyDate = skyParam["FromDate"].ToString().Trim();//乘机日期     

                        if (TicketNumber == "")
                        {
                            continue;
                        }
                        //提取票号信息指令组合
                        string strInstruction = string.Format("DETR:TN/{0}", TicketNumber);
                        //发送指令
                        string recvData = SendIns.Send(strInstruction, ref Office, 0);
                        if ((recvData.ToUpper().Contains("NOT EXIST") || recvData.ToUpper().Contains("TICKET NUMBER") || recvData.ToUpper().Contains("NOT FOUND") || recvData.ToUpper().Contains("AUTHORITY") || recvData.ToUpper().Contains("没有权限")) && FlyDate != "")
                        {
                            //检查乘机日期
                            DateTime dt1 = System.DateTime.Parse("1901-01-01");
                            DateTime.TryParse(FlyDate, out dt1);
                            if (DateTime.Compare(dt1, System.DateTime.Now) < 0)
                            {
                                //乘机日期已过期 不能提交
                                IsSuc = true;
                                sbLog.AppendFormat("乘机人{0}票号{1}乘机日期已过期,不能提交申请退废票！", PassengerName, TicketNumber);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        //票号状态
                        Status = format.GetTicketStatus(recvData);
                        if (Status.ToUpper() != "OPEN FOR USE")
                        {
                            IsSuc = true;
                            if (Status == "")
                            {
                                sbLog.AppendFormat("乘机人{0}票号{1}为无效票号,不能提交申请退废票！", PassengerName, TicketNumber);
                            }
                            else
                            {
                                sbLog.AppendFormat("乘机人{0}票号（{1}）状态为{2},不能提交申请退废票！", PassengerName, TicketNumber, Status);
                            }
                            break;
                        }
                    }
                }
                if (!IsSuc)
                {
                    IsOpen = true;
                }
                errMsg = sbLog.ToString();
            }
            return IsOpen;
        }

        /// <summary>
        /// 在指定时间范围内是否可以提价申请
        /// </summary>
        /// <param name="ApplayType"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        private string TimeIsCommit(string ApplayType, Tb_Ticket_Order Order)
        {
            //true可以提交申请 false不能提交申请
            string msg = "";

            try
            {
                if (ApplayType == "3")//退票
                {

                }
                else if (ApplayType == "4")//废
                {
                    if (Order.CPTime.ToString("yyyy-MM-dd") == System.DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        //获取当天废票时间(接口和本地)
                        string[] strTime = GetTime(Order);
                        string PolicyCancelTime = strTime[1];
                        DateTime nowTime = DateTime.Now.AddMinutes(20);//提交时间 ，提前20 分钟提交

                        if (PolicyCancelTime != "" && PolicyCancelTime.Split('-').Length == 2)
                        {
                            string start = DateTime.Now.ToString("yyyy-MM-dd") + " " + PolicyCancelTime.Split('-')[0] + ":00";
                            string end = PolicyCancelTime.Split('-')[1] + ":00";
                            if (end == "00:00:00")
                            {
                                end = "23:59:59";
                            }
                            end = DateTime.Now.ToString("yyyy-MM-dd") + " " + end;
                            DateTime temp = DateTime.Parse(start);
                            DateTime temp1 = DateTime.Parse(end);

                            //在指定时间范围内就可以提交
                            if (DateTime.Compare(temp, nowTime) < 0 && DateTime.Compare(temp1, nowTime) > 0)
                            {
                                //可以提交
                            }
                            else
                            {
                                //不能提交
                                msg = string.Format("不能提交废票申请,请在供应商下班前20分钟提交申请！{0}如有疑问请来电咨询！", Order.PolicyCancelTime != "" ? "处理时间(" + Order.PolicyCancelTime + ")" : "");
                            }
                        }
                        else
                        {
                            // 未设置时间 可以提交
                        }
                    }
                    else
                    {
                        //不是当天出票不能废

                        msg = string.Format("不能提交废票申请,请在供应商下班前20分钟提交申请！{0}如有疑问请来电咨询！", Order.PolicyCancelTime != "" ? "处理时间(" + Order.PolicyCancelTime + ")" : "");
                    }
                }
                else if (ApplayType == "5")//废
                {
                    //获取当天废票时间(接口和本地)
                    string[] strTime = GetTime(Order);
                    string PolicyCancelTime = strTime[1];
                    DateTime nowTime = DateTime.Now.AddMinutes(20);//提交时间 ，提前20 分钟提交

                    if (PolicyCancelTime != "" && PolicyCancelTime.Split('-').Length == 2)
                    {
                        string start = DateTime.Now.ToString("yyyy-MM-dd") + " " + PolicyCancelTime.Split('-')[0] + ":00";
                        string end = PolicyCancelTime.Split('-')[1] + ":00";
                        if (end == "00:00:00")
                        {
                            end = "23:59:59";
                        }
                        end = DateTime.Now.ToString("yyyy-MM-dd") + " " + end;
                        DateTime temp = DateTime.Parse(start);
                        DateTime temp1 = DateTime.Parse(end);

                        //在指定时间范围内就可以提交
                        if (DateTime.Compare(temp, nowTime) < 0 && DateTime.Compare(temp1, nowTime) > 0)
                        {
                            //可以提交
                        }
                        else
                        {
                            //不能提交
                            msg = string.Format("不能提交改签申请,请在供应商下班前20分钟提交申请！{0}如有疑问请来电咨询！", Order.PolicyCancelTime != "" ? "处理时间(" + Order.PolicyCancelTime + ")" : "");
                        }
                    }
                    else
                    {
                        // 未设置时间 可以提交
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "时间判断有误,不能提交！";
            }

            return msg;
        }

        /// <summary>
        /// 获取时间（处理时间） 
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        private string[] GetTime(Tb_Ticket_Order Order)
        {
            string[] time = new string[2];
            time[0] = "";
            time[1] = "";

            try
            {
                if (Order.PolicySource > 2)
                {
                    //接口
                    if (!string.IsNullOrEmpty(Order.PolicyCancelTime))
                    {
                        time[0] = Order.PolicyCancelTime;
                        time[1] = Order.PolicyCancelTime;
                    }
                }
                if (string.IsNullOrEmpty(time[0]))
                {
                    string sqlWhere = "UninCode='" + Order.CPCpyNo + "'"; //出票方工作时间
                    PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                    List<User_Company> User_CompanyList = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { sqlWhere }) as List<User_Company>;

                    if (User_CompanyList != null && User_CompanyList.Count > 0)
                    {
                        time[0] = User_CompanyList[0].WorkTime;
                        time[1] = User_CompanyList[0].BusinessTime;
                    }
                }
            }
            catch (Exception)
            {

            }
            return time;
        }

        /// <summary>
        /// 分离或者取消编码
        /// </summary>
        /// <returns></returns>
        private string PnrHandle(Tb_Ticket_Order Order, SendInsManage SendIns, StringBuilder sbLog, List<HashObject> selList, int INFCount)
        {
            string ErrMsg = "";
            try
            {
                if (Order != null && Order.PNR != "" && Order.PNR.Trim().Length == 6)
                {
                    PnrAnalysis.PnrModel pnrMode = SendIns.GetPnr(Order.PNR.Trim(), out ErrMsg);
                    if (pnrMode != null)
                    {
                        if (ErrMsg.Contains("授权"))
                        {
                            string[] strAuthArr = ErrMsg.Split('#')[1].Split('|');
                            ErrMsg = "";
                            foreach (string item in strAuthArr)
                            {
                                ErrMsg += "RMK TJ AUTH " + item + "<br/>";
                            }
                            ErrMsg = "需要授权,授权指令:<br />" + ErrMsg;
                            sbLog.Append("提取编码信息失败," + ErrMsg);
                        }
                        else
                        {
                            if (!pnrMode.PnrStatus.Contains("XX"))
                            {
                                if (pnrMode.PnrStatus.Contains("NO"))
                                {
                                    ErrMsg = "编码（" + Order.PNR + "）状态为NO，系统取消编码失败！";
                                    sbLog.Append(ErrMsg);
                                }
                                else if (pnrMode.PnrStatus.Contains("RR"))
                                {
                                    ErrMsg = "编码（" + Order.PNR + "）状态为RR，该编码已出票，不能取消！";
                                    sbLog.Append(ErrMsg);
                                }
                                else
                                {
                                    string TicketNumber = "";
                                    string Cmd = "", Office = "", RecvData = "";
                                    //用于分离编码的乘客姓名 不含婴儿
                                    List<string> pnameList = new List<string>();
                                    //循环选择的乘机人
                                    foreach (HashObject hashParam in selList)
                                    {
                                        if (hashParam["pasPType"].ToString() != "3")
                                        {
                                            pnameList.Add(hashParam["pasName"].ToString());
                                        }
                                        TicketNumber = hashParam["pasTicketNum"].ToString();
                                        if (TicketNumber != "" && !KongZhiXiTong.Contains("|69|"))
                                        {
                                            Office = hashParam["office"].ToString();
                                            Cmd = "DETR:TN/" + TicketNumber;
                                            RecvData = SendIns.Send(Cmd, ref Office, 0);
                                            if (!RecvData.Contains("OPEN FOR USE"))
                                            {
                                                ErrMsg = "票号（" + TicketNumber + "）状态异常，不能提交申请！";
                                                sbLog.Append(ErrMsg);
                                                break;
                                            }
                                        }
                                    }
                                    //没有出错时 
                                    if (ErrMsg == "")
                                    {
                                        if (INFCount > 0)
                                        {
                                            //编码状态不是已经出票的PNR
                                            if (pnrMode != null && !pnrMode.PnrStatus.Contains("RR"))
                                            {
                                                //有婴儿直接提交
                                                SendIns.CancelPnr(Order.PNR, Order.Office);
                                            }
                                        }
                                        else
                                        {
                                            //编码状态不是已经出票的PNR
                                            if (pnrMode != null)
                                            {
                                                if (!pnrMode.PnrStatus.Contains("RR"))
                                                {
                                                    //取消编码
                                                    if (SendIns.CancelPnr(Order.PNR, Order.Office))
                                                    {
                                                        ErrMsg = "";
                                                    }
                                                    else
                                                    {
                                                        ErrMsg = "取消编码(" + Order.PNR + ")失败！";
                                                    }
                                                }
                                                else
                                                {
                                                    ErrMsg = "编码状态为RR的编码,不能取消！";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        sbLog.Append("提取编码信息失败，系统取消编码失败！");
                    }
                }
                else
                {
                    sbLog.Append("订单不存在！");
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            finally
            {
            }
            return ErrMsg;
        }

        //{"OrderID":{"PassengerID":"123","PassengerName":"XXX","TicketNumber":"12345678"},
        //           {"PassengerID":"123","PassengerName":"XXX","TicketNumber":"12345678"}}
        private string ListToJSon(List<Tb_Ticket_Passenger> passengerList, string orderID)
        {
            StringBuilder sb = new StringBuilder();
            if (passengerList.Count > 0)
            {
                sb.Append("{\"");
                sb.Append(orderID);
                sb.Append("\":");
                foreach (Tb_Ticket_Passenger passenger in passengerList)
                {
                    sb.Append("{\"PassengerID\":");
                    sb.Append("\"");
                    sb.Append(passenger.id);
                    sb.Append("\",");

                    sb.Append("\"PassengerName\":");
                    sb.Append("\"");
                    sb.Append(passenger.PassengerName);
                    sb.Append("\",");

                    sb.Append("\"TicketNumber\":");
                    sb.Append("\"");
                    sb.Append(passenger.TicketNumber);
                    sb.Append("\"},");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("}");
            }
            return sb.ToString();
        }
    }
}