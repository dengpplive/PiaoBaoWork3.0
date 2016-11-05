<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>首页展示数据</title>
    <link type="text/css" href="css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href=" css/table.css" rel="stylesheet" />
    <link type="text/css" href="js/Tooltip/Tooltip.css" rel="stylesheet" />
    <link type="text/css" href=" js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src=" js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src=" js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .red
        {
            color: Red;
        }
        .show
        {
            display: block;
        }
        .hide
        {
            display: none;
        }
        .tdw100
        {
            width: 100px;
        }
        .tdw150
        {
            width: 150px;
        }
        .tdw200
        {
            width: 200px;
        }
    </style>
    <script type="text/javascript">
        function Wopen() {
            //var win=window.open("ShowPage.htm");
            alert("请您尽快在【公司】【个人信息】进行个人资料信息维护，以便于更好的为您服务！");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal runat="server" ID="AccountInfo"></asp:Literal>
    <asp:HiddenField ID="currentuserid" ClientIDMode="Static" runat="server" />
    <div class="title">
        <span>公告查看</span>
    </div>
    <%--公告区域--%>
    <div id="divNotice" runat="server">
        <table id="tb-all-trade" class="tb-all-trade" border="1" cellspacing="0" cellpadding="0"
            width="100%">
            <tr>
                <th>
                    公告标题
                </th>
                <th>
                    公告内容
                </th>
                <th>
                    发布人
                </th>
                <th>
                    发布时间
                </th>
            </tr>
            <asp:Repeater ID="Repeater" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# ShowText(2, Eval("Emergency"), Eval("Title"), Eval("id"))%>
                        </td>
                        <td title='<%# ShowText(3,Eval("Content")) %>'>
                            <%# SubChar(Eval("Content"),50,"...")%>
                        </td>
                        <td>
                            <%# Eval("ReleaseName")%>
                        </td>
                        <td class="tdw100">
                            <%# Eval("ReleaseTime")%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div id="Pagefooter" runat="server" class="hide">
            <input type="hidden" id="hid_PageSize" value="20" runat="server" />
            <input type="hidden" id="Hid_Emergency" runat="server" />
            <input type="hidden" id="Hid_IsPager" runat="server" value="0" />
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
            </webdiyer:AspNetPager>
        </div>
    </div>
    <script type="text/javascript" src="js/Tooltip/ToolTip.js"> </script>
    <script type="text/javascript" src="js/js_WinOpen.js"> </script>
    <script type="text/javascript">
        $(function () {
            initToolTip("td");
            var val=$.trim($("#Hid_Emergency").val());
            var IsPager=$.trim($("#Hid_IsPager").val());
            if(val!=""&&IsPager=="0") {
                var url="Manager/Sys/LookBulletin.aspx";
                //window.open(url,'_blank','height=500, width=700, top=100, left=150, toolbar=no, menubar=no, scrollbars=no,resizable=no,location=no, status=no');
                WinOpen(url,"POST",{ id: val,currentuserid: $("#currentuserid").val() });
            }
        });
      
    </script>
    </form>
</body>
</html>
