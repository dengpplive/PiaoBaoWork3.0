using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model.PayParam
{
    [Serializable]
    public class TenPayParam
    {
        private string _Orderid = string.Empty;//0.订单编号
        private string _UserHostAddress = string.Empty;//1.用户的公网ip
        private string _Desc = string.Empty;//2.商品名称
        private string _Total_Tee = string.Empty;//3.商品金额,以分为单
        private string _Bus_Args = string.Empty;//4.分润数据集: 帐号1^分帐金额1^角色1|帐号2^分帐金额2^角色2|...
        private string _Bus_Desc = string.Empty;  //5.行业描述信息
        private string _purchaser_id = string.Empty;//买家帐号
        private string _attach = string.Empty;//商家数据包,默认操作用户ID
        private string _payNo;//交易号
        private int _backState;//退款状态[1:分账回退成功，]
        private string _OldOrderid; //旧订单
        private string _Date;//订单日期
        private string _bankType = "0";
        //银行编码
        public string BankType
        {
            get { return _bankType; }
            set { _bankType = value; }
        }

        //订单日期
        public string Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        //旧订单
        public string OldOrderid
        {
            get { return _OldOrderid; }
            set { _OldOrderid = value; }
        }
        
        //分账回退
        public int BackState
        {
            get { return _backState; }
            set { _backState = value; }
        }
        //交易号
        public string PayNo
        {
            get { return _payNo; }
            set { _payNo = value; }
        }
        /// <summary>
        /// 商家数据包
        /// </summary>
        public string Attach
        {
            get { return _attach; }
            set { _attach = value; }
        }

        /// <summary>
        /// 支付帐号
        /// </summary>
        public string Purchaser_id
        {
            get { return _purchaser_id; }
            set { _purchaser_id = value; }
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Orderid
        {
            get { return _Orderid; }
            set { _Orderid = value; }
        }
        /// <summary>
        /// 用户的公网ip
        /// </summary>
        public string UserHostAddress
        {
            get { return _UserHostAddress; }
            set { _UserHostAddress = value; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Desc
        {
            get { return _Desc; }
            set { _Desc = value; }
        }
        /// <summary>
        /// 商品金额,以分为单
        /// </summary>
        public string Total_Tee
        {
            get { return _Total_Tee; }
            set { _Total_Tee = value; }
        }
        /// <summary>
        /// 分润数据集: 帐号1^分帐金额1^角色1|帐号2^分帐金额2^角色2|...
        /// </summary>
        public string Bus_Args
        {
            get { return _Bus_Args; }
            set { _Bus_Args = value; }
        }
        /// <summary>
        /// 行业描述信息
        /// </summary>
        public string Bus_Desc
        {
            get { return _Bus_Desc; }
            set { _Bus_Desc = value; }
        }
    }
}
