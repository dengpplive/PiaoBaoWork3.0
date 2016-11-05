<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReceivablesBind.aspx.cs"
    Inherits="Financial_ReceivablesBind" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>收款账号管理</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
     .but_hj{}
     .tb-all-trade td{ padding:5px}
     .tb-all-trade th{ padding:5px; border-bottom:1px #ddd solid;border-right:1px #ddd solid}
     .btn-ok-s input{ margin:0px; padding:0; border:1px #0092FF solid; padding:1px}
     .btn_hj{ padding:0px; margin:0px; border:1px #000 solid}
     .btn-ok-s{ padding:0; margin:0; border:0}
    </style>
    <script type="text/javascript">
        function showdialog(t) {
            $("#dd").html(t);
            $("#dd").dialog({
                title: '标题',
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
    <form id="form2" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <!-- Tabs -->
        <div class="title" style="width: 100%;">
            <span>收款账号管理</span>
        </div>
        <table id="tbReceivables" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
            width="100%">
            <thead>
                <tr>
                    <th style="width: 8%">
                        支付公司
                    </th>
                    <th style="width: 20%; text-align: left;">
                        <asp:Label ID="lblShouKuan" runat="server" Text="支付收款账号"></asp:Label>
                    </th>
                    <th id="thid" visible="false" runat="server" style="width: 20%; text-align: left;">
                        <asp:Label ID="lblChongZhi" runat="server" Text="充值收款账号"></asp:Label>
                    </th>
                </tr>
            </thead>
            <tr>
                <td>
                    支付宝
                </td>
                <td style="text-align: left;">
                    <!--支付收款账号-->
                    <asp:TextBox ID="txtZFB" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                    <span class="btn btn-ok-s"><asp:Button ID="btnZFBOK" CssClass="btn_hj"   runat="server" Text="保存" Visible="true" OnClick="btnZFBOK_Click" /></span><asp:LinkButton
                        ID="lbtnZFBSign" runat="server" Visible="true" OnClick="lbtnZFBSign_Click">签约</asp:LinkButton>
                </td>
                <td id="tdzfb" style="text-align: left;" runat="server" visible="false">
                    <!--充值收款账号-->
                    <asp:TextBox ID="txtZFB_Repay" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                    <span class="btn btn-ok-s"><asp:Button ID="btnZFBOK_Repay" runat="server" Text="保存" Visible="true" OnClick="btnZFBOK_Repay_Click" /></span>
                    <asp:LinkButton ID="lbtnZFBSign_Repay" runat="server" Visible="true" OnClick="lbtnZFBSign_Repay_Click">签约</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td>
                    快钱
                </td>
                <td style="text-align: left;">
                    <!--支付收款账号-->
                    <asp:TextBox ID="txtKQ" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                    <span class="btn btn-ok-s"><asp:Button ID="btnKQOK" runat="server" CssClass="btn_hj"  Text="保存" Visible="true" OnClick="btnKQOK_Click" /></span>
                    <asp:LinkButton ID="lbtnKQSign" runat="server" Visible="false">签约</asp:LinkButton>
                </td>
                <td id="tdkq" style="text-align: left;" runat="server" visible="false">
                    <!--充值收款账号-->
                    <asp:TextBox ID="txtKQ_Repay" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                    <span class="btn btn-ok-s"><asp:Button ID="btnKQOK_Repay" runat="server" Text="保存" Visible="true" OnClick="btnKQOK_Repay_Click" /></span><asp:LinkButton
                        ID="lbtnKQSign_Repay" runat="server" Visible="false">签约</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td>
                    汇付
                </td>
                <td style="text-align: left;">
                    <!--支付收款账号-->
                    <asp:TextBox ID="txtHF" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                    <span class="btn btn-ok-s"><asp:Button ID="btnHFOK" CssClass="btn_hj"  runat="server" Text="保存" Visible="true" OnClick="btnHFOK_Click" /></span>
                    <asp:LinkButton ID="lbtnHFSign" runat="server" Visible="true" OnClick="lbtnHFSign_Click">签约</asp:LinkButton>
                </td>
                <td id="tbhf" style="text-align: left;" runat="server" visible="false">
                    <!--充值收款账号-->
                    <asp:TextBox ID="txtHF_Repay" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                     <span class="btn btn-ok-s"><asp:Button ID="btnHFOK_Repay" runat="server" Text="保存" Visible="true" OnClick="btnHFOK_Repay_Click" /></span>
                    <asp:LinkButton ID="lbtnHFSign_Repay" runat="server" Visible="true" OnClick="lbtnHFSign_Repay_Click">签约</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td>
                    财付通
                </td>
                <td style="text-align: left;">
                    <!--支付收款账号-->
                    <asp:TextBox ID="txtCFT" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                    <span class="btn btn-ok-s"><asp:Button ID="btnCFTOK" CssClass="btn_hj"  runat="server" Text="保存" Visible="true" OnClick="btnCFTOK_Click" /></span>
                    <asp:LinkButton ID="lbtnCFTSign" runat="server" Visible="true" OnClick="lbtnCFTSign_Click">签约</asp:LinkButton>
                </td>
                <td id="tbcft" style="text-align: left;" runat="server" visible="false">
                    <!--充值收款账号-->
                    <asp:TextBox ID="txtCFT_Repay" runat="server" CssClass="txt" Style="width: 180px;"></asp:TextBox>
                    <span class="btn btn-ok-s"><asp:Button ID="btnCFTOK_Repay" runat="server" Text="保存" Visible="true" OnClick="btnCFTOK_Repay_Click" /></span>
                    <asp:LinkButton ID="lbtnCFTSign_Repay" runat="server" Visible="true" OnClick="lbtnCFTSign_Repay_Click">签约</asp:LinkButton>
                </td>
            </tr>
        </table>
        <input type="hidden" value="0" id="hidden_strValue" runat="server" />
        <input type="hidden" value="0" id="hidden_id" runat="server" />
        <input type="hidden" value="0" id="hidden_ZFB" runat="server" />
        <input type="hidden" value="0" id="hidden_KQ" runat="server" />
        <input type="hidden" value="0" id="hidden_HF" runat="server" />
        <input type="hidden" value="0" id="hidden_CFT" runat="server" />
    </div>
    <div id="dd">
    </div>
    </form>
</body>
</html>
