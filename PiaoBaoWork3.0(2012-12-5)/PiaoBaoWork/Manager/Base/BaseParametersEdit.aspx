<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseParametersEdit.aspx.cs"
    Inherits="Manager_Base_BaseParametersEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加基数参数</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">
<!--
        function showdialog3(t,p) {
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
                        $("#dd").dialog("close");
                        if (p == null) {
                            location.href = "BaseParametersEdit.aspx?currentuserid=" + $("#currentuserid").val();
                        }
                    }
                }
            });
        }
        //日期大小比较 大于返回true 小于false
        function CompareDate(sdate,edate) {
            var strDate1=sdate.split('-');
            var date1;
            if(strDate1.length==3) {
                date1=new Date(strDate1[0],strDate1[1],strDate1[2]);
            }
            var strDate2=edate.split('-');
            var date2;
            if(strDate1.length==3) {
                date2=new Date(strDate2[0],strDate2[1],strDate2[2]);
            }
            return (date1>date2);
        }
        function validate() {
            var reflag=false;
            var msg="";
            var d1=CompareDate($("#txtStartDate").val(),$("#txtEndDate").val());
            if(d1) {
                msg="有效起始日期不能大于有效截止日期";
            }
            if(msg!="") {
                showdialog3(msg,'show');
            } else {
                reflag=true;
            }
            return reflag;
        }
//-->

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
    </style>
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <div id="dd">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="title">
        <span>添加基数参数</span>
    </div>
    <div>
        <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
            border="1" style="border-collapse: collapse;">
            <tr>
                <th>
                    公司编号:
                </th>
                <td align="left">
                    <asp:TextBox ID="txtCompanyNo" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <th>
                </th>
                <td>
                </td>
                <th>
                </th>
                <td>
                </td>
            </tr>
            <tr>
                <th>
                    参数名:
                </th>
                <td align="left">
                    <asp:TextBox ID="txtParamName" runat="server" MaxLength="100"></asp:TextBox>
                </td>
                <th>
                    参数值:
                </th>
                <td align="left">
                    <asp:TextBox ID="txtParamValue" MaxLength="500" runat="server" TextMode="MultiLine" Height="64px"></asp:TextBox>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <th>
                    参数描述:
                </th>
                <td align="left">
                    <asp:TextBox ID="txtParamDescript" runat="server" TextMode="MultiLine" Height="74px"
                        Width="196px" MaxLength="500"></asp:TextBox>
                </td>
                <th>
                    参数备注:
                </th>
                <td align="left">
                    <asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Height="84px" Width="189px"
                        MaxLength="100"></asp:TextBox>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <th>
                    有效起始日期：
                </th>
                <td align="left">
                    <input type="text" id="txtStartDate" style="width: 130px;" readonly="true" runat="server"
                        class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                </td>
                <th>
                    有效截止日期：
                </th>
                <td align="left">
                    <input type="text" id="txtEndDate" style="width: 130px;" readonly="true" runat="server"
                        class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td colspan="4">
                    <span class="btn btn-ok-s">
                        <asp:Button ID="btnSave" runat="server" Text=" 保 存 " OnClick="btnSave_Click" OnClientClick="return validate(); " /></span>&nbsp;
                    &nbsp;&nbsp;&nbsp;<span class="btn btn-ok-s">
                        <asp:Button ID="btnGo" runat="server" Text=" 返 回 "/>
                    </span>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
