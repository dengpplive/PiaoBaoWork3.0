<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PolicySupplyList.aspx.cs" Inherits="Policy_PolicySupplyList" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .ui-corner-all
        {
            padding: 1px 6px;
        }
    </style>
    <script type="text/javascript">
        function showdialog(t) {
            $("#showOne").html(t);
            $("#showOne").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                width: 250,
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="showOne"></div>
      <div id="tabs">
        <div class="title">
            <span>平台补点列表</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                公司名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCopName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                承运人：
                            </th>
                            <td>
                            <asp:TextBox ID="txtAirPortCode" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询" OnClick="btnQuery_Click" CausesValidation="false">
                                    </asp:Button></span>&nbsp; <span class="btn btn-ok-s">
                                        <asp:Button ID="btnreset" runat="server" Text=" 重 置 " CausesValidation="false" OnClick="btnreset_Click">
                                        </asp:Button></span> &nbsp; <span class="btn btn-ok-s">
                                            <asp:Button ID="btnadd" runat="server" Text=" 添 加 " CausesValidation="false">
                                            </asp:Button></span>
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
                            公司名称
                        </th>
                        <th>
                            公司类型
                        </th>
                         <th>
                            承运人
                        </th>
                        <th>
                            出发城市
                        </th>
                        <th>
                            到达城市
                        </th>
                        <th>
                            补充返点值
                        </th>
                        <th>
                            状态
                        </th>
                        <th>
                            操作员
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repList" runat="server" onitemcommand="repList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                                <%#Eval("CpyName")%>
                            </td>
                            <td>
                                <%#Eval("CpyType").ToString() == "2" ? "运营商" : "供应商" %>
                            </td>
                            <td>
                                <%#Eval("CarryCode")%>
                            </td>
                            <td>
                                <%#Eval("FromCityCode")%>
                            </td>
                            <td>
                                <%#Eval("ToCityCode")%>
                            </td>
                            <td>
                                <%# Eval("PolicyPoint")%>%
                            </td>
                             <td>
                                <%# Eval("State").ToString() == "1" ? "启用" : "禁用" %>
                            </td>
                             <td>
                                <%# Eval("OperLoginName")%>
                            </td>
                            <td class="Operation">
                                <a href='PolicySupplyEdit.aspx?id=<%#Eval("Id") %>&currentuserid=<%=this.mUser.id.ToString() %>'>修改</a>
                                 <br />
                            <asp:LinkButton ID="lbtnDel" runat="server" CommandName="Del" CommandArgument='<%# Eval("id") %>'>删除</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
