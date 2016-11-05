using System;
namespace PbProject.Model
{
    /// <summary>
    /// User_Company:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class User_Company
    {
        public User_Company()
        { }
        #region Model
        private Guid _id;
        private string _unincode = "";
        private string _uninallname = "";
        private string _uninname = "";
        private string _contactuser = "";
        private string _contacttel = "";
        private string _fax = "";
        private DateTime _createtime = DateTime.Now;
        private string _provice = "";
        private string _city = "";
        private string _uninaddress = "";
        private string _email = "";
        private string _website = "";
        private int _accountstate = 0;
        private string _remark = "";
        private DateTime _takeeffectdate = DateTime.Now;
        private DateTime _invalidationdate = DateTime.Now;
        private int _accountcount = 0;
        private string _tel = "";
        private int _roletype = 0;
        private decimal _accountmoney = 0M;
        private decimal _maxdebtmoney = 0M;
        private int _maxdebtdays = 0;
        private string _worktime;
        private string _businesstime;
        private string _groupid;
        private int _isprompt = 0;
        private int _isempprompt = 0;
        private int _prompttime = 5;
        private string _a1 = "";
        private string _a2 = "";
        private string _a3 = "";
        private string _a4 = "";

        private string _accountpwd = "";

        /// <summary>
        /// 账号余额支付密码
        /// </summary>
        public string AccountPwd
        {
            set { _accountpwd = value; }
            get { return _accountpwd; }
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
        /// 公司编号
        /// </summary>
        public string UninCode
        {
            set { _unincode = value; }
            get { return _unincode; }
        }
        /// <summary>
        /// 单位全称
        /// </summary>
        public string UninAllName
        {
            set { _uninallname = value; }
            get { return _uninallname; }
        }
        /// <summary>
        /// 单位简称
        /// </summary>
        public string UninName
        {
            set { _uninname = value; }
            get { return _uninname; }
        }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactUser
        {
            set { _contactuser = value; }
            get { return _contactuser; }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactTel
        {
            set { _contacttel = value; }
            get { return _contacttel; }
        }
        /// <summary>
        /// 传真
        /// </summary>
        public string Fax
        {
            set { _fax = value; }
            get { return _fax; }
        }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 省份
        /// </summary>
        public string Provice
        {
            set { _provice = value; }
            get { return _provice; }
        }
        /// <summary>
        /// 市
        /// </summary>
        public string City
        {
            set { _city = value; }
            get { return _city; }
        }
        /// <summary>
        /// 单位地址
        /// </summary>
        public string UninAddress
        {
            set { _uninaddress = value; }
            get { return _uninaddress; }
        }
        /// <summary>
        /// email
        /// </summary>
        public string Email
        {
            set { _email = value; }
            get { return _email; }
        }
        /// <summary>
        /// 网址
        /// </summary>
        public string WebSite
        {
            set { _website = value; }
            get { return _website; }
        }
        /// <summary>
        /// 状态 0=禁用，1=启用
        /// </summary>
        public int AccountState
        {
            set { _accountstate = value; }
            get { return _accountstate; }
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
        /// 生效日期
        /// </summary>
        public DateTime TakeEffectDate
        {
            set { _takeeffectdate = value; }
            get { return _takeeffectdate; }
        }
        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime InvalidationDate
        {
            set { _invalidationdate = value; }
            get { return _invalidationdate; }
        }
        /// <summary>
        /// 账号个数
        /// </summary>
        public int AccountCount
        {
            set { _accountcount = value; }
            get { return _accountcount; }
        }
        /// <summary>
        /// 办公电话
        /// </summary>
        public string Tel
        {
            set { _tel = value; }
            get { return _tel; }
        }
        /// <summary>
        /// 角色类型 1=平台，2=落地运营商，3=供应商，4=分销商，5=采购商
        /// </summary>
        public int RoleType
        {
            set { _roletype = value; }
            get { return _roletype; }
        }
        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal AccountMoney
        {
            set { _accountmoney = value; }
            get { return _accountmoney; }
        }
        /// <summary>
        /// 最大欠款额度
        /// </summary>
        public decimal MaxDebtMoney
        {
            set { _maxdebtmoney = value; }
            get { return _maxdebtmoney; }
        }
        /// <summary>
        /// 最大欠款天数（账期）
        /// </summary>
        public int MaxDebtDays
        {
            set { _maxdebtdays = value; }
            get { return _maxdebtdays; }
        }
        /// <summary>
        /// 供应上下班时间
        /// </summary>
        public string WorkTime
        {
            set { _worktime = value; }
            get { return _worktime; }
        }
        /// <summary>
        /// 业务处理时间
        /// </summary>
        public string BusinessTime
        {
            set { _businesstime = value; }
            get { return _businesstime; }
        }
        /// <summary>
        /// 扣点组id
        /// </summary>
        public string GroupId
        {
            set { _groupid = value; }
            get { return _groupid; }
        }
        /// <summary>
        /// 是否开启订单提示 0=关闭，1=开启
        /// </summary>
        public int IsPrompt
        {
            set { _isprompt = value; }
            get { return _isprompt; }
        }
        /// <summary>
        /// 是否开启员工订单提示 0=关闭，1=开启
        /// </summary>
        public int IsEmpPrompt
        {
            set { _isempprompt = value; }
            get { return _isempprompt; }
        }
        /// <summary>
        /// 订单提醒时间间隔(秒)
        /// </summary>
        public int PromptTime
        {
            set { _prompttime = value; }
            get { return _prompttime; }
        }

        private int _robinnerTime = 60;
        /// <summary>
        /// 抢票持续时间 默认在60分钟内
        /// </summary>
        public int RobInnerTime
        {
            get
            {
                return _robinnerTime;
            }
            set
            {
                _robinnerTime = value;
            }
        }

        private string _robSetting = "";
        /// <summary>
        /// 抢票次数和时间间隔设置 第几次^时间   如:1^10|2^15
        /// </summary>
        public string RobSetting
        {
            get
            {
                return _robSetting;
            }
            set
            {
                _robSetting = value;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public string A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        #endregion Model

    }
}

