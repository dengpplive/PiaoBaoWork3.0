<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DiscountSetDetail.aspx.cs"
    Inherits="DiscountSet_DiscountSetDetail" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>扣点组明细</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
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
        bd
        {
            border: 0px solid black;
        }
        .ullist
        {
            margin: 0 0 0 0;
            padding: 0 0 0 0;
            list-style-type: none;
        }
        .ullist li
        {
            display: inline;
            list-style-type: none;
            float: left;
        }
    </style>
    <style type="text/css">
        #newtable
        {
            font-size: 12px;
        }
        .divTab
        {
            border-collapse: collapse;
        }
        .inputPoint
        {
            width: 50px;
        }
        .divtd
        {
            width: 80px;
        }
        .domaintab
        {
            width: 600px;
        }
        .xftd
        {
            width: 200px;
        }
        .editInput
        {
            width: 100px;
        }
        .qutd
        {
            width: 250px;
        }
        
        #bendiTable td
        {
            text-align: center;
        }
        #jkTable td
        {
            text-align: center;
        }
        
        #gxTable td
        {
            text-align: center;
        }
        
        .borderShow
        {
            border: 1px solid #999;
        }
        .tdright
        {
            text-align: right;
        }
    </style>
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


    function showdlg(t) {
        $("#ddd").html(t);
        $("#ddd").dialog({
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
                    if (t.indexOf("成功") != -1) {
                        document.getElementById("btnQuery").click();
                    }
                }
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
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="扣点组明细" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table class="Search" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <th>
                            扣点组名称：
                        </th>
                        <td>
                            <asp:TextBox ID="txtGroupName" Width="115px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <td align="center" colspan="4">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text="查询" 
                                onclick="btnQuery_Click"  />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnAdd" runat="server" Text="添加扣点明细" 
                                onclick="btnAdd_Click"/>
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnDel" runat="server" Text="批量删除" onclick="btnDel_Click" />
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="0" cellpadding="0" border="0">
            <thead>
                <tr>
                    <th>
                        <input type="checkbox" id="cbAll" onclick="checkThis()" />
                    </th>
                  
                    <th>
                        扣点组名称
                    </th>
                    <th>
                        类型
                    </th>
                    <th>
                        竞价平台序号
                    </th>
                    <th>
                        承运人代码
                    </th>
                    <th>
                        出发城市代码
                    </th>
                    <th>
                        到达城市代码
                    </th>
                    <th>
                        时间范围
                    </th>
                    <th>
                        扣点范围^点数^现返
                    </th>
                    <th>
                        调整类型
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repList" runat="server" onitemcommand="repList_ItemCommand">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <th>
                            <input runat="server" name="cbItems" id='cbItem' type="checkbox" value='<%# Eval("did")%>' onclick="checkItem(this);" />
                        </th>
                      
                        <td>
                            <%# Eval("GroupName")%>
                        </td>
                        <td>
                            <%# Eval("BaseType").ToString() == "1" ? "本地" : Eval("BaseType").ToString() == "2" ? "竞价平台" : "共享"%>
                        </td>
                        <td>
                            <%# Eval("PolicySource").ToString()  %>
                        </td>
                        <td>
                            <%# Eval("CarryCode")%>
                        </td>
                        <td>
                             <%# Eval("FromCityCode").ToString().Length > 20 ? Eval("FromCityCode").ToString().Substring(0, 20) + "..." : Eval("FromCityCode").ToString()%>
                        </td>
                        <td>
                        <%# Eval("ToCityCode").ToString().Length > 20 ? Eval("ToCityCode").ToString().Substring(0, 20) + "..." : Eval("ToCityCode").ToString()%>
                        </td>
                        <td>
                            <%# Eval("TimeScope")%>
                        </td>
                        <td>
                            <%# Eval("PointScope").ToString().Replace("|","<br>")%>
                        </td>
                        <td>
                            <%# Eval("SelectType").ToString() == "1" ? "扣点" : Eval("SelectType").ToString() == "2" ? "留点" : "补点"%>
                        </td>
                        <td style="width: 100px;">
                            <a href='DiscountSet.aspx?gdid=<%# Eval("did") %>&groupid=<%# Eval("id") %>&currentuserid=<%=this.mUser.id.ToString() %>'>修改</a>
                            <br />
                            <asp:LinkButton ID="lbtDel" runat="server" CommandArgument='<%# Eval("did") %>' CommandName="Del">删除</asp:LinkButton>
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
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" >
            </webdiyer:AspNetPager>
    </div>
    <div id="showOne">
    </div>
    <div id="ddd">
    </div>
    <input type="hidden" id="hid_groupid" runat="server" />
    </form>
</body>
</html>
