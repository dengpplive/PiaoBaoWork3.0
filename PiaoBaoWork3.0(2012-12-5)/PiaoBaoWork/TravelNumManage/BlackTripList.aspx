<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BlackTripList.aspx.cs" Inherits="TravelNumManage_BlackTripList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>行程单发放</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .inputBorder
        {
            border: 1px solid #999;
        }
        .colorGreen
        {
            color: Green;
        }
        .colorRed
        {
            color: red;
        }
        .colorYellow
        {
            color: #ED911B;
        }
        .td_w100
        {
            width: 100px;
        }
        .td_w150
        {
            width: 150px;
        }
        .td_w110
        {
            width: 110px;
        }
    </style>
    <script type="text/javascript">
        function showdialog(t,p) {
            jQuery("#showOne").html(t);
            jQuery("#showOne").dialog({
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
                        jQuery(this).dialog('close');
                        if(p!=null&&p=="1") {
                            jQuery("#btnQuery").click();
                        }
                    }
                }
            });
        }
        function ReturnGo() {
            location.href="ManageUserTrip.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
        }
    </script>
    <script language="javascript" type="text/javascript">
        ///选中所有的CheckBox
        function selectAll(obj) {
            jQuery("input[name='ckHang'][type='checkbox']").attr("checked",obj.checked);
        }
        function FaFangTrip(param) {
            var ids='';
            if(param.id!=null&&param.type=="0") {
                ids=param.id;
            } else {
                var ckObj=jQuery("input[name='ckHang'][type='checkbox']:checked");
                var len=ckObj.length;
                if(len==0) {
                    showdialog("请选择需要发放的空白行程单！");
                    return false;
                }
                var ids=[];
                ckObj.each(function (index,box) {
                    ids.push(jQuery(box).val());
                });
                if(ids.length==0) {
                    showdialog("请选择需要发放的空白行程单！");
                    return false;
                }
                ids=ids.join('@');
            }
            var ownercpyno=jQuery("#Hid_OwnerCpyNo").val();
            var ownercpyname=jQuery("#Hid_OwnerCpyName").val();
            var usecpyno=jQuery("#Hid_UseCpyNo").val();
            var usecpyname=jQuery("#Hid_UseCpyName").val();

            //操作
            var url="BlackTripList.aspx";
            var param=
                    {
                        Id: escape(ids),
                        OwnerCpyNo: escape(ownercpyno),
                        OwnerCpyName: escape(ownercpyname),
                        UseCpyNo: escape(usecpyno),
                        UseCpyName: escape(usecpyname),
                        OP: "huishou",
                        num: Math.random(),
                        currentuserid:'<%=this.mUser.id.ToString() %>'
                    };
            var target=event.srcElement?event.srcElement:event.target;
            jQuery(target).attr("disabled",true);
            jQuery.post(url,param,function (data) {
                jQuery(target).attr("disabled",false);
                var arr=data.split('##');
                if(arr.length==2) {
                    if(arr[0]=="1") {
                        showdialog("发放空白行程单成功！",1);
                    } else {
                        showdialog("发放空白行程单失败！原因如下:<br />"+arr[1]);
                    }
                } else {
                    showdialog("发放空白行程单失败！");
                }
            },"text");

            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="c-list-filter">
            <div class="container">
                <table cellspacing="0" cellpadding="2" border="0">
                    <tr>
                        <td align="center" colspan="4">
                            行程单:
                            <asp:TextBox ID="txtNumber" runat="server" Width="120" MaxLength="10"></asp:TextBox>
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text="查询" OnClick="btnQuery_Click" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnBack" runat="server" Text="发放选中行程单" OnClientClick="return FaFangTrip({type:1});" />
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <table id="tb-all-trade" class="tb-all-trade" width="98%" cellspacing="0" style="margin: 0 1% 0 1%;"
            cellpadding="0" border="1">
            <thead>
                <tr>
                    <th class="">
                        <input id="ckAll" type="checkbox" onclick='selectAll(this)' />
                    </th>
                    <th>
                        客户帐号
                    </th>
                    <th>
                        行程单号
                    </th>
                    <th>
                        Office号
                    </th>
                    <th>
                        状态
                    </th>
                    <th>
                        领用时间
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repList" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <input id="ckHang" type="checkbox" name="ckHang" value='<%#Eval("id")%>' />
                        </td>
                        <td>
                            <%=UseAccount%>
                        </td>
                        <td>
                            <%#Eval("TripNum")%>
                        </td>
                        <td>
                            <%#Eval("CreateOffice")%>
                        </td>
                        <td>
                            <%#  ShowText(0, Eval("TripStatus"))%>
                        </td>
                        <td>
                            <%#Eval("UseTime")%>
                        </td>
                        <td>
                            <a href="#" onclick="return FaFangTrip({type:0,id:'<%#Eval("id")%>'})">发放行程单</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <input type="hidden" id="hid_PageSize" value="20" runat="server" />
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
        </webdiyer:AspNetPager>
    </div>
    <div id="showOne">
    </div>
    <%--所属公司编号--%>
    <input type="hidden" id="Hid_OwnerCpyNo" runat="server" />
    <%--所属公司名称--%>
    <input type="hidden" id="Hid_OwnerCpyName" runat="server" />
    <%--领用公司编号--%>
    <input type="hidden" id="Hid_UseCpyNo" runat="server" />
    <%--领用公司名称--%>
    <input type="hidden" id="Hid_UseCpyName" runat="server" />
    </form>
</body>
</html>
