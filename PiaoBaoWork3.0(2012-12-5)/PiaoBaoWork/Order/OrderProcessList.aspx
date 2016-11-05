<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderProcessList.aspx.cs"
    Inherits="Order_OrderProcessList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>待出票订单</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script id="script_k0" src="../js/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
        .Search tr
        {
            height: 30px;
            line-height: 30px;
        }
        .tb-all-trade td
        {
            text-align: center;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .red
        {
            color: red;
        }
        .green
        {
            color: green;
        }
        .tdbg
        {
            background-color: Red;
            font-weight: bold;
        }
        .sec1
        {
            background: url(../img/all_pic.gif);
            background-position: -262px -36px;
            text-align: center;
            cursor: hand;
            color: #000;
        }
        .sec2
        {
            background: url(../img/all_pic.gif);
            background-position: -79px -36px;
            text-align: center;
            cursor: hand;
            color: #fff;
        }
    </style>
    <script type="text/javascript">
        //为Jquery重新命名
        var $J=jQuery=jQuery.noConflict(false);
        //-->

        function showdialog(t) {
            $J("#dd").html(t);
            $J("#dd").dialog({
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
                        $J(this).dialog('close');
                    },
                    "取消": function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }
        function showdialogsByPol(id) {
            $J('#Hid_IdByPol').val(id);
            var opValues=document.getElementById("hidSelValuesByPol").value;
            var divValue=" <table width='100%'><tr><td align='right'>订单来源:</td><td>";
            divValue+=" <select id='SelvalueByPol' onchange='ddd(this.value);' style=\"width: 100%;\">";
            divValue+=opValues; //选择理由
            divValue+=" </select>";
            divValue+=" </td></tr><tr id='outTr' style='display:none;'>";
            divValue+="   <td align='right'>";
            divValue+=" <span style='color:Red'>*</span>外部订单号: </td>";
            divValue+=" <td colspan='3'>";

            //divValue += "<input id=\"txtResoneByPol\" type=\"text\" style=\"width:200px;\"/></td></tr><tr><td></td><td>&nbsp;";

            //            divValue += " <input id='btnUpdate' type='button' value='确 定' onclick='fbtnRefuseByPol()'/>&nbsp;";
            //            divValue += " <input id='btnNo' type='button' value='取 消' onclick='btnCloseByPol()'/></td></tr></table>";

            divValue+="<input id=\"txtResoneByPol\" type=\"text\" style=\"width:200px;\"/></td></tr>";
            divValue+="</table>";

            $J("#dd").html(divValue);
            $J("#dd").dialog({
                title: '温馨提示',
                bgiframe: true,
                width: 300,
                height: 200,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        fbtnRefuseByPol();
                    },
                    "取消": function () {
                        btnCloseByPol();
                    }
                }
            });
        }
        function fbtnRefuseByPol() {
            var PolicySource=document.getElementById("SelvalueByPol").value;

            if(document.getElementById("outTr").style.display=="") {
                if(document.getElementById("txtResoneByPol").value=="") {
                    alert("外部订单号不能为空！");
                    return;
                }
            }
            $J("#dd").dialog("close");
            document.getElementById("hidPol").value=PolicySource;
            document.getElementById("hidOutOrderId").value=document.getElementById("txtResoneByPol").value;
            document.getElementById("btnPolicySource").click();
        }
        function btnCloseByPol() {
            $J("#dd").dialog("close");
        }

        function ddd(PolicySource) {
            if(parseInt(PolicySource)>2&&PolicySource!="9") {
                document.getElementById("outTr").style.display="";
            }
            else {
                document.getElementById("outTr").style.display="none";
            }
        }

        function showdialogs(id) {
            $J('#hid_order_id').val(id);

            var opValues=$J('#hid_SelValues').val();
            var divValue=" <table width='100%'><tr><td align='right'>选项:</td><td>";
            divValue+=" <select id='Selvalue' onchange='document.getElementById(\"txtResone\").value=this.value' style=\"width: 100%;\">";
            divValue+=opValues; //选择理由
            divValue+=" </select>";
            divValue+=" </td></tr><tr>";
            divValue+="   <td align='right'>";
            divValue+=" <span style='color:Red'>*</span>理由: </td>";
            divValue+=" <td colspan='3'>";

            //divValue += " <textarea id='txtResone'style=\"width:200px; height: 90px;\"></textarea></td></tr><tr><td></td><td>&nbsp;";
            //            divValue += " <input id='btnUpdate' type='button' value='确 定' onclick='fbtnRefuse()'/>&nbsp;";
            //            divValue += " <input id='btnNo' type='button' value='取 消' onclick='btnClose()'/></td></tr></table>";

            divValue+=" <textarea id='txtResone'style=\"width:200px; height: 90px;\"></textarea></td>";
            divValue+=" </td></tr></table>";

            $J("#dd").html(divValue);
            $J("#dd").dialog({
                title: '温馨提示',
                bgiframe: true,
                width: 300,
                height: 200,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        fbtnRefuse();
                    },
                    "取消": function () {
                        btnClose();
                    }
                }
            });
        }

        function fbtnRefuse() {
            var pek=$J("#txtResone").val();
            pek=pek.replace(" ","");

            if(pek==""||pek.length==0) {
                alert("请输入理由！");
                return;
            }

            if(pek.length>200) {
                alert("理由太长，不能超过200个字符！");
                return;
            }
            $J("#dd").dialog("close");

            $J("#hid_pek").val(pek);
            document.getElementById("btnRefuse").click();
        }
        function btnClose() {
            $J("#dd").dialog("close");
        }
        //订单提醒跳转 查询订单
        function RequestQuery(isQuery) {
            if(isQuery=="1") {
                document.getElementById("btnQuery").click();
            }
        }


        function ChangeAirPolicy(obj) {
            try {
                var UseType=0;
                var PMFee=0;
                var ABFee=0;
                var FuelFee=0;

                var Policy=parseFloat(obj.value); //点数
                var JSFee=0;

                if(Policy=="") {
                    showdialog("请输入返点!");
                } else if(parseFloat(Policy)<0) {
                    showdialog("返点必须大于0!");
                }
                else {
                    var allUsers=$J("#repOrderList_hidValPriceSVal_"+obj.title).val();
                    if(allUsers!=undefined&&allUsers!=null) {
                        var passengers=allUsers.split('|');
                        if(passengers!=null&&passengers.length>0) {
                            for(var i=0;i<passengers.length;i++) {
                                var passenger=passengers[i].split('^');

                                UseType=passenger[0];
                                PMFee=parseFloat(passenger[1]);
                                ABFee=parseFloat(passenger[2]);
                                FuelFee=parseFloat(passenger[3]);

                                if(UseType==1||UseType==2) {
                                    JSFee+=PMFee*(1-(Policy/100))+ABFee+FuelFee;
                                }
                                else if(UseType==3) {
                                    JSFee+=PMFee+ABFee+FuelFee;
                                }
                            }
                        }
                    }

                    JSFee=JSFee.toFixed(2);
                    $J("#repOrderList_txtCgFeeBB_"+obj.title).val(JSFee);
                }
            } catch(Error) {
                showdialog("请正确输入返点!");
            }
        }


        function ChangeAirPolicyFeeChange(obj) {
            try {
                var UseType=0;
                var PMFee=0;
                var ABFee=0;
                var FuelFee=0;
                var uCount=0;
                var price=parseFloat(obj.value); //金额

                if(price=="") {
                    showdialog("请正确输入金额!");
                } else if(parseFloat(price)<0) {
                    showdialog("请正确输入金额大于0!");
                }
                else {
                    var allUsers=$J("#repOrderList_hidValPriceSVal_"+obj.title).val();
                    if(allUsers!=undefined&&allUsers!=null) {
                        var passengers=allUsers.split('|');
                        if(passengers!=null&&passengers.length>0) {
                            for(var i=0;i<passengers.length;i++) {
                                var passenger=passengers[i].split('^');
                                UseType=passenger[0];
                                if(UseType==1||UseType==2) {
                                    uCount++; //累计成人个数
                                    PMFee=passenger[1];
                                    ABFee=passenger[2];
                                    FuelFee=passenger[3];
                                }
                                else if(UseType==3) {
                                    price=price-parseFloat(passenger[1])-parseFloat(passenger[2])-parseFloat(passenger[3]);
                                }
                            }
                        }
                    }
                    var payMoney=(price/uCount)-ABFee-FuelFee; //单人价格
                    var Policy=(PMFee-payMoney)/PMFee;
                    Policy=Policy*100;
                    Policy=Policy.toFixed(2);

                    $J("#repOrderList_txtAirlinePReturn_"+obj.title).val(Policy);
                }
            } catch(Error) {
                showdialog("请正确输入金额!");
            }
        }


        function GetAuth(Office) {
            var strOffice="RMK TJ AUTH "+Office;
            showdialog("授权指令:"+strOffice.toUpperCase());
            return false;
        }

    </script>
    <script type="text/javascript" src="../js/js_GetUserInfo.js"> </script>
    <script type="text/javascript" src="../js/js_Copy.js"></script>
