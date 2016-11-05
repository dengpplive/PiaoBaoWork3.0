using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.IO;
using PbProject.ConsoleServerProc;
using log4net;
using System.Text.RegularExpressions;
using PbProject.Logic;
using PbProject.Logic.ControlBase;
using System.Diagnostics;
using PbProject.ConsoleServerProc.Utils;
using System.Web;
using PbProject.Logic.Buy;

namespace ConsoleServerProc
{
    public partial class PbMain : Form
    {
        //用于显示日志
        public delegate void DleShowLog(int LogType, string Data);

        //调试标志
        public bool DebugFlag = false;

        public PbMain()
        {
            InitializeComponent();
        }

        #region 自动更新航线舱位价格相关
        //从配置文件中读取配置参数信息
        private void ReadSystemParam()
        {
            try
            {
                //读取配置文件信息
                XmlDocument xmlDoc = new XmlDocument();
                if (File.Exists("sysinfo.xml"))
                {
                    xmlDoc.Load("sysinfo.xml");

                    XmlNode root = xmlDoc.SelectSingleNode("EtermClient");
                    XmlNode xe1 = root.SelectSingleNode("ShareInfo");
                    XmlElement xe2 = (XmlElement)xe1;
                    txtServerIp.Text = xe2.GetAttribute("serverip");
                    txtPort.Text = xe2.GetAttribute("port");
                    txtOfficeNum.Text = xe2.GetAttribute("officenum");
                    string tmpstr = xe2.GetAttribute("autoupdate");
                    if (tmpstr == "1")
                    {
                        chkAutoUpdate.Checked = true;
                    }
                    else
                    {
                        chkAutoUpdate.Checked = false;
                    }
                    tmpstr = xe2.GetAttribute("autoupdatedays");
                    if (tmpstr == "")
                    {
                        numPerDays.Value = 0;
                    }
                    else
                    {
                        numPerDays.Value = int.Parse(tmpstr);
                    }

                    tmpstr = xe2.GetAttribute("chkfromcity");
                    if (tmpstr == "1")
                    {
                        chkFromCity.Checked = true;
                    }
                    else
                    {
                        chkFromCity.Checked = false;
                    }

                    txtFromCity.Text = xe2.GetAttribute("fromcity");

                    tmpstr = xe2.GetAttribute("chktocity");
                    if (tmpstr == "1")
                    {
                        chkToCity.Checked = true;
                    }
                    else
                    {
                        chkToCity.Checked = false;
                    }
                    txtToCity.Text = xe2.GetAttribute("tocity");

                    tmpstr = xe2.GetAttribute("chkbeginendtime");
                    if (tmpstr == "1")
                    {
                        chkBeginEndTime.Checked = true;
                    }
                    else
                    {
                        chkBeginEndTime.Checked = false;
                    }
                    dtpBeginTime.Text = xe2.GetAttribute("begintime");
                    dtpEndTime.Text = xe2.GetAttribute("endtime");
                }
            }
            catch (Exception ex)
            {
                //记录错误日志
                Log.Record("ReadSysInfoError.log", ex, "读取配置文件SysInfo.xml出错！");
            }
        }

