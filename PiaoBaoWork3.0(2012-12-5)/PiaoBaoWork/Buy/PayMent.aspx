<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PayMent.aspx.cs" Inherits="Buy_PayMent" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>订单支付</title>
    <link href="../css/List.css" rel="stylesheet" type="text/css" />
    <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../Css/JPstep.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #PassengerDiv
        {
            border: 1px solid #FFC674;
            padding: 15px;
            margin-bottom: 10px;
        }
        #PassengerDiv .PrototypeDiv
        {
            border: 1px solid #FFC674;
            width: 99%;
            margin: 5px 0;
        }
        .Passengertab
        {
        }
        .Passengertab td
        {
            height: 30px;
            text-align: center;
        }
        #PriceDiv
        {
            width: 100%;
            height: 25px;
            border: 1px solid #FF9966;
            background-color: #FFFFCC;
            text-align: center;
            line-height: 25px;
            vertical-align: middle;
        }
        .DivDistribution
        {
            border: 1px solid #FFC674;
            padding: 15px;
            margin-bottom: 10px;
            background-color: White;
        }
        #tabDistribution
        {
            width: 99%;
            display: none;
            margin-top: 5px;
            line-height: 30px;
        }
        .normal-content h5
        {
            font-size: 14px;
            color: #606060;
            line-height: 30px;
            text-align: left;
        }
        .normal-content h5 b
        {
            color: #909090;
            font-size: 12px;
        }
        .flighttab tr
        {
            height: 30px;
            line-height: 30px;
        }
        .options
        {
            height: 20px;
            width: 100%;
            overflow: hidden;
        }
        .options li
        {
            float: left;
            float: left;
            margin-right: 10px;
        }
        .ui-widget-header
        {
            height: 30px;
            line-height: 30px;
        }
        #btnAdd, #btnFind
        {
            border: 1px solid #FF9D4E;
            background: #FF9D4E url(../../CSS/smoothness/images/ui-bg_glass_75_3b97d6_1x400.png) 50% 50% repeat-x;
            font-weight: normal;
            color: white;
            -moz-border-radius: 4px;
            -webkit-border-radius: 4px;
            border-radius: 4px;
            padding: 2px 6px;
            text-decoration: none;
            position: relative;
        }
        .table_info_open
        {
            background-color: White;
            border-top: 1px dashed #E6E6E6;
        }
        .table_info_open th
        {
            width: 80px;
            text-align: right;
        }
        .table_info_open td
        {
            text-align: left;
            padding-left: 10px;
        }
        .MoreConditionA
        {
            background: url("../../img/ArrowDown2.gif") no-repeat scroll right center transparent;
            float: left;
            height: 30px;
            line-height: 30px;
            padding: 0 15px 0 7px;
        }
        .MoreCondition
        {
            background: url("../../img/ArrowUp2.gif") no-repeat scroll right center transparent;
            float: left;
            height: 30px;
            line-height: 30px;
            padding: 0 15px 0 7px;
        }
        .btn-ok-s
        {
            padding: 1px;
            float: left;
            margin-right: 3px;
        }
        .btn-ok-s input
        {
            background: url("../../img/bg-btn.png") no-repeat scroll 0px -75px;
            cursor: pointer;
            font-family: Tahoma;
            outline: none;
            color: White;
            padding: 3px 5px;
            border: none;
            border: 0;
            background-repeat: no-repeat;
        }
        
        #divMorePayment5
        {
            width: 100%;
            overflow: hidden;
        }
        #divMorePayment5 li
        {
            display: block;
            float: left;
            margin-right: 5px;
        }
        #divMorePayment5 a
        {
            border: 1px solid rgb(255, 255, 255);
        }
        #divMorePayment5 a:hover
        {
            border: 1px solid #d1ccd0;
        }
        
        
        #divMorePaymentd6
        {
            width: 100%;
            overflow: hidden;
        }
        #divMorePayment6 li
        {
            display: block;
            float: left;
            margin-right: 5px;
        }
        #divMorePayment6 a
        {
            border: 1px solid rgb(255, 255, 255);
        }
        #divMorePayment6 a:hover
        {
            border: 1px solid #d1ccd0;
        }
        #divPayType input
        {
            margin: 0 0 0 10px;
        }
        #divPayType label
        {
            margin: 0 0 0 2px;
        }
        #divPayType img
        {
            width: 107px;
            height: 44px;
        }
        .hj td
        {
            background: #ffffff;
        }
        #divMorePayment40
        {
            border:1px solid #dddddd;
            padding:5px;  
        }
         #r_bank_type40 label
         {
             width:110px;
             display:inline-block;
          }
    </style>
    <script type="text/javascript">

        function showdialogOne(t) {
            $("#showOne").html(t);
            $("#showOne").dialog({
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

                        if(t.indexOf("成功")!= -1) {
                            window.location.href="../Order/OrderList.aspx?currentuserid="+$("#currentuserid").val();
                        } else {
                            $("#txtAccountPayPwd").val("");
                        }
                    }
                }
            });
        }


        function showdialogTwo(t) {
            $("#showDiv").html(t);
            $("#showDiv").dialog({
                title: '温馨提示',
                bgiframe: true,
                height: 220,
                width: 350,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                }
            });
        }

        function showloading(t) {
            $("#showDiv").html(t);
            $("#showDiv").dialog({
                title: '温馨提示',
                bgiframe: true,
                height: 220,
                width: 350,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                }
            });
        }

        function btnClose() {
            //重新支付
            $("#showDiv").dialog("close");
            GoPay();  //重新支付
        }
        function btnOk() {
            //支付成功
            window.location.href="../Order/OperationingOrderList.aspx?currentuserid="+$("#currentuserid").val();
        }

        //去支付
        function GoPay() {
            try {
                var payWay=$("#Hid_payWay").val();
                var id=$("#Hid_id").val();
                var Code="";

                if(payWay!=undefined&&payWay!=""&&payWay!="0") {

                    if(payWay=="14"||payWay=="15") {
                        //document.getElementById("btnPay").click(); //线下支付
                        var vals="";
                        var accountPayPwd=$("#txtAccountPayPwd").val();
                        if(accountPayPwd==null||accountPayPwd==undefined)
                            accountPayPwd="";

                        if(payWay=="14"&&(accountPayPwd==undefined||accountPayPwd==null||accountPayPwd=="")) {
                            vals="请输入账户支付密码！";
                        }

                        if(vals=="") {
                            showloading(document.getElementById("divLoading").innerHTML); //本地支付弹出提示层
                            var url="PayMent.aspx";
                            var param={
                                AccountPayPwd: accountPayPwd,
                                Hidid: id,
                                HidpayWay: payWay,
                                Method: "AjacPay",
                                num: Math.random(),
                                currentuserid: $("#currentuserid").val()
                            };
                            $.post(url,param,fn_Hand,"text");
                        } else {
                            showdialogOne(vals);
                        }
                    }
                    else {
                        showdialogTwo(document.getElementById("divShowSel").innerHTML); //在线支付弹出提示层
                        if(payWay=="5"||payWay=="6"||payWay=="7"||payWay=="8" || payWay=="40") {
                            var name="r_bank_type"+payWay;
                            var eless=document.getElementsByName(name);
                            for(var i=0;i<eless.length;i++) {
                                if(eless[i].checked) {
                                    Code=eless[i].value;
                                    break;
                                }
                            }
                        }

                        var param="type=0&id="+id+"&payway="+payWay+"&code="+Code+"&price=0&rand="+Math.random()+"&sdate="+Date().toString();
                        var url = "../Pay/Pay.aspx?param=" + escape(param) + "&currentuserid=" + $("#currentuserid").val();
                        window.open(url);
                    }
                } else {
                    showdialogOne("请选择支付方式!");
                }
            } catch(e) {
                showdialogOne("支付失败!");
            }
        }

        function fn_Hand(data) {
            $("#showDiv").dialog("close");
            showdialogOne(data); //显示支付返回结果
        }

        //验证密码 与回车
        function vateData() {
            //            try {
            //                if ($("#HidValue").val() == "1" || $("#HidValue").val() == "6") {
            //                    //if ($("#hidIsOpenPayPassword").val() == "1") {
            //                    if ($("#divXNPay")[0].style.display == "block" || $("#divXNPay")[0].style.display == "") {
            //                        if ($("#txtPayPwd").val() != '') {
            //                            return true
            //                        } else {
            //                            if (document.getElementById("hidPayPassword").value == "") {
            //                                alert("请设置支付密码！");
            //                            }
            //                            else {
            //                                alert("请输入支付密码！");
            //                            }
            //                            return false
            //                        }
            //                    }
            //                    //}
            //                }
            //            } catch (e) {
            //                //alert("支付失败!");
            //                showdialogOne("该订单无法支付!");
            //                return false
            //            }
            //            return true
        }
        function keyEnter(v,evt) {
            //            var fg = true;
            //            try {
            //                if (evt.keyCode == 13) {
            //                    if (v == '') {
            //                        fg = false;
            //                        return fg;
            //                    }
            //                    document.getElementById("btnPay").click();
            //                    fg = false;
            //                }
            //            } catch (e) {
            //                //alert("支付失败!");
            //                showdialogOne("支付失败!");
            //                fg = false;
            //            }
            //            return fg;
        }
        //选择支付方式
        function ChangePayType() {
            try {

                // 隐藏 state
                var tempMorePayment="";
                for(var i=1;i<=40;i++) {
                    tempMorePayment=$("#"+"divMorePayment"+i);
                    if(tempMorePayment!=null&&tempMorePayment!=undefined)
                        tempMorePayment.hide();

                    //$("#" + "divMorePayment" + i).hide();
                }
                $("#divPayTypeImg").hide();

                // 隐藏 end

                var val="";
                var radListItems=document.getElementsByName("rblPayType");
                for(var i=0;i<radListItems.length;i++) {
                    if(radListItems[i].type!=null&&radListItems[i].checked) {
                        val=radListItems[i].value;
                        break;
                    }
                }

                $("#Hid_payWay").val(val);

                var imgUrl="";
                var altVal="";
                if(val==0) {
                    return;
                }
                if(val=="1"||val=="2"||val=="3"||val=="4") {

                    if (val == "1") {
                        imgUrl = "zhifubaologo.png";
                        altVal = "支付宝";

                    }
                    else if (val == "2") {
                        imgUrl = "Kuaiqianlogo.png";
                        altVal = "快钱";

                    }
                    else if (val == "3") {
                        imgUrl = "HuiFulogo.png";
                        altVal = "汇付";
                    }
                    else if (val == "4") {
                        imgUrl = "CaiFuTongLogo.png";
                        altVal = "财付通";
                    }
                    else if (val == "40") {
                        altVal = "信用卡大额支付";
                    }
                    imgUrl="../img/"+imgUrl;
                    $("#divPayTypeImg").show(); //显示图片
                    $("#imgUrl").attr("src",imgUrl); //图片路径
                    $("#imgUrl").attr("alt",altVal); //提示
                }
                else if(val=="5"||val=="6"||val=="7"||val=="8" || val=="40") {
                    $("#divMorePayment"+val).show();  //更多银行   //快钱网银
                } else if(val=="14") {
                    $("#divMorePayment"+val).show();  //显示支付密码
                }
                else if(val=="15") {

                }
            }
            catch(e) {
                //alert("数据加载错误!");
                showdialogOne("数据加载错误!");
            }
        }
        function btnGoPay_onclick() {

        }

        //加载
        function selectload() {
            try {

                //                //是否显示网银
                //                if ($("#Hid_payType").val() != "0") {
                //                    var divId = divMorePayment + $("#Hid_payType").val();
                //                    $("#" + divId).show();
                //                }

                var IsPolicy=$("#Hid_IsPolicy").val();
                if(IsPolicy=="0") {//显示
                    $("#skyFF tr td[showpolicy]").show();
                } else if(IsPolicy=="1") {//隐藏
                    $("#skyFF tr td[showpolicy]").hide();
                }
                ChangePayType();
            }
            catch(e) {

            }
        }
    </script>
