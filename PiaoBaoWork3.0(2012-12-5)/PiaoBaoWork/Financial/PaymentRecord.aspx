<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaymentRecord.aspx.cs" Inherits="Financial_PaymentRecord" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .ui-corner-all
        {
            padding: 1px 6px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            // Tabs
            $('#tabs').tabs();
        });
        function showdialog(t) {
            $("#show").html(t);
            $("#show").dialog({
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
                    }
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="show">
    </div>
    <div id="tabs">
        <div class="title">
            <span runat="server" id='spantitle'></span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                客户名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtUninAllNAME" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                交易时间：
                            </th>
                            <td>
                                <input type="text" id="txtGoAlongTime1" style="width: 110px;" readonly="true" class="inputtxtdat"
                                    runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />--
                                <input type="text" id="txtGoAlongTime2" style="width: 110px;" readonly="true" class="inputtxtdat"
                                    runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                                
                            </td>
                            <th>
                            交易类型：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlpaytype" runat="server">
                                <asp:ListItem Value="-1">所有类型</asp:ListItem>
                                <asp:ListItem Value="1">支付</asp:ListItem>
                                <asp:ListItem Value="2">退款</asp:ListItem>
                                <asp:ListItem Value="3">充值</asp:ListItem>
                                <asp:ListItem Value="4">上调</asp:ListItem>
                                <asp:ListItem Value="5">下调</asp:ListItem>
                                <asp:ListItem Value="6">分润</asp:ListItem>
                                <asp:ListItem Value="7">欠款明细</asp:ListItem>
                                <asp:ListItem Value="8">欠款销账</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询" OnClick="btnQuery_Click" CausesValidation="false">
                                    </asp:Button></span>&nbsp; <span class="btn btn-ok-s">
                                        <asp:Button runat="server" ID="btnPrint" Text="导出Excel" OnClick="btnPrint_Click">
                                        </asp:Button></span>&nbsp;<span class="btn btn-ok-s">
                                            <asp:Button ID="btnreset" runat="server" Text=" 重 置 " CausesValidation="false" OnClick="btnreset_Click">
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
                            客户名
                        </th>
                        <div id="div1" runat="server">
                        <th>
                            订单号
                        </th>
                        <th>
                            PNR
                        </th>
                        <th>
                            之前账户余额
                        </th>
                        </div>
                        <th>
                            交易金额
                        </th>
                        <div id="div2" runat="server">
                        <th>
                            账户余额
                        </th>
                        <th>
                            交易类型
                        </th>
                        </div>
                        <th>
                            交易时间
                        </th>
                        <th>
                            操作员
                        </th>
                         <th id="OperReasonth" runat="server">
                            交易金额说明
                        </th>
                        <th>
                            备注
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repPosList" runat="server" 
                    onitemdatabound="repPosList_ItemDataBound">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                                <%#GetCom(Eval("PayCpyNo").ToString(),Eval("RecCpyNo").ToString(),Eval("PayCpyName").ToString(),Eval("RecCpyName").ToString())%>
                            </td>
                            <div id="div3" runat="server">
                            <td>
                            <div id="Div0" runat="server" visible='<%#Eval("OrderId").ToString().Substring(0, 1) == "0" ? true : false%>'>
                                <a href='../Order/OrderDetail.aspx?orderid=<%# Eval("OrderId")%>&Url=../Financial/PaymentRecord.aspx&currentuserid=<%=this.mUser.id.ToString() %>'><%#Eval("OrderId")%></a>
                            </div>
                                <div id="Div1" runat="server" visible='<%#Eval("OrderId").ToString().Substring(0, 1) == "0" ? false : true%>'>
                                <%#Eval("OrderId")%>
                                </div>
                            </td>
                             <td>
                                <%#Eval("PNR")%>
                            </td>
                            <td>
                                <%# Eval("PreRemainMoney").ToString()%>
                            </td>
                            </div>
                            <td>
                                <%# Eval("PayMoney").ToString()%>
                            </td>
                            <div id="div4" runat="server">
                            <td>
                                <%# Eval("RemainMoney").ToString()%>
                            </td>
                            <td>
                                <%# Eval("childname")%>
                            </td>
                            </div>
                            <td>
                                <%# Eval("OperTime").ToString()%>
                            </td>
                            <td>
                                <%# Eval("OperLoginName")%>
                            </td>
                            <td id="OperReasontd" runat="server" class="Operation">
                                <span title='<%# Eval("OperReason").ToString()%>'>
                                    <%# jieduanStr(Eval("OperReason").ToString())%>
                                </span>
                            </td>
                            <td>
                             <%# Eval("Remark").ToString().Replace("|","<br>").Replace(",","<br>")%>
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
