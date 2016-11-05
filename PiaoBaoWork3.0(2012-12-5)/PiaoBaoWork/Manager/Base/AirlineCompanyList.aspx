<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AirlineCompanyList.aspx.cs"
    Inherits="Sys_AirlineCompanyList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title></title>
    <script type="text/javascript" src="../../js/My97DatePicker/WdatePicker.js"></script>
    <style type="text/css">
        .s
        {
            border: 1px solid #000000;
            background-color: #666666;
        }
        .paginator
        {
            font: 11px Arial;
            padding: 10px 20px 10px 0; /*&#19978;&#21491;&#19979;&#24038;*/
            margin: 0px;
        }
        .paginator a
        {
            padding: 1px 6px;
            border: solid 1px #ddd;
            background: #fff;
            text-decoration: none;
            margin-right: 2px;
        }
        .paginator .cpb
        {
            padding: 1px 6px;
            font-weight: bold;
            font-size: 13px;
            border: none;
        }
        .paginator a:hover
        {
            color: #fff;
            background: #ffa501;
            border-color: #ffa501;
            text-decoration: none;
        }
        .Search tr
        {
            height: 30px;
            line-height: 30px;
        }
    </style>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">
        function showimg(obj) {
            $("#"+obj).show();
        }
        function hideimg(obj) {
            $("#"+obj).hide();
        }
        function showdialog(t) {
            $("#dd").html(t);
            $("#dd").dialog({
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
                        $(this).dialog('close');
                    }
                }
            });
        }
        $(function () {
            // Tabs
            $('#tabs').tabs();
            //&#25353;&#29275;&#26679;&#24335;
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); }
				);
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <!-- Tabs -->
    <div id="tabs">
        <div class="title">
            <span>承运人管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                承运人全称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCarrier" CssClass="inputtxtdat" runat="server" MaxLength="25"></asp:TextBox>
                            </td>
                            <th id="dd1">
                                承运人简称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtShort" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th id="dd2">
                                类型：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlGNGJ" runat="server" Width="115px">
                                    <asp:ListItem Value="ALL">全部</asp:ListItem>
                                    <asp:ListItem Value="1">国内</asp:ListItem>
                                    <asp:ListItem Value="0">国际</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                二字码：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCode" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                是否销售：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlXiaoShou" runat="server" Width="115px">
                                    <asp:ListItem Value="-1" Selected="True">全部</asp:ListItem>
                                    <asp:ListItem Value="0">不销售</asp:ListItem>
                                    <asp:ListItem Value="1">销售</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td colspan="2">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" /></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" cellspacing="0" cellpadding="0" border="0"
                width="100%">
                <thead>
                    <tr>
                        <th>
                            操作
                        </th>
                        <th>
                            承运人全称
                        </th>
                        <th>
                            承运人简称
                        </th>
                        <th>
                            二字码
                        </th>
                        <th>
                            类型
                        </th>
                        <th>
                            是否销售
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="Repeater" runat="server" OnItemCommand="Repeater_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                            <td class="Operation">
                                <a href="AirlineCompanyEdit.aspx?id=<%#Eval("Id")%>&currentuserid=<%=this.mUser.id.ToString() %>">修 改</a> |
                                <asp:LinkButton ID="lnkbtnDel" runat="server" CommandName="Del" CommandArgument='<%#Eval("Id")%>'
                                    OnClientClick="return confirm('确认删除？')">删 除</asp:LinkButton>
                            </td>
                            <td>
                                <%#Eval("AirName")%>
                            </td>
                            <td>
                                <%#Eval("ShortName")%>
                            </td>
                            <td>
                                <%#Eval("Code")%>
                            </td>
                            <td>
                                <%#Eval("Type").Equals(1)?"国内":"国际"%>
                            </td>
                            <td>
                                <%# GetXS(Eval("A1"))?"销售":"不销售"%>
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
