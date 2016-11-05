using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
using PBPid.WebManage;

//自动更新航线舱位票价处理类

namespace ConsoleServerProc
{
    /// <summary>
    /// 需要传递的设置参数信息
    /// </summary>
    public class UpdatePriceWorkInfo
    {
        //发送指令使用配置参数
        public string serverip = "";
        public int serverport = 350;
        public string officenum = "";

        //自动更新周期
        //开启标志
        public bool autoupdateflag = false;
        //间隔天数（以完整完成一次更新后开始计算下次开始日期）
        public int autoupdatedays = 0;

        //出发城市限制
        public bool fromcityflag = false;
        public string fromcity = "";

        //到达城市限制
        public bool tocityflag = false;
        public string tocity = "";

        //服务开始结束时间限制
        public bool timeflag = false;
        public string beginendtime = "";

        //上次结束记录的出发到达城市id
        public string startfromtocityid = "";
        public string PreEndTime = "";

        //本轮更新是否完成
        public bool EndFlag = false;
        //本轮结束时间
        public DateTime EndTime = DateTime.Now;
    }

    /// <summary>
    /// FD结果类
    /// </summary>
    public class FDInfo
    {
        //出发城市
        public string FromCity = "";
        //到达城市
        public string ToCity = "";
        //里程
        public string Mileage = "0";
        //舱位价格列表
        public List<ClassPriceInfo> ClassPriceList = new List<ClassPriceInfo>();
        //航空公司全价列表
        public List<FullPriceInfo> FullPriceList = new List<FullPriceInfo>();
    }

    /// <summary>
    /// 舱位价格信息
    /// </summary>
    public class ClassPriceInfo
    {
        //承运人代码
        public string CarrierCode = "";
        //舱位代码
        public string ClassCode = "";
        //舱位价格
        public string ClassPrice = "";
        //舱位等级代码
        public string ClassLevel = "";
    }

    /// <summary>
    /// 全价信息
    /// </summary>
    public class FullPriceInfo
    {
        //承运人代码
        public string carrier = "";
        //全价价格
        public string fullPrice = "";
    }

    /// <summary>
    /// 自动更新舱位票价处理类
    /// </summary>
    public static class UpdateClassPriceProc
    {
        public static PbMain tmpFrm = null;

        //定时监测线程
        public static Thread AutoCheckThreadServer = null;

        //自动更新线程
        public static Thread UpdatePriceThreadServer = null;

        //传递参数信息类
        public static UpdatePriceWorkInfo m_UpdatePriceWorkInfo = new UpdatePriceWorkInfo();

        /// <summary>
        /// 开始更新处理线程及自动监测线程
        /// </summary>
        public static void StartServer()
        {
            //开始服务
            if (UpdatePriceThreadServer != null)
            {
                try
                {
                    UpdatePriceThreadServer.Abort();
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("UpdateClassPrice.log", ex, "StartServer：停止更新票价处理线程出错！");
                }
                UpdatePriceThreadServer = null;
            }

            //创建并启动处理线程
            //UpdatePriceThreadServer = new Thread(new ThreadStart(AutoUpdate));
            //UpdatePriceThreadServer.Start();

            try
            {
                if (AutoCheckThreadServer != null)
                {
                    AutoCheckThreadServer.Abort();
                }
            }
            catch (Exception ex)
            {
                //记录错误日志
                Log.Record("UpdateClassPrice.log", ex, "StartServer：停止自动监测线程出错！");
            }
            AutoCheckThreadServer = null;

            //自动监测线程
            AutoCheckThreadServer = new Thread(new ThreadStart(AutoCheck));
            AutoCheckThreadServer.Start();
        }

