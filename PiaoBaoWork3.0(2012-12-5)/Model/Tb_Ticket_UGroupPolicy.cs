using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_Ticket_UGroupPolicy:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_Ticket_UGroupPolicy
    {
        public Tb_Ticket_UGroupPolicy()
        { }
        #region Model
        private Guid _id;
        private string _cpyno=string.Empty;
        private string _cpyname = string.Empty;
        private int _cpytype=0;
        private DateTime _opertime = Convert.ToDateTime("1901-01-01");
        private string _operloginname = string.Empty;
        private string _operusername = string.Empty;
        private string _aircode = string.Empty;
        private string _fromcitycode = string.Empty;
        private string _fromcityname = string.Empty;
        private string _tocitycode = string.Empty;
        private string _tocityname = string.Empty;
        private string _flightno = string.Empty;
        private string _class = string.Empty;
        private string _planetype = string.Empty;
        private int _policytype=0;
        private DateTime _flightstartdate = Convert.ToDateTime("1901-01-01");
        private DateTime _flightenddate = Convert.ToDateTime("1901-01-01");
        private DateTime _printstartdate = Convert.ToDateTime("1901-01-01");
        private DateTime _printenddate = Convert.ToDateTime("1901-01-01");
        private string _flighttime = string.Empty;
        private int _advancedays=0;
        private int _seatcount = 0;
        private bool _pricetype = false;
        private decimal _prices = 0m;
        private decimal _rebate = 0m;
        private decimal _oilprice = 0m;
        private decimal _buildprice = 0m;
        private decimal _airrebate = 0m;
        private decimal _downrebate = 0m;
        private decimal _laterrebate = 0m;
        private decimal _sharerebate = 0m;
        private string _officecode = string.Empty;
        private string _remarks = string.Empty;
        private int _a1 = 0;
        private int _a2 = 0;
        private decimal _a3 = 0m;
        private decimal _a4 = 0m;
        private DateTime _a5 = Convert.ToDateTime("1901-01-01");
        private DateTime _a6 = Convert.ToDateTime("1901-01-01");
        private string _a7 = string.Empty;
        private string _a8 = string.Empty;
        /// <summary>
        /// Id主键
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
        /// 航空公司代码
        /// </summary>
        public string AirCode
        {
            set { _aircode = value; }
            get { return _aircode; }
        }
        /// <summary>
        /// 出发城市代码
        /// </summary>
        public string FromCityCode
        {
            set { _fromcitycode = value; }
            get { return _fromcitycode; }
        }
        /// <summary>
        /// 出发城市名称
        /// </summary>
        public string FromCityName
        {
            set { _fromcityname = value; }
            get { return _fromcityname; }
        }
        /// <summary>
        /// 到达城市代码
        /// </summary>
        public string ToCityCode
        {
            set { _tocitycode = value; }
            get { return _tocitycode; }
        }
        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCityName
        {
            set { _tocityname = value; }
            get { return _tocityname; }
        }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNo
        {
            set { _flightno = value; }
            get { return _flightno; }
        }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Class
        {
            set { _class = value; }
            get { return _class; }
        }
        /// <summary>
        /// 机型
        /// </summary>
        public string PlaneType
        {
            set { _planetype = value; }
            get { return _planetype; }
        }
        /// <summary>
        /// 1=B2B，2=BSP，3=BSP/B2B
        /// </summary>
        public int PolicyType
        {
            set { _policytype = value; }
            get { return _policytype; }
        }
        /// <summary>
        /// 预订起始日期
        /// </summary>
        public DateTime FlightStartDate
        {
            set { _flightstartdate = value; }
            get { return _flightstartdate; }
        }
        /// <summary>
        /// 预订截止日期
        /// </summary>
        public DateTime FlightEndDate
        {
            set { _flightenddate = value; }
            get { return _flightenddate; }
        }
        /// <summary>
        /// 出票起始日期
        /// </summary>
        public DateTime PrintStartDate
        {
            set { _printstartdate = value; }
            get { return _printstartdate; }
        }
        /// <summary>
        /// 出票截止日期
        /// </summary>
        public DateTime PrintEndDate
        {
            set { _printenddate = value; }
            get { return _printenddate; }
        }
        /// <summary>
        /// 起飞抵达时间
        /// </summary>
        public string FlightTime
        {
            set { _flighttime = value; }
            get { return _flighttime; }
        }
        /// <summary>
        /// 提前天数
        /// </summary>
        public int AdvanceDays
        {
            set { _advancedays = value; }
            get { return _advancedays; }
        }
        /// <summary>
        /// 座位数
        /// </summary>
        public int SeatCount
        {
            set { _seatcount = value; }
            get { return _seatcount; }
        }
        /// <summary>
        /// 优惠类型（false=折扣，true=价格）
        /// </summary>
        public bool PriceType
        {
            set { _pricetype = value; }
            get { return _pricetype; }
        }
        /// <summary>
        /// 票价
        /// </summary>
        public decimal Prices
        {
            set { _prices = value; }
            get { return _prices; }
        }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Rebate
        {
            set { _rebate = value; }
            get { return _rebate; }
        }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal OilPrice
        {
            set { _oilprice = value; }
            get { return _oilprice; }
        }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal BuildPrice
        {
            set { _buildprice = value; }
            get { return _buildprice; }
        }
        /// <summary>
        /// 航空公司返点
        /// </summary>
        public decimal AirRebate
        {
            set { _airrebate = value; }
            get { return _airrebate; }
        }
        /// <summary>
        /// 下级返点
        /// </summary>
        public decimal DownRebate
        {
            set { _downrebate = value; }
            get { return _downrebate; }
        }
        /// <summary>
        /// 下级后返
        /// </summary>
        public decimal LaterRebate
        {
            set { _laterrebate = value; }
            get { return _laterrebate; }
        }
        /// <summary>
        /// 共享返点
        /// </summary>
        public decimal ShareRebate
        {
            set { _sharerebate = value; }
            get { return _sharerebate; }
        }
        /// <summary>
        /// 出票Office号
        /// </summary>
        public string OfficeCode
        {
            set { _officecode = value; }
            get { return _officecode; }
        }
        /// <summary>
        /// 出票备注
        /// </summary>
        public string Remarks
        {
            set { _remarks = value; }
            get { return _remarks; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public int A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public int A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public decimal A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public decimal A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public DateTime A5
        {
            set { _a5 = value; }
            get { return _a5; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public DateTime A6
        {
            set { _a6 = value; }
            get { return _a6; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public string A7
        {
            set { _a7 = value; }
            get { return _a7; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public string A8
        {
            set { _a8 = value; }
            get { return _a8; }
        }
        #endregion Model

    }
}