</head>
<body onload="selectload()">
    <div id="showOne">
    </div>
    <div id="showDiv">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="main-normal">
        <div class="listing">
            <div style="padding: 10px; background-color: #fffaef" class="normal-content">
                <div class="titleheight">
                    <h3>
                        订单付款&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;订单号：<asp:Label ID="lblOrderId" runat="server"></asp:Label><font></font></h3>
                    <ul>
                        <li><em class="current"></em>预订</li>
                        <li><em class="current"></em>填写订单</li>
                        <li><em class="current"></em>确认</li>
                        <li><em class="current"></em>支付</li>
                        <li><em></em>完成</li>
                    </ul>
                </div>
                <div>
                    <div style="background-color: #fff3d9" class="DivDistribution">
                        <table style="width: 100%; text-align: center;" class="flighttab">
                            <tbody>
                                <tr style="height: 30px; line-height: 30px;">
                                    <td align="left" colspan="10">
                                        <h5 style="float: left">
                                            航班信息</h5>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="10">
                                        <asp:Repeater ID="repTicketSkyWay" runat="server">
                                            <HeaderTemplate>
                                                <table style="width: 100%; text-align: center;" class="flighttab" id="skyFF">
                                                    <tr class="info-head">
                                                        <td>
                                                            航程
                                                        </td>
                                                        <td style="width: 12%">
                                                            起飞日期
                                                        </td>
                                                        <td style="width: 12%">
                                                            起飞时间
                                                        </td>
                                                        <td style="width: 12%">
                                                            到达时间
                                                        </td>
                                                        <td style="width: 8%">
                                                            承运人
                                                        </td>
                                                        <td style="width: 10%">
                                                            航班号
                                                        </td>
                                                        <td style="width: 8%">
                                                            舱位
                                                        </td>
                                                        <td style="width: 8%">
                                                            机型
                                                        </td>
                                                        <td style="width: 5%">
                                                            编码
                                                        </td>
                                                        <td style="width: 10%" showpolicy="1">
                                                            政策
                                                        </td>
                                                    </tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr style="line-height: 30px;" class="hj">
                                                    <td>
                                                        <%#Eval("FromCityName") + "(" + Eval("FromCityCode") + ")-" + Eval("ToCityName") + "(" + Eval("ToCityCode") + ")"%>
                                                    </td>
                                                    <td>
                                                        <%#SkyWayMsg(1, DataBinder.Eval(Container.DataItem,"FromDate", "{0:yyyy-MM-dd}"))%>
                                                    </td>
                                                    <td>
                                                        <%#SkyWayMsg(2, DataBinder.Eval(Container.DataItem, "FromDate", "{0:HH:mm}"))%>
                                                    </td>
                                                    <td>
                                                        <%#SkyWayMsg(3, DataBinder.Eval(Container.DataItem, "ToDate", "{0:HH:mm}"))%>
                                                    </td>
                                                    <td>
                                                        <%#SkyWayMsg(4, Eval("CarryCode").ToString())%>
                                                    </td>
                                                    <td>
                                                        <%#SkyWayMsg(6, Eval("FlightCode").ToString())%>
                                                    </td>
                                                    <td>
                                                        <%#SkyWayMsg(8, Eval("Space").ToString())%>
                                                    </td>
                                                    <td>
                                                        <%#SkyWayMsg(7, Eval("Aircraft").ToString())%>
                                                    </td>
                                                    <td>
                                                        <%=PNR%>
                                                    </td>
                                                    <td showpolicy="1">
                                                        <%=PolicyAndYongJing %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table> </td> </tr>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                        <tr>
                                            <td colspan="10" style="width: 100%;">
                                                <table cellspacing="0" cellpadding="0" border="0" width="100%" class="table_info_open">
                                                    <tr>
                                                        <th valign="top">
                                                            乘机人信息:
                                                        </th>
                                                        <td>
                                                            <asp:Label ID="lblPassenger" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>
                                                            联系人信息:
                                                        </th>
                                                        <td>
                                                            <asp:Label ID="LinkMan" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label
                                                                ID="LinkPhone" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="BodyBox">
                        <div class="ListTit">
                            <b>支付信息</b></div>
                        <div class="Statis">
                            <h2>
                                支付总价格： <span style="color: #FF3300; font-weight: bold;">
                                    <asp:Label ID="lblPay" runat="server"></asp:Label>
                                </span>元 共<asp:Label ID="lblPassengerNum" runat="server"></asp:Label>人
                            </h2>
                            <div id="divShow1" style="margin-top: 10px; padding: 10px; background: none; text-align: left;
                                border-top: 1px dashed #d1d1d1;" runat="server">
                                <div id="divPayType">
                                    <asp:RadioButtonList ID="rblPayType" onclick="ChangePayType()" runat="server" RepeatColumns="10">
                                        <%--  <asp:ListItem Value="0" Text="网银" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="支付宝"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="快钱"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="汇付"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="财付通"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="收银"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="账户余额"></asp:ListItem>--%>
                                    </asp:RadioButtonList>
                                </div>
                                <br />
                                <div id="divMorePayment5" style="display: none;">
                                    <h2>
                                        支付宝网银</h2>
                                    <ul>
                                        <li><a class="bank-logo cmb" title="招商银行">
                                            <input type="radio" value="CMB" name="r_bank_type5" checked="checked" /></a></li>
                                        <li><a class="bank-logo icbc" title="中国工商银行">
                                            <input type="radio" value="ICBCB2C" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo ccb" title="中国建设银行">
                                            <input type="radio" value="CCB" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo spdb" title="上海浦东发展银行">
                                            <input type="radio" value="SPDB" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo abc" title="中国农业银行">
                                            <input type="radio" value="ABC" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo cmbc" title="中国民生银行">
                                            <input type="radio" value="CMBC" name="r_bank_type5" /></a></li>
                                        <%--   <li><a class="bank-logo sdb" title="深圳发展银行">
                                            <input type="radio" value="SDB" name="r_bank_type5" /></a></li>--%>
                                        <li><a class="bank-logo cib" title="兴业银行">
                                            <input type="radio" value="CIB" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo pingan" title="平安银行">
                                            <input type="radio" value="SPABANK" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo boc" title="中国交通银行">
                                            <input type="radio" value="COMM" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo zxyh" title="中信银行">
                                            <input type="radio" value="CITIC" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo bofc" title="中国银行">
                                            <input type="radio" value="BOCB2C" name="r_bank_type5" /></a></li>
                                        <%--<li><a class="bank-logo PSBC" title="中国邮政储蓄银行">
                                            <input type="radio" value="PSBC-DEBIT" name="r_bank_type5" /></a></li>--%>
                                        <%--   <li><a class="bank-logo cebb" title="光大银行">
                                            <input type="radio" value="CEBBANK" name="r_bank_type5" /></a></li>
                                        <li><a class="bank-logo gfyh" title="广东发展银行">
                                            <input type="radio" value="GDB" name="r_bank_type5" /></a></li>--%>
                                        <%--<li><a class="bank-logo GZCB" title="广州市商业银行">
                                            <input type="radio" value="GZCB" name="r_bank_type5" /></a></li>--%>
                                        <%--  <li><a class="bank-logo bob" title="北京银行">
                                            <input type="radio" value="BJBANK" name="r_bank_type5" /></a></li>--%>
                                        <%-- <li><a class="bank-logo njcb" title="南京银行">
                                            <input type="radio" value="NJCB" name="r_bank_type5" /></a></li>--%>
                                        <%--  <li><a class="bank-logo nbcb" title="宁波银行">
                                            <input type="radio" value="NBBANK" name="r_bank_type5" /></a></li>--%>
                                        <%-- <li><a class="bank-logo HXB" title="华夏银行">
                                            <input type="radio" value="HXB" name="r_bank_type5" /></a></li>--%>
                                        <%--   <li><a class="bank-logo BJRCB" title="北京农业商业银行">
                                            <input type="radio" value="BJRCB" name="r_bank_type5" /></a></li>--%>
                                        <%--   <li><a class="bank-logo BEA" title="东亚银行">
                                            <input type="radio" value="BEA" name="r_bank_type5" /></a></li>--%>
                                        <%-- <li><a class="bank-logo CBHB" title="渤海银行">
                                            <input type="radio" value="CBHB" name="r_bank_type5" /></a></li>--%>
                                        <%--  <li><a class="bank-logo POST" title="中国邮政储蓄">
                                            <input type="radio" value="POSTGC" name="r_bank_type5" /></a></li>--%>
                                        <%-- <li><a class="bank-logo SHRCC" title="上海农村商业银行">
                                            <input type="radio" value="SHRCC" name="r_bank_type5" /></a></li>--%>
                                        <%--   <li><a class="bank-logo HZB" title="杭州银行">
                                            <input type="radio" value="HZCBB2B" name="r_bank_type5" /></a></li>--%>
                                        <%--      <li><a class="bank-logo CZB" title="浙商银行">
                                            <input type="radio" value="CZB" name="r_bank_type5" /></a></li>--%>
                                        <%--  <li><a class="bank-logo SHB" title="上海银行">
                                            <input type="radio" value="SHBANK" name="r_bank_type5" /></a></li>--%>
                                        <%--   <li><a class="bank-logo HSB" title="微商银行">
                                            <input type="radio" value="HSB" name="r_bank_type5" /></a></li>--%>
                                    </ul>
                                    <%--    企业网银支付
                                    <ul>
                                        <li><a class="bank-logo abc" title="中国农业银行">
                                            <input type="radio" value="ABCBTB" name="r_bank_type5" /></a> </li>
                                        <li><a class="bank-logo ccb" title="中国建设银行">
                                            <input type="radio" value="CCBBTB" name="r_bank_type5" /></a> </li>
                                        <li><a class="bank-logo icbc" title="中国工商银行">
                                            <input type="radio" value="ICBCBTB" name="r_bank_type5" /></a> </li>
                                        <li></li>
                                        <a class="bank-logo spdb" title="上海浦东发展银行">
                                            <input type="radio" value="SPDBBTB" name="r_bank_type5" /></a>
                                    </ul>--%>
                                </div>
                                <div id="divMorePayment6" style="display: none;">
                                    <h2>
                                        快钱网银</h2>
                                    <ul>
                                        <li><a class="bank-logo cmb" title="招商银行">
                                            <input type="radio" value="CMB" name="r_bank_type6" checked="checked" /></a></li>
                                        <li><a class="bank-logo icbc" title="中国工商银行">
                                            <input type="radio" value="ICBC" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo ccb" title="中国建设银行">
                                            <input type="radio" value="CCB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo spdb" title="上海浦东发展银行">
                                            <input type="radio" value="SPDB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo abc" title="中国农业银行">
                                            <input type="radio" value="ABC" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo cmbc" title="中国民生银行">
                                            <input type="radio" value="CMBC" name="r_bank_type6" /></a></li>
                                        <%--  <li><a class="bank-logo sdb" title="深圳发展银行">
                                            <input type="radio" value="SDB" name="r_bank_type6" /></a></li>--%>
                                        <li><a class="bank-logo cib" title="兴业银行">
                                            <input type="radio" value="CIB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo pingan" title="平安银行">
                                            <input type="radio" value="PAB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo boc" title="中国交通银行">
                                            <input type="radio" value="BCOM" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo zxyh" title="中信银行">
                                            <input type="radio" value="CITIC" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo cebb" title="光大银行">
                                            <input type="radio" value="CEB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo gfyh" title="广东发展银行">
                                            <input type="radio" value="GDB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo GZCB" title="广州市商业银行">
                                            <input type="radio" value="GZCB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo bofc" title="中国银行">
                                            <input type="radio" value="BOC" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo bob" title="北京银行">
                                            <input type="radio" value="BOB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo njcb" title="南京银行">
                                            <input type="radio" value="NJCB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo nbcb" title="宁波银行">
                                            <input type="radio" value="NBCB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo HXB" title="华夏银行">
                                            <input type="radio" value="HXB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo BJRCB" title="北京农业商业银行">
                                            <input type="radio" value="BJRCB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo BEA" title="东亚银行">
                                            <input type="radio" value="BEA" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo CBHB" title="渤海银行">
                                            <input type="radio" value="CBHB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo POST" title="中国邮政储蓄">
                                            <input type="radio" value="POST" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo PSBC" title="中国邮政储蓄银行">
                                            <input type="radio" value="PSBC" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo SHRCC" title="上海农村商业银行">
                                            <input type="radio" value="SHRCC" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo HZB" title="杭州银行">
                                            <input type="radio" value="HZB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo CZB" title="浙商银行">
                                            <input type="radio" value="CZB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo SHB" title="上海银行">
                                            <input type="radio" value="SHB" name="r_bank_type6" /></a></li>
                                        <li><a class="bank-logo HSB" title="微商银行">
                                            <input type="radio" value="HSB" name="r_bank_type6" /></a></li>
                                    </ul>
                                </div>
                                <div id="divMorePayment7" style="display: none;">
                                    <h2>
                                        汇付网银</h2>
                                    <ul>
                                        <li><a class="bank-logo cmb" title="招商银行">
                                            <input type="radio" value="CMB" name="r_bank_type7" checked="checked" /></a></li>
                                    </ul>
                                </div>
                                <div id="divMorePayment8" style="display: none;">
                                    <h2>
                                        财付通网银</h2>
                                    <ul>
                                        <li><a class="bank-logo cmb" title="招商银行">
                                            <input type="radio" value="CMB" name="r_bank_type8" checked="checked" /></a></li>
                                    </ul>
                                </div>
                                <div id="divMorePayment14" style="display: none;">
                                    <h2>
                                        账户余额支付</h2>
                                    <table style="width: 100%;">
                                        <tr runat="server" id="trbtnPwdUrl" visible="true">
                                            <td style="width: 100%; text-align: center;">
                                                <asp:LinkButton ID="lbtnPwd" runat="server">未设置账户支付密码,请先设置账户支付密码！</asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr runat="server" id="trbtnPwd" visible="false">
                                            <td style="width: 100%; text-align: center;">
                                                账户支付密码：<%--<asp:TextBox ID="txtAccountPayPwd" runat="server"></asp:TextBox>--%>
                                                <input type="password" id="txtAccountPayPwd" value="" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div id="divMorePayment40">
                                    <asp:RadioButtonList ID="r_bank_type40" runat="server" ClientIDMode="Static" RepeatColumns="6" RepeatDirection="Horizontal" RepeatLayout="Flow" DataValueField="Code" DataTextField="Name">
                                    </asp:RadioButtonList>
                                </div>
                                <div id="divPayTypeImg" style="width: 200px; margin: auto; line-height: 30px; display: none;">
                                    <img src="" id="imgUrl" alt="" onclick="GoPay()" />
                                </div>
                            </div>
                        </div>
                        <div style="width: 200px; overflow: hidden; text-align: center; margin: auto;">
                            <span class="btn btn-ok-s" runat="server" id="spanBtnPay" visible="false" style="margin: 2px 0 10px 0">
                                <input type="button" value="去支付" runat="server" id="btnGoPay" onclick="GoPay()" /></span>
                            <span id="spanNoPay" runat="server" visible="true" style="color: Red; display: block;
                                font-size: 20px"><b>该订单无法支付</b></span>
                        </div>
                        <%--<asp:Button runat="server" Text="去支付" ID="btnPay" Style="display: none;" title="2"
                            OnClientClick="return vateData();" OnClick="btnPay_Click" />--%>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="divShowSel" style="display: none;">
        <table style="width: 100%; height: 98%;" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td height="40px" align="right">
                    <img src="../img/paysucBG.jpg" />
                </td>
                <td style="font-size: 14px; font-weight: bold; color: #323232">
                    请您在新开的支付网站页面上完成付款！
                </td>
            </tr>
            <tr>
                <td height="40px;" colspan="2" align="center">
                    完成付款后请根据您的付款情况点击下方按钮
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="center">
                                <span class="btn btn-ok-s">
                                    <input id="btnNo" type="button" value="重新支付" onclick="btnClose()" /></span><span
                                        class="btn btn-ok-s"><input id="btnUpdate" type="button" value="支付成功" onclick="btnOk()" /></span>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div id="divLoading" style="display: none;">
        <table style="width: 100%; height: 98%;" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    请稍等，正在处理支付……
                </td>
            </tr>
            <tr>
                <td align="center">
                    <img src="../img/loading.gif">
                </td>
            </tr>
        </table>
    </div>
    <%-- 订单id--%>
    <input type="hidden" runat="server" id="Hid_id" value="0" />
    <%--支付方式--%>
    <input type="hidden" runat="server" id="Hid_payWay" value="0" />
    <%-- 网银类型--%>
    <input type="hidden" runat="server" id="Hid_payType" value="0" />
    <%--是否隐藏政策返点  0 默认显示，1 隐藏 --%>
    <input id="Hid_IsPolicy" type="hidden" runat="server" value="0" />
    </form>
</body>
</html>
