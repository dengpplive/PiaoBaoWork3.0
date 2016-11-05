<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditRareBuildOil.aspx.cs"
    Inherits="Manager_Base_EditRareBuildOil" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>生僻航线基建燃油编辑</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .input
        {
            width: 100px;
        }
        .th
        {
            width: 100px;
        }
    </style>
    <script type="text/javascript">
        var $J=jQuery.noConflict(false);
        var $J=jQuery.noConflict(false);
        //显示对话框
        function showdialog(t) {
            var div=document.getElementById("dg");
            if(div==null) {
                div=document.createElement("div");
                div.id="dg";
                if(document.all) {
                    document.body.appendChild(div);
                }
                else {
                    document.insertBefore(div,document.body);
                }
            }
            $J(dg).html(t);
            $J(dg).dialog({
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
                        $J(this).dialog('close');
                    },
                    "取消": function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }
        function SaveData() {
            var IsSuc=true;
            var msg="";
            var FromCityCode=$J.trim($J("#txtFromCityCode").val());
            var ToCityCode=$J.trim($J("#txtToCityCode").val());
            var TAX=$J.trim($J("#txtTAX").val());
            var RQFare=$J.trim($J("#txtRQFare").val());
            if($J.isNaN(TAX)) {
                msg="输入基建费格式错误，必须为数字";
            }
            if($J.isNaN(RQFare)) {
                msg="输入燃油费格式错误，必须为数字";
            }
            if(RQFare=="") {
                msg="输入燃油费不能为空";
            }
            if(TAX=="") {
                msg="输入基建费不能为空";
            }
            if(ToCityCode=="") {
                msg="到达城市三字码不能为空";
            }
            if(FromCityCode=="") {
                msg="出发城市三字码不能为空";
            }
            if(msg!="") {
                IsSuc=false;
                showdialog(msg);
            }
            return IsSuc;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="tabs-1">
        <div class="title">
            <span>生僻航线基建燃油编辑</span>
        </div>
        <div class="c-list-filter">
            <table class="Search" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;">
                <tr>
                    <th class="th">
                        出发城市三字码：
                    </th>
                    <td>
                        <asp:TextBox ID="txtFromCityCode" runat="server" class="input"></asp:TextBox>
                    </td>
                    <th class="th">
                        基建费:
                    </th>
                    <td>
                        <asp:TextBox ID="txtTAX" runat="server" MaxLength="5" class="input"></asp:TextBox>
                    </td>
                    <th class="th">
                        燃油费：
                    </th>
                    <td>
                        <asp:TextBox ID="txtRQFare" runat="server" MaxLength="5" class="input"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th class="th">
                        到达城市三字码：
                    </th>
                    <td>
                        <asp:TextBox ID="txtToCityCode" runat="server" class="input"></asp:TextBox>
                    </td>
                    <th class="th">
                        乘客类型：
                    </th>
                    <td>
                        <asp:DropDownList ID="ddlPasType" runat="server" Width="145px">
                            <asp:ListItem Text="成人" Value="1"></asp:ListItem>
                            <asp:ListItem Text="儿童" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <th>
                    </th>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="4">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " OnClick="btnAdd_Click" OnClientClick="return SaveData();" /></span>
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnGoBack" runat="server" Text="返回" OnClick="btnGoBack_Click" /></span>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <asp:HiddenField ID="Hidd_uid" runat="server" />
    </form>
</body>
</html>
