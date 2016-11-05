using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Collections;
using tenpay;
using PbProject.Model.PayParam;
using System.Configuration;
using System.IO;

namespace PbProject.Logic.Pay
{
    /// <summary>
    /// 财付通
    /// </summary>
    public class TenPay
    {
        #region 参数
        /// <summary>
        /// 商户号
        /// </summary>
        private string bargainor_id = "1215129501";
        /// <summary>
        /// 密钥
        /// </summary>
        private string key = "3e3e67589c4e066b7e50f7460ba2cbe0";
        /// <summary>
        /// 编码
        /// </summary>
        private string _input_charset = "";
        /// <summary>
        /// 支付通知url
        /// </summary>
        private string _ReturnUrl = ConfigurationManager.AppSettings["_TenPayReturnUrl"].ToString();
        /// <summary>
        /// 加密文件
        /// </summary>
        public string pfxPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Pay/Key/1215129501_20121224104503.pfx";

        #endregion

        public TenPay()
        {
            HttpContext.Current.Request.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
        }

        public TenPay(bool type)
        { 
        
        }

        /// <summary>
        /// 支付接口（暂时不用）
        /// </summary>
        /// <param name="tenPayParam">参数 Model</param>
        /// <returns></returns>
        public string payRequest(TenPayParam tenPayParam)
        {
            string result = "";
            try
            {
                string desc = tenPayParam.Desc;//商品名称
                string ip = tenPayParam.UserHostAddress;//用户的公网ip
                string sp_billno = tenPayParam.Orderid;//商户订单号
                string total_fee = tenPayParam.Total_Tee;//商品金额,以分为单

                //当前时间 yyyyMMdd
                string date = DateTime.Now.ToString("yyyyMMdd");
                //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                string strReq = "" + DateTime.Now.ToString("HHmmss") + TenpayUtil.BuildRandomStr(4);
                //商户订单号，不超过32位，财付通只做记录，不保证唯一性
                //string sp_billno = strReq;
                // 财付通交易单号，规则为：10位商户号+8位时间（YYYYmmdd)+10位流水号 ,商户根据自己情况调整，只要保证唯一及符合规则就行
                string transaction_id = bargainor_id + date + strReq;
                //创建PayRequestHandler实例
                PayRequestHandler reqHandler = new PayRequestHandler(HttpContext.Current);
                //初始化
                reqHandler.init();
                //设置密钥
                reqHandler.setKey(key);
                //-----------------------------
                //设置支付参数
                //-----------------------------
                reqHandler.setParameter("bargainor_id", bargainor_id);//商户号
                reqHandler.setParameter("sp_billno", sp_billno);	//商家订单号
                reqHandler.setParameter("transaction_id", transaction_id);//财付通交易单号
                reqHandler.setParameter("return_url", _ReturnUrl);//支付通知url
                reqHandler.setParameter("desc", desc); //商品名称
                reqHandler.setParameter("total_fee", total_fee);//商品金额,以分为单
                //用户的公网ip,测试时填写127.0.0.1,只能支持10分以下交易
                reqHandler.setParameter("spbill_create_ip", ip);
                //获取请求带参数的url
                string requestUrl = reqHandler.getRequestURL();

                #region GET  提交方式

                //result = "<a target=\"_blank\" href=\"" + requestUrl + "\">" + "财付通支付" + "</a>";

                #endregion

                #region POST 提交方式
                StringBuilder sb = new StringBuilder();
                sb.Append("<form name='alipaysubmit' method='post' action='" + requestUrl + "'>");
                Hashtable ht = reqHandler.getAllParameters();
                foreach (DictionaryEntry de in ht)
                {
                    sb.Append("<input type='hidden' name='" + de.Key + "' value='" + de.Value + "' >");
                }
                sb.Append("</form>");
                sb.Append("<script>");
                sb.Append("document.alipaysubmit.submit()");
                sb.Append("</script>");
                result = sb.ToString();

                #endregion
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        /// <summary>
        /// 建立委托退款关系请求
        /// </summary>
        /// <returns></returns>
        public string TrustRefundRequest()
        {
            return string.Format("https://www.tenpay.com/cgi-bin/trust/showtrust_refund.cgi?spid={0}", bargainor_id);
        }
        /// <summary>
        /// 签约查询
        /// </summary>
        /// <returns></returns>
        public int TrustRequest(string account)
        {
            int result = 0;
            BaseSplitRequestHandler reqHandler = new BaseSplitRequestHandler(HttpContext.Current);
            //通信对象
            TenpayHttpClient httpClient = new TenpayHttpClient();
            //应答对象
            ScriptClientResponseHandler resHandler = new ScriptClientResponseHandler();
            //设置请求参数
            reqHandler.init();
            reqHandler.setKey(key);
            reqHandler.setGateUrl("https://mch.tenpay.com/cgi-bin/inquire_entrust_relation.cgi");

            reqHandler.setParameter("cmdno", "99");
            reqHandler.setParameter("bargainor_id", bargainor_id);
            reqHandler.setParameter("purchaser_id", account);
            reqHandler.setParameter("type", "1");
            reqHandler.setParameter("return_url", "http://127.0.0.1/");
            reqHandler.setParameter("version", "4");

            httpClient.setCertInfo(pfxPath, bargainor_id);
            string requestUrl = reqHandler.getRequestURL();
            //设置请求内容
            httpClient.setReqContent(requestUrl);
            //设置 超时
            httpClient.setTimeOut(10);
            string rescontent = string.Empty;
            //后台调用
            if (httpClient.call())
            {
                //获取结果
                rescontent = httpClient.getResContent();
                resHandler.setKey(key);
                //设置结果参数
                resHandler.setContent(rescontent);
                //判断签名及结果
                if (resHandler.isTenpaySign() && resHandler.getParameter("status") == "1")
                {
                    result = 1;
                }
                else
                {
                    result = 2;
                }
            }
            return result;
        }

        /// <summary>
        /// 支付并分账接口
        /// </summary>
        /// <param name="tenPayParam">参数 Model</param>
        /// <returns></returns>
        public string SplitPayRequest(TenPayParam tenPayParam)
        {
            string result = "";
            try
            {
                string desc = tenPayParam.Desc;//商品名称
                string ip = tenPayParam.UserHostAddress;//用户的公网ip
                string sp_billno = tenPayParam.Orderid;//商户订单号
                string total_fee = tenPayParam.Total_Tee;//商品金额,以分为单
                string bus_Args = tenPayParam.Bus_Args;//分账明细
                string bus_desc = tenPayParam.Bus_Desc;//行业描述信息
                string purchaser_id = tenPayParam.Purchaser_id;
                string bank_type = tenPayParam.BankType;
                string attach=tenPayParam.Attach;
                //当前时间 yyyyMMdd
                string date = DateTime.Now.ToString("yyyyMMdd");
                //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                string strReq = "" + DateTime.Now.ToString("HHmmss") + TenpayUtil.BuildRandomStr(4);
                //商户订单号，不超过32位，财付通只做记录，不保证唯一性
                //string sp_billno = strReq;
                // 财付通交易单号，规则为：10位商户号+8位时间（YYYYmmdd)+10位流水号 ,商户根据自己情况调整，只要保证唯一及符合规则就行
                string transaction_id = bargainor_id + date + strReq;
                //创建BaseSplitRequestHandler实例
                //BaseSplitRequestHandler reqHandler = new BaseSplitRequestHandler(Context);
                BaseSplitRequestHandler reqHandler = new BaseSplitRequestHandler(HttpContext.Current);
                //初始化
                reqHandler.init();
                //设置密钥
                reqHandler.setKey(key);
                reqHandler.setGateUrl("https://www.tenpay.com/cgi-bin/v4.0/pay_set.cgi");
                //-----------------------------
                //设置支付参数
                //-----------------------------
                reqHandler.setParameter("bank_type", bank_type);
                reqHandler.setParameter("cmdno", "1");// 财付通支付支付接口填  1  
                reqHandler.setParameter("date", DateTime.Now.ToString("yyyyMMdd"));
                reqHandler.setParameter("fee_type", "1");
                reqHandler.setParameter("version", "4");//版本号必须填4
                reqHandler.setParameter("bargainor_id", bargainor_id);//商户号
                reqHandler.setParameter("sp_billno", sp_billno);	//商家订单号
                reqHandler.setParameter("transaction_id", transaction_id);//财付通交易单号
                reqHandler.setParameter("return_url", _ReturnUrl);//支付通知url
                reqHandler.setParameter("desc", desc);//商品名称
                reqHandler.setParameter("total_fee", total_fee);	//商品金额,以分为单位
                //用户的公网ip,测试时填写127.0.0.1,只能支持10分以下交易
                //reqHandler.setParameter("spbill_create_ip", ip);
                //业务类型
                reqHandler.setParameter("bus_type", "97");//暂固定为97
                reqHandler.setParameter("purchaser_id", purchaser_id);
                reqHandler.setParameter("attach", attach);
                /**
                 * 业务参数
                 * 帐号^分帐金额^角色
                 * 角色说明:	1:供应商 2:平台服务方 4:独立分润方
                 * 帐号1^分帐金额1^角色1|帐号2^分帐金额2^角色2|...
                 */
                reqHandler.setParameter("bus_args", bus_Args);
                //行业描述信息
                //reqHandler.setParameter("bus_desc", "12345^深圳-上海^1^fady^庄^13800138000");
                reqHandler.setParameter("bus_desc", bus_desc);//业务描述，特定格式的字符串，格式为：PNR^航程^机票张数^机票销售商在机票平台的id^联系人姓名^联系电话

                //用户的公网ip,测试时填写127.0.0.1,只能支持10分以下交易
                reqHandler.setParameter("spbill_create_ip", ip);
                //获取请求带参数的url
                string requestUrl = reqHandler.getRequestURL();

                #region POST 提交方式
                StringBuilder sb = new StringBuilder();
                sb.Append("<form name='alipaysubmit' method='post' action='" + requestUrl + "'>");
                Hashtable ht = reqHandler.getAllParameters();
                foreach (DictionaryEntry de in ht)
                {
                    sb.Append("<input type='hidden' name='" + de.Key + "' value='" + de.Value + "' >");
                }
                sb.Append("</form>");
                sb.Append("<script>");
                sb.Append("document.alipaysubmit.submit()");
                sb.Append("</script>");
                result = sb.ToString();
                #endregion

                //获取debug信息,建议把请求和debug信息写入日志，方便定位问题
                //string debuginfo = reqHandler.getDebugInfo();
                //Response.Write("<br/>requestUrl:" + requestUrl + "<br/>");
                //Response.Write("<br/>debuginfo:" + debuginfo + "<br/>");

            }
            catch (Exception ex)
            {

            }
            return result;
        }
        /// <summary>
        /// 支付并分账应答接口
        /// </summary>
        /// <returns></returns>
        public string SplitPayResponse()
        {
            string msg = string.Empty;

            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(HttpContext.Current);
            resHandler.setKey(key);
            //判断是否签名
            if (resHandler.isTenpaySign())
            {
                //交易单号
                string transaction_id = resHandler.getParameter("transaction_id");

                //金额金额,以分为单位
                string total_fee = resHandler.getParameter("total_fee");

                //支付结果
                string pay_result = resHandler.getParameter("pay_result");

                if (pay_result.Equals("0"))
                {
                    /*
                     平台业务
                     */
                    msg = "支付成功";
                }
                else
                {
                    msg = "支付失败";
                }
            }
            else 
            {
                msg = "认证签名失败";    
            }
            return msg;

        }
        /// <summary>
        /// 分账退款接口
        /// </summary>
        /// <returns></returns>
        public bool ClientSplitRollback(TenPayParam tenPayParam)
        {
            PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
            bool result = true;
            if (tenPayParam.BackState == 1)
            {
                #region 平台退款
                string details = string.Format("分账参数:{0}", tenPayParam.Bus_Args);
                //平台退款
                int returnresult = ClientPlatformRefund(tenPayParam);

                OnErrorNew("订单号：" + tenPayParam.Orderid + ",平台退款状态：" + returnresult + ",details: " + details);

                switch (returnresult)
                {
                    case 0:
                        bill.CreateBillRefundFailedLog(null, tenPayParam.Orderid, "财付通平台退款通信失败:" + details);
                        result = false;
                        break;
                    case 1:
                        bill.CreateBillRefund(tenPayParam.Orderid, tenPayParam.PayNo, 4, "在线退款", "财付通退款", details);
                        result = true;
                        break;
                    case 2:
                        bill.CreateBillRefundFailedLog(null, tenPayParam.Orderid, "财付通平台退款业务错误信息或签名错误:" + details);
                        result = false;
                        break;
                    default:
                        break;
                }
                #endregion
            }
            else
            {

                //创建请求对象
                BaseSplitRequestHandler reqHandler = new BaseSplitRequestHandler(HttpContext.Current);
                //通信对象
                TenpayHttpClient httpClient = new TenpayHttpClient();
                //应答对象
                ScriptClientResponseHandler resHandler = new ScriptClientResponseHandler();
                //请求参数设置

                reqHandler.init();
                reqHandler.setKey(key);
                reqHandler.setGateUrl("https://mch.tenpay.com/cgi-bin/split_rollback.cgi");

                reqHandler.setParameter("cmdno", "95");
                reqHandler.setParameter("version", "4");
                reqHandler.setParameter("fee_type", "1");
                reqHandler.setParameter("bargainor_id", bargainor_id);		//商户号
                reqHandler.setParameter("sp_billno", tenPayParam.OldOrderid);				//商家订单号
                reqHandler.setParameter("transaction_id", tenPayParam.PayNo);	//财付通交易单号[外部订单号]
                reqHandler.setParameter("return_url", "http://127.0.0.1/");			//后台系统调用，必现填写为http://127.0.0.1/
                reqHandler.setParameter("total_fee", tenPayParam.Total_Tee);
                //退款ID，同个ID财付通认为是同一个退款,格式为109+10位商户号+8位日期+7位序列号
                string refund_id = string.Format("109{0}{1}{2}", bargainor_id, DateTime.Now.ToString("yyyyMMdd"), TenpayUtil.BuildRandomStr(7));
                reqHandler.setParameter("refund_id", refund_id);
                reqHandler.setParameter("bus_type", "97");
                //格式:退款总额| (账户^退款金额)[|(账户^退款金额)]*
                reqHandler.setParameter("bus_args", tenPayParam.Bus_Args);

                //-----------------------------
                //设置通信参数
                //-----------------------------
                //证书必须放在用户下载不到的目录，避免证书被盗取
                httpClient.setCertInfo(pfxPath, bargainor_id);

                string requestUrl = reqHandler.getRequestURL();
                //设置请求内容
                httpClient.setReqContent(requestUrl);
                //设置超时
                httpClient.setTimeOut(10);

                string rescontent = "";
                //后台调用

                if (httpClient.call())
                {
                    //获取结果
                    rescontent = httpClient.getResContent();

                    resHandler.setKey(key);
                    //设置结果参数
                    resHandler.setContent(rescontent);

                    //判断签名及结果
                    if (resHandler.isTenpaySign() && resHandler.getParameter("pay_result") == "0")
                    {
                        //分账回退成功
                        bill.UpdateBackState(tenPayParam.OldOrderid, 1);
                        //取结果参数做业务处理
                        // resultMsg = "OK,transaction_id=" + resHandler.getParameter("transaction_id");

                        string details = string.Format("财付通交易号:{0} || {1}", resHandler.getParameter("transaction_id"), tenPayParam.Bus_Args);
                        //平台退款
                        int returnresult = ClientPlatformRefund(tenPayParam);
                        switch (returnresult)
                        {
                            case 0:
                                bill.CreateBillRefundFailedLog(null, tenPayParam.Orderid, "财付通平台退款通信失败:" + details);
                                result = false;
                                break;
                            case 1:
                                bill.CreateBillRefund(tenPayParam.Orderid, tenPayParam.PayNo, 4, "在线退款", "财付通退款", details);
                                result = true;
                                break;
                            case 2:
                                bill.CreateBillRefundFailedLog(null, tenPayParam.Orderid, "财付通平台退款业务错误信息或签名错误:" + details);
                                result = false;
                                break;
                            default:
                                break;
                        }


                    }
                    else
                    {
                        result = false;
                        string details = string.Format("财付通分账退款业务错误信息或签名错误:{0},{1}--{2}", resHandler.getParameter("pay_result"), resHandler.getParameter("pay_info"), tenPayParam.Bus_Args);
                        bill.CreateBillRefundFailedLog(null, tenPayParam.Orderid, details); //分账退款失败
                        //错误时，返回结果未签名。
                        //如包格式错误或未确认结果的，请使用原来订单号重新发起，确认结果，避免多次操作
                    }

                }
                else
                {
                    result = false;
                    //后台调用通信失败
                    string details = string.Format("财付通分账退款通信失败:{0}{1}=={2}", httpClient.getErrInfo(), httpClient.getResponseCode(), tenPayParam.Bus_Args);
                    bill.CreateBillRefundFailedLog(null, tenPayParam.Orderid, details); //退款失败
                    //有可能因为网络原因，请求已经处理，但未收到应答。
                }
            }
            return result;
        }
        /// <summary>
        /// 平台退款
        /// </summary>
        /// <param name="tenPayParam"></param>
        /// <returns></returns>
        public int ClientPlatformRefund(TenPayParam tenPayParam)
        {
            int result = 0;
            //创建请求对象
            BaseSplitRequestHandler reqHandler = new BaseSplitRequestHandler(HttpContext.Current);

            //通信对象
            TenpayHttpClient httpClient = new TenpayHttpClient();

            //应答对象
            ScriptClientResponseHandler resHandler = new ScriptClientResponseHandler();

            //-----------------------------
            //设置请求参数
            //-----------------------------
            reqHandler.init();
            reqHandler.setKey(key);
            reqHandler.setGateUrl("https://mch.tenpay.com/cgi-bin/refund_b2c_split.cgi");

            reqHandler.setParameter("cmdno", "93");
            reqHandler.setParameter("version", "4");
            reqHandler.setParameter("fee_type", "1");
            reqHandler.setParameter("bargainor_id", bargainor_id);		//商户号
            reqHandler.setParameter("sp_billno", tenPayParam.OldOrderid);				//商家订单号
            reqHandler.setParameter("transaction_id", tenPayParam.PayNo);	//财付通交易单号
            reqHandler.setParameter("return_url", "http://127.0.0.1/");			//后台系统调用，必现填写为http://127.0.0.1/
            reqHandler.setParameter("total_fee", tenPayParam.Total_Tee);
            //退款ID，同个ID财付通认为是同一个退款,格式为109+10位商户号+8位日期+7位序列号
            string refund_id = string.Format("109{0}{1}{2}", bargainor_id, DateTime.Now.ToString("yyyyMMdd"), TenpayUtil.BuildRandomStr(7));
            reqHandler.setParameter("refund_id", refund_id);
            reqHandler.setParameter("refund_fee", tenPayParam.Date);//在这里date指的是退款金额


            //-----------------------------
            //设置通信参数
            //-----------------------------
            //证书必须放在用户下载不到的目录，避免证书被盗取
            httpClient.setCertInfo(pfxPath, bargainor_id);

            string requestUrl = reqHandler.getRequestURL();
            //设置请求内容
            httpClient.setReqContent(requestUrl);
            //设置超时
            httpClient.setTimeOut(10);

            string rescontent = "";
            //后台调用
            if (httpClient.call())
            {
                //获取结果
                rescontent = httpClient.getResContent();

                resHandler.setKey(key);
                //设置结果参数
                resHandler.setContent(rescontent);

                //判断签名及结果
                if (resHandler.isTenpaySign() && resHandler.getParameter("pay_result") == "0")
                {
                    //取结果参数做业务处理
                    result = 1;
                }
                else
                {
                    //错误时，返回结果未签名。
                    //如包格式错误或未确认结果的，请使用原来订单号重新发起，确认结果，避免多次操作
                    result = 2;
                 //string msg=("业务错误信息或签名错误:" + resHandler.getParameter("pay_result") + "," + resHandler.getParameter("pay_info") + "<br>");
                }

            }
            return result;
        }
        /// <summary>
        /// 订单查询接口
        /// </summary>
        /// <param name="tenpayParam"></param>
        /// <returns></returns>
        public string ClientSplitInquire(TenPayParam tenpayParam)
        {
            StringBuilder sbxml = new StringBuilder();
            //创建请求对象
            BaseSplitRequestHandler reqHandler = new BaseSplitRequestHandler(HttpContext.Current);

            //通信对象
            TenpayHttpClient httpClient = new TenpayHttpClient();

            //应答对象
            ScriptClientResponseHandler resHandler = new ScriptClientResponseHandler();

            //-----------------------------
            //设置请求参数
            //-----------------------------
            reqHandler.init();
            reqHandler.setKey(key);
            reqHandler.setGateUrl("https://mch.tenpay.com/cgi-bin/inquire_refund.cgi");

            reqHandler.setParameter("cmdno", "96");
            reqHandler.setParameter("version", "4");
            reqHandler.setParameter("bargainor_id", bargainor_id);		//商户号
            reqHandler.setParameter("sp_billno", tenpayParam.Orderid); //商家订单号
            reqHandler.setParameter("transaction_id", tenpayParam.PayNo);//财付通订单号
            reqHandler.setParameter("return_url", "http://127.0.0.1/");			//后台系统调用，必现填写为http://127.0.0.1/
            reqHandler.setParameter("date", tenpayParam.Date);
           
            //设置通信参数
            httpClient.setCertInfo(pfxPath, bargainor_id);
            string requestUrl = reqHandler.getRequestURL();
            //设置请求内容
            httpClient.setReqContent(requestUrl);
            //设置超时
            httpClient.setTimeOut(10);
            string rescontent = "";
            //后台调用
            if (httpClient.call())
            {
                //获取结果
                rescontent = httpClient.getResContent();

                resHandler.setKey(key);
                //设置结果参数
                resHandler.setContent(rescontent);

                //判断签名及结果
                if (resHandler.isTenpaySign() && resHandler.getParameter("pay_result")=="0")
                {
                    sbxml.Append("订单已支付成功!\r\n");
                    sbxml.AppendFormat("财付通订单号:{0}\r\n", resHandler.getParameter("transaction_id"));
                    sbxml.AppendFormat("支付时间:{0}\r\n", resHandler.getParameter("pay_time"));
                    sbxml.AppendFormat("bus_type:{0}\r\n", resHandler.getParameter("bus_type"));
                    sbxml.AppendFormat("分账参数:{0}\r\n", resHandler.getParameter("bus_args"));
                    if (!string.IsNullOrEmpty(resHandler.getParameter("bus_split_refund_args")))
                        sbxml.AppendFormat("分账回退成功:{0}\r\n", resHandler.getParameter("bus_split_refund_args"));
                    else
                        sbxml.AppendFormat("未分账回退\r\n");
                    if (!string.IsNullOrEmpty(resHandler.getParameter("bus_platform_re")))
                        sbxml.AppendFormat("平台退款成功:{0}\r\n", resHandler.getParameter("bus_platform_re"));
                    else
                        sbxml.Append("未平台退款\r\n");
                    if (!string.IsNullOrEmpty(resHandler.getParameter("bus_freeze_args")))
                        sbxml.AppendFormat("已冻结:{0}\r\n", resHandler.getParameter("bus_freeze_args"));
                    else
                        sbxml.Append("账户未冻结\r\n");
                }
                else
                {
                    sbxml.Append("业务错误信息或签名错误\r\n返回码:" + resHandler.getParameter("pay_result") + "\r\n返回信息:" + resHandler.getParameter("pay_info"));
                }
            }
            else
            {
                //后台调用通信失败
                sbxml.Append("call err:" + httpClient.getErrInfo() + "\r\n" + httpClient.getResponseCode());
                //有可能因为网络原因，请求已经处理，但未收到应答。
            }
            return sbxml.ToString();

        }
        /// <summary>
        /// 记录文本日志
        /// </summary>
        /// <param name="content">记录内容</param>
        private void OnErrorNew(string errContent)
        {
            try
            {
                #region 记录文本日志

                string errlog = "记录时间：" + DateTime.Now.ToString();
                errlog += "\r\n  记录信息:" + errContent;
                errlog += "\r\n----------------------------------------------------------------------------------------------------";
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logs\\LogicPay\\TenPay\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
                fs.WriteLine(errlog);
                fs.Close();

                #endregion
            }
            catch
            {

            }
        }
    }
}