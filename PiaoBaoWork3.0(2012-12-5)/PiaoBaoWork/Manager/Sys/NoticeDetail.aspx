<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NoticeDetail.aspx.cs" Inherits="Sys_NoticeDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>公告</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
     <table width="100%">
        <tr>
            <td align="center">
                <asp:Label ID="Title" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="#3399FF"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="color: Red" align="right">
                发布时间：<asp:Label ID="ReleaseTime" runat="server" ForeColor="Red"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 发布人姓名：<asp:Label ID="Name" runat="server" ForeColor="Red"></asp:Label>
                <hr style="border-bottom-style: dotted; border-left-style: dotted; border-right-style: dotted;
                    border-top-style: dotted" color="#848484" size="1" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <div id="attach" runat="server">
                   <%-- <asp:LinkButton ID="lbdownfile" runat="server" onclick="lbdownfile_Click">下载附件</asp:LinkButton>--%>
                </div>
            </td>
        </tr>
        <tr>
            <td align="left">
                <div runat="server" id="Content">
                
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