        //把配置参数信息写入到配置文件
        private void WriteSystemParam()
        {
            try
            {
                //记录密码(保存到本地文件) 
                XmlDocument xmlDoc = new XmlDocument();
                if (File.Exists("sysinfo.xml"))
                {
                    xmlDoc.Load("sysinfo.xml");

                    XmlNode root = xmlDoc.SelectSingleNode("EtermClient");
                    XmlNode xe1 = root.SelectSingleNode("ShareInfo");
                    XmlElement xe2 = (XmlElement)xe1;
                    //服务器IP
                    xe2.SetAttribute("serverip", txtServerIp.Text);
                    //端口
                    xe2.SetAttribute("port", txtPort.Text);
                    //office号
                    xe2.SetAttribute("officenum", txtOfficeNum.Text);
                    //自动更新标志
                    if (chkAutoUpdate.Checked)
                    {
                        xe2.SetAttribute("autoupdate", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("autoupdate", "0");
                    }
                    //自动更新间隔天数
                    xe2.SetAttribute("autoupdatedays", numPerDays.Value.ToString());
                    //指定出发城市标志
                    if (chkFromCity.Checked)
                    {
                        xe2.SetAttribute("chkfromcity", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("chkfromcity", "0");
                    }
                    //出发城市
                    xe2.SetAttribute("fromcity", txtFromCity.Text);
                    //指定到达城市标志
                    if (chkToCity.Checked)
                    {
                        xe2.SetAttribute("chktocity", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("chktocity", "0");
                    }
                    //到达城市
                    xe2.SetAttribute("tocity", txtToCity.Text);

                    //指定开始结束时间标志
                    if (chkBeginEndTime.Checked)
                    {
                        xe2.SetAttribute("chkbeginendtime", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("chkbeginendtime", "0");
                    }
                    //开始时间
                    xe2.SetAttribute("begintime", dtpBeginTime.Text);
                    //结束时间
                    xe2.SetAttribute("endtime", dtpEndTime.Text);

                    xmlDoc.Save("sysinfo.xml");
                }
                else
                {
                    //创建配置文件

                    //加入XML的声明段落
                    XmlNode xn1 = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                    xmlDoc.AppendChild(xn1);

                    XmlElement xe1 = xmlDoc.CreateElement("", "EtermClient", "");
                    xmlDoc.AppendChild(xe1);

                    XmlElement xe2 = xmlDoc.CreateElement("", "ShareInfo", "");

                    //服务器IP
                    xe2.SetAttribute("serverip", txtServerIp.Text);
                    //端口
                    xe2.SetAttribute("port", txtPort.Text);
                    //office号
                    xe2.SetAttribute("officenum", txtOfficeNum.Text);
                    //自动更新标志
                    if (chkAutoUpdate.Checked)
                    {
                        xe2.SetAttribute("autoupdate", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("autoupdate", "0");
                    }
                    //自动更新间隔天数
                    xe2.SetAttribute("autoupdatedays", numPerDays.Value.ToString());
                    //指定出发城市标志
                    if (chkFromCity.Checked)
                    {
                        xe2.SetAttribute("chkfromcity", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("chkfromcity", "0");
                    }
                    //出发城市
                    xe2.SetAttribute("fromcity", txtFromCity.Text);
                    //指定到达城市标志
                    if (chkToCity.Checked)
                    {
                        xe2.SetAttribute("chktocity", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("chktocity", "0");
                    }
                    //到达城市
                    xe2.SetAttribute("tocity", txtToCity.Text);

                    //指定开始结束时间标志
                    if (chkBeginEndTime.Checked)
                    {
                        xe2.SetAttribute("chkbeginendtime", "1");
                    }
                    else
                    {
                        xe2.SetAttribute("chkbeginendtime", "0");
                    }
                    //开始时间
                    xe2.SetAttribute("begintime", dtpBeginTime.Text);
                    //结束时间
                    xe2.SetAttribute("endtime", dtpEndTime.Text);

                    xe1.AppendChild(xe2);
                    xmlDoc.Save("sysinfo.xml");
                }
            }
            catch (Exception ex)
            {
                //写错误日志
                Log.Record("WriteSysInfoError.log", ex, "写入配置文件sysinfo.xml出错！");
            }
        }

        //声明一个委托类型，该委托类型无输入参数和输出参数
        public delegate void ProcessDelegate(string strText);

        //写入当前状态信息
        public void WriteLog(string strText)
        {
            if (richTextBox1.InvokeRequired)
            {
                ProcessDelegate OutDelegate = new ProcessDelegate(WriteLog);

                this.BeginInvoke(OutDelegate, new object[] { strText });
                return;
            }

            //如果日志超过500条，则清空
            //if (richTextBox2.Lines.Length > 500)
            //{
            //    richTextBox2.Clear();
            //}

            richTextBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
            richTextBox1.AppendText(strText);

            if (UpdateClassPriceProc.m_UpdatePriceWorkInfo.EndFlag)
            {
                if (!chkAutoUpdate.Checked)
                {
                    button1.Enabled = true;
                    button2.Enabled = false;
                }
            }
        }

        //开始更新票价处理
        private void button1_Click(object sender, EventArgs e)
        {
            if (DebugFlag)
            {
                if (!chkFromCity.Checked || !chkToCity.Checked)
                {
                    MessageBox.Show("请输入出发城市或到达城市三字码！");
                    return;
                }
            }

            button1.Enabled = false;
            button2.Enabled = true;
            try
            {
                #region 赋值相关参数设置信息
                UpdateClassPriceProc.tmpFrm = this;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.serverip = txtServerIp.Text;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.serverport = int.Parse(txtPort.Text);
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.officenum = txtOfficeNum.Text;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.autoupdateflag = chkAutoUpdate.Checked;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.autoupdatedays = int.Parse(numPerDays.Value.ToString());
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.fromcityflag = chkFromCity.Checked;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.fromcity = txtFromCity.Text;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.tocityflag = chkToCity.Checked;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.tocity = txtToCity.Text;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.timeflag = chkBeginEndTime.Checked;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.beginendtime = dtpBeginTime.Text + "|" + dtpEndTime.Text;

                UpdateClassPriceProc.m_UpdatePriceWorkInfo.EndFlag = false;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.EndTime = DateTime.Now;
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.startfromtocityid = "";
                UpdateClassPriceProc.m_UpdatePriceWorkInfo.PreEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:SS");
                #endregion 赋值相关参数设置信息

                UpdateClassPriceProc.StartServer();
            }
            catch (Exception ex)
            {
                //记录错误日志
                Log.Record("UpdateClassPriceError.log", ex, "开始服务按钮");
            }
        }

        //停止更新票价处理
        private void button2_Click(object sender, EventArgs e)
        {
            //停止服务
            try
            {
                UpdateClassPriceProc.StopServer();
            }
            catch (Exception ex)
            {
                //记录错误日志
                Log.Record("UpdateClassPriceError.log", ex, "停止服务按钮");
            }
            button1.Enabled = true;
            button2.Enabled = false;
        }

        #endregion 自动更新航线舱位价格相关

        #region 自动取消PNR相关

        string strCompanySQL = "";
        /// <summary>
        /// 是否开始取消编码的成功
        /// </summary>
        private bool IsOpenXePnr = false;

        /// <summary>
        /// 取消编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ck_XePnr_CheckedChanged(object sender, EventArgs e)
        {
            IsOpenXePnr = ck_XePnr.Checked;
        }

        #endregion 自动取消PNR相关

        private void Form1_Load(object sender, EventArgs e)
        {
            if (DebugFlag)
            {
                ck_ALL.Hide();
                btnAutoStart.Hide();
                btnAutoStop.Hide();
                AutoLeft.Hide();
                AutoRight.Hide();
            }
            Log.filepath = Application.StartupPath;
            #region 初始化自动更新航线舱位价格相关信息
            //读取配置信息
            ReadSystemParam();
            button1.Enabled = true;
            button2.Enabled = false;
            #endregion 初始化自动更新航线舱位价格相关信息

            #region 初始化自动出票
            InitAuto();
            #endregion
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                UpdateClassPriceProc.StopServer();
                StopServer();
                Environment.Exit(0);
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// 开始自动出票服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoStart_Click(object sender, EventArgs e)
        {
            //开始服务
            StartServer();
        }

        /// <summary>
        /// 停止自动出票服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoStop_Click(object sender, EventArgs e)
        {
            //停止服务
            StopServer();
        }

        private void LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = Application.StartupPath + "\\Log";
            if (Directory.Exists(dir))
            {
                Process.Start(dir);
            }
            else
            {
                Process.Start(Application.StartupPath);
            }
        }


        #region 自动出票

        #region 变量
        /// <summary>
        /// 是否开启BSP自动出票 true 开启 false关闭
        /// </summary>
        bool BSPIsOpen = false;
        int BSPSecond = 30;//扫描时间
        int BSPCount = 3;//失败扫描次数
        Thread BSPTh = null;
        //选择的落地运营商列表
        List<string> SelGYList = null;
        List<ListParam> SelGYTextList = null;
        B2BAutoTicket B2BAuto = null;
        RobTicket robTicket = null;
        /// <summary>
        /// 是否开启抢票 true false
        /// </summary>
        bool IsOpenRobTicket = false;
        private ILog errorLog = null;
        private ILog infoLog = null;



        private Thread m_ThreadChinapnr = null;
        int m_ChinapnrRefreshTime = 30;//单位秒

        /// <summary>
        /// 委文本托
        /// </summary>
        /// <param name="str"></param>
        private delegate void delegateSetLabel(string str);


        #endregion

        public void InitAuto()
        {
            BSPAutoTicket BSPManage = new BSPAutoTicket();
            List<ListParam> m_ListParam = BSPManage.GetGYList();
            this.lb_GY.DataSource = m_ListParam;
            this.lb_GY.DisplayMember = "ShowText";
            this.lb_GY.ValueMember = "CpyNo";
            this.lb_GY.Tag = m_ListParam;
        }
        /// <summary>
        /// 获取选中的CpyNo
        /// </summary>
        /// <returns></returns>
        public List<string> GetCheckedCpyNo()
        {
            List<string> selIndex = new List<string>();
            for (int i = 0; i < this.lb_GY.Items.Count; i++)
            {
                if (this.lb_GY.GetItemChecked(i))
                {
                    this.lb_GY.SetSelected(i, true);
                    selIndex.Add("'" + this.lb_GY.SelectedValue.ToString() + "'");
                }
            }
            return selIndex;
        }
        /// <summary>
        /// 获取选中的文本列表
        /// </summary>
        /// <returns></returns>
        public List<ListParam> GetCheckedText()
        {
            List<ListParam> selText = new List<ListParam>();
            for (int i = 0; i < this.lb_GY.Items.Count; i++)
            {
                if (this.lb_GY.GetItemChecked(i))
                {
                    this.lb_GY.SetSelected(i, true);
                    string CpyNo = this.lb_GY.SelectedValue.ToString();
                    if (this.lb_GY.Tag != null)
                    {
                        List<ListParam> lstParam = this.lb_GY.Tag as List<ListParam>;
                        if (lstParam != null && lstParam.Count > 0)
                        {
                            ListParam LP = lstParam.Find((p) => p.CpyNo == CpyNo);
                            if (LP != null)
                            {
                                selText.Add(LP);
                            }
                        }
                    }
                }
            }
            return selText;
        }
        //全选落地运营商和供应
        private void ck_ALL_CheckedChanged(object sender, EventArgs e)
        {
            if (this.lb_GY.Items.Count > 0)
            {
                int Count = this.lb_GY.Items.Count;
                for (int i = 0; i < Count; i++)
                {
                    this.lb_GY.SetItemChecked(i, ck_ALL.Checked);
                }
            }
        }

        /// <summary>
        ///  公用记录日志 
        /// </summary>
        /// <param name="LogType">0.记录BSP自动出票日志 1.取消编码日志,2.支付宝自动出票日志 3.汇付自动出票日志 4.抢票日志</param>
        /// <param name="Data"></param>
        public void ShowLog(int LogType, string Data)
        {
            if (this.InvokeRequired)
            {
                Invoke(new DleShowLog(ShowLog), new object[] { LogType, Data });
            }
            else
            {
                if (LogType == 0)
                {
                    if (Log_BSP.TextLength > 20000)
                    {
                        Log_BSP.Clear();
                    }
                    Log_BSP.AppendText(Data + "\r\r");
                    Log_BSP.ScrollToCaret();
                    //记录日志
                    PnrAnalysis.LogText.LogWrite(Data + "\r\r", "BSP");
                }
                else if (LogType == 1)
                {
                    if (log_XePnr.TextLength > 20000)
                    {
                        log_XePnr.Clear();
                    }
                    log_XePnr.AppendText(Data + "\r\r");
                    log_XePnr.ScrollToCaret();
                    //记录日志
                    PnrAnalysis.LogText.LogWrite(Data + "\r\r", "XePnr");
                }
                else if (LogType == 2)//支付宝
                {
                    if (logBlock_Alipay.TextLength > 20000)
                    {
                        logBlock_Alipay.Clear();
                    }
                    logBlock_Alipay.AppendText("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Data + "\r\r");
                    logBlock_Alipay.ScrollToCaret();
                    //记录日志
                    PnrAnalysis.LogText.LogWrite("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Data + "\r\r", "B2B\\Alipay_B2B");
                }
                else if (LogType == 3)//汇付
                {
                    if (logBlock_Chinapnr.TextLength > 20000)
                    {
                        logBlock_Chinapnr.Clear();
                    }
                    logBlock_Chinapnr.AppendText("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Data + "\r\r");
                    logBlock_Chinapnr.ScrollToCaret();
                    //记录日志
                    PnrAnalysis.LogText.LogWrite("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Data + "\r\r", "B2B\\China_B2B");
                }
                else if (LogType == 4)//抢票
                {
                    if (rich_RobTicket.TextLength > 20000)
                    {
                        rich_RobTicket.Clear();
                    }
                    rich_RobTicket.AppendText("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Data + "\r\r");
                    rich_RobTicket.ScrollToCaret();
                    //记录日志
                    PnrAnalysis.LogText.LogWrite("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Data + "\r\r", "RobTicket");
                }
            }
        }
        /// <summary>
        /// 是否开始取消编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ck_BSP_CheckedChanged(object sender, EventArgs e)
        {
            BSPIsOpen = ck_BSP.Checked;
        }
        private void cbk_RobTicket_CheckedChanged(object sender, EventArgs e)
        {
            IsOpenRobTicket = cbk_RobTicket.Checked;
        }
        public void InitData()
        {
            //选中的供应和落地运营商CpyNo
            SelGYList = GetCheckedCpyNo();
            SelGYTextList = GetCheckedText();
        }
        /// <summary>
        /// 开始服务
        /// </summary>
        public void StartServer()
        {
            try
            {
                InitData();
                if (SelGYList.Count == 0)
                {
                    MessageBox.Show("请选择落地运营商！");
                    return;
                }

                #region BSP自动出票
                if (BSPTh != null && BSPTh.IsAlive)
                {
                    BSPTh.Abort();
                    BSPTh = null;
                }

                int.TryParse(txtBSPMinute.Text.Trim(), out BSPSecond);
                int.TryParse(txtBSPCount.Text.Trim(), out BSPCount);
                BSPAutoTicket.BSPShowLog bspLog = new BSPAutoTicket.BSPShowLog(ShowLog);
                try
                {
                    if (BSPTh != null && BSPTh.IsAlive)
                    {
                        BSPTh.Abort();
                    }
                }
                catch (Exception)
                {
                }
                BSPTh = new Thread(new ThreadStart(delegate()
                {
                    new BSPAutoTicket().BSPStart(SelGYList, SelGYTextList, BSPSecond, BSPCount, bspLog);
                }));
                //开启BSP
                if (BSPIsOpen)
                {
                    BSPTh.Start();
                }
                #endregion



                #region B2B自动出票
                B2BAutoTicketStartCheck(SelGYList, SelGYTextList);
                #endregion


                #region 取消编码

                try
                {
                    #region 赋值相关参数设置信息
                    XePNR.m_XePNRInfo.EndFlag = false;
                    XePNR.LPList = SelGYTextList;
                    int ReTryCount = 0, InterMinutes = 1, day = 2;
                    //失败尝试次数
                    if (int.TryParse(txtXePnrReTryCount.Text.Trim(), out ReTryCount))
                    {
                        XePNR.m_XePNRInfo.ReTryCount = ReTryCount;
                    }
                    //遍历时间间隔
                    if (int.TryParse(txt_InterMinutes.Text.Trim(), out InterMinutes))
                    {
                        XePNR.m_XePNRInfo.InterMinutes = InterMinutes;
                    }
                    if (int.TryParse(txtDay.Text.Trim(), out day))
                    {
                        XePNR.m_XePNRInfo.day = day;
                    }
                    XePNR.XeLog = new XePNR.XePnrShowLog(ShowLog);
                    XePNR.m_XePNRInfo.CompanyNoList = SelGYList;// string.Join(",", SelGYList.ToArray());
                    XePNR.m_XePNRInfo.XEMinutes = int.Parse(txt_XEMinutes.Text);
                    #endregion 赋值相关参数设置信息
                    if (IsOpenXePnr)
                    {
                        XePNR.StartServer();
                    }
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("XePnrError.log", ex, "开始服务按钮");
                }

                #endregion

                #region 抢票程序

                robTicket = new RobTicket();
                //扫描时间间隔 默认3分钟
                int robTime = 180;
                int.TryParse(txt_RobScanTime.Text.Trim(), out robTime);
                robTicket.m_ScanTime = robTime;
                //扫描几分钟之类的订单 默认1小时
                int robInnerTime = 60;
                int.TryParse(txtInnerTime.Text.Trim(), out robInnerTime);
                robTicket.m_InnerScanTime = robInnerTime;
                //扫描次数
                int m_ScanCount = 1;
                int.TryParse(txtScanCount.Text.Trim(), out m_ScanCount);
                robTicket.m_ScanCount = m_ScanCount;


                //记录扫描日志
                robTicket.m_Log = new RobTicket.RobShowLog(ShowLog);
                //落地运营商公司信息
                robTicket.lstCpyNo = SelGYList;
                robTicket.SelGYTextList = SelGYTextList;
                if (IsOpenRobTicket)
                {
                    robTicket.StartScan();
                }
                #endregion

            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("开始自动出票服务:" + ex.Message, "AutoBSP");
            }
            finally
            {
                if (SelGYList != null && SelGYList.Count > 0)
                {
                    btnAutoStart.Enabled = false;
                    btnAutoStop.Enabled = true;
                }
            }
        }


        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopServer()
        {
            try
            {
                #region BSP自动出票
                if (BSPTh != null && BSPTh.IsAlive)
                {
                    BSPTh.Abort();
                    BSPTh = null;
                }
                #endregion

                #region 取消编码
                try
                {
                    XePNR.XeLog = null;
                    XePNR.m_XePNRInfo.EndFlag = true;
                    XePNR.StopServer();
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("XePnrError.log", ex, "停止服务按钮");
                }
                #endregion

                //B2B自动出票
                if (B2BAuto != null)
                {
                    B2BAuto.Close();
                }
                //抢票
                if (robTicket != null)
                {
                    robTicket.StopScan();
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("停止自动出票服务:" + ex.Message, "AutoBSP");
            }
            finally
            {
                if (SelGYList != null && SelGYList.Count > 0)
                {
                    btnAutoStart.Enabled = true;
                    btnAutoStop.Enabled = false;
                }
            }

        }
        /// <summary>
        /// B2B自动出票启动判断
        /// </summary>
        private void B2BAutoTicketStartCheck(List<string> CpyNoList, List<ListParam> LPList)
        {
            B2BAuto = new B2BAutoTicket();
            //用于记录日志 委托
            B2BAutoTicket.B2BShowLog b2bLog = new B2BAutoTicket.B2BShowLog(ShowLog);
            B2BAuto.m_Log = b2bLog;
            //tl：B2B自动出票
            if (this.ck_ChinaPay.Checked)
            {
                if (this.ck_Alipay.Checked)
                    MessageBox.Show("支付宝本票通与汇付天下只能选择一个");
                else
                {
                    //这里启用汇付天下自动出票
                    B2BAuto.ChinapnrStart(CpyNoList, LPList, b2bLog);
                }
            }
            else
            {
                if (this.ck_Alipay.Checked)
                {
                    //这里启用支付宝本票通
                    B2BAuto.AlipayStart(CpyNoList, LPList, b2bLog);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            PbProject.Model.User_Company mTopcom = new PbProject.Logic.User.User_CompanyBLL().GetCompany("100001000045");
            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(mTopcom.UninCode.ToString());
            PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(ParList);

            PbProject.Logic.Pay.AliPay tempAlipay = new PbProject.Logic.Pay.AliPay();
            string msg = "";
            string sqlWhere = " orderid='0130502163134897981'";
            BaseDataManage Manage = new BaseDataManage(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Dal.dll");
            List<PbProject.Model.Tb_Ticket_Order> orderList = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<PbProject.Model.Tb_Ticket_Order>;

            PbProject.Model.Tb_Ticket_Order order = orderList[0];
            tempAlipay.QueryPriceByPNR(order, BS, ref msg);
        }





        #endregion















    }
}