</head>
<body>
    <%--城市控件使用容器开始--%>
    <div id="jsContainer">
        <div id="jsHistoryDiv" class="hide">
            <iframe id="jsHistoryFrame" name="jsHistoryFrame" src="about:blank"></iframe>
        </div>
        <textarea id="jsSaveStatus" class="hide"></textarea>
        <div id="tuna_jmpinfo" style="visibility: hidden; position: absolute; z-index: 120;
            overflow: hidden;">
        </div>
        <div id="tuna_alert" style="display: none; position: absolute; z-index: 999; overflow: hidden;">
        </div>
        <%--日期容器--%>
        <div style="position: absolute; top: 0; z-index: 120; display: none" id="tuna_calendar"
            class="tuna_calendar">
        </div>
    </div>
    <%--城市控件使用容器结束--%>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="dd">
    </div>
    <div id="divDG">
    </div>
    <div id="tabs">
        <div class="title">
            <span>待出票订单</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="moreSearch" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <th>
                                订单号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtOrderId" Width="138px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                PNR：
                            </th>
                            <td>
                                <asp:TextBox ID="txtPNR" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                乘机日期：
                            </th>
                            <td>
                                <input id="txtFromDate1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                    onfocus="WdatePicker({isShowClear:true})" />-<input id="txtFromDate2" type="text"
                                        readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                出发城市：
                            </th>
                            <td>
                                <input name="txtFromCity" class="inputtxtdat" type="text" id="txtFromCity" runat="server"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 110px;" mod_address_reference="hidFromCity" />
                                <input type="hidden" runat="server" id="hidFromCity" />
                            </td>
                            <th>
                                到达城市：
                            </th>
                            <td>
                                <input name="txtToCity" class="inputtxtdat" type="text" id="txtToCity" runat="server"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 110px;" mod_address_reference="hidToCity" />
                                <input type="hidden" runat="server" id="hidToCity" />
                            </td>
                            <th>
                                创建日期：
                            </th>
                            <td>
                                <input id="txtCreateTime1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                    onfocus="WdatePicker({isShowClear:true})" />-<input id="txtCreateTime2" type="text"
                                        readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                            </td>
                        </tr>
                        <tr id="More">
                            <th>
                                乘机人：
                            </th>
                            <td>
                                <asp:TextBox ID="txtPassengerName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                客户名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCorporationName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                订单状态：
                            </th>
                            <td>
                                <asp:DropDownList ID="ddlStatus" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                航班号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtFlightCode" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                航空公司：
                            </th>
                            <td>
                                <uc1:SelectAirCode ID="SelectAirCode1" runat="server" IsDShowName="false" DefaultOptionValue="" />
                            </td>
                            <%--                     <th runat="server" id="OrderSourceth">
                                订单来源：
                            </th>
                            <td runat="server" id="OrderSourcetd">
                                <asp:RadioButtonList ID="rbtlOrderS" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                    <asp:ListItem Selected="True" Value="0" Text="全部"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="C站订单"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="分销订单"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>--%>
                        </tr>
                        <tr>
                            <td colspan="6" style="text-align: center;">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" /></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div>
                <table id="secTable" border="0" cellspacing="0" cellpadding="0">
                    <tr align="left">
                        <td class="sec2" width="97px" id="orderstatus0" runat="server">
                            <asp:LinkButton ID="btn0" runat="server" Height="27px" Style="line-height: 24px;
                                text-decoration: none; color: #666;" OnClick="btn0_Click">所有订单</asp:LinkButton>
                            <asp:Label runat="server" ID="lab0" Text="0" ForeColor="Red"></asp:Label>
                        </td>
                        <td class="sec1" width="97px" id="orderstatus1" runat="server">
                            <asp:LinkButton ID="btn1" runat="server" Height="27px" Style="line-height: 24px;
                                text-decoration: none; color: #666;" OnClick="btn1_Click">本地(B2B)</asp:LinkButton>
                            <asp:Label runat="server" ID="lab1" Text="0" ForeColor="Red"></asp:Label>
                        </td>
                        <td class="sec1" width="97px" id="orderstatus2" runat="server">
                            <asp:LinkButton ID="btn2" runat="server" Height="27px" Style="line-height: 24px;
                                text-decoration: none; color: #666;" OnClick="btn2_Click">本地(BSP)</asp:LinkButton>
                            <asp:Label runat="server" ID="lab2" Text="0" ForeColor="Red"></asp:Label>
                        </td>
                        <td class="sec1" width="97px" id="orderstatus3" runat="server">
                            <asp:LinkButton ID="btn3" runat="server" Height="27px" Style="line-height: 24px;
                                text-decoration: none; color: #666;" OnClick="btn3_Click">平台订单</asp:LinkButton>
                            <asp:Label runat="server" ID="lab3" Text="0" ForeColor="Red"></asp:Label>
                        </td>
                        <td class="sec1" width="97px" id="orderstatus4" runat="server">
                            <asp:LinkButton ID="btn4" runat="server" Height="27px" Style="line-height: 24px;
                                text-decoration: none; color: #666;" OnClick="btn4_Click">共享订单</asp:LinkButton>
                            <asp:Label runat="server" ID="lab4" Text="0" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                width="100%">
                <thead>
                    <tr>
                        <th>
                            操作
                        </th>
                        <th>
                            锁定帐号
                        </th>
                        <th>
                            客户名称
                        </th>
                        <th>
                            PNR
                        </th>
                        <th>
                            政策返点
                        </th>
                        <th>
                            政策来源
                        </th>
                        <th>
                            代付状态
                        </th>
                        <th>
                            时长
                        </th>
                        <th>
                            乘机时间
                        </th>
                        <th>
                            乘机人
                        </th>
                        <th>
                            行程
                        </th>
                        <th>
                            航班号
                        </th>
                        <th>
                            支付时间
                        </th>
                        <th>
                            订单金额
                        </th>
                        <th>
                            代付金额
                        </th>
                        <th>
                            订单编号
                        </th>
                        <th>
                            订单状态
                        </th>
                        <th>
                            创建时间
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repOrderList" runat="server" OnItemCommand="repOrderList_ItemCommand"
                    OnItemDataBound="repOrderList_ItemDataBound">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                            <td class="Operation" style="line-height: 30px;">
                                <asp:LinkButton runat="server" ID="lbtnOrderOp" Text="处理订单" CommandName="OrderOp"
                                    CommandArgument='<%# Eval("id")%>'></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbtnDetail" Text="订单详情" CommandName="Detail" CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                                <%# ShowText(9, Eval("PolicySource"), Eval("PrintOffice"), Eval("OrderSourceType"))%>
                            </td>
                            <td>
                                <asp:Label ID="lblLockLoginName" runat="server" Text='<%# Eval("LockLoginName")%>'></asp:Label>
                            </td>
                            <td>
                                <%# GetUserName(Eval("OrderId").ToString(), Eval("OwnerCpyName").ToString(), Eval("CPCpyNo").ToString(), Eval("PolicySource").ToString())%>
                            </td>
                            <td>
                                <%# Eval("PNR")%><input type="button" onclick="copyToClipboard('<%# Eval("PNR")%>')"
                                    class="hide" value="复制" />
                                <%# ShowText(0, Eval("AutoPrintFlag"), Eval("PolicyType"), Eval("PolicyId"))%>
                            </td>
                            <td>
                                <%# Eval("PolicyPoint")%>
                            </td>
                            <td>
                                <%-- <%# GetStrValue("24", Eval("PolicySource").ToString())%>--%>
                                <%--<%# GetStrValue("24", Eval("PolicySource").ToString(), Eval("CPCpyNo").ToString(), Eval("PolicyType").ToString())%>--%>
                                <%# ShowText(8, Eval("PolicySource"), Eval("CPCpyNo"), Eval("PolicyType"))%>
                            </td>
                            <td>
                                <%--代付状态--%>
                                <%# ShowText(2,Eval("OutOrderPayFlag"))%>
                            </td>
                            <td>
                                <%# ShowText(3, Eval("OrderLeayTime"))%>
                            </td>
                            <td <%# ShowText(4,Eval("AirTime"))%>>
                                <%# ShowText(5, Eval("AirTime"))%>
                            </td>
                            <td>
                                <%# Eval("PassengerName").ToString().Replace("|","<br>")%>
                            </td>
                            <td>
                                <%# ShowText(1,Eval("Travel"))%>
                            </td>
                            <td>
                                <%# ShowText(1, Eval("CarryCode"))%>
                                <%# ShowText(1, Eval("FlightCode"))%>
                            </td>
                            <td>
                                <%# ShowText(5, Eval("PayTime"))%>
                                <%--支付时间--%>
                            </td>
                            <td>
                                <%# Eval("OrderMoney")%>
                            </td>
                            <td>
                                <%# Eval("OutOrderPayMoney")%>
                            </td>
                            <td>
                                <%# ShowText(7, Eval("OrderId"),Eval("IsChdFlag"),Eval("IsCHDETAdultTK"),Eval("OutOrderId"))%>
                                <%# ShowAdult(Eval("AssociationOrder"), Eval("IsChdFlag"))%>
                            </td>
                            <td>
                                <%--<%# GetStrValue("1", Eval("OrderStatusCode").ToString())%>--%>
                                <span style='color: Red;'>已经支付<br />
                                    等待出票</span>
                            </td>
                            <td>
                                <%# ShowText(6, Eval("CreateTime"))%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="19" align="center">
                                <div style="display: none;" id="divRepeater" runat="server">
                                    <table id="table5" border="0" cellspacing="0" cellpadding="0" width="80%" style="margin: auto;
                                        border: 1px solid #0092ff; border-bottom: none;">
                                        <thead>
                                            <tr style="line-height: 30px;">
                                                <th style="width: 180px">
                                                    乘客姓名
                                                </th>
                                                <th style="width: 50px">
                                                    乘客类型
                                                </th>
                                                <th style="width: 115px">
                                                    证件类型
                                                </th>
                                                <th style="width: 170px">
                                                    证件号码
                                                </th>
                                                <th style="width: 130px">
                                                    手机号
                                                </th>
                                                <th style="width: 130px">
                                                    票号
                                                </th>
                                                <th style="width: 50px">
                                                    机票状态
                                                </th>
                                                <th style="width: 60px">
                                                    舱位价
                                                </th>
                                                <th style="width: 90px">
                                                    机建/燃油
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepPassenger" runat="server">
                                            <ItemTemplate>
                                                <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                    <td style="text-align: center;">
                                                        <asp:Label runat="server" Text='<%# Eval("PassengerName") %>' ID="lblPassengerName"></asp:Label>
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Label runat="server" Text='<%# GetDictionaryName("6", Eval("PassengerType").ToString())%>'
                                                            ID="lblPassengerType"></asp:Label>
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:DropDownList ID="ddlCType" runat="server" Enabled="false">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:HiddenField runat="server" ID="Hid_PassengerID" Value='<%# Eval("Id")%>' />
                                                        <asp:TextBox ID="txtCid" runat="server" Width="160px" MaxLength="20" Enabled="false"
                                                            Text='<%# Eval("Cid")%>' CssClass="TextBorder"></asp:TextBox>
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:Label runat="server" Text='<%# Eval("A10")%>' ID="lblTel"></asp:Label>
                                                    </td>
                                                    <td style="text-align: center;">
                                                        <asp:TextBox ID="txtTicketNumber" runat="server" Text='<%# Eval("TicketNumber")%>'
                                                            Width="120px" MaxLength="14" CssClass="TextBorder" />
                                                    </td>
                                                    <td style="width: 30px; text-align: center;">
                                                        <asp:Label runat="server" Text='<%# GetDictionaryName("9", Eval("TicketStatus").ToString())%>'
                                                            ID="lblTicketStatus"></asp:Label>
                                                    </td>
                                                    <td style="text-align: center; color: Red;">
                                                        <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("PMFee")%>'></asp:Label>
                                                    </td>
                                                    <td style="text-align: center; color: Red;">
                                                        <%#Eval("ABFee")%>/<%#Eval("FuelFee")%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                    <table border="0" cellspacing="0" cellpadding="0" width="80%" style="border: 1px solid #0092ff;
                                        border-top: none;" align="center">
                                        <tr>
                                            <td style="text-align: right;">
                                                PNR:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:TextBox ID="txtPnr" Width="60px" runat="server" CssClass="TextBorder"></asp:TextBox>
                                                <asp:Label ID="lblPnrNew" runat="server" ForeColor="Red" Text="可换编码:"></asp:Label>
                                                <asp:TextBox ID="txtPnrNew" Width="60px" runat="server" MaxLength="6" CssClass="TextBorder"></asp:TextBox>
                                            </td>
                                            <td style="text-align: right;">
                                                政策来源:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:DropDownList ID="ddlPolicySource" Width="105px" runat="server">
                                                </asp:DropDownList>
                                                <asp:Label ID="lblPolicySource" runat="server" Text="" Visible="false"></asp:Label>
                                            </td>
                                            <td style="text-align: right;">
                                                <asp:Label ID="PaywayOrprintsubject" runat="server" Text="支付方式:"></asp:Label>
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:DropDownList ID="ddlPayWay" Enabled="false" Width="105px" runat="server">
                                                </asp:DropDownList>
                                                <span style="color: Red; font-size: 15px;">*</span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right;">
                                                大编码:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:TextBox ID="txtBigPNR" Width="60px" runat="server" Enabled="false" CssClass="TextBorder"></asp:TextBox>
                                            </td>
                                            <td style="text-align: right;">
                                                代付返点(%):
                                            </td>
                                            <td style="text-align: left;">
                                                <%--  <asp:TextBox ID="txtAirlinePReturn" onblur='ChangeAirPolicy(this)' ToolTip="<%# Container.ItemIndex %>" Width="105px" runat="server" Text="" CssClass="TextBorder"></asp:TextBox>--%>
                                                <asp:TextBox ID="txtAirlinePReturn" ToolTip="<%# Container.ItemIndex %>" Width="105px"
                                                    runat="server" Text="" CssClass="TextBorder"></asp:TextBox><span style="color: Red;
                                                        font-size: 15px;">*</span>
                                            </td>
                                            <td style="text-align: right;">
                                                代付金额(￥):
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:TextBox ID="txtCgFeeBB" Width="105px" onblur="ChangeAirPolicyFeeChange(this)"
                                                    ToolTip="<%# Container.ItemIndex %>" runat="server" CssClass="TextBorder"></asp:TextBox><span
                                                        style="color: Red; font-size: 15px;">*</span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right;">
                                                出票人:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:TextBox ID="txtCpNameBB" Enabled="false" Width="120px" runat="server" CssClass="TextBorder"></asp:TextBox>
                                            </td>
                                            <td style="text-align: right;">
                                                <%-- <asp:Label ID="lblPol" runat="server" Text="出票方式:"></asp:Label>--%>
                                                外部订单号:
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:TextBox ID="txtOutOrderId" Width="200px" runat="server" CssClass="TextBorder"></asp:TextBox>
                                                <%--     <asp:DropDownList ID="ddlPol" runat="server">
                                                    <asp:ListItem Value="0" Text="BSP"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="B2B"></asp:ListItem>
                                                </asp:DropDownList>--%>
                                            </td>
                                            <td colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right;">
                                                备注(出票/拒绝)：
                                            </td>
                                            <td colspan="5" style="text-align: left;">
                                                <asp:TextBox ID="txtRemak" MaxLength="200" Width="98%" runat="server" CssClass="TextBorder"></asp:TextBox>
                                                <asp:HiddenField ID="Hid_Id" runat="server" Value='<%# Eval("id")%>' />
                                            </td>
                                        </tr>
                                        <tr id="trCPBTwoB2" runat="server">
                                            <td colspan="6" height="30" valign="middle" style="text-align: center;" align="center">
                                                <asp:Button ID="lbtnPay" runat="server" CssClass="btn btn-ok-s" Width="80" Text="代付"
                                                    CommandName="Pay" CommandArgument='<%#Eval("id")%>' Visible="false"></asp:Button>
                                                <asp:Button ID="LinkDetailByJK" runat="server" CssClass="btn btn-ok-s" Width="80"
                                                    Text="状态查询" CommandName="DetailByJK" CommandArgument='<%#Eval("id")%>'></asp:Button>
                                                <asp:Button ID="btnFH" runat="server" CssClass="btn btn-ok-s" Width="80" Text="复合票号"
                                                    CommandName="FH" CommandArgument='<%# Eval("id")%>' Visible="true"></asp:Button>
                                                <asp:Button ID="btnCP" runat="server" CssClass="btn btn-ok-s" Width="80" Text="确定出票"
                                                    CommandName="CP" CommandArgument='<%# Eval("id")%>' Visible="true"></asp:Button>
                                                <asp:Button ID="btnJJCP" runat="server" CssClass="btn btn-ok-s" Width="80" Text="拒绝出票"
                                                    CommandArgument='<%# Eval("id")%>' Visible="true"></asp:Button>
                                                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-ok-s" Width="80" Text="保存"
                                                    CommandName="Save" CommandArgument='<%# Eval("id")%>' Visible="true"></asp:Button>
                                                <asp:Button ID="btnClose" runat="server" CssClass="btn btn-ok-s" Width="80" Text="关闭并解锁"
                                                    CommandName="Close" CommandArgument='<%# Eval("id")%>' Visible="true"></asp:Button>
                                                <asp:HiddenField ID="hid_PolicySource" runat="server" Value='<%# Eval("PolicySource")%>' />
                                                <asp:HiddenField ID="hid_PayWay" runat="server" Value='<%# Eval("PayWay")%>' />
                                                <asp:HiddenField ID="hid_PayFlag" runat="server" Value='<%# Eval("OutOrderPayFlag")%>' />
                                                <asp:HiddenField ID="hidValPriceSVal" runat="server" Value="" />
                                                <asp:HiddenField ID="hid_CPCpyNo" runat="server" Value='<%# Eval("CPCpyNo")%>' />
                                                <%--计算价格使用--%>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
            </webdiyer:AspNetPager>
        </div>
        <input type="hidden" id="hid_pek" runat="server" value="" />
        <input type="hidden" id="hid_order_id" runat="server" value="" />
        <input type="hidden" id="hid_SelValues" runat="server" value="" />
        <input type="hidden" id="Hid_IdByPol" runat="server" />
        <input type="hidden" id="hidSelValuesByPol" runat="server" />
        <input type="hidden" id="hidPol" runat="server" />
        <input type="hidden" id="hidOutOrderId" runat="server" />
        <span style="display: none;">
            <asp:Button ID="btnRefuse" runat="server" CssClass="btn btn-ok-s" Width="80" Text="拒绝出票"
                OnClick="btnRefuse_Click"></asp:Button>
        </span>
    </div>
    <script type="text/javascript" language="javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../js/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        $J(ReLoad);  
    </script>
    </form>
</body>
</html>
