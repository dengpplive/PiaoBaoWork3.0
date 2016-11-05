<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="CreateOrderPayDetail: 确定订单时:生成订单账单明细"
            OnClick="Button1_Click" />
        <br />
        <br />
        <br />
         <asp:TextBox ID="TextBox5" runat="server"></asp:TextBox>
        <asp:Button ID="Button9" runat="server" Text="通过订单号，计算订单金额" 
            onclick="Button9_Click" />
        <br />
        <br />
        <asp:Button ID="Button2" runat="server" Text="UpdateOrderPayDetail:  切换支付方式：修改订单账单明细数据"
            OnClick="Button2_Click" />
        <br />
        <br />
        <asp:Button ID="Button3" runat="server" Text="payRequest:  财付通" OnClick="Button3_Click" />
        <br />
        <br />
        <asp:Button ID="Button4" runat="server" Text="数据处理" OnClick="Button4_Click" />
        <br />
        <br />
        订单ID:<asp:TextBox ID="TextBox2" runat="server" Width="275px" Text="C121DF14-0DFE-4368-823F-70F8FC0B4D4B"></asp:TextBox>
        <br />
        订单号:<asp:TextBox ID="TextBox3" runat="server" Width="275px" Text="0130109173642630248"></asp:TextBox>
        &nbsp;<asp:Button ID="Button7" runat="server" Text="订单支付" OnClick="Button7_Click" />
        <br />
        <br />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button5" runat="server" Text="登录密码加密" OnClick="Button5_Click" />
        <br />
        <br />
        <br />
        <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
        <asp:Button ID="Button8" runat="server" Text="测试" OnClick="Button8_Click" />
        <br />
        <br /><br />
         <asp:TextBox ID="TextBox6" runat="server"></asp:TextBox>
        <asp:Button ID="Button6" runat="server" Text="邮箱验证" onclick="Button6_Click"  />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
