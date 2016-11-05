using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using PbProject.Logic.Order;
using PnrAnalysis;
using PnrAnalysis.Model;
using PbProject.WebCommon.Web.Cache;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.Logic.Pay;
using PbProject.Logic.PID;
using System.Text.RegularExpressions;
using System.Diagnostics;
namespace PbProject.Logic.Buy
{
    /// <summary>
    /// PNR导入管理
    /// </summary>
    public class PnrImportManage
    {
        /// <summary>
        /// 数据库管理
        /// </summary>
        BaseDataManage baseManage = new BaseDataManage();
        /// <summary>
        /// 订单管理
        /// </summary>
        Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
        /// <summary>
        /// 缓存管理
        /// </summary>
        CacheManage m_cache = new CacheManage();
        /// <summary>
        /// 航班查询
        /// </summary>
        AirQurey airqurey = new AirQurey();
        /// <summary>
        /// Pnr内容格式化
        /// </summary>
        FormatPNR m_pnrformat = new FormatPNR();

        #region 局部变量
        SendInsManage SendIns = null;
        //控台权限 获取控制系统权限 
        string KongZhiXiTong = string.Empty;
        //供应控制分销开关 
        string GongYingKongZhiFenXiao = string.Empty;
        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        User_Employees m_User = null;
        /// <summary>
        /// 当前登录用户公司信息
        /// </summary>
        User_Company m_Company = null;
        /// <summary>
        /// 当前登录用户所属供应商公司信息
        /// </summary>
        User_Company m_SupCompany = null;

        private static List<Bd_Air_AirPort> CityList = null;
        private static List<Bd_Air_Carrier> AirList = null;
        private static List<Tb_Ticket_PrintOffice> PrintOfficeList = null;
        #endregion

        public PnrImportManage(User_Employees _CurrLoginUser, User_Company _CurrLoginCompany, User_Company _SupCompany, string _KongZhiXiTong, string _GongYingKongZhiFenXiao, ConfigParam _configParam)
        {
            KongZhiXiTong = string.IsNullOrEmpty(_KongZhiXiTong) ? "" : _KongZhiXiTong;
            GongYingKongZhiFenXiao = string.IsNullOrEmpty(_GongYingKongZhiFenXiao) ? "" : _GongYingKongZhiFenXiao;
            m_User = _CurrLoginUser;
            m_Company = _CurrLoginCompany;
            m_SupCompany = _SupCompany;
            //扩展参数
            ParamEx pe = new ParamEx();
            pe.UsePIDChannel = KongZhiXiTong != null && KongZhiXiTong.Contains("|48|") ? 2 : 0;
            SendIns = new SendInsManage(_CurrLoginUser.LoginName, _CurrLoginCompany.UninCode, pe, _configParam);
        }

        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ErrTip(string pnr, string data)
        {
            if (data.Contains("授权"))
            {
                string[] strArr = data.Split('#');
                if (strArr.Length == 2)
                {
                    string[] strOffice = strArr[1].Split('|');
                    List<string> listOffice = new List<string>();
                    foreach (string item in strOffice)
                    {
                        listOffice.Add(string.Format("RMK TJ AUTH {0} <br/>", item));
                    }
                    data = "该" + pnr.Trim() + "需要授权，授权指令如下:<br/>" + string.Join("", listOffice.ToArray());
                }
            }
            return data;
        }

        /// <summary>
        /// 查找城市
        /// </summary>
        /// <param name="CityAndCode">城市名或者城市三字码</param>
        /// <param name="CityList"></param>
        /// <returns></returns>
        public Bd_Air_AirPort GetCityInfo(string CityAndCode, List<Bd_Air_AirPort> CityList)
        {
            Bd_Air_AirPort bd_air_airport = CityList.Find(delegate(Bd_Air_AirPort _city)
            {
                return (_city.CityCodeWord.ToUpper() == CityAndCode.ToUpper() || _city.CityName.Trim() == CityAndCode.Trim());
            });
            return bd_air_airport;
        }
        /// <summary>
        /// 获取航空公司信息
        /// </summary>
        /// <param name="CityAndCode">航空公司名称或者二字码</param>
        /// <param name="CityList"></param>
        /// <returns></returns>
        public Bd_Air_Carrier GetAirInfo(string NameAndCode, List<Bd_Air_Carrier> CityList)
        {
            Bd_Air_Carrier bd_air_carrier = CityList.Find(delegate(Bd_Air_Carrier _air)
            {
                return (_air.AirName.ToUpper() == NameAndCode.ToUpper() || _air.Code.Trim() == NameAndCode.Trim() || _air.ShortName == NameAndCode.Trim());
            });
            return bd_air_carrier;
        }
        /// <summary>
        /// 获取出票航空公司信息
        /// </summary>
        /// <param name="CityAndCode">航空公司二字码</param>
        /// <param name="CityList"></param>
        /// <returns></returns>
        public Tb_Ticket_PrintOffice GetAirTicketInfo(string NameAndCode, List<Tb_Ticket_PrintOffice> CityList)
        {
            Tb_Ticket_PrintOffice tb_ticket_printoffice = CityList.Find(delegate(Tb_Ticket_PrintOffice _air)
            {
                return (_air.AirCode.ToUpper() == NameAndCode.ToUpper());
            });
            return tb_ticket_printoffice;
        }
        public List<Bd_Air_Carrier> GetAirList()
        {
            return this.baseManage.CallMethod("Bd_Air_Carrier", "GetList", null, new object[] { "" }) as List<Bd_Air_Carrier>;
        }
        public List<Bd_Air_AirPort> GetCityList()
        {
            return this.baseManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=1" }) as List<Bd_Air_AirPort>;
        }
        public List<Tb_Ticket_PrintOffice> GetPrintOfficeList()
        {
            return this.baseManage.CallMethod("Tb_Ticket_PrintOffice", "GetList", null, new object[] { "" }) as List<Tb_Ticket_PrintOffice>;
        }
        public List<Bd_Air_Fares> GetYFaresList(string sqlWhere)
        {
            return this.baseManage.CallMethod("Bd_Air_Fares", "GetList", null, new object[] { sqlWhere }) as List<Bd_Air_Fares>;
        }
        /// <summary>
        /// 多航段中航空公司二字码是否相同
        /// </summary>
        /// <param name="SkyWay"></param>
        /// <returns></returns>
        public bool ValCarrier(List<Tb_Ticket_SkyWay> SkyWay)
        {
            bool IsSame = false;
            if (SkyWay.Count > 0)
            {
                string Carray = SkyWay[0].CarryCode.ToUpper();
                foreach (Tb_Ticket_SkyWay item in SkyWay)
                {
                    if (item.CarryCode.ToUpper() == Carray)
                    {
                        IsSame = true;
                    }
                    else
                    {
                        IsSame = false;
                        break;
                    }
                }
            }
            else
            {
                IsSame = true;
            }
            return IsSame;
        }

