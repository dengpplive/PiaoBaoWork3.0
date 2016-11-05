using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Net;
using System.Web;
using System.Configuration;
using IRemoteMethodSpace;
using System.Web.Caching;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using DataBase.Data;
namespace PbProject.Logic.Buy
{
    public class AirQurey
    {
        #region 参数
        /// <summary>
        /// AVH线程
        /// </summary>
        public Thread avhthread;
        /// <summary>
        /// 基础数据线程
        /// </summary>
        public Thread datathread;
        /// <summary>
        /// 政策线程
        /// </summary>
        public Thread policythread;
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        private string fromcity = "";
        /// <summary>
        /// 中转城市城市
        /// </summary>
        private string middlecity = "";
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        private string tocity = "";
        /// <summary>
        /// 出发时间
        /// </summary>
        private string fromtime = "";
        /// <summary>
        /// 往返与联程第二日程出发时间
        /// </summary>
        private string Totimes = "";
        /// <summary>
        /// 承运人代码
        /// </summary>
        private string cairrys = "";
        /// <summary>
        /// 行程类型 1:单程 2：往返 3：联程
        /// </summary>
        private int Traveltypes = 1;
        /// <summary>
        /// 员工model
        /// </summary>
        private PbProject.Model.User_Employees mUser = null;
        /// <summary>
        /// avh结果字符串
        /// </summary>
        private string avhresult = "";
        /// <summary>
        /// avh结果状态
        /// 0正在准备获取,1未开启配置,2航段信息解析失败,3无数据返回,4超时,5获取成功,6代理人配置信息异常
        /// </summary>
        private int avhresultState = 0;
        /// <summary>
        /// 基础数据ds
        /// </summary>
        private DataSet dsData = null;
        /// <summary>
        /// 政策ds
        /// </summary>
        private DataSet dsPolicy = null;
        /// <summary>
        /// IBE 数据
        /// </summary>
        private DataSet dsIBE = null;
        /// <summary>
        /// 政策获取是否完成
        /// </summary>
        private int isPolicyOk = 0;
        /// <summary>
        /// 基础数据获取状态(0正准备获取,1未获取,2获取成功,3缓存关闭获取失败)
        /// 注意,状态只是"获取的状态",获取成功了,并不代表数据正确
        /// </summary>
        private int DataOk = 0;
        /// <summary>
        /// 页面显示字符串
        /// </summary>
        private string PageStr = "";
        /// <summary>
        /// 公司实体
        /// </summary>
        private PbProject.Model.User_Company mCompanys = null;
        /// <summary>
        /// 航班数量
        /// </summary>
        private int nums = 0;
        /// <summary>
        /// 是否显示共享
        /// </summary>
        private bool IsShowShare;
        /// <summary>
        /// 参数信息
        /// </summary>
        private List<PbProject.Model.Bd_Base_Parameters> mbaseParameters;
        /// <summary>
        /// 自定义参数
        /// </summary>
        private PbProject.Model.definitionParam.BaseSwitch BaseParams;
        /// <summary>
        /// 缓存对象
        /// </summary>
        public static IRemoteMethod remoteobj = null;
        BaseDataManage Manage = new BaseDataManage();
        private string cacheInfo = "";


        #endregion

