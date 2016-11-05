<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderTest.aspx.cs" Inherits="Pay_OrderTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退款处理：技术使用</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%; text-align: center;">
            <tr>
                <td valign="top" style="width: 40%; text-align: center;">
                    <table style="width: 100%; text-align: center;">
                        <tr>
                            <td style="width: 30%; text-align: right;">
                                支付方式
                            </td>
                            <td style="width: 70%; text-align: left;">
                                <asp:RadioButtonList ID="rblPayType" runat="server" RepeatColumns="5">
                                    <asp:ListItem Text="无" Value="0" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="支付宝" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="快钱" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="汇付" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="财付通" Value="4"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                交易号/流水号
                            </td>
                            <td style="text-align: left;">
                                <asp:TextBox ID="txtOnlineNo" runat="server" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                原支付订单号
                            </td>
                            <td style="text-align: left;">
                                <asp:TextBox ID="txtOldOrder" runat="server" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                退款订单号
                            </td>
                            <td style="text-align: left;">
                                <asp:TextBox ID="txtNewOrder" runat="server" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                收款账号
                            </td>
                            <td style="text-align: left;">
                                <asp:TextBox ID="txtAct" runat="server" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                退款金额
                            </td>
                            <td style="text-align: left;">
                                <asp:TextBox ID="txtPrice" runat="server" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                手续费费率
                            </td>
                            <td style="text-align: left;">
                                <asp:TextBox ID="txtRate" runat="server" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <br />
                                <asp:Button ID="btnQueryBalance" runat="server" Text="账户余额查询" OnClick="btnQueryBalance_Click" />
                                &nbsp;
                                <asp:Button ID="btnQuerySign" runat="server" Text="账户签约查询" OnClick="btnQuerySign_Click" />
                                &nbsp;
                                <asp:Button ID="btnSign" runat="server" Text="进行账户签约" onclick="btnSign_Click" />
                                <br />
                                <br />
                                <asp:Button ID="btnPaySel" runat="server" Text="支付查询处理" OnClick="btnPaySel_Click" />
                                &nbsp;
                                <asp:Button ID="btnRefundSel" runat="server" Text="退款查询处理" OnClick="btnRefundSel_Click" />
                                &nbsp;
                                <asp:Button ID="btnRefund" runat="server" Text="手动退款处理" OnClientClick="return confirm('请勿随便调试?');"
                                    OnClick="btnRefund_Click" />
                                <br />
                                <br />
                                <asp:Button ID="btnShenQ" runat="server" Text="调试申请退款" OnClick="btnShenQ_Click" OnClientClick="return confirm('请勿随便调试?');" />
                                &nbsp;
                                <asp:Button ID="btnTuik" runat="server" Text="调试处理退款" OnClick="btnTuik_Click" OnClientClick="return confirm('请勿随便调试?');" />
                                &nbsp;
                                <asp:Button ID="btnCAEPay" runat="server" Text="CAE自动代扣" OnClick="btnCAEPay_Click"
                                    OnClientClick="return confirm('请勿随便调试?');" />
                                <br />
                                <br />
                                <asp:Button ID="btnPayDetail" runat="server" Text="显示账单明细" OnClick="btnPayDetail_Click" />
                                &nbsp;
                                <asp:Button ID="btnPayMoney" runat="server" Text="计算订单金额" OnClick="btnPayMoney_Click" />
                                &nbsp;
                                <asp:Button ID="btnClear" runat="server" Text="清空查询结果" OnClientClick="return confirm('是否清空查询结果?');"
                                    OnClick="btnClear_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" style="width: 60%; text-align: center;">
                    <table style="width: 100%; text-align: center;">
                        <tr>
                            <td style="text-align: left; color: Red;" colspan="2">
                                <br />
                                <asp:TextBox ID="txtDetail" runat="server" Rows="8" TextMode="MultiLine" Width="100%"
                                    Height="100px" BackColor="Black" ForeColor="Lime"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">
                                <span style="color: Red;"></span>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left;">
                                <asp:TextBox ID="txtReturnValue" runat="server" Rows="20" TextMode="MultiLine" Width="100%"
                                    BackColor="Black" ForeColor="Lime"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
