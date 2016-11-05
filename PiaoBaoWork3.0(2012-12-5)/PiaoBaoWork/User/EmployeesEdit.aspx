<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EmployeesEdit.aspx.cs" Inherits="User_EmployeesEdit" EnableEventValidation="false"%>
<%@ Register src="../UserContrl/UserPowerControl.ascx" tagname="UserPowerControl" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>个人信息维护</title>
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
        function showErr(c1, c2) {
            if ($("#" + c1).val() == "") {
                $("#" + c2).html("不能为空！");
            }
            else if (c1 == "txtQQ") {
                var zz = /^\d*$/
                if (!zz.test($("#" + c1).val())) {
                    $("#" + c2).html("QQ只能为数字！");
                }
                else {
                    $("#" + c2).html("<b>*</b>");
                }
            }
            else if (c1 == "txtEmail") {
                var zz = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/
                if (!zz.test($("#" + c1).val())) {
                    $("#" + c2).html("邮箱格式错误！");
                }
                else {
                    $("#" + c2).html("<b>*</b>");
                }
            }
            else if (c1 == "txtPhone") {
                var zz = /^\d{11}$/
                if (!zz.test($("#" + c1).val())) {
                    $("#" + c2).html("手机号码为11位数字！");
                }
                else {
                    $("#" + c2).html("");
                }
            }
            else {
                $("#" + c2).html("");
            }
        }
        function showAllErr() {
            var bools = 0;
            var yzqq = /^\d*$/;
            var yzemail = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
            var yzphone = /^\d{11}$/;
            if ($("#txtQQ").val() == "") {
                $("#spanQQ").html("不能为空！");
                bools++;
            }
            else if (!yzqq.test($("#txtQQ").val())) {
                $("#spanQQ").html("QQ只能为数字！");
                bools++;
            }
            else {
                $("#spanQQ").html("");
            }
            if ($("#txtMSN").val() == "") {
                $("#spanMSN").html("不能为空！");
                bools++;
            }
            else {
                $("#spanMSN").html("");
            }
            if ($("#txtEmail").val() == "") {
                $("#spanEmail").html("不能为空！");
                bools++;
            }
            else if (!yzemail.test($("#txtEmail").val())) {
                $("#spanEmail").html("邮箱格式错误！");
                bools++;
            }
            else {
                $("#spanEmail").html("");
            }
            if ($("#txtTel").val() == "") {
                $("#spanTel").html("不能为空！");
                bools++;
            }
            else {
                $("#spanTel").html("");
            }
            if ($("#txtPhone").val() == "") {
                $("#spanPhone").html("不能为空！");
                bools++;
            } else if (!yzphone.test($("#txtPhone").val())) {
                $("#spanPhone").html("手机号码为11位数字！");
                bools++;
            }
            else {
                $("#spanPhone").html("");
            }
            if ($("#txtPostalCode").val() == "") {
                $("#spanPostalCode").html("不能为空！");
                bools++;
            }
            else {
                $("#spanPostalCode").html("");
            }
            if ($("#txtAdress").val() == "") {
                $("#spanAdress").html("不能为空！");
                bools++;
            }
            else {
                $("#spanAdress").html("");
            }
            if (bools > 0) {
                return false;
            }
            else {
                return true;
            }
        }
        function addTR(i) {
            //debugger;
            document.getElementById("hidtxtCount").value = parseInt(document.getElementById("hidtxtCount").value) + 1;
            //隐藏当前tr中的添加，删除按钮
            if (i < 4) {
                document.getElementById("sAdd" + i).style.display = "none";
            }
            if (i > 0) {
                document.getElementById("sDelete" + i).style.display = "none";
            }
            //显示下一个tr
            //document.getElementById("tr" + (i + 1)).style.display = "block";
            $("#tr" + (i + 1)).removeClass("hide");
        }

        //删除一个
        function delTR(i) {
            document.getElementById("hidtxtCount").value = parseInt(document.getElementById("hidtxtCount").value) - 1;
            //显示上一个tr中的添加，删除按钮
            document.getElementById("sAdd" + (i - 1)).style.display = "block";
            if (i > 1) {
                document.getElementById("sDelete" + (i - 1)).style.display = "block";
            }
            //隐藏当前tr
            //document.getElementById("tr" + i).style.display = "none";
            $("#tr" + i).addClass("hide");
        }
        function cleartxt() {
            for (var i = 0; i < 5; i++) {
                document.getElementById("txtA"+i).value = "";
            }
            document.getElementById("hidtxtCount").value = 1;
        }
    </script>
    <style type="text/css">
    .txtip
    {
         width: 200px;
        }
         .hide
        {
            display: none;
        }
    </style>
