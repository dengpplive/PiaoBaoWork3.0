using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using PbProject.Model;
using System.Linq;
using PbProject.WebCommon.Utility.Encoding;
using DataBase.Data;
using PbProject.Logic.ControlBase;
using PnrAnalysis.Model;
using PbProject.Logic.PID;
using PbProject.WebCommon.Utility;

/// <summary>
/// 修改订单信息
/// </summary>
public partial class Order_OrderUpdate : BasePage
{
    private List<Bd_Base_Dictionary> dicList = new List<Bd_Base_Dictionary>();
    private List<Bd_Air_Carrier> CarrierList = new List<Bd_Air_Carrier>();

    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            return BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
        }
    }
    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            return BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
        }
    }

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
                if (Request.QueryString["id"] != null && Request.QueryString["Url"] != null)
                {
                    //订单号
                    OrderBind();
                    Hid_GoUrl.Value = string.Format("{0}?currentuserid={1}", Request.QueryString["Url"].ToString(), Request.QueryString["currentuserid"].ToString());//返回  
                }
            }

        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 初始化字典表
    /// </summary>
    public void GetDictionary()
    {
        try
        {
            if (dicList.Count == 0)
            {
                dicList = new Bd_Base_DictionaryBLL().GetList();
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 初始化航空公司
    /// </summary>
    public void GetCarrier()
    {
        try
        {
            if (CarrierList.Count == 0)
            {
                CarrierList = new PbProject.Dal.ControlBase.BaseData<Bd_Air_Carrier>().GetList("");
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 绑定订单信息
    /// </summary>
    private void OrderBind()
    {
        txtOrderReason.Text = "";

        try
        {
            string sqlWhere = Request.QueryString["id"].ToString().Replace("'", "");
            Tb_Ticket_Order mOrder = baseDataManage.CallMethod("Tb_Ticket_Order", "GetById", null, new Object[] { sqlWhere }) as Tb_Ticket_Order;
            if (mOrder != null)
            {
                List<Bd_Base_Dictionary> bDictionaryList = null;
                if (dicList.Count == 0)
                    GetDictionary();

                //用到的隐藏信息

                sqlWhere = " OrderId='" + mOrder.OrderId + "' ";
                #region 订单信息

                //订单信息
                txtInPayNo.Text = mOrder.InPayNo;
                lblLockId.Text = mOrder.LockLoginName;
                lblLockTime.Text = mOrder.LockTime.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : mOrder.LockTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtOrderId.Text = mOrder.OrderId;
                //客规
                txtKeGui.Text = string.IsNullOrEmpty(mOrder.KeGui) ? "" : mOrder.KeGui;
                txtFGQ.Text = mOrder.PolicyCancelTime;

                //lblOrderStatusCode.Text =  mOrder.OrderStatusCode.ToString();
                //txtOrderStatusCode.Text =  GetDictionaryName("1", mOrder.OrderStatusCode.ToString());
                //txtOrderStatusCode.ForeColor = mOrder.OrderStatusCode == 3 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                //lblPayWay.Text = mOrder.PayWay.ToString();

                txtPayMoney.Text = mOrder.PayMoney.ToString("F2");
                txtPayNo.Text = mOrder.PayNo;
                ddlPayStatus.SelectedValue = (mOrder.PayStatus == 1) ? "1" : "0";
                txtPNR.Text = mOrder.PNR;
                txtOldPolicyPoint.Text = mOrder.OldPolicyPoint.ToString();//原政策
                txtPolicyPoint.Text = mOrder.PolicyPoint.ToString();//出票政策
                txtReturnPoint.Text = mOrder.ReturnPoint.ToString();//最终政策
                txtReturnMoney.Text = mOrder.ReturnMoney.ToString();//现返
                txtOldReturnMoney.Text = mOrder.OldReturnMoney.ToString();//原现返金额
                txtPolicyPoint2.Text = mOrder.A7.ToString();
                txtPolicyRemark.Text = mOrder.PolicyRemark;
                txtOrderMoney.Text = mOrder.OrderMoney.ToString("F2");

                ddlA9.SelectedValue = (!string.IsNullOrEmpty(mOrder.A9) && mOrder.A9 == "1") ? "1" : "0";//机票检查

                #region 支付方式
                bDictionaryList = (from Bd_Base_Dictionary d in dicList where d.ParentID == 4 orderby d.ChildID select d).ToList<Bd_Base_Dictionary>();

                if (bDictionaryList != null && bDictionaryList.Count > 0)
                {
                    ddlPayWay.DataSource = bDictionaryList;
                    ddlPayWay.DataTextField = "ChildDescription";
                    ddlPayWay.DataValueField = "ChildID";
                    ddlPayWay.DataBind();
                }
                if (mOrder.PayWay != 0)
                {
                    ddlPayWay.SelectedValue = mOrder.PayWay.ToString();
                }
                else
                {
                    ddlPayWay.Items.Insert(0, new ListItem("未支付", "0"));
                }

                #endregion

                #region 订单状态
                bDictionaryList = (from Bd_Base_Dictionary d in dicList where d.ParentID == 1 orderby d.ChildID select d).ToList<Bd_Base_Dictionary>();

                if (bDictionaryList != null && bDictionaryList.Count > 0)
                {
                    ddlOrderStatusCode.DataSource = bDictionaryList;
                    ddlOrderStatusCode.DataTextField = "ChildName";
                    ddlOrderStatusCode.DataValueField = "ChildID";
                    ddlOrderStatusCode.DataBind();
                }

                ddlOrderStatusCode.SelectedValue = mOrder.OrderStatusCode.ToString();
                #endregion

                #region 政策来源
                bDictionaryList = (from Bd_Base_Dictionary d in dicList where d.ParentID == 24 orderby d.ChildID select d).ToList<Bd_Base_Dictionary>();

                if (bDictionaryList != null && bDictionaryList.Count > 0)
                {
                    ddlPolicySource.DataSource = bDictionaryList;
                    ddlPolicySource.DataTextField = "ChildName";
                    ddlPolicySource.DataValueField = "ChildID";
                    ddlPolicySource.DataBind();
                }

                ddlPolicySource.SelectedValue = mOrder.PolicySource.ToString();
                //lblPolicySource.Text =  mOrder.PolicySource.ToString();
                #endregion

                #region 订单来源
                bDictionaryList = (from Bd_Base_Dictionary d in dicList where d.ParentID == 33 orderby d.ChildID select d).ToList<Bd_Base_Dictionary>();

                if (bDictionaryList != null && bDictionaryList.Count > 0)
                {
                    ddlOrderSourceType.DataSource = bDictionaryList;
                    ddlOrderSourceType.DataTextField = "ChildName";
                    ddlOrderSourceType.DataValueField = "ChildID";
                    ddlOrderSourceType.DataBind();
                }

                ddlOrderSourceType.SelectedValue = mOrder.OrderSourceType.ToString();
                #endregion

                trOutOrder.Visible = true;
                //代付信息
                txtOutOrderId.Text = mOrder.OutOrderId;

                ddlOutOrderPayFlag.SelectedValue = (mOrder.OutOrderPayFlag == true) ? "1" : "0";

                txtOutOrderPayMoney.Text = mOrder.OutOrderPayMoney.ToString("F2");
                txtOutOrderPayNo.Text = mOrder.OutOrderPayNo;


                // mOrder.TGQApplyReason  退改签申请理由
                // mOrder.TGQRefusalReason  退改签拒绝理由
                // mOrder.YDRemark （订票备注）预订时备注信息
                // mOrder.CPRemark （出票备注）出票时备注信息

                // 显示 预订备注
                trYDRemark.Visible = true;
                txtYDRemark.Text = mOrder.YDRemark;


                if (mOrder.TicketStatus == 1)
                {

                }
                else if (mOrder.TicketStatus == 2)
                {
                    // 显示 预订备注  出票备注
                    trCPRemark.Visible = true;
                    txtCPRemark.Text = mOrder.CPRemark;
                }
                else if (mOrder.TicketStatus == 3 || mOrder.TicketStatus == 4 || mOrder.TicketStatus == 5)
                {
                    //退废改  申请理由
                    trTGQApplyReason.Visible = true;
                    txtTGQApplyReason.Text = mOrder.TGQApplyReason;

                    // 拒绝理由
                    trTGQRefusalReason.Visible = true;
                    txtTGQRefusalReason.Text = mOrder.TGQRefusalReason;

                }
                else if (mOrder.TicketStatus == 6)
                {
                    if (mOrder.OrderStatusCode == 5 || mOrder.OrderStatusCode == 20)
                    {
                        // 拒绝理由
                        trTGQRefusalReason.Visible = true;
                        txtTGQRefusalReason.Text = mOrder.TGQRefusalReason;
                    }
                }

                #endregion

                #region 乘机人信息

                List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;

                if (PassengerList != null && PassengerList.Count > 0)
                {
                    RepPassenger.DataSource = PassengerList;
                    RepPassenger.DataBind();
                }

                #endregion

                #region 行程信息

                //现在
                List<Tb_Ticket_SkyWay> SkyWayList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_SkyWay>;

                if (SkyWayList != null && SkyWayList.Count > 0)
                {
                    RepSkyWay.DataSource = SkyWayList;
                    RepSkyWay.DataBind();
                }

                //改签的，显示原航程信息
                if (mOrder.TicketStatus == 5)
                {
                    trRepSkyWayOld.Visible = true;
                    string tempSqlWhere = "OrderId='" + mOrder.OldOrderId + "'";
                    List<Tb_Ticket_SkyWay> SkyWayListOld = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { tempSqlWhere }) as List<Tb_Ticket_SkyWay>;

                    if (SkyWayListOld != null && SkyWayListOld.Count > 0)
                    {
                        RepSkyWayOld.DataSource = SkyWayListOld;
                        RepSkyWayOld.DataBind();
                    }
                }
                else
                {
                    trRepSkyWayOld.Visible = false;
                }
                #endregion

                #region 日志信息

                string sqlAirOrderWhere = " OrderId='" + mOrder.OrderId + "'";
                if (mCompany.RoleType == 1)
                    sqlAirOrderWhere += " and WatchType in(0,1,2,3,4,5)";
                else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
                    sqlAirOrderWhere += " and WatchType in(2,3,4,5)";
                else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                    sqlAirOrderWhere += " and WatchType in(4,5)";
                sqlAirOrderWhere += " order by OperTime ";

                List<Log_Tb_AirOrder> AirOrderList = baseDataManage.CallMethod("Log_Tb_AirOrder", "GetList", null, new Object[] { sqlAirOrderWhere }) as List<Log_Tb_AirOrder>;

                if (AirOrderList != null && AirOrderList.Count > 0)
                {
                    RepOrderLog.DataSource = AirOrderList;
                    RepOrderLog.DataBind();
                }

                #endregion

                ViewState["mOrder"] = mOrder;
                ViewState["PassengerList"] = PassengerList;
                ViewState["SkyWayList"] = SkyWayList;

            }
        }
        catch (Exception ex)
        {

        }
    }

    #region 乘机人处理

    /// <summary>
    /// 乘机人处理
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RepPassenger_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        bool result = false;

        try
        {
            if (e.CommandName == "Update")
            {
                if (string.IsNullOrEmpty(((TextBox)e.Item.FindControl("PassengerReason")).Text.Trim().Trim()))
                {
                    msg = "请输入理由！";
                }
                else
                {
                    #region 1.获取修改的数据

                    StringBuilder updateOrder = new StringBuilder();
                    StringBuilder Contents = new StringBuilder();

                    string id = e.CommandArgument.ToString();
                    string tempStr = ""; //变量

                    List<Tb_Ticket_Passenger> PassengerList = ViewState["PassengerList"] as List<Tb_Ticket_Passenger>;
                    Tb_Ticket_Passenger oldPassenger = null;

                    foreach (Tb_Ticket_Passenger item in PassengerList)
                    {
                        if (id == item.id.ToString())
                        {
                            oldPassenger = item; //修改之前的数据
                            break;
                        }
                    }
                    string OrderId = oldPassenger.OrderId;
                    //赋值：实体
                    //Tb_Ticket_Passenger newPassenger = null;
                    //newPassenger = this.baseDataManage.CallMethod("Tb_Ticket_Passenger", "CopyModel", null, new object[] { oldPassenger }) as Tb_Ticket_Passenger;

                    //修改理由 
                    tempStr = ((TextBox)e.Item.FindControl("PassengerReason")).Text.Trim();
                    Contents.Append("修改乘机人" + ((TextBox)e.Item.FindControl("PassengerName")).Text.Trim() + "数据(理由):" + tempStr + "<br />");

                    //乘客类型 
                    tempStr = ((DropDownList)e.Item.FindControl("PassengerType")).SelectedValue;
                    if (oldPassenger.PassengerType != int.Parse(tempStr))
                    {
                        updateOrder.Append(" PassengerType='" + tempStr + "',");
                        Contents.Append("(乘客类型) 原:" + oldPassenger.PassengerType + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }
                    bool IsUpdatePassengerName = false;
                    ////乘客姓名
                    tempStr = ((TextBox)e.Item.FindControl("PassengerName")).Text.Trim();
                    if (oldPassenger.PassengerName != tempStr)
                    {
                        IsUpdatePassengerName = true;
                        updateOrder.Append(" PassengerName='" + tempStr + "',");
                        Contents.Append("(乘客姓名) 原:" + oldPassenger.PassengerName + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //证件类型 
                    tempStr = ((DropDownList)e.Item.FindControl("CType")).SelectedValue;
                    if (oldPassenger.CType != tempStr)
                    {
                        updateOrder.Append(" CType='" + tempStr + "',");
                        Contents.Append("(证件类型) 原:" + oldPassenger.CType + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //证件号 
                    tempStr = ((TextBox)e.Item.FindControl("Cid")).Text.Trim();
                    if (oldPassenger.Cid != tempStr)
                    {
                        updateOrder.Append(" Cid='" + tempStr + "',");
                        Contents.Append("(证件号) 原:" + oldPassenger.Cid + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //票号 
                    tempStr = ((TextBox)e.Item.FindControl("TicketNumber")).Text.Trim();
                    if (oldPassenger.TicketNumber != tempStr)
                    {
                        updateOrder.Append(" TicketNumber='" + tempStr + "',");
                        Contents.Append("(票号) 原:" + oldPassenger.TicketNumber + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //机票状态 
                    tempStr = ((DropDownList)e.Item.FindControl("TicketStatus")).SelectedValue;
                    if (oldPassenger.TicketStatus != int.Parse(tempStr))
                    {
                        updateOrder.Append(" TicketStatus='" + tempStr + "',");
                        Contents.Append("(机票状态) 原:" + oldPassenger.TicketStatus + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //舱位价 
                    tempStr = ((TextBox)e.Item.FindControl("PMFee")).Text.Trim();
                    if (oldPassenger.PMFee != decimal.Parse(tempStr))
                    {
                        updateOrder.Append(" PMFee='" + tempStr + "',");
                        Contents.Append("(舱位价) 原:" + oldPassenger.PMFee + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //机建 
                    tempStr = ((TextBox)e.Item.FindControl("ABFee")).Text.Trim();
                    if (oldPassenger.ABFee != decimal.Parse(tempStr))
                    {
                        updateOrder.Append(" ABFee='" + tempStr + "',");
                        Contents.Append("(机建) 原:" + oldPassenger.ABFee + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //燃油 
                    tempStr = ((TextBox)e.Item.FindControl("FuelFee")).Text.Trim();
                    if (oldPassenger.FuelFee != decimal.Parse(tempStr))
                    {
                        updateOrder.Append(" FuelFee='" + tempStr + "',");
                        Contents.Append("(燃油) 原:" + oldPassenger.FuelFee + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //退废手续费 
                    tempStr = ((TextBox)e.Item.FindControl("TGQHandlingFee")).Text.Trim();
                    if (oldPassenger.TGQHandlingFee != decimal.Parse(tempStr))
                    {
                        updateOrder.Append(" TGQHandlingFee='" + tempStr + "',");
                        Contents.Append("(退废手续费) 原:" + oldPassenger.TGQHandlingFee + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    // 退废改处理标示： 1 / true  不能提交，0 / false 可以提交

                    //退废改操作 
                    tempStr = ((DropDownList)e.Item.FindControl("IsBack")).SelectedValue;
                    if (oldPassenger.IsBack != (tempStr == "1" ? true : false))
                    {
                        updateOrder.Append(" IsBack='" + tempStr + "',");
                        Contents.Append("(退废改操作) 原:" + oldPassenger.IsBack + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //行程单号 
                    tempStr = ((TextBox)e.Item.FindControl("TravelNumber")).Text.Trim();
                    if (oldPassenger.TravelNumber != tempStr)
                    {
                        updateOrder.Append(" TravelNumber='" + tempStr + "',");
                        Contents.Append("(行程单号) 原:" + oldPassenger.TravelNumber + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }
                    //行程单状态
                    tempStr = ((DropDownList)e.Item.FindControl("ddlTrvalStatus")).SelectedValue;
                    if (oldPassenger.TravelStatus.ToString() != tempStr && tempStr != "")
                    {
                        updateOrder.Append(" TravelStatus='" + tempStr + "',");
                        Contents.Append("(行程单状态) 原:" + (oldPassenger.TravelStatus == 0 ? "未创建" : oldPassenger.TravelStatus == 1 ? "已创建" : oldPassenger.TravelStatus == 2 ? "已作废" : "") + ",新:<span style='color:Red;'>" + (tempStr == "0" ? "未创建" : tempStr == "1" ? "已创建" : tempStr == "2" ? "已作废" : "") + "</span><br />");
                    }

                    #endregion

                    #region 2.添加订单日志

                    Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = oldPassenger.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.CpyNo = mCompany.UninCode;
                    OrderLog.CpyType = mCompany.RoleType;
                    OrderLog.CpyName = mCompany.UninAllName;
                    OrderLog.OperContent = Contents.ToString();
                    OrderLog.WatchType = 1;

                    #endregion

                    string sql = "";
                    string setWhere = "";

                    if (updateOrder == null || updateOrder.ToString() == "")
                    {
                        msg = "没有修改过数据！";
                    }
                    else
                    {
                        setWhere = updateOrder.ToString().TrimEnd(',');
                        sql = "update Tb_Ticket_Passenger set " + setWhere + " where id='" + id + "'";
                    }

                    if (string.IsNullOrEmpty(msg))
                    {
                        List<string> sqlList = new List<string>();
                        //修改乘机人表中的乘机人名字
                        sqlList.Add(sql);
                        //是否修改了订单表乘机人名字
                        if (IsUpdatePassengerName)
                        {
                            int ItemCount = RepPassenger.Items.Count;
                            //乘机人名字
                            List<string> pasList = new List<string>();
                            for (int i = 0; i < ItemCount; i++)
                            {
                                tempStr = ((TextBox)e.Item.FindControl("PassengerName")).Text.Trim();
                                TextBox txtPasName = RepPassenger.Items[i].FindControl("PassengerName") as TextBox;
                                if (txtPasName != null)
                                {
                                    pasList.Add(txtPasName.Text.Trim());
                                }
                            }
                            //修改订单中的乘机人名字
                            if (pasList.Count > 0)
                            {
                                sqlList.Add(string.Format(" update Tb_Ticket_Order set PassengerName='{0}' where OrderId='{1}' ", string.Join("|", pasList.ToArray()), OrderId));
                            }
                        }
                        result = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().UpdateOrder(sqlList, OrderLog);
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }

        if (result == true)
        {
            msg = "更新成功!";
            //OrderBind();
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');location.href='" + Hid_GoUrl.Value + "'", true);
        }
        else
        {
            msg = string.IsNullOrEmpty(msg) ? "更新失败!" : msg;
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }


    }

    /// <summary>
    /// 乘机人处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RepPassenger_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemIndex != -1)
            {
                DropDownList PassengerType = ((DropDownList)e.Item.FindControl("PassengerType"));
                DropDownList CType = ((DropDownList)e.Item.FindControl("CType"));
                DropDownList TicketStatus = ((DropDownList)e.Item.FindControl("TicketStatus"));
                DropDownList TrvalStatus = ((DropDownList)e.Item.FindControl("ddlTrvalStatus"));
                TrvalStatus.SelectedValue = ((HiddenField)e.Item.FindControl("Hid_TripStatus")).Value;

                PassengerType.Items.Clear();
                CType.Items.Clear();
                TicketStatus.Items.Clear();

                List<Bd_Base_Dictionary> bDictionaryList = null;

                if (dicList.Count == 0)
                {
                    GetDictionary();
                }

                bDictionaryList = (from Bd_Base_Dictionary d in dicList where d.ParentID == 6 orderby d.ChildID select d).ToList<Bd_Base_Dictionary>();
                if (bDictionaryList != null && bDictionaryList.Count > 0)
                {
                    PassengerType.DataSource = bDictionaryList;
                    PassengerType.DataTextField = "ChildName";
                    PassengerType.DataValueField = "ChildID";
                    PassengerType.DataBind();
                }

                bDictionaryList = (from Bd_Base_Dictionary d in dicList where d.ParentID == 7 orderby d.ChildID select d).ToList<Bd_Base_Dictionary>();
                if (bDictionaryList != null && bDictionaryList.Count > 0)
                {
                    CType.DataSource = bDictionaryList;
                    CType.DataTextField = "ChildName";
                    CType.DataValueField = "ChildID";
                    CType.DataBind();
                }


                bDictionaryList = (from Bd_Base_Dictionary d in dicList where d.ParentID == 9 orderby d.ChildID select d).ToList<Bd_Base_Dictionary>();
                if (bDictionaryList != null && bDictionaryList.Count > 0)
                {
                    TicketStatus.DataSource = bDictionaryList;
                    TicketStatus.DataTextField = "ChildName";
                    TicketStatus.DataValueField = "ChildID";
                    TicketStatus.DataBind();
                }

                PassengerType.SelectedValue = ((Label)e.Item.FindControl("lblPassengerType")).Text.Trim();
                CType.SelectedValue = ((Label)e.Item.FindControl("lblCType")).Text.Trim();
                TicketStatus.SelectedValue = ((Label)e.Item.FindControl("lblTicketStatus")).Text.Trim();

                ((DropDownList)e.Item.FindControl("IsBack")).SelectedValue =
                    (((HiddenField)e.Item.FindControl("Hid_IsBack")).Value == "1" || ((HiddenField)e.Item.FindControl("Hid_IsBack")).Value.Trim().ToUpper() == "TRUE") ? "1" : "0";
            }
        }
        catch (Exception ex)
        {

        }
    }

    #endregion

    #region 航段处理

    /// <summary>
    /// 航段处理
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RepSkyWay_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        bool result = false;

        try
        {
            if (e.CommandName == "Update")
            {
                #region Update

                if (string.IsNullOrEmpty(((TextBox)e.Item.FindControl("SkyWayReason")).Text.Trim().Trim()))
                {
                    msg = "请输入理由！";
                }
                else
                {
                    #region 1.获取修改的数据

                    StringBuilder updateOrder = new StringBuilder();
                    StringBuilder Contents = new StringBuilder();

                    string id = e.CommandArgument.ToString();
                    string tempStr = ""; //变量

                    List<Tb_Ticket_SkyWay> SkyWayList = ViewState["SkyWayList"] as List<Tb_Ticket_SkyWay>;
                    Tb_Ticket_SkyWay oldSkyWay = null;

                    foreach (Tb_Ticket_SkyWay item in SkyWayList)
                    {
                        if (id == item.id.ToString())
                        {
                            oldSkyWay = item; //修改之前的数据
                            break;
                        }
                    }

                    //赋值：实体
                    //Tb_Ticket_SkyWay newSkyWay = null;
                    //newSkyWay = this.baseDataManage.CallMethod("Tb_Ticket_SkyWay", "CopyModel", null, new object[] { oldSkyWay }) as Tb_Ticket_SkyWay;

                    //修改理由 
                    tempStr = ((TextBox)e.Item.FindControl("SkyWayReason")).Text.Trim();
                    Contents.Append("修改航段数据(理由):" + tempStr + "<br />");

                    //起飞城市 
                    tempStr = ((TextBox)e.Item.FindControl("FromCityCode")).Text.Trim();
                    if (oldSkyWay.FromCityCode != tempStr)
                    {
                        updateOrder.Append(" FromCityCode='" + tempStr + "',");
                        Contents.Append("(起飞城市) 原:" + oldSkyWay.FromCityCode + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    tempStr = ((TextBox)e.Item.FindControl("FromCityName")).Text.Trim();
                    if (oldSkyWay.FromCityName != tempStr)
                    {
                        updateOrder.Append(" FromCityName='" + tempStr + "',");
                        Contents.Append("(起飞城市) 原:" + oldSkyWay.FromCityName + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //到达城市 

                    tempStr = ((TextBox)e.Item.FindControl("ToCityCode")).Text.Trim();
                    if (oldSkyWay.ToCityCode != tempStr)
                    {
                        updateOrder.Append(" ToCityCode='" + tempStr + "',");
                        Contents.Append("(到达城市) 原:" + oldSkyWay.ToCityCode + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    tempStr = ((TextBox)e.Item.FindControl("ToCityName")).Text.Trim();
                    if (oldSkyWay.ToCityName != tempStr)
                    {
                        updateOrder.Append(" ToCityName='" + tempStr + "',");
                        Contents.Append("(到达城市) 原:" + oldSkyWay.ToCityName + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //起飞日期 
                    tempStr = ((TextBox)e.Item.FindControl("FromDate")).Text.Trim();
                    if (oldSkyWay.FromDate.ToString("yyyy-MM-dd HH:mm") != tempStr)
                    {
                        updateOrder.Append(" FromDate='" + tempStr + ":00',");
                        Contents.Append("(起飞日期) 原:" + oldSkyWay.FromDate + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //到达日期
                    tempStr = ((TextBox)e.Item.FindControl("ToDate")).Text.Trim();
                    if (oldSkyWay.ToDate.ToString("yyyy-MM-dd HH:mm") != tempStr)
                    {
                        updateOrder.Append(" ToDate='" + tempStr + ":00',");
                        Contents.Append("(到达日期) 原:" + oldSkyWay.ToDate + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //承运人 
                    tempStr = ((DropDownList)e.Item.FindControl("CarryCode")).SelectedValue;
                    if (oldSkyWay.CarryCode != tempStr)
                    {
                        updateOrder.Append(" CarryCode='" + tempStr + "',");
                        Contents.Append("(承运人) 原:" + oldSkyWay.CarryCode + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //航班号 
                    tempStr = ((TextBox)e.Item.FindControl("FlightCode")).Text.Trim();
                    if (oldSkyWay.FlightCode != tempStr)
                    {
                        updateOrder.Append(" FlightCode='" + tempStr + "',");
                        Contents.Append("(航班号) 原:" + oldSkyWay.FlightCode + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //舱位 
                    tempStr = ((TextBox)e.Item.FindControl("Space")).Text.Trim();
                    if (oldSkyWay.Space != tempStr)
                    {
                        updateOrder.Append(" Space='" + tempStr + "',");
                        Contents.Append("(舱位) 原:" + oldSkyWay.Space + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //折扣 
                    tempStr = ((TextBox)e.Item.FindControl("Discount")).Text.Trim();
                    if (oldSkyWay.Discount != tempStr)
                    {
                        updateOrder.Append(" Discount='" + tempStr + "',");
                        Contents.Append("(折扣) 原:" + oldSkyWay.Discount + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }

                    //机型 
                    tempStr = ((TextBox)e.Item.FindControl("Aircraft")).Text.Trim();
                    if (oldSkyWay.Aircraft != tempStr)
                    {
                        updateOrder.Append(" Aircraft='" + tempStr + "',");
                        Contents.Append("(机型) 原:" + oldSkyWay.Aircraft + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    }


                    #endregion

                    #region 2.添加订单日志

                    Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = oldSkyWay.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.CpyNo = mCompany.UninCode;
                    OrderLog.CpyType = mCompany.RoleType;
                    OrderLog.CpyName = mCompany.UninAllName;
                    OrderLog.OperContent = Contents.ToString();
                    OrderLog.WatchType = 1;

                    #endregion

                    string sql = "";

                    if (updateOrder == null)
                    {
                        msg = "没有修改过数据！";
                    }
                    else
                    {
                        string setWhere = updateOrder.ToString().TrimEnd(',');

                        sql = "update Tb_Ticket_SkyWay set " + setWhere + " where id='" + id + "'";
                    }

                    if (string.IsNullOrEmpty(msg))
                    {
                        List<string> sqlList = new List<string>();
                        sqlList.Add(sql);
                        result = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().UpdateOrder(sqlList, OrderLog);
                    }
                }
                #endregion
            }
            else
            {

            }
        }
        catch (Exception ex)
        {

        }

        if (result == true)
        {
            msg = "更新成功!";
            //OrderBind();

            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');location.href='" + Hid_GoUrl.Value + "'", true);
        }
        else
        {
            msg = string.IsNullOrEmpty(msg) ? "更新失败!" : msg;
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }

    /// <summary>
    /// 航段处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RepSkyWay_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemIndex != -1)
            {
                DropDownList CarryCode = ((DropDownList)e.Item.FindControl("CarryCode"));
                CarryCode.Items.Clear();

                if (CarrierList.Count == 0)
                {
                    GetCarrier();
                }

                foreach (Bd_Air_Carrier item in CarrierList)
                {
                    ListItem items = new ListItem();
                    items.Text = item.Code + "-" + item.ShortName;
                    items.Value = item.Code;
                    CarryCode.Items.Add(items);
                }

                string carryCode = ((Label)e.Item.FindControl("lblCarryCode")).Text.Trim();

                if (string.IsNullOrEmpty(carryCode))
                {

                }
                else
                {
                    CarryCode.SelectedValue = carryCode;
                }



            }
        }
        catch (Exception)
        {


        }
    }

    #endregion

    /// <summary>
    /// 修改订单数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOrderUpdate_Click(object sender, EventArgs e)
    {
        string msg = "";
        bool result = false;


        try
        {

            #region Update

            if (string.IsNullOrEmpty(txtOrderReason.Text.Trim().Trim()))
            {
                msg = "请输入理由！";
            }
            else
            {
                #region 1.获取修改的数据

                StringBuilder updateOrder = new StringBuilder();
                StringBuilder Contents = new StringBuilder();
                string tempStr = ""; //变量

                Tb_Ticket_Order mOrder = ViewState["mOrder"] as Tb_Ticket_Order;

                //赋值：实体
                //Tb_Ticket_SkyWay Tb_Ticket_Order = null;
                //newSkyWay = this.baseDataManage.CallMethod("Tb_Ticket_Order", "CopyModel", null, new object[] { mOrder }) as Tb_Ticket_Order;


                //修改理由 
                tempStr = txtOrderReason.Text.Trim().Trim();
                Contents.Append("修改订单数据(理由):" + tempStr + "<br />");

                //PNR 
                tempStr = txtPNR.Text.Trim();
                if (mOrder.PNR != tempStr)
                {
                    updateOrder.Append(" PNR='" + tempStr + "',");
                    Contents.Append("(PNR) 原:" + mOrder.PNR + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //订单金额 
                tempStr = txtPayMoney.Text.Trim();
                if (mOrder.PayMoney != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" PayMoney='" + tempStr + "',");
                    Contents.Append("(订单金额) 原:" + mOrder.PayMoney + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //订单来源 
                tempStr = ddlOrderSourceType.SelectedValue;
                if (mOrder.OrderSourceType != int.Parse(tempStr))
                {
                    updateOrder.Append(" OrderSourceType='" + tempStr + "',");
                    //Contents.Append("(订单来源) 原:" + mOrder.OrderSourceType + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    Contents.Append("(订单来源) 原:" + GetDictionaryName("33", mOrder.OrderSourceType.ToString()) + ",新:<span style='color:Red;'>" + GetDictionaryName("33", tempStr) + "</span><br />");
                }

                //政策来源 
                tempStr = ddlPolicySource.SelectedValue;
                if (mOrder.PolicySource != int.Parse(tempStr))
                {
                    updateOrder.Append(" PolicySource='" + tempStr + "',");
                    //Contents.Append("(政策来源) 原:" + mOrder.PolicySource + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    Contents.Append("(政策来源) 原:" + GetDictionaryName("24", mOrder.PolicySource.ToString()) + ",新:<span style='color:Red;'>" + GetDictionaryName("24", tempStr) + "</span><br />");
                }

                //订单状态 
                tempStr = ddlOrderStatusCode.SelectedValue;
                if (mOrder.OrderStatusCode != int.Parse(tempStr))
                {

                    updateOrder.Append(" OrderStatusCode='" + tempStr + "',");
                    //Contents.Append("(订单状态) 原:" + mOrder.OrderStatusCode + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    Contents.Append("(订单状态) 原:" + GetDictionaryName("1", mOrder.OrderStatusCode.ToString()) + ",新:<span style='color:Red;'>" + GetDictionaryName("1", tempStr) + "</span><br />");
                }

                //支付方式 
                tempStr = ddlPayWay.SelectedValue;
                if (mOrder.PayWay != int.Parse(tempStr))
                {
                    updateOrder.Append(" PayWay='" + tempStr + "',");
                    //Contents.Append("(支付方式) 原:" + mOrder.PayWay + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    Contents.Append("(支付方式) 原:" + GetDictionaryName("4", mOrder.PayWay.ToString()) + ",新:<span style='color:Red;'>" + GetDictionaryName("4", tempStr) + "</span><br />");
                }

                //内部交易流水号 
                tempStr = txtInPayNo.Text.Trim();
                if (mOrder.InPayNo != tempStr)
                {
                    updateOrder.Append(" InPayNo='" + tempStr + "',");
                    Contents.Append("(内部交易流水号) 原:" + mOrder.InPayNo + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //支付流水号 
                tempStr = txtPayNo.Text.Trim();
                if (mOrder.PayNo != tempStr)
                {
                    updateOrder.Append(" PayNo='" + tempStr + "',");
                    Contents.Append("(支付流水号) 原:" + mOrder.PayNo + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //原政策点数 
                tempStr = txtOldPolicyPoint.Text.Trim();
                if (mOrder.OldPolicyPoint != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" PolicyPoint='" + tempStr + "',");
                    Contents.Append("(原政策点数) 原:" + mOrder.OldPolicyPoint + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //出票政策点数 
                tempStr = txtPolicyPoint.Text.Trim();
                if (mOrder.PolicyPoint != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" PolicyPoint='" + tempStr + "',");
                    Contents.Append("(原政策点数) 原:" + mOrder.PolicyPoint + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //最终政策点数 
                tempStr = txtReturnPoint.Text.Trim();
                if (mOrder.ReturnPoint != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" PayNo='" + tempStr + "',");
                    Contents.Append("(最终政策点数) 原:" + mOrder.ReturnPoint + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //原现返 
                tempStr = txtOldReturnMoney.Text.Trim();
                if (mOrder.OldReturnMoney != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" ReturnMoney='" + tempStr + "',");
                    Contents.Append("(政策现返金额) 原:" + mOrder.OldReturnMoney + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //出票现返 
                tempStr = txtReturnMoney.Text.Trim();
                if (mOrder.ReturnMoney != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" ReturnMoney='" + tempStr + "',");
                    Contents.Append("(政策现返金额) 原:" + mOrder.ReturnMoney + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //出票方收款金额 
                tempStr = txtOrderMoney.Text.Trim();
                if (mOrder.OrderMoney != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" OrderMoney='" + tempStr + "',");
                    Contents.Append("(出票方收款金额) 原:" + mOrder.OrderMoney + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //废、改处理时间 
                tempStr = txtFGQ.Text.Trim();
                if (mOrder.PolicyCancelTime != tempStr)
                {
                    updateOrder.Append(" PolicyCancelTime='" + tempStr + "',");
                    Contents.Append("(废、改处理时间) 原:" + mOrder.PolicyCancelTime + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //支付状态
                tempStr = ddlPayStatus.SelectedValue;
                if (mOrder.PayStatus != int.Parse(tempStr))
                {
                    updateOrder.Append(" PayStatus='" + tempStr + "',");
                    Contents.Append("(支付状态) 原:" + (mOrder.PayStatus == 0 ? "未付" : "已付") + ",新:<span style='color:Red;'>" + (tempStr == "0" ? "未付" : "已付") + "</span><br />");
                }

                //客规参考
                tempStr = txtKeGui.Text.Trim();
                if (mOrder.KeGui != tempStr)
                {
                    updateOrder.Append(" KeGui='" + tempStr + "',");
                    Contents.Append("(客规参考) 原:" + mOrder.KeGui + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                //政策备注
                tempStr = txtPolicyRemark.Text.Trim();
                if (mOrder.PolicyRemark != tempStr)
                {
                    updateOrder.Append(" PolicyRemark='" + tempStr + "',");
                    Contents.Append("(政策备注) 原:" + mOrder.PolicyRemark + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                // 外部订单号
                tempStr = txtOutOrderId.Text.Trim();
                if (mOrder.OutOrderId != tempStr)
                {
                    updateOrder.Append(" OutOrderId='" + tempStr + "',");
                    Contents.Append("(外部订单号) 原:" + mOrder.OutOrderId + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                // 代付状态
                tempStr = ddlOutOrderPayFlag.SelectedValue;
                if (mOrder.OutOrderPayFlag != ((tempStr == "1") ? true : false))
                {
                    updateOrder.Append(" OutOrderPayFlag='" + tempStr + "',");
                    //Contents.Append("(代付状态) 原:" + mOrder.OutOrderPayFlag + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                    Contents.Append("(代付状态) 原:" + (mOrder.OutOrderPayFlag == false ? "未付" : "已付") + ",新:<span style='color:Red;'>" + (tempStr == "0" ? "未付" : "已付") + "</span><br />");
                }

                // 外部订单代付金额
                tempStr = txtOutOrderPayMoney.Text.Trim();
                if (mOrder.OutOrderPayMoney != decimal.Parse(tempStr))
                {
                    updateOrder.Append(" OutOrderPayMoney='" + tempStr + "',");
                    Contents.Append("(外部订单代付金额) 原:" + mOrder.OutOrderPayMoney + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                // 外部订单支付流水号
                tempStr = txtOutOrderPayNo.Text.Trim();
                if (mOrder.OutOrderPayNo != tempStr)
                {
                    updateOrder.Append(" OutOrderPayNo='" + tempStr + "',");
                    Contents.Append("(外部订单支付流水号) 原:" + mOrder.OutOrderPayNo + ",新:<span style='color:Red;'>" + tempStr + "</span><br />");
                }

                // (支付/退废)状态检查
                tempStr = ddlA9.SelectedValue;
                if (mOrder.A9 != tempStr)
                {
                    updateOrder.Append(" A9='" + tempStr + "',");
                    Contents.Append("((支付/退废)检查) 原:" + (mOrder.A9 == "1" ? "不检查" : "要检查") + ",新:<span style='color:Red;'>" + (tempStr == "1" ? "不检查" : "要检查") + "</span><br />");
                }

                #endregion

                #region 2.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = txtOrderId.Text.Trim().Trim();
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = Contents.ToString();
                OrderLog.WatchType = 1;

                #endregion

                string sql = "";

                if (updateOrder == null)
                {
                    msg = "没有修改过数据！";
                }
                else
                {
                    string setWhere = updateOrder.ToString().TrimEnd(',');

                    sql = "update Tb_Ticket_Order set " + setWhere + " where OrderId='" + txtOrderId.Text.Trim().Trim() + "'";
                }

                if (string.IsNullOrEmpty(msg))
                {
                    List<string> sqlList = new List<string>();
                    sqlList.Add(sql);
                    result = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().UpdateOrder(sqlList, OrderLog);
                }
            }
            #endregion

        }
        catch (Exception ex)
        {

        }

        if (result == true)
        {
            msg = "更新成功!";
            //OrderBind();

            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');location.href='" + Hid_GoUrl.Value + "'", true);
        }
        else
        {
            msg = string.IsNullOrEmpty(msg) ? "更新失败!" : msg;

            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetTime(object obj, int type)
    {
        string msg = "";
        try
        {
            if (type == 1)
            {
                msg = obj != null ? DateTime.Parse(obj.ToString()).ToString("yyyy-MM-dd HH:mm") : "";
            }
        }
        catch (Exception ex)
        {

        }

        return msg;
    }
}