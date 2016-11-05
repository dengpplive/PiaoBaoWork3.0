<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LineOrderProcess.aspx.cs"
    Inherits="Order_LineOrderProcess" %>

<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>线下订单处理</title>
    <link type="text/css" href="../CSS/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../CSS/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../JS/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../JS/jquery-ui-1.8.13.custom.min.js"></script>
    <script id="script_k0" src="../js/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
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
            color: green;
        }
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
        }
        .flighttab tr
        {
            height: 30px;
            line-height: 30px;
        }
        .TextBorder
        {
            border: 1px solid #999;
        }
        .ulClass li
        {
            padding: 2px;
        }
        .tab_hj th
        {
            padding: 2px 3px 2px 2px;
        }
        .tab_hj
        {
            background: none;
        }
    </style>
    <script type="text/javascript">
        var $J=jQuery.noConflict(false);
    </script>
    <script type="text/javascript" src="../js/js_LineOrderProcess.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
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
    <div id="show">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div>
        <div class="title">
            <asp:Label ID="lblShow" Text="线下订单处理" runat="server" />
        </div>
        <br />
        <div class="c-list-filter">
            <div class="container">
                <table cellspacing="0" id="tb_group" cellpadding="0" border="0" class="Search">
                    <tr>
                        <th>
                            订单号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtOrderId" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            出发城市：
                        </th>
                        <td>
                            <%--  <uc1:SelectAirCode ID="ddlFromCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--出发城市--"
                                ChangeFunctionName="GetFromCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />--%>
                            <input name="ddlFromCity" class="inputtxtdat" type="text" id="ddlFromCity" runat="server"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 110px;" mod_address_reference="Hid_fromCode" />
                        </td>
                        <th>
                            订单状态：
                        </th>
                        <td>
                            <asp:DropDownList ID="ddlStatus" runat="server" Width="117px">
                                <asp:ListItem Text="全部状态" Value="0"></asp:ListItem>
                            </asp:DropDownList>
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
                            乘机人：
                        </th>
                        <td>
                            <asp:TextBox ID="txtPassengerName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            到达城市：
                        </th>
                        <td>
                            <%-- <uc1:SelectAirCode ID="ddlToCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--到达城市--"
                                ChangeFunctionName="GetToCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />--%>
                            <input name="ddlToCity" class="inputtxtdat" type="text" id="ddlToCity" runat="server"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 110px;" mod_address_reference="Hid_toCode" />
                        </td>
                        <th>
                            <span runat="server" id="div_GY">运营商：</span>
                        </th>
                        <td>
                            <select name="ddlGY" id="ddlGY" onchange="SetHid(this)" runat="server" style="width: 118px;">
                            </select>
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
                    <tr>
                        <td colspan="6" style="text-align: center;">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" OnClientClick="return validate();" /></span>
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClientClick="return ClearData();" /></span>
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
                        客户名称
                    </th>
                    <th>
                        订单号
                    </th>
                    <th>
                        乘机人
                    </th>
                    <th>
                        创建时间
                    </th>
                    <th>
                        起飞时间
                    </th>
                    <th>
                        行程
                    </th>
                    <th>
                        政策
                    </th>
                    <th>
                        申请状态
                    </th>
                    <th>
                        锁定账号
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="RepOrderList" runat="server">
                <ItemTemplate>
                    <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                        <td class="pnr" style="width: 6%;">
                            <a href="#" onclick="return GetUserInfo('<%# Eval("OrderId") %>')">
                                <%# Eval("OwnerCpyName")%></a>
                        </td>
                        <td style="width: 7%;">
                            <%#Eval("OrderId")%>
                        </td>
                        <td style="width: 7%;">
                            <%# ShowText(1, Eval("PassengerName"))%>
                        </td>
                        
                        <td style="width: 7%;">
                            <%# ShowText(0, Eval("CreateTime"))%>
                        </td>
                        <td style="width: 7%;">
                            <%# ShowText(2, Eval("AirTime"))%>
                        </td>
                        <td style="width: 7%;">
                            <%# ShowText(5, Eval("Travel"))%>
                        </td>
                        <td style="width: 7%;">
                            <%#  Eval("ReturnPoint")%>
                        </td>
                        <td style="width: 7%;">
                            <%# ShowText(4, Eval("OrderStatusCode"))%>
                        </td>
                        <td style="width: 7%;">
                            <%# Eval("LockLoginName")%>
                        </td>
                        <td style="width: 5%;" align="center">
                            <div id='showOP_<%# Eval("id") %>'>
                                <div class='<%# ShowText(6, Eval("OrderStatusCode"))%>'>
                                    <a id='a_shenghe_<%# Eval("id") %>' href="#" onclick="return ShowDiv('<%# Eval("id") %>') ">
                                        审核订单</a>
                                    <br />
                                    <a id='a_jujue_<%# Eval("id") %>' href="#" onclick="return Update('<%# Eval("id") %>',0)">
                                        拒绝订单</a>
                                    <br />
                                </div>
                                <a href='OrderDetail.aspx?id=<%# Eval("id") %>&Url=LineOrderProcess.aspx&currentuserid=<%=this.mUser.id.ToString() %>'>
                                    订单详情</a><br />
                            </div>
                            <div id='processDiv_<%# Eval("id") %>' class="hide" style="background: none">
                                <ul class="ulClass">
                                    <li>&nbsp;&nbsp;&nbsp;<label for='ckIsHandInput_<%# Eval("id") %>'>
                                        <input type="checkbox" id='ckIsHandInput_<%# Eval("id") %>' />&nbsp;&nbsp;手动录价</label><br />
                                    </li>
                                    <li class='<%# ShowHand(Eval("IsChdFlag").ToString(),Eval("HaveBabyFlag").ToString())%>'>
                                        <label for='ck_NotAdult_<%# Eval("id") %>'>
                                            <input type="checkbox" id='ck_NotAdult_<%# Eval("id") %>'>不算成人</label><br />
                                    </li>
                                    <li>
                                        <%--出票公司编号--%>
                                        <input type="hidden" id='CPCpyNo_<%# Eval("id") %>' value='<%# Eval("CPCpyNo") %>' />
                                        <table id='tab_pnr_<%# Eval("id") %>' border="0" align="center" class="tab_hj" cellpadding="2"
                                            cellspacing="2" style="width: 96%;">
                                            <tr>
                                                <th align="right">
                                                    PNR:
                                                </th>
                                                <th align="left">
                                                    <input type="text" id='txtPnr_<%# Eval("id") %>' size="5" maxlength="6" style="width: 55px" />
                                                </th>
                                            </tr>
                                            <tr>
                                                <th align="right">
                                                    Office:
                                                </th>
                                                <th align="left">
                                                    <input type="text" id='txtOffice_<%# Eval("id") %>' size="5" value="<%# Eval("PrintOffice") %>"
                                                        maxlength="6" style="width: 55px" />
                                                </th>
                                            </tr>
                                            <tr>
                                                <th align="right">
                                                    政策返点:
                                                </th>
                                                <th align="left">
                                                    <input type="text" id='txtPolicy_<%# Eval("id") %>' value="0" size="5" style="width: 55px" />
                                                </th>
                                            </tr>
                                            <tr name="price" class="hide">
                                                <th align="right">
                                                    舱位价:
                                                </th>
                                                <th align="left">
                                                    <input type="text" id='txtSeatPrice_<%# Eval("id") %>' value="0" size="8" style="width: 55px" />
                                                </th>
                                            </tr>
                                            <tr name="price" class="hide">
                                                <th align="right">
                                                    机建费:
                                                </th>
                                                <th align="left">
                                                    <input type="text" id='txtJJPrice_<%# Eval("id") %>' value="0" size="8" style="width: 55px" />
                                                </th>
                                            </tr>
                                            <tr name="price" class="hide">
                                                <th align="right">
                                                    燃油费:
                                                </th>
                                                <th align="left">
                                                    <input type="text" id='txtRQPrice_<%# Eval("id") %>' value="0" size="8" style="width: 55px" />
                                                </th>
                                            </tr>
                                            <tr>
                                                <th align="right">
                                                    政策类型:
                                                </th>
                                                <th align="left">
                                                    <select id='selPolicyType_<%# Eval("id") %>' style="width: 58px;">
                                                        <option value="1">B2B</option>
                                                        <option value="2">BSP</option>
                                                        <%--  <option value="3">B2B/BSP</option>--%>
                                                    </select>
                                                </th>
                                            </tr>
                                        </table>
                                    </li>
                                    <li><a href="#" id='btnCommit_<%# Eval("id")  %>'>提交</a>&nbsp;<a href="#" id='btnCancel_<%# Eval("id") %>'>取消</a>
                                    </li>
                                </ul>
                                <input type="hidden" id='hid_OrderId_<%# Eval("id")  %>' value='<%# Eval("OrderId")  %>' />
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <input type="hidden" id="hid_PageSize" value="20" runat="server" />
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    <%--出发城市三字码 --%>
    <input type="hidden" id="Hid_fromCode" runat="server" />
    <%--到达城市三字码 --%>
    <input type="hidden" id="Hid_toCode" runat="server" />
    <%--出发城市 --%>
    <input type="hidden" id="Hid_FromCity" runat="server" />
    <%--到达城市 --%>
    <input type="hidden" id="Hid_ToCity" runat="server" />
    <%--登录账号--%>
    <input type="hidden" id="Hid_LoginName" runat="server" />
    <%--角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" />
    <script language="javascript" type="text/javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../js/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        $J(function () {
            ReLoad();
        });
    </script>
    </form>
</body>
</html>
