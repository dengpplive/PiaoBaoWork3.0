<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AccountChange.aspx.cs" Inherits="Financial_AccountChange" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
     <div id="show">
     </div>
     <div id="tabs">
        <div class="title">
            <span><asp:Label ID="lblUninAllName" runat="server" Text="Label"></asp:Label>-账户设置</span>
        </div>
            <div id="tabs-1">
                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table_info"
                id="table_info">
               
                <tr>
                    <th style="width: 50%;">
                        账户余额设置
                    </th>
                  <%--  <th style="width: 50%;">
                        欠款设置
                    </th>--%>
                </tr>
                <tr>
                   <%-- <td id="Storedtd2" style=" display:none; text-align:center;color:Red;font-size:20px; font-weight:bold;">未开启此功能</td>--%>
                    <td id="Storedtd">
                        <table width="100%">
                            <tr>
                                <td class="td">
                                   账户余额：
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblBeforehandFund" runat="server" Text="Label"></asp:Label>
                                </td>
                               
                            </tr>
                            <tr>
                            <td class="td">
                                    调整方式：
                                </td>
                            <td align="left">
                                     
                                       <asp:RadioButtonList ID="rblState" runat="server" CssClass="rblState" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0" Selected="True">充值</asp:ListItem>
                            <asp:ListItem Value="1">代扣</asp:ListItem>
                        </asp:RadioButtonList>
                              
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    调整金额：
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtFee" CssClass="txt" runat="server" ValidationGroup="Fee"></asp:TextBox>元
                                  <asp:RegularExpressionValidator ID="valeAddAmount" runat="server" ControlToValidate="txtFee"
                                        Display="Dynamic" ErrorMessage="调整金额必须为数字" ValidationExpression="^\d+(\.\d+)?$"
                                        ForeColor="red" ValidationGroup="Fee">调整金额必须为数字</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFee"
                                        Display="Dynamic" ErrorMessage="调整金额不能为空" ForeColor="red" ValidationGroup="Fee">不能为空</asp:RequiredFieldValidator>
                                </td>
                
                            </tr>
                            <tr>
                            <td class="td">
                                    调整理由：
                                </td>
                            <td align="left">
                                    <asp:TextBox ID="txtReason" CssClass="txt" runat="server" TextMode="MultiLine" Width="80%"
                                        Height="50px" ValidationGroup="Fee"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtReason"
                                        Display="Dynamic" ErrorMessage="调整理由不能为空" ForeColor="red" ValidationGroup="Fee">不能为空</asp:RequiredFieldValidator>
                                 
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: center;">
                                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btnNormal" OnClick="LinkButton1_Click"
                                        OnClientClick="return true; " ValidationGroup="Fee">保 存</asp:LinkButton>
                      
                                </td>
                            </tr>
                        </table>
                    </td>
                        
                </tr>
                
            </table>

            
            </div>

            <div id="tabs-2">
                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table_info" id="table_info2">
                    <tr>
                    <th style="width: 50%;">
                        最大欠款额度及天数设置
                    </th>
                    
                </tr>
                    <tr>
                   <td>
                      <table width="100%">
                        <tr>
                    <td class="td">
                        当前最大欠款额度：
                    </td>
                    <td align="left">
                        <asp:Label ID="lblMaxDebtMoney" runat="server" Text="Label"></asp:Label>
                    </td>
                               
                 </tr>
                            <tr>
                              <td class="td">
                                    调整最大欠款额度：
                                </td>
                                <td align="left">
                                          <asp:TextBox ID="txtMaxDebtMoney" CssClass="txt" runat="server" ValidationGroup="Fee"></asp:TextBox>元
                                  <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtMaxDebtMoney"
                                        Display="Dynamic" ErrorMessage="调整金额必须为数字" ValidationExpression="^\d+(\.\d+)?$"
                                        ForeColor="red" ValidationGroup="Fee">调整金额必须为数字</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtMaxDebtMoney"
                                        Display="Dynamic" ErrorMessage="调整金额不能为空" ForeColor="red" ValidationGroup="MaxDebtMoney">不能为空</asp:RequiredFieldValidator>
                                   </td>
                                </tr>
                                  <tr>
                    <td class="td">
                        最大欠款天数：
                    </td>
                    <td align="left">
                        <asp:Label ID="lblMaxdays" runat="server" Text="Label"></asp:Label>
                    </td>
                               
                 </tr>
                                <tr>
                              <td class="td">
                                    调整最大欠款天数：
                                </td>
                                <td align="left">
                                          <asp:TextBox ID="txtMaxDebtDays" CssClass="txt" runat="server" ValidationGroup="Fee" MaxLength=5 onkeyup="value=value.replace(/[^0-9]/g,'')"></asp:TextBox>
                                          天
                                  <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtMaxDebtDays"
                                        Display="Dynamic" ErrorMessage="天数必须为数字" ValidationExpression="^\d$"
                                        ForeColor="red" ValidationGroup="Fee">必须为数字</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtMaxDebtDays"
                                        Display="Dynamic" ErrorMessage="天数不能为空" ForeColor="red" ValidationGroup="MaxDebtMoney">不能为空</asp:RequiredFieldValidator>
                                   </td>
                                </tr>
                                 <tr>
                                <td colspan="2" style="text-align: center;">
                                    <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btnNormal"
                                        OnClientClick="return true; " ValidationGroup="MaxDebtMoney" 
                                        onclick="LinkButton2_Click">保 存</asp:LinkButton>
                                     <%--   --%>
                                </td>
                  </tr>
                    <tr>
                    <td colspan="2" style="text-align: center;">
                        <a href="LowerFillList.aspx?currentuserid=<%=this.mUser.id.ToString() %>" class="btn btnNormal">返 回</a>
                    </td>
                </tr>
                              </table>
                             </td>

                    </tr>
                   
            </table>
