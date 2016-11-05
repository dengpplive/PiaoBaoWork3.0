<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OffLineList.aspx.cs" Inherits="Financial_OffLineList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        function OnClickgetUrl(url) {
            window.location.href = url;
        }
    </script>
</head>
<body>
   <form id="form1" runat="server">
   <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="show"></div>
     <div id="tabs">
        <div class="title">
            <span>线下收银汇总</span>
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
                                客户账号：
                            </th>
                             <td>
                                <asp:TextBox ID="txtLoginName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                             <th>
                                 出票时间：
                            </th>
                            <td>
                                <input type="text" id="txtGoAlongTime1" style="width: 110px;" readonly="true" class="inputtxtdat"
                                    runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />--
                                <input type="text" id="txtGoAlongTime2" style="width: 110px;" readonly="true" class="inputtxtdat"
                                    runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                            </td>
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询" OnClick="btnQuery_Click" CausesValidation="false">
                                    </asp:Button></span> <span class="btn btn-ok-s">
                                            <asp:Button runat="server" ID="btnPrint" Text="导出Excel" OnClick="btnOut_Click">
                                            </asp:Button></span>
                                            <span class="btn btn-ok-s">
                                        <asp:Button ID="btnreset" runat="server" Text=" 重 置 " CausesValidation="false" OnClick="btnreset_Click">
                                        </asp:Button></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
           <div>
                       <asp:GridView ID="gvOffLineInfo" Width="100%" runat="server"
                            EmptyDataText="查无统计信息！" CssClass="tb-all-trade" 
                           onrowdatabound="gvOffLineInfo_RowDataBound" >
                        </asp:GridView>
                        <div style="display: none">
                            <asp:GridView ID="gvOffLineInfoNew" Width="100%" runat="server" EmptyDataText="查无统计信息！"
                                CssClass="tb-all-trade">
                            </asp:GridView>
                        </div>
                       </div>
        </div>
      
    </div>
    </form>
</body>
</html>
