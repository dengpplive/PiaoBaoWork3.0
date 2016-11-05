using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using PnrAnalysis;
using PiaoBao.API.Common;
using PiaoBao.API.Common.AirQuery;
using System.Data;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using PbProject.Logic.Policy;
using PbProject.Logic.PID;
using PnrAnalysis.Model;

namespace PiaoBao.API.Services
{
    public class SpecialPriceServices : BaseServices
    {
        BaseDataManage Manage = new BaseDataManage();
        UserLoginInfo userInfo;
        public override void Query(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            userInfo = AuthLogin.GetUserInfo(Username);
            
            AirQueryCommon airQuery = new AirQueryCommon();

            
            string cacheNameGuid = parames["cacheNameGuid"];
            string fullFlightNo = parames["fullFlightNo"];//航班号如3U8881
            string cairrGuid = parames["cairrGuid"];//航班唯一编号
            string policyGuid = parames["policyGuid"];//政策唯一编号

            #region 取基础航班信息
            PbProject.WebCommon.Utility.Cache.CacheByNet pwucc = new PbProject.WebCommon.Utility.Cache.CacheByNet();
            DataSet dsCacheData = pwucc.GetCacheData(cacheNameGuid);
            if (dsCacheData == null) { writer.WriteEx(541,"cache timeout","缓存过期"); }
            var dr= dsCacheData.Tables[fullFlightNo].Select("guid='"+cairrGuid+"'").FirstOrDefault();
            if (dr == null) { writer.WriteEx(541, "not found filght", "没有找到航班信息"); }
            string CarrCode = (string)dr["CarrCode"];//承运人
            string FlightNo = (string)dr["FlightNo"];//航班号
            string StartCityCode = (string)dr["StartCityCode"];//起飞城市
            string ToCityCode = (string)dr["ToCityCode"];//抵达城市
            string StartTime = (string)dr["StartTime"];//起飞时间
            string EndTime = (string)dr["EndTime"];//抵达时间
            string StartDate = (string)dr["StartDate"];//起飞日期
            string Space = (string)dr["Space"];//舱位
            string ABFare = (string)dr["ABFee"];//机建
            string RQFare = (string)dr["FuelAdultFee"];//燃油
            string XSFee = (string)dr["XSFee"];//舱位价格
            
            #endregion
            #region 取政策
            string DownPoint, DownReturnMoney, SpacePrice;
            List<Tb_Ticket_Policy> objList = Manage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { "id='" + policyGuid + "'" }) as List<Tb_Ticket_Policy>;
            if (objList.Count() == 0)
            {
                List<Tb_Ticket_BookPolicy> bList = Manage.CallMethod("Tb_Ticket_BookPolicy", "GetList", null, new object[] { "PolicyId='" + policyGuid + "'" }) as List<Tb_Ticket_BookPolicy>;
                if (bList.Count() == 0)
                {
                    writer.WriteEx(564, "not found policy", "没有找到政策:" + policyGuid);
                    return;
                }
                else
                {
                    var pi=bList[0];
                    DownPoint = (pi.PReturn * 100).ToString();
                    DownReturnMoney = "0";
                    SpacePrice = "0";
                }
            }
            else
            {
                var policyInfo = objList[0];
                DownPoint = policyInfo.DownPoint.ToString();//政策
                DownReturnMoney = policyInfo.DownReturnMoney.ToString();//现返
                SpacePrice = policyInfo.SpacePrice.ToString();//固定特价
            }
            #endregion
            bool isGuding = false;
            
           
            if (airQuery.IsSpecialSpace(userInfo, Space, CarrCode))
            {
                SpecialPrice sp = PlyMatch(userInfo, StartCityCode, ToCityCode, StartTime, EndTime,
                    StartDate, Space, CarrCode, FlightNo, DownPoint, DownReturnMoney, cairrGuid, SpacePrice,
                    ABFare, RQFare, isGuding, XSFee);
                if (sp == null)
                {
                    writer.WriteEx(580, "get data error", "未查询到特价");
                }
                else
                {
                    writer.Write(sp);
                }
            }
            else
            {
                writer.WriteEx(570, "not a special space", "该舱位不是特价舱位");
            }



        }


