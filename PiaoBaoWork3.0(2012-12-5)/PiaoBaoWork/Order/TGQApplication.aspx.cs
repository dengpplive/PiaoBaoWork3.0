using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;
using PbProject.Model;
using PbProject.Logic.PID;
using DataBase.Data;
using PbProject.WebCommon.Utility;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.Logic.Order;

/// <summary>
/// 退改签申请处理
/// </summary>
public partial class Order_TGQApplication : BasePage
{

    #region 属性
    private string strKongZhiXiTong = string.Empty;
    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string NewKongZhiXiTong
    {
        get
        {
            return strKongZhiXiTong;
        }
        set
        {
            strKongZhiXiTong = value;
        }
    }
    private string Escape(string s)
    {
        StringBuilder sb = new StringBuilder();
        byte[] byteArr = System.Text.Encoding.Unicode.GetBytes(s);

        for (int i = 0; i < byteArr.Length; i += 2)
        {
            sb.Append("%u");
            sb.Append(byteArr[i + 1].ToString("X2"));//把字节转换为十六进制的字符串表现形式
            sb.Append(byteArr[i].ToString("X2"));
        }
        return sb.ToString();
    }
    #endregion

    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
                {
                    ViewState["Id"] = Request.QueryString["Id"].ToString();
                    PageDataBind();
                }
                if (Request.QueryString["Url"] != null && Request.QueryString["Url"].ToString() != "")
                {
                    //返回Url
                    Hid_GoUrl.Value = Request.QueryString["Url"].ToString() + "?currentuserid=" + this.mUser.id.ToString(); //返回
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 页面信息绑定
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            string sqlWhere = " id='" + ViewState["Id"].ToString() + "' ";

            List<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            Tb_Ticket_Order mOrder = OrderList != null && OrderList.Count > 0 ? OrderList[0] : null;

            if (mOrder != null)
            {
                sqlWhere = " OrderId='" + mOrder.OrderId + "' ";

                #region 订单信息

                //订单信息
                lblInPayNo.Text = mOrder.InPayNo;
                lblLockId.Text = mOrder.LockLoginName;
                lblLockTime.Text = mOrder.LockTime.ToString("yyyy-MM-dd HH:mm:ss");
                lblOrderId.Text = mOrder.OrderId;

                //lblOrderSourceType.Text = mOrder.OrderSourceType.ToString();
                //lblOrderStatusCode.Text = mOrder.OrderStatusCode.ToString();
                lblOrderSourceType.Text = GetDictionaryName("33", mOrder.OrderSourceType.ToString());
                lblOrderStatusCode.Text = GetDictionaryName("1", mOrder.OrderStatusCode.ToString());

                lblPayMoney.Text = mOrder.PayMoney.ToString("F2");
                lblPayNo.Text = mOrder.PayNo;
                lblPayStatus.Text = (mOrder.PayStatus == 1) ? "已付" : "未付";

                //lblPayWay.Text = mOrder.PayWay.ToString();
                lblPayWay.Text = GetDictionaryName("4", mOrder.PayWay.ToString());
                lblPNR.Text = mOrder.PNR;
                lblPolicyRemark.Text = mOrder.PolicyRemark;
                //lblPolicySource.Text = GetDictionaryName("24", mOrder.PolicySource.ToString());
                lblPolicyPoint.Text = mOrder.ReturnPoint + "/" + mOrder.ReturnMoney;


                lblCreateTime.Text = mOrder.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                lblPayTime.Text = (mOrder.PayTime.ToString("yyyy-MM-dd") == "1900-01-01") ? "" : mOrder.PayTime.ToString("yyyy-MM-dd HH:mm:ss");
                lblCPTime.Text = mOrder.CPTime.ToString("yyyy-MM-dd HH:mm:ss");


                #endregion

                #region 乘机人信息

                List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;

                if (PassengerList != null && PassengerList.Count > 0)
                {
                    List<Tb_Ticket_Passenger> IsShowAppBtn = PassengerList.FindAll(delegate(Tb_Ticket_Passenger pas)
                      {
                          return !pas.IsBack;
                      });
                    if (IsShowAppBtn == null || IsShowAppBtn.Count == 0)
                    {
                        //隐藏申请按钮
                        spanApplay.Visible = false;
                    }
                    RepPassenger.DataSource = PassengerList;
                    RepPassenger.DataBind();
                }

                #endregion

                #region 行程信息

                //现在
                List<Tb_Ticket_SkyWay> SkyWayList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { sqlWhere + " order by FromDate " }) as List<Tb_Ticket_SkyWay>;

                if (SkyWayList != null && SkyWayList.Count > 0)
                {
                    //航程信息
                    RepSkyWay.DataSource = SkyWayList;
                    RepSkyWay.DataBind();
                    //改签航程
                    repGQ.DataSource = SkyWayList;
                    repGQ.DataBind();
                }

                #endregion

                #region 日志信息

                string sqlAirOrderWhere = " OrderId='" + mOrder.OrderId + "'";
                sqlAirOrderWhere += " and WatchType in(4,5)";
                sqlAirOrderWhere += " order by OperTime ";

                //if (mCompany.RoleType == 1)
                //    sqlAirOrderWhere += " and WatchType in(0,1,2,3,4,5)";
                //else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
                //    sqlAirOrderWhere += " and WatchType in(2,3,4,5)";
                //else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)

                List<Log_Tb_AirOrder> AirOrderList = baseDataManage.CallMethod("Log_Tb_AirOrder", "GetList", null, new Object[] { sqlAirOrderWhere }) as List<Log_Tb_AirOrder>;

                if (AirOrderList != null && AirOrderList.Count > 0)
                {
                    RepOrderLog.DataSource = AirOrderList;
                    RepOrderLog.DataBind();
                }

                #endregion


                ViewState["order"] = mOrder;
                ViewState["passengerList"] = PassengerList;
                ViewState["skyWayList"] = SkyWayList;

                //退 废 改 理由绑定
                ReasonBind();
                if (mOrder.CPTime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    //当天，只能废票可以退票
                    rblList.Items.Add(new ListItem("退票申请", "3"));//退
                    rblList.Items.Add(new ListItem("废票申请", "4")); //废
                    rblList.Items.Add(new ListItem("改签申请", "5")); //改签
                    rblList.Items[1].Selected = true;
                    //rblList.Items[0].Enabled = false;
                }
                else
                {
                    //不是当天出票只能废票，不能退票
                    rblList.Items.Add(new ListItem("退票申请", "3"));//退
                    rblList.Items.Add(new ListItem("废票申请", "4")); //废
                    rblList.Items.Add(new ListItem("改签申请", "5")); //改签
                    rblList.Items[0].Selected = true;
                    rblList.Items[1].Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    /// 退费改签理由绑定
    /// </summary>
    public void ReasonBind()
    {
        //退票 非自愿                                                             
        List<Bd_Base_Dictionary> TPDic = GetDictionaryList("21");
        List<Bd_Base_Dictionary> ZYTP = (from Bd_Base_Dictionary dic in TPDic
                                         where dic.A1 == 0//1自愿 0非自愿
                                         orderby dic.ChildID
                                         select dic).ToList<Bd_Base_Dictionary>();

        ddlTPApp.Items.Clear();
        ddlTPApp1.Items.Clear();
        foreach (Bd_Base_Dictionary dic in ZYTP)//非自愿
        {
            ListItem listitem = new ListItem();
            listitem.Text = dic.ChildName;
            listitem.Value = dic.A3 + "|" + Guid.NewGuid();
            ddlTPApp.Items.Add(listitem);
        }


        //退票 自愿
        ZYTP = (from Bd_Base_Dictionary dic in TPDic
                where dic.A1 == 1//1自愿 0非自愿
                orderby dic.ChildID
                select dic).ToList<Bd_Base_Dictionary>();

        foreach (Bd_Base_Dictionary dic in ZYTP)//自愿
        {
            ListItem listitem = new ListItem();
            listitem.Text = dic.ChildName;
            listitem.Value = dic.A3 + "|" + Guid.NewGuid();
            ddlTPApp1.Items.Add(listitem);
        }

        List<Bd_Base_Dictionary> FPDic = GetDictionaryList("27");
        ddlFPApp.Items.Clear();
        foreach (Bd_Base_Dictionary dic in FPDic)
        {
            ListItem listitem = new ListItem();
            listitem.Text = dic.ChildName;
            listitem.Value = dic.A3 + "|" + Guid.NewGuid();
            ddlFPApp.Items.Add(listitem);
        }
        //改签
        ddlGQApp.DataSource = GetDictionaryList("28");
        ddlGQApp.DataTextField = "ChildName";
        ddlGQApp.DataValueField = "ChildID";
        ddlGQApp.DataBind();
    }

    /// <summary>
    /// 处理退改签 处理
    /// </summary>
    private void TGQ(StringBuilder sbContent)
    {
        bool result = false;
        string msg = "";
        //选择申请类型
        int ApplayType = int.Parse(rblList.SelectedValue);
        try
        {
            //订单管理类
            Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
            //克隆一份 不修改ViewState中的数据
            Tb_Ticket_Order TempOrder = ViewState["order"] as Tb_Ticket_Order;
            Tb_Ticket_Order Order = TempOrder != null ? (TempOrder.Clone() as Tb_Ticket_Order) : null;
            //克隆一份不修改ViewState中的数据
            List<Tb_Ticket_Passenger> TempPassengerList = ViewState["passengerList"] as List<Tb_Ticket_Passenger>;
            List<Tb_Ticket_Passenger> PassengerList = new List<Tb_Ticket_Passenger>();
            if (TempPassengerList != null)
            {
                foreach (Tb_Ticket_Passenger item in TempPassengerList)
                {
                    PassengerList.Add((item.Clone() as Tb_Ticket_Passenger));
                }
            }
            //克隆一份不修改ViewState中的数据
            List<Tb_Ticket_SkyWay> TempNewSkyWayList = ViewState["skyWayList"] as List<Tb_Ticket_SkyWay>;
            List<Tb_Ticket_SkyWay> NewSkyWayList = new List<Tb_Ticket_SkyWay>();
            if (TempNewSkyWayList != null)
            {
                foreach (Tb_Ticket_SkyWay item in TempNewSkyWayList)
                {
                    NewSkyWayList.Add((item.Clone() as Tb_Ticket_SkyWay));
                }
            }

            //ViewState中的数据不为空
            if (TempOrder != null && TempPassengerList != null && TempNewSkyWayList != null)
            {
                #region 处理数据
                List<Tb_Ticket_Passenger> tempPassengerList = new List<Tb_Ticket_Passenger>();
                //记录出票订单id
                List<string> passengerId = new List<string>();
                List<Tb_Ticket_Passenger> NewPassengerList = new List<Tb_Ticket_Passenger>();
                Tb_Ticket_Passenger Passenger = null;
                //新订单号
                string NewOrderId = OrderManage.GetOrderId("0");
                int TicketNum = 0;

                #region 处理乘机人
                List<HashObject> pasList = GetPassengerInfo();
                string strId = "", IsChecked = "0";
                for (int i = 0; i < pasList.Count; i++)
                {
                    Passenger = new Tb_Ticket_Passenger();
                    strId = pasList[i].GetValue<string>("id");
                    IsChecked = pasList[i].GetValue<string>("IsChecked");
                    Guid id = Guid.Parse(strId);

                    if (PassengerList != null && PassengerList.Count > 0)
                        tempPassengerList = PassengerList.Where(w => w.id == id).ToList<Tb_Ticket_Passenger>();

                    if (tempPassengerList != null && tempPassengerList.Count > 0)
                        Passenger = tempPassengerList[0];

                    //选中的乘客
                    if (IsChecked == "1")
                    {
                        //原乘客id
                        passengerId.Add(pasList[i].GetValue<string>("id"));
                        Passenger.id = Guid.NewGuid();
                        NewPassengerList.Add(Passenger);
                        TicketNum++;
                    }
                }

                #endregion

                #region 处理航段表
                List<HashObject> skyWay = GetSkyInfo();
                //改签用到航班号和舱位
                List<string> FlightCodeList = new List<string>();
                List<string> SpaceList = new List<string>();
                //临时日期
                DateTime TempDate = DateTime.Parse("1901-01-01");
                DateTime fromDate = DateTime.Parse("1901-01-01");
                for (int i = 0; i < NewSkyWayList.Count; i++)
                {
                    //处理改签航段
                    if (ApplayType == 5)
                    {
                        foreach (HashObject skyParam in skyWay)
                        {
                            //找到相同航段id
                            if (skyParam.GetValue<string>("SkyId") == NewSkyWayList[i].id.ToString())
                            {
                                if (DateTime.TryParse(skyParam.GetValue<string>("FromDate"), out fromDate))
                                {
                                    NewSkyWayList[i].FromDate = fromDate;
                                }
                                if (TempDate.ToString("yyyy-MM-dd") == "1901-01-01")
                                {
                                    TempDate = NewSkyWayList[i].FromDate;
                                }
                                if (DateTime.TryParse(skyParam.GetValue<string>("ToDate"), out fromDate))
                                {
                                    NewSkyWayList[i].ToDate = fromDate;
                                }
                                NewSkyWayList[i].FlightCode = skyParam.GetValue<string>("FlightCode");
                                NewSkyWayList[i].Space = skyParam.GetValue<string>("Space");
                                //航班号和舱位
                                FlightCodeList.Add(NewSkyWayList[i].FlightCode);
                                SpaceList.Add(NewSkyWayList[i].Space);
                            }
                        }
                    }
                    //新id
                    NewSkyWayList[i].id = Guid.NewGuid();
                }

                #endregion

                if (TicketNum == 0)
                {
                    msg = "请选择乘客!";
                }
                else
                {

                    string PassengerNameS = "";
                    foreach (Tb_Ticket_Passenger item in NewPassengerList)
                    {
                        PassengerNameS += item.PassengerName + "|";
                    }
                    PassengerNameS = PassengerNameS.TrimEnd('|');


                    //新id
                    Order.id = Guid.NewGuid();
                    Order.PassengerName = PassengerNameS;
                    Order.PassengerNumber = NewPassengerList.Count;
                    Order.OldOrderId = Order.OrderId; //原订单号
                    Order.OrderId = NewOrderId; //新订单号
                    //Order.PayNo = "";
                    Order.InPayNo = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetIndexId();
                    Order.TicketStatus = ApplayType; //机票状态
                    Order.CreateTime = DateTime.Now;
                    Order.YDRemark = txtRemark.Text.Trim();  //退改签申请备注
                    Order.PayDebtsMoney = 0;
                    Order.LockLoginName = "";
                    Order.LockCpyNo = "";
                    Order.LockTime = DateTime.Parse("1900-01-01 00:00:00.000");

                    string strType = "";

                    if (ApplayType == 3)//退票
                    {
                        Order.OrderStatusCode = 7;
                        Order.A8 = Hid_ZY.Value;

                        if (Hid_ZY.Value == "1")
                        {
                            strType = "自愿申请";
                            Order.TGQApplyReason = ddlTPApp1.SelectedItem.Text.Trim() + "--自愿申请";  //申请退票理由:自愿

                        }
                        else
                        {
                            strType = "非自愿申请";
                            Order.TGQApplyReason = ddlTPApp.SelectedItem.Text.Trim() + "--非自愿申请"; //申请退票理由：非自愿
                        }
                    }
                    else if (ApplayType == 4)//废票
                    {
                        Order.OrderStatusCode = 8;
                        Order.TGQApplyReason = ddlFPApp.SelectedItem.Text; //申请废票理由 
                    }
                    else if (ApplayType == 5)//改签
                    {
                        Order.OrderStatusCode = 6;
                        Order.AirTime = TempDate;
                        Order.FlightCode = string.Join("/", FlightCodeList.ToArray());
                        Order.Space = string.Join("/", SpaceList.ToArray());
                        //改签申请理由  
                        Order.TGQApplyReason = ddlGQApp.SelectedItem.Text;

                        //改签处理其它参数
                        Order.PayWay = 0; //默认支付方式
                        Order.PayNo = "";
                    }
                    //生成订单
                    result = OrderManage.CreateOrderTFG(Order, NewPassengerList, passengerId, NewSkyWayList, mUser, mCompany, strType, sbContent.ToString(), out msg);
                }

                #endregion
            }
            else
            {
                msg = "页面数据丢失,请重新进入该页面！";
            }
        }
        catch (Exception ex)
        {

        }
        string values = "";

        if (result)
        {
            values = "showdialog('订单申请成功!',{type:0,url:'" + Hid_GoUrl.Value + "'});";
        }
        else
        {
            if (string.IsNullOrEmpty(msg))
            {
                if (ApplayType == 3)
                    values = "showdialog('订单申请退票失败!');";
                else if (ApplayType == 4)
                    values = "showdialog('订单申请废票失败!');";
                else if (ApplayType == 5)
                    values = "showdialog('订单申请改签失败!');ApplayType();";
            }
            else
            {
                values = "showdialog('" + msg + "');";
            }
        }

        ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), values, true);
    }

