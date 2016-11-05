<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OpenTicket.aspx.cs" Inherits="Bill_OpenTicket" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Open票查询</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/Statements.css" rel="stylesheet" />
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
    </style>
    <script type="text/javascript">
    <!--
        var $J=jQuery.noConflict(false);
        //只用来显示提示信息对话框
        function showMsgDg(t) {
            $J("#show").html(t);
            $J("#show").dialog({
                title: '提示',
                bgiframe: true,
                height: 180,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }
        //函数 暂停毫秒 回调函数
        function Sleep(obj,iMinSecond,CallFn) {
            if(obj.NextStep==null) {
                obj.NextStep=function () {
                    if(CallFn!=null) {
                        CallFn();
                    }
                }
            }
            if(obj.GoOn==null) {
                obj.GoOn=function () {
                    if(obj.NextStep) obj.NextStep();
                    else {
                        if(obj) {
                            obj();
                        }
                    }
                }
            }
            setTimeout(function () { obj.GoOn() },iMinSecond);
        }
        function StartScan() {
            var trCol=$J("#dgvList tr:gt(0)");
            //移除空票号
            var remove=[];
            trCol.each(function (index,tr) {
                if($J(tr).find("td:eq(0)").text()=="") {
                    remove.push(tr);
                }
            });
            for(var i=0;i<remove.length;i++) {
                $J(trCol).remove(remove[i]);
            }
            var num=0;
            if(trCol.length>0) {
                var Office=$J("#Hid_Office").val();
                var CpyNo=$J("#Hid_CpyNo").val();
                var ddlSel=$J("#ddlSel").val();
                var url="../AJAX/GetHandler.ashx";

                var TicketNumber=$J.trim($J(trCol[num]).find("td:eq(0)").text());
                TicketNumber=TicketNumber.replace("'","");
                var param=
                {
                    OpName: escape("OpenTicket"),
                    TicketNumber: escape(TicketNumber),
                    SelScan: escape(ddlSel),
                    CpyNo: escape(CpyNo),
                    Office: escape(Office),
                    InsType: escape("3"),
                    num: Math.random(),
                    currentuserid: $J("#currentuserid").val()
                };
                function fn_Call(data) {
                    try {
                        var strArr=unescape(data).split('#######');
                        if(strArr.length==2&&strArr[1].indexOf("|")!= -1) {
                            //第一个为票号 第二个为状态
                            var strTK=strArr[1].split('|');
                            if(strTK.length==2) {
                                //设置状态
                                $J(trCol[num]).find("td:eq(1)").text(strTK[1]);
                                $J(trCol[num]).find("td:eq(0)").text(strTK[0]);
                                // $J(trCol[num]).find("td:eq(0)").attr("style","vnd.ms-excel.numberformat:@");
                                num++;
                                if(num>=trCol.length) {
                                    //完成
                                    alert('扫描完成');
                                    $J("#btnStartScan").attr("disabled",false);
                                    $J("#span_2").show();

                                    $J("#Hid_ImportData").val(encodeURIComponent($J("#divCon").html()));
                                } else {
                                    //暂停半秒
                                    Sleep(this,500,function () {
                                        TicketNumber=$J.trim($J(trCol[num]).find("td:eq(0)").text());
                                        TicketNumber=TicketNumber.replace("'","");
                                        param.TicketNumber=TicketNumber;
                                        if(param.TicketNumber!="") {
                                            //后来请求
                                            $J.post(url,param,fn_Call,"text");
                                        }
                                    });
                                }
                            }
                        }
                    } catch(e) {
                        alert(e.message);
                    }
                }
                $J("#btnStartScan").attr("disabled",true);
                //第一次请求
                $J.post(url,param,fn_Call,"text");
            }
            return false;
        }
    -->    
    </script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <div id="show">
    </div>
    <form id="form1" runat="server">
    <div id="tabs">
        <div class="title">
            <span>open票导入</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                Excel路径：
                            </th>
                            <td>
                                <asp:FileUpload ID="fileUpload" runat="server" />
                            </td>
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnReadExcel" runat="server" Text="读取选择的Excel" OnClick="btnReadExcel_Click">
                                    </asp:Button>
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                出票时间：
                            </th>
                            <td>
                                <input type="text" id="txtCPStartTime" style="width: 110px;" readonly="true" class="inputtxtdat"
                                    runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />--
                                <input type="text" id="txtCPEndTime" style="width: 110px;" readonly="true" class="inputtxtdat"
                                    runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                            </td>
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnReadSysTicketNumber" runat="server" Text="读取系统票号" OnClick="btnReadSysTicketNumber_Click">
                                    </asp:Button>
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:DropDownList ID="ddlSel" runat="server">
                                    <asp:ListItem Text="全部" Value="0" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="只显示有效票号" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="只显示无效票号" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                                <span class="btn btn-ok-s" runat="server" id="span_1">
                                    <asp:Button ID="btnStartScan" runat="server" Text="开始扫描" OnClientClick="return StartScan();">
                                    </asp:Button>
                                </span><span class="btn btn-ok-s hide" runat="server" id="span_2">
                                    <asp:Button ID="btnImportData" runat="server" Text="导出到指定路径" OnClick="btnImportData_Click">
                                    </asp:Button>
                                </span>
                            </td>
                        </tr>
                    </table>
                    <div id="divCon">
                        <asp:Repeater ID="Repeater" runat="server">
                            <HeaderTemplate>
                                <table cellspacing="0" rules="all" border="1" id="dgvList" style="width: 100%; border-collapse: collapse;
                                    text-align: center;">
                                    <tr>
                                        <th scope="col" style="width: 500px;">
                                            票号
                                        </th>
                                        <th scope="col">
                                            状态
                                        </th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td format="0">
                                        <%# Eval("票号") %>
                                    </td>
                                    <td>
                                        <%# Eval("状态") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <table width="100%" border="0" class="sugges">
        <tr>
            <td class="sugtitle">
                温馨提示：<br />
            </td>
        </tr>
        <tr>
            <td class="sugcontent" style="color: Red;">
                * 如果正在获取票号信息请不要关闭此窗口，等待结果显示后即可。
            </td>
        </tr>
        <tr>
            <td class="sugcontent" style="color: Red;">
                请使用IE浏览器导出报表。
            </td>
        </tr>
        <tr>
            <td class="sugcontent" style="color: Red;">
                请按规定格式导入报表。<asp:LinkButton ID="lbtnDownloadExcel" runat="server" OnClick="lbtnDownloadExcel_Click">点击下载导入报表格式</asp:LinkButton>
            </td>
        </tr>
    </table>
    <input id="Hid_Office" type="hidden" runat="server" />
    <input id="Hid_CpyNo" type="hidden" runat="server" />
    <input id="Hid_TicketNumber" type="hidden" runat="server" />
    <input id="Hid_ImportData" type="hidden" runat="server" />
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    </form>
</body>
</html>
