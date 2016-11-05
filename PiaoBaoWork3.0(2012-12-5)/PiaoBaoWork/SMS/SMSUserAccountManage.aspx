<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SMSUserAccountManage.aspx.cs"
    Inherits="SMS_SMSUserAccountManage" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>客户账户管理</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <style type="text/css">
    .dxzs{}
    </style>
    <script type="text/javascript">
        //动态创建div
        function DynCreateDiv(id) {
            var div = document.getElementById(id);
            if (div == null) {
                div = document.createElement("div");
                div.id = id;
                if (document.all) {
                    document.body.appendChild(div);
                }
                else {
                    document.insertBefore(div, document.body);
                }
            }
            return div;
        }
        function showHtmldiv(html, w, h) {
            DynCreateDiv("ddv");
            jQuery("#ddv").html(html);
            jQuery("#ddv").dialog({
                title: '短信赠送',
                bgiframe: true,
                height: h,
                width: w,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {

                }
            });
            //防止出现按钮
            jQuery("#ddv").dialog("option", "buttons", {});
        }
        $(function () {
            $("a.dxzs").bind("click", function () {
                var cpyno = $(this).attr("metacpyno");
                var url = "SmsGive.aspx?cpyno=" + cpyno + "&currentuserid=<%=this.mUser.id.ToString() %>";
                showHtmldiv("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='no' allowtransparency='yes' width='100%' style='margin:0px;padding:0px;' height='100%' src='" + url + "'/>", 300, 200);
            })
        })
    </script>
</head>
<body>
    
    <form id="form1" runat="server">
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="客户账户管理" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table id="moreSearch" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        
                        <th style="width: 100px;">
                            公司名称：
                        </th>
                        <td>
                            <asp:TextBox ID="txtCpyName" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <div class="c-list-filter-go">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tbody>
                                        <tr>
                                            <td align="center">
                                                <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " 
                                                    OnClick="btnQuery_Click" />
                                                </span>
                                                <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" />
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
                        剩余条数
                    </th>
                    <th>
                        发送条数
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="RptShow" runat="server">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <td class="Details">
                            <%# Eval("UninAllName").ToString()%>
                        </td>
                        <td>
                            <%# Eval("SmsRemainCount").ToString().Length == 0 ? "0" : Eval("SmsRemainCount")%>
                        </td>
                        <td>
                            <%#  Eval("SmsSendCount").ToString().Length == 0 ? "0" : Eval("SmsSendCount")%>
                        </td>
                        <td>
                            <a href="SmsSendInfo.aspx?cpyno=<%# Eval("CpyNo").ToString() %>&flag=1&currentuserid=<%=this.mUser.id.ToString() %>">发送查询 </a>
                            <br />
                            <a href="Transaction.aspx?cpyno=<%# Eval("CpyNo").ToString() %>&flag=1&currentuserid=<%=this.mUser.id.ToString() %>">充值查询 </a>
                            <br />
                            <a href="#" class="dxzs" metacpyno="<%# Eval("CpyNo") %>">短信赠送</a>

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
