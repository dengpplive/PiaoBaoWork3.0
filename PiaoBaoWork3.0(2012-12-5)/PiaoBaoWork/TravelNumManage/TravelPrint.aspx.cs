using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Text;
using PbProject.Logic;
using PbProject.Logic.Order;
using PbProject.Logic.ControlBase;
using System.Drawing.Text;
using System.Drawing;

public partial class TravelNumManage_TravelPrint : BasePage
{
    /// <summary>
    /// 订单管理操作类
    /// </summary>
    Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();
    PnrAnalysis.Model.TripPrintData pageobj = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (Request["PrintTime"] == null && Request["optype"] == null)
                {
                    if (Context.Handler is IObject)
                    {
                        pageobj = (PnrAnalysis.Model.TripPrintData)(Context.Handler as IObject).PageObj;
                    }
                }
            }
            //保存位置
            SavePoint();
            //清空数据
            ClearPageData();
            int DataSource = 0;
            if (pageobj == null)
            {
                DataSource = 0;
                //重新设置显示数据
                pageobj = GetPrintData();
            }
            else
            {
                DataSource = 1;
            }
            //显示数据
            ShowData(pageobj, DataSource);
            //修改打印时间
            UpdatePrintTime();
            //获取字体
            GetFont();
        }
        catch (Exception)
        {
        }
    }

    public void GetFont()
    {
        InstalledFontCollection MyFont = new InstalledFontCollection();
        FontFamily[] MyFontFamilies = MyFont.Families;
        foreach (FontFamily item in MyFontFamilies)
        {
            selFont0.Items.Add(new ListItem(item.Name, item.Name));
        }
    }

    //测试数据
    public void TestData()
    {
        lblTravelNumber.InnerHtml = "4515548754";  // 行程单号
        lblPassengerName.InnerHtml = "张静云";  // 乘机人姓名
        lblPersonCardID.InnerHtml = "420821198710072018"; //  乘机人证件号
        qianzhu.InnerHtml = "不得签转变更退票"; // 签注
        lblPnr.InnerHtml = "NMHGFD"; // Pnr编码

        //第一程
        lblTo1.InnerHtml = "T2武汉WUH"; // 出发城市名称及三字码
        lblCarrier.InnerHtml = "国航"; //  承运人二字码
        lblFlight.InnerHtml = "CA1425"; //   航班号
        lblClass.InnerHtml = "Y";  //    仓位 座位等级
        lblDate.InnerHtml = "2012-12-12"; //     日期
        lblTime.InnerHtml = "08:00"; //     时间
        lblFareBasis.InnerHtml = "Y100"; // 客票级别/客票类型
        lblValidDate.InnerHtml = "2013-01-24"; // 客票生效日期
        lblInvalidDate.InnerHtml = "2013-03-25"; // 客票失效日期
        lblAllow.InnerHtml = "20K"; //      免费行李

        //第二程
        lblFrom.InnerHtml = "T3成都CTU";   // 出发城市名称及三字码
        lblCarrier1.InnerHtml = "国航"; // 承运人二字码
        lblFlight1.InnerHtml = "CA1425"; //  航班号
        lblClass1.InnerHtml = "H";  //   仓位 座位等级
        lblDate1.InnerHtml = "2013-01-26"; //    日期
        lblTime1.InnerHtml = "09:21"; //    时间
        lblFareBasis1.InnerHtml = "K"; //  客票级别/客票类型
        lblValidDate1.InnerHtml = "2013-05-14"; //  客票生效日期
        lblInvalidDate1.InnerHtml = "2013-05-19"; // 客票失效日期
        lblAllow1.InnerHtml = "30K";   //lblAllow1       免费行李

        //第三程
        lblTo2.InnerHtml = "T2广州CAN";  // 出发城市名称及三字码
        lblCarrier2.InnerHtml = "国航";  // 承运人二字码
        lblFlight2.InnerHtml = "CA3695"; // 航班号
        lblClass2.InnerHtml = "P"; // 仓位 座位等级
        lblDate2.InnerHtml = "2013-04-14"; //  日期
        lblTime2.InnerHtml = "09:24"; //  时间
        lblFareBasis2.InnerHtml = "Y"; // 客票级别/客票类型
        lblValidDate2.InnerHtml = "2013-05-12"; // 客票生效日期
        lblInvalidDate2.InnerHtml = "2013-05-19"; //  客票失效日期
        lblAllow2.InnerHtml = "40K";  //     免费行李

        //第四程
        lblTo3.InnerHtml = "T2西安XIY"; // 出发城市名称及三字码
        lblCarrier3.InnerHtml = "国航"; // 承运人二字码
        lblFlight3.InnerHtml = "CA2412";  //  航班号
        lblClass3.InnerHtml = "L";  // 仓位 座位等级
        lblDate3.InnerHtml = "2013-05-13"; //  日期
        lblTime3.InnerHtml = "18:45"; //  时间
        lblFareBasis3.InnerHtml = "Q";  //  客票级别/客票类型
        lblValidDate3.InnerHtml = "2013-05-12"; // 客票生效日期
        lblInvalidDate3.InnerHtml = "2013-05-16"; //  客票失效日期
        lblAllow3.InnerHtml = "40K"; //  免费行李

        //第五程
        lblTo4.InnerHtml = "VOID"; //lblTo4 VOID

        lblFare.InnerHtml = "1450.00"; //lblFare   票价
        lblTax.InnerHtml = "100.00"; //    基建
        lblYQ.InnerHtml = "80.00"; //     燃油
        lblOtherTaxes.InnerHtml = "0.00";  // 其他税
        lblTotal.InnerHtml = "3000.00"; //   合计
        lblTicNo.InnerHtml = "0182312064381"; //   票号
        lblCheckNum.InnerHtml = "1458"; //  验证码
        msgInfo.InnerHtml = "提示信息";  //msgInfo  提示信息
        lblInsurance.InnerHtml = "XXX"; //lblInsurance 保险费
        lblAgentCode.InnerHtml = "CTU324"; //lblAgentCode  Office
        Agent_Code.InnerHtml = "08311284"; //Agent_Code   销售单位代号
        lblConjunctionTKT.InnerHtml = "成都可可可可可可可可可可可"; //lblConjunctionTKT  填开单位
        lblIssuedBy.InnerHtml = "tiankaidanwei"; //lblIssuedBy  填开单位
        lblIssuedDate.InnerHtml = System.DateTime.Now.ToString("yyyy-MM-dd");  //lblIssuedDate 打印日期 即使用日期

    }

    public void SavePoint()
    {
        if (Request["optype"] != null && Request["optype"].ToString() != "")
        {
            string strOpType = Request["optype"].ToString();
            //标识 操作类型 数据
            string result = "0$##$" + strOpType + "$##$";
            try
            {
                Tb_SendInsData m_Tb_SendInsData = null;
                string sqlWhere = string.Format(" CpyNo='{0}' and UserAccount='{1}' and SendIns='SaveTripPoint' ", mUser.CpyNo, mUser.LoginName);
                if (strOpType == "save")
                {
                    string strPoint = HttpUtility.UrlDecode(Request["Point"].ToString(), Encoding.Default);
                    List<Tb_SendInsData> Params = this.baseDataManage.CallMethod("Tb_SendInsData", "GetList", null, new object[] { sqlWhere }) as List<Tb_SendInsData>;
                    //是否添加一条数据
                    bool IsAdd = false;
                    //存在更新 不存在添加
                    if (Params != null && Params.Count > 0)
                    {
                        m_Tb_SendInsData = Params[0];
                        m_Tb_SendInsData.RecvData = strPoint;
                        IsAdd = false;
                    }
                    else
                    {
                        m_Tb_SendInsData = new Tb_SendInsData();
                        m_Tb_SendInsData.SendTime = DateTime.Now;
                        m_Tb_SendInsData.RecvTime = System.DateTime.Now.AddYears(10);
                        m_Tb_SendInsData.SendIns = "SaveTripPoint";
                        m_Tb_SendInsData.UserAccount = mUser.LoginName;
                        m_Tb_SendInsData.RecvData = strPoint;
                        m_Tb_SendInsData.CpyNo = mUser.CpyNo;
                        IsAdd = true;
                    }
                    bool IsSuc = false;
                    if (IsAdd)
                    {
                        List<string> sqlList = new List<string>();
                        List<string> Removelist = new List<string>();
                        Removelist.Add("id");
                        sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(m_Tb_SendInsData, Removelist));
                        if (sqlList.Count > 0)
                        {
                            string errMsg = "";
                            IsSuc = this.baseDataManage.ExecuteSqlTran(sqlList, out errMsg);
                        }
                    }
                    else
                    {
                        IsSuc = (bool)this.baseDataManage.CallMethod("Tb_SendInsData", "Update", null, new object[] { m_Tb_SendInsData });
                    }
                    if (IsSuc)
                    {
                        result = "1$##$" + strOpType + "$##$保存成功";
                    }
                    else
                    {
                        result = "0$##$" + strOpType + "$##$保存失败";
                    }
                }
                else if (strOpType == "load")
                {
                    List<Tb_SendInsData> Params = this.baseDataManage.CallMethod("Tb_SendInsData", "GetList", null, new object[] { sqlWhere }) as List<Tb_SendInsData>;
                    if (Params != null && Params.Count > 0)
                    {
                        Tb_SendInsData tb_sendinsdata = Params[0];
                        result = "1$##$" + strOpType + "$##$" + tb_sendinsdata.RecvData;
                    }
                    else
                    {
                        result = "0$##$" + strOpType + "$##$加载失败";
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                OutPut(escape(result));
            }
        }
    }

    /// <summary>
    /// 清空页面上的数据
    /// </summary>
    public void ClearPageData()
    {
        lblTravelNumber.InnerHtml = "";  // 行程单号
        lblPassengerName.InnerHtml = "";  // 乘机人姓名
        lblPersonCardID.InnerHtml = ""; //  乘机人证件号
        qianzhu.InnerHtml = ""; // 签注
        lblPnr.InnerHtml = ""; // Pnr编码

        //第一程
        lblTo1.InnerHtml = ""; // 出发城市名称及三字码
        lblCarrier.InnerHtml = ""; //  承运人二字码
        lblFlight.InnerHtml = ""; //   航班号
        lblClass.InnerHtml = "";  //    仓位 座位等级
        lblDate.InnerHtml = ""; //     日期
        lblTime.InnerHtml = ""; //     时间
        lblFareBasis.InnerHtml = ""; // 客票级别/客票类型
        lblValidDate.InnerHtml = ""; // 客票生效日期
        lblInvalidDate.InnerHtml = ""; // 客票失效日期
        lblAllow.InnerHtml = ""; //      免费行李

        //第二程
        lblFrom.InnerHtml = "";   // 出发城市名称及三字码
        lblCarrier1.InnerHtml = ""; // 承运人二字码
        lblFlight1.InnerHtml = ""; //  航班号
        lblClass1.InnerHtml = "";  //   仓位 座位等级
        lblDate1.InnerHtml = ""; //    日期
        lblTime1.InnerHtml = ""; //    时间
        lblFareBasis1.InnerHtml = ""; //  客票级别/客票类型
        lblValidDate1.InnerHtml = ""; //  客票生效日期
        lblInvalidDate1.InnerHtml = ""; // 客票失效日期
        lblAllow1.InnerHtml = "";   //lblAllow1       免费行李

        //第三程
        lblTo2.InnerHtml = "";  // 出发城市名称及三字码
        lblCarrier2.InnerHtml = "";  // 承运人二字码
        lblFlight2.InnerHtml = ""; // 航班号
        lblClass2.InnerHtml = ""; // 仓位 座位等级
        lblDate2.InnerHtml = ""; //  日期
        lblTime2.InnerHtml = ""; //  时间
        lblFareBasis2.InnerHtml = ""; // 客票级别/客票类型
        lblValidDate2.InnerHtml = ""; // 客票生效日期
        lblInvalidDate2.InnerHtml = ""; //  客票失效日期
        lblAllow2.InnerHtml = "";  //     免费行李

        //第四程
        lblTo3.InnerHtml = ""; // 出发城市名称及三字码
        lblCarrier3.InnerHtml = ""; // 承运人二字码
        lblFlight3.InnerHtml = "";  //  航班号
        lblClass3.InnerHtml = "";  // 仓位 座位等级
        lblDate3.InnerHtml = ""; //  日期
        lblTime3.InnerHtml = ""; //  时间
        lblFareBasis3.InnerHtml = "";  //  客票级别/客票类型
        lblValidDate3.InnerHtml = ""; // 客票生效日期
        lblInvalidDate3.InnerHtml = ""; //  客票失效日期
        lblAllow3.InnerHtml = ""; //  免费行李

        //第五程
        lblTo4.InnerHtml = ""; //lblTo4 VOID

        lblFare.InnerHtml = ""; //lblFare   票价
        lblTax.InnerHtml = ""; //    基建
        lblYQ.InnerHtml = ""; //     燃油
        lblOtherTaxes.InnerHtml = "";  // 其他税
        lblTotal.InnerHtml = ""; //   合计
        lblTicNo.InnerHtml = ""; //   票号
        //lblCheckNum.Value = ""; //  验证码
        msgInfo.InnerHtml = "";  //msgInfo  提示信息
        lblInsurance.InnerHtml = ""; //lblInsurance 保险费
        lblAgentCode.InnerHtml = ""; //lblAgentCode  Office
        Agent_Code.InnerHtml = ""; //Agent_Code   销售单位代号
        lblConjunctionTKT.InnerHtml = ""; //lblConjunctionTKT  填开单位
        lblIssuedBy.InnerHtml = ""; //lblIssuedBy  填开单位
        lblIssuedDate.InnerHtml = "";  //lblIssuedDate 打印日期 即使用日期
    }


    /// <summary>
    /// 获取打印数据
    /// </summary>
    /// <returns></returns>
    public PnrAnalysis.Model.TripPrintData GetPrintData()
    {
        PnrAnalysis.Model.TripPrintData model = null;
        if (Request["OrderId"] != null && Request["OrderId"].ToString() != "" && Request["PasId"] != null && Request["PasId"].ToString() != "")
        {
            model = new PnrAnalysis.Model.TripPrintData();
            model.m_strOrderId = Request["OrderId"].ToString();
            model.m_strPassengerId = Request["PasId"].ToString();

            //乘机人id
            Hid_Pid.Value = model.m_strPassengerId;

            OrderInputParam InputParam = new OrderInputParam();
            string ErrMsg = "";
            //成人订单数据显示            
            InputParam = OrderBLL.GetOrder(model.m_strOrderId, InputParam, out ErrMsg);
            if (InputParam != null && InputParam.OrderParamModel.Count > 0)
            {
                OrderMustParamModel OMP = InputParam.OrderParamModel[0];
                Tb_Ticket_Passenger Passenger = OMP.PasList.Find(delegate(Tb_Ticket_Passenger _TP)
                {
                    return _TP.id.ToString().ToUpper() == model.m_strPassengerId.ToUpper();
                });
                Tb_TripDistribution tb_tripdistribution = null;
                Tb_Ticket_Order Order = OMP.Order;
                List<Tb_Ticket_SkyWay> skyList = OMP.SkyList;
                PbProject.Model.ConfigParam config = this.configparam;
                if (Passenger != null && Order != null)
                {
                    if (Passenger.TravelNumber.Trim() != "")
                    {
                        string sqlWhere = string.Format(" TripNum='{0}' and UseCpyNo='{1}' ", Passenger.TravelNumber.Trim(), Order.OwnerCpyNo);
                        List<Tb_TripDistribution> TTPList = this.baseDataManage.CallMethod("Tb_TripDistribution", "GetList", null, new object[] { sqlWhere }) as List<Tb_TripDistribution>;
                        if (TTPList != null && TTPList.Count > 0)
                        {
                            tb_tripdistribution = TTPList[0];
                        }
                    }
                    //管理员
                    if (mCompany.RoleType == 1)
                    {
                        string GYCpyNo = Order.OwnerCpyNo;
                        if (Order.OwnerCpyNo.Length >= 12)
                        {
                            GYCpyNo = GYCpyNo.Substring(0, 12);
                        }
                        List<Bd_Base_Parameters> baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + GYCpyNo + "'" }) as List<Bd_Base_Parameters>;
                        config = Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
                    }

                    #region 获取 配置号
                    if (tb_tripdistribution != null)
                    {
                        //航协号
                        model.m_strIataCode = tb_tripdistribution.IataCode;
                        //中文填开单位
                        model.m_strCNTKTConjunction = tb_tripdistribution.OwnerCpyName;
                        //office
                        model.m_strAgentOffice = tb_tripdistribution.CreateOffice;
                        //行程单号
                        model.m_strTravelNumber = tb_tripdistribution.TripNum;
                        //行程单id
                        TripId.Value = tb_tripdistribution.id.ToString();
                    }
                    else
                    {
                        //行程单号为空时
                        if (config != null)
                        {
                            string PrintOffice = GetPrintOffice(Order.OwnerCpyNo, Order.CarryCode.Split('/')[0]);
                            string[] Arroffice = config.Office.Split('^');//office
                            string[] ArrIataCode = config.IataCode.Split('^');//航协号
                            string[] ArrTicketCompany = config.TicketCompany.Split('^');//公司名称
                            for (int i = 0; i < Arroffice.Length; i++)
                            {
                                if (Arroffice.Length == ArrIataCode.Length && Arroffice.Length == ArrTicketCompany.Length)
                                {
                                    if (PrintOffice == "")
                                    {
                                        //Office
                                        model.m_strAgentOffice = Arroffice[i];
                                        //中文填开单位
                                        model.m_strCNTKTConjunction = ArrTicketCompany[i];
                                        //航协号
                                        model.m_strIataCode = ArrIataCode[i];//航协号
                                        break;
                                    }
                                    else
                                    {
                                        if (PrintOffice.ToUpper() == Arroffice[i].ToUpper())
                                        {
                                            //Office
                                            model.m_strAgentOffice = Arroffice[i];//Office
                                            //中文填开单位
                                            model.m_strCNTKTConjunction = ArrTicketCompany[i];//填开单位
                                            //航协号
                                            model.m_strIataCode = ArrIataCode[i];//航协号
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    //乘客姓名
                    model.m_strPassengerName = Passenger.PassengerName;
                    //乘客证件号
                    model.m_strPassengerCardId = Passenger.Cid;
                    //签注
                    model.m_strEndorsements = "不得签转";
                    //小编码
                    model.m_strPnrB = Order.PNR.ToUpper();
                    //行程单号
                    model.m_strTravelNumber = Passenger.TravelNumber.Trim();
                    //舱位价
                    model.m_strSpaceFare = Passenger.PMFee.ToString("F2");
                    //基建费
                    model.m_strABFare = Passenger.ABFee.ToString("F2");
                    //燃油费
                    model.m_strFuelFare = Passenger.FuelFee.ToString("F2");
                    //总价
                    model.m_strTotalFare = (Passenger.PMFee + Passenger.ABFee + Passenger.FuelFee).ToString("F2");
                    //保险费
                    model.m_strInsuranceFare = "XXX";
                    //其他费用
                    model.m_strOtherFare = "0.00";
                    //票号
                    model.m_strTicketNumber = Passenger.TicketNumber;
                    //验证码
                    model.m_strCheckNum = Passenger.TravelNumber.Length > 4 ? (Passenger.TravelNumber.Substring(Passenger.TravelNumber.Length - 4, 4)) : lblCheckNum.InnerText;
                    //填开日期
                    model.m_strIssuedDate = System.DateTime.Now.ToString("yyyy-MM-dd");
                    //航段
                    if (skyList != null && skyList.Count > 0)
                    {
                        //第一段
                        model.m_strFCityName1 = skyList[0].FromCityName;//出发城市
                        model.m_strTCityName1 = skyList[0].ToCityName;//到达城市
                        model.m_strAirName1 = skyList[0].CarryName;//航空公司名称
                        model.m_strAirCode1 = skyList[0].CarryCode;//航空公司二字码
                        model.m_strFlightNum1 = skyList[0].FlightCode;//航班号
                        model.m_strSpace1 = skyList[0].Space.ToUpper();//舱位
                        model.m_strTicketBasis1 = skyList[0].Space.ToUpper();//舱位等级
                        model.m_strFlyDate1 = skyList[0].FromDate.ToString("yyyy-MM-dd");
                        model.m_strFlyStartTime1 = skyList[0].FromDate.ToString("HH:mm");
                        model.m_strFlyEndTime1 = skyList[0].ToDate.ToString("HH:mm");//到达时间
                        model.m_strTerminal1 = skyList[0].Terminal;//航站楼

                        //行李                   
                        model.m_strAllowPacket1 = "20K";
                        if (model.m_strSpace1 == "C")
                        {
                            model.m_strAllowPacket1 = "30K";
                        }
                        else if (model.m_strSpace1 == "F")
                        {
                            model.m_strAllowPacket1 = "40K";
                        }
                        //-------------------------------------------
                        //第二段
                        if (skyList.Count > 1)
                        {
                            model.m_strFCityName2 = skyList[1].FromCityName;//出发城市
                            model.m_strTCityName2 = skyList[1].ToCityName;//到达城市
                            model.m_strAirName2 = skyList[1].CarryName;//航空公司名称
                            model.m_strAirCode2 = skyList[1].CarryCode;//航空公司二字码
                            model.m_strFlightNum2 = skyList[1].FlightCode;//航班号
                            model.m_strSpace2 = skyList[1].Space.ToUpper();//舱位
                            model.m_strTicketBasis2 = skyList[1].Space.ToUpper();//舱位等级
                            model.m_strFlyDate2 = skyList[1].FromDate.ToString("yyyy-MM-dd");
                            model.m_strFlyStartTime2 = skyList[1].FromDate.ToString("HH:mm");
                            model.m_strFlyEndTime2 = skyList[1].ToDate.ToString("HH:mm");//到达时间
                            model.m_strTerminal2 = skyList[1].Terminal;//航站楼
                            //行李                   
                            model.m_strAllowPacket2 = "20K";
                            if (model.m_strSpace2 == "C")
                            {
                                model.m_strAllowPacket2 = "30K";
                            }
                            else if (model.m_strSpace2 == "F")
                            {
                                model.m_strAllowPacket2 = "40K";
                            }
                            if (model.m_strFCityName2 == model.m_strTCityName1)
                            {
                                if (model.m_strFCityName1 != model.m_strTCityName2)
                                {
                                    model.m_strTravelType = "3";//联程
                                }
                                else
                                {
                                    model.m_strTravelType = "2";//往返
                                }
                            }
                            else
                            {
                                if (skyList.Count == 2)
                                {
                                    model.m_strTravelType = "4";//缺口程
                                }
                                else
                                {
                                    model.m_strTravelType = "5";//多程
                                }
                            }
                        }
                        else
                        { 
                        
                        }
                    }
                }
            }
        }
        return model;
    }

    /// <summary>
    /// 数据显示
    /// </summary>
    /// <param name="TPD"></param>
    public void ShowData(PnrAnalysis.Model.TripPrintData TPD, int DataSource)
    {
        try
        {
            if (TPD != null)
            {
                //行程单号
                inputCheckNum.Value = TPD.m_strTravelNumber;
                //乘客姓名
                lblPassengerName.InnerText = TPD.m_strPassengerName;
                //乘机人ID
                Hid_Pid.Value = TPD.m_strPassengerId;
                //乘客证件号
                lblPersonCardID.InnerHtml = TPD.m_strPassengerCardId;
                //签注
                qianzhu.InnerHtml = TPD.m_strEndorsements;
                //编码
                lblPnr.InnerHtml = TPD.m_strPnrB;
                //票号  
                lblTicNo.InnerText = TPD.m_strTicketNumber;
                //验证码
                lblCheckNum.InnerHtml = TPD.m_strCheckNum;
                //航协号
                Agent_Code.InnerHtml = TPD.m_strIataCode;
                //填开单位
                lblConjunctionTKT.InnerText = TPD.m_strCNTKTConjunction;
                //Office
                lblAgentCode.InnerText = TPD.m_strAgentOffice;
                //舱位价
                lblFare.InnerText = "CNY   " + TPD.m_strSpaceFare;
                //基建
                lblTax.InnerText = "CN" + TPD.m_strABFare;
                //燃油
                lblYQ.InnerText = "YQ" + TPD.m_strFuelFare;
                //合计  
                lblTotal.InnerText = "CNY   " + TPD.m_strTotalFare;
                //保险费
                lblInsurance.InnerText = TPD.m_strInsuranceFare;
                //填开日期
                lblIssuedDate.InnerText = TPD.m_strIssuedDate;
                //
                lblTo4.InnerText = "VOID";
                //-----航段-----------------------------------

                //出发城市
                lblTo1.InnerText = TPD.m_strFCityName1;
                //承运人
                lblCarrier.InnerText = TPD.m_strAirName1;
                //行李                   
                lblAllow.InnerText = TPD.m_strAllowPacket1;
                //舱位
                lblClass.InnerText = TPD.m_strSpace1;
                //仓位等级
                lblFareBasis.InnerText = TPD.m_strTicketBasis1;
                //航班号
                lblFlight.InnerText = DataSource == 0 ? (TPD.m_strAirCode1 + TPD.m_strFlightNum1) : TPD.m_strFlightNum1;
                //日期
                lblDate.InnerText = TPD.m_strFlyDate1;
                //时间
                lblTime.InnerText = TPD.m_strFlyStartTime1;
                //提示信息
                msgInfo.InnerText = TPD.m_strPromptMsg;

                if (TPD.m_strTravelType == "1")//单程
                {
                    //到达城市
                    lblFrom.InnerText = TPD.m_strTCityName1;
                    lblTime1.InnerText = TPD.m_strFlyEndTime1;
                    lblTo4.InnerText = "VOID";
                    lblCarrier1.InnerText = "VOID";
                }
                else if (TPD.m_strTravelType == "2")//往返
                {
                    lblFrom.InnerText = TPD.m_strFCityName2;
                    lblCarrier1.InnerText = TPD.m_strAirName2;
                    lblFlight1.InnerText = DataSource == 0 ? (TPD.m_strAirCode2 + TPD.m_strFlightNum2) : TPD.m_strFlightNum2;
                    lblClass1.InnerText = TPD.m_strSpace2;
                    lblDate1.InnerText = TPD.m_strFlyDate2;
                    lblTime1.InnerText = TPD.m_strFlyStartTime2;
                    lblFareBasis1.InnerText = TPD.m_strTicketBasis2;
                    lblValidDate1.InnerText = TPD.m_strTicketValidBefore2;
                    lblInvalidDate1.InnerText = TPD.m_strTicketValidAfter2;
                    lblAllow1.InnerText = TPD.m_strAllowPacket2;
                    lblTo2.InnerText = TPD.m_strTCityName2;
                    lblCarrier2.InnerText = "VOID";
                }
                else if (TPD.m_strTravelType == "3")//联程
                {
                    lblFrom.InnerText = TPD.m_strFCityName2;
                    lblCarrier1.InnerText = TPD.m_strAirName2;
                    lblFlight1.InnerText = DataSource == 0 ? (TPD.m_strAirCode2 + TPD.m_strFlightNum2) : TPD.m_strFlightNum2;
                    lblClass1.InnerText = TPD.m_strSpace2;
                    lblDate1.InnerText = TPD.m_strFlyDate2;
                    lblTime1.InnerText = TPD.m_strFlyStartTime2;
                    lblFareBasis1.InnerText = TPD.m_strTicketBasis2;
                    lblValidDate1.InnerText = TPD.m_strTicketValidBefore2;
                    lblInvalidDate1.InnerText = TPD.m_strTicketValidAfter2;
                    lblAllow1.InnerText = TPD.m_strAllowPacket2;
                    lblTo2.InnerText = TPD.m_strTCityName2;
                    lblCarrier2.InnerText = "VOID";

                }
                else if (TPD.m_strTravelType == "4")//缺口程
                {

                }
                else if (TPD.m_strTravelType == "5")//多程
                {

                }
            }
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// 获取航空公司出票Office号 
    /// </summary>
    /// <param name="CarryCode"></param>
    /// <param name="defaultOffice"></param>
    /// <returns></returns>
    public string GetPrintOffice(string CpyNo, string CarryCode)
    {
        string PrintOffice = "";
        string sqlWhere = string.Format(" CpyNo='{0}' and AirCode='{1}' ", CpyNo, CarryCode);
        List<Tb_Ticket_PrintOffice> list = this.baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_PrintOffice>;
        if (list != null && list.Count > 0)
        {
            if (!string.IsNullOrEmpty(list[0].OfficeCode))
            {
                PrintOffice = list[0].OfficeCode;
            }
        }
        return PrintOffice;
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
        }
        return AppPath;
    }

    public void UpdatePrintTime()
    {
        if (Request["PrintTime"] != null && Request["PrintTime"].ToString() != "" && Request["tid"] != null && Request["tid"].ToString() != "")
        {
            Response.Clear();
            int suc = 0;
            string Tid = Request["tid"].ToString();
            string pid = Request["pid"].ToString();
            string printSQL = "";
            List<string> listsql = new List<string>();
            if (!string.IsNullOrEmpty(Tid))
            {
                printSQL = string.Format(" update dbo.Tb_TripDistribution set PrintTime=getdate() where id='{0}'", Tid.Replace("\'", ""));
                listsql.Add(printSQL);
            }
            //if (!string.IsNullOrEmpty(pid))
            //{
            //    printSQL = string.Format(" update dbo.Tb_Ticket_Passenger set A5=getdate() where id='{0}'", pid.Replace("\'", ""));
            //    listsql.Add(printSQL);
            //}
            string errMsg = "";
            if (listsql.Count > 0 && this.baseDataManage.ExecuteSqlTran(listsql, out errMsg))
            {
                suc = 1;
            }
            else
            {
                suc = 0;
            }
            Response.Write(suc.ToString());
            Response.Flush();
            Response.End();
        }
    }

    /// <summary>
    ///  方法使用
    /// </summary>
    /// <returns></returns>
    public string getUrl()
    {
        string AppPath = "";
        HttpContext HttpCurrent = HttpContext.Current;
        HttpRequest Req;
        if (HttpCurrent != null)
        {
            Req = HttpCurrent.Request;
            if (Req.Url != null)
            {
                AppPath = Req.Url.Authority.ToString();
            }
        }
        return AppPath;
    }

    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        try
        {
            Response.ContentType = "text/plain";
            Response.Clear();
            Response.Write(result);
            Response.Flush();
            Response.End();
        }
        catch (Exception)
        {
        }
    }
}