<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageUserTrip.aspx.cs" Inherits=" TravelNumManage_ManageUserTrip" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>下级用户行程单管理</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .TextBorder
        {
            border: 1px solid #999;
        }
    </style>
    <script language="javascript" type="text/javascript">
        //对话框包含处理        
        function showdialog(t,p) {
            jQuery("select").hide();
            jQuery("#showOne").html(t);
            jQuery("#showOne").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    jQuery("select").show();
                },
                buttons: {
                    '确定': function () {
                        var obj=event.srcElement?event.srcElement:event.target;
                        jQuery(obj)[0].disabled=true;
                        jQuery(this).dialog('close');
                        if(p=="0") {
                            location.go(-1);
                        }
                    }
                }
            });
        }
        function vate() {
            var f=true;
            var RoleType=jQuery("#Hid_RoleType").val();
            if(RoleType=="1") {
                var val=jQuery.trim(jQuery("#ddlGY").val());
                if(val=="") {
                    f=false;
                    showdialog("请选择供应或者落地运营商");
                }
            }
            return f;
        }
    </script>
</head>
<body>
    <div id="showOne">
    </div>
    <form id="form1" runat="server">
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="行程单管理" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table class="Search" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <th>
                            客户帐号
                        </th>
                        <td>
                            <asp:TextBox ID="txtAccount" Width="150px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            客户名称：
                        </th>
                        <td>
                            <asp:TextBox ID="txtCompanyName" Width="150px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <td id="tr1" runat="server">
                            供应或者落地运营商
                        </td>
                        <td id="tr2" runat="server">
                            <asp:DropDownList ID="ddlGY" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td id="tr3" runat="server">
                            <asp:CheckBox ID="ckFenPei" runat="server" Text="已分配用户" />
                        </td>
                        <td align="center" colspan="4">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text="查询" OnClick="btnQuery_Click" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button runat="server" ID="btnPrint" Text="导出Excel" OnClick="btnPrint_Click">
                                </asp:Button></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="0" cellpadding="0"
            border="0">
            <thead>
                <tr>
                    <th>
                        客户帐号
                    </th>
                    <th>
                        客户名称
                    </th>
                    <th>
                        所有
                    </th>
                    <th>
                        已使用
                    </th>
                    <th>
                        未使用
                    </th>
                    <th>
                        作废已审核
                    </th>
                    <th>
                        作废未审核
                    </th>
                    <th>
                        空白回收
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repList" runat="server" OnItemCommand="repList_ItemCommand">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <td style="width: 10%">
                            <%#Eval("LoginName")%>
                        </td>
                        <td>
                            <%#Eval("UninAllName")%>
                        </td>
                        <td>
                            <%#Eval("Total")%>
                        </td>
                        <td>
                            <%#Eval("IsUse")%>
                        </td>
                        <td>
                            <%#Eval("NoUse")%>
                        </td>
                        <td>
                            <%#Eval("IsVoid")%>
                        </td>
                        <td>
                            <%#Eval("IsNotVoid")%>
                        </td>
                        <td>
                            <%#Eval("TotalBack")%>
                        </td>
                        <td style="width: 200px;">
                            <asp:LinkButton runat="server" ID="lbtnDetail" CommandArgument='<%# ShowText(0,Eval("LoginName"),Eval("UserName"),Eval("CpyNo"),Eval("UninAllName"),Eval("OwnerCpyNo"),Eval("OwnerCpyName")) %>'
                                CommandName="Detail" Text="查看详情" OnClientClick="return vate()"></asp:LinkButton>
                            <br />
                            <asp:LinkButton runat="server" ID="lbtnSend" CommandArgument='<%# ShowText(0,Eval("LoginName"),Eval("UserName"),Eval("CpyNo"),Eval("UninAllName"),Eval("OwnerCpyNo"),Eval("OwnerCpyName")) %>'
                                CommandName="TripSend" Text="行程单发放" OnClientClick="return vate()"></asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <input type="hidden" id="Hid_PageSize" runat="server" value="20" />
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    <%--该页面使用角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" />
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    </form>
</body>
</html>
