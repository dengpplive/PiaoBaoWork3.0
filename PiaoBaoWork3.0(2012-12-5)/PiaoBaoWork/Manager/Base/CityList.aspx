<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CityList.aspx.cs" Inherits="Sys_CityList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>城市管理</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .container tr
        {
            height: 30px;
            line-height: 30px;
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

        $J(function () {
            // Tabs
            $J('#tabs').tabs();

        });

         
       
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div id="tabs">
        <div class="title">
            <span>城市管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                城市三字码：
                            </th>
                            <td>
                                <asp:TextBox CssClass="inputtxtdat" ID="txtCode" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                城市中文：
                            </th>
                            <td>
                                <asp:TextBox CssClass="inputtxtdat" ID="txtCity" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                城市全拼：
                            </th>
                            <td>
                                <asp:TextBox CssClass="inputtxtdat" ID="txtSpell" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                国家名字：
                            </th>
                            <td>
                                <asp:TextBox CssClass="inputtxtdat" ID="txtCountries" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                国家类型：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlType" runat="server" Width="115px">
                                    <asp:ListItem Selected="True" Text="不限" Value="wx"></asp:ListItem>
                                    <asp:ListItem Text="国内" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="国际" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td colspan="2">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " /></span>
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
                            城市中文
                        </th>
                        <th>
                            城市全拼
                        </th>
                        <th>
                            城市简称
                        </th>
                        <th>
                            城市三字码
                        </th>
                        <th>
                            城市其他三字码
                        </th>
                        <th>
                            机场名称
                        </th>
                        <th>
                            国家名字
                        </th>
                        <th>
                            国家类型
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repCityList" runat="server" OnItemCommand="repCityList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                            <td>
                                <%#Eval("CityName")%>
                            </td>
                            <td>
                                <%#Eval("CityQuanPin")%>
                            </td>
                            <td>
                                <%#Eval("CityJianPin")%>
                            </td>
                            <td>
                                <%#Eval("CityCodeWord")%>
                            </td>
                            <td>
                                <%#Eval("CityOtherCodeWord")%>
                            </td>
                            <td>
                                <%#Eval("AirPortName")%>
                            </td>
                            <td>
                                <%#Eval("Continents")%>
                            </td>
                            <td>
                                <%#ReturnType(Eval("IsDomestic").ToString().Trim())%>
                            </td>
                            <td class="Operation">
                                <a href="CityEdit.aspx?Id=<%#Eval("Id") %>&currentuserid=<%=this.mUser.id.ToString() %>">
                                    修改</a>|<asp:LinkButton ID="lnkBtnDel" runat="server" CommandName="Del" CommandArgument='<%#Eval("Id") %>'
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
    </form>
</body>
</html>
