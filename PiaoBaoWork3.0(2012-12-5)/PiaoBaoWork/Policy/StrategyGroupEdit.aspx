<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StrategyGroupEdit.aspx.cs" Inherits="Policy_StrategyGroupEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
   <link href="../css/table.css" rel="stylesheet" type="text/css" />
     <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
     <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        #table_info .td
        {
            width: 120px;
            text-align: right;
            font-size: 12px;
        }
        #table_info span
        {
            font-size: 12px;
        }
        .alignLeft
        {
            text-align: left;
        }
        .Panelbg
        {
            background-color: White;
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
         $(document).ready(function () {
             $('#tabs').tabs();
             var zz1 = /^(0|[1-9]\d*)|(0|[1-9]\d*)\.(\d+)$/
             $("#txtPolicyPoint").keyup(function () {
                 if (!zz1.test($("#txtPolicyPoint").val())) {
                     $("#txtPolicyPoint").val("");
                 }
             })
         })
         function showErr(c1, c2) {

             Pause(this, 500);
             this.NextStep = function () {
                 if ($("#" + c1).val() == "") {
                     $("#" + c2).html("不能为空！");
                 }
                 else if (c1 == "txtUnitePoint") {
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
             if ($("#txtGroupName").val() == "") {
                 bools++;
                 $("#spanGroupName").html("不能为空！");
             }
             else {
                 $("#spanGroupName").html("<b>*</b>");
             }

             if ($("#txtUnitePoint").val() == "") {
                 bools++;
                 $("#spanUnitePoint").html("不能为空！");
             }
             else {
                 $("#spanUnitePoint").html("<b>*</b>");
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
            <span>扣点组编辑</span>
        </div>
    <div id="tabs" class="infomain">
        <div class="mainPanel">
         <div>
            <table width="100%" border="1" style="border-collapse: collapse;" cellpadding="0"
                cellspacing="0" id="table_info" class="table_info">
               <tr>
                    <td class="td">
                        扣点组名称：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtGroupName" CssClass="txt" runat="server" onblur="showErr('txtGroupName','spanGroupName')"></asp:TextBox>
                        <span id="spanGroupName" style="color: Red;"><b>*</b></span>
                    </td>
                   <td class="td">
                        是否为默认：
                    </td>
                    <td class="alignLeft">
                        <asp:RadioButtonList ID="rblDefaultFlag" runat="server" 
                            RepeatDirection="Horizontal">
                            <asp:ListItem Value="false" Selected="True">否</asp:ListItem>
                             <asp:ListItem Value="true">是</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                  <td class="td">
                        是否协调：
                    </td>
                    <td class="alignLeft">
                            <asp:RadioButtonList ID="rblUniteFlag" runat="server" 
                            RepeatDirection="Horizontal">
                            <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                             <asp:ListItem Value="1">是</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td class="td">
                        协调点数：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtUnitePoint" CssClass="txt" runat="server" onkeyup="value=value.replace(/[^0-9.]/g,'')" onblur="showErr('txtUnitePoint','spanUnitePoint')"></asp:TextBox>
                        <span id="spanUnitePoint" style="color: Red;"><b>*</b></span>
                    </td>
                 
                </tr>
               
               
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="lbsave" runat="server" Text="保  存" onclick="lbsave_Click" />
                        </span>&nbsp; &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                            <asp:Button ID="btnBack" runat="server" Text="返 回"/>
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
