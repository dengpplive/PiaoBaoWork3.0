<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderUpdate.aspx.cs" Inherits="Order_OrderUpdate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>修改订单信息</title>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../css/detail.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">

        function showdialog(t) {
            $("#showOne").html(t);
            $("#showOne").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $(this).dialog('close');
                    }
                }
            });
        }

        function GoPrint(url) {
            window.open(url);
            return false;
        }
    </script>
    <script type="text/javascript" src="../js/js_OrderDetail.js"></script>
    <style type="text/css">
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
</head>
<body>
    <form id="form2" runat="server">
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
                                            修改订单信息</h1>
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
                                            <td class="tdNew" style="width: 12%;">
                                                订单编号:
                                            </td>
                                            <td style="width: 26%; text-align: left;">
                                                <asp:TextBox ID="txtOrderId" runat="server" Enabled="false"></asp:TextBox>
                                            </td>
                                            <td class="tdNew" style="width: 10%">
                                                PNR:
                                            </td>
                                            <td style="width: 20%; text-align: left;">
                                                <asp:TextBox ID="txtPNR" runat="server" MaxLength="6"></asp:TextBox>
                                            </td>
                                            <td class="tdNew" style="width: 12%">
                                                订单金额:
                                            </td>
                                            <td style="width: 20%; text-align: left;">
                                                <asp:TextBox ID="txtPayMoney" runat="server" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                订单来源:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:DropDownList ID="ddlOrderSourceType" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="tdNew">
                                                政策来源:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:DropDownList ID="ddlPolicySource" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="tdNew">
                                                订单状态:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:DropDownList ID="ddlOrderStatusCode" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                支付方式:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:DropDownList ID="ddlPayWay" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="tdNew">
                                                内部交易流水号:
                                            </td>
                                            <td style="text-align: left">
                                                <asp:TextBox ID="txtInPayNo" runat="server"></asp:TextBox>
                                            </td>
                                            <td class="tdNew">
                                                支付流水号:
                                            </td>
                                            <td style="text-align: left">
                                                <asp:TextBox ID="txtPayNo" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                原政策点数:
                                            </td>
                                            <td align="left" valign="middle">
                                                <asp:TextBox ID="txtOldPolicyPoint" runat="server" Enabled="false"></asp:TextBox>
                                            </td>
                                            <td class="tdNew">
                                                出票政策点数:
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtPolicyPoint" runat="server" Enabled="false"></asp:TextBox>
                                            </td>
                                            <td class="tdNew">
                                                最终政策点数:
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtReturnPoint" runat="server" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <td class="tdNew">
                                            原现返金额:
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtOldReturnMoney" runat="server" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td class="tdNew">
                                            出票政策现返:
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtReturnMoney" runat="server" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td class="tdNew">
                                            出票方收款金额:
                                        </td>
                                        <td style="text-align: left">
                                            <asp:TextBox ID="txtOrderMoney" runat="server" Enabled="false"></asp:TextBox>
                                        </td>
                            </tr>
                            <tr>
                                <td class="tdNew">
                                    废、改处理时间:
                                </td>
                                <td style="text-align: left">
                                    <asp:TextBox ID="txtFGQ" runat="server" Width="100px"></asp:TextBox>
                                    <span style="color: Red">格式:05:00-22:30</span>
                                </td>
                                <td class="tdNew">
                                    支付状态:
                                </td>
                                <td style="text-align: left">
                                    <asp:DropDownList ID="ddlPayStatus" runat="server">
                                        <asp:ListItem Value="0" Text="未付">未付</asp:ListItem>
                                        <asp:ListItem Value="1" Text="已付">已付</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="tdNew">
                                    (支付/退废)检查:
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlA9" runat="server">
                                        <asp:ListItem Value="0" Text="要检查">要检查</asp:ListItem>
                                        <asp:ListItem Value="1" Text="不检查">不检查</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNew" style="background-image: url('../img/title1.png');">
                                    客规参考:
                                </td>
                                <td colspan="5" style="text-align: left;">
                                    <asp:TextBox ID="txtKeGui" runat="server" Height="50px" TextMode="MultiLine" Width="600px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNew" style="background-image: url('../img/title1.png');">
                                    政策备注:
                                </td>
                                <td colspan="5" style="text-align: left">
                                    <asp:TextBox ID="txtPolicyRemark" runat="server" Height="50px" TextMode="MultiLine"
                                        Width="600px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNew" style="background-image: url('../img/title1.png');">
                                    <span style="color: Red;">修改理由:</span>
                                </td>
                                <td colspan="5" style="text-align: left">
                                    <asp:TextBox ID="txtOrderReason" runat="server" Height="50px" TextMode="MultiLine"
                                        Width="600px"></asp:TextBox>
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
                                <td style="width: 26%; text-align: left">
                                    <asp:TextBox ID="txtOutOrderId" runat="server"></asp:TextBox>
                                </td>
                                <td class="tdNew" style="width: 10%">
                                    代付状态:
                                </td>
                                <td style="width: 20%; font-weight: bold; text-align: left">
                                    <asp:DropDownList ID="ddlOutOrderPayFlag" runat="server">
                                        <asp:ListItem Value="1" Text="已付">已付</asp:ListItem>
                                        <asp:ListItem Value="0" Text="未付">未付</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td class="tdNew" style="width: 12%">
                                    外部订单代付金额:
                                </td>
                                <td style="width: 20%; text-align: left">
                                    <asp:TextBox ID="txtOutOrderPayMoney" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNew">
                                    代付点数:
                                </td>
                                <td style="text-align: left">
                                    <asp:TextBox ID="txtPolicyPoint2" runat="server"></asp:TextBox>
                                </td>
                                <td class="tdNew">
                                    外部支付流水号:
                                </td>
                                <td style="text-align: left">
                                    <asp:TextBox ID="txtOutOrderPayNo" runat="server"></asp:TextBox>
                                </td>
                                <td class="tdNew">
                                    外部订单代付时间:
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" style="text-align: center">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnOrderUpdate" runat="server" Text="修改订单" OnClientClick="return confirm('确定修改?请仔细核对数据');"
                                OnClick="btnOrderUpdate_Click" /></span>
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
                                            <%#Eval("FromDate")%>
                                        </td>
                                        <td style="width: 10%; text-align: center;">
                                            <%#Eval("ToDate")%>
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
                                        <td style="width: 10%; text-align: center;">
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
                                    <th>
                                        <span style="color: Red;">修改理由</span>
                                    </th>
                                    <th>
                                        操作
                                    </th>
                                </tr>
                            </thead>
                            <asp:Repeater ID="RepSkyWay" runat="server" OnItemCommand="RepSkyWay_ItemCommand"
                                OnItemDataBound="RepSkyWay_ItemDataBound">
                                <ItemTemplate>
                                    <tr class="leftliebiao_checi" style="line-height: 30px;">
                                        <td style="width: 10%; text-align: center;">
                                            <%--起飞城市--%>
                                            <asp:TextBox runat="server" ID="FromCityCode" Text='<%# Eval("FromCityCode") %>'
                                                Width="30px"></asp:TextBox>
                                            <asp:TextBox runat="server" ID="FromCityName" Text='<%# Eval("FromCityName") %>'
                                                Width="60px"></asp:TextBox>
                                        </td>
                                        <td style="width: 10%; text-align: center;">
                                            <%--到达城市--%>
                                            <asp:TextBox runat="server" ID="ToCityCode" Text='<%# Eval("ToCityCode") %>' Width="30px"></asp:TextBox>
                                            <asp:TextBox runat="server" ID="ToCityName" Text='<%# Eval("ToCityName") %>' Width="60px"></asp:TextBox>
                                        </td>
                                        <td style="width: 12%; text-align: center;">
                                            <%--起飞日期--%><asp:TextBox runat="server" ID="FromDate" Text='<%# GetTime(Eval("FromDate"),1) %>'
                                                onfocus="WdatePicker({isClear:false,autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" CssClass="Wdate inputtxtdat"
                                                Width="140px"></asp:TextBox>
                                        </td>
                                        <td style="width: 12%; text-align: center;">
                                            <%--到达日期--%><asp:TextBox runat="server" ID="ToDate" Text='<%# GetTime(Eval("ToDate"),1) %>'
                                                onfocus="WdatePicker({isClear:false,autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" CssClass="Wdate inputtxtdat"
                                                Width="140px"></asp:TextBox>
                                        </td>
                                        <td style="width: 8%; text-align: center;">
                                            <%--承运人--%>
                                            <asp:Label runat="server" ID="lblCarryCode" Text='<%# Eval("CarryCode")%>' Visible="false"></asp:Label>
                                            <asp:DropDownList runat="server" ID="CarryCode">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="width: 8%; text-align: center;">
                                            <%--航班号--%><asp:TextBox runat="server" ID="FlightCode" Text='<%# Eval("FlightCode") %>'
                                                Width="50px"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%; text-align: center;">
                                            <%--舱位--%><asp:TextBox runat="server" ID="Space" Text='<%# Eval("Space") %>' Width="50px"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%; text-align: center; color: Red;">
                                            <%--折扣--%><asp:TextBox runat="server" ID="Discount" Text='<%# Eval("Discount") %>'
                                                Width="50px"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%; text-align: center; color: Red;">
                                            <%--机型--%><asp:TextBox runat="server" ID="Aircraft" Text='<%#Eval("Aircraft") %>'
                                                Width="50px"></asp:TextBox>
                                        </td>
                                        <td style="text-align: center; color: Red;">
                                            <%--修改原因描述--%><asp:TextBox runat="server" ID="SkyWayReason" Text=''></asp:TextBox>
                                        </td>
                                        <td style="width: 10%; text-align: center; color: Red;">
                                            <span class="btn btn-ok-s">
                                                <asp:Button ID="btnSkyWayUpdate" runat="server" Text=" 修 改 " CommandArgument='<%# Eval("id")%>'
                                                    CommandName="Update" OnClientClick="return confirm('确定修改?请仔细核对数据');" />
                                            </span>
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
                                        机票状态
                                    </th>
                                    <th>
                                        舱位价
                                    </th>
                                    <th>
                                        机建
                                    </th>
                                    <th>
                                        燃油
                                    </th>
                                    <th>
                                        退废手续费
                                    </th>
                                    <th>
                                        退废改操作
                                    </th>
                                    <th>
                                        行程单状态
                                    </th>
                                    <th>
                                        行程单号
                                    </th>
                                    <th>
                                        <span style="color: Red;">修改理由</span>
                                    </th>
                                    <th>
                                        操作
                                    </th>
                                </tr>
                            </thead>
                            <asp:Repeater ID="RepPassenger" runat="server" OnItemCommand="RepPassenger_ItemCommand"
                                OnItemDataBound="RepPassenger_ItemDataBound">
                                <ItemTemplate>
                                    <tr style="line-height: 30px;">
                                        <td style="width: 6%; text-align: center;">
                                            <%--姓名--%>
                                            <asp:TextBox runat="server" ID="PassengerName" Text='<%#Eval("PassengerName") %>'
                                                Width="45px"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%; text-align: center;">
                                            <%--类型--%>
                                            <asp:Label runat="server" ID="lblPassengerType" Text='<%# Eval("PassengerType")%>'
                                                Visible="false"></asp:Label>
                                            <asp:DropDownList runat="server" ID="PassengerType" Width="50px">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="width: 6%; text-align: center;">
                                            <%--证件类型--%>
                                            <asp:Label runat="server" ID="lblCType" Text='<%# Eval("CType")%>' Visible="false"></asp:Label>
                                            <asp:DropDownList runat="server" ID="CType" Width="70px">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="width: 12%; text-align: center;">
                                            <%--证件号码--%>
                                            <asp:TextBox runat="server" ID="Cid" Text='<%#Eval("Cid") %>' Width="150px"></asp:TextBox>
                                        </td>
                                        <td style="width: 12%; text-align: center;">
                                            <%--票号--%>
                                            <asp:TextBox runat="server" ID="TicketNumber" Text='<%#Eval("TicketNumber") %>' Width="120px"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%;">
                                            <%--状态--%>
                                            <asp:Label runat="server" ID="lblTicketStatus" Text='<%# Eval("TicketStatus")%>'
                                                Visible="false"></asp:Label>
                                            <asp:DropDownList runat="server" ID="TicketStatus" Width="50px">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="width: 8%; color: Red;">
                                            <%--舱位价--%>
                                            <asp:TextBox runat="server" ID="PMFee" Text='<%#Eval("PMFee")%>' Enabled="false"
                                                Width="60px"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%; color: Red;">
                                            <%--机建--%>
                                            <asp:TextBox runat="server" ID="ABFee" Text='<%#Eval("ABFee") %>' Enabled="false"
                                                Width="50px"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%; color: Red;">
                                            <%--燃油--%>
                                            <asp:TextBox runat="server" ID="FuelFee" Text='<%#Eval("FuelFee") %>' Enabled="false"
                                                Width="50px"></asp:TextBox>
                                        </td>
                                        <td style="width: 8%; color: Red;">
                                            <%--退废手续费--%>
                                            <asp:TextBox runat="server" ID="TGQHandlingFee" Text='<%# Eval("TGQHandlingFee")%>'
                                                Enabled="false" Width="50px"></asp:TextBox>
                                        </td>
                                        <td style="width: 4%; text-align: center;">
                                            <%--退废状态--%>
                                            <asp:HiddenField runat="server" ID="Hid_IsBack" Value='<%# Eval("IsBack")%>'></asp:HiddenField>
                                            <asp:DropDownList runat="server" ID="IsBack" Width="75px">
                                                <asp:ListItem Text="不能操作" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="可操作" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlTrvalStatus" runat="server">
                                                <asp:ListItem Value="0">未创建</asp:ListItem>
                                                <asp:ListItem Value="1">已创建</asp:ListItem>
                                                <asp:ListItem Value="2">已作废</asp:ListItem>
                                            </asp:DropDownList>
                                              <asp:HiddenField runat="server" ID="Hid_TripStatus" Value='<%# Eval("TravelStatus")%>'></asp:HiddenField>
                                        </td>
                                        <td>
                                            <%--行程单号--%>
                                            <asp:TextBox runat="server" ID="TravelNumber" Text='<%# Eval("TravelNumber")%>' Width="90px"></asp:TextBox>
                                        </td>
                                        <td style="width: 8%; color: Red;">
                                            <%--修改原因描述--%>
                                            <asp:TextBox runat="server" ID="PassengerReason" Width="100px" Text=''></asp:TextBox>
                                        </td>
                                        <td style="width: 8%; color: Red;">
                                            <span class="btn btn-ok-s">
                                                <asp:Button ID="btnPassengerUpdate" runat="server" Text=" 修 改 " CommandArgument='<%# Eval("id")%>'
                                                    CommandName="Update" OnClientClick="return confirm('确定修改?请仔细核对数据');" />
                                            </span>
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
                                                    <%# Eval("OperType")%>
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
            </tr> </table>
        </div>
    </div>
    <div id="showOne">
    </div>
    <div id="html">
    </div>
    <input type="hidden" id="Hid_GoUrl" runat="server" />
    </form>
</body>
</html>
