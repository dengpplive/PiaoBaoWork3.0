using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_Ticket_Policy:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_Ticket_Policy
    {
        public Tb_Ticket_Policy()
        { }
        #region Model
        private Guid _id;
        private string _cpyno = "";
        private string _cpyname = "";
        private int _policykind = 0;
        private int _generationtype = 0;
        private int _releasetype = 0;
        private string _carrycode = "";
        private int _traveltype = 0;
        private int _policytype = 0;
        private int _teamflag = 0;
        private string _office = "";
        private string _startcitynamecode = "";
        private int _startcitynamesame = 0;
        private string _middlecitynamecode = "";
        private int _middlecitynamesame = 0;
        private string _targetcitynamecode = "";
        private int _targetcitynamesame = 0;
        private int _applianceflighttype = 0;
        private string _applianceflight = "";
        private string _unapplianceflight = "";
        private string _scheduleconstraints = "";
        private string _shippingspace = "";
        private string _interpolicyid = "";
        private decimal _spaceprice = 0M;
        private decimal _referenceprice = 0M;
        private int _advanceday = 0;
        private decimal _airrebate = 0M;
        private decimal _airrebatereturnmoney = 0M;
        private decimal _downpoint = 0M;
        private decimal _downreturnmoney = 0M;
        private decimal _laterpoint = 0M;
        private decimal _laterreturnmoney = 0M;
        private decimal _sharepoint = 0M;
        private decimal _sharepointreturnmoney = 0M;
        private DateTime _flightstartdate = Convert.ToDateTime("1900-01-01");
        private DateTime _flightenddate = Convert.ToDateTime("1900-01-01");
        private DateTime _printstartdate = Convert.ToDateTime("1900-01-01");
        private DateTime _printenddate = Convert.ToDateTime("1900-01-01");
        private DateTime _auditdate = Convert.ToDateTime("1900-01-01");
        private int _audittype = 0;
        private string _auditloginname = "";
        private string _auditname = "";
        private DateTime _createdate = DateTime.Now;
        private string _createloginname = "";
        private string _createname = "";
        private DateTime _updatedate = DateTime.Now;
        private string _updateloginname = "";
        private string _updatename = "";
        private string _remark = "";
        private int _isapplytoshareflight = 0;
        private string _shareaircode = "";
        private int _isloweropen = 0;
        private int _highpolicyflag = 0;
        private int _autoprintflag = 0;
        private string _groupid = "";
        private int _IsPause = 0;//0解挂 1挂起
        private int _a1 = 0;
        private int _a2 = 0;
        private int _a3 = 0;
        private int _a4 = 0;
        private decimal _a5 = 0M;
        private decimal _a6 = 0M;
        private decimal _a7 = 0M;
        private decimal _a8 = 0M;
        private DateTime _a9 = Convert.ToDateTime("1900-01-01");
        private DateTime _a10 = Convert.ToDateTime("1900-01-01");
        private DateTime _a11 = Convert.ToDateTime("1900-01-01");
        private DateTime _a12 = Convert.ToDateTime("1900-01-01");
        private string _a13 = "";
        private string _a14 = "";
        private string _a15 = "";
        private string _a16 = "";
        #region 这里的字段(只加字段不加属性)数据库里没有,只是实体里使用,添加字段不会影响操作数据库的方法
        public string _WorkTime = "";//正常上班时间00:00-00:00
        public string _PolicyCancelTime = "";//废票时间
        public string _PolicyReturnTime = "";//退票时间
        public string _FPGQTime = "";// 废票改签时间 00:00-00:00
        public string _OldPolicyPoint = "0";// 原始政策
        public string _OldreturnMoney = "0";// 原始现返
        public string _PolicyPoint = "0";// 出票政策
        public string _returnMoney = "0";// 出票现返
        public string _DiscountDetail = "";//扣点明细(用户1^扣点点数^现返|用户2^扣点点数^现返|……)（a^0.1^1|）
        public int _orderByPolicy = 0;//政策排序用
        public decimal _patchPonit = 0M;//补点金额
        public decimal _AirPayMoney = 0m;//B2B航空公司政策支付金额
        #endregion
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
        /// 供应商名字
        /// </summary>
        public string CpyName
        {
            set { _cpyname = value; }
            get { return _cpyname; }
        }
        /// <summary>
        /// 政策种类 0.通用， 1.普通，2.特价
        /// </summary>
        public int PolicyKind
        {
            set { _policykind = value; }
            get { return _policykind; }
        }
        /// <summary>
        /// 票价生成方式 1.正常价格，2.动态特价，3.固定特价
        /// </summary>
        public int GenerationType
        {
            set { _generationtype = value; }
            get { return _generationtype; }
        }
        /// <summary>
        /// 发布类型 1.出港，2.入港,3.全国
        /// </summary>
        public int ReleaseType
        {
            set { _releasetype = value; }
            get { return _releasetype; }
        }
        /// <summary>
        /// 承运人 航空公司编号
        /// </summary>
        public string CarryCode
        {
            set { _carrycode = value; }
            get { return _carrycode; }
        }
        /// <summary>
        /// 行程类型 1.单程，2.往返/单程，3.往返，4.中转联程
        /// </summary>
        public int TravelType
        {
            set { _traveltype = value; }
            get { return _traveltype; }
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
        /// 团队标志 0.普通，1.团队
        /// </summary>
        public int TeamFlag
        {
            set { _teamflag = value; }
            get { return _teamflag; }
        }
        /// <summary>
        /// 出票Office号
        /// </summary>
        public string Office
        {
            set { _office = value; }
            get { return _office; }
        }
        /// <summary>
        /// 出发城市三字码（全国政策填：ALL）
        /// </summary>
        public string StartCityNameCode
        {
            set { _startcitynamecode = value; }
            get { return _startcitynamecode; }
        }
        /// <summary>
        /// 出发城市同城机场共享政策 1.是，2.否
        /// </summary>
        public int StartCityNameSame
        {
            set { _startcitynamesame = value; }
            get { return _startcitynamesame; }
        }
        /// <summary>
        /// 中转城市三字码
        /// </summary>
        public string MiddleCityNameCode
        {
            set { _middlecitynamecode = value; }
            get { return _middlecitynamecode; }
        }
        /// <summary>
        /// 中转城市同城机场共享政策 1.是，2.否
        /// </summary>
        public int MiddleCityNameSame
        {
            set { _middlecitynamesame = value; }
            get { return _middlecitynamesame; }
        }
        /// <summary>
        /// 到达城市三字码（全国政策填：ALL）
        /// </summary>
        public string TargetCityNameCode
        {
            set { _targetcitynamecode = value; }
            get { return _targetcitynamecode; }
        }
        /// <summary>
        /// 到达城市同城机场共享政策 1.是，2.否
        /// </summary>
        public int TargetCityNameSame
        {
            set { _targetcitynamesame = value; }
            get { return _targetcitynamesame; }
        }
        /// <summary>
        /// 适用航班号类型 1.全部2.适用3.不适用
        /// </summary>
        public int ApplianceFlightType
        {
            set { _applianceflighttype = value; }
            get { return _applianceflighttype; }
        }
        /// <summary>
        /// 适用航班
        /// </summary>
        public string ApplianceFlight
        {
            set { _applianceflight = value; }
            get { return _applianceflight; }
        }
        /// <summary>
        /// 不适用航班
        /// </summary>
        public string UnApplianceFlight
        {
            set { _unapplianceflight = value; }
            get { return _unapplianceflight; }
        }
        /// <summary>
        /// 班期限制 周一到周日
        /// </summary>
        public string ScheduleConstraints
        {
            set { _scheduleconstraints = value; }
            get { return _scheduleconstraints; }
        }
        /// <summary>
        /// 舱位
        /// </summary>
        public string ShippingSpace
        {
            set { _shippingspace = value; }
            get { return _shippingspace; }
        }

        /// <summary>
        /// 接口政策ID
        /// </summary>
        public string InterPolicyId
        {
            set { _interpolicyid = value; }
            get { return _interpolicyid; }
        }
        /// <summary>
        /// 舱位价格（固定特价）
        /// </summary>
        public decimal SpacePrice
        {
            set { _spaceprice = value; }
            get { return _spaceprice; }
        }
        /// <summary>
        /// 参考价格(特价)
        /// </summary>
        public decimal ReferencePrice
        {
            set { _referenceprice = value; }
            get { return _referenceprice; }
        }
        /// <summary>
        /// 提前天数
        /// </summary>
        public int AdvanceDay
        {
            set { _advanceday = value; }
            get { return _advanceday; }
        }
        /// <summary>
        /// 航空公司返点
        /// </summary>
        public decimal AirReBate
        {
            set { _airrebate = value; }
            get { return _airrebate; }
        }
        /// <summary>
        /// 航空公司现返金额
        /// </summary>
        public decimal AirReBateReturnMoney
        {
            set { _airrebatereturnmoney = value; }
            get { return _airrebatereturnmoney; }
        }
        /// <summary>
        /// 下级分销返点
        /// </summary>
        public decimal DownPoint
        {
            set { _downpoint = value; }
            get { return _downpoint; }
        }
        /// <summary>
        /// 下级分销现返金额
        /// </summary>
        public decimal DownReturnMoney
        {
            set { _downreturnmoney = value; }
            get { return _downreturnmoney; }
        }
        /// <summary>
        /// 下级分销后返
        /// </summary>
        public decimal LaterPoint
        {
            set { _laterpoint = value; }
            get { return _laterpoint; }
        }
        /// <summary>
        /// 下级分销后返现返金额
        /// </summary>
        public decimal LaterReturnMoney
        {
            set { _laterreturnmoney = value; }
            get { return _laterreturnmoney; }
        }
        /// <summary>
        /// 共享政策返点
        /// </summary>
        public decimal SharePoint
        {
            set { _sharepoint = value; }
            get { return _sharepoint; }
        }
        /// <summary>
        /// 共享政策现返金额
        /// </summary>
        public decimal SharePointReturnMoney
        {
            set { _sharepointreturnmoney = value; }
            get { return _sharepointreturnmoney; }
        }
        /// <summary>
        /// 乘机生效日期
        /// </summary>
        public DateTime FlightStartDate
        {
            set { _flightstartdate = value; }
            get { return _flightstartdate; }
        }
        /// <summary>
        /// 乘机失效日期
        /// </summary>
        public DateTime FlightEndDate
        {
            set { _flightenddate = value; }
            get { return _flightenddate; }
        }
        /// <summary>
        /// 出票生效日期
        /// </summary>
        public DateTime PrintStartDate
        {
            set { _printstartdate = value; }
            get { return _printstartdate; }
        }
        /// <summary>
        /// 出票失效日期
        /// </summary>
        public DateTime PrintEndDate
        {
            set { _printenddate = value; }
            get { return _printenddate; }
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditDate
        {
            set { _auditdate = value; }
            get { return _auditdate; }
        }
        /// <summary>
        /// 审核状态 1.已审，2.未审
        /// </summary>
        public int AuditType
        {
            set { _audittype = value; }
            get { return _audittype; }
        }
        /// <summary>
        /// 审核人帐户
        /// </summary>
        public string AuditLoginName
        {
            set { _auditloginname = value; }
            get { return _auditloginname; }
        }
        /// <summary>
        /// 审核人姓名
        /// </summary>
        public string AuditName
        {
            set { _auditname = value; }
            get { return _auditname; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            set { _createdate = value; }
            get { return _createdate; }
        }
        /// <summary>
        /// 创建人帐户
        /// </summary>
        public string CreateLoginName
        {
            set { _createloginname = value; }
            get { return _createloginname; }
        }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateName
        {
            set { _createname = value; }
            get { return _createname; }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            set { _updatedate = value; }
            get { return _updatedate; }
        }
        /// <summary>
        /// 更新人账户
        /// </summary>
        public string UpdateLoginName
        {
            set { _updateloginname = value; }
            get { return _updateloginname; }
        }
        /// <summary>
        /// 更新人姓名
        /// </summary>
        public string UpdateName
        {
            set { _updatename = value; }
            get { return _updatename; }
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
        /// 这条政策是否适用于共享航班 1适用 0不适用
        /// </summary>
        public int IsApplyToShareFlight
        {
            set { _isapplytoshareflight = value; }
            get { return _isapplytoshareflight; }
        }
        /// <summary>
        /// 适用共享航空公司二字码 如:CA/CZ/ZH/HU
        /// </summary>
        public string ShareAirCode
        {
            set { _shareaircode = value; }
            get { return _shareaircode; }
        }
        /// <summary>
        /// 是否往返低开 0不低开 1低开　
        /// </summary>
        public int IsLowerOpen
        {
            set { _isloweropen = value; }
            get { return _isloweropen; }
        }
        /// <summary>
        /// 是否高返政策 1是 其他不是
        /// </summary>
        public int HighPolicyFlag
        {
            set { _highpolicyflag = value; }
            get { return _highpolicyflag; }
        }
        /// <summary>
        /// 自动出票方式 手动(0或者null空)， 半自动1， 自动2
        /// </summary>
        public int AutoPrintFlag
        {
            set { _autoprintflag = value; }
            get { return _autoprintflag; }
        }
        /// <summary>
        /// 专属扣点组ID
        /// </summary>
        public string GroupId
        {
            set { _groupid = value; }
            get { return _groupid; }
        }
        /// <summary>
        ///政策挂起解挂  0解挂 1挂起
        /// </summary>
        public int IsPause
        {
            get
            {
                return _IsPause;
            }
            set
            {
                _IsPause = value;
            }
        }
        /// <summary>
        /// 默认政策 1成人默认政策 2儿童默认政策  默认0为非默认政策
        /// </summary>
        public int A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 政策来源方式(0:发布1：导入)
        /// </summary>
        public int A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A5
        {
            set { _a5 = value; }
            get { return _a5; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A6
        {
            set { _a6 = value; }
            get { return _a6; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A7
        {
            set { _a7 = value; }
            get { return _a7; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal A8
        {
            set { _a8 = value; }
            get { return _a8; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime A9
        {
            set { _a9 = value; }
            get { return _a9; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime A10
        {
            set { _a10 = value; }
            get { return _a10; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime A11
        {
            set { _a11 = value; }
            get { return _a11; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime A12
        {
            set { _a12 = value; }
            get { return _a12; }
        }
        /// <summary>
        /// b2b航空公司政策 1是 其他否
        /// </summary>
        public string A13
        {
            set { _a13 = value; }
            get { return _a13; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A14
        {
            set { _a14 = value; }
            get { return _a14; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A15
        {
            set { _a15 = value; }
            get { return _a15; }
        }
        /// <summary>
        /// A16临时启用字段(0和空代表要共享,1代表不共享)
        /// </summary>
        public string A16
        {
            set { _a16 = value; }
            get { return _a16; }
        }
        #endregion Model

    }
}

