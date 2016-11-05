using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_Ticket_Order:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_Ticket_Order : ICloneable
    {
        public Tb_Ticket_Order()
        { }
        #region Model
        private Guid _id;
        private string _orderid = "";
        private string _oldorderid = "";
        private int _ordersourcetype = 0;
        private string _pnr = "";
        private string _bigcode = "";
        private string _office = "";
        private string _printoffice = "";
        private DateTime _createtime = DateTime.Now;
        private string _createloginname = "";
        private string _createusername = "";
        private string _createcpyno = "";
        private string _createcpyname = "";
        private string _ownercpyno = "";
        private string _ownercpyname = "";
        private DateTime _cptime = Convert.ToDateTime("1900-01-01");
        private string _cploginname = "";
        private string _cpname = "";
        private string _cpcpyno = "";
        private string _cpcpyname = "";
        private DateTime _paytime = Convert.ToDateTime("1900-01-01");
        private int _paystatus = 0;
        private int _payway = 0;
        private int _orderstatuscode = 0;
        private int _traveltype = 0;
        private string _travel = "";
        private string _travelcode = "";
        private DateTime _airtime = Convert.ToDateTime("1900-01-01");
        private string _carrycode = "";
        private string _flightcode = "";
        private string _space = "";
        private string _discount = "";
        private int _passengernumber = 0;
        private string _passengername = "";
        private string _linkman = "";
        private string _linkmanphone = "";
        private decimal _abfee = 0M;
        private decimal _fuelfee = 0M;
        private decimal _pmfee = 0M;
        private decimal _babyfee = 0M;
        private string _policyid = "";
        private decimal _airpoint = 0M;
        private decimal _policypoint = 0M;
        private decimal _returnpoint = 0M;
        private decimal _laterpoint = 0M;
        private decimal _returnmoney = 0M;
        private decimal _policymoney = 0M;
        private decimal _handlingrate;
        private decimal _handlingmoney = 0M;
        private decimal _paymoney = 0M;
        private decimal _ordermoney = 0M;
        private string _discountdetail;
        private string _policyremark = "";
        private int _policytype = 0;
        private decimal _tgqhandlingfee = 0M;
        private string _tgqapplyreason = "";
        private string _tgqrefusalreason = "";
        private string _lockloginname = "";
        private string _lockcpyno = "";
        private DateTime _locktime = Convert.ToDateTime("1900-01-01");
        private string _changepnr = "";
        private bool _allowchangepnrflag = true;
        private string _ydremark = "";
        private string _cpremark = "";
        private int _policysource = 0;
        private string _associationorder = "";
        private string _policycanceltime = "";
        private string _policyreturntime = "";
        private string _outorderid = "";
        private bool _outorderpayflag = false;
        private decimal _outorderpaymoney = 0M;
        private string _outorderpayno = "";
        private string _inpayno = "";
        private string _payno = "";
        private bool _debtspayflag = false;
        private decimal _paydebtsmoney = 0M;
        private int _autoprintflag = 0;
        private int _autoprinttimes = 0;
        private int _outorderrefundflag = 0;
        private decimal _outorderrefundmoney = 0M;
        private bool _ischdflag = false;
        private bool _havebabyflag = false;
        private string _jinrigycode = "";
        private string _babypatcontent = "";
        private string _kegui = "";
        private int _a1 = 0;
        private int _a2 = 0;
        private int _a3 = 0;
        private DateTime _a4 = Convert.ToDateTime("1900-01-01");
        private DateTime _a5 = Convert.ToDateTime("1900-01-01");
        private decimal _a6 = 0M;
        private decimal _a7 = 0M;
        private string _a8 = "";
        private string _a9 = "";
        private string _a10 = "";
        private decimal _a11 = 0M;
        private decimal _a12 = 0M;
        private decimal _a13 = 0M;
        private decimal _a14 = 0M;
        private decimal _a15 = 0M;
        private int _ticketStatus = 0;
        private decimal _OldPolicyPoint = 0m;
        private decimal _OldReturnMoney = 0m;
        private int _IsCHDETAdultTK = 0;
        private string _CHDToAdultPat = "";


        #region 临时属性数据库不存在该字段 用于显示
        private string _suppendstatus = "";
        /// <summary>
        /// 用于订单列表显示 只用于查询
        /// </summary>       
        [SetModelProperty(false, false, false, false, true)]
        public string SuppendStatus
        {
            get
            {
                return _suppendstatus;

            }
            set
            {
                _suppendstatus = value;
            }
        }

        private int _OrderLeayTime = 0;
        /// <summary>
        /// 订单时长
        /// </summary>       
        [SetModelProperty(false, false, false, false, true)]
        public int OrderLeayTime
        {
            get
            {
                return _OrderLeayTime;

            }
            set
            {
                _OrderLeayTime = value;
            }
        }

        private string _NewCpCpyName = string.Empty;
        /// <summary>
        /// 订单时长
        /// </summary>       
        [SetModelProperty(false, false, false, false, true)]
        public string NewCpCpyName
        {
            get
            {
                return _NewCpCpyName;

            }
            set
            {
                _NewCpCpyName = value;
            }
        }
        #endregion

        /// <summary>
        ///  机票状态: 1.预订，2.出票，3.退票，4.废票，5.改签，6取消
        /// </summary>
        public int TicketStatus
        {
            set { _ticketStatus = value; }
            get { return _ticketStatus; }
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
        /// 原始订单ID
        /// </summary>
        public string OldOrderId
        {
            set { _oldorderid = value; }
            get { return _oldorderid; }
        }
        /// <summary>
        /// 订单来源类型  1=白屏预订，2=PNR编码导入，3=PNR内容生成 ，4=线下订单 ,5 =票宝对外接口订单 6.PNR编码导入(运营商) 7.PNR内容导入(运营商) 8.PNR编码导入(系统) 9.PNR内容导入(系统) 10.PNR导入(升舱换开),11.PNR入库记账(运营商),12.PNR入库记账(系统) 13.抢票订单
        /// </summary>
        public int OrderSourceType
        {
            set { _ordersourcetype = value; }
            get { return _ordersourcetype; }
        }
        /// <summary>
        /// PNR
        /// </summary>
        public string PNR
        {
            set { _pnr = value; }
            get { return _pnr; }
        }
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigCode
        {
            set { _bigcode = value; }
            get { return _bigcode; }
        }
        /// <summary>
        /// 预订Office号
        /// </summary>
        public string Office
        {
            set { _office = value; }
            get { return _office; }
        }
        /// <summary>
        /// 出票Office号
        /// </summary>
        public string PrintOffice
        {
            set { _printoffice = value; }
            get { return _printoffice; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 创建人登录帐号
        /// </summary>
        public string CreateLoginName
        {
            set { _createloginname = value; }
            get { return _createloginname; }
        }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateUserName
        {
            set { _createusername = value; }
            get { return _createusername; }
        }
        /// <summary>
        /// 创建公司编号
        /// </summary>
        public string CreateCpyNo
        {
            set { _createcpyno = value; }
            get { return _createcpyno; }
        }
        /// <summary>
        /// 创建公司名称
        /// </summary>
        public string CreateCpyName
        {
            set { _createcpyname = value; }
            get { return _createcpyname; }
        }
        /// <summary>
        /// 订单所属公司编号
        /// </summary>
        public string OwnerCpyNo
        {
            set { _ownercpyno = value; }
            get { return _ownercpyno; }
        }
        /// <summary>
        /// 订单所属公司名称
        /// </summary>
        public string OwnerCpyName
        {
            set { _ownercpyname = value; }
            get { return _ownercpyname; }
        }
        /// <summary>
        /// 出票时间
        /// </summary>
        public DateTime CPTime
        {
            set { _cptime = value; }
            get { return _cptime; }
        }
        /// <summary>
        /// 出票人登录帐号
        /// </summary>
        public string CPLoginName
        {
            set { _cploginname = value; }
            get { return _cploginname; }
        }
        /// <summary>
        /// 出票人姓名
        /// </summary>
        public string CPName
        {
            set { _cpname = value; }
            get { return _cpname; }
        }
        /// <summary>
        /// 出票公司编号
        /// </summary>
        public string CPCpyNo
        {
            set { _cpcpyno = value; }
            get { return _cpcpyno; }
        }
        /// <summary>
        /// 出票公司名称
        /// </summary>
        public string CPCpyName
        {
            set { _cpcpyname = value; }
            get { return _cpcpyname; }
        }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayTime
        {
            set { _paytime = value; }
            get { return _paytime; }
        }
        /// <summary>
        /// 支付状态:（0=未付，1=已付）
        /// </summary>
        public int PayStatus
        {
            set { _paystatus = value; }
            get { return _paystatus; }
        }
        /// <summary>
        /// PayWay 支付方式：1支付宝、2快钱、3汇付、4财付通、5支付宝网银、6快钱网银 、7汇付网银、8财付通网银、9支付宝pos、10快钱pos、11汇付pos、12财付通pos、13易宝pos、14账户支付、15收银
        /// </summary>
        public int PayWay
        {
            set { _payway = value; }
            get { return _payway; }
        }
        /// <summary>
        /// 订单状态编号  见字典表
        /// </summary>
        public int OrderStatusCode
        {
            set { _orderstatuscode = value; }
            get { return _orderstatuscode; }
        }
        /// <summary>
        /// 行程类型 1=单程，2=往返，3=中转联程 4=多程
        /// </summary>
        public int TravelType
        {
            set { _traveltype = value; }
            get { return _traveltype; }
        }
        /// <summary>
        /// 行程
        /// </summary>
        public string Travel
        {
            set { _travel = value; }
            get { return _travel; }
        }
        /// <summary>
        /// 行程三字码
        /// </summary>
        public string TravelCode
        {
            set { _travelcode = value; }
            get { return _travelcode; }
        }
        /// <summary>
        /// 乘机日期 
        /// </summary>
        public DateTime AirTime
        {
            set { _airtime = value; }
            get { return _airtime; }
        }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarryCode
        {
            set { _carrycode = value; }
            get { return _carrycode; }
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
        /// 折扣 
        /// </summary>
        public string Discount
        {
            set { _discount = value; }
            get { return _discount; }
        }
        /// <summary>
        /// 乘客数
        /// </summary>
        public int PassengerNumber
        {
            set { _passengernumber = value; }
            get { return _passengernumber; }
        }
        /// <summary>
        /// 乘客姓名 以|分隔
        /// </summary>
        public string PassengerName
        {
            set { _passengername = value; }
            get { return _passengername; }
        }
        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan
        {
            set { _linkman = value; }
            get { return _linkman; }
        }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string LinkManPhone
        {
            set { _linkmanphone = value; }
            get { return _linkmanphone; }
        }
        /// <summary>
        /// 机建费（合计）
        /// </summary>
        public decimal ABFee
        {
            set { _abfee = value; }
            get { return _abfee; }
        }
        /// <summary>
        /// 燃油费（合计）
        /// </summary>
        public decimal FuelFee
        {
            set { _fuelfee = value; }
            get { return _fuelfee; }
        }
        /// <summary>
        /// 票面价（合计）
        /// </summary>
        public decimal PMFee
        {
            set { _pmfee = value; }
            get { return _pmfee; }
        }
        /// <summary>
        /// 婴儿票面价
        /// </summary>
        public decimal BabyFee
        {
            set { _babyfee = value; }
            get { return _babyfee; }
        }
        /// <summary>
        /// 政策ID
        /// </summary>
        public string PolicyId
        {
            set { _policyid = value; }
            get { return _policyid; }
        }
        /// <summary>
        /// 航空公司返点
        /// </summary>
        public decimal AirPoint
        {
            set { _airpoint = value; }
            get { return _airpoint; }
        }
        /// <summary>
        /// 原始政策返点 订单用来显示
        /// </summary>
        public decimal OldPolicyPoint
        {
            get { return _OldPolicyPoint; }
            set { _OldPolicyPoint = value; }
        }

        /// <summary>
        /// 出票政策点数 用来计算
        /// </summary>
        public decimal PolicyPoint
        {
            set { _policypoint = value; }
            get { return _policypoint; }
        }
        /// <summary>
        /// 原始政策现返 订单用来显示
        /// </summary>
        public decimal OldReturnMoney
        {
            get { return _OldReturnMoney; }
            set { _OldReturnMoney = value; }
        }
        /// <summary>
        /// 出票政策现返 用来计算
        /// </summary>
        public decimal ReturnMoney
        {
            set { _returnmoney = value; }
            get { return _returnmoney; }
        }

        /// <summary>
        /// 实际返点（扣点后）
        /// </summary>
        public decimal ReturnPoint
        {
            set { _returnpoint = value; }
            get { return _returnpoint; }
        }
        /// <summary>
        /// 后返点
        /// </summary>
        public decimal LaterPoint
        {
            set { _laterpoint = value; }
            get { return _laterpoint; }
        }


        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal PolicyMoney
        {
            set { _policymoney = value; }
            get { return _policymoney; }
        }
        /// <summary>
        /// 手续费费率
        /// </summary>
        public decimal HandlingRate
        {
            set { _handlingrate = value; }
            get { return _handlingrate; }
        }
        /// <summary>
        /// 总交易手续费
        /// </summary>
        public decimal HandlingMoney
        {
            set { _handlingmoney = value; }
            get { return _handlingmoney; }
        }
        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal PayMoney
        {
            set { _paymoney = value; }
            get { return _paymoney; }
        }

        /// <summary>
        ///  出票方收款金额
        /// </summary>
        public decimal OrderMoney
        {
            set { _ordermoney = value; }
            get { return _ordermoney; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DiscountDetail
        {
            set { _discountdetail = value; }
            get { return _discountdetail; }
        }
        /// <summary>
        /// 政策备注
        /// </summary>
        public string PolicyRemark
        {
            set { _policyremark = value; }
            get { return _policyremark; }
        }
        /// <summary>
        /// 政策类型 1=B2B，2=BSP，3=B2B/BSP
        /// </summary>
        public int PolicyType
        {
            set { _policytype = value; }
            get { return _policytype; }
        }
        /// <summary>
        /// 退改签手续费(总费用)
        /// </summary>
        public decimal TGQHandlingFee
        {
            set { _tgqhandlingfee = value; }
            get { return _tgqhandlingfee; }
        }
        /// <summary>
        /// 退改签申请理由
        /// </summary>
        public string TGQApplyReason
        {
            set { _tgqapplyreason = value; }
            get { return _tgqapplyreason; }
        }
        /// <summary>
        /// 退改签拒绝理由
        /// </summary>
        public string TGQRefusalReason
        {
            set { _tgqrefusalreason = value; }
            get { return _tgqrefusalreason; }
        }
        /// <summary>
        /// 锁定帐户
        /// </summary>
        public string LockLoginName
        {
            set { _lockloginname = value; }
            get { return _lockloginname; }
        }
        /// <summary>
        /// 锁定帐号所属公司编号
        /// </summary>
        public string LockCpyNo
        {
            set { _lockcpyno = value; }
            get { return _lockcpyno; }
        }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime LockTime
        {
            set { _locktime = value; }
            get { return _locktime; }
        }
        /// <summary>
        /// 换编码出票编码
        /// </summary>
        public string ChangePNR
        {
            set { _changepnr = value; }
            get { return _changepnr; }
        }
        /// <summary>
        /// 是否允许换编码出票（0=不允许，1=允许）
        /// </summary>
        public bool AllowChangePNRFlag
        {
            set { _allowchangepnrflag = value; }
            get { return _allowchangepnrflag; }
        }
        /// <summary>
        /// （订票备注）预订时备注信息
        /// </summary>
        public string YDRemark
        {
            set { _ydremark = value; }
            get { return _ydremark; }
        }
        /// <summary>
        /// （出票备注）出票时备注信息
        /// </summary>
        public string CPRemark
        {
            set { _cpremark = value; }
            get { return _cpremark; }
        }
        /// <summary>
        /// 订单来源：1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享,10易行
        /// </summary>
        public int PolicySource
        {
            set { _policysource = value; }
            get { return _policysource; }
        }
        /// <summary>
        /// 关联订单号（儿童）
        /// </summary>
        public string AssociationOrder
        {
            set { _associationorder = value; }
            get { return _associationorder; }
        }
        /// <summary>
        /// 废票时间
        /// </summary>
        public string PolicyCancelTime
        {
            set { _policycanceltime = value; }
            get { return _policycanceltime; }
        }
        /// <summary>
        /// 退票时间
        /// </summary>
        public string PolicyReturnTime
        {
            set { _policyreturntime = value; }
            get { return _policyreturntime; }
        }
        /// <summary>
        /// 外部订单编号
        /// </summary>
        public string OutOrderId
        {
            set { _outorderid = value; }
            get { return _outorderid; }
        }
        /// <summary>
        /// 外部订单代付标志（0=未代付，1=已代付）
        /// </summary>
        public bool OutOrderPayFlag
        {
            set { _outorderpayflag = value; }
            get { return _outorderpayflag; }
        }
        /// <summary>
        /// 外部订单代付金额
        /// </summary>
        public decimal OutOrderPayMoney
        {
            set { _outorderpaymoney = value; }
            get { return _outorderpaymoney; }
        }
        /// <summary>
        /// 外部订单支付流水号
        /// </summary>
        public string OutOrderPayNo
        {
            set { _outorderpayno = value; }
            get { return _outorderpayno; }
        }
        /// <summary>
        /// 内部交易流水号
        /// </summary>
        public string InPayNo
        {
            set { _inpayno = value; }
            get { return _inpayno; }
        }
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PayNo
        {
            set { _payno = value; }
            get { return _payno; }
        }
        /// <summary>
        /// 欠款还清标志（1=未还清，0=已还清）
        /// </summary>
        public bool DebtsPayFlag
        {
            set { _debtspayflag = value; }
            get { return _debtspayflag; }
        }
        /// <summary>
        /// 欠款金额
        /// </summary>
        public decimal PayDebtsMoney
        {
            set { _paydebtsmoney = value; }
            get { return _paydebtsmoney; }
        }
        /// <summary>
        /// 自动出票标志（0=手动，1=半自动，2=全自动）
        /// </summary>
        public int AutoPrintFlag
        {
            set { _autoprintflag = value; }
            get { return _autoprintflag; }
        }
        /// <summary>
        /// 自动出票尝试次数
        /// </summary>
        public int AutoPrintTimes
        {
            set { _autoprinttimes = value; }
            get { return _autoprinttimes; }
        }
        /// <summary>
        /// 外部订单是否已经退废票标志（0=未退废，1=已退废）
        /// </summary>
        public int OutOrderRefundFlag
        {
            set { _outorderrefundflag = value; }
            get { return _outorderrefundflag; }
        }
        /// <summary>
        /// 外部订单退废票金额
        /// </summary>
        public decimal OutOrderRefundMoney
        {
            set { _outorderrefundmoney = value; }
            get { return _outorderrefundmoney; }
        }
        /// <summary>
        /// 是否儿童订单（0=成人订单，1=儿童订单）
        /// </summary>
        public bool IsChdFlag
        {
            set { _ischdflag = value; }
            get { return _ischdflag; }
        }
        /// <summary>
        /// 是否有婴儿（0=无婴儿，1=有婴儿）
        /// </summary>
        public bool HaveBabyFlag
        {
            set { _havebabyflag = value; }
            get { return _havebabyflag; }
        }
        /// <summary>
        /// 有婴儿时存储婴儿Pat内容数据
        /// </summary>
        public string BabyPatContent
        {
            set { _babypatcontent = value; }
            get { return _babypatcontent; }
        }
        /// <summary>
        /// 今日供应商代码
        /// </summary>
        public string JinriGYCode
        {
            set { _jinrigycode = value; }
            get { return _jinrigycode; }
        }
        /// <summary>
        /// 客规
        /// </summary>
        public string KeGui
        {
            set { _kegui = value; }
            get { return _kegui; }
        }
        /// <summary>
        /// 是否儿童出成人票默认0否 1是
        /// </summary>
        public int IsCHDETAdultTK
        {
            get
            {
                return _IsCHDETAdultTK;
            }
            set
            {
                _IsCHDETAdultTK = value;
            }
        }
        /// <summary>
        /// 儿童出成人票 儿童PAT内容
        /// </summary>
        public string CHDToAdultPat
        {
            get
            {
                return _CHDToAdultPat;
            }
            set
            {
                _CHDToAdultPat = value;
            }
        }
        private int _IsUpdatePAT = 0;
        /// <summary>
        /// 是否生僻航线修改基建或者燃油 1是 0否
        /// </summary>
        public int IsUpdatePAT
        {
            get
            {
                return _IsUpdatePAT;
            }
            set
            {
                _IsUpdatePAT = value;
            }
        }

        private int _RobTicketStatus = 0;
        /// <summary>
        /// 抢票状态 0.没有抢票(默认) 1.抢票中 2.抢票成功 3.抢票失败
        /// </summary>
        public int RobTicketStatus
        {
            get
            {
                return _RobTicketStatus;
            }
            set
            {
                _RobTicketStatus = value;
            }
        }

        private int _RobTicketCount = 0;
        /// <summary>
        /// 抢票尝试次数
        /// </summary>
        public int RobTicketCount
        {
            get
            {
                return _RobTicketCount;
            }
            set
            {
                _RobTicketCount = value;
            }
        }

        private string _RobOrderId = string.Empty;
        /// <summary>
        /// 抢票订单关联的原订单号
        /// </summary>
        public string RobOrderId
        {
            get
            {
                return _RobOrderId;
            }
            set
            {
                _RobOrderId = value;
            }
        }

        private DateTime _LastScanTime = Convert.ToDateTime("1900-01-01");
        /// <summary>
        /// 上次抢票扫描时间
        /// </summary>
        public DateTime LastScanTime
        {
            get
            {
                return _LastScanTime;
            }
            set
            {
                _LastScanTime = value;
            }
        }

        /// <summary>
        /// 1.为确认订单 0其他未确认
        /// </summary>
        public int A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 政策种类  0.通用， 1.普通，2.特价
        /// </summary>
        public int A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 后返状态标示： 0 默认值、1 已经后返、2 已退后返
        /// </summary>
        public int A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 航空公司退款时间
        /// </summary>
        public DateTime A4
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
        ///  航空公司或接口退款金额
        /// </summary>
        public decimal A6
        {
            set { _a6 = value; }
            get { return _a6; }
        }
        /// <summary>
        /// (代付)返点（用于显示）
        /// </summary>
        public decimal A7
        {
            set { _a7 = value; }
            get { return _a7; }
        }
        /// <summary>
        /// 0.非自愿,1.自愿
        /// </summary>
        public string A8
        {
            set { _a8 = value; }
            get { return _a8; }
        }
        /// <summary>
        ///如果改值为： 1 支付时、退废票 不用检查编码
        /// </summary>
        public string A9
        {
            set { _a9 = value; }
            get { return _a9; }
        }
        /// <summary>
        /// 是否已经发出自动出票请求(winform端)。1为已经发送，0或空为未发送
        /// </summary>
        public string A10
        {
            set { _a10 = value; }
            get { return _a10; }
        }
        /// <summary>
        /// 补点金额
        /// </summary>
        public decimal A11
        {
            set { _a11 = value; }
            get { return _a11; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A12
        {
            set { _a12 = value; }
            get { return _a12; }
        }
        /// <summary>
        /// 后返金额
        /// </summary>
        public decimal A13
        {
            set { _a13 = value; }
            get { return _a13; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A14
        {
            set { _a14 = value; }
            get { return _a14; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A15
        {
            set { _a15 = value; }
            get { return _a15; }
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

