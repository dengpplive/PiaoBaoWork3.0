<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SMSRateSetManage.aspx.cs" Inherits="SMS_SMSRateSetManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link href="../js/My97DatePicker/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.4.1.js"></script>
    <script type="text/jscript">
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd"></div>
    <div id="tabs">
        <div class="title">
            <span>短信销售参数列表</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                状态：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlState" runat="server" Width="130px">
                                <asp:ListItem Value="">所有</asp:ListItem>
                                <asp:ListItem Value="0">禁用</asp:ListItem>
                                <asp:ListItem Value="1">启用</asp:ListItem>
                            </asp:DropDownList>
                            </td>
                          
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询" OnClick="btnQuery_Click" CausesValidation="false">
                                    </asp:Button></span>&nbsp;<span class="btn btn-ok-s">
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
                            <th style="width:10%;">
                                金额
                            </th>
                            <th style="width:10%;">
                                条数
                            </th>
                            <th style="width:10%;">
                                单价
                            </th>
                            <th style="width:20%;">
                                备注
                            </th>
                            <th style="width:10%;">
                                状态
                            </th>
                            <th style="width:20%;">
                                时间
                            </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repList" runat="server" 
                    onitemcommand="repList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                         <td>
                                    <%--金额--%><%#Eval("RatesMoney")%>元
                                </td>
                                <td>
                                    <%--条数--%><%#Eval("RatesCount")%>条
                                </td>
                                <td>
                                    <%--单价--%><%#Eval("RatesUnitPrice")%>元
                                </td>
                                <td>
                                    <%--备注--%><%#Eval("RatesRemark")%>
                                </td>
                                <td>
                                    <%--状态--%><%#Eval("RatesState").ToString() == "1" ? "启用" : "禁用"%>
                                </td>
                                <td>
                                    <%--时间--%><%#Eval("RatesDateTime")%>
                                </td>
                            <td class="Operation">
                                <a href='SMSRateSetEdit.aspx?id=<%#Eval("id") %>&currentuserid=<%=this.mUser.id.ToString() %>'>修改</a>
                                    <asp:LinkButton runat="server" ID="btn_Del" CommandName="Del" OnClientClick="return confirm('确认删除？');"
                                    CommandArgument='<%# Eval("id") %>' Text="删除"></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