</div>
    </div>
   
    </form>
</body>
</html>
  <%--        <td id="Limittd" style=" display:none;">
                        <table width="100%">
                            <tr>
                                <td class="td">
                                    最大欠款额度：
                                </td>
                                <td>
                                    <input type="text" id="txtMaxForeheadLimit" class="inputtxtdat" runat="server" maxlength="10"
                                        style="width: 120px" onkeyup="checkInputTxt(this)" />
                               
                                </td>
                            </tr>
                            <tr id="QKMS" runat="server">
                                <td class="td">
                                    欠款提醒模式：
                                </td>
                                <td align="left">
                                    T +
                                    <asp:DropDownList ID="ddlowed" CssClass="inputtxtdat" runat="server">
                                        <asp:ListItem Value="0" Text="0"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                        <asp:ListItem Value="6" Text="6"></asp:ListItem>
                                        <asp:ListItem Value="7" Text="7"></asp:ListItem>
                                        <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                        <asp:ListItem Value="15" Text="15"></asp:ListItem>
                                        <asp:ListItem Value="30" Text="30"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr id="QKLSED" runat="server">
                                <td class="td">
                                    临时额度：
                                </td>
                                <td>
                                    <input type="text" id="txtLinShiPrice" class="inputtxtdat" runat="server" maxlength="10"
                                        style="width: 120px" onkeyup="checkInputTxt(this)" />
                                </td>
                            </tr>
                            <tr id="QKLSFW" runat="server">
                                <td class="td">
                                    临时范围：
                                </td>
                                <td>
                                    <input type="text" id="txtTimeS" class="inputtxtdat" style="width: 120px;" readonly="true"
                                        runat="server" onfocus="WdatePicker({isShowWeek:false,isShowClear:false,dateFmt:'yyyy-MM-dd'})" />至
                                    <input type="text" id="txtTimeE" class="inputtxtdat" style="width: 120px;" readonly="true"
                                        runat="server" onfocus="WdatePicker({isShowWeek:false,isShowClear:false,dateFmt:'yyyy-MM-dd'})" />
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    调整内容：
                                </td>
                                <td>
                                    <asp:RadioButton ID="rbtnNowlimit" Checked="true" runat="server" Text="当前欠款" GroupName="limittype1">
                                    </asp:RadioButton>
                                    <asp:RadioButton ID="rbtnMaxlimit" runat="server" Text="最大欠款" GroupName="limittype1">
                                    </asp:RadioButton>
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    调整方式：
                                </td>
                                <td>
                                    <asp:RadioButton ID="rbtnlimit1" Checked="true" runat="server" Text="上调" GroupName="limittype">
                                    </asp:RadioButton>
                                    <asp:RadioButton ID="rbtnlimit2" runat="server" Text="下调" GroupName="limittype">
                                    </asp:RadioButton>
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    调整额度：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtlimit" CssClass="txt" runat="server" ValidationGroup="limit"></asp:TextBox>元
                                    <asp:RegularExpressionValidator ID="REVtxtlimit" runat="server" ControlToValidate="txtlimit"
                                        Display="Dynamic" ErrorMessage="调整额度必须为数字" ValidationExpression="^\d+(\.\d+)?$"
                                        ForeColor="red" ValidationGroup="limit">调整额度必须为数字</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                 
                            <tr>
                                <td colspan="2" style="text-align: center;">
                                    <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btnNormal" ValidationGroup="limit"
                                        OnClick="LinkButton2_Click" OnClientClick="return  numlimit();">保 存</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </td>--%>
                 <%--   <td id="Limittd2" style=" display:none; text-align:center;color:Red;font-size:20px; font-weight:bold;">未开启此功能</td>--%>