<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PolicyGuide.aspx.cs" Inherits="Policy_PolicyGuide" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
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
    <form id="form1" runat="server">
     <div id="dd">
    </div>
        <div class="mainPanel">
           <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

            <table id="table_info" class="table_info" width="100%" cellspacing="0" cellpadding="0"
                border="0">
                <tr class="leftliebiao_checi">
                    <td class="bt" colspan="3">
                        竞价平台政策排序
                    </td>
                </tr>
                <tr>
                <td width="15%">
                 <asp:ListBox ID="lboxPlat" runat="server" Width="100px" Height="110px" AutoPostBack="true">
                    </asp:ListBox>
                </td>
                <td width="10%">
                    <span class="btn btn-ok-s"><asp:Button ID="btUp" runat="server" Text="向上" 
                        onclick="btUp_Click" /></span>
                    <br />
                    <br />
                <span class="btn btn-ok-s"><asp:Button ID="btDown" runat="server" Text="向下" 
                        onclick="btDown_Click" /></span>
                </td>
                <td>
                <span class="btn btn-ok-s"><asp:Button ID="btSave" runat="server" Text="保存" 
                        onclick="btSave_Click"/></span>
                </td>
                </tr>
                
                </table>
                 </ContentTemplate>
            </asp:UpdatePanel>
                </div>
    </form>
</body>
</html>
