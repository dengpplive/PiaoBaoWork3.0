using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Logic.Order;
using System.Text;
using PbProject.Logic.PID;
public partial class Air_Order_HangProcess : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            this.currentuserid.Value = this.mUser.id.ToString();
        if (mCompany.RoleType > 3)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您无权访问该页面',{op:1});", true);
            return;
        }
        if (!IsPostBack)
        {
            if (Request.QueryString["Id"] != null)
            {
                string Id = Request.QueryString["Id"].ToString();
                string Url = Request.QueryString["Url"].ToString()+"?currentuserid="+this.currentuserid.Value.ToString();
                string Type = Request.QueryString["Type"].ToString();
                ViewState["Id"] = Id;
                ViewState["Url"] = Url;
                ViewState["Type"] = Type;
                if (Type == "1")
                {
                    Suptitle.InnerHtml = "票号解挂处理";
                    btnOk.Text = "解挂";
                }
                else
                {
                    Suptitle.InnerHtml = "票号挂起处理";
                    btnOk.Text = "挂起";
                }
                PageDataBind();
            }
        }
    }
    /// <summary>
    /// 页面信息绑定
    /// </summary>
    private void PageDataBind()
    {
        Tb_Ticket_Order mOrder = null;
        string sqlWhere = string.Format("id='{0}'", ViewState["Id"].ToString());
        List<Tb_Ticket_Order> OrderList = this.baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Order>;
        if (OrderList != null && OrderList.Count > 0)
        {
            mOrder = OrderList[0];
            string OrderId = mOrder.OrderId;
            ViewState["mOrder"] = mOrder;
            if (mOrder.LockLoginName == "")
            {
                lblLockId.Text = "无";
                lblLockTime.Text = "";
            }
            else
            {
                lblLockId.Text = mOrder.LockLoginName;
                lblLockTime.Text = mOrder.LockTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            //查询挂起或者未挂的乘客
            sqlWhere = string.Format(" OrderId='{0}' and Suspended={1}", OrderId, ViewState["Type"].ToString());
            //乘机人信息
            IList<Tb_Ticket_Passenger> PassengerList = this.baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;
            RepPassenger.DataSource = PassengerList;
            RepPassenger.DataBind();
            ViewState["mPassenger"] = PassengerList;

            //订单操作日志信息
            sqlWhere = string.Format(" OrderId='{0}' order by OperTime desc ", OrderId);
            IList<Log_Tb_AirOrder> OrderLogList = this.baseDataManage.CallMethod("Log_Tb_AirOrder", "GetList", null, new object[] { sqlWhere }) as List<Log_Tb_AirOrder>;
            RepOrderLog.DataSource = OrderLogList;
            RepOrderLog.DataBind();
        }

    }

    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(ViewState["Url"].ToString());
    }
    /// <summary>
    /// 挂起解挂
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        Process();
    }
    /// <summary>
    /// 页面数据显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="objArr"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] objArr)
    {
        string result = "";
        if (type == 0)//乘客类型
        {
            if (PasAndCardTypeList == null)
            {
                PasAndCardTypeList = GetPasAndCardDic();
            }
            if (objArr != null && objArr.Length == 1)
            {
                string PasChildID = objArr[0].ToString();
                var v = (from Bd_Base_Dictionary d in PasAndCardTypeList
                         where d.ParentID == 6 && d.ChildID.ToString() == PasChildID
                         select d).ToList<Bd_Base_Dictionary>();
                if (v.Count > 0)
                {
                    result = v[0].ChildName;
                }
            }
        }
        else if (type == 1)//乘客证件类型
        {
            if (PasAndCardTypeList == null)
            {
                PasAndCardTypeList = GetPasAndCardDic();
            }
            if (objArr != null && objArr.Length == 1)
            {
                string PasChildID = objArr[0].ToString();
                var v = (from Bd_Base_Dictionary d in PasAndCardTypeList
                         where d.ParentID == 7 && d.ChildID.ToString() == PasChildID
                         select d).ToList<Bd_Base_Dictionary>();
                if (v.Count > 0)
                {
                    result = v[0].ChildName;
                }
            }
        }
        else if (type == 2)//乘客票号挂起状态
        {
            if (objArr != null && objArr.Length == 1)
            {
                if (objArr[0].ToString() == "1" || objArr[0].ToString().ToUpper() == "TRUE")
                {
                    result = "<font class=\"red\">已挂</font>";
                }
                else
                {
                    result = "<font class=\"green\">未挂</font>";
                }
            }
        }
        return result;
    }
    List<Bd_Base_Dictionary> PasAndCardTypeList = null;
    public List<Bd_Base_Dictionary> GetPasAndCardDic()
    {
        string sqlWhere = " parentid in(6,7) order by ChildID";
        PasAndCardTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        return PasAndCardTypeList;
    }
    /// <summary>
    /// 票号挂起解挂操作
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pasList"></param>
    /// <param name="CpyNo"></param>
    /// <returns></returns>
    public void TicketSupLock(string type, List<Tb_Ticket_Passenger> pasList, out string ErrMsg)
    {
        ErrMsg = "";
        try
        {
            //日志
            StringBuilder sbLog = new StringBuilder();
            Tb_Ticket_Order mOrder = ViewState["mOrder"] as Tb_Ticket_Order;
            if (mOrder == null)
            {
                ErrMsg = "订单不存在！";
            }
            if (ErrMsg == "")
            {
                string CpyNo = mOrder.OwnerCpyNo;
                string Office = mOrder.Office;
                string PrintOffice = mOrder.PrintOffice;
                //挂起还是解挂
                int ticketType = type == "0" ? 1 : 2;
                PbProject.Model.ConfigParam config = null;
                if (mCompany.RoleType == 1)
                {
                    List<Bd_Base_Parameters> baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo=Left('" + CpyNo + "',12)" }) as List<Bd_Base_Parameters>;
                    config = PbProject.Logic.ControlBase.Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
                }
                if (this.configparam != null)
                {
                    config = this.configparam;
                }
                Tb_Ticket_OrderBLL ManageBLL = new Tb_Ticket_OrderBLL();
                List<string> listIds = new List<string>();
                //扩展参数
                ParamEx pe = new ParamEx();
                pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                PbProject.Logic.PID.SendInsManage SendManage = new PbProject.Logic.PID.SendInsManage(mUser.LoginName, mCompany.UninCode,pe, config);
                for (int i = 0; i < pasList.Count; i++)
                {
                    if (pasList[i].TicketNumber.Trim() != "")
                    {
                        //发送指令
                        if (!SendManage.TicketNumberLock(ticketType, pasList[i].TicketNumber, Office, out ErrMsg))
                        {
                            sbLog.Append("<br />乘客" + pasList[i].PassengerName + "机票" + pasList[i].TicketNumber + (type == "1" ? "解挂" : "挂起") + "失败原因:" + ErrMsg);
                        }
                        else
                        {
                            pasList[i].Suspended = ticketType == 1 ? true : false;
                            sbLog.Append("<br />乘客" + pasList[i].PassengerName + "机票" + pasList[i].TicketNumber + (type == "1" ? "解挂" : "挂起") + "成功，");
                        }
                    }
                    else
                    {
                        sbLog.Append("<br />乘客" + pasList[i].PassengerName + "机票" + pasList[i].TicketNumber + (type == "1" ? "解挂" : "挂起") + "失败原因:票号不能为空");
                    }
                }
                if (sbLog.ToString().Length > 0)
                {
                    ErrMsg = sbLog.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ErrMsg + ex.Message;
        }
    }

    /// <summary>
    /// 挂起解挂处理
    /// </summary>
    private void Process()
    {
        IList<Tb_Ticket_Passenger> mPassenger = ViewState["mPassenger"] as IList<Tb_Ticket_Passenger>;
        Tb_Ticket_Order mOrder = ViewState["mOrder"] as Tb_Ticket_Order;
        if (mOrder == null)
        {
            return;
        }
        //操作日志
        StringBuilder sbLog = new StringBuilder();
        List<Tb_Ticket_Passenger> SelPasList = new List<Tb_Ticket_Passenger>();
        List<string> SelPasIdsList = new List<string>();
        sbLog.Append("对乘客 ");
        for (int i = 0; i < RepPassenger.Items.Count; i++)
        {
            string id = (RepPassenger.Items[i].FindControl("hid_Id") as System.Web.UI.HtmlControls.HtmlInputHidden).Value;
            bool isOk = ((CheckBox)RepPassenger.Items[i].FindControl("cboSelect")).Checked;

            for (int j = 0; j < mPassenger.Count; j++)
            {
                if (id == mPassenger[j].id.ToString())
                {
                    if (isOk)
                    {
                        SelPasList.Add(mPassenger[i]);
                        SelPasIdsList.Add("'" + mPassenger[i].id + "'");
                        sbLog.Append(mPassenger[i].PassengerName + ",");
                    }
                }
            }
        }
        //没有选中乘客
        if (SelPasIdsList.Count == 0)
        {
            return;
        }
        string opLog = "";
        //解挂
        if (ViewState["Type"].ToString() == "1")
        {
            sbLog.Append(" 进行机票解挂");
            opLog = "机票解挂";
        }
        //挂起
        else
        {
            sbLog.Append(" 进行机票挂起");
            opLog = "机票挂起";
        }
        string ErrMsg = "";
        TicketSupLock(ViewState["Type"].ToString(), SelPasList, out ErrMsg);
        sbLog.Append(ErrMsg);
        //修改SQL
        List<string> sqlList = new List<string>();
        foreach (Tb_Ticket_Passenger pas in SelPasList)
        {
            //乘客
            sqlList.Add(string.Format(" update Tb_Ticket_Passenger set Suspended={0} where id ='{1}'", pas.Suspended ? "1" : "0", pas.id.ToString()));
        }

        //日志信息   
        Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
        OrderLog.id = Guid.NewGuid();
        OrderLog.OperContent = sbLog.ToString();
        OrderLog.OperLoginName = mUser.LoginName;
        OrderLog.OperTime = DateTime.Now;
        OrderLog.OperType = opLog;
        OrderLog.OperUserName = mUser.UserName;
        OrderLog.OrderId = mOrder.OrderId;
        OrderLog.WatchType = mCompany.RoleType;
        OrderLog.CpyName = mCompany.UninAllName;
        OrderLog.CpyNo = mCompany.UninCode;
        OrderLog.CpyType = mCompany.RoleType;
        OrderLog.WatchType = mCompany.RoleType;
        sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog));//3.添加订单日志
        //修改数据库
        bool IsSuc = this.baseDataManage.ExecuteSqlTran(sqlList, out ErrMsg);
        string url = ViewState["Url"].ToString();
        if (IsSuc)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('操作成功!',{op:1,url:'" + url + "'});", true);
            PageDataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('操作失败!',{op:0,url:'" + url + "'});", true);
        }
    }
}