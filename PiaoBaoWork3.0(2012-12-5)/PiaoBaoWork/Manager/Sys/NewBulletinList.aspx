<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewBulletinList.aspx.cs"
    Inherits="Sys_BulletinList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>公告查看</title>
    <style type="text/css">
        .s
        {
            border: 1px solid #000000;
            background-color: #666666;
        }
        .paginator
        {
            font: 11px Arial;
            padding: 10px 20px 10px 0;
            margin: 0px;
        }
        .paginator a
        {
            padding: 1px 6px;
            border: solid 1px #ddd;
            background: #fff;
            text-decoration: none;
            margin-right: 2px;
        }
        .paginator .cpb
        {
            padding: 1px 6px;
            font-weight: bold;
            font-size: 13px;
            border: none;
        }
        .paginator a:hover
        {
            color: #fff;
            background: #ffa501;
            border-color: #ffa501;
            text-decoration: none;
        }
        .red
        {
            color: Red;
        }
    </style>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../../js/area.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            // Tabs
            $('#tabs').tabs();
        });
        function showimg(obj) {
            $("#"+obj).show();
        }
        function hideimg(obj) {
            $("#"+obj).hide();
        }
        function showdetail(id) {
            showdialog("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='yes' allowtransparency='yes' width='100%' height='100%' src='LookBulletin.aspx?id="+id+"&currentuserid=<%=this.mUser.id.ToString() %>'/>",380,900);
        }
        function showdialog(t,h,w) {
            $("#dd").html(t);
            $("#dd").dialog({
                title: '查看公告',
                bgiframe: true,
                height: h,
                width: w,
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
        $(function () {
            $('#tabs').tabs();
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); });

        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="dd">
    </div>
    <div class="title">
        <span>公告查看</span>
    </div>
    <!-- Tabs -->
    <div id="tabs">
        <table id="table_info" class="table_info" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <th style="width: 30%">
                    公告标题
                </th>
                <th style="width: 30%">
                    发布人
                </th>
                <th style="width: 30%">
                    发布时间
                </th>
                <th style="width: 10%;">
                    下载附件
                </th>
            </tr>
            <asp:Repeater ID="Repeater" runat="server">
                <ItemTemplate>
                    <tr class="leftliebiao_checi">
                        <td style='text-align: center'>
                            <%# ShowText(0, Eval("Emergency"), Eval("Title"), Eval("id"))%>
                        </td>
                        <td>
                            <%#Eval("ReleaseName")%>
                        </td>
                        <td>
                            <%#Eval("ReleaseTime")%>
                        </td>
                        <td>
                            <%# ShowText(1, Eval("AttachmentFileName"), Eval("id"))%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    </form>
</body>
</html>
