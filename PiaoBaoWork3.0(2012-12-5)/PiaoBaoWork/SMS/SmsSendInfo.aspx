<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SmsSendInfo.aspx.cs" Inherits="SMS_SmsSendInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>发送记录查询</title>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="发送明细" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table id="moreSearch" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <th>
                            发送日期：
                        </th>
                        <td>
                            <input id="txtDateS" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                onfocus="WdatePicker({isShowClear:true})" />-<input id="txtDateE" type="text" readonly="readonly"
                                    runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                        </td>
                        <td>
                            <div class="c-list-filter-go">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tbody>
                                        <tr>
                                            <td align="center" colspan="4">
                                                <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" />
                                                </span><span class="btn btn-ok-s">
                                                    <asp:Button ID="btnReturn" runat="server" Text=" 返 回 " 
                                                    onclick="btnReturn_Click"/>
                                                </span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
           
        </div>
        <table id="tb-all-trade" class="tb-all-trade zhengce" width="100%" cellspacing="0"
            cellpadding="0" border="0">
            <thead>
                <tr>
                   
                    <th>
                        公司名称
                    </th>
                    <th>
                        发送时间
                    </th>
                    <th>
                        发送条数
                    </th>
                    <th>
                        发送内容
                    </th>
                    <th>
                        接收号码
                    </th>
                    <th>
                        发送状态
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="RptShow" runat="server">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <td>
                            <%#Eval("SmsUnit")%>
                        </td>
                        <td>
                            <%# Eval("SmsCreateDate")%>
                        </td>
                        <td style="width: 10%">
                            <%# Eval("SmsUserCount")%>
                        </td>
                        <td class="Details" style="width: 25%" title='<%# Eval("SmsSendContent").ToString() %>'>
                            <%# Eval("SmsSendContent").ToString().Length > 40 ? Eval("SmsSendContent").ToString().Substring(0, 40) + "..." : Eval("SmsSendContent").ToString()%>
                        </td>
                        <td style="width: 25%" title='<%# Eval("SmsAcceptMobilePhone").ToString() %>'>
                            <%# Eval("SmsAcceptMobilePhone").ToString().Length > 40 ? Eval("SmsAcceptMobilePhone").ToString().Substring(0, 40) + "..." : Eval("SmsAcceptMobilePhone").ToString()%>
                        </td>
                        <td>
                            <%# Eval("SmsSendState").ToString() == "0" ? "失败" : "成功"%>
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
    <div id="Show" style="width: 100%; text-align: center; font-size: 12px; line-height: 20px;
        color: Gray; margin-top: 0;" runat="server" visible="false">
        没有符合你要搜索的条件,请输入正确的查询条件！
    </div>
    <input type="hidden" runat="server" id="hiShow" value="0" />
    <div id="showOne">
    </div>
    </form>
</body>
</html>
