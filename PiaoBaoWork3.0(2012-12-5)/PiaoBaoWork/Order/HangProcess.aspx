<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HangProcess.aspx.cs" Inherits="Air_Order_HangProcess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>挂起/解挂处理</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../css/detail.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">
        //显示隐藏遮罩层 true 显示 false隐藏
        function ShowDiv(flag) {
            if(flag) {
                $("#overlay").show();
                $("#loading").show();
            } else {
                $("#overlay").hide();
                $("#loading").hide();
            }
        }
        $(function () {
            $('#tabs').tabs();
        });
        function showdialog(t,param) {
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
                        if(param!=null&&param.op!=null) {
                            if(param.op==1) {
                                // location.reload(true);
                            }
                        }
                        ShowDiv(false);
                    }
                }
            });
        }

        function SelAll(obj) {
            $("input[id*='RepPassenger_cboSelect'][type='checkbox']").attr("checked",obj.checked);
        }

        //挂起之前的操作
        function Validate() {
            var fg=true;
            var selLen=$("input[id*='RepPassenger_cboSelect'][type='checkbox']:checked").length;
            if(selLen==0) {
                fg=false;
                showdialog('请选择需要进行操作的乘客！');
            } else {
                ShowDiv(true);
            }
            return fg;
        }   
    </script>
    <style type="text/css">
        .table_info th, .table_info td
        {
            border-color: #CCCCCC;
            border-style: solid;
            border-width: 1px 1px 1px 1px;
            color: #606060;
        }
        table
        {
            border-collapse: collapse;
        }
        .tdNew
        {
            font-weight: bold;
            background: url("../img/title.png") repeat-x scroll 0 0 transparent;
            text-align: right;
        }
        .td1
        {
            color: Red;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .inputBorder
        {
            border: 1px solid #999;
        }
        .green
        {
            color: Green;
        }
        .red
        {
            color: red;
        }
        /*-----------------加载遮罩---------------------------------*/
        #loading
        {
            margin-top: 10px;
            width: 420px;
            border: 0 none;
            text-align: center;
            padding: 40px 30px 40px 30px;
            color: #707070;
            font-size: 18px;
            line-height: 180%;
            position: fixed;
            left: 30%;
            top: 30%;
            z-index: 1000;
            background: url(../images/mainbg.gif);
        }
        #overlay
        {
            background-color: #333333;
            left: 0;
            filter: alpha(opacity=50); /* IE */
            -moz-opacity: 0.5; /* 老版Mozilla */
            -khtml-opacity: 0.5; /* 老版Safari */
            opacity: 0.5;
            position: fixed;
            top: 0;
            width: 100%;
            z-index: 999;
            height: 100%;
        }
    </style>
</head>
<body>
    <div id="showOne">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <div id="tabs-1" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
            <table width="100%" align="center" border="0" cellpadding="0" cellspacing="0" style="border: 1px solid #E6E6E6;">
                <tr>
                    <td class="mainl">
                    </td>
                    <td>
                        <table width="100%" align="center" class="detail" border="0" cellpadding="5" cellspacing="0"
                            style="padding: 5px;">
                            <tr>
                                <td>
                                    <div class="ebill-bg-top">
                                        <h1>
                                            <span runat="server" id="Suptitle">挂起/解挂处理</span></h1>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="table6" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <thead>
                                            <tr>
                                                <th>
                                                    <input type="checkbox" id="ckall" onclick="SelAll(this)" />
                                                </th>
                                                <th>
                                                    乘客姓名
                                                </th>
                                                <th>
                                                    乘客类型
                                                </th>
                                                <th>
                                                    证件类型
                                                </th>
                                                <th>
                                                    证件号码
                                                </th>
                                                <th>
                                                    票号
                                                </th>
                                                <th>
                                                    状态
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepPassenger" runat="server">
                                            <ItemTemplate>
                                                <tr style="line-height: 30px;">
                                                    <td style="width: 5%; text-align: center;">
                                                        <asp:CheckBox ID="cboSelect" name="cboSelect" runat="server" />
                                                        <input id="hid_Id" type="hidden" runat="server" value='<%# Eval("id") %>' />
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%#Eval("PassengerName")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# ShowText(0, Eval("PassengerType"))%>
                                                    </td>
                                                    <td style="width: 15%; text-align: center;">
                                                        <%# ShowText(1, Eval("PassengerType"))%>
                                                    </td>
                                                    <td style="width: 30%; text-align: center;">
                                                        <%#Eval("Cid")%>
                                                    </td>
                                                    <td style="width: 20%; text-align: center;">
                                                        <%#Eval("TicketNumber")%>
                                                    </td>
                                                    <td style="width: 15%; color: Red;">
                                                        <%# ShowText(2, Eval("Suspended"))%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td align="center">
                                                <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnOk" runat="server" Text="挂起" OnClientClick="return Validate();"
                                                        OnClick="btnOk_Click" />
                                                </span><span class="btn btn-ok-s">
                                                    <asp:Button ID="btnCancel" runat="server" Text="返回" OnClick="btnCancel_Click" />
                                                </span>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="ebill">
                                        <div>
                                            <span class="ebill-account"><span class="inner"><span class="inner">基础操作信息</span> </span>
                                            </span>
                                        </div>
                                        <div class="ebill-info-bg-inner">
                                            <table border="0" cellspacing="0" cellpadding="0" width="100%">
                                                <tr>
                                                    <td style="text-align: right; border: 1px">
                                                        <asp:Label ID="lbl1" runat="server" Text="锁定操作员：" Width="100px"></asp:Label>
                                                    </td>
                                                    <td class="tab_in_td_f" style="width: 40%;">
                                                        <asp:Label ID="lblLockId" runat="server" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Label ID="lbl2" runat="server" Text="锁定时间：" Width="100px"></asp:Label>
                                                    </td>
                                                    <td class="tab_in_td_f" style="width: 40%;">
                                                        <asp:Label ID="lblLockTime" runat="server" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="table5" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                                width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>
                                                            操作时间
                                                        </th>
                                                        <th>
                                                            操作员账号
                                                        </th>
                                                        <th>
                                                            操作员姓名
                                                        </th>
                                                        <th>
                                                            操作类型
                                                        </th>
                                                        <th>
                                                            详细记录
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <asp:Repeater ID="RepOrderLog" runat="server">
                                                    <ItemTemplate>
                                                        <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperTime")%>
                                                            </td>
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperLoginName")%>
                                                            </td>
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperUserName")%>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("OperType")%>
                                                            </td>
                                                            <td style="width: 45%; text-align: center;">
                                                                <asp:Label Style="word-break: break-all; white-space: normal" ID="lblLogContent"
                                                                    Width="100%" runat="server" Text=' <%#Eval("OperContent")%>' ToolTip='<%#Eval("OperContent") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                            <div class="fn-clear">
                                            </div>
                                        </div>
                                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="ebill-info-bg">
                                            <tr>
                                                <td class="ebill-info-bgl">
                                                </td>
                                                <td class="ebill-info-bgc">
                                                    &nbsp;
                                                </td>
                                                <td class="ebill-info-bgr">
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="mainr">
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="overlay" runat="server" style="display: none;">
    </div>
    <div id="loading" style="display: none;">
        请稍等，正在处理数据<br />
        <span id="spastr">……</span><br />
        <img src="../img/loading.gif"></div>
    </form>
</body>
</html>
