<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OpenAccount.aspx.cs" Inherits="Account_OpenAccount"
    EnableEventValidation="false" %>

<%@ Register Src="../UserContrl/Importanter.ascx" TagName="Importanter" TagPrefix="uc1" %>
<%@ Register Src="../UserContrl/ImportanterC.ascx" TagName="ImportanterC" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>开户</title>
    <meta http-equiv="x-ua-compatible" content="ie=7" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .rblZH, .rblZH td
        {
            margin: 0 0 0 0;
            padding: 0 0 0 0;
            border: 0px;
        }
        
        .style2
        {
            color: red;
        }
        .table_info td
        {
            text-align: left;
        }
        .table_info tr
        {
            line-height: 10px;
            height: 10px;
        }
        .table_info th, .table_info td
        {
            line-height: 25px;
            border-style: solid;
            border: 1px;
        }
        .table_info
        {
            width: 100%;
            text-align: center;
            margin: 0px;
            word-break: break-all;
        }
        .table_info .td
        {
            background-color: #ffffff;
            color: #424242;
            font-size: 12px;
            line-height: 24px;
            margin: 0;
            text-align: right;
        }
        .style3
        {
            height: 20px;
        }
        .style4
        {
            height: 20px;
        }
        .style5
        {
            height: 20px;
        }
    </style>
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../js/area.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <script src="../js/Validation.js" type="text/javascript"></script>
    <style type="text/css">
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

        function checkyz() {

            if($("#hidId").val()=="") {
                $("#ckzh").html("");
                //验证账号
                if($("#txtZH").val()=="") {
                    $("#ckzh").html("<font color=red>必填</font>");
                    $("#txtZH").focus();

                    return false;
                }
                //                var reg = /^[*]{4,18}|/;
                //                if (!reg.test($("#txtZH").val())) {
                //                    $("#ckzh").html("<font color=red>长度必须在4-18位之间</font>");
                //                    $("#txtZH").focus();
                //                    return false;
                //                }

                //验证密码
                $("#ckmm").html("");

                if($("#txtPass").val()=="") {
                    $("#ckmm").html("<font color=red>必填</font>");
                    $("#txtPass").focus();
                    return false;
                }
                var reg=/^\w{2,18}/;
                if(!reg.test($("#txtPass").val())) {
                    $("#ckmm").html("<font color=red>只能输入字母或数字，且长度必须在2-18位之间</font>");
                    $("#txtPass").focus();
                    return false;
                }
            }

            //验证日期
            $("#ckrq").html("");

            if($("#txtShiXiaoDate").val()=="") {
                $("#ckrq").html("<font color=red>必填</font>");
                $("#txtShiXiaoDate").focus();
                return false;
            }
            //验证姓名
            $("#ckxm").html("");

            if($("#txtName").val()=="") {
                $("#ckxm").html("<font color=red>必填</font>");
                $("#txtName").focus();
                return false;
            }

            //验证公司名称
            $("#ckmc").html("");

            if($("#txtUnitName").val()=="") {
                $("#ckmc").html("<font color=red>必填</font>");
                $("#txtUnitName").focus();
                return false;
            }


            //验证电话
            $("#ckdh").html("");

            if($("#txtBanGongTel").val()=="") {
                $("#ckdh").html("<font color=red>必填</font>");
                $("#txtBanGongTel").focus();
                return false;
            }

            //验证联系人
            $("#cklxr").html("");

            if($("#txtLXR").val()=="") {
                $("#cklxr").html("<font color=red>必填</font>");
                $("#txtLXR").focus();
                return false;
            }
            if($("#SaleValue").val()=="") {

                alert("业务员必选");
                return false;
            }

            //验证手机
            $("#cksj").html("");

            if($("#txtLXTel").val()=="") {
                $("#cksj").html("<font color=red>必填</font>");
                $("#txtLXTel").focus();
                return false;
            }
            var reg=/^\d{11}/;
            if(!reg.test($("#txtLXTel").val())) {
                $("#cksj").html("<font color=red>只能输入数字，且长度必须在11位</font>");
                $("#txtLXTel").focus();
                return false;
            }
            $("#ckacount").html("");

            if($("#txtAccountCount").val()=="") {
                $("#ckacount").html("<font color=red>必填</font>");
                $("#txtAccountCount").focus();
                return false;
            }
            if($("#hidroletype").val()!="4"&&$("#hidroletype").val()!="5") {
                $("#ckzfbfl").html("");
                if($("#txtcollectionRateAlipay").val()==""||parseFloat($("#txtcollectionRateAlipay").val())<0.001) {
                    $("#ckzfbfl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectionRateAlipay").focus();
                    return false;
                }
                $("#ckzfbgxfl").html("");
                if($("#txtcollectiongxRateAlipay").val()==""||parseFloat($("#txtcollectiongxRateAlipay").val())<0.001) {
                    $("#ckzfbgxfl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectiongxRateAlipay").focus();
                    return false;
                }

                $("#ckkqfl").html("");
                if($("#txtcollectionRate99Bill").val()==""||parseFloat($("#txtcollectionRate99Bill").val())<0.001) {
                    $("#ckkqfl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectionRate99Bill").focus();
                    return false;
                }
                $("#ckkqgxfl").html("");
                if($("#txtcollectiongxRate99Bill").val()==""||parseFloat($("#txtcollectiongxRate99Bill").val())<0.001) {
                    $("#ckkqgxfl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectiongxRate99Bill").focus();
                    return false;
                }
                $("#ckhffl").html("");
                if($("#txtcollectionRateChinaPNR").val()==""||parseFloat($("#txtcollectionRateChinaPNR").val())<0.001) {
                    $("#ckhffl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectionRateChinaPNR").focus();
                    return false;
                }

                $("#ckhfgxfl").html("");
                if($("#txtcollectiongxRateChinaPNR").val()==""||parseFloat($("#txtcollectiongxRateChinaPNR").val())<0.001) {
                    $("#ckhfgxfl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectiongxRateChinaPNR").focus();
                    return false;
                }

                $("#ckcftfl").html("");
                if($("#txtcollectionRateTenpay").val()==""||parseFloat($("#txtcollectionRateTenpay").val())<0.001) {
                    $("#ckcftfl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectionRateTenpay").focus();
                    return false;
                }

                $("#ckcftgxfl").html("");
                if($("#txtcollectiongxRateTenpay").val()==""||parseFloat($("#txtcollectiongxRateTenpay").val())<0.001) {
                    $("#ckcftgxfl").html("<font color=red>不能小于0.001</font>");
                    $("#txtcollectiongxRateTenpay").focus();
                    return false;
                }
            }

            return true;

        }

        function ToLeft(idA,idB,Is)//单个向左
        {
            if((document.getElementById("SalesManA")!=undefined)
            &&(document.getElementById("SalesManA")!=null)
            &&(document.getElementById("SalesManA").length>=1)) {
                alert("只能选择一个业务员！");
            } else {
                var SelectA=document.getElementById(idA);var SelectB=document.getElementById(idB);var SelectIndex=SelectB.selectedIndex;
                if(SelectIndex== -1) {
                    if(SelectB.options.length!=0) { SelectB.selectedIndex=0;SelectIndex=0; } else { return false; }
                }
                var varItem=new Option(SelectB.options[SelectIndex].text,SelectB.options[SelectIndex].value);
                SelectA.options.add(varItem);
                var Hidden=document.getElementById('SaleValue');
                Hidden.value="";
                Hidden.value=SelectB.options[SelectIndex].value.toUpperCase()+"/";
                SelectB.remove(SelectIndex);
                if(SelectB.options.length!=0) { if(Is=="0") { return false; } SelectB.selectedIndex=0; }
            }

        }
        function ToRight(idA,idB,Is)//单个向右
        {
            //            if ((document.getElementById("SalesManA") != undefined)
            //            && (document.getElementById("SalesManA") != null)
            //            && (document.getElementById("SalesManA").length == 1)) {
            //                alert("只能选择一个业务员！");
            //                return false;
            //            }
            var SelectA=document.getElementById(idB);var SelectB=document.getElementById(idA);var SelectIndex=SelectB.selectedIndex;
            if(SelectIndex== -1) {
                if(SelectB.options.length!=0) { SelectB.selectedIndex=0;SelectIndex=0; } else { return false; }
            }
            var varItem=new Option(SelectB.options[SelectIndex].text,SelectB.options[SelectIndex].value);
            SelectA.options.add(varItem);
            var Hidden=document.getElementById('SaleValue');
            Hidden.value=Hidden.value.replace(SelectB.options[SelectIndex].value.toUpperCase()+"/","");
            SelectB.remove(SelectIndex);
            if(SelectB.options.length!=0) { if(Is=="0") { return false; } SelectB.selectedIndex=0; }
        }
        function ToLeftAll(idA,idB)//全部向左
        {
            if((document.getElementById("SalesManA")!=undefined)
            &&(document.getElementById("SalesManA")!=null)
            &&(document.getElementById("SalesManA").length>=1)) {
                alert("只能选择一个业务员！");
            } else {
                var SelectA=document.getElementById(idA);
                var SelectB=document.getElementById(idB);
                var Hidden=document.getElementById('SaleValue');
                Hidden.value="";
                for(var i=0;i<SelectB.options.length;i++) {
                    SelectA.options.add(new Option(SelectB.options[i].text,SelectB.options[i].value));
                    Hidden.value=SelectB.options[i].value.toUpperCase()+"/";
                }
                SelectB.options.length=0;
            }

        }
        function ToRightAll(idA,idB)//全部向右
        {
            //            if ((document.getElementById("SalesManA") != undefined)
            //            && (document.getElementById("SalesManA") != null)
            //            && (document.getElementById("SalesManA").length == 1)) {
            //                alert("只能选择一个业务员！");
            //                return false;
            //            }
            var SelectA=document.getElementById(idB);
            var SelectB=document.getElementById(idA);
            var Hidden=document.getElementById('SaleValue');
            for(var i=0;i<SelectB.options.length;i++) {
                SelectA.options.add(new Option(SelectB.options[i].text,SelectB.options[i].value));
            }
            Hidden.value="";
            SelectB.options.length=0;
        }

       
        
    </script>
    <script language="javascript" type="text/javascript">
        //根据 ie 兼容性 隐藏页面下拉列表元素
        function checkBrowser_hideElement() { 
            var isIE = !!window.ActiveXObject;
            var isIE6 = isIE && !window.XMLHttpRequest;
            var isIE8 = isIE && !!document.documentMode;
            var isIE7 = isIE && !isIE6 && !isIE8;

            if (isIE6) {
                document.getElementById("province").style.display = "none";
                document.getElementById("city").style.display = "none";
            }
        }
        //根据 ie 兼容性 显示页面下拉列表元素
        function checkBrowser_showElement() {
            var isIE = !!window.ActiveXObject;
            var isIE6 = isIE && !window.XMLHttpRequest;
            var isIE8 = isIE && !!document.documentMode;
            var isIE7 = isIE && !isIE6 && !isIE8;

            if (isIE6) {
                document.getElementById("province").style.display = "";
                document.getElementById("city").style.display = "";
            }
        }

        function showdialog(t) {
            checkBrowser_hideElement();

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
                },
                close: function () {
         
                    checkBrowser_showElement();
                }
            });
        }
         function showdialog2(t) {
            checkBrowser_hideElement();

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
                        checkBrowser_showElement();
                    } 
                },
                close: function () {
//                    $(this).dialog('close');
                    checkBrowser_showElement();
                }
            });
        }
        function showdialog3(t,ids) {
            checkBrowser_hideElement();

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
                        location.href='ExploitData.aspx?id='+ids+'&currentuserid=<%=this.mUser.id.ToString() %>';
                        checkBrowser_showElement();
                    },
                    "取消": function () {
                        location.href="DJCpy.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
                        checkBrowser_showElement();
                    }
                },
                close: function () {
 //                   $(this).dialog('close');
                    checkBrowser_showElement();
                }
            });
        }
        //封装格式化日期
        Date.prototype.format = function(format){ 
            var o = { 
                "M+" : this.getMonth()+1, //month 
                "d+" : this.getDate(), //day 
                "h+" : this.getHours(), //hour 
                "m+" : this.getMinutes(), //minute 
                "s+" : this.getSeconds(), //second 
                "q+" : Math.floor((this.getMonth()+3)/3), //quarter 
                "S" : this.getMilliseconds() //millisecond 
            } 

            if(/(y+)/.test(format)) { 
                format = format.replace(RegExp.$1, (this.getFullYear()+"").substr(4 - RegExp.$1.length)); 
            } 

            for(var k in o) { 
                if(new RegExp("("+ k +")").test(format)) { 
                    format = format.replace(RegExp.$1, RegExp.$1.length==1 ? o[k] : ("00"+ o[k]).substr((""+ o[k]).length)); 
                } 
            } 
            return format; 
        }
        
       
      
        function valall()
        {
            if (!checkyz()) {
              return false;
            }
            else
            {
                return true;
            }
        }
       function rolechange()
       {
            var rblvlaue="";
            var vRbtid = document.getElementById("rbroletype");
            //得到所有radio
            var vRbtidList = vRbtid.getElementsByTagName("INPUT");
             for (var i = 0; i < vRbtidList.length; i++) {
                if (vRbtidList[i].checked) {
                    var text = vRbtid.cells[i].innerText;
                    rblvlaue = vRbtidList[i].value;
                }
            }
            if (parseInt(rblvlaue)!=2 && parseInt(rblvlaue)!=4) {
            document.getElementById("txtAccountCount").value="0";
                document.getElementById("txtAccountCount").disabled=true;
                
            }else {
            document.getElementById("txtAccountCount").disabled=false;
            document.getElementById("txtAccountCount").value = document.getElementById("hidaccount").value;
            }
       }


        $(function () {
        <%if (Request.QueryString["id"]==null){ %>
            init();
        <%}else{ %>
        var pc = $("#pc").val().split('|');
        initxiugai(pc[0],pc[1]);//   init();
        <%} %>
            if($("txtShiXiaoDate").val() == ""){
                var d = new Date();
                d.setYear(parseInt( d.getFullYear()) + 1);
                $("#txtShiXiaoDate").val(d.format("yyyy-MM-dd")); 
            }
        });      
        //----------------------------------------------------
        





    </script>
    <script language="javascript" src="../js/js_UserAccount.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuser" runat="server" ClientIDMode="Static" />
    <input id="SaleValue" type="hidden" style="height: 24px" value="" runat="server" />
    <%--<asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>--%>
    <div id="dd">
    </div>
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" cellpadding="0" cellspacing="0" id="table1" class="table_info">
                <tr>
                    <td colspan="4" class="bt">
                        帐号信息
                    </td>
                </tr>
                <tr>
                    <td class="td" style="height: 24px; width: 100px;">
                        <span class="style2">*</span>帐号：
                    </td>
                    <td class="style3">
                        <asp:TextBox ID="txtZH" CssClass="txt" ClientIDMode="Static" runat="server" MaxLength="18"
                            onblur="return Validation('LoginName','txtZH','ckzh');" Height="16px" Width="115px"></asp:TextBox><span
                                style="font-size: 12px" id="ckzh"></span>
                    </td>
                    <td class="td" style="height: 24px; width: 100px;">
                        <span class="style2">*</span>密码：
                    </td>
                    <td class="style3">
                        <asp:TextBox ID="txtshowpass" CssClass="txt" runat="server" MaxLength="18" Visible="false"
                            Enabled="false" Width="115px" Height="16px"></asp:TextBox>
                        <asp:TextBox ID="txtPass" CssClass="txt" runat="server" MaxLength="18" onblur="return Validation('Password','txtPass','ckmm');"
                            Width="115px" Height="16px"></asp:TextBox><span style="font-size: 12px" id="ckmm"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td" style="height: 20px; width: 100px;">
                        <span class="style2">*</span>失效日期：
                    </td>
                    <td class="style4">
                        <asp:TextBox ID="txtShiXiaoDate" CssClass="Wdate txt" runat="server" EnableViewState="False"
                            onfocus="WdatePicker({isShowWeek:true,minDate:'%y-%M-{%d+1}'})" Width="115px"
                            Height="16px" onblur="return Validation('IsNotNull','txtShiXiaoDate','ckrq');"></asp:TextBox><span
                                style="font-size: 12px" id="ckrq"></span>
                    </td>
                    <td class="td" style="height: 20px">
                        <span class="style2">*</span>姓名：
                    </td>
                    <td class="style4">
                        <asp:TextBox ID="txtName" CssClass="txt" runat="server" MaxLength="18" Width="115px"
                            Height="16px" onblur="return Validation('IsChinese','txtName','ckxm');"></asp:TextBox><span
                                style="font-size: 12px" id="ckxm"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td" style="height: 20px; width: 100px;">
                        帐号状态：
                    </td>
                    <td class="style4">
                        <asp:RadioButtonList ID="rblState" runat="server" CssClass="rblState" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0">禁用</asp:ListItem>
                            <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td class="td" style="height: 20px">
                        姓名简码：
                    </td>
                    <td class="style4">
                        <asp:TextBox ID="txtNameEasy" CssClass="txt" runat="server" MaxLength="18" Width="115px"
                            Height="16px" onblur="return Validation('IsChinese','txtName','ckxm');"></asp:TextBox><span
                                style="font-size: 12px" id="Span1"></span>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="bt">
                        公司信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style2" style="height: 20px; width: 100px;">*</span>公司名称：
                    </td>
                    <td>
                        <asp:TextBox ID="txtUnitName" CssClass="txt" runat="server" Columns="16" MaxLength="25"
                            Width="115px" Height="16px" onblur="return Validation('IsChinese','txtUnitName','ckmc');"></asp:TextBox><span
                                style="font-size: 12px" id="ckmc"></span>
                    </td>
                    <td class="td" style="height: 20px; width: 100px;">
                        <span class="style2">*</span>公司电话：
                    </td>
                    <td>
                        <asp:TextBox ID="txtBanGongTel" MaxLength="14" CssClass="txt" runat="server" Columns="16"
                            Width="115px" Height="16px" onblur="return Validation('Tel','txtBanGongTel','ckdh');"></asp:TextBox><span
                                style="font-size: 12px" id="ckdh"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style2" style="height: 20px; width: 100px;">*</span>联系人：
                    </td>
                    <td>
                        <asp:TextBox ID="txtLXR" CssClass="txt" runat="server" MaxLength="20" Columns="16"
                            Width="115px" Height="16px" onblur="return Validation('IsChinese','txtLXR','cklxr');"></asp:TextBox><span
                                style="font-size: 12px" id="cklxr"></span>
                    </td>
                    <td class="td">
                        <span class="style2" style="height: 20px; width: 100px;">*</span>手机号码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtLXTel" MaxLength="11" CssClass="txt" runat="server" Columns="16"
                            Width="115px" Height="16px" onblur="return Validation('Mobile','txtLXTel','cksj');"></asp:TextBox><span
                                style="font-size: 12px" id="cksj"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        电子邮箱：
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" CssClass="txt" runat="server" MaxLength="50" Columns="16"
                            Width="115px" Height="16px" onblur="return Validation('Email','txtEmail','ckemail');"></asp:TextBox><span
                                style="font-size: 12px" id="ckemail"></span>
                    </td>
                    <td class="td">
                        传真号码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtFax" CssClass="txt" MaxLength="13" runat="server" Columns="16"
                            onkeyup="value=value.replace(/[^0-9]/g,'')" onpaste="return false" Width="115px"
                            Height="16px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="td" style="height: 28px; width: 100px;">
                        <%--<span class="style2">*</span>--%>
                        省份：
                    </td>
                    <td class="style5">
                        <asp:DropDownList ID="province" CssClass="txt" runat="server" onChange="select()"
                            Height="21px" Width="128px">
                        </asp:DropDownList>
                        <asp:Label ID="labprovince" runat="server" Text="" Visible="false"></asp:Label>
                    </td>
                    <td class="td" style="height: 28px; width: 100px;">
                        <%--<span class="style2">*</span>--%>市：
                    </td>
                    <td class="style5">
                        <asp:DropDownList ID="city" CssClass="txt" runat="server" Height="21px" Width="128px">
                        </asp:DropDownList>
                        <asp:Label ID="labcity" runat="server" Text="" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="td" style="width: 100px;">
                        网址：
                    </td>
                    <td>
                        <asp:TextBox ID="txtWebSite" CssClass="txt" runat="server"></asp:TextBox>
                    </td>
                    <td class="td" style="width: 100px;">
                        <span class="style2" style="height: 20px; width: 100px;">*</span>可开下级账号数
                    </td>
                    <td>
                        <asp:TextBox ID="txtAccountCount" CssClass="txt" MaxLength="10" Width="115px" runat="server"
                            onkeyup="value=value.replace(/[^0-9]/g,'')" onblur="return Validation('IsNotNull','txtAccountCount','Spanacount')"
                            Text="0"></asp:TextBox>
                        <span style="font-size: 12px" id="ckacount"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td" style="width: 100px;">
                        <%--<span class="style2">*</span>--%>单位地址：
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="txtUnitAddr" CssClass="txt" runat="server" MaxLength="100" Columns="16"
                            Height="22px" Width="70%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="td" style="width: 100px;">
                        角色类型：
                    </td>
                    <td>
                        <%--<asp:RadioButtonList ID="rbroletype" runat="server" AutoPostBack="true"
                            RepeatDirection="Horizontal" 
                            onselectedindexchanged="rbroletype_SelectedIndexChanged" Height="29px">
                        </asp:RadioButtonList>--%>
                        <asp:RadioButtonList ID="rbroletype" runat="server" onclick="rolechange()" RepeatDirection="Horizontal"
                            Height="29px">
                        </asp:RadioButtonList>
                    </td>
                    <td class="td" style="height: 20px; width: 100px;">
                        公司状态：
                    </td>
                    <td class="style4">
                        <asp:RadioButtonList ID="rblcpystate" runat="server" CssClass="rblState" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0">禁用</asp:ListItem>
                            <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr id="tr_Prompt" runat="server">
                    <td class="td" style="width: 100px;">
                        订单提醒：
                    </td>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="cbkPrompt" runat="server" Text="开启订单提醒" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbkEmpPrompt" runat="server" Text="开启员工订单提醒" />
                                </td>
                                <td>
                                    时间间隔:
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="ddlPromptTime" runat="server" RepeatColumns="10">
                                        <asp:ListItem Value="15" Selected="True">15秒</asp:ListItem>
                                        <asp:ListItem Value="30">30秒</asp:ListItem>
                                        <asp:ListItem Value="60">60秒</asp:ListItem>
                                        <asp:ListItem Value="90">90秒</asp:ListItem>
                                        <asp:ListItem Value="120">120秒</asp:ListItem>
                                        <asp:ListItem Value="180">180秒</asp:ListItem>
                                        <asp:ListItem Value="240">240秒</asp:ListItem>
                                        <asp:ListItem Value="300">300秒</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server" visible="false" id="trtime">
                    <td class="td" style="width: 100px;">
                        上下班时间：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlworkHtime" runat="server">
                            <asp:ListItem Value="00">00</asp:ListItem>
                            <asp:ListItem Value="05">05</asp:ListItem>
                            <asp:ListItem Value="06">06</asp:ListItem>
                            <asp:ListItem Value="07">07</asp:ListItem>
                            <asp:ListItem Value="08">08</asp:ListItem>
                            <asp:ListItem Value="09">09</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                        </asp:DropDownList>
                        时
                        <asp:DropDownList ID="ddlworkMtime" runat="server">
                            <asp:ListItem Value="00"></asp:ListItem>
                            <asp:ListItem Value="30"></asp:ListItem>
                        </asp:DropDownList>
                        分 至&nbsp;
                        <asp:DropDownList ID="ddlafterworkHtime" runat="server">
                            <asp:ListItem Value="17">17</asp:ListItem>
                            <asp:ListItem Value="18">18</asp:ListItem>
                            <asp:ListItem Value="19">19</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="21">21</asp:ListItem>
                            <asp:ListItem Value="22">22</asp:ListItem>
                            <asp:ListItem Value="23">23</asp:ListItem>
                        </asp:DropDownList>
                        时
                        <asp:DropDownList ID="ddlafterworkMtime" runat="server">
                            <asp:ListItem Value="00"></asp:ListItem>
                            <asp:ListItem Value="30"></asp:ListItem>
                            <asp:ListItem Value="59"></asp:ListItem>
                        </asp:DropDownList>
                        分
                    </td>
                    <td class="td" style="width: 100px;">
                        业务处理时间：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlBusinessHstartTime" runat="server">
                            <asp:ListItem Value="00">00</asp:ListItem>
                            <asp:ListItem Value="05">05</asp:ListItem>
                            <asp:ListItem Value="06">06</asp:ListItem>
                            <asp:ListItem Value="07">07</asp:ListItem>
                            <asp:ListItem Value="08">08</asp:ListItem>
                            <asp:ListItem Value="09">09</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                        </asp:DropDownList>
                        时
                        <asp:DropDownList ID="ddlBusinessMstartTime" runat="server">
                            <asp:ListItem Value="00"></asp:ListItem>
                            <asp:ListItem Value="30"></asp:ListItem>
                        </asp:DropDownList>
                        分 至&nbsp;
                        <asp:DropDownList ID="ddlBusinessHendTime" runat="server">
                            <asp:ListItem Value="17">17</asp:ListItem>
                            <asp:ListItem Value="18">18</asp:ListItem>
                            <asp:ListItem Value="19">19</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="21">21</asp:ListItem>
                            <asp:ListItem Value="22">22</asp:ListItem>
                            <asp:ListItem Value="23">23</asp:ListItem>
                        </asp:DropDownList>
                        时
                        <asp:DropDownList ID="ddlBusinessMendTime" runat="server">
                            <asp:ListItem Value="00"></asp:ListItem>
                            <asp:ListItem Value="30"></asp:ListItem>
                            <asp:ListItem Value="59"></asp:ListItem>
                        </asp:DropDownList>
                        分
                    </td>
                </tr>
            </table>
            <div id="divQuanXian" style="display: none" runat="server">
                <table width="100%" cellpadding="0" cellspacing="0" id="table311" class="table_info">
                    <tr>
                        <td colspan="8" class="bt">
                            网银类型
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            网银类型：
                        </td>
                        <td colspan="7">
                            <asp:RadioButtonList ID="rblPayType" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="0" Selected="True">不使用网银</asp:ListItem>
                                <asp:ListItem Value="5">支付宝网银</asp:ListItem>
                                <asp:ListItem Value="6">快钱网银</asp:ListItem>
                                <asp:ListItem Value="7">汇付网银</asp:ListItem>
                                <asp:ListItem Value="8">财付通网银</asp:ListItem>
                                <%--<asp:ListItem Value="4">财付通</asp:ListItem>--%>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            网银账号及收款费率(<span style="color: Red">提示:千分之一费率值设为0.001</span>)
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;" width="16%">
                            支付宝收款帐号：
                        </td>
                        <td width="0">
                            <asp:TextBox ID="txtZfbPay" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;" width="20%">
                            支付宝充值收款帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtZfbPayCZ" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;" width="12%">
                            <span class="style2">*</span>本地费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectionRateAlipay" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="font-size: 12px"
                                    id="ckzfbfl"></span>
                        </td>
                        <td style="text-align: right;" width="12%">
                            <span class="style2">*</span>共享费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectiongxRateAlipay" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="font-size: 12px"
                                    id="ckzfbgxfl"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            快钱收款帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtQKPay" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            快钱充值收款帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtQKPayCZ" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            <span class="style2">*</span>本地费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectionRate99Bill" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="font-size: 12px"
                                    id="ckkqfl"></span>
                        </td>
                        <td style="text-align: right;">
                            <span class="style2">*</span>共享费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectiongxRate99Bill" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="font-size: 12px"
                                    id="ckkqgxfl"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            汇付收款帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtHfPay" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            汇付充值收款帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtHfPayCZ" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            <span class="style2">*</span>本地费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectionRateChinaPNR" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                            <span style="font-size: 12px" id="ckhffl"></span>
                        </td>
                        <td style="text-align: right;">
                            <span class="style2">*</span>共享费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectiongxRateChinaPNR" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="font-size: 12px"
                                    id="ckhfgxfl"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            财付通收款帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtCftPay" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            财付通充值收款帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtCftPayCZ" runat="server" CssClass="txt" Enabled="false" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            <span class="style2">*</span>本地费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectionRateTenpay" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="font-size: 12px"
                                    id="ckcftfl"></span>
                        </td>
                        <td style="text-align: right;">
                            <span class="style2">*</span>共享费率：
                        </td>
                        <td>
                            <asp:TextBox ID="txtcollectiongxRateTenpay" CssClass="txt" runat="server" Width="160px"
                                Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="font-size: 12px"
                                    id="ckcftgxfl"></span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            采购佣金取舍
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8">
                            <asp:RadioButtonList ID="rblsetCommission" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="0">采购佣金保留元</asp:ListItem>
                                <asp:ListItem Value="1">采购佣金保留角</asp:ListItem>
                                <asp:ListItem Value="2" Selected="True">采购佣金保留分</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            重要标志设置（供应控制分销权限）<%-- <a href="UserPermissions.aspx" style="float: right;">添加</a>--%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8">
                            <uc1:Importanter ID="Importanter1" runat="server" Identificationtype="0" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            重要标志设置（控制系统权限）
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8">
                            <uc1:Importanter ID="Importanter2" runat="server" Identificationtype="1" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            抢票参数设置
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8">
                            <p>
                                <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;扫描订单生成</span><asp:TextBox ID="txtRobMinuteInner"
                                    runat="server" Text="60" MaxLength="10" size="5"></asp:TextBox><span>分钟之内的抢票订单</span></p>
                            <table width="40%" cellpadding="0" cellspacing="0" border="1" style="border-collapse: collapse;
                                text-align: center;" id="robTab">
                                <thead>
                                    <tr>
                                        <th>
                                            扫描次数
                                        </th>
                                        <th>
                                            时间间隔（分钟）
                                        </th>
                                        <th>
                                            操作
                                        </th>
                                    </tr>
                                    <tr id="trrob_0" class="hide">
                                        <td>
                                            <span id="robscancount_0">第1次</span>
                                        </td>
                                        <td>
                                            <input type="text" id="txtscanTime_0" value="10" maxlength="4" size="5" />
                                        </td>
                                        <td>
                                            <a onclick="AddRob()">添加</a>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            黑屏参数设置
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            <span class="tishi">*</span>&nbsp; 网页黑屏IP：
                        </td>
                        <td>
                            <asp:TextBox ID="txtBlankScreenIp" CssClass="txt" runat="server" Width="160px" MaxLength="15"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            <span class="tishi">*</span>&nbsp; 网页黑屏端口：
                        </td>
                        <td colspan="5">
                            <asp:TextBox ID="txtBlankScreenPort" CssClass="txt" Width="160px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            白屏IP：
                        </td>
                        <td>
                            <asp:TextBox ID="txtWhiteIP" CssClass="txt" Width="160px" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            <span class="tishi">*</span>&nbsp;白屏交互端口：
                        </td>
                        <td colspan="5">
                            <asp:TextBox ID="txtWhiteScreenPort" CssClass="txt" Width="160px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            <span class="tishi">*</span>&nbsp; Office号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtoffice" CssClass="txt" runat="server" Width="320px"></asp:TextBox>
                        </td>
                        <td colspan="7">
                            <span style="font-size: 12px; color: Red;">多个OFfice用"^"隔开 预订使用第一个Office号 </span>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            网页黑屏帐号：
                        </td>
                        <td>
                            <asp:TextBox ID="txtBlankUser" CssClass="txt" runat="server" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            网页黑屏密码：
                        </td>
                        <td>
                            <asp:TextBox ID="txtBlankPwd" CssClass="txt" runat="server" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            EC网页黑屏监听端口:
                        </td>
                        <td colspan="3" style="text-align: left;">
                            <asp:TextBox ID="txtECBlankPort" CssClass="txt" runat="server" Width="160px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            开票单位名称：
                        </td>
                        <td style="text-align: left;">
                            <asp:TextBox ID="txtTicketCompany" runat="server" CssClass="txt" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            航协号：
                        </td>
                        <td style="text-align: left;">
                            <asp:TextBox ID="txtHangxiehao" runat="server" CssClass="txt" Width="160px"></asp:TextBox>
                        </td>
                        <td colspan="4" style="text-align: left;">
                            <span style="font-size: 12px; color: Red;">多个航协号和单位名称用“^”隔开</span>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            PID与KeyNo：
                        </td>
                        <td style="text-align: left;">
                            <asp:TextBox ID="txtPidKeyNo" runat="server" CssClass="txt" Width="160px"></asp:TextBox>
                            <span style="font-size: 12px; color: Red;">PID与KeyNo用"^隔开"</span>
                        </td>
                        <td style="text-align: right;">
                        </td>
                        <td style="text-align: left;">
                        </td>
                        <td colspan="4" style="text-align: left;">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            大配置参数设置
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            <span class="tishi">*</span>&nbsp; 大配置IP：
                        </td>
                        <td>
                            <asp:TextBox ID="txtBigConfigIP" CssClass="txt" runat="server" Width="160px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            大配置端口：
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtBigConfigPort" CssClass="txt" runat="server" Width="160px"></asp:TextBox>
                        </td>
                        <td colspan="2">
                            <div>
                                <span class="tishi">*</span>&nbsp; 大配置Office：
                                <asp:TextBox ID="txtBigOffice" CssClass="txt" runat="server" Width="160px"></asp:TextBox>
                            </div>
                        </td>
                        <td style="text-align: right;">
                            <span class="tishi">*</span>&nbsp; 大配置名称与密码：
                        </td>
                        <td>
                            <asp:TextBox ID="txtBigPwd" CssClass="txt" runat="server" Width="160px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" class="bt">
                            接口账号设置
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            517接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKact517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.517na.com','517')">http://www.517na.com</a>
                        </td>
                        <td style="text-align: right;">
                            517接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwd517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            517接口KEY:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKkey517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            517预存款账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyckack517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            517预存款密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyckpwd517" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            51book接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKact51book" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.51book.com','51book')">http://www.51book.com</a>
                        </td>
                        <td style="text-align: right;">
                            51book接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwd51book" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            51book接口KEY:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKkey51book" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            51book通知地址:
                        </td>
                        <td>
                            <asp:TextBox ID="txtNoticeURL51book" CssClass="txt" Width="120px" runat="server"
                                Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            百托接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKactBT" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.baitour.com','bt')">http://www.baitour.com</a>
                        </td>
                        <td style="text-align: right;">
                            百托接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwdBT" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            百托接口KEY:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKkeyBT" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            票盟接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKactPM" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://www.piaomeng.net.cn','pm')">http://www.piaomeng.net.cn</a>
                        </td>
                        <td style="text-align: right;">
                            票盟接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwdPM" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            票盟接口KEY:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKkeyPM" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            今日接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKactJR" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            &nbsp;&nbsp;<a title="点击进入" style="color: Blue" onclick="window.open('http://new.jinri.net.cn','jr')">http://new.jinri.net.cn</a>
                        </td>
                        <td style="text-align: right;">
                            今日接口子账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwdJR" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            8000yi接口账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKact8000yi" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>&nbsp;&nbsp;<a
                                title="点击进入" style="color: Blue" onclick="window.open('http://www.8000yi.com','8000yi')">http://www.8000yi.com</a>
                        </td>
                        <td style="text-align: right;">
                            8000翼接口密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtJKpwd8000yi" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            8000翼代扣支付宝:
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtJKDKZFB8000yi" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                            <asp:LinkButton ID="lk8000yiZFBSigning" runat="server" OnClick="lk8000yiZFBSigning_Click">签约</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            易行总账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyixing" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>&nbsp;&nbsp;<a
                                title="点击进入" style="color: Blue" onclick="window.open('http://www.yeexing.com','yeexing')">http://www.yeexing.com</a>
                        </td>
                        <td style="text-align: right;">
                            易行供应账号:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyixinggy" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>
                        </td>
                        <td style="text-align: right;">
                            易行供应密码:
                        </td>
                        <td>
                            <asp:TextBox ID="txtyixingpwd" CssClass="txt" Width="120px" runat="server" Text=""></asp:TextBox>                            
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="YiXingSigning_Click">签约</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divFenXiaoQuanXian" style="display: none" runat="server">
                <table width="100%" cellpadding="0" cellspacing="0" id="table2" class="table_info">
                    <tr>
                        <td colspan="8" class="bt">
                            参数信息<%-- <a href="UserPermissions.aspx" style="float: right;">添加</a>--%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8">
                            <uc2:ImportanterC ID="Importanter3" runat="server" CpyNo="" />
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <table width="100%" cellpadding="0" cellspacing="0" id="table3" class="table_info">
                    <tr>
                        <td colspan="8" class="bt">
                            所属业务员
                        </td>
                    </tr>
                    <tr>
                        <td class="td">
                        </td>
                        <td colspan="5">
                            <table width="648px" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div>
                                            <%--<span class="style2">*</span>--%>已选业务员
                                            <asp:Label ID="lblUserMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                                            <br />
                                            <asp:ListBox ID="SalesManA" runat="server" Height="100px" Width="200px" onclick="ToRight('SalesManA','SalesManB','0');">
                                            </asp:ListBox>
                                        </div>
                                    </td>
                                    <td>
                                        <div style="width: 48px; height: 100px; margin-left: 20px; margin-top: 25px;">
                                            <input id="Button1" type="button" style="height: 24px; width: 31px" value=">" onclick="ToRight('SalesManA','SalesManB','1');" />
                                            <input id="Button2" type="button" style="height: 24px; width: 31px" value="<" onclick="ToLeft('SalesManA','SalesManB','1');" />
                                            <input id="Button3" type="button" style="height: 24px; width: 31px" value=">>" onclick="ToRightAll('SalesManA','SalesManB','1'); " />
                                            <input id="Button4" type="button" style="height: 24px; width: 31px; display: none"
                                                value="<<" onclick="ToLeftAll('SalesManA','SalesManB','1'); " />
                                            <input id="Hidden1" type="hidden" style="height: 24px" value="" runat="server" />
                                        </div>
                                    </td>
                                    <td>
                                        <div>
                                            备选业务员
                                            <br />
                                            <asp:ListBox ID="SalesManB" runat="server" Height="100px" Width="200px" onclick="ToLeft('SalesManA','SalesManB','0');">
                                            </asp:ListBox>
                                        </div>
                                    </td>
                                    <td>
                                        <div style="width: 200px; padding-left: 10px; padding-right: 10px; margin-top: 5px;">
                                            <fieldset>
                                                <legend style="color: #666666">温馨提示:</legend>请您在右边的员工列表中选择该用户所属的业务员 ,单击或点击" < "添加至左边的所属业务员列表中
                                            </fieldset>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <asp:LinkButton ID="lbtnOK" runat="server" CssClass="btn btnNormal" OnClientClick="return handle();"
                            OnClick="lbtnOK_Click">保  存 </asp:LinkButton>
                        &nbsp; &nbsp;&nbsp;&nbsp;<a href="ComPanyList.aspx?currentuserid=<%=this.mUser.id.ToString() %>"
                            class="btn btnNormal">返 回</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <%-- </ContentTemplate>
    </asp:UpdatePanel>--%>
    <input type="hidden" runat="server" id="hidId" value="" />
    <input type="hidden" id="hidPro" runat="server" value="" />
    <input type="hidden" id="hidCity" runat="server" value="" />
    <input id="pc" type="hidden" runat="server" />
    <input id="hidaccount" type="hidden" runat="server" value="0" />
    <input id="hidroletype" type="hidden" runat="server" value="4" />
    <%--抢票参数设置--%>
    <input id="Hid_RobSetting" type="hidden" runat="server" />
    </form>
</body>
</html>
