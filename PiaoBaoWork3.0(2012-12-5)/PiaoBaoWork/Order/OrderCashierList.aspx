<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderCashierList.aspx.cs"
    Inherits="Order_OrderCashierList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>待收银订单</title>
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
    </style>
    <script type="text/javascript">
        var $J=jQuery.noConflict(false);
        function showdialog(t,param) {
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
                        if(param!=null) {
                            if(param.type=="0") {
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

        //-----------------------------------------------------------------------------
        //全选
        function SelAll(obj) {
            $J("input[name='ckItem'][type='checkbox']").attr("checked",obj.checked);
        }
        //是否验证通过
        function OrderValidate() {
            var isPass=false;
            var ckItems=$J("input[name='ckItem'][type='checkbox']:checked");
            if(ckItems.length==0) {
                showdialog("请选择需要操作的订单");
            } else {
                var ids=[];
                for(var i=0;i<ckItems.length;i++) {
                    if(ckItems[i].checked) {
                        ids.push(ckItems[i].value);
                    }
                }
                if(ids.length>0) {
                    $J("#Hid_SelOrderId").val(ids.join("@@"));
                    isPass=true;
                }
                else {
                    $J("#Hid_SelOrderId").val("");
                }
            }
            return isPass;
        }
        //订单提醒跳转 查询订单
        function RequestQuery(isQuery) {
            if(isQuery=="1") {
                document.getElementById("btnQuery").click();
            }
        }
    </script>
    <script src="../js/js_GetUserInfo.js" type="text/javascript"></script>
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
    <form id="form2" runat="server">
    <asp:HiddenField ID="currentuserid" ClientIDMode="Static" runat="server" />
    <div style="top: 0px;">
        <div class="title">
            <span>待收银订单</span>
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
                                航班号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtFlightCode" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                客户名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCorporationName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                航空公司：
                            </th>
                            <td colspan="5">
                                <uc1:SelectAirCode ID="SelectAirCode1" runat="server" IsDShowName="false" DefaultOptionValue="" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" style="text-align: center;">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" /></span>
                                <span class="btn btn-ok-s" runat="server" id="spanUnLock" visible="false">
                                    <asp:Button ID="btnUnLock" runat="server" Text="订单收银" OnClientClick="return OrderValidate()"
                                        OnClick="btnUnLock_Click" /></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                width="100%">
                <thead>
                    <tr>
                        <%--<th>
                            <input id="ckAll" type="checkbox" onclick="SelAll(this)" />
                        </th>--%>
                        <th>
                            客户名称
                        </th>
                        <th>
                            订单编号
                        </th>
                        <th>
                            PNR
                        </th>
                        <th>
                            政策来源
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
                            订单状态
                        </th>
                        <th>
                            支付状态
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
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';" id='OrderTr_ <%# Eval("id")%>'>
                            <%-- <td style="width: 5%; text-align: center;">
                                <input id='ckItem<%# Eval("id")%>' type="checkbox" name="ckItem" value='<%# Eval("id")+"##"+  Eval("OrderId")%>' />
                            </td>--%>
                            <td>
                                <a href="#" onclick="return GetUserInfo('<%# Eval("OrderId") %>')">
                                    <%# Eval("OwnerCpyName")%></a>
                            </td>
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("PNR")%>
                            </td>
                            <td>
                                <%# GetDicName(24, Eval("PolicySource").ToString())%>
                            </td>
                            <td>
                                <%# Eval("CreateTime")%>
                            </td>
                            <td>
                                <%# ShowText(1,Eval("PassengerName"))%>
                            </td>
                            <td>
                                <%# ShowText(2,Eval("AirTime"))%>
                            </td>
                            <td>
                                <%#ShowText(4, Eval("Travel"))%>
                            </td>
                            <td>
                                <%# ShowText(4,Eval("FlightCode"))%>
                            </td>
                            <td>
                                <%# Eval("OrderMoney")%>
                            </td>
                            <td>
                                <%# GetStrValue(1, Eval("OrderStatusCode").ToString())%>
                            </td>
                            <td>
                                <%# ShowText(5, Eval("PayStatus"))%>
                            </td>
                            <td>
                                <%# Eval("LockLoginName")%>
                            </td>
                            <td class="Operation" style="line-height: 30px;">
                                <asp:LinkButton runat="server" ID="lbtnCashier" Text="确定收银" CommandName="Cashier"
                                    CommandArgument='<%#Eval("id")%>' OnClientClick="return confirm('是否确定收银？');"></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnDetail" Text="订单详情" CommandName="Detail" CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                                <asp:HiddenField ID="Hid_OrderStatusCode" runat="server" Value='<%# Eval("OrderStatusCode")%>' />
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
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../js/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
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
