﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BillOfFz.aspx.cs" Inherits="Bill_BillOfFz" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/Statements.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <style type="text/css">
        tr td .tds
        {
            color: red;
        }
        #moreSearch th
        {
            width: 90px;
        }
        .Search th
        {
            width: 90px;
        }
        TABLE
        {
            font-size: 12px;
        }
        .xiaoshou td, .tb-all-trade1 td, .tb-all-trade1 th
        {
            width: 4%;
        }
        .tb-all-trade1 tbody td
        {
            border-bottom: 1px solid #D2D2D2;
            text-align: center;
        }
        .tb-all-trade1 thead th, .tb-all-trade1 thead td
        {
            background: none repeat scroll 0 0 #F5F5F5;
            border-bottom: 1px solid #D2D2D2;
            padding: 2px 3px;
        }
        .tb-all-trade th, .tb-all-trade td
        {
            white-space: nowrap;
        }
        
        
        .table1
        {
            width: 2000px;
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            border: none;
        }
        .table1 td, .table1 th
        {
            width: 3%;
            border-bottom: 1px solid #D2D2D2;
            border-left: none;
            border-right: 1px solid #D2D2D2;
            border-top: none;
            margin: 0px;
            padding: 0px;
            text-align: center;
            border-collapse: collapse;
        }
        .table1 th
        {
            background-color: #EFF4F8;
        }
        
        .table1 #td1, .table1 #td2, .table1 #td3, .table1 #td4, .table1 #td5, .table1 #td6, .table1 #td7, .table1 #td8
        {
            color: Blue;
        }
        
        .table1 #td9, .table1 #td10, .table1 #td11, .table1 #td12, .table1 #td13, .table1 #td14
        {
            color: Green;
        }
        
        .table1 #td15, .table1 #td16, .table1 #td17, .table1 #td18, .table1 #td19, .table1 #td20
        {
            color: Red;
        }
        
        .table2
        {
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            border: none;
        }
        .table1 #trend
        {
            color: red;
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
        function OpenOrclose(strValue, index, evt) {
            var colspancount = document.getElementById("hidcolspancount").value;
            var _target = evt.srcElement ? evt.srcElement : evt.target;
            var id = _target.parentNode.parentNode.rowIndex;
            var myTable = document.getElementById("gvTicketSellCount");
            var aid = "a" + index;
            var txtvalue = $("#" + aid).text();
            var trid = "tr" + index;
            strValue = unescape(strValue);
            if (txtvalue == "田") {
                $("#" + aid).text('曰');
                //显示
                if (document.getElementById(trid) != null && document.getElementById(trid) != undefined) {
                    document.getElementById(trid).style.display = "block";
                } else {
                    var otr = myTable.insertRow(id + 1);
                    otr.id = "tr" + index;
                    var ocell = otr.insertCell(0);
                    ocell.colSpan = colspancount;
                    ocell.style.width = "2000px";
                    ocell.style.border = "0";
                    $(ocell).html(strValue);
                }
            } else {
                $("#" + trid).remove();
                $("#" + aid).text('田');
            }
        }
        function OnClickgetUrl(url) {
            window.open(url, "_blank");
        }
    </script>
    <script type="text/javascript">

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
     <%--城市控件开始--%>
    <link type="text/css" href="../css/smoothness/CityStyle.css" rel="stylesheet" />
     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
     <script src="../js/newcitydata.js" type="text/javascript" charset="gb2312"></script>
    <script src="../js/j.suggest.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $("#txtStart").suggest(citys, { hot_list: commoncitys, dataContainer: '#cityArr_Word', attachObject: '#suggest', iframeUrl: "../blank.htm", HidId: "hiStart" });
            $("#txtConn").suggest(citys, { hot_list: commoncitys, dataContainer: '#cityArr_Middle', attachObject: '#Middle_results', iframeUrl: "../blank.htm", HidId: "hiConn" });
            $("#txtTarget").suggest(citys, { hot_list: commoncitys, dataContainer: '#cityArr_ToCity', attachObject: '#To_results', iframeUrl: "../blank.htm", HidId: "hiTarget" });
        })
    </script>
    <%--城市控件结束--%>
