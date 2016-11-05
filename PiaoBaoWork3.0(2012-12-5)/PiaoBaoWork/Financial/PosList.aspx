<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PosList.aspx.cs" Inherits="Financial_PosList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
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
        $(function () {
            // Tabs
            $('#tabs').tabs();
        });
        function showdialog(t) {
            $("#show").html(t);
            $("#show").dialog({
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="show"></div>
    <div id="tabs">
        <div class="title">
            <span>pos机管理</span>
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
                                Pos机编号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtTerminalNo" CssClass="inputtxtdat" runat="server"></asp:TextBox>
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
                            Pos机编号
                        </th>
                        <th>
                            费率
                        </th>
                        <th>
                            Pos机类型
                        </th>
                        <th>
                            公司编号
                        </th>
                        <th>
                            公司名称
                        </th>
                        <th>
                            操作员
                        </th>
                        <th>
                            操作员所属公司
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repPosList" runat="server" 
                    onitemcommand="repPosList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                                <%#Eval("PosNo")%>
                            </td>
                            <td>
                                <%#Eval("PosRate") %>
                            </td>
                            <td>
                                <%#ReturnType(Eval("PosMode").ToString())%>
                            </td>
                            <td>
                                <%#Eval("CpyNo")%>
                            </td>
                            <td>
                                <%#Eval("CpyName")%>
                            </td>
                            <td>
                                <%#Eval("OperLoginName")%>
                            </td>
                            <td>
                                <%# Eval("OperCpyName")%>
                            </td>
                            <td class="Operation">
                                <a href='PosEdit.aspx?id=<%#Eval("Id") %>&currentuserid=<%=this.mUser.id.ToString() %>'>修改</a>
                                 <asp:LinkButton ID="lbdelete" runat="server" CommandArgument='<%# Eval("id") %>'
                                    OnClientClick="if(confirm('是否确认删除？')){return true;}else{return false;}">删除</asp:LinkButton>
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
