<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DiscountUserSet.aspx.cs"
    Inherits="DiscountSet_DiscountUserSet" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>扣点组用户设置</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" language="javascript">
     <!--
        function ShowDlg(html, t, w, h) {
            $("#showOne").html(html);
            $("#showOne").dialog({
                title: t,
                bgiframe: true,
                position: ['center', 0],
                width: w,
                height: h,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
            }
        });
    }
    function showdialog(t) {
        $("#showOne").html(t);
        $("#showOne").dialog({
            title: '提示',
            bgiframe: true,
            height: 140,
            width: 250,
            modal: true,
            overlay: {
                backgroundColor: '#000',
                opacity: 0.5
            },
            buttons: {
                '确定': function () {
                    $(this).dialog('close');
                    if (t.indexOf("成功") != -1) {
                        document.getElementById("btnQuery").click();
                    }
                }
            }
        });
    }
   
    function checkThis() {
        var cbAll = document.getElementById("cbAll");
        var oArray = new Array();
        oArray = document.getElementsByTagName("input");
        for (var i = 0; i < oArray.length; i++) {
            if (oArray[i].type == "checkbox" && oArray[i].id != "cbAll") {
                if (cbAll.checked)
                    oArray[i].checked = true;
                else
                    oArray[i].checked = false;
            }
        }
    }
    function checkItem(obj) {
        var cbAll = document.getElementById("cbAll");
        var oArray = new Array();
        var cbArray = new Array();
        oArray = document.getElementsByTagName("input");
        for (var i = 0; i < oArray.length; i++) {
            if (oArray[i].type == "checkbox" && oArray[i].id != "cbAll")
                cbArray.push(oArray[i]);
        }

        var flag = true;
        if (obj.checked) { //当选中状态时，判断是否所有都选中；如果是，“全选”勾选框选中
            for (var i = 0; i < cbArray.length; i++) {
                if (!cbArray[i].checked) {
                    flag = false;
                    break;
                }
            }

            cbAll.checked = flag;
        } else {    //不是选中状态时，判断是否都选中；如果不是，“全选”勾选框曲线选中
            cbAll.checked = false;
        }
    }
     //-->
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="扣点组用户设置" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table class="Search" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <th>
                            扣点组：
                        </th>
                        <td>
                            <asp:DropDownList ID="ddlGroup" CssClass="txt" runat="server" AppendDataBoundItems="true">
                            <asp:ListItem Value="-1" Selected="True">--所有组--</asp:ListItem>
                        <asp:ListItem Value="" >未分组</asp:ListItem>
                        </asp:DropDownList>
                        </td>
                        <th>
                            用户账号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtLoginName" Width="115px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <td align="center" colspan="4">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text="查询" OnClick="btnQuery_Click" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnAddGroupUser" runat="server" Text="将选择用户划分到选择的组中" 
                                onclick="btnAddGroupUser_Click"/>
                            </span>
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
                        <%--<input type="checkbox" id="ckIDs" name="ckIDs" onclick="SelectAll()" />--%>
                         <input type="checkbox" id="cbAll" onclick="checkThis()" />
                    </th>
                    <th>
                        所属组
                    </th>
                    <th>
                        公司名称
                    </th>
                    <th>
                        公司账号
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repList" runat="server">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <td>
                             <input runat="server" name="cbItems" id='cbItem' type="checkbox" value='<%# Eval("id")%>'
                                            onclick="checkItem(this);" />
                        </td>
                        <td>
                        <%# ReturnName(Eval("GroupId").ToString()) %>
                        </td>
                        <td>
                            <%# Eval("UninAllName")%>
                        </td>
                        <td>
                            <%# Eval("LoginName")%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
            </webdiyer:AspNetPager>
    </div>
    <div id="showOne">
    </div>
    <input id="hidValue" type="hidden" runat="server"/>
    </form>
</body>
</html>
