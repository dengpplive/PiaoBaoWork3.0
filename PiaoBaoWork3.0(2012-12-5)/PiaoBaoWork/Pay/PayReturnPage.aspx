<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PayReturnPage.aspx.cs" Inherits="Pay_PayReturnPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <div>
        <table style="width: 100%;">
            <tr>
                <td style="width: 10%;">
                    <asp:Label ID="Label1" runat="server" Text="支付方式"></asp:Label>
                </td>
                <td style="width: 90%;">
                    <asp:RadioButtonList ID="rblPayType" runat="server" RepeatColumns="10">
                        <asp:ListItem Text="无" Value="0" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="支付宝" Value="1"></asp:ListItem>
                        <asp:ListItem Text="快钱" Value="2"></asp:ListItem>
                        <asp:ListItem Text="汇付" Value="3"></asp:ListItem>
                        <asp:ListItem Text="财付通" Value="4"></asp:ListItem>
                        <asp:ListItem Text="支付宝POS" Value="9"></asp:ListItem>

                        <asp:ListItem Text="汇付POS" Value="12"></asp:ListItem>
                        <asp:ListItem Text="易宝POS" Value="13"></asp:ListItem>

                         <asp:ListItem Text="B2B自动出票（支付宝）" Value="20"></asp:ListItem>

                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="通知内容"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtReturnValue" runat="server" Rows="30" MaxLength="300" TextMode="MultiLine"
                        Width="100%" BackColor="Black" ForeColor="Lime"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button ID="btnNotify" runat="server" Text="手动补发通知" OnClientClick="return confirm('确定要补发通知吗？此操作非技术人员请谨慎操作！！！');"
                        OnClick="btnNotify_Click" />
                    <span style="color: Red">注：在线支付、POS支付！！！</span>
                </td>
            </tr>
        </table>
    </div>
    <asp:Label ID="lblUrl" runat="server" Text=""></asp:Label>
    </form>
</body>
</html>
