<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CabinDiscountEdit.aspx.cs"
    Inherits="Sys_CabinDiscountEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>舱位编辑</title>
    <script src="../../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <link href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
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
                        location.href = "CabinDiscountList.aspx?currentuserid=" + $("#currentuserid").val();
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


//        function showErr(c1, c2) {

//            Pause(this, 500);
//            this.NextStep = function () {
//                if ($("#" + c1).val() == "") {
//                    $("#" + c2).html("不能为空！");
//                }
//                else if (c1 == "txtDiscountRate") {
//                    var zz = /^(0|[1-9]\d*)|(0|[1-9]\d*)\.(\d+)$/
//                    if (!zz.test($("#" + c1).val())) {
//                        $("#" + c2).html("折扣只能为数字！");
//                    }
//                    else {
//                        $("#" + c2).html("<b>*</b>");
//                    }
//                }
//                else {
//                    $("#" + c2).html("<b>*</b>");
//                }
//            }
        //        }
        function showErr(c1, c2) {

            Pause(this, 500);
            this.NextStep = function () {
                if ($("#" + c1).val() == "") {
                    $("#" + c2).html("不能为空！");
                }
                else if (c1 == "txtDiscountRate") {
                    var zz = /^(0|[1-9]\d*)|(0|[1-9]\d*)\.(\d+)$/
                    if (isNaN($("#" + c1).val())) {
                        $("#" + c2).html("折扣只能为数字！");
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
            if ($("#txtCabin").val() == "") {
                bools++;
                $("#spanCabin").html("不能为空！");
            }
            else {
                $("#spanCabin").html("<b>*</b>");
            }

            if ($("#txtDiscountRate").val() == "") {
                bools++;
                $("#spanDiscountRate").html("不能为空！");
            }
            else {
                $("#spanDiscountRate").html("<b>*</b>");
            }

            if ($("#txtBeginTime").val() == "") {
                bools++;
                $("#spanBeginTime").html("不能为空！");
            }
            else {
                $("#spanBeginTime").html("<b>*</b>");
            }

            if ($("#txtEndTime").val() == "") {
                bools++;
                $("#spanEndTime").html("不能为空！");
            }
            else {
                $("#spanEndTime").html("<b>*</b>");
            }

            if ($("#txtAirPortCode").val() == "") {
                bools++;
                $("#spanAirPortCode").html("不能为空！");
            }
            else {
                $("#spanAirPortCode").html("<b>*</b>");
            }

            if ($("#txtAirPortName").val() == "") {
                bools++;
                $("#spanAirPortName").html("不能为空！");
            }
            else {
                $("#spanAirPortName").html("<b>*</b>");
            }
            if (bools > 0) {
                return false;
            }
            else {
                return true;
            }
        }

        function CompareDate(sdate, edate) {
            var strDate1 = sdate.split('-');
            var date1;
            if (strDate1.length == 3) {
                date1 = new Date(strDate1[0], strDate1[1], strDate1[2]);
            }
            var strDate2 = edate.split('-');
            var date2;
            if (strDate1.length == 3) {
                date2 = new Date(strDate2[0], strDate2[1], strDate2[2]);
            }
            return (date1 > date2);
        }

        function StartTime(Val) {
            var endtime = $("#txtEndTime").val();
            var spanBeginTime = $("#spanBeginTime");
            var spanEndTime = $("#spanEndTime");
            if (endtime != "" && Val != "") {
                var flag = CompareDate(endtime, Val);
                if (!flag) {
                    spanBeginTime.html("开始日期不能大于结束日期");
                    spanEndTime.html("");
                    return;
                }
            }
            $("#txtBeginTime").val(Val);
        }
        function EndTime(Val) {
            var starttime = $("#txtBeginTime").val();
            var spanEndTime = $("#spanEndTime");

            if (starttime != "" && Val != "") {
                var flag = CompareDate(Val, starttime);
                if (!flag) {
                    spanEndTime.html("开始日期不能大于结束日期");
                    $("#spanBeginTime").html("");
                    return;
                }
            }
            $("#txtEndTime").val(Val);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="dd">
    </div>
    <div class="Panelbg">
        <div>
            <table width="100%" border="1" style="border-collapse: collapse;" cellpadding="0"
                cellspacing="0" id="table_info" class="table_info">
                <tr>
                    <td colspan="4" style="font-size: 12px;">
                        舱位基本信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        舱位：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtCabin" CssClass="txt" runat="server" onblur="showErr('txtCabin','spanCabin')"></asp:TextBox>
                        <span id="spanCabin" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        舱位价：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtDiscountRate" CssClass="txt" runat="server" onblur="showErr('txtDiscountRate','spanDiscountRate')"></asp:TextBox>
                        <span id="spanDiscountRate" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        承运人代码：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtAirPortCode" CssClass="txt" runat="server" onblur="showErr('txtAirPortCode','spanAirPortCode')"></asp:TextBox>
                        <span id="spanAirPortCode" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        承运人名称：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtAirPortName" CssClass="txt" runat="server" onblur="showErr('txtAirPortName','spanAirPortName')"></asp:TextBox>
                        <span id="spanAirPortName" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        出发城市三字码：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtFromCityCode" CssClass="txt" runat="server"></asp:TextBox><span
                            id="span4" style="color: Red;"><b>&nbsp;</b></span>
                    </td>
                    <td class="td">
                        &nbsp;到达城市三字码：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtToCityCode" CssClass="txt" runat="server"></asp:TextBox><span
                            id="span3" style="color: Red;"><b>&nbsp;</b></span>
                    </td>
                </tr>
                <tr style="display: none;">
                    <td class="td">
                        出发城市名称：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtFormCity" CssClass="txt" runat="server"></asp:TextBox><span id="span2"
                            style="color: Red;"><b>&nbsp;</b></span>
                    </td>
                    <td class="td">
                        到达城市名称：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="txtToCity" CssClass="txt" runat="server"></asp:TextBox>
                        <span id="span1" style="color: Red;"><b>&nbsp;</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        起始时间：
                    </td>
                    <td class="alignLeft">
                        <input type="text" id="txtBeginTime" style="width: 150px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd',onpicked:function() {StartTime($dp.cal.getP('y')+'-'+$dp.cal.getP('M')+'-'+$dp.cal.getP('d'));}})" />
                        <span id="spanBeginTime" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        终止时间：
                    </td>
                    <td class="alignLeft">
                        <input type="text" id="txtEndTime" style="width: 150px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd',onpicked:function() {EndTime($dp.cal.getP('y')+'-'+$dp.cal.getP('M')+'-'+$dp.cal.getP('d'));}})" />
                        <span id="spanEndTime" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        国际/国内：
                    </td>
                    <td class="alignLeft">
                        <asp:DropDownList ID="ddlIsGN" CssClass="txt" runat="server" Width="150px">
                            <asp:ListItem Selected="True" Text="国内" Value="0"></asp:ListItem>
                            <asp:ListItem Text="国际" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="td">
                    </td>
                    <td>
                    </td>
                </tr>
                <%--<tr>
                <td colspan="4" class="bt">
						详细信息
					</td>
            </tr>
            <tr>
                <td class="td">退票规定：</td>
                <td colspan="3"><asp:TextBox ID="txtDishonoredBillPrescript"  CssClass="txt" runat="server" 
                        TextMode="MultiLine" Width="500px"></asp:TextBox></td>
            </tr>
            <tr>
                <td  class="td">改签规定：</td>
                <td colspan="3"><asp:TextBox ID="txtLogChangePrescript"  CssClass="txt" runat="server" 
                        TextMode="MultiLine" Width="500px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="td">升舱规定：</td>
                <td colspan="3"><asp:TextBox ID="txtUpCabinPrescript" CssClass="txt" runat="server" 
                        TextMode="MultiLine" Width="500px"></asp:TextBox></td>
            </tr>--%>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" Text="保  存" />
                        </span>&nbsp; &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                            <asp:Button ID="btnBack" runat="server" Text="返 回" OnClick="btnBack_Click" />
                        </span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
