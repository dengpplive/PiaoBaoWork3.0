using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;
using PnrAnalysis;
using System.Text.RegularExpressions;
using PbProject.Model;
using PbProject.Logic.Order;
using PbProject.Logic.Pay;
using PnrAnalysis.Model;
using PbProject.WebCommon.Utility;
using PbProject.Model.definitionParam;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.Logic.ControlBase;
using PbProject.Logic.PID;
using PbProject.Logic.Policy;
/// <summary>
/// 生成订单（确定订单页面）
/// </summary>
public partial class Buy_Confirmation : BasePage
{
    /// <summary>
    /// 订单管理操作类
    /// </summary>
    Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();
    /// <summary>
    /// 乘机人和证件类型列表
    /// </summary>
    List<Bd_Base_Dictionary> PasAndCardTypeList = new List<Bd_Base_Dictionary>();

    #region 参数
    /// <summary>
    /// 成人订单号
    /// </summary>
    public string AdultOrderId
    {
        get { return (string)ViewState["AdultOrderId"]; }
        set { ViewState["AdultOrderId"] = value; }
    }
    /// <summary>
    /// 儿童订单号
    /// </summary>
    public string ChildOrderId
    {
        get { return (string)ViewState["ChildOrderId"]; }
        set { ViewState["ChildOrderId"] = value; }
    }
    /// <summary>
    /// 结果参数
    /// </summary>
    public string Result
    {
        get { return (string)ViewState["Result"]; }
        set { ViewState["Result"] = value; }
    }
    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
            }
            return result;
        }
    }
    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
            }
            return result;
        }
    }
    /// <summary>
    /// 平台接口是否低开 1是0 否
    /// </summary>
    //public string IsInterfaceLower
    //{
    //    get
    //    {
    //        return KongZhiXiTong.Contains("|60|") ? "1" : "0";
    //    }
    //}
    /// <summary>
    /// 是否开启大配置
    /// </summary>
    public bool IsOpenBigConfig
    {
        get
        {
            return KongZhiXiTong.Contains("|39|");
        }
    }
    /// <summary>
    /// 接口不出儿童票是否显示 0默认显示 1不显示
    /// </summary>
    public string ChildNotTicketIsShowRemark
    {
        get
        {
            return "0"; //BaseParams.getParams(baseParametersList).ChildNotTicketIsShowRemark ? "0" : "1";
        }
    }
    #endregion



    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string ErrMsg = "";
        try
        {
            if (!IsPostBack)
            {
                this.currentuserid.Value = this.mUser.id.ToString();
                //匹配政策 是否在工作时间内
                bool b_IsInWorkTime = IsInWorkTime();
                Hid_IsInWorkTime.Value = b_IsInWorkTime ? "1" : "0";

                //初始化参数
                InitParam();
                //显示页面数据
                ShowPageData();
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message;
        }
        //if (ErrMsg != "")
        //{
        //    PnrAnalysis.LogText.LogWrite("确认页面加载:" + ErrMsg, "GoConfirmation");
        //}
    }

    /// <summary>
    /// 是否在工作时间内
    /// </summary>
    /// <returns></returns>
    public bool IsInWorkTime()
    {
        //是否在工作时间内 true是(默认) false否
        bool m_IsInWorkTime = true;
        try
        {
            string WorkTime = mSupCompany != null && !string.IsNullOrEmpty(mSupCompany.WorkTime) ? mSupCompany.WorkTime : "";
            string[] strArr = WorkTime.Split('-');
            if (strArr.Length != 2)
            {
                strArr = new string[2];
                strArr[0] = "00:01";
                strArr[1] = "23:59";
            }
            DateTime now = System.DateTime.Now;
            DateTime dt = new DateTime(now.Year, now.Month, now.Day);

            DateTime startTime = DateTime.Parse(dt.ToString("yyyy-MM-dd") + " " + strArr[0] + ":00");
            DateTime endTime = DateTime.Parse(dt.ToString("yyyy-MM-dd") + " " + strArr[1] + ":00");
            //当前时间不在设置的时间范围内
            if (DateTime.Compare(now, startTime) < 0 || DateTime.Compare(endTime, now) < 0)
            {
                //不在工作时间内
                m_IsInWorkTime = false;
            }
        }
        catch (Exception)
        {
        }
        return m_IsInWorkTime;
    }
    /// <summary>
    /// 初始化页面参数 或者传过来的数据
    /// </summary>
    public void InitParam()
    {

        if (Request["AdultOrderId"] != null)
        {
            AdultOrderId = Request["AdultOrderId"];
        }
        if (Request["ChildOrderId"] != null)
        {
            ChildOrderId = Request["ChildOrderId"];
        }
        if (PasAndCardTypeList.Count == 0)
        {
            string sqlWhere = " parentid in(6,7) order by ChildID";
            PasAndCardTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }
        //隐藏域赋值 平台接口是否低开
        //Hid_IsLower.Value = IsInterfaceLower;
        //接口不出儿童票是否显示 0默认显示 1不显示
        Hid_NotChildTicketShow.Value = ChildNotTicketIsShowRemark;
        //供应商Office
        //Hid_SupperOffice.Value = this.configparam.Office;
        //是否开启大配置
        Hid_IsOpenBigConfig.Value = IsOpenBigConfig ? "1" : "0";
        //扣点组Id
        Hid_GroupId.Value = string.IsNullOrEmpty(mCompany.GroupId) ? "" : mCompany.GroupId;
        //隐藏政策
        Hid_IsPolicy.Value = mUser.UserPower.Contains("|2|") ? "1" : "0";
        //是否开启显示后返权限
        Hid_IsHouFanOpen.Value = KongZhiXiTong != null && KongZhiXiTong.Contains("|36|") ? "1" : "0";
        //账户余额支付
        Hid_zhanghu.Value = (GongYingKongZhiFenXiao != null && GongYingKongZhiFenXiao.Contains("|76|")) ? "1" : "0";
        // 提示 有收银权限
        Hid_shouying.Value = GongYingKongZhiFenXiao != null && GongYingKongZhiFenXiao.Contains("|79|") ? "1" : "0";
    }
    /// <summary>
    /// 获取证件号和乘客类型
    /// </summary>
    /// <param name="ParentId"></param>
    /// <param name="ChildId"></param>
    /// <returns></returns>
    public string GetDictionaryName(int ParentId, int ChildId)
    {
        string reData = "";
        if (PasAndCardTypeList.Count == 0)
        {
            string sqlWhere = " parentid in(6,7) order by ChildID";
            PasAndCardTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }
        if (PasAndCardTypeList != null)
        {
            Bd_Base_Dictionary tempDic = PasAndCardTypeList.Find(delegate(Bd_Base_Dictionary _dic)
              {
                  return (_dic.ParentID == ParentId && _dic.ChildID == ChildId) ? true : false;
              });
            if (tempDic != null)
            {
                reData = tempDic.ChildName;
            }
        }
        return reData;
    }
    /// <summary>
    /// 获取政策实体列表
    /// </summary>
    /// <returns></returns>
    public AjaxPolicyMatchOutData GetPolicy()
    {
        AjaxPolicyMatchOutData APM = null;
        string strHidData = Hid_Data.Value;
        if (strHidData.Trim() != "")
        {
            string strData = HttpUtility.UrlDecode(strHidData, Encoding.Default);
            APM = JsonHelper.JsonToObj<AjaxPolicyMatchOutData>(strData);
        }
        return APM;
    }
    /// <summary>
    /// 获取婴儿价格信息
    /// </summary>
    /// <returns></returns>
    public PatInfo GetINFPAT()
    {
        string strPrice = Hid_INFGDPrice.Value;
        string[] strArr = strPrice.Split('@');
        PatInfo pat = null;
        if (strArr.Length > 0)
        {
            string[] strData = strArr[0].Split('|');
            if (strData.Length == 3)
            {
                pat = new PatInfo();
                decimal SeatPrice = 0m, ABFare = 0m, RQFare = 0m;
                decimal.TryParse(strData[0], out SeatPrice);
                decimal.TryParse(strData[1], out ABFare);
                decimal.TryParse(strData[2], out RQFare);
                pat.Fare = strData[0];
                pat.TAX = strData[1];
                pat.RQFare = strData[2];
                pat.Price = (SeatPrice + ABFare + RQFare).ToString();
                pat.PriceType = "3";
            }
        }
        return pat;
    }


    /// <summary>
    /// 显示页面数据
    /// </summary>
    private void ShowPageData()
    {
        OrderInputParam InputParam = new OrderInputParam();
        if (AdultOrderId != null)
        {
            string ErrMsg = "";
            //成人订单数据显示            
            InputParam = OrderBLL.GetOrder(AdultOrderId, InputParam, out ErrMsg);
        }
        if (ChildOrderId != null)
        {
            //儿童订单数据显示
            string ErrMsg = "";
            //儿童订单数据显示            
            InputParam = OrderBLL.GetOrder(ChildOrderId, InputParam, out ErrMsg);
        }
        string SpecialType = Request["SpecialType"] != null ? Request["SpecialType"].ToString() : "0";
        if (InputParam.OrderParamModel.Count > 0)
        {
            //存入视图 用于修改按钮操作
            ViewState["OrderParamModel"] = InputParam;
            //将航段信息存入缓存 配政策使用
            PbProject.Logic.Buy.AirQurey airqurey = new PbProject.Logic.Buy.AirQurey();
            //缓存使用的GUID
            Hid_CacheGUID.Value = airqurey.SkyListSaveCache(InputParam, SpecialType);
            #region 显示部分
            //显示PNR
            ShowPnr(InputParam);
            List<Tb_Ticket_SkyWay> SkyWayList = InputParam.OrderParamModel[0].SkyList;
            //显示乘客
            ShowPasList(InputParam.OrderParamModel);
            //显示政策价格
            PriceShow(InputParam);
            //显示航段信息
            ShowSkyWay(SkyWayList);
            #endregion
        }
    }
    /// <summary>
    /// 显示PNR
    /// </summary>
    /// <param name="InputParam"></param>
    public void ShowPnr(OrderInputParam InputParam)
    {
        StringBuilder sbPnr = new StringBuilder();
        if (InputParam.PnrInfo != null)
        {
            if (InputParam.PnrInfo.AdultPnr != "" && InputParam.PnrInfo.AdultPnr.Length == 6)
            {
                sbPnr.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;成人编码：" + InputParam.PnrInfo.AdultPnr);
            }
            if (InputParam.PnrInfo.childPnr != "" && InputParam.PnrInfo.childPnr.Length == 6)
            {
                sbPnr.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;儿童编码：" + InputParam.PnrInfo.childPnr);
            }

        }
        lblPnr.Text = sbPnr.ToString();
    }
    /// <summary>
    /// 显示航段信息
    /// </summary>
    /// <param name="SkyWayList"></param>
    public void ShowSkyWay(List<Tb_Ticket_SkyWay> SkyWayList)
    {
        StringBuilder SkyWayStr = new StringBuilder();
        decimal zk = 0;
        Tb_Ticket_SkyWay sw1 = null;
        Tb_Ticket_SkyWay sw2 = null;
        int i = 0;
        string strCarrayCode = "";//航空公司
        string strSpace = "";//舱位
        foreach (Tb_Ticket_SkyWay sw in SkyWayList)
        {
            if (sw1 == null && i == 0)
            {
                sw1 = sw;
            }
            if (sw2 == null && i == 1)
            {
                sw2 = sw;
            }
            //航程
            SkyWayStr.AppendFormat("<td style=\"font-weight:bold;color:#404040;\">{0}({1})-{2}({3})</td>", sw.FromCityName, sw.FromCityCode, sw.ToCityName, sw.ToCityCode);
            //起飞日期
            SkyWayStr.AppendFormat("<td>{0}</td>", sw.FromDate.ToString("yyyy-MM-dd"));
            //起抵时间
            SkyWayStr.AppendFormat("<td style=\"font-weight:bold;color:#404040;\">{0}-{1}</td>", sw.FromDate.ToShortTimeString(), sw.ToDate.ToShortTimeString());
            //承运人 航班号 机型
            SkyWayStr.AppendFormat("<td>{0}</td><td>{1}</td><td><span id=\"JX_Body\">{2}</span></td>", sw.CarryCode, sw.FlightCode, sw.Aircraft);
            //舱位
            zk = decimal.Parse(string.IsNullOrEmpty(sw.Discount) ? "0" : sw.Discount);
            SkyWayStr.AppendFormat("<td style=\"font-weight:bold;color:#404040;\">{0}/{1}</td>", sw.Space, zk <= 0 ? "特价" : zk.ToString());
            SkyWayStr.Append("</tr>");
            strSpace += sw.Space + "/";
            strCarrayCode = sw.CarryCode;
            i++;
        }
        Hid_Space.Value = strSpace.Trim('/');
        Hid_CarrayCode.Value = strCarrayCode;

        if (sw1 != null)
        {
            //出发城市
            Hid_FromCode.Value = sw1.FromCityCode;
            //到达城市
            Hid_ToCode.Value = sw1.ToCityCode;
            //出发日期
            Hid_FromDate.Value = sw1.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
            //到达日期
            Hid_ToDate.Value = sw1.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
        if (Hid_TravelType.Value == "3" && sw2 != null)
        {
            //中转联程
            Hid_MiddleCode.Value = sw2.FromCityCode;
            Hid_ToCode.Value = sw2.ToCityCode;

            //出发日期
            Hid_FromDate.Value = sw1.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
            //到达日期
            Hid_ToDate.Value = sw2.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
        literSKY.Text = SkyWayStr.ToString();
    }
    /// <summary>
    /// 显示乘客
    /// </summary>
    /// <param name="PMList"></param>
    public void ShowPasList(List<OrderMustParamModel> PMList)
    {
        StringBuilder PassengerStr = new StringBuilder();
        foreach (OrderMustParamModel item in PMList)
        {
            foreach (Tb_Ticket_Passenger pas in item.PasList)
            {
                PassengerStr.Append("<tr>");
                PassengerStr.AppendFormat("<td style='text-align:left;padding-left:3%;'><span style='font-weight:bold'>乘机人</span>：{0}</td>", pas.PassengerName);
                PassengerStr.AppendFormat("<td style='text-align:left;padding-left:3%;'><span style='font-weight:bold'>乘客类型</span>：{0}</td>", GetDictionaryName(6, pas.PassengerType));

                if (Regex.IsMatch(pas.Cid, @"^\d{4}\-\d{2}\-\d{2}$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    PassengerStr.Append("<td style='text-align:left;padding-left:3%;'><span style='font-weight:bold'>证件类型</span>：出生日期</td>");
                }
                else
                {
                    PassengerStr.AppendFormat("<td style='text-align:left;padding-left:3%;'><span style='font-weight:bold'>证件类型</span>：{0}</td>", GetDictionaryName(7, int.Parse(pas.CType)));
                }
                PassengerStr.AppendFormat("<td style='text-align:left;padding-left:3%;'><span style='font-weight:bold'>证件号码</span>：{0}</td>", pas.Cid);
                PassengerStr.Append("</tr>");
            }
        }
        literPessStr.Text = PassengerStr.ToString();
    }


    /// <summary>
    /// 政策价格显示
    /// </summary>
    /// <param name="InputParam"></param>
    public void PriceShow(OrderInputParam InputParam)
    {
        RePnrObj PnrInfo = InputParam.PnrInfo;
        Tb_Ticket_Order AdultOrder = null;
        Tb_Ticket_Order ChildOrder = null;
        List<Tb_Ticket_Passenger> AdultPasList = null;//包含婴儿
        List<Tb_Ticket_Passenger> ChildPasList = null;//儿童
        List<string> ParamList = new List<string>();
        decimal YFare = 0m;
        foreach (OrderMustParamModel OM in InputParam.OrderParamModel)
        {
            if (OM.Order.IsChdFlag)
            {
                ChildOrder = OM.Order;
                ChildPasList = OM.PasList;
                if (OM.Order.IsCHDETAdultTK == 1 && OM.Order.OrderSourceType == 1)
                {
                    Hid_IsCHDETAdultTK.Value = "1";//是否儿童出成人票
                }
            }
            else
            {
                AdultOrder = OM.Order;
                AdultPasList = OM.PasList;
                YFare = OM.SkyList[0].FareFee;
            }
        }
        if (AdultOrder != null && AdultPasList != null && AdultPasList.Count > 0)
        {
            //成人
            Hid_OrderFlag.Value = "0";
            //行程类型
            Hid_TravelType.Value = AdultOrder.TravelType.ToString();
            //成人订单号
            Hid_OrderID.Value = AdultOrder.OrderId;
            SetAdultHidPrice(AdultOrder, AdultPasList, PnrInfo, YFare);
            ParamList.Add("id=" + AdultOrder.id);
            //联系人和电话
            lblLinkMan.Text = AdultOrder.LinkMan;
            lblLinkPhone.Text = AdultOrder.LinkManPhone;
            //订单来源
            Hid_OrderSourceType.Value = AdultOrder.OrderSourceType.ToString();

            //用于b2b政策
            Hid_Pnr.Value = AdultOrder.PNR;
            Hid_BigPnr.Value = AdultOrder.BigCode;
            Hid_Office.Value = AdultOrder.Office;
        }
        if (ChildOrder != null && ChildPasList != null && ChildPasList.Count > 0)
        {
            //儿童
            Hid_OrderFlag.Value = "1";
            if (ParamList.Count == 0)
            {
                ParamList.Add("id=" + ChildOrder.id);
            }
            //联系人和电话
            lblLinkMan.Text = ChildOrder.LinkMan;
            lblLinkPhone.Text = ChildOrder.LinkManPhone;
            //订单来源
            Hid_OrderSourceType.Value = ChildOrder.OrderSourceType.ToString();
        }
        //用于跳转到支付页面
        Result = string.Join("&", ParamList.ToArray());
        //页面上订单标识
        if (AdultOrder != null && ChildOrder != null)
        {
            //成人和儿童都有
            Hid_OrderFlag.Value = "2";
            Hid_TravelType.Value = AdultOrder.TravelType.ToString();
        }
    }
    /// <summary>
    /// 设置成人政策价格显示 低价格@高价格@成人Y仓一半价格(儿童不出成人票价格)
    /// </summary>
    /// <param name="AdultOrder"></param>
    /// <param name="AdultPasList"></param>
    /// <param name="PnrInfo"></param>
    /// <param name="YFare">航段表中的Y舱价格</param>
    public void SetAdultHidPrice(Tb_Ticket_Order AdultOrder, List<Tb_Ticket_Passenger> AdultPasList, RePnrObj PnrInfo, decimal YFare)
    {
        Tb_Ticket_Passenger AdultPas = AdultPasList.Find(delegate(Tb_Ticket_Passenger p1)
        {
            return p1.PassengerType == 1;
        });
        Tb_Ticket_Passenger INFPas = AdultPasList.Find(delegate(Tb_Ticket_Passenger p1)
        {
            return p1.PassengerType == 3;
        });
        PnrModel PnrM = PnrInfo.PnrList[0];
        #region 成人
        if (AdultPas != null)
        {
            PatModel PatM = PnrInfo.PatModelList[0];
            if (PatM != null && PatM.PatList.Count > 0)
            {
                Hid_AdultPriceCount.Value = PatM.PatPriceCount.ToString();
            }
        }
        #endregion

        if (INFPas != null)
        {
            decimal Fare = INFPas.PMFee;//舱位价
            decimal TAX = INFPas.ABFee;//机建
            decimal RQFare = INFPas.FuelFee;//燃油  
            //不是白屏预订和PNR导入处理
            if (AdultOrder.OrderSourceType != 1 && AdultOrder.OrderSourceType != 2)
            {
                Fare = YFare * (0.1m);//舱位价
                TAX = 0m;//机建
                RQFare = 0m;//燃油  
            }
            string strPrice = Fare + "|" + TAX + "|" + RQFare + "@" + Fare + "|" + TAX + "|" + RQFare + "@" + Fare + "|" + TAX + "|" + RQFare + "@";
            PatModel PatM = PnrInfo.PatModelList[2];
            if (PatM != null && PatM.PatList.Count > 0)
            {
                PatInfo patFirst = PatM.PatList[0];//低价格
                PatInfo patLast = PatM.PatList[PatM.PatList.Count - 1];//高价格
                PatInfo PatYCH = PatM.ChildYPat == null ? patFirst : PatM.ChildYPat;
                strPrice = string.Format("{0}|{1}|{2}@{3}|{4}|{5}@{6}|{7}|{8}", patFirst.Fare, patFirst.TAX, patFirst.RQFare, patLast.Fare, patLast.TAX, patLast.RQFare, PatYCH.Fare, PatYCH.TAX, PatYCH.RQFare);
                Hid_INFPriceCount.Value = PatM.PatPriceCount.ToString();
            }
            Hid_INFGDPrice.Value = strPrice;
            Hid_HasINF.Value = "1";//是否有婴儿
        }
    }

    /// <summary>
    /// 白屏预订 乘客婴儿个数与编码中解析出来的婴儿个数比较是否一致 给出个提示
    /// </summary>
    /// <returns></returns>
    public bool yudingINFCheck(RePnrObj PnrInfo, List<Tb_Ticket_Passenger> PasList)
    {
        bool IsCheck = false;
        int INFCount = 0;
        int tempINFCount = 0;
        try
        {
            foreach (Tb_Ticket_Passenger pas in PasList)
            {
                if (pas.PassengerType == 3)
                {
                    INFCount++;
                }
            }
            if (PnrInfo.PnrList.Length > 0 && PnrInfo.PnrList[0] != null)
            {
                foreach (PassengerInfo item in PnrInfo.PnrList[0]._PassengerList)
                {
                    if (item.PassengerType == "3")
                    {
                        tempINFCount++;
                    }
                }
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            if (INFCount != tempINFCount)
            {
                IsCheck = true;
            }
        }
        return IsCheck;
    }



    /// <summary>
    /// 确定订单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSub_Click(object sender, EventArgs e)
    {
        if (this.mSupCompany == null || this.mCompany == null || this.mUser == null || this.configparam == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showMsgDg('该网页已失效,请刷新页面或者重新登录后再操作!');", true);
            return;
        }
        #region 更新订单 主要修改价格,政策和添加订单账单明细
        Bill bill = new Bill();

        Data d = new Data(this.mUser.CpyNo);//采购佣金进舍规则: 0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分

        string ErrMsg = "";
        bool IsSuc = false;
        //扩展参数
        ParamEx pe = new ParamEx();
        pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
        //开启使用特价缓存  true 开启 false关闭
        bool IsUseSpCache = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|99|");
        //发送指令管理类
        SendInsManage SendIns = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, this.configparam);
        //提示
        StringBuilder sbTip = new StringBuilder();
        try
        {
            OrderInputParam InputParam = ViewState["OrderParamModel"] as OrderInputParam;
            AjaxPolicyMatchOutData APM = GetPolicy();
            //供应商Office
            string GYOffice = this.configparam != null ? this.configparam.Office.ToUpper() : "";
            if (InputParam != null && APM != null && APM.OutPutPolicyList != null && APM.OutPutPolicyList.Count > 0)
            {
                AjAxPolicyParam AdultPolicy = null;
                AjAxPolicyParam ChildPolicy = null;
                int adultIndex = -1, childIndex = -1;
                if (int.TryParse(Hid_SelIndex.Value, out adultIndex))
                {
                    AdultPolicy = APM.OutPutPolicyList[adultIndex];
                    if (AdultPolicy.DefaultType == "2")
                    {
                        AdultPolicy = null;
                    }
                }
                if (int.TryParse(Hid_SelChildIndex.Value, out childIndex))
                {
                    ChildPolicy = APM.OutPutPolicyList[childIndex];
                    if (ChildPolicy.DefaultType != "2")
                    {
                        ChildPolicy = null;
                    }
                }
                //至少选择一条政策 成人或者儿童
                if (AdultPolicy == null && ChildPolicy == null)
                {
                    ErrMsg = "请选择一条政策数据！";
                }

                if (ErrMsg == "")
                {
                    PatInfo INFPrice = null;
                    //编码解析类
                    PnrAnalysis.FormatPNR pnrformat = new PnrAnalysis.FormatPNR();
                    if (Hid_HasINF.Value == "1")
                    {
                        //婴儿价格
                        INFPrice = GetINFPAT();
                    }
                    //订单实体需要更改数据的字段名
                    List<string> UpdateOrderFileds = new List<string>();
                    //订单修改字段 为下面订单赋值需要将赋值的字段加入列表中 否则赋值无效
                    UpdateOrderFileds.AddRange(new string[] {"OldRerurnMoney","OldPolicyPoint","OutOrderPayMoney","A7","A13","A1","A11","PrintOffice", "CPCpyNo","YDRemark", "PolicyId", "AirPoint", "PolicyPoint", "ReturnPoint", "PolicyMoney","PolicyCancelTime","PolicyReturnTime",
                    "DiscountDetail","PolicyType","PolicySource","AutoPrintFlag","PolicyRemark","PMFee","ABFee","FuelFee","BabyFee" ,"PayMoney","OrderMoney","Space",
                    "JinriGYCode"                    
                    });
                    //航段实体需要更改数据的字段名
                    List<string> UpdateSkyWayFileds = new List<string>();
                    //航段修改字段
                    UpdateSkyWayFileds.AddRange(new string[] { "SpacePrice", "ABFee", "FuelFee", "Discount" });
                    //乘客实体需要更改数据的字段名
                    List<string> UpdatePasFileds = new List<string>();
                    //修改乘机人
                    UpdatePasFileds.AddRange(new string[] { "PMFee", "ABFee", "FuelFee" });
                    //承运人二字码
                    string CarrayCode = string.Empty;
                    string orderIDs = "";
                    //婴儿与编码中的婴儿个数是否不一致 true不一致 false一致
                    bool IsINFCheck = false;
                    //婴儿与编码中的婴儿个数是否不一致提示
                    string INFCountCheckMsg = "<b class=\"red\">编码中婴儿个数与预订婴儿个数不一致，请手动补全编码中婴儿！</b>";
                    //更新特价缓存
                    string SpCache = "<b class=\"red\">特价缓存数据已更新</b>";
                    bool IsUpdateSP = false;
                    //Bill返回有无SQL
                    bool IsBillOK = false;
                    //比较航空公司支付金额对比 true 政策变动 false可以生成订单
                    bool CmpPayMoney = false;

                    //修改实体相关的值后更新即可
                    for (int i = 0; i < InputParam.OrderParamModel.Count; i++)
                    {
                        OrderMustParamModel item = InputParam.OrderParamModel[i];
                        //承运人二字码
                        CarrayCode = item.Order.CarryCode.Split('/')[0].ToUpper().Trim();

                        if (orderIDs.Contains(item.Order.OrderId))
                            continue;
                        else
                            orderIDs += item.Order.OrderId + "|";

                        #region 设置需要更改数据的字段名集合
                        item.UpdateOrderFileds = UpdateOrderFileds;
                        item.UpdateSkyWayFileds = UpdateSkyWayFileds;
                        item.UpdatePassengerFileds = UpdatePasFileds;
                        #endregion

                        #region 实体处理
                        //订单中的总价
                        decimal TotalPMPrice = 0m, TotalABFare = 0, TotalRQFare = 0m;
                        //预订备注信息
                        item.Order.YDRemark = txtRemak.Text.Trim().Replace("'", "");

                        //订单处理 成人订单政策
                        if (!item.Order.IsChdFlag && AdultPolicy != null)
                        {
                            #region 成人或者婴儿实体价格赋值
                            //检测白屏预订婴儿个数与编码中的婴儿个数
                            if (item.Order.OrderSourceType == 1)
                            {
                                IsINFCheck = yudingINFCheck(InputParam.PnrInfo, item.PasList);
                            }

                            //婴儿价格
                            decimal INFPMFee = 0m, INFABFare = 0m, INFRQFare = 0m;
                            if (INFPrice != null)
                            {
                                decimal.TryParse(INFPrice.Fare, out INFPMFee);
                                decimal.TryParse(INFPrice.TAX, out INFABFare);
                                decimal.TryParse(INFPrice.RQFare, out INFRQFare);
                            }
                            //成人价格
                            decimal PMFee = AdultPolicy.SeatPrice, ABFare = AdultPolicy.ABFare, RQFare = AdultPolicy.RQFare;

                            #region 特价缓存处理
                            //特价时特价缓存处理  为特价且PAT内容不为空
                            if (IsUseSpCache && AdultPolicy.PolicyKind == 2 && item.SkyList[0].Pat.Trim() != "")
                            {
                                //白屏和PNR导入
                                if (item.Order.OrderSourceType == 1 || item.Order.OrderSourceType == 2 || item.Order.OrderSourceType == 6 || item.Order.OrderSourceType == 10)
                                {
                                    //特价缓存
                                    SpecialCabinPriceInfoBLL SpBll = new SpecialCabinPriceInfoBLL();
                                    string errMsg = "";
                                    PnrAnalysis.PatModel Pat = pnrformat.GetPATInfo(item.SkyList[0].Pat.Trim(), out errMsg);
                                    if (Pat.UninuePatList.Count > 0)
                                    {
                                        decimal m_Fare = 0m;
                                        decimal m_TAX = 0m;
                                        decimal m_RQFare = 0m;
                                        decimal.TryParse(Pat.UninuePatList[0].Fare, out m_Fare);
                                        decimal.TryParse(Pat.UninuePatList[0].TAX, out m_TAX);
                                        decimal.TryParse(Pat.UninuePatList[0].RQFare, out m_RQFare);
                                        //价格不相等
                                        if (m_Fare != PMFee)
                                        {
                                            //存入缓存
                                            IsUpdateSP = SpBll.SaveSpPrice(item.SkyList[0].CarryCode.ToUpper(), item.SkyList[0].FlightCode.ToUpper(), item.SkyList[0].FromDate, item.SkyList[0].FromCityCode, item.SkyList[0].ToCityCode, item.SkyList[0].Space, m_Fare, m_TAX, m_RQFare);
                                        }
                                    }
                                }
                            }
                            #endregion


                            //乘机人实体处理
                            for (int j = 0; j < item.PasList.Count; j++)
                            {
                                if (item.PasList[j].PassengerType == 1)
                                {
                                    //成人
                                    item.PasList[j].PMFee = PMFee;
                                    item.PasList[j].ABFee = ABFare;
                                    item.PasList[j].FuelFee = RQFare;
                                }
                                else
                                {
                                    //婴儿
                                    if (item.PasList[j].PassengerType == 3 && INFPrice != null)
                                    {
                                        item.PasList[j].PMFee = INFPMFee;
                                        item.PasList[j].ABFee = INFABFare;
                                        item.PasList[j].FuelFee = INFRQFare;
                                    }
                                }
                                if (item.PasList[j].PassengerType == 1 || item.PasList[j].PassengerType == 3)
                                {
                                    //订单价格
                                    TotalPMPrice += item.PasList[j].PMFee;
                                    TotalABFare += item.PasList[j].ABFee;
                                    TotalRQFare += item.PasList[j].FuelFee;
                                }
                            }
                            //航段实体处理
                            //string Discount = "0";
                            for (int k = 0; k < item.SkyList.Count; k++)
                            {
                                item.SkyList[k].ABFee = ABFare;
                                item.SkyList[k].FuelFee = RQFare;
                                //只是单程才重新赋值
                                if (item.SkyList.Count == 1)
                                {
                                    item.SkyList[k].SpacePrice = PMFee;
                                }
                                //item.SkyList[k].Discount = AdultPolicy.DiscountRate.ToString();
                                //if (Discount.Length > 10)
                                //{
                                //    Discount = Discount.Substring(0, 10);
                                //}
                                //item.SkyList[k].Discount = Discount;
                            }
                            //成人订单(含有婴儿) 赋值
                            item.Order.PMFee = TotalPMPrice;
                            item.Order.ABFee = TotalABFare;
                            item.Order.FuelFee = TotalRQFare;
                            if (INFPrice != null)
                            {
                                //婴儿票面价
                                item.Order.BabyFee = INFPMFee;
                            }
                            item.Order.PolicyId = AdultPolicy.PolicyId;
                            item.Order.PolicyPoint = AdultPolicy.PolicyPoint;
                            item.Order.ReturnMoney = AdultPolicy.PolicyReturnMoney;
                            item.Order.AirPoint = AdultPolicy.AirPoint;
                            item.Order.ReturnPoint = AdultPolicy.ReturnPoint;
                            item.Order.LaterPoint = AdultPolicy.LaterPoint;
                            item.Order.PolicyMoney = AdultPolicy.PolicyYongJin;
                            item.Order.DiscountDetail = AdultPolicy.DiscountDetail;
                            item.Order.PolicyType = int.Parse(AdultPolicy.PolicyType);
                            item.Order.PolicySource = int.Parse(AdultPolicy.PolicySource);
                            item.Order.AutoPrintFlag = int.Parse(AdultPolicy.AutoPrintFlag);
                            item.Order.PolicyCancelTime = AdultPolicy.FPGQTime;
                            item.Order.PolicyReturnTime = AdultPolicy.PolicyReturnTime;
                            item.Order.JinriGYCode = AdultPolicy.JinriGYCode;
                            //出票公司编号
                            string CPCpyNo = string.IsNullOrEmpty(AdultPolicy.CPCpyNo) ? mSupCompany.UninCode : AdultPolicy.CPCpyNo;
                            item.Order.CPCpyNo = CPCpyNo.Length > 12 ? CPCpyNo.Substring(0, 12) : CPCpyNo;

                            item.Order.PolicyRemark = AdultPolicy.PolicyRemark;//政策备注
                            //原始政策返点
                            item.Order.OldPolicyPoint = AdultPolicy.OldPolicyPoint;
                            //原始政策现返
                            item.Order.OldReturnMoney = AdultPolicy.OldPolicyReturnMoney;

                            item.Order.A1 = 1;//已确认
                            item.Order.A2 = AdultPolicy.PolicyKind;//政策种类
                            item.Order.A7 = AdultPolicy.AirPoint; //航空公司返点
                            item.Order.A11 = AdultPolicy.PatchPonit;//补点

                            //计算订单金额  
                            // item.Order.PayMoney = d.CreateOrderPayMoney(item.Order, item.PasList);
                            //出票方收款金额
                            //item.Order.OrderMoney = d.CreateOrderOrderMoney(item.Order, item.PasList);

                            //出票Office
                            if (AdultPolicy.PolicyOffice != "")
                            {
                                item.Order.PrintOffice = AdultPolicy.PolicyOffice;
                            }
                            if ((item.Order.OrderSourceType == 1 || item.Order.OrderSourceType == 2))
                            {
                                //自动授权Office
                                if (AdultPolicy.PolicyOffice.Trim().Length == 6 && !GYOffice.Contains(AdultPolicy.PolicyOffice.Trim().ToUpper()))
                                {
                                    SendIns.AuthToOffice(item.Order.PNR, AdultPolicy.PolicyOffice, item.Order.Office, out ErrMsg);
                                }
                                //备注HU的A舱要添加一个指令才能入库，OSI HU CKIN SSAC/S1
                                if (AdultPolicy.PolicySource == "1" && AdultPolicy.PolicyType == "1" && AdultPolicy.AutoPrintFlag == "2" && item.Order.PNR.Trim().Length == 6 && item.Order.CarryCode.ToUpper().Trim() == "HU" && item.Order.Space.ToUpper().Trim() == "A")
                                {
                                    string Office = item.Order.Office, Cmd = string.Format("RT{0}|OSI HU CKIN SSAC/S1^\\", item.Order.PNR.Trim());
                                    SendIns.Send(Cmd, ref Office, 10);
                                }
                            }
                            if (item.Order.PolicySource <= 2)
                            {
                                //本地政策提示
                                sbTip.Append("</br><ul><li>1.请于一小时内支付此订单,未支付将自动取消</li><li>2.编码内容中必须存在证件内容一项</li><li>3.PNR需要包含证件号</li><li>" + (IsINFCheck ? "4." + INFCountCheckMsg : "") + "</li></ul>");
                            }
                            else
                            {
                                //接口和共享政策提示
                                if (AdultPolicy.PolicyOffice.Trim().Length == 6)
                                {
                                    sbTip.Append("</br><ul><li>1.编码内容中必须存在证件内容一项</li><li>2.PNR需要包含证件号</li><li>3.请授权,授权指令：RMK TJ AUTH " + AdultPolicy.PolicyOffice + "</li>" + (IsINFCheck ? "4." + INFCountCheckMsg : "") + "</ul>");
                                }
                                else
                                {
                                    sbTip.Append("<ul ><li>1.编码内容中必须存在证件内容一项!</li><li>2.PNR需要包含证件号!</li>" + (IsINFCheck ? "3." + INFCountCheckMsg : "") + "</ul>");
                                }
                            }
                            //缓存数据已更新
                            if (IsUpdateSP)
                            {
                                sbTip.Append("<p>" + SpCache + "</p>");
                            }
                            #endregion
                        }
                        else
                        {
                            if (ChildPolicy != null)
                            {
                                #region 儿童实体赋值
                                //儿童订单政策

                                //儿童价格
                                decimal PMFee = ChildPolicy.SeatPrice, ABFare = ChildPolicy.ABFare, RQFare = ChildPolicy.RQFare;
                                //乘机人实体处理
                                for (int j = 0; j < item.PasList.Count; j++)
                                {
                                    if (item.PasList[j].PassengerType == 2)
                                    {
                                        //儿童
                                        item.PasList[j].PMFee = PMFee;
                                        item.PasList[j].ABFee = ABFare;
                                        item.PasList[j].FuelFee = RQFare;
                                        //订单价格
                                        TotalPMPrice += item.PasList[j].PMFee;
                                        TotalABFare += item.PasList[j].ABFee;
                                        TotalRQFare += item.PasList[j].FuelFee;
                                    }
                                }
                                //航段实体处理
                                //string Discount = "0";
                                List<string> OrderSpaceList = new List<string>();
                                //编码中的航段信息
                                List<LegInfo> pnrLeg = InputParam.PnrInfo.PnrList[1] != null ? InputParam.PnrInfo.PnrList[1]._LegList : null;
                                for (int k = 0; k < item.SkyList.Count; k++)
                                {
                                    item.SkyList[k].ABFee = ABFare;
                                    item.SkyList[k].FuelFee = RQFare;
                                    //只是单程才重新赋值
                                    if (item.SkyList.Count == 1)
                                    {
                                        item.SkyList[k].SpacePrice = PMFee;
                                    }
                                    //非儿童出成人票
                                    decimal dis = 0;
                                    if (item.Order.IsCHDETAdultTK != 1)
                                    {
                                        //2013-7-9修改
                                        decimal.TryParse(item.SkyList[k].Discount, out dis);
                                        if (!("FCY".Contains(item.SkyList[k].Space.Substring(0, 1)) || dis > 100))
                                        {
                                            item.SkyList[k].Space = "Y";
                                        }
                                        //比较编码中的舱位
                                        if (pnrLeg != null)
                                        {
                                            LegInfo li = pnrLeg.Find(delegate(LegInfo leg)
                                            {
                                                return leg.AirCode == CarrayCode && leg.FromCode == item.SkyList[k].FromCityCode && leg.ToCode == item.SkyList[k].ToCityCode;
                                            });
                                            if (li != null && li.Seat != item.SkyList[k].Space)
                                            {
                                                item.SkyList[k].Space = li.Seat.Trim();
                                            }
                                        }
                                        item.SkyList[k].Discount = "50";
                                    }
                                    OrderSpaceList.Add(item.SkyList[k].Space);
                                }
                                //儿童订单赋值                              
                                item.Order.PMFee = TotalPMPrice;
                                item.Order.ABFee = TotalABFare;
                                item.Order.FuelFee = TotalRQFare;
                                item.Order.Space = string.Join("/", OrderSpaceList.ToArray());
                                //出票公司编号
                                string CPCpyNo = string.IsNullOrEmpty(ChildPolicy.CPCpyNo) ? mSupCompany.UninCode : ChildPolicy.CPCpyNo;
                                item.Order.CPCpyNo = CPCpyNo.Length > 12 ? CPCpyNo.Substring(0, 12) : CPCpyNo;
                                item.Order.PolicyId = ChildPolicy.PolicyId;
                                item.Order.AirPoint = ChildPolicy.AirPoint;
                                item.Order.PolicyPoint = ChildPolicy.PolicyPoint;
                                item.Order.ReturnPoint = ChildPolicy.ReturnPoint;
                                item.Order.LaterPoint = ChildPolicy.LaterPoint;
                                item.Order.ReturnMoney = ChildPolicy.PolicyReturnMoney;
                                item.Order.PolicyMoney = ChildPolicy.PolicyYongJin;
                                item.Order.DiscountDetail = ChildPolicy.DiscountDetail;
                                item.Order.PolicyType = int.Parse(ChildPolicy.PolicyType);
                                item.Order.PolicySource = int.Parse(ChildPolicy.PolicySource);
                                item.Order.AutoPrintFlag = int.Parse(ChildPolicy.AutoPrintFlag);
                                item.Order.PolicyCancelTime = ChildPolicy.FPGQTime;
                                item.Order.PolicyReturnTime = ChildPolicy.PolicyReturnTime;
                                item.Order.PolicyRemark = ChildPolicy.PolicyRemark;//政策备注
                                //原始政策返点
                                item.Order.OldPolicyPoint = ChildPolicy.OldPolicyPoint;
                                //原始政策现返
                                item.Order.OldReturnMoney = ChildPolicy.OldPolicyReturnMoney;

                                item.Order.A1 = 1;//已确认
                                item.Order.A7 = ChildPolicy.AirPoint; //航空公司返点
                                //政策种类
                                item.Order.A2 = ChildPolicy.PolicyKind;

                                //计算订单金额;
                                //item.Order.PayMoney = d.CreateOrderPayMoney(item.Order, item.PasList);
                                //出票方收款金额
                                // item.Order.OrderMoney = d.CreateOrderOrderMoney(item.Order, item.PasList);

                                //出票Office
                                if (ChildPolicy.PolicyOffice != "")
                                {
                                    item.Order.PrintOffice = ChildPolicy.PolicyOffice;
                                }
                                //---------------------------------------

                                #endregion
                            }
                        }

                        //代付返点，金额
                        if (item.Order.PolicySource > 2)
                        {
                            // 接口 取原始政策
                            item.Order.A7 = item.Order.OldPolicyPoint;
                            item.Order.OutOrderPayMoney = d.CreateOrderIntfacePrice(item.Order, item.PasList);
                        }
                        else
                        {
                            //本地 取航空公司政策
                            decimal tempOldPolicyPoint = item.Order.OldPolicyPoint;

                            item.Order.OldPolicyPoint = item.Order.A7;
                            item.Order.OutOrderPayMoney = d.CreateOrderIntfacePrice(item.Order, item.PasList);
                            item.Order.OldPolicyPoint = tempOldPolicyPoint;
                        }

                        item.Order.A13 = d.CreateOrderIntfacePrice(item.Order, item.PasList);// 后返金额

                        #endregion

                        #region 添加订单账单明细sql
                        //List<string> sqlList = bill.CreateOrderAndTicketPayDetail(item.Order, item.PasList);
                        List<string> sqlList = bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);
                        if (sqlList != null && sqlList.Count > 0)
                        {
                            IsBillOK = true;
                            InputParam.ExecSQLList.AddRange(sqlList.ToArray());
                        }
                        #endregion

                        //航空公司B2B政策支付金额与本地订单支付金额对比
                        if (AdultPolicy != null && AdultPolicy.PolicyId.Contains("b2bpolicy") && AdultPolicy.AirPayMoney < item.Order.PayMoney)
                        {
                            CmpPayMoney = true;
                            break;
                        }
                    }//For结束
                    //订单金额是否有误
                    bool IsOrderPayZero = false;
                    foreach (OrderMustParamModel item in InputParam.OrderParamModel)
                    {
                        //判断金额是否正确
                        if (item.Order.PayMoney <= 0 || ((item.Order.PayMoney + item.Order.PayMoney * 0.003M) < item.Order.OrderMoney))
                        {
                            IsOrderPayZero = true;
                            PbProject.WebCommon.Log.Log.RecordLog("Confirmation.aspx", "PayMoneyError|" + ErrMsg + "订单：PayMoney=" + item.Order.PayMoney + " OrderMoney=" + item.Order.OrderMoney + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                            break;
                        }
                    }
                    #region 验证和修改订单
                    if (!IsBillOK)
                    {
                        ErrMsg = "订单生成失败！！";
                        PbProject.WebCommon.Log.Log.RecordLog("Confirmation.aspx", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                    }
                    else
                    {
                        if (!IsOrderPayZero)
                        {
                            if (!CmpPayMoney)
                            {
                                //修改订单有关实体信息
                                IsSuc = OrderBLL.UpdateOrder(ref InputParam, out ErrMsg);
                                if (IsSuc)
                                {
                                    ErrMsg = "订单生成成功！" + sbTip.ToString();
                                }
                                else
                                {
                                    PbProject.WebCommon.Log.Log.RecordLog("Confirmation.aspx", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                                    ErrMsg = "订单生成失败！";
                                }
                            }
                            else
                            {
                                PbProject.WebCommon.Log.Log.RecordLog("Confirmation.aspx", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 此政策已变动，请重新选择！\t" + ErrMsg + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                                ErrMsg = "此政策已变动，请重新选择！";
                            }
                        }
                        else
                        {
                            PbProject.WebCommon.Log.Log.RecordLog("Confirmation.aspx", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                            ErrMsg = "订单金额错误,生成订单失败！";
                        }
                    }
                    #endregion
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showMsgDg('" + ErrMsg + "');", true);
                }
            }
            else
            {
                ErrMsg = "未获取到政策数据，生成订单失败！";
            }
        }
        catch (Exception ex)
        {
            PbProject.WebCommon.Log.Log.RecordLog("Confirmation.aspx", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg, true, HttpContext.Current.Request);
            ErrMsg = "订单生成异常," + ex.Message;
            DataBase.LogCommon.Log.Error("Confirmation.aspx?currentuserid=" + this.mUser.id.ToString(), ex);
        }
        finally
        {
            if (IsSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdg('" + ErrMsg + "',{type:1,currentuserid:'" + this.currentuserid.Value.ToString() + "',result:'" + Result + "'});", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showMsgDg('" + ErrMsg + "');", true);
            }
        }
        #endregion
    }
}
