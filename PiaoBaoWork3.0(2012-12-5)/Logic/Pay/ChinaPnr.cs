using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Configuration;
using PbProject.Model.PayParam;

namespace PbProject.Logic.Pay
{
    /// <summary>
    /// 汇付天下支付处理
    /// </summary>
    public class ChinaPnr
    {
        #region 参数定义
        /// <summary>
        /// 版本号
        /// </summary>
        public string _Version = "10";
        /// <summary>
        /// 商户号
        /// </summary>
        public string _MerId = "871505";
        /// <summary>
        /// 接口费率
        /// </summary>
        public decimal _Rate = 0.001M;
        /// <summary>
        /// 供应商费率
        /// </summary>
        public decimal _SupplyRate = 0.001M;
        /// <summary>
        /// 同步地址
        /// </summary>
        public string _RetUrl = "";
        /// <summary>
        /// 异步地址
        /// </summary>
        public string _BgRetUrld = "";
        /// <summary>
        /// 签名key文件绝对路径
        /// </summary>
        public string _MerKeyUrl =AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Pay/Key/MerPrK871505.key";
        /// <summary>
        /// 验证key文件绝对路径
        /// </summary>
        public string _PgKeyUrl = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Pay/Key/PgPubk.key";

        #endregion

        #region 公共方法

