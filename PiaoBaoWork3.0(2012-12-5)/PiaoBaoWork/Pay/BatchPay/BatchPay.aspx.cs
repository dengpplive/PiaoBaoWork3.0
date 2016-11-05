using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.Pay;
using PbProject.Dal.Mapping;
using PbProject.Logic.Pay.batch_trans;

public partial class Pay_BatchPay_BatchPay : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
            System.Text.StringBuilder detail_data = new System.Text.StringBuilder();
            //--付款信息
            string payNum = getParameter("payNum");
            string payName = getParameter("payName");
            string payReason = getParameter("payReason");
            decimal batch_fee = 0;
            //--收款信息
            string payeeNum = getParameter("payeeNum");
            string payeeName = getParameter("payeeName");
            string payeeAmount = getParameter("payeeAmount");
            if (string.IsNullOrEmpty(payNum) || string.IsNullOrEmpty(payName) || string.IsNullOrEmpty(payReason) || string.IsNullOrEmpty(payeeNum) || string.IsNullOrEmpty(payeeName) || string.IsNullOrEmpty(payeeAmount))
            {
                Response.Write("参数信息录入不完整!");
                Response.End();
            }
            //=============生成数据==================
            string[] num = payeeNum.Split(',');
            string[] name = payeeName.Split(',');
            string[] amount = payeeAmount.Split(',');
            int batch_num = num.Length;
            if (batch_num > 0)
            {
                List<string> sqlList = new List<string>();
                if (batch_num == name.Length && batch_num == amount.Length)
                {
                    string datenum = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    for (int i = 0; i < num.Length; i++)
                    {
                        string ordernum=string.Format("{0}{1}",datenum,i);
                        batch_fee += Convert.ToDecimal(amount[i]);
                        if (i > 0)
                            detail_data.Append("|");
                        detail_data.AppendFormat("{0}^{1}^{2}^{3}^{4}",ordernum, num[i], name[i], amount[i], payReason);
                        string sql = string.Format("insert into BatchPay(PayAccount,PayName,OrderID,GetAccount,GetName,PayMoney,PayRemark,OutOrderId,UserID,PayCompanyID) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", payNum, payName, ordernum, num[i], name[i], amount[i], payReason, string.Empty, this.mUser.id, this.mUser.CpyNo);
                        sqlList.Add(sql);
                    }
                    string msg = string.Empty;
                    new SqlHelper().ExecuteSqlTran(sqlList, out msg);
                }
                //把请求参数打包成数组
                SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                sParaTemp.Add("partner", Config.Partner);
                sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
                sParaTemp.Add("service", "batch_trans_notify");
                sParaTemp.Add("notify_url", "http://lzh.mypb.cn/Pay/BatchPay/Notify_Url.aspx");
                sParaTemp.Add("email", payNum);
                sParaTemp.Add("account_name", payName);
                sParaTemp.Add("pay_date", DateTime.Now.ToString("yyyyMMdd"));
                sParaTemp.Add("batch_no", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                sParaTemp.Add("batch_fee", batch_fee.ToString("F2"));
                sParaTemp.Add("batch_num", batch_num.ToString());
                sParaTemp.Add("detail_data", detail_data.ToString());
                string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
                Response.Write(sHtmlText);
            
        }
    }
    private string getParameter(string param)
    {
        string paramValue = string.Empty;
        object obj = Request.Form[param];
        if (obj != null)
        {
            paramValue = obj.ToString();
        }
        return paramValue;
    }
}