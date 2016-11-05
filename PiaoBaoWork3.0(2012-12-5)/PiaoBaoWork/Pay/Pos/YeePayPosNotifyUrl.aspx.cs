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
using System.Xml.Linq;
using System.Xml;

/// <summary>
/// 易宝POS 通知页面 
/// </summary>
public partial class Pay_Pos_YeePayPosNotifyUrl : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                OnErrorNew("Pay_Pos_YeePayPosNotifyUrl", false);
                SetData();
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// SetData
    /// </summary>
    private void SetData()
    {
        #region 参数

        //head
        string Version = "";
        string ServiceCode = "";
        string TransactionID = "";
        string SrcSysID = "";
        string DstSysID = "";
        string Result_code = "";
        string Result_msg = "";
        string order_no = "";
        string HMAC = "";
        //body
        string Employee_ID = "";
        string PosSn = "";
        string PosRequestID = "";
        string ReferNo = "";
        string OrderNo = "";
        string Amount = "";
        string Split = "";
        string ReqTime = "";
        string PayTypeCode = "";
        string PayMethod = "";
        string ChequeNo = "";
        string BankCardName = "";
        string BankCardNo = "";
        string BankOrderNo = "";
        string YeepayOrderNo = "";

        #endregion

        try
        {
            if (Request.RequestType == "POST")
            {
                //接收并读取POST过来的XML文件流
                StreamReader reader = new StreamReader(Request.InputStream);
                String xmlData = reader.ReadToEnd();
                XElement root = XElement.Parse(xmlData);

                //测试数据
                //String xmlData = "<?xml version='1.0' encoding='UTF-8'?><COD-MS><SessionHead><Version>1.0</Version><ServiceCode>COD403</ServiceCode><TransactionID>loyoyoCOD403201206133852742769</TransactionID><SrcSysID>yeepay</SrcSysID><DstSysID>loyoyo</DstSysID><ReqTime>20120613185635</ReqTime><HMAC>46a2796241d09cef6e2d7a47d66b58f3</HMAC></SessionHead><SessionBody><EmployeeID>123456</EmployeeID><PosSn>0373NL230526</PosSn><OrderNo></OrderNo><Amount>0.02</Amount><PosRequestID>100045</PosRequestID><ReferNo>295220459300</ReferNo><PayTypeCode>1</PayTypeCode><PayMethod>1</PayMethod><ChequeNo></ChequeNo><BankCardNo>532458******8327</BankCardNo><BankCardName>中国建设银行</BankCardName><BankOrderNo>06061311064879185607216579479811</BankOrderNo><YeepayOrderNo>817769940727510I</YeepayOrderNo></SessionBody></COD-MS>";
                //XElement root = XElement.Parse(xmlData);

                #region 解析参数

                //第一段
                foreach (XElement xe in root.Elements("SessionHead"))
                {
                    if (xe.Element("Version") != null)
                    {
                        Version = xe.Element("Version").Value;
                    }
                    if (xe.Element("ServiceCode") != null)
                    {
                        ServiceCode = xe.Element("ServiceCode").Value;
                    }
                    if (xe.Element("TransactionID") != null)
                    {
                        TransactionID = xe.Element("TransactionID").Value;
                    }
                    if (xe.Element("SrcSysID") != null)
                    {
                        SrcSysID = xe.Element("SrcSysID").Value;
                    }
                    if (xe.Element("DstSysID") != null)
                    {
                        DstSysID = xe.Element("DstSysID").Value;
                    }
                    if (xe.Element("ReqTime") != null)
                    {
                        ReqTime = xe.Element("ReqTime").Value;
                    }
                    if (xe.Element("HMAC") != null)
                    {
                        HMAC = xe.Element("HMAC").Value;
                    }
                }
                //第二段
                foreach (XElement xe in root.Elements("SessionBody"))
                {
                    if (xe.Element("EmployeeID") != null)
                    {
                        Employee_ID = xe.Element("EmployeeID").Value;
                    }
                    if (xe.Element("PosSn") != null)
                    {
                        PosSn = xe.Element("PosSn").Value;
                    }
                    if (xe.Element("PosRequestID") != null)
                    {
                        PosRequestID = xe.Element("PosRequestID").Value;
                    }
                    if (xe.Element("ReferNo") != null)
                    {
                        ReferNo = xe.Element("ReferNo").Value;
                    }
                    if (xe.Element("OrderNo") != null)
                    {
                        order_no = xe.Element("OrderNo").Value;
                    }
                    if (xe.Element("Amount") != null)
                    {
                        Amount = xe.Element("Amount").Value;
                    }
                    if (xe.Element("Split") != null)
                    {
                        Split = xe.Element("Split").Value;
                    }
                    if (xe.Element("ReqTime") != null)
                    {
                        ReqTime = xe.Element("ReqTime").Value;
                    }
                    if (xe.Element("PayTypeCode") != null)
                    {
                        PayTypeCode = xe.Element("PayTypeCode").Value;
                    }
                    if (xe.Element("PayMethod") != null)
                    {
                        PayMethod = xe.Element("PayMethod").Value;
                    }
                    if (xe.Element("ChequeNo") != null)
                    {
                        ChequeNo = xe.Element("ChequeNo").Value;
                    }
                    if (xe.Element("BankCardName") != null)
                    {
                        BankCardName = xe.Element("BankCardName").Value;
                    }
                    if (xe.Element("BankCardNo") != null)
                    {
                        BankCardNo = xe.Element("BankCardNo").Value;
                    }
                    if (xe.Element("BankOrderNo") != null)
                    {
                        BankOrderNo = xe.Element("BankOrderNo").Value;
                    }
                    if (xe.Element("YeepayOrderNo") != null)
                    {
                        YeepayOrderNo = xe.Element("YeepayOrderNo").Value;
                    }
                }

                #endregion

                #region 处理

                string strOnError = "";
                bool result = false;

                try
                {
                    strOnError += "/开始记录账单";

                    if (ServiceCode == "COD403")
                    {
                        result = new PbProject.Logic.Pay.Bill().CreateLogMoneyDetail("", YeepayOrderNo, Amount, 13, PosSn, "POS充值", "POS充值");
                    }
                    else if (ServiceCode == "COD406")
                    {
                        result = new PbProject.Logic.Pay.Bill().CreateCancelLogMoneyDetail("", YeepayOrderNo, Amount, 13, PosSn, "POS充值失败,已经撤销充值", "POS充值失败,已经撤销充值");
                    }
                    else
                    {
                        strOnError += "/ServiceCode:" + ServiceCode;
                    }

                    if (result == true)
                    {
                        strOnError += "/记录账单成功";
                    }
                    else
                    {
                        strOnError += "/记录账单失败";
                    }
                    
                    Result_code = "2";
                    Result_msg = "成功";
                }
                catch (Exception ex)
                {
                    Result_code = "3";
                    Result_msg = "失败";

                    strOnError += "/失败（catch）:" + ex;
                }

                OnErrorNew(strOnError,false);

                //构造返回字符串
                StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<COD-MS>");
                sb.Append("<SessionHead>");
                sb.Append("<Result_code>" + Result_code + "</Result_code>");
                sb.Append("<Result_msg>" + Result_msg + "</Result_msg>");
                sb.Append("<Version>" + Version + "</Version>");
                sb.Append("<ServiceCode>" + ServiceCode + "</ServiceCode>");
                sb.Append("<TransactionID>" + TransactionID + "</TransactionID>");
                sb.Append("<SrcSysID>" + SrcSysID + "</SrcSysID>");
                sb.Append("<DstSysID>" + DstSysID + "</DstSysID>");
                sb.Append("<RespTime>" + ReqTime + "</RespTime>");
                sb.Append("<HMAC>" + HMAC + "</HMAC>");
                sb.Append("</SessionHead>");
                sb.Append("<SessionBody>");
                sb.Append("<OrderNo>" + OrderNo + "</OrderNo>");
                sb.Append("</SessionBody>");
                sb.Append("</COD-MS>");

                OnErrorNew(xmlData + "\r\n" + sb.ToString() + "\r\n end",false);

                Response.Write(sb.ToString());

                #endregion
            }
            else
            {
                OnErrorNew("提交方式不是 POST: Request.RequestType=" + Request.RequestType, false);
            }
        }
        catch (Exception ex)
        {
            OnErrorNew("SetData_catch:" + ex.ToString(),false);
        }
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