</head>
<body>
    <form id="form2" runat="server">
    <div class="infomain">
        <div class="mainPanel">
            <table id="table_info" class="table_info" width="100%" cellspacing="0" cellpadding="0"
                border="0">
                <tr>
                    <td colspan="6" class="bt">
                        个人信息维护
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        帐号：
                    </td>
                    <td>
                        <asp:TextBox ID="txtLoginName" class="txt" Enabled="false" runat="server"></asp:TextBox>
                    </td>
                    <td class="td">
                        工号：
                    </td>
                    <td>
                        <asp:TextBox ID="txtWorkNum" class="txt" Enabled="false" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        姓名：
                    </td>
                    <td>
                        <asp:TextBox ID="txtUserName" class="txt" Enabled="false" runat="server"></asp:TextBox>
                    </td>
                    <td class="td">
                        性别：
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblType" runat="server" RepeatColumns="3">
                            <asp:ListItem Text="男" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="女" Value="0"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        QQ：
                    </td>
                    <td>
                        <asp:TextBox ID="txtQQ" class="txt" runat="server" MaxLength="11" onblur="showErr('txtQQ','spanQQ')"></asp:TextBox><span
                            id="spanQQ" style="color: Red;"></span>
                    </td>
                    <td class="td">
                        MSN：
                    </td>
                    <td>
                        <asp:TextBox ID="txtMSN" class="txt" runat="server" MaxLength="50" onblur="showErr('txtMSN','spanMSN')"></asp:TextBox><span
                            id="spanMSN" style="color: Red;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        联系电话：
                    </td>
                    <td>
                        <asp:TextBox ID="txtTel" class="txt" runat="server" MaxLength="13" onblur="showErr('txtTel','spanTel')"></asp:TextBox><span
                            id="spanTel" style="color: Red;"></span>
                    </td>
                    <td class="td">
                        手机号码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtPhone" class="txt" runat="server" MaxLength="11" onblur="showErr('txtPhone','spanPhone')"></asp:TextBox><span
                            id="spanPhone" style="color: Red;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        Email：
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" class="txt" MaxLength="50" runat="server" onblur="showErr('txtEmail','spanEmail')"></asp:TextBox><span
                            id="spanEmail" style="color: Red;"></span>
                    </td>
                    <td class="td">
                        邮政编码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtPostalCode" class="txt" MaxLength="6" runat="server"></asp:TextBox><span
                            id="spanPostalCode" style="color: Red;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        联系地址：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtAdress" class="txt" MaxLength="150" runat="server" Width="300px"></asp:TextBox><span
                            id="spanAdress" style="color: Red;"></span>
                    </td>
                </tr>
                <div id="divipset" runat="server">
                <tr>
                    <td colspan="6" class="bt">
                        允许登录IP设置
                    </td>
                </tr>
                <tr id="tr0" runat="server">
                        <th class="w_td">
                            IP1
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA0" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span  id="sAdd0" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd0" onclick="addTR(0)" /></span>
                                    </td>
                                     <td>
                                        <span id="Span1" runat="server">
                                            <input type="button" value=" 清  空 " id="btclear" onclick="cleartxt()"/></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr1" runat="server" class="hide">
                        <th class="w_td">
                            IP2
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA1" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sAdd1" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd1" onclick="addTR(1)" /></span>
                                    </td>
                                    <td>
                                        <span id="sDelete1" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete1" onclick="delTR(1)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr2" runat="server" class="hide">
                        <th class="w_td">
                            IP3
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA2" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sAdd2" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd2" onclick="addTR(2)" /></span>
                                    </td>
                                    <td>
                                        <span id="sDelete2" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete2" onclick="delTR(2)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr3" runat="server" class="hide">
                        <th class="w_td">
                            IP4
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA3" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sAdd3" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd3" onclick="addTR(3)" /></span>
                                    </td>
                                    <td>
                                        <span id="sDelete3" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete3" onclick="delTR(3)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr4" runat="server" class="hide">
                        <th class="w_td">
                            IP5
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA4" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sDelete4" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete4" onclick="delTR(4)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </div>
                <tr>
                    <td colspan="6" class="bt">
                        个人权限开关设置
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <uc1:UserPowerControl ID="UserPowerControl1" runat="server"/>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="lbtnOk" runat="server" CssClass="btn btnNormal" OnClick="lbtnOk_Click"
                            OnClientClick="return showAllErr();">保  存</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="show">
    </div>
       <input type="hidden" runat="server" id="hidtxtCount" value="1" />
    </form>
</body>
</html>
