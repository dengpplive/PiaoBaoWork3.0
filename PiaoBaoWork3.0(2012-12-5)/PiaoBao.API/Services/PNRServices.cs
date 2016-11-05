using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using PnrAnalysis.Model;
using PnrAnalysis;
using PbProject.Model;
using Newtonsoft.Json.Linq;
using PbProject.Logic.Order;
using PbProject.Logic.ControlBase;
using PbProject.Model.definitionParam;
using PbProject.Logic.Pay;

namespace PiaoBao.API.Services
{
    public class PNRServices:BaseServices
    {
        BaseDataManage baseDataManage = new BaseDataManage();
        bool allowChangePNRFlag;
        bool isAsAdultOrder;
        string cHDAssociationAdultOrderId;
        string travelType;
        bool isCHDToAudltTK;
        string linkMan;
        string linkManPhone;
        /// <summary>
        /// 创建PNR时 生成一个没有政策的订单信息
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="parames"></param>
        public override void Create(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            List<Tb_Ticket_Passenger> pList=new List<Tb_Ticket_Passenger>();
            List<Tb_Ticket_SkyWay> skywaylist = new List<Tb_Ticket_SkyWay>();

            #region 取提交的参数信息

            string adultPnr = parames["adultPnr"];//只是儿童时需要备注的成人编码
            string pListStr = parames["pList"];//乘机人信息 名称,类型,证件ID,儿童生日/名称,类型,证件ID,儿童生日
            string skywaylistStr = parames["skywaylist"];//航段信息 承运人,航班号,起飞日期,到达日期,起飞三字码,到达三字码,起飞城市,到达城市,舱位,折扣/航段信息 承运人,航班号,起飞日期,到达日期,起飞三字码,到达三字码,起飞城市,到达城市,舱位,折扣
            allowChangePNRFlag = parames["allowChangePNRFlag"]=="1";//是否允许换编码出票
            isAsAdultOrder = parames["isAsAdultOrder"] == "1";//是否关联成人订单号
            cHDAssociationAdultOrderId = parames["cHDAssociationAdultOrderId"];//关联成人定单号
            isCHDToAudltTK = parames["isCHDToAudltTK"] == "1";//儿童票出成人票
            linkMan = parames["linkMan"];//联系人
            linkManPhone = parames["linkManPhone"];//联系人电话
            travelType = parames["travelType"];//航班类型  1=单程，2=往返，3=中转联程 4=多程
            foreach (var pStr in pListStr.Split('/'))
            {
                var ps=pStr.Split(',');
                if (ps[1] == "2" && ps.Count() == 4)
                {
                        pList.Add(new Tb_Ticket_Passenger()
                        {
                            PassengerName = ps[0],
                            PassengerType = int.Parse(ps[1]),
                            Cid = ps[2],
                            A7 = ps[3]
                        });
                }
                else if (ps[1] != "2")
	            {
                     pList.Add(new Tb_Ticket_Passenger()
                        {
                            PassengerName = ps[0],
                            PassengerType = int.Parse(ps[1]),
                            Cid = ps[2]
                        });
	            }
                
                else
                {
                    writer.WriteEx(550, "passenger format error", "乘机人格式有误");
                }
            }
            foreach (var kStr in skywaylistStr.Split('/'))
            {
                var ks = kStr.Split(',');
                if (ks.Count() == 10)
                {
                    skywaylist.Add(new Tb_Ticket_SkyWay()
                    {
                        CarryCode = ks[0],
                        FlightCode = ks[1],
                        FromDate = DateTime.Parse(ks[2]),
                        ToDate = DateTime.Parse(ks[3]),
                        FromCityCode = ks[4],
                        ToCityCode = ks[5],
                        FromCityName=ks[6],
                        ToCityName=ks[7],
                        Space = ks[8],
                        Discount = ks[9]
                    });
                }
                else
                {
                    writer.WriteEx(550, "skyway format error", "航段格式有误");
                }
            }
            #endregion

            UserLoginInfo userLogin = AuthLogin.GetUserInfo(Username);
            List<IPassenger> pasList = new List<IPassenger>();
            List<ISkyLeg> skyList = new List<ISkyLeg>();
            SendNewPID pid = new SendNewPID();
            PnrParamObj PnrParam = new PnrParamObj();
            //必填项 是否开启新版PID发送指令 
            PnrParam.UsePIDChannel = userLogin.FQP.KongZhiXiTong != null && userLogin.FQP.KongZhiXiTong.Contains("|48|") ? 2 : 0;  //2;
            PnrParam.ServerIP = userLogin.Configparam.WhiteScreenIP;
            PnrParam.ServerPort = int.Parse(string.IsNullOrEmpty(userLogin.Configparam.WhiteScreenPort) ? "0" : userLogin.Configparam.WhiteScreenPort);
            PnrParam.Office = userLogin.Configparam.Office;
            PnrParam.CarryCode = skywaylist[0].CarryCode;
            PnrParam.PasList = pasList;
            PnrParam.SkyList = skyList;
            //只是儿童时需要备注的成人编码 
            PnrParam.AdultPnr = adultPnr;
            //是否儿童出成人票
            PnrParam.ChildIsAdultEtdz = isCHDToAudltTK ? "1" : "0"; ;
            //可选项
            PnrParam.UserName = Username;
       

             
            //输入的手机号码 预订编码CT项电话
            if (userLogin.FQP.KongZhiXiTong == null || !userLogin.FQP.KongZhiXiTong.Contains("|19|"))
            {
                PnrParam.CTTel = userLogin.User != null ? userLogin.User.Tel : "";
                PnrParam.CTCTPhone = linkManPhone != "" ? linkManPhone : (userLogin.Company != null && userLogin.Company.ContactTel.Trim() != "" ? userLogin.Company.ContactTel.Trim() : "");
            }
            else
            {
                PnrParam.CTTel = userLogin.mSupCompany.Tel != null ? userLogin.mSupCompany.Tel : "";
                PnrParam.CTCTPhone = linkManPhone != "" ? linkManPhone : (userLogin.mSupCompany != null && userLogin.mSupCompany.ContactTel.Trim() != "" ? userLogin.mSupCompany.ContactTel.Trim() : "");
            }

            //关闭生成订单联系人默认值   生成订单时，联系人不需要默认值，让用户自己填写  
            if (userLogin.FQP.KongZhiXiTong != null && userLogin.FQP.KongZhiXiTong.Contains("|55|"))
            {
                PnrParam.CTTel = userLogin.User != null ? userLogin.User.Tel : "";
                PnrParam.CTCTPhone = linkManPhone;
            }


            PnrParam.PID = userLogin.Configparam.Pid;
            PnrParam.KeyNo = userLogin.Configparam.KeyNo;
            //乘机人
            foreach (Tb_Ticket_Passenger pas in pList)
            {
                IPassenger p1 = new IPassenger();
                pasList.Add(p1);
                p1.PassengerName = pas.PassengerName;
                p1.PassengerType = pas.PassengerType;
                p1.PasSsrCardID = pas.Cid;
                p1.ChdBirthday = pas.A7;
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
            string msg = "";
            if (GenerationOrder(userLogin, skywaylist, pList, pObj,out msg))
            {
                writer.Write(new { pnr = pObj, orderID = msg });
            }
            else
            {
                writer.WriteEx(565, "create order error", "创建空白定单失败:"+msg);
            }

            
        }
        #region 创建没有政策的订单操作
        /// <summary>
        /// 生成没有政策信息的订单，确定订单时回写政策数据
        /// </summary>
        /// <param name="FQP"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool GenerationOrder(UserLoginInfo userLogin, List<Tb_Ticket_SkyWay> skyWay, List<Tb_Ticket_Passenger> pasList, RePnrObj pnrInfo,out string msg)
        {
            bool IsSuc = false;
            msg="";
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
                if (userLogin.Company == null || userLogin.BaseParametersList == null)
                {
                    msg = "mCompany公司信息丢失";
                    return IsSuc;
                }
                //订单管理
                Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
                OrderInputParam OrderParam = new OrderInputParam();
                OrderMustParamModel ParamModel = new OrderMustParamModel();

                //构造生成订单需要的信息
                List<Tb_Ticket_SkyWay> SkyWay1 = skyWay;
                //构造乘客信息            
                List<Tb_Ticket_Passenger> Paslist = pasList;
                //订单日志记录
                Log_Tb_AirOrder logOrder = new Log_Tb_AirOrder();
                logOrder.OperTime = DateTime.Now;
                logOrder.OperType = "创建订单";
                logOrder.OperContent =userLogin.User.LoginName + "于" + logOrder.OperTime + "创建订单。";
                logOrder.OperLoginName =userLogin.User.LoginName;
                logOrder.OperUserName = userLogin.User.UserName;
                logOrder.CpyNo = userLogin.Company.UninCode;
                logOrder.CpyName = userLogin.Company.UninName;
                logOrder.CpyType = userLogin.Company.RoleType;
                logOrder.WatchType = 5;
                if (IsExistCHD)
                {
                    //添加权限 是否可以预定儿童票 未加
                    if (userLogin.FQP.GongYingKongZhiFenXiao != null && userLogin.FQP.GongYingKongZhiFenXiao.Contains("|90|"))
                    {
                        msg = "目前暂时无法预订儿童票！";
                    }
                }

                //关联成人订单号
                if (isAsAdultOrder)
                {
                    #region 关联成人订单号
                    //开启儿童编码必须关联成人编码或者成人订单号
                    if (userLogin.FQP.KongZhiXiTong != null && userLogin.FQP.KongZhiXiTong.Contains("|95|"))
                    {
                        string sqlWhere = "";
                        //儿童订单关联成人订单号
                        if (cHDAssociationAdultOrderId == "")
                        {
                            msg = "关联成人订单不能为空！";
                        }
                        else
                        {
                            sqlWhere = string.Format(" OrderId='{0}' ", cHDAssociationAdultOrderId);
                            
                            List<Tb_Ticket_Order> list = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Order>;
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
                                    if (userLogin.FQP.KongZhiXiTong == null || !userLogin.FQP.KongZhiXiTong.Contains("|91|"))
                                    {
                                        if (msg == "" && !ValSkyWay(cHDAssociationAdultOrderId, SkyWay1))
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
                    string defaultOffice =userLogin.Configparam.Office.Split('^')[0];
                    string CpyNo = userLogin.Company.UninCode, CarryCode = SkyWay1[0].CarryCode;
                    string PrintOffice = GetPrintOffice(CpyNo, CarryCode);
                    if (!string.IsNullOrEmpty(PrintOffice))
                    {
                        //Office = PrintOffice;
                    }
                    
                    //记录指令
                    SaveInsInfo(pnrInfo, userLogin);
                    pnrInfo.PrintOffice = PrintOffice;
                    //记录Pnr日志Id=
                    List<string> pnrLogList = new List<string>();
                    string AdultPnr = string.Empty;
                    string childPnr = string.Empty;
                    //成人预订信息编码记录            
                    if (pnrInfo.AdultYudingList.Count > 0)
                    {
                        AdultPnr = pnrInfo.AdultPnr;
                        if (string.IsNullOrEmpty(AdultPnr) || AdultPnr.Trim().Length != 6)
                        {
                            //提示pnr失败信息
                            string yudingRecvData = pnrInfo.AdultYudingList.Values[0];
                            msg = ShowPnrFailInfo(1, yudingRecvData);
                            AdultPnr = "";
                        }
                        //记录编码日志
                        YuDingPnrLog(userLogin, pnrInfo, pnrInfo.AdultYudingList.Keys[0], pnrInfo.AdultYudingList.Values[0], AdultPnr, pnrInfo.Office, ref pnrLogList);
                        if (AdultPnr.Length == 6 && (pnrInfo.PatModelList[0] == null || pnrInfo.PatModelList[0].PatList.Count == 0))
                        {
                            msg = "成人编码" + AdultPnr + "未能PAT取到价格，原因如下:<br />" + pnrInfo.PatList[0];
                        }
                    }
                    //儿童预订信息编码记录
                    if (pnrInfo.ChildYudingList.Count > 0)
                    {
                        childPnr = pnrInfo.childPnr;
                        if (string.IsNullOrEmpty(childPnr) || childPnr.Trim().Length != 6)
                        {
                            //提示pnr失败信息
                            string yudingRecvData = pnrInfo.ChildYudingList.Values[0];
                            msg = ShowPnrFailInfo(2, yudingRecvData);
                            childPnr = "";
                        }
                        //记录编码日志
                        YuDingPnrLog(userLogin, pnrInfo, pnrInfo.ChildYudingList.Keys[0], pnrInfo.ChildYudingList.Values[0], childPnr, pnrInfo.Office, ref pnrLogList);
                        if (childPnr.Length == 6 && (pnrInfo.PatModelList[1] == null || pnrInfo.PatModelList[1].PatList.Count == 0))
                        {
                            msg = "儿童编码" + childPnr + "未能PAT取到价格，原因如下:<br />" + pnrInfo.PatList[0];
                        }
                    }
                    #endregion

                    #region 组合生成订单所需要的实体数据
                    //成人+婴儿 成人+成人 儿童+备注成人订单号  只生成一个订单
                    //成人+儿童且没有备注订单号  成人+儿童+婴儿   生成两个订单
                    //存在儿童 也存在成人
                    if (IsExistCHD && IsExistAdult)
                    {
                        if (!isAsAdultOrder)
                        {
                            IsSecOrder = true;
                            //生成儿童订单
                            List<Tb_Ticket_SkyWay> SkyWay2 =skyWay;
                            //儿童乘客列表
                            List<Tb_Ticket_Passenger> ChildList = new List<Tb_Ticket_Passenger>();
                            foreach (Tb_Ticket_Passenger pas in Paslist)
                            {
                                if (pas.PassengerType == 2)
                                {
                                    ChildList.Add(pas);
                                }
                            }
                            ChildOrder = GetOrder(userLogin, true, ChildList, SkyWay2);
                            Log_Tb_AirOrder logOrder1 = new Log_Tb_AirOrder();
                            logOrder1.OperTime = DateTime.Now;
                            logOrder1.OperType = "创建订单";
                            logOrder1.OperContent = userLogin. User.LoginName + "于" + logOrder.OperTime + "创建订单。";
                            logOrder1.OperLoginName = userLogin.User.LoginName;
                            logOrder1.OperUserName = userLogin.User.UserName;
                            logOrder1.CpyNo = userLogin.Company.UninCode;
                            logOrder1.CpyName = userLogin.Company.UninName;
                            logOrder1.CpyType = userLogin.Company.RoleType;
                            logOrder1.WatchType = 5;

                            //加入参数
                            OrderMustParamModel ParamModel1 = new OrderMustParamModel();
                            OrderParam.PnrInfo = pnrInfo;
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
                        AdultOrder = GetOrder( userLogin, false, Paslist, SkyWay1);
                    }
                    else
                    {
                        //为一个订单时
                        AdultOrder = GetOrder(userLogin, IsExistCHD, Paslist, SkyWay1);
                        if (isAsAdultOrder)
                        {
                            AdultOrder.PNR = RmkAdultPnr;
                            AdultOrder.AssociationOrder = cHDAssociationAdultOrderId;
                        }
                    }
                    //  是否有婴儿
                    AdultOrder.HaveBabyFlag = IsExistINF;
                    //
                    OrderParam.PnrInfo = pnrInfo;
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
                            Paramlist.Add("ChildOrderId=" + item.Order.OrderId);
                        else
                            Paramlist.Add("AdultOrderId=" + item.Order.OrderId);
                    }
                    if (IsSuc)
                    {
                        msg = string.Join("&", Paramlist.ToArray());
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
                //异常信息
                msg = ex.Message;
                IsSuc = false;
            }
            return IsSuc;
        }
        /// <summary>
        /// 验证儿童与成人航线是否一样
        /// </summary>
        /// <param name="AdtOrderId"></param>
        /// <param name="CHDSkyWay"></param>
        /// <returns></returns>
        private bool ValSkyWay(string AdtOrderId, List<Tb_Ticket_SkyWay> CHDSkyWay)
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
                DataBase.LogCommon.Log.Error("Create.aspx页面ValSkyWay", ex);
                IsSuc = false;
            }
            return IsSuc;
        }
        /// <summary>
        /// 获取航空公司出票Office号 
        /// </summary>
        /// <param name="CarryCode"></param>
        /// <param name="defaultOffice"></param>
        /// <returns></returns>
        private string GetPrintOffice(string CpyNo, string CarryCode)
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
        /// 保存指令信息到数据库
        /// </summary>
        /// <returns></returns>
        private bool SaveInsInfo(RePnrObj PnrInfo, UserLoginInfo userLogin)
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
                    string UserAccount = userLogin.User.LoginName, CpyNo =userLogin.Company.UninCode, serverIPPort = PnrInfo.ServerIP + ":" + PnrInfo.ServerPort, Office = PnrInfo.Office;
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
                DataBase.LogCommon.Log.Error("Create.aspx页面SaveInsInfo", ex);
            }
            return IsSuc;
        }
        /// <summary>
        /// 提示编码生成失败原因
        /// </summary>
        /// <param name="type"></param>
        /// <param name="yudingRecvData"></param>
        /// <returns></returns>
        private string ShowPnrFailInfo(int type, string yudingRecvData)
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
            else if (yudingRecvData.ToUpper().Contains("WSACancelBlockingCall") || yudingRecvData.ToUpper().Contains("TRANSACTION IN PROGRESS") || yudingRecvData.ToUpper().Contains("FORMAT") || yudingRecvData.ToUpper().Contains("NO PNR") || yudingRecvData.ToUpper().Contains("CHECK TKT DATE") || yudingRecvData.ToUpper().Contains("为空") || yudingRecvData.ToUpper().Contains("对象的实例"))
            {
                msg = (type == 1 ? "成人" : "儿童") + "编码生成失败！原因如下:<br />" + yudingRecvData;
            }
            else
            {
                msg = (type == 1 ? "成人" : "儿童") + "编码生成失败！";
            }
            return msg;
        }
        /// <summary>
        /// 预订编码日志
        /// </summary>
        /// <param name="SSContent"></param>
        /// <param name="ResultContent"></param>
        /// <param name="Pnr"></param>
        /// <param name="pnrLogList"></param>
        /// <returns></returns>
        private bool YuDingPnrLog(UserLoginInfo userLogin, RePnrObj PnrInfo, string SSContent, string ResultContent, string Pnr, string Office, ref List<string> pnrLogList)
        {
            bool Insert = false;
            try
            {
                Log_Pnr logpnr = new Log_Pnr();
                logpnr.CpyNo = userLogin.Company.UninCode;
                logpnr.CpyName = userLogin.Company.UninCode;
                logpnr.CpyType = userLogin.Company.RoleType;
                logpnr.OperTime = System.DateTime.Now;
                logpnr.OperLoginName = userLogin.User.LoginName;
                logpnr.OperUserName = userLogin.User.UserName;
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
                DataBase.LogCommon.Log.Error("Create.aspx页面YuDingPnrLog", ex);
            }
            return Insert;
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
        private Tb_Ticket_Order GetOrder(UserLoginInfo userLogin, bool IsChild, List<Tb_Ticket_Passenger> PasList, List<Tb_Ticket_SkyWay> SkyWayList)
        {
            Tb_Ticket_Order to = new Tb_Ticket_Order();
            try
            {
                to.LinkMan = linkMan == "" ? userLogin.Company.ContactUser : linkMan;
                to.LinkManPhone = linkManPhone == "" ? userLogin.Company.ContactTel : linkManPhone;
                //白屏预订
                to.OrderSourceType = 5;//
                to.OrderStatusCode = 1;//默认新订单等待支付
                to.PolicySource = 1;//默认b2b


                to.CreateCpyName = userLogin.Company != null ? userLogin.Company.UninAllName : "";
                to.CreateCpyNo = userLogin.Company != null ? userLogin.Company.UninCode : "";

                to.CreateLoginName = userLogin.User.LoginName;
                to.CreateUserName = userLogin.User.UserName;
                to.OwnerCpyNo = userLogin.Company != null ? userLogin.Company.UninCode : "";
                to.OwnerCpyName = userLogin.Company != null ? userLogin.Company.UninAllName : "";
                //是否允许换编码出票
                to.AllowChangePNRFlag = allowChangePNRFlag;
               
                to.TravelType =int.Parse(travelType);
                to.CarryCode = SkyWayList[0].CarryCode;
                to.FlightCode = SkyWayList[0].FlightCode;

                //to.AirTime = GetAirTime(ViewState["Time"].ToString());
                to.AirTime = SkyWayList[0].FromDate;

                to.Travel = SkyWayList[0].FromCityName + "-" + SkyWayList[0].ToCityName + (SkyWayList.Count == 1 ? "" : "/"+SkyWayList[1].FromCityName + "-" + SkyWayList[1].ToCityName);
                to.TravelCode = SkyWayList[0].FromCityCode + "-" + SkyWayList[0].ToCityCode + (SkyWayList.Count == 1 ? "" : "/" + SkyWayList[1].FromCityCode + "-" + SkyWayList[1].ToCityCode);
                to.Space = SkyWayList[0].Space + (SkyWayList.Count == 1 ? "" : "/" + SkyWayList[1].Space); ;
                to.Discount = SkyWayList[0].Discount + (SkyWayList.Count == 1 ? "" : "/" + SkyWayList[1].Discount);
                
                //乘客人数
                to.PassengerNumber = PasList.Count;
                //乘客姓名 已"|"分割
                List<string> PasNameList = new List<string>();
                foreach (Tb_Ticket_Passenger item in PasList)
                {
                    PasNameList.Add(item.PassengerName);
                }
                to.PassengerName = string.Join("|", PasNameList.ToArray());
                
                
                //为儿童订单 且儿童不出成人票
                if (IsChild)
                {
                    if (!isCHDToAudltTK)
                    {
                        to.ABFee = 0m;
                        //to.FuelFee = (0.5m) * decimal.Parse(FQP.FuelAdultFee.ToString().Split('~')[0]);
                        to.Space = "Y";
                        to.Discount = "100";
                    }
                    to.IsChdFlag = true;//儿童
                    ///是否儿童出成人票
                    to.IsCHDETAdultTK = isCHDToAudltTK ? 1 : 0;
                }
                else
                {
                    to.ABFee =0;
                    to.FuelFee =0;
                    to.IsChdFlag = false;//成人  

                    
                }
                //客规
                to.KeGui = "";
            }
            catch (Exception ep)
            {
                DataBase.LogCommon.Log.Error("生成PNR页面GetOrder", ep);
            }
            return to;
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
                DataBase.LogCommon.Log.Error("PNR创建接口GetStrSkyWay", ex);
            }
            return reCity;
        }

        #endregion
    }


}