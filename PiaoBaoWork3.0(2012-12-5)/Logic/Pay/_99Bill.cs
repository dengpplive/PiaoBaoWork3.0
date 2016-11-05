using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;
using System.Web.UI;
using System.Xml;
using System.Configuration;
using PbProject.Model.PayParam;

namespace PbProject.Logic.Pay
{
    /// <summary>
    /// 快钱
    /// </summary>
    public class _99Bill
    {
        #region 参数定义


        /// <summary>
        /// Pos机pid
        /// </summary>
        public string _PosId = "999530045110017";
        /// <summary>
        /// Pos账号
        /// </summary>
        public string _PosEmail = "519333620@qq.com";
        /// <summary>
        /// Pos商户号(自动支付)
        /// </summary>
        public string _PosAutoId = "10021368641";
        /// <summary>
        /// Pos自动支付密钥
        /// </summary>
        public string _PosAutoKey = "74HGGJH2AFXH6ZTF";
        /// <summary>
        /// 主收款账号
        /// </summary>
        public string LinkEmail = "1076090377@qq.com";
        /// <summary>
        /// 收款pid
        /// </summary>
        public string _ReceiveAutoId = "10021368644";
        /// <summary>
        /// 收款自动支付密钥
        /// </summary>
        public string _ReceiveAutokey = "Q5QM3JKHE83DR8UZ";



        /// <summary>
        /// 分润pid
        /// </summary>
        public string _merchantAcctId = "10022169944"; // 新分润pid
        /// <summary>
        /// 分润支付密钥(MD5加密),修改加密方式后 不需要支付密钥
        /// </summary>
        public string _key = "YGX547YU3QHWEC39";
        /// <summary>
        /// 分润退款密钥(MD5加密),修改加密方式后 不需要支付密钥
        /// </summary>
        public string _Refundkey = "BZM9X9ZCSJBF6ISC";
        /// <summary>
        /// 字符集 只能选择1、2、3，1代表UTF-8; 2代表GBK; 3代表gb2312
        /// </summary>
        public string _inputCharset = "1";
        /// <summary>
        /// 网关版本 
        /// </summary>
        public string _version = "v2.0";
        /// <summary>
        /// 只能选择1、2、3，1代表中文；2代表英文
        /// </summary>
        public string _language = "1";
        /// <summary>
        /// 签名类型 1为MD5 // 4.为新的加密方式
        /// </summary>
        public string _signType = "4";
        /// <summary>
        /// 提交地址
        /// </summary>
        public string _url = "";
        /// 接口费率
        /// </summary>
        public decimal _Rate = 0.001M;
        /// <summary>
        /// 供应商费率
        /// </summary>
        public decimal _SupplyRate = 0.001M;

        /// <summary>
        /// 同步地址
        /// </summary>
        public string _ReturnUrl = "";//同步地址
        /// <summary>
        /// 异步地址
        /// </summary>
        public string _NotifyUrl = "";//异步地址

        #endregion

        #region 公共方法

        public _99Bill()
        {
            _ReturnUrl = ConfigurationManager.AppSettings["_99BillReturnUrl"].ToString();
            _NotifyUrl = ConfigurationManager.AppSettings["_99BillNotifyUrl"].ToString();

            SessionContent sessionContent = HttpContext.Current.Session["user"] as SessionContent;

            if (sessionContent != null)
            {

            }
            else
            {

            }
        }

        public _99Bill(bool type)
        {
          
        }

        /// <summary>
        /// 构造参数字符串
        /// </summary>
        /// <param name="returnStr">源字符串</param>
        /// <param name="paramId">参数名</param>
        /// <param name="paramValue">参数值</param>
        /// <returns></returns>
        public String appendParam(String returnStr, String paramId, String paramValue)
        {
            if (returnStr != "")
            {
                if (paramValue != "")
                {
                    returnStr += "&" + paramId + "=" + paramValue;
                }
            }
            else
            {
                if (paramValue != "")
                {
                    returnStr = paramId + "=" + paramValue;
                }
            }
            return returnStr;
        }

