<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PolicyShareTakeoffEdit.aspx.cs" Inherits="Policy_PolicyShareTakeoffEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../js/area.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <script src="../js/Validation.js" type="text/javascript"></script>
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
        });
        $("#txtPolicyMoney").keyup(function () {
            if (!zz1.test($("#txtPolicyMoney").val())) {
                $("#txtPolicyMoney").val("");
            }
        });
        $("#txtPbPoint").keyup(function () {
            if (!zz1.test($("#txtPbPoint").val())) {
                $("#txtPbPoint").val("");
            }
        });
        $("#txtPbMoney").keyup(function () {
            if (!zz1.test($("#txtPbMoney").val())) {
                $("#txtPbMoney").val("");
            }
        })
    })
    function showErr(c1, c2) {

        Pause(this, 500);
        this.NextStep = function () {
            if ($("#" + c1).val() == "") {
                $("#" + c2).html("不能为空！");
            }
            else if (c1 == "txtPolicyPoint" || c1 == "txtPolicyMoney" || c1 == "txtPbPoint" || c1 == "txtPbMoney") {
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

        if ($("#ddljk").val() == "0") {
            bools++;
            $("#spanddljk").html("接口必选！");
        }
        else {
            $("#spanddljk").html("<b>*</b>");
        }
        if ($("#hidroletype").val() == "平台") {
            if ($("#txtPbPoint").val() == "") {
                bools++;
                $("#spanPbPoint").html("不能为空！");
            }
            else if (parseFloat($("#txtPbPoint").val()) > 100) {
                bools++;
                $("#spanPbPoint").html("扣点不能大于100%！");
            }
            else {
                $("#spanPbPoint").html("<b>*</b>");
            }
            if ($("#txtPbMoney").val() == "") {
                bools++;
                $("#spanPbMoney").html("不能为空！");
            }
            else {
                $("#spanPbMoney").html("<b>*</b>");
            }
            if ($("#txtCpyName").val() == "") {
                bools++;
                $("#spanCpyName").html("不能为空！");
            }
            else {
                $("#spanCpyName").html("<b>*</b>");
            }
        }
        else 
        {
            if ($("#txtPolicyPoint").val() == "" ) {
                bools++;
                $("#spanPolicyPoint").html("不能为空！");
            }
            else if (parseFloat($("#txtPolicyPoint").val())>100) {
                bools++;
                $("#spanPolicyPoint").html("扣点不能大于100%！");
            }
            else {
                $("#spanPolicyPoint").html("<b>*</b>");
            }

            if ($("#txtPolicyMoney").val() == "") {
                bools++;
                $("#spanPolicyMoney").html("不能为空！");
            }
            else {
                $("#spanPolicyMoney").html("<b>*</b>");
            }
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
        <span>共享接口扣点设置</span>
    </div>
    <div id="tabs" class="infomain">
        <div class="mainPanel">
            <div>
                <table width="100%" border="1" style="border-collapse: collapse;" cellpadding="0"
                    cellspacing="0" id="table_info" class="table_info">
                    <tr>
                        <div id="showcpyname" runat="server" visible="false">
                        <td class="td">
                            公司名称：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtCpyName" ReadOnly="true" CssClass="txt" runat="server" TextMode="MultiLine"></asp:TextBox>
                            <span id="spanCpyName" style="color: Red;"><b>*</b></span>
                                <span class="btn btn-ok-s" runat="server" id="showbt" style="text-align:center">
                                    <asp:Button ID="btselect" runat="server" Text="选择公司" PostBackUrl="~/Policy/CpyList.aspx?policytype=kd&currentuserid=<%=this.mUser.id.ToString() %>" />
                                </span>
                        </td>
                        </div>
                        <td class="td">
                            接口：
                        </td>
                        <td class="alignLeft">
                            <asp:DropDownList ID="ddljk" CssClass="txt" runat="server" AppendDataBoundItems="true" >
                            <asp:ListItem Value="0" Selected="True">--选择接口--</asp:ListItem>
                                </asp:DropDownList>
                            <span id="spanddljk" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
                    <tr runat="server" id="showpolicy" visible="false">
                        <td class="td">
                            扣点：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtPolicyPoint" CssClass="txt" runat="server" onfocus="showErr('ddljk','spanddljk')" onblur="showErr('txtPolicyPoint','spanPolicyPoint')" MaxLength=3></asp:TextBox>%
                            <span id="spanPolicyPoint" style="color: Red;"><b>*</b></span>
                        </td>
                        <td class="td">
                            现返：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtPolicyMoney" CssClass="txt" runat="server" onblur="showErr('txtPolicyMoney','spanPolicyMoney')"></asp:TextBox>
                            <span id="spanPolicyMoney" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
                    <tr runat="server" id="showpb" visible="false">
                        <td class="td">
                            票宝扣点：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtPbPoint" CssClass="txt" runat="server" onblur="showErr('txtPbPoint','spanPbPoint')" MaxLength=3></asp:TextBox>%
                            <span id="spanPbPoint" style="color: Red;"><b>*</b></span>
                        </td>
                        <td class="td">
                            票宝现返：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtPbMoney" CssClass="txt" runat="server" onblur="showErr('txtPbMoney','spanPbMoney')"></asp:TextBox>
                            <span id="spanPbMoney" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                    <tr>
                        <td height="35" align="center" class="btni">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="lbsave" runat="server" Text="保 存" OnClick="lbsave_Click" />
                            </span>&nbsp; &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                                <asp:Button ID="btnBack" runat="server" Text="返 回"/>
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hidroletype" runat="server" />
    </form>
</body>
</html>
