using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_Ticket_Passenger:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_Ticket_Passenger : ICloneable
    {
        public Tb_Ticket_Passenger()
        { }
        #region Model
        private Guid _id;
        private string _orderid = "";
        private int _passengertype = 0;
        private string _passengername = "";
        private string _ctype = "";
        private string _cid = "";
        private string _setcode = "";
        private string _ticketnumber = "";
        private string _travelnumber = "";
        private decimal _pmfee = 0M;
        private decimal _abfee = 0M;
        private decimal _fuelfee = 0M;
        private decimal _tgqhandlingfee = 0M;
        private string _remark = "";
        private bool _suspended = false;
        private int _ticketstatus = 0;
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

        private bool _isback = false; // 退废改处理标示

        /// <summary>
        /// 退废改处理标示： 1 / true  不能提交，0 / false 可以提交
        /// </summary>
        public bool IsBack
        {
            set { _isback = value; }
            get { return _isback; }
        }

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
        /// 乘客类型 1=成人，2=儿童，3=婴儿
        /// </summary>
        public int PassengerType
        {
            set { _passengertype = value; }
            get { return _passengertype; }
        }
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName
        {
            set { _passengername = value; }
            get { return _passengername; }
        }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string CType
        {
            set { _ctype = value; }
            get { return _ctype; }
        }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string Cid
        {
            set { _cid = value; }
            get { return _cid; }
        }
        /// <summary>
        /// 结算码
        /// </summary>
        public string SetCode
        {
            set { _setcode = value; }
            get { return _setcode; }
        }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber
        {
            set { _ticketnumber = value; }
            get { return _ticketnumber; }
        }
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TravelNumber
        {
            set { _travelnumber = value; }
            get { return _travelnumber; }
        }

        private int _TravelStatus = 0;
        /// <summary>
        /// 行程单状态 0 未创建 1已创建 2已作废
        /// </summary>
        public int TravelStatus
        {
            get
            {
                return _TravelStatus;
            }
            set
            {
                _TravelStatus = value;
            }
        }
        /// <summary>
        /// 票面价
        /// </summary>
        public decimal PMFee
        {
            set { _pmfee = value; }
            get { return _pmfee; }
        }
        /// <summary>
        /// 机建
        /// </summary>
        public decimal ABFee
        {
            set { _abfee = value; }
            get { return _abfee; }
        }
        /// <summary>
        /// 燃油
        /// </summary>
        public decimal FuelFee
        {
            set { _fuelfee = value; }
            get { return _fuelfee; }
        }
        /// <summary>
        /// 退改签手续费
        /// </summary>
        public decimal TGQHandlingFee
        {
            set { _tgqhandlingfee = value; }
            get { return _tgqhandlingfee; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 机票挂起标志 （0=未挂，1=挂起）
        /// </summary>
        public bool Suspended
        {
            set { _suspended = value; }
            get { return _suspended; }
        }
        /// <summary>
        /// 机票状态 1.预订，2.出票，3.退票，4.废票，5.改签，6取消
        /// </summary>
        public int TicketStatus
        {
            set { _ticketstatus = value; }
            get { return _ticketstatus; }
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
        /// 儿童出生日期 2013-6-30 用于编码中Chld项标识
        /// </summary>
        public string A7
        {
            set { _a7 = value; }
            get { return _a7; }
        }
        /// <summary>
        /// 航空公司卡号 1111111111
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