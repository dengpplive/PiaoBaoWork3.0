<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TriplList.aspx.cs" Inherits="TravelNumManage_TriplList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>行程单管理</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
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
        .colorFP
        {
            color: #D0FFA0;
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
        /*行程单状态颜色显示*/
    </style>
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showdialog(t,p) {
            jQuery("select").hide();
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
                close: function () {
                    jQuery("select").show();
                },
                buttons: {
                    '确定': function () {
                        var obj=event.srcElement?event.srcElement:event.target;
                        jQuery(obj)[0].disabled=true;
                        jQuery(this).dialog('close');
                        if(p=="0") {
                            location.go(-1);
                        } else if(p=="1") {
                            jQuery("#btnQuery").click();
                        }
                    }
                }
            });
        }
        //清空数据
        function Clear() {
            jQuery("input[type='text']").val("");
            jQuery("select option").eq(0).attr("selected",true);
        }
        //全选
        function SelAll(obj) {
            jQuery("input[type='checkbox'][name='ck_name']:visible").attr("checked",obj.checked);
        }
        function cancelTrip() {
            jQuery("#showOne").dialog("close");
        }
        //显示修改Office号
        function ShowOffice() {
            var cks=jQuery("input[type='checkbox'][name='ck_name']:checked");
            if(cks.length==0) {
                showdialog("请勾选需要修改Office项的复选框！");
            } else {
                var ids=[];
                cks.each(function (index,ck) {
                    ids.push("'"+jQuery(ck).val()+"'");
                });
                idds=escape(ids.join(','));
                var html="<input type='hidden' id='hid_ids' value='"+idds+"' /><table><tr><td>选定复选框office修改为：</td><td><input type='text' id='newOffice'  /></td><td><span id='msg'><font style='color:red;'>*</font></span></td></tr>";
                html+="<tr><td colspan='3' align='center'><span class='btn btn-ok-s'><input type=\"button\" id=\"btnUpdate1\" value='确定' onclick='return Update({t:0});' /></span>&nbsp;&nbsp;<span class='btn btn-ok-s'><input type=\"button\" id=\"btnCancel\" value=\"关闭\" onclick=\"cancelTrip()\" /></span></td></tr>";
                html+="</table>";
                showdialog(html);
                jQuery("#newOffice")[0].focus();
            }
            return false;
        }
        function Update(type) {
            var cks=jQuery("input[type='checkbox'][name='ck_name']:visible");
            var msg="";
            var ids='';
            var Office="";
            var idArr=[];
            cks.each(function (index,tr) {
                if(jQuery(tr).attr("checked")) {
                    idArr.push("'"+jQuery(tr).val()+"'");
                }
            });
            //修改Office
            if(type.t=="0") {
                ids=unescape(jQuery("#hid_ids").val());
                Office=jQuery.trim(jQuery("#newOffice").val());
                if(Office=="") {
                    msg="修改的Office不能为空";
                    jQuery("#msg").html("<font style='color:red;'>* 修改的Office不能为空！</font>");
                }
            } else if(type.t=="1") {
                if(type.id!=null) {
                    ids=type.id;
                } else {
                    //空白回收                                      
                    if(idArr.length==0) {
                        msg="请选择需要空白回收的行程单！"
                    } else {
                        ids=idArr.join(',');
                    }
                }
            } else if(type.t=="2") {
                //审核作废行程单
                if(type.id!=null) {
                    ids=type.id;
                } else {
                    if(idArr.length==0) {
                        msg="请选择需要审核作废的行程单！"
                    } else {
                        ids=idArr.join(',');
                    }
                }
            } else if(type.t=="3") {
                //拒绝作废行程单                
                if(type.id!=null) {
                    ids=type.id;
                } else {
                    if(idArr.length==0) {
                        msg="请选择需要拒绝作废的行程单！"
                    } else {
                        ids=idArr.join(',');
                    }
                }
            }
            if(msg=="") {
                var target=event.srcElement?event.srcElement:event.target;
                jQuery(target).attr("disabled",true);               
                jQuery.post("TriplList.aspx",
                {
                    type: type.t,
                    IDs: escape(ids),
                    Office: escape(Office),
                    num: Math.random(),
                    currentuserid: '<%=this.mUser.id.ToString() %>'
                },
                function (data) {
                    jQuery(target).attr("disabled",false);
                    var strArr=data.split("@@");
                    if(strArr[0]=="1") {
                        if(type.t=="0") {
                            showdialog("修改成功",1);
                        } else if(type.t=="1") {
                            showdialog("空白回收行程单成功",1);
                        }
                        else if(type.t=="2") {
                            showdialog("审核作废行程单成功",1);
                        }
                        else if(type.t=="3") {
                            showdialog("拒绝作废行程单成功",1);
                        }
                    } else if(strArr[0]=="0") {
                        if(type.t=="0") {
                            showdialog("修改Office失败");
                        } else if(type.t=="1") {
                            showdialog("空白回收行程单失败");
                        }
                        else if(type.t=="2") {
                            showdialog("审核作废行程单失败",1);
                        }
                        else if(type.t=="3") {
                            showdialog("拒绝作废行程单失败",1);
                        }
                    }
                },"text");
            } else {
                showdialog(msg);
            }
            return false;
        }
        function SelData() {
            var f=true;
            var RoleType=jQuery.trim(jQuery("#Hid_RoleType").val());
            var isImport=jQuery("#Hid_Import").val()=="1"?true:false;
            if(isImport&&RoleType=="1") {
                var val=jQuery.trim(jQuery("#ddlGY").val());
                if(val=="") {
                    showdialog("请选择需要进行入库的供应或者落地运营商！");
                    f=false;
                }
            }
            return f;
        }
    </script>
