<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddTripNum.aspx.cs" Inherits="TravelNumManage_AddTripNum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>为用户分配行程单</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
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
    </style>
    <script language="javascript" type="text/javascript">
    <!--
        //对话框包含处理        
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
                        }
                    }
                }
            });
        }


        function showdialogX(t) {
            jQuery("#showOne").html(t);
            jQuery("#showOne").dialog({
                title: '行程单发放',
                bgiframe: true,
                height: 470,
                width: 650,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                }
            });
        }
        function GoBack() {
            window.location.href="ManageUserTrip.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
        }
        //不缓存url
        function SetUrlRandomParameter(_url) {
            var url;
            if(_url.indexOf("?")>0) {
                url=_url+"&rand="+Math.random();
            }
            else {
                url=_url+"?rand="+Math.random();
            }
            return url;
        }
        //选择行程单
        function SelectTrip() {
            var OwnerCpyNo=escape(jQuery("#Hid_OwnerCpyNo").val());
            var OwnerCpyName=escape(jQuery("#Hid_OwnerCpyName").val());
            var UseCpyNo=escape(jQuery("#Hid_UseCpyNo").val());
            var UseCpyName=escape(jQuery("#Hid_UseCpyName").val());

            var url="BlackTripList.aspx?OwnerCpyNo="+OwnerCpyNo+"&UseCpyNo="+UseCpyNo+"&OwnerCpyName="+OwnerCpyName+"&UseCpyName="+UseCpyName+"&currentuserid=<%=this.mUser.id.ToString() %>";
            showdialogX("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='yes' allowtransparency='yes' width='100%' style='margin:0px;padding:0px;' height='100%' src='"+SetUrlRandomParameter(url)+"'/>",470,650);
        }
        function SetTextVal() {
            try {
                var val=jQuery.trim(jQuery("#ddlOffice").val());
                if(val!="") {
                    var officeArr=val.split('@@');
                    if(officeArr.length==3) {
                        jQuery("#txtIataCode").val(officeArr[1]);
                        jQuery("#txtTicketCompany").val(officeArr[2]);
                    }
                }
            } catch(e) {
                alert(e.mesage);
            }
        }
        //入库0或者分配1
        function AddTrip(type) {
            
            var msg='';
            try {
                var startcode=jQuery.trim(jQuery("#txtStartCode").val());
                var startnum=jQuery.trim(jQuery("#txtStartNum").val());
                var endnum=jQuery.trim(jQuery("#txtEndNum").val());

                var office=jQuery.trim(jQuery("select[id='ddlOffice'] option:selected").text()).toUpperCase();
                var iatacode=jQuery.trim(jQuery("#txtIataCode").val());
                var ticketcompany=jQuery.trim(jQuery("#txtTicketCompany").val());
                //所属公司
                var ownercpyno=jQuery.trim(jQuery("#Hid_OwnerCpyNo").val());
                var ownercpyname=jQuery.trim(jQuery("#Hid_OwnerCpyName").val());
                //使用公司
                var usecpyno=jQuery.trim(jQuery("#Hid_UseCpyNo").val());
                var usecpyname=jQuery.trim(jQuery("#Hid_UseCpyName").val());

                //操作员
                var addloginname=jQuery.trim(jQuery("#Hid_AddLoginName").val());
                var addusername=jQuery.trim(jQuery("#Hid_AddUserName").val());
                //id
                var AppId=jQuery.trim(jQuery("#Hid_AppId").val());
                var AdultRemark=jQuery.trim(jQuery("#txtApplyAdultRemark").val());

                if(jQuery.trim(startcode)=='') {
                    msg="行程单编号不能为空！";
                } if(jQuery.trim(startcode).length!=6) {
                    msg="行程单编号输入长度不完整！";
                }
                var Pattern=/^\d{1,}$/;
                if(!Pattern.test(startcode)) {
                    msg="行程单编号必须为数字！";
                }
                if(!Pattern.test(startnum)) {
                    msg="行程单开始编号必须为数字！";
                }
                if(!Pattern.test(endnum)) {
                    msg="行程单结束编号必须为数字！";
                }
                if(jQuery.isNaN(startnum)||jQuery.isNaN(endnum)||jQuery.isNaN(startcode)) {
                    msg="行程单号段格式错误,必须为数字！"
                }
                if(parseInt(startnum,10)>parseInt(endnum,10)) {
                    msg="行程单开始编号必须小于行程单结束编号！";
                }
                if(office=="") {
                    msg="创建行程单Office不能为空！";
                }
                if(msg!='') {
                    showdialog(msg);
                } else {
                    //操作
                    var url="AddTripNum.aspx";
                    var op="trip";
                    var param=
                    {
                        StartCode: escape(startcode),
                        StartNum: escape(startnum),
                        EndNum: escape(endnum),
                        Office: escape(office),
                        IataCode: escape(iatacode),
                        TicketCompany: escape(ticketcompany),
                        OwnerCpyNo: escape(ownercpyno),
                        OwnerCpyName: escape(ownercpyname),
                        UseCpyNo: escape(usecpyno),
                        UseCpyName: escape(usecpyname),
                        AddLoginName: escape(addloginname),
                        AddUserName: escape(addusername),
                        AdultRemark: escape(AdultRemark),
                        AppId: escape(AppId),
                        OpType: escape(type),
                        OP: escape(op),
                        num: Math.random(),
                        currentuserid:'<%=this.mUser.id.ToString() %>'
                    };
                    var target=event.srcElement?event.srcElement:event.target;
                    jQuery(target).attr("disabled",true);
                    jQuery.post(url,param,function (data) {
                        jQuery(target).attr("disabled",false);
                        if(data!="") {
                            var arr=data.split('@@');
                            if(arr[0]=="1") {
                                showdialog(arr[1]);
                            } else {
                                showdialog(arr[1]);
                            }
                        } else {
                            showdialog("操作失败！");
                        }
                    },"text");
                }
            } catch(e) {
                alert(e.message);
            }
            return false;
        }
    //-->
    </script>
