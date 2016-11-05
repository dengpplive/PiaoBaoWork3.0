<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Payment.aspx.cs" Inherits="SMS_Payment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>订单支付</title>
    <link href="../css/List.css" rel="stylesheet" type="text/css" />
    <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../Css/JPstep.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
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
            background: #FF9D4E url(../CSS/smoothness/images/ui-bg_glass_75_3b97d6_1x400.png) 50% 50% repeat-x;
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
            background: url("../img/ArrowDown2.gif") no-repeat scroll right center transparent;
            float: left;
            height: 30px;
            line-height: 30px;
            padding: 0 15px 0 7px;
        }
        .MoreCondition
        {
            background: url("../img/ArrowUp2.gif") no-repeat scroll right center transparent;
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
            border: 0px solid #D74C00;
        }
        .btn-ok-s input
        {
            background: url("../../img/bg-btn.png") repeat-x scroll 0px -75px;
            cursor: pointer;
            font-family: Tahoma;
            outline: none;
            color: White;
            padding: 3px 5px;
            border: none;
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
        
        .info-head th{ color:#333}
    </style>
    <script type="text/javascript">
        function moreSearchOrder() {
            if ($("#moreSearchOrder").css("display") == "none") {
                $("#moreSearchOrderA").attr("class", "MoreCondition");
            }
            else {
                $("#moreSearchOrderA").attr("class", "MoreConditionA");
            }
            $("#moreSearchOrder").toggle('slow');
        }
        function morePaymentMethod() {
            if ($("#morePayment").css("display") == "none") {
                $("#morePaymentA").attr("class", "MoreCondition");
            }
            else {
                $("#morePaymentA").attr("class", "MoreConditionA");
            }
            $("#morePayment").toggle('slow');
        }
    </script>
    <style type="text/css">
        body {font-family: 微软雅黑,tahoma,arial,sans-serif;font-size: 12px;margin: 0; padding:0px;}
        h1,h2,h3,h4,h5,h6,h7{ margin:0px; padding:0px;}
        #PassengerDiv
        {
            border: 1px solid #A6C8E4;
            padding: 8px;
            margin-bottom: 10px;
        }
        #PassengerDiv .PrototypeDiv
        {
            border: 1px solid #A6C8E4;
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
            border: 1px solid #A6C8E4;
            background-color: #FFFFCC;
            text-align: center;
            line-height: 25px;
            vertical-align: middle;
        }
        .DivDistribution
        {
            border: 1px solid #A6C8E4;
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
            border: 1px solid #00a2ff;
            background: #FF9D4E url(../CSS/smoothness/images/ui-bg_glass_75_3b97d6_1x400.png) 50% 50% repeat-x;
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
            background: url("../img/ArrowDown2.gif") no-repeat scroll right center transparent;
            float: left;
            height: 30px;
            line-height: 30px;
            padding: 0 15px 0 7px;
        }
        .MoreCondition
        {
            background: url("../img/ArrowUp2.gif") no-repeat scroll right center transparent;
            float: left;
            height: 30px;
            line-height: 30px;
            padding: 0 15px 0 7px;
        }
        .info-head { background-color:#1A8EA6;color: White;height: 30px;line-height: 30px;}
        .BodyBox {border: 1px solid #DFDFDF;}
        .ListTit {background: url("../img/TitBg1.gif") repeat-x scroll 0 0 transparent;height: 30px;line-height: 30px;}
        .ListTit b {display: block;float: left;font-size: 14px;height: 30px;padding-left: 25px;}
        .Statis {background-color: #FFFFFF; padding: 0 10px;}
        .Statis h2 {font-size: 14px; height: 27px;line-height: 27px;text-align: left;}
        #divMorePaymentZfb ul{ height:120px; background:#ffffff; margin:10px 0; padding:10px 0 0 0; border:1px #ddd solid; }
        #divMorePaymentZfb ul li{ float:left; width:165px}
    </style>
    <script type="text/javascript">
        $(function () {
            // Tabs
            $('#tabs').tabs();
        });
        function showdialog(t) {
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
                        window.location.href = "../Order/OperationingOrderList.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
                    }
                }
            });
        }
       
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
                    }
                }
            });
        }
        function Pay() {
            var One = document.getElementById("hiOrderId").value;
            if (document.getElementById("KQPay").style.display == "block") {
                window.open('../../Pay/Pay.aspx?Id=' + One + "&Type=1&n=" + Math.random()+"&currentuserid=<%=this.mUser.id.ToString() %>");
            }
            else if (document.getElementById("ZFBPay").style.display == "block") {
                window.open('../../Pay/Pay.aspx?Id=' + One + "&Type=2&n=" + Math.random()+"&currentuserid=<%=this.mUser.id.ToString() %>");
            }
            return false;
        }
        function ChangeZFB() {
            try {
                document.getElementById("ZFBPay").style.display = "block";
                document.getElementById("spanKQPay").style.display = "block";
                document.getElementById("KQPay").style.display = "none";
            }
            catch (e)
            { }
        }
        function ChangeKQ() {
            try {
                document.getElementById("spanKQPay").style.display = "block";
                document.getElementById("KQPay").style.display = "block";
                document.getElementById("ZFBPay").style.display = "none";
            }
            catch (e)
            { }
        }

        function selectload() {
            var eless = document.getElementsByName("payway");
            if (eless.length > 0) {
                eless[0].checked = true;
                if (eless[0].value == "2") {
                    ChangeZFB();
                }
                if (eless[0].value == "1") {
                    ChangeKQ();
                }
            }
        }
        function showdialogmsg(t) {
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
                    '是': function () {
                        return true;
                    },
                    '否': function () {
                        return false;
                    }
                }
            });
        }
    </script>
     <script type="text/javascript">
         var listName = "rblPayType";

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

         function showdialog2(t) {
             $("#dd").html(t);
             $("#dd").dialog({
                 title: '充值',
                 bgiframe: true,
                 modal: true,
                 overlay: {
                     backgroundColor: '#000',
                     opacity: 0.5
                 }
             });
         }

         function showdialog3(t) {
             $("#dd").html(t);
             $("#dd").dialog({
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

         function confirmPrice(id) {
             try {
                 var msg = "";
                 var price = document.getElementById("lblPay").innerText;
               
                 var payWay = $("#hidPayWay").val(); //支付方式
                 var payCodeType = ""; //网银类型
                 var code = ""; //网银银行代码

                 if (payWay == "" || payWay == "0")
                     msg = "请选择充值方式！";
                 else if (Number(payWay) > 4) {
                     var accountPayPwd = $("#txtAccountPayPwd").val();
                     if (accountPayPwd == null || accountPayPwd == undefined)
                         accountPayPwd = "";

                     if (payWay == "14" && (accountPayPwd == undefined || accountPayPwd == null || accountPayPwd == "")) {
                         msg = "请输入账户支付密码！";
                     }

                     payCodeType = $("#hidWangYingType").val();

                     var name = "";
                     if (payWay == "5")
                         name = "r_bank_typeZfb";
                     else if (payWay == "6")
                         name = "r_bank_type";
                     else if (payWay == "7")
                         name = "";
                     else if (payWay == "8")
                         name = "";

                     var eless = document.getElementsByName(name);
                     for (var i = 0; i < eless.length; i++) {
                         if (eless[i].checked) {
                             code = eless[i].value;
                             break;
                         }
                     }
                 }
                 if (msg == "") {
                     var divValueNew = document.getElementById("divShowSel").innerHTML;
                     showdialog3(divValueNew); //支付弹出提示层
                     var param = "type=2&price=" + price + "&payway=" + payWay + "&code=" + code + "&rand=" + Math.random()+"&currentuserid=<%=this.mUser.id.ToString() %>";
                     $("#hid_url").val(param);
                     jQuery("#btnPay").click();

                  
                 } else {
                     showdialog(msg);
                 }
             } catch (e) {
                 showdialog("支付异常!");
             }
         }

         function openUrl(id) {
             var param = $("#hid_url").val() + "&id=" + id+"&currentuserid=<%=this.mUser.id.ToString() %>";

             var url = "../Pay/Pay.aspx?param=" + escape(param)+"&currentuserid=<%=this.mUser.id.ToString() %>"; 

             window.open(url, "_blank");
         }


         function btnSel() {
             var s = "";
             var radListItems = document.getElementsByName(listName);
             for (var i = 0; i < radListItems.length; i++) {
                 if (radListItems[i].type != null && radListItems[i].checked) {
                     s = radListItems[i].value;
                     break;
                 }
             }
             $("#hidPayWay").val(s);

             if (Number(s) > 4) {
                 if (Number(s)==14) {
                     $("#divMorePayment14").show();
                     $("#divMorePaymentZfb").hide();
                     $("#divMorePayment").hide();
                 } else {
                     fromLoad('show');
                 }

             } else {
                 $("#divMorePayment14").hide();
                 fromLoad('hide');
             }
         }
         function fromLoad(obj) {
             var wangYingType = $("#hidWangYingType").val();

             if (obj == "show") {
                 if (wangYingType == "5")
                     $("#divMorePaymentZfb").show();
                 else if (wangYingType == "6")
                     $("#divMorePayment").show();
                 else if (wangYingType == "7")
                     $("#divMorePayment").show();
                 else if (wangYingType == "8")
                     $("#divMorePayment").show();
             } else if (obj == "hide") {
                 if (wangYingType == "5")
                     $("#divMorePaymentZfb").hide();
                 else if (wangYingType == "6")
                     $("#divMorePayment").hide();
                 else if (wangYingType == "7")
                     $("#divMorePayment").hide();
                 else if (wangYingType == "8")
                     $("#divMorePayment").hide();
             }
         }

         function btnClose() {
             //重新支付
             $("#dd").dialog("close");
         }

         function btnOk() {
             //支付成功
         }
     </script>
</head>
<body onload="selectload();btnSel()">
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="main-normal" style=" background:#ffffff">

        <div class="listing">
            <div style="padding: 10px; background-color: #f1f8ff" class="normal-content">
                <div>
                    <div style="background-color: #dbeeff" class="DivDistribution">
                        <table style="width: 100%; text-align: center;" class="flighttab">
                            <tbody>
                                <tr style="height: 30px; line-height: 30px;">
                                    <td align="left" colspan="12">
                                        <h5 style="float: left">
                                            购买短信信息</h5>
                                    </td>
                                </tr>
                                <tr class="info-head">
                                    <th style="width: 10%;">
                                        金额
                                    </th>
                                    <th style="width: 10%;">
                                        条数
                                    </th>
                                    <th style="width: 10%;">
                                        单价
                                    </th>
                                    <th style="width: 20%;">
                                        备注
                                    </th>
                                </tr>
                                <tr style="line-height: 30px;">
                                    <td>
                                        <%--金额--%><%=SmsRatesMoney%>元
                                    </td>
                                    <td>
                                        <%--条数--%><%=SmsRatesCount%>条
                                    </td>
                                    <td>
                                        <%--单价--%><%=SmsRatesUnitPrice%>元
                                    </td>
                                    <td>
                                        <%--备注--%><%=SmsRatesRemark%>
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
                                总价：<span style="color: #FF3300; font-weight: bold;"><asp:Label ID="lblPay" runat="server"></asp:Label></span>元
                            </h2>
                            <div style="margin-top: 10px; padding: 10px; background: none; text-align: left;
                                border-top: 1px dashed #d1d1d1;" runat="server" id="divShow1">
                                <asp:RadioButtonList ID="rblPayType" runat="server" RepeatColumns="6" onclick="btnSel()" >
                                </asp:RadioButtonList><%----%>
                            </div>
                        </div>
                    </div>
                    <br />
                   
                     <div id="divMorePayment" style="display: none;">
                        <h2>
                            快钱网银</h2>
                        <ul>
                            <li><a class="bank-logo cmb" title="招商银行">
                                <input type="radio" value="CMB" name="r_bank_type" checked="checked" /></a></li>
                            <li><a class="bank-logo icbc" title="中国工商银行">
                                <input type="radio" value="ICBC" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo ccb" title="中国建设银行">
                                <input type="radio" value="CCB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo spdb" title="上海浦东发展银行">
                                <input type="radio" value="SPDB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo abc" title="中国农业银行">
                                <input type="radio" value="ABC" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo cmbc" title="中国民生银行">
                                <input type="radio" value="CMBC" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo sdb" title="深圳发展银行">
                                <input type="radio" value="SDB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo cib" title="兴业银行">
                                <input type="radio" value="CIB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo pingan" title="平安银行">
                                <input type="radio" value="PAB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo boc" title="中国交通银行">
                                <input type="radio" value="BCOM" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo zxyh" title="中信银行">
                                <input type="radio" value="CITIC" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo cebb" title="光大银行">
                                <input type="radio" value="CEB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo GZCB" title="广州市商业银行">
                                <input type="radio" value="GZCB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo bofc" title="中国银行">
                                <input type="radio" value="BOC" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo bob" title="北京银行">
                                <input type="radio" value="BOB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo njcb" title="南京银行">
                                <input type="radio" value="NJCB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo nbcb" title="宁波银行">
                                <input type="radio" value="NBCB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo HXB" title="华夏银行">
                                <input type="radio" value="HXB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo BJRCB" title="北京农业商业银行">
                                <input type="radio" value="BJRCB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo BEA" title="东亚银行">
                                <input type="radio" value="BEA" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo CBHB" title="渤海银行">
                                <input type="radio" value="CBHB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo POST" title="中国邮政储蓄">
                                <input type="radio" value="POST" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo PSBC" title="中国邮政储蓄银行">
                                <input type="radio" value="PSBC" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo SHRCC" title="上海农村商业银行">
                                <input type="radio" value="SHRCC" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo HZB" title="杭州银行">
                                <input type="radio" value="HZB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo CZB" title="浙商银行">
                                <input type="radio" value="CZB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo SHB" title="上海银行">
                                <input type="radio" value="SHB" name="r_bank_type" /></a></li>
                            <li><a class="bank-logo HSB" title="微商银行">
                                <input type="radio" value="HSB" name="r_bank_type" /></a></li>
                        </ul>
                    </div>
                    <div id="divMorePaymentZfb" style="display: none;">
                        <h2 style=" font-size:16px">
                            支付宝网银</h2>
                        <ul>
                            <li><a class="bank-logo cmb" title="招商银行">
                                <input type="radio" value="CMB" name="r_bank_typeZfb" checked="checked" /></a></li>
                            <li><a class="bank-logo icbc" title="中国工商银行">
                                <input type="radio" value="ICBCB2C" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo ccb" title="中国建设银行">
                                <input type="radio" value="CCB" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo spdb" title="上海浦东发展银行">
                                <input type="radio" value="SPDB" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo abc" title="中国农业银行">
                                <input type="radio" value="ABC" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo cmbc" title="中国民生银行">
                                <input type="radio" value="CMBC" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo sdb" title="深圳发展银行">
                                <input type="radio" value="SDB" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo cib" title="兴业银行">
                                <input type="radio" value="CIB" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo pingan" title="平安银行">
                                <input type="radio" value="SPABANK" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo boc" title="中国交通银行">
                                <input type="radio" value="COMM" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo zxyh" title="中信银行">
                                <input type="radio" value="CITIC" name="r_bank_typeZfb" /></a></li>
                            <li><a class="bank-logo bofc" title="中国银行">
                                <input type="radio" value="BOCB2C" name="r_bank_typeZfb" /></a></li>
                            <li style=" display:none"><a class="bank-logo PSBC" title="中国邮政储蓄银行">
                                <input type="radio" value="PSBC-DEBIT" name="r_bank_typeZfb" /></a></li>
                           
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
                                                账户支付密码：<asp:TextBox ID="txtAccountPayPwd" runat="server" TextMode="Password"></asp:TextBox>
                                                <%--<input type="password" id="txtAccountPayPwd" value="" />--%>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                     <table width="240px" align="center">
                        <tbody>
                            <tr>
                                <td align="center">
                                <span id="spanid" class="btn btn-ok-s"  runat="server" visible="true" >
                                    <input type="button" id='btnOk' value="确认充值" onclick="confirmPrice('spanid')" class="btn big1 cp"  />
                                </span>
                                <span style="display: none;">
                                   <asp:Button runat="server" ID="btnPay" Text="去支付" CssClass="btn big1 cp" 
                                        onclick="btnPay_Click" />
                                </span>
                                    <span class="btn btn-ok-s" >
                                    <asp:Button runat="server" Text="返回" CssClass="btn big1 cp" ID="btnReturn" OnClick="btnReturn_Click" />
                                    </span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div id="divMorePaymentHF" style="display: none;">
                        <h2>
                            快钱网银</h2>
                    </div>
                    <div id="divMorePaymentCFT" style="display: none;">
                        <h2>
                            财付通网银</h2>
                    </div>
                </div>
            </div>
        </div>
    </div>
      <div id="dd">
        </div>
        <input type="hidden" id='hidPayWay' value="" runat="server" />
        <input type="hidden" id='hidWangYingType' value="0" runat="server" />
    <input type="hidden" runat="server" id="hiOrderId" />
    <div id="showOne" runat="server">
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
                                <span class="btn-ok-s">
                                    <input id="btnNo" type="button" value="重新确认充值" onclick="btnClose()" /></span><span
                                        class="btn-ok-s"><input id="btnUpdate" type="button" value="充值成功" onclick="btnOk()" /></span>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>

    <input type="hidden" id="hid_url" value="" runat="server" />
    </form>
</body>
</html>
