<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IBEsel.aspx.cs" Inherits="Manager_Base_IBEsel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>IBE航班查询</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
        <script language="javascript" type="text/javascript">
    <!--
            function showdialog(t) {
                $("#showOne").html(t);
                $("#showOne").dialog({
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

            function onchangeSel(obj) {
                if (obj != null && obj != undefined) {
                    $("#hid_selValue").val(obj.value);
                    $("#btnSelTwo").click();
                    //  document.getElementById("btnOk2").click();
                }
            }
    //-->
    </script>
</head>
<body>
    <form id="form2" runat="server">
    <div id="tabs">
        <div class="title">
            <span>IBE航班查询</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                        border="1" style="border-collapse: collapse;">
                        <tr>
                            <td style="text-align: right">
                                出发城市(三字码):
                            </td>
                            <td style="text-align: left">
                                <asp:TextBox ID="txtToCity" Width="50px" runat="server"></asp:TextBox>
                                <span style="color: Red;">必填</span>
                            </td>
                            <td style="text-align: right">
                                到达城市(三字码):
                            </td>
                            <td style="text-align: left">
                                <asp:TextBox ID="txtFromCity" Width="50px" runat="server"></asp:TextBox>
                                <span style="color: Red;">必填</span>
                            </td>
                            <td style="text-align: right">
                                起飞时间(格式：28NOV12):
                            </td>
                            <td style="text-align: left">
                                <asp:TextBox ID="txtTime" runat="server"></asp:TextBox>
                                <span style="color: Red;">必填</span>
                            </td>
                            <td style="text-align: right">
                                航空公司(二字码):
                            </td>
                            <td style="text-align: left">
                                <asp:TextBox ID="txtAirCode" Width="50px" runat="server"></asp:TextBox>
                                <span style="color: Red;"></span>
                            </td>
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" />
                                </span>&nbsp;&nbsp; <span class="btn btn-ok-s">
                                    <asp:Button ID="btnClear" runat="server" Text=" 清 空 " OnClick="btnClear_Click" />
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9">
                                <asp:TextBox ID="txtReturnValue" runat="server" Rows="10" MaxLength="255" TextMode="MultiLine"
                                    Width="100%" BackColor="Black" ForeColor="Lime"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <tabled class="tb-all-trade" width="100%" cellspacing="" cellpadding="0" border="1"
                        style="border-collapse: collapse;">
                        <tr>
                            <td style="width: 8%; text-align: right;">
                                更多信息：
                            </td>
                            <td style="width: 92%; text-align: left;">
                                <asp:DropDownList ID="ddlNum" runat="server" onchange="onchangeSel(this)">
                                </asp:DropDownList>
                                <input type="hidden" id="hid_selValue" runat="server" value="" />
                                <span style="display: none;">
                                    <asp:Button ID="btnSelTwo" runat="server" Text=" 查 询 " OnClick="btnSelTwo_Click" />
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:GridView ID="gvValue" CssClass="tb-all-trade" Width="100%" EmptyDataText=""
                                    runat="server">
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="showOne">
    </div>
    </form>
</body>
</html>
