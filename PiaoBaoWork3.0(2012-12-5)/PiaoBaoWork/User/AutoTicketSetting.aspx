<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AutoTicketSetting.aspx.cs"
    Inherits="User_AutoTicketSetting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/area.js" type="text/javascript"></script>
    <title>收款账号管理</title>
    <script type="text/javascript">
        $(function () {
            // Tabs
            $('#tabs').tabs();
        });
        function showdialog(t) {
            $("#dd").html(t);
            $("#dd").dialog({
                title: '标题',
                bgiframe: true,
                height: 180,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $(this).dialog('close');
                        //location.href = "OnlineList.aspx";
                    }
                }
            });
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="tabs">
        <div id="AutoCpSet" runat="server">
            <div class="title">
                <span>自动出票代扣帐号绑定</span>
            </div>
            <div id="Div1">
                <table id="Table1" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                    width="100%">
                    <thead>
                        <tr>
                            <td class="td">
                                国航(CA)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtCAaount" runat="server" CssClass="txt" Style="width: 70px;"></asp:TextBox>
                                <asp:TextBox ID="txtCApwd" runat="server" CssClass="txt" Style="width: 70px;"></asp:TextBox>
                            </td>
                            <td class="td">
                                东航(MU)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtMUaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtMUpwd" runat="server" CssClass="txt" Style="width: 70px;"></asp:TextBox>
                            </td>
                            <td class="td">
                                海航(HU)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtHUaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtHUpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="td">
                                深航(ZH)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtZHaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtZHpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                联航(KN)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtKNaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtKNpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                山航(SC)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtSCaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtSCpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="td">
                                川航(3U)：
                            </td>
                            <td>
                                <asp:TextBox ID="txt3Uaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txt3Upwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                南航(CZ)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtCZaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtCZpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                厦航(MF)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtMFaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtMFpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="td">
                                上航(FM)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtFMaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtFMpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                吉航(HO)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtHOaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtHOpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                昆航(KY)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtKYaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtKYpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="td">
                                幸福航空(JR)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtJRaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtJRpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                成都航空(EU)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtEUaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtEUpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                西藏航空(TV)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtTVaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtTVpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="td">
                                天津(GS)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtGSaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtGSpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                首都(JD)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtJDaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtJDpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                西部(PN)：
                            </td>
                            <td>
                                <asp:TextBox ID="txtPNaount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txtPNpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="td">
                                祥鹏(8L)：
                            </td>
                            <td>
                                <asp:TextBox ID="txt8Laount" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                                <asp:TextBox ID="txt8Lpwd" runat="server" Style="width: 70px;" CssClass="txt"></asp:TextBox>
                            </td>
                            <td class="td">
                                
                            </td>
                            <td>
                               
                            </td>
                            <td class="td">
                                
                            </td>
                            <td>
                              
                            </td>
                        </tr>
                        <tr>
                            <th style="width: 10%">
                                失败重新调次数
                            </th>
                            <td style="width: 10%">
                                <asp:TextBox ID="txtfailcount" CssClass="txtyan" Width="200px" runat="server"></asp:TextBox>
                            </td>
                            <th>
                            </th>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <th style="width: 10%">
                                支付宝
                            </th>
                            <td style="width: 10%">
                                <asp:TextBox ID="txtAutoCPAlipay" CssClass="txtyan" Width="200px" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbAutoCPQY" runat="server" OnClick="lbAutoCPQY_Click">签约</asp:LinkButton>&nbsp;&nbsp;&nbsp;
                            </td>
                            <th>
                            </th>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <th style="width: 10%">
                                汇付天下
                            </th>
                            <td style="width: 10%">
                                <label for="txtChinapnrPwd">
                                    钱管家操作号</label>
                                <asp:TextBox ID="txtChinanprAccount" CssClass="txtyan" Width="150px" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <label for="txtChinapnrPwd">
                                    交易密码</label>
                                <asp:TextBox ID="txtChinapnrPwd" CssClass="txtyan" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <label>
                                    支付方式：</label>
                                <asp:RadioButton ID="rbXinyong" GroupName="payType" Text="信用账户" runat="server" />
                                <asp:RadioButton ID="rbFukuan" GroupName="payType" Text="付款账户" runat="server" />
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <asp:RadioButton ID="rbAlipay" GroupName="payWay" Text="支付宝" runat="server" />
                                <asp:RadioButton ID="rbChinapnr" GroupName="payWay" Text="汇付天下" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" style="text-align: center;">
                                <asp:LinkButton ID="lbAutoCPSave" runat="server" OnClick="lbAutoCPSave_Click" CssClass="btn btnNormal">保存</asp:LinkButton>
                            </td>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
    <div id="dd">
    </div>
    </form>
</body>
</html>
