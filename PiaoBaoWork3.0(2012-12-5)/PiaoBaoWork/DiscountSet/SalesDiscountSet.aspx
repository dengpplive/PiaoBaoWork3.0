<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SalesDiscountSet.aspx.cs" Inherits="DiscountSet_SalesDiscountSet" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
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
         function Pause(obj, iMinSecond) {
             if (window.eventList == null) window.eventList = new Array();
             var ind = -1;
             for (var i = 0; i < window.eventList.length; i++) {
                 if (window.eventList[i] == null) {
                     window.eventList[i] = obj;
                     ind = i;
                     break;
                 }
             }
             if (ind == -1) {
                 ind = window.eventList.length;
                 window.eventList[ind] = obj;
             }
             setTimeout("GoOn(" + ind + ")", iMinSecond);
         }

         //js继续函数    
         function GoOn(ind) {
             var obj = window.eventList[ind];
             window.eventList[ind] = null;
             if (obj.NextStep) obj.NextStep();
             else obj();
         }
         function showErr(c1, c2) {
             Pause(this, 500);
             this.NextStep = function () {
                 if ($("#" + c1).val() == "") {
                     $("#" + c2).html("不能为空！");
                 }
                 else if (c1 == "txtPoint" || c1 == "txtMoney") {
                     if (isNaN($("#" + c1).val())) {
                         $("#" + c2).html("只能为数字！");
                     }
                     else {
                         $("#" + c2).html("<b>*</b>");
                     }
                 }
                 else {
                     $("#" + c2).html("<b>*</b>");
                 }
             }
         }
         function showAllErr() {
             var bools = 0;
             if ($("#txtPoint").val() == "") {
                 bools++;
                 $("#spanPoint").html("不能为空！");
             }
             else {
                 $("#spanPoint").html("<b>*</b>");
             }

             if ($("#txtMoney").val() == "") {
                 bools++;
                 $("#spanMoney").html("不能为空！");
             }
             else {
                 $("#spanMoney").html("<b>*</b>");
             }
             if (bools > 0) {
                 return false;
             }
             else {
                 return true;
             }
         }
         </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div class="title">
            <span>分销扣点设置</span>
        </div>
    <div id="tabs" class="infomain">
        <div class="mainPanel">
         <div>
            <table width="100%" border="1" style="border-collapse: collapse;" cellpadding="0"
                cellspacing="0" id="table_info" class="table_info">
                <tr>
                 <td class="td">
                            调整类型：
                        </td>
                        <td>
                            <asp:RadioButtonList ID="rblSelectType" runat="server" RepeatColumns="3">
                                <asp:ListItem Value="1" Text="扣点" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="2" Text="留点"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                </tr>
                  
                <tr>
                        <td class="td">
                           扣点：
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtPoint" CssClass="txt" runat="server" MaxLength='9'  onkeyup="value=value.replace(/[^0-9.]/g,'')" onblur="showErr('txtPoint','spanPoint')"></asp:TextBox>%
                            <span id="spanPoint" style="color: Red;"><b>*</b></span>
                        </td>
                        <td class="td">
                            现返：
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtMoney" CssClass="txt" runat="server" MaxLength='9'  onkeyup="value=value.replace(/[^0-9.]/g,'')" onblur="showErr('txtMoney','spanMoney')"></asp:TextBox>元
                            <span id="spanMoney" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="lbsave" runat="server" Text="保  存" onclick="lbsave_Click" OnClientClick="return showAllErr()" />
                        </span>
                    </td>
                </tr>
            </table>
        </div>
        </div>
    </div>
    </form>
</body>
</html>
