using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_SendInsData:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_SendInsData
    {
        public Tb_SendInsData()
        { }
        #region Model
        private int _id = 1;
        private string _sendins = string.Empty;
        private string _recvdata = string.Empty;
        private DateTime _sendtime = Convert.ToDateTime("1900-01-01");
        private DateTime _recvtime = Convert.ToDateTime("1900-01-01");
        private string _office = string.Empty;
        private string _serveripandport = string.Empty;
        private string _useraccount = string.Empty;
        private string _cpyno = string.Empty;
        private int _sendinstype = 0;
        private DateTime _adddate = DateTime.Now;
        private string _a1 = string.Empty;
        private string _a2 = string.Empty;
        private string _a3 = string.Empty;
        private string _a4 = string.Empty;
        private string _a5 = string.Empty;
        private string _a6 = string.Empty;
        private string _a7 = string.Empty;
        private string _a8 = string.Empty;
        private string _a9 = string.Empty;
        private string _a10 = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 发送指令
        /// </summary>
        public string SendIns
        {
            set { _sendins = value; }
            get { return _sendins; }
        }
        /// <summary>
        /// 指令接收数据
        /// </summary>
        public string RecvData
        {
            set { _recvdata = value; }
            get { return _recvdata; }
        }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime
        {
            set { _sendtime = value; }
            get { return _sendtime; }
        }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime RecvTime
        {
            set { _recvtime = value; }
            get { return _recvtime; }
        }
        /// <summary>
        /// 使用的Office号
        /// </summary>
        public string Office
        {
            set { _office = value; }
            get { return _office; }
        }
        /// <summary>
        /// IP 和端口
        /// </summary>
        public string ServerIPAndPort
        {
            set { _serveripandport = value; }
            get { return _serveripandport; }
        }
        /// <summary>
        /// 发送指令的用户账号即登录账号
        /// </summary>
        public string UserAccount
        {
            set { _useraccount = value; }
            get { return _useraccount; }
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
        /// 发送指令类型 默认0 可以自定义
        /// 自定义指令类型 默认0  1取消编码 2获取PNR内容 3获取Pat内容 4对编码授予指定Office的权限 5.循环供应商Office提取指令 6.行程单操作指令,7票号挂起解挂,8修改证件号 9使用WebService发送的指令 10.备注指令OSI HU CKIN SSAC/S1 11.标识控台发送指令,12.获取特价预订编码指令 13.扫描程序发送的指令 14.抢票预订编码 15.发送特价获取指令 16.记录PNR内容导入 17.xe出票时限 18.BSP扫描指令
        /// </summary>
        public int SendInsType
        {
            set { _sendinstype = value; }
            get { return _sendinstype; }
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate
        {
            set { _adddate = value; }
            get { return _adddate; }
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
        public string A6
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