    /// <summary>
    /// 判断PNR是否显示
    /// </summary>
    /// <param name="PNR">PNR</param>
    /// <param name="OrderSource">订单来源</param>
    /// <param name="PayStatus">支付状态</param>
    /// <param name="CpyId">客户公司编号</param>
    /// <returns>返回处理后的信息</returns>
    public string PNRShow(string PNR, string OrderSource, string PayStatus)
    {
        string Message = "";
        try
        {
            if (OrderSource == "PNR导入")
            {
                Message = PNR.ToString();
            }
            else if (PayStatus == "2")
            {
                Message = PNR.ToString();
            }
            else
            {
                Message = "";
            }
        }
        catch
        {
            return "";
        }
        return Message;
    }

    /// <summary>
    /// 显示客规
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string DataSourceMessageKeGui(string str)
    {
        if (str == null || str == "")
        {
            return "暂无客规!!!";
        }
        //【退票规定】 收取30％退票废  【变更规定】 每次改期收取票面价20％的改期废  
        return str;
    }

    /// <summary>
    /// 票号状态是否为Open for Use 状态 true为Open for Use  否则不是
    /// </summary>
    /// <param name="TicketNumber"></param>
    /// <param name="Office"></param>
    /// <returns></returns>
    public bool PassengerTicketIsOpen(List<HashObject> pasList, List<HashObject> skyList, SendInsManage SendIns, StringBuilder orderLog, out string errMsg)
    {
        bool IsOpen = false;
        errMsg = "";
        //第一航段
        HashObject skyParam = skyList[0];
        StringBuilder sbLog = new StringBuilder();
        //格式化pnr类
        PnrAnalysis.FormatPNR format = new PnrAnalysis.FormatPNR();
        string TicketNumber = "", Office = "", Status = "", PassengerName = "", FlyDate = "";
        bool IsSuc = false;
        //申请类型3退票申请 4废票申请 5改签申请
        string ApplayType = rblList.SelectedValue;
        if (pasList != null && pasList.Count > 0)
        {
            foreach (HashObject param in pasList)
            {
                if (param.Count == 10 && param["IsChecked"].ToString() == "1")
                {
                    PassengerName = param["pasName"].ToString().Trim();//乘机人姓名
                    TicketNumber = param["pasTicketNum"].ToString().Trim();//票号
                    Office = param["office"].ToString().Trim();//Office
                    FlyDate = skyParam["FromDate"].ToString().Trim();//乘机日期     

                    if (TicketNumber == "")
                    {
                        continue;
                    }
                    //提取票号信息指令组合
                    string strInstruction = string.Format("DETR:TN/{0}", TicketNumber);
                    orderLog.Append("票号验证指令:" + strInstruction + "\r\n");
                    Office = !this.configparam.Office.Contains(Office) ? this.configparam.Office.Split('^')[0] : Office;
                    //发送指令
                    string recvData = SendIns.Send(strInstruction, ref Office, 0);
                    orderLog.Append("接收：" + recvData + "\r\n");
                    if ((recvData.ToUpper().Contains("NOT EXIST") || recvData.ToUpper().Contains("TICKET NUMBER") || recvData.ToUpper().Contains("NOT FOUND") || recvData.ToUpper().Contains("AUTHORITY") || recvData.ToUpper().Contains("没有权限")) && FlyDate != "")
                    {
                        //检查乘机日期
                        DateTime dt1 = System.DateTime.Parse("1901-01-01");
                        DateTime.TryParse(FlyDate, out dt1);
                        if (DateTime.Compare(dt1, System.DateTime.Now) < 0)
                        {
                            //乘机日期已过期 不能提交
                            IsSuc = true;
                            sbLog.AppendFormat("乘机人{0}票号{1}乘机日期已过期,不能提交申请退废票！", PassengerName, TicketNumber);
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //票号状态
                    Status = format.GetTicketStatus(recvData);
                    if (recvData.Contains("OPEN FOR USE"))
                    {
                        continue; // 可以提交
                    }
                    else if (recvData.Contains("REFOUND") || recvData.Contains("VOID"))
                    {
                        continue; // 可以提交
                    }

                    if (ApplayType == "3") //退
                    {
                        if (recvData.Contains("CHECKED IN") || recvData.Contains("CHECK IN") || recvData.Contains("USED/FLOWN") || recvData.Contains("SUSPENDED") || recvData.Contains("CHANGE"))
                        {
                            sbLog.Append("乘机人" + PassengerName + "票号（" + TicketNumber + "）状态" + Status + "，不能提交退票申请！");
                            IsSuc = true;
                            break;
                        }
                    }
                    else if (ApplayType == "4")//废
                    {
                        if (recvData.Contains("CHECKED IN") || recvData.Contains("CHECK IN") || recvData.Contains("USED/FLOWN") || recvData.Contains("SUSPENDED"))
                        {
                            sbLog.Append("乘机人" + PassengerName + "票号（" + TicketNumber + "）状态" + Status + "，不能提交废票申请！");
                            IsSuc = true;
                            break;
                        }
                    }
                    if (Status.ToUpper() != "OPEN FOR USE")
                    {
                        if (Status == "")
                        {
                            sbLog.AppendFormat("乘机人{0}票号{1}为无效票号,不能提交申请退废票！", PassengerName, TicketNumber);
                        }
                        else
                        {
                            sbLog.AppendFormat("乘机人{0}票号（{1}）状态为{2},不能提交申请退废票！", PassengerName, TicketNumber, Status);
                        }
                        IsSuc = true;
                        break;
                    }
                }
            }
            orderLog.Append("IsSuc=" + IsSuc.ToString() + "\r\n");
            if (!IsSuc)
            {
                IsOpen = true;
            }
            errMsg = sbLog.ToString();
            orderLog.Append("票号给出提示:" + errMsg + "\r\n");
        }
        return IsOpen;
    }

    /// <summary>
    /// 获取航段信息
    /// </summary>
    /// <returns></returns>
    public List<HashObject> GetSkyInfo()
    {
        Tb_Ticket_Order Order = ViewState["order"] as Tb_Ticket_Order;
        List<Tb_Ticket_SkyWay> listsky = ViewState["skyWayList"] as List<Tb_Ticket_SkyWay>;
        List<HashObject> list = new List<HashObject>();
        string IsFP = "1";
        if (Order != null && listsky != null && listsky.Count > 0)
        {
            string TravelCode = Order.TravelCode.Split('/')[0];
            string Travel = Order.Travel.Split('/')[0];
            string fromcode = TravelCode.Split('-')[0];
            string tocode = TravelCode.Split('-')[1];
            string FromCity = Travel.Split('-')[0];
            string ToCity = Travel.Split('-')[1];
            string FromDate = "";
            //查找第一程航段
            Tb_Ticket_SkyWay sky = listsky.Find(delegate(Tb_Ticket_SkyWay _sky)
            {
                return ((_sky.FromCityCode.ToUpper() == fromcode.ToUpper() || _sky.FromCityName == FromCity) && (_sky.ToCityCode.ToUpper() == tocode.ToUpper() || _sky.ToCityName == ToCity));
            });
            if (sky != null)
            {
                FromDate = sky.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
                //坐了飞机以后还能提交退票 增加一个废票的限制，飞机起飞两个半小时内不能提交废票操作，只能走退票通道 1能提交废票 0不能提交废票
                double Minutes = (sky.FromDate - System.DateTime.Now).TotalMinutes;
                IsFP = Minutes < 150 ? "0" : "1";
            }
        }
        HashObject parameter = null;
        //航段信息
        string skyStr = Hid_skyData.Value;
        string[] strArr = skyStr.Split('^');
        foreach (string PasModel in strArr)
        {
            parameter = new HashObject();
            string[] _strArr = PasModel.Split(new string[] { "@@@@" }, StringSplitOptions.None);
            if (_strArr != null && _strArr.Length == 10)
            {
                parameter.Add("SkyId", _strArr[0]);
                parameter.Add("FromCityName", _strArr[1]);
                parameter.Add("ToCityName", _strArr[2]);
                parameter.Add("FromCityCode", _strArr[3]);
                parameter.Add("ToCityCode", _strArr[4]);
                parameter.Add("FromDate", _strArr[5]);
                parameter.Add("ToDate", _strArr[6]);
                parameter.Add("CarryCode", _strArr[7]);
                parameter.Add("FlightCode", _strArr[8]);
                parameter.Add("Space", _strArr[9]);
                parameter.Add("IsFP", IsFP);//是否可以废票
                list.Add(parameter);
            }
        }
        return list;
    }

    /// <summary>
    /// 获取乘机人信息
    /// </summary>
    /// <returns></returns>
    public List<HashObject> GetPassengerInfo()
    {
        List<HashObject> list = new List<HashObject>();
        try
        {
            Tb_Ticket_Order Order = ViewState["order"] as Tb_Ticket_Order;
            HashObject parameter = null;
            string pasStr = Hid_pasData.Value;
            string[] strArr = pasStr.Split('^');
            foreach (string PasModel in strArr)
            {
                parameter = new HashObject();
                string[] _strArr = PasModel.Split(new string[] { "@@@@" }, StringSplitOptions.None);
                if (_strArr != null && _strArr.Length == 9)
                {
                    parameter.Add("id", _strArr[0]);
                    parameter.Add("pasName", _strArr[1]);
                    parameter.Add("pasPType", _strArr[2]);
                    parameter.Add("pasCType", _strArr[3]);
                    parameter.Add("pasCid", _strArr[4]);
                    parameter.Add("pasTravelNumber", _strArr[5]);
                    parameter.Add("pasTicketNum", _strArr[6]);
                    parameter.Add("pasIsBack", _strArr[7]);
                    parameter.Add("IsChecked", _strArr[8]);//是否选中1是0否
                    parameter.Add("office", Order.Office);
                    list.Add(parameter);
                }
            }
        }
        catch (Exception)
        {
        }
        return list;
    }

    /// <summary>
    /// 该乘机人票号 提示对应的行程单是否作废
    /// </summary>
    /// <param name="pas"></param>
    /// <returns></returns>
    public bool TripIsVoid(List<HashObject> strList, SendInsManage SendIns, StringBuilder orderLog, out string msg)
    {
        bool Isvoid = true;
        msg = "";
        try
        {
            StringBuilder sbLog = new StringBuilder();
            string[] OfficeNum = this.configparam.Office.Split(new string[] { "|", " ", "/", ",", "，", "\\", "#" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (HashObject param in strList)
            {
                if (param.Count == 10 && param["IsChecked"].ToString() == "1")
                {
                    string PasName = param["pasName"].ToString().Trim();//乘机人姓名
                    string PasA8 = param["pasTravelNumber"].ToString().Trim();//行程单号                  
                    string PasTicketNumber = param["pasTicketNum"].ToString().Trim();//票号
                    //if (PasA8 != "")
                    //{
                    //Office
                    string office = param["office"].ToString().Trim();
                    //真实状态
                    string strInstruction = string.Format("DETR:TN/{0},F", PasTicketNumber);
                    office = !this.configparam.Office.Contains(office) ? this.configparam.Office.Split('^')[0] : office;
                    string recvData = SendIns.Send(strInstruction, ref office, 0);
                    orderLog.Append("提取行程单验证指令：" + strInstruction + "\r\n");
                    orderLog.Append("接收：" + recvData + "\r\n");
                    List<string> lst = new List<string>();
                    lst.AddRange(new string[]{
                    "没有权限",
                    "NOT EXIST",
                    "TICKET NUMBER",
                    "NOT FOUND",
                    "NO RECORD",
                    "超时"    ,
                    "无法从传输连接中读取数据"
                    });
                    bool isContain = false;
                    foreach (string item in lst)
                    {
                        if (recvData.ToUpper().Contains(item))
                        {
                            isContain = true;
                            break;
                        }
                    }
                    if (recvData != "" && !isContain && recvData.ToUpper().Contains("RP" + PasA8))
                    {
                        Isvoid = false;
                        sbLog.AppendFormat("乘机人:{0} 票号{1}所关联的行程单号未作废<br />请先作废行程单后再操作！", PasName, PasTicketNumber);
                    }
                    else
                    {
                        if (recvData.Contains("授权"))
                        {
                            Isvoid = false;
                            sbLog.AppendFormat("乘机人:{0} 票号{1}验证行程单状态失败,需授权,授权指令:RMK TJ AUTH " + (office == "" ? OfficeNum[0] : office.Trim(new char[] { '$' })), PasName, PasTicketNumber);
                        }
                    }
                    // }
                }
            }
            msg = sbLog.ToString();
            orderLog.Append("Isvoid=：" + Isvoid.ToString() + "\r\n");
            orderLog.Append("提示信息:" + msg + "\r\n");
        }
        catch (Exception)
        {
        }
        return Isvoid;
    }

    /// <summary>
    /// 页面数据显示处理
    /// </summary>
    /// <returns></returns>
    public string ShowData(int type, object data)
    {
        string result = "";
        try
        {
            if (type == 0)
            {
                result = "show";
                //乘客列表复选框
                if (data != null && data != DBNull.Value && data.ToString() != "")
                {
                    if (data.ToString() == "1" || data.ToString().ToUpper() == "TRUE")
                    {
                        result = "hide";
                    }
                }
            }
            else if (type == 1)
            {
                //乘客类型
                if (data != null && data != DBNull.Value && data.ToString() != "")
                {
                    result = GetDictionaryName("6", data.ToString());
                    result = string.Join("<br />", result.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            else if (type == 2)
            {
                //证件类型
                if (data != null && data != DBNull.Value && data.ToString() != "")
                {
                    result = GetDictionaryName("7", data.ToString());
                    result = string.Join("<br />", result.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            else if (type == 3)
            {
                //机票状态
                if (data != null && data != DBNull.Value && data.ToString() != "")
                {
                    result = GetDictionaryName("9", data.ToString());
                    result = string.Join("<br />", result.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
                }

            }
            else if (type == 4)
            {
                result = "hide";
                //是否显示复制按钮
                if (data != null && data != DBNull.Value && data.ToString() != "")
                {
                    result = "show";
                }
            }
        }
        catch
        {
            result = "";
        }
        return result;
    }

    /// <summary>
    /// 申请
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        PbProject.Logic.Order.Tb_Ticket_OrderBLL orderBll = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
        Tb_Ticket_Order Order = ViewState["order"] as Tb_Ticket_Order;
        //提示信息
        string msg = "";
        //订单日志
        StringBuilder orderLog = new StringBuilder();
        try
        {
            this.NewKongZhiXiTong = this.KongZhiXiTong;
            //共享订单权限
            if (Order.PolicySource == 9 && Order.CPCpyNo != "")
            {
                //select top 10 * from dbo.Bd_Base_Parameters where CpyNo='' and SetName='kongZhiXiTong'
                string sqlWhere = string.Format("CpyNo='{0}' and SetName='kongZhiXiTong'", Order.CPCpyNo);
                List<Bd_Base_Parameters> supParams = this.baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Parameters>;
                this.NewKongZhiXiTong = BaseParams.getParams(supParams).KongZhiXiTong;
            }

            //申请类型3退票申请 4废票申请 5改签申请
            string ApplayType = rblList.SelectedValue;
            //扩展参数
            ParamEx pe = new ParamEx();
            pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
            //发送指令管理类
            SendInsManage SendIns = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, configparam);

            #region

            //选择的乘客信息
            List<HashObject> pasList = GetPassengerInfo();
            //选中的 成人 儿童 婴儿
            int AdultCount = 0, CHDCount = 0, INFCount = 0;
            //没选中的 成人 儿童 婴儿
            int NoSelAdultCount = 0, NoSelCHDCount = 0, NoSelINFCount = 0;
            //可以提交的人数
            int CommitCount = 0;
            //选择的乘机人
            List<HashObject> selList = new List<HashObject>();
            foreach (HashObject param in pasList)
            {
                if (param["pasIsBack"].ToString() == "0") //获取没有提交的用户
                {
                    //可以提交的人数
                    CommitCount++;

                    //选择的乘客数
                    if (param["IsChecked"].ToString() == "1")
                    {
                        if (param["pasPType"].ToString() == "1")
                        {
                            AdultCount++;
                        }
                        else if (param["pasPType"].ToString() == "2")
                        {
                            CHDCount++;
                        }
                        else if (param["pasPType"].ToString() == "3")
                        {
                            INFCount++;
                        }
                        selList.Add(param);
                    }
                    else
                    {
                        if (param["pasPType"].ToString() == "1")
                        {
                            NoSelAdultCount++;
                        }
                        else if (param["pasPType"].ToString() == "2")
                        {
                            NoSelCHDCount++;
                        }
                        else if (param["pasPType"].ToString() == "3")
                        {
                            NoSelINFCount++;
                        }
                    }
                }
            }

            #endregion

            //航段信息
            List<HashObject> skyList = GetSkyInfo();

            //自愿退票，废票  都需要判断行程单状态和取消编码
            //非自愿退票，不用验证 行程单状态和编码

            #region
            if (ApplayType != "5" && Order.A9 != "1")//退废票
            {
                //行程单状态判断                           
                if (!NewKongZhiXiTong.Contains("|68|"))
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        //行程单状态判断
                        if (!TripIsVoid(pasList, SendIns, orderLog, out msg))
                            msg = (string.IsNullOrEmpty(msg)) ? "不能提交,行程单状态异常！" : msg;
                    }
                }
                if (string.IsNullOrEmpty(msg))
                {
                    //票号状态判断
                    if (!NewKongZhiXiTong.Contains("|69|"))
                    {
                        if (string.IsNullOrEmpty(msg))
                        {
                            //票号状态判断
                            if (!PassengerTicketIsOpen(pasList, skyList, SendIns, orderLog, out msg))
                                msg = (string.IsNullOrEmpty(msg)) ? "不能提交,票号状态异常！" : msg;
                        }
                    }
                }
                if (string.IsNullOrEmpty(msg))
                {
                    //废票 飞机起飞两个半小时内不能提交废票操作，只能走退票通道
                    if (ApplayType == "4" && skyList.Count > 0)
                    {
                        //是否可以提交废票 1可以 0不可以
                        HashObject strArr = skyList[0] as HashObject;

                        if (strArr.Count == 11 && strArr["IsFP"].ToString() == "0")
                        {
                            if (NewKongZhiXiTong != null && !NewKongZhiXiTong.Contains("|16|"))
                                msg = "飞机起飞两个半小时内不能提交废票操作，只能走退票通道";
                        }
                    }
                }
            }

            #endregion

            if (string.IsNullOrEmpty(msg))
                msg = TimeIsCommit(ApplayType, Order);

            if (string.IsNullOrEmpty(msg))
            {
                //含有没有提交的婴儿并且没有成人时 不可以提交
                if (NoSelINFCount > 0 && NoSelAdultCount == 0)
                {
                    msg = "婴儿订单必须有成人陪同！";
                    orderLog.Append("成人带婴儿订单,需要手动处理编码！");
                }
            }

            if (string.IsNullOrEmpty(msg))
            {
                string divMsg = "";
                int IsCommit = 1;
                #region 判断可以提交
                //保留编码日志
                StringBuilder sbContent = new StringBuilder();
                orderLog.Append("PNR=" + Order.PNR + "  CPCpyNo=" + Order.CPCpyNo + " OrderSourceType=" + Order.OrderSourceType + " ApplayType=" + ApplayType + "  Order.A9=" + Order.A9 + " 自愿:" + (Hid_ZY.Value == "1" ? "自愿" : "非自愿"));
                orderLog.Append("\r\n权限数据值:NewKongZhiXiTong=" + NewKongZhiXiTong);
                // 没有婴儿 并且需要检查编码
                if (INFCount == 0 && Order.A9 != "1")
                {
                    //退费票且没有婴儿
                    if (ApplayType != "5" && (AdultCount > 0 || CHDCount > 0))
                    {
                        //开启退废票（分离、取消）
                        if (NewKongZhiXiTong.Contains("|32|"))
                        {
                            ////退废处理 判断 分离编码 和 取消编码
                            divMsg = PnrHandle(SendIns, selList, orderLog, sbContent, AdultCount, CHDCount, INFCount, CommitCount, out IsCommit, out msg);
                        }
                    }
                }
                #endregion


                if (IsCommit != 1 && !string.IsNullOrEmpty(msg))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(divMsg))
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialogN2('" + divMsg + "');", true);
                    }
                    else
                    {
                        orderLog.Append("订单号:" + Order.OrderId + "提交了");
                        TGQ(sbContent); //直接提交
                    }
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
            }
        }
        catch (Exception)
        {

        }
        finally
        {

            if (Hid_CommitType.Value == "0")
            {
                Hid_pasData.Value = "";
                Hid_skyData.Value = "";
            }
            PnrAnalysis.LogText.LogWrite("PnrHandle退改签日志:" + orderLog.ToString() + "\r\n=========================\r\n", "TGQApplication");
            //解锁
            orderBll.LockOrder(false, Order.id.ToString(), mUser, mCompany);
        }
    }

    /// <summary>
    /// 在指定时间范围内是否可以提价申请
    /// </summary>
    /// <param name="ApplayType"></param>
    /// <param name="Order"></param>
    /// <returns></returns>
    public string TimeIsCommit(string ApplayType, Tb_Ticket_Order Order)
    {
        //true可以提交申请 false不能提交申请
        string msg = "";

        try
        {
            if (ApplayType == "3")//退票
            {

            }
            else if (ApplayType == "4")//废
            {
                if (Order.CPTime.ToString("yyyy-MM-dd") == System.DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    //获取当天废票时间(接口和本地)
                    string[] strTime = GetTime(Order);
                    string PolicyCancelTime = strTime[1];
                    DateTime nowTime = DateTime.Now.AddMinutes(20);//提交时间 ，提前20 分钟提交

                    if (PolicyCancelTime != "" && PolicyCancelTime.Split('-').Length == 2)
                    {
                        string start = DateTime.Now.ToString("yyyy-MM-dd") + " " + PolicyCancelTime.Split('-')[0] + ":00";
                        string end = PolicyCancelTime.Split('-')[1] + ":00";
                        if (end == "00:00:00")
                        {
                            end = "23:59:59";
                        }
                        end = DateTime.Now.ToString("yyyy-MM-dd") + " " + end;
                        DateTime temp = DateTime.Parse(start);
                        DateTime temp1 = DateTime.Parse(end);

                        //在指定时间范围内就可以提交
                        if (DateTime.Compare(temp, nowTime) < 0 && DateTime.Compare(temp1, nowTime) > 0)
                        {
                            //可以提交
                        }
                        else
                        {
                            //不能提交
                            msg = string.Format("不能提交废票申请,请在供应商下班前20分钟提交申请！{0}如有疑问请来电咨询！", Order.PolicyCancelTime != "" ? "处理时间(" + Order.PolicyCancelTime + ")" : "");
                        }
                    }
                    else
                    {
                        // 未设置时间 可以提交
                    }
                }
                else
                {
                    //不是当天出票不能废

                    msg = string.Format("不能提交废票申请,请在供应商下班前20分钟提交申请！{0}如有疑问请来电咨询！", Order.PolicyCancelTime != "" ? "处理时间(" + Order.PolicyCancelTime + ")" : "");
                }
            }
            else if (ApplayType == "5")//废
            {
                //获取当天废票时间(接口和本地)
                string[] strTime = GetTime(Order);
                string PolicyCancelTime = strTime[1];
                DateTime nowTime = DateTime.Now.AddMinutes(20);//提交时间 ，提前20 分钟提交

                if (PolicyCancelTime != "" && PolicyCancelTime.Split('-').Length == 2)
                {
                    string start = DateTime.Now.ToString("yyyy-MM-dd") + " " + PolicyCancelTime.Split('-')[0] + ":00";
                    string end = PolicyCancelTime.Split('-')[1] + ":00";
                    if (end == "00:00:00")
                    {
                        end = "23:59:59";
                    }
                    end = DateTime.Now.ToString("yyyy-MM-dd") + " " + end;
                    DateTime temp = DateTime.Parse(start);
                    DateTime temp1 = DateTime.Parse(end);

                    //在指定时间范围内就可以提交
                    if (DateTime.Compare(temp, nowTime) < 0 && DateTime.Compare(temp1, nowTime) > 0)
                    {
                        //可以提交
                    }
                    else
                    {
                        //不能提交
                        msg = string.Format("不能提交改签申请,请在供应商下班前20分钟提交申请！{0}如有疑问请来电咨询！", Order.PolicyCancelTime != "" ? "处理时间(" + Order.PolicyCancelTime + ")" : "");
                    }
                }
                else
                {
                    // 未设置时间 可以提交
                }
            }
        }
        catch (Exception ex)
        {
            msg = "时间判断有误,不能提交！";
        }

        return msg;
    }

    /// <summary>
    /// 获取时间（处理时间） 
    /// </summary>
    /// <param name="Order"></param>
    /// <returns></returns>
    public string[] GetTime(Tb_Ticket_Order Order)
    {
        string[] time = new string[2];
        time[0] = "";
        time[1] = "";

        try
        {
            if (Order.PolicySource > 2)
            {
                //接口
                if (!string.IsNullOrEmpty(Order.PolicyCancelTime))
                {
                    string sqlWhere = "UninCode='" + Order.CPCpyNo + "'"; //出票方工作时间
                    PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                    List<User_Company> User_CompanyList = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { sqlWhere }) as List<User_Company>;

                    string[] pDateTime = Order.PolicyCancelTime.Split('-');
                    DateTime pDateTime1 = Convert.ToDateTime(pDateTime[0]);
                    DateTime pDateTime2 = Convert.ToDateTime(pDateTime[1]);
                    if (User_CompanyList != null && User_CompanyList.Count > 0)
                    {
                        string[] worktime = User_CompanyList[0].WorkTime.Split('-');
                        string[] bustime = User_CompanyList[0].BusinessTime.Split('-');
                        DateTime worktime1 = Convert.ToDateTime(worktime[0]);
                        DateTime worktime2 = Convert.ToDateTime(worktime[1]);
                        DateTime bustime1 = Convert.ToDateTime(bustime[0]);
                        DateTime bustime2 = Convert.ToDateTime(bustime[1]);

                        string returntime1 = pDateTime1.CompareTo(worktime1) >= 0 ? pDateTime[0] : worktime[0];
                        string returntime2 = pDateTime2.CompareTo(worktime2) >= 0 ? worktime[1] : pDateTime[1];

                        string busreturntime1 = pDateTime1.CompareTo(bustime1) >= 0 ? pDateTime[0] : bustime[0];
                        string busreturntime2 = pDateTime2.CompareTo(bustime2) >= 0 ? bustime[1] : pDateTime[1];
                        time[0] = string.Format("{0}-{1}", returntime1, returntime2);
                        time[1] = string.Format("{0}-{1}", busreturntime1, busreturntime2);
                    }
                }
            }
            //取本地时间
            if (string.IsNullOrEmpty(time[0]))
            {
                string sqlWhere = "UninCode='" + Order.CPCpyNo + "'"; //出票方工作时间
                PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                List<User_Company> User_CompanyList = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { sqlWhere }) as List<User_Company>;

                if (User_CompanyList != null && User_CompanyList.Count > 0)
                {
                    time[0] = User_CompanyList[0].WorkTime;
                    time[1] = User_CompanyList[0].BusinessTime;
                }
            }
        }
        catch (Exception)
        {

        }
        return time;
    }

    /// <summary>
    ///  分离或者取消编码
    /// </summary>
    /// <param name="SendIns">指令管理</param>
    /// <param name="selList">勾选的乘客</param>
    /// <param name="sbLog">记录文本日志</param>
    /// <param name="sbContent">记录订单日志</param>
    /// <param name="AdultCount">选择的成人数</param>
    /// <param name="CHDCount">选择的儿童数</param>
    /// <param name="INFCount">选择的婴儿数</param>
    /// <param name="CommitCount">提交人数</param>
    /// <param name="IsCommit">输出是否需要提交</param>
    /// <param name="ErrMsg">内部错误</param>
    /// <returns></returns>
    public string PnrHandle(SendInsManage SendIns, List<HashObject> selList, StringBuilder sbLog, StringBuilder sbContent, int AdultCount, int CHDCount, int INFCount, int CommitCount, out  int IsCommit, out string ErrMsg)
    {
        //对话框显示数据
        string OuputHTML = "";
        ErrMsg = "";
        IsCommit = 1;//是否可以提交 1可以 0不可以
        try
        {
            //申请类型3退票申请 4废票申请 5改签申请
            string ApplayType = rblList.SelectedValue;
            //true 自愿 false 非自愿
            bool IsZy = Hid_ZY.Value == "1" ? true : false;
            bool IsCancelSplitPnr = (Hid_IsCancel.Value == "1" && Hid_CommitType.Value != "0") ? true : false;

            #region 理由 是否取消或者分离编码
            //是否取消或者分离编码
            bool IsXePnr = false;
            //退票
            if (ApplayType == "3")
            {
                if (IsZy)
                {
                    IsXePnr = ddlTPApp1.SelectedValue.Split('|')[0] == "1" ? true : false;
                }
                else
                {
                    IsXePnr = ddlTPApp.SelectedValue.Split('|')[0] == "1" ? true : false;
                }
            }
            //废票
            if (ApplayType == "4")
            {
                IsXePnr = ddlFPApp.SelectedValue.Split('|')[0] == "1" ? true : false;
            }
            #endregion


            Tb_Ticket_Order Order = ViewState["order"] as Tb_Ticket_Order;
            if (Order != null && Order.PNR != "" && Order.PNR.Trim().Length == 6)
            {
                sbLog.Append("========================\r\n订单号:" + Order.OrderId + " 申请:" + ApplayType == "3" ? "退票申请" : (ApplayType == "4" ? "废票申请" : "改签申请"));
                sbLog.Append("  IsXePnr=" + IsXePnr + "\r\n");
                PnrAnalysis.PnrModel pnrMode = SendIns.GetPnr(Order.PNR.Trim(), out ErrMsg);
                if (pnrMode != null)
                {
                    sbLog.Append("  ErrMsg=" + ErrMsg + " PnrStatus=" + pnrMode.PnrStatus);
                    if (ErrMsg.Contains("授权"))
                    {
                        string[] strAuthArr = ErrMsg.Split('#')[1].Split('|');
                        ErrMsg = "";
                        foreach (string item in strAuthArr)
                        {
                            ErrMsg += "RMK TJ AUTH " + item + "<br/>";
                        }
                        ErrMsg = "需要授权,授权指令:<br />" + ErrMsg;
                        sbLog.Append("提取编码信息失败," + ErrMsg);
                        sbContent.Append("提取编码信息失败," + ErrMsg);
                        //开启申请退费票，提取编码失败，允许提交 103
                        if (NewKongZhiXiTong.Contains("|103|"))
                        {
                            ErrMsg = "";
                            IsCommit = 1;
                        }
                        else
                        {
                            IsCommit = 0;
                        }
                    }
                    else
                    {
                        if (!pnrMode.PnrStatus.Contains("XX"))
                        {
                            if (pnrMode.PnrStatus.Contains("NO"))
                            {
                                ErrMsg = "编码（" + Order.PNR + "）状态为NO，系统取消编码失败！";
                                sbLog.Append(ErrMsg);
                            }
                            //else if (pnrMode.PnrStatus.Contains("RR"))
                            //{
                            //    ErrMsg = "编码（" + Order.PNR + "）状态为RR，该编码已出票，不能取消！";
                            //    sbLog.Append(ErrMsg);
                            //}
                            else
                            {
                                string TicketNumber = "";
                                string Cmd = "", Office = "", RecvData = "";
                                //用于分离编码的乘客姓名 不含婴儿
                                List<string> pnameList = new List<string>();
                                //循环选择的乘机人
                                foreach (HashObject hashParam in selList)
                                {
                                    if (hashParam["pasPType"].ToString() != "3")
                                    {
                                        pnameList.Add(hashParam["pasName"].ToString());
                                    }
                                    TicketNumber = hashParam["pasTicketNum"].ToString();
                                    if (TicketNumber != "" && !NewKongZhiXiTong.Contains("|69|"))
                                    {
                                        Office = hashParam["office"].ToString();
                                        Cmd = "DETR:TN/" + TicketNumber;
                                        RecvData = SendIns.Send(Cmd, ref Office, 0);

                                        if (RecvData.Contains("OPEN FOR USE"))
                                        {
                                            continue; // 可以提交
                                        }
                                        else if (RecvData.Contains("REFOUND") || RecvData.Contains("VOID"))
                                        {
                                            return ""; //直接提交
                                        }

                                        if (ApplayType == "3") //退
                                        {
                                            if (RecvData.Contains("CHECKED IN") || RecvData.Contains("CHECK IN") || RecvData.Contains("USED/FLOWN") || RecvData.Contains("SUSPENDED") || RecvData.Contains("CHANGE"))
                                            {
                                                ErrMsg = "票号（" + TicketNumber + "）状态异常，不能提交退票申请！";
                                                sbLog.Append(ErrMsg);
                                                IsCommit = 0;
                                                break;
                                            }
                                        }
                                        else if (ApplayType == "4")//废
                                        {
                                            if (RecvData.Contains("CHECKED IN") || RecvData.Contains("CHECK IN") || RecvData.Contains("USED/FLOWN") || RecvData.Contains("SUSPENDED"))
                                            {
                                                ErrMsg = "票号（" + TicketNumber + "）状态异常，不能提交废票申请！";
                                                sbLog.Append(ErrMsg);
                                                IsCommit = 0;
                                                break;
                                            }
                                        }

                                        // CHECK IN、USED/FLOWN、SUSPENDED、CHANGE //不允许提交废票申请
                                        //CHECK IN 、USED/FLOWN、SUSPENDED //不允许提交退票申请
                                        //OPRN FOR USE //显示分离取消编码选择
                                        //REFOUND、 VOID //可以直接提交退废申请
                                    }
                                }

                                //没有出错时 
                                if (ErrMsg == "")
                                {
                                    #region

                                    if (INFCount > 0)
                                    {
                                        //编码状态不是已经出票的PNR
                                        //if (pnrMode != null && !pnrMode.PnrStatus.Contains("RR"))
                                        //{
                                        if (((ApplayType == "3" && IsXePnr) || ApplayType == "4") && IsCancelSplitPnr)
                                        {
                                            //有婴儿直接提交                                           
                                            if (SendIns.CancelPnr(Order.PNR, Order.Office))
                                            {
                                                ErrMsg = "";
                                                sbContent.Append("，取消编码【" + Order.PNR + "】成功");
                                                sbLog.Append("订单号:" + Order.OrderId + " PNR:" + Order.PNR + "取消编码成功 ");
                                            }
                                            else
                                            {
                                                ErrMsg = "，取消编码【" + Order.PNR + "】失败！";
                                                sbContent.Append(ErrMsg);
                                                IsCommit = 0;
                                                sbLog.Append(ErrMsg);
                                            }
                                        }
                                        // }
                                    }
                                    else
                                    {
                                        //可以提交的乘客总人数
                                        int TotalCount = CommitCount;
                                        //编码提交标志0开始 1取消编码 2分离编码
                                        string CommitType = Hid_CommitType.Value;
                                        if (CommitType == "0")
                                        {
                                            //是取消还是分离 
                                            if (TotalCount == selList.Count)
                                            {
                                                OuputHTML += " <table width=\"100%\"><tr><td></td><td align=\"center\">是否取消编码？</td></tr> ";
                                                OuputHTML += "<tr><td></td><td align=\"center\"> <br /><input id=\"cboType\" checked=\"true\" type=\"checkbox\" />取消编码</td></tr>";
                                                OuputHTML += "<tr><td align=\"center\" colspan=\"2\"><br /> <input id=\"btnUpdate\" type=\"button\" value=\"确 定\" onclick=\"OK(" + ApplayType + ")\"/>&nbsp;";
                                                OuputHTML += " <input id=\"btnNo\" type=\"button\" value=\"取 消\" onclick=\"btnClose()\"/></td></tr></table>";
                                                Hid_CommitType.Value = "1";
                                            }
                                            else
                                            {
                                                OuputHTML = " <table width=\"100%\"><tr><td></td><td align=\"center\"><input id=\"cboType\" checked=\"true\" type=\"checkbox\" />分离编码,并取消分离后的新编码？</td></tr> ";
                                                OuputHTML += "<tr><td align=\"center\" colspan=\"2\"><br /> <input id=\"btnUpdate\" type=\"button\" value=\"确 定\" onclick=\"OK(" + ApplayType + ")\"/>&nbsp;";
                                                OuputHTML += " <input id=\"btnNo\" type=\"button\" value=\"取 消\" onclick=\"btnClose()\"/></td></tr></table>";
                                                Hid_CommitType.Value = "2";
                                            }
                                        }
                                        else
                                        {
                                            //分离还是取消  保留编码日志
                                            if (CommitType != "0")
                                            {
                                                if (Hid_IsCancel.Value == "0")
                                                {
                                                    if (CommitType == "1")//取消保留编码
                                                    {
                                                        sbContent.Append("，用户选择保留编码，不取消编码 ");
                                                    }
                                                    else if (CommitType == "2")//分离乘客保留编码
                                                    {
                                                        sbContent.Append("，用户选择保留编码，不分离编码 ");
                                                    }
                                                }
                                            }

                                            if (Hid_IsCancel.Value == "1")
                                            {
                                                sbLog.Append("取消或者分离编码  订单号:" + Order.OrderId + " PNR:" + Order.PNR);
                                                if (CommitType == "1")
                                                {
                                                    //编码状态不是已经出票的PNR
                                                    if (pnrMode != null)
                                                    {
                                                        //if (!pnrMode.PnrStatus.Contains("RR"))
                                                        //{
                                                        if (((ApplayType == "3" && IsXePnr) || ApplayType == "4") && IsCancelSplitPnr)
                                                        {
                                                            //取消编码
                                                            if (SendIns.CancelPnr(Order.PNR, Order.Office))
                                                            {
                                                                ErrMsg = "";
                                                                sbContent.Append("，取消编码【" + Order.PNR + "】成功");
                                                                sbLog.Append("订单号:" + Order.OrderId + " PNR:" + Order.PNR + "取消编码成功 ");
                                                            }
                                                            else
                                                            {
                                                                ErrMsg = "，取消编码【" + Order.PNR + "】失败";
                                                                sbContent.Append(ErrMsg);
                                                                IsCommit = 0;
                                                            }
                                                        }
                                                        //}
                                                        //else
                                                        //{
                                                        //    ErrMsg = "编码状态为RR的编码,不能取消！";
                                                        //    IsCommit = 0;
                                                        //}
                                                        sbLog.Append(ErrMsg);
                                                    }
                                                }
                                                else if (CommitType == "2")
                                                {
                                                    if (((ApplayType == "3" && IsXePnr) || ApplayType == "4") && IsCancelSplitPnr)
                                                    {
                                                        string NewPnr = "";
                                                        bool IsCancelNewPnr = false;
                                                        //分离编码
                                                        if (SendIns.SplitPnr(Order.PNR, Order.Office, pnameList, ref NewPnr, ref IsCancelNewPnr, out ErrMsg))
                                                        {
                                                            sbContent.Append(((NewPnr.Trim() != "" ? "，分离编码【" + Order.PNR + "】成功,新编码【" + NewPnr + "】" : "") + (IsCancelNewPnr ? "已取消" : "")) + (!ErrMsg.Contains("申请的乘客姓名") ? (ErrMsg != "" ? ("未取消原因:" + ErrMsg) : "") : "分离成功," + ErrMsg));
                                                            sbLog.Append("订单号:" + Order.OrderId + " PNR:" + Order.PNR + "分离编码【" + Order.PNR + "】成功,新编码【" + NewPnr + "】" + (IsCancelNewPnr ? "已取消" : "未取消！" + (ErrMsg != "" ? ("未取消原因:" + ErrMsg) : "")));
                                                            ErrMsg = "";
                                                        }
                                                        else
                                                        {
                                                            if (ErrMsg.Contains("中不存在以下申请的乘客姓名"))
                                                            {
                                                                sbContent.Append("<br />," + ErrMsg);
                                                                sbLog.Append("可以提交");
                                                                IsCommit = 1;
                                                            }
                                                            else
                                                            {
                                                                if (!ErrMsg.Contains("LEASE WAIT - TRANSACTION IN PROGRESS"))
                                                                {
                                                                    ErrMsg = "分离编码【" + Order.PNR + "】失败,原因如下:" + ErrMsg;
                                                                }
                                                                IsCommit = 0;
                                                                sbLog.Append("禁止提交");
                                                            }
                                                        }
                                                    }
                                                    sbLog.Append(ErrMsg);
                                                }
                                            }
                                            else
                                            {
                                                sbLog.Append("   订单号:" + Order.OrderId + " PNR:" + Order.PNR);
                                                sbContent.Append("   订单号:" + Order.OrderId + " PNR:" + Order.PNR);
                                            }
                                            Hid_CommitType.Value = "0";
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    Hid_CommitType.Value = "0";
                                    Hid_IsCancel.Value = "0";
                                }
                            }
                        }
                    }
                }
                else
                {
                    sbLog.Append("提取编码【" + Order.PNR + "】信息失败！");
                    sbContent.Append("提取编码【" + Order.PNR + "】信息失败！");
                    if (IsZy)
                    {
                        IsCommit = 0;
                    }
                    if (ErrMsg.Contains("授权"))
                    {
                        string[] strAuthArr = ErrMsg.Split('#')[1].Split('|');
                        ErrMsg = "";
                        foreach (string item in strAuthArr)
                        {
                            ErrMsg += "RMK TJ AUTH " + item + "<br/>";
                        }
                        ErrMsg = "需要授权,授权指令:<br />" + ErrMsg;
                        sbContent.Append(ErrMsg);
                    }
                    //开启申请退费票，提取编码失败，允许提交 103
                    if (NewKongZhiXiTong.Contains("|103|"))
                    {
                        ErrMsg = "";
                        IsCommit = 1;
                    }
                    else
                    {
                        IsCommit = 0;
                    }
                }
            }
            else
            {
                sbLog.Append("订单不存在！");
                IsCommit = 0;
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message;
            IsCommit = 0;
        }
        finally
        {

        }
        return OuputHTML;
    }
}
