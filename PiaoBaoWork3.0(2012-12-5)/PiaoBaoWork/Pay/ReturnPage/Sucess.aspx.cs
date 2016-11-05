using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// 在线支付返回提示页面
/// </summary>
public partial class Pay_ReturnPage_Sucess : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString["PayType"] != null
               && Request.QueryString["ReturnType"] != null
               && Request.QueryString["OrderId"] != null
                && Request.QueryString["Price"] != null
                 && Request.QueryString["OnLineNo"] != null)
            {
                 string payType = Request.QueryString["PayType"].ToString(); //支付方式 1支付宝、2快钱、3汇付、4财付通
                 string returnType = Request.QueryString["ReturnType"].ToString();//返回结果 0 支付失败，1 支付成功
                 string orderId = Request.QueryString["OrderId"].ToString();//订单号
                 string price = Request.QueryString["Price"].ToString();//支付金额
                 string onLineNo = Request.QueryString["OnLineNo"].ToString();//交易号

                 decimal payPrice = decimal.Parse(price);

                 if (payType == "1")
                 { 
                     #region 支付宝

                     #endregion
                 }
                 else if (payType == "2")
                 {
                     payPrice = payPrice / 100;
                     payPrice = Math.Round(payPrice, 2);
                 }
                 else if (payType == "3")
                 {

                 }
                 else if (payType == "4")
                 {
                     payPrice = payPrice / 100;
                     payPrice = Math.Round(payPrice, 2);
                 }

                 lblOrderId.Text = orderId;
                 lblOnLineNo.Text = onLineNo;
                 lblPrice.Text = payPrice.ToString("F2");
                 tbid.Visible = true;

                 if (returnType == "1")
                 {
                     //支付成功
                     trSuccess.Visible = true;
                     trFail2.Visible = true;

                     trFail.Visible = false;
                 }
                 else 
                 {
                     //支付失败
                     trSuccess.Visible = false;
                     trFail2.Visible = false;

                     trFail.Visible = true;
                 }
            }
        }
        catch (Exception)
        {
            
        }
    }
}