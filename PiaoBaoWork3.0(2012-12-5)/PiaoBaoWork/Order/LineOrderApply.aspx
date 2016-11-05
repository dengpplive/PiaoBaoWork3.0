<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LineOrderApply.aspx.cs" Inherits="Order_LineOrderApply" %>

<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>线下订单申请</title>
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
        .tdSelW
        {
            width: 120px;
        }
        .tabx td
        {
            padding: 3px;
        }
        .tabx th
        {
            padding: 3px;
        }
        .TextBorder
        {
            border: 1px solid #999;
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
        #loading
        {
            margin-top: 10px;
            width: 420px;
            border: 0 none;
            text-align: center;
            padding: 40px 30px 40px 30px;
            color: #707070;
            font-size: 18px;
            line-height: 180%;
            position: fixed;
            left: 30%;
            top: 30%;
            z-index: 1000;
            background: url(../images/mainbg.gif);
        }
        #overlay
        {
            background-color: #333333;
            left: 0;
            filter: alpha(opacity=50); /* IE */
            -moz-opacity: 0.5; /* 老版Mozilla */
            -khtml-opacity: 0.5; /* 老版Safari */
            opacity: 0.5;
            position: fixed;
            top: 0;
            width: 100%;
            z-index: 999;
            height: 100%;
        }
        .PNRcls
        {
            width: 600px;
            height: 200px;
            color: green;
            background-color: Black;
            overflow: auto;
            border: 1px solid #999;
        }
        /*自动提示样式*/
        .pTop
        {
            background-color: #5285B9;
            color: White;
        }
        .pButtom
        {
            background-color: Gray;
        }
        .suggec
        {
            background-color: #A9E7BA;
        }
        #suggestions
        {
            border: solid 1px #7F9DB9;
        }
        #suggestions, #suggestions ul
        {
            margin: 0;
            padding: 0;
        }
        #suggestions ul li
        {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 20px;
            line-height: 20px;
            text-indent: 10px;
            cursor: pointer;
            list-style: none;
            border-bottom: solid 1px gray;
        }
    </style>
    <script type="text/javascript" src="../JS/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../JS/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/json2.js"></script>
    <script id="script_k1" src="../js/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <script type="text/javascript">
        var jQueryOne=jQuery.noConflict(false);       
    </script>
    <script src="../js/js_CompareDate.js" type="text/javascript"></script>
    <script src="../js/js_CardValid.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/js_LineOrderApply.js" language="javascript"></script>
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
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="title">
        <asp:Label ID="lblShow" Text="线下订单申请" runat="server" />
    </div>
    <br />
    <div id="tabs-1" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
        <table width="100%" align="center" border="0" cellpadding="0" cellspacing="0" style="border: 1px solid #E6E6E6;">
            <tr>
                <td class="mainl">
                </td>
                <td>
                    <table width="100%" align="center" class="detail" border="0" cellpadding="5" cellspacing="0"
                        style="padding: 5px;">
                        <tr>
                            <td align="left">
                                <table>
                                    <tr>
                                        <td>
                                            <div>
                                                <strong style="color: #FC9331; font-size: 14px">订单生成方式:</strong>
                                                <label for="rdOrderType0">
                                                    <input type="radio" id="rdOrderType0" name="OrderType" value="0" checked="checked"
                                                        onclick="SelPnrType(this)" />手动录入
                                                </label>
                                                <label for="rdOrderType1">
                                                    <input type="radio" id="rdOrderType1" name="OrderType" value="1" onclick="SelPnrType(this)" />PNR导入
                                                </label>
                                                <label for="rdOrderType2">
                                                    <input type="radio" id="rdOrderType2" name="OrderType" value="2" onclick="SelPnrType(this)" />PNR内容导入
                                                </label>
                                                <span id="OrderSpan" class="hide">&nbsp;&nbsp;&nbsp;&nbsp;<font class="red"><b>注: PNR或者PNR内容获取订单数据不全时，请手动补全订单信息后再申请</b></font></span>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="tr_pnr" class="hide">
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            PNR:
                                        </th>
                                        <td align="left">
                                            <input id="txtPNR" type="text" maxlength="6" style="width: 80px;" />
                                        </td>
                                        <td align="left">
                                            <span class="btn btn-ok-s">
                                                <input id="txtGetPnr" type="button" value="PNR获取订单" onclick="ApplayPnr(this)" />
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="tr_PnrCon" class="hide">
                            <td>
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <th align="left">
                                            PNR内容
                                        </th>
                                        <th>
                                            PNR内容标准示例
                                        </th>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <textarea id="txtPNRCon" cols="17" rows="2" maxlength="1000" class="PNRcls"></textarea>
                                        </td>
                                        <td align="left">
                                            <textarea id="txtExplame" runat="server" cols="17" rows="2" maxlength="1000" readonly="true"
                                                class="PNRcls" onselectstart="return false" oncopy="return false"></textarea>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <span class="btn btn-ok-s">
                                                <input id="btnGetPnrCon" type="button" value="PNR内容获取订单" onclick="ApplayPnr(this)" />
                                            </span>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td align="left">
                                            <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 0 0 5px 0;
                                                border: 1px #e2f1f6 solid">
                                                <img align="absmiddle" src="../Images/point.gif" />
                                                <strong style="color: #FC9331;">航程选择</strong>
                                            </div>
                                            <table border="0">
                                                <tr>
                                                    <td>
                                                        <span><b>选择航段:</b></span>
                                                        <select name="selSky" id="selddlSky" class="tdSelW" onchange="setSkyGroup(this.value)">
                                                        </select>
                                                        <label for="cityTypeSel1_0">
                                                            <input type="radio" name="cityType_0" id="cityTypeSel1_0" value="0" checked="checked" />国内城市</label>
                                                        <label for="cityTypeSel2_0">
                                                            <input type="radio" name="cityType_0" id="cityTypeSel2_0" value="1" />国际城市</label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tblsky" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                                width="100%">
                                                <thead>
                                                    <tr id="skyheader">
                                                        <th>
                                                            序号
                                                        </th>
                                                        <th>
                                                            航班号
                                                        </th>
                                                        <th>
                                                            出发城市
                                                        </th>
                                                        <th>
                                                            到达城市
                                                        </th>
                                                        <th>
                                                            出发日期
                                                        </th>
                                                        <th>
                                                            到达日期
                                                        </th>
                                                        <th>
                                                            舱位
                                                        </th>
                                                        <th class="hide">
                                                            操作
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <%--第一航段 --%>
                                                    <tr id="trsky_0">
                                                        <td align="center" style="width: 4%;">
                                                            <span id="spansky_0">1</span>
                                                        </td>
                                                        <td align="center">
                                                            <%--航班号--%><input type="text" id="flight_0" style="width: 50px" maxlength="6" />
                                                        </td>
                                                        <td align="center">
                                                            <input name="ddlFromCity" class="inputtxtdat" type="text" id="ddlFromCity_0" runat="server"
                                                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                                                style="width: 110px;" mod_address_reference="hidfromcode_0" />
                                                            <input type="hidden" id="hidfromcode_0" />
                                                        </td>
                                                        <td align="center">
                                                            <input name="ddlToCity" class="inputtxtdat" type="text" id="ddlToCity_0" runat="server"
                                                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                                                style="width: 110px;" mod_address_reference="hidtocode_0" />
                                                            <input type="hidden" id="hidtocode_0" />
                                                        </td>
                                                        <td align="center">
                                                            <%--开始日期--%>
                                                            <input type="text" name="startdate" readonly="readonly" id="startdate_0" runat="server"
                                                                class="Wdate inputtxtdat" style="width: 130px;" onfocus="WdatePicker({isShowClear:false,minDate:'%y-%M-%d %H:%m',autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" />
                                                        </td>
                                                        <td align="center">
                                                            <%--结束日期--%>
                                                            <input type="text" name="enddate" readonly="readonly" id="enddate_0" runat="server"
                                                                class="Wdate inputtxtdat" style="width: 130px;" onfocus="WdatePicker({isShowClear:false,minDate:'%y-%M-%d %H:%m',autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" />
                                                        </td>
                                                        <td align="center">
                                                            <%--舱位--%>
                                                            <input type="text" id="txtNewSeat_0" style="width: 50px" maxlength="2" />
                                                        </td>
                                                        <td style="width: 5%;" class="hide">
                                                            <div id="skydiv_0">
                                                                <a href="#" onclick="return addSkyGroup(event); ">添加</a>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" height="30" valign="bottom">
                                <div id="tdWay">
                                    <span class="btn btn-ok-s">
                                        <input type="button" value="添加航段" id="btnok" onclick="addSkyGroup(event)" />
                                    </span><span class="btn btn-ok-s">
                                        <input type="button" value="删除航段" id="btnde" onclick="removeSkyGroup(event)" />
                                    </span>
                                    <%--<span class="btn btn-ok-s">
                                        <input type="button" value="查看政策" id="Button1" onclick='javascript:location.href = "GroupOrderList.aspx?type=1"' />
                                    </span>--%>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 5px  0 5px 0;
                                    border: 1px #e2f1f6 solid">
                                    <img align="absmiddle" src="../Images/point.gif" />
                                    <strong style="color: #FC9331;">乘机人信息</strong>
                                </div>
                                <table id="tblPassenger" class="" border="0" cellspacing="0" cellpadding="0" width="100%">
                                    <tr>
                                        <th align="left" style="padding: 3px 0">
                                            乘机人数:
                                            <select name="passengers" id="passengers" class="tdSelW" onchange="setShowGroup(this.value)">
                                            </select>
                                        </th>
                                    </tr>
                                    <tr id="trPassenger">
                                        <td>
                                            <table class="Passengertab else-table" style="width: 100%" id="tab_Pas" border="0"
                                                cellpadding="0" cellspacing="0">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 2%; text-align: center;">
                                                            序号
                                                        </th>
                                                        <th style="width: 13%; text-align: center;">
                                                            乘客姓名
                                                        </th>
                                                        <th style="width: 7%; text-align: center;">
                                                            乘客类型
                                                        </th>
                                                        <th style="width: 10%; text-align: center;">
                                                            证件类型
                                                        </th>
                                                        <th style="width: 19%; text-align: center;">
                                                            证件号
                                                        </th>
                                                        <th style="width: 19%; text-align: center;">
                                                            手机号
                                                        </th>
                                                        <th style="width: 7%; text-align: center;">
                                                            操作
                                                        </th>
                                                    </tr>
                                                    <tr id="tr_Pas_0" class="hide">
                                                        <td>
                                                            <span id="xuhao_0">1</span>
                                                        </td>
                                                        <td>
                                                            <input id="txtPasName_0" type="text" size="15" style="width: 120px;" maxlength="30" />
                                                            <span id="msgPasName_0"><font class='red'>*</font></span>
                                                        </td>
                                                        <td>
                                                            <select id="SelPasType_0" style="width: 100px;" runat="server">
                                                            </select>
                                                        </td>
                                                        <td>
                                                            <select id="SelCardType_0" style="width: 100px;" runat="server">
                                                            </select>
                                                        </td>
                                                        <td>
                                                            <span id="msgCardNum_0">
                                                                <input id="txtCardNum_0" type="text" style="width: 150px;" maxlength="18" size="25" />
                                                            </span><span id="msgBirthday_0" class="hide">
                                                                <input type="text" id="txtBirthday_0" runat="server" style="width: 150px;" readonly="readonly"
                                                                    size="25" class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,maxDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                                                            </span><span id="msgcardid_0"><font class='red'>*</font></span>
                                                        </td>
                                                        <td>
                                                            <input id="txtPhone_0" type="text" size="15" style="width: 100px;" maxlength="11" />
                                                            <span id="spanPhone_0"><font class='red'></font></span>
                                                            <input id="flyerremark_0" type="hidden" />
                                                        </td>
                                                        <td style="width: 12%; text-align: center;">
                                                            <div id="op_div_0">
                                                                <%-- <a href="#" onclick="return addGroup(event); ">添加</a>--%>
                                                                <span id="span_Flyer" class="btn btn-ok-s">
                                                                    <input id="btnFind" type="button" value="选择常旅客" onclick="SelectPassenger(0)" /></span>
                                                                &nbsp;<span class="btn btn-ok-s"><input type="button" value="移除" onclick="return removeGroup('0');" /></span>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" height="30" valign="bottom">
                                <div id="Div1">
                                    <span class="btn btn-ok-s">
                                        <input type="button" value="添加乘客" id="btnAddPas" onclick="return addGroup(event);" />
                                    </span><span class="btn btn-ok-s">
                                        <input type="button" value="删除乘客" id="btnDelPas" onclick="return removeGroup();" />
                                    </span>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 5px 0 5px 0;
                                    border: 1px #e2f1f6 solid">
                                    <img align="absmiddle" src="../Images/point.gif" /><strong style="color: #FC9331;">
                                        联系方式</strong>
                                </div>
                                <table class="else-table tabx" border="0" cellspacing="0" cellpadding="0" width="100%">
                                    <tr>
                                        <th align="right" width="10%" style="border-bottom: 1px #ddd c1c1c1">
                                            <span style="color: Red">*</span>联系人:
                                        </th>
                                        <td align="left">
                                            <input type="text" id="linkName" runat="server" style="width: 120px" maxlength="30" />
                                        </td>
                                        <th align="right">
                                            <span style="color: Red">*</span>联系人手机:
                                        </th>
                                        <td align="left">
                                            <input type="text" id="linkPhone" runat="server" style="width: 120px" maxlength="13" />
                                        </td>
                                        <td class="td" align="left" style="display: none;">
                                            <span style="color: Red">*</span>联系人电话:
                                            <input type="text" id="linkTel" runat="server" style="width: 120px" maxlength="12" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th class="td" align="right">
                                            备注信息:
                                        </th>
                                        <td class="td" align="left" colspan="4">
                                            <asp:TextBox runat="server" ID="linkRemark" Height="48px" TextMode="MultiLine" Width="957px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="height: 30px">
                            <td align="center">
                                <span class="btn btn-ok-s">
                                    <input id="btnConfirmApplay" type="button" value="确定申请" onclick="return Apply(this);" />
                                </span><span class="btn btn-ok-s">
                                    <input type="button" id="btncz" value="重置数据" />
                                </span><span class="btn btn-ok-s">
                                    <input type="button" value=" 返 回 " onclick="javascript:history.go(-1)" />
                                </span>
                                <br />
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="mainr">
                </td>
            </tr>
        </table>
    </div>
    <div id="overlay" runat="server" style="display: none;">
    </div>
    <div id="loading" style="display: none;">
        <span id="span_text">请稍等，正在处理PNR数据<br />
        </span><span id="spastr">……</span><br />
        <img src="../img/loading.gif"></div>
    <%--证件号类型--%>
    <input id="Hid_CardType" type="hidden" runat="server" />
    <%--选择的航空公司--%>
    <input id="Hid_AirCode" type="hidden" runat="server" />
    <%--机型--%>
    <input id="Hid_PlaneType" type="hidden" runat="server" />
    <%--国内基建--%>
    <input id="Hid_innerFare" type="hidden" runat="server" />
    <%--公司编号--%>
    <input id="Hid_UninCode" type="hidden" runat="server" />
    <%--登录名--%>
    <input id="Hid_LoginName" type="hidden" runat="server" />
    <%--城市数据--%>
    <input id="Hid_CityData" type="hidden" runat="server" />
    <%--航空公司数据--%>
    <input id="Hid_AirData" type="hidden" runat="server" />
    <%--0手动输入 1Pnr获取 2Pnr内容获取--%>
    <input id="Hid_OrderfangShi" type="hidden" value="0" />
    <%--pnr--%>
    <input id="Hid_Pnr" type="hidden" value="" />
    <%--新加数据用于常旅客--%>
    <input id="Hid_LoginAccount" type="hidden" runat="server" />
    <input id="Hid_LoginID" type="hidden" runat="server" />
    <%--常旅客数据--%>
    <input id="Hid_FlyerList" type="hidden" runat="server" />
    <%--出票公司编号--%>
    <input id="Hid_CPCpyNo" type="hidden" runat="server" />
    </form>
</body>
</html>
