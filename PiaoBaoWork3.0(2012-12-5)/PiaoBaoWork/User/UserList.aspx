<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserList.aspx.cs" Inherits="Company_UserList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>员工管理</title>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <style type="text/css">
        .s
        {
            border: 1px solid #000000;
            background-color: #666666;
        }
        .paginator
        {
            font: 11px Arial;
            padding: 10px 20px 10px 0; /*&#19978;&#21491;&#19979;&#24038;*/
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
    </style>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">
        function showimg(obj) {
            $("#" + obj).show();
        }
        function hideimg(obj) {
            $("#" + obj).hide();
        }
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
        function showdialog2(t) {
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
        $(function () {
            // Tabs
            $('#tabs').tabs();
            //&#25353;&#29275;&#26679;&#24335;
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); }
				);

        });
        function moreSearchOrder() {
            if ($("#moreSearchOrder").css("display") == "none") {
                $("#moreSearchOrderA").attr("class", "MoreCondition");
            }
            else {
                $("#moreSearchOrderA").attr("class", "MoreConditionA");
            }
            $("#moreSearchOrder").toggle('slow');
        }
   
    </script>
    <style type="text/css">
        /*demo page css*/
        body
        {
            font: 62.5% "Trebuchet MS" , sans-serif;
            margin: 0px;
        }
        #dialog_link
        {
            padding: .4em 1em .4em 20px;
            text-decoration: none;
            position: relative;
        }
        #dialog_link span.ui-icon
        {
            margin: 0 5px 0 0;
            position: absolute;
            left: .2em;
            top: 50%;
            margin-top: -8px;
        }
        #result
        {
            width: 100%;
            height: 600px;
            border: 1px solid #dfeffc;
            margin-top: 10px;
        }
        .demoHeaders
        {
            margin-top: 2em;
        }
        ul#icons
        {
            margin: 0;
            padding: 0;
        }
        ul#icons li
        {
            margin: 2px;
            position: relative;
            padding: 4px 0;
            cursor: pointer;
            float: left;
            list-style: none;
        }
        ul#icons span.ui-icon
        {
            float: left;
            margin: 0 4px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <!-- Tabs -->
        <div class="title" style="width: 98%;">
            <span>员工管理</span>
        </div>
        <table border="0" style="width: 98%;" cellpadding="0" cellspacing="0" class="Search">
            <tr>
                <th>
                    工号：
                </th>
                <td>
                    <asp:TextBox ID="WorkNum" CssClass=" inputtxtdat" runat="server" MaxLength="16"></asp:TextBox>
                </td>
                <th id="dd1">
                    姓名：
                </th>
                <td>
                    <asp:TextBox ID="UserName" CssClass=" inputtxtdat" runat="server" MaxLength="25"></asp:TextBox>
                </td>
                <th>
                    性别：
                </th>
                <td>
                    <asp:RadioButtonList ID="Sex" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True" Value="ALL">全部</asp:ListItem>
                        <asp:ListItem Value="0">男</asp:ListItem>
                        <asp:ListItem Value="1">女</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <th>
                    手机号码：
                </th>
                <td>
                    <asp:TextBox ID="Phone" CssClass=" inputtxtdat" runat="server" MaxLength="11"></asp:TextBox>
                </td>
                <th>
                    部门：
                </th>
                <td>
                    <asp:DropDownList ID="ddlBM" CssClass="inputtxtdat" Width="115px" runat="server" AppendDataBoundItems="true">
                        
                    </asp:DropDownList>
                </td>
                <th>
                    状态：
                </th>
                <td>
                    <asp:RadioButtonList ID="ddlStatus" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="-1" Selected="True">全部</asp:ListItem>
                        <asp:ListItem Value="1">启用</asp:ListItem>
                        <asp:ListItem Value="0">禁用</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <th>
                    证件号码：
                </th>
                <td>
                    <asp:TextBox ID="CertificateNum" CssClass=" inputtxtdat" runat="server" MaxLength="16"></asp:TextBox>
                </td>
                <th id="dd2">
                    联系电话：
                </th>
                <td>
                    <asp:TextBox ID="Tel" runat="server" CssClass=" inputtxtdat" MaxLength="11"></asp:TextBox>
                </td>
                <td align="center" colspan="2">
                    <span class="btn btn-ok-s">
                        <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" />
                    </span><span class="btn btn-ok-s">
                        <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" />
                    </span><span class="btn btn-ok-s">
                        <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " OnClick="btnAdd_Click"></asp:Button></span>
                </td>
            </tr>
        </table>
        <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
            width="98%">
            <thead>
                <tr>
                    <th style="width: 8%;">
                        用户姓名
                    </th>
                    <th style="width: 11%;">
                        登录账号
                    </th>
                    <th style="width: 7%;">
                        状态
                    </th>
                    <th style="width: 8%;">
                        工号
                    </th>
                    <th style="width: 8%;">
                        联系电话
                    </th>
                    <th style="width: 8%;">
                        手机号码
                    </th>
                    <th style="width: 8%;">
                        Email
                    </th>
                    <th style="width: 8%;">
                        创建日期
                    </th>
                     <th style="width: 8%;">
                       到期日期
                    </th>
                     <th style="width: 8%;">
                        登录次数
                    </th>
                     <th style="width: 8%;">
                       最后登录时间
                    </th>
                    <th style="width: 10%;">
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="Repeater" runat="server" OnItemCommand="Repeater_ItemCommand">
                <ItemTemplate>
                    <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                        <td>
                                <%# getStr(Eval("UserName").ToString(), 8)%>
                        </td>
                        <td>
                            <%# getStr(Eval("LoginName").ToString(), 8)%>
                        </td>
                        <td>
                            <%# Eval("State").ToString() == "0" ? "<span style='color:red'>禁用</span>" : "启用"%>
                        </td>
                        <td>
                            <%# getStr(Eval("WorkNum").ToString(), 10)%>
                        </td>
                        <td>
                            <%# getStr(Eval("Tel").ToString(), 13)%>
                        </td>
                        <td title='<%#Eval("Phone")%>'>
                            <%# getStr(Eval("Phone").ToString(), 11)%>
                        </td>
                        <td title='<%#Eval("Email")%>'>
                            <%# getStr(Eval("Email").ToString(),10)%>
                        </td>
                        <td>
                            <%# getTime(Eval("StartTime").ToString())%>
                        </td>
                           <td>
                            <%# getTime(Eval("OverDueTime").ToString())%>
                        </td>
                        <td>
                            <%# Eval("CountLogin") %>
                        </td>
                        <td>
                            <%# Eval("LastLoginTime") %>
                        </td>
                        <td class="Operation">
                            <a href="AddUser.aspx?id=<%#Eval("Id")%>&currentuserid=<%=this.mUser.id.ToString() %>">修 改 </a>
                            <br />
                            <asp:LinkButton ID="UpDateButton" runat="server" Text='<%#Eval("State").ToString() == "1" ? "冻 结" : "解 冻"%>' CommandArgument='<%#Eval("id")%>'
                                CommandName="Update" OnClientClick=" return  confirm('确定要修改状态?');" />
                            <br />
                            <asp:LinkButton ID="lbtnUpDatePwd" runat="server" Text="密码恢复" CommandArgument='<%#Eval("id")%>'
                                CommandName="UpDatePwd" OnClientClick=" return  confirm('确定要密码恢复?');" />
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
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    <div id="dd">
    </div>
    </form>
</body>
</html>
