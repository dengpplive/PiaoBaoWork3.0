<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CpyBankAccountEdit.aspx.cs" Inherits="Financial_CpyBankAccountEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            color: Red;
        }
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
       
        function showAllErr() {
            var bools = 0;
            if ($("#ddlBankType").val() == "0") {
                bools++;
                $("#spBankType").html("请选择！");
            }
            if ($("#ddlUseType").val() == "0") {
                bools++;
                $("#spUseType").html("请选择！");
            }

            if ($("#txtAccount").val() == "") {
                bools++;
                $("#spAccount").html("不能为空！");
            }

            if ($("#ddlBankType").val() == "1") {
                if ($("#txtBankName").val() == "") {
                    bools++;
                    $("#spBankName").html("不能为空！");
                }
                if ($("#txtAccountBank").val() == "") {
                    bools++;
                    $("#spAccountBank").html("不能为空！");
                }

                if ($("#txtAccountUserName").val() == "") {
                    bools++;
                    $("#spAccountUserName").html("不能为空！");
                }
            }
           
            return bools > 0 ? false : true;
          
        }
    </script>
</head>
<body>
     <form id="form1" runat="server">
     <div id="dd">
    </div>
    <div class="infomain">
        <div class="mainPanel">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info" class="table_info">
                <tr>
                    <td colspan="4" class="bt">
                        公司收支账号编辑
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>账号类型：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlBankType" CssClass="txt" runat="server" AutoPostBack="true" 
                            Style="width: 185px;" 
                            onselectedindexchanged="ddlBankType_SelectedIndexChanged">
                        <asp:ListItem Selected="True" Value="0">--选择--</asp:ListItem>
                        <asp:ListItem Value="1">银行账户</asp:ListItem>
                        <asp:ListItem Value="2">支付宝</asp:ListItem>
                        <asp:ListItem Value="3">快钱</asp:ListItem>
                        <asp:ListItem Value="4">汇付天下</asp:ListItem>
                        <asp:ListItem Value="5">财付通</asp:ListItem>
                        </asp:DropDownList>
                        <span id="spBankType" class="style1"></span>
                    </td>
                    <td class="td">
                        <span class="style1">*</span>使用类型：
                    </td>
                    <td>
                       <asp:DropDownList ID="ddlUseType" CssClass="txt" runat="server" Style="width: 185px;">
                         <asp:ListItem Selected="True" Value="0">--选择--</asp:ListItem>
                        <asp:ListItem Value="1">分账账户</asp:ListItem>
                        <asp:ListItem Value="2">支付收款</asp:ListItem>
                        <asp:ListItem Value="3">充值收款</asp:ListItem>
                        <asp:ListItem Value="4">代付账号</asp:ListItem>
                        <asp:ListItem Value="5">扣点分账账号</asp:ListItem>
                        </asp:DropDownList>
                        <span id="spUseType" class="style1"></span>
                    </td>
                </tr>
                 <tr>
                    <td class="td">
                        <span class="style1">*</span>账号：
                    </td>
                    <td>
                        <asp:TextBox ID="txtAccount" CssClass="txt" runat="server" Columns="16" Style="width: 175px;"></asp:TextBox>
                        <span id="spAccount" class="style1"></span>
                    </td>
                    <td class="td">
                        签约状态：
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblSignFlag" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="true">是</asp:ListItem>
                            <asp:ListItem Value="false" Selected="True">否</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <div runat="server" id="trshow" visible="false">
                <tr>
                    <td class="td">
                        <span class="style1">*</span>银行名称：
                    </td>
                    <td>
                        <asp:TextBox ID="txtBankName" CssClass="txt" runat="server" Columns="16" Style="width: 175px;"></asp:TextBox>
                        <span id="spBankName" class="style1"></span>
                    </td>
                    <td class="td">
                        <span class="style1">*</span>开户行：
                    </td>
                    <td>
                        <asp:TextBox ID="txtAccountBank" CssClass="txt" runat="server" Columns="16" Style="width: 175px;"></asp:TextBox>
                        <span id="spAccountBank" class="style1"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                       <span class="style1">*</span>开户人：
                    </td>
                    <td colspan="3" rowspan="2">
                    <asp:TextBox ID="txtAccountUserName" CssClass="txt" runat="server" Columns="16" Style="width: 175px;"></asp:TextBox>
                        <span id="spAccountUserName" class="style1"></span>
                    </td>
                </tr>
                </div>
            </table>
            </ContentTemplate>
            </asp:UpdatePanel>
            
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="lbsave" runat="server" CssClass="btn btnNormal"
                            onclick="lbsave_Click">保  存 </asp:LinkButton>
                        &nbsp;&nbsp;&nbsp; <a href="CpyBankAccountList.aspx?currentuserid=<%=this.mUser.id.ToString() %>" class="btn btnNormal">返 回 </a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <input type="hidden" runat="server" id="hidAirNoCount" value="1" />
    </form>
</body>
</html>
