<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintAfter.aspx.cs" Inherits="TravelNumManage_PrintAfter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>行程单打印数据修改</title>
    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
            font-size: 13px;
            color: #333;
        }
        .table
        {
            width: 800px;
            margin: 1px auto;
            border: 1px solid #999;
            border-collapse: 0;
            border-spacing: 0;
            background: #eee;
        }
        .table td
        {
            height: 18px;
        }
        .table2
        {
            width: 100%;
            border: 1px dotted #999;
        }
        .input_75
        {
            width: 60px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_40
        {
            width: 40px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_60
        {
            width: 50px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_25
        {
            width: 25px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_105
        {
            width: 105px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_175
        {
            width: 175px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_185
        {
            width: 185px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_380
        {
            width: 350px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_125
        {
            width: 125px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .td
        {
            width: 5px;
        }
        .width_40
        {
            width: 40px;
            text-align: center;
        }
        .width_50
        {
            width: 50px;
            text-align: center;
        }
        .width_60
        {
            width: 60px;
            text-align: center;
        }
        .input_90
        {
            width: 90px;
            text-align: center;
        }
        .width_105
        {
            width: 40px;
            text-align: center;
        }
        .pd
        {
            margin-top: 0;
        }
        .ff
        {
            margin-top: 0;
        }
        .height
        {
            height: 5px;
            overflow: hidden;
            font-size: 0;
        }
        .user
        {
            width: 109px;
        }
        .Hide
        {
            display: none;
        }
        .style1
        {
            width: 53px;
        }
    </style>
    <style media="print" type="text/css">
        body
        {
            font-family: TEC;
        }
        .table
        {
            width: 700px;
            margin: 1px auto;
            border: 1px solid #999;
            border-collapse: 0;
            border-spacing: 0;
        }
        .table td
        {
            height: 18px;
        }
        .table2
        {
            width: 100%;
            border: 0;
        }
        .input_75
        {
            width: 60px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_80
        {
            width: 65px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_85
        {
            width: 70px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_35
        {
            width: 35px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_40
        {
            width: 40px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_60
        {
            width: 50px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_25
        {
            width: 25px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_90
        {
            width: 90px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_105
        {
            width: 105px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_175
        {
            width: 175px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_185
        {
            width: 185px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_205
        {
            width: 205px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_380
        {
            width: 350px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_125
        {
            width: 125px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .input_195
        {
            width: 195px;
            height: 18px;
            line-height: 18px;
            border: 1px solid #999;
        }
        .ff
        {
            margin-top: -17px;
        }
        .pd
        {
            margin-top: 15px;
        }
        .pd_3
        {
            margin-top: 3px;
        }
        .sx
        {
            width: 80px;
        }
        .sx_txt
        {
            width: 118px;
        }
        .tkrq
        {
            width: 108px;
        }
        .lx_txt
        {
            width: 240px;
        }
        .Noprint
        {
            display: none;
        }
        .td
        {
            width: 5px;
        }
        .height
        {
            height: 5px;
            overflow: hidden;
            font-size: 0;
        }
        .pd_center
        {
            text-align: center;
        }
        .mf
        {
            margin-left: -10px;
        }
    </style>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script> 
</head>
<body>
    <form id="form1" runat="server"  >
    <p align="center" class="Noprint">
        <b>打印数据修改</b></p>
    <table class="table">
        <tr>
            <td colspan="10" style="text-align: right;">
                印刷序列号:
            </td>
            <td style="width: 175px;">
                <input type="text" id="txtTravelNumber" runat="server" class="input_175" readonly="readonly" />
            </td>
        </tr>
    </table>
    <table class="table">
        <tr>
            <td>
                旅客姓名
            </td>
            <td class="user">
                <input type="text" id="txtPassengerName" runat="server" class="input_125" />
            </td>
            <td>
                有效身份证件号码:
            </td>
            <td>
                <input type="text" id="txtPassengerCardId" runat="server" class="input_175" />
            </td>
            <td>
                签注:
            </td>
            <td>
                <asp:DropDownList ID="ddlEndorsements" runat="server" class="input_175">
                    <asp:ListItem Value="不得签转" Selected="True">不得签转</asp:ListItem>
                    <asp:ListItem Value="不得签转更改">不得签转更改</asp:ListItem>
                    <asp:ListItem Value="不得签转，改退收费">不得签转，改退收费</asp:ListItem>
                    <asp:ListItem Value="不得签转变更退票">不得签转变更退票</asp:ListItem>
                    <asp:ListItem Value="不得签转不得改期不得退票">不得签转不得改期不得退票</asp:ListItem>
                    <asp:ListItem Value="不得签转，仅限原出票处退票">不得签转，仅限原出票处退票</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <table class="table">
        <tr>
            <td>
                订座记录
            </td>
            <td colspan="2">
                <input type="text" id="txtPnrB" runat="server" class="input_90" />
            </td>
            <td>
                承运人
            </td>
            <td class="width_60">
                航班号
            </td>
            <td>
                舱位等级
            </td>
            <td class="width_60">
                日期
            </td>
            <td class="width_60">
                时间
            </td>
            <td>
                票价级别/客票类型
            </td>
            <td>
                客票生效日期
            </td>
            <td>
                有效截止日期
            </td>
            <td>
                免费行李
            </td>
        </tr>
    </table>
    <table class="table pd">
        <tr>
            <td>
                自FROM
            </td>
            <td>
                <input type="text" id="txtFCityName1" runat="server" class="input_40" />
            </td>
            <%-- <td>
               <input type="text" id="txtTCityName1" runat="server" class="input_40 Hide" />
            </td>--%>
            <td>
                <input type="text" id="txtAirName1" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlightNum1" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtSpace1" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlyDate1" runat="server" class="input_60" />
            </td>
            <td class="mf">
                <input type="text" id="txtFlyStartTime1" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtTicketBasis1" runat="server" class="input_105" />
            </td>
            <td>
                <input type="text" id="txtTicketValidBefore1" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtTicketValidAfter1" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtAllowPacket1" runat="server" class="input_40" text="20K" />
            </td>
        </tr>
        <tr>
            <td>
                至 TO
            </td>
            <td>
                <input type="text" id="txtFCityName2" runat="server" class="input_40" />
            </td>
            <%--  <td>
                <input type="text" id="txtTCityName2" runat="server" class="input_40 Hide" />
            </td>--%>
            <td>
                <input type="text" id="txtAirName2" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlightNum2" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtSpace2" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlyDate2" runat="server" class="input_60" />
            </td>
            <td class="mf">
                <input type="text" id="txtFlyStartTime2" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtTicketBasis2" runat="server" class="input_105" />
            </td>
            <td>
                <input type="text" id="txtTicketValidBefore2" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtTicketValidAfter2" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtAllowPacket2" runat="server" class="input_40" />
            </td>
        </tr>
    </table>
    <table class="table pd">
        <tr>
            <td>
                至 TO&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td>
                <input type="text" id="txtFCityName3" runat="server" class="input_40" />
            </td>
            <%-- <td>
               <input type="text" id="txtTCityName1" runat="server" class="input_40 Hide" />
            </td>--%>
            <td>
                <input type="text" id="txtAirName3" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlightNum3" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtSpace3" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlyDate3" runat="server" class="input_60" />
            </td>
            <td class="mf">
                <input type="text" id="txtFlyStartTime3" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtTicketBasis3" runat="server" class="input_105" />
            </td>
            <td>
                <input type="text" id="txtTicketValidBefore3" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtTicketValidAfter3" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtAllowPacket3" runat="server" class="input_40" />
            </td>
        </tr>
        <tr>
            <td>
                至 TO
            </td>
            <td>
                <input type="text" id="txtFCityName4" runat="server" class="input_40" />
            </td>
            <%--  <td>
                <input type="text" id="txtTCityName2" runat="server" class="input_40 Hide" />
            </td>--%>
            <td>
                <input type="text" id="txtAirName4" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlightNum4" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtSpace4" runat="server" class="input_40" />
            </td>
            <td>
                <input type="text" id="txtFlyDate4" runat="server" class="input_60" />
            </td>
            <td class="mf">
                <input type="text" id="txtFlyStartTime4" runat="server" class="input_60" />
            </td>
            <td>
                <input type="text" id="txtTicketBasis4" runat="server" class="input_105" />
            </td>
            <td>
                <input type="text" id="txtTicketValidBefore4" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtTicketValidAfter4" runat="server" class="input_75" />
            </td>
            <td>
                <input type="text" id="txtAllowPacket4" runat="server" class="input_40" />
            </td>
        </tr>
    </table>
    <table class="table">
        <tr>
            <td>
                至 TO
            </td>
            <td>
                <input type="text" id="txtFCityName5" runat="server" class="input_60" />
                <%-- &nbsp;<input type="text" id="t3CityTxt" runat="server" class="input_40 Hide" />--%>
            </td>
            <td colspan="10">
                <table class="table2">
                    <tr>
                        <td>
                            票价
                        </td>
                        <td style="margin-left: -15px;">
                            <input type="text" id="txtSpaceFare" runat="server" class="input_105" />
                        </td>
                        <td>
                            机建费
                        </td>
                        <td>
                            <input type="text" id="txtABFare" runat="server" class="input_75" />
                        </td>
                        <td>
                            燃油费
                        </td>
                        <td>
                            <input type="text" id="txtFuelFare" runat="server" class="input_75" />
                        </td>
                        <td>
                            其他
                        </td>
                        <td>
                            <input type="text" id="txtOtherFare" runat="server" class="input_40" />
                        </td>
                        <td>
                            合计
                        </td>
                        <td>
                            <input type="text" id="txtTotalFare" runat="server" class="input_125" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table class="table">
        <tr>
            <td class="sx">
                电子客票号码
            </td>
            <td class="sx_txt" style="padding-left: 15px;">
                <input type="text" id="txtTicketNumber" runat="server" class="input_205" />
            </td>
            <td>
                验证码
            </td>
            <td>
                <input type="text" id="txtCheckNum" runat="server" class="input_75" />
            </td>
            <td>
                提示信息
            </td>
            <td class="lx_txt">
                <input type="text" id="txtPromptMsg" runat="server" class="input_195" />
            </td>
            <td>
                保险费
            </td>
            <td style="text-align: center;">
                <input type="text" id="txtInsuranceFare" runat="server" text="XXX" class="input_75" />
            </td>
        </tr>
    </table>
    <table class="table">
        <tr>
            <td style="width: 90px;">
                销售单位代号
            </td>
            <td class="style2" style="padding-left: 15px;">
                <input type="text" id="txtAgentOffice" runat="server" class="input_125" />
                <span style="margin-left: 35px;">填开单位</span>
            </td>
            <td>
                填开日期
            </td>
        </tr>
    </table>
    <table class="table ff">
        <tr>
            <td class="sx">
                AGENT&nbsp; CODE
            </td>
            <td style="padding-left: 15px;">
                <input type="text" id="txtIataCode" runat="server" class="input_125" />
                &nbsp;&nbsp;&nbsp;&nbsp;<span style="margin-left: 30px;"><input type="text" id="txtCNTKTConjunction"
                    runat="server" class="input_380" />
                </span>
            </td>
            <td style="padding-left: 10px;">
                <input type="text" id="txtIssuedDate" readonly="readonly" runat="server" class="input_125"
                    onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd',minDate:'%y-%M-%d'})" />
            </td>
        </tr>
    </table>
    <table id="btnTable" style="border: 0px; text-align: center; margin: 1 auto;" align="center">
        <tr class="Noprint">
            <td colspan="3" align="right">
                <asp:Button ID="btnGo" runat="server" Text="进入打印页面" onclick="btnGo_Click" />
            </td>
        </tr>
    </table>  
    </form>
</body>
</html>
