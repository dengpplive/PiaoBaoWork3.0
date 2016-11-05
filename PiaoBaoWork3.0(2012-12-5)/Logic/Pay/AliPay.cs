using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Web.UI;
using PbProject.Model.PayParam;
using System.Xml;

namespace PbProject.Logic.Pay
{
    /// <summary>
    ///支付宝接口操作
    /// </summary>
    public class AliPay
    {
        #region 参数定义


        ///// <summary>
        ///// 分账帐号
        ///// </summary>
        //public string _serveremail = "pay@chinacgt.com"; //新
        ///// <summary>
        ///// 安全校验码
        ///// </summary>
        //public string _code = "wzju0u3crc2ggqup3j7bmeirjwv87220";//新
        ///// <summary>
        ///// 合作伙伴ID
        ///// </summary>
        //public string _partner = "2088701598548382";//新

        /// <summary>
        /// 分账帐号
        /// </summary>
        public string _serveremail = "pay@mypb.cn"; //新
        /// <summary>
        /// 安全校验码
        /// </summary>
        public string _code = "cbbw2k1l2zt551nhrgixp42meah5dt66";//新
        /// <summary>
        /// 合作伙伴ID
        /// </summary>
        public string _partner = "2088801432717360";//新

        /// <summary>
        /// 返回地址
        /// </summary>
        public string _returnurl = "";
        /// <summary>
        /// 通知地址 
        /// </summary>
        public string _notifyurl = "";
        /// <summary>
        /// 收款帐号
        /// </summary>
        public string _receiceemail = "";

        /// <summary>
        /// 支付宝接口地址
        /// </summary>
        public string _alipayurl = "https://mapi.alipay.com/gateway.do?";  //_alipayurl ="https://www.alipay.com/cooperate/gateway.do?"；

        /// <summary>
        /// 支付宝接口返回地址
        /// </summary>
        public string _alipayreurl = "http://notify.alipay.com/trade/notify_query.do?";  //  string _alipayreurl = "https://mapi.alipay.com/trade/notify_query.do?";

        /// <summary>
        /// 支付宝接口交易费率，收取费率下限为1元，上线不能操作30W
        /// </summary>
        public decimal _rates = 0.001M;
        /// <summary>
        /// 供应收款费率
        /// </summary>
        public decimal _supperates = 0;

        /// <summary>
        /// 默认参数
        /// </summary> 
        public AliPay()
        {
            try
            {
                _returnurl = ConfigurationManager.AppSettings["_AliPayReturnUrl"].ToString();
                _notifyurl = ConfigurationManager.AppSettings["_AliPayNotifyUrl"].ToString();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 默认参数
        /// </summary> 
        public AliPay(bool type)
        {

        }

        /// <summary>
        /// 支付宝通知类型
        /// </summary>
        /// <param name="notifytype">通知参数</param>
        /// <returns></returns>
        public int ReturnType(string notifytype)
        {
            //支付通知
            if (notifytype == "trade_status_sync")
            {
                return 1;
            }
            //退款通知
            if (notifytype == "batch_refund_notify")
            {
                return 2;
            }
            return 0;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 支付宝签名MD5
        /// </summary>
        /// <param name="parameter">需要加密的参数字符串，格式为参数字符串+安全校验码</param>
        /// <param name="_input_charset">字符格式，不传则默认为utf-8</param>
        /// <returns>返回</returns>
        public string GetMD5(string parameter, string _input_charset)
        {
            if (_input_charset.Trim() == "")
            {
                _input_charset = "utf-8";
            }
            /// <summary>
            /// 与ASP兼容的MD5加密算法
            /// </summary> 
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(parameter));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把需要签名的参数数组按英文字母升序排列
        /// </summary>
        /// <param name="r">参数数组</param>
        /// <returns>返回数组</returns>
        public string[] BubbleSort(string[] r)
        {
            /// <summary>
            /// 冒泡排序法
            /// </summary> 
            int i, j; //交换标志 
            string temp;
            bool exchange;
            for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false;
                //本趟排序开始前，交换标志应为假
                for (j = r.Length - 2; j >= i; j--)
                {
                    if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)　//交换条件
                    {
                        temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;

                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }
                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// GET方式提交到指定URL
        /// </summary>
        /// <param name="url">要提交的URL地址包含参数</param> 
        /// <returns>返回请求页面的HTML</returns>
        public string GetUrlData(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    WebClient webclient = new WebClient();
                    if (url.Length < 255)
                    {
                        byte[] pagedate = webclient.DownloadData(url);
                        return Encoding.Default.GetString(pagedate);
                    }
                    else
                    {
                        //get 转换成 post 提交
                        System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
                        string[] strValues = url.Split('?')[1].Split('&');
                        for (int i = 0; i < strValues.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(strValues[i]))
                            {
                                string[] strNew = strValues[i].Split('=');
                                nvc.Add(strNew[0], strNew[1]);
                            }
                        }
                        byte[] pagedate = webclient.UploadValues(url, "POST", nvc);
                        return Encoding.Default.GetString(pagedate);
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取远程服务器ATN结果
        /// </summary>
        /// <param name="a_strUrl"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public string Get_Http(string a_strUrl, int timeout)
        {
            string strResult;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(a_strUrl);
                myReq.Timeout = timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }
                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {

                strResult = "错误：" + exp.Message;
            }
            return strResult;
        }

        /// <summary>
        /// 截取2个匹配之间的字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="startStr">前一个匹配</param>
        /// <param name="lastStr">后一个匹配</param>
        /// <returns>返回第一个匹配项</returns>
        public string GetString(string str, string startStr, string lastStr)
        {
            string regexStr = @"" + startStr + "([^<]*)" + lastStr + "";
            Match mc = Regex.Match(str, regexStr, RegexOptions.IgnoreCase);
            string strResult = mc.Groups[0].ToString();
            strResult = strResult.Replace(startStr, "");
            strResult = strResult.Replace(lastStr, "");
            strResult = strResult.Replace("\r", "");
            strResult = strResult.Replace("\n", "");
            strResult = strResult.Replace("\t", "");
            return strResult.Trim();

        }

        /// <summary>
        /// 返回GET方式提交的参数字符串
        /// </summary>
        /// <param name="parameter">参数数组，不包含sign和sign_type参数，构造为“参数名=参数值”格式</param>
        /// <param name="_input_charset">字符格式，不传则默认为utf-8</param>
        /// <returns>返回字符串</returns>
        public string ReturnUrl(string[] parameter, string _input_charset)
        {
            if (_input_charset.Trim() == "")
            {
                _input_charset = "utf-8";
            }
            string[] Sortedstr = BubbleSort(parameter);
            StringBuilder prestr = new StringBuilder();
            int i;
            for (i = 0; i < Sortedstr.Length; i++)
            {
                if ((Sortedstr[i].Split('=')[1]) != "")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr.Append(Sortedstr[i]);
                    }
                    else
                    {
                        prestr.Append(Sortedstr[i] + "&");
                    }
                }
            }
            prestr.Append(_code);
            //生成签名方式
            string sign = GetMD5(prestr.ToString(), _input_charset);
            //构造支付Url
            StringBuilder url = new StringBuilder();
            for (i = 0; i < Sortedstr.Length; i++)
            {
                if ((Sortedstr[i].Split('=')[1]) != "")
                {
                    url.Append(Sortedstr[i].Split('=')[0] + "=" + HttpUtility.UrlEncode(Sortedstr[i].Split('=')[1]) + "&");
                }

            }
            url.Append("sign=" + sign + "&sign_type=MD5");
            return url.ToString();
        }

        /// <summary>
        /// 返回构造好的签名字符串
        /// </summary>
        /// <param name="parameter">参数数组，不包含sign和sign_type参数，构造为“参数名=参数值”格式</param>
        /// <param name="_input_charset">字符格式，不传则默认为utf-8</param>
        /// <returns>返回字符串</returns>
        public string ReturnParameter(string[] parameter, string _input_charset)
        {
            if (_input_charset.Trim() == "")
            {
                _input_charset = "utf-8";
            }
            string[] Sortedstr = BubbleSort(parameter);
            StringBuilder prestr = new StringBuilder();
            int i;
            for (i = 0; i < Sortedstr.Length; i++)
            {
                if ((Sortedstr[i].Split('=')[1]) != "")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr.Append(Sortedstr[i]);
                    }
                    else
                    {
                        prestr.Append(Sortedstr[i] + "&");
                    }
                }
            }
            prestr.Append(_code);
            //生成签名方式
            string sign = GetMD5(prestr.ToString(), _input_charset);
            return sign;
        }

