using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PbProject.Logic.ControlBase;
using PbProject.Model;
using PnrAnalysis;
using ConsoleServerProc;
using System.Threading;
using System.Diagnostics;
namespace PbProject.ConsoleServerProc
{
    public partial class OpenTicket : Form
    {
        public OpenTicket()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string SourFileName = txtSource.Text.Trim();
            string DescFileName = txtSource.Text.Trim();
            if (SourFileName != "" && DescFileName != "")
            {
                StreamReader sr = new StreamReader(SourFileName, Encoding.Default);
                string strData = sr.ReadToEnd();
                sr.Close();
                string[] strArr = strData.Split(new string[] { "\r\n", "\n", "\r", " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                List<string> lst = new List<string>();
                lst.AddRange(strArr);

                PP p = new PP();
                ParamInfo pi = new ParamInfo();
                pi.CompanyName = Program.UserModel.COMPANY.UninAllName;
                pi.CpyNo = Program.UserModel.COMPANY.UninCode;
                pi.IP = Program.UserModel.CONFIGPARAM.WhiteScreenIP;
                pi.Port = Program.UserModel.CONFIGPARAM.WhiteScreenPort;
                pi.Office = Program.UserModel.CONFIGPARAM.Office.Split('^')[0];
                List<ParamInfo> piList = new List<ParamInfo>();
                piList.Add(pi);

                p.ll = piList;
                p.ticketList = lst;
                button2.Enabled = false;
                int second = 1;
                int.TryParse(txtSecond.Text.Trim(), out second);
                p.ScanJJ = second;
                backgroundWorker1.RunWorkerAsync(p);
            }
            else
            {
                MessageBox.Show("路径错误");
            }
        }
        string path = "";
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                txtSource.Text = open.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog open = new FolderBrowserDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                txtDesc.Text = open.SelectedPath;
                path = open.SelectedPath;
            }
        }




        public class ParamInfo
        {
            private string m_CompanyName = string.Empty;
            public string CompanyName
            {
                get
                {
                    return m_CompanyName;
                }
                set
                {
                    m_CompanyName = value;
                }
            }
            private string _CpyNo = string.Empty;

            public string CpyNo
            {
                get
                {
                    return _CpyNo;
                }
                set
                {
                    _CpyNo = value;
                }
            }
            public string Office = string.Empty;
            public string IP = string.Empty;
            public string Port = string.Empty;
        }

        public class PP
        {
            public List<ParamInfo> ll = new List<ParamInfo>();
            public List<string> ticketList = new List<string>();
            public List<string> resultList = new List<string>();
            /// <summary>
            /// 扫描时间间隔 秒
            /// </summary>
            public int ScanJJ = 1;

        }

        //进程
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (richTextBox1.Lines.Length > 500)
            {
                richTextBox1.Clear();
            }
            richTextBox1.AppendText(e.UserState.ToString() + "\r\n");
            richTextBox1.ScrollToCaret();
            LabResult.Text = e.ProgressPercentage + "%";
        }
        //结束
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("完成");
            button2.Enabled = true;
        }
        //工作
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            PP m = e.Argument as PP;
            if (m != null)
            {
                ParamInfo PI = m.ll[0];
                ParamObject PM = new ParamObject();
                PM.ServerIP = PI.IP;
                PM.ServerPort = int.Parse(PI.Port);
                PM.Office = PI.Office;
                PnrAnalysis.FormatPNR format = new FormatPNR();
                int i = 1;
                foreach (string item in m.ticketList)
                {
                    if (item.Replace("-", "").Trim() != "")
                    {
                        //发送指令                       
                        PM.code = "detr:TN" + item.Replace("-", "").Trim();
                        string RecvData = SendNewPID.SendCommand(PM);
                        string TicketStatus = format.GetTicketStatus(RecvData);
                        if (TicketStatus.ToUpper().Contains("OPEN FOR USE"))
                        {
                            m.resultList.Add(item);
                            LogWrite(item + "\r\n", path);
                        }
                        backgroundWorker1.ReportProgress((i * 100) / m.ticketList.Count, "发送：" + PM.code + "结果:" + RecvData);
                        Thread.Sleep(m.ScanJJ * 1000);
                    }

                    i++;
                }
                e.Result = m;
            }
        }
        public static void LogWrite(string Con, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(Con))
                {
                    Con = "日志数据为空";
                }

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                if (!path.EndsWith("\\"))
                {
                    path = path + "\\";
                }
                System.IO.File.AppendAllText(path + System.DateTime.Now.ToString("yyyy-MM-dd HH") + "OPEN_FOR_USE.txt", Con);
            }
            catch
            {
            }
        }
        private void OpenTicket_Load(object sender, EventArgs e)
        {

        }

        private void OpenTicket_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception)
            {
            }
        }
    }
}
