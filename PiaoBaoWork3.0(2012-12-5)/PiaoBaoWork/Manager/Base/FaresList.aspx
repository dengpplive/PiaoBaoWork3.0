<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FaresList.aspx.cs" Inherits="Sys_FaresList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>票价管理</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
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
        .style1
        {
            width: 105px;
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
        $J(function () {
            // Tabs
            $J('#tabs').tabs();

        });
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
            var zz1=/^\d+(\.\d{2})?$/
            var zz2=/^\d*$/
            $J("#txtFareFee1").keyup(function () {
                if(!zz1.test($J("#txtFareFee1").val())) {
                    $J("#txtFareFee1").val("");
                }
            })
            $J("#txtFareFee2").keyup(function () {
                if(!zz1.test($J("#txtFareFee1").val())) {
                    $J("#txtFareFee1").val("");
                }
            })

            $J("#txtMileage1").keyup(function () {
                if(!zz2.test($J("#txtMileage1").val())) {
                    $J("#txtMileage1").val("");
                }
            })
            $J("#txtMileage2").keyup(function () {
                if(!zz2.test($J("#txtMileage2").val())) {
                    $J("#txtMileage2").val("");
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
            <span>票价管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th style="width: 120px;">
                                出发城市三字码:
                            </th>
                            <td>
                                <%-- 出发城市--%>
                                <input name="txtFromCode" class="inputtxtdat" runat="server" type="text" id="txtFromCode"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 150px;" mod_address_reference="FromCityCode" />
                                <input id="FromCityCode" name="FromCityCode" type="hidden" runat="server" />
                            </td>
                            <th>
                                国际/国内：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlType" runat="server" Width="115px">
                                    <asp:ListItem Selected="True" Text="不限" Value="wx"></asp:ListItem>
                                    <asp:ListItem Text="国内" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="国际" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th style="width: 120px;">
                                到达城市三字码:
                            </th>
                            <td>
                                <%-- 到达城市--%>
                                <input name="txtToCode" class="inputtxtdat" runat="server" type="text" id="txtToCode"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 150px;" mod_address_reference="ToCityCode" />
                                <input id="ToCityCode" name="ToCityCode" type="hidden" runat="server" />
                            </td>
                            <th>
                                承运人:
                            </th>
                            <td>
                                <uc1:SelectAirCode ID="txtCarryCode" IsShowAll="true" IsDShowName="false" DefaultOptionValue=""
                                    runat="server" />
                            </td>
                            <td colspan="3">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " /></span> <span class="btn btn-ok-s">
                                        <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" /></span>
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
                            出发城市三字码
                        </th>
                        <th>
                            到达城市三字码
                        </th>
                        <th>
                            出发城市
                        </th>
                        <th>
                            到达城市
                        </th>
                        <th>
                            票价
                        </th>
                        <th>
                            里程
                        </th>
                        <th>
                            航空公司
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
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repFaresList" runat="server" OnItemCommand="repFaresList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                            <td>
                                <%#Eval("FromCityCode")%>
                            </td>
                            <td>
                                <%#Eval("ToCityCode")%>
                            </td>
                            <td>
                                <%#Eval("FromCityName")%>
                            </td>
                            <td>
                                <%#Eval("ToCityName")%>
                            </td>
                            <td>
                                <%#Eval("FareFee")%>
                            </td>
                            <td>
                                <%#Eval("Mileage")%>
                            </td>
                            <td>
                                <%#Eval("CarryCode")%>
                            </td>
                            <td>
                                <%#ReturnType(Eval("IsDomestic").ToString().Trim())%>
                            </td>
                            <td>
                                <%#Eval("EffectTime").ToString().Split(' ')[0]%>
                            </td>
                            <td>
                                <%#Eval("InvalidTime").ToString().Split(' ')[0]%>
                            </td>
                            <td class="Operation">
                                <a href="FaresEdit.aspx?Id=<%#Eval("Id") %>&currentuserid=<%=this.mUser.id.ToString() %>">
                                    修改</a>
                                <asp:LinkButton ID="lnkBtnDel" runat="server" CommandName="Del" CommandArgument='<%#Eval("Id") %>'
                                    OnClientClick="return confirm('确认删除？')" Text="删除"></asp:LinkButton>
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
    <script language="javascript" type="text/javascript">
        $J(function () {
            ReLoad();
        });
    </script>
    </form>
</body>
</html>
