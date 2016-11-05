using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Net;
using System.IO;
using PbProject.Model;

namespace PbProject.Logic.PTInterface
{
    /// <summary>
    /// 票宝开放服务接口异步通知
    /// </summary>
    public class PbInterfaceNotify
    {
        public static PbInterfaceNotify instance;
        public static PbInterfaceNotify GetInstance()
        {
            if (instance == null)
                instance = new PbInterfaceNotify();
            return instance;
        }
        public PbInterfaceNotify()
        { }

        /// <summary>
        /// 异步通知出票票号
        /// </summary>
        /// <param name="order">订单 model</param>
        /// <returns></returns>
        public bool NotifyTicketNo(Tb_Ticket_Order order)
        {
            bool bResult = false;

            try
            {
                string orderId = order.OrderId;

                string logMsg = "";

                #region 查询订单，并通知

                //Tb_Ticket_Order order = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(orderId);
                
                if (order.OrderSourceType ==4)   // "接口订单"
                {
                    //  查询通知地址
                   string notifyUrl = "";

                   #region 查询通知地址


                   //IList<User_Company> companyList = (new User_CompanyService()).SelectById(order.OwnerCpyNo);
                   //if (companyList != null && companyList.Count > 0)
                   // notifyUrl = companyList[0].A82;

		            notifyUrl = "";

	               #endregion

                    if (!string.IsNullOrEmpty(notifyUrl))
                    {

                        IList<Tb_Ticket_Passenger> passengerList = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL().GetPasListByOrderID(order.OrderId);

                        if (passengerList != null && passengerList.Count > 0)
                        {
                            string sign = "";
                            string returnOrderId = "";
                            string returnTicketInfomation = "";
                            returnOrderId = orderId;
                            foreach (Tb_Ticket_Passenger dataItem in passengerList)
                            {
                                //乘机人^证件号^票号|乘机人^证件号^票号
                                returnTicketInfomation += string.Format("{0}^{1}^{2}|", dataItem.PassengerName, dataItem.Cid, dataItem.TicketNumber);
                            }
                            returnTicketInfomation = returnTicketInfomation.Length > 0 ? returnTicketInfomation.Substring(0, returnTicketInfomation.Length - 1) : returnTicketInfomation;
                            sign = GetMD5Value(string.Format("{0}{1}F8653E70-D165-8661-C0EB-E645-11F2CD4E", returnOrderId, returnTicketInfomation));
                            notifyUrl += string.Format("?OrderId={0}&TicketInfomation={1}&Sign={2}", returnOrderId, HttpUtility.UrlEncode(returnTicketInfomation), sign);
                            string getResult = GetHttpDataByGet(notifyUrl, Encoding.UTF8);
                            if (string.IsNullOrEmpty(getResult))
                                logMsg = string.Format("通知失败，通知地址：{0}", notifyUrl);
                            else
                            {
                                logMsg = string.Format("通知成功，通知地址：{0}", notifyUrl);
                                bResult = true;
                            }
                        }
                        else
                            logMsg = "通知失败，订单无关联乘客信息。";
                    }
                    else
                        logMsg = "通知失败，未找到订单所在的公司的通知地址";
                }


                #endregion

                #region 记录日志

                if (!string.IsNullOrEmpty(logMsg))
                {
                    logMsg = string.Format("订单号：{0}；信息：{1}", orderId, logMsg);
                    RecordLog(logMsg);
                }

                #endregion


            }
            catch (Exception)
            {

                throw;
            }

            return bResult;
        }

        #region Util
        /// <summary>
        /// 返回MD5加密值
        /// </summary>
        /// <param name="data">被加密数据</param>
        /// <returns></returns>
        public string GetMD5Value(string data)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(data, "MD5");
        }
        /// <summary>
        /// 获取HTTP数据 GET方式
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="encoding">编码，例如：Encoding.UTF8</param>
        /// <returns></returns>
        public string GetHttpDataByGet(string url, Encoding encoding)
        {
            string result = string.Empty;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 60000;
                request.AllowAutoRedirect = false;
                request.Method = "GET";

                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    reader = new StreamReader(response.GetResponseStream(), encoding);
                    result = reader.ReadToEnd();
                }
            }
            catch { }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (reader != null)
                    reader.Close();
                if (request != null)
                    request = null;
            }
            return result;
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logContents">日志内容</param>
        public void RecordLog(string logContents)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("记录时间：" + DateTime.Now.ToString() + "\r\n");
                sb.AppendFormat("      IP：" + HttpContext.Current.Request.UserHostAddress + "\r\n");
                sb.AppendFormat(" Content: " + logContents + "\r\n");
                sb.AppendFormat("----------------------------------------------------------------------------------------------------\r\n\r\n");
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\PbInterfaceNotifyClass\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
                fs.WriteLine(sb.ToString());
                fs.Close();
            }
            catch (Exception) { }
        }
        #endregion
    }
}