</head>
<body>
    <div id="showOne">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div>
        <div class="title">
            <asp:Label ID="lblShow" Text="行程单管理" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table cellspacing="5" cellpadding="3" border="0" style="width: 100%;">
                    <tr>
                        <th align="right">
                            行程单号段：
                        </th>
                        <td>
                            <input type="text" id="txtTripStart" class="td_w110 inputBorder inputtxtdat" runat="server"
                                maxlength="10" />-<input type="text" id="txtTripEnd" class="td_w110 inputBorder inputtxtdat"
                                    runat="server" maxlength="10" />
                        </td>
                        <th align="right">
                            票号段：
                        </th>
                        <td>
                            <input type="text" id="txtTicketStartNum" maxlength="15" class="td_w110 inputBorder inputtxtdat"
                                runat="server" />-
                            <input type="text" id="txtTicketEndNum" maxlength="15" class="td_w110 inputBorder inputtxtdat"
                                runat="server" />
                        </td>
                        <th align="right">
                            &nbsp;状态：
                        </th>
                        <td>
                            <asp:DropDownList ID="ddlTripState" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <th align="right">
                            发放时间：
                        </th>
                        <td>
                            <input id="txtAddStart" runat="server" class="inputBorder inputtxtdat" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                                readonly="true" style="width: 110px;" type="text" />-<input id="txtAddEnd" runat="server"
                                    class="inputBorder inputtxtdat" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                                    readonly="true" style="width: 110px;" type="text" />
                        </td>
                        <th align="right">
                            &nbsp;使用时间：
                        </th>
                        <td>
                            <input id="txtCreateStart" runat="server" class="inputBorder inputtxtdat" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                                readonly="true" style="width: 110px;" type="text" />-
                            <input id="txtCreateEnd" runat="server" class="inputBorder inputtxtdat" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                                readonly="true" style="width: 110px;" type="text" />&nbsp;
                        </td>
                        <td>
                            作废时间：
                        </td>
                        <td>
                            <input id="txtVoidStart" runat="server" class="inputBorder inputtxtdat" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                                readonly="true" style="width: 110px;" type="text" />-<input id="txtVoidEnd" runat="server"
                                    class="inputBorder inputtxtdat" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                                    readonly="true" style="width: 110px;" type="text" />
                        </td>
                    </tr>
                    <tr>
                        <th align="right">
                            创建使用Office:
                        </th>
                        <td>
                            <input type="text" id="txtOffice" class="td_w110 inputBorder inputtxtdat" maxlength="6"
                                runat="server" />
                        </td>
                        <td>
                            <span id="tr1" runat="server">供应或者落地运营商</span>
                        </td>
                        <td>
                            <span id="tr2" runat="server">
                                <asp:DropDownList ID="ddlGY" runat="server">
                                </asp:DropDownList>
                            </span>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="4">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text="查 询" OnClick="btnQuery_Click" OnClientClick="return SelData();" />
                            </span>&nbsp;&nbsp; <span class="btn btn-ok-s" runat="server" id="span1">
                                <asp:Button ID="Button1" runat="server" Text="批量空白回收" OnClientClick="return Update({t:1})" />
                            </span>&nbsp;&nbsp; <span class="btn btn-ok-s" runat="server" id="span2">
                                <asp:Button ID="Button2" runat="server" Text="批量审核作废行程单" OnClientClick="return Update({t:2})" />
                            </span><span class="btn btn-ok-s" runat="server" id="span3">
                                <asp:Button ID="Button3" runat="server" Text="批量拒绝作废行程单" OnClientClick="return Update({t:3})" />
                            </span><span class="btn btn-ok-s" runat="server" id="span_import">
                                <asp:Button ID="btImport" runat="server" Text="入库" OnClick="btImport_Click" OnClientClick="return SelData();" />
                            </span>&nbsp;&nbsp; <span class="btn btn-ok-s" runat="server" id="span_Office">
                                <asp:Button ID="btnUpdateOffice" runat="server" Text="修改Office" OnClientClick="return ShowOffice();" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnReset" runat="server" Text="重 置" OnClientClick="return Clear();" />
                            </span>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <table id="tb-all-trade" class="tb-all-trade" width="98%" cellspacing="0" style="margin: 0 1% 0 1%;"
            cellpadding="0" border="1">
            <thead>
                <tr>
                    <th class='<%=ShowText(2,"") %>'>
                        <input id="ckAll" name="all" type="checkbox" onclick="SelAll(this)" />
                    </th>
                    <th>
                        领用公司
                    </th>
                    <th>
                        行程单号
                    </th>
                    <th>
                        Office号
                    </th>
                    <th>
                        票号
                    </th>
                    <th>
                        状态
                    </th>
                    <th>
                        拒绝理由
                    </th>
                    <th class="<%= IsImport?"show":"hide" %>">
                        入库时间
                    </th>
                    <th class="td_w100">
                        领用时间
                    </th>
                    <th class="td_w100">
                        创建打印时间
                    </th>
                    <th class="td_w100">
                        作废时间
                    </th>
                    <th class='<%=ShowText(2,"") %>'>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repList" runat="server">
                <ItemTemplate>
                    <tr>
                        <td class='<%=ShowText(2,"") %>'>
                            <input id="ck" name="ck_name" type="checkbox" value='<%# Eval("id")%>' class='<%# ShowText(5,Eval("TripStatus")) %>' />
                        </td>
                        <td>
                            <%--领用公司名称--%>
                            <%# Eval("UseCpyName")%>
                        </td>
                        <td>
                            <%# Eval("TripNum")%>
                        </td>
                        <td>
                            <%--Office--%>
                            <%# Eval("CreateOffice")%>
                        </td>
                        <td>
                            <%--票号--%>
                            <%# Eval("TicketNum")%>
                        </td>
                        <td>
                            <%--状态--%>
                            <%# ShowText(0,Eval("TripStatus"))%>
                        </td>
                        <td title='<%# Eval("Remark").ToString() == "0" ? "" : Eval("Remark").ToString()%>'>
                            <%--拒绝理由--%>
                            <%#ShowText(1,Eval("Remark"))%>
                        </td>
                        <td class="td_w100 <%= IsImport?"show":"hide" %>">
                            <%--入库时间--%>
                            <%# Eval("AddTime").ToString().Contains("1901") || Eval("AddTime").ToString().Contains("1900") ? "" : DataBinder.Eval(Container.DataItem, "AddTime", "{0:yyyy-MM-dd<br \\/>HH:mm:ss}").ToString()%>
                        </td>
                        <td class="td_w100">
                            <%--发放时间--%>
                            <%# Eval("UseTime").ToString().Contains("1901") || Eval("UseTime").ToString().Contains("1900") ? "" : DataBinder.Eval(Container.DataItem, "UseTime", "{0:yyyy-MM-dd<br \\/>HH:mm:ss}").ToString()%>
                        </td>
                        <td class="td_w100">
                            <%--使用时间 打印时间--%>
                            <%# Eval("PrintTime").ToString().Contains("1901") || Eval("PrintTime").ToString().Contains("1900") ? "" : DataBinder.Eval(Container.DataItem, "PrintTime", "{0:yyyy-MM-dd<br \\/>HH:mm:ss}").ToString()%>
                        </td>
                        <td class="td_w100">
                            <%--作废时间--%>
                            <%# Eval("InvalidTime").ToString().Contains("1901") || Eval("InvalidTime").ToString().Contains("1900") ? "" : DataBinder.Eval(Container.DataItem, "InvalidTime", "{0:yyyy-MM-dd<br \\/>HH:mm:ss}").ToString()%>
                        </td>
                        <td class='<%=ShowText(2,"") %>'>
                            <div class='<%# ShowText(4,Eval("TripStatus")) %>'>
                                <a href="#" onclick="return Update({t:1,id:'<%# Eval("id")%>'})">回收空白行程单</a>
                            </div>
                            <div class='<%# ShowText(3,Eval("TripStatus")) %>'>
                                <a href="#" onclick="return Update({t:2,id:'<%# Eval("id")%>'})">审核作废行程单</a><br />
                                <a href="#" onclick="return Update({t:3,id:'<%# Eval("id")%>'})">拒绝作废行程单</a>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <input type="hidden" id="Hid_PageSize" runat="server" value="10" />
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    <%--该页面使用角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" />
    <%--领用公司编号--%>
    <input type="hidden" id="Hid_UseCpyNo" runat="server" />
    <%--发放所属公司编号--%>
    <input type="hidden" id="Hid_OwnerCpyNo" runat="server" />
    <%--是否入库页面 1是 0否--%>
    <input type="hidden" id="Hid_Import" runat="server" value="0" />
    </form>
</body>
</html>
