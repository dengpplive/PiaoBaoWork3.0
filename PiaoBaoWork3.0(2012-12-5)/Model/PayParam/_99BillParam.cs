using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model.PayParam
{
    [Serializable]
    public class _99BillParam
    {
        private string _Orderid = string.Empty;//0.订单编号
        private string _Price = string.Empty;//1.订单金额，单位“分”
        private string _Money = string.Empty;//2.供应收款金额，单位“分”
        private string _Pname = string.Empty;//3.商品名称
        private string _Ext = string.Empty;//4.自定义字段
        private string _Detail = string.Empty;//5.分润数据集
        private string _Ptype = string.Empty;//6.分润类别，1 立刻分润 0 异步分润
        private string _Payid = string.Empty;//7.付款帐户
        private string _Paytype = string.Empty;//8.  00：组合支付，10：银行卡支付
        private string _Bankcode = string.Empty;//9: 银行代码

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Orderid
        {
            get { return _Orderid; }
            set { _Orderid = value; }
        }
        /// <summary>
        /// 订单金额，单位“分”
        /// </summary>
        public string Price
        {
            get { return _Price; }
            set { _Price = value; }
        }
        /// <summary>
        /// 供应收款金额，单位“分”
        /// </summary>
        public string Money
        {
            get { return _Money; }
            set { _Money = value; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Pname
        {
            get { return _Pname; }
            set { _Pname = value; }
        }
        /// <summary>
        /// 自定义字段
        /// </summary>
        public string Ext
        {
            get { return _Ext; }
            set { _Ext = value; }
        }
        /// <summary>
        /// 分润数据集
        /// </summary>
        public string Detail
        {
            get { return _Detail; }
            set { _Detail = value; }
        }
        /// <summary>
        /// 分润类别，1 立刻分润 0 异步分润
        /// </summary>
        public string Ptype
        {
            get { return _Ptype; }
            set { _Ptype = value; }
        }
        /// <summary>
        /// 付款帐户
        /// </summary>
        public string Payid
        {
            get { return _Payid; }
            set { _Payid = value; }
        }
        /// <summary>
        /// 00：组合支付，10：银行卡支付
        /// </summary>
        public string Paytype
        {
            get { return _Paytype; }
            set { _Paytype = value; }
        }
        /// <summary>
        /// 银行代码
        /// </summary>
        public string Bankcode
        {
            get { return _Bankcode; }
            set { _Bankcode = value; }
        }
    }
}
