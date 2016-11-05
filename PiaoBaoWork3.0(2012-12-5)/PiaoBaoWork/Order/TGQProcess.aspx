<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TGQProcess.aspx.cs" Inherits="Order_TGQProcess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>退改签审核处理</title>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../css/detail.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">

        var $J=jQuery.noConflict(false);

        $J(function () {
            // Tabs
            $J('#tabs').tabs();
        });

        function showdialog(t) {
            $J("#showOne").html(t);
            $J("#showOne").dialog({
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
                        $J(this).dialog('close');
                    }
                }
            });
        }

        function showdialogOne(t,One) {
            $J("#showOne").html(t);
            $J("#showOne").dialog({
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
                        location.href=One;
                    }
                }
            });
        }

        function showLoading() {
            var divValue="<div id=\"loading\" style=\"display: block;\">正在处理请稍等...<br /><img src=\"../img/loading.gif\">";
            divValue+="<br /></div>";

            $J("#showOne").html(divValue);
            $J("#showOne").dialog({
                title: '温馨提示',
                bgiframe: true,
                width: 300,
                height: 200,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                }
            });
        }

        function opOrder(msg,type) {

            if(confirm(msg)) {
                if(type==1) {
                    var UnApp=$J("#txtTGQRefusalReason").val();
                    if(UnApp=="") {
                        showdialog("请输入您拒绝理由！");
                        return false;
                    }
                }
                showLoading(); //加载遮罩层
            } else {
                return false;
            }
            return true;
        }





        //---------------------------------------------------------------- 
        //padRight(10,'0')
        String.prototype.padRight=function (length,char) {
            var d=this;
            var len=d.length;
            while(len<length) {
                d=d+char;
                len++;
            }
            return d;
        }
        //数字 保留小数位数
        function ShowPoint(number,PointNum) {
            var tempNum=number.toString();
            if(tempNum.indexOf(".")!= -1)//存在小数点
            {
                var strArr=tempNum.split('.');
                if(strArr[1].length>PointNum) {
                    strArr[1]=strArr[1].substring(0,PointNum);
                } else {
                    strArr[1].padRight(PointNum,'0');
                }
                tempNum=strArr[0]+"."+strArr[1];
            }
            return parseFloat(tempNum,10);
        }
        function JisuanFee(val,fg) {
            var number=val.toString().replace("%","");
            if(number!="") {
                number=parseInt(number,10)/100;
                $J("#table2 tr[id*='tr_']").each(function (index,tr) {
                    var jtr=$J(tr);
                    if(fg=="1") {//手输

                        $J("#td3").hide();
                        jtr.find("input[type='text'][id*='txtTGQHandlingFee']").attr("readonly",false);

                    } else if(fg=="0")//下拉
                    {
                        $J("#td3").show();

                        if(val!= -1) {
                            var PMFee=parseFloat(jtr.find("input[type='hidden'][id*='Hid_PMFee']").val(),10);
                            var TGQHandlingFee=ShowPoint(PMFee*number,2);
                            jtr.find("input[type='text'][id*='txtTGQHandlingFee']").val(TGQHandlingFee);
                        }

                        jtr.find("input[type='text'][id*='txtTGQHandlingFee']").attr("readonly",true);
                    }
                });
            }
        }
        //续费输入范围限制
        function TGQHandlingFee() {
            var value=$J.trim($J(this).val());

            if($J.isNaN(value)) {
                alert("手续费输入错误，只能输入数字");
                $J(this).val("0");
            } else {
                var inputTGQHandlingFee=parseFloat(value);
                var IsOk=true;
                $J("#table2 tr[id*='tr_']").each(function (index,tr) {
                    var jtr=$J(tr);
                    //舱位价
                    var PMFee=parseFloat(jtr.find("input[type='hidden'][id*='Hid_PMFee']").val(),10);
                    var ABFee=parseFloat(jtr.find("input[type='hidden'][id*='Hid_ABFee']").val(),10);
                    var FuelFee=parseFloat(jtr.find("input[type='hidden'][id*='Hid_FuelFee']").val(),10);

                    if(inputTGQHandlingFee>(PMFee+ABFee+FuelFee)||inputTGQHandlingFee<0) {
                        IsOk=false;
                        return false;
                    }

                });
                if(!IsOk) {
                    alert("输入手续费超出范围！");
                    $J(this).val("0");
                }
            }
        }
        $J(function () {
            //注册手续费输入范围限制
            $J("input[id*='txtTGQHandlingFee']").blur(TGQHandlingFee);
            var val=$J("#hid_TicketStatus").val();
            if(val=="5") {
                $J("input[id*='txtTGQHandlingFee']").hide();
            } else {
                $J("input[id*='txtTGQHandlingFee']").show();
            }
        })


        function yzGQPrice(objId) {
            var value=$J.trim($J("#"+objId).val());
            if($J.isNaN(value)) {
                alert("改签补差金额输入错误，只能输入数字");
                $J(this).val("0");
            } else {
                if(value<0) {
                    alert("改签补差金额输入错误，只能输入数字");
                    $J(this).val("0");
                }
            }
        }

        function GQPrice(objId,type,msgType) {
            if(type==1) {
                $J("#"+objId).val("");
            } else if(type==2) {
                var val=$J("#"+objId).val();
                if(val=="")
                    $J("#"+objId).val("0");
            }
            else if(type==3) {

                if(msgType==1) {
                    msg="航空公司/平台退款金额输入错误，只能输入数字";
                } else if(msgType==3) {
                    msg="改签补差金额输入错误，只能输入数字";
                } else {
                    msg="金额输入错误，只能输入数字";
                }

                var value=$J.trim($J("#"+objId).val());
                if($J.isNaN(value)) {
                    alert(msg);
                    $J(this).val("0");
                } else {
                    if(value<0) {
                        alert(msg);
                        $J(this).val("0");
                    }
                }
            }
        }

    </script>
    <style type="text/css">
        .table_info th, .table_info td
        {
            border-color: #CCCCCC;
            border-style: solid;
            border-width: 1px;
            color: #606060;
        }
        table
        {
            border-collapse: collapse;
        }
        .tdNew
        {
            font-weight: bold;
            background: url("../img/title.png") repeat-x scroll 0 0 transparent;
            text-align: right;
        }
        .td1
        {
            color: Red;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <div id="tabs-1" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
            <table width="100%" cellspacing="0" cellpadding="0" border="0" align="center" style="border: 1px solid #E6E6E6;">
                <tbody>
                    <tr>
                        <td class="mainl">
                        </td>
                        <td>
                            <table width="100%" cellspacing="0" cellpadding="5" border="0" align="center" style="padding: 5px;"
                                class="detail">
                                <tbody>
                                    <tr>
                                        <td>
                                            <div class="ebill-bg-top">
                                                <h1>
                                                    <asp:Label ID="lblShow" runat="server" Text=""></asp:Label></h1>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="order1">
                                        <td>
                                            <div>
                                                <strong style="color: #0C88BE">订单信息</strong>
                                            </div>
                                            <table id="table9" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                                border="0">
                                                <tr>
                                                    <td class="tdNew" style="width: 12%">
                                                        订单编号:
                                                    </td>
                                                    <td style="width: 26%" class=" td1">
                                                        <asp:Label ID="lblOrderId" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="tdNew" style="width: 10%">
                                                        PNR:
                                                    </td>
                                                    <td style="width: 20%; font-weight: bold">
                                                        <asp:Label ID="lblPNR" runat="server" Text=""></asp:Label>
                                                        <asp:Label ID="lblShowPNR" runat="server" Text="" Visible="false"></asp:Label>
                                                    </td>
                                                    <td class="tdNew" style="width: 12%">
                                                        订单金额:
                                                    </td>
                                                    <td style="width: 20%" class=" td1">
                                                        <asp:Label ID="lblPayMoney" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNew">
                                                        订单来源:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblOrderSourceType" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="tdNew">
                                                        政策来源:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblPolicySource" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="tdNew">
                                                        政策点数:
                                                    </td>
                                                    <td class="td1" align="center" valign="middle">
                                                        <asp:Label ID="lblPolicyPoint" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNew">
                                                        支付方式:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblPayWay" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="tdNew">
                                                        内部交易流水号:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblInPayNo" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="tdNew">
                                                        支付流水号:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblPayNo" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNew">
                                                        订单状态:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblOrderStatusCode" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="tdNew">
                                                        支付状态:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblPayStatus" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td class="tdNew">
                                                    </td>
                                                    <td style="text-align: left">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNew">
                                                        废、改处理时间:
                                                    </td>
                                                    <td colspan="5" style="text-align: left">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNew" style="background-image: url('../img/title1.png');">
                                                        客规参考:
                                                    </td>
                                                    <td colspan="5" style="text-align: left;">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNew" style="background-image: url('../img/title1.png');">
                                                        政策备注:
                                                    </td>
                                                    <td colspan="5" style="text-align: left">
                                                        <asp:Label ID="lblPolicyRemark" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trRepSkyWayOld" visible="false">
                                        <td>
                                            <div>
                                                <strong style="color: Red">原行程信息</strong>
                                            </div>
                                            <table id="table1" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                                width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>
                                                            起飞城市
                                                        </th>
                                                        <th>
                                                            到达城市
                                                        </th>
                                                        <th>
                                                            起飞日期
                                                        </th>
                                                        <th>
                                                            到达日期
                                                        </th>
                                                        <th>
                                                            承运人
                                                        </th>
                                                        <th>
                                                            航班号
                                                        </th>
                                                        <th>
                                                            舱位
                                                        </th>
                                                        <th>
                                                            折扣
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <asp:Repeater ID="RepSkyWayOld" runat="server">
                                                    <ItemTemplate>
                                                        <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                            <td style="width: 10%; text-align: center;">
                                                                <asp:Label ID="lblSkyWayId" runat="server" Text='<%# Eval("Id")%>' Visible="false"></asp:Label>
                                                                <%# Eval("FromCityName") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("ToCityName") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("FromDate") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("ToDate") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("CarryCode")%>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("FlightCode") %>
                                                            </td>
                                                            <td style="width: 5%; text-align: center;">
                                                                <%# Eval("Space") %>
                                                            </td>
                                                            <td style="width: 5%; text-align: center; color: Red;">
                                                                <%# Eval("Discount") %>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div>
                                                <strong style="color: #0C88BE">
                                                    <asp:Label runat="server" ID="labHCName" Text="行程信息"></asp:Label></strong>
                                            </div>
                                            <table id="table4" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                                width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>
                                                            起飞城市
                                                        </th>
                                                        <th>
                                                            到达城市
                                                        </th>
                                                        <th>
                                                            起飞日期
                                                        </th>
                                                        <th>
                                                            到达日期
                                                        </th>
                                                        <th>
                                                            承运人
                                                        </th>
                                                        <th>
                                                            航班号
                                                        </th>
                                                        <th>
                                                            舱位
                                                        </th>
                                                        <th>
                                                            折扣
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <asp:Repeater ID="RepSkyWay" runat="server">
                                                    <ItemTemplate>
                                                        <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                            <td style="width: 10%; text-align: center;">
                                                                <asp:Label ID="lblSkyWayId" runat="server" Text='<%# Eval("Id")%>' Visible="false"></asp:Label>
                                                                <%# Eval("FromCityName") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("ToCityName") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("FromDate") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("ToDate") %>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("CarryCode")%>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%# Eval("FlightCode") %>
                                                            </td>
                                                            <td style="width: 5%; text-align: center;">
                                                                <asp:Label ID="lblSpace" runat="server" Text='<%# Eval("Space") %>'></asp:Label>
                                                            </td>
                                                            <td style="width: 5%; text-align: center; color: Red;">
                                                                <asp:TextBox ID="txtDiscount" runat="server" Text='<%# Eval("Discount") %>' Visible="false"
                                                                    Width="60px"></asp:TextBox>
                                                                <asp:Label ID="lblDiscount" runat="server" Text='<%# Eval("Discount") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">
                                            <div style="text-align: left; margin: 0; padding: 0;">
                                                <table align="left">
                                                    <tr>
                                                        <td>
                                                            <strong style="color: #0C88BE">乘客信息</strong>
                                                        </td>
                                                        <td id="td4" runat="server" visible="false">
                                                            &nbsp;&nbsp;&nbsp;&nbsp; 修改手续费
                                                        </td>
                                                        <td id="td2" runat="server" visible="false">
                                                            <asp:RadioButtonList ID="rblList" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="1" onclick="JisuanFee(1,1)">手输</asp:ListItem>
                                                                <asp:ListItem Value="-1" Selected="True" onclick="JisuanFee(-1,0)">选择</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                        <td id="td3" runat="server" visible="false">
                                                            <asp:DropDownList ID="selSXF" runat="server" onchange="JisuanFee(this.value,0)">
                                                                <asp:ListItem Value="0">0%</asp:ListItem>
                                                                <asp:ListItem Value="5">5%</asp:ListItem>
                                                                <asp:ListItem Value="10">10%</asp:ListItem>
                                                                <asp:ListItem Value="15">15%</asp:ListItem>
                                                                <asp:ListItem Value="20">20%</asp:ListItem>
                                                                <asp:ListItem Value="25">25%</asp:ListItem>
                                                                <asp:ListItem Value="30">30%</asp:ListItem>
                                                                <asp:ListItem Value="35">35%</asp:ListItem>
                                                                <asp:ListItem Value="40">40%</asp:ListItem>
                                                                <asp:ListItem Value="45">45%</asp:ListItem>
                                                                <asp:ListItem Value="50">50%</asp:ListItem>
                                                                <asp:ListItem Value="55">55%</asp:ListItem>
                                                                <asp:ListItem Value="60">60%</asp:ListItem>
                                                                <asp:ListItem Value="65">65%</asp:ListItem>
                                                                <asp:ListItem Value="70">70%</asp:ListItem>
                                                                <asp:ListItem Value="75">75%</asp:ListItem>
                                                                <asp:ListItem Value="80">80%</asp:ListItem>
                                                                <asp:ListItem Value="85">85%</asp:ListItem>
                                                                <asp:ListItem Value="90">90%</asp:ListItem>
                                                                <asp:ListItem Value="95">95%</asp:ListItem>
                                                                <asp:ListItem Value="100">100%</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table id="table2" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                                border="0">
                                                <thead>
                                                    <tr>
                                                        <th>
                                                            乘客姓名
                                                        </th>
                                                        <th>
                                                            乘客类型
                                                        </th>
                                                        <th>
                                                            证件类型
                                                        </th>
                                                        <th>
                                                            证件号
                                                        </th>
                                                        <th>
                                                            票号
                                                        </th>
                                                        <th>
                                                            机票状态
                                                        </th>
                                                        <th>
                                                            舱位价
                                                        </th>
                                                        <th>
                                                            机建/燃油
                                                        </th>
                                                        <th>
                                                            退废手续费
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <asp:Repeater ID="RepPassenger" runat="server">
                                                    <ItemTemplate>
                                                        <tr style="line-height: 30px; width: 100%;" name="trPassenger" id='tr_<%# Eval("Id").ToString() %>'>
                                                            <td style="width: 6%; text-align: center;">
                                                                <%#Eval("PassengerName")%>
                                                            </td>
                                                            <td style="width: 4%; text-align: center;">
                                                                <%#  GetDictionaryName("6", Eval("PassengerType").ToString())%>
                                                            </td>
                                                            <td style="width: 6%; text-align: center;">
                                                                <%#  GetDictionaryName("7", Eval("CType").ToString())%>
                                                            </td>
                                                            <td style="width: 18%; text-align: center;">
                                                                <%#Eval("Cid")%>
                                                            </td>
                                                            <td style="width: 18%; text-align: center;">
                                                                <%#Eval("TicketNumber")%>
                                                            </td>
                                                            <td style="width: 4%;">
                                                                <%#  GetDictionaryName("9", Eval("TicketStatus").ToString())%>
                                                            </td>
                                                            <td style="width: 6%; color: Red;">
                                                                <%#Eval("PMFee")%>
                                                            </td>
                                                            <td style="width: 6%;">
                                                                <%#Eval("ABFee")%>
                                                                /
                                                                <%#Eval("FuelFee")%>
                                                            </td>
                                                            <td style="width: 6%; color: Red;">
                                                                <input type="text" size="8" id="txtTGQHandlingFee" value='<%# ShowText(0,Eval("TGQHandlingFee"))%>'
                                                                    runat="server" />
                                                                <input type="hidden" id="Hid_Id" value='<%#Eval("id")%>' runat="server" />
                                                                <input type="hidden" id="Hid_PMFee<%#Eval("id")%>" value='<%#Eval("PMFee")%>' />
                                                                <input type="hidden" id="Hid_ABFee<%#Eval("id")%>" value='<%#Eval("ABFee")%>' />
                                                                <input type="hidden" id="Hid_FuelFee<%#Eval("id")%>" value='<%#Eval("FuelFee")%>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="100%">
                                            <table id="table3" class="table_info" border="0" cellspacing="0" cellpadding="0"
                                                width="100%">
                                                <tr>
                                                    <td style="text-align: right;">
                                                        <asp:Label ID="lblYDRemark" runat="server" Text="备注说明：" Width="150px"></asp:Label>
                                                    </td>
                                                    <td style="width: 98%; text-align: left;">
                                                        <asp:TextBox ID="txtYDRemark" Enabled="false" runat="server" Height="60px" TextMode="MultiLine"
                                                            Width="600px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right;">
                                                        <asp:Label ID="lblTGQApplyReason" runat="server" Text="申请理由：" Width="150px"></asp:Label>
                                                    </td>
                                                    <td style="width: 98%; text-align: left;">
                                                        <asp:TextBox ID="txtTGQApplyReason" Enabled="false" runat="server" Height="60px"
                                                            TextMode="MultiLine" Width="600px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right;">
                                                        <asp:Label ID="lblTGQRefusalReason" runat="server" Text="拒绝理由：" Width="150px"></asp:Label>
                                                    </td>
                                                    <td style="width: 98%; text-align: left;">
                                                        <asp:TextBox ID="txtTGQRefusalReason" runat="server" Height="60px" TextMode="MultiLine"
                                                            Width="600px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="trTF" style="display: none">
                                                    <td colspan="2" style="text-align: center; width: 100%;">
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td style="text-align: right; width: 200px;">
                                                                    <b>航空公司/平台退款金额:</b>
                                                                </td>
                                                                <td style="text-align: left; width: 160px;">
                                                                    <asp:TextBox ID="txtA6" onFocus="GQPrice(this.id,1,1)" onBlur="GQPrice(this.id,2,1)"
                                                                        onkeyup="return GQPrice(this.id,3,1)" runat="server" Width="150px"></asp:TextBox>
                                                                </td>
                                                                <td style="text-align: right; width: 200px;">
                                                                    <b>航空公司/平台退款时间:</b>
                                                                </td>
                                                                <td style="text-align: left; width: 160px;">
                                                                    <asp:TextBox ID="txtA4" runat="server" onfocus="WdatePicker({isShowClear:false,autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                                                                        Width="150px"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <b><span style="color: Red">注:航空公司/平台退款金额和时间</span></b>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="trGQ" style="display: none">
                                                    <td colspan="2" style="text-align: center; width: 100%;">
                                                        <table style="width: 100%;" class="else-table" >
                                                            <tr>
                                                                <td style="width: 50%; text-align: right">
                                                                    <h2 style="color: Red">
                                                                        <b>改签补差金额: </b>
                                                                    </h2>
                                                                </td>
                                                                <td align="left" style="width: 50%;">
                                                                    <asp:TextBox runat="server" Style="font-size: 18px; text-align: center" Height="25px"
                                                                        Width="120px" ID="txtGQPrice" onFocus="GQPrice(this.id,1,3)" onBlur="GQPrice(this.id,2,3)"
                                                                        onkeyup="return GQPrice(this.id,3,3)" MaxLength="10" Text="0"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table width="100%">
                                                <tbody>
                                                    <tr>
                                                        <td align="center">
                                                            <span class="btn btn-ok-s" runat="server" id="span0" visible="false">
                                                                <asp:Button ID="btnSH" runat="server" Text="审核中等待处理" OnClick="btnSH_Click" />
                                                            </span><span class="btn btn-ok-s" runat="server" id="span1" visible="false">
                                                                <asp:Button ID="btnOktoSH" runat="server" Text="通过审核不退款" OnClick="btnOktoSH_Click"
                                                                    OnClientClick="return opOrder('是否同意审核?');" />
                                                            </span><span class="btn btn-ok-s" runat="server" id="span2" visible="false">
                                                                <asp:Button ID="btnNoOk" runat="server" Text="拒绝审核" OnClick="btnNoOk_Click" OnClientClick="return opOrder('是否拒绝审核？',1);" />
                                                            </span><span class="btn btn-ok-s" runat="server" id="span3" visible="false">
                                                                <asp:Button ID="btnOk" runat="server" Text="通过审核并退款" OnClick="btnOk_Click" OnClientClick="return opOrder('是否通过审核并退款?');" />
                                                            </span><span class="btn btn-ok-s" runat="server" id="span4" visible="false">
                                                                <asp:Button ID="btnTK" runat="server" Text="退款" OnClientClick="return opOrder('是否同意退款?');"
                                                                    OnClick="btnTK_Click" />
                                                            </span><span class="btn btn-ok-s">
                                                                <asp:Button ID="btnCancel" runat="server" Text="返回" OnClick="btnCancel_Click" />
                                                            </span>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="ebill">
                                                <div>
                                                    <span class="ebill-account"><span class="inner"><span class="inner">基础操作信息</span> </span>
                                                    </span>
                                                </div>
                                                <div class="ebill-info-bg-inner">
                                                    <table border="0" cellspacing="0" cellpadding="0" width="100%">
                                                        <tr>
                                                            <td style="text-align: right; border: 1px">
                                                                <asp:Label ID="lbl1" runat="server" Text="锁定操作员：" Width="100px"></asp:Label>
                                                            </td>
                                                            <td class="tab_in_td_f" style="width: 40%;">
                                                                <asp:Label ID="lblLockId" runat="server" Text="Label"></asp:Label>
                                                            </td>
                                                            <td style="text-align: right">
                                                                <asp:Label ID="lbl2" runat="server" Text="锁定时间：" Width="100px"></asp:Label>
                                                            </td>
                                                            <td class="tab_in_td_f" style="width: 40%;">
                                                                <asp:Label ID="lblLockTime" runat="server" Text="Label"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table id="table5" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                                        width="100%">
                                                        <thead>
                                                            <tr>
                                                                <th>
                                                                    操作时间
                                                                </th>
                                                                <th>
                                                                    操作员账号
                                                                </th>
                                                                <th>
                                                                    操作员姓名
                                                                </th>
                                                                <th>
                                                                    操作类型
                                                                </th>
                                                                <th>
                                                                    详细记录
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <asp:Repeater ID="RepOrderLog" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                                    <td style="width: 15%; text-align: center;">
                                                                        <%#Eval("OperTime")%>
                                                                    </td>
                                                                    <td style="width: 15%; text-align: center;">
                                                                        <%#Eval("OperLoginName")%>
                                                                    </td>
                                                                    <td style="width: 15%; text-align: center;">
                                                                        <%#Eval("OperUserName")%>
                                                                    </td>
                                                                    <td style="width: 10%; text-align: center;">
                                                                        <%#Eval("OperType")%>
                                                                    </td>
                                                                    <td style="width: 45%; text-align: center;">
                                                                        <asp:Label Style="word-break: break-all; white-space: normal" ID="lblLogContent"
                                                                            Width="100%" runat="server" Text=' <%#Eval("OperContent")%>' ToolTip='<%#Eval("OperContent") %>'></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </table>
                                                    <div class="fn-clear">
                                                    </div>
                                                </div>
                                                <table width="100%" cellspacing="0" cellpadding="0" border="0" class="ebill-info-bg">
                                                    <tbody>
                                                        <tr>
                                                            <td class="ebill-info-bgl">
                                                            </td>
                                                            <td class="ebill-info-bgc">
                                                            </td>
                                                            <td class="ebill-info-bgr">
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td class="mainr">
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <input type="hidden" runat="server" id="hid_TicketStatus" value="0" />
    <input type="hidden" runat="server" id="Hid_OrderStatus" />
    <div id="showOne">
    </div>
    </form>
</body>
</html>