        #endregion

        #region 支付宝支付方法

        /// <summary>
        /// 支付分润接口
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns>返回构造好的post表单</returns>
        public string GetPay(PbProject.Model.PayParam.AliPayParam aliPayParam)
        {
            string returnformValue = "";
            try
            {
                #region 参数赋值

                string out_trade_no = aliPayParam.Out_trade_no;  //0.内部订单号
                string subject = aliPayParam.Subject;//1.商品名称
                string body = aliPayParam.Body;//2.商品描述
                string total_fee = aliPayParam.Total_fee;  //3.订单总价
                string royalty_parameters = aliPayParam.Royalty_parameters;//4.分润参数
                string extend_param = aliPayParam.Extend_param; // 5.公用业务扩展参数，支付宝用于 显示 PNR （格式：参数名1^参数值1|参数名2^参数值2|......）
                string extra_common_param = aliPayParam.Extra_common_param;//6.自定义字段
                string defaultBank = aliPayParam.DefaultBank; //7.网银标示

                string service = "create_direct_pay_by_user";
                string payment_type = "1"; //支付类型(商品购买)，默认为1,
                string seller_email = _serveremail;  //分账帐号
                string partner = _partner;//合作伙伴ID
                string show_url = "www.alipay.com";  //支付宝地址
                string return_url = _returnurl;
                string notify_url = _notifyurl;
                string _input_charset = "utf-8";
                string sign_type = "MD5"; //签名方式

                //分润类型，默认为10
                string royalty_type = "10";
                string sign = "";

                #endregion

                #region 签名赋值
                List<string> list = new List<string>();
                list.Add("service=" + service);
                list.Add("partner=" + partner);
                list.Add("subject=" + subject);
                list.Add("body=" + body);
                list.Add("out_trade_no=" + out_trade_no);
                list.Add("total_fee=" + total_fee);
                list.Add("show_url=" + show_url);
                list.Add("payment_type=" + payment_type);
                list.Add("seller_email=" + seller_email);
                list.Add("notify_url=" + notify_url);
                list.Add("_input_charset=" + _input_charset);
                list.Add("return_url=" + return_url);
                list.Add("royalty_parameters=" + royalty_parameters);
                list.Add("royalty_type=" + royalty_type);
                //list.Add("default_login=Y" );//自动登录标识
                if (!string.IsNullOrEmpty(defaultBank) && defaultBank.Trim() != "")
                    list.Add("defaultbank=" + defaultBank); //网银使用
                if (!string.IsNullOrEmpty(defaultBank) && defaultBank.Trim() != "")
                    list.Add("paymethod=bankPay"); //网银使用

                if (!string.IsNullOrEmpty(extend_param) && extend_param.Trim() != "")
                    list.Add("extend_param=" + extend_param);//公用业务扩展参数
                list.Add("extra_common_param=" + extra_common_param);
                string[] parameter = list.ToArray();
                //签名
                sign = ReturnParameter(parameter, "");
                #endregion

                #region 组合表单赋值

                StringBuilder sb = new StringBuilder();
                sb.Append("<form name='alipaysubmit' method='post' action='" + _alipayurl + "_input_charset=" + _input_charset + "'>");
                sb.Append("<input type='hidden' name='body' value=" + body + ">");
                //sb.Append("<input type='hidden' name='default_login' value=Y>"); //自动登录标识
                if (!string.IsNullOrEmpty(defaultBank) && defaultBank.Trim() != "") //网银使用
                    sb.Append("<input type='hidden' name='defaultbank' value=" + defaultBank + ">");
                sb.Append("<input type='hidden' name='notify_url' value=" + notify_url + ">");
                sb.Append("<input type='hidden' name='out_trade_no' value=" + out_trade_no + ">");
                sb.Append("<input type='hidden' name='partner' value=" + partner + ">");
                if (!string.IsNullOrEmpty(defaultBank) && defaultBank.Trim() != "") //网银使用
                    sb.Append("<input type='hidden' name='paymethod' value='bankPay'>");
                sb.Append("<input type='hidden' name='payment_type' value=" + payment_type + ">");
                sb.Append("<input type='hidden' name='return_url' value=" + return_url + ">");
                sb.Append("<input type='hidden' name='seller_email' value=" + seller_email + ">");
                sb.Append("<input type='hidden' name='service' value=" + service + ">");
                sb.Append("<input type='hidden' name='show_url' value=" + show_url + ">");
                sb.Append("<input type='hidden' name='subject' value=" + subject + ">");
                sb.Append("<input type='hidden' name='total_fee' value=" + total_fee + ">");
                sb.Append("<input type='hidden' name='sign' value=" + sign + ">");
                sb.Append("<input type='hidden' name='sign_type' value=" + sign_type + ">");
                sb.Append("<input type='hidden' name='royalty_parameters' value=" + royalty_parameters + ">");
                sb.Append("<input type='hidden' name='royalty_type' value=" + royalty_type + ">");
                if (!string.IsNullOrEmpty(extend_param) && extend_param.Trim() != "")
                    sb.Append("<input type='hidden' name='extend_param' value=" + extend_param + ">");  //公用业务扩展参数
                sb.Append("<input type='hidden' name='extra_common_param' value=" + extra_common_param + ">");
                sb.Append("</form>");
                sb.Append("<script>");
                sb.Append("document.alipaysubmit.submit()");
                sb.Append("</script>");

                #endregion

                OnErrorNew("GetPay() 支付分润接口(提交数据): sb: " + sb.ToString()); //监控记录日志

                returnformValue = sb.ToString();
            }
            catch (Exception ex)
            {

            }
            return returnformValue;
        }

