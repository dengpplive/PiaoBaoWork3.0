<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FaresEdit.aspx.cs" Inherits="Sys_FaresEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>票价编辑</title>
    <script src="../../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../../js/area.js" type="text/javascript"></script>
    <link href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../../css/table.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #table_info .td
        {
            width: 120px;
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
                        location.href="FaresList.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
                    }
                }
            });
        }
        function showErr(c1,c2) {
            if($("#"+c1).val()=="") {
                $("#"+c2).html("不能为空！");
            }
            else if(c1=="txtFareFee") {
                var zz=/^\d+(\.\d{2})?$/
                if(!zz.test($("#txtFareFee").val())) {
                    $("#spanFareFee").html("票价格式错误！");
                }
                else {
                    $("#spanFareFee").html("<b>*</b>");
                }
            }
            else if(c1=="txtMileage") {
                var zz=/^\d*$/
                if(!zz.test($("#spanMileage").val())) {
                    $("#spanMileage").html("里程格式错误！");
                }
                else {
                    $("#spanMileage").html("<b>*</b>");
                }
            }
            else {
                $("#"+c2).html("<b>*</b>");
            }
        }
        function showAllErr() {
            var bools=0;
            if($("#txtFromCityName").val()=="") {
                bools++;
                $("#spanFromCityName").html("不能为空！");
            }
            else {
                $("#spanFromCityName").html("<b>*</b>");
            }

            if($("#txtFromCityCode").val()=="") {
                bools++;
                $("#spanFromCityCode").html("不能为空！");
            }
            else {
                $("#spanFromCityCode").html("<b>*</b>");
            }

            if($("#txtToCityName").val()=="") {
                bools++;
                $("#spanToCityName").html("不能为空！");
            }
            else {
                $("#spanToCityName").html("<b>*</b>");
            }

            if($("#txtToCityCode").val()=="") {
                bools++;
                $("#spanToCityCode").html("不能为空！");
            }
            else {
                $("#spanToCityCode").html("<b>*</b>");
            }

            if($("#txtFareFee").val()=="") {
                bools++;
                $("#spanFareFee").html("不能为空！");
            }
            else {
                $("#spanFareFee").html("<b>*</b>");
            }

            if($("#txtMileage").val()=="") {
                bools++;
                $("#spanMileage").html("不能为空！");
            }
            else {
                $("#spanMileage").html("<b>*</b>");
            }


            if($("#txtEffectTime").val()=="") {
                bools++;
                $("#spanEffectTime").html("不能为空！");
            }
            else {
                $("#spanEffectTime").html("<b>*</b>");
            }

            if($("#txtInvalidTime").val()=="") {
                bools++;
                $("#spanInvalidTime").html("不能为空！");
            }
            else {
                $("#spanInvalidTime").html("<b>*</b>");
            }
            if(bools>0) {
                return false;
            }
            else {
                return validate();
            }
        }
        //日期大小比较 大于返回true 小于false
        function CompareDate(sdate,edate) {
            var strDate1=sdate.split('-');
            var date1;
            if(strDate1.length==3) {
                date1=new Date(strDate1[0],strDate1[1],strDate1[2]);
            }
            var strDate2=edate.split('-');
            var date2;
            if(strDate1.length==3) {
                date2=new Date(strDate2[0],strDate2[1],strDate2[2]);
            }
            return (date1>date2);
        }
        function validate() {            
            var reflag=false;
            var msg="";
            var d1=CompareDate($("#txtEffectTime").val(),$("#txtInvalidTime").val());
            if(d1) {
                msg="生效日期不能大于失效日期";
            }
            var Mileage=$.trim($("#txtMileage").val());
            var FareFee=$.trim($("#txtFareFee").val());
            var farereg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
            var MileageReg=/^\d{1,4}$/;
            if(Mileage==""||!MileageReg.test(Mileage)) {
                msg="输入里程格式错误";
            }
            if(FareFee==""||!farereg.test(FareFee)) {
                msg="输入票价格式错误";
            }
            if(msg!="") {
                showdialog(msg);
            } else {
                reflag=true;
            }
            return reflag;
        }
    </script>
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
                        票价基本信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        出发城市：
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromCityName" runat="server" CssClass="txt" onblur="showErr('txtFromCityName','spanFromCityName')"></asp:TextBox>
                        <span id="spanFromCityName" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        出发城市三字码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromCityCode" runat="server" CssClass="txt" onblur="showErr('txtFromCityCode','spanFromCityCode')"></asp:TextBox>
                        <span id="spanFromCityCode" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        到达城市：
                    </td>
                    <td>
                        <asp:TextBox ID="txtToCityName" runat="server" CssClass="txt" onblur="showErr('txtToCityName','spanToCityName')"></asp:TextBox>
                        <span id="spanToCityName" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        到达城市三字码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtToCityCode" runat="server" CssClass="txt" onblur="showErr('txtToCityCode','spanToCityCode')"></asp:TextBox>
                        <span id="spanToCityCode" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        票价：
                    </td>
                    <td>
                        <asp:TextBox ID="txtFareFee" runat="server" CssClass="txt" onblur="showErr('txtFareFee','spanFareFee')"></asp:TextBox>
                        <span id="spanFareFee" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        里程：
                    </td>
                    <td>
                        <asp:TextBox ID="txtMileage" runat="server" CssClass="txt" onblur="showErr('txtMileage','spanMileage')"></asp:TextBox>
                        <span id="spanMileage" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        航空公司二字码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtCarryCode" CssClass="txt" MaxLength="2" runat="server"></asp:TextBox>
                    </td>
                    <td class="td">
                        国际/国内：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlIsDomestic" CssClass="txt" runat="server" Width="152px">
                            <asp:ListItem Selected="True" Text="国内" Value="1"></asp:ListItem>
                            <asp:ListItem Text="国际" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        生效日期：
                    </td>
                    <td>
                        <asp:TextBox ID="txtEffectTime" runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                            CssClass="Wdate txt"></asp:TextBox>
                        <span id="spanEffectTime" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        失效日期：
                    </td>
                    <td>
                        <asp:TextBox ID="txtInvalidTime" runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                            CssClass="Wdate txt"></asp:TextBox>
                        <span id="spanInvalidTime" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btnNormal" OnClick="LinkButton1_Click"
                            OnClientClick="return showAllErr();">保  存</asp:LinkButton>
                        &nbsp; &nbsp;&nbsp;&nbsp;<a href="FaresList.aspx?currentuserid=<%=this.mUser.id.ToString() %>" class="btn btnNormal">返 回</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
