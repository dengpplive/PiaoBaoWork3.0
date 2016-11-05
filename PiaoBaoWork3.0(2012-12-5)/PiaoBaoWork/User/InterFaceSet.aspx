<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InterFaceSet.aspx.cs" Inherits="User_InterFaceSet" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
     <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
      <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
      <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
     <style type="text/css">
        .rblZH, .rblZH td
        {
            margin: 0 0 0 0;
            padding: 0 0 0 0;
            border: 0px;
        }
        
        .style2
        {
            color: red;
        }
        .table_info td
        {
            text-align: left;
        }
        .table_info tr
        {
            line-height: 10px;
            height: 10px;
        }
        .table_info th, .table_info td
        {
            line-height: 25px;
            border-style: solid;
            border: 1px;
        }
        .table_info
        {
            width: 100%;
            text-align: center;
            margin: 0px;
            word-break: break-all;
        }
        .table_info .td
        {
            background-color: #ffffff;
            color: #424242;
            font-size: 12px;
            line-height: 24px;
            margin: 0;
            text-align: right;
        }
        .style3
        {
            height: 20px;
        }
        .style4
        {
            height: 20px;
        }
        .style5
        {
            height: 20px;
        }
    </style>
    <script type="text/javascript">
        //根据 ie 兼容性 隐藏页面下拉列表元素
        function checkBrowser_hideElement() {
            var isIE = !!window.ActiveXObject;
            var isIE6 = isIE && !window.XMLHttpRequest;
            var isIE8 = isIE && !!document.documentMode;
            var isIE7 = isIE && !isIE6 && !isIE8;

            if (isIE6) {
                document.getElementById("province").style.display = "none";
                document.getElementById("city").style.display = "none";
            }
        }
        //根据 ie 兼容性 显示页面下拉列表元素
        function checkBrowser_showElement() {
            var isIE = !!window.ActiveXObject;
            var isIE6 = isIE && !window.XMLHttpRequest;
            var isIE8 = isIE && !!document.documentMode;
            var isIE7 = isIE && !isIE6 && !isIE8;

            if (isIE6) {
                document.getElementById("province").style.display = "";
                document.getElementById("city").style.display = "";
            }
        }
        function showdialog(t) {
            checkBrowser_hideElement();
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
                },
                close: function () {

                    checkBrowser_showElement();
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
   <div class="infomain">
        <div class="mainPanel">
     <table id="table_info" class="table_info" width="100%" cellspacing="0" cellpadding="0"
                border="0">
      <tr>
                        <td colspan="8" class="bt">
                            接口账号设置
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            517接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKact517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.517na.com','517')">http://www.517na.com</a>
                        </td>
                        <td style="text-align: right;">
                            517接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwd517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            517接口KEY:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKkey517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            517预存款账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyckack517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            517预存款密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyckpwd517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            51book接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKact51book" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.51book.com','51book')">http://www.51book.com</a>
                        </td>
                        <td style="text-align: right;">
                            51book接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwd51book" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            51book接口KEY:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKkey51book" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            51book通知地址:
                        </td>
                        <td>
                            <asp:TextBox ID="txtNoticeURL51book" CssClass="txt" Width="120px" runat="server"
                                Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            百托接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKactBT" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.baitour.com','bt')">http://www.baitour.com</a>
                        </td>
                        <td style="text-align: right;">
                            百托接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwdBT" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            百托接口KEY:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKkeyBT" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            票盟接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKactPM" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.piaomeng.net.cn','pm')">http://www.piaomeng.net.cn</a>
                        </td>
                        <td style="text-align: right;">
                            票盟接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwdPM" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            票盟接口KEY:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKkeyPM" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            今日接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKactJR" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://new.jinri.net.cn','jr')">http://new.jinri.net.cn</a>
                        </td>
                        <td style="text-align: right;">
                            今日接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwdJR" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            8000yi接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKact8000yi" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>&nbsp;&nbsp;<a
                                title="点击进入" style="color: Blue" onclick="window.open('http://www.8000yi.com','8000yi')">http://www.8000yi.com</a>
                        </td>
                        <td style="text-align: right;">
                            8000翼接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwd8000yi" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            8000翼代扣支付宝:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKDKZFB8000yi" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            <asp:LinkButton ID="lk8000yiZFBSigning" runat="server" 
                                onclick="lk8000yiZFBSigning_Click" >签约</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            易行总账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyixing" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>&nbsp;&nbsp;<a
                                title="点击进入" style="color: Blue" onclick="window.open('http://www.yeexing.com','yeexing')">http://www.yeexing.com</a>
                        </td>
                        <td style="text-align: right;">
                            易行供应账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyixinggy" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
    </table>
    <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="lbtnOK" runat="server" CssClass="btn btnNormal"
                            OnClick="lbtnOK_Click">保  存 </asp:LinkButton>
                    </td>
                </tr>
            </table>
    </div>
    </div>
    </form>
</body>
</html>
