using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.Pay;
using System.Collections.Specialized;

public partial class Pay_ReturnPage_AddPayMoney : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AcceptParam AP = new AcceptParam();
        bool result = false;
        try
        {
            if (!IsPostBack)
            {
                NameValueCollection NC = HttpContext.Current.Request.Form;
                AP.sbLog.Append("\r\n\r\n同步数据参数:\r\n");
                bool IsValid = false;
                string Val = "";
                foreach (string key in NC.Keys)
                {
                    Val = HttpUtility.UrlDecode(NC[key]);
                    AP.sbLog.Append(key + "=" + Val + "\r\n");
                    switch (key)
                    {
                        case "Pnr":
                            {
                                AP.m_Pnr = Val;
                                if (string.IsNullOrEmpty(AP.m_Pnr.Trim()))
                                {
                                    AP.sbLog.Append("PNR不能为空！");
                                    IsValid = true;
                                }
                                break;
                            }
                        case "Office":
                            {
                                AP.m_Office = Val;
                                break;
                            }
                        case "OrderId":
                            {
                                AP.m_OrderId = Val;
                                if (string.IsNullOrEmpty(AP.m_OrderId.Trim()))
                                {
                                    AP.sbLog.Append("订单号不能为空！");
                                    IsValid = true;
                                }
                                break;
                            }
                        case "LoginName":
                            {
                                AP.m_LoginName = Val;
                                if (string.IsNullOrEmpty(AP.m_LoginName.Trim()))
                                {
                                    AP.sbLog.Append("登录账号不能为空！");
                                    IsValid = true;
                                }
                                break;
                            }
                        case "CompanyName":
                            {
                                AP.m_CompanyName = Val;
                                if (string.IsNullOrEmpty(AP.m_CompanyName.Trim()))
                                {
                                    AP.sbLog.Append("公司名称不能为空！");
                                    IsValid = true;
                                }
                                break;
                            }
                        case "Price":
                            {
                                decimal.TryParse(Val, out AP.m_Price);
                                if (AP.m_Price <= 0 || AP.m_Price > 2000000)
                                {
                                    AP.sbLog.Append("输入金额超出范围！");
                                    IsValid = true;
                                }
                                break;
                            }
                        case "PassengerList":
                            {
                                AP.m_PassengerList = Val;
                                if (string.IsNullOrEmpty(AP.m_PassengerList.Trim()))
                                {
                                    AP.sbLog.Append("乘客姓名不能为空！");
                                    IsValid = true;
                                }
                                break;
                            }
                        default:
                            break;
                    }
                    if (IsValid)
                    {
                        break;
                    }
                }
                if (IsValid)
                {
                    AP.sbLog.Append("结束==========");
                    PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), AP.sbLog.ToString(), false, HttpContext.Current.Request);
                }
                else
                {
                    Bill bill = new Bill();
                    //调用同步方法
                    result = bill.TongBuData(AP);
                }
            }
        }
        catch (Exception ex)
        {
            AP.sbLog.Append("页面Pay_ReturnPage_AddPayMoney异常:" + ex.Message);
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), AP.sbLog.ToString(), false, HttpContext.Current.Request);
        }
        finally
        {
            string strResult = "FAIL";
            if (result)
            {
                strResult = "SUCESS";
            }
            else
            {
                strResult = "FAIL";
            }
            HttpContext.Current.Response.Write(strResult);
        }
    }
}