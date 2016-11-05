<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddUser.aspx.cs" Inherits="Air_Person_AddUser"
    EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="x-ua-compatible" content="ie=7" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            color: red;
        }
        .txtip
        {
         width: 200px;
        }
         .hide
        {
            display: none;
        }
    </style>
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <script src="../js/Validation.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

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

        function checkzh() {
            if($("#txtUser").val()=="") {
                $("#ckzh").html("");
                return;
            }
            $.post("../Ajax/UserZh.aspx",{ zh: $("#txtUser").val(),currentuserid:'<%=this.mUser.id.ToString() %>' },
                            function (data) {
                                var r=data;
                                if(r=="1") {
                                    $("#ckzh").html("<font color=red>账号存在</font>");
                                    $("#txtUser").select();
                                } else {
                                    $("#ckzh").html("<font color=green>账号可用</font>");
                                }
                            },"text"
        );
        }
        //根据 ie 兼容性 隐藏页面下拉列表元素
        function checkBrowser_hideElement() {
            var isIE=!!window.ActiveXObject;
            var isIE6=isIE&&!window.XMLHttpRequest;
            var isIE8=isIE&&!!document.documentMode;
            var isIE7=isIE&&!isIE6&&!isIE8;

            if(isIE6) {
                document.getElementById("ddlCertificateType").style.display="none";
            }
        }
        //根据 ie 兼容性 显示页面下拉列表元素
        function checkBrowser_showElement() {
            var isIE=!!window.ActiveXObject;
            var isIE6=isIE&&!window.XMLHttpRequest;
            var isIE8=isIE&&!!document.documentMode;
            var isIE7=isIE&&!isIE6&&!isIE8;

            if(isIE6) {
                document.getElementById("ddlCertificateType").style.display="";
            }
        }

        function getValue() {
            try {
                return checkyz();
            }
            catch(ex)
                    { }
        }

        function checkTxtTel(obj) {
            var val="";
            if(obj.value!=null&&obj.value.length>=1) {
                val=obj.value.substring(obj.value.length-1,obj.value.length); // 获取子字符串。
            }
            else if(obj.value!=null&&obj.value.length==14) {
                val="";
            }
            var str="0123456789";
            if(obj.value.length==4) {
                str+="-";
            }
            else if(obj.value.length==5) {
                str+="-";
                var str4=obj.value.substring(3,4);
                var str5=obj.value.substring(4,5);
                if(str4==str5&&str4=="-") {
                    val="";
                }
            }
            if(val!="") {
                val=(str.indexOf(val)> -1)?val:"";
            }
            obj.value=obj.value.substring(0,obj.value.length-1)+val;
        }
        //验证
        function checkyz() {
            if($("#hidId").val()=="") {
                $("#ckzh").html("");
                //验证账号
                if($("#txtUser").val()=="") {
                    $("#ckzh").html("<font color=red>必填</font>");
                    $("#txtUser").focus();
                    return false;
                }
                var reg=/^\w{2,18}/;
                if(!reg.test($("#txtUser").val())) {
                    $("#ckzh").html("<font color=red>只能输入字母或数字，且长度必须在2-18位之间</font>");
                    $("#txtUser").focus();
                    return false;
                }
                //            else {
                //                $.post("../Ajax/UserZh.aspx", { zh: $("#txtUser").val() },
                //                            function (data) {
                //                                var r = data;
                //                                if (r == "1") {
                //                                    $("#ckzh").html("<font color=red>账号存在</font>");
                //                                    $("#txtUser").focus();
                //                                    return false;
                //                                } else {
                //                                    $("#ckzh").html("<font color=green>账号可用</font>");

                //                                }
                //                            }, "text"
                //        );
                //            }


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
            //验证工号
            $("#ckgh").html("");

            if($("#txtGong").val()=="") {
                $("#ckgh").html("<font color=red>必填</font>");
                $("#txtGong").focus();
                return false;
            }
            //验证姓名
            $("#ckxm").html("");

            if($("#txtName").val()=="") {
                $("#ckxm").html("<font color=red>必填</font>");
                $("#txtName").focus();
                return false;
            }

            //            if ($.trim($("#HiddSaveId").val()) == "") {
            //                showDiv("请选择一个职位权限！");
            //                return false;
            //            }

            return true;
        }
        function checkUrlDepartPermission() {
            var id=$("#Permissions").val();
            var text=$("#Permissions").find(":selected").text();
            var url="../Company/Department.aspx?Type=1&id="+id+"&rnd="+Math.random()+"&currentuserid=<%=this.mUser.id.ToString() %>";
            showdialog_zs(url,400,400);

        }
        function save(id) {

            $("#HiddSaveId").val(id);
            $("#Permissions option[value='"+id+"']").attr("selected",true);
            /*
            for (var i = 0; i < $("#Permissions")[0].length; i++) {
            if ($("#Permissions")[0][i].value == id) {
            $("#Permissions")[0][i].selected = true;
            }
            }
            $("#HiddSaveId").val(id);
            */
        }
        //页面加载
        $(document).ready(function () {
            var id=$("#Permissions option:selected").val();
            if(id!=undefined) {
                $("#HiddSaveId").val(id);
            }
        });
        function addTR(i) {
            //debugger;
            document.getElementById("hidtxtCount").value = parseInt(document.getElementById("hidtxtCount").value) + 1;
            //隐藏当前tr中的添加，删除按钮
            if (i < 4) {
                document.getElementById("sAdd" + i).style.display = "none";
            }
            if (i > 0) {
                document.getElementById("sDelete" + i).style.display = "none";
            }
            //显示下一个tr
            document.getElementById("tr" + (i + 1)).style.display = "block";
        }

        //删除一个
        function delTR(i) {
            document.getElementById("hidtxtCount").value = parseInt(document.getElementById("hidtxtCount").value) - 1;
            //显示上一个tr中的添加，删除按钮
            document.getElementById("sAdd" + (i - 1)).style.display = "block";
            if (i > 1) {
                document.getElementById("sDelete" + (i - 1)).style.display = "block";
            }
            //隐藏当前tr
            document.getElementById("tr" + i).style.display = "none";
        }
        function cleartxt() {
            for (var i = 0; i < 5; i++) {
                document.getElementById("txtA" + i).value = "";
            }
            document.getElementById("hidtxtCount").value = 1;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <input type="hidden" id="picurl" name="picurl" />
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info">
                <tr>
                    <td colspan="4" class="bt">
                        基本信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>帐号：
                    </td>
                    <td>
                        <asp:TextBox ID="txtUser" CssClass="txt" runat="server" MaxLength="18" Columns="16"></asp:TextBox><span
                            style="font-size: 12px" id="ckzh"></span>
                    </td>
                    <td class="td">
                        <span class="style1">*</span>密码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtPass" CssClass="txt" runat="server" Columns="18" MaxLength="18"
                            TextMode="Password"></asp:TextBox>
                        <input type="hidden" value="" runat="server" id="hidPass" /><span style="font-size: 12px"
                            id="ckmm"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>工号：
                    </td>
                    <td>
                        <asp:TextBox ID="txtGong" CssClass="txt" runat="server" Columns="16" MaxLength="20"></asp:TextBox><span
                            style="font-size: 12px" id="ckgh"></span>
                    </td>
                    <td class="td">
                        <span class="style1">*</span>真实姓名：
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" CssClass="txt" runat="server" Columns="16" MaxLength="20"></asp:TextBox><span
                            style="font-size: 12px" id="ckxm"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        到期日期：
                    </td>
                    <td>
                        <input id="txtovertime" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                            onfocus="WdatePicker({isShowClear:false})" />
                    </td>
                    <td class="td">
                        姓名简码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtNameEasy" CssClass="txt" runat="server" Columns="16" MaxLength="20"></asp:TextBox><span
                            style="font-size: 12px" id="Span1"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        证件类型：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCertificateType" CssClass="txt" runat="server" DataTextField="ChildName"
                            DataValueField="ChildID" AppendDataBoundItems="true">
                            <asp:ListItem Value="" Selected="True">--选择--</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="td">
                        证件号码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtCertificateNum" CssClass="txt" runat="server" Columns="16"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        性别：
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblSex" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0" Selected="True">男</asp:ListItem>
                            <asp:ListItem Value="1">女</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td class="td">
                        状态：
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblState" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0">禁用</asp:ListItem>
                            <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        部门页面权限：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlUserPermissions" runat="server">
                        </asp:DropDownList>
                        <asp:LinkButton ID="lbtnAddUserPermissions" runat="server">添加</asp:LinkButton>
                    </td>
                    <td class="td">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="bt">
                        联系信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        手机号码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtPhone" CssClass="txt" runat="server" Columns="16" MaxLength="11"
                            onkeyup="value=value.replace(/[^0-9]/g,'')" onpaste="return true"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtPhone"
                            ErrorMessage="号码不符合" ForeColor="Red" ValidationExpression="((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)"
                            Display="Dynamic"></asp:RegularExpressionValidator>
                    </td>
                    <td class="td">
                        &nbsp;联系电话：
                    </td>
                    <td>
                        <asp:TextBox ID="txtTel" CssClass="txt" runat="server" Columns="16" MaxLength="13"
                            onkeyup="checkTxtTel(this)" onpaste="return true"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtTel"
                            ErrorMessage="号码不符合" ForeColor="Red" ValidationExpression="((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)"
                            Display="Dynamic"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        QQ：
                    </td>
                    <td>
                        <asp:TextBox ID="txtQQ" CssClass="txt" runat="server" Columns="16" MaxLength="12"
                            onkeyup="value=value.replace(/[^0-9]/g,'')" onpaste="return true"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="txtQQ"
                            ErrorMessage="QQ号码不合法" ForeColor="Red" ValidationExpression="[1-9][0-9]{4,}"
                            Display="Dynamic"></asp:RegularExpressionValidator>
                    </td>
                    <td class="td">
                        MSN：
                    </td>
                    <td>
                        <asp:TextBox ID="txtMSN" CssClass="txt" runat="server" Columns="16" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        Email：
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" CssClass="txt" runat="server" Columns="16" MaxLength="50"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtEmail"
                            ErrorMessage="email不符合" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            Display="Dynamic"></asp:RegularExpressionValidator>
                    </td>
                    <td class="td">
                        邮政编码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtYB" CssClass="txt" runat="server" Columns="16" MaxLength="6"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtYB"
                            ErrorMessage="邮编不符合" ForeColor="Red" ValidationExpression="\d{6}" Display="Dynamic"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        联系地址：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtAddr" CssClass="txt" runat="server" Columns="60" MaxLength="100"
                            Width="495px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        备注：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtBZ" CssClass="txt" runat="server" Columns="16" Height="37px"
                            TextMode="MultiLine" Width="496px" MaxLength="150"></asp:TextBox>
                    </td>
                </tr>
                 <div id="divipset" runat="server" visible="false">
                <tr>
                    <td colspan="6" class="bt">
                        允许登录IP设置
                    </td>
                </tr>
                <tr id="tr0" runat="server">
                        <th class="w_td">
                            IP1
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA0" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span  id="sAdd0" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd0" onclick="addTR(0)" /></span>
                                    </td>
                                     <td>
                                        <span id="Span2" runat="server">
                                            <input type="button" value=" 清  空 " id="btclear" onclick="cleartxt()"/></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr1" runat="server" class="hide">
                        <th class="w_td">
                            IP2
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA1" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sAdd1" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd1" onclick="addTR(1)" /></span>
                                    </td>
                                    <td>
                                        <span id="sDelete1" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete1" onclick="delTR(1)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr2" runat="server" class="hide">
                        <th class="w_td">
                            IP3
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA2" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sAdd2" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd2" onclick="addTR(2)" /></span>
                                    </td>
                                    <td>
                                        <span id="sDelete2" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete2" onclick="delTR(2)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr3" runat="server" class="hide">
                        <th class="w_td">
                            IP4
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA3" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sAdd3" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd3" onclick="addTR(3)" /></span>
                                    </td>
                                    <td>
                                        <span id="sDelete3" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete3" onclick="delTR(3)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                <tr id="tr4" runat="server" class="hide">
                        <th class="w_td">
                            IP5
                        </th>
                        <td colspan="3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA4" runat="server" CssClass="txtip" MaxLength="15" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span id="sDelete4" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete4" onclick="delTR(4)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
            </div>
             <tr align="center">
                    <td height="35" align="center" class="btni" colspan="4">
                    <div style="text-align:center">
                    <asp:LinkButton ID="lbsave" runat="server" CssClass="btn btnNormal" OnClientClick="return getValue();"
                            OnClick="lbsave_Click">保  存 </asp:LinkButton>
                        &nbsp; &nbsp;&nbsp;&nbsp;<a href="UserList.aspx?currentuserid=<%=this.mUser.id.ToString() %>" class="btn btnNormal">取 消 </a>
                    </div>
                    </td>
                </tr>
            </table>
           
        </div>
        <table width="100%" border="0" class="sugges">
            <tr>
                <td class="sugtitle">
                    温馨提示：
                </td>
            </tr>
            <tr>
                <td class="sugcontent">
                    1、帐号只能输入英文字符和整数，密码默认123参数设置，可修改。
                    <br />
                    2、帐号不能小于三位；访问密码不能小于四位；会员姓名不能小于二位。
                </td>
            </tr>
        </table>
        <input type="hidden" id="hidId" runat="server" value="" />
        <input type="hidden" id="HiddSaveId" runat="server" value="" />
    </div>
    <asp:Literal ID="LiterScript" runat="server"></asp:Literal>
    <input type="hidden" runat="server" id="hidtxtCount" value="1" />
    </form>
</body>
</html>
