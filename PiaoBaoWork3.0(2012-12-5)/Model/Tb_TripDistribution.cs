using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_TripDistribution:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_TripDistribution
    {
        public Tb_TripDistribution()
        { }
        #region Model
        private Guid _id;
        private string _ownercpyno = "";
        private string _ownercpyname = "";
        private DateTime _addtime = DateTime.Now;
        private string _addloginname = "";
        private string _addusername = "";
        private string _tripnum = "";
        private string _usecpyno = "";
        private string _usecpyname = "";
        private DateTime _usetime = Convert.ToDateTime("1900-01-01");
        private DateTime _printtime = Convert.ToDateTime("1900-01-01");
        private DateTime _invalidtime = Convert.ToDateTime("1900-01-01");
        private string _ticketnum = "";
        private int _tripstatus = 0;
        private string _createoffice = "";
        private string _iatacode = "";
        private string _companyname = "";
        private string _remark = "";
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
        public string OwnerCpyNo
        {
            set { _ownercpyno = value; }
            get { return _ownercpyno; }
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string OwnerCpyName
        {
            set { _ownercpyname = value; }
            get { return _ownercpyname; }
        }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime AddTime
        {
            set { _addtime = value; }
            get { return _addtime; }
        }
        /// <summary>
        /// 入库操作员登录名
        /// </summary>
        public string AddLoginName
        {
            set { _addloginname = value; }
            get { return _addloginname; }
        }
        /// <summary>
        /// 入库操作员名称
        /// </summary>
        public string AddUserName
        {
            set { _addusername = value; }
            get { return _addusername; }
        }
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TripNum
        {
            set { _tripnum = value; }
            get { return _tripnum; }
        }
        /// <summary>
        /// 领用公司编号
        /// </summary>
        public string UseCpyNo
        {
            set { _usecpyno = value; }
            get { return _usecpyno; }
        }
        /// <summary>
        /// 领用公司名称
        /// </summary>
        public string UseCpyName
        {
            set { _usecpyname = value; }
            get { return _usecpyname; }
        }
        /// <summary>
        /// 领用时间
        /// </summary>
        public DateTime UseTime
        {
            set { _usetime = value; }
            get { return _usetime; }
        }
        /// <summary>
        /// 创建打印时间
        /// </summary>
        public DateTime PrintTime
        {
            set { _printtime = value; }
            get { return _printtime; }
        }
        /// <summary>
        /// 作废时间
        /// </summary>
        public DateTime InvalidTime
        {
            set { _invalidtime = value; }
            get { return _invalidtime; }
        }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNum
        {
            set { _ticketnum = value; }
            get { return _ticketnum; }
        }
        /// <summary>
        /// 行程单状态
        /// </summary>
        public int TripStatus
        {
            set { _tripstatus = value; }
            get { return _tripstatus; }
        }
        /// <summary>
        /// 创建Office号
        /// </summary>
        public string CreateOffice
        {
            set { _createoffice = value; }
            get { return _createoffice; }
        }
        /// <summary>
        /// 航协号
        /// </summary>
        public string IataCode
        {
            set { _iatacode = value; }
            get { return _iatacode; }
        }
        /// <summary>
        /// 填开单位
        /// </summary>
        public string CompanyName
        {
            set { _companyname = value; }
            get { return _companyname; }
        }
        /// <summary>
        /// 行程单备注
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }

        /// <summary>
        ///用于查询排序 失败尝试次数
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
        #endregion Model

    }
}

