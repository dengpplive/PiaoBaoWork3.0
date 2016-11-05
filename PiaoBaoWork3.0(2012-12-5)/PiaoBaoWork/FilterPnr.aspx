<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FilterPnr.aspx.cs" Inherits="FilterPnr" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 101px;
        }
        .style2
        {
            width: 381px;
        }
        .style3
        {
            width: 60px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    <table style="width: 100%;">
        <tr>
            <td class="style1">
                &nbsp;
            </td>
            <td class="style2">
                待筛选PNR:
            </td>
            <td class="style3">
                &nbsp;
            </td>
            <td>
                筛选结果（系统中的PNR）:
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="style1">
                &nbsp;
            </td>
            <td class="style2">
                <textarea id="txtPnrs" runat="server" cols="50" name="S1" rows="20"></textarea>
            </td>
            <td class="style3">
                <asp:Button ID="btnFilter" runat="server" Text="筛选&gt;&gt;" OnClick="btnFilter_Click" />
            </td>
            <td>
                <textarea id="txtResult" runat="server" cols="50" name="S2" rows="20"></textarea>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="style1">
                &nbsp;
            </td>
            <td class="style2">
                &nbsp;
            </td>
            <td class="style3">
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
