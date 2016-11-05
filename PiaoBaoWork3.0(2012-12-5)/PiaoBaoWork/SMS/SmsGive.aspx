<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SmsGive.aspx.cs" Inherits="SMS_SmsGive" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.4.1.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align:center">
        条数:<asp:TextBox ID="txtcount" runat="server" onkeyup="value=value.replace(/[^0-9]/g,'')" MaxLength="5" ></asp:TextBox><asp:RequiredFieldValidator
            ID="RequiredFieldValidator1" runat="server" ErrorMessage="必填" 
            ControlToValidate="txtcount" ForeColor="Red"></asp:RequiredFieldValidator>
      
    </div>
    <div style="text-align:center">
      <span class="btn btn-ok-s">
            <asp:Button ID="btnok" runat="server" Text="确 定" OnClick="btnQuery_Click" />
        </span>
    </div>
    </form>
</body>
</html>