        private void SetBaseParams()
        {
            if (mCompanys.UninCode.Length > 12)
            {
                string GCpyNo = mCompanys.UninCode.Substring(0, 12);//获取上级供应商或落地运营商的ID
                string sqlwhere = "CpyNo='" + GCpyNo + "'";
                List<Bd_Base_Parameters> objList = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlwhere }) as List<Bd_Base_Parameters>;
                BaseParams = PbProject.WebCommon.Utility.BaseParams.getParams(objList);
            }
            else
            {
                BaseParams = PbProject.WebCommon.Utility.BaseParams.getParams(mbaseParameters);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="manageconfigmodel">平台参数model</param>
        /// <param name="msupperconfigmodel">平台供应商参数model</param>
        public AirQurey()
        {
            string currentuserid = HttpContext.Current.Request["currentuserid"].ToString();
            //SessionContent sessionContent = HttpContext.Current.Session[currentuserid] as SessionContent;
            SessionContent sessionContent = HttpContext.Current.Application[currentuserid] as SessionContent;
            mbaseParameters = sessionContent.BASEPARAMETERS;
            mUser = sessionContent.USER;
            mCompanys = sessionContent.COMPANY;
            SetBaseParams();
        }

        public AirQurey(List<PbProject.Model.Bd_Base_Parameters> mbaseParameters, PbProject.Model.User_Employees mUser, PbProject.Model.User_Company mCompanys)
        {
            this.mbaseParameters = mbaseParameters;
            this.mUser = mUser;
            this.mCompanys = mCompanys;

            SetBaseParams();


        }
        /// <summary>
        /// 处理webservice返回的数据
        /// </summary>
        /// <param name="ds"></param>
        private string reorganizedData(DataSet ds)
        {
            StringBuilder sb = new StringBuilder("");
            try
            {
                if (!ds.Tables.Contains("errorTable"))//没有返回错误数据
                {
                    int FltShopAVJourneyCount = ds.Tables["FltShopAVJourney"].Rows.Count;
                    int FltShopAVOptCount = ds.Tables["FltShopAVOpt"].Rows.Count;
                    int CabinAllCount = ds.Tables["CabinAll"].Rows.Count;
                    int TaxAllCount = ds.Tables["TaxAll"].Rows.Count;
                    int FltShopAVFltCount = ds.Tables["FltShopAVFlt"].Rows.Count;
                    int FltShopPSCount = ds.Tables["FltShopPS"].Rows.Count;
                    int RouteAllCount = ds.Tables["RouteAll"].Rows.Count;
                    int FltShopNFareCount = ds.Tables["FltShopNFare"].Rows.Count;
                    int FltShopPFareCount = ds.Tables["FltShopPFare"].Rows.Count;
                    int FltShopRuleCount = ds.Tables["FltShopRule"].Rows.Count;

                    for (int i = 0; i < FltShopAVJourneyCount; i++)
                    {
                        for (int j = 0; j < FltShopAVOptCount; j++)
                        {
                            for (int k = 0; k < FltShopAVFltCount; k++)
                            {
                                if (!ds.Tables["FltShopAVFlt"].Rows[k]["A0"].ToString().Contains(ds.Tables["FltShopAVOpt"].Rows[j]["A0"].ToString()))
                                {
                                    continue;
                                }
                                if (k != 0)
                                {
                                    sb.Append("^");
                                }
                                sb.Append(changeDate(ds.Tables["FltShopAVFlt"].Rows[k]["A4"].ToString()));//0.出发日期
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A7"].ToString().Substring(0, 2) + ":" + ds.Tables["FltShopAVFlt"].Rows[k]["A7"].ToString().Substring(2, 2));//1.起飞时间
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A9"].ToString().Substring(0, 2) + ":" + ds.Tables["FltShopAVFlt"].Rows[k]["A9"].ToString().Substring(2, 2));//2.到达时间
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A1"].ToString());//3.航空公司
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A2"].ToString());//4.航班号
                                sb.Append(",");
                                string getCabinNameAndAvAll = "";
                                string getChildCabinNameAndAvAll = "";
                                for (int m = 0; m < CabinAllCount; m++)
                                {
                                    if (ds.Tables["CabinAll"].Rows[m]["A0"].ToString() == ds.Tables["FltShopAVFlt"].Rows[k]["A0"].ToString())
                                    {
                                        if (ds.Tables["CabinAll"].Rows[m]["A1"].ToString().Length == 1)
                                        {
                                            getCabinNameAndAvAll += ds.Tables["CabinAll"].Rows[m]["A1"].ToString() + ds.Tables["CabinAll"].Rows[m]["A2"].ToString();
                                        }
                                        if (ds.Tables["CabinAll"].Rows[m]["A1"].ToString().Length > 1)
                                        {
                                            if (getChildCabinNameAndAvAll != "")
                                            {
                                                getChildCabinNameAndAvAll += " ";
                                            }
                                            getChildCabinNameAndAvAll += ds.Tables["CabinAll"].Rows[m]["A1"].ToString() + ds.Tables["CabinAll"].Rows[m]["A2"].ToString();
                                        }
                                    }
                                }
                                sb.Append(getCabinNameAndAvAll + "*9");//5.舱位信息
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A5"].ToString());//6.起始城市
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A6"].ToString());//7.目的城市
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A11"].ToString());//8.机型
                                sb.Append(",");
                                sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A19"].ToString());//9.经停
                                sb.Append(",");
                                string isA13 = "0";
                                if (ds.Tables["FltShopAVFlt"].Rows[k]["A13"].ToString() != "")
                                {
                                    isA13 = "1";
                                }
                                sb.Append(isA13);//10.餐食
                                sb.Append(",");
                                string isA16 = "1";//基本都是电子客票,默认为1
                                if (ds.Tables["FltShopAVFlt"].Rows[k]["A16"].ToString() != "E")
                                {
                                    isA16 = "0";
                                }
                                sb.Append(isA16);//11.电子客票    
                                sb.Append(",");
                                string isA22 = "False";
                                if (ds.Tables["FltShopAVFlt"].Rows[k]["A22"].ToString() != "")
                                {
                                    isA22 = "True";
                                }
                                sb.Append(isA22);//12.是否共享
                                sb.Append(",");
                                sb.Append(getChildCabinNameAndAvAll);//13.子舱位
                                sb.Append(",");
                                string FltShopAVFltA20 = ds.Tables["FltShopAVFlt"].Rows[k]["A20"].ToString();
                                string FltShopAVFltA21 = ds.Tables["FltShopAVFlt"].Rows[k]["A21"].ToString();
                                if (FltShopAVFltA20 == "")
                                {
                                    FltShopAVFltA20 = "--";
                                }
                                if (FltShopAVFltA21 == "")
                                {
                                    FltShopAVFltA21 = "--";
                                }
                                sb.Append(FltShopAVFltA20 + FltShopAVFltA21);//14.出发城市航站楼+到达城市航站楼
                                //2012-11-20,08:00,10:30,ZH,4113,F9Y9B9MSHSKSLSQSGSVSESTSS9*9,CTU,PEK,321,0,0,1,True,M1S,T2T3^

                                //2012-11-20,08:00,10:30,ZH,4113,FAYABASA*9,CTU,PEK,321,0,,E,CA4113,,T2T3^2012-11-20,09:00,11:35,ZH,4101,FAYABASA*9,CTU,PEK,321,0,,E,CA4101,,T2T3


                            }
                        }
                    }
                }
                else
                {
                    OnError(ds.Tables["errorTable"].Rows[0][0].ToString(), "reorganizedData IBE接口错误信息");
                }

            }
            catch (Exception ex)
            {
                OnError(ex.Message, "reorganizedData 通过IBE接口查询航班");
            }
            return sb.ToString();

        }

        /// <summary>
        /// 处理webservice返回的数据
        /// </summary>
        /// <param name="ds"></param>
        private string reorganizedDataYeeXing(DataSet ds)
        {
            StringBuilder sb = new StringBuilder("");
            try
            {
                //2012-11-20,08:00,10:30,ZH,4113,FAYABASA*9,CTU,PEK,321,0,,E,CA4113,,T2T3^
                //2012-11-20,09:00,11:35,ZH,4101,FAYABASA*9,CTU,PEK,321,0,,E,CA4101,,T2T3

                int flightCount = ds.Tables["flight"].Rows.Count;
                int cabinCount = ds.Tables["cabin"].Rows.Count;

                for (int i = 0; i < flightCount; i++)
                {

                    sb.Append(DateTime.Parse(ds.Tables["flight"].Rows[i]["startTime"].ToString()).ToString("yyyy-MM-dd"));//0.出发日期
                    sb.Append(",");
                    sb.Append(DateTime.Parse(ds.Tables["flight"].Rows[i]["startTime"].ToString()).ToString("HH:ss"));//1.起飞时间
                    sb.Append(",");
                    sb.Append(DateTime.Parse(ds.Tables["flight"].Rows[i]["endTime"].ToString()).ToString("HH:ss"));//2.到达时间
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["airComp"].ToString());//3.航空公司
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["flightno"].ToString().Substring(2, ds.Tables["flight"].Rows[i]["flightno"].ToString().Length - 2));//4.航班号
                    sb.Append(",");
                    for (int j = 0; j < cabinCount; j++)
                    {
                        if (ds.Tables["flight"].Rows[i]["flight_Id"].ToString() == ds.Tables["cabin"].Rows[j]["cabins_Id"].ToString())
                        {
                            sb.Append(ds.Tables["cabin"].Rows[j]["cabinCode"].ToString());
                            sb.Append(ds.Tables["cabin"].Rows[j]["cabinSales"].ToString());

                        }
                    }
                    sb.Append("*9");//5.舱位信息
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["orgCity"].ToString());//6.起始城市
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["dstCity"].ToString());//7.目的城市
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["planeType"].ToString());//8.机型
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["stopNumber"].ToString());//9.经停
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["mealCode"].ToString());//10.餐食
                    sb.Append(",");
                    sb.Append("1");//11.电子客票  
                    sb.Append(",");
                    sb.Append("False");//12.是否共享
                    sb.Append("");//13.子舱位
                    sb.Append(",");
                    sb.Append(ds.Tables["flight"].Rows[i]["departTerm"].ToString() + ds.Tables["flight"].Rows[i]["arrivalTerm"].ToString());//14.出发城市航站楼+到达城市航站楼
                    if (i != flightCount - 1)
                    {
                        sb.Append("^");
                    }
                }

            }
            catch (Exception ex)
            {
                OnError(ex.Message, "reorganizedData 通过IBE接口查询航班");
            }
            return sb.ToString();

        }
        /// <summary>
        /// 转换时间
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string changeDate(string code)
        {
            //20dec12  2012-11-20
            string month = "";
            string[] str = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == code.Substring(2, 3))
                {
                    month = (i + 1).ToString();
                    break;
                }
            }
            return "20" + code.Substring(5, 2) + "-" + month + "-" + code.Substring(0, 2);
        }
        /// <summary>
        /// 转换时间
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string changeDate2(string code)
        {
            //2012-11-20 20DEC12
            string month = code.Substring(5, 2);
            string monthCode = "";
            switch (month)
            {
                case "01":
                    monthCode = "JAN";
                    break;
                case "02":
                    monthCode = "FEB";
                    break;
                case "03":
                    monthCode = "MAR";
                    break;
                case "04":
                    monthCode = "APR";
                    break;
                case "05":
                    monthCode = "MAY";
                    break;
                case "06":
                    monthCode = "JUN";
                    break;
                case "07":
                    monthCode = "JUL";
                    break;
                case "08":
                    monthCode = "AUG";
                    break;
                case "09":
                    monthCode = "SEP";
                    break;
                case "10":
                    monthCode = "OCT";
                    break;
                case "11":
                    monthCode = "NOV";
                    break;
                case "12":
                    monthCode = "DEC";
                    break;
                default:
                    break;
            }
            return code.Substring(8, 2) + monthCode + code.Substring(2, 2);
        }
        public string NewQueryInterface(string fcity, string mcity, string tcity, string time, string Totime, string carry, int TravelType, DataSet dsIBE)
        {
            string NewStr = "";
            try
            {
                string stime = DateTime.Now.ToString("mm:ss:fff");
                string result = "";
                int flag = 0;
                if (BaseParams.KongZhiXiTong.Contains("|53|"))//开启自己的配置
                {
                    flag = 1;
                }
                if (BaseParams.KongZhiXiTong.Contains("|44|"))//开启IBE接口
                {
                    flag = 2;
                }
                //IBE和易行同时开启,易行优先
                if (BaseParams.KongZhiXiTong.Contains("|92|"))//开启易行接口
                {
                    flag = 3;
                }

                switch (flag)
                {
                    case 0:
                        result = "1";
                        NewStr = "1";
                        break;
                    case 1://自己的配置
                        string rs = "";
                        string errors = "";
                        int temp = 450;
                        int.TryParse(BaseParams.HeiPingCanShu.Split('|')[3] != "" ? BaseParams.HeiPingCanShu.Split('|')[3] : "450", out temp);
                        string strIP = BaseParams.HeiPingCanShu.Split('|')[2] != "" ? BaseParams.HeiPingCanShu.Split('|')[2] : "203.88.210.234";
                        PBPid.WebManage.WebManage.ServerIp = strIP;
                        PBPid.WebManage.WebManage.ServerPort = temp;
                        PBPid.WebManage.WebManage.PidSendCommand_av(fromcity, tocity, carry, fromtime, "0000", BaseParams.HeiPingCanShu.Split('|')[4].Split('^')[0], "AVH", ref rs, ref errors, strIP, temp);
                        //2012-09-20,1610,1715,ZH,9554,F6P4A2OQDQY9BQMQHQJQQQZQGQVQWQEQTQSQX2TQEQ*9,LHW,XIY,320,0,0,1,false,M1A V1Q,T2T3
                        if (rs.Contains("由于"))
                        {
                            rs = "6";
                            NewStr = "6";
                        }
                        result = rs;
                        break;
                    case 2://IBE
                        if (dsIBE != null)
                        {
                            if (dsIBE.Tables.Count == 1)
                            {
                                result = "3";
                                NewStr = "3";
                            }
                            else
                            {
                                result = reorganizedData(dsIBE);
                            }
                        }
                        else
                        {
                            result = "4";
                            NewStr = "4";
                        }
                        break;
                    case 3://易行
                        //数据提供方非本公司,不能保证数据的稳定性,故而判断偏多,修改者不要随意删除
                        if (dsIBE != null)
                        {
                            if (dsIBE.Tables.Count > 0)
                            {
                                if (dsIBE.Tables[0].Rows[0]["is_success"].ToString().ToUpper() == "F")
                                {

                                    result = "3";
                                    NewStr = "3";
                                }
                                else
                                {
                                    result = reorganizedDataYeeXing(dsIBE);
                                }
                            }
                            else
                            {
                                result = "4";
                                NewStr = "4";
                            }
                        }
                        else
                        {
                            result = "4";
                            NewStr = "4";
                        }
                        break;
                    default:
                        result = "1";
                        NewStr = "1";
                        break;
                }


                if (result != "3" && result != "4" && result != "1" && result != "6")
                {
                    string[] slist = result.Split('^');
                    for (int i = 0; i < slist.Length; i++)
                    {
                        string[] strlist = slist[i].Split(',');
                        if (strlist.Length >= 13)
                        {

                            //判断是否开启共享航班
                            int x = 0;
                            if (BaseParams.KongZhiXiTong.Contains("|74|"))//开启共享航班开关
                            {
                                x++;
                            }
                            if (IsShowShare)
                            {
                                x = 0;
                            }
                            if (x == 0 && strlist[12].ToString().ToUpper().Trim() == "TRUE")
                            {
                                continue;
                            }
                            if (cairrys.Trim() != "" && !strlist[3].Contains(cairrys))
                            {
                                continue;
                            }
                            if (!strlist[1].Contains(":"))
                            {
                                strlist[1] = strlist[1].Substring(0, 2) + ":" + strlist[1].Substring(2, 2);
                            }
                            DateTime date = DateTime.Parse(strlist[0] + " " + strlist[1]);
                            DateTime nowdate = DateTime.Now;
                            if (date < nowdate)
                            {
                                continue;
                            }

                            //0.出发日期
                            NewStr += strlist[0];
                            //1.起飞时间
                            NewStr += "," + strlist[1];
                            //2.到达时间
                            NewStr += "," + strlist[2];
                            //3.航空公司
                            NewStr += "," + strlist[3];
                            if (strlist[12].ToString().ToUpper().Trim() == "TRUE")
                            {
                                strlist[4] += "*";
                            }
                            //4.航班号
                            NewStr += "," + strlist[4];
                            //5.舱位信息
                            NewStr += "," + strlist[5].Split('*')[0];
                            //6.起始城市
                            NewStr += "," + strlist[6];
                            //7.目的城市
                            NewStr += "," + strlist[7];
                            //8.机型
                            NewStr += "," + strlist[8];
                            //9.经停
                            NewStr += "," + strlist[9];
                            //10.餐食
                            NewStr += "," + strlist[10];
                            //11.电子客票
                            NewStr += "," + strlist[11];
                            //12.是否共享
                            NewStr += "," + strlist[12];
                            //13.出发城市航站楼
                            //NewStr += "," + ((strlist.Length == 15 && strlist[14].Length >= 4) ? strlist[14].Substring(0, 2) : "");


                            if (strlist.Length == 15)
                            {
                                string temp = "";
                                switch (strlist[14].Length)
                                {
                                    case 2:
                                        temp = strlist[14].Substring(0, 1);
                                        break;
                                    case 3:
                                        //处理航站楼 返回为  T3D或DT3这类格式的
                                        if (strlist[14].Substring(0, 1) == "D")
                                        {
                                            temp = strlist[14].Substring(0, 1);

                                        }
                                        else
                                        {
                                            temp = strlist[14].Substring(0, 2);
                                        }
                                        break;
                                    case 4:
                                        temp = strlist[14].Substring(0, 2);
                                        break;
                                    default:
                                        temp = "";
                                        break;
                                }
                                NewStr += "," + temp;
                            }
                            else
                            {
                                NewStr += "," + "--";
                            }
                            //14.到达城市航站楼
                            //NewStr += "," + ((strlist.Length == 15 && strlist[14].Length >= 4) ? strlist[14].Substring(2, 2) : "");
                            if (strlist.Length == 15)
                            {
                                string temp = "";
                                switch (strlist[14].Length)
                                {
                                    case 2:
                                        temp = strlist[14].Substring(1, 1);
                                        break;
                                    case 3:
                                        //处理航站楼 返回为  T3D或DT3这类格式的
                                        if (strlist[14].Substring(0, 1) == "D")
                                        {
                                            temp = strlist[14].Substring(1, 2);

                                        }
                                        else
                                        {
                                            temp = strlist[14].Substring(2, 1);
                                        }
                                        break;
                                    case 4:
                                        temp = strlist[14].Substring(2, 2);
                                        break;
                                    default:
                                        temp = "";
                                        break;
                                }
                                NewStr += "," + temp;
                            }
                            else
                            {
                                NewStr += "," + "--";
                            }
                            string ChildSpaces = "";
                            if (strlist.Length > 13)
                            {
                                if (strlist[13].Trim() != "")
                                {
                                    string[] ChildSpaceList = strlist[13].Split(' ');
                                    foreach (string ChildSpace in ChildSpaceList)
                                    {
                                        ChildSpaces += ChildSpace.Substring(0, ChildSpace.Length - 1);
                                    }
                                }
                            }
                            //15.子舱位
                            NewStr += "," + ChildSpaces;
                            NewStr += "^";
                        }
                    }
                    if (NewStr != "")
                    {
                        NewStr = NewStr.Substring(0, NewStr.Length - 1);
                    }
                }
                string etime = DateTime.Now.ToString("mm:ss:fff");
                OnError("查询时间:" + stime + "--" + etime + ",长度:" + NewStr.Length, "NewQueryInterface 通过接口查询航班");
            }
            catch (Exception ex)
            {
                NewStr = "2";
                OnError(ex.ToString(), "NewQueryInterface 通过接口查询航班");
            }
            return NewStr;
        }
        /// <summary>
        /// 查询航班
        /// </summary>
        /// <param name="fcity">出发城市三字码</param>
        /// <param name="mcity">中转城市三字码</param>
        /// <param name="tcity">到达城市三字码</param> 
        /// <param name="time">出发时间</param> 
        /// <param name="Totime">往返与联程第二次出发时间</param>
        /// <param name="cairry">承运人</param>  
        /// <param name="type">类型 1为 单程往返，2联程</param>
        /// <param name="TravelType">行程类型</param>
        /// <param name="mEmployees">员工model</param> 
        /// <param name="mCompany">公司model</param> 
        /// <returns></returns>
        public string Start(PbProject.Model.definitionParam.SelectCityParams selectCityParams)
        {


            fromcity = selectCityParams.fcity;
            middlecity = selectCityParams.mcity;
            tocity = selectCityParams.tcity;
            fromtime = selectCityParams.time;
            Totimes = selectCityParams.Totime;
            mUser = selectCityParams.mEmployees;
            mCompanys = selectCityParams.mCompany;
            cairrys = selectCityParams.cairry;
            Traveltypes = selectCityParams.TravelType;
            IsShowShare = selectCityParams.IsShowGX;
            //记录查询记录
            StartAirQueryInsertThread();
            //线程开始前先获取IBE接口数据
            dsIBE = getIBEDataSet(cairrys, fromcity, tocity, fromtime);
            //开始线程
            StartAVHThread();
            StartDataThread();
            //StartPolicyThread();
            StartQuery();
            string result = PageStr;
            selectCityParams.num = nums;
            return result;
        }

        #region 抢票查询航班 只匹配基础数据
        public DataSet RobTicketFlightQuery(PbProject.Model.definitionParam.SelectCityParams selectCityParams)
        {
            fromcity = selectCityParams.fcity;
            middlecity = selectCityParams.mcity;
            tocity = selectCityParams.tcity;
            fromtime = selectCityParams.time;
            Totimes = selectCityParams.Totime;
            mUser = selectCityParams.mEmployees;
            mCompanys = selectCityParams.mCompany;
            cairrys = selectCityParams.cairry;
            Traveltypes = selectCityParams.TravelType;
            IsShowShare = selectCityParams.IsShowGX;

            //记录查询记录
            StartAirQueryInsertThread();
            //线程开始前先获取IBE接口数据
            dsIBE = getIBEDataSet(cairrys, fromcity, tocity, fromtime);
            //开始线程
            StartAVHThread();
            StartDataThread();
            DataSet ds = GetRobTicketData();
            return ds;
        }

        public DataSet GetRobTicketData()
        {
            //匹配基础数据信息
            DataSet ds = null;
            int x = 0;
            string errorInfo = "";
            while (true)
            {
                x++;
                #region 超过30秒的情况
                if (x > 300)
                {
                    //超过30秒,只要不是成功,无论是什么状态,都默认为超时
                    if (avhresultState != 5)
                    {
                        errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeTimeOut;
                    }
                    if (DataOk != 2)
                    {
                        errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.cacheTimeOut;
                    }
                    break;
                }
                #endregion
                #region 返回了状态,但是状态不合格,过滤掉

                //IBE或易行无数据返回,则线程不需要等待了,直接跳出
                if (DataOk == 1)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV;
                    break;
                }

                //缓存服务关闭,直接跳出
                if (DataOk == 3)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.cacheClose;
                    break;
                }
                //未开启配置,则线程不需要等待了,直接跳出
                if (avhresultState == 1)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.noAV;
                    break;
                }
                //航段信息解析失败,则线程不需要等待了,直接跳出
                if (avhresultState == 2)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.avAnalysis;
                    break;
                }
                //IBE无数据返回,则线程不需要等待了,直接跳出
                if (avhresultState == 3)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV;
                    break;
                }
                //IBE超时,则线程不需要等待了,直接跳出
                if (avhresultState == 4)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV;
                    break;
                }
                //代理人配置异常
                if (avhresultState == 6)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.avHeiPinAbnormity;
                    break;
                }
                #endregion
                #region 获取到了的情况(数据不一定正确,但是获取过程是完成了)

                if (avhresultState == 5 && DataOk == 2)
                {
                    if (dsData.Tables.Count < 7)
                    {
                        errorInfo += PbProject.Model.definitionParam.airSelectErrorShow.dataBaseError;
                    }
                    if (errorInfo == "")
                    {
                        ds = MatchBasis(avhresult, dsData, null, mCompanys, Traveltypes, ref nums, middlecity, mUser);
                    }
                    else
                    {
                        PageStr = errorInfo;
                    }
                    break;
                }
                #endregion
                Thread.Sleep(100);
            }
            return ds;
        }
        #endregion
        #region 接口使用

        /// <summary>
        /// 查询航班
        /// </summary>
        /// <param name="fcity">出发城市三字码</param>
        /// <param name="mcity">中转城市三字码</param>
        /// <param name="tcity">到达城市三字码</param> 
        /// <param name="time">出发时间</param> 
        /// <param name="Totime">往返与联程第二次出发时间</param>
        /// <param name="cairry">承运人</param>  
        /// <param name="type">类型 1为 单程往返，2联程</param>
        /// <param name="TravelType">行程类型</param>
        /// <param name="mEmployees">员工model</param> 
        /// <param name="mCompany">公司model</param> 
        /// <param name="cacheGUID">out 缓存guid 键</param>
        /// <returns></returns>
        public DataSet StartForInterface(PbProject.Model.definitionParam.SelectCityParams selectCityParams, out string cacheGUID)
        {
            fromcity = selectCityParams.fcity;
            middlecity = selectCityParams.mcity;
            tocity = selectCityParams.tcity;
            fromtime = selectCityParams.time;
            Totimes = selectCityParams.Totime;
            mUser = selectCityParams.mEmployees;
            mCompanys = selectCityParams.mCompany;
            cairrys = selectCityParams.cairry;
            Traveltypes = selectCityParams.TravelType;
            IsShowShare = selectCityParams.IsShowGX;

            //记录查询记录
            StartAirQueryInsertThread();

            //线程开始前先获取IBE接口数据


            dsIBE = getIBEDataSet(cairrys, fromcity, tocity, fromtime);

            //开始线程
            StartAVHThread();
            StartDataThread();
            //StartPolicyThread();
            DataSet ds = StartQueryForInterface(out cacheGUID);
            selectCityParams.num = nums;
            return ds;
        }

        /// <summary>
        /// 查询航班方法
        /// </summary>
        /// <param name="cacheGUID">out 缓存guid 键</param>
        private DataSet StartQueryForInterface(out string cacheGUID)
        {
            DataSet ds = null;
            cacheGUID = string.Empty;
            int x = 0;
            string errorInfo = "";
            while (true)
            {
                x++;
                #region 超过30秒的情况
                if (x > 300)
                {
                    //超过30秒,只要不是成功,无论是什么状态,都默认为超时
                    if (avhresultState != 5)
                    {
                        errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeTimeOut;
                        throw new Exception(errorInfo);
                    }
                    if (DataOk != 2)
                    {
                        errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.cacheTimeOut;
                        throw new Exception(errorInfo);
                    }
                    break;

                }
                #endregion
                #region 返回了状态,但是状态不合格,过滤掉

                //IBE或易行无数据返回,则线程不需要等待了,直接跳出
                if (DataOk == 1)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV;
                    throw new Exception(errorInfo);
                }

                //缓存服务关闭,直接跳出
                if (DataOk == 3)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.cacheClose;
                    throw new Exception(errorInfo);
                }
                //未开启配置,则线程不需要等待了,直接跳出
                if (avhresultState == 1)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.noAV;
                    throw new Exception(errorInfo);

                }
                //航段信息解析失败,则线程不需要等待了,直接跳出
                if (avhresultState == 2)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.avAnalysis;
                    throw new Exception(errorInfo);

                }
                //IBE无数据返回,则线程不需要等待了,直接跳出
                if (avhresultState == 3)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV;
                    throw new Exception(errorInfo);

                }
                //IBE超时,则线程不需要等待了,直接跳出
                if (avhresultState == 4)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV;
                    throw new Exception(errorInfo);

                }
                //代理人配置异常
                if (avhresultState == 6)
                {
                    errorInfo = PbProject.Model.definitionParam.airSelectErrorShow.avHeiPinAbnormity;
                    throw new Exception(errorInfo);

                }
                #endregion
                #region 获取到了的情况(数据不一定正确,但是获取过程是完成了)

                if (avhresultState == 5 && DataOk == 2)
                {

                    if (dsData.Tables.Count < 7)
                    {
                        errorInfo += PbProject.Model.definitionParam.airSelectErrorShow.dataBaseError;
                        throw new Exception(errorInfo);
                    }
                    if (errorInfo == "")
                    {
                        ds = OutDataSet(mCompanys, avhresult, Traveltypes, ref nums, middlecity, dsData, dsPolicy, mUser, out cacheGUID);
                    }
                    break;
                }
                #endregion
                Thread.Sleep(100);
            }
            return ds;
        }

        /// <summary>
        /// 查询航班，无排序
        /// </summary>
        /// <param name="NowCompany"></param>
        /// <param name="trastr"></param>
        /// <param name="trtype"></param>
        /// <param name="num"></param>
        /// <param name="tcity"></param>
        /// <param name="BasisDs"></param>
        /// <param name="PolicyDs"></param>
        /// <param name="cacheGUID">out 缓存guid 键</param>
        /// <returns></returns>
        public DataSet OutDataSet(PbProject.Model.User_Company NowCompany, string trastr, int trtype, ref int num, string tcity, DataSet BasisDs, DataSet PolicyDs, PbProject.Model.User_Employees mUser, out string cacheGUID)
        {
            string errorInfo = "";
            cacheGUID = string.Empty;
            //HttpContext.Current.Session["flyValue"] = new DataSet();
            //HttpContext.Current.Session["flyTable"] = new DataTable();
            //string jsonTable = "";
            DataSet dsAnalysisStr = null;
            //string chaceNameByGUID = "";
            if (trastr.Trim().Replace("", "") == "")
            {
                errorInfo = "当前航线无直飞";
                throw new Exception(errorInfo);
            }
            else
            {
                dsAnalysisStr = MatchBasis(trastr, BasisDs, PolicyDs, NowCompany, trtype, ref num, tcity, mUser);
                //基础数据临时存入缓存
                PbProject.WebCommon.Utility.Cache.CacheByNet pwucc = new PbProject.WebCommon.Utility.Cache.CacheByNet();
                cacheGUID = Guid.NewGuid().ToString();
                //过期时间设置为十分钟
                pwucc.SetCacheData(dsAnalysisStr, cacheGUID, 10);
                //jsonTable = PbProject.WebCommon.Utility.Encoding.JsonHelper.DataSetToJson(dsAnalysisStr);
            }
            //jsonTable = jsonTable + "infoSplit" + chaceNameByGUID + "infoSplit" + errorInfo;
            //if (ds.Tables.Count == 0)
            //{
            //    return "<font style=\"font-size:18px;font-weight:bold;color:red;\">当前航线无直飞</font>";
            //}
            //#region 用于排序
            //DataRow dr = null;
            //DataTable dt = ds.Tables[0].Clone();
            //for (int i = 0; i < ds.Tables.Count; i++)
            //{
            //    dr = ds.Tables[i].Rows[0];
            //    dt.ImportRow(dr);

            //}
            //#endregion

            //HttpContext.Current.Session["flyValue"] = ds;
            //HttpContext.Current.Session["flyTable"] = dt;

            //num = ds.Tables.Count;
            return dsAnalysisStr;

            //OutString(ds, NowCompany);
        }

        #endregion

        /// <summary>
        /// 获取IBE接口的数据 
        /// </summary>
        /// <param name="airCode"></param>
        /// <param name="fromCityCode"></param>
        /// <param name="toCityCode"></param>
        /// <param name="airTime"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public DataSet getIBEDataSet(string airCode, string fromCityCode, string toCityCode, string airTime)
        {
            DataSet ds = null;
            try
            {
                int falg = 0;
                if (BaseParams.KongZhiXiTong.Contains("|53|"))//开启自己的配置
                {
                    falg = 0;
                }
                if (BaseParams.KongZhiXiTong.Contains("|44|"))//开启IBE接口
                {
                    falg = 1;
                }
                //同时开启IBE和易行,优先易行
                if (BaseParams.KongZhiXiTong.Contains("|92|"))//开启易行接口
                {
                    falg = 2;
                }
                switch (falg)
                {
                    case 0:
                        break;
                    case 1://IBE
                        string FlyAir = "CTU";//出港城市
                        string changeDateAirTime = changeDate2(airTime);//转换后的时间
                        w_IBEWebservice.WebService1SoapClient ibeclient = new w_IBEWebservice.WebService1SoapClient();
                        ds = ibeclient.getBasicData(FlyAir, airCode, fromCityCode, toCityCode, changeDateAirTime, "0000");
                        break;
                    case 2://易行
                        w_YeeXingService.YeeXingSerivceSoapClient yeeXingClient = new w_YeeXingService.YeeXingSerivceSoapClient();
                        string _yeeXingAccout = "";
                        _yeeXingAccout = BaseParams.JieKouZhangHao.Split('|')[6].Split('^')[0];

                        ds = yeeXingClient.QueryFlight(_yeeXingAccout, fromCityCode, toCityCode, airTime, "", airCode);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                OnError("IBE,webservice获取报错", "IBE,webservice获取报错");
                ds = null;

            }
            return ds;
        }
        /// <summary>
        /// 记录查询线程
        /// </summary>
        private void StartAirQueryInsertThread()
        {
            ThreadPool.QueueUserWorkItem(AirQueryInsert);
        }
        /// <summary>
        /// 记录查询数据
        /// </summary>
        /// <param name="o"></param>
        private void AirQueryInsert(object o)
        {
            try
            {
                IHashObject parameter = new HashObject();
                parameter.Add("CpyNo", mCompanys.UninCode);
                parameter.Add("LoginName", mUser.LoginName);
                parameter.Add("FirstFromDate", fromtime);
                parameter.Add("SecondFromDate", Totimes);
                parameter.Add("FromCityCode", fromcity);
                parameter.Add("ToCityCode", tocity);
                parameter.Add("CarryCode", cairrys);
                parameter.Add("MidelCityCode", middlecity);
                parameter.Add("CreateTime", DateTime.Now);
                new BaseDataManage().CallMethod("Tb_Air_AirQuery", "Insert", null, new object[] { parameter });
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        /// <summary>
        /// 查询航班方法
        /// </summary>
        private void StartQuery()
        {
            int x = 0;
            string errorInfo = "";
            while (true)
            {
                x++;
                #region 超过30秒的情况
                if (x > 300)
                {
                    //超过30秒,只要不是成功,无论是什么状态,都默认为超时
                    if (avhresultState != 5)
                    {
                        errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.ibeTimeOut + "</font>";
                        PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    }
                    if (DataOk != 2)
                    {
                        errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.cacheTimeOut + "</font>";
                        PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    }
                    break;

                }
                #endregion
                #region 返回了状态,但是状态不合格,过滤掉

                //IBE或易行无数据返回,则线程不需要等待了,直接跳出
                if (DataOk == 1)
                {
                    errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV + "</font>";
                    PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    break;
                }

                //缓存服务关闭,直接跳出
                if (DataOk == 3)
                {
                    errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.cacheClose + "</font>";
                    PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    break;
                }
                //未开启配置,则线程不需要等待了,直接跳出
                if (avhresultState == 1)
                {
                    errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.noAV + "</font>";
                    PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    break;
                }
                //航段信息解析失败,则线程不需要等待了,直接跳出
                if (avhresultState == 2)
                {
                    errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.avAnalysis + "</font>";
                    PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    break;
                }
                //IBE无数据返回,则线程不需要等待了,直接跳出
                if (avhresultState == 3)
                {
                    errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV + "</font>";
                    PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    break;
                }
                //IBE超时,则线程不需要等待了,直接跳出
                if (avhresultState == 4)
                {
                    errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.ibeNoAV + "</font>";
                    PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    break;
                }
                //代理人配置异常
                if (avhresultState == 6)
                {
                    errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.avHeiPinAbnormity + "</font>";
                    PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    break;
                }
                #endregion
                #region 获取到了的情况(数据不一定正确,但是获取过程是完成了)

                if (avhresultState == 5 && DataOk == 2)
                {

                    if (dsData.Tables.Count < 7)
                    {
                        errorInfo += "<font style=\"font-size:18px;font-weight:bold;color:red;\">" + PbProject.Model.definitionParam.airSelectErrorShow.dataBaseError + "</font>";
                    }
                    if (errorInfo == "")
                    {

                        PageStr = OutString(mCompanys, avhresult, Traveltypes, ref nums, middlecity, dsData, dsPolicy, mUser);
                    }
                    else
                    {
                        PageStr = "infoSplit" + "infoSplit" + errorInfo;
                    }
                    break;
                }
                #endregion
                Thread.Sleep(100);
            }
        }
        /// <summary>
        /// 开始查询航班线程
        /// </summary>
        private void StartAVHThread()
        {
            ThreadPool.QueueUserWorkItem(StartAVHQuery);
        }
        /// <summary>
        /// 查询航班方法
        /// </summary>
        private void StartAVHQuery(object o)
        {
            avhresult = NewQueryInterface(fromcity, middlecity, tocity, fromtime, Totimes, cairrys, Traveltypes, dsIBE);
            avhresultState = 5;
            //未开启配置
            if (avhresult.Trim() == "1")
            {
                avhresultState = 1;
            }
            //航段信息解析失败
            if (avhresult.Trim() == "2")
            {
                avhresultState = 2;
            }
            //IBE无数据返回
            if (avhresult.Trim() == "3" || avhresult.Trim() == "")
            {
                avhresultState = 3;
            }
            //IBE超时
            if (avhresult.Trim() == "4")
            {
                avhresultState = 4;
            }
            //代理人配置异常
            if (avhresult.Trim() == "6")
            {
                avhresultState = 6;
            }
        }
        /// <summary>
        /// 开始查询基础数据线程
        /// </summary>
        private void StartDataThread()
        {
            ThreadPool.QueueUserWorkItem(StartDataQuery);
        }
        /// <summary>
        /// 查询基础数据方法
        /// </summary>
        private void StartDataQuery(object o)
        {

            int DataState = 0;
            int flag = 0;
            if (BaseParams.KongZhiXiTong.Contains("|53|"))//开启自己的配置
            {
                flag = 1;
            }
            if (BaseParams.KongZhiXiTong.Contains("|44|"))//开启IBE接口
            {
                flag = 2;
            }
            //IBE和易行同时开启,易行优先
            if (BaseParams.KongZhiXiTong.Contains("|92|"))//开启易行接口
            {
                flag = 3;
            }
            switch (flag)
            {
                case 0:
                    break;
                case 1:
                    dsData = Basis(fromcity, tocity, fromtime, ref DataState, ref cacheInfo);
                    break;
                case 2:
                    //IBE没有数据则不用去获取基础数据
                    if (dsIBE != null && dsIBE.Tables.Count != 1)
                    {
                        dsData = Basis(fromcity, tocity, fromtime, ref DataState, dsIBE, ref cacheInfo);
                    }
                    else
                    {
                        DataState = 1;
                    }
                    break;
                case 3:
                    if (dsIBE != null && dsIBE.Tables[0].Rows[0]["is_success"].ToString().ToUpper() == "T" && dsIBE.Tables.Count > 1)
                    {
                        dsData = BasisYeeXing(fromcity, tocity, fromtime, ref DataState, dsIBE, ref cacheInfo);
                    }
                    else
                    {
                        DataState = 1;
                    }
                    break;
                default:
                    break;
            }
            if (cacheInfo == "CacheClose")
            {
                DataState = 3;
            }
            DataOk = DataState;

        }
        /// <summary>
        /// 开始查询政策线程
        /// </summary>
        private void StartPolicyThread()
        {
            ThreadPool.QueueUserWorkItem(StartPolicyQuery);
        }
        /// <summary>
        /// 查询政策方法
        /// </summary>
        public void StartPolicyQuery(object o)
        {

            int IsOk = 0;
            //PiaoBao.BLLLogic.Policy.PolicyMatching Policy = PiaoBao.BLLLogic.Factory_Air.CreatePolicyMatching();
            //dsPolicy = Policy.CachePolicy(fromcity, middlecity, tocity, fromtime, Totimes, Traveltypes, mUser, types, out IsOk, mCompanys, true, 0);
            isPolicyOk = IsOk;
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="content">记录内容</param>
        /// <param name="method">方法</param>
        public void OnError(string content, string method)
        {
            //#region 记录数据日志
            //try
            //{
            //    content = (content == null) ? "" : content;
            //    method = (method == null) ? "" : method;
            //    string author = (mUser.UserName == null) ? "" : mUser.UserName;
            //    PiaoBao.Models.Log_Erorr erorr = new PiaoBao.Models.Log_Erorr
            //    {
            //        Author = author,
            //        ErorrTime = DateTime.Now,
            //        ErrorName = content,
            //        A1 = "new",
            //        Page = "PiaoBao.BLLLogic.Interface.AirQurey",
            //        Method = method
            //    };
            //    PiaoBao.BLLLogic.Factory_System.CreateLog_ErorrManager().InsertLog_Erorr(erorr);
            //}
            //catch { }
            //#endregion
        }
        /// <summary>
        /// 获取基础数据(纯读取缓存方式)
        /// </summary>
        /// <param name="FCitycode">出发城市三字码</param>
        /// <param name="TCitycode">到达城市三字码</param>
        /// <param name="StartDate">出发时间</param>
        /// <param name="State">状态 出入前赋值false，当为true时即为获取完毕</param>
        public DataSet Basis(string FCitycode, string TCitycode, string StartDate, ref int State, ref string CacheClose)
        {
            string CacheUrl = ConfigurationManager.AppSettings["CacheUrl"].ToString();
            DataSet dsTemp = CreateTableStructure();
            try
            {
                if (remoteobj == null)
                {
                    //ChannelServices.RegisterChannel(new TcpClientChannel());
                    remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), CacheUrl);
                }
                //下句代码测试缓存是否有连接,不要删除
                try
                {
                    remoteobj.GetServerTime();
                }
                catch (Exception)
                {
                    CacheClose = "CacheClose";
                }

                if (CacheClose == "")
                {
                    //获取舱位折扣
                    string[] CityList = new string[2];
                    CityList[0] = "PEK,NAY";
                    CityList[1] = "SHA,PVG";
                    string _FCitycode = "'" + FCitycode + "'";
                    string _TCitycode = "'" + TCitycode + "'";
                    for (int i = 0; i < CityList.Length; i++)
                    {
                        if (CityList[i].Contains(FCitycode.ToUpper()))
                        {
                            _FCitycode = "'" + CityList[i].Split(',')[0] + "','" + CityList[i].Split(',')[1] + "'";
                        }
                        if (CityList[i].Contains(TCitycode.ToUpper()))
                        {
                            _TCitycode = "'" + CityList[i].Split(',')[0] + "','" + CityList[i].Split(',')[1] + "'";
                        }
                    }
                    //获取全价和里程
                    string newcitywhere = " and FromCityCode in (" + _FCitycode + "," + _TCitycode + ") and ToCityCode in (" + _TCitycode + "," + _FCitycode + ")";
                    DataRow[] tempBd_Air_FaresDR = remoteobj.GetBd_Air_Fares(" EffectTime<='" + StartDate + "' and InvalidTime>='" + StartDate + "' " + newcitywhere).Tables[0].Select("1=1", "FromCityCode desc");
                    StringBuilder sbAllY = new StringBuilder("|");//存储Y舱舱位价
                    for (int i = 0; i < tempBd_Air_FaresDR.Length; i++)
                    {
                        //参考格式如下,后期代码如有变动,请参照实际值
                        //|Y^CA^1000|Y^MU^1100|  
                        //如果未获得对应承运人的Y舱价格,则读取
                        if (!sbAllY.ToString().ToUpper().Contains("^" + tempBd_Air_FaresDR[i]["CarryCode"].ToString() + "^"))
                        {
                            sbAllY.Append("Y^");
                            sbAllY.Append(tempBd_Air_FaresDR[i]["CarryCode"].ToString() + "^");
                            sbAllY.Append(tempBd_Air_FaresDR[i]["FareFee"].ToString() + "|");
                            DataRow tempRow = dsTemp.Tables["Bd_Air_Fares"].NewRow();
                            object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        tempBd_Air_FaresDR[i]["FareFee"].ToString(),
                                        tempBd_Air_FaresDR[i]["Mileage"].ToString(),
                                        tempBd_Air_FaresDR[i]["CarryCode"].ToString(),
                                        FCitycode,
                                        TCitycode
                                       };
                            tempRow.ItemArray = objs;
                            //写入全价里程表
                            dsTemp.Tables["Bd_Air_Fares"].Rows.Add(tempRow);

                        }
                    }
                    string CityW = " and FromCityCode in (''," + _FCitycode + ") and ToCityCode in (''," + _TCitycode + ")";
                    //获取舱位折扣表数据
                    DataTable tempBd_Air_CabinDiscountdt = remoteobj.GetBd_Air_CabinDiscount(" 1=1 " + CityW).Tables[0];
                    for (int i = 0; i < tempBd_Air_CabinDiscountdt.Rows.Count; i++)
                    {
                        DataRow tempRow1 = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();
                        decimal jiageD = 0M;
                        decimal.TryParse(tempBd_Air_CabinDiscountdt.Rows[i]["CabinPrice"].ToString(), out jiageD);
                        if (jiageD <= 0)
                        {
                            continue;
                        }
                        string zhekou1 = getDangQianChengYunZheKou(sbAllY, tempBd_Air_CabinDiscountdt.Rows[i]["Cabin"].ToString(), jiageD);
                        if (zhekou1 == "0")
                        {
                            continue;
                        }
                        object[] objs1 = { 
                                                dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Count+1,
                                                tempBd_Air_CabinDiscountdt.Rows[i]["Cabin"].ToString(),//舱位
                                                tempBd_Air_CabinDiscountdt.Rows[i]["AirCode"].ToString(),//承运人
                                                FCitycode,
                                                TCitycode,
                                                "2000-08-09 01:01:01",
                                                "2100-08-09 01:01:01",
                                                zhekou1,
                                                jiageD
                                               };
                        tempRow1.ItemArray = objs1;
                        //写入舱位折扣表
                        dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow1);
                    }



                    //获取缓存基础舱位
                    DataTable dsBd_Air_BaseCabinTempAll = remoteobj.GetBd_Air_BaseCabin(" 1=1 ").Tables[0];

                    for (int i = 0; i < dsBd_Air_BaseCabinTempAll.Rows.Count; i++)
                    {
                        string tempAirPortCode = dsBd_Air_BaseCabinTempAll.Rows[i]["AirCode"].ToString();
                        string tempSpace = dsBd_Air_BaseCabinTempAll.Rows[i]["Cabin"].ToString();

                        //如果舱位折扣表里没有对应的数据,则用基本舱位算法添加
                        DataRow[] tempdrs = dsTemp.Tables["Bd_Air_CabinDiscount"].Select("Cabin = '" + tempSpace + "' and AirCode = '" + tempAirPortCode + "'");
                        //如果缓存舱位在IBE数据中查询不到,则添加
                        if (tempdrs == null || tempdrs.Length == 0)
                        {
                            DataRow tempRow1 = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();
                            decimal jiageHCD = 0M;
                            string zhekou1 = "0";
                            zhekou1 = dsBd_Air_BaseCabinTempAll.Rows[i]["Rebate"].ToString();
                            if (zhekou1 == "0")
                            {
                                continue;
                            }
                            jiageHCD = decimal.Parse(getDangQianChengYunYunJia(sbAllY, tempAirPortCode, zhekou1));
                            if (jiageHCD <= 0)
                            {
                                continue;
                            }
                            object[] objs1 = { 
                                                 dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Count+1,
                                                dsBd_Air_BaseCabinTempAll.Rows[i]["Cabin"].ToString(),//舱位
                                                tempAirPortCode,//承运人
                                                FCitycode,
                                                TCitycode,
                                                "2000-08-09 01:01:01",
                                                "2100-08-09 01:01:01",
                                                zhekou1,
                                                jiageHCD
                                               };
                            tempRow1.ItemArray = objs1;
                            //写入舱位折扣表
                            dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow1);
                        }
                    }
                    //获取机建价格    
                    dsTemp.Merge(remoteobj.GetBd_Air_Aircraft(" 1=1 ").Tables[0]);
                    //获取燃油价格
                    dsTemp.Merge(remoteobj.GetBd_Air_Fuel(" StartTime<='" + StartDate + "' and EndTime>='" + StartDate + "' ").Tables[0]);
                    //获取承运人
                    DataTable Carrierdt = remoteobj.GetBd_Air_Carrier("1=1").Tables[0];
                    Carrierdt.TableName = "Carrier";
                    dsTemp.Merge(Carrierdt);
                    //获取城市
                    string CityWhere = " and ( CityCodeWord='" + FCitycode + "' or CityCodeWord='" + TCitycode + "'";
                    for (int i = 0; i < CityList.Length; i++)
                    {
                        if (CityList[i].Contains(FCitycode.ToUpper()))
                        {
                            CityWhere += " or CityCodeWord='" + CityList[i].Split(',')[0] + "' or CityCodeWord='" + CityList[i].Split(',')[1] + "'";
                        }
                        if (CityList[i].Contains(TCitycode.ToUpper()))
                        {
                            CityWhere += " or CityCodeWord='" + CityList[i].Split(',')[0] + "' or CityCodeWord='" + CityList[i].Split(',')[1] + "'";
                        }
                    }
                    CityWhere += ")";
                    dsTemp.Merge(remoteobj.GetBd_Air_Airport(" IsDomestic = 1" + CityWhere).Tables[0]);
                    //获取退改签规定
                    dsTemp.Merge(remoteobj.GetBd_Air_TGQProvision(" 1=1 ").Tables[0]);
                }
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("由于") != -1)
                {
                    remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), CacheUrl);
                }
                OnError(e.ToString(), "缓存");
            }
            State = 2;
            return dsTemp;


        }
        /// <summary>
        /// 获取基础数据(IBE接口+读取缓存的方式)
        /// </summary>
        /// <param name="FCitycode">出发城市三字码</param>
        /// <param name="TCitycode">到达城市三字码</param>
        /// <param name="StartDate">出发时间</param>
        /// <param name="State">状态 出入前赋值false，当为true时即为获取完毕</param>
        public DataSet Basis(string FCitycode, string TCitycode, string StartDate, ref int State, DataSet dsIBE, ref string CacheClose)
        {

            string CacheUrl = ConfigurationManager.AppSettings["CacheUrl"].ToString();

            DataSet dsTemp = CreateTableStructure();
            try
            {
                if (remoteobj == null)
                {
                    remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), CacheUrl);
                }
                //下句代码测试缓存是否有连接,不要删除
                try
                {
                    remoteobj.GetServerTime();
                }
                catch (Exception)
                {
                    CacheClose = "CacheClose";
                }

                if (CacheClose == "")
                {
                    //获取舱位折扣
                    string[] CityList = new string[2];
                    CityList[0] = "PEK,NAY";
                    CityList[1] = "SHA,PVG";
                    string _FCitycode = "'" + FCitycode + "'";
                    string _TCitycode = "'" + TCitycode + "'";
                    if (!dsIBE.Tables.Contains("errorTable"))//没有返回错误数据
                    {
                        #region 数据条数
                        int FltShopAVJourneyCount = dsIBE.Tables["FltShopAVJourney"].Rows.Count;
                        int FltShopAVOptCount = dsIBE.Tables["FltShopAVOpt"].Rows.Count;
                        int CabinAllCount = dsIBE.Tables["CabinAll"].Rows.Count;
                        int TaxAllCount = dsIBE.Tables["TaxAll"].Rows.Count;
                        int FltShopAVFltCount = dsIBE.Tables["FltShopAVFlt"].Rows.Count;
                        int FltShopPSCount = dsIBE.Tables["FltShopPS"].Rows.Count;
                        int RouteAllCount = dsIBE.Tables["RouteAll"].Rows.Count;
                        int FltShopNFareCount = dsIBE.Tables["FltShopNFare"].Rows.Count;
                        int FltShopPFareCount = dsIBE.Tables["FltShopPFare"].Rows.Count;
                        int FltShopRuleCount = dsIBE.Tables["FltShopRule"].Rows.Count;
                        #endregion
                        #region 获取所有承运人
                        StringBuilder chengyunren = new StringBuilder("|");

                        if (FltShopAVFltCount > 0)
                        {
                            for (int i = 0; i < FltShopAVFltCount; i++)
                            {
                                //参考格式如下,后期代码如有变动,请参照实际值
                                //|CA|MU|
                                string tempC = dsIBE.Tables["FltShopAVFlt"].Rows[i]["A1"].ToString();
                                //已经添加了承运人不再添加
                                if (!chengyunren.ToString().Contains("|" + tempC + "|"))
                                {
                                    chengyunren.Append(tempC);
                                    chengyunren.Append("|");
                                }

                            }
                        }
                        else
                        {
                            //获取承运人
                            DataTable CarrierdtS = remoteobj.GetBd_Air_Carrier(" 1=1 ").Tables[0];
                            int CarrierdtRowsCount = CarrierdtS.Rows.Count;
                            for (int i = 0; i < CarrierdtRowsCount; i++)
                            {
                                string tempC = CarrierdtS.Rows[i]["Code"].ToString();
                                //已经添加了承运人不再添加
                                if (!chengyunren.ToString().Contains("|" + tempC + "|"))
                                {
                                    chengyunren.Append(tempC);
                                    chengyunren.Append("|");
                                }
                            }

                        }
                        #endregion
                        #region 获取 Y舱位(全价舱),承运人,价格
                        StringBuilder YPirce = new StringBuilder("|");
                        for (int i = 0; i < FltShopPFareCount; i++)
                        {
                            if (dsIBE.Tables["FltShopPFare"].Rows[i]["A2"].ToString().ToUpper() == "Y")
                            {
                                //参考格式如下,后期代码如有变动,请参照实际值
                                //|Y^CA^1000|Y^MU^1100|  
                                YPirce.Append("Y");
                                YPirce.Append("^");
                                YPirce.Append(dsIBE.Tables["FltShopPFare"].Rows[i]["A1"].ToString());
                                YPirce.Append("^");
                                YPirce.Append(dsIBE.Tables["FltShopPFare"].Rows[i]["A3"].ToString());
                                YPirce.Append("|");
                                //Y舱的价格写入  全价和里程
                                DataRow tempRow = dsTemp.Tables["Bd_Air_Fares"].NewRow();
                                object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        dsIBE.Tables["FltShopPFare"].Rows[i]["A3"].ToString(),
                                        dsIBE.Tables["FltShopPFare"].Rows[i]["A3"].ToString(),
                                        dsIBE.Tables["FltShopPFare"].Rows[i]["A1"].ToString(),
                                        //dsIBE.Tables["FltShopPFare"].Rows[i]["A5"].ToString(),
                                        FCitycode,
                                        //dsIBE.Tables["FltShopPFare"].Rows[i]["A6"].ToString()
                                        TCitycode
                                       };
                                tempRow.ItemArray = objs;
                                //写入全价里程表
                                dsTemp.Tables["Bd_Air_Fares"].Rows.Add(tempRow);
                            }
                        }
                        #endregion
                        #region 判断是否所有承运人的Y舱价格都已经取到,如果没有取到,则由缓存读取
                        string[] chengyunrenS = chengyunren.ToString().Split('|');
                        for (int i = 0; i < chengyunrenS.Length; i++)
                        {
                            if (chengyunrenS[i] == "")
                            {
                                continue;
                            }
                            //如果没有,则读取缓存
                            if (!YPirce.ToString().Contains("^" + chengyunrenS[i] + "^"))
                            {
                                YPirce.Append("Y");
                                YPirce.Append("^");
                                YPirce.Append(chengyunrenS[i]);//承运人
                                YPirce.Append("^");
                                string Yjiage = "";
                                Yjiage = getYSpacePirce(_FCitycode, _TCitycode, chengyunrenS[i], StartDate);
                                YPirce.Append(Yjiage);//价格读取缓存中的价格
                                YPirce.Append("|");

                                //没有Y舱的情况,   全价和里程表的数据补录

                                DataRow tempRow = dsTemp.Tables["Bd_Air_Fares"].NewRow();
                                object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        Yjiage,
                                        Yjiage,
                                        chengyunrenS[i],//承运人
                                        FCitycode,//"出发城市",
                                        TCitycode//"到达城市"
                                       };
                                tempRow.ItemArray = objs;
                                //写入全价里程表
                                dsTemp.Tables["Bd_Air_Fares"].Rows.Add(tempRow);
                            }
                        }
                        #endregion
                        #region  获取子舱位信息
                        StringBuilder childCabinInfoAll = new StringBuilder(",");
                        for (int i = 0; i < CabinAllCount; i++)//循环舱位
                        {
                            //如果有子舱位(位数大于2就为子舱位,如M1,W1)
                            if (dsIBE.Tables["CabinAll"].Rows[i]["A1"].ToString().Length == 2)
                            {
                                //获取承运人,拼接为字符串
                                for (int j = 0; j < FltShopAVFltCount; j++)
                                {
                                    if (dsIBE.Tables["CabinAll"].Rows[i]["A0"].ToString() == dsIBE.Tables["FltShopAVFlt"].Rows[j]["A0"].ToString())
                                    {
                                        //拼接为承运人加舱位,如,CAM1,CAW1,ZHM1,
                                        childCabinInfoAll.Append(dsIBE.Tables["FltShopAVFlt"].Rows[j]["A1"].ToString() + dsIBE.Tables["CabinAll"].Rows[i]["A1"].ToString() + ",");
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 获取IBE普通舱位
                        for (int i = 0; i < FltShopPFareCount; i++)
                        {
                            #region 子舱位由緩存讀取,不讀取IBE(有子舱位,父舱位也由缓存读取,因为IBE有子舱位时,父舱位价格有时会混乱)
                            string airAndCabin = dsIBE.Tables["FltShopPFare"].Rows[i]["A1"].ToString() + dsIBE.Tables["FltShopPFare"].Rows[i]["A2"].ToString();

                            if (childCabinInfoAll.ToString().Contains(airAndCabin))
                            {
                                //符合条件极为子舱位或对应主舱位,由缓存获取数据
                                continue;
                            }
                            #endregion
                            DataRow tempRow = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();
                            string jiage = dsIBE.Tables["FltShopPFare"].Rows[i]["A3"].ToString();//舱位价
                            if (jiage == "")
                            {
                                jiage = "0";
                            }
                            //当前舱位价格
                            decimal jiageD = decimal.Parse(jiage);
                            //获取当前承运人
                            string dangqianChengyunren = dsIBE.Tables["FltShopPFare"].Rows[i]["A1"].ToString();
                            //折扣等于舱位价除以Y舱价格
                            string zhekou = getDangQianChengYunZheKou(YPirce, dangqianChengyunren, jiageD);
                            object[] objs = { i+1,
                                        dsIBE.Tables["FltShopPFare"].Rows[i]["A2"],//舱位
                                        dangqianChengyunren,//承运人
                                        FCitycode,
                                        TCitycode,
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        zhekou,
                                        jiageD
                                       };
                            tempRow.ItemArray = objs;
                            //写入舱位折扣表
                            dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow);
                        }
                        #endregion
                        #region 获取IBE特价舱位
                        //for (int i = 0; i < FltShopNFareCount; i++)
                        //{
                        //    DataRow tempRow = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();
                        //    string jiage = dsIBE.Tables["FltShopNFare"].Rows[i]["A4"].ToString();//舱位价
                        //    if (jiage == "")
                        //    {
                        //        jiage = "0";
                        //    }
                        //    //当前舱位价格
                        //    decimal jiageD = decimal.Parse(jiage);
                        //    //获取当前承运人
                        //    string dangqianChengyunren = dsIBE.Tables["FltShopNFare"].Rows[i]["A1"].ToString();
                        //    //折扣等于舱位价除以Y舱价格
                        //    string zhekou = getDangQianChengYunZheKou(YPirce, dangqianChengyunren, jiageD);
                        //    object[] objs = { i+1,
                        //                    dsIBE.Tables["FltShopNFare"].Rows[i]["A2"],//舱位
                        //                    dsIBE.Tables["FltShopNFare"].Rows[i]["A1"],//承运人
                        //                    FCitycode,
                        //                    TCitycode,
                        //                    "2000-08-09 01:01:01",
                        //                    "2100-08-09 01:01:01",
                        //                    zhekou,
                        //                    jiageD
                        //                   };
                        //    tempRow.ItemArray = objs;
                        //    //写入舱位折扣表
                        //    dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow);
                        //}
                        #endregion
                        #region IBE有舱位但是舱位价格不全(如,有R舱,但是没有R舱的价格),则从缓存中获取合并
                        //循环所有承运人
                        string CityW = " and FromCityCode in (''," + _FCitycode + ") and ToCityCode in (''," + _TCitycode + ") ";
                        //获取缓存舱位
                        DataTable dsBd_Air_CabinDiscountTempAll = remoteobj.GetBd_Air_CabinDiscount(" 1=1 " + CityW).Tables[0];
                        //获取缓存基础舱位(如果舱位折扣表的数据仍然不全,则使用基本舱位折扣计算)
                        DataTable dsBd_Air_BaseCabinTempAll = remoteobj.GetBd_Air_BaseCabin(" 1=1 ").Tables[0];

                        //先用舱位折扣表的数据查找缺漏的舱位价
                        for (int i = 0; i < chengyunrenS.Length; i++)
                        {
                            if (chengyunrenS[i] == "")
                            {
                                continue;
                            }
                            string tempAirPortCode = chengyunrenS[i];
                            DataRow[] dsBd_Air_CabinDiscountTemp = dsBd_Air_CabinDiscountTempAll.Select("AirCode = '" + tempAirPortCode + "'", "FromCityCode desc");


                            for (int j = 0; j < dsBd_Air_CabinDiscountTemp.Length; j++)
                            {
                                string tempSpace = dsBd_Air_CabinDiscountTemp[j]["Cabin"].ToString();
                                //获取缓存舱位进行筛选
                                DataRow[] tempdrs = dsTemp.Tables["Bd_Air_CabinDiscount"].Select("Cabin = '" + tempSpace + "' and AirCode = '" + tempAirPortCode + "'");
                                //如果缓存舱位在IBE数据中查询不到,则添加
                                if (tempdrs == null || tempdrs.Length == 0)
                                {
                                    DataRow tempRow1 = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();

                                    string jiageHC = dsBd_Air_CabinDiscountTemp[j]["CabinPrice"].ToString();//价格
                                    if (jiageHC.Trim() == "")
                                    {
                                        jiageHC = "0";
                                    }
                                    decimal jiageHCD = 0m;
                                    string zhekou1 = "0";
                                    if (jiageHC == "0")
                                    {
                                        //如果舱位折扣表也缺失数据,则读取基本舱位表,根据基本舱位的折扣来计算
                                        DataRow[] dsBd_Air_BaseCabinTemp = dsBd_Air_BaseCabinTempAll.Select("AirCode = '" + tempAirPortCode + "' and Cabin='" + tempSpace + "'", "Rebate asc");
                                        //如果有多条同样航空公司同舱位的折扣,取第一个(正序排序的,即默认取低的折扣)
                                        if (dsBd_Air_BaseCabinTemp.Length > 0)
                                        {
                                            zhekou1 = dsBd_Air_BaseCabinTemp[0]["Rebate"].ToString();
                                            jiageHCD = decimal.Parse(getDangQianChengYunYunJia(YPirce, tempAirPortCode, zhekou1));
                                        }

                                    }
                                    else
                                    {
                                        jiageHCD = decimal.Parse(jiageHC);
                                        zhekou1 = getDangQianChengYunZheKou(YPirce, tempAirPortCode, jiageHCD); ;//折扣
                                    }
                                    object[] objs1 = { 
                                        dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Count+1,
                                        dsBd_Air_CabinDiscountTemp[j]["Cabin"].ToString(),//舱位
                                        dsBd_Air_CabinDiscountTemp[j]["AirCode"].ToString(),//承运人
                                        FCitycode,
                                        TCitycode,
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        zhekou1,
                                        jiageHCD
                                       };
                                    tempRow1.ItemArray = objs1;
                                    //写入舱位折扣表
                                    dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow1);
                                }
                            }
                        }
                        //再用基本舱位表的数据查找缺漏的舱位价
                        for (int i = 0; i < chengyunrenS.Length; i++)
                        {
                            if (chengyunrenS[i] == "")
                            {
                                continue;
                            }
                            string tempAirPortCode = chengyunrenS[i];
                            DataRow[] dsBd_Air_BaseCabinTemp = dsBd_Air_BaseCabinTempAll.Select("AirCode = '" + tempAirPortCode + "'", "Rebate asc");


                            for (int j = 0; j < dsBd_Air_BaseCabinTemp.Length; j++)
                            {
                                string tempSpace = dsBd_Air_BaseCabinTemp[j]["Cabin"].ToString();
                                //获取缓存舱位进行筛选
                                DataRow[] tempdrs = dsTemp.Tables["Bd_Air_CabinDiscount"].Select("Cabin = '" + tempSpace + "' and AirCode = '" + tempAirPortCode + "'");
                                //如果缓存舱位在IBE数据中查询不到,则添加
                                if (tempdrs == null || tempdrs.Length == 0)
                                {
                                    DataRow tempRow1 = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();
                                    decimal jiageHCD = 0m;
                                    string zhekou1 = "0";

                                    zhekou1 = dsBd_Air_BaseCabinTemp[j]["Rebate"].ToString();
                                    jiageHCD = decimal.Parse(getDangQianChengYunYunJia(YPirce, tempAirPortCode, zhekou1));

                                    object[] objs1 = { 
                                        dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Count+1,
                                        dsBd_Air_BaseCabinTemp[j]["Cabin"].ToString(),//舱位
                                        dsBd_Air_BaseCabinTemp[j]["AirCode"].ToString(),//承运人
                                        FCitycode,
                                        TCitycode,
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        zhekou1,
                                        jiageHCD
                                       };
                                    tempRow1.ItemArray = objs1;
                                    //写入舱位折扣表
                                    dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow1);
                                }
                            }
                        }



                        #endregion
                        #region 全部运价都没获取就读取缓存的
                        if (dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Count == 0)
                        {
                            string CityWs = " and FromCityCode in (''," + _FCitycode + ") and ToCityCode in (''," + _TCitycode + ")";
                            DataTable dt = remoteobj.GetBd_Air_CabinDiscount(" 1=1 " + CityWs).Tables[0];
                            DataRow[] dsBd_Air_CabinDiscountTemp = dt.Select("1=1", "FromCityCode desc");
                            int dtRowsCount = dsBd_Air_CabinDiscountTemp.Length;
                            for (int i = 0; i < dtRowsCount; i++)
                            {

                                //获取缓存舱位进行筛选
                                DataRow[] tempdrs = dsTemp.Tables["Bd_Air_CabinDiscount"].Select("Cabin = '" + dsBd_Air_CabinDiscountTemp[i]["Cabin"].ToString() + "' and AirCode = '" + dsBd_Air_CabinDiscountTemp[i]["AirCode"].ToString() + "'");
                                //如果缓存舱位在IBE数据中查询不到,则添加
                                if (tempdrs == null || tempdrs.Length == 0)
                                {
                                    DataRow tempRow = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();
                                    string jiageHC = dsBd_Air_CabinDiscountTemp[i]["CabinPrice"].ToString();//价格
                                    if (jiageHC.Trim() == "")
                                    {
                                        jiageHC = "0";
                                    }
                                    decimal jiageHCD = 0m;
                                    string zhekou1 = "0";
                                    if (jiageHC == "0")
                                    {
                                        //如果舱位折扣表也缺失数据,则读取基本舱位表,根据基本舱位的折扣来计算
                                        DataRow[] dsBd_Air_BaseCabinTemp = dsBd_Air_BaseCabinTempAll.Select("AirCode = '" + dsBd_Air_CabinDiscountTemp[i]["AirCode"].ToString() + "' and Cabin='" + dsBd_Air_CabinDiscountTemp[i]["Cabin"].ToString() + "'", "Rebate asc");
                                        //如果有多条同样航空公司同舱位的折扣,取第一个(正序排序的,即默认取低的折扣)
                                        zhekou1 = dsBd_Air_BaseCabinTemp[0]["Rebate"].ToString();
                                        jiageHCD = decimal.Parse(getDangQianChengYunYunJia(YPirce, dsBd_Air_CabinDiscountTemp[i]["AirCode"].ToString(), zhekou1));
                                    }
                                    else
                                    {
                                        jiageHCD = decimal.Parse(jiageHC);
                                        zhekou1 = getDangQianChengYunZheKou(YPirce, dsBd_Air_CabinDiscountTemp[i]["AirCode"].ToString(), jiageHCD); ;//折扣
                                    }


                                    object[] objs = { i+1,
                                        dsBd_Air_CabinDiscountTemp[i]["Cabin"].ToString(),
                                        dsBd_Air_CabinDiscountTemp[i]["AirCode"].ToString(),
                                        FCitycode,
                                        TCitycode,
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        zhekou1,
                                       jiageHCD
                                       };
                                    tempRow.ItemArray = objs;
                                    //写入舱位折扣表
                                    dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow);
                                }

                            }

                        }
                        #endregion

                        //获取机建价格   
                        dsTemp.Merge(remoteobj.GetBd_Air_Aircraft("1=1").Tables[0]);
                        decimal ranyou = 0m;
                        //获取燃油,取一条信息即可
                        for (int i = 0; i < TaxAllCount; i++)
                        {
                            if (dsIBE.Tables["TaxAll"].Rows[i]["A1"].ToString() == "YQ")
                            {
                                ranyou = decimal.Parse(dsIBE.Tables["TaxAll"].Rows[i]["A2"].ToString() == "" ? "0" : dsIBE.Tables["TaxAll"].Rows[i]["A2"].ToString());
                                if (ranyou <= 0m) continue;//避免燃油为0
                                DataRow tempRow = dsTemp.Tables["Bd_Air_Fuel"].NewRow();
                                object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        ranyou.ToString("f0"),
                                        ranyou.ToString("f0")
                                       };
                                tempRow.ItemArray = objs;
                                //写入燃油表
                                dsTemp.Tables["Bd_Air_Fuel"].Rows.Add(tempRow);
                                break;
                            }
                        }
                        //没有燃油加一条为0的燃油
                        if (dsTemp.Tables["Bd_Air_Fuel"].Rows.Count <= 0)
                        {
                            ranyou = 0m;
                            DataRow tempRow = dsTemp.Tables["Bd_Air_Fuel"].NewRow();
                            object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        ranyou.ToString("f0"),
                                        ranyou.ToString("f0")
                                       };
                            tempRow.ItemArray = objs;
                            //写入燃油表
                            dsTemp.Tables["Bd_Air_Fuel"].Rows.Add(tempRow);
                        }


                        //获取承运人
                        DataTable Carrierdt = remoteobj.GetBd_Air_Carrier(" 1=1 ").Tables[0];

                        Carrierdt.TableName = "Carrier";
                        dsTemp.Merge(Carrierdt);
                        //获取城市
                        string CityWhere = " and ( CityCodeWord='" + FCitycode + "' or CityCodeWord='" + TCitycode + "'";
                        for (int i = 0; i < CityList.Length; i++)
                        {
                            if (CityList[i].Contains(FCitycode.ToUpper()))
                            {
                                CityWhere += " or CityCodeWord='" + CityList[i].Split(',')[0] + "' or CityCodeWord='" + CityList[i].Split(',')[1] + "'";
                            }
                            if (CityList[i].Contains(TCitycode.ToUpper()))
                            {
                                CityWhere += " or CityCodeWord='" + CityList[i].Split(',')[0] + "' or CityCodeWord='" + CityList[i].Split(',')[1] + "'";
                            }
                        }
                        CityWhere += ")";
                        dsTemp.Merge(remoteobj.GetBd_Air_Airport(" IsDomestic = 1" + CityWhere).Tables[0]);
                        //获取退改签规定
                        dsTemp.Merge(remoteobj.GetBd_Air_TGQProvision(" 1=1 ").Tables[0]);
                    }
                    else
                    {
                        OnError(dsIBE.Tables["errorTable"].Rows[0][0].ToString(), "Basis IBE接口错误信息");
                    }

                }
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("由于") != -1)
                {
                    remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), CacheUrl);
                }
                OnError(e.Message, "Basis 通过IBE接口查询航班");
            }

            State = 2;
            return dsTemp;
        }
        /// <summary>
        /// 获取基础数据(易行接口+读取缓存的方式)
        /// </summary>
        /// <param name="FCitycode">出发城市三字码</param>
        /// <param name="TCitycode">到达城市三字码</param>
        /// <param name="StartDate">出发时间</param>
        /// <param name="State">状态 出入前赋值false，当为true时即为获取完毕</param>
        public DataSet BasisYeeXing(string FCitycode, string TCitycode, string StartDate, ref int State, DataSet dsIBE, ref string CacheClose)
        {

            string CacheUrl = ConfigurationManager.AppSettings["CacheUrl"].ToString();
            DataSet Cacheds = new DataSet();
            DataSet dsTemp = CreateTableStructure();
            try
            {
                if (remoteobj == null)
                {
                    remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), CacheUrl);
                }
                //下句代码测试缓存是否有连接,不要删除
                try
                {
                    remoteobj.GetServerTime();
                }
                catch (Exception)
                {
                    CacheClose = "CacheClose";
                }

                if (CacheClose == "")
                {
                    //获取舱位折扣
                    string[] CityList = new string[2];
                    CityList[0] = "PEK,NAY";
                    CityList[1] = "SHA,PVG";
                    string _FCitycode = "'" + FCitycode + "'";
                    string _TCitycode = "'" + TCitycode + "'";

                    #region 数据条数


                    int flightCount = dsIBE.Tables["flight"].Rows.Count;
                    int cabinCount = dsIBE.Tables["cabin"].Rows.Count;


                    #endregion
                    #region 获取所有承运人
                    StringBuilder chengyunren = new StringBuilder("|");

                    if (flightCount > 0)
                    {
                        for (int i = 0; i < flightCount; i++)
                        {
                            //参考格式如下,后期代码如有变动,请参照实际值
                            //|CA|MU|
                            string tempC = dsIBE.Tables["flight"].Rows[i]["airComp"].ToString();
                            //已经添加了承运人不再添加
                            if (!chengyunren.ToString().Contains("|" + tempC + "|"))
                            {
                                chengyunren.Append(tempC);
                                chengyunren.Append("|");
                            }

                        }
                    }
                    else
                    {
                        //获取承运人
                        DataTable CarrierdtS = remoteobj.GetBd_Air_Carrier(" 1=1 ").Tables[0];
                        int CarrierdtRowsCount = CarrierdtS.Rows.Count;
                        for (int i = 0; i < CarrierdtRowsCount; i++)
                        {
                            string tempC = CarrierdtS.Rows[i]["Code"].ToString();
                            //已经添加了承运人不再添加
                            if (!chengyunren.ToString().Contains("|" + tempC + "|"))
                            {
                                chengyunren.Append(tempC);
                                chengyunren.Append("|");
                            }
                        }

                    }
                    #endregion
                    #region 获取 Y舱位(全价舱),承运人,价格
                    StringBuilder YPirce = new StringBuilder("|");
                    for (int i = 0; i < cabinCount; i++)
                    {
                        if (dsIBE.Tables["cabin"].Rows[i]["cabinCode"].ToString().ToUpper() == "Y")
                        {
                            //参考格式如下,后期代码如有变动,请参照实际值
                            //|Y^CA^1000|Y^MU^1100|  
                            YPirce.Append("Y");
                            YPirce.Append("^");

                            string tempAirComp = "";
                            for (int j = 0; j < flightCount; j++)
                            {
                                if (dsIBE.Tables["cabin"].Rows[i]["cabins_Id"].ToString() == dsIBE.Tables["flight"].Rows[j]["flight_Id"].ToString())
                                {
                                    tempAirComp = dsIBE.Tables["flight"].Rows[j]["airComp"].ToString();
                                    break;
                                }
                            }

                            YPirce.Append(tempAirComp);
                            YPirce.Append("^");
                            YPirce.Append(dsIBE.Tables["cabin"].Rows[i]["ibePrice"].ToString());
                            YPirce.Append("|");
                            //Y舱的价格写入  全价和里程
                            DataRow tempRow = dsTemp.Tables["Bd_Air_Fares"].NewRow();
                            object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        dsIBE.Tables["cabin"].Rows[i]["ibePrice"].ToString(),
                                        dsIBE.Tables["cabin"].Rows[i]["ibePrice"].ToString(),
                                        tempAirComp,
                                        //dsIBE.Tables["FltShopPFare"].Rows[i]["A5"].ToString(),
                                        FCitycode,
                                        //dsIBE.Tables["FltShopPFare"].Rows[i]["A6"].ToString()
                                        TCitycode
                                       };
                            tempRow.ItemArray = objs;
                            //写入全价里程表
                            dsTemp.Tables["Bd_Air_Fares"].Rows.Add(tempRow);
                        }
                    }
                    #endregion
                    #region 判断是否所有承运人的Y舱价格都已经取到,如果没有取到,则由缓存读取
                    string[] chengyunrenS = chengyunren.ToString().Split('|');
                    for (int i = 0; i < chengyunrenS.Length; i++)
                    {
                        if (chengyunrenS[i] == "")
                        {
                            continue;
                        }
                        //如果没有,则读取缓存
                        if (!YPirce.ToString().Contains("^" + chengyunrenS[i] + "^"))
                        {
                            YPirce.Append("Y");
                            YPirce.Append("^");
                            YPirce.Append(chengyunrenS[i]);//承运人
                            YPirce.Append("^");
                            string Yjiage = "";
                            Yjiage = getYSpacePirce(_FCitycode, _TCitycode, chengyunrenS[i], StartDate);
                            YPirce.Append(Yjiage);//价格读取缓存中的价格
                            YPirce.Append("|");

                            //没有Y舱的情况,   全价和里程表的数据补录

                            DataRow tempRow = dsTemp.Tables["Bd_Air_Fares"].NewRow();
                            object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        Yjiage,
                                        Yjiage,
                                        chengyunrenS[i],//承运人
                                        FCitycode,//"出发城市",
                                        TCitycode//"到达城市"
                                       };
                            tempRow.ItemArray = objs;
                            //写入全价里程表
                            dsTemp.Tables["Bd_Air_Fares"].Rows.Add(tempRow);
                        }
                    }
                    #endregion
                    #region 获取IBE普通舱位
                    for (int i = 0; i < cabinCount; i++)
                    {
                        DataRow tempRow = dsTemp.Tables["Bd_Air_CabinDiscount"].NewRow();
                        string jiage = "0";
                        string dangqianChengyunren = "";
                        for (int j = 0; j < flightCount; j++)
                        {
                            if (dsIBE.Tables["flight"].Rows[j]["flight_Id"].ToString() == dsIBE.Tables["cabin"].Rows[i]["cabins_Id"].ToString())
                            {
                                //获取当前承运人
                                dangqianChengyunren = dsIBE.Tables["flight"].Rows[j]["airComp"].ToString();
                                break;
                            }
                        }
                        string tempCabin = dsIBE.Tables["cabin"].Rows[i]["cabinCode"].ToString();//舱位
                        jiage = dsIBE.Tables["cabin"].Rows[i]["ibePrice"].ToString();//舱位价        
                        if (jiage == "")
                        {
                            jiage = "0";
                        }
                        //当前舱位价格
                        decimal jiageD = decimal.Parse(jiage);

                        //折扣等于舱位价除以Y舱价格
                        string zhekou = getDangQianChengYunZheKou(YPirce, dangqianChengyunren, jiageD);
                        object[] objs = { i+1,
                                        tempCabin,//舱位
                                        dangqianChengyunren,//承运人
                                        FCitycode,
                                        TCitycode,
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        zhekou,
                                        jiageD
                                       };
                        tempRow.ItemArray = objs;
                        //写入舱位折扣表
                        dsTemp.Tables["Bd_Air_CabinDiscount"].Rows.Add(tempRow);
                    }
                    #endregion


                    //获取机建价格   
                    dsTemp.Merge(remoteobj.GetBd_Air_Aircraft("1=1").Tables[0]);
                    decimal ranyou = 0m;
                    string ranyouS = "0";
                    //获取燃油,取一条信息即可
                    try
                    {
                        ranyouS = dsIBE.Tables["result"].Rows[0]["oilFee"].ToString().Split('：')[1].ToString();
                    }
                    catch (Exception)
                    {

                    }
                    ranyou = decimal.Parse(ranyouS);
                    if (ranyou <= 0m)
                    {
                        // oilFee
                    }
                    else
                    {
                        DataRow tempRow = dsTemp.Tables["Bd_Air_Fuel"].NewRow();
                        object[] objs = {
                                    "2000-08-09 01:01:01",
                                    "2100-08-09 01:01:01",
                                    ranyou.ToString("f0"),
                                    ranyou.ToString("f0")
                                   };
                        tempRow.ItemArray = objs;
                        //写入燃油表
                        dsTemp.Tables["Bd_Air_Fuel"].Rows.Add(tempRow);
                    }


                    //没有燃油加一条为0的燃油
                    if (dsTemp.Tables["Bd_Air_Fuel"].Rows.Count <= 0)
                    {
                        ranyou = 0m;
                        DataRow tempRow = dsTemp.Tables["Bd_Air_Fuel"].NewRow();
                        object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        ranyou.ToString("f0"),
                                        ranyou.ToString("f0")
                                       };
                        tempRow.ItemArray = objs;
                        //写入燃油表
                        dsTemp.Tables["Bd_Air_Fuel"].Rows.Add(tempRow);
                    }


                    //获取承运人
                    DataTable Carrierdt = remoteobj.GetBd_Air_Carrier(" 1=1 ").Tables[0];

                    Carrierdt.TableName = "Carrier";
                    dsTemp.Merge(Carrierdt);
                    //获取城市
                    string CityWhere = " and ( CityCodeWord='" + FCitycode + "' or CityCodeWord='" + TCitycode + "'";
                    for (int i = 0; i < CityList.Length; i++)
                    {
                        if (CityList[i].Contains(FCitycode.ToUpper()))
                        {
                            CityWhere += " or CityCodeWord='" + CityList[i].Split(',')[0] + "' or CityCodeWord='" + CityList[i].Split(',')[1] + "'";
                        }
                        if (CityList[i].Contains(TCitycode.ToUpper()))
                        {
                            CityWhere += " or CityCodeWord='" + CityList[i].Split(',')[0] + "' or CityCodeWord='" + CityList[i].Split(',')[1] + "'";
                        }
                    }
                    CityWhere += ")";
                    dsTemp.Merge(remoteobj.GetBd_Air_Airport(" IsDomestic = 1" + CityWhere).Tables[0]);
                    //获取退改签规定
                    dsTemp.Merge(remoteobj.GetBd_Air_TGQProvision(" 1=1 ").Tables[0]);
                }
                else
                {
                    OnError(dsIBE.Tables["errorTable"].Rows[0][0].ToString(), "易行接口错误信息");
                }


            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("由于") != -1)
                {
                    remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), CacheUrl);
                }
                OnError(e.Message, "Basis 易行接口查询航班");
            }

            State = 2;
            return dsTemp;
        }


        /// <summary>
        /// 创建基础数据需要的表结构
        /// </summary>
        /// <returns></returns>
        private DataSet CreateTableStructure()
        {
            DataSet dsNew = new DataSet();
            //舱位折扣
            DataTable dtNew1 = new DataTable();
            dtNew1.TableName = "Bd_Air_CabinDiscount";
            DataColumn id = new DataColumn("id");//1
            DataColumn Cabin = new DataColumn("Cabin");//2
            DataColumn AirPortCode = new DataColumn("AirCode");//3
            DataColumn FromCityCode = new DataColumn("FromCityCode");//4
            DataColumn ToCityCode = new DataColumn("ToCityCode");//5
            DataColumn BeginTime = new DataColumn("BeginTime");//6
            DataColumn EndTime = new DataColumn("EndTime");//7
            DataColumn DiscountRate = new DataColumn("DiscountRate");//8
            DataColumn xsFee = new DataColumn("xsFee");//9
            dtNew1.Columns.Add(id);
            dtNew1.Columns.Add(Cabin);
            dtNew1.Columns.Add(AirPortCode);
            dtNew1.Columns.Add(FromCityCode);
            dtNew1.Columns.Add(ToCityCode);
            dtNew1.Columns.Add(BeginTime);
            dtNew1.Columns.Add(EndTime);
            dtNew1.Columns.Add(DiscountRate);
            dtNew1.Columns.Add(xsFee);
            //全价和里程
            DataTable dtNew2 = new DataTable();
            dtNew2.TableName = "Bd_Air_Fares";
            DataColumn EffectTime = new DataColumn("EffectTime");//1
            DataColumn InvalidTime = new DataColumn("InvalidTime");//2
            DataColumn FareFee = new DataColumn("FareFee");//3
            DataColumn Mileage = new DataColumn("Mileage");//4
            DataColumn CarryCode = new DataColumn("CarryCode");//5
            DataColumn FromCityCode2 = new DataColumn("FromCityCode");//6
            DataColumn ToCityCode2 = new DataColumn("ToCityCode");//7
            dtNew2.Columns.Add(EffectTime);
            dtNew2.Columns.Add(InvalidTime);
            dtNew2.Columns.Add(FareFee);
            dtNew2.Columns.Add(Mileage);
            dtNew2.Columns.Add(CarryCode);
            dtNew2.Columns.Add(FromCityCode2);
            dtNew2.Columns.Add(ToCityCode2);
            //机建价格    
            //DataTable dtNew3 = new DataTable();
            //dtNew3.TableName = "Bd_Air_Aircraft";
            //DataColumn ABFeeN = new DataColumn("ABFeeN");//1
            //DataColumn Aircraft = new DataColumn("Aircraft");//2
            //dtNew3.Columns.Add(ABFeeN);
            //dtNew3.Columns.Add(Aircraft);
            //燃油价格
            DataTable dtNew4 = new DataTable();
            dtNew4.TableName = "Bd_Air_Fuel";
            DataColumn StartTime = new DataColumn("StartTime");//1
            DataColumn EndTime4 = new DataColumn("EndTime");//2
            DataColumn LowAdultFee = new DataColumn("LowAdultFee");//3
            DataColumn ExceedAdultFee = new DataColumn("ExceedAdultFee");//4
            DataColumn LowChildFee = new DataColumn("LowChildFee");//5
            DataColumn ExceedChildFee = new DataColumn("ExceedChildFee");//6
            dtNew4.Columns.Add(StartTime);
            dtNew4.Columns.Add(EndTime4);
            dtNew4.Columns.Add(LowAdultFee);
            dtNew4.Columns.Add(ExceedAdultFee);
            dtNew4.Columns.Add(LowChildFee);
            dtNew4.Columns.Add(ExceedChildFee);

            //承运人
            //DataTable dtNew5 = new DataTable();
            //dtNew5.TableName = "Carrier";
            //DataColumn Code = new DataColumn("Code");//1
            //DataColumn ShortName = new DataColumn("ShortName");//2
            //DataColumn A1 = new DataColumn("A1");//3
            //DataColumn Type = new DataColumn("Type");//4

            //dtNew5.Columns.Add(Code);
            //dtNew5.Columns.Add(ShortName);
            //dtNew5.Columns.Add(A1);
            //dtNew5.Columns.Add(Type);

            //城市
            //DataTable dtNew6 = new DataTable();
            //dtNew6.TableName = "Bd_Base_City";
            //DataColumn City = new DataColumn("City");//1
            //DataColumn spell = new DataColumn("spell");//2
            //DataColumn Code6 = new DataColumn("Code");//3
            //DataColumn Countries = new DataColumn("Countries");//4
            //DataColumn type = new DataColumn("type");//5

            //dtNew6.Columns.Add(City);
            //dtNew6.Columns.Add(spell);
            //dtNew6.Columns.Add(Code6);
            //dtNew6.Columns.Add(Countries);
            //dtNew6.Columns.Add(type);

            //特价舱位信息
            //DataTable dtNew7 = new DataTable();
            //dtNew7.TableName = "ds";
            //DataColumn CarryCode7 = new DataColumn("CarryCode");//1
            //DataColumn Spaces = new DataColumn("Spaces");//2
            //DataColumn LogChangePrescript = new DataColumn("LogChangePrescript");//3
            //DataColumn DishonoredBillPrescript = new DataColumn("DishonoredBillPrescript");//4
            //DataColumn UpCabinPrescript = new DataColumn("UpCabinPrescript");//5

            //dtNew7.Columns.Add(CarryCode7);
            //dtNew7.Columns.Add(Spaces);
            //dtNew7.Columns.Add(LogChangePrescript);
            //dtNew7.Columns.Add(DishonoredBillPrescript);
            //dtNew7.Columns.Add(UpCabinPrescript);


            dsNew.Tables.Add(dtNew1);
            dsNew.Tables.Add(dtNew2);
            //dsNew.Tables.Add(dtNew3);
            dsNew.Tables.Add(dtNew4);
            //dsNew.Tables.Add(dtNew5);
            //dsNew.Tables.Add(dtNew6);
            //dsNew.Tables.Add(dtNew7);
            return dsNew;
        }
        /// <summary>
        /// 当前承运人的舱位的折扣
        /// </summary>
        /// <param name="YPirce">关联Y舱,承运人,Y舱价格的字符串</param>
        /// <param name="dangqianChengyunren">当前承运人</param>
        /// <param name="jiageD">当前舱位价格</param>
        /// <returns>折扣</returns>
        private string getDangQianChengYunZheKou(StringBuilder YPirce, string dangqianChengyunren, decimal jiageD)
        {
            string rs = "0";
            string[] AllY = YPirce.ToString().Split('|');
            string dangqianY = "";
            for (int j = 0; j < AllY.Length; j++)
            {
                if (AllY[j].ToUpper().Contains("^" + dangqianChengyunren.ToUpper() + "^"))
                {
                    dangqianY = AllY[j].Split('^')[2];
                    break;
                }
            }
            decimal dangqianYD;
            try
            {
                dangqianYD = decimal.Parse(dangqianY);
            }
            catch (Exception)
            {
                dangqianYD = jiageD;//转换有问题则默认为Y舱价格就是本身价格
            }
            if (dangqianYD != 0)
            {
                rs = (new PbProject.Logic.Pay.DataAction().FourToFiveNum((jiageD / dangqianYD), 4) * 100).ToString("f2");

            }
            return rs;

        }
        /// <summary>
        /// 根据折扣获取当前舱位价
        /// </summary>
        /// <param name="YPirce">Y舱价格</param>
        /// <param name="zhekou">折扣</param>
        /// <returns></returns>
        private string getDangQianChengYunYunJia(StringBuilder YPirce, string dangqianChengyunren, string zhekou)
        {
            string rs = "0";
            string[] AllY = YPirce.ToString().Split('|');
            string dangqianY = "";
            for (int j = 0; j < AllY.Length; j++)
            {
                if (AllY[j].ToUpper().Contains("^" + dangqianChengyunren.ToUpper() + "^"))
                {
                    dangqianY = AllY[j].Split('^')[2];
                    break;
                }
            }
            decimal dangqianYD;
            try
            {
                dangqianYD = decimal.Parse(dangqianY);
            }
            catch (Exception)
            {
                dangqianYD = 0m;//转换有问题则默认0
            }
            Pay.DataAction plpd = new Pay.DataAction();

            rs = plpd.MinusCeilTen(dangqianYD * decimal.Parse(zhekou) / 100).ToString("f0");

            return rs;
        }
        /// <summary>
        /// 查询航班，无排序
        /// </summary>
        /// <param name="NowCompany"></param>
        /// <param name="trastr"></param>
        /// <param name="trtype"></param>
        /// <param name="num"></param>
        /// <param name="tcity"></param>
        /// <param name="BasisDs"></param>
        /// <param name="PolicyDs"></param>
        /// <returns></returns>
        public string OutString(PbProject.Model.User_Company NowCompany, string trastr, int trtype, ref int num, string tcity, DataSet BasisDs, DataSet PolicyDs, PbProject.Model.User_Employees mUser)
        {
            string errorInfo = "";
            //HttpContext.Current.Session["flyValue"] = new DataSet();
            //HttpContext.Current.Session["flyTable"] = new DataTable();
            string jsonTable = "";
            string chaceNameByGUID = "";
            if (trastr.Trim().Replace("", "") == "")
            {
                errorInfo = "<font style=\"font-size:18px;font-weight:bold;color:red;\">当前航线无直飞</font>";
            }
            else
            {

                DataSet dsAnalysisStr = MatchBasis(trastr, BasisDs, PolicyDs, NowCompany, trtype, ref num, tcity, mUser);
                //基础数据临时存入缓存
                PbProject.WebCommon.Utility.Cache.CacheByNet pwucc = new WebCommon.Utility.Cache.CacheByNet();
                chaceNameByGUID = Guid.NewGuid().ToString();
                //过期时间设置为两分钟
                pwucc.SetCacheData(dsAnalysisStr, chaceNameByGUID, 2);
                jsonTable = PbProject.WebCommon.Utility.Encoding.JsonHelper.DataSetToJson(dsAnalysisStr);
            }
            jsonTable = jsonTable + "infoSplit" + chaceNameByGUID + "infoSplit" + errorInfo;
            //if (ds.Tables.Count == 0)
            //{
            //    return "<font style=\"font-size:18px;font-weight:bold;color:red;\">当前航线无直飞</font>";
            //}
            //#region 用于排序
            //DataRow dr = null;
            //DataTable dt = ds.Tables[0].Clone();
            //for (int i = 0; i < ds.Tables.Count; i++)
            //{
            //    dr = ds.Tables[i].Rows[0];
            //    dt.ImportRow(dr);

            //}
            //#endregion

            //HttpContext.Current.Session["flyValue"] = ds;
            //HttpContext.Current.Session["flyTable"] = dt;

            //num = ds.Tables.Count;
            return jsonTable;

            //OutString(ds, NowCompany);
        }
        /// <summary>
        /// 解析基础航班数据
        /// </summary>
        /// <param name="travstr"></param>
        /// <param name="BasisDs"></param>
        /// <param name="PolicyDs"></param>
        /// <param name="NowCompany"></param>
        /// <param name="trtype"></param>
        /// <param name="num"></param>
        /// <param name="tcity"></param>
        /// <param name="IsSecond"></param>
        /// <param name="mUser"></param>
        /// <returns></returns>
        public DataSet MatchBasis(string travstr, DataSet BasisDs, DataSet PolicyDs, PbProject.Model.User_Company NowCompany, int trtype, ref int num, string tcity, PbProject.Model.User_Employees mUser)
        {
            string begintime = DateTime.Now.ToString("mm:ss:fff");
            //航班数量(承运人加航班号确定一个航班)
            DataSet ds = AnalysisStr(travstr, BasisDs.Tables["Carrier"].Select(" SaleFlag=0"));


            List<Tb_SpecialCabin_PriceInfo> listTSP = new List<Tb_SpecialCabin_PriceInfo>();
            //开启使用特价缓存 从缓存取数据
            if (BaseParams.KongZhiXiTong.Contains("|99|"))
            {
                DateTime ftime = DateTime.Now;
                DateTime.TryParse(fromtime, out  ftime);
                listTSP = getCacheSpecialPrice(cairrys, ftime, fromcity, tocity);
            }

            if (ds != null)
            {
                num = ds.Tables.Count;
            }
            DataSet newds = new DataSet();
            //舱位折扣
            DataTable dtCabinDiscount = BasisDs.Tables["Bd_Air_CabinDiscount"];
            int tempI = ds.Tables.Count;
            PbProject.Logic.Pay.DataAction plpd = new PbProject.Logic.Pay.DataAction();
            for (int i = 0; i < tempI; i++)
            {
                #region 售罄
                if (ds.Tables[i].Rows.Count == 1 && ds.Tables[i].Rows[0]["TickNum"].ToString() == "--")
                {
                    ds.Tables[i].Rows[0]["XSFee"] = decimal.Parse("00");
                    ds.Tables[i].Rows[0]["SJFee"] = decimal.Parse("00");
                    ds.Tables[i].Rows[0]["FareFee"] = "--";
                    ds.Tables[i].Rows[0]["Mileage"] = "--";
                    ds.Tables[i].Rows[0]["ABFee"] = "--";
                    ds.Tables[i].Rows[0]["FuelAdultFee"] = "--";
                    ds.Tables[i].Rows[0]["FuelChildFee"] = "--";
                    ds.Tables[i].Rows[0]["DiscountRate"] = decimal.Parse("00");
                    ds.Tables[i].Rows[0]["DishonoredBillPrescript"] = "--";
                    ds.Tables[i].Rows[0]["LogChangePrescript"] = "--";
                    ds.Tables[i].Rows[0]["UpCabinPrescript"] = "--";
                    ds.Tables[i].Rows[0]["OldPolicy"] = "--";
                    ds.Tables[i].Rows[0]["GYPolicy"] = "--";
                    ds.Tables[i].Rows[0]["FXPolicy"] = "--";
                    ds.Tables[i].Rows[0]["PolicySource"] = "--";
                    ds.Tables[i].Rows[0]["PolicyId"] = "--";
                    ds.Tables[i].Rows[0]["PolicyType"] = "--";
                    ds.Tables[i].Rows[0]["Remark"] = "--";
                    //承运人
                    if (BasisDs.Tables["Carrier"].Select("Code = '" + ds.Tables[i].Rows[0]["CarrCode"].ToString() + "'").Length != 0)
                    {
                        ds.Tables[i].Rows[0]["Carrier"] = BasisDs.Tables["Carrier"].Select("Code = '" + ds.Tables[i].Rows[0]["CarrCode"].ToString() + "'")[0]["ShortName"].ToString();
                    }
                    //出发城市
                    if (BasisDs.Tables["Bd_Base_City"].Select("Code = '" + ds.Tables[i].Rows[0]["StartCityCode"].ToString() + "'").Length != 0)
                    {
                        ds.Tables[i].Rows[0]["FromCity"] = BasisDs.Tables["Bd_Base_City"].Select("Code = '" + ds.Tables[i].Rows[0]["StartCityCode"].ToString() + "'")[0]["City"].ToString();
                    }
                    //到达城市
                    if (BasisDs.Tables["Bd_Base_City"].Select("Code = '" + ds.Tables[i].Rows[0]["ToCityCode"].ToString() + "'").Length != 0)
                    {
                        ds.Tables[i].Rows[0]["ToCity"] = BasisDs.Tables["Bd_Base_City"].Select("Code = '" + ds.Tables[i].Rows[0]["ToCityCode"].ToString() + "'")[0]["City"].ToString();
                    }
                }
                #endregion
                # region 有座位
                else
                {
                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {

                        string FareFee = "0";//全价
                        string Mileage = "0";//里程
                        string ABFee = "0";//机建
                        string FuelAdultFee = "0";//成人燃油价
                        string FuelChildFee = "0";//儿童燃油价
                        decimal DiscountRate = 0.00m;//折扣
                        string DishonoredBillPrescript = "";//退票规定
                        string LogChangePrescript = "";//签转规定
                        string UpCabinPrescript = "";//升舱规定
                        decimal xs = 0.00m;//销售价
                        DataRow dr = ds.Tables[i].Rows[j];
                        #region 填充基础数据
                        //全价、里程
                        if (BasisDs.Tables["Bd_Air_Fares"].Rows.Count != 0)
                        {
                            DataRow[] drs = BasisDs.Tables["Bd_Air_Fares"].Select("CarryCode = '" + dr["CarrCode"].ToString() + "' and FromCityCode='" + dr["StartCityCode"].ToString() + "' and ToCityCode='" + dr["ToCityCode"].ToString() + "'");
                            if (drs.Length <= 0)
                            {
                                drs = BasisDs.Tables["Bd_Air_Fares"].Select("CarryCode = ''and FromCityCode='" + dr["StartCityCode"].ToString() + "' and ToCityCode='" + dr["ToCityCode"].ToString() + "'");
                            }
                            if (drs.Length <= 0)
                            {
                                drs = BasisDs.Tables["Bd_Air_Fares"].Select("CarryCode = '" + dr["CarrCode"].ToString() + "'and FromCityCode='" + dr["ToCityCode"].ToString() + "' and ToCityCode='" + dr["StartCityCode"].ToString() + "'");
                            }
                            if (drs.Length <= 0)
                            {
                                drs = BasisDs.Tables["Bd_Air_Fares"].Select("CarryCode = ''and FromCityCode='" + dr["ToCityCode"].ToString() + "' and ToCityCode='" + dr["StartCityCode"].ToString() + "'");
                            }
                            if (drs.Length > 0)
                            {
                                FareFee = drs[0]["FareFee"].ToString();
                                Mileage = drs[0]["Mileage"].ToString();
                            }
                        }
                        //机建
                        if (BasisDs.Tables["Bd_Air_Aircraft"].Rows.Count != 0)
                        {
                            DataRow[] drs = BasisDs.Tables["Bd_Air_Aircraft"].Select("Aircraft='" + dr["Model"].ToString() + "'");
                            if (drs.Length > 0)
                            {
                                ABFee = drs[0]["ABFeeN"].ToString();
                            }
                            else
                            {
                                ABFee = "50";
                            }
                        }
                        //燃油
                        if (BasisDs.Tables["Bd_Air_Fuel"].Rows.Count != 0)
                        {
                            if (int.Parse(decimal.Parse(Mileage).ToString("f0")) < 800)
                            {
                                FuelAdultFee = BasisDs.Tables["Bd_Air_Fuel"].Rows[0]["LowAdultFee"].ToString();
                                FuelChildFee = BasisDs.Tables["Bd_Air_Fuel"].Rows[0]["LowChildFee"].ToString();
                            }
                            else
                            {
                                FuelAdultFee = BasisDs.Tables["Bd_Air_Fuel"].Rows[0]["ExceedAdultFee"].ToString();
                                FuelChildFee = BasisDs.Tables["Bd_Air_Fuel"].Rows[0]["ExceedChildFee"].ToString();
                            }
                        }
                        //舱位折扣、客规
                        decimal xsFeeNow = 0.00m;//用来保存IBE中时时读取
                        if (FareFee != "0")
                        {
                            if (dtCabinDiscount.Rows.Count != 0)
                            {
                                DataRow[] drs = dtCabinDiscount.Select("Cabin='" + dr["Space"] + "' and AirCode = '" + dr["CarrCode"].ToString() + "' and FromCityCode = '" + dr["StartCityCode"].ToString() + "' and ToCityCode='" + dr["ToCityCode"].ToString() + "'");
                                DataRow drCabinDiscount = RowTimeClear(drs, dr["StartDate"].ToString());
                                if (drCabinDiscount == null)
                                {
                                    drs = dtCabinDiscount.Select("Cabin='" + dr["Space"] + "' and AirCode = '" + dr["CarrCode"].ToString() + "' and FromCityCode = '' and ToCityCode=''");
                                    drCabinDiscount = RowTimeClear(drs, dr["StartDate"].ToString());
                                }
                                if (drCabinDiscount != null)
                                {
                                    DiscountRate = decimal.Parse(drCabinDiscount["DiscountRate"].ToString());
                                    try
                                    {
                                        xsFeeNow = decimal.Parse(drCabinDiscount["xsFee"].ToString());// 非IBE方式无此字段,默认为0
                                    }
                                    catch (Exception)
                                    {
                                        xsFeeNow = 0.00m;
                                    }

                                }
                                drs = BasisDs.Tables["Bd_Air_TGQProvision"].Select("Spaces like '%" + dr["Space"] + "%' and CarryCode = '" + dr["CarrCode"].ToString() + "'");
                                if (drs.Length > 0)
                                {
                                    drCabinDiscount = drs[0];
                                    DishonoredBillPrescript = drs[0]["DishonoredBillPrescript"].ToString();
                                    LogChangePrescript = drs[0]["LogChangePrescript"].ToString();
                                    UpCabinPrescript = drs[0]["UpCabinPrescript"].ToString();
                                }
                            }
                        }
                        //承运人
                        if (BasisDs.Tables["Carrier"].Select("Code = '" + dr["CarrCode"].ToString() + "'").Length != 0)
                        {
                            ds.Tables[i].Rows[j]["Carrier"] = BasisDs.Tables["Carrier"].Select("Code = '" + dr["CarrCode"].ToString() + "'")[0]["ShortName"].ToString();
                        }
                        //出发城市
                        if (BasisDs.Tables["Bd_Air_Airport"].Select("CityCodeWord = '" + dr["StartCityCode"].ToString() + "'").Length != 0)
                        {
                            ds.Tables[i].Rows[j]["FromCity"] = BasisDs.Tables["Bd_Air_Airport"].Select("CityCodeWord = '" + dr["StartCityCode"].ToString() + "'")[0]["CityName"].ToString();
                        }
                        //到达城市
                        if (BasisDs.Tables["Bd_Air_Airport"].Select("CityCodeWord = '" + dr["ToCityCode"].ToString() + "'").Length != 0)
                        {
                            ds.Tables[i].Rows[j]["ToCity"] = BasisDs.Tables["Bd_Air_Airport"].Select("CityCodeWord = '" + dr["ToCityCode"].ToString() + "'")[0]["CityName"].ToString();
                        }
                        if (xsFeeNow != 0)//如果能取到IBE的实时运价就用运价,如果特殊情况没有,就使用老方法(折扣乘Y舱价格)
                        {
                            xs = xsFeeNow;
                        }
                        else
                        {
                            xs = plpd.FourToFiveNum(decimal.Parse(FareFee) * DiscountRate / 100, 0);
                            xs = plpd.FourToFiveNum(xs / 10, 0) * 10;
                            //xs = Math.Round((decimal.Parse(FareFee) * DiscountRate / 100), 0);
                            //xs = Math.Round(xs / 10, MidpointRounding.AwayFromZero) * 10;
                        }

                        ds.Tables[i].Rows[j]["XSFee"] = xs;
                        ds.Tables[i].Rows[j]["PMFee"] = xs + decimal.Parse(ABFee) + decimal.Parse(FuelAdultFee);
                        ds.Tables[i].Rows[j]["FareFee"] = FareFee;
                        ds.Tables[i].Rows[j]["Mileage"] = Mileage;
                        ds.Tables[i].Rows[j]["ABFee"] = ABFee;
                        ds.Tables[i].Rows[j]["FuelAdultFee"] = FuelAdultFee;
                        ds.Tables[i].Rows[j]["FuelChildFee"] = FuelChildFee;
                        //ds.Tables[i].Rows[j]["DiscountRate"] = decimal.Parse(DiscountRate.ToString("f2"));
                        ds.Tables[i].Rows[j]["DiscountRate"] = plpd.FourToFiveNum(DiscountRate, 0);
                        ds.Tables[i].Rows[j]["DishonoredBillPrescript"] = DishonoredBillPrescript;
                        ds.Tables[i].Rows[j]["LogChangePrescript"] = LogChangePrescript;
                        ds.Tables[i].Rows[j]["UpCabinPrescript"] = UpCabinPrescript;
                        #endregion
                        //特价舱位
                        if (ds.Tables[i].Rows[j]["SpecialType"].ToString() == "1")
                        {
                            bool haveCacheSpecial = false;
                            for (int s = 0; s < listTSP.Count; s++)
                            {
                                if (listTSP[s].SpCabin.ToUpper() == ds.Tables[i].Rows[j]["Space"].ToString()
                                    && listTSP[s].SpAirCode.ToUpper() == ds.Tables[i].Rows[j]["CarrCode"].ToString()
                                    //添加航班号（2013-5-28）
                                    && 
                                    (listTSP[s].SpFlightCode!=null? listTSP[s].SpFlightCode.ToUpper():"")   == (ds.Tables[i].Rows[j]["FlightNo"] != DBNull.Value ? ds.Tables[i].Rows[j]["FlightNo"].ToString() : "")
                                    )
                                {
                                    ds.Tables[i].Rows[j]["DiscountRate"] = "-2";
                                    ds.Tables[i].Rows[j]["XSFee"] = listTSP[s].SpPrice;
                                    ds.Tables[i].Rows[j]["PMFee"] = listTSP[s].SpPrice + listTSP[s].SpABFare + listTSP[s].SpRQFare;
                                    //特价的客规暂时为空(不知道与普通舱位客规是否相同)
                                    ds.Tables[i].Rows[j]["DishonoredBillPrescript"] = "";
                                    ds.Tables[i].Rows[j]["LogChangePrescript"] = "";
                                    ds.Tables[i].Rows[j]["UpCabinPrescript"] = "";
                                    ds.Tables[i].Rows[j]["ABFee"] = listTSP[s].SpABFare;
                                    ds.Tables[i].Rows[j]["FuelAdultFee"] = listTSP[s].SpRQFare;
                                    haveCacheSpecial = true;
                                    break;
                                }
                            }
                            if (!haveCacheSpecial)
                            {
                                ds.Tables[i].Rows[j]["DiscountRate"] = -1;
                                ds.Tables[i].Rows[j]["XSFee"] = 0m;
                                ds.Tables[i].Rows[j]["PMFee"] = 0m;
                                //特价的客规暂时为空(不知道与普通舱位客规是否相同)
                                ds.Tables[i].Rows[j]["DishonoredBillPrescript"] = "";
                                ds.Tables[i].Rows[j]["LogChangePrescript"] = "";
                                ds.Tables[i].Rows[j]["UpCabinPrescript"] = "";
                            }

                        }
                    }
                }
                if (ds.Tables[i].Rows.Count != 0)
                {

                    DataTable sTempDt = new DataTable(ds.Tables[i].TableName);
                    DataView dv = ds.Tables[i].DefaultView;
                    dv.Sort = "DiscountRate";
                    //if (NowCompany.RoleType == 1 || NowCompany.IsPolicy == 1)
                    //{
                    //    dv.Sort = "SJFee";
                    //}
                    sTempDt = dv.ToTable();
                    sTempDt = NewDsByDt(sTempDt);
                    if (sTempDt.Rows.Count > 0)
                    {
                        newds.Tables.Add(sTempDt);
                    }

                }
                # endregion
            }
            //string endtime = DateTime.Now.ToString("mm:ss:fff");
            //OnError(begintime + "-" + endtime, "数据匹配时间");
            return newds;
        }

        public List<Tb_SpecialCabin_PriceInfo> getCacheSpecialPrice(string SpAirCode, DateTime FlyDate, string FromCityCode, string ToCityCode)
        {
            List<Tb_SpecialCabin_PriceInfo> list = new List<Tb_SpecialCabin_PriceInfo>();
            var plps = new PbProject.Logic.Policy.SpecialCabinPriceInfoBLL();
            list = plps.GetSpPrice(SpAirCode, FlyDate, FromCityCode, ToCityCode);
            return list;
        }
        /// <summary>
        /// 解析航线字符串构建航线ds集合
        /// </summary>
        /// <param name="travstr">航线字符串</param>
        /// <returns>DataSet</returns>
        public DataSet AnalysisStr(string travstr, DataRow[] DisPlayCarrDrs)
        {
            string[] Tras = travstr.Split('^');
            Tras = stringlist(Tras);
            DataSet ds = new DataSet();
            string DisPlayCarrStr = "";
            foreach (DataRow DisPlayCarrDr in DisPlayCarrDrs)
            {
                DisPlayCarrStr += DisPlayCarrDr["Code"].ToString() + ",";
            }
            //获取特价舱位(如果有特价舱位,则多添加一个对应的航班信息)
            string GCpyNo = mCompanys.UninCode.Substring(0, 12);//获取上级供应商或落地运营商的ID
            string sqlwhere = " 1=1 "
                + " and CpyNo='" + GCpyNo + "'";
            List<Tb_SpecialCabin> objList = Manage.CallMethod("Tb_SpecialCabin", "GetList", null, new object[] { sqlwhere }) as List<Tb_SpecialCabin>;

            //获取普通舱位(如果不是普通舱位,则不添加普通舱位)

            string sqlwhere1 = " 1=1 ";
            List<Bd_Air_BaseCabin> objListB = Manage.CallMethod("Bd_Air_BaseCabin", "GetList", null, new object[] { sqlwhere1 }) as List<Bd_Air_BaseCabin>;


            for (int i = 0; i < Tras.Length; i++)
            {//2011-1-12,09:55,11:05,MU,2342,F8A2Y9K9BSESHSLSMSNSRSSSVSTSWSXSGSQSU5ISZS*9,CTU,XIY,320,0,0,1,False,T1,T2
                DataTable dt = SingleAirlinedt(Tras[i], objList, objListB);
                if (dt.Rows.Count > 0)
                {
                    if (!DisPlayCarrStr.Contains(dt.Rows[0]["CarrCode"].ToString()))
                    {
                        ds.Tables.Add(dt);
                    }
                }
            }
            return ds;
        }
        /// <summary>
        /// 字符串排序
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        private string[] stringlist(string[] strs)
        {
            string[] strlist = new string[strs.Length];
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("time", typeof(DateTime));
            DataColumn dc2 = new DataColumn("index", typeof(int));
            dt.Columns.Add(dc);
            dt.Columns.Add(dc2);
            for (int i = 0; i < strs.Length; i++)
            {
                DataRow dr = dt.NewRow();
                string[] s1 = strs[i].Split(',');
                dr["time"] = DateTime.Parse(s1[1]);
                dr["index"] = i;
                dt.Rows.Add(dr);
            }
            DataTable newdt = dt.Clone();
            DataView dv = dt.DefaultView;
            dv.Sort = "time";
            newdt = dv.ToTable();
            for (int i = 0; i < newdt.Rows.Count; i++)
            {
                strlist[i] = strs[int.Parse(newdt.Rows[i]["index"].ToString())];
            }
            return strlist;
        }
        /// <summary>
        /// 构建单条航线字符串dt集合
        /// </summary>
        /// <param name="airlinestr">单条航线字符串</param>
        /// <param name="objList">特殊舱位</param>
        /// <param name="objListB">普通舱位</param>
        /// <returns>DataTable</returns>
        private DataTable SingleAirlinedt(string airlinestr, List<Tb_SpecialCabin> objList, List<Bd_Air_BaseCabin> objListB)
        {
            string[] strlist = airlinestr.Split(',');
            DataTable dt = CreateAirInfoTable(strlist[3] + strlist[4]);
            strlist[5] = Spaces(strlist[5].ToString().Trim().Replace("", ""));
            if (strlist[5] != "")
            {
                bool specialType = false;//是否有特价舱位
                bool baseType = false;//是否有普通舱位
                string[] spaces = strlist[5].Split('_')[0].Split('^');
                string[] ticknums = strlist[5].Split('_')[1].Split('^');
                for (int i = 0; i < spaces.Length; i++)
                {
                    //先判断是否有特价舱位,有的话多加一条
                    specialType = juageSpecialType(strlist[3], spaces[i], objList);
                    //判断是否是基础舱位,不是就添加基础舱位
                    baseType = juageBaseType(strlist[3], spaces[i], objListB);

                    if (baseType)
                    {
                        DataRow dr = dt.NewRow();
                        dr["SpecialType"] = 0;
                        dr["CarrCode"] = strlist[3];
                        dr["FlightNo"] = strlist[4];
                        dr["Model"] = strlist[8];
                        dr["Space"] = spaces[i];
                        dr["StartDate"] = strlist[0];
                        dr["StartCityCode"] = strlist[6];
                        dr["ToCityCode"] = strlist[7];
                        dr["StartTime"] = strlist[1];
                        dr["EndTime"] = strlist[2];
                        dr["TickNum"] = ticknums[i];
                        if (!strlist[4].Contains("*"))
                        {
                            if (strlist[9] == "1")
                            {
                                dr["IsStop"] = "经停";
                            }
                        }
                        dr["dterm"] = strlist[13];
                        dr["aterm"] = strlist[14];
                        dr["GUID"] = Guid.NewGuid();
                        dt.Rows.Add(dr);
                    }

                    if (specialType)
                    {
                        DataRow drTemp = dt.NewRow();
                        drTemp["SpecialType"] = 1;//类型赋值为特价,暂时不分动态固态,待政策匹配后确定
                        drTemp["CarrCode"] = strlist[3];
                        drTemp["FlightNo"] = strlist[4];
                        drTemp["Model"] = strlist[8];
                        drTemp["Space"] = spaces[i];
                        drTemp["StartDate"] = strlist[0];
                        drTemp["StartCityCode"] = strlist[6];
                        drTemp["ToCityCode"] = strlist[7];
                        drTemp["StartTime"] = strlist[1];
                        drTemp["EndTime"] = strlist[2];
                        drTemp["TickNum"] = ticknums[i];
                        if (!strlist[4].Contains("*"))
                        {
                            if (strlist[9] == "1")
                            {
                                drTemp["IsStop"] = "经停";
                            }
                        }
                        drTemp["dterm"] = strlist[13];
                        drTemp["aterm"] = strlist[14];
                        drTemp["GUID"] = Guid.NewGuid();
                        dt.Rows.Add(drTemp);
                    }

                    if (strlist[15].Contains(spaces[i]))
                    {
                        if (baseType)
                        {
                            DataRow dr2 = dt.NewRow();
                            dr2["SpecialType"] = 0;
                            dr2["CarrCode"] = strlist[3];
                            dr2["FlightNo"] = strlist[4];
                            dr2["Model"] = strlist[8];
                            dr2["Space"] = spaces[i] + "1";
                            dr2["StartDate"] = strlist[0];
                            dr2["StartCityCode"] = strlist[6];
                            dr2["ToCityCode"] = strlist[7];
                            dr2["StartTime"] = strlist[1];
                            dr2["EndTime"] = strlist[2];
                            dr2["TickNum"] = ticknums[i];
                            if (!strlist[4].Contains("*"))
                            {
                                if (strlist[9] == "1")
                                {
                                    dr2["IsStop"] = "经停";
                                }
                            }
                            dr2["dterm"] = strlist[13];
                            dr2["aterm"] = strlist[14];
                            dr2["GUID"] = Guid.NewGuid();
                            dt.Rows.Add(dr2);
                        }
                        if (specialType)
                        {
                            DataRow drTemp = dt.NewRow();
                            drTemp["SpecialType"] = 1;//类型赋值为特价,暂时不分动态固态,待政策匹配后确定
                            drTemp["CarrCode"] = strlist[3];
                            drTemp["FlightNo"] = strlist[4];
                            drTemp["Model"] = strlist[8];
                            drTemp["Space"] = spaces[i] + "1";
                            drTemp["StartDate"] = strlist[0];
                            drTemp["StartCityCode"] = strlist[6];
                            drTemp["ToCityCode"] = strlist[7];
                            drTemp["StartTime"] = strlist[1];
                            drTemp["EndTime"] = strlist[2];
                            drTemp["TickNum"] = ticknums[i];
                            if (!strlist[4].Contains("*"))
                            {
                                if (strlist[9] == "1")
                                {
                                    drTemp["IsStop"] = "经停";
                                }
                            }
                            drTemp["dterm"] = strlist[13];
                            drTemp["aterm"] = strlist[14];
                            drTemp["GUID"] = Guid.NewGuid();
                            dt.Rows.Add(drTemp);
                        }
                    }
                }
            }
            return dt;
        }
        /// <summary>
        /// 判断特价舱位
        /// </summary>
        /// <param name="carryCode"></param>
        /// <param name="space"></param>
        /// <param name="objList"></param>
        /// <returns></returns>
        public bool juageSpecialType(string carryCode, string space, List<Tb_SpecialCabin> objList)
        {
            bool isOK = false;
            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i].SpAirCode.Contains(carryCode))//承运人
                {
                    if (objList[i].SpCabin.Contains(space))//舱位
                    {
                        isOK = true;
                        break;
                    }
                }
            }
            return isOK;
        }
        /// <summary>
        /// 判断特价舱位
        /// </summary>
        /// <param name="carryCode"></param>
        /// <param name="space"></param>
        /// <param name="objList"></param>
        /// <returns></returns>
        public bool juageBaseType(string carryCode, string space, List<Bd_Air_BaseCabin> objList)
        {
            bool isOK = false;
            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i].AirCode.Contains(carryCode))//承运人
                {
                    if (objList[i].Cabin.Contains(space))//舱位
                    {
                        isOK = true;
                        break;
                    }
                }
            }
            return isOK;
        }

        /// <summary>
        /// 分解舱位
        /// </summary>
        /// <param name="space">舱位字符串</param>
        /// <returns></returns>
        private string Spaces(string space)
        {
            string s = "";
            string ss = "";
            string sss = "";
            for (int i = 0; i < (space.Length / 2); i++)
            {
                string newstr1 = "a123456789";
                string newstr2 = space.Substring((i * 2), 2).Substring(1);
                if (newstr1.Contains(newstr2.ToLower()))
                {
                    ss += space.Substring(i * 2, 2).Substring(0, 1) + "^";
                    if (space.Substring(i * 2, 2).Substring(1, 1).ToLower() == "a")
                    {
                        sss += ">9^";
                    }
                    else
                    {
                        sss += space.Substring(i * 2, 2).Substring(1, 1) + "^";
                    }
                }
            }
            if (ss != "" && sss != "")
            {
                ss = ss.Substring(0, ss.Length - 1);
                sss = sss.Substring(0, sss.Length - 1);
                s = ss + "_" + sss;
            }
            else
            {
                s = "--_--";
            }
            return s;
        }
        /// <summary>
        /// 生效失效时间
        /// </summary>
        /// <param name="drs"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        private DataRow RowTimeClear(DataRow[] drs, string dates)
        {
            DataRow dr = null;
            for (int i = 0; i < drs.Length; i++)
            {
                if (DateTime.Parse(drs[i]["BeginTime"].ToString()) <= DateTime.Parse(dates) && DateTime.Parse(drs[i]["EndTime"].ToString()) >= DateTime.Parse(dates))
                {
                    dr = drs[i];
                    break;
                }
            }
            return dr;
        }
        /// <summary>
        /// 去除舱位折扣为0的航线
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable NewDsByDt(DataTable dt)
        {
            DataTable newdt = dt.Clone();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                if (dt.Rows[j]["DiscountRate"].ToString() != "0")
                {
                    DataRow newdr = newdt.NewRow();
                    for (int z = 0; z < dt.Columns.Count; z++)
                    {
                        newdr[z] = dt.Rows[j][z].ToString();
                    }
                    newdt.Rows.Add(newdr);
                }
            }

            return newdt;
        }
        /// <summary>
        /// 创建一个航班信息的表
        /// 名字为承运人加航班号(如:3U8881)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable CreateAirInfoTable(string tableName)
        {

            DataTable dt = new DataTable(tableName);
            //航线对应GUID
            DataColumn dcGUID = new DataColumn("GUID");
            dt.Columns.Add(dcGUID);
            //承运人代码
            DataColumn dcCarrCode = new DataColumn("CarrCode");
            dt.Columns.Add(dcCarrCode);
            //航班号
            DataColumn dcFlightNo = new DataColumn("FlightNo");
            dt.Columns.Add(dcFlightNo);
            //机型
            DataColumn dcModel = new DataColumn("Model");
            dt.Columns.Add(dcModel);
            //舱位
            DataColumn dcSpace = new DataColumn("Space");
            dt.Columns.Add(dcSpace);
            //出发日期
            DataColumn dcStartDate = new DataColumn("StartDate");
            dt.Columns.Add(dcStartDate);
            //出发城市代码
            DataColumn dcStartCityCode = new DataColumn("StartCityCode");
            dt.Columns.Add(dcStartCityCode);
            //到达城市代码
            DataColumn dcToCityCode = new DataColumn("ToCityCode");
            dt.Columns.Add(dcToCityCode);
            //起飞时间
            DataColumn dcStartTime = new DataColumn("StartTime");
            dt.Columns.Add(dcStartTime);
            //到达时间
            DataColumn dcEndTime = new DataColumn("EndTime");
            dt.Columns.Add(dcEndTime);
            //全价(既Y舱价格)
            DataColumn dc1 = new DataColumn("FareFee");
            dt.Columns.Add(dc1);
            //里程
            DataColumn dc2 = new DataColumn("Mileage");
            dt.Columns.Add(dc2);
            //机建
            DataColumn dc3 = new DataColumn("ABFee");
            dt.Columns.Add(dc3);
            //成人燃油
            DataColumn dc4 = new DataColumn("FuelAdultFee");
            dt.Columns.Add(dc4);
            //儿童燃油
            DataColumn dc5 = new DataColumn("FuelChildFee");
            dt.Columns.Add(dc5);
            //舱位折扣
            DataColumn dc6 = new DataColumn("DiscountRate", typeof(decimal));
            dt.Columns.Add(dc6);
            //座位数
            DataColumn dc7 = new DataColumn("TickNum");
            dt.Columns.Add(dc7);
            //承运人中文
            DataColumn dc8 = new DataColumn("Carrier");
            dt.Columns.Add(dc8);
            //出发城市中文
            DataColumn dc9 = new DataColumn("FromCity");
            dt.Columns.Add(dc9);
            //到达城市中文
            DataColumn dc10 = new DataColumn("ToCity");
            dt.Columns.Add(dc10);
            //舱位价格
            DataColumn dc11 = new DataColumn("XSFee");
            dt.Columns.Add(dc11);
            //退票规定
            DataColumn dc12 = new DataColumn("DishonoredBillPrescript");
            dt.Columns.Add(dc12);
            //签转规定
            DataColumn dc13 = new DataColumn("LogChangePrescript");
            dt.Columns.Add(dc13);
            //升舱规定
            DataColumn dc14 = new DataColumn("UpCabinPrescript");
            dt.Columns.Add(dc14);
            //实际支付  算过返点后加机建加燃油的价格
            DataColumn dc15 = new DataColumn("SJFee");
            dt.Columns.Add(dc15);
            //原始政策
            DataColumn dc16 = new DataColumn("OldPolicy");
            dt.Columns.Add(dc16);
            //供应政策
            DataColumn dc17 = new DataColumn("GYPolicy");
            dt.Columns.Add(dc17);
            //分销政策
            DataColumn dc18 = new DataColumn("FXPolicy");
            dt.Columns.Add(dc18);
            //政策来源
            DataColumn dc19 = new DataColumn("PolicySource");
            dt.Columns.Add(dc19);
            //政策编号
            DataColumn dc20 = new DataColumn("PolicyId");
            dt.Columns.Add(dc20);
            //政策类型
            DataColumn dc21 = new DataColumn("PolicyType");
            dt.Columns.Add(dc21);
            //备注
            DataColumn dc22 = new DataColumn("Remark");
            dt.Columns.Add(dc22);
            //经停
            DataColumn dc23 = new DataColumn("IsStop");
            dt.Columns.Add(dc23);
            //现返
            DataColumn dc24 = new DataColumn("ReturnMoney");
            dt.Columns.Add(dc24);
            //特价类型
            DataColumn dc25 = new DataColumn("SpecialType");
            dt.Columns.Add(dc25);
            //出发城市航站楼
            DataColumn dc26 = new DataColumn("dterm");
            dt.Columns.Add(dc26);
            //到达城市航站楼
            DataColumn dc27 = new DataColumn("aterm");
            dt.Columns.Add(dc27);
            //是否低开 1低开 0不低开（特价，接口默认低开，本地普通默认不低开）
            DataColumn dc28 = new DataColumn("IsLowerOpen");
            dt.Columns.Add(dc28);
            //c站政策值类型1点子2价格
            DataColumn dc29 = new DataColumn("PointType");
            dt.Columns.Add(dc29);
            //票面价 舱位价+机建+燃油
            DataColumn dc30 = new DataColumn("PMFee");
            dt.Columns.Add(dc30);
            //后返标示
            DataColumn dc31 = new DataColumn("A24");
            dt.Columns.Add(dc31);
            //高返标示
            DataColumn dc32 = new DataColumn("A21");
            //共享标示
            DataColumn dc33 = new DataColumn("IsShareFlight");
            dt.Columns.Add(dc33);
            //PAT内容
            DataColumn dcPatContent = new DataColumn("PatContent");
            dt.Columns.Add(dcPatContent);
            //PAT儿童内容
            DataColumn dcCHDPatContent = new DataColumn("CHDPatContent");
            dt.Columns.Add(dcCHDPatContent);
            //儿童出成人票时的儿童PAT内容          
            DataColumn dcChdToAdultPatCon = new DataColumn("ChdToAdultPatCon");
            dt.Columns.Add(dcChdToAdultPatCon);
            return dt;
        }
        /// <summary>
        /// 蒋航段信息存入缓存
        /// </summary>
        /// <param name="SkyList"></param>
        /// <returns></returns>
        public string SkyListSaveCache(OrderInputParam InputParam, string SpecialType)
        {
            List<Tb_Ticket_SkyWay> SkyList = InputParam.OrderParamModel[0].SkyList;
            Tb_Ticket_SkyWay CHDSky = null;
            //儿童出成人票 儿童PAT内容
            string ChdToAdultPatCon = string.Empty;

            foreach (OrderMustParamModel item in InputParam.OrderParamModel)
            {
                //如果为儿童订单
                if (item.Order.IsChdFlag)
                {
                    CHDSky = item.SkyList[0];
                    if (item.Order.IsCHDETAdultTK == 1)
                    {
                        //儿童PAT内容
                        ChdToAdultPatCon = item.Order.CHDToAdultPat;
                    }
                }
            }
            string chaceNameByGUID = "";
            if (SkyList != null && SkyList.Count > 0)
            {
                //订单是成人还是儿童 最终有一个订单
                OrderMustParamModel OM = null;
                //多个订单找成人订单
                if (InputParam.OrderParamModel.Count > 1)
                {
                    foreach (OrderMustParamModel item in InputParam.OrderParamModel)
                    {
                        if (!item.Order.IsChdFlag)
                        {
                            OM = item;
                            break;
                        }
                    }
                }
                else
                {
                    OM = InputParam.OrderParamModel[0];
                }
                //取成人还是儿童价格索引
                int index = OM.Order.IsChdFlag ? 1 : 0;
                //PAT价格列表 根据权限获取政策是否低开 取高价格，子仓位，低价格
                List<PnrAnalysis.PatInfo> PatList = new List<PnrAnalysis.PatInfo>();
                PatList = InputParam.PnrInfo.PatModelList[index] != null ? InputParam.PnrInfo.PatModelList[index].PatList : PatList;
                int PatListCount = 0;
                //判断是否P出了价格(大于0就是P出了价格)
                PatListCount = PatList.Count;
                DataSet ds = new DataSet();
                string tableName = "";
                foreach (Tb_Ticket_SkyWay sky in SkyList)
                {
                    tableName = sky.CarryCode + sky.FlightCode;
                    DataTable table = CreateAirInfoTable(tableName);
                    DataRow dr = table.NewRow();
                    table.Rows.Add(dr);
                    dr["SpecialType"] = SpecialType;
                    dr["CarrCode"] = sky.CarryCode.ToUpper();
                    dr["FlightNo"] = sky.FlightCode.ToUpper();
                    dr["Model"] = sky.Aircraft.ToUpper();
                    dr["Space"] = sky.Space.ToUpper();
                    dr["StartDate"] = sky.FromDate.ToString("yyyy-MM-dd");
                    dr["StartCityCode"] = sky.FromCityCode;
                    dr["ToCityCode"] = sky.ToCityCode;
                    dr["StartTime"] = sky.FromDate.ToString("HH:mm");
                    dr["EndTime"] = sky.ToDate.ToString("HH:mm");
                    dr["IsShareFlight"] = sky.IsShareFlight;
                    dr["PatContent"] = sky.Pat;
                    //儿童PAT价格
                    dr["CHDPatContent"] = CHDSky != null ? CHDSky.Pat : "";
                    //儿童出成人票时的儿童PAT价格
                    dr["ChdToAdultPatCon"] = ChdToAdultPatCon;
                    //Y舱价格 后面计算折扣可能用到
                    decimal FareFee = sky.FareFee;
                    dr["FareFee"] = sky.FareFee;
                    if (PatListCount == 0)
                    {
                        //没P出价格,则读取白屏查询的价格等信息
                        decimal PMFee = sky.SpacePrice + sky.ABFee + sky.FuelFee;
                        dr["XSFee"] = sky.SpacePrice;
                        dr["ABFee"] = sky.ABFee;
                        dr["FuelAdultFee"] = sky.FuelFee;
                        dr["PMFee"] = PMFee;
                        dr["DiscountRate"] = sky.Discount;

                    }
                    else
                    {
                        //对patModel 根据权限取价格 PatList价格从低到高排序
                        //根据权限去高价还是低价 还未处理

                        decimal SeatPrice = 0;//PAT出来的舱位价
                        decimal.TryParse(PatList[0].Fare, out SeatPrice);
                        dr["XSFee"] = SeatPrice;//PAT出来的舱位价
                        dr["ABFee"] = PatList[0].TAX;//PAT出来的机建
                        dr["FuelAdultFee"] = PatList[0].RQFare;//PAT出来的燃油
                        dr["PMFee"] = PatList[0].Price;//PAT出来的总价 舱位价+机建+燃油
                        dr["DiscountRate"] = FareFee != 0 ? (SeatPrice / FareFee) : 0;//计算折扣  
                    }
                    dr["GUID"] = Guid.NewGuid();
                    ds.Tables.Add(table);
                }
                //基础数据临时存入缓存
                PbProject.WebCommon.Utility.Cache.CacheByNet pwucc = new WebCommon.Utility.Cache.CacheByNet();
                chaceNameByGUID = Guid.NewGuid().ToString();
                //过期时间设置为两分钟
                pwucc.SetCacheData(ds, chaceNameByGUID, 2);
            }
            return chaceNameByGUID;
        }
        /// <summary>
        /// 获取舱位表航段价格
        /// </summary>
        /// <param name="sky"></param>
        /// <returns></returns>
        public Tb_Ticket_SkyWay GetSkyInfo(Tb_Ticket_SkyWay sky)
        {
            string CacheUrl = ConfigurationManager.AppSettings["CacheUrl"].ToString();
            DataSet Cacheds = new DataSet();
            try
            {
                if (remoteobj == null)
                {
                    remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), CacheUrl);
                }
                string sqlWhere = string.Format(" AirCode='{0}' and Cabin='{1}' and (FromCityCode ='' or FromCityCode='{2}' or FromCityCode is null) and (ToCityCode='' or ToCityCode='{3}' or ToCityCode is null)  and BeginTime<='{4}' and EndTime>='{4}'",
                sky.CarryCode, sky.Space, sky.FromCityCode, sky.ToCityCode, sky.FromDate.ToString("yyyy-MM-dd HH:mm:ss")
                );
                DataTable table = remoteobj.GetBd_Air_CabinDiscount(sqlWhere).Tables[0];
                if (table != null && table.Rows.Count > 0)
                {
                    decimal CabinPrice = -1m;
                    foreach (DataRow dr in table.Rows)
                    {
                        if (dr["FromCityCode"].ToString() != "" && dr["ToCityCode"].ToString() != "")
                        {
                            decimal.TryParse(dr["CabinPrice"].ToString(), out CabinPrice);
                            break;
                        }
                        else
                        {
                            decimal.TryParse(dr["CabinPrice"].ToString(), out CabinPrice);
                        }
                    }
                    sky.SpacePrice = CabinPrice;
                }
            }
            catch (Exception ex)
            {
                string mss = ex.Message;
            }
            return sky;
        }
        public string getYSpacePirce(string _FCitycode, string _TCitycode, string chengyunren, string StartDate)
        {
            string newcitywhere1 = " and FromCityCode = " + _FCitycode + " and ToCityCode = " + _TCitycode + " and carryCode='" + chengyunren + "'";
            //出发到达城市反过来再查询一次加承运人
            string newcitywhere2 = " and FromCityCode = " + _TCitycode + " and ToCityCode = " + _FCitycode + " and carryCode='" + chengyunren + "'";
            //查询出发到达城市的不加承运人
            string newcitywhere3 = " and FromCityCode = " + _FCitycode + " and ToCityCode = " + _TCitycode + " and carryCode='' ";
            //出发到达城市反过来再查询一次
            string newcitywhere4 = " and FromCityCode = " + _TCitycode + " and ToCityCode = " + _FCitycode + " and carryCode='' ";
            string Cjiage = "";

            DataTable dtBd_Air_Fares = new DataTable();
            dtBd_Air_Fares = remoteobj.GetBd_Air_Fares(" EffectTime<='" + StartDate + "' and InvalidTime>='" + StartDate + "' " + newcitywhere1).Tables[0];
            if (dtBd_Air_Fares == null || dtBd_Air_Fares.Rows.Count == 0)
            {
                dtBd_Air_Fares = remoteobj.GetBd_Air_Fares(" EffectTime<='" + StartDate + "' and InvalidTime>='" + StartDate + "' " + newcitywhere2).Tables[0];
            }
            if (dtBd_Air_Fares == null || dtBd_Air_Fares.Rows.Count == 0)
            {
                dtBd_Air_Fares = remoteobj.GetBd_Air_Fares(" EffectTime<='" + StartDate + "' and InvalidTime>='" + StartDate + "' " + newcitywhere3).Tables[0];
            }
            if (dtBd_Air_Fares == null || dtBd_Air_Fares.Rows.Count == 0)
            {
                dtBd_Air_Fares = remoteobj.GetBd_Air_Fares(" EffectTime<='" + StartDate + "' and InvalidTime>='" + StartDate + "' " + newcitywhere4).Tables[0];
            }
            if (dtBd_Air_Fares != null && dtBd_Air_Fares.Rows.Count > 0)
            {
                Cjiage = dtBd_Air_Fares.Rows[0]["FareFee"].ToString();
            }
            if (Cjiage == "")//如果没有读取到价格,则赋值为0
            {
                Cjiage = "0";
            }
            return Cjiage;
        }
    }

}
