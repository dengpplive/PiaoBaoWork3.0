<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderCoordinate.aspx.cs"
    Inherits="Order_OrderCoordinate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../css/detail.css" rel="stylesheet" />
     <script src="../../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
  
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            <strong style="color: #0C88BE;">操作日志</strong>
        </div>
        <table id="table5" class="else-table" border="0" cellspacing="0" cellpadding="0"
            width="100%">
            <thead>
                <tr>
                    <th>
                        操作时间
                    </th>
                    <th>
                        操作员账号
                    </th>
                    <th>
                        操作员姓名
                    </th>
                    <th>
                        操作类型
                    </th>
                    <th>
                        详细记录
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="RepOrderLog" runat="server">
                <ItemTemplate>
                    <tr class="leftliebiao_checi" style="line-height: 30px;">
                        <td style="width: 15%; text-align: center;">
                            <%#Eval("OperTime")%>
                        </td>
                        <td style="width: 15%; text-align: center;">
                            <%#Eval("OperLoginName")%>
                        </td>
                        <td style="width: 15%; text-align: center;">
                            <%#Eval("OperUserName")%>
                        </td>
                        <td style="width: 10%; text-align: center;">
                            <%#Eval("OperType")%>
                        </td>
                        <td style="width: 45%; text-align: center;">
                            <asp:Label Style="word-break: break-all; white-space: normal" ID="lblLogContent"
                                Width="100%" runat="server" Text=' <%#Eval("OperContent")%>' ToolTip='<%#Eval("OperContent") %>'></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div>
            <strong style="color: #0C88BE;">业务协调</strong>
        </div>
        <table class="table_info">
            <tr class="leftliebiao_checi">
                <td>
                    类型
                </td>
                <td>
                    <asp:DropDownList ID="ddltype" runat="server">
                    <asp:ListItem Value="协调">协调</asp:ListItem>
                    <asp:ListItem Value="催单">催单</asp:ListItem>
                    <asp:ListItem Value="航变">航变</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    备注
                </td>
                <td>
                    <asp:TextBox ID="txtRemark" runat="server" Height="60px" TextMode="MultiLine" MaxLength="200"
                        Width="600px"></asp:TextBox>
                </td>
            </tr>
            <tr>
            <td colspan="2" style="text-align:center">
                <asp:Button ID="btsave" runat="server" Text="保存" onclick="btsave_Click" />
            </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