        /// <summary>
        /// MD5加密 ，不用了 使用：证书加密（2012-01-10 :jh）（代扣使用，不是分账）
        /// </summary>
        /// <param name="dataStr">要加密的字符串</param>
        /// <param name="codeType">加密类型</param>
        /// <returns></returns>
        public string GetMD5(string dataStr, string codeType)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(codeType).GetBytes(dataStr));
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 快钱 新加密方式（证书加密） （2012-01-10 :jh） (分账使用)
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public string GetEncrypting(string strValue)
        {
            ///PKI加密
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strValue);
            string mapPath = HttpContext.Current.Server.MapPath("~/Pay/Key/tester-rsa.pfx");
            X509Certificate2 cert = new X509Certificate2(mapPath, "mypb51cbc", X509KeyStorageFlags.MachineKeySet);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);
            return System.Convert.ToBase64String(f.CreateSignature(result)).ToString();
        }

        /// <summary>
        /// GET方式提交到指定URL
        /// </summary>
        /// <param name="url">要提交的URL地址包含参数</param> 
        /// <returns>返回请求页面的HTML</returns>
        public string GetUrlData(string url)
        {
            WebClient webclient = new WebClient();
            byte[] pagedate = webclient.DownloadData(url);
            byte[] value = Encoding.Convert(Encoding.UTF8, Encoding.Default, pagedate, 0, pagedate.Length);
            return Encoding.Default.GetString(value, 0, value.Length);

        }

        /// <summary>
        /// 功能函数
        /// </summary>
        /// <param name="returnStr"></param>
        /// <param name="paramId"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private static string appendParamall(string returnStr, string paramId, string paramValue)
        {
            if (returnStr != "")
            {
                returnStr += "&" + paramId + "=" + paramValue;
            }
            else
            {
                returnStr = paramId + "=" + paramValue;
            }
            return returnStr;
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
        /// PostXML数据到服务器及获取返回的xml值
        /// </summary>
        public string HttpPost(string url, string data)
        {
            string postData = data;	//xml数据
            string Web = url;	//网关地址
            try
            {
                //将数据提交到快钱服务器
                WebRequest myWebRequest = WebRequest.Create(url);
                myWebRequest.Method = "POST";
                myWebRequest.ContentType = "application/x-www-form-urlencoded";
                Stream streamReq = myWebRequest.GetRequestStream();
                byte[] byteArray = Encoding.GetEncoding("utf-8").GetBytes(postData);
                streamReq.Write(byteArray, 0, byteArray.Length);
                streamReq.Close();

                //获取服务器返回的XML数据
                WebResponse myWebResponse = myWebRequest.GetResponse();
                StreamReader sr = new StreamReader(myWebResponse.GetResponseStream());
                string res = sr.ReadToEnd();
                sr.Close();
                return res;	//返回xml数据
            }
            catch (Exception e)
            {
                string sStr = e.Message.ToString();
                return sStr; //返回错误
            }
        }

        /// <summary>
        /// 自动支付(提交申请)
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="price">总金额</param>
        /// <param name="amount">主收款金额</param>
        /// <param name="detail">分润字符串</param>
        /// <param name="pname">商品名字</param>
        /// <param name="ext">备用字段</param>
        /// <param name="url">接受地址</param>
        public string AutoPay(string orderid, string price, string amount, string detail, string pname, string ext, string url)
        {
            string result = "";
            try
            {
                string submitURL = "https://www.99bill.com/msgateway/recvMsgatewayMerchantInfoAction.htm";
                //字符集.固定选择值。可为空 默认值为1 
                string inputCharset = "1";
                //接受支付结果的页面地址.与[bgUrl]不能同时为空。必须是绝对地址。 
                string pageUrl = url;
                //服务器接受支付结果的后台地址.与[pageUrl]不能同时为空。必须是绝对地址。 
                string bgUrl = url;
                //分账网关收款接口商户密钥（不能在页面显示） 
                //string key = _key;
                //网关版本.固定值 本代码版本号固定为v2.0 
                string version = "v2.0";
                //语言种类.固定选择值。 只能选择1、2、3 
                string language = "1";
                //签名类型.固定值 1代表MD5签名 , 4 pki加密类型
                string signType = "4";
                //主收款方联系方式类型.固定选择值  
                string payeeContactType = "1";
                //主收款方联系方式（快钱email帐号，可以为供应商，也可以为平台商）  
                string payeeContact = LinkEmail.Trim();
                //支付人姓名 
                string payerName = "";
                //支付人联系方式类型.固定选择值 
                string payerContactType = "1";
                //支付人联系方式（目前只有email，如果提供的不是email，则这个值为空） 
                string payerContact = _PosEmail;
                //商户订单号 
                string orderId = orderid;
                //订单金额 
                string orderAmount = price;
                //主收款方应收款  
                string payeeAmount = amount;
                //订单提交时间  
                string orderTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                //商品名称 
                string productName = "";
                //商品数量 
                string productNum = "1";
                //商品描述   
                string productDesc = "";
                //扩展字段1  
                string ext1 = ext;
                //扩展字段2 
                string ext2 = "";
                //支付方式.固定选择值 只能选择00、10、11、12、13、14
                ///00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）.14：B2B支付（网关支付页面只显示B2B支付，但需要向快钱申请开通才能使用）
                string payType = "12";
                //银行代码 
                string bankId = "";
                //快钱的合作伙伴的账户号(平台商ID)  
                string pid = _merchantAcctId;
                //分账数据  
                string sharingData = detail;
                //分账标志 固定值：1、0  1 代表支付成功立刻分账  
                string sharingPayFlag = "1";
                //提交方式
                ///00:代表前台提交；01：代表后台提交 (paytype=12才行) 若为空代表为00 
                string submitType = "01";

                //生成加密签名串
                ///请务必按照如下顺序和规则组成加密串！
                string signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "pageUrl", pageUrl);
                signMsgVal = appendParam(signMsgVal, "bgUrl", bgUrl);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "language", language);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "payeeContactType", payeeContactType);
                signMsgVal = appendParam(signMsgVal, "payeeContact", payeeContact);
                signMsgVal = appendParam(signMsgVal, "payerName", payerName);
                signMsgVal = appendParam(signMsgVal, "payerContactType", payerContactType);
                signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "orderAmount", orderAmount);
                signMsgVal = appendParam(signMsgVal, "payeeAmount", payeeAmount);
                signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
                signMsgVal = appendParam(signMsgVal, "productName", productName);
                signMsgVal = appendParam(signMsgVal, "productNum", productNum);
                signMsgVal = appendParam(signMsgVal, "productDesc", productDesc);
                signMsgVal = appendParam(signMsgVal, "ext1", ext1);
                signMsgVal = appendParam(signMsgVal, "ext2", ext2);
                signMsgVal = appendParam(signMsgVal, "payType", payType);
                signMsgVal = appendParam(signMsgVal, "bankId", bankId);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "sharingData", sharingData);
                signMsgVal = appendParam(signMsgVal, "sharingPayFlag", sharingPayFlag);
                signMsgVal = appendParam(signMsgVal, "submitType", submitType);
                //signMsgVal = appendParam(signMsgVal, "key", key);

                //inputCharset的取值应与服务器的编码方式或URL编码方式（因为传输是以GET方式提交的）相一致；
                //如果是utf-8编码，则下面应该这样用，GetMD5(signMsgVal, "utf-8").ToUpper()
                //如果是gb2312编码，则下面应该这样用，GetMD5(signMsgVal, "gb2312").ToUpper()

                //string signMsg = GetMD5(signMsgVal, "utf-8").ToUpper();//签名后的数据   MD5

                string signMsg = GetEncrypting(signMsgVal); //块钱新加密
                signMsg = HttpContext.Current.Server.UrlEncode(signMsg);

                string urlMsgVal = "";
                urlMsgVal = appendParamall(urlMsgVal, "inputCharset", inputCharset);
                urlMsgVal = appendParamall(urlMsgVal, "pageUrl", pageUrl);
                urlMsgVal = appendParamall(urlMsgVal, "bgUrl", bgUrl);
                urlMsgVal = appendParamall(urlMsgVal, "version", version);
                urlMsgVal = appendParamall(urlMsgVal, "language", language);
                urlMsgVal = appendParamall(urlMsgVal, "signType", signType);
                urlMsgVal = appendParamall(urlMsgVal, "payeeContactType", payeeContactType);
                urlMsgVal = appendParamall(urlMsgVal, "payeeContact", payeeContact);
                urlMsgVal = appendParamall(urlMsgVal, "payerName", payerName);
                urlMsgVal = appendParamall(urlMsgVal, "payerContactType", payerContactType);
                urlMsgVal = appendParamall(urlMsgVal, "payerContact", payerContact);
                urlMsgVal = appendParamall(urlMsgVal, "orderId", orderId);
                urlMsgVal = appendParamall(urlMsgVal, "orderAmount", orderAmount);
                urlMsgVal = appendParamall(urlMsgVal, "payeeAmount", payeeAmount);
                urlMsgVal = appendParamall(urlMsgVal, "orderTime", orderTime);
                urlMsgVal = appendParamall(urlMsgVal, "productName", productName);
                urlMsgVal = appendParamall(urlMsgVal, "productNum", productNum);
                urlMsgVal = appendParamall(urlMsgVal, "productDesc", productDesc);
                urlMsgVal = appendParamall(urlMsgVal, "ext1", ext1);
                urlMsgVal = appendParamall(urlMsgVal, "ext2", ext2);
                urlMsgVal = appendParamall(urlMsgVal, "payType", payType);
                urlMsgVal = appendParamall(urlMsgVal, "bankId", bankId);
                urlMsgVal = appendParamall(urlMsgVal, "pid", pid);
                urlMsgVal = appendParamall(urlMsgVal, "sharingData", sharingData);
                urlMsgVal = appendParamall(urlMsgVal, "sharingPayFlag", sharingPayFlag);
                urlMsgVal = appendParamall(urlMsgVal, "submitType", submitType);
                urlMsgVal = appendParamall(urlMsgVal, "signMsg", signMsg);

                result = GetUrlData(submitURL + "?" + urlMsgVal);
                result = result.Replace("\r\n", "");

                //加载XML数据
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);

                //结果
                string payresult = xmlDoc.SelectSingleNode("//requestResult").InnerText.ToString();

                if (payresult == "10")
                {
                    result = DeductPay(orderid, price, "CGB");
                }
                else
                {
                    result = "";
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 代扣支付
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="price">总金额</param>
        /// <param name="payname">付款人名字</param> 
        /// <returns></returns>
        public string DeductPay(string orderid, string price, string payname)
        {
            string getDdata = "";
            string result = "";
            string url = "https://www.99bill.com/gateway/recvBackendPayInfoAction.htm";
            try
            {
                //自动支付接口商户密钥（不能在页面显示）
                string key = _PosAutoKey;
                //字符集.固定选择值。可为空。 只能选择1、2、3. 
                string inputCharset = "1";
                //网关版本.固定值 
                string version = "v1.0";
                //签名类型.固定值 
                string signType = "1";  // 1 md5
                //提交人商户编号 
                string memberCode = _PosAutoId;
                //支付人姓名 
                string payerName = payname;
                //支付人联系方式类型.固定选择值 
                string payerContactType = "1";
                //支付人联系方式 
                string payerContact = _PosEmail;
                //支付渠道 
                string payChannel = "1";
                //支付方式 
                string payType = "12";
                //付款人编号(扣款账户，会员编号加01) 数字串  若支付方式为12则为付款人在快钱的人民币账号。
                string payerNo = _PosAutoId + "01";
                //收款商户编号 
                string merchantAcctId = _merchantAcctId + "02";
                //商户订单号(原交易订单号) 
                string orderId = orderid;
                //请求编号 
                string requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
                //付款金额 
                string payAmount = price;
                //支付请求时间 
                string payTime = DateTime.Now.ToString("yyyyMMddHHmmss");

                //生成加密签名串 
                string signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "memberCode", memberCode);
                signMsgVal = appendParam(signMsgVal, "payerName", payerName);
                signMsgVal = appendParam(signMsgVal, "payerContactType", payerContactType);
                signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
                signMsgVal = appendParam(signMsgVal, "payChannel", payChannel);
                signMsgVal = appendParam(signMsgVal, "payType", payType);
                signMsgVal = appendParam(signMsgVal, "payerNo", payerNo);
                signMsgVal = appendParam(signMsgVal, "merchantAcctId", merchantAcctId);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "requestId", requestId);
                signMsgVal = appendParam(signMsgVal, "payAmount", payAmount);
                signMsgVal = appendParam(signMsgVal, "payTime", payTime);
                signMsgVal = appendParam(signMsgVal, "key", key);

                string signMsg = GetMD5(signMsgVal, "utf-8").ToUpper();//签名后的数据


                //以下为将提交的参数组成字符串
                string urlMsgVal = "";
                urlMsgVal = appendParamall(urlMsgVal, "inputCharset", inputCharset);
                urlMsgVal = appendParamall(urlMsgVal, "version", version);
                urlMsgVal = appendParamall(urlMsgVal, "signType", signType);
                urlMsgVal = appendParamall(urlMsgVal, "memberCode", memberCode);
                urlMsgVal = appendParamall(urlMsgVal, "payerName", payerName);
                urlMsgVal = appendParamall(urlMsgVal, "payerContactType", payerContactType);
                urlMsgVal = appendParamall(urlMsgVal, "payerContact", payerContact);
                urlMsgVal = appendParamall(urlMsgVal, "payChannel", payChannel);
                urlMsgVal = appendParamall(urlMsgVal, "payType", payType);
                urlMsgVal = appendParamall(urlMsgVal, "payerNo", payerNo);
                urlMsgVal = appendParamall(urlMsgVal, "merchantAcctId", merchantAcctId);
                urlMsgVal = appendParamall(urlMsgVal, "orderId", orderId);
                urlMsgVal = appendParamall(urlMsgVal, "requestId", requestId);
                urlMsgVal = appendParamall(urlMsgVal, "payAmount", payAmount);
                urlMsgVal = appendParamall(urlMsgVal, "payTime", payTime);
                urlMsgVal = appendParamall(urlMsgVal, "signMsg", signMsg);

                getDdata = urlMsgVal;//以POST方式提交的URL数据
            }
            catch (Exception ex)
            {

            }

            try  //获取返回的值并处理数据
            {
                string res = HttpPost(url, getDdata);//提交数据到网关，并获取网关返回的xml数据
                res = res.Replace("\r\n", "");

                //加载XML数据
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(res);

                //以下为获取XML数据相应节点的值开始 
                //版本号
                string version1 = xmlDoc.SelectSingleNode("//version").InnerText.ToString();
                //签名类型
                string signType1 = xmlDoc.SelectSingleNode("//signType").InnerText.ToString();
                //提交人商户编号
                string memberCode1 = xmlDoc.SelectSingleNode("//memberCode").InnerText.ToString();
                //支付人姓名
                string payerName1 = xmlDoc.SelectSingleNode("//payerName").InnerText.ToString();
                //支付人联系方式类型
                string payerContactType1 = xmlDoc.SelectSingleNode("//payerContactType").InnerText.ToString();
                //支付人联系方式
                string payerContact1 = xmlDoc.SelectSingleNode("//payerContact").InnerText.ToString();
                //支付方式
                string payType1 = xmlDoc.SelectSingleNode("//payType").InnerText.ToString();
                //付款人会员编号
                string payerNo1 = xmlDoc.SelectSingleNode("//payerNo").InnerText.ToString();
                //商户编号
                string merchantAcctId1 = xmlDoc.SelectSingleNode("//merchantAcctId").InnerText.ToString();
                //商家订单号
                string orderId1 = xmlDoc.SelectSingleNode("//orderId").InnerText.ToString();
                //付款交易号
                string dealId1 = xmlDoc.SelectSingleNode("//dealId").InnerText.ToString();
                //付款金额
                string payAmount1 = xmlDoc.SelectSingleNode("//payAmount").InnerText.ToString();
                //支付请求时间
                string payTime1 = xmlDoc.SelectSingleNode("//payTime").InnerText.ToString();
                //支付结果
                string payResult1 = xmlDoc.SelectSingleNode("//payResult").InnerText.ToString();
                //错误代码
                string errCode1 = xmlDoc.SelectSingleNode("//errCode").InnerText.ToString();
                //签名字符串
                string signMsg1 = xmlDoc.SelectSingleNode("//signMsg").InnerText.ToString();

                //将返回的数据组成签名串 
                string key1 = _PosAutoKey;
                string signMsgVal1 = "";
                signMsgVal1 = appendParam(signMsgVal1, "version", version1);
                signMsgVal1 = appendParam(signMsgVal1, "signType", signType1);
                signMsgVal1 = appendParam(signMsgVal1, "memberCode", memberCode1);
                signMsgVal1 = appendParam(signMsgVal1, "payerName", payerName1);
                signMsgVal1 = appendParam(signMsgVal1, "payerContactType", payerContactType1);
                signMsgVal1 = appendParam(signMsgVal1, "payerContact", payerContact1);
                signMsgVal1 = appendParam(signMsgVal1, "payType", payType1);
                signMsgVal1 = appendParam(signMsgVal1, "payerNo", payerNo1);
                signMsgVal1 = appendParam(signMsgVal1, "merchantAcctId", merchantAcctId1);
                signMsgVal1 = appendParam(signMsgVal1, "orderId", orderId1);
                signMsgVal1 = appendParam(signMsgVal1, "dealId", dealId1);
                signMsgVal1 = appendParam(signMsgVal1, "payAmount", payAmount1);
                signMsgVal1 = appendParam(signMsgVal1, "payTime", payTime1);
                signMsgVal1 = appendParam(signMsgVal1, "payResult", payResult1);
                signMsgVal1 = appendParam(signMsgVal1, "errCode", errCode1);
                signMsgVal1 = appendParam(signMsgVal1, "key", key1);

                string signRtnMsg = GetMD5(signMsgVal1, "utf-8").ToUpper();//将返回的数据签名

                //签名验证
                if (signMsg1 == signRtnMsg)
                {
                    if (payResult1 == "10")
                    {
                        payResult1 = "Success";
                    }
                    else
                    {
                        payResult1 = "Failure";

                    }
                }
                else
                {
                    if (payResult1 == "10")
                    {
                        payResult1 = "Success2";

                    }
                    else
                    {
                        payResult1 = "Failure2";
                    }
                }
                result = payResult1;
            }
            catch (Exception exx)
            {

            }
            return result;
        }

        /// <summary>
        /// 直接支付
        /// </summary>
        /// <param name="orderid">订单编号</param>
        /// <param name="price">订单金额</param>
        /// <param name="pname">商品名称</param>
        /// <param name="ext">自定义字段</param> 
        /// <returns></returns>
        public string Pay(string orderid, string price, string pname, string ext)
        {
            try
            {
                string url = "https://www.99bill.com/gateway/recvMerchantInfoAction.htm";
                //string url = "https://sandbox.99bill.com/gateway/recvMerchantInfoAction.htm"; 

                string merchantAcctId = _merchantAcctId;
                string key = _key;
                string inputCharset = _inputCharset;
                string pageUrl = _ReturnUrl;//同步地址
                string bgUrl = _NotifyUrl;//异步
                string version = _version;
                string language = _language;

                //签名类型 1为MD5 // 4.为新的加密方式
                string signType = "1";  //
                //支付人姓名 
                string payerName = "";
                //支付人联系方式类型.默认1
                string payerContactType = "1";
                //支付人联系方式 
                string payerContact = "";
                //商户订单号 
                string orderId = orderid;
                //订单金额 
                string orderAmount = price;
                //订单提交时间 
                string orderTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                //商品名称 
                string productName = pname;
                //商品数量 默认1
                string productNum = "1";
                //商品代码 
                string productId = "";
                //商品描述
                string productDesc = "";
                //扩展字段1 
                string ext1 = ext;
                //扩展字段2 
                string ext2 = "";
                //支付方式.固定选择值
                //只能选择00、10、11、12、13、14
                //00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）
                string payType = "00";
                //同一订单禁止重复提交标志
                //固定选择值： 1、0
                //1代表同一订单号只允许提交1次；0表示同一订单号在没有支付成功的前提下可重复提交多次。默认为0建议实物购物车结算类商户采用0；虚拟产品类商户采用1
                string redoFlag = "1";
                //快钱的合作伙伴的账户号
                ///如未和快钱签订代理合作协议，不需要填写本参数
                string pid = "";

                //加密
                string signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "pageUrl", pageUrl);
                signMsgVal = appendParam(signMsgVal, "bgUrl", bgUrl);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "language", language);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "merchantAcctId", merchantAcctId);
                signMsgVal = appendParam(signMsgVal, "payerName", payerName);
                signMsgVal = appendParam(signMsgVal, "payerContactType", payerContactType);
                signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "orderAmount", orderAmount);
                signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
                signMsgVal = appendParam(signMsgVal, "productName", productName);
                signMsgVal = appendParam(signMsgVal, "productNum", productNum);
                signMsgVal = appendParam(signMsgVal, "productId", productId);
                signMsgVal = appendParam(signMsgVal, "productDesc", productDesc);
                signMsgVal = appendParam(signMsgVal, "ext1", ext1);
                signMsgVal = appendParam(signMsgVal, "ext2", ext2);
                signMsgVal = appendParam(signMsgVal, "payType", payType);
                signMsgVal = appendParam(signMsgVal, "redoFlag", redoFlag);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "key", key);
                //加密后的字符串
                string signMsg = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(signMsgVal, "MD5").ToUpper();

                //string signMsg = GetEncrypting(signMsgVal); //块钱 新加密

                StringBuilder sb = new StringBuilder();
                sb.Append("<form name='kqPay'  id='kqPay'  method='post' action='" + url + "'>");
                sb.Append("<input type='hidden' name='inputCharset' value='" + inputCharset + "'/>");
                sb.Append("<input type='hidden' name='pageUrl' value='" + pageUrl + "'/>");
                sb.Append("<input type='hidden' name='bgUrl' value='" + bgUrl + "'/>");
                sb.Append("<input type='hidden' name='version' value='" + version + "'/>");
                sb.Append("<input type='hidden' name='language' value='" + language + "'/>");
                sb.Append("<input type='hidden' name='signType' value='" + signType + "'/>");
                sb.Append("<input type='hidden' name='signMsg' value='" + signMsg + "'/>");
                sb.Append("<input type='hidden' name='merchantAcctId' value='" + merchantAcctId + "'/>");
                sb.Append("<input type='hidden' name='payerName' value='" + payerName + "'/>");
                sb.Append("<input type='hidden' name='payerContactType' value='" + payerContactType + "'/>");
                sb.Append("<input type='hidden' name='payerContact' value='" + payerContact + "'/>");
                sb.Append("<input type='hidden' name='orderId' value='" + orderId + "'/>");
                sb.Append("<input type='hidden' name='orderAmount' value='" + orderAmount + "'/>");
                sb.Append("<input type='hidden' name='orderTime' value='" + orderTime + "'/>");
                sb.Append("<input type='hidden' name='productName' value='" + productName + "'/>");
                sb.Append("<input type='hidden' name='productNum' value='" + productNum + "'/>");
                sb.Append("<input type='hidden' name='productId' value='" + productId + "'/>");
                sb.Append("<input type='hidden' name='productDesc' value='" + productDesc + "'/>");
                sb.Append("<input type='hidden' name='ext1' value='" + ext1 + "'/>");
                sb.Append("<input type='hidden' name='ext2' value='" + ext2 + "'/>");
                sb.Append("<input type='hidden' name='payType' value='" + payType + "'/>");
                sb.Append("<input type='hidden' name='redoFlag' value='" + redoFlag + "'/>");
                sb.Append("<input type='hidden' name='pid' value='" + pid + "'/>");
                sb.Append("</form>");
                sb.Append("<script>");
                sb.Append(" document.getElementById(\"kqPay\").submit();");
                sb.Append("</script>");

                return sb.ToString();
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// 支付及分润
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public string GetPay(Model.PayParam._99BillParam billParam)
        {
            string strVal = "";
            try
            {
                #region 参数赋值

                string orderid = billParam.Orderid;//0.订单编号
                string price = billParam.Price;//1.订单金额，单位“分”
                string money = billParam.Money;//2.供应收款金额，单位“分”
                string pname = billParam.Pname;//3.商品名称
                string ext = billParam.Ext;//4.自定义字段
                string detail = billParam.Detail;//5.分润数据集
                string ptype = billParam.Ptype;//6.分润类别，1 立刻分润 0 异步分润
                string payid = billParam.Payid;//7.付款帐户
                string paytype = billParam.Paytype;//8.  00：组合支付，10：银行卡支付
                string bankcode = billParam.Bankcode;//9: 银行代码

                string url = "https://www.99bill.com/msgateway/recvMsgatewayMerchantInfoAction.htm";
                //string url = "https://sandbox.99bill.com/msgateway/recvMsgatewayMerchantInfoAction.htm";

                //string key = _key;
                string inputCharset = _inputCharset;
                string pageUrl = _ReturnUrl;//同步地址
                string bgUrl = _NotifyUrl;//异步
                string version = _version;
                string language = _language;
                string signType = _signType;

                //我们的联系方式类别
                string payeeContactType = "1";
                //我们的联系方式
                string payeeContact = LinkEmail.Trim();

                //支付人姓名 
                string payerName = "";
                //支付人联系方式类型.默认1
                string payerContactType = "1";
                if (payid == "")
                {
                    payerContactType = "";
                }
                //支付人联系方式 
                string payerContact = payid;
                //商户订单号 
                string orderId = orderid;
                //订单金额 
                string orderAmount = price;
                //我们的收款金额
                string payeeAmount = money;
                //订单提交时间 
                string orderTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                //商品名称 
                string productName = pname;
                //商品数量 默认1
                string productNum = "1";
                //商品描述
                string productDesc = "";
                //扩展字段1 
                string ext1 = ext;
                //扩展字段2 
                string ext2 = "";
                //银行代码
                string bankId = bankcode;
                //分润数据集
                string sharingData = detail;
                //分润类别，1 代表支付成功立刻分账 0 代表异步分账
                string sharingPayFlag = ptype;
                //支付方式.固定选择值
                //只能选择00、10、11、12、13、14
                //00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）
                string payType = paytype;
                //商户号
                string pid = _merchantAcctId;



                #endregion

                #region 加密

                //加密
                string signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "pageUrl", pageUrl);
                signMsgVal = appendParam(signMsgVal, "bgUrl", bgUrl);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "language", language);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "payeeContactType", payeeContactType);
                signMsgVal = appendParam(signMsgVal, "payeeContact", payeeContact);
                signMsgVal = appendParam(signMsgVal, "payerName", payerName);
                signMsgVal = appendParam(signMsgVal, "payerContactType", payerContactType);
                signMsgVal = appendParam(signMsgVal, "payerContact", payerContact);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "orderAmount", orderAmount);
                signMsgVal = appendParam(signMsgVal, "payeeAmount", payeeAmount);
                signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
                signMsgVal = appendParam(signMsgVal, "productName", productName);
                signMsgVal = appendParam(signMsgVal, "productNum", productNum);
                signMsgVal = appendParam(signMsgVal, "productDesc", productDesc);
                signMsgVal = appendParam(signMsgVal, "ext1", ext1);
                signMsgVal = appendParam(signMsgVal, "ext2", ext2);
                signMsgVal = appendParam(signMsgVal, "payType", payType);
                signMsgVal = appendParam(signMsgVal, "bankId", bankId);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "sharingData", sharingData);
                signMsgVal = appendParam(signMsgVal, "sharingPayFlag", sharingPayFlag);
                //signMsgVal = appendParam(signMsgVal, "key", key);
                //加密后的字符串
                //string signMsg = GetMD5(signMsgVal, "UTF-8").ToUpper();

                string signMsg = GetEncrypting(signMsgVal); //块钱 新加密

                #endregion 加密

                #region 组合表单数据
                StringBuilder sb = new StringBuilder();

                sb.Append("<form name='kqPay'  id='kqPay'  method='post' action='" + url + "'>");
                sb.Append("<input type='hidden' name='inputCharset' value='" + inputCharset + "'/>");
                sb.Append("<input type='hidden' name='pageUrl' value='" + pageUrl + "'/>");
                sb.Append("<input type='hidden' name='bgUrl' value='" + bgUrl + "'/>");
                sb.Append("<input type='hidden' name='version' value='" + version + "'/>");
                sb.Append("<input type='hidden' name='language' value='" + language + "'/>");
                sb.Append("<input type='hidden' name='signType' value='" + signType + "'/>");
                sb.Append("<input type='hidden' name='payeeContactType' value='" + payeeContactType + "'/>");
                sb.Append("<input type='hidden' name='payeeContact' value='" + payeeContact + "'/>");
                sb.Append("<input type='hidden' name='payerName' value='" + payerName + "'/>");
                sb.Append("<input type='hidden' name='payerContactType' value='" + payerContactType + "'/>");
                sb.Append("<input type='hidden' name='payerContact' value='" + payerContact + "'/>");
                sb.Append("<input type='hidden' name='orderId' value='" + orderId + "'/>");
                sb.Append("<input type='hidden' name='orderAmount' value='" + orderAmount + "'/>");
                sb.Append("<input type='hidden' name='payeeAmount' value='" + payeeAmount + "'/>");
                sb.Append("<input type='hidden' name='orderTime' value='" + orderTime + "'/>");
                sb.Append("<input type='hidden' name='productName' value='" + productName + "'/>");
                sb.Append("<input type='hidden' name='productNum' value='" + productNum + "'/>");
                sb.Append("<input type='hidden' name='productDesc' value='" + productDesc + "'/>");
                sb.Append("<input type='hidden' name='ext1' value='" + ext1 + "'/>");
                sb.Append("<input type='hidden' name='ext2' value='" + ext2 + "'/>");
                sb.Append("<input type='hidden' name='payType' value='" + payType + "'/>");
                sb.Append("<input type='hidden' name='bankId' value='" + bankId + "'/>");
                sb.Append("<input type='hidden' name='pid' value='" + pid + "'/>");
                sb.Append("<input type='hidden' name='sharingData' value='" + sharingData + "'/>");
                sb.Append("<input type='hidden' name='sharingPayFlag' value='" + sharingPayFlag + "'/>");
                sb.Append("<input type='hidden' name='signMsg' value='" + signMsg + "'/>");
                sb.Append("</form>");
                sb.Append("<script>");
                sb.Append(" document.getElementById(\"kqPay\").submit();");
                sb.Append("</script>");
                strVal = sb.ToString();
                #endregion
            }
            catch (Exception ex)
            {

            }
            return strVal;
        }

        /// <summary>
        /// 异步分润
        /// </summary>
        /// <param name="orderid">订单编号</param>
        /// <param name="receiveid">要分润的帐户</param> 
        /// <returns>返回成功或者失败</returns>
        public bool AgainPay(string orderid, string receiveid)
        {
            try
            {
                string url = "http://www.99bill.com/msgateway/recvMerchantSharingAction.htm";
                //string url = "http://sandbox.99bill.com/msgateway/recvMerchantSharingAction.htm";

                string inputCharset = _inputCharset;
                string version = _version;
                string signType = "4";
                string orderId = orderid;
                string sharingTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                string pid = _merchantAcctId;
                //分润方收款帐户
                string sharingInfo = receiveid;
                String signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "sharingTime", sharingTime);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "sharingInfo", sharingInfo);

                //string signMsg = GetMD5(signMsgVal, "UTF-8").ToUpper();
                string signMsg = GetEncrypting(signMsgVal); //块钱 新加密

                signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "sharingTime", sharingTime);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "sharingInfo", sharingInfo);
                signMsgVal = appendParam(signMsgVal, "signMsg", signMsg);

                //返回结果
                string result = GetUrlData(url + "?" + signMsgVal);
                result = GetString(result, "<payResult>", "</payResult>");
                if (result == "10")
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="orderid">订单编号(原订单号)</param>
        /// <param name="price">订单总价</param>
        /// <param name="ext">自定义字段</param>
        /// <param name="orderidNew">订单编号（退废订单编号）</param>
        /// <param name="orderidNo">退款流水号</param>
        /// <param name="detail">退分润数据集</param>
        /// <returns></returns>
        public string Refund(string[] detile)
        {
            string result = "";
            try
            {
                string orderid = detile[0];
                string price = detile[1];
                string ext = detile[2];
                string detail = detile[3];
                string orderidNew = detile[4];
                string orderidNo = detile[5];

                string url = "https://www.99bill.com/msgateway/recvMerchantRefundAction.htm";
                //string url = "https://sandbox.99bill.com/msgateway/recvMerchantRefundAction.htm";

                string inputCharset = _inputCharset;
                string version = _version;
                string signType = _signType;
                string orderId = orderid;
                string pid = _merchantAcctId;
                //退款流水号
                //string seqId = DateTime.Now.ToString("yyyyMMddHHmmss");
                string str = orderidNo + orderidNew;
                string seqId = str;

                //退款总金额
                string returnAllAmount = price;
                string returnTime = orderidNo; //DateTime.Now.ToString("yyyyMMddHHmmss");
                string ext1 = ext;
                string ext2 = "";
                //退款明细
                string returnDetail = detail;
                //string returnDetail = "1^805692109@qq.com^80000^退款";
                //退款在分润明细
                string returnSharingDetail = "";
                //指定快钱账户
                string assignAcct = "";
                //退款标志 0 代表同步，1 代表异步
                string refundFlag = "";
                //string key = _Refundkey;
                string signMsgVal = "";

                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "seqId", seqId);
                signMsgVal = appendParam(signMsgVal, "returnAllAmount", returnAllAmount);
                signMsgVal = appendParam(signMsgVal, "returnTime", returnTime);
                signMsgVal = appendParam(signMsgVal, "ext1", ext1);
                signMsgVal = appendParam(signMsgVal, "ext2", ext2);
                signMsgVal = appendParam(signMsgVal, "returnDetail", returnDetail);
                signMsgVal = appendParam(signMsgVal, "returnSharingDetail", returnSharingDetail);
                signMsgVal = appendParam(signMsgVal, "assignAcct", assignAcct);
                signMsgVal = appendParam(signMsgVal, "refundFlag", refundFlag);
                //signMsgVal = appendParam(signMsgVal, "key", key);
                //string signMsg = GetMD5(signMsgVal, "UTF-8").ToUpper();

                string signMsg = GetEncrypting(signMsgVal); //块钱 新加密
                signMsg = HttpContext.Current.Server.UrlEncode(signMsg);

                string urlVal = "";
                urlVal += "inputCharset=" + inputCharset;
                urlVal += "&version=" + version;
                urlVal += "&signType=" + signType;
                urlVal += "&orderId=" + orderId;
                urlVal += "&pid=" + pid;
                urlVal += "&seqId=" + seqId;
                urlVal += "&returnAllAmount=" + returnAllAmount;
                urlVal += "&returnTime=" + returnTime;
                urlVal += "&ext1=" + ext1;
                urlVal += "&ext2=" + ext2;
                urlVal += "&returnDetail=" + returnDetail;
                urlVal += "&returnSharingDetail=" + returnSharingDetail;
                urlVal += "&assignAcct=" + assignAcct;
                urlVal += "&refundFlag=" + refundFlag;
                urlVal += "&signMsg=" + signMsg;

                result = GetUrlData(url + "?" + urlVal);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 验证退款
        /// </summary>
        /// <param name="value">接收的退款字符串</param>
        /// <returns></returns>
        public bool IsRefund(string value)
        {
            try
            {
                XElement root = XElement.Parse(value);
                string orderId = "";
                string pid = "";
                string processId = "";
                string seqId = "";
                string returnAllAmount = "";
                string returnFee = "";
                string dealId = "";
                string returnSupAmount = "";
                string result = "";
                string errCode = "";
                string returnData = "";
                string signMsg = "";

                if (root.Element("orderId") != null)
                {
                    orderId = root.Element("orderId").Value;
                }
                if (root.Element("pid") != null)
                {
                    pid = root.Element("pid").Value;
                }
                if (root.Element("processId") != null)
                {
                    processId = root.Element("processId").Value;
                }
                if (root.Element("seqId") != null)
                {
                    seqId = root.Element("seqId").Value;
                }
                if (root.Element("returnAllAmount") != null)
                {
                    returnAllAmount = root.Element("returnAllAmount").Value;
                }
                if (root.Element("returnFee") != null)
                {
                    returnFee = root.Element("returnFee").Value;
                }
                if (root.Element("dealId") != null)
                {
                    dealId = root.Element("dealId").Value;
                }
                if (root.Element("returnSupAmount") != null)
                {
                    returnSupAmount = root.Element("returnSupAmount").Value;
                }
                if (root.Element("result") != null)
                {
                    result = root.Element("result").Value;
                }
                if (root.Element("errCode") != null)
                {
                    errCode = root.Element("errCode").Value;
                }
                if (root.Element("returnData") != null)
                {
                    string returnContactType = "";
                    string returnContact = "";
                    string returnAmount = "";
                    string returnDesc = "";
                    string microErrorCode = "";
                    foreach (XElement xe in root.Elements("returnData"))
                    {
                        foreach (XElement xe2 in xe.Elements("returnDetail"))
                        {
                            if (xe2.Element("returnContactType") != null)
                            {
                                returnContactType = xe2.Element("returnContactType").Value;
                            }
                            if (xe2.Element("returnContact") != null)
                            {
                                returnContact = xe2.Element("returnContact").Value;
                            }
                            if (xe2.Element("returnAmount") != null)
                            {
                                returnAmount = xe2.Element("returnAmount").Value;
                            }
                            if (xe2.Element("returnDesc") != null)
                            {
                                returnDesc = xe2.Element("returnDesc").Value;
                            }
                            if (xe2.Element("microErrorCode") != null)
                            {
                                microErrorCode = xe2.Element("microErrorCode").Value;
                            }
                            returnData += returnContactType + "^" + returnContact + "^" + returnAmount + "^" + returnDesc + "^" + microErrorCode + "|";
                        }
                    }
                    if (returnData.Length > 0)
                    {
                        //去尾部,
                        returnData = returnData.Substring(0, returnData.Length - 1);
                    }
                }
                if (root.Element("signMsg") != null)
                {
                    signMsg = root.Element("signMsg").Value;
                }

                //string key = _Refundkey;

                string signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "processId", processId);
                signMsgVal = appendParam(signMsgVal, "seqId", seqId);
                signMsgVal = appendParam(signMsgVal, "returnAllAmount", returnAllAmount);
                signMsgVal = appendParam(signMsgVal, "returnFee", returnFee);
                signMsgVal = appendParam(signMsgVal, "returnSupAmount", returnSupAmount);
                signMsgVal = appendParam(signMsgVal, "result", result);
                signMsgVal = appendParam(signMsgVal, "errCode", errCode);
                signMsgVal = appendParam(signMsgVal, "returnData", returnData);

                //signMsgVal = appendParam(signMsgVal, "key", key);
                //string _signMsg = GetMD5(signMsgVal, "UTF-8").ToUpper();

                #region 验证方法二 接收解密验证

                //signMsgVal = signMsgVal + "&";
                ///pki加密方式 使用的是快钱的cer证书 
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signMsgVal);
                //byte[] SignatureByte = Convert.FromBase64String(signMsg);
                byte[] SignatureByte = Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(signMsg));

                X509Certificate2 cert = new X509Certificate2(HttpContext.Current.Server.MapPath("~/Pay/Key/99bill.cert.rsa.20140728.cer"), "");
                RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
                rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
                RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
                byte[] results;
                f.SetHashAlgorithm("SHA1");
                SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                results = sha.ComputeHash(bytes);

                if (f.VerifySignature(results, SignatureByte))
                {
                    //成功
                    if (result == "10")
                    {
                        return true;
                    }
                    else
                    {

                    }
                }
                else
                {

                }

                #endregion
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// 退款接口查询 1 分账退款查询
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="orderIndexNo">退款订单流水号</param>
        /// <returns></returns>
        public string GetRefund(string orderid, string orderIndexNo)
        {
            try
            {
                string url = "https://www.99bill.com/msgateway/recvMerchantRefundQueryAction.htm";
                //string url = "https://sandbox.99bill.com/msgateway/recvMerchantRefundQueryAction.htm";

                string inputCharset = _inputCharset;
                string version = _version;
                //string signType = "1";
                string signType = "4";
                string queryType = "0";  //长度 1，固定值为0，单笔查询
                string queryMode = "1";  //长度 1，固定值为1，订单基本信息
                string pid = _merchantAcctId;
                string orderId = orderid;//商家订单号
                string refundSeqid = orderIndexNo; //退款流水号，商户退款时提交的流水号
                string key = _key;
                string signMsgVal = "";

                signMsgVal = appendParam(signMsgVal, "inputCharset", inputCharset);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "queryType", queryType);
                signMsgVal = appendParam(signMsgVal, "queryMode", queryMode);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "refundSeqid", refundSeqid);

                //signMsgVal = appendParam(signMsgVal, "key", key);
                //string signMsg = GetMD5(signMsgVal, "UTF-8").ToUpper();

                string signMsg = GetEncrypting(signMsgVal); //块钱 新加密
                signMsg = HttpContext.Current.Server.UrlEncode(signMsg);

                string urlVal = "";
                urlVal += "inputCharset=" + inputCharset;
                urlVal += "&version=" + version;
                urlVal += "&signType=" + signType;
                urlVal += "&queryType=" + queryType;
                urlVal += "&queryMode=" + queryMode;
                urlVal += "&pid=" + pid;
                urlVal += "&orderId=" + orderId;
                urlVal += "&refundSeqid=" + refundSeqid;
                urlVal += "&signMsg=" + signMsg;

                //返回结果
                string result = GetUrlData(url + "?" + urlVal);
                return result;
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// 退款接口查询 2（非分账退款查询）
        /// </summary>
        /// <param name="orderid">原商户订单号</param>
        /// <param name="orderIndexNo">退款流水号</param>
        /// <param name="price">退款金额</param>
        /// <returns></returns>
        public string GetRefundNew(string OrderId, string orderIndexNo, string price)
        {
            try
            {
                #region 组合数据

                string url = "https://www.99bill.com/webapp/receiveDrawbackAction.do";

                string merchant_id = _merchantAcctId; //商户编码
                string version = "bill_drawback_api_1"; //退款版本号 固定值
                string command_type = "001"; // 操作类型 固定值
                string txOrder = orderIndexNo;//退款流水号
                string amount = price;  //退款金额
                //string postdate = DateTime.Now.ToString("yyyyMMddHHmmss"); //退款提交时间 14位 年4，月2，日2，时2，分2，秒2， 
                string postdate = orderIndexNo.Split('_')[0];

                string orderid = OrderId; //原商户订单号 ，与支付时间订单号相同
                //string merchant_key = "DYNAJUEXX5AQGTJH";
                string merchant_key = _Refundkey;
                string signMsgVal = "";

                signMsgVal = appendParam(signMsgVal, "merchant_id", merchant_id);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "command_type", command_type);
                signMsgVal = appendParam(signMsgVal, "txOrder", txOrder);
                signMsgVal = appendParam(signMsgVal, "amount", amount);
                signMsgVal = appendParam(signMsgVal, "postdate", postdate);
                signMsgVal = appendParam(signMsgVal, "orderid", orderid);
                signMsgVal = appendParam(signMsgVal, "merchant_key", merchant_key);

                string mac = GetMD5(signMsgVal, "UTF-8").ToUpper();

                string urlVal = "";
                urlVal += "merchant_id=" + merchant_id;
                urlVal += "&version=" + version;
                urlVal += "&command_type=" + command_type;
                urlVal += "&txOrder=" + txOrder;
                urlVal += "&amount=" + amount;
                urlVal += "&postdate=" + postdate;
                urlVal += "&orderid=" + orderid;
                urlVal += "&mac=" + mac;

                #endregion
                string result = GetUrlData(url + "?" + urlVal);
                return result;
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// 支付查询接口
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="orderIndexNo"></param>
        /// <returns></returns>
        public string GetPayReturn(string orderid)
        {
            try
            {
                string url = "https://www.99bill.com/msgateway/recvMerchantQueryAction.htm";
                //string url = "http://www.99bill.com/msgateway/recvMerchantQueryAction.htm";
                string version = _version;
                string signType = "1";
                string queryType = "0";  //长度 1，固定值为0，单笔查询
                string queryMode = "1";  //长度 1，固定值为1，订单基本信息
                string orderId = orderid; //商户订单号
                string pid = _merchantAcctId;
                string key = _key;
                string signMsgVal = "";

                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "queryType", queryType);
                signMsgVal = appendParam(signMsgVal, "queryMode", queryMode);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "pid", pid);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "key", key);
                string signMsg = GetMD5(signMsgVal, "UTF-8").ToUpper();

                string urlVal = "";
                urlVal += "&version=" + version;
                urlVal += "&queryType=" + queryType;
                urlVal += "&queryMode=" + queryMode;
                urlVal += "&orderId=" + orderId;
                urlVal += "&pid=" + pid;
                urlVal += "&signType=" + signType;
                urlVal += "&key=" + key;
                urlVal += "&signMsg=" + signMsg;

                //返回结果
                string result = GetUrlData(url + "?" + urlVal);
                return result;
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        #endregion

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
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logs\\LogicPay\\_99Bill\\";
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