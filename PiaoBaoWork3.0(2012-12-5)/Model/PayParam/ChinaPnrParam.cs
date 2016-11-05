using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model.PayParam
{
    [Serializable]
    public class ChinaPnrParam
    {
        private string _Orderid = string.Empty;//0.订单编号
        private string _Price = string.Empty;//1.订单总价
        private string _Buyphone = string.Empty;//2.买家电话
        private string _Buyid = string.Empty;//3.买家id
        private string _Pnr = string.Empty;//4.pnr
        private string _Details = string.Empty;//5.分润数据集
        private string _Merpriv = string.Empty;//6.自定义字段

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Orderid
        {
            get { return _Orderid; }
            set { _Orderid = value; }
        }
        /// <summary>
        /// 订单总价
        /// </summary>
        public string Price
        {
            get { return _Price; }
            set { _Price = value; }
        }
        /// <summary>
        /// 买家电话
        /// </summary>
        public string Buyphone
        {
            get { return _Buyphone; }
            set { _Buyphone = value; }
        }
        /// <summary>
        /// 买家id
        /// </summary>
        public string Buyid
        {
            get { return _Buyid; }
            set { _Buyid = value; }
        }
        /// <summary>
        /// pnr
        /// </summary>
        public string Pnr
        {
            get { return _Pnr; }
            set { _Pnr = value; }
        }
        /// <summary>
        /// 分润数据集
        /// </summary>
        public string Details
        {
            get { return _Details; }
            set { _Details = value; }
        }
        /// <summary>
        /// 自定义字段
        /// </summary>
        public string Merpriv
        {
            get { return _Merpriv; }
            set { _Merpriv = value; }
        }
    }
}
