<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserSumMary.aspx.cs" Inherits="Financial_UserSumMary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
        function OnClickgetUrl(url) {
            window.location.href = url;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="show">
    </div>
    <div id="tabs">
        <div class="title">
            <span runat="server" id='spantitle'></span>
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
                                        </td>
                                        <th>
                                            客户账号：
                                        </th>
                                        <td>
                                            <asp:TextBox ID="txtUserAccount" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
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
                                                 <span class="btn btn-ok-s">
                                                    <asp:Button ID="btndetails" runat="server" Text="查看明细" 
                                                    onclick="btndetails_Click">
                                                    </asp:Button>
                                                </span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                         <div>
                       <asp:GridView ID="gvUserSumMaryInfo" Width="100%" runat="server" EnableViewState="false"
                            EmptyDataText="查无信息！" CssClass="tb-all-trade" 
                                 onrowdatabound="gvUserSumMaryInfo_RowDataBound">
                        </asp:GridView>
                        <div style="display: none">
                            <asp:GridView ID="gvUserSumMaryInfoNew" Width="100%" runat="server" EmptyDataText="查无信息！"
                                CssClass="tb-all-trade">
                            </asp:GridView>
                        </div>
                       </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
