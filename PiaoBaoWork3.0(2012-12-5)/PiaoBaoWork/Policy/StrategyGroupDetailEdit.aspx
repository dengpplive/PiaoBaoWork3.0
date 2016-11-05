<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StrategyGroupDetailEdit.aspx.cs" Inherits="Policy_StrategyGroupDetailEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
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
                 else if (c1 == "txtPoint" || c1 == "txtMoney" || c1 == "txtPointScope1" || c1 == "txtPointScope2") {
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
             if ($("#ddlGroup").val() == "") {
                 bools++;
                 $("#spanGroup").html("不能为空！");
             }
             else {
                 $("#spanGroup").html("<b>*</b>");
             }

             if ($("#ddlbasetype").val() == "0") {
                 bools++;
                 $("#spanbasetype").html("必选！");
             }
             else if ($("#ddlbasetype").val() == "2") {
                 if ($("#ddljk").val() == "") {
                     bools++;
                     $("#spanddljk").html("接口必选！");
                 } else {
                     $("#spanddljk").html("<b>*</b>");
                 }
             }
             else {
                 $("#spanbasetype").html("<b>*</b>");
             }

             if ($("#txttxtStartTime").val() == "" || $("#txtEndTime").val() == "") {
                 bools++;
                 $("#spanTimeScope").html("不能为空！");
             }
             else {
                 $("#spanTimeScope").html("<b>*</b>");
             }


             if ($("#txtCarryCode").val() == "") {
                 bools++;
                 $("#spanCarryCode").html("不能为空！");
             }
             else {
                 $("#spanCarryCode").html("<b>*</b>");
             }

             if ($("#txtFromCityCode").val() == "") {
                 bools++;
                 $("#spanFromCityCode").html("不能为空！");
             }
             else {
                 $("#spanFromCityCode").html("<b>*</b>");
             }

             if ($("#txtToCityCode").val() == "") {
                 bools++;
                 $("#spanToCityCode").html("不能为空！");
             }
             else {
                 $("#spanToCityCode").html("<b>*</b>");
             }

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

             if ($("#txtPointScope1").val() == "" || $("#txtPointScope2").val() == "") {
                 bools++;
                 $("#spanPointScope").html("不能为空！");
             }
             else {
                 $("#spanPointScope").html("<b>*</b>");
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
            <span>扣点组扣点详情编辑</span>
        </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <div id="tabs" class="infomain">
        <div class="mainPanel">
         <div>
            <table width="100%" border="1" style="border-collapse: collapse;" cellpadding="0"
                cellspacing="0" id="table_info" class="table_info">
                <tr>
                <td class="td">
                        扣点组：
                    </td>
                    <td class="alignLeft">
                        <asp:DropDownList ID="ddlGroup" CssClass="txt" runat="server" AppendDataBoundItems="true">
                         <asp:ListItem Value="" Selected="True">--选择--</asp:ListItem>
                        </asp:DropDownList>
                        <span id="spanGroup" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                                政策类型：
                            </td>
                            <td class="alignLeft">
                                <asp:DropDownList ID="ddlbasetype" runat="server" CssClass="txt" AutoPostBack="true" 
                                    onselectedindexchanged="ddlbasetype_SelectedIndexChanged">
                                <asp:ListItem Value="0" Selected="True">--选择--</asp:ListItem>
                                <asp:ListItem Value="1">本地</asp:ListItem>
                                <asp:ListItem Value="2">接口</asp:ListItem>
                                <asp:ListItem Value="3">共享</asp:ListItem>
                                </asp:DropDownList><span id="spanbasetype" style="color: Red;"><b>*</b></span>
                                <span id="showjk" runat="server" visible="false">
                                &nbsp;&nbsp;&nbsp;接口:&nbsp; <asp:DropDownList ID="ddljk" CssClass="txt" runat="server" AppendDataBoundItems="true" >
                            <asp:ListItem Value="" Selected="True">--选择接口--</asp:ListItem>
                                </asp:DropDownList><span id="spanddljk" style="color: Red;"><b>*</b></span></span>
                            </td>
                   
                </tr>
                <tr>
                <td class="td">
                        航空公司代码：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtCarryCode" CssClass="txt" runat="server" onblur="showErr('txtCarryCode','spanCarryCode')"></asp:TextBox>
                        <span id="spanCarryCode" style="color: Red;"><b>*</b></span>
                    </td>
                     <td class="td">
                            出发城市代码：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtFromCityCode" CssClass="txt" runat="server" onfocus="showErr('txtCpyName','spanCpyName')" onblur="showErr('txtFromCityCode','spanFromCityCode')"></asp:TextBox>
                            <span id="spanFromCityCode" style="color: Red;"><b>*</b></span>
                        </td>
                </tr>
                <tr>
                <td class="td">
                        调整类型：
                    </td>
                    <td class="alignLeft">
                       <asp:RadioButtonList ID="rblSelectType" runat="server" 
                            RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Selected="True">扣点</asp:ListItem>
                             <asp:ListItem Value="2">留点</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                     <td class="td">
                            到达城市代码：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtToCityCode" CssClass="txt" runat="server" onblur="showErr('txtToCityCode','spanToCityCode')"></asp:TextBox>
                            <span id="spanToCityCode" style="color: Red;"><b>*</b></span>
                        </td>
                </tr>
                <tr>
                        <td class="td">
                           点数：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtPoint" CssClass="txt" runat="server"  onkeyup="value=value.replace(/[^0-9.]/g,'')" onblur="showErr('txtPoint','spanPoint')"></asp:TextBox>%
                            <span id="spanPoint" style="color: Red;"><b>*</b></span>
                        </td>
                        <td class="td">
                            现返：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtMoney" CssClass="txt" runat="server"  onkeyup="value=value.replace(/[^0-9.]/g,'')" onblur="showErr('txtMoney','spanMoney')"></asp:TextBox>元
                            <span id="spanMoney" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
                    <tr>
                      <td class="td">
                            返点范围：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtPointScope1" CssClass="txt" Width="80px" runat="server"  onkeyup="value=value.replace(/[^0-9.]/g,'')" onblur="showErr('txtPointScope1','spanPointScope')"></asp:TextBox>%
                            &nbsp;-<asp:TextBox ID="txtPointScope2" CssClass="txt" Width="80px" runat="server" onkeyup="value=value.replace(/[^0-9.]/g,'')"  onblur="showErr('txtPointScope2','spanPointScope')"></asp:TextBox>%
                            <span id="spanPointScope" style="color: Red;"><b>*</b></span>
                        </td>
                        <td class="td">
                            时间段：
                        </td>
                        <td class="alignLeft">
                          <asp:TextBox ID="txtStartTime"  CssClass="Wdate inputtxtdat"  runat="server" class="Wdate" EnableViewState="False"
                                    onfocus="WdatePicker({isShowWeek:false})"></asp:TextBox>
                                    -<asp:TextBox ID="txtEndTime" CssClass="Wdate inputtxtdat"  runat="server" class="Wdate" EnableViewState="False"
                                    onfocus="WdatePicker({isShowWeek:false})"></asp:TextBox>
                            <span id="spanTimeScope" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="lbsave" runat="server" Text="保 存" onclick="lbsave_Click" />
                        </span>&nbsp; &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                            <asp:Button ID="btnBack" runat="server" Text="返 回"/>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
        </div>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
