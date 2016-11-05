<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LogOperationlist.aspx.cs" Inherits="Manager_Log_LogOperationlist" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../../js/My97DatePicker/WdatePicker.js"></script>
    <style type="text/css">
        .Search tr
        {
            height: 30px;
            line-height: 30px;
        }
        .tb-all-trade td
        {
            text-align: center;
        }
    </style>
    <script type="text/javascript">
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
    <div id="dd">
    </div>
    <div class="title">
            <span>操作日志查询</span>
        </div>
    <div id="tabs">
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;">
                        <tr>
                            <th>
                                公司编号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCpyNo" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                模块类型：
                            </th>
                            <td>
                                <asp:TextBox ID="txtModuleName" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                操作账号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtLoginName" runat="server"></asp:TextBox>
                            </td>
                              <th>
                                记录时间：
                            </th>
                            <td>
                                <asp:TextBox ID="txtStartTime"  CssClass="Wdate inputtxtdat"  runat="server" class="Wdate" EnableViewState="False"
                                    onfocus="WdatePicker({isShowWeek:false})"></asp:TextBox>
                                    -<asp:TextBox ID="txtEndTime" CssClass="Wdate inputtxtdat"  runat="server" class="Wdate" EnableViewState="False"
                                    onfocus="WdatePicker({isShowWeek:false})"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                            </th>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td colspan="3">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                      <span class="btn btn-ok-s">  <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" /></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                border="1" style="border-collapse: collapse;">
                <thead>
                    <tr>
                        <th>
                            公司编号
                        </th>
                        <th>
                            模块名称
                        </th>
                        <th>
                            操作类型
                        </th>
                        <th>
                            操作账号
                        </th>
                        <th>
                            操作用户名
                        </th>
                        <th>
                            记录时间
                        </th>
                        <th>
                            Ip
                        </th>
                        <th>
                            内容
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repErrorList" runat="server">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                                <%#Eval("CpyNo")%>
                            </td>
                             <td>
                                <%#Eval("ModuleName")%>
                            </td>
                            <td>
                                <%#Eval("OperateType")%>
                            </td>
                            <td>
                                <%#Eval("LoginName")%>
                            </td>
                           <td>
                                <%#Eval("UserName")%>
                           </td>
                           
                            <td>
                                <%#Eval("CreateTime")%>
                            </td>
                          
                            <td>
                                 <%#Eval("ClientIP")%>
                            </td>
                            <td align="left" style="width:300px">
                                 <%#Eval("OptContent")%>
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
    </div>
    </form>
</body>
</html>
