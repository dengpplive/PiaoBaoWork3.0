using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_Ticket_SkyWay:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_Ticket_SkyWay:ICloneable
    {
        public Tb_Ticket_SkyWay()
        { }
        #region Model
        private Guid _id;
        private string _orderid = "";
        private DateTime _fromdate = Convert.ToDateTime("1900-01-01");
        private DateTime _todate = Convert.ToDateTime("1900-01-01");
        private string _fromcityname = "";
        private string _fromcitycode = "";
        private string _tocityname = "";
        private string _tocitycode = "";
        private string _carrycode = "";
        private string _carryname = "";
        private string _flightcode = "";
        private string _space = "";
        private string _discount = "";
        private string _aircraft = "";
        private string _mileage = "";
        private decimal _abfee = 0M;
        private decimal _fuelfee = 0M;
        private decimal _farefee = 0M;
        private decimal _spaceprice;
        private decimal _yfarefee = 0m;
        private int _isshareflight = 0;
        private string _terminal = "";
        private string _pnrcontent = "";
        private string _newpnrcontent = "";
        private string _pat = "";
        private int _a1 = 0;
        private int _a2 = 0;
        private decimal _a3 = 0M;
        private decimal _a4 = 0M;
        private DateTime _a5 = Convert.ToDateTime("1900-01-01");
        private DateTime _a6 = Convert.ToDateTime("1900-01-01");
        private string _a7 = "";
        private string _a8 = "";
        private string _a9 = "";
        private string _a10 = "";
        /// <summary>
        /// 主键
        /// </summary>
        public Guid id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId
        {
            set { _orderid = value; }
            get { return _orderid; }
        }
        /// <summary>
        /// 起飞日期
        /// </summary>
        public DateTime FromDate
        {
            set { _fromdate = value; }
            get { return _fromdate; }
        }
        /// <summary>
        /// 到达日期
        /// </summary>
        public DateTime ToDate
        {
            set { _todate = value; }
            get { return _todate; }
        }
        /// <summary>
        /// 起飞城市
        /// </summary>
        public string FromCityName
        {
            set { _fromcityname = value; }
            get { return _fromcityname; }
        }
        /// <summary>
        /// 起飞城市三字码
        /// </summary>
        public string FromCityCode
        {
            set { _fromcitycode = value; }
            get { return _fromcitycode; }
        }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityName
        {
            set { _tocityname = value; }
            get { return _tocityname; }
        }
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCityCode
        {
            set { _tocitycode = value; }
            get { return _tocitycode; }
        }
        /// <summary>
        /// 承运人代码
        /// </summary>
        public string CarryCode
        {
            set { _carrycode = value; }
            get { return _carrycode; }
        }
        /// <summary>
        /// 承运人名称
        /// </summary>
        public string CarryName
        {
            set { _carryname = value; }
            get { return _carryname; }
        }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightCode
        {
            set { _flightcode = value; }
            get { return _flightcode; }
        }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Space
        {
            set { _space = value; }
            get { return _space; }
        }
        /// <summary>
        /// 折扣 0或者负数为特价
        /// </summary>
        public string Discount
        {
            set { _discount = value; }
            get { return _discount; }
        }
        /// <summary>
        /// 机型
        /// </summary>
        public string Aircraft
        {
            set { _aircraft = value; }
            get { return _aircraft; }
        }
        /// <summary>
        /// 里程
        /// </summary>
        public string Mileage
        {
            set { _mileage = value; }
            get { return _mileage; }
        }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal ABFee
        {
            set { _abfee = value; }
            get { return _abfee; }
        }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal FuelFee
        {
            set { _fuelfee = value; }
            get { return _fuelfee; }
        }
        /// <summary>
        /// 票价Y舱价格
        /// </summary>
        public decimal FareFee
        {
            set { _farefee = value; }
            get { return _farefee; }
        }
        /// <summary>
        /// 舱位价格
        /// </summary>
        public decimal SpacePrice
        {
            set { _spaceprice = value; }
            get { return _spaceprice; }
        }
        /// <summary>
        /// 是否为共享航班 
        /// </summary>
        public int IsShareFlight
        {
            set { _isshareflight = value; }
            get { return _isshareflight; }
        }
        /// <summary>
        /// 乘机及停靠航站楼
        /// </summary>
        public string Terminal
        {
            set { _terminal = value; }
            get { return _terminal; }
        }
        /// <summary>
        /// Pnr的原始内容
        /// </summary>
        public string PnrContent
        {
            set { _pnrcontent = value; }
            get { return _pnrcontent; }
        }
        /// <summary>
        /// 处理过后的PNR内容
        /// </summary>
        public string NewPnrContent
        {
            set { _newpnrcontent = value; }
            get { return _newpnrcontent; }
        }
        /// <summary>
        /// PAT的结果内容
        /// </summary>
        public string Pat
        {
            set { _pat = value; }
            get { return _pat; }
        }
        private string _OldPAT = string.Empty;
        /// <summary>
        /// 生僻航线修改基建或者燃油后原来PAT
        /// </summary>
        public string OldPAT
        {
            get
            {
                return _OldPAT;
            }
            set
            {
                _OldPAT = value;
            }
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


        #region ICloneable 成员

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        #endregion
    }
}

