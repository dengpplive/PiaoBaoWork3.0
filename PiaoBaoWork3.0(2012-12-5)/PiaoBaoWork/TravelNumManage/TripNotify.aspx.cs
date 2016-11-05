using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.ControlBase;
using PbProject.Model;
using System.Collections.Specialized;
using System.Text;
public partial class TravelNumManage_TripNotify : System.Web.UI.Page
{
    public class Param
    {
        public string OpType = string.Empty;
        public string LoginName = string.Empty;
        public string CompanyName = string.Empty;
        public string TicketNo = string.Empty;
        public string TripNo = string.Empty;
        public string Pnr = string.Empty;
        public string Office = string.Empty;
        public string Msg = string.Empty;
    }
    public Param GetReuest(StringBuilder sbLog)
    {
        Param pm = new Param();
        NameValueCollection NC = HttpContext.Current.Request.Form;
        sbLog.Append("\r\n\r\n行程单同步数据参数:\r\n");
        string Val = "";
        foreach (string key in NC.Keys)
        {
            Val = HttpUtility.UrlDecode(NC[key]);
            sbLog.Append(key + "=" + Val + "\r\n");
            switch (key)
            {
                case "OpType":
                    {
                        pm.OpType = Val;
                        if (string.IsNullOrEmpty(pm.OpType.Trim()))
                        {
                            pm.Msg = "操作类型不能为空！";
                        }
                        break;
                    }
                case "LoginName":
                    {
                        pm.LoginName = Val;
                        if (string.IsNullOrEmpty(pm.LoginName.Trim()))
                        {
                            pm.Msg = "登录账号不能为空！";
                        }
                        break;
                    }
                case "CompanyName":
                    {
                        pm.CompanyName = Val;
                        if (string.IsNullOrEmpty(pm.CompanyName.Trim()))
                        {
                            pm.Msg = "公司名称不能为空！";
                        }
                        break;
                    }
                case "TicketNo":
                    {
                        pm.TicketNo = Val;
                        if (string.IsNullOrEmpty(pm.TicketNo.Trim()))
                        {
                            pm.Msg = "票号不能为空！";
                        }
                        break;
                    }
                case "TripNo":
                    {
                        pm.TripNo = Val;
                        if (string.IsNullOrEmpty(pm.TripNo.Trim()))
                        {
                            pm.Msg = "行程单号不能为空！";
                        }
                        break;
                    }
                case "Pnr":
                    {
                        pm.Pnr = Val;
                        break;
                    }
                case "Office":
                    {
                        pm.Office = Val;
                        break;
                    }
                default:
                    break;
            }
        }
        return pm;
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        //日志
        StringBuilder sbLog = new StringBuilder();
        try
        {
            Param pm = GetReuest(sbLog);
            if (pm.Msg == "")
            {
                string UseCpyNo = string.Empty;
                BaseDataManage Manage = new BaseDataManage();
                string sqlWhere = string.Format(" LoginName='{0}'  ", pm.LoginName);
                List<User_Employees> empList = Manage.CallMethod("User_Employees", "GetList", null, new object[] { sqlWhere }) as List<User_Employees>;
                if (empList != null && empList.Count > 0)
                {
                    User_Employees m_UserEmployees = empList[0];
                    UseCpyNo = m_UserEmployees.CpyNo;
                    User_Company m_UserCompany = null;
                    sqlWhere = string.Format(" UninAllName='{0}' and UninCode='{1}'", pm.CompanyName, m_UserEmployees.CpyNo);
                    List<User_Company> comList = Manage.CallMethod("User_Company", "GetList", null, new object[] { sqlWhere }) as List<User_Company>;
                    if (comList != null && comList.Count > 0)
                    {
                        m_UserCompany = comList[0];
                        UseCpyNo = m_UserCompany.UninCode;
                    }
                }
                sqlWhere = string.Format(" TripNum='{0}' and UseCpyNo='{1}' ", pm.TripNo, UseCpyNo);
                List<Tb_TripDistribution> TripList = Manage.CallMethod("Tb_TripDistribution", "GetList", null, new object[] { sqlWhere }) as List<Tb_TripDistribution>;
                if (TripList != null && TripList.Count > 0)
                {
                    List<string> sqlList = new List<string>();
                    if (pm.OpType == "create")
                    {
                        //创建成功
                        sqlList.Add(string.Format(" update Tb_TripDistribution set TripStatus=9,TicketNum='{0}' where TripNum='{1}' and id='{2}' ", pm.TicketNo, pm.TripNo, TripList[0].id.ToString()));
                    }
                    else if (pm.OpType == "void")
                    {
                        //作废成功
                        sqlList.Add(string.Format(" update Tb_TripDistribution set TripStatus=6 where TripNum='{0}' and id='{1}' ", pm.TripNo, TripList[0].id.ToString()));
                    }
                    if (sqlList.Count > 0)
                    {
                        string err = "";
                        if (Manage.ExecuteSqlTran(sqlList, out err))
                        {
                            sbLog.Append("时间:" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + " 公司编号:" + UseCpyNo + "  行程单号:" + pm.TripNo + "同步成功！\r\n\r\n");
                        }
                        else
                        {
                            sbLog.Append("时间:" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + " 公司编号:" + UseCpyNo + " 行程单号:" + pm.TripNo + "同步失败！\r\n\r\n");
                        }
                    }
                }
                else
                {
                    sbLog.Append("时间:" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + " 公司编号:" + UseCpyNo + " 行程单号:" + pm.TripNo + " 不存在！\r\n\r\n");
                }
            }
            else
            {
                sbLog.Append("时间:" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + " " + pm.Msg + "\r\n\r\n");
            }
        }
        catch (Exception ex)
        {
            sbLog.Append("异常:" + ex.Message);
        }
        finally
        {
            PnrAnalysis.LogText.LogWrite(sbLog.ToString(), "TongBuTrip");
        }
    }

}