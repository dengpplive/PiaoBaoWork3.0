using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Pay_99BillNotifyUrl
/// </summary>
public partial class Pay_Pos_99BillPosNotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("进入 Pay_99BillPosNotifyUrl_Page_Load（）" + Request.Url.ToString(),true);

        string str = IsValidationNew();

        Response.Write(str);
    }
    /// <summary>
    /// 执行方法
    /// </summary>
    private string IsValidationNew()
    {
        string rtnOk = "06";

        if (Request["merchantId"]==null)
        { 
            return "";
        }

        try
        {
            PbProject.Logic.Pay._99Bill _99bill = new PbProject.Logic.Pay._99Bill();

            string processFlag = Request["processFlag"].ToString().Trim();
            string txnType = Request["txnType"].ToString().Trim();
            string orgTxnType = Request["orgTxnType"].ToString().Trim();
            string amt = Request["amt"].ToString().Trim();
            string orgExternalTraceNo = Request["orgExternalTraceNo"].ToString().Trim();
            string terminalOperId = Request["terminalOperId"].ToString().Trim();
            string terminalId = Request["terminalId"].ToString().Trim();//
            string authCode = Request["authCode"].ToString().Trim();
            string RRN = Request["RRN"].ToString().Trim();
            string shortPAN = Request["shortPAN"].ToString().Trim();
            string responseCode = Request["responseCode"].ToString().Trim();
            string responseMessage = Request["responseMessage"].ToString().Trim();//
            string cardType = Request["cardType"].ToString().Trim();
            string issuerId = Request["issuerId"].ToString().Trim();
            string issuerIdView = Request["issuerIdView"].ToString().Trim();//
            string signature = Request["signature"].ToString().Trim();//
            string externalTraceNo = Request["externalTraceNo"].ToString().Trim();  // 订单号
            string merchantId = Request["merchantId"].ToString().Trim();//
            string txnTime = Request["txnTime"].ToString().Trim();

            responseMessage = HttpUtility.UrlDecode(responseMessage, Encoding.GetEncoding("utf-8"));
            issuerIdView = HttpUtility.UrlDecode(issuerIdView, Encoding.GetEncoding("utf-8"));


            string val = "";
            val = appendParam(val, processFlag);
            val = appendParam(val, txnType);
            val = appendParam(val, orgTxnType);
            val = appendParam(val, amt);
            val = appendParam(val, externalTraceNo);
            val = appendParam(val, orgExternalTraceNo);
            val = appendParam(val, terminalOperId);
            val = appendParam(val, authCode);
            val = appendParam(val, RRN);
            val = appendParam(val, txnTime);
            val = appendParam(val, shortPAN);
            val = appendParam(val, responseCode);
            val = appendParam(val, cardType);
            val = appendParam(val, issuerId);

            if (CerRSAVerifySignature(val, signature, HttpContext.Current.Server.MapPath("~/Pay/Key/mgw.cer")))
            {
                OnErrorNew("支付失败: " + externalTraceNo, false);

                rtnOk = "1";

                #region 验签成功

                //消费交易
                if (txnType == "PUR")
                {
                    //成功
                    if (processFlag == "0")
                    {
                        //PiaoBao.BLLLogic.Pay.YeePay yeepay = new PiaoBao.BLLLogic.Pay.YeePay();
                        //OnErrorNew("开始记录");
                        ////默认支持银行卡
                        //string pay_type = "1";
                        //yeepay.Charge(terminalId, amt, pay_type, RRN, txnTime.Replace(" ", ""));
                        //OnErrorNew("结束记录");



                        ///// <summary>
                        ///// 充值成功,生成充值账单
                        ///// </summary>
                        ///// <param name="orderId">订单编号</param>
                        ///// <param name="payNo">交易号</param>
                        ///// <param name="price">交易金额</param>
                        ///// <param name="payWay"> //payWay支付方式:1支付宝/2快钱/3汇付/4/财付通/5支付宝网银/6快钱网银/7汇付网银/8财付通网银/
                        ///// 9支付宝pos/10快钱pos/11汇付pos/12财付通/13易宝pos/14账户支付</param>
                        ///// <param name="useId">付款人id 或者 POS机编号</param>
                        ///// <param name="operReason">原因或操作描述</param>
                        ///// <param name="remark">备注</param>

                        PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
                        bill.CreateLogMoneyDetail(externalTraceNo, externalTraceNo, amt, 10, terminalId, "在线支付", "在线充值");  // 在线充值

                        //报告给快钱处理结果，并提供将要重定向的地址。
                        OnErrorNew("支付成功:" + externalTraceNo, false);

                        rtnOk = "0";
                    }
                }

                #endregion
            }
            else
            {
                OnErrorNew("验签失败:" + externalTraceNo, false);
                rtnOk = "IG";
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("处理异常:" , true);
        }
        return rtnOk;
    }

    //验签方法
    public static bool CerRSAVerifySignature(string OriginalString, string SignatureString, string pubkey_path)
    {
        byte[] OriginalByte = System.Text.Encoding.UTF8.GetBytes(OriginalString);

        byte[] SignatureByte = Convert.FromBase64String(SignatureString);
        X509Certificate2 x509 = new X509Certificate2(pubkey_path);
        RSACryptoServiceProvider rsapub = (RSACryptoServiceProvider)x509.PublicKey.Key;
        rsapub.ImportCspBlob(rsapub.ExportCspBlob(false));
        RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapub);
        f.SetHashAlgorithm("SHA1");
        SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
        byte[] HashData = sha.ComputeHash(OriginalByte);

        if (f.VerifySignature(HashData, SignatureByte))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public string appendParam(string returnStr, string paramValue)
    {
        if (!string.IsNullOrEmpty(paramValue) && (paramValue != ""))
        {
            returnStr += paramValue;
        }
        return returnStr;
    }

    /// <summary>
    /// 记录文本日志
    /// </summary>
    /// <param name="content">记录内容</param>
    /// <param name="IsPostBack">是否记录 Request 参数</param>
    private void OnErrorNew(string errContent, bool IsRecordRequest)
    {
        try
        {
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), errContent, IsRecordRequest, null);

            #region 记录文本日志

            /*    
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("记录时间：" + DateTime.Now.ToString() + "\r\n");
            sb.AppendFormat("  IP ：" + Page.Request.UserHostAddress + "\r\n");
            sb.AppendFormat("  Content : " + errContent + "\r\n");

            if (IsRecordRequest)
            {
                #region 记录 Request 参数
                try
                {
                    sb.AppendFormat("  Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");

                    if (HttpContext.Current.Request != null)
                    {
                        if (HttpContext.Current.Request.HttpMethod == "POST")
                        {
                            #region POST 提交
                            if (HttpContext.Current.Request.Form.Count != 0)
                            {
                                for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
                                {
                                    sb.AppendFormat(HttpContext.Current.Request.Form.Keys[i].ToString() + " = " + HttpContext.Current.Request.Form[i].ToString() + "\r\n");
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" HttpContext.Current.Request.Form.Count = 0 \r\n");
                            }

                            #endregion
                        }
                        else if (HttpContext.Current.Request.HttpMethod == "GET")
                        {
                            #region GET 提交

                            if (HttpContext.Current.Request.QueryString.Count != 0)
                            {
                                for (int i = 0; i < HttpContext.Current.Request.QueryString.Count; i++)
                                {
                                    sb.AppendFormat(HttpContext.Current.Request.QueryString.Keys[i].ToString() + " = " + HttpContext.Current.Request.QueryString[i].ToString() + "\r\n");
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" HttpContext.Current.QueryString.Form.Count = 0 \r\n");
                            }

                            #endregion
                        }
                        else
                        {
                            #region 不是 GET 和 POST

                            sb.AppendFormat("  不是 GET 和 POST, Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");

                            System.Collections.Specialized.NameValueCollection nv = Request.Params;
                            foreach (string key in nv.Keys)
                            {
                                sb.AppendFormat("{0}={1} \r\n", key, nv[key]);
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        sb.AppendFormat("  HttpContext.Current.Request=null \r\n");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendFormat("  catch: " + ex + "\r\n");
                }

                #endregion
            }

            sb.AppendFormat("----------------------------------------------------------------------------------------------------\r\n");
            sb.AppendFormat("----------------------------------------------------------------------------------------------------");

            string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\" + Page + "\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
            fs.WriteLine(sb.ToString());
            fs.Close();
             */

            #endregion
        }
        catch (Exception)
        {

        }
    }
}