</head>
<body>
   <form id="form1" runat="server">
    <div id="tabs">
        <div class="title">
            <span id="lblShow">分账报表</span>
        </div>
        <div id="tabs-1">
            <div class="container">
                <table class="Search" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <th>
                            订单编号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtOrderId" Width="138px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            PNR：
                        </th>
                        <td>
                            <asp:TextBox ID="txtPNR" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                          <th>
                           (出/退)票时间：
                        </th>
                        <td colspan="3">
                            <input type="text" id="cptimestart" style="width: 110px;" readonly="true" class="inputtxtdat"
                                runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />--
                            <input type="text" id="cptimeend" style="width: 110px;" readonly="true" class="inputtxtdat"
                                runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                        
                        </td>
                       
                    </tr>
                    <tr>
                        <th>
                            支付方式：
                        </th>
                        <td>
                            <asp:DropDownList ID="ddlPayWay" CssClass="inputtxtdat" runat="server" Width="115px">
                            </asp:DropDownList>
                        </td>
                         <th>
                              出发城市：</th>
                        <td>
                          <input name="cityArr_Word" type="hidden" id="cityArr_Word" />
                            <asp:TextBox ID="txtStart" CssClass="inputtxtdat" runat="server" onkeyup="this.focus()"
                                Style="width: 110px;" onkeydown="return keydownsearch(event);"></asp:TextBox>
                            <div id="suggest" class="ac_results">
                            </div>
                            <input id="hiStart" name="hiStart" type="hidden" runat="server" style="width: 100px;" />
                        </td>
                       
                         <th>
                            支付时间：
                        </th>
                        <td colspan="3">
                            <input type="text" id="txtPayTime1" style="width: 110px;" readonly="true" class="inputtxtdat"
                                runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />--
                            <input type="text" id="txtPayTime2" style="width: 110px;" readonly="true" class="inputtxtdat"
                                runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                        </td>
                       
                    </tr>
                    <tr>
                        <th>
                            机票状态：
                        </th>
                        <td>
                            <asp:DropDownList ID="ddlTicketState" CssClass="inputtxtdat" runat="server" Width="115px"
                                AppendDataBoundItems="true" DataTextField="ChildName" DataValueField="ChildID">
                                <asp:ListItem Value="">不限</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                      
                         <th>
                          
                            到达城市：</th>
                        <td>
                           <asp:TextBox ID="txtTarget" runat="server" CssClass="inputtxtdat" onkeyup="this.focus()"
                                onkeydown="return keydownsearch(event);" Style="width: 110px;"></asp:TextBox>
                            <input name="cityArr_ToCity" type="hidden" id="cityArr_ToCity" />
                            <div id="To_results" class="ac_results">
                            </div>
                            <input id="hiTarget" name="hiTarget" type="hidden" runat="server" style="width: 100px;" />
                        </td>
                       
                       <th>
                            创建时间：
                        </th>
                        <td colspan="3">
                            <input type="text" id="txtCreateTime1" style="width: 110px;" readonly="true" class="inputtxtdat"
                                runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />--
                            <input type="text" id="txtCreateTime2" style="width: 110px;" readonly="true" class="inputtxtdat"
                                runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                           
                        </td>
                       
                    </tr>
                    <tr runat="server" id="showtryy">
                        <th>
                              政策来源：</th>
                        <td>
                        <asp:DropDownList ID="ddlPolicySource" Width="115" runat="server">
                            </asp:DropDownList>
                        </td>
                         <th>
                            客户名称：
                        </th>
                        <td>
                            <asp:TextBox ID="txtCustomer" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                         <th>
                            客户账号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtLoginName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        
                    </tr>
                    <tr>
                        <th>
                            承运人：</th>
                        <td>
                             <uc1:SelectAirCode ID="ddlCarrier" CssClass="inputtxtdat" DefaultOptionText="" DefaultOptionValue="" IsDShowName="false" runat="server" />
                        </td>
                          <th>
                              每页显示条数：
                        </th>
                        <td align="center">
                            <select id="selPageSize" style="width: 115px" runat="server">
                                <option value="20">20</option>
                                <option value="50">50</option>
                                <option value="100">100</option>
                            </select>
                        </td>
                            <th>
                            票号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtTicketNum" CssClass="inputtxtdat" runat="server" MaxLength="14"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th runat="server" id="OrderSourceth">
                            政策类型：
                        </th>
                        <td runat="server" id="OrderSourcetd">
                            <asp:RadioButtonList ID="rbtlOrderS" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Selected="True" Value="0" Text="B2B/BSP"></asp:ListItem>
                                <asp:ListItem Value="1" Text="B2B"></asp:ListItem>
                                <asp:ListItem Value="2" Text="BSP"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                          <th runat="server" id="cpuserth">
                            操作员：
                        </th>
                        <td runat="server" id="cpusertd">
                            <asp:TextBox ID="txtCPUser" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                        <div align="right">
                        <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click"></asp:Button>
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click"></asp:Button>
                            </span>
                        </div>
                            
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    
           
           
    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">
        <tr>
            <td style="width: 100%;">
                <label runat="server" id="lal3" visible="false">
                    <strong style="color: #0C88BE;">分账明细</strong></label>
                <asp:LinkButton ID="lbtnDc3" runat="server" OnClick="lbtnDc3_Click" Visible="false">导出Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td style="width: 100%;">
                <asp:GridView ID="gvTicketDetail" Width="100%" runat="server" OnRowDataBound="gvTicketDetail_RowDataBound"
                    EmptyDataText="查无机票信息明细！" CssClass="tb-all-trade">
                </asp:GridView>
                <div style="display: none">
                    <asp:GridView ID="gvTicketDetailNew" Width="100%" runat="server" EmptyDataText="查无机票信息明细！"
                        CssClass="tb-all-trade" onrowdatabound="gvTicketDetailNew_RowDataBound">
                    </asp:GridView>
                </div>
                <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                    CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="10" PagingButtonSpacing="3px"
                    PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                    AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                    EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                    ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                    SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
                </webdiyer:AspNetPager>
            </td>
        </tr>
    </table>
    <table style="width: 100%;" border="0" class="sugges">
        <tr>
            <td class="sugtitle">
                温馨提示：<br />
            </td>
        </tr>
        <tr>
            <td class="sugcontent">
                1.请使用IE浏览器导出报表。</td>
        </tr>
    </table>
    <div id="show">
    </div>
    <input id="hidId" type="hidden" runat="server" />
    <input id="hidcolspancount" type="hidden" runat="server" />
    </form>
</body>
</html>