        /// <summary>
        /// 停止更新处理线程及自动监测线程
        /// </summary>
        public static void StopServer()
        {
            try
            {
                if (UpdatePriceThreadServer != null)
                {
                    UpdatePriceThreadServer.Abort();
                }
            }
            catch (Exception ex)
            {
                //记录错误日志
                Log.Record("UpdateClassPrice.log", ex, "StopServer：停止更新票价处理线程出错！");
            }
            UpdatePriceThreadServer = null;

            try
            {
                if (AutoCheckThreadServer != null)
                {
                    AutoCheckThreadServer.Abort();
                }
            }
            catch (Exception ex)
            {
                //记录错误日志
                Log.Record("UpdateClassPrice.log", ex, "StopServer：停止自动监测线程出错！");
            }
            AutoCheckThreadServer = null;
        }

        /// <summary>
        /// 更新票价处理
        /// </summary>
        public static void AutoUpdate()
        {
            string StartTime = m_UpdatePriceWorkInfo.beginendtime.Split('|')[0];
            string EndTime = m_UpdatePriceWorkInfo.beginendtime.Split('|')[1];

            while (true)
            {
                try
                {
                    #region 自动更新处理

                    WebManage.ServerIp = m_UpdatePriceWorkInfo.serverip;
                    WebManage.ServerPort = m_UpdatePriceWorkInfo.serverport;

                    //如果指定了出发到达城市，则直接调用更新价格
                    if ((m_UpdatePriceWorkInfo.fromcityflag) && (m_UpdatePriceWorkInfo.tocityflag))
                    {
                        UpdatePrice(m_UpdatePriceWorkInfo.fromcity, m_UpdatePriceWorkInfo.tocity);

                        m_UpdatePriceWorkInfo.EndFlag = true;
                        m_UpdatePriceWorkInfo.EndTime = DateTime.Now;

                        //调用label的invoke方法
                        tmpFrm.WriteLog("价格更新完毕！\r\n");

                        break;
                    }
                    else
                    {
                        #region 获取待更新的数据集

                        string tmpSQL = " 1=1 ";

                        //检查上次
                        if (m_UpdatePriceWorkInfo.startfromtocityid != "")
                        {
                            tmpSQL += " and id>='" + m_UpdatePriceWorkInfo.startfromtocityid + "' ";
                        }

                        //检查是否指定了出发城市
                        if (m_UpdatePriceWorkInfo.fromcityflag)
                        {
                            tmpSQL += " and FromCityCode='" + m_UpdatePriceWorkInfo.fromcity + "'";
                        }

                        //检查是否指定了到达城市
                        if (m_UpdatePriceWorkInfo.tocityflag)
                        {
                            tmpSQL += " and ToCityCode='" + m_UpdatePriceWorkInfo.tocity + "'";
                        }

                        tmpSQL += " order by id";

                        BaseDataManage Manage = new BaseDataManage(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Dal.dll");
                        List<Bd_Air_Fares> list = Manage.CallMethod("Bd_Air_Fares", "GetList", null, new object[] { tmpSQL }) as List<Bd_Air_Fares>;

                        #endregion 获取待更新的数据集

                        for (int i = 0; i < list.Count; i++)
                        {
                            //获取待操作的记录
                            Bd_Air_Fares tmpBd_Air_Fares = list[i];
                            //赋值当前操作的记录id
                            m_UpdatePriceWorkInfo.startfromtocityid = tmpBd_Air_Fares.id.ToString();

                            //出发城市
                            string tmpfromcity = tmpBd_Air_Fares.FromCityCode;
                            //到达城市
                            string tmptocity = tmpBd_Air_Fares.ToCityCode;

                            UpdatePrice(tmpfromcity, tmptocity);

                            #region 检查自动更新时间限制
                            if (m_UpdatePriceWorkInfo.timeflag)
                            {
                                //检查当前开始、结束时间
                                string tmpNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                string tmpStartTime = DateTime.Now.ToString("yyyy-MM-dd ") + StartTime;
                                string tmpEndTime = DateTime.Now.ToString("yyyy-MM-dd ") + EndTime;
                                if (!CheckIfInAutoUpdateTime(DateTime.Parse(tmpStartTime), DateTime.Parse(tmpEndTime)))
                                {
                                    //不在自动更新时间内，则退出线程处理
                                    m_UpdatePriceWorkInfo.PreEndTime = tmpNowTime;

                                    //调用label的invoke方法
                                    tmpFrm.WriteLog("更新到：" + tmpfromcity + tmptocity + "\r\n当前时间已经超过结束时间,中止更新等待下次自动更新！\r\n");

                                    break;
                                }
                            }
                            #endregion 检查自动更新时间限制
                        }
                        m_UpdatePriceWorkInfo.EndFlag = true;
                        m_UpdatePriceWorkInfo.EndTime = DateTime.Now;

                        //调用label的invoke方法
                        tmpFrm.WriteLog("价格更新完毕！\r\n");

                        break;
                    }

                    #endregion 自动更新处理

                    #region 检查自动更新时间限制
                    if (m_UpdatePriceWorkInfo.timeflag)
                    {
                        //检查当前开始、结束时间
                        string tmpNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        string tmpStartTime = DateTime.Now.ToString("yyyy-MM-dd ") + StartTime;
                        string tmpEndTime = DateTime.Now.ToString("yyyy-MM-dd ") + EndTime;
                        if (!CheckIfInAutoUpdateTime(DateTime.Parse(tmpStartTime), DateTime.Parse(tmpEndTime)))
                        {
                            //不在自动更新时间内，则退出线程处理

                            //调用label的invoke方法
                            tmpFrm.WriteLog("当前时间已经超过结束时间,中止更新等待下次自动更新！\r\n");

                            break;
                        }
                    }
                    #endregion 检查自动更新时间限制
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("UpdateClassPrice.log", ex, "AutoUpdate：自动更新票价线程处理出错！");
                }
            }
        }

        /// <summary>
        /// 自动监测线程
        /// </summary>
        public static void AutoCheck()
        {
            string StartTime = m_UpdatePriceWorkInfo.beginendtime.Split('|')[0];
            string EndTime = m_UpdatePriceWorkInfo.beginendtime.Split('|')[1];

            while (true)
            {
                try
                {
                    #region 如果已经开启自动更新周期并且上次更新已经完毕，则检查是否已到下次更新日期
                    if ((m_UpdatePriceWorkInfo.autoupdateflag) && (m_UpdatePriceWorkInfo.EndFlag))
                    {
                        if (DateTime.Parse(m_UpdatePriceWorkInfo.EndTime.ToString("yyyy-MM-dd ") +
                            m_UpdatePriceWorkInfo.beginendtime.Substring(0, m_UpdatePriceWorkInfo.beginendtime.IndexOf("|"))).
                            AddDays(m_UpdatePriceWorkInfo.autoupdatedays).CompareTo(DateTime.Now) <= 1)
                        {
                            //准备开始下次更新
                            m_UpdatePriceWorkInfo.EndTime = DateTime.Now;
                            m_UpdatePriceWorkInfo.EndFlag = false;
                            m_UpdatePriceWorkInfo.startfromtocityid = "";
                            if (UpdatePriceThreadServer != null)
                            {
                                //开始处理过程
                                try
                                {
                                    UpdatePriceThreadServer.Abort();
                                }
                                catch (Exception ex)
                                {
                                    Log.Record("UpdateClassPrice.log", ex, "AutoCheck：自动监测线程处理，释放更新票价线程出错1！");
                                }
                                UpdatePriceThreadServer = null;
                            }

                            //创建并启动处理线程
                            UpdatePriceThreadServer = new Thread(new ThreadStart(AutoUpdate));
                            UpdatePriceThreadServer.Start();
                        }
                        continue;
                    }
                    #endregion 如果已经开启自动更新周期并且上次更新已经完毕，则检查是否已到下次更新日期

                    //如果已经更新完毕则退出
                    if (m_UpdatePriceWorkInfo.EndFlag)
                    {
                        return;
                    }

                    #region 检查是否开启定时更新
                    if ((m_UpdatePriceWorkInfo.timeflag) && (m_UpdatePriceWorkInfo.PreEndTime != ""))
                    {
                        string tmpNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        string tmpStartTime = DateTime.Now.ToString("yyyy-MM-dd ") + StartTime;
                        string tmpEndTime = DateTime.Now.ToString("yyyy-MM-dd ") + EndTime;

                        //已经到了开始时间
                        if (CheckIfInAutoUpdateTime(DateTime.Parse(tmpStartTime), DateTime.Parse(tmpEndTime)))
                        {
                            m_UpdatePriceWorkInfo.PreEndTime = "";
                            //
                            if (UpdatePriceThreadServer != null)
                            {
                                if (UpdatePriceThreadServer != null)
                                {
                                    //开始处理过程
                                    try
                                    {
                                        UpdatePriceThreadServer.Abort();
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Record("UpdateClassPrice.log", ex, "AutoCheck：自动监测线程处理，释放更新票价线程出错2！");
                                    }
                                    UpdatePriceThreadServer = null;
                                }

                                //调用label的invoke方法
                                tmpFrm.WriteLog("准备开始自动更新...\r\n");

                                //创建并启动处理线程
                                UpdatePriceThreadServer = new Thread(new ThreadStart(AutoUpdate));
                                UpdatePriceThreadServer.Start();
                            }
                            else
                            {
                                //调用label的invoke方法
                                tmpFrm.WriteLog("准备开始自动更新...\r\n");

                                //创建并启动处理线程
                                UpdatePriceThreadServer = new Thread(new ThreadStart(AutoUpdate));
                                UpdatePriceThreadServer.Start();
                            }
                            continue;
                        }
                    }
                    #endregion 检查是否开启定时更新

                    //如果为启动定时更新并且处理线程为NULL，则开启并启动线程
                    if ((!m_UpdatePriceWorkInfo.timeflag) && (UpdatePriceThreadServer == null))
                    {
                        //调用label的invoke方法
                        tmpFrm.WriteLog("准备开始自动更新...\r\n");

                        //创建并启动处理线程
                        UpdatePriceThreadServer = new Thread(new ThreadStart(AutoUpdate));
                        UpdatePriceThreadServer.Start();

                        continue;
                    }

                    //如果没有自动更新周期和更新起始结束时间，则退出线程处理
                    if ((!m_UpdatePriceWorkInfo.timeflag) && (!m_UpdatePriceWorkInfo.autoupdateflag))
                    {
                        break;
                    }

                    //线程休眠1分钟
                    Thread.Sleep(60000);
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("UpdateClassPrice.log", ex, "AutoCheck：自动监测线程处理出错！");
                }
            }
        }

        /// <summary>
        /// 检查当前时间是否在自动更新时间之内
        /// </summary>
        /// <param name="tmpSTime">开始时间</param>
        /// <param name="tmpETime">结束时间</param>
        /// <returns></returns>
        public static bool CheckIfInAutoUpdateTime(DateTime tmpSTime, DateTime tmpETime)
        {
            DateTime tmpDT1 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "23:59:59");
            DateTime tmpDT2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "00:00:00");
            //例如： 01:00----07:30
            #region 情况一
            if (tmpSTime.CompareTo(tmpETime) == -1)
            {
                if ((DateTime.Now.CompareTo(tmpSTime) == 1) && (DateTime.Now.CompareTo(tmpETime) == -1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion 情况一

            //例如： 22:00---07:00
            #region 情况二
            if (tmpSTime.CompareTo(tmpETime) == 1)
            {
                // 22:00---23:59:59
                if ((DateTime.Now.CompareTo(tmpSTime) == 1) && (DateTime.Now.CompareTo(tmpDT1) == -1))
                {
                    return true;
                }
                // 00:00-07:00
                else if ((DateTime.Now.CompareTo(tmpETime) == -1) && (DateTime.Now.CompareTo(tmpDT2) == 1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion 情况二

            return false;
        }

        /// <summary>
        /// 解析FD结果，填入FD结果类
        /// </summary>
        /// <param name="fdResultContent">FD结果字符串</param>
        public static void GetFDInfo(ref FDInfo FD, string fdResultContent)
        {

            //>PFDXUZXIY  ^FD:XUZXIY/15JAN13/                     /CNY /TPM  860 / 
            //^01 GS/F       /  1430.00=  2860.00/F/F/  /   .   /25MAR12        /GS01  
            //^02 GS/C       /  1240.00=  2480.00/C/C/  /   .   /25MAR12        /GS01  
            //^03 GS/Y       /   950.00=  1900.00/Y/Y/  /   .   /25MAR12        /GS01  
            //^04 GS/B       /   860.00=  1720.00/B/Y/  /   .   /25MAR12        /GS02  
            //^05 GS/H       /   810.00=  1620.00/H/Y/  /   .   /25MAR12        /GS02  
            //^06 GS/K       /   760.00=  1520.00/K/Y/  /   .   /25MAR12        /GS02  
            //^07 GS/L       /   710.00=  1420.00/L/Y/  /   .   /25MAR12        /GS02  
            //^08 GS/M       /   670.00=  1340.00/M/Y/  /   .   /25MAR12        /GS02  
            //^09 GS/Q       /   570.00=  1140.00/Q/Y/  /   .   /25MAR12        /GS02  
            //^10 GS/X       /   480.00=   960.00/X/Y/  /   .   /25MAR12        /GS02  ^11 GS/U       /   430.00=   860.00/U/Y/  /   .   /25MAR12        /GS02  ^12 GS/E       /   380.00=   760.00/E/Y/  /   .   /25MAR12        /GS02  ^13 HU/R       /  2380.00=  4760.00/R/F/  /   .   /25MAR12        /HU07  ^14 HU/C       /  1430.00=  2860.00/C/C/  /   .   /25SEP12        /HU08  ^15 HU/F       /  1430.00=  2860.00/F/F/  /   .   /25MAR12        /HU07  ^16 HU/C1      /  1140.00=  2280.00/C/C/  /   .   /25SEP12        /HU08  ^17 HU/Y       /   950.00=  1900.00/Y/Y/  /   .   /25MAR12        /HU08  ^18 HU/B       /   860.00=  1720.00/B/Y/  /   .   /25MAR12        /HU09  ^    ^PAGE 1/3^>PFDXUZXIY  ^FD:XUZXIY/15JAN13/                     /CNY /TPM  860 / ^19 HU/H       /   810.00=  1620.00/H/Y/  /   .   /25MAR12        /HU09  ^20 HU/K       /   760.00=  1520.00/K/Y/  /   .   /25MAR12        /HU09  ^21 HU/L       /   710.00=  1420.00/L/Y/  /   .   /25MAR12        /HU09  ^22 HU/M       /   670.00=  1340.00/M/Y/  /   .   /25MAR12        /HU09  ^23 HU/M1      /   620.00=  1240.00/M/Y/  /   .   /25MAR12        /HU09  ^24 HU/Q       /   570.00=  1140.00/Q/Y/  /   .   /25MAR12        /HU09  ^25 HU/Q1      /   520.00=  1040.00/Q/Y/  /   .   /25MAR12        /HU09  ^26 HU/X       /   480.00=   960.00/X/Y/  /   .   /25MAR12        /HU09  ^27 HU/U       /   430.00=   860.00/U/Y/  /   .   /25MAR12        /HU09  ^28 HU/E       /   380.00=   760.00/E/Y/  /   .   /25MAR12        /HU09  ^29 MU/F       /  1710.00          /F/F/  /   .   /01JAN13 30JUN13/MU04  ^30 MU/C       /  1430.00          /C/C/  /   .   /01JAN13 30JUN13/MU04  ^31 MU/F       /            2860.00/F/F/  /   .   /01JAN13 30JUN13/MU04  ^32 MU/C       /            2480.00/C/C/  /   .   /01JAN13 30JUN13/MU04  ^33 MU/Y       /   950.00=  1900.00/Y/Y/  /   .   /01JAN13 30JUN13/MU05  ^34 MU/B       /   860.00=  1720.00/B/Y/  /   .   /01JAN13 30JUN13/MU06  ^35 MU/E       /   810.00=  1620.00/E/Y/  /   .   /01JAN13 30JUN13/MU06  ^36 MU/H       /   760.00=  1520.00/H/Y/  /   .   /01JAN13 30JUN13/MU06  ^    ^PAGE 2/3^>PFDXUZXIY  ^FD:XUZXIY/15JAN13/                     /CNY /TPM  860 / ^37 MU/L       /   710.00=  1420.00/L/Y/  /   .   /01JAN13 30JUN13/MU06  ^38 MU/M       /   670.00=  1340.00/M/Y/  /   .   /01JAN13 30JUN13/MU06  ^39 MU/N       /   620.00=  1240.00/N/Y/  /   .   /01JAN13 30JUN13/MU06  ^40 MU/R       /   570.00=  1140.00/R/Y/  /   .   /01JAN13 30JUN13/MU06  ^41 MU/S       /   520.00=  1040.00/S/Y/  /   .   /01JAN13 30JUN13/MU06  ^42 MU/V       /   480.00=   960.00/V/Y/  /   .   /01JAN13 30JUN13/MU06  ^43 MU/T       /   430.00=   860.00/T/Y/  /   .   /01JAN13 30JUN13/MU06  ^    ^PAGE 3/3^^

            //以^分割
            string[] sl = fdResultContent.Split('^');

            //是否已经获得里程标志
            bool MilFlag = false;

            //避免添加重复舱位
            string airclass = "|";

            string tmpstr = "";

            for (int i = 0; i < sl.Length; i++)
            {
                try
                {
                    if (sl[i].Trim() == "")
                    {
                        continue;
                    }

                    tmpstr = sl[i];
                    #region 获取里程信息
                    if (!MilFlag)
                    {
                        if (sl[i].IndexOf("/TPM") != -1)
                        {
                            tmpstr = sl[i].Substring(sl[i].IndexOf("/TPM") + 4).Trim();
                            FD.Mileage = tmpstr.Substring(0, tmpstr.IndexOf("/")).Trim();
                            MilFlag = true;
                        }
                        continue;
                    }
                    #endregion 获取里程信息

                    #region 前三位不是数字则继续循环
                    tmpstr = sl[i].Substring(0, 3).Trim();
                    try
                    {
                        int.Parse(tmpstr);
                    }
                    catch
                    {
                        continue;
                    }
                    #endregion 前三位不是数字则继续循环

                    ClassPriceInfo tmpClassPriceInfo = new ClassPriceInfo();

                    //以空格和/进行分割
                    string[] sl2 = sl[i].Split(new Char[] { ' ', '/' });
                    int index = 0;
                    for (int j = 0; j < sl2.Length; j++)
                    {
                        if (sl2[j] == "")
                            continue;

                        switch (index)
                        {
                            case 0:
                                if (sl2[j].Trim().Length > 2)
                                {
                                    tmpClassPriceInfo.CarrierCode = sl2[j].Substring(3);
                                    index++;
                                }
                                index++;
                                break;
                            case 1:
                                tmpClassPriceInfo.CarrierCode = sl2[j];
                                index++;
                                break;
                            case 2:
                                if (sl2[j].Trim().Length == 3)
                                {
                                    tmpClassPriceInfo.ClassCode = sl2[j].Trim().Substring(0, 1);
                                }
                                else
                                {
                                    tmpClassPriceInfo.ClassCode = sl2[j];
                                }
                                index++;
                                break;
                            case 3:
                                if (sl2[j].IndexOf("=") == -1)
                                {
                                    tmpClassPriceInfo.ClassPrice = sl2[j].Trim();
                                    index++;
                                    break;
                                }
                                tmpClassPriceInfo.ClassPrice = sl2[j].Substring(0, sl2[j].IndexOf("="));
                                index++;
                                break;
                            case 4:
                            case 5:
                                index++;
                                break;
                            case 6:
                                tmpClassPriceInfo.ClassLevel = sl2[j];
                                break;
                        }
                        if (index >= 6)
                            break;
                    }

                    //跳过重复舱位
                    if (airclass.Contains("|" + tmpClassPriceInfo.CarrierCode + tmpClassPriceInfo.ClassCode + "|"))
                    {
                        continue;
                    }

                    airclass += tmpClassPriceInfo.CarrierCode + tmpClassPriceInfo.ClassCode + "|";

                    FD.ClassPriceList.Add(tmpClassPriceInfo);

                    if (tmpClassPriceInfo.ClassCode.ToLower() == "y")
                    {
                        FullPriceInfo tmpFullPriceInfo = new FullPriceInfo();
                        tmpFullPriceInfo.carrier = tmpClassPriceInfo.CarrierCode;
                        tmpFullPriceInfo.fullPrice = tmpClassPriceInfo.ClassPrice;
                        FD.FullPriceList.Add(tmpFullPriceInfo);
                    }
                }
                catch (Exception ex)
                {
                    Log.Record("UpdateClassPrice.log", ex, "GetFDInfo:处理出现错误！" + sl[i]);
                }

            }
        }

        /// <summary>
        /// 更新数据库的舱位运价
        /// </summary>
        /// <param name="FD">运价结果信息</param>
        public static void UpdateDataBasePriceInfo(FDInfo FD)
        {
            BaseDataManage Manage = new BaseDataManage(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Dal.dll");

            #region 航线舱位价格数据处理
            //删除航线舱位价格数据
            string tmpSQL = " FromCityCode='" + FD.FromCity + "' and ToCityCode='" + FD.ToCity + "' ";
            bool DelFlag = (bool)(Manage.CallMethod("Bd_Air_CabinDiscount", "DeleteBySQL", null, new object[] { tmpSQL }));

            if (!DelFlag)
            {
                Log.Record("UpdateClassPrice.log", "UpdateDataBasePriceInfo:处理出现错误！删除Bd_Air_CabinDiscount数据：" +
                        FD.FromCity + "|" + FD.ToCity);
                return;
            }

            //循环插入数据
            for (int i = 0; i < FD.ClassPriceList.Count; i++)
            {
                try
                {
                    Bd_Air_CabinDiscount tmpCabinDiscount = new Bd_Air_CabinDiscount();
                    tmpCabinDiscount.AirCode = FD.ClassPriceList[i].CarrierCode;

                    tmpCabinDiscount.Cabin = FD.ClassPriceList[i].ClassCode;

                    tmpCabinDiscount.CabinName = FD.ClassPriceList[i].ClassLevel;

                    tmpCabinDiscount.CabinPrice = decimal.Parse(FD.ClassPriceList[i].ClassPrice);

                    tmpCabinDiscount.FromCityCode = FD.FromCity;

                    tmpCabinDiscount.ToCityCode = FD.ToCity;

                    tmpCabinDiscount.IsGN = 0;

                    //添加数据
                    bool AddFlag = (bool)Manage.CallMethod("Bd_Air_CabinDiscount", "Insert", null, new object[] { tmpCabinDiscount });

                    if (!AddFlag)
                    {
                        Log.Record("UpdateClassPrice.log", "UpdateDataBasePriceInfo:处理出现错误！添加Bd_Air_CabinDiscount数据：" +
                            FD.ClassPriceList[i].CarrierCode + "|" + FD.ClassPriceList[i].ClassCode + "|" + FD.FromCity + "|" + FD.ToCity + "|" + FD.ClassPriceList[i].ClassPrice);
                    }
                }
                catch (Exception ex)
                {
                    Log.Record("UpdateClassPrice.log", ex, "UpdateDataBasePriceInfo:处理出现错误！添加Bd_Air_CabinDiscount数据");
                }
            }
            #endregion 航线舱位价格数据处理


            #region 航线价格数据处理
            //删除航线舱位价格数据
            tmpSQL = " FromCityCode='" + FD.FromCity + "' and ToCityCode='" + FD.ToCity + "' ";
            DelFlag = (bool)(Manage.CallMethod("Bd_Air_Fares", "DeleteBySQL", null, new object[] { tmpSQL }));

            if (!DelFlag)
            {
                Log.Record("UpdateClassPrice.log", "UpdateDataBasePriceInfo:处理出现错误！删除Bd_Air_Fares航线价格数据：" +
                        FD.FromCity + "|" + FD.ToCity);
                return;
            }

            //循环插入数据
            for (int i = 0; i < FD.FullPriceList.Count; i++)
            {
                try
                {
                    Bd_Air_Fares tmpAirFares = new Bd_Air_Fares();

                    tmpAirFares.CarryCode = FD.FullPriceList[i].carrier;

                    tmpAirFares.FromCityCode = FD.FromCity;

                    tmpAirFares.ToCityCode = FD.ToCity;

                    tmpAirFares.FareFee = decimal.Parse(FD.FullPriceList[i].fullPrice);

                    tmpAirFares.IsDomestic = 0;

                    tmpAirFares.Mileage = int.Parse(FD.Mileage);

                    //添加数据
                    bool AddFlag = (bool)Manage.CallMethod("Bd_Air_Fares", "Insert", null, new object[] { tmpAirFares });

                    if (!AddFlag)
                    {
                        Log.Record("UpdateClassPrice.log", "UpdateDataBasePriceInfo:处理出现错误！添加Bd_Air_Fares数据：" +
                            FD.FullPriceList[i].carrier + "|" + FD.FullPriceList[i].fullPrice + "|" + FD.FromCity + "|" + FD.ToCity);
                    }
                }
                catch (Exception ex)
                {
                    Log.Record("UpdateClassPrice.log", ex, "UpdateDataBasePriceInfo:处理出现错误！添加Bd_Air_Fares数据");
                }
            }
            #endregion 航线价格数据处理
        }

        /// <summary>
        /// 更新运价处理
        /// </summary>
        /// <param name="tmpfromcity">出发城市</param>
        /// <param name="tmptocity">到达城市</param>
        public static void UpdatePrice(string tmpfromcity, string tmptocity)
        {
            //调用黑屏获取运价
            //发送指令并获取返回内容
            string ResultContent = WebManage.SendCommand("FD:" + tmpfromcity + tmptocity, m_UpdatePriceWorkInfo.officenum, true, true, m_UpdatePriceWorkInfo.serverip, m_UpdatePriceWorkInfo.serverport);

            //解析FD的结果
            FDInfo tmpFD = new FDInfo();
            tmpFD.FromCity = tmpfromcity;
            tmpFD.ToCity = tmptocity;
            GetFDInfo(ref tmpFD, ResultContent);

            //如果解析出了运价信息则更新
            if ((tmpFD.ClassPriceList.Count > 0) && (tmpFD.FullPriceList.Count > 0))
            {
                //更新数据库数据
                UpdateDataBasePriceInfo(tmpFD);
                Log.Record("UpdateClassPrice.log", "城市对：" + tmpfromcity + tmptocity + "，更新完毕...");
            }
            else
            {
                Log.Record("UpdateClassPrice.log", "城市对：" + tmpfromcity + tmptocity + "，缺少全价或舱位价格信息，略过更新...");
            }
        }
    }
}
