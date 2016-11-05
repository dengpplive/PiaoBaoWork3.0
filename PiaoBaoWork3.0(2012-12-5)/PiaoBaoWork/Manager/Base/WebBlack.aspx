<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebBlack.aspx.cs" Inherits="Base_WebBlack" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>供应商黑屏查看</title>
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">
        function showdialog(t) {
            $("#divdg").html(t);
            $("#divdg").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                }
            });
        }
        function SetSel(obj) {
            var selValue=$(obj).val();
            $("#Hid_sup").val(selValue);
            if($.trim(selValue)!="") {
                var OfficeGroup=selValue.split('#')[1].split('$@@@@$')[0];
                var heiping=selValue.split('#')[1].split('$@@@@$')[1];
                var arr=heiping.split('|');
                var IP=arr[2]+":"+arr[3];
                $("#Hid_CurrOffice").val(OfficeGroup);
                $("#useOffice").html('<br />Office：<font style="color:red;">'+OfficeGroup+'</font>&nbsp;&nbsp;IP:<font style="color:red;">'+IP+'</font>');
            } else {
                $("#useOffice").html('');
            }
        }
        $(document).keydown(function (event) {
            if(event.keyCode==13) {
                secondGo();
            }
        });
        function secondGo() {
            var obj=$("#btnGet");
            var userValue=$.trim($("#UserList").val());
            if(userValue=="") {
                showdialog("请选择供应商！");
                return;
            }
            var arr=userValue.split('#');
            if(arr!=null&&arr.length==2) {
                var unicode=arr[0];
                //var office = arr[1].split('$@@@@$')[0];
                var orther=arr[1].split('$@@@@$')[1];
                var ins=$("#txtSendIns").val();
                var office=$("#txtOffice").val();
                if($.trim(ins)!="") {
                    obj.attr("disabled",true);
                    $.post("../../AJAX/GetInsInfo.aspx?currentuserid=<%=this.mUser.id.ToString() %>",
                    {
                        SendIns: ins,
                        Office: office,
                        CpyCode: unicode,
                        Other: orther,
                        nm: Math.random()
                    },
                    function (data) {
                        obj.attr("disabled",false);
                        data=unescape(data);
                        if(data.indexOf('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">')!= -1) {
                            data="用户登录已失效,请登录后再试！";
                            top.location.href="../../Login.aspx";
                        }
                        data=data+"\r\n";
                        $("#txtRecvData").val($("#txtRecvData").val()+data);
                    },"text");
                } else {
                    showdialog("输入指令不能为空！");
                }
            }
        }
        function QueryFlow() {
            var OfficeText=$.trim($("#Hid_CurrOffice").val());
            var sDate=$.trim($("#txtStartDate").val());
            var eDate=$.trim($("#txtEndDate").val());
            if(sDate==''||eDate=='') {
                sDate='';
                eDate='';
            }
            var OfficeArr=OfficeText.split('^');
            if($.trim(OfficeArr[0])=="") {
                $("#divFlow").html("<span style='color:red;'>请选择落地运营商</span>");
                return;
            }
            var url="../../AJAX/GetHandler.ashx";
            var i=0;
            var param=
            {
                OpName: escape("GetFlow"),
                StartDate: escape(sDate),
                EndDate: escape(eDate),
                Office: escape(OfficeArr[i]),
                nm: Math.random(),
                currentuserid: '<%= this.mUser.id.ToString()%>'
            };
            $("#divFlow").html("");
            $("#btnQueryFlow").attr("disabled",true);
            function d(data) {
                i++;
                try {
                    data=unescape(data);
                    if(data.indexOf('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">')!= -1) {
                        data="用户登录已失效,请登录后再试！";
                        top.location.href="../../Login.aspx";
                    } else {

                        if(data!="") {
                            var strArr=data.split('#######');
                            //#######
                            if(strArr!=null&&strArr.length==2) {
                                data=strArr[1];
                                $("#divFlow").html($("#divFlow").html()+"<br /><br /><span style='color:red;font-size:25px;'>"+data.replace('|',":")+"条</span>");
                            }
                        }
                    }
                    if(i<=(OfficeArr.length-1)) {
                        param.Office=escape(OfficeArr[i]);
                        $("#btnQueryFlow").attr("disabled",true);
                        $.post(url,param,d,"text");
                    }
                } catch(e) {

                } finally {
                    $("#btnQueryFlow").attr("disabled",false);
                }
            }
            $.post(url,param,d,"text");
        }
    </script>
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            color: #00FF00;
        }
        .style2
        {
            color: #CC0000;
        }
        .title
        {
            font-size: 14px;
            font-weight: bolder;
            height: 100%;
            line-height: 20px;
            padding: 3px 0;
            color: #0092FF;
            text-align: left;
            background-color: #EFF4F8;
            border: 1px solid #D4D4D4;
        }
        .title span
        {
            margin-left: 15px;
        }
        .style3
        {
            color: #FF0000;
        }
        
        .AP_div
        {
            /*右侧内容区*/
            position: absolute;
            display: block; /*background-color: Red;*/
            border: solid 1px green;
            z-index: 200;
            left: 650px;
            top: 440px;
            width: 550px;
            height: 130px;
        }
    </style>
