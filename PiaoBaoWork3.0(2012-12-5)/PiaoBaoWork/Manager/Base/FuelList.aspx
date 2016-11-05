<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FuelList.aspx.cs" Inherits="Sys_FuelList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>燃油管理</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../../js/area.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../js/My97DatePicker/WdatePicker.js"></script>
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
                    },
                    "取消": function () {
                        $(this).dialog('close');
                    }
                }
            });
        }
        $(function () {
            // Tabs
            $('#tabs').tabs();
            var zz1 = /^\d*$/
            $("#txtLowAdultFee").keyup(function () {
                if (!zz1.test($("#txtLowAdultFee").val())) {
                    $("#txtLowAdultFee").val("");
                }
            })
            $("#txtLowChildFee").keyup(function () {
                if (!zz1.test($("#txtLowChildFee").val())) {
                    $("#txtLowChildFee").val("");
                }
            })
            $("#txtExceedAdultFee").keyup(function () {
                if (!zz1.test($("#txtExceedAdultFee").val())) {
                    $("#txtExceedAdultFee").val("");
                }
            })
            $("#txtExceedChildFee").keyup(function () {
                if (!zz1.test($("#txtExceedChildFee").val())) {
                    $("#txtExceedChildFee").val("");
                }
            })
        });

        function ClearData() {
            var inputs = document.getElementsByTagName("input");
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type && inputs[i].type == "text") {
                    inputs[i].value = "";
                }
            }
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 196px;
        }
        .Search th
        {
            width: 120px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div id="tabs">
        <div class="title">
            <span>燃油管理</span>
        </div>
        <div id="tabs-1">
            <table id="tb-all-trade" class="tb-all-trade" cellspacing="0" cellpadding="0" border="0"
                width="100%">
                <thead>
                    <tr>
                        <th>
                            低于800公里成人
                        </th>
                        <th>
                            超过800公里成人
                        </th>
                        <th>
                            低于800公里儿童
                        </th>
                        <th>
                            超过800公里儿童
                        </th>
                        <th>
                            生效时间
                        </th>
                        <th>
                            失效时间
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                    <td>
                        <div id="div_LowAdultFee" runat="server">
                        </div>
                    </td>
                    <td>
                        <div id="div_ExceedAdultFee" runat="server">
                        </div>
                    </td>
                    <td>
                        <div id="div_LowChildFee" runat="server">
                        </div>
                    </td>
                    <td>
                        <div id="div_ExceedChildFee" runat="server">
                        </div>
                    </td>
                    <td>
                        <div id="div_StartTime" runat="server">
                        </div>
                    </td>
                    <td>
                        <div id="div_EndTime" runat="server">
                        </div>
                    </td>
                    <td class="Operation">
                        <asp:LinkButton ID="btnUpdate" runat="server" Text="修改" OnClick="btnUpdate_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="Search">
                <tr>
                    <th>
                        低于800公里成人：
                    </th>
                    <td align="left" class="style1">
                        <asp:TextBox ID="txtLowAdultFee" CssClass="inputtxtdat" runat="server" Style="width: 120px;"></asp:TextBox>
                        <span id="spanCabin" style="color: Red;"><b>*</b></span>
                    </td>
                    <th>
                        低于800公里儿童：
                    </th>
                    <td align="left" style="width: 100px;">
                        <asp:TextBox ID="txtLowChildFee" CssClass="inputtxtdat" runat="server" Style="width: 120px;"></asp:TextBox>
                        <span id="span1" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <th>
                        超过800公里成人：
                    </th>
                    <td align="left" class="style1">
                        <asp:TextBox ID="txtExceedAdultFee" CssClass="inputtxtdat" runat="server" Style="width: 120px;"></asp:TextBox>
                        <span id="spanAirPortCode" style="color: Red;"><b>*</b></span>
                    </td>
                    <th>
                        超过800公里儿童：
                    </th>
                    <td align="left" style="width: 100px;">
                        <asp:TextBox ID="txtExceedChildFee" CssClass="inputtxtdat" runat="server" Style="width: 120px;"></asp:TextBox>
                        <span id="spanAirPortName" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <th>
                        生效时间：
                    </th>
                    <td align="left">
                        <input type="text" id="txtStartDate" style="width: 130px;" readonly="true" class="inputBorder"
                            runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />至
                        <input type="text" id="txtEndDate" style="width: 130px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                    </td>
                    <th>
                    </th>
                    <td align="left">
                        <span id="span2" style="color: Red;"><b>*</b></span>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="center">
                        <div style="text-align: center;">
                            <span class="btn btn-ok-s">
                                <asp:Button runat="server" ID="btnAdd" Text=" 保 存 " OnClick="btnAdd_Click" />
                            </span>&nbsp; <span class="btn btn-ok-s">
                                <asp:Button runat="server" ID="Button1" Text=" 清 空 " OnClick="Button1_Click" />
                            </span>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
