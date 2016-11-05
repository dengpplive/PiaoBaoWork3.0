<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApplyTravel.aspx.cs" Inherits="TravelNumManage_ApplyTravel" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>行程单申请</title>
    <script src="../JS/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link type="text/css" href="../CSS/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../CSS/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../JS/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../JS/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .inputBorder
        {
            border: 1px solid #999;
        }
        .td_w80
        {
            width: 80px;
        }
        .td_w100
        {
            width: 100px;
        }
        .td_w150
        {
            width: 150px;
        }
        .td_w220
        {
            width: 220px;
        }
        .td_w110
        {
            width: 110px;
        }
        .td_w120
        {
            width: 120px;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
    </style>
    <script type="text/javascript">
    <!--
        function showdialog(t,param) {
            jQuery("select").hide();
            jQuery("#showdg").html(t);
            jQuery("#showdg").dialog({
                title: param!=null&&param.t!=null?param.t:'提示',
                bgiframe: true,
                height: 250,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    jQuery("select").show();
                },
                buttons: {
                    '确定': function () {

                        var obj=event.srcElement?event.srcElement:event.target;
                        jQuery(obj).attr("disabled",true);
                        if(param!=null) {
                            if(param.p=="-1") {
                                jQuery(this).dialog('close');
                                jQuery("#btnQuery").click();
                            } else if(param.p=="0") {
                                location.go(-1);
                            } else if(param.p=="1") {
                                confirmApply();
                                jQuery(obj).attr("disabled",false);
                            }
                            else if(param.p=="2") {
                                cancelApplay(param.id);
                                jQuery(obj).attr("disabled",false);
                            }
                        } else {
                            jQuery(this).dialog('close');
                        }
                    }
                }
            }).css({ "width": "auto","height": "auto" });
        }
        //提示
        function showdialog0(t,p) {
            jQuery("#Div1").html(t);
            jQuery("#Div1").dialog({
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
                            jQuery("#btnQuery")[0].click();
                        }
                    }
                }
            }).css({ "width": "auto","height": "auto" });
        }
        //显示申请对话框
        function ShowApply() {
            var RoleType=jQuery("#Hid_RoleType").val();
            if(RoleType=="1") {
                var val=jQuery.trim(jQuery("#ddlGY").val());
                if(val=="") {
                    showdialog0("请选择供应或者落地运营商！");
                    return false;
                }
            }
            var html="<table><tr><td>申请张数:</td><td><input type=\"text\" id=\"txtNum\" maxlength=\"50\"/></td></tr>"+
            "<tr><td>备注:</td><td><textarea id=\"ApplyRemark\" cols=\"35\" rows=\"6\" maxlength=\"200\"></textarea></td></tr><table>";

            showdialog(html,{ p: 1 });
            return false;
        }
        function clear() {
            jQuery("input[type='text']").val("");
            jQuery("select option").eq(0).attr("selected",true);
        }
        //确认申请
        function confirmApply() {

            var txtNum=jQuery.trim(jQuery("#txtNum").val());
            var ApplyRemark=jQuery.trim(jQuery("#ApplyRemark").val());
            if(txtNum=="") {
                showdialog0("申请行程单张数不能为空！");
                return false;
            }
            if(jQuery.isNaN(txtNum)) {
                showdialog0("申请行程单张数只能为数字！");
                return false;
            }
            if(parseFloat(txtNum,10)<=0||parseFloat(txtNum,10)>=10000) {
                showdialog0("申请行程单张数范围在1-10000之间！");
                return false;
            }

            if(ApplyRemark=="") {
                showdialog0("备注不能为空！");
                return false;
            }
            var CpyName=jQuery.trim(jQuery("#Hid_CpyName").val());
            var CpyNo=jQuery.trim(jQuery("#Hid_CpyNo").val());
            var Account=jQuery.trim(jQuery("#Hid_Account").val());
            var UserName=jQuery.trim(jQuery("#Hid_UserName").val());
            var AuditCpyNo=jQuery.trim(jQuery("#Hid_AuditCpyNo").val());
            var obj=event.srcElement?event.srcElement:event.target;
            jQuery(obj).attr("disabled",true);
            jQuery.post("ApplyTravel.aspx",{ optype: "TripApply",
                CpyName: escape(CpyName),
                CpyId: escape(CpyNo),
                Account: escape(Account),
                UserName: escape(UserName),
                ApplyNum: escape(txtNum),
                ApplyRemark: escape(ApplyRemark),
                AuditCpyNo: escape(AuditCpyNo),
                num: Math.random(),
                currentuserid:'<%=this.mUser.id.ToString() %>'
            },
                            function (data) {
                                jQuery(obj).attr("disabled",false);
                                jQuery("#showdg").dialog("close");
                                if(data=="1") {
                                    showdialog("申请成功",{ p: -1 });
                                } else {
                                    showdialog("申请失败");
                                }
                            },"text"
            );
            return true;
        }

        //拒绝申请
        function cancelApplay(id) {
            var roleType=parseInt(jQuery("#Hid_RoleType").val(),10);
            if(roleType<4) {
                var CpyName=jQuery.trim(jQuery("#Hid_CpyName").val());
                var CpyNo=jQuery.trim(jQuery("#Hid_CpyNo").val());
                var Account=jQuery.trim(jQuery("#Hid_Account").val());
                var UserName=jQuery.trim(jQuery("#Hid_UserName").val());

                var Remark=jQuery("#txtCancelRemark").val();
                jQuery.post("ApplyTravel.aspx",{
                    optype: "jujue",
                    id: escape(id),
                    AuditRemark: escape(Remark),
                    AuditAccount: escape(Account),
                    AuditUserName: escape(UserName),
                    AuditCpyNo: escape(CpyNo),
                    AuditCpyName: escape(CpyName),
                    num: Math.random(),
                    currentuserid:'<%=this.mUser.id.ToString() %>'
                },
            function (data) {
                jQuery("#showdg").dialog("close");
                if(data=="1") {
                    showdialog("取消成功",{ p: -1 });
                } else {
                    showdialog("取消失败");
                }
            },"text");
            }
        }

        function setHidVal(obj) {
            var RoleType=jQuery("#Hid_RoleType").val();
            if(RoleType=="1") {
                var cpyNo=jQuery.trim(jQuery(obj).val().split('@@')[0]);
                if(cpyNo!="") {
                    jQuery("#Hid_AuditCpyNo").val(cpyNo);
                }
            }
        }

        //显示取消审核       
        function CancelAduilt(id) {
            var html='<table><tr><td>取消审核说明:</td><td><textarea id="txtCancelRemark" rows="3" cols="30" maxlength="150"></textarea></td></tr></table>';
            showdialog(html,{ t: "取消审核",p: 2,id: id });
        }
        function vate() {
            var f=true;
            var RoleType=jQuery("#Hid_RoleType").val();
            if(RoleType=="1") {
                var val=jQuery.trim(jQuery("#ddlGY").val());
                if(val=="") {
                    f=false;
                    showdialog("请选择供应或者落地运营商！");
                }
            }
            return f;
        }
        //分配
        function Fenpei(id) {
            var RoleType=jQuery("#Hid_RoleType").val();
            var url="AddTripNum.aspx?AppId="+id+"&currentuserid=<%=this.mUser.id.ToString() %>";
            if(RoleType=="1") {
                var selVal=jQuery.trim(jQuery("#ddlGY").val());
                if(selVal=="") {
                    showdialog("请选择供应或者落地运营商！");
                    return false
                } else {
                    var strArr=selVal.split('@@');
                    var v=[];
                    if(strArr.length==4) {
                        v.push("OwnerCpyNo="+strArr[0]);
                        v.push("OwnerCpyName="+strArr[1]);
                        v.push("AddLoginName="+strArr[2]);
                        v.push("AddUserName=" + strArr[3]);
                        url=url+"&"+v.join('&');
                    }
                }
            }
            location.href=url;
            return false;
        }     
    //-->
    </script>
