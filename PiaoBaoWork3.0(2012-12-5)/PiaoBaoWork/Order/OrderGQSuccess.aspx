<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderGQSuccess.aspx.cs" Inherits="Order_OrderGQSuccess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>确认改签</title>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/Calendar/WdatePicker.js"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../css/detail.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
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
        .else-table tbody .tbl_Point td
        {
            border: none;
        }
    </style>
    <script type="text/javascript">

        function showdialogOne(t, One) {
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
                        window.location.href = One;
                    }
                }
            });
        }

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

        function showLoading() {
            var divValue = "<div id=\"loading\" style=\"display: block;\">正在处理请稍等...<br /><img src=\"../img/loading.gif\">";
            divValue += "<br /></div>";

            $("#showOne").html(divValue);
            $("#showOne").dialog({
                title: '温馨提示',
                bgiframe: true,
                width: 300,
                height: 200,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                }
            });
        }

        function opOrder(msg, type) {

            if (confirm(msg)) {
                if (type == 1) {
                    var UnApp = $("#txtTGQRefusalReason").val();
                    if (UnApp == "") {
                        showdialog("请输入您拒绝理由！");
                        return false;
                    }
                }
                showLoading(); //加载遮罩层
            } else {
                return false;
            }
            return true;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="tabs">
        <div id="tabs-1" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
            <table width="100%" align="center" border="0" cellpadding="0" cellspacing="0" style="border: 1px solid #E6E6E6;">
                <tr>
                    <td>
                        <table width="100%" align="center" class="detail" border="0" cellpadding="5" cellspacing="0"
                            style="padding: 5px;">
                            <tr>
                                <td>
                                    <div class="ebill-bg-top">
                                        <h1>
                                            确认改签</h1>
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
                                                政策来源:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPolicySource" runat="server" Text=""></asp:Label>
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
                                                订单状态:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblOrderStatusCode" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                支付状态:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayStatus" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                机票状态检查:
                                            </td>
                                            <td style="text-align: left">
                                                <asp:DropDownList ID="ddlCheckType" Enabled="false" runat="server">
                                                    <asp:ListItem Text="需要检查" Value="0" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="不需要检查" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                                <span class="btn btn-ok-s" style="display: none;">
                                                    <asp:Button ID="btnUpdate" runat="server" Text="修 改" />
                                                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                废、改处理时间:
                                            </td>
                                            <td colspan="5" style="text-align: left">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew" style="background-image: url('../img/title1.png');">
                                                客规参考:
                                            </td>
                                            <td colspan="5" style="text-align: left;">
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
                                                    机票状态
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
                                                <th>
                                                    备注
                                                </th>
                                                <th>
                                                    行程单号
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepPassenger" runat="server">
                                            <ItemTemplate>
                                                <tr style="line-height: 30px; width: 100%;" name="trPassenger" id='tr_<%# Eval("Id").ToString() %>'>
                                                    <td style="width: 6%; text-align: center;">
                                                        <%#Eval("PassengerName")%>
                                                    </td>
                                                    <td style="width: 4%; text-align: center;">
                                                        <%-- <%#Eval("PassengerType")%>--%>
                                                        <%#  GetDictionaryName("6", Eval("PassengerType").ToString())%>
                                                    </td>
                                                    <td style="width: 6%; text-align: center;">
                                                        <%-- <%#Eval("CType")%>--%>
                                                        <%#  GetDictionaryName("7", Eval("CType").ToString())%>
                                                    </td>
                                                    <td style="width: 18%; text-align: center;">
                                                        <%#Eval("Cid")%>
                                                    </td>
                                                    <td style="width: 18%; text-align: center;">
                                                        <%#Eval("TicketNumber")%>
                                                    </td>
                                                    <td style="width: 4%;">
                                                        <%--  <%#Eval("TicketStatus")%>--%>
                                                        <%#  GetDictionaryName("9", Eval("CType").ToString())%>
                                                    </td>
                                                    <td style="width: 6%; color: Red;">
                                                        <%#Eval("PMFee")%>
                                                    </td>
                                                    <td style="width: 6%;">
                                                        <%#Eval("ABFee")%>
                                                        /
                                                        <%#Eval("FuelFee")%>
                                                    </td>
                                                    <td style="width: 6%; color: Red;">
                                                        <%#Eval("TGQHandlingFee")%>
                                                    </td>
                                                    <td style="width: 4%; text-align: center;">
                                                        <%#Eval("Remark")%>
                                                    </td>
                                                    <td style="width: 8%;">
                                                        <%#Eval("TravelNumber")%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td width="100%">
                                    <table id="table3" class="table_info" border="0" cellspacing="0" cellpadding="0"
                                        width="100%">
                                        <tr>
                                            <td style="text-align: right;">
                                                <asp:Label ID="lblYDRemark" runat="server" Text="备注说明：" Width="150px"></asp:Label>
                                            </td>
                                            <td style="width: 98%; text-align: left;">
                                                <asp:TextBox ID="txtYDRemark" Enabled="false" runat="server" Height="60px" TextMode="MultiLine"
                                                    Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right;">
                                                <asp:Label ID="lblTGQApplyReason" runat="server" Text="申请理由：" Width="150px"></asp:Label>
                                            </td>
                                            <td style="width: 98%; text-align: left;">
                                                <asp:TextBox ID="txtTGQApplyReason" Enabled="false" runat="server" Height="60px"
                                                    TextMode="MultiLine" Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right;">
                                                <asp:Label ID="lblTGQRefusalReason" runat="server" Text="拒绝理由：" Width="150px"></asp:Label>
                                            </td>
                                            <td style="width: 98%; text-align: left;">
                                                <asp:TextBox ID="txtTGQRefusalReason" runat="server" Height="60px" TextMode="MultiLine"
                                                    Width="600px"></asp:TextBox>
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
                                                    <asp:Button ID="btnOk" runat="server" Text=" 确认改签  " OnClick="btnOk_Click" />
                                                </span><span class="btn btn-ok-s">
                                                    <asp:Button ID="btnUnOk" runat="server" Text=" 拒绝改签 " OnClientClick="return opOrder('是否拒绝审核？',1);"
                                                        OnClick="btnUnOk_Click" />
                                                </span><span class="btn btn-ok-s" runat="server" id="spanReturn" visible="false">
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
                                                            <td style="width: 20%; text-align: center;">
                                                                <%#Eval("OperTime")%>
                                                            </td>
                                                            <td style="width: 20%; text-align: center;">
                                                                <%#Eval("OperLoginName")%>
                                                            </td>
                                                            <td style="width: 20%; text-align: center;">
                                                                <%#Eval("OperUserName")%>
                                                            </td>
                                                            <td style="width: 20%; text-align: center;">
                                                                <%#Eval("OperType")%>
                                                            </td>
                                                            <td style="width: 40%; text-align: center;">
                                                                <asp:Label ID="lblLogContent" runat="server" Text=' <%#Eval("OperContent")%>' ToolTip='<%#Eval("OperContent") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="showOne">
    </div>
    <input type="hidden" id="Hid_GoUrl" runat="server" />
    </form>
</body>
</html>
