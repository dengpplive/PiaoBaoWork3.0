using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using PbProject.Logic.Pay.batch_trans;
using PbProject.Dal.Mapping;

public partial class Pay_BatchPay_Notify_Url : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<string> sqlList = new List<string>();
        SortedDictionary<string, string> sPara = GetRequestPost();
        if (sPara.Count > 0)
        {
            Notify aliNotify = new Notify();
            bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);
            if (verifyResult)
            {
                //批量付款数据中转账成功的详细信息
                string success_details = GetArr("success_details");
                if (!string.IsNullOrEmpty(success_details))
                {
                    sqlList.AddRange(UpdateSql(success_details));
                }
                //批量付款数据中转中失败的详细信息
                string fail_details = GetArr("fail_details");
                
                if (!string.IsNullOrEmpty(fail_details))
                {
                    sqlList.AddRange(UpdateSql(fail_details));
                }
                string msg = string.Empty;
                new SqlHelper().ExecuteSqlTran(sqlList, out msg);
                Response.Write("success");
            }
            else
            {
                Response.Write("fail");
            }
        }
        else
        {
            Response.Write("无通知参数");
        }
    }
    private List<string> UpdateSql(string details)
    {
        List<string> sqlList = new List<string>();
        if (!string.IsNullOrEmpty(details))
        {
            string[] payee = details.Split('|');
            for (int i = 0; i < payee.Length; i++)
            {
                if (!string.IsNullOrEmpty(payee[i]))
                {
                    string[] itemPayee = payee[i].Split('^');
                    if (itemPayee.Length == 8)
                    {
                        if (itemPayee[4].ToUpper() == "S")
                            sqlList.Add(string.Format("update BatchPay set OutOrderId='{0}',Result=1,Remark='{2}',PayTime=GETDATE() where OrderID='{1}'", itemPayee[6], itemPayee[0], payee[i]));
                        else
                            sqlList.Add(string.Format("update BatchPay set OutOrderId='{0}',Result=2,Remark='{2}',PayTime=GETDATE() where OrderID='{1}'", itemPayee[6], itemPayee[0], payee[i]));
                    }
                }
            }
        }
        return sqlList;
    }
    private string GetArr(string key)
    {
        string result=string.Empty;
        object obj = Request.Form[key];
        if (obj != null)
            result = obj.ToString();
        return result;
    }
    /// <summary>
    /// 获取支付宝post过来的通知消息，并以“参数名=参数值”的形式形成数组
    /// </summary>
    /// <returns>request回来的信息形成的数组</returns>
    public SortedDictionary<string, string> GetRequestPost()
    {
        int i = 0;
        SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
        NameValueCollection coll;
        coll = Request.Form;
        String[] requestItem = coll.AllKeys;
        for (i = 0; i < requestItem.Length; i++)
        {
             sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
        }
        return sArray;
    }
}