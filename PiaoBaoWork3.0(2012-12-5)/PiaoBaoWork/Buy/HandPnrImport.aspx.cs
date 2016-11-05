using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.Logic.Order;
using PnrAnalysis.Model;
using System.Collections;
using PbProject.Logic.Pay;
public partial class Buy_HandPnrImport : BasePage
{
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
    private string UnEscape(string s)
    {

        string str = s.Remove(0, 2);//删除最前面两个＂%u＂
        string[] strArr = str.Split(new string[] { "%u" }, StringSplitOptions.None);//以子字符串＂%u＂分隔
        byte[] byteArr = new byte[strArr.Length * 2];
        for (int i = 0, j = 0; i < strArr.Length; i++, j += 2)
        {
            byteArr[j + 1] = Convert.ToByte(strArr[i].Substring(0, 2), 16);  //把十六进制形式的字串符串转换为二进制字节
            byteArr[j] = Convert.ToByte(strArr[i].Substring(2, 2), 16);
        }
        str = System.Text.Encoding.Unicode.GetString(byteArr);　//把字节转为unicode编码
        return str;
    }


    /// <summary>
    /// 乘机人和证件类型列表 订单状态 订单来源类型
    /// </summary>
    List<Bd_Base_Dictionary> PasAndCardTypeList = new List<Bd_Base_Dictionary>();
    /// <summary>
    /// 航段城市
    /// </summary>
    List<Bd_Air_AirPort> CityList = new List<Bd_Air_AirPort>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
        }
        if (Request["create"] != null && Request["create"].ToString() == "1")
        {
            Create_Click();
        }
        else
        {
            if (!IsPostBack)
            {
                if (Context.Handler is IObject)
                {
                    PnrImportParam pageobj = (PnrImportParam)(Context.Handler as IObject).PageObj;
                    if (pageobj != null && pageobj.OrderParam.OrderParamModel.Count > 0)
                    {
                        // ViewState["param"] = pageobj;
                        ShowData(pageobj);
                    }
                    else
                    {
                        //if (ViewState["param"] != null)
                        //{
                        //    pageobj = ViewState["param"] as PnrImportParam;
                        //    ShowData(pageobj);
                        //}
                        //else
                        //{
                        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('传入对象为空！');", true);
                        //}
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('对象为空！');", true);
                }
            }
        }
    }
    /// <summary>
    /// 显示绑定数据
    /// </summary>
    /// <param name="Param"></param>
    public void ShowData(PnrImportParam Param)
    {
        //初始化页面需要操作的航段和乘机人实体信息
        Hid_SkyModel.Value = escape(JsonHelper.ObjToJson<Tb_Ticket_SkyWay>(Param.OrderParam.OrderParamModel[0].SkyList[0]));
        Hid_PasModel.Value = escape(JsonHelper.ObjToJson<Tb_Ticket_Passenger>(Param.OrderParam.OrderParamModel[0].PasList[0]));
        //编码信息
        Hid_ALLInfo.Value = Escape(JsonHelper.ObjToJson<PnrImportParam>(Param));

        //显示数据
        lblCustomer.Text = Param.m_UserInfo.LoginName + "-" + Param.m_CurCompany.UninAllName;
        lblCpyNo.Text = Param.m_CurCompany.UninCode;
        Hid_OrderType.Value = Param.OrderParam.OrderParamModel[0].Order.IsChdFlag ? "1" : "0";
        if (PasAndCardTypeList.Count == 0)
        {
            string sqlWhere = " parentid in(1,6,7,33) order by ChildID";
            PasAndCardTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }
        if (CityList.Count == 0)
        {
            CityList = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=1" }) as List<Bd_Air_AirPort>;
        }
        //绑定状态
        BindData();
        //绑定城市
        BindCity();

        txtBirday_0.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
    }
    /// <summary>
    /// 字典表绑定
    /// </summary>
    public void BindData()
    {
        if (PasAndCardTypeList != null && PasAndCardTypeList.Count > 0)
        {
            pas_type_0.Items.Clear();//绑定乘客类型
            pas_cardtype_0.Items.Clear();// 绑定证件号类型
            o_orderstatuscode.Items.Clear();//绑定订单状态
            o_ordersource.Items.Clear();//  绑定订单来源类型
            foreach (Bd_Base_Dictionary item in PasAndCardTypeList)
            {
                if (6 == item.ParentID)//绑定乘客类型
                {
                    ListItem lim = new ListItem();
                    lim.Text = item.ChildName;
                    lim.Value = item.ChildID.ToString();
                    if (Hid_OrderType.Value == "0" && !item.ChildName.Contains("儿童"))
                    {
                        pas_type_0.Items.Add(lim);
                    }
                    else
                    {
                        if (Hid_OrderType.Value == "1" && item.ChildName.Contains("儿童"))
                        {
                            pas_type_0.Items.Add(lim);
                        }
                    }
                }
                else if (7 == item.ParentID)//绑定证件号类型
                {
                    ListItem lim = new ListItem();
                    lim.Text = item.ChildName;
                    lim.Value = item.ChildID.ToString();
                    pas_cardtype_0.Items.Add(lim);
                }
                else if (1 == item.ParentID)//绑定订单状态
                {
                    ListItem lim = new ListItem();
                    lim.Text = item.ChildName;
                    lim.Value = item.ChildID.ToString();
                    o_orderstatuscode.Items.Add(lim);
                }
                else if (33 == item.ParentID)//绑定订单来源类型
                {
                    ListItem lim = new ListItem();
                    lim.Text = item.ChildName;
                    lim.Value = item.ChildID.ToString();
                    o_ordersource.Items.Add(lim);
                }
            }
        }
    }
    /// <summary>
    /// 绑定城市
    /// </summary>
    public void BindCity()
    {
        if (CityList.Count == 0)
        {
            CityList = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=1" }) as List<Bd_Air_AirPort>;
        }

        //排序 
        SortedList sortLst = new SortedList();
        foreach (Bd_Air_AirPort city in CityList)
        {
            ListItem item = new ListItem();
            item.Text = city.CityCodeWord + "-" + city.CityName;
            item.Value = city.CityCodeWord;
            if (!sortLst.ContainsKey(city.CityCodeWord))
            {
                sortLst.Add(city.CityCodeWord, item);
            }
        }
        ListItem[] newItem = new ListItem[sortLst.Values.Count];
        sortLst.Values.CopyTo(newItem, 0);
        ddlFromCity_0.Items.Clear();
        ddlToCity_0.Items.Clear();
        //ddlFromCity_0.Items.Add(new ListItem("--出发城市--", ""));
        ddlFromCity_0.Items.AddRange(newItem);
        //ddlToCity_0.Items.Add(new ListItem("--到达城市--", ""));
        ddlToCity_0.Items.AddRange(newItem);
    }
    public void OutPut(string result)
    {
        Response.Clear();
        Response.ContentType = "text/plain";
        Response.Write(result);
        Response.Flush();
        Response.End();
    }

    //生成订单
    public void Create_Click()
    {
        string Msg = "";
        bool IsSuc = false;
        string ErrMsg = "";
        try
        {
            if (mCompany == null)
            {
                Msg = "0##订单生成成失败！原因如下<br />页面已过期，请刷新页面或者重新登录！##";
            }
            else
            {
                //订单管理           
                Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
                string data = Request["data"].ToString();
                PnrImportParam PImport = JsonHelper.JsonToObj<PnrImportParam>(data);
                if (PImport != null)
                {
                    OrderMustParamModel item = PImport.OrderParam.OrderParamModel[0];

                    //航段---------------------------------------------------------
                    List<string> CarryCodeList = new List<string>();
                    List<string> FlightCodeList = new List<string>();
                    List<string> TravelList = new List<string>();
                    List<string> AirTimeList = new List<string>();
                    List<string> TravelCodeList = new List<string>();
                    List<string> SpaceList = new List<string>();
                    List<string> DisCountList = new List<string>();
                    for (int i = 0; i < item.SkyList.Count; i++)
                    {
                        CarryCodeList.Add(item.SkyList[i].CarryCode);
                        FlightCodeList.Add(item.SkyList[i].FlightCode);
                        AirTimeList.Add(item.SkyList[i].FromDate.ToString("yyyy-MM-dd HH:mm"));
                        TravelList.Add(item.SkyList[i].FromCityName + "-" + item.SkyList[i].ToCityName);
                        TravelCodeList.Add(item.SkyList[i].FromCityCode + "-" + item.SkyList[i].ToCityCode);
                        SpaceList.Add(item.SkyList[i].Space);
                        DisCountList.Add(item.SkyList[i].Discount);
                    }
                    item.Order.CarryCode = string.Join("/", CarryCodeList.ToArray());
                    item.Order.FlightCode = string.Join("/", FlightCodeList.ToArray());
                    item.Order.AirTime = DateTime.Parse(AirTimeList[0]);
                    item.Order.Travel = string.Join("/", TravelList.ToArray());
                    item.Order.TravelCode = string.Join("/", TravelCodeList.ToArray());
                    item.Order.Space = string.Join("/", SpaceList.ToArray());
                    item.Order.Discount = string.Join("/", DisCountList.ToArray());
                    //------------------------------------------------------------------------------------
                    //备注
                    item.Order.YDRemark = HttpUtility.UrlDecode(Request["Remark"].ToString(), Encoding.Default);//txtRemark.Value.Trim();//订单生成备注
                    //订单中的总价
                    decimal TotalPMPrice = 0m, TotalABFare = 0, TotalRQFare = 0m;
                    decimal PolicyMoney = 0m;//订单佣金
                    Data d = new Data(this.mUser.CpyNo);//采购佣金进舍规则: 0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分

                    //乘客人数
                    item.Order.PassengerNumber = item.PasList.Count;
                    //乘客姓名 已"|"分割
                    List<string> PasNameList = new List<string>();
                    //乘机人实体处理
                    for (int j = 0; j < item.PasList.Count; j++)
                    {
                        //订单价格
                        TotalPMPrice += item.PasList[j].PMFee;
                        TotalABFare += item.PasList[j].ABFee;
                        TotalRQFare += item.PasList[j].FuelFee;
                        if (item.PasList[j].PassengerType == 1 || item.PasList[j].PassengerType == 2)
                        {
                            PolicyMoney = d.CreateCommissionCG(item.PasList[j].PMFee, item.Order.PolicyPoint);
                        }
                        //订单中的乘客显示
                        PasNameList.Add(item.PasList[j].PassengerName);
                    }
                    //乘客姓名
                    item.Order.PassengerName = string.Join("|", PasNameList.ToArray());

                    //订单赋值
                    item.Order.PMFee = TotalPMPrice;
                    item.Order.ABFee = TotalABFare;
                    item.Order.FuelFee = TotalRQFare;
                    //政策返点
                    item.Order.PolicyPoint = item.Order.PolicyPoint;
                    //最终返点
                    item.Order.ReturnPoint = item.Order.ReturnPoint;
                    //原始政策返点
                    item.Order.OldPolicyPoint = item.Order.PolicyPoint;
                    if (mCompany.RoleType == 1)
                    {
                        //出票公司编号
                        item.Order.CPCpyNo = string.IsNullOrEmpty(PImport.m_SupCompany.UninCode) ? PImport.m_SupCompany.UninCode : "";
                    }
                    else
                    {
                        //出票公司编号
                        item.Order.CPCpyNo = mCompany.UninCode;
                    }
                    //出票公司编号                    
                    item.Order.CPCpyNo = item.Order.CPCpyNo.Length > 12 ? item.Order.CPCpyNo.Substring(0, 12) : item.Order.CPCpyNo;
                    //item.Order.PayMoney = d.CreateOrderPayMoney(item.Order, item.PasList);//计算订单金额
                    //item.Order.OrderMoney = d.CreateOrderOrderMoney(item.Order, item.PasList);
                    Bill bill = new Bill();
                    bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);

                    //佣金
                    item.Order.PolicyMoney = PolicyMoney;
                    item.Order.A1 = 1;//已确认

                    //开启添加订单账单明细sql
                    PImport.OrderParam.IsCreatePayDetail = 1;
                    //判断金额是否正确
                    if (item.Order.PayMoney <= 0 || item.Order.OrderMoney <= 0 || ((item.Order.PayMoney + item.Order.PayMoney * 0.003M) < item.Order.OrderMoney))
                    {
                        Msg = "0##订单生成成失败！原因如下<br />订单金额有误##";
                    }
                    else
                    {
                        //生成订单
                        IsSuc = OrderManage.CreateOrder(ref PImport.OrderParam, out ErrMsg);
                        if (IsSuc)
                        {
                            Msg = "1##订单生成成功!##" + "订单号:" + PImport.OrderParam.OrderParamModel[0].Order.OrderId;
                        }
                        else
                        {
                            Msg = "0##订单生成成失败！原因如下<br />" + ErrMsg + "##";
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Msg = "0##页面异常！原因如下<br />" + ex.Message + "##";
            //DataBase.LogCommon.Log.Error("HandPnrImport.aspx:" + ErrMsg, ex);
            PnrAnalysis.LogText.LogWrite("HandPnrImport.aspx:" + ErrMsg + "|" + ex.Message, "HandPnrImport");
        }
        finally
        {
            PnrAnalysis.LogText.LogWrite("HandPnrImport.aspx:" + Msg + "\r\n", "HandPnrImport");
            OutPut(Msg);
        }
    }
}