        /// <summary>
        /// 返回数据
        /// </summary>
        /// <param name="fromcity">起飞城市三字码</param>
        /// <param name="tocity">抵达城市三字码</param>
        /// <param name="totime">起飞时间</param>
        /// <param name="arrtime">抵达时间</param>
        /// <param name="starttime">起飞日期</param>
        /// <param name="cw">舱位</param>
        /// <param name="aircode">航空公司</param>
        /// <param name="aircono">航班号</param>
        /// <param name="fd">返点</param>
        /// <param name="strYh">现返</param>
        /// <param name="hidId">GUID</param>
        /// <param name="Fare">固定特价</param>
        /// <param name="TAX">基建</param>
        /// <param name="RQFare">燃油</param>
        /// <param name="isGuding">是否固定特价</param>
        /// <param name="Yprice">是否固定特价</param>
        private SpecialPrice PlyMatch(UserLoginInfo userInfo, string fromcity, string tocity, string totime, string arrtime, string starttime,
            string cw, string aircode, string aircono, string fd, string strYh, string hidId,
            string Fare, string TAX, string RQFare, bool isGuding, string Yprice
            )
        {
            SpecialPrice sp = new SpecialPrice();
            try
            {
                
                string[] strValue = new string[4];
                //非固定特价PATA数据
                if (!isGuding)
                {
                    #region pat 数据
                    // ig|SS MU747/X/30NOV/KMGSHANN1/0730 1025|pat:a&kmg226#1 
                    //发送指令获取
                    DateTime dt = DateTime.Parse(starttime);
                    string cmd = "SS " + aircode + aircono + "/" + cw + "/" + dt.ToString("R").Substring(4, 7).Replace(" ", "") + dt.Year.ToString().Substring(2) + "/" + fromcity + tocity + "NN1/" + totime.Replace(":", "") + " " + arrtime.Replace(":", "") + "|pat:a";
                    string zhiling = cmd;
                    string strVale = string.Empty;
                    //---------修改部分----------    
                    if (userInfo.Configparam != null)
                    {
                        //开启使用特价缓存  true 开启 false关闭
                        bool IsUseSpCache = userInfo.FQP.KongZhiXiTong != null && userInfo.FQP.KongZhiXiTong.Contains("|99|");
                        //特价缓存
                        SpecialCabinPriceInfoBLL SpBll = new SpecialCabinPriceInfoBLL();
                        Tb_SpecialCabin_PriceInfo PriceInfo = null;
                        if (IsUseSpCache)
                        {
                            SpBll.GetSpPrice(aircode, aircono, DateTime.Parse(starttime + " " + totime + ":00"), fromcity, tocity, cw);
                        }
                        if (PriceInfo == null)
                        {
                            //格式化编码内容类
                            PnrAnalysis.FormatPNR pnrformat = new PnrAnalysis.FormatPNR();
                            //扩展参数
                            ParamEx pe = new ParamEx();
                            pe.UsePIDChannel = userInfo.FQP.KongZhiXiTong != null && userInfo.FQP.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                            SendInsManage SendManage = new SendInsManage(userInfo.User.LoginName, userInfo.Company.UninCode, pe, userInfo.Configparam);
                            string Office = userInfo.Configparam.Office.Split('^')[0];
                            strVale = SendManage.Send(cmd, ref Office, 15);//发送获取特价指令
                            string error = "";
                            decimal spFare = 0m, spABFare = 0m, spRQFare = 0m;
                            //价格实体
                            PnrAnalysis.PatModel Pat = pnrformat.GetPATInfo(strVale, out error);
                            if (Pat != null && Pat.UninuePatList.Count > 0)
                            {
                                strValue[0] = Pat.UninuePatList[0].Fare;//舱位价
                                strValue[1] = Pat.UninuePatList[0].TAX;//基建
                                strValue[2] = Pat.UninuePatList[0].RQFare;//燃油
                                strValue[3] = Pat.UninuePatList[0].Price;//总计(舱位价+基建+燃油)  
                                decimal.TryParse(strValue[0], out spFare);
                                decimal.TryParse(strValue[1], out spABFare);
                                decimal.TryParse(strValue[2], out spRQFare);
                            }
                            else
                            {
                                //-----------------2013-5-6添加----------------------------------------------
                                //没有PAT出价格 预订一个编码PAT价格后取消PNR 即使编码没有取消 后台程序自动取消
                                string ErrMsg = "";
                                if (ConfigIsSet(out ErrMsg))
                                {
                                    //----------构造航段和乘客---------------
                                    List<Tb_Ticket_Passenger> pList = GetPassengerList();
                                    List<Tb_Ticket_SkyWay> skywaylist = new List<Tb_Ticket_SkyWay>();
                                    Tb_Ticket_SkyWay sky = new Tb_Ticket_SkyWay();
                                    sky.CarryCode = aircode;
                                    sky.FlightCode = aircono;
                                    sky.FromDate = DateTime.Parse(starttime + " " + totime + ":00");
                                    sky.ToDate = DateTime.Parse(starttime + " " + arrtime + ":00");
                                    sky.FromCityCode = fromcity;
                                    sky.ToCityCode = tocity;
                                    sky.Space = cw;
                                    skywaylist.Add(sky);

                                    //-------------------------
                                    //预订编码 获取价格
                                    RePnrObj pnrObj = GetPnrInfo(aircode, pList, skywaylist, out ErrMsg);
                                    string AdultPnr = string.Empty;
                                    Log_Pnr logAdultPnr = null;
                                    //成人预订信息编码记录            
                                    if (pnrObj.AdultYudingList.Count > 0)
                                    {
                                        AdultPnr = pnrObj.AdultPnr;
                                        if (string.IsNullOrEmpty(AdultPnr) || AdultPnr.Trim().Length != 6)
                                        {
                                            AdultPnr = "";
                                        }
                                        //记录编码日志
                                        YuDingPnrLog(pnrObj, pnrObj.AdultYudingList.Keys[0], pnrObj.AdultYudingList.Values[0], AdultPnr, pnrObj.Office, out logAdultPnr);
                                    }
                                    if (!string.IsNullOrEmpty(pnrObj.AdultPnr))
                                    {
                                        //取消编码
                                        if (SendManage.CancelPnr(pnrObj.AdultPnr, pnrObj.Office))
                                        {
                                            if (logAdultPnr != null)
                                            {
                                                //修改状态
                                                logAdultPnr.Flag = true;
                                                string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Pnr>.CreateUpdateModelSql(logAdultPnr, "id");
                                                Manage.ExecuteNonQuerySQLInfo(tempSql);
                                            }
                                        }
                                    }
                                    //获取价格
                                    if (pnrObj.PatModelList != null && pnrObj.PatModelList.Length > 0)
                                    {
                                        Pat = pnrObj.PatModelList[0];
                                        if (Pat != null && Pat.UninuePatList.Count > 0)
                                        {
                                            strValue[0] = Pat.UninuePatList[0].Fare;//舱位价
                                            strValue[1] = Pat.UninuePatList[0].TAX;//基建
                                            strValue[2] = Pat.UninuePatList[0].RQFare;//燃油
                                            strValue[3] = Pat.UninuePatList[0].Price;//总计(舱位价+基建+燃油)
                                            decimal.TryParse(strValue[0], out spFare);
                                            decimal.TryParse(strValue[1], out spABFare);
                                            decimal.TryParse(strValue[2], out spRQFare);
                                        }
                                    }
                                }
                                //---------------------------------------------------------------
                            }
                            if (spFare != 0m)
                            {
                                //存入缓存
                                SpBll.SaveSpPrice(aircode.ToUpper(), aircono, DateTime.Parse(starttime + " " + totime + ":00"), fromcity, tocity, cw, spFare, spABFare, spRQFare);
                            }
                        }
                        else
                        {
                            strValue[0] = PriceInfo.SpPrice.ToString();//舱位价
                            strValue[1] = PriceInfo.SpABFare.ToString();//基建
                            strValue[2] = PriceInfo.SpRQFare.ToString();//燃油
                            strValue[3] = (PriceInfo.SpPrice + PriceInfo.SpABFare + PriceInfo.SpRQFare).ToString();
                        }
                    }
                    #endregion
                }
                else
                {
                    strValue[0] = Fare;//舱位价
                    strValue[1] = TAX;//基建
                    strValue[2] = RQFare;//燃油
                    strValue[3] = (decimal.Parse(Fare) + decimal.Parse(TAX) + decimal.Parse(RQFare)).ToString();
                }

                //实付金额
                string sjvalue = new PbProject.Logic.Pay.Data(userInfo.Company.UninCode).CreatePassengerPayFee(decimal.Parse(strValue[0]), decimal.Parse(strValue[1]), decimal.Parse(strValue[2]), decimal.Parse(fd), decimal.Parse(strYh), 1).ToString();
                string pyj = new PbProject.Logic.Pay.Data(userInfo.Company.UninCode).CreateCommissionCG(decimal.Parse(strValue[0]), decimal.Parse(fd)).ToString();

                string ZK = "";
                if (Yprice != "0")//如果传的有Y舱价格,则是特价类型的,则计算折扣
                {
                    PbProject.Logic.Pay.DataAction plpd = new PbProject.Logic.Pay.DataAction();
                    //特价时候需要计算折扣,让前台计算加入哪一个折扣范围
                    decimal tempFare = 0;
                    decimal.TryParse(strValue[0], out tempFare);
                    decimal tempYprice = 0;
                    decimal.TryParse(Yprice, out tempYprice);
                    //计算特价的折扣
                    ZK = (new PbProject.Logic.Pay.DataAction().FourToFiveNum((tempFare / tempYprice), 4) * 100).ToString("f2");
                    decimal tempZK = 0;
                    decimal.TryParse(ZK, out tempZK);
                    ZK = plpd.FourToFiveNum(tempZK, 0).ToString();
                }


                if (strValue[0] != "" || strValue[1] != "" || strValue[2] != "" || strValue[3] != "")
                {
                    decimal tempPrice = 0;
                    decimal.TryParse(strValue[0], out tempPrice);


                    #region  pat 有数据
                    sp.SpacePrice = tempPrice.ToString("f0");
                    sp.Tax = strValue[1];
                    sp.RQFare = strValue[2];
                    sp.TotalPrice = strValue[3];
                    sp.RealPayPrice = sjvalue;
                    sp.Commission = pyj;
                    sp.GUID = hidId;
                    sp.Discount = ZK;
                    #endregion
                }
                else
                {
                    #region pat 没有数据
                    
                    #endregion pat 出来没有数据
                }

                //最终sb格式 0舱位价@1基建@2燃油@3总计(舱位价+基建+燃油)@4实付金额@5佣金@6guid@7特价计算的折扣
                return sp;
                //Response.Write("");
            }
            catch (Exception ex)
            {
                return sp;
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
                PbProject.Model.ConfigParam config = userInfo.Configparam;
                string UserCpyNo = userInfo.User.CpyNo;
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
            }
            catch (Exception ex)
            {
                Msg = "预订编码配置信息异常,无法预订！";
                IsSet = false;
            }
            return IsSet;
        }
        /// <summary>
        /// 获取乘机人
        /// </summary>
        /// <returns></returns>
        public List<Tb_Ticket_Passenger> GetPassengerList()
        {
            string[] strPasName = new string[]{
            "刘梦雯",
            "黄翔",
            "李忠",
            "杨晓红",
            "幸坤建",
            "刘爱珠",
            "陈国利",
            "何晓波",
            "王学成",
            "郝连娟",
            "周廷洋",
            "李财玉",
            "刘雪琴",   
            "沈军",
            "朱万春",
            "姚贵玉",
            "李静",
            "翁琳",
            "吴俊",	
            "陈刚",	
            "李芳明"	                 
       };
            string[] strPasId = new string[]{
        "522321196906260415",
        "652827196911273119",
        "432426197011210371",
        "510211196911223422",
        "610402197003061299",
        "152221199002091849",
        "510525198503118432",
        "142327198010040213",
        "152224198212097581",
        "522423198112197319",
        "650202194410110716",
         "513624198010185727",
         "650300197207290332",
        "653201197604071014",
        "520102196102045822",
        "420105198009073647",
        "510226197305057187",
        "522423195403290026",
        "512923197410241294",
        "43022519760706153X",
        "620121196709260556"        
        };
            List<Tb_Ticket_Passenger> paslist = new List<Tb_Ticket_Passenger>();
            try
            {
                //随机取一位乘客
                Random rd = new Random((int)System.DateTime.Now.Ticks);
                int i = rd.Next(0, strPasName.Length);
                Tb_Ticket_Passenger Passenger = new Tb_Ticket_Passenger();
                Passenger.PassengerName = strPasName[i];
                Passenger.PassengerType = 1;
                Passenger.Cid = strPasId[i];
                paslist.Add(Passenger);
            }
            catch (Exception)
            {
                Tb_Ticket_Passenger Passenger = new Tb_Ticket_Passenger();
                Passenger.PassengerName = "张三";
                Passenger.PassengerType = 1;
                Passenger.Cid = "2525145554";
                paslist.Add(Passenger);
            }
            return paslist;
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
        public RePnrObj GetPnrInfo(string CarryCode, List<Tb_Ticket_Passenger> pList, List<Tb_Ticket_SkyWay> skywaylist, out string ErrMsg)
        {
            ErrMsg = "";
            List<IPassenger> pasList = new List<IPassenger>();
            List<ISkyLeg> skyList = new List<ISkyLeg>();
            SendNewPID pid = new SendNewPID();
            PnrParamObj PnrParam = new PnrParamObj();
            //必填项 是否开启新版PID发送指令 
            PnrParam.UsePIDChannel = userInfo.FQP.KongZhiXiTong != null && userInfo.FQP.KongZhiXiTong.Contains("|48|") ? 2 : 0;  //2;
            PnrParam.ServerIP = userInfo.Configparam.WhiteScreenIP;
            PnrParam.ServerPort = int.Parse(string.IsNullOrEmpty(userInfo.Configparam.WhiteScreenPort) ? "0" : userInfo.Configparam.WhiteScreenPort);
            PnrParam.Office = userInfo.Configparam.Office.Split('^')[0];
            PnrParam.CarryCode = CarryCode;
            PnrParam.PasList = pasList;
            PnrParam.SkyList = skyList;
            PnrParam.IsGetSpecialPrice = 1;//为获取特价       
            //是否儿童出成人票
            PnrParam.ChildIsAdultEtdz = "0";
            //可选项
            PnrParam.UserName = userInfo.User != null ? userInfo.User.LoginName : "";

            //输入的手机号码 预订编码CT项电话
            PnrParam.CTTel = "028-55555555";
            PnrParam.CTCTPhone = "15928636274";


            PnrParam.PID = userInfo.Configparam.Pid;
            PnrParam.KeyNo = userInfo.Configparam.KeyNo;
            //乘机人
            foreach (Tb_Ticket_Passenger pas in pList)
            {
                IPassenger p1 = new IPassenger();
                pasList.Add(p1);
                p1.PassengerName = pas.PassengerName;
                p1.PassengerType = pas.PassengerType;
                p1.PasSsrCardID = pas.Cid;
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
            //记录指令
            SaveInsInfo(pObj, userInfo.User, userInfo.Company);
            return pObj;
        }
        /// <summary>
        /// 预订编码日志
        /// </summary>
        /// <param name="SSContent"></param>
        /// <param name="ResultContent"></param>
        /// <param name="Pnr"></param>
        /// <param name="pnrLogList"></param>
        /// <returns></returns>
        public bool YuDingPnrLog(RePnrObj PnrInfo, string SSContent, string ResultContent, string Pnr, string Office, out Log_Pnr logpnr)
        {
            bool Insert = false;
            logpnr = new Log_Pnr();
            try
            {
                logpnr.CpyNo = userInfo.Company.UninCode;
                logpnr.CpyName = userInfo.Company.UninCode;
                logpnr.CpyType = userInfo.Company.RoleType;
                logpnr.OperTime = System.DateTime.Now;
                logpnr.OperLoginName = userInfo.User.LoginName;
                logpnr.OperUserName = userInfo.User.UserName;
                logpnr.SSContent = SSContent;
                logpnr.ResultContent = ResultContent;
                logpnr.PNR = Pnr;
                logpnr.OfficeCode = Office;
                logpnr.A7 = "[特价编码]" + PnrInfo.ServerIP + "|" + PnrInfo.ServerPort;//IP和端口
                Insert = (bool)Manage.CallMethod("Log_Pnr", "Insert", null, new object[] { logpnr });
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:YuDingPnrLog 编码:" + Pnr + "】================================================================\r\n 异常信息:" + ex.Message + "\r\n", "SpPatAjax");
            }
            return Insert;
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
                    string UserAccount = userInfo.User.LoginName, CpyNo = userInfo.Company.UninCode, serverIPPort = PnrInfo.ServerIP + ":" + PnrInfo.ServerPort, Office = PnrInfo.Office;
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
                            ins.SendInsType = 12;//特价指令
                            sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(ins, Removelist));
                        }
                    }
                    if (sqlList.Count > 0)
                    {
                        IsSuc = Manage.ExecuteSqlTran(sqlList, out errMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuc = false;
                errMsg = ex.Message + ex.StackTrace.ToString();
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:SaveInsInfo】================================================================\r\n 异常信息:" + errMsg + "\r\n", "SpPatAjax");
            }
            return IsSuc;
        }


    }
}