</head>
<body>
    <div id="divdg">
    </div>
    <form id="form1" runat="server">
    <div class="title">
        <ul style="list-style-type: none; margin: 0 0; padding: 0 0; position: relative;">
            <li>供应商黑屏查看</li>
        </ul>
    </div>
    <div style="text-align: center;">
        <table cellspacing="5" cellpadding="0" border="0" width="100%">
            <tr>
                <th valign="top" style="width: 120px;">
                    接收数据:
                </th>
                <td colspan="2" align="left">
                    <textarea id="txtRecvData" rows="25" cols="300" style="color: green; background-color: black;
                        width: 100%!important; min-width: 600px!important; height: auto; min-height: 400px;"
                        ondblclick="this.value=''"></textarea>
                </td>
            </tr>
            <tr>
                <th valign="top" style="width: 120px;">
                </th>
                <td colspan="2" align="left" class="style3">
                    注:双击清屏
                </td>
            </tr>
            <tr>
                <th style="width: 120px;">
                    运营商:
                </th>
                <td align="left">
                    <asp:Literal ID="UserSeelct" runat="server"></asp:Literal>&nbsp;&nbsp;&nbsp;&nbsp;<span
                        id="useOffice"></span>
                    <div class="AP_div">
                        <table>
                            <tr>
                                <td>
                                    日期：
                                </td>
                                <td>
                                    <input type="text" id="txtStartDate" style="width: 130px;" readonly="true" runat="server"
                                        class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />至
                                    <input type="text" id="txtEndDate" style="width: 130px;" readonly="true" runat="server"
                                        class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                                </td>
                                <td>
                                    <span class="btn btn-ok-s">
                                        <input id="btnQueryFlow" type="button" value="查询流量" onclick="QueryFlow()" /></span>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <div id="divFlow">
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <th style="width: 120px;">
                    指令:
                </th>
                <td align="left">
                    <input type="text" style="width: 250px;" id="txtSendIns" value="" />
                    <span class="style1">注:多个指令用&quot;|&quot; 隔开</span>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <th style="width: 120px;">
                    Office:
                </th>
                <td align="left">
                    <input style="width: 250px;" type="text" id="txtOffice" value="" /><span class="style1">
                        注: Office为空循环该供应商所有Office</span>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="2" align="left">
                    <span class="btn btn-ok-s">
                        <input type="button" id="btnGet" value="发送指令" onclick="secondGo()" /></span>
                    <span class="style2">注:如果选择的供应商开启了新PID 发送指令就会用新的PID否则用票宝管家</span>
                </td>
            </tr>
        </table>
    </div>
    <input type="hidden" id="Hid_sup" value="" runat="server" />
    <input type="hidden" id="Hid_CurrOffice" value="" runat="server" />
    </form>
</body>
</html>