        /// <summary>
        /// 默认参数
        /// </summary> 
        public ChinaPnr()
        {
            try
            {
                _RetUrl = ConfigurationManager.AppSettings["_ChinaPnrReturnUrl"].ToString();
                _BgRetUrld = ConfigurationManager.AppSettings["_ChinaPnrNotifyUrl"].ToString();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 默认参数
        /// </summary> 
        public ChinaPnr(bool type)
        {

        }
        /// <summary>
        /// 默认参数
        /// </summary>
        /// <param name="merId"></param>
        public ChinaPnr(string merId)
        {
            _MerId = merId;
            _MerKeyUrl = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Pay/Key/MerPrK" + merId + ".key";
        }

        /// <summary>
        /// GET方式提交到指定URL
        /// </summary>
        /// <param name="url">要提交的URL地址包含参数</param> 
        /// <returns>返回请求页面的HTML</returns>
        public string GetUrlData(string url)
        {
            WebClient webclient = new WebClient();
            byte[] pagedate = webclient.DownloadData(url);
            return Encoding.Default.GetString(pagedate);
        }

        /// <summary>
        /// 返回签名字符串
        /// </summary>
        /// <param name="merId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetValue(string merId, string data)
        {
            string chkValue = "";

            CHINAPNRLib.NetpayClientClass netpay = new CHINAPNRLib.NetpayClientClass();

            chkValue = netpay.SignMsg0(merId, _MerKeyUrl, data, data.Length);

            return chkValue;
        }

        /// <summary>
        /// 支付及分润接口
        /// </summary>
        public string Buy(ChinaPnrParam chinaPnrParam)
        {
            string returnFormValue = "";
            try
            {
                #region 参数赋值
		 
                string OrdId = chinaPnrParam.Orderid;
                string OrdAmt = chinaPnrParam.Price;
                string UsrMp = chinaPnrParam.Buyphone;
                string PayUsrId = chinaPnrParam.Buyid;
                string PnrNum = chinaPnrParam.Pnr;
                string DivDetails = chinaPnrParam.Details;
                string MerPriv = chinaPnrParam.Merpriv;

                string url = "http://mas.chinapnr.com/gar/RecvMerchant.do";
                string Version = _Version;
                string CmdId = "Buy";
                string MerId = _MerId;
                string CurCode = "RMB";
                string Pid = "";
                string RetUrl = _RetUrl;
                string GateId = "";
                string OrderType = "";
                string BgRetUrl = _BgRetUrld;

                #endregion

                //加密
                string Data = Version + CmdId + MerId + OrdId + OrdAmt + CurCode + Pid + RetUrl + MerPriv + GateId + UsrMp + DivDetails + OrderType + PayUsrId + PnrNum + BgRetUrl;
                string ChkValue = GetValue(MerId, Data);

                #region 组合数据
                StringBuilder sb = new StringBuilder();
                sb.Append("<form name='BuyForm'  id='BuyForm'  method='post' action='" + url + "'>");
                sb.Append("<input type='hidden' name='Version' value='" + Version + "'/>");
                sb.Append("<input type='hidden' name='CmdId' value='" + CmdId + "'/>");
                sb.Append("<input type='hidden' name='MerId' value='" + MerId + "'/>");
                sb.Append("<input type='hidden' name='OrdId' value='" + OrdId + "'/>");
                sb.Append("<input type='hidden' name='OrdAmt' value='" + OrdAmt + "'/>");
                sb.Append("<input type='hidden' name='CurCode' value='" + CurCode + "'/>");
                sb.Append("<input type='hidden' name='Pid' value='" + Pid + "'/>");
                sb.Append("<input type='hidden' name='RetUrl' value='" + RetUrl + "'/>");
                sb.Append("<input type='hidden' name='MerPriv' value='" + MerPriv + "'/>");
                sb.Append("<input type='hidden' name='GateId' value='" + GateId + "'/>");
                sb.Append("<input type='hidden' name='UsrMp' value='" + UsrMp + "'/>");
                sb.Append("<input type='hidden' name='DivDetails' value='" + DivDetails + "'/>");
                sb.Append("<input type='hidden' name='OrderType' value='" + OrderType + "'/>");
                sb.Append("<input type='hidden' name='PayUsrId' value='" + PayUsrId + "'/>");
                sb.Append("<input type='hidden' name='PnrNum' value='" + PnrNum + "'/>");
                sb.Append("<input type='hidden' name='BgRetUrl' value='" + BgRetUrl + "'/>");
                sb.Append("<input type='hidden' name='ChkValue' value='" + ChkValue + "'/>");
                sb.Append("</form>");
                sb.Append("<script>");
                sb.Append(" document.getElementById(\"BuyForm\").submit();");
                sb.Append("</script>");
                returnFormValue = sb.ToString();

                OnErrorNew("Data:" + Data + " \r\n ChkValue:" + ChkValue + " \r\n sb:" + returnFormValue);
                #endregion 
            }
            catch (Exception)
            {

            }
            return returnFormValue;
        }

        /// <summary>
        /// 结算接口
        /// </summary>
        /// <param name="orderid">订单编号</param>
        /// <returns></returns>
        public string PaymentConfirm(string orderid)
        {
            string url = "http://mas.chinapnr.com/gao/entry.do";
            string Version = _Version;
            string CmdId = "PaymentConfirm";
            string MerId = _MerId;
            string OrdId = orderid;

            string Data = Version + CmdId + MerId + OrdId;
            string ChkValue = GetValue(MerId, Data);

            StringBuilder sb = new StringBuilder();
            sb.Append(url + "?");
            sb.Append("Version=" + Version);
            sb.Append("&CmdId=" + CmdId);
            sb.Append("&MerId=" + MerId);
            sb.Append("&OrdId=" + OrdId);
            sb.Append("&ChkValue=" + ChkValue);

            string Result = GetUrlData(sb.ToString());
            return Result;
        }


        /// <summary>
        /// 退款接口
        /// </summary>
        /// <param name="orderid">订单编号</param>
        /// <param name="oldorderid">原订单编号</param>
        /// <param name="price">退款总金额</param>
        /// <param name="details">退分润数据集</param>
        /// <returns></returns>
        public string Refund(string orderid, string oldorderid, string price, string details)
        {
            string url = "http://mas.chinapnr.com/gao/entry.do";
            string Version = _Version;
            string CmdId = "Refund";
            string MerId = _MerId;
            string DivDetails = details;
            string RefAmt = price;
            string OrdId = orderid;
            string OldOrdId = oldorderid;
            string BgRetUrl = _BgRetUrld;

            string Data = Version + CmdId + MerId + DivDetails + RefAmt + OrdId + OldOrdId + BgRetUrl;
            string ChkValue = GetValue(MerId, Data);

            StringBuilder sb = new StringBuilder();
            sb.Append(url + "?");
            sb.Append("Version=" + Version);
            sb.Append("&CmdId=" + CmdId);
            sb.Append("&MerId=" + MerId);
            sb.Append("&OrdId=" + OrdId);
            sb.Append("&OldOrdId=" + OldOrdId);
            sb.Append("&DivDetails=" + DivDetails);
            sb.Append("&RefAmt=" + RefAmt);
            sb.Append("&BgRetUrl=" + BgRetUrl);
            sb.Append("&ChkValue=" + ChkValue);

            string Result = GetUrlData(sb.ToString());

            OnErrorNew("Data:" + Data + " \r\n ChkValue:" + ChkValue + " \r\n Result:" + Result);

            return Result;
        }

        /// <summary>
        /// 签约接口
        /// </summary>
        /// <param name="userid">签约的账号</param>
        /// <param name="operid">管理员账号</param> 
        /// <returns></returns>
        public string Sign(string userid, string operid)
        {
            string msg = "";
            try
            {
                string url = "http://mas.chinapnr.com/gau/UnifiedServlet";
                string Version = _Version;
                string CmdId = "Sign";
                string MerId = _MerId;
                string UsrId = userid;
                string OperId = operid;
                string MerDate = DateTime.Now.ToString("yyyyMMdd");
                string MerTime = DateTime.Now.ToString("HHmmss");
                string BgRetUrl = _BgRetUrld;

                string Data = Version + CmdId + MerId + UsrId + OperId + MerDate + MerTime + BgRetUrl;
                string ChkValue = GetValue(MerId, Data);

                StringBuilder sb = new StringBuilder();
                sb.Append("<form name='BuyForm'  id='BuyForm'  method='post' action='" + url + "'>");
                sb.Append("<input type='hidden' name='Version' value='" + Version + "'/>");
                sb.Append("<input type='hidden' name='CmdId' value='" + CmdId + "'/>");
                sb.Append("<input type='hidden' name='MerId' value='" + MerId + "'/>");
                sb.Append("<input type='hidden' name='UsrId' value='" + UsrId + "'/>");
                sb.Append("<input type='hidden' name='OperId' value='" + OperId + "'/>");
                sb.Append("<input type='hidden' name='MerDate' value='" + MerDate + "'/>");
                sb.Append("<input type='hidden' name='MerTime' value='" + MerTime + "'/>");
                sb.Append("<input type='hidden' name='BgRetUrl' value='" + BgRetUrl + "'/>");
                sb.Append("<input type='hidden' name='ChkValue' value='" + ChkValue + "'/>");
                sb.Append("</form>");
                sb.Append("<script>");
                sb.Append(" document.getElementById(\"BuyForm\").submit();");
                sb.Append("</script>");

                OnErrorNew("签约接口:" + sb.ToString()); //记录日志

                msg = sb.ToString();
            }
            catch (Exception ex)
            {
                OnErrorNew(ex.ToString());
                msg = ex.ToString();
            }
            return msg;
        }


        /// <summary>
        /// 签约查询接口
        /// </summary>
        /// <param name="userid">签约的账号</param>
        /// <param name="operid">管理员账号</param> 
        /// <returns></returns>
        public bool QuerySign(string userid, string operid)
        {
            string url = "http://mas.chinapnr.com/gau/UnifiedServlet";
            string Version = _Version;
            string CmdId = "QuerySign";
            string MerId = _MerId;
            string UsrId = userid;
            string OperId = operid;
            string MerDate = DateTime.Now.ToString("yyyyMMdd");
            string MerTime = DateTime.Now.ToString("HHmmss");

            string Data = Version + CmdId + MerId + UsrId + OperId + MerDate + MerTime;
            string ChkValue = GetValue(MerId, Data);

            StringBuilder sb = new StringBuilder();
            sb.Append(url + "?");
            sb.Append("Version=" + Version);
            sb.Append("&CmdId=" + CmdId);
            sb.Append("&MerId=" + MerId);
            sb.Append("&UsrId=" + UsrId);
            sb.Append("&OperId=" + OperId);
            sb.Append("&MerDate=" + MerDate);
            sb.Append("&MerTime=" + MerTime);
            sb.Append("&ChkValue=" + ChkValue);

            string Result = GetUrlData(sb.ToString());

            OnErrorNew("签约接口查询:" + sb.ToString()); //记录日志

            if (Result.Contains("成功"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 退款查询接口
        /// </summary> 
        /// <param name="orderid">订单编号</param> 
        /// <returns></returns>
        public string QueryRefundStatus(string orderid)
        {
            //gaq/entry.do
            //gar/entry.co

            string url = "http://mas.chinapnr.com/gaq/entry.do";
            string Version = _Version;
            string CmdId = "QueryRefundStatus";
            string MerId = _MerId;
            string OrdId = orderid;

            string Data = Version + CmdId + MerId + OrdId;
            string ChkValue = GetValue(MerId, Data);

            StringBuilder sb = new StringBuilder();
            sb.Append(url + "?");
            sb.Append("Version=" + Version);
            sb.Append("&CmdId=" + CmdId);
            sb.Append("&MerId=" + MerId);
            sb.Append("&OrdId=" + OrdId);
            sb.Append("&ChkValue=" + ChkValue);

            OnErrorNew("退款查询接口:" + sb.ToString()); //记录日志

            string Result = GetUrlData(sb.ToString());
            return Result;
        }

        /// <summary>
        /// 支付查询接口 
        /// </summary> 
        /// <param name="orderid">订单编号</param> 
        /// <returns></returns>
        public string QueryStatus(string orderid)
        {
            //gaq/entry.do
            //gar/entry.co

            string url = "http://mas.chinapnr.com/gaq/entry.do";
            string Version = _Version;
            string CmdId = "QueryStatus";
            string MerId = _MerId;
            string OrdId = orderid;

            string Data = Version + CmdId + MerId + OrdId;
            string ChkValue = GetValue(MerId, Data);

            StringBuilder sb = new StringBuilder();
            sb.Append(url + "?");
            sb.Append("Version=" + Version);
            sb.Append("&CmdId=" + CmdId);
            sb.Append("&MerId=" + MerId);
            sb.Append("&OrdId=" + OrdId);
            sb.Append("&ChkValue=" + ChkValue);

            OnErrorNew("支付查询接口:" + sb.ToString()); //记录日志

            string Result = GetUrlData(sb.ToString());
            return Result;
        }

        /// <summary>
        /// 账户余额查询接口  
        /// </summary> 
        /// <param name="orderid">订单编号</param> 
        /// <returns></returns>
        public string QueryBalance(string usrId)
        {
            //gaq/entry.do
            //gar/entry.co

            string url = "http://mas.chinapnr.com/gaq/entry.do";
            string Version = _Version;
            string CmdId = "QueryBalance";
            string MerId = _MerId;
            string UsrId = usrId;

            string Data = Version + CmdId + MerId + UsrId;
            string ChkValue = GetValue(MerId, Data);

            StringBuilder sb = new StringBuilder();
            sb.Append(url + "?");
            sb.Append("Version=" + Version);
            sb.Append("&CmdId=" + CmdId);
            sb.Append("&MerId=" + MerId);
            sb.Append("&UsrId=" + UsrId);
            sb.Append("&ChkValue=" + ChkValue);

            //AcctBal是账户余额
            //SepBal是专用余额
            //TmpBal是临时余额
            //AvlBal可用余额
            //LiqBal待结算余额

            OnErrorNew("账户余额查询接口:" + sb.ToString()); //记录日志

            string Result = GetUrlData(sb.ToString());
            return Result;
        }


        #endregion

        #region 机票业务方法

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="id">订单id</param>
        /// <param name="mUser">当前操作员model</param> 
        /// <param name="mCompany">当前公司model</param> 
        public bool ChinaPnrRefund(string id)
        {
            try
            {
                string[] Details = ReturnRefundDetails(int.Parse(id));

                string value = Refund(Details[0], Details[1], Details[2], Details[3]);
                //测试数据
                //string value = "CmdId=RefundRespCode=000000ChkValue=1B1C8FDB7CE9E5689868FAD2D7380C0FC3C7A5744BC30B6582D03670CA5E4567BC1047BC36480A6EA5DB78E2B0DB162EE5C6C19AB928E13732450A89726E9FD0AEDEE90D70E64079CB6A8586DF54E574D20E11AB2590DB1BD20FE5532F5C8D3739906D4517B8BBF6EF9E27769D8D9FFB8A9E0F791BB712A140B5C28C46A9AFF2ErrMsg=成功";

                if (value.Contains("RespCode=000000") && value.Contains("ErrMsg=成功"))
                {
                    try
                    {
                        //退款成功
                    }
                    catch (Exception ex)
                    {

                    }
                    return true;
                }
                else
                {
                    //退款失败
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnErrorNew("退款异常" + ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 构造分润字符串
        /// </summary>
        /// <param name="id">订单id</param> 
        /// <returns></returns>
        public ChinaPnrParam ReturnPayDetails(int id)
        {
            ChinaPnrParam chinaPnrParam = new ChinaPnrParam();

            try
            {
               
            }
            catch (Exception ex)
            {
               
            }
            return chinaPnrParam;
        }

        /// <summary>
        /// 构造退款字符串
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        public string[] ReturnRefundDetails(int id)
        {
            string[] Details = new string[5];

            try
            {

            }
            catch (Exception)
            {

            }
            return Details;
        }

        #endregion

        #region 记录文本日志
        /// <summary>
        /// 记录文本日志
        /// </summary>
        /// <param name="content">记录内容</param>
        private void OnErrorNew(string errContent)
        {
            try
            {
                #region 记录文本日志

                string errlog = "记录时间：" + DateTime.Now.ToString();
                errlog += "\r\n  记录信息:" + errContent;
                errlog += "\r\n----------------------------------------------------------------------------------------------------";
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Logs\\LogicPay\\ChinaPnr\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
                fs.WriteLine(errlog);
                fs.Close();

                #endregion
            }
            catch
            {

            }
        }
        #endregion
    }
}
