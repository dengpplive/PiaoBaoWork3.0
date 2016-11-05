<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OnlineRepayment.aspx.cs"
    Inherits="Financial_OnlineRepayment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>在线充值还款</title>
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
            border: 1px solid #D74C00;
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
                var price = $("#txtPrice").val();

                if (price == "")
                    msg = "请输入充值金额！";
                else if (isNaN(price))
                    msg = "请正确输入充值金额！"
                else {
                    if (Number(price) < 100)
                        msg = "充值金额不能小于 100 元！";
                }
                var payWay = $("#hidPayWay").val(); //支付方式
                var payCodeType = ""; //网银类型
                var code = ""; //网银银行代码

                if (payWay == "" || payWay == "0")
                    msg = "请选择充值方式！";
                else if (Number(payWay) > 4) {
                    payCodeType = $("#hidWangYingType").val();

                    var name = "";
                    if (payWay == "5")
                        name = "r_bank_type5";
                    else if (payWay == "6")
                        name = "r_bank_type6";
                    else if (payWay == "7")
                        name = "r_bank_type7";
                    else if (payWay == "8")
                        name = "r_bank_type8";
                    else if (payWay == "40")
                        name = "r_bank_type40";
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
                    //$("#" + id).hide();
                    //showdialog("ok");
                    var param = "type=1&id=&price=" + price + "&payway=" + payWay + "&code=" + code + "&rand=" + Math.random();
                    var url = "../Pay/Pay.aspx?param=" + escape(param) + "&currentuserid=<%=this.mUser.id.ToString() %>";

                    window.open(url, "_blank");

                    //$("#hid_url").val(url);
                    //document.getElementById("btnPayOk").onclick();

                    //showdialog2("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='yes' allowtransparency='yes' width='100%' style='margin:0px;padding:0px;' height='100%' src='" + url + "'/>");
                } else {
                    showdialog(msg);
                }
            } catch (e) {
                showdialog("支付异常!");
            }
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
            if (Number(s) == 40) {
                $("#divMorePayment5,#divMorePayment6,#divMorePayment7,#divMorePayment8").hide();
                $("#divMorePayment40").show();
            }
            else if (Number(s) == 5) {
                $("#divMorePayment40,#divMorePayment6,#divMorePayment7,#divMorePayment8").hide();
                $("#divMorePayment5").show();
            } else {
                $("#divMorePayment40,#divMorePayment6,#divMorePayment7,#divMorePayment8,#divMorePayment5").hide();
            }
            /*if (Number(s) > 4) {
                fromLoad('show');
            } else {
                fromLoad('hide');
            }*/
        }
        function fromLoad(obj) {
            var wangYingType = $("#hidWangYingType").val();
            if (obj == "show") {
                if (wangYingType == "5")
                    $("#divMorePayment5").show();
                else if (wangYingType == "6")
                    $("#divMorePayment6").show();
                else if (wangYingType == "7")
                    $("#divMorePayment7").show();
                else if (wangYingType == "8")
                    $("#divMorePayment8").show();
                else if (wangYingType == "40")
                    $("#divMorePayment40").show();
            } else if (obj == "hide") {
                if (wangYingType == "5")
                    $("#divMorePayment5").hide();
                else if (wangYingType == "6")
                    $("#divMorePayment6").hide();
                else if (wangYingType == "7")
                    $("#divMorePayment7").hide();
                else if (wangYingType == "8")
                    $("#divMorePayment8").hide();
                else if (wangYingType == "40")
                    $("#divMorePayment40").hide();
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
<body onload="fromLoad('show')">
    <form id="form2" runat="server">
    <div id="tabs-1">
        <div class="title" style="height: 20px; padding: 3px 0 0 4px;">
            <span>在线充值还款</span>
        </div>
        <div class="c-list-filter">
            <div class="container" style="padding-bottom: 0px;">
                <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                    border="0" style="border-collapse: collapse;">
                    <tr>
                        <td style="text-align: right; width: 40%; font-size: 14px; height: 40px">
                            <span style="font-size:10.5pt;mso-bidi-font-size:11.0pt;
font-family:宋体;mso-ascii-font-family:Calibri;mso-hansi-font-family:Calibri;
mso-bidi-font-family:&quot;Times New Roman&quot;;mso-font-kerning:1.0pt;mso-ansi-language:
EN-US;mso-fareast-language:ZH-CN;mso-bidi-language:AR-SA">充值还款</span>金额 ：
                        </td>
                        <td style="text-align: left; width: 60%">
                            <input type="text" id="txtPrice" value="" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 40%; font-size: 14px; height: 40px;">
                            <span style="font-size:10.5pt;mso-bidi-font-size:11.0pt;
font-family:宋体;mso-ascii-font-family:Calibri;mso-hansi-font-family:Calibri;
mso-bidi-font-family:&quot;Times New Roman&quot;;mso-font-kerning:1.0pt;mso-ansi-language:
EN-US;mso-fareast-language:ZH-CN;mso-bidi-language:AR-SA">充值还款</span>方式 ：
                        </td>
                        <td style="text-align: left;">
                            <asp:RadioButtonList ID="rblPayType" runat="server" RepeatColumns="10" onclick="btnSel()">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
                <div id="divMorePayment5" style="display: none;">
                    <h4>
                        选择银行</h4>
                    <%--支付宝网银--%>
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
                </div>
                <div id="divMorePayment6" style="display: none;">
                    <h4>
                        选择银行
                    </h4>
                    <%--快钱网银--%>
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
                        <li><a class="bank-logo sdb" title="深圳发展银行">
                            <input type="radio" value="SDB" name="r_bank_type6" /></a></li>
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
                    <h4>
                        选择银行</h4>
                    <%--汇付网银--%>
                </div>
                <div id="divMorePayment8" style="display: none;">
                    <h4>
                        选择银行</h4>
                    <%--财付通网银--%>
                </div>
                <div id="divMorePayment40" style="display:none;">
                    <asp:RadioButtonList ID="r_bank_type40" runat="server" ClientIDMode="Static" RepeatColumns="6" RepeatDirection="Horizontal" RepeatLayout="Flow" DataValueField="Code" DataTextField="Name">
                    </asp:RadioButtonList>
                </div>
                <div style="width: 200px; overflow: hidden; text-align: center; margin: auto;">
                    <span id="spanid" class="btn btn-ok-s" runat="server" visible="true" style="border: 0;
                        background-repeat: no-repeat">
                        <input type="button" id='btnOk' value="确认充值" onclick="confirmPrice('spanid')" />
                    </span><span style="display: none;">
                        <asp:Button runat="server" ID="btnPayOk" Text="去支付" OnClick="btnPayOk_Click" />
                    </span>
                </div>
            </div>
        </div>
        <div id="dd">
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
                                        <input id="btnNo" type="button" value="重新确认充值" onclick="btnClose()" /></span><span
                                            class="btn btn-ok-s"><input id="btnUpdate" type="button" value="充值成功" onclick="btnOk()" /></span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <input type="hidden" id='hidPayWay' value="" runat="server" />
        <input type="hidden" id='hidWangYingType' value="0" runat="server" />
        <input type="hidden" id="hid_url" value="" runat="server" />
    </div>
    </form>
</body>
</html>
