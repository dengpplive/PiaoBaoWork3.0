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
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// 快钱返回
/// </summary>
public partial class Pay_ReturnPage_99BillReturnUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("Pay_ReturnPage_99BillReturnUrl", true);
        if (Request.QueryString["version"] != null)
        {
            string value = IsValidation();

            if (value != "")
            {
                Response.Redirect(value);
            }
        }
    }
    /// <summary>
    /// 处理快钱返回
    /// </summary>
    private string IsValidation()
    {
        string value = "";

        try
        {
            PbProject.Logic.Pay._99Bill _99bill = new PbProject.Logic.Pay._99Bill();

            #region 参数
            
            String rtnUrl = "";
            //设置人民币网关密钥 
            //String key = _99bill._key;
            //获取网关版本 
            String version = Request["version"].ToString().Trim();
            //获取语言种类 
            String language = Request["language"].ToString().Trim();
            //签名类型.固定值 
            String signType = Request["signType"].ToString().Trim();
            //获取支付方式 
            String payType = Request["payType"].ToString().Trim();
            //获取银行代码 
            String bankId = Request["bankId"].ToString().Trim();
            //商户号 
            String pid = Request["pid"].ToString().Trim();
            //获取商户订单号
            String orderId = Request["orderId"].ToString().Trim();
            //获取订单提交时间 
            String orderTime = Request["orderTime"].ToString().Trim();
            //获取原始订单金额 
            String orderAmount = Request["orderAmount"].ToString().Trim();
            //获取快钱交易号 
            String dealId = Request["dealId"].ToString().Trim();
            //获取银行交易号 
            String bankDealId = Request["bankDealId"].ToString().Trim();
            //获取在快钱交易时间 
            String dealTime = Request["dealTime"].ToString().Trim();
            //获取实际支付金额 
            String payAmount = Request["payAmount"].ToString().Trim();
            //我们的联系方式 
            String fee = Request["fee"].ToString().Trim();
            //我们的联系方式类型
            String payeeContactType = Request["payeeContactType"].ToString().Trim();
            //我们的收款金额 
            String payeeContact = Request["payeeContact"].ToString().Trim();
            //获取交易手续费 
            String payeeAmount = Request["payeeAmount"].ToString().Trim();
            //获取扩展字段1
            String ext1 = Request["ext1"].ToString().Trim();
            //获取扩展字段2
            String ext2 = Request["ext2"].ToString().Trim();
            //获取处理结果
            //10代表 成功; 11代表 失败
            //00代表 下订单成功（仅对电话银行支付订单返回）;01代表 下订单失败（仅对电话银行支付订单返回）
            String payResult = Request["payResult"].ToString().Trim();
            //分润字符串 
            String sharingResult = Request["sharingResult"].ToString().Trim();
            //获取错误代码 
            String errCode = Request["errCode"].ToString().Trim();
            //获取加密签名串
            String signMsg = Request["signMsg"].ToString().Trim();

            #endregion

            #region 生成加密串。必须保持如下顺序。
            String merchantSignMsgVal = "";
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "version", version);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "language", language);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "signType", signType);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "payType", payType);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "bankId", bankId);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "pid", pid);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "orderId", orderId);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "orderTime", orderTime);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "orderAmount", orderAmount);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "dealId", dealId);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "bankDealId", bankDealId);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "dealTime", dealTime);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "payAmount", payAmount);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "fee", fee);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "payeeContactType", payeeContactType);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "payeeContact", payeeContact);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "payeeAmount", payeeAmount);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "ext1", ext1);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "ext2", ext2);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "payResult", payResult);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "sharingResult", sharingResult);
            merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "errCode", errCode);
            // merchantSignMsgVal = _99bill.appendParam(merchantSignMsgVal, "key", key); //md5 加密使用
            // String merchantSignMsg = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(merchantSignMsgVal, "MD5");
            #endregion

            #region 块钱（证书加密）

            //merchantSignMsgVal = merchantSignMsgVal + "&";
            ///pki加密方式 使用的是快钱的cer证书 
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(merchantSignMsgVal);

            byte[] SignatureByte = Convert.FromBase64String(signMsg);
            //Response.Write(SignatureByte);
            X509Certificate2 cert = new X509Certificate2(Server.MapPath("~/Pay/Key/99bill.cert.rsa.20140728.cer"), "");
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
            rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);
            //Response.Write(result);

            #endregion

            //OnErrorNew("等待验签", false);

            //商家进行数据处理，并跳转会商家显示支付结果的页面
            ///首先进行签名字符串验证
            //if (signMsg.ToUpper() == merchantSignMsg.ToUpper())
            if (f.VerifySignature(result, SignatureByte))
            {
                //OnErrorNew("验签成功", false);

                #region 验签成功

                if (payResult == "10")
                {
                    if (orderId.Substring(0, 1) == "0")
                    {
                        #region 机票

                        #endregion
                    }
                    else if (orderId.Substring(0, 1) == "1")
                    {
                        #region 充值

                        #endregion
                    }
                    else if (orderId.Substring(0, 1) == "2")
                    {
                        #region 短信
                        
                        #endregion
                    }
                    value = "Sucess.aspx?PayType=2&ReturnType=1&OrderId=" + orderId + "&Price=" + orderAmount + "&OnLineNo=" + dealId;
                }
                else
                {

                }
                #endregion
            }
            else
            {
                //OnErrorNew("验签失败", false);
            }
        }
        catch (Exception ex)
        {
            //OnErrorNew("catch:" + ex, false);
        }

        return value;
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
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), errContent, IsRecordRequest, System.Web.HttpContext.Current.Request);

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