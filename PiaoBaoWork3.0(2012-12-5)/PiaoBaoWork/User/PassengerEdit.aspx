<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PassengerEdit.aspx.cs" Inherits="User_PassengerEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>常旅客编辑</title>
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .style1{ color:RED; margin:0 3PX 0 0}
        #btnAdd_0{ padding:0; margin:0}
    </style>
    <script type="text/javascript" src="../js/js_PassengerEdit.js"> </script>
</head>
<body>
    <div id="dialog">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info" class="table_info">
                <tr>
                    <td colspan="4" class="bt">
                        基本信息
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>姓名：
                    </td>
                    <td>
                        <asp:TextBox ID="txtUser" CssClass="txt" runat="server" MaxLength="7" onkeyup="value=value.replace(/[^\u4e00-\u9fa5]/g,'')"></asp:TextBox>
                        <span id="spName"></span>
                    </td>
                    <td class="td">
                        <span class="style1">*</span>手机号码：
                    </td>
                    <td>
                        <asp:TextBox ID="txtPhone" CssClass="txt" runat="server" MaxLength="11" Columns="16"
                            onkeyup="value=value.replace(/[^0-9]/g,'')" onpaste="return false"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RfvPhone" runat="server" ControlToValidate="txtPhone"
                            ErrorMessage="手机必填" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                        <span id="spPhone"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>证件类型：
                    </td>
                    <td>
                        <select name="ddlCardType" id="ddlCardType" class="txt" runat="server">
                        </select>
                    </td>
                    <td class="td">
                        <span class="style1">*</span><label id="cartNum">证件号码：</label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCardNum" CssClass="txt" runat="server" MaxLength="18"></asp:TextBox>
                        <input type="text" id="txtDate" style="width: 130px;" readonly="true" class="hide"
                            runat="server" onfocus="WdatePicker({isShowClear:false,maxDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" /><span
                                id="spCard"></span>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        <span class="style1">*</span>性别：
                    </td>
                    <td>
                        <label for="sex_0">
                            <input type="radio" name="sex" id="sex_0" value="0" checked="checked" />男</label>
                        <label for="sex_1">
                            <input type="radio" name="sex" id="sex_1" value="0" />女</label>
                    </td>
                    <td class="td">
                        旅客类型：
                    </td>
                    <td>
                        <asp:Literal ID="literPasType" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        出生日期：
                    </td>
                    <td>
                        <input type="text" readonly="readonly" id="txtBirthday" runat="server" class="Wdate inputtxtdat"
                            style="width: 205px" onfocus="WdatePicker({isShowClear:false,maxDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                    </td>
                    <td class="td">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <table id="tab_carry">
                            <tr id="trcarry_0">
                                <td class="td">
                                    航空公司：
                                </td>
                                <td>
                                    <input name="txtCarryCode" type="text" maxlength="3" id="txtCarryCode_0" autocomplete="off"
                                        onpropertychange="txtSetSel(this,'ddlCarryCode',0)" style="width: 30px; margin-bottom: 2px;
                                        margin-top: -2px;" />
                                    <select name="ddlCarryCode" id="ddlCarryCode_0" runat="server" onchange="ddlSetText(this,'txtCarryCode',0)"
                                        style="width: 118px;">
                                        <option value="">--航空公司--</option>
                                    </select>
                                </td>
                                <td class="td">
                                    卡号：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtAirNo_0" CssClass="txt" runat="server" MaxLength="20"></asp:TextBox>
                                </td>
                                <td>
                                   
                                        <span class="btn btn-ok-s" >
                                            <input type="button" style=" background-repeat:repeat-x; padding:0; margin:0" value="添 加" id="btnAdd_0" onclick="addGroup(event,'carry')" /></span>
                                    
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="td">
                        备注：
                    </td>
                    <td colspan="3" rowspan="2">
                        <asp:TextBox ID="txtRemark" CssClass="txt" runat="server" Columns="16" Height="37px"
                            TextMode="MultiLine" Width="496px" MaxLength="150"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                <tr>
                    <td height="35" align="center" class="btni">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnSave" runat="server" Text="保  存" OnClientClick="return SaveData();" />
                        </span>&nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                            <input type="button" value="返 回 " onclick="location.href='PassengerList.aspx?currentuserid=<%=this.mUser.id.ToString() %>'" />
                        </span>
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
                    1、姓名只能填写中文姓名。
                    <br />
                    2、温馨提示内容进行修改，常旅客不仅只是中文一种格式，还有英文如 kkkk/lll这种格式。
                </td>
            </tr>
        </table>
    </div>
    <%--航空公司和卡号--%>
    <input id="Hid_CpyandNo" type="hidden" runat="server" />
    <%--0 添加 1编辑--%>
    <input id="Hid_IsEdit" type="hidden" runat="server" value="0" />
    <%--编辑数据--%>
    <input id="Hid_Flyer" type="hidden" runat="server" value="" />
    <%--编辑数据Id--%>
    <input id="Hid_id" type="hidden" runat="server" />
    <%--证件数据--%>
    <input id="Hid_CardData" type="hidden" runat="server" />
    </form>
</body>
</html>
