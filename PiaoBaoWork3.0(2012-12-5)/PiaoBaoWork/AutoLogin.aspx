<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AutoLogin.aspx.cs" Inherits="AutoLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript">
        function AutoLogin(url) {
            window.open(unescape(url), '_blank', 'height=800, width=1024,toolbar=yes, menubar=yes, scrollbars=yes,resizable=yes,location=yes, status=yes');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size: 12px">
        <table style="width: 100%;">
            <tr>
                <td style="width: 15%;">
                    登录账号:
                </td>
                <td style="width: 85%;">
                    <asp:TextBox ID="txtLogin" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    类型：
                </td>
                <td>
                    <asp:RadioButtonList ID="rblType" runat="server" RepeatColumns="3">
                        <asp:ListItem Value="91" Selected="True">正式站</asp:ListItem>
                        <asp:ListItem Value="204">测试站</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    登录地址:
                </td>
                <td>
                    <asp:TextBox ID="txtUrl" runat="server" Width="90%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button ID="btn" runat="server" Text="自动登录" OnClick="btn_Click" />
                </td>
            </tr>
        </table>
        <br />
    </div>
    </form>
</body>
</html>
