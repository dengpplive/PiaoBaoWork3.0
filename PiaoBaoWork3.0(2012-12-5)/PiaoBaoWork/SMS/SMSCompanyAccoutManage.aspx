<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SMSCompanyAccoutManage.aspx.cs"
    Inherits="SMS_SMSCompanyAccoutManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link href="../js/My97DatePicker/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.4.1.js"></script>
    
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="title">
        <span>公司账户管理</span></div>
    <div class="c-list-filter">
        <table>
            <tr id="Trsmscount" runat="server">
                <td align="right" style="width: 100px;">
                    账户剩余条数:
                </td>
                <td style="width: auto;">
                    <asp:Label ID="lbsmsCount" runat="server" Text="0"></asp:Label>
                    条&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
                <td align="left">
                    <%--<a href="Transaction.aspx?id=<%=cpyId%>">查看充值明细 </a>--%>
                    <span class="btn btn-ok-s">
                        <asp:Button runat="server" Text="查看充值明细" OnClick="btnRechargeDetail_Click" />
                    </span>
                </td>
            </tr>
            <tr>
                <td align="right" style="width: 100px;">
                    已发送条数:
                </td>
                <td style="width: auto;">
                     <asp:Label ID="lbsmsSendCount" runat="server" Text="0"></asp:Label>
                    条&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
                <td align="left">
                    <%--<a href="Send.aspx?id=<%=cpyId%>">查看发送明细 </a>--%>
                    <span class="btn btn-ok-s">
                        <asp:Button runat="server" Text="查看发送明细" ID="btnSend" OnClick="btnSendDetail_Click" />
                    </span>
                </td>
            </tr>
        </table>
        <table id="Tbsmstemplate" runat="server">
            <tr>
                <td align="right" style="width: 100px;">
                    账户充值:
                </td>
                <td align="left" style="width: auto;">
                    <asp:RadioButtonList ID="rblSmsTemplate" runat="server" RepeatDirection="Horizontal">
                    </asp:RadioButtonList>
                </td>
                <td>
                    <%--<a id="paySms">点击购买</a>--%>
                    <span class="btn btn-ok-s">
                        <asp:Button runat="server" Text="立即购买" ID="btnBuy" OnClick="btnBuy_Click" />
                    </span>
                </td>
            </tr>
        </table>
       
    </div>
    </form>
</body>
</html>
