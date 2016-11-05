using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model.PayParam
{
    [Serializable]
    public class AliPayParam
    {
        private string _Out_trade_no = string.Empty;
        private string _Subject = string.Empty;
        private string _Body = string.Empty;
        private string _Total_fee = string.Empty;
        private string _Royalty_parameters = string.Empty;
        private string _Extend_param = string.Empty;
        private string _Extra_common_param=string.Empty;
        private string _DefaultBank = string.Empty;

        /// <summary>
        /// 订单号
        /// </summary>
        public string Out_trade_no
        {
            get { return _Out_trade_no; }
            set { _Out_trade_no = value; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string Body
        {
            get { return _Body; }
            set { _Body = value; }
        }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string Total_fee
        {
            get { return _Total_fee; }
            set { _Total_fee = value; }
        }
        /// <summary>
        /// 分润参数
        /// </summary>
        public string Royalty_parameters
        {
            get { return _Royalty_parameters; }
            set { _Royalty_parameters = value; }
        }
        /// <summary>
        ///  公用业务扩展参数，支付宝用于 显示 PNR （格式：参数名1^参数值1|参数名2^参数值2|......）
        /// </summary>
        public string Extend_param
        {
            get { return _Extend_param; }
            set { _Extend_param = value; }
        }
        /// <summary>
        /// 自定义字段
        /// </summary>
        public string Extra_common_param
        {
            get { return _Extra_common_param; }
            set { _Extra_common_param = value; }
        }
        /// <summary>
        /// 网银标示
        /// </summary>
        public string DefaultBank
        {
            get { return _DefaultBank; }
            set { _DefaultBank = value; }
        }

        /// <summary>
        /// 获取交易状态 枚举说明
        /// </summary>
        /// <param name="tradeStatus"></param>
        /// <returns></returns>
        public string GetTradeStatus(string tradeStatus)
        {
            string msg = "";
            try
            {
                if (!string.IsNullOrEmpty(tradeStatus))
                {
                    tradeStatus = tradeStatus.Replace("<error>", "").Replace("</error>", "").Replace("<trade_status>", "").Replace("</trade_status>", "");

                    switch (tradeStatus)
                    {
                        #region 交易状态

                        case "WAIT_BUYER_PAY": msg = "等待买家付款"; break;
                        case "WAIT_SELLER_SEND_GOODS": msg = "买家已付款，等待卖家发货"; break;
                        case "WAIT_BUYER_CONFIRM_GOODS": msg = "卖家已发货，等待买家确认"; break;
                        case "TRADE_FINISHED": msg = "交易成功结束"; break;
                        case "TRADE_CLOSED": msg = "交易中途关闭（已结束，未成功完成"; break;
                        case "WAIT_SYS_CONFIRM_PAY": msg = "支付宝确认买家银行汇款中，暂勿发货"; break;
                        case "WAIT_SYS_PAY_SELLER": msg = "买家确认收货，等待支付宝打款给卖家"; break;
                        case "TRADE_REFUSE": msg = "立即支付交易拒绝"; break;
                        case "TRADE_REFUSE_DEALING": msg = "立即支付交易拒绝中"; break;
                        case "TRADE_CANCEL": msg = "立即支付交易取消"; break;
                        case "TRADE_PENDING": msg = "等待卖家收款"; break;
                        case "TRADE_SUCCESS": msg = "支付成功"; break;
                        case "BUYER_PRE_AUTH": msg = "买家已付款 （语音支付）"; break;
                        case "COD_WAIT_SELLER_SEND_GOODS": msg = "等待卖家发货 （货到付款）"; break;
                        case "COD_WAIT_BUYER_PAY": msg = "等待买家签收付款 （货到付款）"; break;
                        case "COD_WAIT_SYS_PAY_SELLER": msg = "签收成功等待系统打款给卖家 （货到付款）"; break;
                        case "ZHIFUBAO_CONFIRM": msg = "客服代买家确认收货"; break;
                        case "ZHIFUBAO_CANCEL_FP": msg = "客服代付款方取消快速支付"; break;
                        case "DAEMON_CONFIRM_CANCEL_PRE_AUTH": msg = "超时程序取消预授权"; break;
                        case "DAEMON_CONFIRM_CLOSE": msg = "超时程序因买家不付款关闭交易"; break;

                        #endregion

                        #region 退款状态

                        case "WAIT_SELLER_AGREE": msg = "退款协议等待卖家确认中"; break;
                        case "SELLER_REFUSE_BUYER": msg = "卖家不同意协议，等待买家修改"; break;
                        case "WAIT_BUYER_RETURN_GOODS": msg = "退款协议达成，等待买家退货"; break;
                        case "WAIT_SELLER_CONFIRM_GOODS": msg = "等待卖家收货"; break;
                        case "REFUND_SUCCESS": msg = "退款成功"; break;
                        case "REFUND_CLOSED": msg = "退款关闭"; break;
                        case "WAIT_ALIPAY_REFUND": msg = "等待支付宝退款"; break;
                        case "ACTIVE_REFUND": msg = "进行中的退款，供查询 "; break;
                        case "OVERED_REFUND": msg = "结束的退款"; break;
                        case "ALL_REFUND_STATUS": msg = "所有退款，供查询"; break;


                        #endregion

                        #region 超时状态

                        #endregion 

                        default: msg = "查询错误,请联系技术处理"; break;
                    }
                }
                else
                {
                    msg = "获取状态值错误！";
                }
            }
            catch (Exception)
            {
                msg = "查询异常,请联系技术处理";
            }

            return msg;
        }
    }
}
