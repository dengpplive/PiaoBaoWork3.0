<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SmsTemplateManage.aspx.cs"
    Inherits="SMS_SmsTemplateManage" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link href="../js/My97DatePicker/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        #txttemplatecontent
        {
            height: 178px;
            width: 378px;
        }
        #txtway
        {
            height: 141px;
            width: 248px;
        }
    </style>
     <script type="text/javascript">
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
        </script>
</head>
<body>
    <form id="form1" runat="server">
     <div id="dd">
    </div>
    <div class="title">
        <asp:Label ID="lblShow" Text="短信模板管理" runat="server" />
    </div>
    <table style="width: 100%; background:#f9f9f9">
    <tr style="text-align: center">
            <td colspan="4"></td>
     </tr>
        <tr>
            <%--<td style="text-align: left">
                账户名:<asp:TextBox ID="txtusername" runat="server" CssClass="inputtxtdat"></asp:TextBox>
            </td>--%>
            <td style="text-align: left">
                模板名:<asp:TextBox ID="txttempname" runat="server" CssClass="inputtxtdat"></asp:TextBox>
            </td>
            <td style="text-align: left">
                模板类型:<asp:DropDownList ID="ddltype" runat="server" Height="20px" CssClass="inputtxtdat"
                    Width="123px">
                    <asp:ListItem Value="-1" Selected="True">所有模板</asp:ListItem>
                    <asp:ListItem Value="0">标准模板</asp:ListItem>
                    <asp:ListItem Value="1">自定义模板</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: left">
                起始时间:
                <input id="txtCreateTime" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                    onfocus="WdatePicker({isShowClear:true})" />-
                <input id="txtEndTime" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                    onfocus="WdatePicker({isShowClear:true})" />
            </td>
        </tr>
        <tr style="text-align: center">
            <td colspan="4" style=" border-bottom:1px #ddd solid; padding:0 0 5px 0">
                <span class="btn btn-ok-s">
                    <asp:Button ID="btcheack" runat="server" Text="查询" OnClick="btcheack_Click" /></span>
                <span class="btn btn-ok-s">
                    <asp:Button ID="btadd" runat="server" Text="添加" OnClick="btadd_Click" /></span>
                <span class="btn btn-ok-s">
                    <asp:Button ID="btreset" runat="server" Text="重置" OnClick="btreset_Click" /></span>
            </td>
        </tr>
    </table>
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
            width="100%">
            <thead>
                <tr>
                   <%-- <th>
                        添加账户
                    </th>--%>
                    <th>
                        模板名
                    </th>
                    <th>
                        模板内容
                    </th>
                    <th>
                        模板类型
                    </th>
                    <th>
                        日期
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="RepTempLate" runat="server" OnItemCommand="RepTempLate_ItemCommand">
                <ItemTemplate>
                    <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                      <%--  <td style="width: 10%;">
                            <%# Convert.ToInt32(Eval("SmsTpType")) == 0 ? "系统" : Eval("A1")%>
                        </td>--%>
                        <td style="width: 15%;">
                            <%# Eval("SmsTpName")%>
                        </td>
                        <td style="width: 40%;" title='<%# Eval("SmsTpContent")%>'>
                            <%# Eval("SmsTpContent").ToString().Length > 35 ? Eval("SmsTpContent").ToString().Substring(0,35) + "..." : Eval("SmsTpContent").ToString()%>
                        </td>
                        <td style="width: 10%;">
                            <%# Convert.ToInt32(Eval("SmsTpType")) == 1 ? "自定义模板" : "标准模板"%>
                        </td>
                        <td style="width: 15%;">
                            <%# Eval("SmsTpDate")%>
                        </td>
                        <td style="width: 10%;">
                            <div runat="server" visible='<%# Convert.ToInt32(Eval("SmsTpType")) == 0 ? false : true %>'>
                                <asp:LinkButton ID="lbedit" runat="server" CommandName="edit" CommandArgument='<%# Eval("id") %>'>编辑</asp:LinkButton>
                                <asp:LinkButton ID="lbdelete" runat="server" CommandArgument='<%# Eval("id") %>'
                                    OnClientClick="if(confirm('确认删除吗？')){return true;}else{return false;}">删除</asp:LinkButton>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="10" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </asp:Panel>
    <div id="Show" style="width: 100%; text-align: center; font-size: 12px; line-height: 20px;
        color: Gray; margin-top: 0;" runat="server" visible="false">
        没有符合你要搜索的条件,请输入正确的查询条件！
    </div>
    
    <asp:Panel ID="Panel2" runat="server" Visible="false">
    <div style="color:Red">
        <ul>
            <li>温馨提示：可以根据系统所提供的[乘机人] 、[起飞城市] 、[起抵城市] 、[到达城市] 、[起飞日期] 、[航班号]、[订单编号]等字段进行自定义发送内容。 </li>
        </ul>
    </div>
        <table class="tb-all-trade" border="0" cellspacing="0" cellpadding="0" style="width: 100%;">
            <tr>
                <td style="text-align: right">
                    模板名：
                </td>
                <td style="text-align: left">
                    <asp:TextBox ID="txttemplatename" runat="server" CssClass="inputtxtdat"></asp:TextBox>
                </td>
                <%-- <td></td>--%>
            </tr>
            <tr>
                <td style="text-align: right; width: 20%">
                    模板内容：
                </td>
                <td style="text-align: left; width: 50%">
                    <textarea id="txttemplatecontent" runat="server"></textarea>
                    <span style="color:Red">最多400字符!(一个汉字等于2个字符)</span></td>
                <%--<td style="text-align:left; margin-top:0px;">
            <asp:CheckBoxList ID="CheckBoxList1" runat="server">
                <asp:ListItem Value="[起底时间]">起底时间</asp:ListItem>
                <asp:ListItem Value="[起抵城市]">起抵城市</asp:ListItem>
                <asp:ListItem Value="[航班号]">航班号</asp:ListItem>
                <asp:ListItem Value="[票号]">票号</asp:ListItem>
            </asp:CheckBoxList>
            <br />
              <asp:CheckBoxList ID="CheckBoxList2" runat="server">
                  <asp:ListItem Value="[起底时间(回程)]">起底时间(回程)</asp:ListItem>
                  <asp:ListItem Value="[起抵城市(回程)]">起抵城市(回程)</asp:ListItem>
                  <asp:ListItem Value="[航班号(回程)]">航班号(回程)</asp:ListItem>
              </asp:CheckBoxList>
              <br />
              <asp:Button runat="server" ID="btaddcblist" Text="添加" 
                  onclick="btaddcblist_Click" />
          </td>--%>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <span class="btn btn-ok-s">
                        <asp:Button ID="btaddtemplate" runat="server" Text="添加模板" OnClick="btaddtemplate_Click" /></span>
                    <span class="btn btn-ok-s">
                        <asp:Button ID="btclear" runat="server" Text="清空数据" OnClick="btclear_Click" /></span>
                    <span class="btn btn-ok-s">
                        <asp:Button ID="btqx" runat="server" Text="取消" OnClick="btqx_Click" /></span><asp:HiddenField
                            ID="HiddenField1" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
