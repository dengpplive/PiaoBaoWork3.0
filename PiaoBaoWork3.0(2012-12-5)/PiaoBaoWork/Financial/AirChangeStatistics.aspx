<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AirChangeStatistics.aspx.cs" Inherits="Financial_AirChangeStatistics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/Statements.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
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
    <div id="show"></div>
    <div id="tabs">
        <div class="title">
            <span>航变统计</span>
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
                                            账号：
                                        </th>
                                        <td>
                                         <asp:TextBox ID="txtUserAccount" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                                        </td>
                                      <th>
                                            时间范围：
                                        </th>
                                        <td >
                                            <input id="txtTimeBegin" type="text" readonly="readonly" runat="server"  class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:false,isShowWeek:false})" />-
                                            <input id="txtTimeEnd" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:false,isShowWeek:false})" />
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
                       <asp:GridView ID="gvinfo" Width="100%" runat="server"
                            EmptyDataText="查无统计信息！" CssClass="tb-all-trade">
                        </asp:GridView>
                        
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
    </form>
</body>
</html>
