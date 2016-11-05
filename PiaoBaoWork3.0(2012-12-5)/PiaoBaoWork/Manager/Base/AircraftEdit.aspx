<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AircraftEdit.aspx.cs" Inherits="AircraftEdit" %>

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
        function showdialog3(t,p) {
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
                        if (p != null && p == "1") {
                            location.href = "AircraftList.aspx?currentuserid=" + $("#currentuserid").val();
                        } else {
                            $(this).dialog('close');
                        }
                    }
                }
            });
        }
    </script>
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../../css/table.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            color: Red;
        }
    </style>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="dd">
    </div>
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info" class="table_info">
                <tr>
                    <td colspan="4" class="bt">
                        机型基本信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>机型：
                    </td>
                    <td>
                        <asp:TextBox ID="txtJiXing" CssClass="txt" runat="server" Columns="16"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtJiXing"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    <td class="td">
                        <span class="style1">*</span>国内机建：
                    </td>
                    <td>
                        <asp:TextBox ID="txtJJN" CssClass="txt" runat="server" Columns="16" onkeyup="value=value.replace(/[^0-9\.]/ig,'');"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtJJN"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>国际机建：
                    </td>
                    <td>
                        <asp:TextBox ID="txtJJW" CssClass="txt" runat="server" Columns="16" onkeyup="value=value.replace(/[^0-9\.]/ig,'');"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtJJW"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btnNormal" OnClick="LinkButton1_Click">保  存 </asp:LinkButton>
                        &nbsp; &nbsp;&nbsp;&nbsp;<a href="AircraftList.aspx?currentuserid=<%=this.mUser.id.ToString() %>" class="btn btnNormal">返 回 </a>
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
                    1、机建必须为数字。
                    <br>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
