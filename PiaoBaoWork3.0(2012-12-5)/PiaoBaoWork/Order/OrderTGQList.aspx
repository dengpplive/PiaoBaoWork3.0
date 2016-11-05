<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderTGQList.aspx.cs" Inherits="Order_OrderTGQList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>待退改签订单</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script id="script_k0" src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
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
        .sec3
        {
            background: url(../img/all_pic.gif);
            background-position: 0px -157px;
            text-align: center;
            cursor: hand;
            color: #fff;
        }
        .sec4
        {
            background: url(../img/all_pic.gif);
            background-position: -112px -157px;
            text-align: center;
            cursor: hand;
            color: #fff;
        }
        .sec1 a
        {
            display: inline-block;
            height: 27px;
            line-height: 24px;
            text-decoration: none;
            color: #666;
        }
        .sec2 a
        {
            display: inline-block;
            height: 27px;
            line-height: 24px;
            text-decoration: none;
            color: #666;
        }
        .sec3 a
        {
            display: inline-block;
            height: 27px;
            line-height: 24px;
            text-decoration: none;
            color: #FFFFFF;
        }
        .sec4 a
        {
            display: inline-block;
            height: 27px;
            line-height: 24px;
            text-decoration: none;
            color: #FFFFFF;
        }
        
        .nav_tgq
        {
            width: 98%;
            margin: auto;
            text-align: left;
            margin-top: 5px;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .inputBorder
        {
            border: 1px solid #999;
        }
         .red
        {
            color: red;
        }
        .green
        {
            color: green;
        }
    </style>
    <script language="JavaScript" type="text/javascript">

            <!--
        //为Jquery重新命名
        var $J=jQuery=jQuery.noConflict(false);
        //-->

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

        var i=0;
        function moreSearchOrder() {
            if($J("#moreSearchOrder").css("display")=="none") {
                $J("#moreSearchOrderA").attr("class","MoreCondition");
            }
            else {
                $J("#moreSearchOrderA").attr("class","MoreConditionA");
            }
            $J("#moreSearchOrder").toggle('slow');
            if(i==0) {
                document.getElementById("txtFromCity").focus();
                document.getElementById("txtFromCity").blur();
            }
            i++;
        }

        function createXMLHttpRequest() {
            if(window.ActiveXObject) {
                xmlHttps=new ActiveXObject("Microsoft.XMLHTTP");
            }
            else if(window.XMLHttpRequest) {
                xmlHttps=new XMLHttpRequest();
            }
        }

        function mouseOver(oId,Id,e) {
            var showDiv=document.getElementById("showDiv");

            createXMLHttpRequest();
            xmlHttps.open("post","../Ajax/CustomerInfo.aspx?oId="+oId+"&Id="+Id,true);
            xmlHttps.onreadystatechange=callback;
            xmlHttps.send(null);
        }

        function mouseOut() {
            document.getElementById("showDiv").style.display="none";
        }

        function callback() {
            if(xmlHttps.readyState==4) {
                if(xmlHttps.status==200) {
                    var sret=xmlHttps.responseText;

                    $J("#showOne").html(sret);
                    $J("#showOne").dialog({
                        title: '客户信息',
                        bgiframe: true,
                        width: 230,
                        height: 320,
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
            }
        }

        function btnOk(num) {
            document.getElementById("Hid_num").value=num;
            document.getElementById("ddlStatus").value=num;
            SetBtnClass();
            document.getElementById("btnQuery").click();
        }
        function SetBtnClass() {
            var num=document.getElementById("Hid_num").value;
            var tab=document.getElementById("secTable");

            for(var i=0;i<tab.rows[0].cells.length;i++) {
                tab.rows[0].cells[i].className="sec1"; //全部清空样式
            }

            var tdclassName="sec2"; //默认样式

            var tdid="tdSec"+num;

            if(num==2) {
                tdclassName="sec3"; //样式
            } else if(num==3) {
                tdclassName="sec4"; //样式
            } else {
                tdclassName="sec2"; //默认样式
            }

            document.getElementById(tdid).className=tdclassName;  //设置选中样式


            //            for (var i = 0; i < tab.rows[0].cells.length; i++) {
            //                if (i == (num - 1)) {
            //                    tab.rows[0].cells[i].className = "sec2";
            //                } else {
            //                    tab.rows[0].cells[i].className = "sec1";
            //                }
            //            }
            //            if (num == 2) {
            //                tab.rows[0].cells[1].className = "sec3";
            //            } else if (num == 3) {
            //                tab.rows[0].cells[2].className = "sec4";
            //            }

        }
        function ddlSel(obj) {
            document.getElementById("Hid_num").value=obj.value;
            SetBtnClass();
        }
        $J(function () {
            $J("td.Operation > a").click(function () {
                $J("#queryparam").val($J("#txtOrderId,#IsQuery,#currentuserid,#txtPNR,#txtPassengerName,#hidFromCity,#txtFromDate1,#txtFromDate2,#hidToCity,#txtCreateTime1,#txtCreateTime2,#txtFlightCode,#rbtlOrderS,#ddlStatus,#AspNetPager1_input").serialize());
            })
        })
    </script>
    <script src="../js/js_GetUserInfo.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/js_Copy.js"></script>
</head>
<body onload="SetBtnClass()">
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
    <asp:HiddenField ID="queryparam" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="IsQuery" runat="server" ClientIDMode="Static" />
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="待退改签订单" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table id="moreSearch" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <th>
                            订单编号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtOrderId" CssClass="inputtxtdat" Width="138px" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            PNR：
                        </th>
                        <td>
                            <asp:TextBox ID="txtPNR" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                        </td>
                        <th>
                            乘机人：
                        </th>
                        <td>
                            <asp:TextBox ID="txtPassengerName" CssClass="inputtxtdat" runat="server" Width="110px"></asp:TextBox>
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
                            乘机日期：
                        </th>
                        <td colspan="3">
                            <input id="txtFromDate1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                onfocus="WdatePicker({isShowClear:true})" />-<input id="txtFromDate2" type="text"
                                    readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                        </td>
                    </tr>
                    <tr>
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
                        <td colspan="3">
                            <input id="txtCreateTime1" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                onfocus="WdatePicker({isShowClear:true})" />-<input id="txtCreateTime2" type="text"
                                    readonly="readonly" runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                        </td>
                    </tr>
                    <tr id="More" style="display: none;" runat="server">
                        <th>
                            航班号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtFlightCode" CssClass="inputtxtdat" Width="110px" runat="server"></asp:TextBox>
                        </td>
                        <th runat="server" id="OrderSourceth">
                            订单来源：
                        </th>
                        <td colspan="2" runat="server" id="OrderSourcetd">
                            <asp:RadioButtonList ID="rbtlOrderS" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Selected="True" Value="0" Text="全部"></asp:ListItem>
                                <asp:ListItem Value="1" Text="C站订单"></asp:ListItem>
                                <asp:ListItem Value="2" Text="分销订单"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            订单状态：
                        </th>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlStatus" onchange="ddlSel(this)">
                                <%-- <asp:ListItem Text="全部" Value="1"></asp:ListItem>
                                <asp:ListItem Text="退票等待审核" Value="2"></asp:ListItem>
                                <asp:ListItem Text="废票等待审核" Value="3"></asp:ListItem>
                                <asp:ListItem Text="改签" Value="4"></asp:ListItem>
                                <asp:ListItem Text="审核通过,待退款" Value="5"></asp:ListItem>
                                <asp:ListItem Text="拒绝申请的订单" Value="6"></asp:ListItem>
                                <asp:ListItem Text="交易结束" Value="7"></asp:ListItem>
                                <asp:ListItem Text="异地待退废改签订单" Value="8"></asp:ListItem>
                                <asp:ListItem Text="退款中的订单" Value="9"></asp:ListItem>
                                  <asp:ListItem Text="审核中" Value="10"></asp:ListItem>
                                --%>
                            </asp:DropDownList>
                        </td>
                         <th>
                                客户名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtCpyName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                        <td align="center" colspan="2">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnQuery" runat="server" Text="查 询" OnClick="btnQuery_Click" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" />
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="nav_tgq">
            <table id="secTable" style="border-collapse: collapse; z-index: 1; position: relative;"
                border="0" cellspacing="0" cellpadding="0">
                <tr align="left">
                    <td class="sec2" width="97px" id="tdSec1" runat="server" visible="true">
                        <asp:LinkButton ID="btn1" OnClientClick="btnOk(1);return true;" runat="server" Height="27px"
                            Style="">全部</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec10" runat="server" visible="true">
                        <asp:LinkButton ID="btn10" OnClientClick="btnOk(10);return false;" runat="server"
                            Height="27px" Style="">审核中的订单</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec2" runat="server" visible="true">
                        <asp:LinkButton ID="btn2" OnClientClick="btnOk(2);return false;" runat="server" Height="27px"
                            Style="">退票等待审核</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec3" runat="server" visible="true">
                        <asp:LinkButton ID="btn3" OnClientClick="btnOk(3);return false;" runat="server" Height="27px"
                            Style="">废票等待审核</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec4" runat="server" visible="true">
                        <asp:LinkButton ID="btn4" OnClientClick="btnOk(4);return false;" runat="server" Height="27px"
                            Style="">改签订单</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec5" runat="server" visible="true">
                        <asp:LinkButton ID="btn5" OnClientClick="btnOk(5);return false;" runat="server" Height="27px"
                            Style="">审核通过,待退款</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec6" runat="server" visible="true">
                        <asp:LinkButton ID="btn6" OnClientClick="btnOk(6);return false;" runat="server" Height="27px"
                            Style="">拒绝申请的订单</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec7" runat="server" visible="true">
                        <asp:LinkButton ID="btn7" OnClientClick="btnOk(7);return false;" runat="server" Height="27px"
                            Style="">交易结束</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec8" runat="server" visible="true">
                        <asp:LinkButton ID="btn8" OnClientClick="btnOk(8);return false;" runat="server" Height="27px"
                            Style="">异地退废改签订单</asp:LinkButton>
                    </td>
                    <td class="sec1" width="97px" id="tdSec9" runat="server" visible="true">
                        <asp:LinkButton ID="btn9" OnClientClick="btnOk(9);return false;" runat="server" Height="27px"
                            Style="">退款中的订单</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
        <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="0" cellpadding="0"
            border="0">
            <thead>
                <tr>
                    <th>
                        客户名称
                    </th>
                    <th>
                        PNR
                    </th>
                    <th>
                        政策来源
                    </th>
                    <th>
                        创建时间
                    </th>
                    <th>
                        乘机人
                    </th>
                    <th>
                        起飞时间
                    </th>
                    <th>
                        行程
                    </th>
                    <th>
                        航班号
                    </th>
                    <th>
                        订单金额
                    </th>
                    <th>
                        订单状态
                    </th>
                    <th>
                        支付状态
                    </th>
                    <th>
                        是否退款
                    </th>
                    <th>
                        锁定帐号
                    </th>
                    <th>
                        操作
                    </th>
                </tr>
            </thead>
            <asp:Repeater ID="repList" runat="server" OnItemCommand="repList_ItemCommand" OnItemDataBound="repList_ItemDataBound">
                <ItemTemplate>
                    <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                        <%--<td style="width: 10%;" onmouseover='mouseOver(<%# Eval("Id") %>,<%# Eval("CorporationIdInt") %>,window.event);' onmouseout="mouseOut();">--%>
                        <td style="width: 10%;">
                            <a href="#" onclick="return GetUserInfo('<%# Eval("OrderId") %>')">
                                <%# Eval("OwnerCpyName")%></a>
                        </td>
                        <td style="width: 6%;">
                                <%# Eval("PNR")%>
                             <%# (string.IsNullOrEmpty(Eval("BigCode").ToString()) == true ? "" : "<br />大编码:" + Eval("BigCode").ToString())%>
                            <%# ZFZ(Eval("A8").ToString()) %>
                            <input type="button"  onclick="copyToClipboard('<%# Eval("PNR")%>')" class="hide" value="复制" /><br />
                                   
                        </td>
                        <td style="width: 6%;">
                            <%# DataSourceMessage(24, Eval("PolicySource"))%>
                        </td>
                        <td style="width: 7%;">
                            <%#Eval("CreateTime")%>
                        </td>
                        <td style="width: 7%;">
                            <%# DataSourceMessage(23, Eval("PassengerName"))%>
                        </td>
                        <td style="background-color: <%# DataSourceColor(Eval("AirTime"),1)%>; width: 7%;">
                            <%# Eval("AirTime")%>
                        </td>
                        <td style="width: 10%;">
                            <%# Eval("Travel")%>
                        </td>
                        <td style="width: 7%;">
                            <%# Eval("CarryCode")+" "+Eval("FlightCode")%>
                        </td>
                        <td class="Price" style="width: 7%; color: Red;">
                            <%# GetPrice(Eval("OrderMoney"), Eval("TicketStatus"))%>
                        </td>
                        <td style="color: <%# DataSourceColor(Eval("OrderStatusCode").ToString(),2)%>; width: 7%;">
                            <%#DataSourceMessage(1, Eval("OrderStatusCode"))%>
                        </td>
                        <td style="width: 7%;">
                            <%#DataSourceMessage(3, Eval("PayStatus"))%>
                        </td>
                        <td>
                        </td>
                        <td style="width: 7%;">
                            <%# Eval("LockLoginName")%>
                        </td>
                        <td class="Operation" style="width: 10%;">
                            <asp:LinkButton runat="server" ID="lbtnOrderProces" Text="订单处理" CommandName="OrderProces"
                                CommandArgument='<%#Eval("id")%>' Visible="false"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lbtnGQOK" Text="改签处理" CommandName="GQOk" CommandArgument='<%#Eval("id")%>'
                                Visible="false"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lbtnTPExamine" Text="退款" CommandName="TPExamine"
                                CommandArgument='<%#Eval("id")%>' Visible="false"> </asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lbtnDetail" Text="订单详情" CommandName="Detail" CommandArgument='<%#Eval("id")%>'></asp:LinkButton>
                            <asp:HiddenField ID="hid_id" runat="server" Value='<%# Eval("id")%>' />
                            <asp:HiddenField ID="hid_OrderStatusCode" runat="server" Value='<%# Eval("OrderStatusCode")%>' />
                            <asp:HiddenField ID="hid_PayStatus" runat="server" Value='<%# Eval("PayStatus")%>' />
                            <asp:HiddenField ID="hid_PayWay" runat="server" Value='<%# Eval("PayWay")%>' />
                            <asp:HiddenField ID="hid_PolicySource" runat="server" Value='<%# Eval("PolicySource")%>' />
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
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
    <div id="Show" style="width: 100%; text-align: center; font-size: 12px; line-height: 20px;
        color: Gray; margin-top: 0;" runat="server" visible="false">
        没有符合你要搜索的条件,请输入正确的查询条件！
    </div>
    <input type="hidden" runat="server" id="Hid_num" value="1" />
    <div id="showOne">
    </div>
    <div id="showDiv" style="display: none; z-index: 9999; position: absolute;">
    </div>
    <script type="text/javascript" language="javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../JS/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        $J(ReLoad);  
    </script>
    </form>
</body>
</html>
