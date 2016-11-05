using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model
{
    /// <summary>
    ///黑屏 白屏 配置参数信息
    /// </summary>  
    [Serializable]
    public class ConfigParam
    {
        //大配置参数(0大配置IP|1大配置端口|2大配置Office|3大配置名称与密码)

        private string _BigCfgIP = string.Empty;
        /// <summary>
        /// 大配置IP
        /// </summary>
        public string BigCfgIP
        {
            get { return _BigCfgIP; }
            set { _BigCfgIP = value; }
        }
        private string _BigCfgPort = string.Empty;
        /// <summary>
        /// 大配置端口
        /// </summary>
        public string BigCfgPort
        {
            get { return _BigCfgPort; }
            set { _BigCfgPort = value; }
        }
        private string _BigCfgOffice = string.Empty;
        /// <summary>
        /// 大配置Office
        /// </summary>
        public string BigCfgOffice
        {
            get { return _BigCfgOffice; }
            set { _BigCfgOffice = value; }
        }
        private string _BigCfgAccount = string.Empty;
        /// <summary>
        /// 大配置账号
        /// </summary>
        public string BigCfgAccount
        {
            get { return _BigCfgAccount; }
            set { _BigCfgAccount = value; }
        }
        private string _BigCfgPwd = string.Empty;
        /// <summary>
        /// 大配置密码
        /// </summary>
        public string BigCfgPwd
        {
            get { return _BigCfgPwd; }
            set { _BigCfgPwd = value; }
        }
        private string _WebBlackIP = string.Empty;
        /// <summary>
        /// 网页黑屏IP
        /// </summary>
        public string WebBlackIP
        {
            get { return _WebBlackIP; }
            set { _WebBlackIP = value; }
        }
        private string _WebBlackPort = string.Empty;
        /// <summary>
        /// 网页黑屏端口
        /// </summary>
        public string WebBlackPort
        {
            get { return _WebBlackPort; }
            set { _WebBlackPort = value; }
        }
        private string _WebBlackAccount = string.Empty;
        /// <summary>
        /// 网页黑屏帐号
        /// </summary>
        public string WebBlackAccount
        {
            get { return _WebBlackAccount; }
            set { _WebBlackAccount = value; }
        }
        private string _WebBlackPwd = string.Empty;
        /// <summary>
        /// 网页黑屏密码
        /// </summary>
        public string WebBlackPwd
        {
            get { return _WebBlackPwd; }
            set { _WebBlackPwd = value; }
        }
        private string _ECPort = string.Empty;
        /// <summary>
        /// EC网页黑屏监听端口
        /// </summary>
        public string ECPort
        {
            get { return _ECPort; }
            set { _ECPort = value; }
        }
        private string _WhiteScreenIP = string.Empty;
        /// <summary>
        /// 白屏IP
        /// </summary>
        public string WhiteScreenIP
        {
            get { return _WhiteScreenIP; }
            set { _WhiteScreenIP = value; }
        }
        private string _WhiteScreenPort = string.Empty;
        /// <summary>
        /// 白屏交互端口
        /// </summary>
        public string WhiteScreenPort
        {
            get { return _WhiteScreenPort; }
            set { _WhiteScreenPort = value; }
        }
        private string _Office = string.Empty;
        /// <summary>
        /// Office号
        /// </summary>
        public string Office
        {
            get { return _Office; }
            set { _Office = value; }
        }

        private string _TicketCompany = string.Empty;
        /// <summary>
        /// 开票单位名称
        /// </summary>
        public string TicketCompany
        {
            get { return _TicketCompany; }
            set { _TicketCompany = value; }
        }
        private string _IataCode = string.Empty;
        /// <summary>
        /// 航协号
        /// </summary>
        public string IataCode
        {
            get { return _IataCode; }
            set { _IataCode = value; }
        }

        private string _Pid = string.Empty;
        public string Pid
        {
            get
            {
                return _Pid;
            }
            set
            {
                _Pid = value;
            }
        }
        private string _KeyNo = string.Empty;
        public string KeyNo
        {
            get
            {
                return _KeyNo;
            }
            set
            {
                _KeyNo = value;
            }
        }
    }
}
