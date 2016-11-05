<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Company.aspx.cs" Inherits="Company"
    EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>基本信息</title>
    <meta http-equiv="x-ua-compatible" content="ie=7,ie=8" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../js/area.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <script src="../js/j.suggest.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        //根据 ie 兼容性 隐藏页面下拉列表元素
        function checkBrowser() {
            var isIE=!!window.ActiveXObject;
            var isIE6=isIE&&!window.XMLHttpRequest;
            var isIE8=isIE&&!!document.documentMode;
            var isIE7=isIE&&!isIE6&&!isIE8;

            if(isIE6) {
                return true;
            }
        }

        //根据 ie 兼容性 隐藏页面下拉列表元素
        function checkBrowser_hideElement() {
            var isIE=!!window.ActiveXObject;
            var isIE6=isIE&&!window.XMLHttpRequest;
            var isIE8=isIE&&!!document.documentMode;
            var isIE7=isIE&&!isIE6&&!isIE8;

            if(isIE6) {
                document.getElementById("province").style.display="none";
                document.getElementById("city").style.display="none";

                document.getElementById("ddlUpWorkTime").style.display="none";
                document.getElementById("ddlDownWorkTime").style.display="none";

                document.getElementById("ddlTimeFG1").style.display="none";
                document.getElementById("ddlTimeFG2").style.display="none";

                document.getElementById("boxTXTQQ").style.display="none";
            }
        }
        //根据 ie 兼容性 显示页面下拉列表元素
        function checkBrowser_showElement() {
            var isIE=!!window.ActiveXObject;
            var isIE6=isIE&&!window.XMLHttpRequest;
            var isIE8=isIE&&!!document.documentMode;
            var isIE7=isIE&&!isIE6&&!isIE8;

            if(isIE6) {
                document.getElementById("province").style.display="";
                document.getElementById("city").style.display="";

                document.getElementById("ddlUpWorkTime").style.display="";
                document.getElementById("ddlDownWorkTime").style.display="";

                document.getElementById("ddlTimeFG1").style.display="";
                document.getElementById("ddlTimeFG2").style.display="";

                document.getElementById("boxTXTQQ").style.display="";
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

        function showdialog3(t) {
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
                    $(this).dialog('close');
                    checkBrowser_showElement();
                }
            });
        }
        function SetHidBox(fg) {
            var Box=document.getElementById("boxTXTQQ");
            if(Box!=null) {
                if(fg=="add") {
                    //debugger;
                    var v1=$.trim($("#txt_Effect").val());
                    var v2=$.trim($("#txt_Lianxi").val());
                    var v3=$.trim($("#txt_QQ").val());

                    if(v1==""||v2==""||v3=="") {
                        //if (!checkBrowser())
                        showdialog("信息不完整，请输入三项内容！");
                        //else;
                        //showdialog("<iframe style=\"width:100%;height:100%;filter:Alpha(opacity=0);\" border=\"0\" frameborder=\"0\">信息不完整，请输入三项内容！</iframe>");
                        return;
                    }
                    var pt=/\d+/g;
                    if(!pt.test(v3)) {
                        showdialog("QQ号码必须为数字");
                        return;
                    }
                    String.prototype.isMobile=function () {
                        //几种开头的手机号 13 15 158 159 18
                        return (/^(?:13\d|15[89]|18\d)-?\d{5}(\d{3}|\*{3})$/.test(this.toString()));
                    }
                    String.prototype.isTel=function () {
                        //"兼容格式: 国家代码(2到3位)-区号(2到3位)-电话号码(7到8位)-分机号(3位)"
                        //return (/^(([0\+]\d{2,3}-)?(0\d{2,3})-)?(\d{7,8})(-(\d{3,}))?$/.test(this.Trim()));
                        return (/^(\d{7,8})?$/.test(this.toString()));
                    }
                    /*
                    if ((!v3.isMobile()) && (!v3.isTel())) {
                    showdialog("联系方式必须是手机或者电话");
                    return;
                    }
                    */
                    var v4=v1+"  "+v2+"  "+v3;
                    var v5=v1+"#"+v2+"#"+v3;
                    Box.options.add(new Option(v4,v5));
                    Box.options.selectedIndex=Box.options.length-1;
                }
                else if(fg=="del") {
                    // Box.options.remove(Box.options[Box.selectedIndex]);   
                    Box.removeChild(Box.options[Box.selectedIndex]);
                }
                if(fg==undefined) {
                    var hidObj=$.trim($("#Hid_KefuValue").val());
                    //加载
                    if(hidObj!='') {
                        var arr=hidObj.split('@');
                        var dis='';
                        var arr1;
                        for(var i=0;i<arr.length;i++) {
                            if(arr[i]!='') {
                                arr1=arr[i].split('#');
                                dis="描述:"+arr1[0]+" 联系方式:"+arr1[1]+" QQ："+arr1[2];
                                Box.options.add(new Option(dis,arr[i]));
                            }
                        }
                    }
                } else {
                    //操作
                    var v6='';
                    for(var i=0;i<Box.options.length;i++) {
                        v6+=Box.options[i].value+"@";
                    }
                    $("#Hid_KefuValue").val(v6);
                }

            }
        }
        function IsChildTicket(obj) {
            document.getElementById("tr_child").style.display=obj.checked?"block":"none";
            if(document.getElementById("tr_child").style.display=="none") {
                document.getElementById("txtDefauleChildPolicy").value="2.5";
            }
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
        
    </script>
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .table_info .td
        {
            text-align: right;
        }
        .table_info td
        {
            text-align: left;
        }
        .table_info tr
        {
            line-height: 30px;
            height: 0px;
        }
        #table_info tr
        {
            line-height: 30px;
            height: 0px;
        }
    </style>
</head>
<body onload="SetHidBox();">
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div class="infomain">
        <div class="mainPanel">
            <table id="table_info" class="table_info" width="100%" cellspacing="0" cellpadding="0"
                border="0">
                <tr class="leftliebiao_checi">
                    <td colspan="6" class="bt">
                        基本信息
                    </td>
                </tr>
                <tr>
                    <td style="width: 12%; text-align: right; background-color: #ffffff; color: #424242;
                        font-size: 12px; line-height: 24px; margin: 0; border: none;">
                        单位名称：
                    </td>
                    <td style="width: 21%;">
                        <asp:TextBox ID="txtUnitName" CssClass="txt" runat="server" Style="width: 130px;"
                            Columns="18" MaxLength="10"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtUnitName"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 12%; text-align: right; background-color: #ffffff; color: #424242;
                        font-size: 12px; line-height: 24px; margin: 0; border: none;">
                        联系人：
                    </td>
                    <td style="width: 21%;">
                        <asp:TextBox ID="txtLXR" CssClass="txt" runat="server" Style="width: 130px;" Columns="16"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtLXR"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic">
                        </asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 12%; text-align: right; background-color: #ffffff; color: #424242;
                        font-size: 12px; line-height: 24px; margin: 0; border: none;">
                        电子邮箱：
                    </td>
                    <td style="width: 22%;">
                        <asp:TextBox ID="txtEmail" CssClass="txt" runat="server" Style="width: 130px;" Columns="16"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtEmail"
                            ErrorMessage="不符合" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            Display="Dynamic">
                        </asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        公司电话：
                    </td>
                    <td>
                        <asp:TextBox ID="txtBanGongTel" CssClass="txt" runat="server" Style="width: 130px;"
                            Columns="16" onkeyup="checkTxtTel(this)" onpaste="return false">
                        </asp:TextBox>
                    </td>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        联系电话：
                    </td>
                    <td>
                        <asp:TextBox ID="txtLXTel" runat="server" Style="width: 130px;" CssClass="txt" Columns="16"
                            onkeyup="value=value.replace(/[^0-9]/g,'')" onpaste="return false">
                        </asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtLXTel"
                            ErrorMessage="必填" ForeColor="Red" Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtLXTel"
                            ErrorMessage="不符合" ForeColor="Red" ValidationExpression="((\d{11}))" Display="Dynamic">
                        </asp:RegularExpressionValidator>
                    </td>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        传真号码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtFax" runat="server" CssClass="txt" Style="width: 130px;" Columns="16"
                            onkeyup="value=value.replace(/[^0-9]/g,'')" onpaste="return false">
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        省份：
                    </td>
                    <td>
                        <asp:DropDownList ID="province" CssClass="txt" Style="width: 140px;" runat="server"
                            onChange="select()">
                        </asp:DropDownList>
                    </td>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        市：
                    </td>
                    <td>
                        <asp:DropDownList ID="city" CssClass="txt" Style="width: 140px;" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        网址：
                    </td>
                    <td>
                        <asp:TextBox ID="txtWebSite" CssClass="txt" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        单位地址：
                    </td>
                    <td colspan="5">
                        <asp:TextBox ID="txtUnitAddr" Style="width: 600px;" CssClass="txt" runat="server"
                            Columns="16"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" visible="false" id="trtime">
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        上下班时间：
                    </td>
                    <td colspan="2">
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
                            <asp:ListItem Value="59">59</asp:ListItem>
                        </asp:DropDownList>
                        分
                    </td>
                    <td style="text-align: right; background-color: #ffffff; color: #424242; font-size: 12px;
                        line-height: 24px; margin: 0; border: none;">
                        业务处理时间：
                    </td>
                    <td colspan="2">
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
                <tr id="trdlfx" runat="server" visible="false">
                    <td>
                        &nbsp;
                    </td>
                    <td colspan="5">
                        <asp:CheckBox ID="cksetdlfx" runat="server" Text="设置为独立分销" Visible="false" />
                        <asp:CheckBox ID="ckshowdlinfo" runat="server" Text="显示自己独立信息" Visible="false" />
                    </td>
                </tr>
                <tr class="leftliebiao_checi" id="tr_HeadPrompt" runat="server">
                    <td colspan="6" class="bt">
                        订单提醒设置
                    </td>
                </tr>
                <tr id="tr_Prompt" runat="server">
                    <td colspan="6">
                        <table style="margin-left: 100px;">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="cbkPrompt" runat="server" Text="开启订单提醒" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbkEmpPrompt" runat="server" Text="开启员工订单提醒" />
                                </td>
                            </tr>
                            <tr>
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
                <tr class="leftliebiao_checi">
                    <td colspan="6" class="bt">
                        客服信息
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellspacing="0" cellpadding="0" border="0" style="list-style-type: none;">
                            <tr>
                                <td class="td">
                                    描述:
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    联系方式:
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    QQ:
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table cellspacing="0" cellpadding="0" border="0" style="list-style-type: none;">
                            <tr>
                                <td class="td">
                                    <input type="text" id="txt_Effect" style="width: 140px;" />
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    <input type="text" id="txt_Lianxi" maxlength="12" style="width: 140px;" />
                                </td>
                            </tr>
                            <tr>
                                <td class="td">
                                    <input type="text" id="txt_QQ" maxlength="15" style="width: 140px;" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="td" style="text-align: center;">
                        <span class="btn btn-ok-s">
                            <input type="button" id="btnAdd" style="margin-left: -2px;" value="添加" onclick="SetHidBox('add')" />
                        </span>
                    </td>
                    <td colspan="4">
                        <asp:ListBox ID="boxTXTQQ" size="50" runat="server" Style="width: 400px; height: 120px;"
                            ondblclick="SetHidBox('del')" SelectionMode="Multiple"></asp:ListBox>
                        <input type="hidden" id="Hid_KefuValue" runat="server" /><br />
                        <font style="color: Green;">* 双击删除</font>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <span class="btn btn-ok-s">
                            <asp:LinkButton ID="LinkButton1" runat="server" Style="display: block" OnClick="LinkButton1_Click">保  存</asp:LinkButton></span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <input type="hidden" id="hidPro" runat="server" value="" />
    <input type="hidden" id="hidCity" runat="server" value="" />
    </form>
</body>
</html>
