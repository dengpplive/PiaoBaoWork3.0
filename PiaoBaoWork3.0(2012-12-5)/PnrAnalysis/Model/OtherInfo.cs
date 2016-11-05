using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    [Serializable]
    public class OtherInfo
    {
        private string _CT = string.Empty;
        /// <summary>
        /// CT
        /// </summary>
        public string CT
        {
            get { return _CT; }
            set { _CT = value; }
        }
        private string _CTCT = string.Empty;
        /// <summary>
        /// CTCT
        /// </summary>
        public string CTCT
        {
            get { return _CTCT; }
            set { _CTCT = value; }
        }

        private bool _IsTL = false;
        /// <summary>
        /// 编码中是否含有出票时限
        /// </summary>
        public bool IsTL
        {
            get { return _IsTL; }
            set { _IsTL = value; }
        }

        private Tktl _TKTL = null;
        /// <summary>
        /// 出票时限
        /// </summary>
        public Tktl TKTL
        {
            get
            {
                return _TKTL;
            }
            set
            {
                _TKTL = value;
            }
        }
    }
}
