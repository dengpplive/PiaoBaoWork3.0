<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PassengerList.aspx.cs" Inherits="User_PassengerList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>常旅客管理</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">
        function showdialog(t,f) {
            jQuery("select").hide();
            jQuery("#dialog").html(t);
            jQuery("#dialog").dialog({
                title: '提示',
                bgiframe: true,
                height: 180,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    jQuery("select").show();
                },
                buttons: {
                    '确定': function (evt) {
                        jQuery(this).dialog('close');
                        var target=evt.srcElement?evt.srcElement:evt.target;
                        jQuery(target).attr("disabled",true);

                        //代码
                    }
                }
            });
        }
        function CLear() {
            jQuery("input[type='text']").val("");
            jQuery("select option").eq(0).attr("selected",true);
        }
        function SelAll(obj) {
            jQuery("input[name='cBox'][type='checkbox']").attr("checked",obj.checked);
        }
        //批量删除
        function SelPatch() {
            jQuery("#Hid_SelIds").val("");
            var IsReturn=false;
            var msg="";
            var selCK=jQuery("input[name='cBox'][type='checkbox']:checked");
            var selArr=[];
            if(selCK.length<=0) {
                msg="请选择需要删除的常旅客！";
                showdialog(msg);
            } else {
                selCK.each(function (index,box) {
                    selArr.push("'"+jQuery(box).val()+"'");
                });
                if(selArr.length>0) {
                    jQuery("#Hid_SelIds").val(selArr.join(","));
                    IsReturn=true;
                }
            }
            return IsReturn;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dialog">
    </div>
    <div id="tabs">
        <div class="title">
            <span>常旅客管理</span>
        </div>
        <div id="tabs-1">
            <div class="container">
                <table border="0" cellpadding="0" cellspacing="0" class="Search">
                    <tr>
                        <th>
                            姓名：
                        </th>
                        <td>
                            <asp:TextBox ID="txtName" Width="130px" CssClass="inputtxtdat" runat="server" MaxLength="16"></asp:TextBox>
                        </td>
                        <th>
                            手机号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtTel" CssClass="inputtxtdat" runat="server" MaxLength="11"></asp:TextBox>
                        </td>
                        <th>
                            乘客类型：
                        </th>
                        <td>
                            <asp:RadioButtonList ID="rdlPasType" runat="server" RepeatColumns="7">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            证件类型：
                        </th>
                        <td>
                            <asp:DropDownList ID="ddlCardType" runat="server" CssClass="inputtxtdat" Width="135px">
                            </asp:DropDownList>
                        </td>
                        <th>
                            证件号码：
                        </th>
                        <td>
                            <asp:TextBox ID="txtCardNum" Width="130px" CssClass="inputtxtdat" runat="server"
                                Columns="16" MaxLength="20"></asp:TextBox>
                        </td>
                        <td colspan="2">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnPathDel" runat="server" Text=" 删 除 " OnClientClick="return SelPatch();"
                                    OnClick="btnPathDel_Click" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnReset" runat="server" Text="重置数据" OnClientClick="return CLear();" />
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="0" cellpadding="0"
            border="0">
            <thead>
                <tr>
                    <th>
                        <input type="checkbox" id="ckAll" onclick="SelAll(this)" />
                    </th>
                    <th>
                        姓名
                    </th>
                    <th>
                        乘客类型
                    </th>
                    <th>
                        证件类型
                    </th>
                    <th>
                        证件号码
                    </th>
                    <th>
                        手机号码
                    </th>
                    <th>
                        出生日期
                    </th>
                    <th>
                        航空公司卡号
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="Repeater" runat="server" OnItemCommand="Repeater_ItemCommand">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <th>
                            <input type="checkbox" id="cBox" name="cBox" value='<%#Eval("id")%>' />
                        </th>
                        <td class="pnr">
                            <%#Eval("Name")%>
                        </td>
                        <td>
                            <%# ShowFiled(0, Eval("Flyertype"))%>
                        </td>
                        <td>
                            <%#ShowFiled(1, Eval("CertificateType"))%>
                        </td>
                        <td>
                            <%#Eval("CertificateNum")%>
                        </td>
                        <td>
                            <%#Eval("Tel")%>
                        </td>
                        <td>
                            <%#Eval("BronTime", "{0:yyyy-MM-dd}")%>
                        </td>
                        <td>
                            <%#ShowFiled(2,Eval("CpyandNo"))%>
                        </td>
                        <td class="Operation">
                            <a href="PassengerEdit.aspx?id=<%#Eval("Id")%>&currentuserid=<%=this.mUser.id.ToString() %>">修 改 </a>&nbsp;
                            <asp:LinkButton ID="lbdelete" runat="server" CommandName="Del" CommandArgument='<%# Eval("Id") %>'
                                OnClientClick="if(confirm('确认删除吗？')){return true;}else{return false;}">删除</asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <input type="hidden" id="Hid_pageSize" runat="server" value="10" />
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
        </webdiyer:AspNetPager>
    </div>
    <input type="hidden" id="Hid_SelIds" runat="server" />
    </form>
</body>
</html>
