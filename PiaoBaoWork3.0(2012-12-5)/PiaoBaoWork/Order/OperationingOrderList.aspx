<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OperationingOrderList.aspx.cs"
    Inherits="Order_OperationingOrderList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>未付款订单</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script id="script_k0" src="../js/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
        .Search tr
        {
            height: 30px;
            line-height: 30px;
        }
        .tb-all-trade td
        {
            text-align: center;
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
    </style>
    <script type="text/javascript">
        //为Jquery重新命名
        var $J=jQuery=jQuery.noConflict(false);
        //-->

        function showdialog(t) {
            $J("#dd").html(t);
            $J("#dd").dialog({
                title: '标题',
                bgiframe: true,
                width: 300,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $J(this).dialog('close');
                    },
                    "取消": function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }

        function quXiaoOrder(obj,OrderSource) {

            $J('#Hid_id').val(obj);
            var divValue="";

            if(OrderSource=="1") {
                var IsOpenTFSplitPnr=$J("#Hid_IsOpenTFSplitPnr").val();
                var divValue=" <table width='100%'><tr><td></td>";
                if(IsOpenTFSplitPnr=="1") {
                    divValue+="<td align='left'>取消订单同时并取消PNR编码？</td></tr> ";
                    divValue+="<tr><td></td><td align='center'><br /><input id='cboType' type='checkbox' />保留PNR编码</td></tr>";
                } else {
                    divValue+="<td align='left'>确定要取消该订单吗？</td></tr> ";
                    divValue+="<tr style='display:none'><td></td><td align='left'> <br /><input id='cboType' type='checkbox' check='true' /> 保留PNR编码 </td></tr>";
                }

                divValue+=" </table>";

            } else {
                var divValue=" <table width='100%'><tr><td></td><td align='left'>确定要取消该订单吗？</td></tr> ";
                divValue+="<tr style='display:none'><td></td><td align='left'> <br /><input id='cboType' type='checkbox' check='true' /> 保留PNR编码 </td></tr>";
                divValue+=" </table>";
            }

            $J("#dd").html(divValue);
            $J("#dd").dialog({
                title: '标题',
                bgiframe: true,
                width: 300,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        fbtnRefuse();
                    },
                    "取消": function () {
                        btnClose();
                    }
                }
            });
        }
        function fbtnRefuse() {
            if($J("#cboType").attr("checked"))
                $J('#Hid_isCancelPnr').val('0'); //保留编码
            else
                $J('#Hid_isCancelPnr').val('1'); //取消编码

            $J("#dd").dialog("close");
            document.getElementById("btnCancelOrder").click();
        }
        function btnClose() {
            $J("#dd").dialog("close");
        }

    </script>
    <script type="text/javascript" src="../js/js_Copy.js"></script>
