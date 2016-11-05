using System;
namespace PbProject.Model
{
    /// <summary>
    /// Log_Tb_AirOrder:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Log_Tb_AirOrder
    {
        public Log_Tb_AirOrder()
        { }
        #region Model
        private Guid _id;
        private string _orderid = string.Empty;
        private string _opertype = string.Empty;
        private DateTime _opertime = Convert.ToDateTime("1900-01-01");
        private string _operloginname = string.Empty;
        private string _operusername = string.Empty;
        private string _cpyno = string.Empty;
        private int _cpytype = 0;
        private string _cpyname = string.Empty;
        private string _opercontent = string.Empty;
        private int _watchtype = 0;
        private int _a1 = 0;
        private int _a2 = 0;
        private decimal _a3 = 0m;
        private decimal _a4 = 0m;
        private DateTime _a5 = Convert.ToDateTime("1900-01-01");
        private DateTime _a6 = Convert.ToDateTime("1900-01-01");
        private string _a7 = string.Empty;
        private string _a8 = string.Empty;
        private string _a9 = string.Empty;
        private string _a10 = string.Empty;
        /// <summary>
        /// 主键
        /// </summary>
        public Guid id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId
        {
            set { _orderid = value; }
            get { return _orderid; }
        }
        /// <summary>
        /// 操作类型：预订、支付、出票、修改等。
        /// </summary>
        public string OperType
        {
            set { _opertype = value; }
            get { return _opertype; }
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
        /// 公司编号
        /// </summary>
        public string CpyNo
        {
            set { _cpyno = value; }
            get { return _cpyno; }
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
        /// 公司名称
        /// </summary>
        public string CpyName
        {
            set { _cpyname = value; }
            get { return _cpyname; }
        }
        /// <summary>
        /// 操作内容描述
        /// </summary>
        public string OperContent
        {
            set { _opercontent = value; }
            get { return _opercontent; }
        }
        /// <summary>
        /// 查看权限（1.平台 2.运营 3.供应 4.分销 5.采购）
        /// </summary>
        public int WatchType
        {
            set { _watchtype = value; }
            get { return _watchtype; }
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
        /// 
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
        /// <summary>
        /// 
        /// </summary>
        public string A9
        {
            set { _a9 = value; }
            get { return _a9; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A10
        {
            set { _a10 = value; }
            get { return _a10; }
        }
        #endregion Model

    }
}

