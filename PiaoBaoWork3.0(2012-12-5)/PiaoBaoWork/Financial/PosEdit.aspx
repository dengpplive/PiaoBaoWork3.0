<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PosEdit.aspx.cs" EnableEventValidation="false" Inherits="Financial_PosEdit" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript">
        function yanzheng(obj) {
            //先把非数字的都替换掉，除了数字和.
            obj.value = obj.value.replace(/[^\d.]/g, "");
            //必须保证第一个为数字而不是.
            obj.value = obj.value.replace(/^\./g, "");
            //保证只有出现一个.而没有多个.
            obj.value = obj.value.replace(/\.{2,}/g, ".");
            //保证.只出现一次，而不能出现两次以上
            obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
        }
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
      
        function OnSubmit() {
        
                if (document.getElementById("txtTerminalNo").value == "") {
                    showdialog("POS机终端号不能为空");
                    return false;
                } 
                if (document.getElementById("txtfeil").value == "") {
                    showdialog("费率必填");
                    return false;
                }
                else if ($("#txtfeil").val() == "" || parseFloat($("#txtfeil").val()) < 0.001 || parseFloat($("#txtfeil").val()) > 1000) {
                        showdialog("费率不能小于0.001");
                        $("#ckcftfl").html("<font color=red>不能小于0.001</font>");
                        return false;
                }

            }


            function SelCustomer(vals) {
                if (vals != "") {
                    var CustomerList = $("#hidcpy").val().split('|');
                    for (var i = 0; i < CustomerList.length; i++) {
                        if (CustomerList[i].split('^')[2].indexOf(vals) > -1) {
                            $("#ddlcpy").val(CustomerList[i].split('^')[0] +"^"+ CustomerList[i].split('^')[1]);
                            break;
                        }
                    }
                }
                else {
                    $("#ddlcpy").val("");
                }
            }
            function getUninAllname(vals) {
                $("#txtCompanyName").val(vals.options[vals.selectedIndex].text);
            }
            //供应和客户选择
            function ddlSetText(ddlObj, flag, num) {

                var ddlVal = jQuery.trim(jQuery(ddlObj).val()).split('@')[1];
                ddlObj.a = 1;
                
                    jQuery("#" + flag + "_" + num).val(ddlVal);
                    jQuery("#Hid_KH").val(jQuery(ddlObj).val());
                
                ddlObj.a = 0;
            }
            function txtSetSel(txtObj, flag, num) {

                var txtVal = jQuery(txtObj).val();
                var ddlsel = jQuery("#" + flag + "_" + num)[0];
                if (txtVal == "") {
                    jQuery("#" + flag + "_" + num + " option").eq(0).attr("selected", true);
                }
                if (ddlsel == null || ddlsel.a != 1) {
                    if (txtVal != "") {
                        jQuery("#" + flag + "_" + num + " option:contains('" + txtVal + "')").attr("selected", true);
                    }
                    jQuery("#Hid_KH").val(jQuery("#selKH_" + num).val());
                }
            }
    </script>
</head>
<body>
    <form id="form1" runat="server">
     <div id="show"></div>
        <div class="infomain">
            <div class="mainPanel">
            <div class="title">
            <span>Pos机设置</span>
        </div>
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table1">
                   
                    <tr>
                        <td class="td" align="right">
                            Pos机编号：
                        </td>
                        <td align="left" >
                        <asp:TextBox ID="txtTerminalNo" CssClass="txt" runat="server"></asp:TextBox>
                        </td>
                        <td class="td" align="right">
                            Pos机类型：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddltype" runat="server" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Value="0">--选择类型--</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                     <td class="td" align="right">
                            费率：
                        </td>
                        <td>
                        <asp:TextBox ID="txtfeil" runat="server" Text="0.001" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox><span style="color:Red">注：费率千分之一设置为0.001</span>
                        </td>
                        <td class="td" align="right">
                            用户：
                        </td>
                        <td>
                        <uc1:SelectAirCode ID="ddlGYList" TxtWidth="100" ddlWidth="250" IsDShowName="false" InputMaxLength="10" 
                                    runat="server" DefaultOptionValue="" DefaultOptionText="------选择分销------" />
                        </td>
                        
                    </tr>
                    
                    </table>
                <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                    <tr>
                        <td height="35" align="center" class="btni">
                               <span class="btn btn-ok-s">
                                    <asp:Button ID="btnBind" runat="server" Text="绑定" OnClick="btnBind_Click" OnClientClick="return OnSubmit();">
                                    </asp:Button></span> <span class="btn btn-ok-s">
                                        <asp:Button ID="btnRet" runat="server" Text="返回" CausesValidation="false"></asp:Button></span>
                        </td>
                    </tr>
                </table>
            </div>
            </div>
        </div>
        <input type="hidden" id="hidcpy" runat="server" /> 
        <input id="Hid_KH" type="hidden" runat="server" value="" />
    </form>
</body>
</html>
