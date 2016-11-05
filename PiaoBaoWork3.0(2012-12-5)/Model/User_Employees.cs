using System;
namespace PbProject.Model
{
    /// <summary>
    /// User_Employees:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class User_Employees
    {
        public User_Employees()
        { }
        #region Model
        private Guid _id;
        private string _cpyno = "";
        private string _loginname = "";
        private string _loginpassword = "";
        private string _username = "";
        private string _nameeasy = "";
        private string _worknum = "";
        private string _tel = "";
        private string _phone = "";
        private string _email = "";
        private string _address = "";
        private string _certificatetype = "";
        private string _certificatenum = "";
        private int _state = 1;
        private DateTime _createtime = DateTime.Now;
        private DateTime _starttime = DateTime.Now;
        private DateTime _overduetime = DateTime.Now;
        private string _qq = "";
        private string _msn = "";
        private string _remark = "";
        private string _sex = "";
        private string _postalcode = "";
        private string _addresstel = "";
        private string _deptid = "";
        private int _isadmin = 0;
      
        private string _userpower = "";
        private int _a1 = 0;
        private decimal _a2 = 0M;
        private decimal _a3 = 0M;
        private string _a4 = "";
        private string _a5 = "";
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
        /// 登录名
        /// </summary>
        public string LoginName
        {
            set { _loginname = value; }
            get { return _loginname; }
        }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassWord
        {
            set { _loginpassword = value; }
            get { return _loginpassword; }
        }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName
        {
            set { _username = value; }
            get { return _username; }
        }
        /// <summary>
        /// 姓名简码
        /// </summary>
        public string NameEasy
        {
            set { _nameeasy = value; }
            get { return _nameeasy; }
        }
        /// <summary>
        /// 工号
        /// </summary>
        public string WorkNum
        {
            set { _worknum = value; }
            get { return _worknum; }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel
        {
            set { _tel = value; }
            get { return _tel; }
        }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone
        {
            set { _phone = value; }
            get { return _phone; }
        }
        /// <summary>
        /// Email
        /// </summary>
        public string Email
        {
            set { _email = value; }
            get { return _email; }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            set { _address = value; }
            get { return _address; }
        }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType
        {
            set { _certificatetype = value; }
            get { return _certificatetype; }
        }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertificateNum
        {
            set { _certificatenum = value; }
            get { return _certificatenum; }
        }
        /// <summary>
        /// 状态 0=禁用，1=启用
        /// </summary>
        public int State
        {
            set { _state = value; }
            get { return _state; }
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
        /// 有效起始时间
        /// </summary>
        public DateTime StartTime
        {
            set { _starttime = value; }
            get { return _starttime; }
        }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime OverDueTime
        {
            set { _overduetime = value; }
            get { return _overduetime; }
        }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ
        {
            set { _qq = value; }
            get { return _qq; }
        }
        /// <summary>
        /// MSN
        /// </summary>
        public string MSN
        {
            set { _msn = value; }
            get { return _msn; }
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
        /// 性别
        /// </summary>
        public string Sex
        {
            set { _sex = value; }
            get { return _sex; }
        }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode
        {
            set { _postalcode = value; }
            get { return _postalcode; }
        }
        /// <summary>
        /// 住址电话
        /// </summary>
        public string AddressTel
        {
            set { _addresstel = value; }
            get { return _addresstel; }
        }
        /// <summary>
        /// 部门权限id
        /// </summary>
        public string DeptId
        {
            set { _deptid = value; }
            get { return _deptid; }
        }
        /// <summary>
        /// 管理员标志： 0 管理员，1 员工
        /// </summary>
        public int IsAdmin
        {
            set { _isadmin = value; }
            get { return _isadmin; }
        }
       
        /// <summary>
        /// 个人权限开关
        /// </summary>
        public string UserPower
        {
            set { _userpower = value; }
            get { return _userpower; }
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
        public decimal A2
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
        public string A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A5
        {
            set { _a5 = value; }
            get { return _a5; }
        }
        #endregion Model

    }
}

