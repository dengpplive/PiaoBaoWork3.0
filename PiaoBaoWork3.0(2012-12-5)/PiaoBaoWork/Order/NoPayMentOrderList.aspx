<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NoPayMentOrderList.aspx.cs"
    Inherits="Order_NoPayMentOrderList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../CSS/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../CSS/table.css" rel="stylesheet" />
    <style type="text/css">
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
            color: Red;
        }
        .green
        {
            color: green;
        }
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
            float: left;
        }
        .flighttab tr
        {
            height: 30px;
            line-height: 30px;
        }
        .TextBorder
        {
            border: 1px solid #999;
        }
    </style>
    <script type="text/javascript" src="../JS/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../JS/jquery-ui-1.8.13.custom.min.js"></script>
    <script id="script_k0" src="../js/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <script type="text/javascript">

        var $J=jQuery.noConflict(false);

        //出发城市选择
        function GetFromCode(text,val,sel) {
            var Arr=text.split('-');
            if(Arr.length==2) {
                $J("#Hid_fromCode").val(Arr[0]);
                $J("#Hid_FromCity").val(Arr[1]);
            } else {
                $J("#Hid_fromCode").val("");
                $J("#Hid_FromCity").val("");
            }
        }
        //到达城市选择
        function GetToCode(text,val,sel) {
            var Arr=text.split('-');
            if(Arr.length==2) {
                $J("#Hid_toCode").val(Arr[0]);
                $J("#Hid_ToCity").val(Arr[1]);
            } else {
                $J("#Hid_toCode").val("");
                $J("#Hid_ToCity").val("");
            }
        }

        function resetClear() {
            $J("input[type='text']").val("");
            $J("select option").eq(0).attr("selected",true);
        }
        //对话框包含处理
        function showdialog(title,html,f) {
            $J("select").hide();
            $J("#show").html(html);
            $J("#show").dialog({
                title: (title==""||title==null)?'提示':title,
                bgiframe: true,
                height: 180,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    $J("select").show();
                },
                buttons: {
                    '确定': function (evt) {
                        var target=evt.srcElement?evt.srcElement:evt.target;
                        $J(target).attr("disabled",true);
                        if(f!=null) {
                            var isCancelpnr=$J("#ckpnr").attr("checked")?"0":"1";
                            f.isCancelpnr=isCancelpnr;
                            UpdateOrder(f);
                            //$J(this).dialog('close');
                        } else {
                            $J(this).dialog('close');
                            if(html.indexOf("取消成功")!= -1) {
                                $J("#btnQuery").click();
                            }
                        }
                    }
                }
            });
        }
        //取消订单
        function UpdateOrder(model) {
            var url="../AJAX/CommonAjAx.ashx";
            var val_OpFunction="CancelOrder"; //线下订单处理
            var val_OpType="2"; //修改操作   
            var val_OpPage="LineOrderList.aspx";
            var param={
                oid: escape(model.m.oid),
                OrderId: escape(model.m.orderid),
                PNR: escape(model.m.pnr),
                Office: escape(model.m.office),
                LoginName: escape(model.m.LoginName),
                CpyNo: escape(model.m.CpyNo),
                isCancelpnr: escape(model.isCancelpnr),
                OpFunction: escape(val_OpFunction),
                OpType: escape(val_OpType),
                OpPage: escape(val_OpPage),
                num: Math.random(),
                currentuserid: '<%=this.mUser.id.ToString() %>'
            };
            $J.post(url,param,function Handle(data) {
                //处理数据
                var strReArr=data.split('##');
                if(strReArr.length==3) {
                    //错误代码
                    var errCode=strReArr[0];
                    //错误描述
                    var errDes=strReArr[1];
                    //错误结果
                    var result=$J.trim(unescape(strReArr[2]));
                    showdialog("提示",errDes);
                }
            },"text");

        }
        //显示取消订单
        function showCancel(model) {
            var html="";
            var LoginName=$J.trim($J("#Hid_LoginName").val());
            var CpyNo=$J.trim($J("#Hid_CpyNo").val());
            if($J.trim(model.lockU)!=""&&$J.trim(model.lockU)!=LoginName) {
                html="该订单已被账号锁定,请解锁后操作！";
                showdialog("订单取消",html);
            } else {
                html="<table style='width:100%'><tr><td>确定要取消该订单吗?</td></tr><tr><td align='left'><label for='ckpnr'><input type='checkbox' id='ckpnr' checked='checked' />保留编码</label></td></tr></table>";
                model.LoginName=LoginName;
                model.CpyNo=CpyNo;
                showdialog("订单取消",html,{ m: model,op: 1 });
            }
            return false;
        }
    </script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>    
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
    <div id="show">
    </div>
    <form id="form1" runat="server">
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="未还款订单查询" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table class="Search" cellspacing="3" cellpadding="0" border="0">
                    <tbody>
                        <tr>
                            <th>
                                订单编号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtOrderId" Width="138px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                PNR：
                            </th>
                            <td>
                                <asp:TextBox ID="txtPNR" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                乘机日期：
                            </th>
                            <td>
                                <input id="txtFromDate1" type="text" readonly="readonly" style="width: 110px" runat="server"
                                    class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />-
                                <input id="txtFromDate2" type="text" readonly="readonly" style="width: 110px" runat="server"
                                    class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                出发城市：
                            </th>
                            <td>
                                <%--   <uc1:SelectAirCode ID="ddlFromCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--出发城市--"
                                    ChangeFunctionName="GetFromCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />--%>
                                <input name="ddlFromCity" class="inputtxtdat" type="text" id="ddlFromCity" runat="server"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 110px;" mod_address_reference="Hid_fromCode" />
                            </td>
                            <th>
                                到达城市：
                            </th>
                            <td>
                                <%--  <uc1:SelectAirCode ID="ddlToCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--到达城市--"
                                    ChangeFunctionName="GetToCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />--%>
                                <input name="ddlToCity" class="inputtxtdat" type="text" id="ddlToCity" runat="server"
                                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                    style="width: 110px;" mod_address_reference="Hid_toCode" />
                            </td>
                            <th>
                                创建日期：
                            </th>
                            <td>
                                <input id="txtCreateTime1" type="text" readonly="readonly" style="width: 110px" runat="server"
                                    class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />-
                                <input id="txtCreateTime2" type="text" readonly="readonly" style="width: 110px" runat="server"
                                    class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:true})" />
                            </td>
                        </tr>
                        <tr id="More">
                            <th>
                                乘机人：
                            </th>
                            <td>
                                <asp:TextBox ID="txtPassengerName" Width="110" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                航班号：
                            </th>
                            <td>
                                <asp:TextBox ID="txtFlightCode" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <th>
                                航空公司：
                            </th>
                            <td colspan="2">
                                <uc1:SelectAirCode ID="SelectAirCode1" IsDShowName="false" DefaultOptionValue=""
                                    DefaultOptionText="" runat="server" />
                            </td>
                        </tr>

                        <tr>
                            <th>
                            客户名：
                            </th>
                            <td>
                            <asp:TextBox ID="txtcreatecpy" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                            <td>
                            </td>
                            <td colspan="3">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClientClick="return resetClear();"
                                        OnClick="btnClear_Click" /></span>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                    width="100%" style="border-top: 1px #ddd solid; margin-top: 5px">
                    <thead>
                        <tr>
                        <th>
                            客户名
                        </th>
                            <th>
                                订单号
                            </th>
                            <th>
                                PNR
                            </th>
                            <th>
                                订单金额
                            </th>
                            <th>
                                欠款金额
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
                                订单状态
                            </th>
                            <th>
                                锁定用户
                            </th>
                            <th>
                                操作
                            </th>
                        </tr>
                    </thead>
                    <asp:Repeater ID="repList" runat="server">
                        <ItemTemplate>
                            <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                                <td>
                                <%# Eval("CreateCpyName") %>
                                </td>
                                <td style="width: 10%;">
                                    <%# Eval("OrderId") %>
                                </td>
                                <td style="width: 7%;">
                                    <%# Eval("pnr")%>
                                </td>
                                <td style="width: 7%;">
                                <%# GetPrice(Eval("PayMoney"), Eval("OrderMoney"), Eval("TicketStatus"))%>
                                </td>
                                 
                                <td style="width: 7%; color:Red;">
                                    <%# Eval("PayDebtsMoney")%>
                                </td>
                                <td style="width: 7%;">
                                    <%# DataBinder.Eval(Container.DataItem,"CreateTime","{0:yyyy-MM-dd HH:mm:ss}")%>
                                </td>
                                <td style="width: 7%;">
                                    <asp:Label ID="lblId" runat="server" Text='<%#Eval("id") %>' Visible="false"></asp:Label>
                                    <%#ShowText(1, Eval("PassengerName"))%>
                                </td>
                                <td style="width: 7%;">
                                    <%#ShowText(4, Eval("AirTime"))%>
                                </td>
                                <td style="width: 7%;">
                                    <%# ShowText(2,Eval("Travel"))%>
                                </td>
                                <td style="width: 7%;">
                                    <%# Eval("FlightCode")%>
                                </td>
                                <td style="width: 7%;">
                                    <%#ShowText(3, Eval("OrderStatusCode"))%>
                                </td>
                                <td style="width: 7%;">
                                    <%# Eval("LockLoginName")%>
                                </td>
                                <td class="Operation" style="width: 10%;">
                                    <a href='OrderDetail.aspx?id=<%# Eval("id")%>&Url=NoPayMentOrderList.aspx&currentuserid=<%=this.mUser.id.ToString() %>'>订单详情</a><br />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
            <input type="hidden" id="hid_PageSize" value="20" runat="server" />
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
            </webdiyer:AspNetPager>
        </div>
        <%--出发城市三字码 --%>
        <input type="hidden" id="Hid_fromCode" runat="server" />
        <%--到达城市三字码 --%>
        <input type="hidden" id="Hid_toCode" runat="server" />
        <%--出发城市 --%>
        <input type="hidden" id="Hid_FromCity" runat="server" />
        <%--到达城市 --%>
        <input type="hidden" id="Hid_ToCity" runat="server" />
        <%--登录账号--%>
        <input type="hidden" id="Hid_LoginName" runat="server" />
        <%--登录账号--%>
        <input type="hidden" id="Hid_CpyNo" runat="server" />
        <script language="javascript" type="text/javascript">
            //重新加载城市控件
            function ReLoad() {
                var rd=""; //  "?r=" + Math.random();
                var SE=new CtripJsLoader();
                var files=[["../js/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
                SE.scriptAll(files);
            }
            $J(function () {
                ReLoad();
            });
        </script>
    </div>
    </form>
</body>
</html>