</head>
<body>
    <div id="showdg">
    </div>
    <div id="Div1">
    </div>
    <div id="divcancel">
    </div>
    <form id="form1" runat="server">
    <div style="top: 0px;">
        <%--标题--%>
        <div class="title">
            <asp:Label ID="lblShow" Text="行程单申请" runat="server" />
        </div>
        <%--查询条件--%>
        <div class="c-list-filter">
            <div class="container">
                <table cellspacing="5" cellpadding="0" border="0">
                    <tr>
                        <th>
                            申请时间：
                        </th>
                        <td>
                            <input type="text" id="txtApplyStartDate" style="width: 100px;" readonly="true" class="inputBorder inputtxtdat"
                                runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />-
                            <input type="text" id="txtApplyEndDate" style="width: 100px;" readonly="true" runat="server"
                                class="inputBorder inputtxtdat" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                        </td>
                        <th>
                            状态：
                        </th>
                        <td>
                            <asp:DropDownList ID="ddlAdult" runat="server" Style="width: 130px;">
                                <asp:ListItem Value="-1">--审核状态--</asp:ListItem>
                                <asp:ListItem Value="3">未审核</asp:ListItem>
                                <asp:ListItem Value="4">审核通过</asp:ListItem>
                                <asp:ListItem Value="5">审核未通过</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <th id="tr1" runat="server">
                            供应或者落地运营商
                        </th>
                        <td id="tr2" runat="server">
                            <select id="ddlGY" runat="server" style="width: 150px;" onchange="setHidVal(this)">
                            </select>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text="查 询" OnClick="btnQuery_Click" OnClientClick="return vate();" /></span>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s <%=ShowText(2,"") %>">
                                <asp:Button ID="btnApplay" runat="server" Text="申 请" OnClientClick="return ShowApply()" /></span>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                                <asp:Button ID="btnReset" runat="server" Text="重 置" OnClientClick="return clear();" /></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--列表--%>
        <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
            width="100%">
            <thead>
                <tr>
                    <th>
                        申请客户
                    </th>
                    <th>
                        申请账号
                    </th>
                    <th>
                        申请张数
                    </th>
                    <th class="td_w120">
                        申请时间
                    </th>
                    <th class="td_w120">
                        审核时间
                    </th>
                    <th>
                        状态
                    </th>
                    <th>
                        申请说明
                    </th>
                    <th>
                        审核说明
                    </th>
                    <th class='<%=ShowText(1,"") %>'>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repApplyList" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("ApplyCpyName")%>
                        </td>
                        <td>
                            <%#Eval("ApplyAccount")%>
                        </td>
                        <td>
                            <%--申请张数--%>
                            <%#Eval("ApplyCount") %>
                        </td>
                        <td class="td_w100">
                            <%-- 申请时间--%>
                            <%# Eval("ApplyDate").ToString().Contains("1901") || Eval("ApplyDate").ToString().Contains("1900") ? "" : Eval("ApplyDate").ToString()%>
                        </td>
                        <td class="td_w100">
                            <%--审核时间--%>
                            <%# Eval("AuditDate").ToString().Contains("1901") || Eval("AuditDate").ToString().Contains("1900") ? "" : Eval("AuditDate").ToString()%>
                        </td>
                        <td class="td_w80">
                            <%-- 状态--%>
                            <%# ShowText(0,Eval("ApplyStatus"))%>
                        </td>
                        <td title='<%#Eval("ApplyRemark") %>'>
                            <%--申请说明--%>
                            <%# SubStrStr(Eval("ApplyRemark "), 50)%>
                        </td>
                        <td title='<%#Eval("AuditRemark") %>'>
                            <%-- 审核说明--%>
                            <%# SubStrStr(Eval("AuditRemark"),50)%>
                        </td>
                        <td class='<%= ShowText(1,"") %>'>
                            <div class='<%# Eval("ApplyStatus").ToString()=="3"?"show":"hide" %>'>
                                <a href="#" onclick="return Fenpei('<%# Eval("id") %>');">分配</a><br />
                                <a href="#" onclick="return CancelAduilt('<%# Eval("id") %>');">取消</a>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <%--分页--%>
        <input type="hidden" id="hid_PageSize" value="10" runat="server" />
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    <%--隐藏域数据--%>
    <%--客户名称--%>
    <input type="hidden" id="Hid_CpyName" runat="server" />
    <%--客户公司编号--%>
    <input type="hidden" id="Hid_CpyNo" runat="server" />
    <%--申请人帐号--%>
    <input type="hidden" id="Hid_Account" runat="server" />
    <%--申请人姓名--%>
    <input type="hidden" id="Hid_UserName" runat="server" />
    <%--审核公司编号--%>
    <input type="hidden" id="Hid_AuditCpyNo" runat="server" />
    <%--角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" />
    <script type="text/javascript" src="../js/Tooltip/ToolTip.js"> </script>
    <script type="text/javascript">
        <!--
        initToolTip("td");
        //-->
    </script>
    </form>
</body>
</html>
