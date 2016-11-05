using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace PbProject.Model
{
    /// <summary>
    /// Log_Pnr:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Log_Pnr
    {
        public Log_Pnr()
        { }
        #region Model
        private Guid _id;
        private string _cpyno = "";
        private string _cpyname = "";
        private int _cpytype = 0;
        private DateTime _opertime = DateTime.Now;
        private string _operloginname = "";
        private string _operusername = "";
        private string _sscontent = "";
        private string _resultcontent = "";
        private string _pnr = "";
        private string _officecode = "";
        private bool _orderflag = false;
        private bool _flag = false;
        private int _retrycount = 0;
        private int _a1 = 0;
        private int _a2 = 0;
        private decimal _a3 = 0M;
        private decimal _a4 = 0M;
        private DateTime _a5 = Convert.ToDateTime("1900-01-01");
        private DateTime _a6 = Convert.ToDateTime("1900-01-01");
        private string _a7 = "";
        private string _a8 = "";
        /// <summary>
        /// 主键
        /// </summary>
        public Guid id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CpyNo
        {
            set { _cpyno = value; }
            get { return _cpyno; }
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CpyName
        {
            set { _cpyname = value; }
            get { return _cpyname; }
        }
        /// <summary>
        /// 公司类型
        /// </summary>
        public int CpyType
        {
            set { _cpytype = value; }
            get { return _cpytype; }
        }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperTime
        {
            set { _opertime = value; }
            get { return _opertime; }
        }
        /// <summary>
        /// 操作员登录名
        /// </summary>
        public string OperLoginName
        {
            set { _operloginname = value; }
            get { return _operloginname; }
        }
        /// <summary>
        /// 操作员名称
        /// </summary>
        public string OperUserName
        {
            set { _operusername = value; }
            get { return _operusername; }
        }
        /// <summary>
        /// 预订内容
        /// </summary>
        public string SSContent
        {
            set { _sscontent = value; }
            get { return _sscontent; }
        }
        /// <summary>
        /// 航信返回内容
        /// </summary>
        public string ResultContent
        {
            set { _resultcontent = value; }
            get { return _resultcontent; }
        }
        /// <summary>
        /// 解析出的PNR编号
        /// </summary>
        public string PNR
        {
            set { _pnr = value; }
            get { return _pnr; }
        }
        /// <summary>
        /// 订座Office号
        /// </summary>
        public string OfficeCode
        {
            set { _officecode = value; }
            get { return _officecode; }
        }
        /// <summary>
        /// 是否已写入订单（0未写入，1已写入）
        /// </summary>
        public bool OrderFlag
        {
            set { _orderflag = value; }
            get { return _orderflag; }
        }
        /// <summary>
        /// 处理标志（0未处理，1已处理）
        /// </summary>
        public bool Flag
        {
            set { _flag = value; }
            get { return _flag; }
        }
        /// <summary>
        /// 尝试取消次数
        /// </summary>
        public int RetryCount
        {
            set { _retrycount = value; }
            get { return _retrycount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime A5
        {
            set { _a5 = value; }
            get { return _a5; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime A6
        {
            set { _a6 = value; }
            get { return _a6; }
        }
        /// <summary>
        /// 使用的IP端口 192.168.1.2|391
        /// </summary>
        public string A7
        {
            set { _a7 = value; }
            get { return _a7; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A8
        {
            set { _a8 = value; }
            get { return _a8; }
        }
        #endregion Model

        /// <summary>
        /// 字符串显示实体数据
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            StringBuilder sbLog = new StringBuilder();
            try
            {
                List<string> PropertyList = new List<string>();
                Type t = this.GetType();
                PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                object obj = null;
                foreach (PropertyInfo p in properties)
                {
                    obj = p.GetValue(this, null);
                    PropertyList.Add(p.Name + "=" + (obj == null ? "null" : obj));
                }
                if (PropertyList.Count > 0)
                {
                    sbLog.Append(string.Join("\r\n", PropertyList.ToArray()));
                }
            }
            catch (Exception)
            {
            }
            return sbLog.ToString();
        }

    }
}

