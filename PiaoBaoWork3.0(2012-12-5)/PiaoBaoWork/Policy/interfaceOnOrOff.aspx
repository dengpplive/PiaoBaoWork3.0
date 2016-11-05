<%@ Page Language="C#" AutoEventWireup="true" CodeFile="interfaceOnOrOff.aspx.cs" Inherits="Policy_interfaceOnOrOff" %>
<%@ Register Src="../UserContrl/ImportanterGongying.ascx" TagName="ImportanterGongying" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>接口开关控制</title>
      <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
      <script type="text/javascript">
        function showdialogmsg(t) {
            $("#show").html(t);
            $("#show").dialog({
                title: '提示',
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
    <div class="infomain">
        <div class="mainPanel">
            <table id="table_info" class="table_info" width="100%" cellspacing="0" cellpadding="0"
                border="0">
                <tr>
                    <td colspan="6" class="bt">
                        接口开关控制
                    </td>
                </tr>
                 <tr>
                    <td colspan="6">
                        <uc1:ImportanterGongying ID="ImportanterGongying" runat="server"/>
                    </td>
                </tr>
    </table>
     <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btnNormal" OnClick="lbtnOk_Click"
                            >保  存</asp:LinkButton>
                    </td>
                </tr>
            </table>
    </div>
     </div>
       <div id="show">
    </div>
    </form>
</body>
</html>
