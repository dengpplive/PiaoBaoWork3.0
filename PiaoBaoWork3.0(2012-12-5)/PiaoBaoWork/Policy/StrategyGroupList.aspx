<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StrategyGroupList.aspx.cs" Inherits="Policy_StrategyGroupList" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
     <div id="tabs">
        <div class="title">
            <span>扣点组列表</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                扣点组名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtGroupName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                          
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询" OnClick="btnQuery_Click" CausesValidation="false">
                                    </asp:Button></span>&nbsp; <span class="btn btn-ok-s">
                                        <asp:Button ID="btnreset" runat="server" Text=" 重 置 " CausesValidation="false" OnClick="btnreset_Click">
                                        </asp:Button></span> &nbsp; <span class="btn btn-ok-s">
                                            <asp:Button ID="btnadd" runat="server" Text=" 添 加 " CausesValidation="false">
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
                            扣点组名称
                        </th>
                        <th>
                            公司名称
                        </th>
                        <th>
                            是否为默认
                        </th>
                        <th>
                            是否协调
                        </th>
                        <th>
                            协调点数
                        </th>
                        <th>
                            操作员
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repPosList" runat="server" 
                    onitemcommand="repPosList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                         <td>
                                <%#Eval("GroupName")%>
                            </td>
                            <td>
                                <%#Eval("CpyName")%>
                            </td>
                            <td>
                                <%# bool.Parse(Eval("DefaultFlag").ToString()) == true ? "默认" : "非默认"%>
                            </td>
                            <td>
                                <%# Eval("UniteFlag").ToString() == "1" ? "是" : "否"%>
                            </td>
                            <td>
                                <%#Eval("UnitePoint")%>
                            </td>
                             <td>
                                <%# Eval("OperLoginName")%>
                            </td>
                            <td class="Operation">
                                <a href='StrategyGroupEdit.aspx?id=<%#Eval("id") %>&currentuserid=<%=this.mUser.id.ToString() %>'>修改</a>
                                    <asp:LinkButton runat="server" ID="btn_Del" CommandName="Del" OnClientClick="return confirm('删除该组会删除所属该组的所有扣点信息,确认删除？');"
                                    CommandArgument='<%# Eval("id") %>' Text="删除"></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
