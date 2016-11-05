<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="SalesStatistics.aspx.cs"
    Inherits="Financial_SalesStatistics" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>客户销售统计</title>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/Statements.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <style type="text/css">
        #moreSearch th
        {
            width: 80px;
        }
        .Search th
        {
            width: 80px;
        }
        TABLE
        {
            font-size: 12px;
            line-height: 30px;
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



        function SelCustomer(vals) {
            if (vals != "") {
                
                var options = document.getElementById("ddlCustomer").options;
                for (var i = 1; i < options.length; i++) {
                    if (options[i].text.indexOf(vals) > -1) {
                        $("#ddlCustomer").val(options[i].value);
                        break;
                    }
                }
            }
            else {
                $("#ddlCustomer").val("");
            }
        }
        function getUninAllname(vals) {
            $("#txtTo").val(vals.options[vals.selectedIndex].text);
        }
        function SelLoingName(vals) {
            if (vals != "") {
                var options = document.getElementById("ddlloginname").options;
                for (var i = 1; i < options.length; i++) {
                    if (options[i].text.indexOf(vals) > -1) {
                        $("#ddlloginname").val(options[i].value);
                        break;
                    }
                }

            }
            else {
                $("#ddlloginname").val("");
            }
        }
        function getLoginName(vals) {
            $("#txtUserAccount").val(vals.options[vals.selectedIndex].text);
        }
        function OnClickgetUrl(url) {
            window.location.href = url;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="show"></div>
    <div id="tabs">
        <div class="title">
            <span>客户销售统计</span>
        </div>
        <div id="tabs-1">
            <table width="100%">
                <tr>
                    <td>
                        <div class="c-list-filter">
                            <div class="container">
                                <table class="Search" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <th>
                                            客户名称：
                                        </th>
                                        <td>
                                        <asp:TextBox ID="txtTo" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                                           <%-- <asp:TextBox ID="txtTo" Width="110px" CssClass="inputtxtdat" runat="server" onkeyup="SelCustomer(this.value)"></asp:TextBox>
                                              <asp:DropDownList ID="ddlCustomer" runat="server" Width="110px" onchange="getUninAllname(this)" EnableViewState="false">
                                              </asp:DropDownList>--%>
                                        </td>
                                        <th>
                                            客户账号：
                                        </th>
                                        <td>
                                         <asp:TextBox ID="txtUserAccount" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                                           <%-- <asp:TextBox ID="txtUserAccount" Width="110px" CssClass="inputtxtdat" runat="server" onkeyup="SelLoingName(this.value)"></asp:TextBox>
                                            <asp:DropDownList ID="ddlloginname" runat="server" Width="110px" onchange="getLoginName(this)" EnableViewState="false">
                                              </asp:DropDownList>--%>
                                        </td>
                                      <th>
                                    支付方式：
                                    </th>
                                    <td>
                                      <asp:DropDownList ID="ddlpaytype" runat="server" Width="115px" DataTextField="ChildName" 
                                DataValueField="ChildID">
                                
                            </asp:DropDownList>
                                    </td>
                                    </tr>
                                    <tr> 
                                      <th>
                                      航空公司：
                                      </th>
                                       <td>
                                       <uc1:SelectAirCode ID="ddlCarrier" ddlWidth="131" runat="server" DefaultOptionText="" DefaultOptionValue="" IsDShowName="false" />
                                       </td>
                                       <th>
                                            出票时间：
                                        </th>
                                        <td >
                                            <input id="txtCPTimeBegin" type="text" readonly="readonly" runat="server"  class="Wdate inputtxtdat"
                                                onfocus="WdatePicker({isShowClear:false,isShowWeek:false})" />-<input id="txtCPTimeEnd" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                                onfocus="WdatePicker({isShowClear:false,isShowWeek:false})" />
                                        </td>
                                   
                                    <th>
                                    出发城市：
                                    </th>
                                    <td>
                                        <asp:TextBox ID="txtStartCity" runat="server" Width="115px"></asp:TextBox>
                                    </td>
                                    </tr>
                                    
                                </table>
                            </div>
                            <div class="c-list-filter-go">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tbody>
                                        <tr>
                                            <td align="left" colspan="4">
                                                <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnQuery1" runat="server" Text=" 查 询 " OnClick="btnQuery1_Click">
                                                    </asp:Button>
                                                </span><span class="btn btn-ok-s">
                                                    <asp:Button ID="btnClear2" runat="server" Text="重置数据" OnClick="btnClear2_Click">
                                                    </asp:Button>
                                                </span><span class="btn btn-ok-s">
                                                    <asp:Button ID="btnOut" runat="server" Text="导出Excel" OnClick="btnOut_Click"></asp:Button>
                                                </span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                       <div>
                       <asp:GridView ID="gvSalesInfo" Width="100%" runat="server" EnableViewState="false"
                            EmptyDataText="查无统计信息！" CssClass="tb-all-trade" 
                               onrowdatabound="gvSalesInfo_RowDataBound">
                        </asp:GridView>
                        <div style="display: none">
                            <asp:GridView ID="gvSalesInfoNew" Width="100%" runat="server" EmptyDataText="查无统计信息！"
                                CssClass="tb-all-trade">
                            </asp:GridView>
                        </div>
                       </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table width="100%" border="0" class="sugges">
        <tr>
            <td class="sugtitle">
                温馨提示：<br />
            </td>
        </tr>
        <tr>
            <td class="sugcontent">
                请使用IE浏览器导出报表。
            </td>
        </tr>
    </table>
     <input type="hidden" id="hidCustomer" runat="server" /> 
      <input type="hidden" id="hidLoginName" runat="server" /> 
    </form>
</body>
</html>
