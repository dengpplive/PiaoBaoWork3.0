<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AirlineCompanyEdit.aspx.cs"
    Inherits="Sys_AirlineCompanyEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../../js/area.js" type="text/javascript"></script>
    <link href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
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
                    },
                    "取消": function () {
                        $(this).dialog('close');
                    }
                }
            });
        }
        function showdialog3(t) {
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
                        location.href = "AirlineCompanyList.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
                    }
                }
            });
        }
    </script>
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../../css/table.css" rel="stylesheet" type="text/css" />
    <title></title>
    <style type="text/css">
        .style1
        {
            color: Red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info" class="table_info">
                <tr>
                    <td colspan="4" class="bt">
                        承运人基本信息
                    </td>
                </tr>
                <tr>
                    <th class="td">
                        <span class="style1">*</span>承运人全称：
                    </th>
                    <td width="20%">
                        <asp:TextBox ID="txtAriName" CssClass="txt" runat="server" Columns="16" MaxLength="10"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAriName"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    <th class="td">
                        <span class="style1">*</span>承运人简称：
                    </th>
                    <td>
                        <asp:TextBox ID="txtShortName" CssClass="txt" runat="server" Columns="16" MaxLength="2"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtShortName"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <th class="td">
                        <span class="style1">*</span>二字码：
                    </th>
                    <td>
                        <asp:TextBox ID="txtCode" CssClass="txt" runat="server" Columns="16" MaxLength="2" onkeyup="value=value.replace(/[^a-zA-Z0-9]/g,'')"
                            onpaste="return false"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtCode"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    <th class="td">
                        <span class="style1">*</span>国内国际：
                    </th>
                    <td>
                        <asp:RadioButtonList ID="rblGNGJ" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Selected>国内</asp:ListItem>
                            <asp:ListItem Value="0">国际</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                    </td>
                    <th align="right" class="td">
                        <span class="style1">*</span>是否销售：
                    </th>
                    <td>
                        <asp:RadioButtonList ID="radioXS" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0" Selected="True">不销售</asp:ListItem>
                            <asp:ListItem Value="1">销售</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btnNormal" OnClick="LinkButton1_Click">保  存 </asp:LinkButton>
                        &nbsp; &nbsp;&nbsp;&nbsp;<a href="AirlineCompanyList.aspx?currentuserid=<%=this.mUser.id.ToString() %>" class="btn btnNormal">返 回
                        </a>
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
                    1、承运人二字码为大写字母。
                    <br>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