</head>
<body>
    <%--城市控件使用容器开始--%>
    <div id="jsContainer">
        <div id="jsHistoryDiv" class="hide">
            <iframe id="jsHistoryFrame" name="jsHistoryFrame" src="about:blank"></iframe>
        </div>
        <textarea id="jsSaveStatus" class="hide"></textarea>
        <div id="tuna_jmpinfo" style="visibility: hidden; position: absolute; z-index: 120;
            overflow: hidden;">
        </div>
        <div id="tuna_alert" style="display: none; position: absolute; z-index: 999; overflow: hidden;">
        </div>
        <%--日期容器--%>
        <div style="position: absolute; top: 0; z-index: 120; display: none" id="tuna_calendar"
            class="tuna_calendar">
        </div>
    </div>
    <%--城市控件使用容器结束--%>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="dd">
    </div>
    <div id="tabs">
        <div class="title">
            <span>未付款订单</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="moreSearch" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                订单号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtOrderId" Width="138px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                PNR：
                            </th>
                            <td>
                                <asp:TextBox ID="txtPNR" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                乘机日期：
                            </th>
                            <td>
                                <input id="txtFromDate1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                    onfocus="WdatePicker({isShowClear:false})" />-<input id="txtFromDate2" type="text"
                                        readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:false})" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                出发城市：
                            </th>
                            <td>
                                <input name="txtFromCity" class="inputtxtdat" type="text" id="txtFromCity" runat="server"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 110px;" mod_address_reference="hidFromCity" />
                                <input type="hidden" runat="server" id="hidFromCity" />
                            </td>
                            <th>
                                到达城市：
                            </th>
                            <td>
                                <input name="txtToCity" class="inputtxtdat" type="text" id="txtToCity" runat="server"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 110px;" mod_address_reference="hidToCity" />
                                <input type="hidden" runat="server" id="hidToCity" />
                            </td>
                            <th>
                                创建日期：
                            </th>
                            <td>
                                <input id="txtCreateTime1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                    onfocus="WdatePicker({isShowClear:false})" />-<input id="txtCreateTime2" type="text"
                                        readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:false})" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                乘机人：
                            </th>
                            <td>
                                <asp:TextBox ID="txtPassengerName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                航班号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtFlightCode" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                航空公司：
                            </th>
                            <td>
                                <uc1:SelectAirCode ID="SelectAirCode1" runat="server" IsDShowName="false" DefaultOptionValue="" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" style="text-align: center;">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" /></span>
                                <span style="display: none;">
                                    <asp:Button ID="btnCancelOrder" runat="server" Text="取消订单" OnClick="btnCancelOrder_Click" />
                                </span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                width="100%">
                <thead>
                    <tr>
                        <th>
                            订单编号
                        </th>
                        <th>
                            PNR
                        </th>
                        <th>
                            创建时间
                        </th>
                        <th>
                            乘机人
                        </th>
                        <th>
                            乘机时间
                        </th>
                        <th>
                            行程
                        </th>
                        <th>
                            航班号
                        </th>
                        <th>
                            订单金额
                        </th>
                        <th>
                            政策返点
                        </th>
                        <th>
                            订单状态
                        </th>
                        <th>
                            锁定帐号
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repOrderList" runat="server" OnItemCommand="repOrderList_ItemCommand"
                    OnItemDataBound="repOrderList_ItemDataBound">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                            <td>
                                <%# Eval("OrderId")%>
                                <%# ShowText(1, Eval("IsChdFlag"),Eval("IsCHDETAdultTK"))%>
                            </td>
                            <td>
                                <%# Eval("PNR")%>
                                <br />
                                <input type="button" onclick="copyToClipboard('<%# Eval("PNR")%>')" class="hide"
                                    value="复制" />
                            </td>
                            <td>
                                <%# Eval("CreateTime")%>
                            </td>
                            <td>
                                <%# Eval("PassengerName").ToString().Replace("|","<br>")%>
                            </td>
                            <td>
                                <%# Eval("AirTime")%>
                            </td>
                            <td>
                                <%#ShowText(3,Eval("Travel"))%>
                            </td>
                            <td>
                                <%# Eval("CarryCode")%>
                                <br />
                                <%# Eval("FlightCode")%>
                            </td>
                            <td>
                                <%# Eval("PayMoney")%>
                            </td>
                            <td>
                                <%# Eval("ReturnPoint")%>
                            </td>
                            <td>
                                <%# ShowText(2, 1, Eval("OrderStatusCode"))%>
                            </td>
                            <td>
                                <%# Eval("LockLoginName")%>
                            </td>
                            <td class="Operation" style="line-height: 30px;">
                                <asp:LinkButton runat="server" ID="lbtnNewPolicy" Visible="false" Text="重新确认订单" CommandName="NewPolicy"
                                    CommandArgument='<%#Eval("OrderId")%>'></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnPay" Text="订单支付" CommandName="Pay" CommandArgument='<%#Eval("id")%>'>
                                </asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnDetail" Text="订单详情" CommandName="Detail" CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnCancelOrder" Text="取消订单" CommandName="CancelOrder"
                                    CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                                <asp:HiddenField runat="server" ID="Hid_IsChdFlag" Value='<%#Eval("IsChdFlag")%>' />
                                <asp:HiddenField runat="server" ID="Hid_PolicySource" Value='<%#Eval("OrderSourceType")%>' />
                                <asp:HiddenField runat="server" ID="Hid_A1" Value='<%#Eval("A1")%>' />
                                <asp:HiddenField runat="server" ID="Hid_PNR" Value='<%#Eval("PNR")%>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
            </webdiyer:AspNetPager>
        </div>
    </div>
    <input type="hidden" id="Hid_id" value="" runat="server" />
    <input type="hidden" id="Hid_isCancelPnr" value="0" runat="server" />
    <input type="hidden" id="Hid_IsOpenTFSplitPnr" value="0" runat="server" />
    </form>
    <script type="text/javascript" language="javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../js/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        $J(ReLoad);  
    </script>
</body>
</html>
