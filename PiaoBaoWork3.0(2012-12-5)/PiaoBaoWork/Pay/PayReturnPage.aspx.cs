using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// 技术使用
/// </summary>
public partial class Pay_PayReturnPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnNotify_Click(object sender, EventArgs e)
    {
        string url = "";

        try
        {
            string strReturnValue = txtReturnValue.Text.Trim();

            string payType = rblPayType.SelectedValue;

            if (string.IsNullOrEmpty(strReturnValue))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('请输入通知内容！');", true);
            }
            else if (payType == "0")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('请选择支付方式！');", true);
            }
            else if (payType == "1")
            {
                #region 支付宝

                string hiddenStr = "";

                string[] strReturnValueS = strReturnValue.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strReturnValueS.Length; i++)
                {
                    if (strReturnValueS[i] != "")
                    {
                        string[] strs = strReturnValueS[i].Split('=');
                        hiddenStr += "<input type='hidden' name='" + strs[0].Trim() + "' value='" + strs[1].Trim() + "' />";
                    }
                }
                if (!string.IsNullOrEmpty(hiddenStr))
                {
                    hiddenStr = "<form name='aliPay' id='aliPay' method='post' action='ReturnPage/AliPayNotifyUrl.aspx'>" + hiddenStr;
                    hiddenStr += "</form><script>document.aliPay.submit()</script>";
                    Response.Write(hiddenStr);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('提交数据错误！');", true);
                }
                #endregion
            }
            else if (payType == "2")
            {
                #region 快钱
                string hiddenStr = "";

                string[] strReturnValueS = strReturnValue.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strReturnValueS.Length; i++)
                {
                    if (strReturnValueS[i] != "")
                    {
                        string[] strs = strReturnValueS[i].Split('=');
                        hiddenStr += "<input type='hidden' name='" + strs[0].Trim() + "' value='" + strs[1].Trim() + "' />";
                    }
                }
                if (!string.IsNullOrEmpty(hiddenStr))
                {
                    hiddenStr = "<form name='kqPay' id='kqPay' method='post' action='ReturnPage/99BillNotifyUrl.aspx'>" + hiddenStr;
                    hiddenStr += "</form><script>document.kqPay.submit()</script>";
                    Response.Write(hiddenStr);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('提交数据错误！');", true);
                }
                #endregion
            }
            else if (payType == "3")
            {
                #region 汇付

                string hiddenStr = "";

                string[] strReturnValueS = strReturnValue.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strReturnValueS.Length; i++)
                {
                    if (strReturnValueS[i] != "")
                    {
                        string[] strs = strReturnValueS[i].Split('=');
                        hiddenStr += "<input type='hidden' name='" + strs[0].Trim() + "' value='" + strs[1].Trim() + "' />";
                    }
                }
                if (!string.IsNullOrEmpty(hiddenStr))
                {
                    hiddenStr = "<form name='hfPay' id='hfPay' method='post' action='ReturnPage/ChinaPnrNotifyUrl.aspx'>" + hiddenStr;
                    hiddenStr += "</form><script>document.hfPay.submit()</script>";
                    Response.Write(hiddenStr);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('提交数据错误！');", true);
                }

                #endregion
            }
            else if (payType == "4")
            {
                #region 财付通

                #endregion
            }
            else if (payType == "9")
            {
                #region 支付宝POS
                Byte[] strByte = System.Text.Encoding.Default.GetBytes(strReturnValue);
                System.Net.WebClient wc = new System.Net.WebClient();

                //本地测试
                //url = "http://" + Request.Url.Authority + "/PiaoBaoWork/Pay/Pos/AliPayPosNotifyUrl.aspx";

                //正式地址
                 url = "http://" + Request.Url.Authority + "/Pay/Pos/AliPayPosNotifyUrl.aspx";

                wc.UploadData(url, "POST", strByte);
                #endregion
            }
            else if (payType == "12")
            {
                #region 汇付POS
                //Byte[] strByte = System.Text.Encoding.Default.GetBytes(strReturnValue);
                //System.Net.WebClient wc = new System.Net.WebClient();

                ////本地测试
                //url = "http://" + Request.Url.Authority + "/PiaoBaoWork/Pay/Pos/ChinaPnrPosNotifyUrl.aspx";

                ////正式地址
                ////url = "http://" + Request.Url.Authority + "/Pay/Pos/ChinaPnrPosNotifyUrl.aspx";

                //wc.UploadData(url, "POST", strByte);

                string hiddenStr = "";

                string[] strReturnValueS = strReturnValue.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strReturnValueS.Length; i++)
                {
                    if (strReturnValueS[i] != "")
                    {
                        string[] strs = strReturnValueS[i].Split('=');
                        hiddenStr += "<input type='hidden' name='" + strs[0].Trim() + "' value='" + strs[1].Trim() + "' />";
                    }
                }
                if (!string.IsNullOrEmpty(hiddenStr))
                {
                    hiddenStr = "<form name='ChinaPnr' id='ChinaPnr' method='post' action='Pos/ChinaPnrPosNotifyUrl.aspx'>" + hiddenStr;
                    hiddenStr += "</form><script>document.ChinaPnr.submit()</script>";
                    Response.Write(hiddenStr);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('提交数据错误！');", true);
                }


                #endregion
            }
            else if (payType == "13")
            {
                #region 易宝POS
                Byte[] strByte = System.Text.Encoding.Default.GetBytes(strReturnValue);
                System.Net.WebClient wc = new System.Net.WebClient();

                //本地测试
                //url = "http://" + Request.Url.Authority + "/PiaoBaoWork/Pay/Pos/YeePayPosNotifyUrl.aspx";

                //正式地址
                url = "http://" + Request.Url.Authority + "/Pay/Pos/YeePayPosNotifyUrl.aspx";

                wc.UploadData(url, "POST", strByte);
                #endregion
            }
            else if (payType == "20")
            {
                #region 支付宝

                string hiddenStr = "";

                string[] strReturnValueS = strReturnValue.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strReturnValueS.Length; i++)
                {
                    if (strReturnValueS[i] != "")
                    {
                        string[] strs = strReturnValueS[i].Split('=');
                        hiddenStr += "<input type='hidden' name='" + strs[0].Trim() + "' value='" + strs[1].Trim() + "' />";
                    }
                }
                if (!string.IsNullOrEmpty(hiddenStr))
                {
                    hiddenStr = "<form name='aliPay' id='aliPay' method='post' action='PTReturnPage/AutoPayByAlipayNotifyUrl.aspx'>" + hiddenStr;
                    hiddenStr += "</form><script>document.aliPay.submit()</script>";
                    Response.Write(hiddenStr);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('提交数据错误！');", true);
                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('操作异常！');", true);
        }

        lblUrl.Text = url;
    }
}