<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TicketPrintOfficeList.aspx.cs"
    Inherits="Manager_Base_TicketPrintOfficeList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="title">
        <span>航空公司出票设置管理</span>
    </div>
    <div id="tabs">
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;">
                        <tr>
                            <th>
                                承运人：
                            </th>
                            <td>
                                <uc1:SelectAirCode ID="txtAirPortCode" DefaultOptionValue="" IsDShowName="false"
                                    runat="server" style="width: 150px;" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                            </th>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 "/></span><span
                                        class="btn btn-ok-s">
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
                            Office号
                        </th>
                        <th>
                            打票机号
                        </th>
                        <th>
                            操作人
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repList" runat="server" OnItemCommand="repList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                                <%#Eval("AirCode")%>
                            </td>
                            <td>
                                <%#Eval("OfficeCode")%>
                            </td>
                            <td>
                                <%#Eval("PrintCode")%>
                            </td>
                            <td>
                                <%#Eval("OperLoginName")%>
                            </td>
                            <td class="Operation">
                                <a href="TicketPrintOfficeEdit.aspx?Id=<%#Eval("id") %>&currentuserid=<%=this.mUser.id.ToString() %>">修改</a>
                                <asp:LinkButton runat="server" ID="btn_Del" CommandName="Del" OnClientClick="return confirm('确认要删除该舱位吗？');"
                                    CommandArgument='<%# Eval("id") %>' Text="删除"></asp:LinkButton>
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
