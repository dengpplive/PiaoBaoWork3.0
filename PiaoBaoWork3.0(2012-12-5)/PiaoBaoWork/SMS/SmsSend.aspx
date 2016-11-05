<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SmsSend.aspx.cs" Inherits="SMS_SmsSend" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link href="../js/My97DatePicker/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        #txtnum
        {
            height: 100px;
            width: 350px;
        }
        #txtContents
        {
            height: 100px;
            width: 350px;
        }
        .inputtxtdat{ width:350px}
    </style>
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
                    }
                }
            });
        }
        function clearNum() {
            document.getElementById("txtnum").value = "";
        }
        function showdivcontent(s1, s2) {
            document.getElementById(s1).style.display = "none";
            document.getElementById(s2).style.display = "block";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="dd">
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="title">
        <span>发送短信</span><span id="spansycount" runat="server">剩余短信
            <asp:Label ID="lblmsgcount" runat="server" Text="0"></asp:Label>
            &nbsp;条</span></div>
    <table class="tb-all-trade" border="0" cellspacing="0" cellpadding="0" style="width: 100%;">
        <tr>
            <td rowspan="3" align="center">
                接收手机:
            </td>
            <td style="text-align: left">
                <div id="show1">
                    <asp:LinkButton ID="lkbtxj" runat="server" OnClick="lkbtxj_Click">[选择下级用户]</asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton ID="lkbtclk" runat="server" OnClick="lkbtclk_Click">[选择常旅客]</asp:LinkButton>
                    &nbsp; <a href="#" onclick="showdivcontent('show1','show2')">[批量导入号码]</a>&nbsp;
                    <a href="#" onclick="clearNum()">[清空号码]</a>
                </div>
                <div id="show2" style="display: none">
                
                    <asp:FileUpload ID="FileUpload1" CssClass="inputtxtdat, Tbox" runat="server" />
                   
                    <span class="btn btn-ok-s">
                    <asp:Button ID="btdy" runat="server" Text="导入" OnClick="btdy_Click" />
                   
                    <input id="btback" type="button" value="返回" onclick="showdivcontent('show2','show1')" />
                     </span>
                    导入文件格式必须为*.txt文本文件,导入最大号码数100
                </div>
            </td>
        </tr>
        <tr>
            <td style="text-align: left">
                <textarea id="txtnum" runat="server" class="inputtxtdat" rows="10" cols="50"></textarea>
            </td>
        </tr>
        <tr>
            <td style="text-align: left; color: #ff6600;">
                多个号码请用英文“,”隔开 ,最大号码数100个。
            </td>
        </tr>
        <tr>
            <td rowspan="3" align="center">
                短信内容:
            </td>
            <td style="text-align: left">
                <div style="float: left">
                    <asp:DropDownList ID="ddltemptype" runat="server" CssClass="inputtxtdat" AutoPostBack="True"
                        OnSelectedIndexChanged="ddltemptype_SelectedIndexChanged">
                        <asp:ListItem Value="0">标准模板</asp:ListItem>
                        <asp:ListItem Value="1">自定义模板</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;
                    <asp:DropDownList ID="ddltemplate" CssClass="inputtxtdat" runat="server" Height="20px"
                        AutoPostBack="True" OnSelectedIndexChanged="ddltemplate_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div runat="server" id="orderidshow" style="float: left; margin-left: 10px">
                    订单号：<asp:TextBox ID="txtorderno" runat="server" CssClass="inputtxtdat"></asp:TextBox>&nbsp;
                    <span class="btn btn-ok-s">
                        <asp:Button ID="btdrcontents" runat="server" Text="导入内容" OnClick="btdrcontents_Click" /></span></div>
            </td>
        </tr>
        <tr>
            <td style="text-align: left">
                <textarea id="txtContents" runat="server" class="inputtxtdat" rows="10" cols="50"></textarea>
            </td>
        </tr>
        <tr>
            <td style="text-align: left; color: #ff6600;">
                短信内容最多支持400个字，一条短信包含64个字，超过就按多条计费。
            </td>
        </tr>
        <tr>
            <td>
                短信后缀:
            </td>
            <td style="text-align: left; height: 100%">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txthz" runat="server" CssClass="inputtxtdat" AutoPostBack="True"></asp:TextBox><asp:CheckBox
                            ID="CheckBox1" runat="server" Text="后缀为本单位名称" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <span class="btn btn-ok-s">
                    <asp:Button ID="btsendmsg" runat="server" Text="发送短信" OnClick="btsendmsg_Click" /></span>&nbsp;&nbsp;
                <span class="btn btn-ok-s" runat="server" id="spanSel" visible="false">
                    <asp:Button ID="btnSel" runat="server" Text="短信余额查询" OnClick="btnSel_Click" /></span>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
