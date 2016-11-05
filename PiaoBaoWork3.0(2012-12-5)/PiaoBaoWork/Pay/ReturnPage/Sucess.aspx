<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sucess.aspx.cs" Inherits="Pay_ReturnPage_Sucess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>支付返回页面</title>
</head>
<body style="padding: 50px 0 0 0">
    <form id="form1" runat="server">
    <div style="margin: 0 auto; width: 680px;">
        <table cellspacing="0" cellpadding="0" border="0" width="660" runat="server" id="tbid" visible="false">
            <tr runat="server" id="trSuccess">
                <td colspan="2" style="background: url(../../img/paysuce.gif) no-repeat scroll 0 0 transparent;
                    height: 119px;">
                </td>
            </tr>
            <tr runat="server" id="trFail" visible="false">
                <td colspan="2" style="background: url(../../img/payFail.gif) no-repeat scroll 0 0 transparent;
                    height: 119px;">
                </td>
            </tr>
            <tr>
                <td colspan="2" style="height: 50px; line-height: 50px; color: #606060; font-size: 14px;">
                    以下是您的支付信息
                </td>
            </tr>
            <tr>
                <td style="height: 30px; line-height: 30px; color: #ff9833; font-size: 14px; width: 80px;">
                    订单号：
                </td>
                <td>
                    <asp:Label runat="server" ID="lblOrderId"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="border-bottom: 1px solid #dddddd">
                </td>
            </tr>
            <tr  runat="server" id="trFail2">
                <td style="height: 30px; line-height: 30px; color: #ff9833; font-size: 14px; width: 80px;">
                    交易号：
                </td>
                <td>
                    <asp:Label runat="server" ID="lblOnLineNo"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="border-bottom: 1px solid #dddddd">
                </td>
            </tr>
            <tr>
                <td style="height: 30px; line-height: 30px; color: #ff9833; font-size: 14px; width: 80px;">
                    金额：
                </td>
                <td>
                    <asp:Label runat="server" ID="lblPrice"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="border-bottom: 1px solid #dddddd">
                </td>
            </tr>
            <tr>
                <td colspan="2" style="height: 30px; line-height: 30px;">
                    <button id="btnok" onclick="window.close();">
                        确定</button>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
