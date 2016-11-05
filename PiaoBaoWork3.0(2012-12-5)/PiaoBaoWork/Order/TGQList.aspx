<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TGQList.aspx.cs" Inherits="Order_TGQList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>退改签申请</title>
    <script src="../JS/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link type="text/css" href="../CSS/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../CSS/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../JS/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../JS/jquery-ui-1.8.13.custom.min.js"></script>
    <script id="script_k0" src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
        .sec1
        {
            background: url(../img/all_pic.gif);
            background-position: -262px -36px;
            text-align: center;
            cursor: hand;
            color: #000;
        }
        .sec2
        {
            background: url(../img/all_pic.gif);
            background-position: -79px -36px;
            text-align: center;
            cursor: hand;
            color: #fff;
        }
        .nav_tgq
        {
            width: 98%;
            margin: auto;
            text-align: left;
            margin-top: 5px;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .inputBorder
        {
            border: 1px solid #999;
        }
    </style>
    <script type="text/javascript">

                <!--
        //为Jquery重新命名
        var $J = jQuery = jQuery.noConflict(false);
        //-->

        function showdialog(t) {
            $J("#showOne").html(t);
            $J("#showOne").dialog({
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

        function moreSearchOrder() {
            if ($J("#moreSearchOrder").css("display") == "none") {
                $J("#moreSearchOrderA").attr("class", "MoreCondition");
            }
            else {
                $J("#moreSearchOrderA").attr("class", "MoreConditionA");
            }
            $J("#moreSearchOrder").toggle('slow');
        }

        function btnOk(num) {
            document.getElementById("Hid_num").value = num;
            document.getElementById("ddlStatus").value = num;
            SetBtnClass();
            document.getElementById("btnQuery").click();
        }
        function SetBtnClass() {
            var num = document.getElementById("Hid_num").value;
            var tab = document.getElementById("secTable");
            for (var i = 0; i < tab.rows[0].cells.length; i++) {
                if (i == (num - 1)) {
                    tab.rows[0].cells[i].className = "sec2";
                } else {
                    tab.rows[0].cells[i].className = "sec1";
                }
            }
        }

        function ddlSel(obj) {
            document.getElementById("Hid_num").value = obj.value;
            SetBtnClass();
        }

        //ie
        copyValue = function (strValue) {
            if (isIE()) {
                clipboardData.setData("Text", strValue);
            }
            else {
                copy(strValue);
            }
        }
        function isIE(number) {
            if (typeof (number) != number) {
                return !!document.all;
            }
        }
        function copy(text2copy) {
            var flashcopier = 'flashcopier';
            if (!document.getElementById(flashcopier)) {
                var divholder = document.createElement('div');
                divholder.id = flashcopier;
                document.body.appendChild(divholder);
            }
        }

    </script>
    <script type="text/javascript" src="../js/js_Copy.js"></script>
</head>
<body onload="SetBtnClass()">
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
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="退改签申请" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table cellspacing="3" cellpadding="0" border="0" class="Search">
                    <tr>
                        <th>
                            订单编号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtOrderId" Width="138px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            PNR：
                        </th>
                        <td>
                            <asp:TextBox ID="txtPNR" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            乘机日期：
                        </th>
                        <td colspan="2">
                            <input id="txtFromDate1" type="text" readonly="readonly" runat="server" style="width: 110px;"
                                class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />-
                            <input id="txtFromDate2" type="text" readonly="readonly" runat="server" style="width: 110px;"
                                class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
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
                        <td colspan="2">
                            <input id="txtCreateTime1" type="text" readonly="readonly" runat="server" style="width: 110px;"
                                class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />-
                            <input id="txtCreateTime2" type="text" readonly="readonly" runat="server" style="width: 110px;"
                                class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            乘机人：
                        </th>
                        <td>
                            <asp:TextBox ID="txtPassengerName" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            订单状态：
                        </th>
                        <td>
                            <asp:DropDownList runat="server" Width="115px" ID="ddlStatus" onchange="ddlSel(this)">
                                <asp:ListItem Text="已出票交易结束" Value="1"></asp:ListItem>
                                <asp:ListItem Text="进行中的退票" Value="2"></asp:ListItem>
                                <asp:ListItem Text="进行中的废票" Value="3"></asp:ListItem>
                                <asp:ListItem Text="进行中的改签" Value="4"></asp:ListItem>
                                <asp:ListItem Text="审核通过" Value="5"></asp:ListItem>
                                <asp:ListItem Text="拒绝申请的订单" Value="6"></asp:ListItem>
                                <asp:ListItem Text="交易结束" Value="7"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <th>
                            航班号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtFlightCode" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" align="center" style=" padding:5px 0 5px 360px">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" /></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="nav_tgq">
            <table id="secTable" style="border-collapse: collapse; z-index: 1; position: relative;"
                border="0" cellspacing="0" cellpadding="0">
                <tr align="left">
                    <td class="sec2" width="97px" id="orderstatus" runat="server">
                        <asp:LinkButton ID="btn1" OnClientClick="btnOk(1);return false;" runat="server" Height="27px"
                            Style="line-height: 24px; text-decoration: none; color: #666;">已出票交易结束</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px">
                        <asp:LinkButton ID="btn2" OnClientClick="btnOk(2);return false;" runat="server" Height="27px"
                            Style="line-height: 24px; text-decoration: none; color: #666;">进行中的退票</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px">
                        <asp:LinkButton ID="btn3" OnClientClick="btnOk(3);return false;" runat="server" Height="27px"
                            Style="line-height: 24px; text-decoration: none; color: #666;">进行中的废票</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px">
                        <asp:LinkButton ID="btn4" OnClientClick="btnOk(4);return false;" runat="server" Height="27px"
                            Style="line-height: 24px; text-decoration: none; color: #666;">进行中的改签</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px">
                        <asp:LinkButton ID="btn5" OnClientClick="btnOk(5);return false;" runat="server" Height="27px"
                            Style="line-height: 24px; text-decoration: none; color: #666;">审核通过</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px">
                        <asp:LinkButton ID="btn6" OnClientClick="btnOk(6);return false;" runat="server" Height="27px"
                            Style="line-height: 24px; text-decoration: none; color: #666;">拒绝申请订单</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px">
                        <asp:LinkButton ID="btn7" OnClientClick="btnOk(7);return false;" runat="server" Height="27px"
                            Style="line-height: 24px; text-decoration: none; color: #666;">交易结束</asp:LinkButton>
                    </td>
                </tr>
            </table>
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
                        起飞时间
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
            <asp:Repeater ID="repList" runat="server" OnItemCommand="repList_ItemCommand" OnItemDataBound="repList_ItemDataBound">
                <ItemTemplate>
                    <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                        <td style="width: 7%;">
                            <asp:Label ID="lblOrderId" runat="server" Text='<%#Eval("OrderId") %>'></asp:Label>
                        </td>
                        <td class="pnr" style="width: 8%;">
                            <%# Eval("PNR")%>
                             <br /><input type="button"  onclick="copyToClipboard('<%# Eval("PNR")%>')" class="hide" value="复制" />
                        </td>
                        <td style="width: 7%;">
                            <%#Eval("CreateTime")%>
                        </td>
                        <td style="width: 7%;">
                            <%--  <%# Eval("PassengerName")%>--%>
                            <%# DataSourceMessage(23, Eval("PassengerName"))%>
                        </td>
                        <td style="width: 7%;">
                            <%# Eval("AirTime")%>
                        </td>
                        <td style="width: 10%;">
                            <%# Eval("Travel")%>
                        </td>
                        <td style="width: 7%;">
                            <%# Eval("CarryCode").ToString()+"/"+Eval("FlightCode")%>
                        </td>
                        <td class="Price" style="width: 7%; color: Red;">
                            <%# Eval("payMoney")%>
                        </td>
                        <%--  <td style="width: 7%;">--%>
                        <td style="color: <%# DataSourceColor(Eval("OrderStatusCode").ToString(),2)%>; width: 7%;">
                            <%# DataSourceMessage(1,Eval("OrderStatusCode").ToString())%>
                        </td>
                        <td style="width: 6%;">
                            <%# DataSourceMessage(3, Eval("PayStatus").ToString())%>
                        </td>
                        <td style="width: 6%;">
                        </td>
                        <td class="Operation" style="width: 10%;">
                            <asp:LinkButton runat="server" ID="lbtnProcess" Text="退改签申请" CommandName="Process"
                                CommandArgument='<%#Eval("id")%>' Visible="false"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lbtnPay" Text="改签订单支付" CommandName="Pay" CommandArgument='<%#Eval("id")%>'
                                Visible="false"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lbtnPayNo" Text="拒绝补差" CommandName="PayNo" CommandArgument='<%#Eval("id")%>'
                                Visible="false" OnClientClick="if(confirm('是否拒绝补差?')){ return true;}else{return false;}"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lbtnDetail" Text="订单详情" CommandName="Detail" CommandArgument='<%#Eval("id")%>'> 
                            </asp:LinkButton>
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
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
        <div id="Show" style="width: 100%; text-align: center; font-size: 12px; line-height: 20px;
            color: Gray; margin-top: 0;" runat="server" visible="false">
            没有符合你要搜索的条件,请输入正确的查询条件！
        </div>
        <input type="hidden" runat="server" id="hiShow" value="0" />
        <input type="hidden" runat="server" id="Hid_num" value="1" />
        <div id="showOne">
        </div>
    </div>
    <script type="text/javascript" language="javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd = ""; //  "?r=" + Math.random();
            var SE = new CtripJsLoader();
            var files = [["../JS/CitySelect/tuna_100324.js" + rd, "GB2312", true, null], ["../AJAX/GetCity.aspx" + rd, "GB2312", true, null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        $J(ReLoad);  
    </script>
    </form>
</body>
</html>