        /// <summary>
        /// 返回构造好的退款URL地址
        /// </summary>
        /// <param name="strValue">数据集</param>
        /// <returns>提交结果</returns>
        public bool IsRefund(string[] strValue)
        {
            bool result = false;
            try
            {
                #region 参数赋值

                string batch_no = strValue[0];//0.批次号规则
                string batch_num = strValue[1];//1.要退款的支付宝交易号
                string detail_data = strValue[2];//2.退款参数

                //业务参数赋值； 
                string service = "refund_fastpay_by_platform_nopwd";
                string partner = _partner;
                string refund_date = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                string notify_url = _notifyurl;
                string _input_charset = "utf-8";
                string return_type = "xml";

                //构造数组
                string[] parameter ={ 
             "service="+service, 
            "partner=" + partner, 
            "batch_no=" + batch_no, 
            "refund_date=" + refund_date, 
            "batch_num=" + batch_num, 
            "detail_data=" + detail_data, 
            "notify_url=" + notify_url,
            "_input_charset="+  _input_charset,
            "return_type="+ return_type,
            };
                #endregion

                //提交数据
                string url = _alipayurl + ReturnUrl(parameter, "");
                string returnValue = GetUrlData(url);

                OnErrorNew("GetPay() 支付分润接口(提交数据): url: " + url + ",returnValue:" + returnValue); //监控记录日志

                //返回提交结果
                if (GetString(returnValue, "<is_success>", "</is_success>").Trim().ToUpper() == "T")
                {
                    result = true;  // 提交成功
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

        /// <summary>
        /// 用户签约查询接口
        /// </summary>
        /// <param name="email">要查询的用户Email</param> 
        /// <returns>返回结果</returns>
        public bool IsUserSign(string email)
        {
            bool reuslt = false;
            try
            {
                #region 参数赋值

                string service = "query_customer_protocol";
                string partner = _partner;
                string user_email = email;
                string biz_type = "10004";
                string _input_charset = "utf-8";
                //构造数组；
                string[] parameter ={ 
            "service="+service, 
            "partner=" + partner,
            "user_email="+user_email,
            "biz_type="+biz_type,
            "_input_charset="+_input_charset
                };
                #endregion

                //提交
                string url = _alipayurl + ReturnUrl(parameter, "");
                string result = GetUrlData(url);
                if (GetString(result, "<is_success>", "</is_success>").Trim().ToUpper() == "T")
                {
                    reuslt = true;
                }
            }
            catch (Exception)
            {

            }

            return reuslt;
        }

        /// <summary>
        /// 用户在线签约接口
        /// </summary>
        /// <param name="semail">需要签约的Email</param>
        /// <returns></returns>
        public string GetUserSign(string semail)
        {
            string strUrl = "";
            try
            {
                #region 参数赋值

                string service = "sign_protocol_with_partner";
                string partner = _partner;
                string email = semail;
                string return_url = _returnurl;
                string notify_url = _notifyurl;
                string _input_charset = "utf-8";

                //构造数组；
                string[] parameter ={ 
            "service="+service, 
            "partner=" + partner, 
            "email=" + email, 
            "return_url=" + return_url, 
            "notify_url=" + notify_url,
            "_input_charset="+  _input_charset,
            };

                #endregion

                //提交数据
                strUrl = _alipayurl + ReturnUrl(parameter, "");
            }
            catch (Exception)
            {

            }
            return strUrl;
        }

        /// <summary>
        /// 自动出票用户签约自动支付接口
        /// </summary>
        /// <param name="semail">需要签约的Email</param>
        /// <returns></returns>
        public string GetAutoCPUserSign(string semail)
        {
            string strUrl = "";
            try
            {
                string service = "sign_protocol_with_partner";
                string partner = _partner;
                string email = semail;
                string _input_charset = "utf-8";

                //构造数组；
                string[] parameter ={ 
            "service="+service, 
            "partner=" + partner, 
            "email=" + email, 
            "_input_charset="+  _input_charset,
            };

                strUrl = "https://mapi.alipay.com/gateway.do?" + ReturnUrl(parameter, "");
            }
            catch (Exception)
            {

            }
            return strUrl;
        }

        /// <summary>
        /// 用户在线解约接口
        /// </summary>
        /// <param name="semail">需要解约的Email</param>
        /// <returns></returns>
        public bool UnUserSign(string semail)
        {
            bool reuslt = false;
            try
            {
                string service = "customer_unsign";
                string partner = _partner;
                string user_email = semail;
                //业务代码，默认值
                string biz_type = "10004";
                string _input_charset = "utf-8";

                //构造数组；
                string[] parameter ={ 
            "service="+service, 
            "partner=" + partner,  
            "user_email=" + user_email, 
            "biz_type=" + biz_type,  
            "_input_charset="+  _input_charset,
            };
                string url = _alipayurl + ReturnUrl(parameter, "");
                string result = GetUrlData(url);
                if (GetString(result, "<is_success>", "</is_success>").Trim().ToUpper() == "T")
                {
                    reuslt = true;
                }
            }
            catch (Exception)
            {
            }
            return reuslt;
        }

        /// <summary>
        /// 商户签约查询接口
        /// </summary> 
        /// <returns></returns>
        public string IsCustomerSign()
        {
            string strUrl = "";
            try
            {
                string service = "query_partner_protocol";
                string partner = _partner;
                string biz_type = "10004";
                string _input_charset = "utf-8";

                //构造数组；
                string[] parameter ={ 
            "service="+service, 
            "partner=" + partner,  
            "biz_type=" + biz_type,  
            "_input_charset="+  _input_charset,
            };

                strUrl = _alipayurl + ReturnUrl(parameter, "");
            }
            catch (Exception)
            {

            }
            return strUrl;
        }

        /// <summary>
        /// 退款查询接口
        /// </summary>   
        /// <param name="alipayno">支付宝交易号</param>
        /// <returns>数组0为成功标志，数组1为内容</returns>
        public string[] QueryRefundResult(string alipayno)
        {
            string service = "refund_fastpay_query";
            string partner = _partner;
            string _input_charset = "utf-8";
            string trade_no = alipayno;

            //构造数组；
            string[] parameter ={ 
            "service="+service, 
            "partner=" + partner,    
            "_input_charset="+  _input_charset, 
            "trade_no=" + alipayno,
            };

            string url = _alipayurl + ReturnUrl(parameter, "");
            string result = GetUrlData(url);

            if (result.Contains("&"))
            {
                string[] str = result.Split('&');
                if (result.Contains("is_success=T"))
                {
                    str[0] = "T";
                    str[1] = str[1].Replace("result_details=", "");
                    string[] s = str[1].Split('^');
                    str[1] = "";
                    for (int i = 1; i < s.Length; i++)
                    {
                        if (i == s.Length - 1)
                        {
                            str[1] += s[i].ToString();
                        }
                        else
                        {
                            str[1] += s[i].ToString() + "^";
                        }
                    }
                }
                else
                {
                    str[0] = "F";
                    str[1] = str[1].Replace("error_code=", "");
                }
                return str;
            }
            else
            {
                string[] str = new string[2];
                str[0] = "F";
                str[1] = result;
                return str;
            }
        }

        /// <summary>
        /// 退款查询接口
        /// </summary>   
        /// <param name="alipayno">支付宝交易号</param>
        /// <returns></returns>
        public string QueryRefundResultStr(string alipayno)
        {
            string result = "";

            try
            {
                string service = "refund_fastpay_query";
                string partner = _partner;//合作伙伴ID
                string _input_charset = "utf-8";
                string trade_no = alipayno;

                //构造数组；
                string[] parameter ={ 
            "service="+service, 
            "partner=" + partner,    
            "_input_charset="+  _input_charset, 
            "trade_no=" + alipayno,
            };

                string url = _alipayurl + ReturnUrl(parameter, "");

                result = GetUrlData(url);

                //string[] str = result.Split('&');
                //if (result.Contains("is_success=T"))
                //{
                //    str[0] = "T";
                //    str[1] = str[1].Replace("result_details=", "");
                //    string[] s = str[1].Split('^');
                //    str[1] = "";
                //    for (int i = 1; i < s.Length; i++)
                //    {
                //        if (i == s.Length - 1)
                //        {
                //            str[1] += s[i].ToString();
                //        }
                //        else
                //        {
                //            str[1] += s[i].ToString() + "^";
                //        }
                //    }
                //    return str;
                //}
                //else
                //{
                //    str[0] = "F";
                //    str[1] = str[1].Replace("error_code=", "");
                //}
                //return str;
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 支付状态查询
        /// </summary>   
        /// <param name="orderId">支付订单号</param>
        public string TradeQuery(string orderId)
        {
            string result = "";

            try
            {
                string service = "single_trade_query";
                string partner = _partner;//合作伙伴ID
                string _input_charset = "GBK";
                string trade_no = orderId;

                //构造数组；
                string[] parameter ={ 
            "service="+service, 
            "partner=" + partner,    
            "_input_charset="+  _input_charset, 
            "out_trade_no=" + orderId,
            };

                string url = _alipayurl + ReturnUrl(parameter, "");

                result = GetUrlData(url);
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        #endregion

        #region 机票业务方法

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="page"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AliPayRefund(System.Web.UI.Page page, string id)
        {
            string[] detail = ReturnRefundDetails(id);
            return IsRefund(detail);
        }

        /// <summary>
        /// 构造退款字符串
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        private string[] ReturnRefundDetails(string id)
        {

            string[] Details = new string[3];
            /*
           
            Order o = Factory_Air.CreateIPayOrder();
            //订单
            PiaoBao.Models.Tb_Ticket_Order mOrder = o.GetOrder(id);
            //乘客
            IList<PiaoBao.Models.Tb_Ticket_Passenger> mPassenger = o.GetPassenger(mOrder.OrderId);
            //供应公司
            PiaoBao.Models.User_Company mSupplyCompany = o.GetCompany();

            string OldPayfee = mOrder.PayFee;
            //更新费率
            //PiaoBao.BLLLogic.Pay.Data d = new Data();
            PiaoBao.BLLLogic.Pay.Data d = PiaoBao.BLLLogic.Factory_Air.CreateData();

            if (mOrder.OrderStatusCode > 5)
            {
                d.UpdateOrderPassengerData(_rates, _supperates, ref mOrder, ref mPassenger);

                #region 退款计算,支付公司退费率

                PiaoBao.Models.Tb_Ticket_Order mOrderOld = o.GetOrder(mOrder.OldOrderId);

                if (mOrderOld != null)
                {
                    string[] ZFSXPriceDetailsOld = mOrderOld.PayHandlingFee.Split('/');//原订单支付费率
                    decimal priceOld = decimal.Parse(mOrderOld.PayFee.Split('/')[2]); //原订单支付金额
                    decimal pirceNew = decimal.Parse(OldPayfee.Split('/')[2]); //退款金额
                    decimal priceHandlingFee = pirceNew / priceOld * decimal.Parse(ZFSXPriceDetailsOld[1]);
                    priceHandlingFee = d.FourToFiveNum(priceHandlingFee, 2);

                    mOrder.PayHandlingFee = ZFSXPriceDetailsOld[0] + "/" + priceHandlingFee.ToString() + "/" + ZFSXPriceDetailsOld[2];
                }
                #endregion
            }

            //支付宝设置字段
            string[] value = mSupplyCompany.ZFBAccount.Split('|');

            #region 收款账号
            string account = value[0].Trim();
            //供应收款账号
            if (mOrder.PolicySource < 2)
            {
                if (mSupperConfig != null && !string.IsNullOrEmpty(mSupperConfig.a1) && mSupperConfig.a1.Contains("|66|"))
                {
                    if (!string.IsNullOrEmpty(mSupplyCompany.A85) && mSupplyCompany.A85.Trim() != "")
                    {
                        account = mSupplyCompany.A85.Trim();
                    }
                }
            }

            #endregion

            //结算价字符串集合
            string[] PriceDetails = mOrder.PayFee.Split('/');

            //订单退款总价
            decimal Total = decimal.Parse(PriceDetails[2]);
            //订单退改签总价
            decimal TGQ = mOrder.TGQHandlingFee;

            //订单退改签手续费
            //decimal TGQFee = TGQ * _supperates;
            //TGQFee = Math.Round(TGQFee + 0.005M - 0.0001M, 2); // 保留两位小数（只入不舍）
            //TGQFee = Math.Round(TGQFee, 2);  // 采用四舍五入
            //TGQFee = d.FourToFiveNum(TGQFee, 2);// 采用四舍五入

            //支付手续费字符串集合
            string[] ZFSXPriceDetails = mOrder.PayHandlingFee.Split('/');
            //供应支付手续费(包含支付公司手续费)
            decimal ZFSXPrice = decimal.Parse(ZFSXPriceDetails[1]);
            //供应实退 = 订单退款总价 - 手续费(供应收款手续费) - 供应商退改签费用 + 供应商退改签费用（手续费）
            //decimal Price = decimal.Parse(PriceDetails[2]) - ZFSXPrice - TGQ + TGQFee;
            decimal Price = Total - ZFSXPrice - TGQ;
            //我们实退，减去的手续费
            decimal Our = 0;
            Total = Total - TGQ;

            //Random r = new Random();
            //int x1 = r.Next(0, 9);
            //int x2 = r.Next(0, 9);
            //int x3 = r.Next(0, 9);
            //string x = x1.ToString() + x2.ToString() + x3.ToString();
            //string bno = DateTime.Now.ToString("yyyyMMdd") + x;

            string strTime = DateTime.Now.ToString("yyyyMMdd");
            string bno = strTime + mOrder.OrderId;

            Details[0] = bno;
            Details[1] = "1";
            Details[2] += mOrder.OnlineIndex + "^" + Total.ToString("F2") + "^退款|" + "";

            if (Our != 0)
            {
                Details[2] += account + "^^" + _serveremail + "^^" + Price.ToString("F2") + "^退款|" + _receiceemail + "^^" + _serveremail + "^^" + Our.ToString("F2") + "^退款";
            }
            else
            {
                Details[2] += account + "^^" + _serveremail + "^^" + Price.ToString("F2") + "^退款";
            }

            try
            {
                #region 新

                if (mOrder.OrderStatusCode > 5)
                {
                    string updateSql = "update tb_ticket_order set PayFee='" + OldPayfee + "' where OrderId='" + mOrder.OrderId + "'";

                    PiaoBao.BLLLogic.Factory_Air.CreateITb_Ticket_OrderManager().ExecuteBySql(updateSql, null);
                }

                #endregion
            }
            catch (Exception ex)
            {

            }
             * */

            return Details;
        }
        #endregion

        #region 记录文本日志

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
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logs\\LogicPay\\AliPay\\";
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

        #endregion

        #region 自动出票方法

        /// <summary>
        /// RequestGet
        /// </summary>
        /// <param name="TheURL"></param>
        /// <returns></returns>
        private string RequestGet(string TheURL)
        {
            Uri uri = new Uri(TheURL);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            string page;
            try
            {

                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;

                request.Method = "GET";

                request.ContentType = "application/x-www-form-urlencoded";

                request.AllowAutoRedirect = true;

                request.MaximumAutomaticRedirections = 10;

                request.Timeout = (int)new TimeSpan(0, 0, 60).TotalMilliseconds;

                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.Default);

                page = readStream.ReadToEnd();
            }
            catch (Exception ee)
            {
                page = "Fail message : " + ee.Message;
            }
            return page;

        }

        /// <summary>
        /// 支付宝本票通自动代付
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// //public bool AutoPay(PiaoBao.Models.Tb_Ticket_Order AutoOrder, PiaoBao.Models.User_Company mCompany, ref string msg)
        public bool AutoPay(PbProject.Model.Tb_Ticket_Order AutoOrder, PbProject.Model.definitionParam.BaseSwitch BS, ref string msg)
        {
            bool IsOk = false;

            try
            {
                string Url = ConfigurationManager.AppSettings["AliPayAutoCPUrl"].ToString();

                #region 构造URL参数
                #region 基础数据
                if (AutoOrder.BigCode == "")
                {
                    msg = "大编码为空！";
                    return IsOk;
                }
                string cmd = "orderex";
                string fmt = "xml";
                string pnr = AutoOrder.BigCode;
                string bigpnr = "1";
                string air = AutoOrder.CarryCode;
                string pnrc = AutoOrder.PNR;
                string b2buser = "";
                string autopayflag = "1";
                string callbackurl = _notifyurl.Replace("AliPayNotifyUrl.aspx", "AutoPayByAlipayNotifyUrl.aspx");
                string pnrsrcid = AutoOrder.OrderId;
                string checkprice = "0-" + (AutoOrder.PMFee + AutoOrder.ABFee + AutoOrder.FuelFee).ToString();
                string xmlhashead = "false";
                string b2bpwd = "";
                string getticketovertime = "3";
                string payaccount = "";
                if (BS.AutoPayAccount != "" && BS.AutoPayAccount.Split('^')[2] != "")
                {
                    payaccount = BS.AutoPayAccount.Split('^')[2].Split('|')[0].ToString();
                }
                else
                {
                    msg = "自动支付支付宝未绑定！";
                    return IsOk;
                }
                #endregion
                #region 航空公司数据

                string hu_linktel = "13076056938";         //海航联系电话
                string cz_linktel = "13076056938";
                string hu_linkman = HttpUtility.UrlEncode("王永磊");         //海航联系人
                #endregion

                Url = Url + "cmd=" + cmd + "&fmt=" + fmt + "&pnr=" + pnr + "&bigpnr=" + bigpnr + "&air=" + air + "&autopayflag=" + autopayflag + "&callbackurl=" + callbackurl + "&pnrsrcid=" + pnrsrcid + "&xmlhashead=" + xmlhashead + "&getticketovertime=" + getticketovertime + "&payaccount=" + payaccount + "&checkprice=" + checkprice;
                string[] CarrList = BS.AutoAccount.Split(new string[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries); // Regex.Split(BS.AutoAccount, "^^^", RegexOptions.IgnoreCase);
                bool IsExist = false;
                for (int i = 0; i < CarrList.Length; i++)
                {
                    if (CarrList[i].Contains(AutoOrder.CarryCode) && CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[0].Split(':')[0].ToUpper() == AutoOrder.CarryCode.ToUpper())
                    {
                        if (AutoOrder.CarryCode == "HU")
                        {
                            Url = Url + "&hu_linktel=" + hu_linktel + "&hu_linkman=" + hu_linkman;
                        }
                        if (AutoOrder.CarryCode == "CZ")
                        {
                            Url = Url + "&cz_linktel=" + cz_linktel;
                        }
                        b2buser = CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[0].Split(':')[1];  //Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        b2bpwd = CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[1]; //Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];                                      
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    msg = AutoOrder.CarryCode + "帐号和密码为空，不能自动出票!";
                    return IsOk;
                }

                Url = Url + "&b2buser=" + b2buser + "&b2bpwd=" + b2bpwd;
                #endregion

                string sss = RequestGet(Url);
                //string sss = "";

                OnErrorNew("AutoPay():自动出票URL地址() url:" + Url + " \r\n result:" + sss + " \r\n");

                if (sss.Contains("errorinfo"))
                {
                    sss = "<sss>" + sss + "</sss>";
                }

                StringReader rea = new StringReader(sss.Replace(" ", ""));
                System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(rea);
                DataSet ds = new DataSet();
                ds.ReadXml(xmlReader);


                if (ds.Tables[0].TableName == "sss")
                {
                    msg = ds.Tables[0].Rows[0]["errorinfo"].ToString();
                }
                else
                {
                    if (ds.Tables[0].Rows[0]["code"].ToString() == "1" && ds.Tables[0].Rows[0]["paystatus"].ToString() == "1")
                    {
                        IsOk = true;
                        msg = "自动出票启动！";
                    }
                    else
                    {
                        msg = ds.Tables[0].Rows[0]["errorInfo"].ToString();
                        if (msg == "该订单正在被其他业务处理中，请您稍后再提交请求！")
                        {
                            IsOk = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return IsOk;
        }

        #endregion

        #region 获取B2B实时政策
        /// <summary>
        /// 获取B2B实时政策信息
        /// </summary>
        /// <param name="AutoOrder">订单信息</param>
        /// <param name="BS">账户参数信息</param>
        /// <param name="msg">返回消息</param>
        /// <returns>成功:true;失败:false</returns>
        public bool QueryPriceByPNR(PbProject.Model.Tb_Ticket_Order AutoOrder, PbProject.Model.definitionParam.BaseSwitch BS, ref string msg)
        {
            bool IsOk = false;

            #region 示例发送请求及结果返回
            //http://210.14.138.26:6350/alidz.do?cmd=newquerypricebypnr&fmt=xml&pnr=MT2GB2&bigpnr=1&air=3U&ticketprice=&xmlhashead=false&b2buser=SCH640&b2bpwd=888006x

            //<pnrinfo><code>1</code><pnr>MT2GB2</pnr><message></message><air>3U</air><policies><policy><pgid>1</pgid><pgcode>4043972</pgcode><ticketprice>1180.0</ticketprice><policynum>6.00</policynum><totaltax>180.0</totaltax><payprice>1289.2</payprice><fc>-</fc></policy></policies></pnrinfo>
            #endregion
            try
            {
                string Url = ConfigurationManager.AppSettings["AliPayAutoCPUrl"].ToString();

                #region 构造URL参数
                #region 基础数据
                if (AutoOrder.BigCode == "")
                {
                    msg = "大编码为空！";
                    return IsOk;
                }
                string cmd = "newquerypricebypnr";
                string fmt = "xml";
                string pnr = AutoOrder.BigCode;
                string bigpnr = "1";
                string air = AutoOrder.CarryCode;
                string pnrc = AutoOrder.PNR;
                string b2buser = "";
                string ticketprice = "";
                //string callbackurl = _notifyurl.Replace("AliPayNotifyUrl.aspx", "AutoPayByAlipayNotifyUrl.aspx");
                //string pnrsrcid = AutoOrder.OrderId;
                //string checkprice = "0-" + (AutoOrder.PMFee + AutoOrder.ABFee + AutoOrder.FuelFee).ToString();
                string xmlhashead = "false";
                string b2bpwd = "";
                //string getticketovertime = "3";
                //string payaccount = "";
                //string b2bencryptkey = "";
                //if (BS.AutoPayAccount != "" && BS.AutoPayAccount.Split('^')[2] != "")
                //{
                //    payaccount = BS.AutoPayAccount.Split('^')[2].Split('|')[0].ToString();
                //}
                //else
                //{
                //    msg = "自动支付支付宝未绑定！";
                //    return IsOk;
                //}
                #endregion
                #region 航空公司数据

                string hu_linktel = "13076056938";         //海航联系电话
                string cz_linktel = "13076056938";
                string hu_linkman = HttpUtility.UrlEncode("王永磊");         //海航联系人
                #endregion

                Url = Url + "cmd=" + cmd + "&fmt=" + fmt + "&pnr=" + pnr + "&bigpnr=" + bigpnr + "&air=" + air + "&ticketprice=&xmlhashead=" + xmlhashead;
                string[] CarrList = BS.AutoAccount.Split(new string[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries); // Regex.Split(BS.AutoAccount, "^^^", RegexOptions.IgnoreCase);
                bool IsExist = false;
                for (int i = 0; i < CarrList.Length; i++)
                {
                    if (CarrList[i].Contains(AutoOrder.CarryCode) && CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[0].Split(':')[0].ToUpper() == AutoOrder.CarryCode.ToUpper())
                    {
                        if (AutoOrder.CarryCode == "HU")
                        {
                            Url = Url + "&hu_linktel=" + hu_linktel + "&hu_linkman=" + hu_linkman;
                        }
                        if (AutoOrder.CarryCode == "CZ")
                        {
                            Url = Url + "&cz_linktel=" + cz_linktel;
                        }
                        b2buser = CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[0].Split(':')[1];  //Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        b2bpwd = CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[1]; //Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];                                      
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    msg = AutoOrder.CarryCode + "帐号和密码为空，不能获取政策信息!";
                    return IsOk;
                }

                Url = Url + "&b2buser=" + b2buser + "&b2bpwd=" + b2bpwd;
                #endregion

                string sss = RequestGet(Url);
                //string sss = "";

                OnErrorNew("AutoPay():获取政策URL地址() url:" + Url + " \r\n result:" + sss + " \r\n");

                if (sss.Contains("<ticketprice>"))
                {
                    IsOk = true;
                }

                msg = sss;

                //StringReader rea = new StringReader(sss.Replace(" ", ""));
                //System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(rea);
                //DataSet ds = new DataSet();
                //ds.ReadXml(xmlReader);


                //if (ds.Tables[0].TableName == "sss")
                //{
                //    msg = ds.Tables[0].Rows[0]["errorinfo"].ToString();
                //}
                //else
                //{
                //    if (ds.Tables[0].Rows[0]["code"].ToString() == "1" && ds.Tables[0].Rows[0]["paystatus"].ToString() == "1")
                //    {
                //        IsOk = true;
                //        msg = "自动出票启动！";
                //    }
                //    else
                //    {
                //        msg = ds.Tables[0].Rows[0]["errorInfo"].ToString();
                //        if (msg == "该订单正在被其他业务处理中，请您稍后再提交请求！")
                //        {
                //            IsOk = true;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return IsOk;
        }
        #endregion

        #region 批量付款
        /// <summary>
        /// 批量付款
        /// </summary>
        /// <param name="email">付款帐号</param>
        /// <param name="account_name">付款账户名</param>
        /// <param name="batch_fee">付款总金额</param>
        /// <param name="batch_num">付款笔数</param>
        /// <param name="detail_data">付款详细数据</param>
        /// <returns></returns>
        public string BatchPay(string email, string account_name, string batch_fee, string batch_num, string detail_data)
        {
            string pay_date = DateTime.Now.ToString("yyyyMMdd");
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", _partner);
            sParaTemp.Add("_input_charset", "utf-8".ToLower());
            sParaTemp.Add("service", "batch_trans_notify");
            sParaTemp.Add("notify_url", _notifyurl);//服务器异步通知页面
            sParaTemp.Add("email", email);
            sParaTemp.Add("account_name", account_name);
            sParaTemp.Add("pay_date", pay_date);
            sParaTemp.Add("batch_no", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            sParaTemp.Add("batch_fee", batch_fee);
            sParaTemp.Add("batch_num", batch_num);
            sParaTemp.Add("detail_data", detail_data);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='https://mapi.alipay.com/gateway.do?_input_charset=UTF-8' method='get'>");

            foreach (KeyValuePair<string, string> temp in sParaTemp)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='提交' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }
        #endregion

        #region CAE代扣接口

        /// <summary>
        /// 自动代扣
        /// </summary>
        /// <param name="out_order_no">商户订单号</param>
        /// <param name="subject">支付标题：代扣</param>
        /// <param name="amount">金额</param>
        /// <param name="trans_account_out">转出支付宝账号</param>
        /// <param name="trans_account_in">转入支付宝账号</param>
        /// <param name="royalty_parameters">分账明细</param>
        /// <returns></returns>
        public string GetPayCAE(string out_order_no, string subject,
            string amount, string trans_account_out, string trans_account_in, string royalty_parameters)
        {
            string strValue = "";
            try
            {
                ////////////////////////////////////////////请求参数////////////////////////////////////////////
                //服务器异步通知页面路径
                //string notify_url = "http://www.xxx.com/cae_charge_agent-CSHARP-UTF-8/notify_url.aspx";
                //需http://格式的完整路径，不允许加?id=123这类自定义参数
                //string out_order_no = "";//商户订单号,商户网站唯一订单号(商户自定义)
                //string amount = ""; //金额,代扣订单金额
                //string subject = "";//支付宝标题,订单名称摘要
                //string trans_account_out = "";转出支付宝账号,转出的支付宝人民币资金账号（user_id+0156）【该字段还可传递支付宝登录账户（邮箱或手机号）】
                //string trans_account_in = "";//转入支付宝账号,转入的支付宝人民币资金账号（user_id+0156）【该字段还可传递支付宝登录账户（邮箱或手机号）】

                string charge_type = "trade"; //代扣模式,机票代扣时走的是交易模式（固定值为：trade）
                string type_code = _partner + "1000310004"; //代理业务编号,唯一标识商户和支付宝签署的某项业务码(机票CAE代扣的type_code为pid+1000310004)
                string _input_charset = "utf-8";
                ////////////////////////////////////////////////////////////////////////////////////////////////
                //把请求参数打包成数组
                SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                sParaTemp.Add("partner", _partner);
                sParaTemp.Add("_input_charset", _input_charset);
                sParaTemp.Add("service", "cae_charge_agent");
                sParaTemp.Add("notify_url", _notifyurl);
                sParaTemp.Add("out_order_no", out_order_no);
                sParaTemp.Add("amount", amount);
                sParaTemp.Add("subject", subject);
                sParaTemp.Add("trans_account_out", trans_account_out);
                sParaTemp.Add("trans_account_in", trans_account_in);
                sParaTemp.Add("charge_type", charge_type);
                sParaTemp.Add("type_code", type_code);

                if (!string.IsNullOrEmpty(royalty_parameters))
                {
                    sParaTemp.Add("royalty_type", "10"); // 固定值，分账时使用
                    sParaTemp.Add("royalty_parameters", royalty_parameters);
                }

                //建立请求
                string sHtmlText = PbProject.Logic.Pay.batch_trans.Submit.BuildRequest(sParaTemp);

                

                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml(sHtmlText);
                    strValue = xmlDoc.SelectSingleNode("/alipay").InnerText;
                    //Response.Write();

                    #region 记录日志
                    
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in sParaTemp)
                    {
                        sb.Append(item.Key+"="+item.Value+",");
                    }
                    sb.Append("sHtmlText=" + sHtmlText);

                    OnErrorNew("GetPayCAE：" + sb.ToString());

                    #endregion
                }
                catch (Exception exp)
                {
                    //Response.Write();
                }
            }
            catch (Exception)
            {

            }

            return strValue;
        }
        #endregion
    }
}
