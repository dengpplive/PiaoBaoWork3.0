<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RareBuildOilList.aspx.cs"
    Inherits="Manager_Base_RareBuildOilList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>生僻航线基建燃油管理</title>
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
        //显示对话框
        function showdialog(t) {
            var div=document.getElementById("dg");
            if(div==null) {
                div=document.createElement("div");
                div.id="dg";
                if(document.all) {
                    document.body.appendChild(div);
                }
                else {
                    document.insertBefore(div,document.body);
                }
            }
            $J(dg).html(t);
            $J(dg).dialog({
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
        //清空数据
        function ClearData() {
            $J('input[type="text"]').val("");
            $J('select option').eq(0).attr("selected",true)
            return false;
        }
        function SelData() {
            var IsValid=true;
            var msg="";
            var TAX=$J("#txtTAX").val();
            var RQFare=$J("#txtRQFare").val();
            if(TAX!=""&&$J.isNaN(TAX)) {
                msg="机建费必须为数字！";
            } else if(RQFare!=""&&$J.isNaN(RQFare)) {
                msg="燃油费必须为数字！";
            }
            if(msg!="") {
                showdialog(msg);
                IsValid=false;
            }
            return IsValid;
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
    <div id="tabs-1">
        <div class="title">
            <span>生僻航线基建燃油管理</span>
        </div>
        <div class="c-list-filter">
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
                        基建费:
                    </th>
                    <td>
                        <asp:TextBox ID="txtTAX" runat="server" MaxLength="5"></asp:TextBox>
                    </td>
                    <th>
                        燃油费：
                    </th>
                    <td>
                        <asp:TextBox ID="txtRQFare" runat="server" MaxLength="5"></asp:TextBox>
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
                        乘客类型：
                    </th>
                    <td>
                        <asp:DropDownList ID="ddlPasType" runat="server" Width="145px">
                            <asp:ListItem Selected="True" Text="不限" Value=""></asp:ListItem>
                            <asp:ListItem Text="成人" Value="1"></asp:ListItem>
                            <asp:ListItem Text="儿童" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <th>
                    </th>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="4">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click"
                                OnClientClick="return SelData(); " /></span> <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " OnClick="btnAdd_Click" /></span><span
                                        class="btn btn-ok-s">
                                        <asp:Button ID="Reset" runat="server" Text="重置数据" OnClientClick="return ClearData();" /></span>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                border="1" style="border-collapse: collapse;">
                <thead>
                    <tr>
                        <th>
                            操作
                        </th>
                        <th>
                            出发城市三字码
                        </th>
                        <th>
                            到达城市三字码
                        </th>
                        <th>
                            乘客类型
                        </th>
                        <th>
                            基建
                        </th>
                        <th>
                            燃油
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repCabinList" runat="server" OnItemCommand="repCabinList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td class="Operation">
                                <a href="EditRareBuildOil.aspx?id=<%#Eval("id") %>&currentuserid=<%=this.mUser.id.ToString() %>">
                                    修改</a>
                                <asp:LinkButton runat="server" ID="btn_Del" CommandName="Del" OnClientClick="return confirm('确认要删除该航线数据吗？');"
                                    CommandArgument='<%# Eval("id") %>' Text="删除"></asp:LinkButton>
                            </td>
                            <td>
                                <%#Eval("FromCityCode")%>
                            </td>
                            <td>
                                <%#Eval("ToCityCode")%>
                            </td>
                            <td>
                                <%# ShoText(1,Eval("PersonType"))%>
                            </td>
                            <td>
                                <%#Eval("BuildPrice")%>
                            </td>
                            <td>
                                <%#Eval("OilPrice")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="20" PagingButtonSpacing="3px"
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
