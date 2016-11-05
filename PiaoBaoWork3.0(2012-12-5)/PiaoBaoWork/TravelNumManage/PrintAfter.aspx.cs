using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.Order;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using PbProject.WebCommon.Utility.Encoding;
public enum emType
{
    实体数据到界面 = 0,
    界面数据到实体 = 1
}
public partial class TravelNumManage_PrintAfter : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.VData = GetPrintData();
            SetPrintData(this.VData, emType.实体数据到界面);
            //权限
            IsEnabledControl(this.GongYingKongZhiFenXiao != null && this.GongYingKongZhiFenXiao.Contains("|45|"));
        }
    }

    public void IsEnabledControl(bool IsEnabled)
    {
        ControlCollection Ctrls = this.Controls;
        foreach (Control item in Ctrls)
        {
            System.Web.UI.HtmlControls.HtmlInputText text = item as System.Web.UI.HtmlControls.HtmlInputText;
            if (text != null)
            {
                text.Attributes.Add("readonly", IsEnabled ? "true" : "false");
            }
        }
    }

    public PnrAnalysis.Model.TripPrintData VData
    {
        get
        {
            return ViewState["print"] as PnrAnalysis.Model.TripPrintData;
        }
        set
        {
            ViewState["print"] = value;
        }
    }

    public string ReturnQueryDateCode(DateTime date)
    {
        string month = "";
        int day = date.Day;
        string[] sEnMonsthsInfo = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        month = sEnMonsthsInfo[date.Month].ToLower().Substring(0, 3);
        return (day.ToString() + month).ToUpper();
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
    /// 订单管理操作类
    /// </summary>
    Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();
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
                    model.m_strEndorsements = ddlEndorsements.SelectedValue;
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
                    model.m_strCheckNum = Passenger.TravelNumber.Length > 4 ? (Passenger.TravelNumber.Substring(Passenger.TravelNumber.Length - 4, 4)) : txtCheckNum.Value;
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
                    }
                }
            }
        }
        return model;
    }


    /// <summary>
    /// 设置实体数据
    /// </summary>
    /// <param name="printData"></param>
    /// <returns></returns>
    public PnrAnalysis.Model.TripPrintData SetPrintData(PnrAnalysis.Model.TripPrintData printData, emType emtype)
    {
        if (emtype == emType.界面数据到实体)
        {
            if (printData == null)
            {
                printData = new PnrAnalysis.Model.TripPrintData();
            }
            printData.m_strTravelNumber = txtTravelNumber.Value.Trim();
            printData.m_strPassengerName = txtPassengerName.Value.Trim();
            printData.m_strPassengerCardId = txtPassengerCardId.Value.Trim();
            printData.m_strEndorsements = ddlEndorsements.SelectedValue;
            printData.m_strPnrB = txtPnrB.Value.Trim();
            printData.m_strSpaceFare = txtSpaceFare.Value.Trim();
            printData.m_strABFare = txtABFare.Value.Trim();
            printData.m_strFuelFare = txtFuelFare.Value.Trim();
            printData.m_strOtherFare = txtOtherFare.Value.Trim();
            printData.m_strTotalFare = txtTotalFare.Value.Trim();
            printData.m_strTicketNumber = txtTicketNumber.Value.Trim();
            printData.m_strCheckNum = txtCheckNum.Value.Trim();
            printData.m_strPromptMsg = txtPromptMsg.Value.Trim();
            printData.m_strInsuranceFare = txtInsuranceFare.Value.Trim();
            printData.m_strAgentOffice = txtAgentOffice.Value.Trim();
            printData.m_strIataCode = txtIataCode.Value.Trim();
            printData.m_strCNTKTConjunction = txtCNTKTConjunction.Value.Trim();
            printData.m_strIssuedDate = txtIssuedDate.Value.Trim();

            //航段一
            printData.m_strFCityName1 = txtFCityName1.Value.Trim();
            //printData.m_strTCityName1 = txtTCityName1.Value.Trim();
            printData.m_strAirName1 = txtAirName1.Value.Trim();
            printData.m_strFlightNum1 = txtFlightNum1.Value.Trim();
            printData.m_strSpace1 = txtSpace1.Value.Trim();
            printData.m_strFlyDate1 = txtFlyDate1.Value.Trim();
            printData.m_strFlyStartTime1 = txtFlyStartTime1.Value.Trim();
            printData.m_strTicketBasis1 = txtTicketBasis1.Value.Trim();
            printData.m_strTicketValidBefore1 = txtTicketValidBefore1.Value.Trim();
            printData.m_strTicketValidAfter1 = txtTicketValidAfter1.Value.Trim();
            printData.m_strAllowPacket1 = txtAllowPacket1.Value.Trim();

            //航段二
            printData.m_strFCityName2 = txtFCityName2.Value.Trim();
            //printData.m_strTCityName2 = txtTCityName2.Value.Trim();
            printData.m_strAirName2 = txtAirName2.Value.Trim();
            printData.m_strFlightNum2 = txtFlightNum2.Value.Trim();
            printData.m_strSpace2 = txtSpace2.Value.Trim();
            printData.m_strFlyDate2 = txtFlyDate2.Value.Trim();
            printData.m_strFlyStartTime2 = txtFlyStartTime2.Value.Trim();
            printData.m_strTicketBasis2 = txtTicketBasis2.Value.Trim();
            printData.m_strTicketValidBefore2 = txtTicketValidBefore2.Value.Trim();
            printData.m_strTicketValidAfter2 = txtTicketValidAfter2.Value.Trim();
            printData.m_strAllowPacket2 = txtAllowPacket2.Value.Trim();


            //航段三
            printData.m_strFCityName3 = txtFCityName3.Value.Trim();
            //printData.m_strTCityName3 = txtTCityName3.Value.Trim();
            printData.m_strAirName3 = txtAirName3.Value.Trim();
            printData.m_strFlightNum3 = txtFlightNum3.Value.Trim();
            printData.m_strSpace3 = txtSpace3.Value.Trim();
            printData.m_strFlyDate3 = txtFlyDate3.Value.Trim();
            printData.m_strFlyStartTime3 = txtFlyStartTime3.Value.Trim();
            printData.m_strTicketBasis3 = txtTicketBasis3.Value.Trim();
            printData.m_strTicketValidBefore3 = txtTicketValidBefore3.Value.Trim();
            printData.m_strTicketValidAfter3 = txtTicketValidAfter3.Value.Trim();
            printData.m_strAllowPacket3 = txtAllowPacket3.Value.Trim();

            //航段四
            printData.m_strFCityName4 = txtFCityName4.Value.Trim();
            //printData.m_strTCityName4 = txtTCityName4.Value.Trim();
            printData.m_strAirName4 = txtAirName4.Value.Trim();
            printData.m_strFlightNum4 = txtFlightNum4.Value.Trim();
            printData.m_strSpace4 = txtSpace4.Value.Trim();
            printData.m_strFlyDate4 = txtFlyDate4.Value.Trim();
            printData.m_strFlyStartTime4 = txtFlyStartTime4.Value.Trim();
            printData.m_strTicketBasis4 = txtTicketBasis4.Value.Trim();
            printData.m_strTicketValidBefore4 = txtTicketValidBefore4.Value.Trim();
            printData.m_strTicketValidAfter4 = txtTicketValidAfter4.Value.Trim();
            printData.m_strAllowPacket4 = txtAllowPacket4.Value.Trim();

            //航段五
            printData.m_strFCityName5 = txtFCityName5.Value.Trim();
        }
        if (printData != null && emtype == emType.实体数据到界面)
        {
            txtTravelNumber.Value = printData.m_strTravelNumber;
            txtPassengerName.Value = printData.m_strPassengerName;
            txtPassengerCardId.Value = printData.m_strPassengerCardId;
            ddlEndorsements.SelectedValue = printData.m_strEndorsements;
            txtPnrB.Value = printData.m_strPnrB;
            txtSpaceFare.Value = printData.m_strSpaceFare;
            txtABFare.Value = printData.m_strABFare;
            txtFuelFare.Value = printData.m_strFuelFare;
            txtOtherFare.Value = printData.m_strOtherFare;
            txtTotalFare.Value = printData.m_strTotalFare;
            txtTicketNumber.Value = printData.m_strTicketNumber;
            txtCheckNum.Value = printData.m_strCheckNum;
            txtPromptMsg.Value = printData.m_strPromptMsg;
            txtInsuranceFare.Value = printData.m_strInsuranceFare;
            txtAgentOffice.Value = printData.m_strAgentOffice;
            txtIataCode.Value = printData.m_strIataCode;
            txtCNTKTConjunction.Value = printData.m_strCNTKTConjunction;
            txtIssuedDate.Value = printData.m_strIssuedDate;



            //航段一
            txtFCityName1.Value = printData.m_strFCityName1;
            //txtTCityName1.Value = printData.m_strTCityName1;
            txtAirName1.Value = printData.m_strAirName1;
            txtFlightNum1.Value = printData.m_strAirCode1 + printData.m_strFlightNum1;
            txtSpace1.Value = printData.m_strSpace1;
            txtFlyDate1.Value = printData.m_strFlyDate1;
            txtFlyStartTime1.Value = printData.m_strFlyStartTime1;
            txtTicketBasis1.Value = printData.m_strTicketBasis1;
            txtTicketValidBefore1.Value = printData.m_strTicketValidBefore1;
            txtTicketValidAfter1.Value = printData.m_strTicketValidAfter1;
            txtAllowPacket1.Value = printData.m_strAllowPacket1;
            if (printData.m_strTravelType == "1")//单程
            {
                txtFCityName2.Value = printData.m_strTCityName1;
            }
            else if (printData.m_strTravelType == "2")//往返
            {
                txtFCityName2.Value = printData.m_strTCityName1;
                txtFCityName3.Value = printData.m_strTCityName2;
                txtAirName2.Value = printData.m_strAirName2;
                txtFlightNum2.Value = printData.m_strAirCode2 + printData.m_strFlightNum2;
                txtSpace2.Value = printData.m_strSpace2;
                txtFlyDate2.Value = printData.m_strFlyDate2;
                txtFlyStartTime2.Value = printData.m_strFlyStartTime2;
                txtTicketBasis2.Value = printData.m_strTicketBasis2;
                txtTicketValidBefore2.Value = printData.m_strTicketValidBefore2;
                txtTicketValidAfter2.Value = printData.m_strTicketValidAfter2;
                txtAllowPacket2.Value = printData.m_strAllowPacket2;
                txtAirName3.Value = "VOID";
            }
            else if (printData.m_strTravelType == "3")
            {
                txtFCityName2.Value = printData.m_strTCityName1;
                txtFCityName3.Value = printData.m_strTCityName2;
                txtAirName2.Value = printData.m_strAirName2;
                txtFlightNum2.Value = printData.m_strAirCode2 + printData.m_strFlightNum2;
                txtSpace2.Value = printData.m_strSpace2;
                txtFlyDate2.Value = printData.m_strFlyDate2;
                txtFlyStartTime2.Value = printData.m_strFlyStartTime2;
                txtTicketBasis2.Value = printData.m_strTicketBasis2;
                txtTicketValidBefore2.Value = printData.m_strTicketValidBefore2;
                txtTicketValidAfter2.Value = printData.m_strTicketValidAfter2;
                txtAllowPacket2.Value = printData.m_strAllowPacket2;
                txtAirName3.Value = "VOID";
            }
            else if (printData.m_strTravelType == "4")
            {

            }
            else if (printData.m_strTravelType == "5")
            {

            }
            txtFCityName5.Value = "VOID";
            /*
            //航段二
            txtFCityName2.Value = printData.m_strFCityName2;
            txtTCityName2.Value = printData.m_strTCityName2;
            txtAirName2.Value = printData.m_strAirName2;
            txtFlightNum1.Value = printData.m_strFlightNum2;
            txtSpace2.Value = printData.m_strSpace2;
            txtFlyDate2.Value = printData.m_strFlyDate2;
            txtFlyStartTime2.Value = printData.m_strFlyStartTime2;
            txtTicketBasis2.Value = printData.m_strTicketBasis2;
            txtTicketValidBefore2.Value = printData.m_strTicketValidBefore2;
            txtTicketValidAfter2.Value = printData.m_strTicketValidAfter2;
            txtAllowPacket2.Value = printData.m_strAllowPacket2;               

            //航段三
            txtFCityName3.Value = printData.m_strFCityName3;
            txtTCityName3.Value = printData.m_strTCityName3;
            txtAirName3.Value = printData.m_strAirName3;
            txtFlightNum3.Value = printData.m_strFlightNum3;
            txtSpace3.Value = printData.m_strSpace3;
            txtFlyDate3.Value = printData.m_strFlyDate3;
            txtFlyStartTime3.Value = printData.m_strFlyStartTime3;
            txtTicketBasis3.Value = printData.m_strTicketBasis3;
            txtTicketValidBefore3.Value = printData.m_strTicketValidBefore3;
            txtTicketValidAfter3.Value = printData.m_strTicketValidAfter3;
            txtAllowPacket3.Value = printData.m_strAllowPacket3;

            //航段四
            txtFCityName4.Value = printData.m_strFCityName4;
            txtTCityName4.Value = printData.m_strTCityName4;
            txtAirName4.Value = printData.m_strAirName4;
            txtFlightNum4.Value = printData.m_strFlightNum4;
            txtSpace4.Value = printData.m_strSpace4;
            txtFlyDate4.Value = printData.m_strFlyDate4;
            txtFlyStartTime4.Value = printData.m_strFlyStartTime4;
            txtTicketBasis4.Value = printData.m_strTicketBasis4;
            txtTicketValidBefore4.Value = printData.m_strTicketValidBefore4;
            txtTicketValidAfter4.Value = printData.m_strTicketValidAfter4;
            txtAllowPacket4.Value = printData.m_strAllowPacket4;

            //航段五                
            txtFCityName5.Value = printData.m_strFCityName5;
            */
        }
        return printData;
    }
    protected void btnGo_Click(object sender, EventArgs e)
    {
        try
        {
            this.PageObj = SetPrintData(this.VData, emType.界面数据到实体);
            System.IO.StringWriter sw = new System.IO.StringWriter();
            Server.Execute("TravelPrint.aspx?currentuserid=" + mUser.id.ToString(), sw);
            Response.Clear();
            Response.Write(sw.ToString());
            Response.End();
        }
        catch (Exception)
        {
        }
    }
}