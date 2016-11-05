<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderDetail.aspx.cs" Inherits="Order_OrderDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>订单详情</title>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../css/detail.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/json2.js"></script>
    <script type="text/javascript" src="../js/js_OrderDetail.js"></script>
    <script type="text/javascript" src="../js/js_Copy.js"></script>
    <style type="text/css">
        #overlay
        {
            background-color: #333333;
            left: 0;
            filter: alpha(opacity=50); /* IE */
            -moz-opacity: 0.5; /* 老版Mozilla */
            -khtml-opacity: 0.5; /* 老版Safari */
            opacity: 0.5;
            position: fixed;
            top: 0;
            width: 100%;
            z-index: 999;
            height: 100%;
        }
        #loading
        {
            margin-top: 10px;
            width: 420px;
            border: 0 none;
            text-align: center;
            padding: 40px 30px 40px 30px;
            color: white;
            font-size: 18px;
            line-height: 180%;
            position: fixed;
            left: 50%;
            top: 30%;
            z-index: 1000;
            background: url(../images/mainbg.gif);
            margin-left: -210px;
        }
        .table_info th, .table_info td
        {
            border-color: #CCCCCC;
            border-style: solid;
            border-width: 1px 1px 1px 1px;
            color: #606060;
        }
        table
        {
            border-collapse: collapse;
        }
        .tdNew
        {
            font-weight: bold;
            background: url("../img/title.png") repeat-x scroll 0 0 transparent;
            text-align: right;
            width: 10%;
        }
        .td1
        {
            color: Red;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .red
        {
            color: Red;
        }
        .green
        {
            color: Green;
        }
        .else-table tbody .tbl_Point td
        {
            border: none;
        }
    </style>
    <script type="text/javascript">
        function OpenWinSendSms(phone) {
            var OrderId=document.getElementById("Hid_OrderId").value;
            window.open("../SMS/SmsSend.aspx?OrderId="+OrderId+"&phone="+phone+"&currentuserid=<%=this.mUser.id.ToString() %>","发送短信","height=500, width=800, top=150,left=300, toolbar=no, menubar=no, scrollbars=yes, resizable=no, location=no, status=no")
        }
    </script>
</head>
<body>
    <div id="show2">
    </div>
    <div id="showOne">
    </div>
    <div id="html">
    </div>
    <div id="overlay" style="display: none;">
    </div>
    <div id="loading" style="display: none;">
        请稍候<br />
        正在加载中......<br />
        <img src="../img/loading.gif" alt="正在加载中......" />
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <div id="tabs-1" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
            <table width="100%" align="center" border="0" cellpadding="0" cellspacing="0" style="border: 1px solid #E6E6E6;">
                <tr>
                    <td class="mainl">
                    </td>
                    <td>
                        <table width="100%" align="center" class="detail" border="0" cellpadding="5" cellspacing="0"
                            style="padding: 5px;">
                            <tr>
                                <td>
                                    <div class="ebill-bg-top">
                                        <h1>
                                            订单详情</h1>
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="order1">
                                <td>
                                    <div>
                                        <strong style="color: #0C88BE;">订单信息</strong>
                                    </div>
                                    <table id="table9" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <tr>
                                            <td class="tdNew" style="width: 12%">
                                                订单编号:
                                            </td>
                                            <td style="width: 26%" class=" td1">
                                                <asp:Label ID="lblOrderId" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew" style="width: 10%">
                                                PNR:
                                            </td>
                                            <td style="width: 20%; font-weight: bold">
                                                <asp:Label ID="lblPNR" runat="server" Text=""></asp:Label>
                                                <asp:Label ID="lblShowPNR" runat="server" Text="" Visible="false"></asp:Label>
                                            </td>
                                            <td class="tdNew" style="width: 12%">
                                                订单金额:
                                            </td>
                                            <td style="width: 20%" class=" td1">
                                                <asp:Label ID="lblPayMoney" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                订单来源:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblOrderSourceType" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                订单状态:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblOrderStatusCode" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                政策点数:
                                            </td>
                                            <td class="td1" align="center" valign="middle">
                                                <asp:Label ID="lblPolicyPoint" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                支付方式:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayWay" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                内部交易流水号:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblInPayNo" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                支付流水号:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayNo" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                支付状态:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayStatus" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                <span runat="server" id="spanZC" visible="false">政策来源:</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPolicySource" Visible="false" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                <asp:Label ID="td_PayCheck0" runat="server" Text="(支付/退废)检查:"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <span id="td_PayCheck1" runat="server">
                                                    <asp:DropDownList ID="ddlA9" runat="server">
                                                        <asp:ListItem Value="0" Text="要检查">要检查</asp:ListItem>
                                                        <asp:ListItem Value="1" Text="不检查">不检查</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <span class="btn btn-ok-s" id="span_A9" runat="server">
                                                        <asp:Button ID="btnUpStatus" runat="server" Text="修改" OnClick="btnUpStatus_Click" />
                                                    </span></span>
                                            </td>
                                        </tr>
                                        <tr id="trDiscountDetail" runat="server" visible="false">
                                            <td class="tdNew">
                                                手续费率:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblHandlingRate" runat="server"></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                扣点关系:
                                            </td>
                                            <td style="text-align: left">
                                                <asp:Label ID="lblDiscountDetail" runat="server"></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                            </td>
                                            <td align="left">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                支付账号:
                                            </td>
                                            <td style="text-align: center">
                                                <asp:Label ID="lblZFZH" runat="server"></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                废、改处理时间:
                                            </td>
                                            <td colspan="3" style="text-align: left">
                                                <asp:Label ID="labFGQ" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew" style="background-image: url('../img/title1.png');">
                                                客规参考:
                                            </td>
                                            <td colspan="5" style="text-align: left;">
                                                <asp:Label ID="labKeGui" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew" style="background-image: url('../img/title1.png');">
                                                政策备注:
                                            </td>
                                            <td colspan="5" style="text-align: left">
                                                <asp:Label ID="lblPolicyRemark" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trOutOrder" visible="false">
                                <td>
                                    <div>
                                        <strong style="color: #0C88BE;">代付信息</strong>
                                    </div>
                                    <table id="table1" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <tr>
                                            <td class="tdNew" style="width: 12%">
                                                外部订单号:
                                            </td>
                                            <td style="width: 26%" class=" td1">
                                                <asp:Label ID="lblOutOrderId" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew" style="width: 10%">
                                                代付状态:
                                            </td>
                                            <td style="width: 20%; font-weight: bold">
                                                <asp:Label ID="lblOutOrderPayFlag" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew" style="width: 12%">
                                                外部订单代付金额:
                                            </td>
                                            <td style="width: 20%" class=" td1">
                                                <asp:Label ID="lblOutOrderPayMoney" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                政策点数:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPolicyPoint2" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                外部支付流水号:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblOutOrderPayNo" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                外部订单代付时间:
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                废、改处理时间:
                                            </td>
                                            <td colspan="5" style="text-align: left">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="trRepSkyWayOld" visible="false">
                                <td>
                                    <div>
                                        <strong style="color: #0C88BE;">原行程信息</strong>
                                    </div>
                                    <table id="table2" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <thead>
                                            <tr>
                                                <th>
                                                    起飞城市
                                                </th>
                                                <th>
                                                    到达城市
                                                </th>
                                                <th>
                                                    起飞日期
                                                </th>
                                                <th>
                                                    到达日期
                                                </th>
                                                <th>
                                                    承运人
                                                </th>
                                                <th>
                                                    航班号
                                                </th>
                                                <th>
                                                    舱位
                                                </th>
                                                <th>
                                                    折扣
                                                </th>
                                                <th>
                                                    机型
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepSkyWayOld" runat="server">
                                            <ItemTemplate>
                                                <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("FromCityName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("ToCityName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%-- <%#Eval("FromDate")%>--%>
                                                        <%# ShowText(4,Eval("FromDate"))%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%-- <%#Eval("ToDate")%>--%>
                                                        <%# ShowText(4, Eval("ToDate"))%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("CarryName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("FlightCode")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("Space")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center; color: Red;">
                                                        <%#Eval("Discount")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("Aircraft")%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div>
                                        <strong style="color: #0C88BE;">行程信息</strong>
                                    </div>
                                    <table id="table4" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <thead>
                                            <tr>
                                                <th>
                                                    起飞城市
                                                </th>
                                                <th>
                                                    到达城市
                                                </th>
                                                <th>
                                                    起飞日期
                                                </th>
                                                <th>
                                                    到达日期
                                                </th>
                                                <th>
                                                    承运人
                                                </th>
                                                <th>
                                                    航班号
                                                </th>
                                                <th>
                                                    舱位
                                                </th>
                                                <th>
                                                    折扣
                                                </th>
                                                <th>
                                                    机型
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepSkyWay" runat="server">
                                            <ItemTemplate>
                                                <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("FromCityName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("ToCityName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%-- <%#Eval("FromDate")%>--%>
                                                        <%# ShowText(4,Eval("FromDate"))%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%--<%#Eval("ToDate")%>--%>
                                                        <%# ShowText(4, Eval("ToDate"))%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("CarryName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("CarryCode")%>
                                                        <%#Eval("FlightCode")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("Space")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center; color: Red;">
                                                        <%#Eval("Discount")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("Aircraft")%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div>
                                        <strong style="color: #0C88BE;">乘客信息</strong>
                                    </div>
                                    <table id="table6" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <thead>
                                            <tr>
                                                <th>
                                                    乘客姓名
                                                </th>
                                                <th>
                                                    乘客类型
                                                </th>
                                                <th>
                                                    证件类型
                                                </th>
                                                <th>
                                                    证件号
                                                </th>
                                                <th>
                                                    票号
                                                </th>
                                                <th>
                                                    手机号
                                                </th>
                                                <th>
                                                    机票类型
                                                </th>
                                                <th>
                                                    舱位价
                                                </th>
                                                <th>
                                                    机建/燃油
                                                </th>
                                                <th>
                                                    退废手续费
                                                </th>
                                                <%-- <th>
                                                    备注
                                                </th>--%>
                                                <th>
                                                    行程单号
                                                </th>
                                                <th>
                                                    操作
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepPassenger" runat="server" OnItemCommand="RepPassenger_ItemCommand">
                                            <ItemTemplate>
                                                <tr style="line-height: 30px; width: 100%;" name="trPassenger" id='tr_<%# Eval("id").ToString() %>'>
                                                    <td style="width: 6%; text-align: center;">
                                                        <%#Eval("PassengerName")%>
                                                    </td>
                                                    <td style="width: 8%; text-align: center;">
                                                        <%--<%#Eval("PassengerType")%>--%>
                                                        <%#  GetDictionaryName("6", Eval("PassengerType").ToString())%>
                                                    </td>
                                                    <td style="width: 6%; text-align: center;">
                                                        <%--   <%#Eval("CType")%>--%>
                                                        <%#  GetDictionaryName("7", Eval("CType").ToString())%>
                                                    </td>
                                                    <td style="width: 12%; text-align: center;">
                                                        <%--证件号--%><%# IsShowSsrUpdate(Eval("Remark"), ReplaceCHD(Eval("PassengerName")), Eval("PassengerType").ToString(), Eval("Cid"), Eval("id").ToString())%><input
                                                            id='Hid_CID_<%#Eval("id") %>' type="hidden" value='<%# Eval("Cid") %>' />
                                                    </td>
                                                    <td style="width: 12%; text-align: center;">
                                                        <%--票号--%><%#ShowTK(Eval("id"),Eval("PassengerName"), Eval("TicketNumber"), Eval("TravelNumber"))%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# GetTel(Eval("A10"))%>
                                                    </td>
                                                    <td style="width: 8%;">
                                                        <%#  GetDictionaryName("9", Eval("TicketStatus").ToString())%>
                                                    </td>
                                                    <td style="width: 6%; color: Red;">
                                                        <%#Eval("PMFee")%>
                                                        <% if (mCompany != null && mCompany.RoleType == 1)
                                                           { %>
                                                        <br />
                                                        <a href="##" onclick="return showPnrCon();">编码信息</a>
                                                        <%} %>
                                                    </td>
                                                    <td style="width: 6%;">
                                                        <%#Eval("ABFee")%>
                                                        /
                                                        <%#Eval("FuelFee")%>
                                                    </td>
                                                    <td style="width: 6%; color: Red;">
                                                        <%#Eval("TGQHandlingFee")%>
                                                    </td>
                                                    <%--   <td style="width: 4%; text-align: center;">
                                                        <%#Eval("Remark")%>
                                                    </td>--%>
                                                    <td style="width: 8%;">
                                                        <%#ShowText(3, Eval("TravelNumber"), Eval("TravelStatus"))%>
                                                    </td>
                                                    <td style="width: 8%; text-align: center;">
                                                        <span class='<%# ShowText(2,"|100001000034|100001000052|100001000050|")  %>'><a href="#"
                                                            onclick="return GoPrint('../TravelNumManage/<%=IsOpenTravelPrintUpdate %>?OrderId=<%#Eval("OrderId") %>&PasId=<%# Eval("id") %>&currentuserid=<%=this.mUser.id.ToString() %>');">
                                                            打印票据</a><br />
                                                        </span>
                                                        <%--行程单信息--%>
                                                        <span id="SpanTrip" class='<%# ShowText(0,"") %>'><a href="#" onclick="window.open('../TravelNumManage/PrintDownLoad.htm');return false;">
                                                            行程单下载</a><br />
                                                            <%# ShowTrip(Eval("id").ToString(), Eval("PassengerName"), Eval("TicketNumber"), Eval("TravelNumber"), Eval("TravelStatus"))%>
                                                        </span>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr id="tr_PolicySource" runat="server" visible="false">
                                <td align="left">
                                    <table id="tb_policySource" class="else-table" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <th>
                                                政策来源
                                            </th>
                                            <td>
                                                <asp:DropDownList ID="ddlPolicySource" runat="server" Width="150px">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnUpdatePolicySource" runat="server" Text="修改" OnClick="btnUpdatePolicySource_Click" /></span>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="ptShow" visible="false">
                                <td>
                                    <div>
                                        <strong style="color: #0C88BE;">收支信息</strong>
                                    </div>
                                    <table id="table7" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <thead>
                                            <tr>
                                                <th>
                                                    公司编号
                                                </th>
                                                <th>
                                                    公司名称
                                                </th>
                                                <th>
                                                    类型
                                                </th>
                                                <th>
                                                    手续费
                                                </th>
                                                <th>
                                                    (应)收支金额
                                                </th>
                                                <th>
                                                    (实)收支金额
                                                </th>
                                                <th>
                                                    收支账号
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepPayDetail" runat="server">
                                            <ItemTemplate>
                                                <tr style="line-height: 30px; width: 100%;">
                                                    <td style="width: 20%; text-align: center;">
                                                        <%#Eval("CpyNo")%>
                                                    </td>
                                                    <td style="width: 20%; text-align: center;">
                                                        <%#Eval("CpyName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("PayType")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("BuyPoundage")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("PayMoney")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("RealPayMoney")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("PayAccount")%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="table3" class="table_info" border="0" cellspacing="0" cellpadding="0"
                                        width="100%">
                                        <tr runat="server" class="leftliebiao_checi" id="trYDRemark" visible="false">
                                            <td>
                                                备注说明：
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtYDRemark" Enabled="false" runat="server" Height="60px" TextMode="MultiLine"
                                                    Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr runat="server" class="leftliebiao_checi" id="trTGQApplyReason" visible="false">
                                            <td>
                                                申请理由：
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTGQApplyReason" Enabled="false" runat="server" Height="60px"
                                                    TextMode="MultiLine" Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr runat="server" class="leftliebiao_checi" id="trTGQRefusalReason" visible="false">
                                            <td>
                                                拒绝理由：
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTGQRefusalReason" Enabled="false" runat="server" Height="60px"
                                                    TextMode="MultiLine" Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr runat="server" class="leftliebiao_checi" id="trCPRemark" visible="false">
                                            <td>
                                                出票备注：
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCPRemark" runat="server" Height="60px" TextMode="MultiLine" Width="600px"></asp:TextBox>
                                                <span class="btn btn-ok-s" id="span_CPRemark" runat="server" visible="false">
                                                    <asp:Button ID="btnUpdateCPRemark" runat="server" Text="修改" OnClick="btnUpdateCPRemark_Click"
                                                        OnClientClick="return CPValidate();" />
                                                </span>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table>
                                        <tr>
                                            <td align="center">
                                                <span class="btn btn-ok-s">
                                                    <input type="button" id="btnGo" value="返 回" onclick="location.href=document.getElementById('Hid_GoUrl').value" />
                                                </span>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="ebill">
                                        <div>
                                            <span class="ebill-account"><span class="inner"><span class="inner">基础操作信息</span> </span>
                                            </span>
                                        </div>
                                        <div class="ebill-info-bg-inner">
                                            <table border="0" cellspacing="0" cellpadding="0" width="100%">
                                                <tr>
                                                    <td style="text-align: right; border: 1px">
                                                        <asp:Label ID="lbl1" runat="server" Text="锁定操作员：" Width="100px"></asp:Label>
                                                    </td>
                                                    <td class="tab_in_td_f" style="width: 40%;">
                                                        <asp:Label ID="lblLockId" runat="server" Text="Label"></asp:Label>
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Label ID="lbl2" runat="server" Text="锁定时间：" Width="100px"></asp:Label>
                                                    </td>
                                                    <td class="tab_in_td_f" style="width: 40%;">
                                                        <asp:Label ID="lblLockTime" runat="server" Text="Label"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="table5" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                                width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>
                                                            操作时间
                                                        </th>
                                                        <th>
                                                            操作员账号
                                                        </th>
                                                        <th>
                                                            操作员姓名
                                                        </th>
                                                        <th>
                                                            操作类型
                                                        </th>
                                                        <th>
                                                            详细记录
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <asp:Repeater ID="RepOrderLog" runat="server">
                                                    <ItemTemplate>
                                                        <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperTime")%>
                                                            </td>
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperLoginName")%>
                                                            </td>
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperUserName")%>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# ShowText(1,Eval("OperType"))%>
                                                            </td>
                                                            <td style="width: 45%; text-align: center;">
                                                                <asp:Label Style="word-break: break-all; white-space: normal" ID="lblLogContent"
                                                                    Width="100%" runat="server" Text=' <%#Eval("OperContent")%>' ToolTip='<%#Eval("OperContent") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                            <div class="fn-clear">
                                            </div>
                                        </div>
                                        <table width="100%" cellspacing="0" cellpadding="0" border="0" class="ebill-info-bg">
                                            <tbody>
                                                <tr>
                                                    <td class="ebill-info-bgl">
                                                    </td>
                                                    <td class="ebill-info-bgc">
                                                    </td>
                                                    <td class="ebill-info-bgr">
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="mainr">
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <input type="hidden" id="Hid_GoUrl" runat="server" />
    <%--订单号--%>
    <input type="hidden" id="Hid_OrderId" runat="server" />
    <%--分销公司编号--%>
    <input type="hidden" id="Hid_CpyNo" runat="server" value="0" />
    <%--Office--%>
    <input type="hidden" id="Hid_Office" runat="server" value="0" />
    <%--供应商公司编号--%>
    <input type="hidden" id="Hid_GYCpyNo" runat="server" />
    <%--行程单数据--%>
    <input type="hidden" id="Hid_TripData" runat="server" />
    <%--行程单数据可用号段--%>
    <input type="hidden" id="Hid_StartEndNum" runat="server" />
    <%--行程单用户设置号段--%>
    <input type="hidden" id="Hid_SetStartEndNum" runat="server" />
    <%--参数ID--%>
    <input type="hidden" id="Hid_ParamId" runat="server" />
    <%--是否有可用行程单 0无 1有--%>
    <input type="hidden" id="Hid_IsValid" runat="server" value="0" />
    <%--用户角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" value="0" />
    <%--行程单申请参数--%>
    <input type="hidden" id="Hid_ApplyApram" runat="server" value="0" />
    <%--航段实体信息--%>
    <input type="hidden" id="Hid_SkyWay" runat="server" />
    <%--订单状态--%>
    <input type="hidden" id="Hid_OrderStatusCode" runat="server" />
    <%--新版本通知同步旧版本--%>
    <input type="hidden" id="Hid_IPAddress" runat="server" value="100001000049|210.14.138.26:251^100001000045|210.14.138.26:301^100001000051|210.14.138.26:221^100001000038|b2b.hakxd.com^100001000035|210.14.138.26:191^100001000034|210.14.138.26:177^100001000052|210.14.138.26:121^100001000050|210.14.138.26:223" />
    </form>
</body>
</html>
