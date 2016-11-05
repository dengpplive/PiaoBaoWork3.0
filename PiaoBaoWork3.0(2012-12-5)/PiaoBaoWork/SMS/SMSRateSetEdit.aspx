<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SMSRateSetEdit.aspx.cs" Inherits="SMS_SMSRateSetEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        #table_info .td
        {
            width: 120px;
            text-align: right;
            font-size: 12px;
        }
        #table_info span
        {
            font-size: 12px;
        }
        .alignLeft
        {
            text-align: left;
        }
        .Panelbg
        {
            background-color: White;
        }
        .style1
        {
            color: Red;
        }
    </style>
      <script language="javascript" type="text/javascript">
          //验证输入数据
          function ValidatorData() {
              var IsOK = true;
              try {
                  //添加短信模板验证-------------------Start-----------------------------------------
               
                  if ($.trim($("#SmsRatesCount").val()) == "") {
                      showdialog("<font style='color:red;' >* 条数为必填项,不能为空!</font>");
                      return IsOK = false;
                  }

                  if ($.trim($("#SmsRatesUnitPrice").val()) == "") {
                      showdialog("<font style='color:red;' >* 单价为必填项,不能为空!</font>");
                      return IsOK = false;
                  }
                  if ($.isNaN($.trim($("#SmsRatesCount").val()))) {
                      showdialog("<font style='color:red;' >* 条数必须为数字</font>");
                      return IsOK = false;
                  }

                  if ($.isNaN($.trim($("#SmsRatesUnitPrice").val()))) {
                      showdialog("<font style='color:red;' >* 单价必须为数字</font>");
                      return IsOK = false;
                  }
                  //添加短信模板验证-------------------End-----------------------------------------
              } catch (e) {
                  showdialog("<font style='color:red;' >" + e.message + "</font>");
                  IsOK = false;
              }
              return IsOK;
          }

          function showdialog(t) {
              $("#dd").html(t);
              $("#dd").dialog({
                  title: '标题',
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

          function showdialog3(t) {
              $("#dd").html(t);
              $("#dd").dialog({
                  title: '标题',
                  bgiframe: true,
                  height: 140,
                  modal: true,
                  overlay: {
                      backgroundColor: '#000',
                      opacity: 0.5
                  },
                  buttons: {
                      '确定': function () {
                          location.href = "SMSCompanyAccoutManage.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
                      }
                  }
              });
          }
    </script>
</head>
<body>
    <form id="form1" runat="server">
     <div id="dd">
    </div>
     <div class="title">
            <span>短信销售参数编辑</span>
        </div>
    <div id="tabs" class="infomain">
        <div class="mainPanel">
            <table width="100%" border="1" style="border-collapse: collapse;" cellpadding="0"
                cellspacing="0" id="table_info" class="table_info">
                <tr>
                   <%-- <td style="width: 15%; text-align: right;">
                         <span class="style1">*</span>金额：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="SmsRatesMoney" runat="server" CssClass="txt" Width="120px"></asp:TextBox>
                    </td>--%>
                    <td style="width: 15%; text-align: right;">
                        <span class="style1">*</span>条数：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="SmsRatesCount" CssClass="txt" runat="server" Width="120px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 15%; text-align: right;">
                        <span class="style1">*</span>单价：
                    </td>
                    <td class="alignLeft">
                        <asp:TextBox ID="SmsRatesUnitPrice" CssClass="txt" runat="server" Width="120px"></asp:TextBox>元
                    </td>
                   
                </tr>
                <tr>
                 <td style="width: 15%; text-align: right;">
                        <span class="style1">*</span>状态：
                    </td>
                    <td class="alignLeft">
                        <asp:RadioButtonList ID="rblSmsRatesState" runat="server" 
                            RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True" Value="1">启用</asp:ListItem>
                        <asp:ListItem Value="0">禁用</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td style="width: 15%; text-align: right;">
                        备注：
                    </td>
                    <td colspan="2" class="alignLeft">
                        <asp:TextBox ID="SmsRatesRemark" CssClass="txt" runat="server" Width="400px"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="btnSubmit" runat="server" class="btn btnNormal" Text="保  存 "
                            OnClientClick="return ValidatorData();" OnClick="btnSubmit_Click"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="btnback" runat="server" class="btn btnNormal" Text="返回"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="htmlScript" runat="server">
    </div>
    </form>
</body>
</html>
