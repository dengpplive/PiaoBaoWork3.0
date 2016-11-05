<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ComPanyList.aspx.cs" Inherits="User_ComPanyList" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>下级开户</title>
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
    <script src="../js/area.js" type="text/javascript"></script>
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

        function moreSearchOrder() {
            if ($("#moreSearchOrder").css("display") == "none") {
                $("#moreSearchOrderA").attr("class", "MoreCondition");
            }
            else {
                $("#moreSearchOrderA").attr("class", "MoreConditionA");
            }
            $("#moreSearchOrder").toggle('slow');
        }
        function yanzheng(obj) {
            //先把非数字的都替换掉，除了数字和.
            obj.value = obj.value.replace(/[^\d.]/g, "");
            //必须保证第一个为数字而不是.
            obj.value = obj.value.replace(/^\./g, "");
            //保证只有出现一个.而没有多个.
            obj.value = obj.value.replace(/\.{2,}/g, ".");
            //保证.只出现一次，而不能出现两次以上
            obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
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
        ul#icons span.ui-icon
        {
            float: left;
            margin: 0 4px;
        }
        #moreSearch th, .Search th
        {
            width: 120px;
        }
    </style>
</head>
<body onload="init();">
    <%----%>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div id="tabs">
        <div class="title">
            <span>用户管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container">
                    <table border="0" cellpadding="0" cellspacing="0" class="Search" style="width: 100%">
                        <tr>
                            <th style="width: 9%;">
                                登录帐号：
                            </th>
                            <td style="width: 12%;">
                                <asp:TextBox ID="txtUnitCode" CssClass=" inputtxtdat" runat="server" MaxLength="16"></asp:TextBox>
                            </td>
                            <th id="dd1" style="width: 11%;">
                                开户时间：
                            </th>
                            <td style="width: 29%;">
                                <input id="StartTimeSta" runat="server" style="width: 100px" class="Wdate inputtxtdat"
                                    enableviewstate="False" onfocus="WdatePicker({maxDate:'#F{$dp.$D(\'StartTimeStp\')}',onpicked:function() {StartTimeStp.focus();}})"
                                    type="text" />-
                                <input id="StartTimeStp" runat="server" style="width: 100px" class="Wdate inputtxtdat"
                                    enableviewstate="False" onfocus="WdatePicker({minDate:'#F{$dp.$D(\'StartTimeSta\')}'})"
                                    type="text" />
                            </td>
                            <th style="width: 13%;">
                                帐户状态：
                            </th>
                            <td style="width: 26%;">
                                <asp:RadioButtonList ID="rblISDJ" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Selected="True" Value="-1">不限</asp:ListItem>
                                    <asp:ListItem Value="0">冻结</asp:ListItem>
                                     <asp:ListItem Value="1">正常</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                公司名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtUnitName" CssClass=" inputtxtdat" runat="server" MaxLength="25"></asp:TextBox>
                            </td>
                             <th>
                                联系人：
                            </th>
                            <td>
                                <asp:TextBox ID="txtContactUser" CssClass=" inputtxtdat" runat="server" MaxLength="25"></asp:TextBox>
                            </td>
                              
                            <th style="width: 13%;">
                                联系电话：
                            </th>
                            <td>
                                <asp:TextBox ID="txtUserPhone" CssClass=" inputtxtdat" runat="server" MaxLength="25"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                省份：
                            </th>
                            <td>
                                <asp:DropDownList ID="province" CssClass=" inputtxtdat" Width="115px" runat="server"
                                    onChange="select()">
                                </asp:DropDownList>
                            </td>
                         <th>
                                城市：
                            </th>
                            <td>
                                <asp:DropDownList ID="city" CssClass=" inputtxtdat" Width="115px" runat="server"
                                    onchange="citySelect();">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="3"></th>
                            <td align="center">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" />
                                </span><span class="btn btn-ok-s">
                                    <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" OnClientClick="clearprovince()" />
                                </span><span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " />
                                </span>
                            </td>
                           
                        </tr>
                        
                    </table>
                </div>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                width="100%">
                <thead>
                    <tr>
                       <th>
                            登录账号
                       </th>
                        <th>
                            公司编号
                        </th>
                        <th>
                            公司名称
                        </th>
                        <th>
                            公司状态
                        </th>
                        <th>
                            账号状态
                        </th>
                        <th>
                            联系人
                        </th>
                        <th>
                            联系电话
                        </th>
                        <th>
                            开户时间
                        </th>
                        <th>
                            登录次数
                        </th>
                        <th>
                            最后登录时间
                        </th>
                        <th>
                            所属业务员
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="Repeater" runat="server" OnItemCommand="Repeater_ItemCommand" 
                    onitemdatabound="Repeater_ItemDataBound">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                          <td style="width: 7%">
                                <%# Eval("LoginName") %>
                          </td>
                            <td style="width: 10%">
                            <%# Eval("UninCode")%>
                            </td>
                            <td style="width: 10%">
                                <%#Eval("UninAllName")%>
                            </td>
                            <td style="width: 7%">
                                <%#Eval("AccountState").ToString() == "1" ? "正常" : "冻结"%>
                            </td>
                             <td style="width: 7%">
                                <%#Eval("State").ToString() == "0" ? "禁用" : "正常"%>
                            </td>
                            <td style="width: 7%">
                                <%#Eval("ContactUser")%>
                            </td>
                            <td style="width: 7%">
                                <%#Eval("ContactTel")%>
                            </td>
                            <td style="width: 7%">
                                <%#Eval("CreateTime")%>
                            </td>
                            <td style="width: 7%">
                                <%# Eval("CountLogin") %>
                            </td>
                            <td style="width: 7%">
                                <%# Eval("LastLoginTime") %>
                            </td>
                            <td style="width: 7%">
                                <%# Eval("ywy") %>
                            </td>
                            <td class="Operation" style="width:7%">
                            <div runat="server" id="Div1" visible = "false">
                                <a href='OpenAccount.aspx?id=<%#Eval("id")%>&userid=<%# Eval("userid") %>&Cpychildid=<%# Eval("CpyNo") %>&currentuserid=<%=this.mUser.id.ToString() %>'>修 改</a><br />
                                <asp:LinkButton ID="UpDateButton" runat="server" Text='<%#Eval("AccountState").ToString() == "1" ? "冻 结" : "解 冻"%>' CommandArgument='<%#Eval("id")%>'
                                    CommandName="Update" OnClientClick=" return  confirm('确定要修改状态?');" /><br />
                                <asp:LinkButton ID="lbtnUpDatePwd" runat="server" Text="密码恢复" CommandArgument='<%#Eval("UninCode")%>'
                                    CommandName="UpDatePwd" OnClientClick=" return  confirm('确定要密码恢复?');"/><br />
                            </div>
                            <div id="Div2" runat="server" visible = "false">
                                <a href='Company.aspx?cpyno=<%#Eval("UninCode")%>&currentuserid=<%=this.mUser.id.ToString() %>'>修 改</a>
                            </div>
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
    </div>
    <input type="hidden" id="hidPro" runat="server" value="" />
    <input type="hidden" id="hidCity" runat="server" value="" />
    </form>
</body>
</html>
