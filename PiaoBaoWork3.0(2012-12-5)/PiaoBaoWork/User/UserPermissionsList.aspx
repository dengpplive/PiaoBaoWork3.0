<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserPermissionsList.aspx.cs"
    Inherits="User_UserPermissionsList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title></title>
    <meta http-equiv="x-ua-compatible" content="ie=7" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info">
                <tr>
                    <td class="bt">
                        权限
                    </td>
                </tr>
            </table>
            <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                width="100%">
                <thead>
                    <tr>
                        <%-- <th style="width: 5%;">
                            选 择
                        </th>--%>
                        <th style="width: 10%;">
                            权限名称
                        </th>
                        <th style="width: 80%;">
                            功能页面
                        </th>
                        <th style="width: 5%;">
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repUserPermissions" runat="server" OnItemDataBound="repUserPermissions_ItemDataBound">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                            <%--<td>
                                <asp:RadioButton ID="rbtnSel" runat="server" />
                            </td>--%>
                            <td>
                                <%#Eval("DeptName")%>
                            </td>
                            <td>
                                <%# BindPermissionsName(Eval("Permissions") == null ? "" : Eval("Permissions").ToString())%>
                            </td>
                            <td class="Operation" style="width: 7%">
                                <%--<asp:LinkButton ID="UpDateButton" CommandName="Update" runat="server" Text=" 修 改 "
                                    CommandArgument='<%#Eval("id")%>' /><br />--%>
                                <asp:LinkButton ID="UpDateButton" runat="server" Text=" 修 改 " CommandName='<%#Eval("DeptName")%>'
                                    CommandArgument='<%#Eval("id")%>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <%--<td>
                        用户权限:
                        <span class="btn btn-ok-s">
                            <asp:Button ID="Button1" runat="server" Text="保 存" /></span>

                    </td>--%>
                    <td colspan="3">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnAddSel" runat="server"
                                Text="添 加" /></span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
