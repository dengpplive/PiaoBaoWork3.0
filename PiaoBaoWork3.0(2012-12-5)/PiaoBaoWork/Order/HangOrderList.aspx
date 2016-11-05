<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HangOrderList.aspx.cs" Inherits="Air_Order_HangOrderList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>订单挂起/解挂</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" language="javascript">
    <!--
        //为Jquery重新命名
        var $J=jQuery=jQuery.noConflict(false);
    //-->
    </script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script id="script_k0" src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <script src="../js/js_GetUserInfo.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showdialog(t,param) {
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
                        $J(this).dialog('close');
                        if(param!=null) {
                            if(param.op=="1") {
                                history.go(-1);
                            }
                        }
                    }
                }
            });
        }
        
    </script>
     <script type="text/javascript" src="../js/js_Copy.js"></script>
    <style type="text/css">
        .title
        {
            font-size: 14px;
            font-weight: bolder;
            height: 100%;
            line-height: 20px;
            padding: 3px 0;
            color: #0092FF;
            text-align: left;
            background-color: #EFF4F8;
            border: 1px solid #D4D4D4;
        }
        .title span
        {
            margin-left: 15px;
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
        .green
        {
            color: Green;
        }
        .red
        {
            color: red;
        }
        .tb-all-trade td font
        {
            display: block;
            width: 24px;
            float: right;
            margin: 0 6px 0 0;
        }
    </style>
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
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="订单挂起/解挂" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table id="moreSearch" cellspacing="0" cellpadding="0" border="0">
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
                            <asp:TextBox ID="txtPNR" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            航空公司：
                        </th>
                        <td>
                            <uc1:SelectAirCode ID="SelectAirCode1" IsDShowName="false" DefaultOptionValue=""
                                runat="server" />
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
                                style="width: 110px;" mod_address_reference="Scity02" />
                            <input type="hidden" id="Scity02" />
                        </td>
                        <th>
                            乘机日期：
                        </th>
                        <td colspan="3">
                            <input id="txtFromDate1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                onfocus="WdatePicker({isShowClear:true})" />-<input id="txtFromDate2" type="text"
                                    readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            到达城市：
                        </th>
                        <td>
                            <input name="txtToCity" class="inputtxtdat" type="text" id="txtToCity" runat="server"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 110px;" mod_address_reference="Hidden1" />
                            <input type="hidden" name="Hidden1" id="Hidden1" />
                        </td>
                        <th>
                            创建日期：
                        </th>
                        <td colspan="3">
                            <input id="txtCreateTime1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                onfocus="WdatePicker({isShowClear:true})" />-<input id="txtCreateTime2" type="text"
                                    readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
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
                        <td colspan="3">
                            <asp:TextBox ID="txtFlightCode" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th class="hide">
                            行程类型：
                        </th>
                        <td class="hide">
                            <asp:RadioButtonList ID="rbtlTravelType" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Selected="True" Value="0" Text="全部"></asp:ListItem>
                                <asp:ListItem Value="1" Text="单程"></asp:ListItem>
                                <asp:ListItem Value="3" Text="往返"></asp:ListItem>
                                <asp:ListItem Value="4" Text="联程"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <th>
                        </th>
                        <td>
                        </td>
                        <td style="text-align: center;" colspan="2">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" />
                            </span>&nbsp;&nbsp;<span class="btn btn-ok-s">
                                <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" />
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="0" cellpadding="0"
            border="0">
            <thead>
                <tr>
                    <th>
                        客户名称
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
                        实收金额
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repList" runat="server">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <td style="width: 10%;">
                            <a href="#" onclick="return GetUserInfo('<%# Eval("OrderId") %>')">
                                <%# Eval("OwnerCpyName")%></a>
                        </td>
                        <td style="width: 6%;">
                            <%# Eval("PNR")%>
                             <br /><input type="button"  onclick="copyToClipboard('<%# Eval("PNR")%>')" class="hide" value="复制" />
                        </td>
                        <td style="width: 8%;">
                            <%# Eval("CreateTime")%>
                        </td>
                        <td style="width: 8%;">
                            <%# ShowText(1, Eval("SuppendStatus"))%>
                        </td>
                        <td style="width: 8%;">
                            <%# ShowText(2,Eval("AirTime"))%>
                        </td>
                        <td style="width: 10%;">
                            <%#ShowText(4, Eval("Travel"))%>
                        </td>
                        <td style="width: 7%;">
                            <%# ShowText(4,Eval("FlightCode"))%>
                        </td>
                        <td class="Price" style="width: 7%; color: Red;">
                            <%# Eval("PayMoney")%>
                        </td>
                        <td class="Operation" style="width: 8%;">
                            <a href="HangProcess.aspx?Id=<%# Eval("id") %>&Url=HangOrderList.aspx&Type=0&currentuserid=<%=this.mUser.id.ToString() %>">挂起</a><br />
                            <a href="HangProcess.aspx?Id=<%# Eval("id") %>&Url=HangOrderList.aspx&Type=1&currentuserid=<%=this.mUser.id.ToString() %>">解挂</a><br />
                            <a href="OrderDetail.aspx?Id=<%# Eval("id") %>&Url=HangOrderList.aspx&currentuserid=<%=this.mUser.id.ToString() %>">订单详情</a><br />
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
    </div>
    <div id="Show" style="width: 100%; text-align: center; font-size: 12px; line-height: 20px;
        color: Gray; margin-top: 0;" runat="server" visible="false">
        没有符合你要搜索的条件,请输入正确的查询条件！
    </div>
    <input type="hidden" runat="server" id="hiShow" value="0" />
    <div id="showOne">
    </div>
    <div id="showDiv" style="display: none; z-index: 9999; position: absolute;">
    </div>
    </form>
    <script type="text/javascript" language="javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../JS/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        $J(ReLoad);  
    </script>
</body>
</html>
