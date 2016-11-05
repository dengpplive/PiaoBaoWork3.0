<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OperationPolicy.aspx.cs" Inherits="Policy_OperationPolicy" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/Statements.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        #moreSearch th
        {
            width: 110px;
        }
        .Search th
        {
            width: 110px;
        }
        TABLE
        {
            font-size: 12px;
            line-height: 30px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
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
    <div id="show"></div>
     <div id="tabs">
         <div class="title">
            <span>政策导入导出</span> 
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                Excel路径：
                            </th>
                            <td>
                                <asp:FileUpload ID="FileUpload" runat="server" />
                                <input type="hidden" id="Hid1" runat="server" />
                            </td>
                            <td>
                            <span class="btn btn-ok-s">
                                        <asp:Button ID="btn" runat="server" Text="导入政策" onclick="btn_Click">
                                        </asp:Button>
                                    </span>
                            </td>
                            <td>
                           <span class="btn btn-ok-s">
                                        <asp:Button ID="btndc" runat="server" Text="导出政策" onclick="btndc_Click">
                                        </asp:Button>
                                    </span>
                        </td>
                        </tr>
                        </table>
                </div>
                <div class="c-list-filter-go">
                     <asp:GridView ID="GridView1" runat="server" 
                         onrowdatabound="GridView1_RowDataBound">
                    </asp:GridView>
                </div>
                <div id="talestr" style="width:100%;text-align: center;"></div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
