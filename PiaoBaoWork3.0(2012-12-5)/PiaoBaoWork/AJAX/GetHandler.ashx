<%@ WebHandler Language="C#" Class="GetHandler" %>
using System;
using System.Data;
using System.Web;
using System.Web.SessionState;
using PbProject.Logic.ControlBase;
using System.Text;
using PbProject.Model;
using System.IO;
using System.Collections.Generic;
using DataBase.Data;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.Logic.Order;
using PbProject.Logic.Pay;
using PbProject.WebCommon.Utility;
using PbProject.Dal.Mapping;
using PnrAnalysis;
public class GetHandler : HttpHandle
{

    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Ajax_Context.Response.ContentType = "text/plain";
        Ajax_Context.Response.Clear();
        Ajax_Context.Response.Write(result);
        Ajax_Context.Response.Flush();
        Ajax_Context.Response.End();
    }
    /// <summary>
    /// 获取网站url根目录
    /// </summary>
    /// <returns></returns>
    public string getRootURL()
    {
        string AppPath = "";
        HttpContext HttpCurrent = HttpContext.Current;
        HttpRequest Req;
        if (HttpCurrent != null)
        {
            Req = HttpCurrent.Request;
            string UrlAuthority = Req.Url.GetLeftPart(UriPartial.Authority);
            if (Req.ApplicationPath == null || Req.ApplicationPath == "/")
            {
                AppPath = UrlAuthority;
            }
            else
            {
                AppPath = UrlAuthority + Req.ApplicationPath;
            }
            if (!AppPath.EndsWith("/"))
            {
                AppPath += "/";
            }
        }
        return AppPath;
    }
    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name">请求key名称</param>
    /// <param name="DefaultVal">数据默认值</param>   
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Ajax_Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Ajax_Request[Name].ToString(), System.Text.Encoding.Default).Replace("'", "");
        }
        return DefaultVal;
    }

    /// <summary>
    /// 请求数据处理
    /// </summary>
    /// <param name="context"></param>
    /// <summary>  
    /// </summary>
    public override void DoHandleWork()
    {
        string result = ProcessHandler(Context);
        //编码
        OutPut(escape(result));
    }

    public class ReturnData
    {
        public User_Employees m_Employees = null;
        public User_Company m_Company = null;
        public User_Employees m_YeWuYan = null;
    }
    /// <summary>
    /// 数据处理
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public string ProcessHandler(HttpContext context)
    {
        string result = "#######";
        string OpName = GetVal("OpName", "");
        switch (OpName)
        {
            case "GetUser"://获取用户和公司信息
                {
                    result = GetUserInfo();
                }
                break;
            case "Prompt"://订单提醒
                {
                    result = GetOrderPrompt();
                }
                break;
            case "Send":
                {
                    result = GetData();
                }
                break;
            case "UpdateTripStatus"://修改行程单状态
                {
                    result = UpdateTripStatus();
                }
                break;
            case "OpenTicket"://Open票扫描
                {
                    result = OpenTicketScan();
                }
                break;
            case "GetFlow"://获取指令流量
                {
                    result = GetFlow();
                }
                break;
            case "GetTopPoint"://获取政策最高点数
                {
                    result = GetTopPoint();
                }
                break;
            default:
                break;
        }
        return result;
    }
    //获取政策最优点数
    public string GetTopPoint()
    {
        string result = "#######";
        StringBuilder sbResult = new StringBuilder();

        //政策种类  1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
        string PageType = GetVal("PageType", "");
        //航空公司
        string CarryCode = GetVal("CarryCode", "");
        //出发城市
        string fromCityCode = GetVal("FC", "");
        //中转城市
        string middleCityCode = GetVal("MC", "");
        //到达城市
        string toCityCode = GetVal("TC", "");
        //行程类型 1.单程，2.往返/单程，3.往返，4.中转联程
        string TravelType = GetVal("TravelType", "");
        //政策类型
        string PolicyType = GetVal("PolicyType", "");
        //政策返点
        string DownPoint = GetVal("DownPoint", "0");

        StringBuilder sbWhere = new StringBuilder(" 1=1 ");
        if (PolicyType != "" && PolicyType != "0")
        {
            sbWhere.AppendFormat(" and PolicyType={0} ", PolicyType.ToUpper());
        }
        if (TravelType != "" && TravelType != "0")
        {
            sbWhere.AppendFormat(" and TravelType={0} ", TravelType.ToUpper());
        }
        if (PageType == "1")
        {
            sbWhere.Append(" and PolicyKind=1 and AuditType=1 ");
        }
        else if (PageType == "2")
        {
            sbWhere.Append(" and PolicyKind=2 and AuditType=1 ");
        }

        if (!string.IsNullOrEmpty(CarryCode))
        {
            sbWhere.AppendFormat(" and CarryCode='/{0}/' ", CarryCode.ToUpper());
        }
        if (!string.IsNullOrEmpty(fromCityCode))
        {
            sbWhere.AppendFormat(" and StartCityNameCode like '%{0}%' ", fromCityCode.ToUpper());
        }
        if (!string.IsNullOrEmpty(middleCityCode) && TravelType == "4")
        {
            sbWhere.AppendFormat(" and MiddleCityNameCode like '%{0}%' ", middleCityCode.ToUpper());
        }
        if (!string.IsNullOrEmpty(toCityCode))
        {
            sbWhere.AppendFormat(" and TargetCityNameCode like '%{0}%' ", toCityCode.ToUpper());
        }

        if ("123".Contains(PageType))
        {
            sbWhere.Append(" order by DownPoint desc");
            List<Tb_Ticket_Policy> pList = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { " top 1 * ", sbWhere.ToString() }) as List<Tb_Ticket_Policy>;
            if (pList != null && pList.Count > 0)
            {
                sbResult.Append(JsonHelper.ObjToJson<Tb_Ticket_Policy>(pList[0]));
            }
        }
        else
        {
            sbWhere.Append(" order by DownRebate desc");
            List<Tb_Ticket_UGroupPolicy> pList = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "GetList", null, new object[] { " top 1 * ", sbWhere.ToString() }) as List<Tb_Ticket_UGroupPolicy>;
            if (pList != null && pList.Count > 0)
            {
                sbResult.Append(JsonHelper.ObjToJson<Tb_Ticket_UGroupPolicy>(pList[0]));
            }
        }
        if (sbResult.ToString() != "")
        {
            result = "#######" + sbResult.ToString();
        }
        return result;
    }

    //获取配置流量 指令条数
    public string GetFlow()
    {
        string result = "#######";
        string StartDate = GetVal("StartDate", "");
        if (StartDate == "")
        {
            StartDate = "1900-01-01";
        }
        string EndDate = GetVal("EndDate", "");
        if (EndDate == "")
        {
            EndDate = System.DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
        }
        else
        {
            EndDate = EndDate + " 23:59:59";
        }
        if (DateTime.Compare(DateTime.Parse(StartDate), DateTime.Parse(EndDate)) > 0)
        {
            return result = "#######日期范围错误";
        }
        string Office = GetVal("Office", "");
        if (Office != "")
        {
            HashObject hashObj = new HashObject();
            hashObj.Add("startDate", StartDate);
            hashObj.Add("endDate", EndDate);
            hashObj.Add("office", Office);
            DataTable table = this.baseDataManage.EexcProc("GetConfigFlow", hashObj);
            if (table != null && table.Rows.Count > 0)
            {
                DataRow[] drs = table.Select("UserAccount='总条数'");
                if (drs != null && drs.Length > 0)
                {
                    DataRow dr = drs[0];
                    result = "#######" + (dr["office"] != DBNull.Value ? dr["office"].ToString() : "") + "|" + (dr["CNum"] != DBNull.Value ? dr["CNum"].ToString() : "");
                }
                else
                {
                    result = "#######无";
                }
            }
        }
        else
        {
            result = "#######Office不能为空";
        }
        return result;
    }

    public string UpdateTripStatus()
    {
        string result = "#######";
        string Ids = GetVal("Ids", "");
        string RoleType = GetVal("RoleType", "");
        string TpStatus = GetVal("TpStatus", "");
        string TicketNum = GetVal("TicketNum", "");
        string[] strArr = Ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        List<string> lstIds = new List<string>();
        if (strArr.Length > 0)
        {
            lstIds.AddRange(strArr);
        }
        if (RoleType == "1" && TpStatus != "" && Ids.Length > 0)
        {
            string updateFileds = string.Format(" TripStatus={0}", TpStatus);
            if (TicketNum != "")
            {
                updateFileds += string.Format(",TicketNum='{0}' ", TicketNum);
            }
            bool IsSuc = (bool)this.baseDataManage.CallMethod("Tb_TripDistribution", "UpdateByIds", null, new object[] { lstIds, updateFileds });
            if (IsSuc)
            {
                result = "#######修改成功";
            }
            else
            {
                result = "#######修改失败";
            }
        }
        return result;
    }

    public string GetData()
    {
        string result = "#######";
        //公司编号
        string CpyNo = GetVal("CpyNo", "");
        //Office
        string Office = GetVal("Office", "");
        //发送指令
        string SendIns = GetVal("SendIns", "");

        //票号
        string TicketNumber = GetVal("TicketNumber", "");
        string ddlType = GetVal("SelScan", "0");


        //指令类型 0所有 1提取票号 2提取编码 3.Open票
        string InsType = GetVal("InsType", "");
        string recvData = "";
        if (mCompany == null || mUser == null)
        {
            result = "#######网页已过期请重新登录！";
        }
        if (CpyNo.Trim().Length >= 12)
        {
            CpyNo = CpyNo.Substring(0, 12);
        }
        List<Bd_Base_Parameters> baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + CpyNo + "'" }) as List<Bd_Base_Parameters>;
        string strKongZhiXiTong = this.KongZhiXiTong;
        PbProject.Model.ConfigParam config = PbProject.Logic.ControlBase.Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
        if (mCompany.RoleType == 1)
        {
            baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + CpyNo + "'" }) as List<Bd_Base_Parameters>;
            config = PbProject.Logic.ControlBase.Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
            strKongZhiXiTong = BaseParams.getParams(baseParamList).KongZhiXiTong;
        }
        //扩展参数
        PbProject.Logic.PID.ParamEx pe = new PbProject.Logic.PID.ParamEx();
        pe.UsePIDChannel = strKongZhiXiTong != null && strKongZhiXiTong.Contains("|48|") ? 2 : 0;
        PbProject.Logic.PID.SendInsManage SendManage = new PbProject.Logic.PID.SendInsManage(mUser.LoginName, mCompany.UninCode, pe, config);
        int Port = 0;
        if (int.TryParse(config.WhiteScreenPort, out Port))
        {
            Office = string.IsNullOrEmpty(Office) ? config.Office.Split('^')[0] : Office;
            if (InsType == "2")
            {
                recvData = SendManage.Send(SendIns, ref Office, 2);
            }
            else if (InsType == "3")
            {
                SendIns = string.Format("DETR:TN/{0}", TicketNumber);
                if (TicketNumber.Trim() != "")
                {
                    recvData = SendManage.Send(SendIns, ref Office, 0);
                }
            }
            else
            {
                recvData = SendManage.Send(SendIns, ref Office, 0);
            }
            if (InsType == "3")//open票状态
            {
                PnrAnalysis.FormatPNR format = new FormatPNR();
                string TicketStatus = format.GetTicketStatus(recvData);
                if (ddlType == "0")
                {
                    if (TicketStatus.Contains("OPEN FOR USE"))
                    {
                        recvData = TicketNumber + "|★未使用的有效客票【" + TicketStatus + "】";
                    }
                    else
                    {
                        recvData = TicketNumber + "|不存在或已使用的无效客票【" + TicketStatus + "】";
                    }
                }
                else if (ddlType == "1")
                {
                    if (TicketStatus.Contains("OPEN FOR USE"))
                    {
                        recvData = TicketNumber + "|★未使用的有效客票【" + TicketStatus + "】";
                    }
                }
                else
                {
                    if (!TicketStatus.Contains("OPEN FOR USE"))
                    {
                        recvData = TicketNumber + "|不存在或已使用的无效客票【" + TicketStatus + "】";
                    }
                }
            }
            result = "#######" + recvData;
        }
        else
        {
            result = "#######系统配置参数未设置！";
        }
        return result;
    }

    /// <summary>
    /// 获取用户和公司信息
    /// </summary>
    /// <returns></returns>
    public string GetUserInfo()
    {
        string result = "#######";
        string OrderId = GetVal("OrderId", "");
        //User_Employees
        //User_Company
        if (!string.IsNullOrEmpty(OrderId))
        {
            DataTable[] tableArr = this.baseDataManage.GetUserInfoByOrderId(OrderId);
            if (tableArr != null && tableArr.Length >= 2)
            {
                ReturnData data = new ReturnData();
                if (tableArr[0].Rows.Count > 0)
                {
                    data.m_Employees = MappingHelper<User_Employees>.FillModel(tableArr[0].Rows[0]);
                }
                if (tableArr[1].Rows.Count > 0)
                {
                    data.m_Company = MappingHelper<User_Company>.FillModel(tableArr[1].Rows[0]);
                }
                if (tableArr[2].Rows.Count > 0)
                {
                    data.m_YeWuYan = MappingHelper<User_Employees>.FillModel(tableArr[2].Rows[0]);
                }
                string strdata = JsonHelper.ObjToJson<ReturnData>(data);
                result = "成功#######" + strdata;
            }
            else
            {
                result = "获取失败！#######";
            }
        }
        else
        {
            result = "订单号不能为空！#######";
        }
        return result;
    }


    /// <summary>
    /// 获取订单提醒数据
    /// </summary>
    /// <returns></returns>
    public string GetOrderPrompt()
    {
        StringBuilder sbPromptData = new StringBuilder();
        string result = "#######";
        string CpyNo = GetVal("CpyNo", "");
        string RoleType = GetVal("RoleType", "");
        string CurUrl = getRootURL();

        HashObject hash = new HashObject();
        hash.Add("CpyNo", CpyNo);
        DataTable table = this.baseDataManage.EexcProc("GetOrderPrompt", hash);
        int Num = 0;
        if (table != null && table.Rows.Count > 0)
        {
            DataRow dr = table.Rows[0];
            string param = "&currentuserid=" + mUser.id.ToString();
            if (dr["待出票订单数"] != DBNull.Value && dr["待出票订单数"].ToString() != "0")
            {
                //待出票订单数
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderProcessList.aspx?prompt=1" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["待出票订单数"].ToString() + "</strong></a>张机票等待出票 </td></tr>");
                Num++;
            }
            if (dr["申请改签订单数"] != DBNull.Value && dr["申请改签订单数"].ToString() != "0")
            {
                //申请改签订单数
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderTGQList.aspx?prompt=4" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["申请改签订单数"].ToString() + "</strong></a>张机票申请改签 </td></tr>");
                Num++;
            }
            if (dr["申请退票订单数"] != DBNull.Value && dr["申请退票订单数"].ToString() != "0")
            {
                //申请退票订单数
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderTGQList.aspx?prompt=2" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["申请退票订单数"].ToString() + "</strong></a>张机票申请退票 </td></tr>");
                Num++;
            }
            if (dr["申请废票订单数"] != DBNull.Value && dr["申请废票订单数"].ToString() != "0")
            {
                //申请废票订单数
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderTGQList.aspx?prompt=3" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["申请废票订单数"].ToString() + "</strong></a>张机票申请废票 </td></tr>");
                Num++;
            }
            if (dr["异地退废改签订单数"] != DBNull.Value && dr["异地退废改签订单数"].ToString() != "0")
            {
                //异地退废改签订单数
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderTGQList.aspx?prompt=8" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["异地退废改签订单数"].ToString() + "</strong></a>张异地退废改签订单</td></tr>");
                Num++;
            }
            if (dr["退款中的订单"] != DBNull.Value && dr["退款中的订单"].ToString() != "0")
            {
                //退款中的订单
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderTGQList.aspx?prompt=9" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["退款中的订单"].ToString() + "</strong></a>张退款中的订单</td></tr>");
                Num++;
            }

            if (dr["待收银订单数"] != DBNull.Value && dr["待收银订单数"].ToString() != "0")
            {
                //显示数据
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderCashierList.aspx?prompt=1" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["待收银订单数"].ToString() + "</strong></a>张待收银订单</td></tr>");
                Num++;
            }
            if (dr["审核中的订单数"] != DBNull.Value && dr["审核中的订单数"].ToString() != "0")
            {
                //审核中的订单数
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderTGQList.aspx?prompt=10" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["审核中的订单数"].ToString() + "</strong></a>张审核中的订单</td></tr>");
                Num++;
            }
            if (dr["审核通过待退款"] != DBNull.Value && dr["审核通过待退款"].ToString() != "0")
            {
                //审核通过待退款
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/OrderTGQList.aspx?prompt=5" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["审核通过待退款"].ToString() + "</strong></a>张审核通过，待退款订单</td></tr>");
                Num++;
            }
            if (dr["线下订单申请"] != DBNull.Value && dr["线下订单申请"].ToString() != "0")
            {
                //线下订单申请
                sbPromptData.Append("<tr><td align='left' height='28px' ><span style='font-size:12px;'>已有<a href=\"" + CurUrl + "Order/LineOrderProcess.aspx?prompt=5" + param + "\"  target=\"ALLFrame\" style=\" text-decoration:none;\"><strong style=\"color:red;font-size:20px;\">" + dr["线下订单申请"].ToString() + "</strong></a>张线下订单,等待处理</td></tr>");
                Num++;
            }
        }
        StringBuilder PromptUI = new StringBuilder();
        if (sbPromptData.ToString() != "")
        {
            int x1 = 224;
            int x2 = 230;
            x2 = Num * 28;
            x1 = Num * 28;
            if (x2 < 150)
            {
                x2 = 150;
                x1 = 170;
            }
            PromptUI.Append("<div id=\"PopupWin1\" style=\"background:#eef4f7; border-right: 1px solid #e3f3ff;");
            PromptUI.Append("border-bottom: 1px solid #e3f3ff; border-left: 1px solid #e3f3ff; border-top: 1px solid #e3f3ff;");
            PromptUI.Append("position: absolute; z-index: 9999; width: 203px; height: " + x1.ToString() + "px; right: 15px; bottom:5px;position: fixed;");
            PromptUI.Append(" bottom: 15px;\" onselectstart=\"return false;\">");
            PromptUI.Append("<div id=\"PopupWin1_header\" style=\"cursor: default; position: absolute; left: 2px;");
            PromptUI.Append("width: 194px; top: 2px; height: 14px; filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0,");
            PromptUI.Append("StartColorStr='#eef4f7', EndColorStr='#eef4f7'); font: 12px arial,sans-serif;");
            PromptUI.Append(" color:black; text-decoration: none;\">");
            PromptUI.Append("<span id=\"PopupWin1titleEl\">温馨提示：</span><span id=\"span_Close\" style=\"position: absolute; right: 0px;");
            PromptUI.Append("top: 0px; cursor: pointer; color: #728EB8; font: bold 12px arial,sans-serif;");
            PromptUI.Append("position: absolute; right: 3px;\"  ");
            PromptUI.Append("onmousedown=\"event.cancelBubble=true;\"onmouseover=\"style.color='#455690';\" onmouseout=\"style.color='#728EB8';\">X</span></div>");
            PromptUI.Append(" <div id=\"PopupWin1_content\" onmousedown=\"event.cancelBubble=true;\" style=\"border-left: 1px solid #728EB8;");
            PromptUI.Append("border-top: 1px solid #728EB8; border-bottom: 1px solid #B9C9EF; border-right: 1px solid #B9C9EF;");
            PromptUI.Append(" background: #E0FFFF; padding: 2px; overflow: hidden; text-align: center; filter: progid:DXImageTransform.Microsoft.Gradient(GradientType=0,");
            PromptUI.Append("StartColorStr='#f8fdff', EndColorStr='#f8fdff'); position: absolute; left: 2px;");
            PromptUI.Append(" width: 194px; top: 20px; height: " + x2.ToString() + "px;\">");
            PromptUI.Append("<table>");
            PromptUI.Append(sbPromptData.ToString());
            PromptUI.Append("</table>");
            PromptUI.Append("</table></div>");
            PromptUI.Append("</div>");
        }
        result = "#######" + PromptUI.ToString();
        return result;
    }

    /// <summary>
    /// Open票扫描
    /// </summary>
    /// <returns></returns>
    public string OpenTicketScan()
    {
        string result = GetData();
        return result;
    }
}