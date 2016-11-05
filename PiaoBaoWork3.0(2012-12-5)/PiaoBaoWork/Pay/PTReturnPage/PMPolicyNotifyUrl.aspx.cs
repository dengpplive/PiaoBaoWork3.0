using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;
using PbProject.Model;
using System.Data.SqlClient;
using System.Xml;

public partial class Pay_PTReturnPage_PMPolicyNotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string xml = GetValue();
        OnErrorNew("进入 Pay_PMPolicyNotifyUrl（）:" + xml, true);
        if (xml != "")
        {
            GetData(xml);
        }
    }
    private int DeletebookPolicy(string connectionString, string TableName, string Where)
    {
        string queryString = "delete from " + TableName + " where " + Where;
        OnErrorNew("进入 Pay_PMPolicyNotifyUrl（）:DeletebookPolicy  ,"+queryString, true);
        int Rint = 0;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            try
            {
                Rint = command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }
        }
        return Rint;
    }
    private void GetData(string policyinfo)
    {
        try
        {
            string connStringSql = "Data Source=210.14.138.25,1436;Initial Catalog=PBDB;User ID=pb_user;Password=pb_userQWER@2468";
            policyinfo = HttpUtility.UrlDecode(policyinfo, Encoding.GetEncoding("gb2312"));
            //OnErrorNew("policyinfo内容：" + policyinfo, false);
            #region 测试数据
            //policyinfo = "<root><pol event=\"11\" id=\"ffffffff-fff1-9446-2935-201208022131\" rate=\"5.8\" aircom=\"TV\" from=\"CTU\" to=\"LXA/HGH/XNN\" flightno=\"\" flighscope=\"0\" routetype=\"1\" policytype=\"BSP-ET\" fromtime=\"2012-8-2 0:00:00\" totime=\"2012-8-31 23:59:59\" applyclass=\"R;V;G;Q;J;L;K;H;M;B;Y;A;F\" printtickfromtime=\"2012-08-02 00:00:00\" printticktotime=\"2012-08-31 23:59:59\" worktimefrom=\"0800\" worktimeto=\"2300\" paytype=\"1\" changerecord=\"0\" automata=\"0\" contype=\"1\" isspecial=\"0\" note=\"退票参考时间：1700RMK TJ AUTH CTU363\" center=\"\" isshared=\"0\" policyweek=\"1,2,3,4,5,6,0\" providerid=\"E0D76E0C-F910-41D9-8834-39884BA8F31C\" weekendworktimef=\"0830\" weekendworktimet=\"2230\" refundworktimeto=\"1700\" refundweekendworktimeto=\"1600\" officeid=\"CTU363\" /><pol event=\"11\" id=\"ffffffff-fff1-2208-2938-201208022132\" rate=\"5.8\" aircom=\"TV\" from=\"CKG\" to=\"XNN/LXA\" flightno=\"\" flighscope=\"0\" routetype=\"2\" policytype=\"B2B-ET\" fromtime=\"2012-8-2 0:00:00\" totime=\"2012-8-31 23:59:59\" applyclass=\"V;G;Q;J;L;K;H;M;B;Y;F\" printtickfromtime=\"2012-08-02 00:00:00\" printticktotime=\"2012-08-31 23:59:59\" worktimefrom=\"0800\" worktimeto=\"2300\" paytype=\"1\" changerecord=\"0\" automata=\"0\" contype=\"1\" isspecial=\"0\" note=\"退票参考时间：1700RMK TJ AUTH CTU363\" center=\"\" isshared=\"0\" policyweek=\"1,2,3,4,5,6,0\" providerid=\"E0D76E0C-F910-41D9-8834-39884BA8F31C\" weekendworktimef=\"0830\" weekendworktimet=\"2230\" refundworktimeto=\"1700\" refundweekendworktimeto=\"1600\" officeid=\"CTU363\" /><pol event=\"11\" id=\"ffffffff-fff1-4221-2940-201208022134\" rate=\"5.8\" aircom=\"TV\" from=\"XNN\" to=\"CKG/CTU\" flightno=\"\" flighscope=\"0\" routetype=\"1\" policytype=\"BSP-ET\" fromtime=\"2012-8-2 0:00:00\" totime=\"2012-8-31 23:59:59\" applyclass=\"V;G;Q;J;L;K;H;M;B;Y;F;D;Z;P;W;E;T\" printtickfromtime=\"2012-08-02 00:00:00\" printticktotime=\"2012-08-31 23:59:59\" worktimefrom=\"0800\" worktimeto=\"2300\" paytype=\"1\" changerecord=\"0\" automata=\"0\" contype=\"1\" isspecial=\"0\" note=\"退票参考时间：1700RMK TJ AUTH CTU363\" center=\"\" isshared=\"0\" policyweek=\"1,2,3,4,5,6,0\" providerid=\"E0D76E0C-F910-41D9-8834-39884BA8F31C\" weekendworktimef=\"0830\" weekendworktimet=\"2230\" refundworktimeto=\"1700\" refundweekendworktimeto=\"1600\" officeid=\"CTU363\" /></root>";
            #endregion
            policyinfo = policyinfo.Replace("", "");
            StringReader rea = new StringReader(policyinfo);
            DataSet ds = new DataSet();
            XmlTextReader xmlReader = new XmlTextReader(rea);
            ds.ReadXml(xmlReader);

            string insertStr = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["event"].ToString().Trim() != "")
                {
                    if (ds.Tables[0].Rows[i]["event"].ToString().Trim() == "11")//添加政策
                    {
                        try
                        {
                            
                            DeletebookPolicy(connStringSql, "Tb_Ticket_bookPolicy", " 1=1 and PolicyId='" + ds.Tables[0].Rows[i]["id"].ToString().Trim() + "'");
                            Tb_Ticket_BookPolicy bookPolicy = new Tb_Ticket_BookPolicy();
                            bookPolicy.PolicyId = ds.Tables[0].Rows[i]["id"].ToString().Trim();
                            bookPolicy.PolicyState = 1;


                            bookPolicy.FromCity = ds.Tables[0].Rows[i]["from"].ToString().Trim().Replace("/", ",");
                            bookPolicy.ToCity = ds.Tables[0].Rows[i]["to"].ToString().Trim().Replace("/", ",");
                            bookPolicy.Airlines = ds.Tables[0].Rows[i]["aircom"].ToString().Trim();
                            bookPolicy.TripType = Convert.ToInt32(ds.Tables[0].Rows[i]["routetype"].ToString().Trim());
                            bookPolicy.A1 = Convert.ToInt32(ds.Tables[0].Rows[i]["isspecial"].ToString().Trim());
                            
                            if (ds.Tables[0].Rows[i]["flighscope"].ToString().Trim() == "0")//全部
                            {
                                bookPolicy.Flight = "";
                                bookPolicy.NoFlight = "";
                            }
                            if (ds.Tables[0].Rows[i]["flighscope"].ToString().Trim() == "1")//适用航班
                            {
                                bookPolicy.Flight = ds.Tables[0].Rows[i]["flightno"].ToString().Trim();
                                bookPolicy.NoFlight = "";
                            }
                            else if (ds.Tables[0].Rows[i]["flighscope"].ToString().Trim() == "2")//不适用航班
                            {
                                bookPolicy.Flight = "";
                                bookPolicy.NoFlight = ds.Tables[0].Rows[i]["flightno"].ToString().Trim();
                            }


                            bookPolicy.EtcLimit = ds.Tables[0].Rows[i]["policyweek"].ToString().Trim();
                            if (ds.Tables[0].Rows[i]["policytype"].ToString().Trim() == "BSP-ET")
                            {
                                bookPolicy.TicketType = 2;
                            }
                            else if (ds.Tables[0].Rows[i]["policytype"].ToString().Trim() == "B2B-ET")
                            {
                                bookPolicy.TicketType = 1;
                            }
                            else
                            {
                                bookPolicy.TicketType = 0;
                            }
                            string sss = ds.Tables[0].Rows[i]["applyclass"].ToString().Trim();
                            if (!sss.StartsWith("/"))
                            {
                                sss = "/" + sss;
                            }
                            if (!sss.EndsWith("/"))
                            {
                                sss = sss + "/";
                            }
                            bookPolicy.Shipping = sss.Replace(";", "/");

                            decimal ra = decimal.Parse(ds.Tables[0].Rows[i]["rate"].ToString().Trim());
                            bookPolicy.PReturn = decimal.Parse((ra / 100).ToString("f3"));

                            bookPolicy.GTicketTimeE = ds.Tables[0].Rows[i]["printtickfromtime"].ToString().Trim() + "-" + ds.Tables[0].Rows[i]["printticktotime"].ToString().Trim();

                            bookPolicy.EffectiveDate = DateTime.Parse(ds.Tables[0].Rows[i]["fromTime"].ToString().Trim());
                            bookPolicy.ExpiryDate = DateTime.Parse(ds.Tables[0].Rows[i]["toTime"].ToString().Trim());

                            bookPolicy.GYHXNumber = ds.Tables[0].Rows[i]["providerid"].ToString().Trim();
                            bookPolicy.GYPTNumber = ds.Tables[0].Rows[i]["providerid"].ToString().Trim();
                            bookPolicy.Remark = ds.Tables[0].Rows[i]["note"].ToString().Trim();
                            bookPolicy.IsAuGTicket = Convert.ToInt32(ds.Tables[0].Rows[i]["automata"].ToString().Trim());
                            bookPolicy.AddTime = DateTime.Now;
                            bookPolicy.PolicySource = 5;
                            bookPolicy.PolicySourceName = "票盟";
                            bookPolicy.AddCpyName = "";
                            bookPolicy.AddCpyNo = "";
                            string f = ds.Tables[0].Rows[i]["worktimefrom"].ToString().Trim();
                            string t = ds.Tables[0].Rows[i]["worktimeto"].ToString().Trim();
                            if (f.Length == 2)
                            {
                                f = f + "00";
                            }
                            if (t.Length == 2)
                            {
                                t = t + "00";
                            }
                            if (f.IndexOf(":") < 0)
                            {
                                f = f.Substring(0, 2) + ":" + f.Substring(2, 2);
                            }
                            if (t.IndexOf(":") < 0)
                            {
                                t = t.Substring(0, 2) + ":" + t.Substring(2, 2);
                            }
                            bookPolicy.ProviderWorkTime = f + "-" + t; ;
                            DeletebookPolicyNew(connStringSql, InsertbookPolicy(connStringSql, "Tb_Ticket_BookPolicy", bookPolicy));
                            //insertStr = insertStr + InsertbookPolicy(connStringSql, "Tb_Ticket_bookPolicy", bookPolicy);
                            OnErrorNew("添加SQL语句完成（添加政策）PolicyId：" + bookPolicy.PolicyId, false);
                        }
                        catch (Exception ex)
                        {
                            OnErrorNew("插入失败（添加政策），原因：" + ex.Message.ToString(), false);
                        }
                    }
                    if (ds.Tables[0].Rows[i]["event"].ToString().Trim() == "13")//删除政策
                    {
                        try
                        {
                            DeletebookPolicy(connStringSql, "Tb_Ticket_bookPolicy", " 1=1 and PolicyId='" + ds.Tables[0].Rows[i]["id"].ToString().Trim() + "'");
                            OnErrorNew("删除PolicyId完成（删除政策）PolicyId：" + ds.Tables[0].Rows[i]["id"].ToString().Trim(), false);
                        }
                        catch (Exception ex)
                        {
                            OnErrorNew("删除PolicyId失败，原因：" + ex.Message.ToString(), false);
                        }

                    }
                }
            }
            //OnErrorNew("开始执行存储过程：" + DateTime.Now.ToString(), false);
            //string sqlCollect = "exec Policy '" + insertStr + "'";
            //DeletebookPolicyNew(connStringSql, sqlCollect);
            //OnErrorNew("执行存储过程完成：" + DateTime.Now.ToString(), false);
            Response.Write("0");//通知票盟成功
        }
        catch (Exception ex)
        {
            Response.Write("1");//通知票盟失败
            OnErrorNew("执行失败，原因：" + ex.Message.ToString(), false);
        }

    }
    private int DeletebookPolicyNew(string connectionString, string queryString)
    {
        int Rint = 0;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            try
            {
                Rint = command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }

        }
        return Rint;
    }
    private string InsertbookPolicy(string connectionString, string TableName, Tb_Ticket_BookPolicy bookPolicy)
    {
        string queryString = "insert into " + TableName + " (PolicySource,PolicySourceName,PolicyId,PolicyState,FromCity,ToCity,Airlines,TripType,Flight,NoFlight,EtcLimit,TicketType,Shipping,PReturn,GTicketTimeE,EffectiveDate,ExpiryDate,GYHXNumber,InsertDate,UpdateDate,GYPTNumber,IsAuGTicket,Remark,AddTime,AddCpyNo,AddCpyName,ProviderWorkTime,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,A16,A17,A18,A19,A20) values (" + bookPolicy.PolicySource + ",'" + bookPolicy.PolicySourceName + "','" + bookPolicy.PolicyId + "'," + bookPolicy.PolicyState + ",'" + bookPolicy.FromCity + "','" + bookPolicy.ToCity + "','" + bookPolicy.Airlines + "'," + bookPolicy.TripType + ",'" + bookPolicy.Flight + "','" + bookPolicy.NoFlight + "','" + bookPolicy.EtcLimit + "'," + bookPolicy.TicketType + ",'" + bookPolicy.Shipping + "'," + bookPolicy.PReturn + ",'" + bookPolicy.GTicketTimeE + "','" + bookPolicy.EffectiveDate + "','" + bookPolicy.ExpiryDate + "','" + bookPolicy.GYHXNumber + "',getdate(),'" + bookPolicy.UpdateDate + "','" + bookPolicy.GYPTNumber + "'," + bookPolicy.IsAuGTicket + ",'" + bookPolicy.Remark + "','" + bookPolicy.AddTime + "','" + bookPolicy.AddCpyNo + "','" + bookPolicy.AddCpyName + "','" + bookPolicy.ProviderWorkTime + "'," + bookPolicy.A1 + "," + bookPolicy.A2 + "," + bookPolicy.A3 + "," + bookPolicy.A4 + "," + bookPolicy.A5 + "," + bookPolicy.A6 + ",'" + bookPolicy.A7 + "','" + bookPolicy.A8 + "','" + bookPolicy.A9 + "','" + bookPolicy.A10 + "','" + bookPolicy.A11 + "','" + bookPolicy.A12 + "','" + bookPolicy.A13 + "','" + bookPolicy.A14 + "'," + bookPolicy.A15 + "," + bookPolicy.A16 + "," + bookPolicy.A17 + "," + bookPolicy.A18 + ",'" + bookPolicy.A19 + "','" + bookPolicy.A20 + "') ";
        return queryString;
    }
    private string GetValue()
    {
        int nContentLen = (int)Request.ContentLength;
        byte[] buf = new byte[nContentLen];
        int nIndex = 0;
        int nLen = 0;
        while (nLen < nContentLen)
        {
            if (nContentLen - nLen > 512)
            {
                nIndex = Request.InputStream.Read(buf, nLen, 512);
            }
            else
            {
                nIndex = Request.InputStream.Read(buf, nLen, nContentLen - nLen);
            }
            nLen += nIndex;
        }
        if (buf.Length == 0)
        {
            Response.Write("空数据");
            return "";
        }
        string xml = System.Text.Encoding.GetEncoding("utf-8").GetString(buf);
        return xml;
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