        /// <summary>
        /// 验证儿童与成人航线是否一样
        /// </summary>
        /// <param name="AdtOrderId"></param>
        /// <param name="CHDSkyWay"></param>
        /// <returns></returns>
        public bool ValSkyWay(string AdtOrderId, List<Tb_Ticket_SkyWay> CHDSkyWay)
        {
            bool IsSuc = false;
            try
            {
                string sqlWhere = string.Format(" OrderId ='{0}'", AdtOrderId.Replace("\'", ""));
                List<Tb_Ticket_SkyWay> AdultSkyWay = baseManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_SkyWay>;
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
            catch (Exception)
            {
                IsSuc = false;
            }
            return IsSuc;
        }
        /// <summary>
        /// 比较第一个航段数据与第二个航段数据是否相同
        /// </summary>
        /// <param name="SourceSkyWay"></param>
        /// <param name="DescSkyWay"></param>
        /// <returns></returns>
        public bool CompareSkyWay(List<Tb_Ticket_SkyWay> SourceSkyWay, List<Tb_Ticket_SkyWay> DescSkyWay, out string ErrMsg)
        {
            bool IsSame = true;
            ErrMsg = "";
            for (int i = 0; i < SourceSkyWay.Count; i++)
            {
                if (SourceSkyWay[i].FromCityCode != DescSkyWay[i].FromCityCode || SourceSkyWay[i].ToCityCode != DescSkyWay[i].ToCityCode)
                {
                    IsSame = false;
                    ErrMsg = "新编码与原订单中航程信息数据不对一致!";
                    break;
                }
                if (SourceSkyWay[i].CarryCode != DescSkyWay[i].CarryCode)
                {
                    IsSame = false;
                    ErrMsg = "新编码与原订单不是同一个航空公司!";
                    break;
                }
                if (!string.IsNullOrEmpty(SourceSkyWay[i].Discount) && !string.IsNullOrEmpty(DescSkyWay[i].Discount) && decimal.Parse(SourceSkyWay[i].Discount) < decimal.Parse(DescSkyWay[i].Discount))
                {
                    IsSame = false;
                    ErrMsg = "升舱通道新编码舱位必须高于原订单舱位,导入失败!";
                    break;
                }
                else
                {
                    if (SourceSkyWay[i].SpacePrice < DescSkyWay[i].SpacePrice)
                    {
                        IsSame = false;
                        ErrMsg = "升舱通道新编码舱位必须高于原订单舱位,导入失败!";
                        break;
                    }
                }
            }
            return IsSame;
        }

        /// <summary>
        /// Pnr导入
        /// </summary>
        /// <param name="ImportParam"></param>
        /// <returns></returns>
        public bool GetImportPnrInfo(PnrImportParam ImportParam)
        {
            //记录时间
            StringBuilder sbTime = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool IsSuc = false;
            string ErrMsg = "";
            //获取编码信息
            PnrModel pnrModel = null;
            PatModel patModel = null;
            PatModel patInfModel = null;
            OrderMustParamModel ParamModel = new OrderMustParamModel();
            try
            {
                #region 初始化实体
                ImportParam.OrderParam.OrderParamModel.Add(ParamModel);
                Tb_Ticket_Order Order = new Tb_Ticket_Order();
                DataAction d = new DataAction();
                Data data = new Data(ImportParam.m_LoginCompany.UninCode);
                Log_Tb_AirOrder logOrder = new Log_Tb_AirOrder();
                List<Tb_Ticket_Passenger> PasList = new List<Tb_Ticket_Passenger>();
                List<Tb_Ticket_SkyWay> SkyList = new List<Tb_Ticket_SkyWay>();
                ParamModel.Order = Order;
                ParamModel.PasList = PasList;
                ParamModel.SkyList = SkyList;
                ParamModel.LogOrder = logOrder;
                ImportParam.OrderParam.PnrInfo = new RePnrObj();
                //升舱换开订单信息               
                OrderMustParamModel OleOm = null;
                if (CityList == null)
                {
                    CityList = m_cache.GetCacheData("baseCity", "") as List<Bd_Air_AirPort>;
                    if (CityList == null)
                    {
                        CityList = GetCityList();
                        m_cache.SetCacheData("baseCity", CityList, -1, "");
                    }
                }
                if (AirList == null)
                {
                    AirList = m_cache.GetCacheData("baseAir", "") as List<Bd_Air_Carrier>;
                    if (AirList == null)
                    {
                        AirList = GetAirList();
                        m_cache.SetCacheData("baseAir", AirList, -1, "");
                    }
                }
                if (PrintOfficeList == null)
                {
                    PrintOfficeList = m_cache.GetCacheData("PrintOffice", "") as List<Tb_Ticket_PrintOffice>;
                    if (PrintOfficeList == null)
                    {
                        PrintOfficeList = GetPrintOfficeList();
                        m_cache.SetCacheData("PrintOffice", PrintOfficeList, -1, "");
                    }
                }
                ImportParam.CityList = CityList;
                ImportParam.AirList = AirList;
                ImportParam.GYAirTicketlist = PrintOfficeList;
                #endregion
                sw.Stop();
                sbTime.Append("时间1:" + sw.Elapsed.ToString() + "\r\n");
                sw.Restart();
                string Office = "";
                //rt数据
                string RtData = string.Empty;
                //pat数据
                string PatData = string.Empty;
                if (ImportParam.ImportTongDao == 3)
                {
                    #region //大编码导入
                    if (string.IsNullOrEmpty(ImportParam.BigPnr) || ImportParam.BigPnr == "ooooo" || ImportParam.BigPnr == "oooooo" || ImportParam.BigPnr.Trim().Length != 6)
                    {
                        ErrMsg = "输入大编码格式有误！";
                    }
                    else
                    {
                        pnrModel = SendIns.GetBigPnr(ImportParam.BigPnr, out ErrMsg);
                        if (pnrModel == null)
                        {
                            ErrMsg = "未能解析出编码信息，原因如下:<br />" + ErrMsg;
                        }
                        else
                        {
                            ImportParam.Pnr = pnrModel.Pnr;
                            ImportParam.Office = pnrModel._CurrUseOffice;
                        }
                    }
                    #endregion
                }
                else if (ImportParam.ImportTongDao == 4)
                {
                    #region    Pnr内容导入
                    try
                    {
                        //记录PNR内容导入数据
                        Tb_SendInsData PnrContentLog = new Tb_SendInsData();
                        PnrContentLog.SendIns = "";
                        PnrContentLog.RecvData = "";
                        PnrContentLog.SendInsType = 16;//记录PNR内容导入
                        PnrContentLog.SendTime = System.DateTime.Now;
                        PnrContentLog.RecvTime = System.DateTime.Now; ;
                        PnrContentLog.Office = SendIns.m_ConfigParam.Office.Split('^')[0];
                        PnrContentLog.ServerIPAndPort = SendIns.m_ConfigParam.WhiteScreenIP + ":" + SendIns.m_ConfigParam.WhiteScreenPort;
                        PnrContentLog.UserAccount = m_User.LoginName;
                        PnrContentLog.CpyNo = m_User.CpyNo;
                        if (ImportParam.IsMerge == 1)
                        {
                            PnrContentLog.A1 = ImportParam.RTAndPatData;
                        }
                        else
                        {
                            PnrContentLog.A1 = ImportParam.RTData;
                            PnrContentLog.A2 = ImportParam.PATData;
                        }
                        //记录指令
                        SendIns.LogIns(PnrContentLog);
                    }
                    catch
                    {
                    }
                    if (ImportParam.IsMerge == 1)
                    {
                        //处理合并的RT和PAT数据
                        ImportParam.RTAndPatData.ToUpper().LastIndexOf("PAT:A");
                        int rtIndex = ImportParam.RTAndPatData.ToUpper().IndexOf("PAT:A");
                        //获取PAT数据
                        int PatIndex = ImportParam.RTAndPatData.ToUpper().LastIndexOf("PAT:A");
                        if (PatIndex != -1 && rtIndex != -1)
                        {
                            PatData = ">" + ImportParam.RTAndPatData.ToUpper().Substring(PatIndex);
                            RtData = ImportParam.RTAndPatData.ToUpper().Substring(0, rtIndex).Replace(">", "");
                            //处理RT数据
                            RtData = m_pnrformat.PnrHandle(RtData, out ErrMsg);
                        }
                        else
                        {
                            //解析出编码
                            string pnr = m_pnrformat.GetPNR(ImportParam.RTAndPatData.ToUpper(), out ErrMsg);
                            if (string.IsNullOrEmpty(pnr))
                            {
                                ErrMsg = "该Pnr内容格式错误,未能解析出编码！";
                                return IsSuc;
                            }
                            else
                            {
                                ImportParam.Pnr = pnr;
                                pnrModel = m_pnrformat.GetPNRInfo(pnr, ImportParam.RTAndPatData.ToUpper(), false, out ErrMsg);
                                if (pnrModel != null)
                                {
                                    if (pnrModel._PatList.Count == 0)
                                    {
                                        ErrMsg = "PNR内容中未能找到价格和PAT数据,请补全后再导入！";
                                        return IsSuc;
                                    }
                                    else
                                    {
                                        //pnr内容中取Office
                                        if (pnrModel != null && pnrModel._Office != "")
                                        {
                                            //内容导入的使用落地运营商中的第一个Office
                                            string mOffice = SendIns.m_ConfigParam != null ? SendIns.m_ConfigParam.Office.Split('^')[0] : pnrModel._Office;
                                            pnrModel._CurrUseOffice = mOffice;
                                            ImportParam.Office = mOffice;
                                        }
                                        RtData = m_pnrformat.PnrHandle(ImportParam.RTAndPatData, out ErrMsg);
                                        PatInfo pat = pnrModel._PatList[0];
                                        PatData = ">PAT:A                                                                          \r\n>PAT:A                                                                          \r\n01 " + pat.SeatGroup + " FARE:CNY" + pat.Fare + " TAX:CNY" + pat.TAX + " YQ:CNY" + pat.RQFare + "  TOTAL:" + pat.Price + "                   \r\nSFC:01";
                                        patModel = m_pnrformat.GetPATInfo(PatData, out ErrMsg);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //处理RT数据
                        RtData = m_pnrformat.PnrHandle(ImportParam.RTData, out ErrMsg);
                        PatData = ImportParam.PATData;
                    }
                    if (pnrModel == null || patModel == null)
                    {
                        //解析出编码
                        string pnr = m_pnrformat.GetPNR(RtData.ToUpper(), out ErrMsg);
                        if (string.IsNullOrEmpty(pnr))
                        {
                            ErrMsg = "该Pnr内容格式错误,未能解析出编码!！";
                            return IsSuc;
                        }
                        else
                        {
                            ImportParam.Pnr = pnr;
                            pnrModel = m_pnrformat.GetPNRInfo(pnr, RtData, false, out ErrMsg);
                            if (pnrModel != null)
                            {
                                //pnr内容中取Office
                                if (pnrModel != null && pnrModel._Office != "")
                                {
                                    //内容导入的使用落地运营商中的第一个Office
                                    string mOffice = SendIns.m_ConfigParam != null ? SendIns.m_ConfigParam.Office.Split('^')[0] : pnrModel._Office;
                                    pnrModel._CurrUseOffice = mOffice;
                                    ImportParam.Office = mOffice;
                                }
                                //检查PNR内容是否完整
                                if (!pnrModel.PnrConIsOver && !KongZhiXiTong.Contains("|67|"))
                                {
                                    ErrMsg = "PNR内容信息不完整，编码内容需以Office号结尾,请检查编码内容！";
                                    //PNR内容导入且RT和PAT内容合并
                                    if (ImportParam.ImportTongDao == 4 && ImportParam.IsMerge == 1)
                                    {
                                        ErrMsg = "RT编码内容信息不完整，RT编码内容需以Office号结尾,请检查RT编码内容！";
                                    }
                                }
                            }
                            if (ErrMsg == "")
                            {
                                patModel = m_pnrformat.GetPATInfo(PatData, out ErrMsg);
                                if (patModel != null)
                                {
                                    if (patModel.IsOverMaxPrice)
                                    {
                                        ErrMsg = "PAT数据金额超出范围！";
                                    }
                                }
                            }
                        }
                    }
                    ImportParam.SecondPM.PnrContent = RtData;
                    ImportParam.SecondPM.PATContent = PatData;
                    #endregion
                }
                else
                {
                    #region//升舱换开
                    if (ImportParam.ImportTongDao == 5)
                    {
                        OrderInputParam OleOrderParam = new OrderInputParam();
                        OleOrderParam = OrderManage.GetOrder(ImportParam.OrderId, OleOrderParam, out ErrMsg);
                        if (OleOrderParam.OrderParamModel[0].Order == null)
                        {
                            ErrMsg = "原订单号不存在！";
                            return IsSuc;
                        }
                        if (OleOrderParam.OrderParamModel[0].Order.PNR.ToUpper() == ImportParam.Pnr.ToUpper())
                        {
                            ErrMsg = "新编码与原订单中编码相同不能导入!";
                            return IsSuc;
                        }
                        OleOm = OleOrderParam.OrderParamModel[0];
                    }
                    #endregion

                    #region 小编码导入
                    if (ImportParam.IsSecond != 0)
                    {
                        //内容
                        if (ImportParam.IsMerge == 1 && ImportParam.RTAndPatData.Trim() != "")
                        {
                            //两个内容
                            ImportParam.RTAndPatData.ToUpper().LastIndexOf("PAT:A");
                            int rtIndex = ImportParam.RTAndPatData.ToUpper().IndexOf("PAT:A");
                            //获取PAT数据
                            int PatIndex = ImportParam.RTAndPatData.ToUpper().LastIndexOf("PAT:A");
                            if (PatIndex != -1 && rtIndex != -1)
                            {
                                PatData = ">" + ImportParam.RTAndPatData.ToUpper().Substring(PatIndex);
                                RtData = ImportParam.RTAndPatData.ToUpper().Substring(0, rtIndex);
                            }
                            pnrModel = m_pnrformat.GetPNRInfo(ImportParam.Pnr, RtData, false, out ErrMsg);
                            patModel = m_pnrformat.GetPATInfo(PatData, out ErrMsg);
                        }
                        else
                        {
                            if (ImportParam.RTData.Trim() != "")
                            {
                                pnrModel = m_pnrformat.GetPNRInfo(ImportParam.Pnr, ImportParam.RTData.Trim(), false, out ErrMsg);
                                if (pnrModel != null && ImportParam.Office != "")
                                {
                                    pnrModel._CurrUseOffice = ImportParam.Office;
                                }
                            }
                            if (ImportParam.PATData.Trim() != "")
                            {
                                patModel = m_pnrformat.GetPATInfo(PatData, out ErrMsg);
                            }
                        }
                    }
                    if (pnrModel == null)
                    {
                        //小编码导入
                        if (string.IsNullOrEmpty(ImportParam.Office))
                        {
                            pnrModel = SendIns.GetPnr(ImportParam.Pnr, out ErrMsg);
                        }
                        else
                        {
                            pnrModel = SendIns.GetPnr(ImportParam.Pnr, ImportParam.Office, out ErrMsg);
                        }
                    }
                    #endregion
                }

                sw.Stop();
                sbTime.Append("时间2:" + sw.Elapsed.ToString() + "\r\n");
                sw.Restart();

                //---------------------------------------------
                if (pnrModel == null)
                {
                    ErrMsg = ErrTip(ImportParam.Pnr, ErrMsg);
                }
                else
                {
                    if (ErrMsg == "")
                    {
                        Office = pnrModel._CurrUseOffice;
                        //编码
                        string pnr = ImportParam.Pnr;
                        //编码状态
                        string PnrStatus = pnrModel.PnrStatus;
                        if (pnrModel._PassengerList.Count == 0)
                        {
                            ErrMsg = pnr + "未能获取到乘机人信息，导入失败！";
                        }
                        else if (pnrModel._LegList.Count == 0)
                        {
                            ErrMsg = pnr + "未能获取到航段信息,导入失败！";
                        }
                        else
                        {
                            //升舱换开
                            if (ImportParam.ImportTongDao == 5)
                            {
                                if (OleOm != null)
                                {
                                    if (OleOm.PasList.Count != pnrModel._PassengerList.Count)
                                    {
                                        ErrMsg = "新编码(" + pnr + ")与原订单中乘客人数不一致,请仔细检查！";
                                    }
                                    else if (OleOm.SkyList.Count != pnrModel._LegList.Count)
                                    {
                                        ErrMsg = "新编码(" + pnr + ")与原订单中航程信息数据不一致,请仔细检查！";
                                    }
                                }
                            }
                            if (PnrStatus.Trim() == "")
                            {
                                ErrMsg = "该" + pnr + "未能解析出编码状态！";
                                //PNR内容航段信息解析失败！
                            }
                            else
                            {
                                bool JDIsPass = true;
                                //JD导入 验证
                                if (pnrModel._CarryCode.ToUpper().Contains("JD"))
                                {
                                    //编码中成人 儿童 婴儿 个数
                                    int adultNum = 0, childNum = 0, YNum = 0;
                                    foreach (PassengerInfo item in pnrModel._PassengerList)
                                    {
                                        if (item.PassengerType == "1")
                                            adultNum++;
                                        if (item.PassengerType == "2")
                                            childNum++;
                                        if (item.PassengerType == "3")
                                            YNum++;
                                    }
                                    bool b1 = adultNum == 1 && childNum == 0 && YNum == 0;
                                    bool b2 = adultNum == 0 && childNum == 1 && YNum == 0;
                                    bool b3 = adultNum == 1 && childNum == 1 && YNum == 0;
                                    bool b4 = adultNum == 1 && childNum == 1 && YNum == 1;
                                    bool b5 = adultNum == 1 && childNum == 0 && YNum == 1;
                                    if (!(b1 || b2 || b3 || b4 || b5))
                                    {
                                        JDIsPass = false;
                                    }
                                }
                                if (PnrStatus.Contains("XX"))
                                {
                                    ErrMsg = "该" + pnr + "已经取消,无法导入！";
                                }
                                else if (PnrStatus.ToUpper().Contains("RR") && !KongZhiXiTong.Contains("|49|"))
                                {
                                    ErrMsg = "PNR状态为RR无法导入！";
                                }
                                else if (PnrStatus.ToUpper().Contains("NO"))
                                {
                                    ErrMsg = pnr + "状态为NO无法导入！";
                                }
                                else if (PnrStatus.ToUpper().Contains("HL"))
                                {
                                    ErrMsg = pnr + "状态为HL无法导入！";
                                }
                                else if (PnrStatus.ToUpper().Contains("HN"))
                                {
                                    ErrMsg = pnr + "状态为HN无法导入！";
                                }
                                else if (!JDIsPass)
                                {
                                    ErrMsg = "JD航空编码【" + pnr + "】中只能有一个成人,一个儿童,一个婴儿，请手动处理!";
                                }
                                else
                                {
                                    if (pnrModel._PasType == "2" && ImportParam.ImportTongDao != 5)
                                    {
                                        //该公司是否允许出儿童票 暂时没有添加
                                        if (KongZhiXiTong.Contains("|90|"))
                                        {
                                            ErrMsg = "该供应商不出儿童票！";
                                            return IsSuc;
                                        }
                                        if (ImportParam.ImportTongDao == 4 || ImportParam.ImportTongDao == 0)
                                        {
                                            //如果分销或者采购商在普通编码栏导入的是儿童编码提示
                                            if (ImportParam.ImportTongDao == 0 && (ImportParam.m_LoginCompany.RoleType == 4 || ImportParam.m_LoginCompany.RoleType == 5))
                                            {
                                                ErrMsg = "该编码【" + pnr + "】为儿童编码,请选择儿童编码类型！";
                                                return IsSuc;
                                            }
                                            //开启儿童编码必须关联成人编码或者成人订单号
                                            if (KongZhiXiTong != null && KongZhiXiTong.Contains("|95|"))
                                            {
                                                if (string.IsNullOrEmpty(ImportParam.OrderId.Trim()))
                                                {
                                                    //Pnr内容导入
                                                    ImportParam.SecondPM.Office = Office;
                                                    ImportParam.SecondPM.OpType = "1";//继续
                                                    ImportParam.SecondPM.Msg = pnr + "为儿童编码,请输入关联成人订单号或者成人编码！<br /><p><b>关联成人订单号/编码:</b><input type='text' maxlength='20' id='adultOrderId' value='' /><span id=\"msgOrderId\"></span></p>";
                                                    ImportParam.SecondPM.DataFlag = "3";
                                                    ImportParam.IsNextOK = 1;//是否继续
                                                    return IsSuc;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (ImportParam.ImportTongDao != 2)
                                            {
                                                ErrMsg = "该编码(" + pnr.Trim() + ")为儿童编码,请选择儿童编码类型！";
                                                return IsSuc;
                                            }
                                        }
                                    }
                                    //开启PNR中含有真实票号时不能导入
                                    if (pnrModel._TicketNumList.Count > 0 && KongZhiXiTong.Contains("|62|"))
                                    {
                                        ErrMsg = "该编码" + pnr + "已存在票号,无法导入！";
                                        return IsSuc;
                                    }
                                    //乘机人名字检查
                                    if (!pnrModel.PassengerNameIsCorrent && pnrModel.ErrorPassengerNameList.Count > 0)
                                    {
                                        ErrMsg = pnr + "中乘机人名字（" + string.Join(",", pnrModel.ErrorPassengerNameList.ToArray()) + "） 有误,无法导入！";
                                        return IsSuc;
                                    }
                                    //开启缺口程编码导入功能
                                    if (pnrModel.HasGapProcess && !KongZhiXiTong.Contains("|61|"))
                                    {
                                        ErrMsg = "该编码" + pnr + "含有缺口程，暂不支持缺口程编码导入！";
                                        return IsSuc;
                                    }
                                    //PNR内容导入 乘机人中证件号是否重复
                                    if (ImportParam.ImportTongDao == 4 && pnrModel.SsrIsRepeat)
                                    {
                                        ErrMsg = "编码内容中乘客证件号重复，请检查PNR内容证件号！";
                                        return IsSuc;
                                    }
                                    //国航
                                    if (pnrModel._CarryCode == "CA" && string.IsNullOrEmpty(pnrModel._Other.CTCT))
                                    {
                                        if (KongZhiXiTong.Contains("|70|"))
                                        {
                                            ErrMsg = "国航要求必须备注联系项,请备注OSI CA CTCT{手机}后,再导入！";
                                            return IsSuc;
                                        }
                                    }
                                    if (pnrModel._LegList.Count > 2)
                                    {
                                        ErrMsg = "暂不支持多航段导入！";
                                        return IsSuc;
                                    }

                                    sw.Stop();
                                    sbTime.Append("时间3:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                    #region //航段实体信息
                                    List<string> orderCarryCode = new List<string>();
                                    List<string> orderFlightCode = new List<string>();
                                    List<string> orderAirTime = new List<string>();
                                    List<string> orderTravel = new List<string>();
                                    List<string> orderTravelCode = new List<string>();
                                    List<string> orderSpace = new List<string>();
                                    List<string> orderDiscount = new List<string>();
                                    string AirPrintOffice = "";
                                    string CarrayCode = "", Space = "";
                                    foreach (LegInfo Leg in pnrModel._LegList)
                                    {
                                        CarrayCode = Leg.AirCode;
                                        if (!string.IsNullOrEmpty(Leg.ChildSeat))
                                        {
                                            Space = Leg.ChildSeat;
                                        }
                                        else
                                        {
                                            Space = Leg.Seat;
                                        }
                                        Tb_Ticket_SkyWay skyModel = new Tb_Ticket_SkyWay();
                                        skyModel.CarryCode = Leg.AirCode.ToUpper();
                                        skyModel.FlightCode = Leg.FlightNum;
                                        skyModel.FromCityCode = Leg.FromCode;
                                        skyModel.ToCityCode = Leg.ToCode;
                                        DateTime _FromDate = Convert.ToDateTime("1901-01-01");
                                        DateTime _ToDate = Convert.ToDateTime("1901-01-01");
                                        DateTime.TryParse(Leg.FlyDate1 + " " + Leg.FlyStartTime.Insert(2, ":") + ":00", out _FromDate);
                                        DateTime.TryParse(Leg.FlyDateE + " " + Leg.FlyEndTime.Insert(2, ":") + ":00", out _ToDate);
                                        skyModel.FromDate = _FromDate;
                                        skyModel.ToDate = _ToDate;
                                        skyModel.Space = Space;
                                        skyModel.IsShareFlight = Leg.IsShareFlight ? 1 : 0;
                                        skyModel.Terminal = Leg.FromCityT1 + Leg.ToCityT2;
                                        Bd_Air_AirPort cityinfo = GetCityInfo(Leg.FromCode, ImportParam.CityList);
                                        if (cityinfo != null)
                                        {
                                            skyModel.FromCityName = cityinfo.CityName;
                                        }
                                        cityinfo = GetCityInfo(Leg.ToCode, ImportParam.CityList);
                                        if (cityinfo != null)
                                        {
                                            skyModel.ToCityName = cityinfo.CityName;
                                        }
                                        Bd_Air_Carrier airInfo = GetAirInfo(Leg.AirCode.ToUpper(), ImportParam.AirList);
                                        if (airInfo != null)
                                        {
                                            skyModel.CarryName = airInfo.ShortName;
                                        }
                                        if (AirPrintOffice == "")
                                        {
                                            Tb_Ticket_PrintOffice printoffice = GetAirTicketInfo(Leg.AirCode.ToUpper(), ImportParam.GYAirTicketlist);
                                            if (printoffice != null)
                                            {
                                                AirPrintOffice = printoffice.OfficeCode;
                                            }
                                        }
                                        SkyList.Add(skyModel);
                                        //订单需要的信息
                                        orderCarryCode.Add(skyModel.CarryCode);
                                        orderFlightCode.Add(skyModel.FlightCode);
                                        orderAirTime.Add(skyModel.FromDate.ToString("yyyy-MM-dd HH:mm"));
                                        orderTravel.Add(skyModel.FromCityName + "-" + skyModel.ToCityName);
                                        orderTravelCode.Add(Leg.FromCode + "-" + Leg.ToCode);
                                        orderSpace.Add(Space);
                                    }
                                    #endregion

                                    sw.Stop();
                                    sbTime.Append("时间4:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                    if (pnrModel._PasType == "2" && (ImportParam.ImportTongDao == 2 || ImportParam.ImportTongDao == 4 || ImportParam.ImportTongDao == 0))
                                    {
                                        #region 儿童编码
                                        if (KongZhiXiTong != null && KongZhiXiTong.Contains("|95|"))//开启儿童编码必须关联成人编码或者成人订单号
                                        {
                                            ErrMsg = "";
                                            Tb_Ticket_Order AdultOrder = null;
                                            string AssociationAdultPnr = "";//关联成人编码
                                            string PnrPattern = @"^[A-Za-z0-9]{6}$";
                                            Match mch = Regex.Match(ImportParam.OrderId, PnrPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                            //是否为编码
                                            if (mch.Success)
                                            {
                                                AssociationAdultPnr = ImportParam.OrderId;
                                            }
                                            else
                                            {
                                                //否则为订单号
                                                AdultOrder = OrderManage.GetTicketOrderByOrderId(ImportParam.OrderId);
                                                if (AdultOrder == null)
                                                {
                                                    ErrMsg = "<font class='red'>" + pnr + "关联成人订单号(" + ImportParam.OrderId + ")不存在！</font>";
                                                }
                                                else
                                                {
                                                    if (AdultOrder.IsChdFlag)
                                                    {
                                                        ErrMsg = "<font class='red'>该订单(" + ImportParam.OrderId + ")非成人订单,无法导入!</font>";
                                                    }
                                                    else if (AdultOrder.OrderStatusCode < 4)
                                                    {
                                                        ErrMsg = "<font class='red'>关联成人订单(" + ImportParam.OrderId + ")未出票,无法导入!</font>";
                                                    }
                                                    else if (string.IsNullOrEmpty(AdultOrder.PNR))
                                                    {
                                                        ErrMsg = "<font class='red'>关联成人订单编码不存在,无法导入!</font>";
                                                    }
                                                    //else if (AdultOrder.PolicySource > 2)
                                                    //{
                                                    //    ErrMsg = "<font class='red'>该成人订单(" + ImportParam.OrderId + ")非本地订单,无法导入!</font>";
                                                    //}
                                                    else
                                                    {
                                                        if (!ValSkyWay(ImportParam.OrderId, SkyList))
                                                        {
                                                            ErrMsg = "<font class='red'>成人订单（" + ImportParam.OrderId + "）航程与儿童订单航程信息不一致，无法导入！</font>";
                                                        }
                                                    }
                                                    AssociationAdultPnr = AdultOrder.PNR;
                                                }
                                            }
                                            if (ErrMsg != "")
                                            {
                                                //Pnr内容导入
                                                ImportParam.SecondPM.Office = Office;
                                                ImportParam.SecondPM.OpType = "1";//继续
                                                ImportParam.SecondPM.Msg = pnr + "为儿童编码,请输入关联成人订单号或者成人编码！<br /><p><b>关联成人订单号/编码:</b><input type='text' maxlength='20' id='adultOrderId' value='" + ImportParam.OrderId + "' /></p><br/>" + ErrMsg;
                                                ImportParam.SecondPM.DataFlag = "3";
                                                ImportParam.IsNextOK = 1;//是否继续
                                                return IsSuc;
                                            }
                                            //导儿童编码时
                                            if (ImportParam.ImportTongDao == 2 && AssociationAdultPnr != "")
                                            {
                                                //儿童编码中备注成人编码
                                                string rmkIns = string.Format("RT{0}|SSR OTHS {1} ADULT PNR IS {2}|@", pnr.ToUpper(), CarrayCode, AssociationAdultPnr.ToUpper());
                                                SendIns.Send(rmkIns, ref Office, 0);
                                            }
                                        }
                                        #endregion
                                    }
                                    #region Pat信息
                                    string PatErrMsg = "";
                                    if (ImportParam.ImportTongDao != 4)
                                    {
                                        if (ImportParam.ImportTongDao == 2 || pnrModel._PasType == "2")
                                        {
                                            //儿童
                                            patModel = SendIns.GetPat(pnr, Office, 2, out PatErrMsg);
                                        }
                                        else
                                        {
                                            //成人
                                            patModel = SendIns.GetPat(pnr, Office, 1, out PatErrMsg);
                                        }
                                    }
                                    else
                                    {
                                        if (patModel == null && PatData.Trim() != "")
                                        {
                                            patModel = m_pnrformat.GetPATInfo(PatData.Trim(), out ErrMsg);
                                        }
                                    }
                                    if (patModel == null || patModel.PatList.Count == 0)
                                    {
                                        ErrMsg = string.IsNullOrEmpty(PatErrMsg) ? "PAT数据格式不规范，未能解析出价格数据！" : PatErrMsg;
                                        return IsSuc;
                                    }
                                    //是否有婴儿
                                    if (pnrModel.HasINF)
                                    {
                                        //婴儿
                                        patInfModel = SendIns.GetPat(pnr, Office, 3, out PatErrMsg);
                                        if (ImportParam.ImportTongDao != 4)
                                        {
                                            if (patInfModel == null || patInfModel.PatList.Count == 0)
                                            {
                                                ErrMsg = PatErrMsg;
                                                ErrMsg = string.IsNullOrEmpty(PatErrMsg) ? "编码" + pnr + "中婴儿PAT数据格式不规范，未能解析处价格数据！" : PatErrMsg;
                                                return IsSuc;
                                            }
                                        }
                                    }
                                    sw.Stop();
                                    sbTime.Append("时间5:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();
                                    //成人或者儿童的 舱位价 基建 燃油
                                    decimal SeatParice = 0m, JJFare = 0m, RQFare = 0m;
                                    //婴儿 舱位价 基建 燃油
                                    decimal INF_SeatParice = 0m, INF_JJFare = 0m, INF_RQFare = 0m;
                                    //存在子舱位
                                    if (pnrModel.IsExistChildSeat && patModel.ChildPat != null)
                                    {
                                        PatInfo pat = patModel.ChildPat;//子舱位价格
                                        decimal.TryParse(pat.Fare, out SeatParice);
                                        decimal.TryParse(pat.TAX, out JJFare);
                                        decimal.TryParse(pat.RQFare, out RQFare);
                                    }
                                    else
                                    {
                                        PatInfo patPrice = patModel.PatList[0];//低价格                                        
                                        if (ImportParam.m_LoginCompany.RoleType == 1 || ImportParam.m_LoginCompany.RoleType == 2 || ImportParam.m_LoginCompany.RoleType == 3)
                                        {
                                            //开启手工订单取高价
                                            if (KongZhiXiTong != "" && KongZhiXiTong.Contains("|97|"))
                                            {
                                                if (patModel.PatList.Count > 0)
                                                {
                                                    //高价格  最后一个价格
                                                    patPrice = patModel.PatList[patModel.PatList.Count - 1];
                                                }
                                            }
                                        }
                                        //价格
                                        decimal.TryParse(patPrice.Fare, out SeatParice);
                                        decimal.TryParse(patPrice.TAX, out JJFare);
                                        decimal.TryParse(patPrice.RQFare, out RQFare);
                                    }
                                    //婴儿价格
                                    if (pnrModel.HasINF && patInfModel != null && patInfModel.PatList.Count > 0)
                                    {
                                        PatInfo patFirst = patInfModel.PatList[0];
                                        decimal.TryParse(patFirst.Fare, out INF_SeatParice);
                                        decimal.TryParse(patFirst.TAX, out INF_JJFare);
                                        decimal.TryParse(patFirst.RQFare, out INF_RQFare);
                                    }
                                    #endregion

                                    sw.Stop();
                                    sbTime.Append("时间6:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                    #region 乘机人信息
                                    StringBuilder strPassenger = new StringBuilder();
                                    strPassenger.Append("<table id='PasTab'>");
                                    int Pcid = 0;
                                    if (ImportParam.sortList.Count > 0)
                                    {
                                        foreach (KeyValuePair<string, string> de in ImportParam.sortList)
                                        {
                                            PassengerInfo item = pnrModel._PassengerList.Find(delegate(PassengerInfo _Item)
                                            {
                                                return _Item.PassengerName == de.Key.ToString();
                                            });
                                            if (item != null)
                                            {
                                                item.SsrCardID = de.Value.ToString();
                                            }
                                        }
                                    }
                                    bool IsCidIsEmpty = false;//证件号是否有为空的

                                    foreach (PassengerInfo item in pnrModel._PassengerList)
                                    {
                                        Tb_Ticket_Passenger _mPassenger = new Tb_Ticket_Passenger();
                                        _mPassenger.PassengerName = item.PassengerName;
                                        _mPassenger.PassengerType = int.Parse(item.PassengerType);

                                        _mPassenger.CType = "1";

                                        _mPassenger.TicketNumber = item.TicketNum;
                                        if (item.PassengerType == "3")
                                        {
                                            if (ImportParam.sortList.Count > 0)
                                            {
                                                _mPassenger.Cid = item.SsrCardID;
                                            }
                                            else
                                            {
                                                _mPassenger.Cid = item.YinerBirthdayDate;
                                            }
                                            if (Regex.IsMatch(_mPassenger.Cid, @"(^(19|20)\d{2}\-\d{2}\-\d{2}$)|(^(19|20)\d{6}$)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                                            {
                                                _mPassenger.CType = "5";//出生日期
                                            }
                                            //价格
                                            _mPassenger.PMFee = INF_SeatParice;
                                            _mPassenger.ABFee = INF_JJFare;
                                            _mPassenger.FuelFee = INF_RQFare;
                                        }
                                        else
                                        {
                                            _mPassenger.Cid = item.SsrCardID;
                                            _mPassenger.PMFee = SeatParice;
                                            _mPassenger.ABFee = JJFare;
                                            _mPassenger.FuelFee = RQFare;
                                        }
                                        //为记账时
                                        if (ImportParam.IsImportJZ == 1)
                                        {
                                            _mPassenger.TicketStatus = 2;
                                        }
                                        PasList.Add(_mPassenger);
                                        if (string.IsNullOrEmpty(_mPassenger.Cid) && !IsCidIsEmpty)
                                        {
                                            IsCidIsEmpty = true;
                                        }
                                        strPassenger.AppendFormat("<tr><td>乘机人姓名:</td><td>{0}</td><td>证件号码:</td><td><input type='text' id='cid_{1}' name='PasCid' value='{2}' size='20' maxlength='20' pasType='{3}' /></td><td><span id='msg_{4}'></span></td></tr>", item.PassengerName, Pcid, _mPassenger.Cid, _mPassenger.PassengerType, Pcid);
                                        Pcid++;
                                    }
                                    strPassenger.Append("</table>");

                                    //PNR导入没有证件号或者证件号隐藏 自动跳到PNR内容导入   编码中不存在证件号 是否可以导入
                                    if (IsCidIsEmpty && !KongZhiXiTong.Contains("|58|"))//pnrmodel._SSRList.Count == 0
                                    {
                                        if (ImportParam.sortList.Count == 0 && ImportParam.Source == 0)
                                        {
                                            ImportParam.SecondPM.Office = Office;
                                            ImportParam.SecondPM.OpType = "1";//继续
                                            ImportParam.SecondPM.Msg = pnr + "缺少证件号码信息或者证件号码信息隐藏无法获取，请补全证件号后再导入！<br />" + strPassenger.ToString();
                                            ImportParam.SecondPM.PnrContent = pnrModel._OldPnrContent;
                                            ImportParam.SecondPM.PATContent = patModel.PatCon;
                                            ImportParam.SecondPM.DataFlag = "1";
                                            ImportParam.IsNextOK = 1;//是否继续
                                            return IsSuc;
                                        }
                                    }
                                    ImportParam.sortList.Clear();
                                    ImportParam.IsNextOK = 0;//不继续
                                    #endregion

                                    sw.Stop();
                                    sbTime.Append("时间7:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                    string option = "";
                                    string Discount = "";
                                    Bd_Air_Fares yFare = null;
                                    decimal SkySeatPrice = -1m;
                                    //修改航段价格和具体信息
                                    for (int i = 0; i < SkyList.Count; i++)
                                    {
                                        //价格信息
                                        SkyList[i].SpacePrice = SeatParice;
                                        SkyList[i].ABFee = JJFare;
                                        SkyList[i].FuelFee = RQFare;

                                        #region 获取折扣和里程
                                        option = SkyList[i].FromCityCode.ToUpper() + "_" + SkyList[i].ToCityCode.ToUpper() + "_" + SkyList[i].CarryCode.ToUpper();
                                        ImportParam.FareList = m_cache.GetCacheData("yfare", option) as List<Bd_Air_Fares>;
                                        if (ImportParam.FareList == null || ImportParam.FareList.Count == 0)
                                        {
                                            string sqlWhere = string.Format("FromCityCode='{0}' and ToCityCode='{1}' and (CarryCode='' or CarryCode='{2}') and  EffectTime<='{3}' and InvalidTime>='{3}' ", SkyList[i].FromCityCode, SkyList[i].ToCityCode, SkyList[i].CarryCode, SkyList[i].FromDate.ToString("yyyy-MM-dd HH:mm:ss"));
                                            ImportParam.FareList = GetYFaresList(sqlWhere);
                                            if (ImportParam.FareList != null)
                                            {
                                                m_cache.SetCacheData("yfare", ImportParam.FareList, 5, option);
                                            }
                                        }
                                        if (ImportParam.FareList != null)
                                        {
                                            Bd_Air_Fares fare_CarryCode = null;
                                            if (ImportParam.FareList.Count > 0)
                                            {
                                                fare_CarryCode = ImportParam.FareList.Find(delegate(Bd_Air_Fares _fare)
                                                {
                                                    return _fare.CarryCode.ToUpper() == SkyList[i].CarryCode.ToUpper();
                                                });
                                                if (fare_CarryCode == null)
                                                {
                                                    yFare = ImportParam.FareList[0];
                                                }
                                                else
                                                {
                                                    yFare = fare_CarryCode;
                                                }
                                            }
                                        }
                                        if (yFare != null)
                                        {
                                            //里程
                                            SkyList[i].Mileage = yFare.Mileage.ToString();
                                            //Y舱价格
                                            SkyList[i].FareFee = yFare.FareFee;
                                            //pnr内容导入婴儿价格处理
                                            if (INF_SeatParice <= 0)
                                            {
                                                decimal _TempFare = (yFare.FareFee * 0.1m) / 10;
                                                INF_SeatParice = d.FourToFiveNum(_TempFare, 0) * 10;
                                            }
                                            //不是单程的舱位价从数据库取
                                            if (pnrModel.TravelType >= 2)
                                            {
                                                SkySeatPrice = airqurey.GetSkyInfo(SkyList[i]).SpacePrice;
                                                if (SkySeatPrice != -1)
                                                {
                                                    SkyList[i].Discount = FormatPNR.GetZk(yFare.FareFee.ToString(), SkySeatPrice.ToString()).ToString();
                                                    SkyList[i].SpacePrice = SeatParice;
                                                }
                                            }
                                            else
                                            {
                                                SkyList[i].Discount = FormatPNR.GetZk(yFare.FareFee.ToString(), SeatParice.ToString()).ToString();
                                            }
                                        }
                                        //处理折扣长度
                                        Discount = SkyList[i].Discount;
                                        if (Discount.Length > 10)
                                        {
                                            Discount = Discount.Substring(0, 10);
                                        }
                                        SkyList[i].Discount = Discount;
                                        //内容
                                        SkyList[i].PnrContent = pnrModel != null ? pnrModel._OldPnrContent : "";

                                        SkyList[i].Pat = patModel != null ? patModel.PatCon : "";

                                        orderDiscount.Add(SkyList[i].Discount);
                                        #endregion

                                        //skyModel.Discount = 
                                        //skyModel.Aircraft =
                                        //skyModel.Mileage =


                                    }
                                    //更新婴儿舱位价
                                    for (int i = 0; i < PasList.Count; i++)
                                    {
                                        if (PasList[i].PassengerType == 3)
                                        {
                                            PasList[i].PMFee = INF_SeatParice;
                                        }
                                    }

                                    sw.Stop();
                                    sbTime.Append("时间8:" + sw.Elapsed.ToString() + " 编码:" + pnr + "\r\n");
                                    sw.Restart();

                                    #region 订单信息
                                    Order.PNR = pnr;
                                    Order.BigCode = pnrModel._BigPnr;
                                    Order.Office = Office;
                                    Order.PrintOffice = AirPrintOffice;
                                    Order.AllowChangePNRFlag = ImportParam.AllowChangePNR == 1 ? true : false;
                                    Order.IsChdFlag = pnrModel._PasType == "2" ? true : false;
                                    //儿童关联订单号
                                    Order.AssociationOrder = (Order.IsChdFlag && (ImportParam.ImportTongDao == 2 || ImportParam.ImportTongDao == 4)) ? ImportParam.OrderId : "";
                                    //确人订单来源
                                    if (ImportParam.m_LoginCompany != null)
                                    {
                                        if (ImportParam.m_LoginCompany.RoleType == 1)
                                        {
                                            Order.OrderSourceType = ImportParam.ImportTongDao != 4 ? 8 : 9;
                                            //是否未入库记账1是 0否
                                            if (ImportParam.IsImportJZ == 1)
                                            {
                                                Order.OrderSourceType = 12;
                                            }
                                        }
                                        else if (ImportParam.m_LoginCompany.RoleType == 2 || ImportParam.m_LoginCompany.RoleType == 3)
                                        {
                                            Order.OrderSourceType = ImportParam.ImportTongDao != 4 ? 6 : 7;
                                            //是否未入库记账1是 0否
                                            if (ImportParam.IsImportJZ == 1)
                                            {
                                                Order.OrderSourceType = 11;
                                            }
                                        }
                                        else
                                        {
                                            Order.OrderSourceType = ImportParam.ImportTongDao != 4 ? 2 : 3;
                                        }
                                    }

                                    Order.HaveBabyFlag = pnrModel.HasINF;
                                    Order.BabyFee = ((Order.OrderSourceType == 3 || Order.OrderSourceType == 7 || Order.OrderSourceType == 9) && pnrModel.HasINF) ? INF_SeatParice : Order.BabyFee;//婴儿票面价                                  
                                    Order.PolicySource = 1;//默认b2b
                                    Order.OrderStatusCode = 1;//默认新订单等待支付
                                    //记账处理
                                    if (ImportParam.IsImportJZ == 1)
                                    {
                                        Order.OrderStatusCode = 4;
                                        Order.PayStatus = 1;
                                        Order.PayWay = 0;// 0 如果 0有问题，就给默认 1 支付宝
                                        Order.TicketStatus = 2;
                                        Order.CPTime = System.DateTime.Now;
                                        Order.A1 = 1;
                                    }

                                    Order.LinkMan = m_Company.ContactUser;
                                    Order.LinkManPhone = m_Company.ContactTel;

                                    Order.CreateCpyName = ImportParam.m_LoginCompany != null ? ImportParam.m_LoginCompany.UninAllName : "";
                                    Order.CreateCpyNo = ImportParam.m_LoginCompany != null ? ImportParam.m_LoginCompany.UninCode : "";
                                    Order.CreateLoginName = ImportParam.m_LoginUser != null ? ImportParam.m_LoginUser.LoginName : "";
                                    Order.CreateUserName = ImportParam.m_LoginUser != null ? ImportParam.m_LoginUser.UserName : "";

                                    //出票公司编号
                                    string CPCpyNo = m_SupCompany != null ? m_SupCompany.UninCode : "";
                                    Order.CPCpyNo = CPCpyNo.Length > 12 ? CPCpyNo.Substring(0, 12) : CPCpyNo;

                                    Order.OwnerCpyNo = m_Company != null ? m_Company.UninCode : "";
                                    Order.OwnerCpyName = m_Company != null ? m_Company.UninAllName : "";
                                    Order.TravelType = pnrModel.TravelType;
                                    Order.CarryCode = string.Join("/", orderCarryCode.ToArray());
                                    Order.FlightCode = string.Join("/", orderFlightCode.ToArray());

                                    //Order.AirTime = string.Join("/", orderAirTime.ToArray());

                                    Order.AirTime = DateTime.Parse(orderAirTime[0]);

                                    Order.Travel = string.Join("/", orderTravel.ToArray());
                                    Order.TravelCode = string.Join("/", orderTravelCode.ToArray());
                                    Order.Space = string.Join("/", orderSpace.ToArray());
                                    Order.PassengerNumber = pnrModel._PassengerList.Count;

                                    //订单中的总价
                                    decimal TotalPMPrice = 0m, TotalABFare = 0, TotalRQFare = 0m;
                                    decimal PMFee = 0m;
                                    List<string> PasNameList = new List<string>();
                                    foreach (Tb_Ticket_Passenger item in PasList)
                                    {
                                        PasNameList.Add(item.PassengerName);
                                        //订单价格
                                        TotalPMPrice += item.PMFee;
                                        TotalABFare += item.ABFee;
                                        TotalRQFare += item.FuelFee;
                                        if (item.PassengerType != 3)
                                        {
                                            PMFee = item.PMFee;
                                        }
                                    }
                                    Order.PassengerName = string.Join("|", PasNameList.ToArray());
                                    Order.PMFee = TotalPMPrice;
                                    Order.ABFee = TotalABFare;
                                    Order.FuelFee = TotalRQFare;
                                    Order.Discount = string.Join("/", orderDiscount.ToArray());
                                    #endregion

                                    sw.Stop();
                                    sbTime.Append("时间9:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                    #region 升舱换开处理
                                    if (ImportParam.ImportTongDao == 5)
                                    {
                                        if (OleOm != null)
                                        {
                                            if (!CompareSkyWay(SkyList, OleOm.SkyList, out ErrMsg))
                                            {
                                                return IsSuc;
                                            }
                                        }

                                        //原订单实体
                                        Tb_Ticket_Order OleOrder = OleOm.Order;
                                        //订单来源
                                        Order.OrderSourceType = 10;
                                        //原始订单号
                                        Order.OldOrderId = OleOrder.OrderId;
                                        //修改新订单政策信息
                                        Order.PolicyId = OleOrder.PolicyId;
                                        //出票公司编号
                                        CPCpyNo = OleOrder.CPCpyNo;
                                        Order.CPCpyNo = CPCpyNo.Length > 12 ? CPCpyNo.Substring(0, 12) : CPCpyNo;
                                        Order.AirPoint = OleOrder.AirPoint;
                                        Order.PolicyPoint = OleOrder.PolicyPoint;
                                        Order.ReturnPoint = OleOrder.ReturnPoint;
                                        Order.LaterPoint = OleOrder.LaterPoint;
                                        //政策备注
                                        Order.PolicyRemark = OleOrder.PolicyRemark;
                                        //原始政策返点
                                        Order.OldPolicyPoint = OleOrder.OldPolicyPoint;
                                        //原始政策现返
                                        Order.OldReturnMoney = OleOrder.OldReturnMoney;
                                        //佣金
                                        Order.PolicyMoney = data.CreateCommissionCG(PMFee, Order.ReturnPoint);
                                        /*
                                        //计算订单金额         
                                        decimal PayPrice = d.CreateOrderPayMoney(Order, PasList);
                                        Order.PayMoney = PayPrice;
                                        //计算订单金额         
                                        decimal OrderPrice = d.CreateOrderOrderMoney(Order, PasList);
                                        Order.OrderMoney = OrderPrice;
                                        */
                                        Bill bill = new Bill();
                                        bill.CreateOrderAndTicketPayDetailNew(Order, PasList);

                                        Order.ReturnMoney = OleOrder.ReturnMoney;
                                        Order.DiscountDetail = OleOrder.DiscountDetail;
                                        Order.PolicyType = OleOrder.PolicyType;
                                        Order.PolicySource = OleOrder.PolicySource;
                                        Order.AutoPrintFlag = OleOrder.AutoPrintFlag;
                                        Order.PolicyCancelTime = OleOrder.PolicyCancelTime;
                                        Order.PolicyReturnTime = OleOrder.PolicyReturnTime;
                                        //政策种类
                                        Order.A2 = OleOrder.A2;
                                        Order.A1 = 1;
                                        //需要生成账单明细
                                        ImportParam.OrderParam.IsCreatePayDetail = 1;
                                    }
                                    #endregion

                                    sw.Stop();
                                    sbTime.Append("时间10:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                    #region 订单日志
                                    logOrder.OperTime = DateTime.Now;
                                    logOrder.OperType = "创建订单";
                                    logOrder.OperContent = (ImportParam.m_LoginUser != null ? ImportParam.m_LoginUser.LoginName : "") + "于" + logOrder.OperTime + "创建订单。";
                                    logOrder.OperLoginName = (ImportParam.m_LoginUser != null ? ImportParam.m_LoginUser.LoginName : "");
                                    logOrder.OperUserName = (ImportParam.m_LoginUser != null ? ImportParam.m_LoginUser.UserName : "");
                                    logOrder.CpyNo = (ImportParam.m_LoginCompany != null ? ImportParam.m_LoginCompany.UninCode : "");
                                    logOrder.CpyName = (ImportParam.m_LoginCompany != null ? ImportParam.m_LoginCompany.UninName : "");
                                    logOrder.CpyType = ImportParam.m_LoginCompany != null ? ImportParam.m_LoginCompany.RoleType : 1;
                                    logOrder.WatchType = ImportParam.m_LoginCompany != null ? ImportParam.m_LoginCompany.RoleType : 1;
                                    #endregion

                                    sw.Stop();
                                    sbTime.Append("时间11:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                    #region 编码信息

                                    ImportParam.OrderParam.PnrInfo.AdultPnr = pnrModel._PasType == "1" ? pnr : "";
                                    ImportParam.OrderParam.PnrInfo.childPnr = pnrModel._PasType == "1" ? "" : pnr;
                                    ImportParam.OrderParam.PnrInfo.AdultPnrRTContent = pnrModel._PasType == "1" ? pnrModel._OldPnrContent : "";
                                    ImportParam.OrderParam.PnrInfo.childPnrRTContent = pnrModel._PasType == "1" ? "" : pnrModel._OldPnrContent;
                                    if (ImportParam.ImportTongDao != 4)
                                    {
                                        ImportParam.OrderParam.PnrInfo.HandleRTContent = pnrModel._OldPnrContent;
                                    }
                                    else
                                    {
                                        ImportParam.OrderParam.PnrInfo.HandleRTContent = RtData;
                                    }
                                    ImportParam.OrderParam.PnrInfo.AdultPnrToBigPNR = pnrModel._PasType == "1" ? pnrModel._BigPnr : "";
                                    ImportParam.OrderParam.PnrInfo.childPnrToBigPNR = pnrModel._PasType == "1" ? "" : pnrModel._BigPnr;
                                    ImportParam.OrderParam.PnrInfo.Office = Office;
                                    PnrModel[] PnrList = new PnrModel[2];
                                    PnrList[0] = pnrModel._PasType == "1" ? pnrModel : null;
                                    PnrList[1] = pnrModel._PasType == "1" ? null : pnrModel;
                                    ImportParam.OrderParam.PnrInfo.PnrList = PnrList;
                                    ImportParam.OrderParam.PnrInfo.PrintOffice = AirPrintOffice;
                                    string[] strPat = new string[3];
                                    strPat[0] = pnrModel._PasType == "1" ? patModel.PatCon : "";
                                    strPat[1] = pnrModel._PasType == "2" ? patModel.PatCon : "";
                                    strPat[2] = patInfModel != null ? patInfModel.PatCon : "";
                                    ImportParam.OrderParam.PnrInfo.PatList = strPat;
                                    PatModel[] PatModelList = new PatModel[3];
                                    PatModelList[0] = pnrModel._PasType == "1" ? patModel : null;
                                    PatModelList[1] = pnrModel._PasType == "2" ? patModel : null;
                                    PatModelList[2] = patInfModel;
                                    ImportParam.OrderParam.PnrInfo.PatModelList = PatModelList;
                                    ImportParam.RTData = pnrModel._OldPnrContent;
                                    ImportParam.PATData = patModel.PatCon;
                                    ImportParam.RTAndPatData = ImportParam.RTData + "\r\n" + ImportParam.PATData;
                                    #endregion

                                    sw.Stop();
                                    sbTime.Append("时间12:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();


                                    if (ImportParam.IsCreateOrder)
                                    {
                                        //生成订单
                                        IsSuc = OrderManage.CreateOrder(ref ImportParam.OrderParam, out ErrMsg);
                                    }
                                    else
                                    {
                                        //赋值Id
                                        OrderMustParamModel om = ImportParam.OrderParam.OrderParamModel[0];
                                        for (int i = 0; i < om.PasList.Count; i++)
                                        {
                                            om.PasList[i].id = Guid.NewGuid();
                                        }
                                        for (int i = 0; i < om.SkyList.Count; i++)
                                        {
                                            om.SkyList[i].id = Guid.NewGuid();
                                        }
                                        IsSuc = true;
                                    }

                                    sw.Stop();
                                    sbTime.Append("时间13:" + sw.Elapsed.ToString() + "\r\n");
                                    sw.Restart();

                                }
                            }
                        }
                    }
                    else
                    {
                        ImportParam.SecondPM.Msg = ErrMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg += ex.Message;
            }
            finally
            {
                ImportParam.TipMsg = ErrMsg;

                sw.Stop();
                sbTime.Append("时间14:" + sw.Elapsed.ToString() + "\r\n");
                //PnrAnalysis.LogText.LogWrite(sbTime.ToString(), "PNRImport_Time");

            }
            return IsSuc;
        }
    }
}