</head>
<body onload="SetTextVal()">
    <div id="showOne">
    </div>
    <form id="form1" runat="server">
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="行程单发放" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table id="tabtrip" style="width: 90%; margin: 0 5% 0 5%;">
                    <%--申请部分--%>
                    <caption>
                    <asp:Literal ID="ApplyCon" runat="server"></asp:Literal>
                    </caption>
                    <tr>
                        <td align="right" valign="top">
                            号段：
                        </td>
                        <td>
                            <asp:TextBox ID="txtStartCode" runat="server" MaxLength="6" Width="100px" CssClass="inputBorder"></asp:TextBox>
                            <asp:TextBox ID="txtStartNum" runat="server" Width="48px" MaxLength="4" CssClass="inputBorder"></asp:TextBox>
                            (起始号码)<br />
                            <asp:TextBox ID="TextBox3" runat="server" Enabled="false" Style="visibility: hidden;"
                                CssClass="inputBorder" Width="100px"></asp:TextBox>
                            <asp:TextBox ID="txtEndNum" runat="server" Width="48px" MaxLength="4" CssClass="inputBorder"></asp:TextBox>
                            (结束号码)
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td colspan="2" align="left">
                            <span style="color: Green;">注： 行程单编号都是整数,发放行程单的编号结束号码必须大于起始号码</span>
                        </td>
                    </tr>
                    <tr id="tr_001" runat="server">
                        <td align="right">
                            <span>Office号：</span>
                        </td>
                        <td height="30">
                            <select id="ddlOffice" onchange="SetTextVal()" style="width: 150px" runat="server">
                            </select>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr id="tr_003" runat="server">
                        <td align="right">
                            <span>行程单航协号：</span>
                        </td>
                        <td height="30">
                            <asp:TextBox ID="txtIataCode" runat="server" MaxLength="20" Enabled="false" Width="150px"
                                CssClass="inputBorder"></asp:TextBox>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr id="tr_004" runat="server">
                        <td align="right">
                            <span>行程单填开单位：</span>
                        </td>
                        <td height="30">
                            <asp:TextBox ID="txtTicketCompany" runat="server" Enabled="false" MaxLength="30"
                                Width="150px" CssClass="inputBorder"></asp:TextBox>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <%--审核说明--%>
                    <tr id="tr_adultRemark" runat="server" visible="false">
                        <td align="right" valign="top">
                            <span>审核说明：</span>
                        </td>
                        <td>
                            <asp:TextBox ID="txtApplyAdultRemark" runat="server" Width="250px" Height="100px"
                                CssClass="inputBorder" TextMode="MultiLine" MaxLength="200"></asp:TextBox>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr id="tr_btns1" runat="server">
                        <td align="center" colspan="3">
                            <span class="btn btn-ok-s" runat="server" id="spanImport">
                                <asp:Button ID="btnImport" runat="server" Text="入库" OnClientClick="return AddTrip(0);" />
                            </span>&nbsp; <span class="btn btn-ok-s" runat="server" id="spanSendTrip">
                                <asp:Button ID="btnOk" runat="server" Text="确定发放" OnClientClick="return AddTrip(1);" />
                            </span>&nbsp; <span class="btn btn-ok-s">
                                <input type="button" id="btnKonbai" value="使用空白行程单" onclick="SelectTrip()" />
                            </span>&nbsp; <span class="btn btn-ok-s">
                                <input type="button" id="btngo" value="返回" onclick="GoBack()" />
                            </span>&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <%--所属公司编号--%>
    <input type="hidden" id="Hid_OwnerCpyNo" runat="server" />
    <%--所属公司名称--%>
    <input type="hidden" id="Hid_OwnerCpyName" runat="server" />
    <%--入库操作员登录名--%>
    <input type="hidden" id="Hid_AddLoginName" runat="server" />
    <%--入库操作员名称--%>
    <input type="hidden" id="Hid_AddUserName" runat="server" />
    <%--领用公司编号--%>
    <input type="hidden" id="Hid_UseCpyNo" runat="server" />
    <%--领用公司名称--%>
    <input type="hidden" id="Hid_UseCpyName" runat="server" />
    <%--申请行程单id--%>
    <input type="hidden" id="Hid_AppId" runat="server" />
    </form>
</body>
</html>
