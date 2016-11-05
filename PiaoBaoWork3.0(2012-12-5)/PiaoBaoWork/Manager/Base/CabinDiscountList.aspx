<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CabinDiscountList.aspx.cs"
    Inherits="Sys_CabinDiscountList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>舱位管理</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script id="script_k0" src="../../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
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
        .show
        {
            display: block;
        }
        .hide
        {
            display: none;
        }
    </style>
    <script type="text/javascript">
        var $J=jQuery.noConflict(false);
        function showdialog(t) {
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
                    },
                    "取消": function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }
        $J(document).ready(function () {
            $J('#tabs').tabs();
            var zz1=/^\d*$/
            $J("#txtDiscountRate").keyup(function () {
                if(!zz1.test($J("#txtDiscountRate").val())) {
                    $J("#txtDiscountRate").val("");
                }
            })
        })
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../../JS/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        $J(function () {
            ReLoad();
        });        
    </script>
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
    <div id="dd">
    </div>
    <div id="tabs">
        <div class="title">
            <span>舱位管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;">
                        <tr>
                            <th>
                                出发城市：
                            </th>
                            <td>
                                <%-- 出发城市--%>
                                <input name="txtFromCityName" class="inputtxtdat" runat="server" type="text" id="txtFromCityName"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 150px;" mod_address_reference="FromCityCode" />
                                <input id="FromCityCode" name="FromCityCode" type="hidden" runat="server" />
                            </td>
                            <th>
                                国际/国内：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlIsGN" runat="server" Width="145px">
                                    <asp:ListItem Selected="True" Text="不限" Value="wx"></asp:ListItem>
                                    <asp:ListItem Text="国内" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="国际" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <th>
                                承运人代码：
                            </th>
                            <td>
                                <uc1:SelectAirCode ID="txtAirPortCode" DefaultOptionValue="" IsDShowName="false"
                                    runat="server" style="width: 150px;" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                到达城市：
                            </th>
                            <td>
                                <%-- 到达城市--%>
                                <input name="txtToCityName" class="inputtxtdat" runat="server" type="text" id="txtToCityName"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 150px;" mod_address_reference="ToCityCode" />
                                <input id="ToCityCode" name="ToCityCode" type="hidden" runat="server" />
                            </td>
                            <th>
                                舱位：
                            </th>
                            <td>
                                <input type="text" id="txtCabin" class="inputtxtdat" runat="server" />
                            </td>
                            <th>
                                舱位价：
                            </th>
                            <td>
                                <input type="text" id="txtDiscountRate" class="inputtxtdat" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                            </th>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td colspan="3">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " /></span><span class="btn btn-ok-s">
                                        <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" /></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                border="1" style="border-collapse: collapse;">
                <thead>
                    <tr>
                        <th>
                            承运人
                        </th>
                        <th>
                            舱位
                        </th>
                        <th>
                            舱位价格
                        </th>
                        <th>
                            出发城市
                        </th>
                        <th>
                            到达城市
                        </th>
                        <th>
                            出发城市三字码
                        </th>
                        <th>
                            到达城市三字码
                        </th>
                        <th>
                            国际/国内
                        </th>
                        <th>
                            生效时间
                        </th>
                        <th>
                            失效时间
                        </th>
                        <th>
                            添加时间
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repCabinList" runat="server" OnItemCommand="repCabinList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                                <%#Eval("AirCode")%>
                            </td>
                            <td>
                                <%#Eval("Cabin")%>
                            </td>
                            <td>
                                <%#Eval("CabinPrice")%>
                            </td>
                            <td>
                                <%#Eval("FromCity")%>
                            </td>
                            <td>
                                <%#Eval("ToCity")%>
                            </td>
                            <td>
                                <%#Eval("fromCityCode")%>
                            </td>
                            <td>
                                <%#Eval("toCityCode")%>
                            </td>
                            <td>
                                <%#ReturnType(Eval("IsGN").ToString().Trim())%>
                            </td>
                            <td>
                                <%#Eval("BeginTime").ToString().Split(' ')[0]%>
                            </td>
                            <td>
                                <%#Eval("EndTime").ToString().Split(' ')[0]%>
                            </td>
                            <td>
                                <%#Eval("AddDate").ToString().Split(' ')[0]%>
                            </td>
                            <td class="Operation">
                                <a href="CabinDiscountEdit.aspx?Id=<%#Eval("Id") %>&currentuserid=<%=this.mUser.id.ToString() %>">
                                    修改</a>
                                <asp:LinkButton runat="server" ID="btn_Del" CommandName="Del" OnClientClick="return confirm('确认要删除该舱位吗？');"
                                    CommandArgument='<%# Eval("Id") %>' Text="删除"></asp:LinkButton>
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
    </form>
</body>
</html>
