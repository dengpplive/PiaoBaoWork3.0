<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PolicyProtectEdit.aspx.cs" Inherits="Policy_PolicyProtectEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../js/area.js" type="text/javascript"></script>
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

        function Pause(obj, iMinSecond) {
            if (window.eventList == null) window.eventList = new Array();
            var ind = -1;
            for (var i = 0; i < window.eventList.length; i++) {
                if (window.eventList[i] == null) {
                    window.eventList[i] = obj;
                    ind = i;
                    break;
                }
            }
            if (ind == -1) {
                ind = window.eventList.length;
                window.eventList[ind] = obj;
            }
            setTimeout("GoOn(" + ind + ")", iMinSecond);
        }

        //js继续函数    
        function GoOn(ind) {
            var obj = window.eventList[ind];
            window.eventList[ind] = null;
            if (obj.NextStep) obj.NextStep();
            else obj();
        }
      
        function showErr(c1, c2) {

            Pause(this, 500);
            this.NextStep = function () {
                if ($("#" + c1).val() == "") {
                    $("#" + c2).html("不能为空！");
                }
                else {
                    $("#" + c2).html("<b>*</b>");
                }
            }
        }
        function showAllErr() {
            var bools = 0;
            if ($("#txtAirCode").val() == "") {
                bools++;
                $("#spanAirCode").html("不能为空！");
            }
            else {
                $("#spanAirCode").html("<b>*</b>");
            }

            if ($("#txtFromCityCode").val() == "") {
                bools++;
                $("#spanFromCityCode").html("不能为空！");
            }
            else {
                $("#spanFromCityCode").html("<b>*</b>");
            }

            if ($("#txtToCityCode").val() == "") {
                bools++;
                $("#spanToCityCode").html("不能为空！");
            }
            else {
                $("#spanToCityCode").html("<b>*</b>");
            }

            
            if (bools > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    </script>
</head>
<body>
     <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div class="title">
        <span>本地政策保护设置</span>
    </div>
    <div id="tabs" class="infomain">
        <div class="mainPanel">
            <div>
                <table width="100%" border="1" style="border-collapse: collapse;" cellpadding="0"
                    cellspacing="0" id="table_info" class="table_info">
                    <tr>
                        <td class="td">
                            承运人代码：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtAirCode" CssClass="txt" runat="server" MaxLength="3"  onkeyup="value=value.replace(/[^a-zA-Z0-9]/g,'')" onblur="showErr('txtAirCode','spanAirCode')"></asp:TextBox>
                              <span>所有航空公司请用ALL表示</span>
                            <span id="spanAirCode" style="color: Red;"><b>*</b></span>
                        </td>
                        <td class="td">
                            状态：
                        </td>
                        <td class="alignLeft">
                            <asp:RadioButtonList ID="rblstate" runat="server" CssClass="rblState" RepeatDirection="Horizontal">
                                <asp:ListItem Value="0">禁用</asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="td">
                            出发城市代码：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtFromCityCode" CssClass="txt" runat="server" MaxLength="100"   onfocus="showErr('txtCpyName','spanCpyName')" onblur="showErr('txtFromCityCode','spanFromCityCode')"></asp:TextBox>
                             <span>多个城市用/分隔</span>
                            <span id="spanFromCityCode" style="color: Red;"><b>*</b></span>
                        </td>
                        <td class="td">
                            到达城市代码：
                        </td>
                        <td class="alignLeft">
                            <asp:TextBox ID="txtToCityCode" CssClass="txt" runat="server" MaxLength="100"  onblur="showErr('txtToCityCode','spanToCityCode')"></asp:TextBox>
                            <span>多个城市用/分隔</span>
                            <span id="spanToCityCode" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
                    
                </table>
                <table width="100%" cellpadding="0" cellspacing="0" align="center" border="0">
                    <tr>
                        <td height="35" align="center" class="btni">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="lbsave" runat="server" Text="保 存" OnClick="lbsave_Click" />
                            </span>&nbsp; &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                                <asp:Button ID="btnBack" runat="server" Text="返 回" />
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
