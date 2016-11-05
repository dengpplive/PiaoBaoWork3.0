<%@ Page Language="C#" AutoEventWireup="true" CodeFile="drawBillTimel.aspx.cs" Inherits="Policy_drawBillTimel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>出票时间设置</title>
         <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
      <script type="text/javascript">
          function showdialogmsg(t) {
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
    <style type="text/css">
        .style1
        {
            width: 60px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <table id="table_info" class="table_info" width="100%" cellspacing="0" cellpadding="0"
                border="0">
                <tr>
                    <td colspan="6" class="bt">
                        出票时间设置
                    </td>
                </tr>
                   <tr>
                   <td style="width:200px;"></td>
                 <td style="width:60px; text-align:right;">本地B2B:</td>
                        <td style="width:60px;">
                       <asp:TextBox ID="txtB2B" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td  style="width:80px;">
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                        <td class="style1">
                         <asp:Label ID="lblB2B" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;"> 本地BSP:</td>
                        <td style="width:60px;">
                        <asp:TextBox ID="txtBSP" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lblBSP" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;">517:</td>
                         <td style="width:60px;">
                       <asp:TextBox ID="txt517" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lbl517" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;"> 百拓:</td>
                        <td style="width:60px;">
                       <asp:TextBox ID="txtBaiTuo" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lblBaiTuo" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;">八千翼:</td>
                        <td style="width:60px;">
                        <asp:TextBox ID="txt8000Y" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lbl8000Y" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;"> 今日:</td>
                         <td style="width:60px;">
                       <asp:TextBox ID="txtToday" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lblToday" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;"> 票盟:</td>
                         <td style="width:60px;">
                       <asp:TextBox ID="txtPiaoMen" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lblPiaoMen" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;">51Book:</td>
                        <td style="width:60px;">
                        <asp:TextBox ID="txt51Book" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lbl51Book" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;"> 共享政策:</td>
                       <td style="width:60px;">
                       <asp:TextBox ID="txtGongXiang" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lblGongXiang" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
                   <tr>
                    <td style="width:200px;"></td>
                    <td style="width:60px; text-align:right;"> 易行:</td>
                        <td style="width:60px;">
                       <asp:TextBox ID="txtYeeXing" runat="server" Width="60px"></asp:TextBox>
                        </td>
                             <td>
                        <span style="color:Red;width:60px;">参考时间</span>
                        </td>
                         <td class="style1">
                         <asp:Label ID="lblYeeXing" runat="server" Text=""></asp:Label>
                        </td>
                          <td style="width:200px;"></td>
                </tr>
               
    </table>
    <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btnNormal" OnClick="lbtnOk_Click"
                            >保  存</asp:LinkButton>
                    </td>
                </tr>
            </table>
    </div>
       <div id="show">
    </div>
    </form>
</body>
</html>
