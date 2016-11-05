<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderList.aspx.cs" Inherits="Order_OrderList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>订单综合查询</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
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
            color: red;
        }
        .green
        {
            color: green;
        }
        .tdbg
        {
            background-color: Red;
            font-weight: bold;
        }
    </style>
    <script type="text/javascript">
        var $J = jQuery.noConflict(false);
        function showdialog(t, param) {
            $J("#dd").html(t);
            $J("#dd").dialog({
                title: '标题',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $J(this).dialog('close');
                        if (param != null) {
                            if (param.type == "0") {
                                $J("#btnQuery").click();
                            }
                        }
                    },
                    "取消": function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }


        //退款查询使用
        function showdialogNew(t) {

            t = unescape(t);
            $J("#divOnline").html(t);
            $J("#divOnline").dialog({
                title: '温馨提示',
                bgiframe: true,
                height: 500,
                width: 800,
                modal: true,
                overlay: {
                    backgroundColor: '#red',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }

        var timeKey = 5;
        var setIntervalTime;

        function getTime() {
            timeKey--;
            if (timeKey == 0) {
                clearInterval(setIntervalTime);
                $J("#dd").dialog('close');
                document.getElementById("btnQuery").click();

            } else {
                $J("#showTimei").html(timeKey);
            }
        }

        function TitckOrderRefund() {
            $J("#dd").html("<h3 style='color:Red;'>提交退款成功,处理中......</h3><br /> <span style='color:Green'> 本页面将在 &nbsp; <span id='showTimei' style='color:red'>" + timeKey + " </span> &nbsp; 秒后自动刷新!</span>");
            $J("#dd").dialog({
                title: '标题',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#red',
                    opacity: 0.5
                }
            });
            setIntervalTime = setInterval("getTime()", 1000);
        }

        //-----------------------------------------------------------------------------
        //全选
        function SelAll(obj) {
            $J("input[name='ckItem'][type='checkbox']").attr("checked", obj.checked);
        }
        //是否验证通过
        function OrderValidate() {
            var isPass = false;
            var ckItems = $J("input[name='ckItem'][type='checkbox']:checked");
            if (ckItems.length == 0) {
                showdialog("请选择需要操作的订单");
            } else {
                var ids = [];
                for (var i = 0; i < ckItems.length; i++) {
                    if (ckItems[i].checked) {
                        ids.push(ckItems[i].value);
                    }
                }
                if (ids.length > 0) {
                    $J("#Hid_SelOrderId").val(ids.join("@@"));
                    isPass = true;
                }
                else {
                    $J("#Hid_SelOrderId").val("");
                }
            }
            return isPass;
        }

        function GetAuth(Office) {
            var strOffice = "RMK TJ AUTH " + Office;
            showdialog("授权指令:" + strOffice.toUpperCase());
            return false;
        }
        //动态创建div
        function DynCreateDiv(id) {
            var div = document.getElementById(id);
            if (div == null) {
                div = document.createElement("div");
                div.id = id;
                if (document.all) {
                    document.body.appendChild(div);
                }
                else {
                    document.insertBefore(div, document.body);
                }
            }
            return div;
        }
        function showHtmldiv(html, w, h) {
            DynCreateDiv("ddv");
            jQuery("#ddv").html(html);
            jQuery("#ddv").dialog({
                title: '业务协调',
                bgiframe: true,
                height: h,
                width: w,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    
                }
            });
            //防止出现按钮
            jQuery("#ddv").dialog("option", "buttons", {});
        }
        $J(function () {
            $J("a.ywtd").bind("click", function () {
                var orderid = $J(this).attr("metaorderid");
                var url = "OrderCoordinate.aspx?orderid=" + orderid + "&currentuserid=<%=this.mUser.id.ToString() %>";
                showHtmldiv("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='yes' allowtransparency='yes' width='100%' style='margin:0px;padding:0px;' height='100%' src='" + url + "'/>", 800, 400);
            })
        })
        function takeye(orderid) {
            var url = "OrderCoordinate.aspx?orderid=" + encodeURI(orderid) + "&currentuserid=<%=this.mUser.id.ToString() %>";
            showHtmldiv("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='yes' allowtransparency='yes' width='100%' style='margin:0px;padding:0px;' height='100%' src='" + url + "'/>", 750, 400);
        }
    </script>
    <script src="../js/js_GetUserInfo.js" type="text/javascript"></script>
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
    <div id="dd">
    </div>
    <div id="divOnline">
        <%--style="background-color: Black; color: Green;"--%>
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div style="top: 0px;">
        <div class="title">
            <span>订单综合查询</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="moreSearch" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                订单号/票号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtOrderId" CssClass="inputtxtdat" Style="width: 138px;" runat="server"></asp:TextBox>
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
                                    onfocus="WdatePicker({isShowClear:true})" />-<input id="txtFromDate2" type="text"
                                        readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
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
                                    onfocus="WdatePicker({isShowClear:true})" />-<input id="txtCreateTime2" type="text"
                                        readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                            </td>
                        </tr>
                        <tr id="More">
                            <th>
                                乘机人：
                            </th>
                            <td>
                                <asp:TextBox ID="txtPassengerName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                客户名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCorporationName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                订单状态：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlStatus" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
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
                                <uc1:SelectAirCode ID="SelectAirCode1" runat="server" IsDShowName="false" DefaultOptionValue=""
                                    DefaultOptionText="" />
                            </td>
                            <th runat="server" id="thCpyName">
                                出票公司：
                            </th>
                            <td runat="server" id="tdCpyName">
                                <uc1:SelectAirCode ID="ddlGYList" TxtWidth="100" ddlWidth="250" InputMaxLength="300"
                                    IsDShowName="false" runat="server" DefaultOptionValue="" DefaultOptionText="--------选择落地运营商--------" />
                            </td>
                        </tr>
                        <tr style="display: none;">
                            <th style="display: none;">
                                行程类型：
                            </th>
                            <td colspan="5">
                                <asp:RadioButtonList ID="rbtlTravelType" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                    <asp:ListItem Selected="True" Value="0" Text="全部"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="单程"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="往返"></asp:ListItem>
                                    <asp:ListItem Value="4" Text="联程"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" style="text-align: center;">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" /></span>
                                <span class="btn btn-ok-s" runat="server" id="spanUnLock" visible="false">
                                    <asp:Button ID="btnUnLock" runat="server" Text="订单锁定/解锁" OnClientClick="if(confirm('是否订单锁定/解锁?')){return OrderValidate()}else{return false;}"
                                        OnClick="btnUnLock_Click" />
                                        
                                        </span> <span class="btn btn-ok-s" runat="server" id="spanTKZ"
                                            visible="false">
                                            <asp:Button ID="btnTKZ" runat="server" Text="退款中的订单" 
                                    onclick="btnTKZ_Click"  /></span>
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
                            <input id="ckAll" type="checkbox" onclick="SelAll(this)" />
                        </th>
                        <th>
                            操作
                        </th>
                        <% if (IsShow2())
                           {%>
                        <th>
                            客户名称
                        </th>
                        <th class='<%= ShowText(8,"") %>'>
                            出票公司
                        </th>
                        <%} %>
                        <th>
                            订单编号
                        </th>
                        <th>
                            PNR
                        </th>
                        <% if (IsShow())
                           {%>
                        <th runat="server">
                            政策来源
                        </th>
                        <%}
                           else { } %>
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
                            订单状态
                        </th>
                        <th>
                            支付方式
                        </th>
                        <th>
                            锁定帐号
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repOrderList" runat="server" OnItemCommand="repOrderList_ItemCommand"
                    OnItemDataBound="repOrderList_ItemDataBound">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';" id='OrderTr_ <%# Eval("id")%>'>
                            <td style="width: 5%; text-align: center;">
                                <input id='ckItem<%# Eval("id")%>' type="checkbox" name="ckItem" value='<%# Eval("id")+"##"+  Eval("LockLoginName")%>' />
                            </td>
                            <td class="Operation" style="line-height: 30px;">
                                <asp:LinkButton runat="server" ID="lbtnPaySel" Text="支付状态查询" Visible="false" CommandName="PaySel"
                                    CommandArgument='<%#Eval("OrderId")%>'></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnRefundSel" Text="退款查询" Visible="false" CommandName="RefundSel"
                                    CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnRefund" Text="退款" Visible="false" CommandName="Refund"
                                    OnClientClick="return confirm('确定退款?请仔细确认是否退款成功？');" CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnUpdateOrder" Text="修改订单" Visible="false" CommandName="UpdateOrder"
                                    OnClientClick="return confirm('确定修改?');" CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnUnLock" Text="解锁" Visible="false" CommandArgument='<%# Eval("OrderId") %>'
                                    CommandName="unLock" OnClientClick="return confirm('确定解锁吗?')"></asp:LinkButton>
                                <%-- <asp:LinkButton runat="server" ID="lbtnDetail" Text="订单详情" CommandName="Detail" CommandArgument='<%#Eval("id")%>'></asp:LinkButton>--%>
                                <a href="OrderDetail.aspx?Id=<%# Eval("id") %>&Url=OrderList.aspx&currentuserid=<%=mUser.id%>">
                                    订单详情</a>
                                <asp:HiddenField ID="Hid_OrderStatusCode" runat="server" Value='<%# Eval("OrderStatusCode")%>' />
                                <asp:HiddenField ID="Hid_PayWay" runat="server" Value='<%# Eval("PayWay")%>' />
                                <asp:HiddenField ID="Hid_Lock" runat="server" Value='<%# Eval("LockLoginName") %>' />
                                <%# ShowText(11, Eval("PolicySource"), Eval("PrintOffice"), Eval("OrderSourceType"))%>
                                 <% if (IsShow3())
                               {%>
                               <br />
                                <a href="#" class="ywtd" metaorderid="<%# Eval("OrderID") %>">业务协调</a>
                                <%} %>
                            </td>
                            <% if (IsShow2())
                               {%>
                            <td>
                                <%# IsSelfCustomer(Eval("CPCpyNo").ToString(), Eval("OrderId").ToString(), Eval("OwnerCpyName").ToString(), Eval("PolicySource").ToString())%>
                            </td>
                            <%} %>
                            <td class='<%= ShowText(8,"") %>'>
                                <%# Eval("NewCpCpyName")%>
                            </td>
                            <td>
                                <%# ShowText(7, Eval("OrderId"), Eval("OutOrderId"), Eval("IsChdFlag"), Eval("IsCHDETAdultTK"), Eval("OrderSourceType"))%>
                                <%# ShowAdult(Eval("AssociationOrder"), Eval("IsChdFlag"))%>
                            </td>
                            <td>
                                <%# Eval("PNR")%>
                                <br />
                                <input type="button" onclick="copyToClipboard('<%# Eval("PNR")%>')" class="hide"
                                    value="复制" />
                                <br />
                                <%# ZFZ(Eval("A8").ToString()) %>
                            </td>
                            <% if (IsShow())
                               {%>
                            <td>
                                <%# ShowText(9, Eval("PolicySource"), Eval("CPCpyNo"), Eval("PolicyType"), Eval("AutoPrintFlag"), Eval("PolicyId"))%>
                            </td>
                            <%}
                               else { } %>
                            <td>
                                <%--    <%# Eval("CreateTime")%>--%>
                                <%# ShowText(6, Eval("CreateTime"))%>
                            </td>
                            <td>
                                <%# ShowText(1,Eval("PassengerName"))%>
                            </td>
                            <td <%# ShowText(5,Eval("AirTime"))%>>
                                <%# ShowText(2,Eval("AirTime"))%>
                            </td>
                            <td>
                                <%#ShowText(4, Eval("Travel"))%>
                            </td>
                            <td>
                                <%# FlyCode(Eval("CarryCode").ToString(),Eval("FlightCode").ToString())%>
                            </td>
                            <td>
                                <%# GetPrice(Eval("PayMoney"), Eval("OrderMoney"), Eval("TicketStatus"))%>
                            </td>
                            <td>
                                <%# ShowText(10,1, Eval("OrderStatusCode"))%>
                            </td>
                            <td>
                                <%# ShowText(10,4, Eval("PayWay"))%>
                            </td>
                            <td>
                                <%# Eval("LockLoginName")%>
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
    <script type="text/javascript" language="javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd = ""; //  "?r=" + Math.random();
            var SE = new CtripJsLoader();
            var files = [["../js/CitySelect/tuna_100324.js" + rd, "GB2312", true, null], ["../AJAX/GetCity.aspx" + rd, "GB2312", true, null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        $J(function () {
            $J('#tabs').tabs();
            ReLoad();
        });
    </script>
    <%--选择的订单id--%>
    <input id="Hid_SelOrderId" type="hidden" runat="server" />
    </form>
</body>
</html>
