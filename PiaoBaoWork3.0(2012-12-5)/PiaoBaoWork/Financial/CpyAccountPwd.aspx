<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CpyAccountPwd.aspx.cs" Inherits="Financial_CpyAccountPwd" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #table_info th a, #table_info td a
        {
            color: Black;
            text-decoration: none;
        }
    </style>
    <script language="javascript" type="text/javascript">
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
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div class="infomain">
        <div class="mainPanel" runat="server" id="updatepwd">
            <table id="table_info" class="table_info" width="100%" cellspacing="0" cellpadding="0"
                border="0">
                <tr>
                    <td colspan="2" class="bt">
                        修改设置支付密码
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        帐号：
                    </td>
                    <td>
                        <asp:TextBox ID="UserId" CssClass="txt" runat="server" Columns="16" Enabled="False"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        姓名：
                    </td>
                    <td>
                        <asp:TextBox ID="UserName" CssClass="txt" runat="server" Columns="16" Enabled="False"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="troldpwd">
                    <td class="td">
                        原密码：
                    </td>
                    <td>
                        <asp:TextBox ID="OldPWD" CssClass="txt" runat="server" Columns="16" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="OldPWD"
                            ErrorMessage="原密码不能为空" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        新密码：
                    </td>
                    <td>
                        <asp:TextBox ID="NewPwd" CssClass="txt" runat="server" Columns="16" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="NewPwd"
                            ErrorMessage="新密码不能为空" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server"
                            ControlToValidate="NewPwd" ErrorMessage="必须是六位" ForeColor="Red" ValidationExpression="(\d|[a-z]|[A-Z]){6}"
                            Display="Dynamic"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        确认密码：
                    </td>
                    <td>
                        <asp:TextBox ID="RNewPWD" CssClass="txt" runat="server" Columns="16" TextMode="Password"></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="NewPwd"
                            ControlToValidate="RNewPWD" ErrorMessage="两次密码不一致！" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td height="35">
                        <asp:LinkButton ID="lbtnPwd" runat="server" CssClass="btn btnNormal" OnClick="lbtnPwd_Click">保  存</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
       
        <table width="100%" border="0" class="sugges">
            <tr>
                <td class="sugtitle">
                    温馨提示：
                </td>
            </tr>
            <tr>
                <td class="sugcontent">
                    &nbsp;&nbsp;&nbsp;&nbsp; 访问密码长度6-18位。<br />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
