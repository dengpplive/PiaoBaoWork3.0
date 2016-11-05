<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserVirtual.aspx.cs" Inherits="User_UserVirtual" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>账户基本信息</title>
    <meta http-equiv="x-ua-compatible" content="ie=7" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../js/Validation.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info">
                <tr>
                    <td colspan="2" class="bt">
                        账户基本信息
                    </td>
                </tr>
                <tr>
                    <td style="width: 50%;" class="td">
                        公司单位名称：
                    </td>
                    <td style="width: 50%;">
                        <asp:Label ID="lblCpyName" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50%;" class="td">
                        账号可用余额：
                    </td>
                    <td style="width: 50%;">
                        <asp:Label ID="lblAccountMoney" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50%;" class="td">
                        最大欠款额度：
                    </td>
                    <td style="width: 50%;">
                        <asp:Label ID="lblMaxDebtMoney" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 50%;" class="td">
                        还款周期：
                    </td>
                    <td style="width: 50%;">
                        <asp:Label ID="lblMaxDebtDays" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dd">
        </div>
    </div>
    </form>
</body>
</html>
