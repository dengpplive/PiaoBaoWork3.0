<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseParametersList.aspx.cs"
    Inherits="Manager_Base_BaseParametersList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>基础数据参数管理</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../../js/clipboardData.js" type="text/javascript"></script>
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .Mleft
        {
            text-align: left;
        }
        .style1
        {
            color: #FF3300;
        }
    </style>
    <script type="text/javascript">
        function loadEvt() {
            $("#Table1 tr td").dblclick(function (evt) {
                copy(evt);
            });
        }
    </script>
</head>
<body onload="loadEvt()">
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <div class="title">
            <span>基础数据参数管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                        border="1" style="border-collapse: collapse;">
                        <tr>
                            <th class="wtd">
                                参数名称:
                            </th>
                            <th class="Mleft">
                                <asp:TextBox ID="txtParamName" runat="server" MaxLength="100"></asp:TextBox>
                            </th>
                            <th class="wtd">
                                参数值：
                            </th>
                            <th class="Mleft">
                                <asp:TextBox ID="txtParamValue" runat="server"></asp:TextBox>
                            </th>
                            <th class="wtd">
                            </th>
                            <th>
                            </th>
                        </tr>
                        <tr>
                            <th class="wtd">
                                公司编号：
                            </th>
                            <th class="Mleft">
                                <asp:TextBox ID="txtCompanyNo" runat="server" MaxLength="50"></asp:TextBox>
                                <span class="btn btn-ok-s">
                            <asp:Button ID="btnSelectCompany" runat="server" Text="查询公司编号" 
                                    onclick="btnSelectCompany_Click" /></span>
                            
                            </th>
                         <th class="wtd">
                               <%-- 日期：--%>
                            </th>
                            <td colspan="2">
                              <%--  <div style="text-align: left;">
                                    <input type="text" id="txtStartDate" style="width: 130px;" readonly="true" runat="server"
                                        class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />至
                                    <input type="text" id="txtEndDate" style="width: 130px;" readonly="true" runat="server"
                                        class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                                </div>--%>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr class="hide">
                            <th class="wtd">
                                参数描述:
                            </th>
                            <td>
                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Height="66px"
                                    Width="221px" MaxLength="500"></asp:TextBox>
                            </td>
                            <th>
                            </th>
                            <td>
                            </td>
                            <th>
                            </th>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " /></span><span
                                        class="btn btn-ok-s">
                                        <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" /></span>
                            </td>
                        </tr>
                    </table>
                </div>
                <p class="style1">
                    注: 双击单元格复制内容</p>
                <table id="Table1" class="tb-all-trade" cellspacing="0" cellpadding="0" border="0"
                    width="100%">
                    <thead>
                        <tr>
                            <th>
                                公司编号
                            </th>
                            <th>
                                参数名称
                            </th>
                            <th>
                                参数值
                            </th>
                            <th>
                                参数描述
                            </th>
                         <%--   <th>
                                有效起始日期
                            </th>
                            <th>
                                有效截止日期
                            </th>--%>
                            <th>
                                备注
                            </th>
                            <th>
                                操作
                            </th>
                        </tr>
                    </thead>
                    <asp:Repeater ID="Repeater" runat="server">
                        <ItemTemplate>
                            <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                                <td>
                                    <%#Eval("CpyNo")%>
                                </td>
                                <td>
                                    <%#Eval("SetName")%>
                                </td>
                                <td title='<%# Eval("SetValue") %>' tle='<%#Eval("SetValue")%>'>
                                    <%# DisplayChars(Eval("SetValue"),50,"...")%>
                                </td>
                                <td title='<%#Eval("SetDescription")%>' tle='<%#Eval("SetDescription")%>'>
                                    <%# DisplayChars(Eval("SetDescription"),20,"...")%>
                                </td>
                              <%--  <td>
                                    <%#Eval("StartDate")%>
                                </td>
                                <td>
                                    <%#Eval("EndDate")%>
                                </td>--%>
                                <td title='<%#Eval("Remark")%>' tle='<%#Eval("Remark")%>'>
                                    <%#Eval("Remark")%>
                                </td>
                                <td class="Operation">
                                    <a href="BaseParametersEdit.aspx?id=<%#Eval("Id")%>&currentuserid=<%=this.mUser.id.ToString() %>">修 改 </a>
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
    </div>
    <script type="text/javascript" src="../../js/Tooltip/ToolTip.js"> </script>
    <script type="text/javascript">
        <!--
        initToolTip("td");
    </script>
    </form>
</body>
</html>
