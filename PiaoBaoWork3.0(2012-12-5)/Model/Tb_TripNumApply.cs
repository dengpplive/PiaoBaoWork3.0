using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_TripNumApply:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_TripNumApply
    {
        public Tb_TripNumApply()
        { }
        #region Model
        private Guid _id;
        private string _applyaccount = "";
        private string _applyusername = "";
        private string _applycpyno = "";
        private string _applycpyname = "";
        private string _auditaccount = "";
        private string _auditusername = "";
        private string _auditcpyno = "";
        private string _auditcpyname = "";
        private string _applycount = "";
        private int _applystatus = 3;
        private DateTime _applydate = Convert.ToDateTime("1900-01-01");
        private DateTime _auditdate = Convert.ToDateTime("1900-01-01");
        private string _auditremark = "";
        private string _applyremark = "";
        private string _a1 = "";
        private string _a2 = "";
        private string _a3 = "";
        private string _a4 = "";
        private string _a5 = "";
        private int _a6 = 0;
        private int _a7 = 0;
        private int _a8 = 0;
        private int _a9 = 0;
        private int _a10 = 0;
        /// <summary>
        /// 
        /// </summary>
        public Guid id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 申请行程单登陆账号
        /// </summary>
        public string ApplyAccount
        {
            set { _applyaccount = value; }
            get { return _applyaccount; }
        }
        /// <summary>
        /// 申请行程单账号用户姓名
        /// </summary>
        public string ApplyUserName
        {
            set { _applyusername = value; }
            get { return _applyusername; }
        }
        /// <summary>
        /// 申请行程单公司编号
        /// </summary>
        public string ApplyCpyNo
        {
            set { _applycpyno = value; }
            get { return _applycpyno; }
        }
        /// <summary>
        /// 申请行程单公司名称
        /// </summary>
        public string ApplyCpyName
        {
            set { _applycpyname = value; }
            get { return _applycpyname; }
        }
        /// <summary>
        /// 审核行程单账号
        /// </summary>
        public string AuditAccount
        {
            set { _auditaccount = value; }
            get { return _auditaccount; }
        }
        /// <summary>
        /// 审核行程单用户姓名
        /// </summary>
        public string AuditUserName
        {
            set { _auditusername = value; }
            get { return _auditusername; }
        }
        /// <summary>
        /// 行程单审核人公司编号
        /// </summary>
        public string AuditCpyNo
        {
            set { _auditcpyno = value; }
            get { return _auditcpyno; }
        }
        /// <summary>
        /// 行程单审核人公司名称
        /// </summary>
        public string AuditCpyName
        {
            set { _auditcpyname = value; }
            get { return _auditcpyname; }
        }
        /// <summary>
        /// 申请行程单数目
        /// </summary>
        public string ApplyCount
        {
            set { _applycount = value; }
            get { return _applycount; }
        }
        /// <summary>
        /// 申请行程单状态
        /// </summary>
        public int ApplyStatus
        {
            set { _applystatus = value; }
            get { return _applystatus; }
        }
        /// <summary>
        /// 申请行程单日期
        /// </summary>
        public DateTime ApplyDate
        {
            set { _applydate = value; }
            get { return _applydate; }
        }
        /// <summary>
        /// 审核行程单日期
        /// </summary>
        public DateTime AuditDate
        {
            set { _auditdate = value; }
            get { return _auditdate; }
        }
        /// <summary>
        /// 审核行程单备注
        /// </summary>
        public string AuditRemark
        {
            set { _auditremark = value; }
            get { return _auditremark; }
        }
        /// <summary>
        /// 申请行程单备注
        /// </summary>
        public string ApplyRemark
        {
            set { _applyremark = value; }
            get { return _applyremark; }
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
        /// <summary>
        /// 
        /// </summary>
        public string A5
        {
            set { _a5 = value; }
            get { return _a5; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A6
        {
            set { _a6 = value; }
            get { return _a6; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A7
        {
            set { _a7 = value; }
            get { return _a7; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A8
        {
            set { _a8 = value; }
            get { return _a8; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A9
        {
            set { _a9 = value; }
            get { return _a9; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int A10
        {
            set { _a10 = value; }
            get { return _a10; }
        }
        #endregion Model

    }
}

