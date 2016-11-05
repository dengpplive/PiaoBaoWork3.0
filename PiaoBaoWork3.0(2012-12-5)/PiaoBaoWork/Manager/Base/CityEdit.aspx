<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CityEdit.aspx.cs" Inherits="Sys_CityEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>城市编辑</title>
    <script src="../../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../../js/area.js" type="text/javascript"></script>
    <link href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../../kindeditor-3.5-zh_CN/kindeditor.js" type="text/javascript"></script>
    <script src="../../js/Calendar/WdatePicker.js" type="text/javascript"></script>
    <link href="../../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../../css/table.css" rel="stylesheet" type="text/css" />
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
                        location.href="CityList.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
                    }
                }
            });
        }
        function showErr(c1,c2) {
            if($("#"+c1).val()=="") {
                $("#"+c2).html("不能为空！");
            }
            else {
                $("#"+c2).html("<b>*</b>");
            }
        }
        function showAllErr() {
            var bools=0;
            if($("#txtCity").val()=="") {
                bools++;
                $("#spanCity").html("不能为空！");
            }
            else {
                $("#spanCity").html("<b>*</b>");
            }

            if($("#txtQuanPing").val()=="") {
                bools++;
                $("#spanQuanPing").html("不能为空！");
            }
            else {
                $("#spanQuanPing").html("<b>*</b>");
            }

            if($("#txtJianPin").val()=="") {
                bools++;
                $("#spanJianPin").html("不能为空！");
            }
            else {
                $("#spanJianPin").html("<b>*</b>");
            }

            if($("#txtAirPortName").val()=="") {
                bools++;
                $("#spanAirPortName").html("不能为空！");
            }
            else {
                $("#spanAirPortName").html("<b>*</b>");
            }

            if($("#txtCityCode").val()=="") {
                bools++;
                $("#spanCode").html("不能为空！");
            }
            else {
                $("#spanCode").html("<b>*</b>");
            }
            if($("#txtCountry").val()=="") {
                bools++;
                $("#spanCountry").html("不能为空！");
            }
            else {
                $("#spanCountry").html("<b>*</b>");
            }
            if($("#txtContinents").val()=="") {
                bools++;
                $("#spanContinents").html("不能为空！");
            }
            else {
                $("#spanContinents").html("<b>*</b>");
            }
            if(bools>0) {
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
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info" class="table_info">
                <tr>
                    <td colspan="4" class="bt">
                        城市基本信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        城市中文：
                    </td>
                    <td>
                        <asp:TextBox ID="txtCity" CssClass="txt" runat="server" onblur="showErr('txtCity','spanCity')"></asp:TextBox>
                        <span id="spanCity" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        城市全拼：
                    </td>
                    <td>
                        <asp:TextBox ID="txtQuanPing" CssClass="txt" runat="server" onblur="showErr('txtQuanPing','spanQuanPing')"></asp:TextBox>
                        <span id="spanQuanPing" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        城市三字码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtCityCode" CssClass="txt" runat="server" onblur="showErr('txtCityCode','spanCode')"></asp:TextBox>
                        <span id="spanCode" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        城市简拼：
                    </td>
                    <td>
                        <asp:TextBox ID="txtJianPin" CssClass="txt" runat="server" onblur="showErr('txtJianPin','spanJianPin')"></asp:TextBox>
                        <span id="spanJianPin" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        城市其他三字码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtCityOtherCodeWord" CssClass="txt" runat="server"></asp:TextBox>
                    </td>
                    <td class="td">
                        机场名称：
                    </td>
                    <td>
                        <asp:TextBox ID="txtAirPortName" CssClass="txt" runat="server" onblur="showErr('txtAirPortName','spanAirPortName')"></asp:TextBox>
                        <span id="spanAirPortName" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        国家名字：
                    </td>
                    <td>
                        <asp:TextBox ID="txtCountry" CssClass="txt" runat="server" onblur="showErr('txtCountry','spanCountry')"></asp:TextBox>
                        <span id="spanCountry" style="color: Red;"><b>*</b></span>
                    </td>
                    <td class="td">
                        所在洲：
                    </td>
                    <td>
                        <asp:TextBox ID="txtContinents" CssClass="txt" runat="server"></asp:TextBox>
                        <span id="spanContinents" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        国家类型：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlType" CssClass="txt" runat="server">
                            <asp:ListItem Selected="True" Text="国内" Value="1"></asp:ListItem>
                            <asp:ListItem Text="国际" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="td">
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="return showAllErr();"
                            CssClass="btn btnNormal" OnClick="LinkButton1_Click">保  存</asp:LinkButton>
                        &nbsp; &nbsp;&nbsp;&nbsp;<a href="CityList.aspx?currentuserid=<%=this.mUser.id.ToString() %>"
                            class="btn btnNormal">返 回</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
