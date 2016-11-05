<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="Buy_List" %>

<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="" />
    <title>查询预订</title>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/List.css" rel="stylesheet" type="text/css" />
    <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <link href="../Css/JPstep.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../JS/aircity.js" type="text/javascript"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript" id="script_k0"
        charset="utf-8"></script>
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        a:link, a:visited
        {
            color: Black;
            text-decoration: none;
        }
        a:hover, a:active
        {
            color: Blue;
            text-decoration: underline;
        }
        .TempDiv
        {
            width: 100%;
            display: none;
            background-color: #F3F8F8;
            text-align: center;
        }
        .TempDiv table
        {
            width: 100%;
        }
        .TempDiv table td
        {
            width: auto; /*height: 30px;
            line-height: 30px;*/
            font-size: 13px;
            text-align: center;
            vertical-align: middle;
        }
        .FXCss
        {
            color: #DF681F;
            font-weight: bold;
        }
        
        .btnen, .btnen-icon
        {
            cursor: pointer;
            display: inline-block;
            font-size: 12px;
            line-height: 100%;
            text-align: left;
            text-decoration: none;
            vertical-align: middle;
        }
        .btnen, .btnen input, .btnen button, .btnen span, .btnen a
        {
            background-image: url("../img/bg-btn.png");
            background-repeat: repeat-x;
            cursor: pointer;
            font-family: Tahoma;
            outline: medium none;
        }
        .btnen-ok-s, .btnen-cancel-s
        {
            padding: 0px;
        }
        .btnen-ok-s
        {
            background-position: 0 -10px;
            border: 1px solid #D74C00;
        }
        body .btnen input, body .btnen button, body .btnen span, .btnen a
        {
            vertical-align: baseline;
        }
        .btnen-ok-s input, .btnen-ok-s span, .btnen-ok-s a
        {
            background-position: 0 -75px;
        }
        .btnen-ok-s input, .btnen-ok-s span, .btnen-ok-s a
        {
            color: #FFFFFF;
            height: 20px;
            line-height: 20px;
            padding: 0 9px;
        }
        .btnen-ok-s:hover, .btnen-ok-s-hover
        {
            border: 1px solid #E47000;
        }
        .btnen-ok-s:hover input, .btnen-ok-s:hover span, .btnen-ok-s-hover input, .btnen-ok-s-hover span, .btnen-ok-s:hover a
        {
            background-position: 0 -518px;
        }
        span.btnen input, span.btnen a
        {
            margin: 0;
        }
        .btnen span, .btnen input, .btnen button, .btnen a
        {
            border: medium none;
        }
        .tab2 td
        {
            padding: 0px;
        }
        .leftliebiao_checi td
        {
            padding: 0px;
           
        }
        .search_title
        {
        color:#0E68A7;
        font-weight:bold;
        font-size:14px;
        background:url(../images/search_icon.gif) 30px no-repeat;
        width:96%;
        margin:auto;
        padding:18px 0 9px 115px;
        }
        .red{ color:#e5681f; font-family:Verdana; font-weight:;}
    .tab2 td{ font-size:13px; text-align:left}
    .red1{ color:#ff0000; font-family:Verdana; font-weight:;}
    .tabx td{ background:#ffecd9; height:27px; font-weight:bold;}
    .taby td{ border-bottom:1px #bfe0ee dashed; font-size:12px; padding:2px 0}
    #tdBack{ font-size:11px}
    .cang{ color:#6666ad; margin:0 5px 0 3px; font-size:13px; font-weight:bold}
    </style>
    <script type="text/javascript" language="javascript">
        var jQueryOne = jQuery = jQuery.noConflict(false);
        function showdialogGX(t, msg) {
            jQueryOne("#show").html(t);
            jQueryOne("#show").dialog({
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
                        InValue(msg);
                    },
                    '否': function () {
                        jQueryOne("#show").dialog('close');
                    }
                }
            });
        }
        function showdialogGXBack(t) {
            jQueryOne("#show").html(t);
            jQueryOne("#show").dialog({
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
                        InValueBack();
                    },
                    '否': function () {
                        jQueryOne("#show").dialog('close');
                    }
                }
            });
        }
        function showdialog(t) {
            jQueryOne("#show").html(t);
            jQueryOne("#show").dialog({
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
                        location.href = "OrderChangeList.aspx?currentuserid=<%=this.mUser.id.ToString() %>";
                    }
                }
            });
        }
        function showdialogmsg(t) {
            jQueryOne("#show").html(t);
            jQueryOne("#show").dialog({
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
                        jQueryOne("#show").dialog('close');
                    }
                }
            });
        }

        function showMoreInfo(m) {
            jQueryOne("#div_" + m).toggle(100);
        }
        //为预订按钮后面隐藏域复制
        function setHidId(hid) {
            jQueryOne("#Hid_HidId").val(hid);
        }
        function InValueBackCheck() {
            if (jQueryOne("#isGX").val() == "1" || jQueryOne("#isGX2").val() == "1") {
                var msg1 = "";
                var msg2 = "";
                if (jQueryOne("#isGX").val() == "1") {
                    msg1 = "去程航班为共享航班.";
                }
                if (jQueryOne("#isGX2").val() == "1") {
                    msg2 = "回程航班为共享航班.";
                }
                showdialogGXBack(msg1 + msg2 + "是否继续操作")
            } else {
                InValueBack();
            }
        }
        function InValueBack() {

            if (jQueryOne("#hidRoleType").val() == "1" || jQueryOne("#hidRoleType").val() == "2" || jQueryOne("#hidRoleType").val() == "3") {
                showdialogGXBack("运营商不提供订票功能,请用分销或采购账号订购");
                return;
            }

            var sInput = jQueryOne("#FlyInfoBack").val().toString().split('|');
            var info = jQueryOne("#FlyInfo").val().toString().split('|');
            if (sInput.length == 1) {
                showdialogmsg("请选择第二程航班");
                return;
            }
            if (info.length == 1) {
                showdialogmsg("请选择第一程航班");
                return;
            }
            if (info[0] != sInput[0]) {
                showdialogmsg("往返联乘必须是选择同一家航空公司");
                return;
            }
            var Timedif = 0;
            if (info[3].split('=')[0] == sInput[3].split('=')[0]) {
                var BeginTimeH = parseFloat(info[3].split('=')[2].split(':')[0]);
                var BeginTimeM = parseFloat(info[3].split('=')[2].split(':')[1]);
                var EndTimeH = parseFloat(sInput[3].split('=')[1].split(':')[0]);
                var EndTimeM = parseFloat(sInput[3].split('=')[1].split(':')[1]);
                var IsOk = false;
                if (BeginTimeH == 0) {
                    BeginTimeH = 24;
                }
                var Hdif = EndTimeH - BeginTimeH;
                var Mdif = EndTimeM - BeginTimeM;
                if (Hdif < 2) {
                    IsOk = true;
                }
                if (IsOk) {
                    var OkTime = (BeginTimeH + 2);
                    if (OkTime > 24) {
                        OkTime = (OkTime - 24) + ":" + BeginTimeM;
                    }
                    else if (OkTime == 24) {
                        OkTime = "00:" + BeginTimeM;
                    }
                    else {
                        OkTime = OkTime + ":" + BeginTimeM;
                    }
                    showdialogmsg("请预定出发时间在" + OkTime + "后的航班。");
                    return;
                }
            }
            jQueryOne("#Carryer").val(info[0] + "~" + sInput[0]);
            jQueryOne("#FlyNo").val(info[1] + "~" + sInput[1]);
            jQueryOne("#Aircraft").val(info[2] + "~" + sInput[2]);
            jQueryOne("#Time").val(info[3] + "~" + sInput[3]);
            jQueryOne("#City").val(info[4] + "~" + sInput[4]);
            jQueryOne("#ABFee").val(info[5] + "~" + sInput[5]);
            jQueryOne("#FuelAdultFee").val(info[6] + "~" + sInput[6]);
            jQueryOne("#DiscountRate").val(info[7] + "~" + sInput[7]);
            jQueryOne("#TickNum").val(info[8] + "~" + sInput[8]);
            jQueryOne("#XSFee").val(info[9] + "~" + sInput[9]);
            jQueryOne("#FareFee").val(info[10] + "~" + sInput[10]);
            jQueryOne("#Mileage").val(info[11] + "~" + sInput[11]);
            jQueryOne("#Cabin").val(info[12] + "~" + sInput[12]);
            jQueryOne("#Reservation").val(info[13] + "~" + sInput[13]);
            jQueryOne("#SpecialType").val(info[14] + "~" + sInput[14]);
            jQueryOne("#TravelType").val(info[15] + "~" + sInput[15]);
            jQueryOne("#Terminal").val(info[16] + "~" + sInput[16]);
            jQueryOne("#TravelType").val(info[15]);
            document.Order.submit();
        }
        function InValue(s) {
            if (jQueryOne("#hidRoleType").val() == "1" || jQueryOne("#hidRoleType").val() == "2" || jQueryOne("#hidRoleType").val() == "3") {
                showdialogmsg("运营商不提供订票功能,请用分销或采购账号订购");
                return;
            }
            var sInput = s.split('|');
            jQueryOne("#Carryer").val(sInput[0]);
            jQueryOne("#FlyNo").val(sInput[1]);
            jQueryOne("#Aircraft").val(sInput[2]);
            jQueryOne("#Time").val(sInput[3]);
            jQueryOne("#City").val(sInput[4]);
            jQueryOne("#ABFee").val(sInput[5]);
            jQueryOne("#FuelAdultFee").val(sInput[6]);
            jQueryOne("#DiscountRate").val(sInput[7]);
            jQueryOne("#TickNum").val(sInput[8]);
            jQueryOne("#XSFee").val(sInput[9]);
            jQueryOne("#FareFee").val(sInput[10]);
            jQueryOne("#Mileage").val(sInput[11]);
            jQueryOne("#Cabin").val(sInput[12]);
            jQueryOne("#Reservation").val(sInput[13]);
            jQueryOne("#SpecialType").val(sInput[14]);
            jQueryOne("#TravelType").val(sInput[15]);
            jQueryOne("#Terminal").val(sInput[16]);
            //动态 特价时
            if (sInput[14] == 1 && sInput[7] != "-2") {
                //获取价格
                var tjValue = jQueryOne("#" + jQueryOne("#Hid_HidId").val()).val();
                //jQueryOne("#hidTjValue").val();
                if (tjValue != "") {
                    var valStrS = tjValue.split('|');
                    //因为特价是发指令得到的,所以这三个价格要重新赋值,固定特价流程和动态一样,也需走这一步
                    jQueryOne("#ABFee").val(valStrS[1]); //基建
                    jQueryOne("#FuelAdultFee").val(valStrS[2]); //燃油
                    jQueryOne("#XSFee").val(valStrS[0]); //舱位价
                }
            }
            //是否查询共享航班
            var IsShowShare = jQueryOne("#cbIsShowShare")[0].checked ? "true" : "false";
            jQueryOne("#hidIsShowShare").val(IsShowShare);

            //航程类型
            var id = jQueryOne("input[name='travelGroup']:checked").val();
            jQueryOne("#TravelType").val(id);
            jQueryOne("#Hid_travel").val(id);
            if (id == 1) {
                //单程
            } else if (id == 2) {
                //往返
                showts();
                //回程日期
                jQueryOne("#ReturnTime").val(jQueryOne("#txtReturnDate").val());
                document.Order.action = "BackList.aspx";
            }
            else if (id == 3) {
                //联成
                showts();
                //中转日期
                jQueryOne("#ReturnTime").val(jQueryOne("#txtReturnDate").val());
                //到达城市
                jQueryOne("#hidcity").val(jQueryOne("#hidTCityCode").val() + "-" + jQueryOne("#txtToCityCode").val());
                document.Order.action = "BackList.aspx";
            }
            document.Order.submit();
        }
        function showts() {
            document.getElementById("overlay").style.display = "";
            document.getElementById("loading").style.display = "";
        }
        function showts2(id) {
            document.getElementById("overlay").style.display = "none";
            document.getElementById("loading").style.display = "none";
        }
        //设置行程类型
        function setTravelRadio(v) {
            var Groups = document.getElementsByName("travelGroup");
            for (var i = 0; i < Groups.length; i++) {
                if (Groups[i].value == v && !Groups[i].checked) {
                    Groups[i].checked = true;
                    break;
                }
            }
        }
        function flyType(v) {
            var id = jQueryOne("input[name='travelGroup']:checked").val();
            //设置行程类型
            setTravelRadio(v);
            jQueryOne("#Hid_travel").val(v);
            if (v == "1") {
                //单程
                jQueryOne("#Conntitle").hide();
                jQueryOne("#midCityContainer").hide();
                jQueryOne("#tdRerurnTime1").hide();
                jQueryOne("#rtnReturnDate").hide();
                jQueryOne("#td_left").css("padding-left", "15px");
            } else if (v == "2") {
                //往返
                jQueryOne("#Conntitle").hide();
                jQueryOne("#midCityContainer").hide();

                jQueryOne("#tdRerurnTime1").show();
                jQueryOne("#rtnReturnDate").show();
                jQueryOne("#SecondTime").html("往返日期");
                jQueryOne("#td_left").css("padding-left", "15px");

            } else if (v == "3") {
                //联程
                jQueryOne("#Conntitle").show();
                jQueryOne("#midCityContainer").show();
                jQueryOne("#tdRerurnTime1").show();
                jQueryOne("#rtnReturnDate").show();
                jQueryOne("#SecondTime").html("中转日期");
                jQueryOne("#td_left").css("padding-left", "8px");
            }
        }
        function queryCheck() {
            var flag = false;
            //行程类型
            var travel = jQueryOne("#Hid_travel").val();
            var fromCode = jQueryOne("#txtFromCityCode").val();
            var middleCode = jQueryOne("#txtMidCityCode").val();
            var toCode = jQueryOne("#txtToCityCode").val();
            var fromDate = jQueryOne("#txtFromDate").val();
            var returnDate = jQueryOne("#txtReturnDate").val();
            if (travel == "") {
                return flag;
            }
            if (fromCode == "") {
                showdialogmsg("出发城市不能为空！");
                return flag;
            }
            if (toCode == "") {
                showdialogmsg("到达城市不能为空！");
                return flag;
            }
            if (jQueryOne.trim(fromCode).indexOf("中文/英文") > -1) {
                showdialogmsg("请输入正确的出发城市！");
                return flag;
            }
            if (jQueryOne.trim(toCode).indexOf("中文/英文") > -1) {
                showdialogmsg("请输入正确的到达城市！");
                return flag;
            }
            if (fromDate == "") {
                showdialogmsg("出发日期不能为空！");
                return flag;
            }
            if (travel == "1") {
                if (fromCode == toCode) {
                    showdialogmsg("出发城市与到达城市不能相同！");
                    return flag;
                }
                flag = true;
            } else if (travel == "2") {
                if (returnDate == "") {
                    showdialogmsg("回程日期不能为空！");
                    return flag;
                }
                if (fromCode == toCode) {
                    showdialogmsg("出发城市与到达城市不能相同！");
                    return flag;
                }
                var s1 = fromDate.split('-');
                var s2 = returnDate.split('-');
                var d1 = new Date(s1[0], s1[1], s1[2]);
                var d2 = new Date(s2[0], s2[1], s2[2]);
                //                if (d1 >= d2) {
                //                    showdialogmsg("回程日期必须大于出发日期！");
                //                    return flag;
                //                }
                flag = true;
            } else if (travel == "3") {
                if (middleCode == "") {
                    showdialogmsg("中转城市不能为空！");
                    return flag;
                }
                if (fromCode == middleCode) {
                    showdialogmsg("出发城市和中转城市不能相同！");
                    return flag;
                }
                if (middleCode == toCode) {
                    showdialogmsg("中转城市和到达城市不能相同！");
                    return flag;
                }
                if (fromCode == toCode) {
                    showdialogmsg("出发城市与到达城市不能相同！");
                    return flag;
                }
                var s1 = fromDate.split('-');
                var s2 = returnDate.split('-');
                var d1 = new Date(s1[0], s1[1], s1[2]);
                var d2 = new Date(s2[0], s2[1], s2[2]);
                //                if (d1 >= d2) {
                //                    showdialogmsg("中转日期必须大于出发日期！");
                //                    return flag;
                //                }
                flag = true;
            }
            showts();

            var allSelect = document.getElementsByTagName("select");
            for (var i = 0; i < allSelect.length; i++) {
                allSelect[i].style.visibility = "hidden";
            }

            return flag;
        }

        var tempTime = "";
        //设置最小时间
        function getDate() {
            tempTime = tempTime != "" ? tempTime : jQueryOne("#txtFromDate").val();
            WdatePicker({ isShowClear: false, minDate: tempTime, dateFmt: 'yyyy-MM-dd', doubleCalendar: true });
        }
        //设置时间
        function setDate(obj, type) {
            tempTime = obj.value;
            if (type == 1) {
                WdatePicker({ isShowClear: false, minDate: jQueryOne("#hidTime").val(), dateFmt: 'yyyy-MM-dd', doubleCalendar: true });
            } else if (type == 0) {
                try {
                    var txtMiddle = jQueryOne("#txtMiddleDate").val();
                    if (duibi(txtMiddle, tempTime))
                        jQueryOne("#txtMiddleDate").attr("value", obj.value);
                    var txtReturn = jQueryOne("#txtReturnDate").val();
                    if (duibi(txtReturn, tempTime))
                        jQueryOne("#txtReturnDate").attr("value", obj.value);
                }
                catch (e) { }
            }
        }
        // a 开始时间 b 结束时间
        function duibi(a, b) {
            try {
                var arr = a.split("-");
                var starttime = new Date(arr[0], arr[1], arr[2]);
                var starttimes = starttime.getTime();
                var arrs = b.split("-");
                var lktime = new Date(arrs[0], arrs[1], arrs[2]);
                var lktimes = lktime.getTime();
                if (starttimes >= lktimes)
                    return false; //开始时间大于离开时间，请检查
                else
                    return true;

            } catch (e) {
                return false;
            }
        }
        function getColumnTitleBack(showAirInfo) {
            var outstring = "";

            if (showAirInfo == "showAirInfo") {
                outstring += "<div class=\"search_title\">去程机票预订</div>"
            }
            if (showAirInfo == "showAirInfoBack") {
                outstring += "<div class=\"search_title\">回程机票预订</div>"
            }
            outstring += " <table width=\"100%\" class=\"tabx\">";
            outstring += " <tr>";
            outstring += " <td style=\"width: 5%; cursor: pointer; text-decoration: underline; color: Blue;text-align:center\">";
            outstring += " 航班信息";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:center\">";
            outstring += " 起抵时间/机场";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 100以上";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 99-90";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 89-80";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 79-70";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 69-60";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 59-50";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 49-40";
            outstring += " </td>";
            outstring += " <td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 39以下";
            outstring += " </td>";
            outstring += " <td style=\"width: 4%; cursor: pointer; text-decoration: underline; color: Blue;text-align:left\">";
            outstring += " 动态特价";
            outstring += " </td>";
            outstring += " </tr>";
            outstring += " </table>";
            return outstring;
        }
        function getColumnTitle(policyHide) {
            var outstring = "";
            outstring += "<table class=\"tab1\">";
            outstring += "<tr>";
            outstring += " <td style=\"width: 1%;\">";
            outstring += " </td>";
            outstring += "<td style=\"width: 8%; cursor: pointer; text-decoration: underline; color: Blue;\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">航班信息</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">起抵时间/机场</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 5%;\">";
            outstring += "机型";
            outstring += "</td>";
            outstring += "<td style=\"width: 8%;\">";
            outstring += "机建/燃油";
            outstring += "</td>";
            outstring += "<td style=\"width: 8%; cursor: pointer; text-decoration: underline; color: Blue;\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">舱位/折扣</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 5%;\">";
            outstring += "座位数";
            outstring += "</td>";
            outstring += "<td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">舱位价</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 10%;\">";
            outstring += "票面价";
            outstring += "</td>";
            outstring += "<td style=\"width: 12%;" + policyHide + "\">";
            outstring += "政策/佣金";
            outstring += "</td>";
            outstring += "<td style=\"width: 8%;" + policyHide + " cursor: pointer; text-decoration: underline; color: Blue;\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">实付金额</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 7%;\">";
            outstring += "备注";
            outstring += "</td>";
            outstring += "<td style=\"width: 8%;\">";
            outstring += "操作";
            outstring += "</td>";
            outstring += "</tr>";
            outstring += "</table>";
            return outstring;
        }
        //往返联成IBE数据展示方式
        function getAirInfoBack(json, showAirInfo) {
            var json = "{}";
            if (showAirInfo == "showAirInfo") {
                json = '<%=oneWayJson%>';
            }
            if (showAirInfo == "showAirInfoBack") {
                json = '<%=connAndReturnJson%>';
            } 
            var chaceNameByGUID = ""; //基础数据缓存ID
            if (json.split('infoSplit')[2] != '') {
                jQueryOne("#" + showAirInfo).html(json.split('infoSplit')[2]); //输出错误信息
            } else {
                try {

                    chaceNameByGUID = json.split('infoSplit')[1];
                    var obj = jQueryOne.parseJSON(json.split('infoSplit')[0]); //转换航班JSON

                    //王永磊 Kevin 2013-04-11 Edit
                    if (obj != null && obj != undefined && json.split('infoSplit')[0] != "" && json.split('infoSplit')[0] != "{}") {
                        //if (obj == null || obj == undefined) {
                        var outstring = ""; //拼接的表格字符串
                        var strTitle = "";
                        var price = "0";
                        jQueryOne("#" + showAirInfo).html("");
                        //拼接列头
                        outstring += getColumnTitleBack(showAirInfo);
                        var i = 0;

                        for (var tablename in obj) {//循环所有的表
                            i++;

                            outstring += "<table width=\"100%\"  class=\"taby\">";
                            outstring += "<tr onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\"class='leftliebiao_checi'>";
                            var tempDiscount1 = "<td id=\"tdBack" + showAirInfo + i + "_100\" style=\"width: 10%;text-align: left; line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(100以上)
                            var tempDiscount2 = "<td id=\"tdBack" + showAirInfo + i + "_90\" style=\"width: 10%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(99-90)
                            var tempDiscount3 = "<td id=\"tdBack" + showAirInfo + i + "_80\" style=\"width: 10%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(89-80)
                            var tempDiscount4 = "<td id=\"tdBack" + showAirInfo + i + "_70\" style=\"width: 10%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(79-70)
                            var tempDiscount5 = "<td id=\"tdBack" + showAirInfo + i + "_60\" style=\"width: 10%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(69-60)
                            var tempDiscount6 = "<td id=\"tdBack" + showAirInfo + i + "_50\" style=\"width: 10%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(59-50)
                            var tempDiscount7 = "<td id=\"tdBack" + showAirInfo + i + "_40\" style=\"width: 10%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(49-40)
                            var tempDiscount8 = "<td id=\"tdBack" + showAirInfo + i + "_30\" style=\"width: 10%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(39以下)
                            var tempDiscount9 = "<td id=\"tdBack" + showAirInfo + i + "_00\" style=\"width: 4%;text-align:left;line-height:18px\">"; //拼接的折扣字符串,先顺序拼接(特价)
                            var objtablenamelen = obj[tablename].length; //行的数量
                            for (var j = 0; j < objtablenamelen; j++) {

                                //共享航班指定样式 
                                var sty = "";
                                if (obj[tablename][0]["FlightNo"].indexOf("*") != -1) {
                                    sty = "color:red;";

                                }
                                //航班信息只加载一次即可
                                if (j == 0) {

                                    //                                outstring += "<td width=\"3%\" height=\"90\" align=\"center\"><img src=\"../img/air/" + obj[tablename][j]["CarrCode"].toString().toUpperCase() + ".gif\" /></td>";
                                    //                                outstring += "</td>";
                                    outstring += "<td style='width: 5%;" + sty + "'> <img src=\"../img/air/"
                                + obj[tablename][j]["CarrCode"].toString().toUpperCase() + ".gif\" /> "
                            + obj[tablename][j]["Carrier"].toString() + "<strong>" + obj[tablename][j]["CarrCode"].toString()
                            + obj[tablename][j]["FlightNo"].toString() + "</strong>"; //航班信息


                                    outstring += "</td>";

                                    outstring += "<td style='width: 10%; font-size:11px; '>"
                                    outstring += " <strong>" + obj[tablename][j]["StartTime"].toString() + "</strong>"
                            + obj[tablename][j]["FromCity"].toString() + "&nbsp;"
                            + obj[tablename][j]["dterm"] + "<br /> <strong>" + obj[tablename][j]["EndTime"].toString()
                            + "</strong> " + obj[tablename][j]["ToCity"].toString()
                            + "&nbsp;" + obj[tablename][j]["aterm"]; //起抵时间/机场
                                    //outstring += "<br />"+obj[tablename][0]["Model"].toString() + "<br/>" + obj[tablename][0]["IsStop"].toString() + "</td>"; //机型

                                    outstring += "</td>";


                                }

                                var Specialstr = "";
                                if (obj[tablename][j]["SpecialType"].toString() == "1") {

                                    Specialstr += obj[tablename][j]["CarrCode"].toString(); //0
                                    Specialstr += "|" + obj[tablename][j]["FlightNo"].toString(); //1
                                    Specialstr += "|" + obj[tablename][j]["StartCityCode"].toString(); //2
                                    Specialstr += "|" + obj[tablename][j]["ToCityCode"].toString(); //3
                                    Specialstr += "|" + obj[tablename][j]["StartTime"].toString(); //4
                                    Specialstr += "|" + obj[tablename][j]["EndTime"].toString(); //5
                                    Specialstr += "|" + obj[tablename][j]["StartDate"].toString(); //6
                                    Specialstr += "|" + obj[tablename][j]["Space"].toString(); //7
                                    Specialstr += "|" + obj[tablename][j]["GUID"].toString(); //8
                                }
                                //客规
                                var titstr = obj[tablename][j]["DishonoredBillPrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + obj[tablename][j]["LogChangePrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + obj[tablename][j]["UpCabinPrescript"].toString().replace("\r\n", "<br/>");
                                var titstr2 = obj[tablename][j]["DishonoredBillPrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + "^" + obj[tablename][j]["LogChangePrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + "^" + obj[tablename][j]["UpCabinPrescript"].toString().replace("\r\n", "<br/>");

                                //订票参数字段
                                var sItemlist = obj[tablename][j]["CarrCode"] + "^" + obj[tablename][j]["Carrier"] + "|"; //0.承运人
                                sItemlist += obj[tablename][j]["FlightNo"] + "|"; //1.航班号
                                sItemlist += obj[tablename][j]["Model"] + "|"; //2.机型
                                sItemlist += obj[tablename][j]["StartDate"] + "=" + obj[tablename][j]["StartTime"] + "=" + obj[tablename][j]["EndTime"] + "|"; //3.起止时间
                                sItemlist += obj[tablename][j]["StartCityCode"] + "-" + obj[tablename][j]["ToCityCode"] + "^" + obj[tablename][j]["FromCity"] + "-" + obj[tablename][j]["ToCity"] + "|"; //4.起止城市
                                //sItemlist += "|"; //5.经停
                                sItemlist += obj[tablename][j]["ABFee"] + "|"; //5.机建
                                sItemlist += obj[tablename][j]["FuelAdultFee"] + "|"; //6.燃油
                                sItemlist += obj[tablename][j]["DiscountRate"] + "|"; //7.舱位折扣 5%
                                sItemlist += obj[tablename][j]["TickNum"] + "|"; //8.座位数
                                //sItemlist += parseFloat(obj[tablename][j]["XSFee"]).substring(0, parseFloat(obj[tablename][j]["XSFee"]).indexOf(".") + 3) + "|"; //9.销售价 舱位价
                                sItemlist += obj[tablename][j]["XSFee"] + "|"; //9.销售价 舱位价
                                sItemlist += obj[tablename][j]["FareFee"] + "|"; //10.全价  y 舱价格
                                //sItemlist += parseFloat(plic) / 100 + "|"; //11.政策
                                sItemlist += obj[tablename][j]["Mileage"] + "|"; //11.里程
                                sItemlist += obj[tablename][j]["Space"] + "|"; //12.舱位

                                sItemlist += titstr + "|"; //13.客规

                                sItemlist += obj[tablename][j]["SpecialType"] + "|"; //13.特价类型
                                //sItemlist += obj[tablename][j]["IsLowerOpen"] + "|"; //14.是否低开
                                //sItemlist += titstr2; //22  客规(分开的)
                                sItemlist += jQueryOne("#Hid_travel").val() + "|"; //15.行程类型
                                sItemlist += obj[tablename][j]["dterm"] + "^" + obj[tablename][j]["aterm"]; //16.乘机及停靠航站楼    

                                //var zk = obj[tablename][j]["DiscountRate"].toString().replace("-1", "<span style=\"color:red\">特价</span>").replace("-2", "<span style=\"color:red\">特价</span>");
                                //共享航班提示
                                var msg = "saveBackInfo('" + sItemlist + "', '" + showAirInfo + "','0')";
                                if (obj[tablename][0]["FlightNo"].indexOf("*") != -1) {
                                    msg = "saveBackInfo('" + sItemlist + "', '" + showAirInfo + "','1')";
                                }
                                var zk = -1;
                                try {
                                    zk = parseFloat(obj[tablename][j]["DiscountRate"].toString());
                                } catch (e) {
                                    zk = -1;
                                }
                                //销售价
                                var xsfee = "￥" + obj[tablename][j]["XSFee"].split('.')[0];
                                var flag = discountArea(zk);
                                switch (flag) {
                                    case 0:
                                        break;
                                    case 1:
                                        tempDiscount1 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case 2:
                                        tempDiscount2 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case 3:
                                        tempDiscount3 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case 4:
                                        tempDiscount4 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case 5:
                                        tempDiscount5 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case 6:
                                        tempDiscount6 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case 7:
                                        tempDiscount7 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case 8:
                                        tempDiscount8 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                                        break;
                                    case -1:
                                        tempDiscount9 += "<a id='" + obj[tablename][j]["GUID"].toString() + "_A' isck='0' title='点击自动获取舱位价格' href=\"javascript:GetSpecialBack('" + obj[tablename][j]["GUID"].toString() + "','" + Specialstr + "','" + obj[tablename][j]["FareFee"] + "','" + i + "','" + sItemlist + "','" + showAirInfo + "')  \"><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"].toString() + "</strong></a><br />";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            tempDiscount1 += "</td>";
                            tempDiscount2 += "</td>";
                            tempDiscount3 += "</td>";
                            tempDiscount4 += "</td>";
                            tempDiscount5 += "</td>";
                            tempDiscount6 += "</td>";
                            tempDiscount7 += "</td>";
                            tempDiscount8 += "</td>";
                            tempDiscount9 += "</td>";
                            outstring += tempDiscount1 + tempDiscount2 + tempDiscount3 + tempDiscount4 + tempDiscount5 + tempDiscount6 + tempDiscount7 + tempDiscount8 + tempDiscount9;
                            outstring += "</tr>";
                            outstring += "</table>";
                            jQueryOne("#" + showAirInfo).append(outstring);
                            outstring = "";
                        }
                        var yudingStr = "<table align=\"center\"><tr>";
                        yudingStr += "<td height=\"36px\" style=\"text-align:center;\"><span class=\"btnen btnen-ok-s\"><input id='btnCreateBack' type='button' value='预订' onclick='InValueBackCheck()' /></span></td>";
                        yudingStr += "</table></tr>"
                        if (showAirInfo == "showAirInfoBack") {
                            jQueryOne("#backYuDing").append(yudingStr);
                        }
                        //票面价
                        //var PMFee = "￥" + obj[tablename][j]["PMFee"].split('.')[0];
                    }
                } catch (e) {
                    jQueryOne("#" + showAirInfo).html("<font style=\"font-size:18px;font-weight:bold;color:red;\">航班数据解析失败,请重试或联系管理员</font>");
                }
            }
        }

        function saveBackInfo(s, type, isGX) {
            if (type == "showAirInfo") {
                jQueryOne("#FlyInfo").val(s);
                jQueryOne("#isGX").val(isGX);
            }
            if (type == "showAirInfoBack") {
                jQueryOne("#FlyInfoBack").val(s);
                jQueryOne("#isGX2").val(isGX);
            }
        }
        //计算折扣属于哪一个分类区域
        function discountArea(discount) {
            var num = parseFloat(discount);
            var flag = -3;
            if (num >= 100) {
                flag = 1;
            }
            if (num >= 90 && num < 100) {
                flag = 2;
            }
            if (num >= 80 && num < 90) {
                flag = 3;
            }
            if (num >= 70 && num < 80) {
                flag = 4;
            }
            if (num >= 60 && num < 70) {
                flag = 5;
            }
            if (num >= 50 && num < 60) {
                flag = 6;
            }
            if (num >= 40 && num < 50) {
                flag = 7;
            }
            if (num > 0 && num < 40) {
                flag = 8;
            }
            if (num <= 0) {
                flag = -1;
            }
            return flag;
        }

        function getUGroupTitle() {
            var html = "";
            html += "<table  class='tab1'><tr>";
            html += "<td style='width: 10%;'>航班信息</td>";
            html += "<td style='width: 20%;'>起抵时间/机场</td>";
            html += "<td style='width: 10%;'>机型</td>";
            html += "<td style='width: 10%;'>机建/燃油</td>";
            html += "<td style='width: 10%;'>舱位/折扣</td>";
            html += "<td style='width: 10%;'>座位数</td>";
            html += "<td style='width: 10%;'>票价</td>";
            html += "<td style='width: 10%;'>政策</td>";
            html += "<td style='width: 10%;'>操作</td>";
            html += "</tr>"
            return html;
        }
        function getUGroupBody(o) {
            var html = "";
            html += "<tr class='leftliebiao_checi'  onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\">";
            html += "<td><div><img src='../img/air/" + o._aircode + ".gif' />" + o._cpyname + "</div>";
            html += "<dic>" + o._aircode + o._flightno + "</div></td>"
            html += "<td><div>" + o._fromcityname + o._fromcitycode + "-" + o._tocityname + o._tocitycode + "</div>";
            html += "<div>" + o._flighttime + "</div></td>";
            html += "<td>" + o._planetype + "</td>";
            html += "<td>" + o._buildprice + "/" + o._oilprice + "</td>";
            html += "<td><strong style='color:#6666AD'>" + o._class + "</strong>/" + o._rebate + "</td>";
            html += "<td>" + o._seatcount + "</td>";
            html += "<td>" + o._prices + "</td>";
            html += "<td>" + o._downrebate + "</td>";
            html += "<td><span class='btnen btnen-ok-s'><input id='btnApply' type='button' value='申请'/></span></td>";
            html += "</tr>";
            return html;
        }
        function getUGroupPolicyInfo(json, showGroupInfo) {
            var info = jQuery.parseJSON(json);
            var html = "";
            if (info.length > 0) {
                html += getUGroupTitle();
            }
            for (var i = 0; i < info.length; i++) {
                html += getUGroupBody(info[i]);
            }
            html += "</table>";
            jQueryOne('#' + showGroupInfo).html(html);
        }
        function getAirInfo(json, showAirInfo) {
            var json = '<%=oneWayJson%>';
            var airInfoIsOk = false; //航班数据是否正确
            var chaceNameByGUID = ""; //基础数据缓存ID
            if (json.split('infoSplit')[2] != '') {
                jQueryOne("#" + showAirInfo).html(json.split('infoSplit')[2]); //输出错误信息
            } else {
                var count = 0;
                try {
                    if (json.split('infoSplit')[0] != "") {
                        chaceNameByGUID = json.split('infoSplit')[1];
                        var obj = jQueryOne.parseJSON(json.split('infoSplit')[0]); //转换航班JSON

                        //王永磊 kevin 2013-04-11 更改
                        if (obj != null && obj != undefined && json.split('infoSplit')[0] != "" && json.split('infoSplit')[0] != "{}") {
                            //if (obj == null || obj == undefined) {
                            var outstring = ""; //拼接的表格字符串
                            var strTitle = "";
                            var price = "0";
                            jQueryOne("#" + showAirInfo).html("");
                            //拼接列头
                            var policyHide = ""; //默认显示
                            if (jQueryOne("#hidePolicy") != undefined && jQueryOne("#hidePolicy").val() == "True") {//隐藏政策,实付金额
                                policyHide = "display: none;"
                            }
                            outstring += getColumnTitle(policyHide);
                            var i = 0;

                            for (var tablename in obj) {//循环所有的表
                                count++;
                                i++;
                                var objtablenamelen = obj[tablename].length; //行的数量
                                if (objtablenamelen == 1 && obj[tablename][0]["TickNum"] == "--") {
                                    outstring += "<table class='tab2'>";
                                    outstring += "<tr onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\"class='leftliebiao_checi'>";
                                    outstring += "<td style='width: 1%;'></td>";
                                    outstring += "<td style='width: 8%;'><img src=\"../img/air/" + obj[tablename][0]["CarrCode"].toString().toUpperCase() + ".gif\" />" + obj[tablename][0]["Carrier"].toString() + "<br /><b>" + obj[tablename][0]["CarrCode"].toString() + obj[tablename][0]["FlightNo"].toString() + "</b></td>"; //航班信息
                                    outstring += "<td style='width: 10%;'><b>" + obj[tablename][0]["StartTime"].toString() + "</b> " + obj[tablename][0]["FromCity"].toString() + "<br /><b>" + obj[tablename][0]["EndTime"].toString() + "</b> " + obj[tablename][0]["ToCity"].toString() + "</td>"; //起抵时间/机场
                                    outstring += "<td style='width: 5%;'>" + obj[tablename][0]["Model"].toString() + "<br/>" + obj[tablename][0]["IsStop"].toString() + "</td>"; //机型
                                    outstring += "<td style='width: 8%;'>" + obj[tablename][0]["ABFee"].toString() + "<br />" + obj[tablename][0]["FuelAdultFee"].toString() + "</td>"; //机建/燃油
                                    outstring += "<td style='width: 8%;'><strong style=\"color:#6666AD\">" + obj[tablename][0]["Space"].toString() + "</strong>(--折)</td>"; //舱位/折扣
                                    outstring += "<td style='width: 5%;'>" + obj[tablename][0]["TickNum"].toString() + "</td>"; //座位数
                                    outstring += "<td style='width: 10%;' class='FXCss'>--</td>"; //
                                    outstring += "<td style='width: 10%;' class='FXCss'>--</td>"; //
                                    outstring += "<td style='width:12%;" + policyHide + "' class='FXCss'>--</td>"; //优惠
                                    outstring += "<td style='width:8%;" + policyHide + "' class='FXCss'>--</td>"; //实际支付金额
                                    outstring += "<td style='width: 7%;'></td>";
                                    outstring += "<td style='width: 8%;'><strong style=\"color:red;\">无座位销售<strong></td>";
                                    outstring += "</tr></table>";

                                }
                                else {

                                    //非特价索引
                                    var NoTJIndex = 0;
                                    //特价索引
                                    var TJIndex = 0;
                                    for (var j = 0; j < objtablenamelen; j++) {
                                        //政策
                                        var plic = "";
                                        //订票参数字段
                                        var sItemlist = "";
                                        //现返
                                        var xf = "";
                                        //共享航班指定样式
                                        var sty = "";
                                        //备注
                                        var bz = obj[tablename][j]["IsStop"];
                                        //折扣
                                        var zk = obj[tablename][j]["DiscountRate"].toString().replace("-1", "<span style=\"color:red\">特价</span>").replace("-2", "<span style=\"color:red\">特价</span>");
                                        //舱位数量
                                        var SpaceNum = obj[tablename][j]["TickNum"];
                                        //销售价
                                        var xsfee = "￥" + obj[tablename][j]["XSFee"].split('.')[0];
                                        //票面价
                                        var PMFee = "￥" + obj[tablename][j]["PMFee"].split('.')[0];
                                        //客规
                                        var titstr = obj[tablename][j]["DishonoredBillPrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + obj[tablename][j]["LogChangePrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + obj[tablename][j]["UpCabinPrescript"].toString().replace("\r\n", "<br/>");
                                        var titstr2 = obj[tablename][j]["DishonoredBillPrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + "^" + obj[tablename][j]["LogChangePrescript"].toString().replace("\r\n", "<br/>") + "<br/>" + "^" + obj[tablename][j]["UpCabinPrescript"].toString().replace("\r\n", "<br/>");

                                        var titlestr = titstr.replace("<br/>", "") == "" ? "" : "title='" + titstr.replace("<br/>", "\r\n") + "'";
                                        //实际支付价
                                        var sjf = "待计算";
                                        //动态特价的显示
                                        if (obj[tablename][j]["SpecialType"].toString() == "1") {
                                            if (obj[tablename][j]["DiscountRate"].toString() == "-1") {
                                                //此拼接的字符串,后面如果要加东西,请排在后面,不要随意在中间插入
                                                //使用的时候是按索引来使用的
                                                //改变索引容易在其他的地方报错
                                                var Specialstr = "";
                                                Specialstr += obj[tablename][j]["CarrCode"].toString(); //0
                                                Specialstr += "|" + obj[tablename][j]["FlightNo"].toString(); //1
                                                Specialstr += "|" + obj[tablename][j]["StartCityCode"].toString(); //2
                                                Specialstr += "|" + obj[tablename][j]["ToCityCode"].toString(); //3
                                                Specialstr += "|" + obj[tablename][j]["StartTime"].toString(); //4
                                                Specialstr += "|" + obj[tablename][j]["EndTime"].toString(); //5
                                                Specialstr += "|" + obj[tablename][j]["StartDate"].toString(); //6
                                                Specialstr += "|" + obj[tablename][j]["Space"].toString(); //7
                                                Specialstr += "|" + obj[tablename][j]["GUID"].toString(); //8
                                                xsfee = "<span class=\"btnen btnen-ok-s\"><input id='btnSpecial" + obj[tablename][j]["GUID"].toString() + "' type='button' value='获取价格' onclick=\"GetSpecial(this,'" + Specialstr + "','" + obj[tablename][j]["GUID"].toString() + "')\" /></span>";
                                            }
                                            sjf = "特价";
                                        }
                                        //非特价折扣的显示
                                        if (zk.indexOf("特价") == -1) {
                                            zk += "折";
                                        }


                                        //此拼接的字符串,后面如果要加东西,请排在后面,不要随意在中间插入
                                        //使用的时候是按索引来使用的
                                        //改变索引容易在其他的地方报错
                                        sItemlist = obj[tablename][j]["CarrCode"] + "^" + obj[tablename][j]["Carrier"] + "|"; //0.承运人
                                        sItemlist += obj[tablename][j]["FlightNo"] + "|"; //1.航班号
                                        sItemlist += obj[tablename][j]["Model"] + "|"; //2.机型
                                        sItemlist += obj[tablename][j]["StartDate"] + "=" + obj[tablename][j]["StartTime"] + "=" + obj[tablename][j]["EndTime"] + "|"; //3.起止时间
                                        sItemlist += obj[tablename][j]["StartCityCode"] + "-" + obj[tablename][j]["ToCityCode"] + "^" + obj[tablename][j]["FromCity"] + "-" + obj[tablename][j]["ToCity"] + "|"; //4.起止城市
                                        //sItemlist += "|"; //5.经停
                                        sItemlist += obj[tablename][j]["ABFee"] + "|"; //5.机建
                                        sItemlist += obj[tablename][j]["FuelAdultFee"] + "|"; //6.燃油
                                        sItemlist += obj[tablename][j]["DiscountRate"] + "|"; //7.舱位折扣 5%
                                        sItemlist += obj[tablename][j]["TickNum"] + "|"; //8.座位数
                                        //sItemlist += parseFloat(obj[tablename][j]["XSFee"]).substring(0, parseFloat(obj[tablename][j]["XSFee"]).indexOf(".") + 3) + "|"; //9.销售价 舱位价
                                        sItemlist += obj[tablename][j]["XSFee"] + "|"; //9.销售价 舱位价
                                        sItemlist += obj[tablename][j]["FareFee"] + "|"; //10.全价  y 舱价格
                                        //sItemlist += parseFloat(plic) / 100 + "|"; //11.政策
                                        sItemlist += obj[tablename][j]["Mileage"] + "|"; //11.里程
                                        sItemlist += obj[tablename][j]["Space"] + "|"; //12.舱位

                                        sItemlist += titstr + "|"; //13.客规

                                        sItemlist += obj[tablename][j]["SpecialType"] + "|"; //13.特价类型
                                        //sItemlist += obj[tablename][j]["IsLowerOpen"] + "|"; //14.是否低开
                                        //sItemlist += titstr2; //22  客规(分开的)
                                        sItemlist += jQueryOne("#Hid_travel").val() + "|"; //15.行程类型
                                        sItemlist += obj[tablename][j]["dterm"] + "^" + obj[tablename][j]["aterm"]; //16.乘机及停靠航站楼    
                                        //共享航班预定提示
                                        var msg = "InValue(\"" + sItemlist + "\")";
                                        //共享航班提示
                                        if (obj[tablename][0]["FlightNo"].indexOf("*") != -1) {
                                            sty = "color:red;";
                                            msg = "showdialogGX(\"该航班为共享航班，是否继续操作？\",\"" + sItemlist + "\")";
                                        }
                                        outstring += "<table class='tab2'>";
                                        if (obj[tablename][j]["SpecialType"].toString() == "1") {//特价
                                            if (obj[tablename][j]["DiscountRate"].toString() == "-2") {
                                                outstring += "<tr onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\"class='leftliebiao_checi'>";
                                                outstring += "<td style='width: 1%;'></td>";
                                                //航班信息显示
                                                var hb = "";
                                                //起抵信息显示
                                                var qd = "";
                                                //机建信息显示
                                                var jx = "";
                                                if (TJIndex == 0) {
                                                    hb = "<img src=\"../img/air/" + obj[tablename][j]["CarrCode"].toUpperCase() + ".gif\" />" + obj[tablename][j]["Carrier"] + "<br /><b>" + obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"] + "</b>";
                                                    qd = "<b>" + obj[tablename][j]["StartTime"] + "</b> " + obj[tablename][j]["FromCity"] + " " + obj[tablename][j]["dterm"] + "<br /><b>" + obj[tablename][j]["EndTime"] + "</b> " + obj[tablename][j]["ToCity"] + " " + obj[tablename][j]["aterm"];
                                                    jx = obj[tablename][j]["Model"];
                                                }
                                                outstring += "<td style='width: 8%;" + sty + "'>" + hb + "</td>"; //航班信息
                                                outstring += "<td style='width: 10%;'>" + qd + "</td>"; //起抵时间/机场/航站楼
                                                outstring += "<td style='width: 5%;'>" + jx + "</td>"; //机型

                                                outstring += "<td style='width: 8%;'>" + obj[tablename][j]["ABFee"] + "<br />" + obj[tablename][j]["FuelAdultFee"] + "</td>"; //机建/燃油
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "zctt' style='width: 8%;' title=''><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"] + "</strong>(" + zk + ")"; //舱位/折扣
                                                outstring += "<br/><a href='javascript:void(0)' " + titlestr + " >退改签</a>";
                                                outstring += "</td>";
                                                //座位数非>9时显示为红色
                                                if (SpaceNum != ">9") {
                                                    outstring += "<td style='width:5%;color:red;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                                }
                                                else {
                                                    outstring += "<td style='width:5%;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                                }
                                                outstring += "<td id = '" + obj[tablename][j]["GUID"] + "cw' isCache=2 " + strTitle + " style='width: 10%;' class='FXCss'>￥" + obj[tablename][j]["XSFee"].split('.')[0] + "</td>";
                                                outstring += "<td id = '" + obj[tablename][j]["GUID"] + "pm' isCache=2 style='width:10%;' class='FXCss'>" + PMFee + "</td>";
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "zc' style='width:12%;" + policyHide + "' class='FXCss'>政策获取中</td>";
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "sj' style='width:8%;" + policyHide + "' class='FXCss'>￥" + sjf + "</td>";

                                                if (bz == "经停") {
                                                    var flryNo = obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"];
                                                    outstring += "<td id = '" + obj[tablename][j]["GUID"] + "jt' style='width: 7%;' >  <a href=\"javascript:tdonmouseover('" + obj[tablename][j]["GUID"] + "','" + obj[tablename][j]["StartDate"] + "','" + flryNo + "')\" >" + bz + "</a></td>"; //经停
                                                }
                                                else {
                                                    outstring += "<td style='width: 7%;'>" + bz + "</td>";  //经停
                                                }
                                                outstring += "<td style='width: 8%;'><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='预订' onclick='InValue(\"" + sItemlist + "\")' /></span></td></tr>";
                                                outstring += "</tr></table>"; //YYY 2013-3-12 NEW ADD
                                            } else {
                                                outstring += "<tr onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\"class='leftliebiao_checi'>";
                                                outstring += "<td style='width: 1%;'></td>";
                                                //航班信息显示
                                                var hb = "";
                                                //起抵信息显示
                                                var qd = "";
                                                //机建信息显示
                                                var jx = "";
                                                if (TJIndex == 0) {
                                                    hb = "<img src=\"../img/air/" + obj[tablename][j]["CarrCode"].toUpperCase() + ".gif\" />" + obj[tablename][j]["Carrier"] + "<br /><b>" + obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"] + "</b>";
                                                    qd = "<b>" + obj[tablename][j]["StartTime"] + "</b> " + obj[tablename][j]["FromCity"] + " " + obj[tablename][j]["dterm"] + "<br /><b>" + obj[tablename][j]["EndTime"] + "</b> " + obj[tablename][j]["ToCity"] + " " + obj[tablename][j]["aterm"];
                                                    jx = obj[tablename][j]["Model"];
                                                }
                                                outstring += "<td style='width: 8%;" + sty + "'>" + hb + "</td>"; //航班信息
                                                outstring += "<td style='width: 10%;'>" + qd + "</td>"; //起抵时间/机场/航站楼
                                                outstring += "<td style='width: 5%;'>" + jx + "</td>"; //机型

                                                outstring += "<td id = '" + obj[tablename][j]["GUID"] + "jjry' jjry='" + obj[tablename][j]["ABFee"] + "|" + obj[tablename][j]["FuelAdultFee"] + "' style='width: 8%;'></td>"; //机建/燃油
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "zctt' style='width: 8%;' title=''><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"] + "</strong>(" + zk + ")"; //舱位/折扣
                                                outstring += "<br/><a href='javascript:void(0)' " + titlestr + " >退改签</a>";
                                                outstring += "</td>";
                                                //座位数非>9时显示为红色
                                                if (SpaceNum != ">9") {
                                                    outstring += "<td style='width:5%;color:red;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                                }
                                                else {
                                                    outstring += "<td style='width:5%;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                                }
                                                outstring += "<td id = '" + obj[tablename][j]["GUID"] + "cw' isCache=1 style='width: 10%;' class='FXCss'>" + xsfee + "</td>"; //舱位价
                                                outstring += "<td id = '" + obj[tablename][j]["GUID"] + "pm' isCache=1 style='width:10%;'class='FXCss'>￥特价</td>";

                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "sppol' style='width:12%;" + policyHide + "' class='FXCss' sppol='null' spaceprice='null' later='null' >政策获取中</td>"; //优惠
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "sjtj' style='width:8%;" + policyHide + "' class='FXCss'>￥" + sjf + "</td>"; //实付价格
                                                if (bz == "经停") {
                                                    var flryNo = obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"];

                                                    outstring += "<td id = '" + obj[tablename][j]["GUID"] + "jt' style='width: 7%;'>  <a href=\"javascript:tdonmouseover('" + obj[tablename][j]["GUID"] + "','" + obj[tablename][j]["StartDate"] + "','" + flryNo + "')\" >" + bz + "</a></td>"; //经停
                                                }
                                                else {
                                                    outstring += "<td style='width: 7%;'>" + bz + "</td>";  //经停
                                                }

                                                var IsShowBtn = "display:none;";
                                                if (obj[tablename][j]["SpecialType"].toString() == "2") {
                                                    IsShowBtn = "display:block;";
                                                }
                                                outstring += "<td style='width: 8%;'><span id='" + obj[tablename][j]["GUID"] + "yd' style='" + IsShowBtn + "' ><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='预订' onclick='setHidId(\"" + obj[tablename][j]["GUID"] + "HidSpecial\");" + msg + "' /></span></span><input type=\"hidden\" id=\"" + obj[tablename][j]["GUID"] + "HidSpecial\"  /></td>";
                                                outstring += "</tr></table>"; //YYY 2013-3-12 NEW ADD
                                            }
                                            TJIndex++;
                                        }
                                        else {
                                            if (NoTJIndex == 0) {
                                                //航班信息显示
                                                var hb = "";
                                                //起抵信息显示
                                                var qd = "";
                                                //机建信息显示
                                                var jx = "";
                                                if (TJIndex == 0) {
                                                    hb = "<img src=\"../img/air/" + obj[tablename][j]["CarrCode"].toUpperCase() + ".gif\" />" + obj[tablename][j]["Carrier"] + "<br /><b>" + obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"] + "</b>";
                                                    qd = "<b>" + obj[tablename][j]["StartTime"] + "</b> " + obj[tablename][j]["FromCity"] + " " + obj[tablename][j]["dterm"] + "<br /><b>" + obj[tablename][j]["EndTime"] + "</b> " + obj[tablename][j]["ToCity"] + " " + obj[tablename][j]["aterm"];
                                                    jx = obj[tablename][j]["Model"];
                                                }
                                                outstring += "<tr onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\"class='leftliebiao_checi'>";
                                                outstring += "<td style='width: 1%;'></td>";

                                                outstring += "<td style='width: 8%;" + sty + "'>" + hb + "</td>"; //航班信息
                                                outstring += "<td style='width: 10%;'>" + qd + "</td>"; //起抵时间/机场/航站楼
                                                outstring += "<td style='width: 5%;'>" + jx + "</td>"; //机型
                                                outstring += "<td style='width: 8%;'>" + obj[tablename][j]["ABFee"] + "<br />" + obj[tablename][j]["FuelAdultFee"] + "</td>"; //机建/燃油
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "zct' style='width: 8%;'  title='' ><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"] + "</strong>(" + zk + ")"; //舱位/折扣
                                                outstring += "<br/><a href='javascript:void(0)' " + titlestr + " >退改签</a>";
                                                outstring += "</td>";
                                                //座位数非>9时显示为红色
                                                if (SpaceNum != ">9") {
                                                    outstring += "<td style='width:5%;color:red;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                                }
                                                else {
                                                    outstring += "<td style='width:5%;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                                }
                                                outstring += "<td " + strTitle + " style='width: 10%;' class='FXCss'>" + xsfee + "<br/><a href='javascript:showMoreInfo(" + i + ")' style=\"font-weight:normal;\">更多舱位</a></td>"; //舱位价
                                                outstring += "<td style='width:10%' class='FXCss'>" + PMFee + "</td>";
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "zc' style='width:12%;" + policyHide + "' class='FXCss'>政策获取中</td>"; //优惠
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "sj' style='width:8%;" + policyHide + "' class='FXCss'>￥" + sjf + "</td>"; //实付价格
                                                if (bz == "经停") {
                                                    var flryNo = obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"];

                                                    outstring += "<td id = '" + obj[tablename][j]["GUID"] + "jt' style='width: 7%;' >  <a href=\"javascript:tdonmouseover('" + obj[tablename][j]["GUID"] + "','" + obj[tablename][j]["StartDate"] + "','" + flryNo + "')\" >" + bz + "</a></td>"; //经停
                                                }
                                                else {
                                                    outstring += "<td style='width: 7%;'>" + bz + "</td>";  //经停
                                                }
                                                outstring += "<td style='width: 8%;'><span class=\"btnen btnen-ok-s\" id='spCZ' ><input id='btnCreate' type='button' value='预订' onclick='" + msg + "' /></span></td>";
                                                outstring += "</tr></table>";
                                                NoTJIndex++;
                                                outstring += " <div id='div_" + i + "' class='TempDiv'>";
                                                outstring += " <table  class='tab2'>";
                                            }

                                            else {
                                                outstring += "<tr onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\"class='leftliebiao_checi' ><td style='width: 1%;'></td><td style='width: 8%;'></td><td style='width: 10%;'></td><td style='width: 5%;'></td><td style='width: 8%;'></td>";

                                                outstring += "<td  id='" + obj[tablename][j]["GUID"] + "zct'  title='' style='width: 8%;'><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"] + "</strong>（" + zk + "）";
                                                outstring += "<br/><a href='javascript:void(0)' " + titlestr + " >退改签</a></td>";
                                                if (SpaceNum != ">9") {
                                                    outstring += "<td style='width: 5%;color:red;'>" + obj[tablename][j]["TickNum"] + "</td>";
                                                }
                                                else {
                                                    outstring += "<td style='width: 5%;'>" + obj[tablename][j]["TickNum"] + "</td>";
                                                }
                                                outstring += "<td " + strTitle + " style='width: 10%;' class='FXCss'>￥" + obj[tablename][j]["XSFee"].split('.')[0] + "</td>";
                                                outstring += "<td style='width:10%;' class='FXCss'>" + PMFee + "</td>";
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "zc' style='width:12%;" + policyHide + "' class='FXCss'>政策获取中</td>";
                                                outstring += "<td id='" + obj[tablename][j]["GUID"] + "sj' style='width:8%;" + policyHide + "' class='FXCss'>￥" + sjf + "</td>";
                                                outstring += "<td style='width: 7%;'></td><td style='width: 8%;'><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='预订' onclick='InValue(\"" + sItemlist + "\")' /></span></td></tr>";
                                            }

                                        }

                                    }
                                    //if (NoTJIndex!=0) {
                                    outstring += "</table>";
                                    outstring += "</div>";
                                    //}

                                    jQueryOne("#" + showAirInfo).append(outstring);
                                    outstring = "";
                                }
                            }

                            airInfoIsOk = true;
                        } else {
                            airInfoIsOk = false;
                            jQueryOne("#" + showAirInfo).html("<font style=\"font-size:18px;font-weight:bold;color:red;\">航班数据解析失败,请重试或联系管理员</font>");
                        }
                    } else {
                        airInfoIsOk = false;
                    }
                }
                catch (e) {
                    count;
                    airInfoIsOk = false;
                    jQueryOne("#" + showAirInfo).html("<font style=\"font-size:18px;font-weight:bold;color:red;\">航班数据解析失败,请重试或联系管理员</font>");
                }
            }
            //清除查询提示
            showts2("");
            //解析成功
            if (airInfoIsOk) {
                jQueryOne("#divShowWeek").show();
                jQueryOne.get("../AJAX/GetPolicyInfo.ashx", {
                    StartCityNameCode: jQueryOne("#hidFCityCode").val(),
                    MiddCityNameCode: jQueryOne("#hidMCityCode").val(),
                    TargetCityNameCode: jQueryOne("#hidTCityCode").val(),
                    FromDate: jQueryOne("#txtFromDate").val(),
                    ReturnDate: jQueryOne("#txtReturnDate").val(),
                    TravelType: jQueryOne("#Hid_travel").val(),
                    chaceNameByGUID: chaceNameByGUID,
                    IsOrGetPolicy: "true",
                    GroupId: jQueryOne("#Hid_GroupId").val(),
                    OrderID: "",
                    HavaChild: "false",
                    IsINF: "false",
                    random: Math.random(),
                    currentuserid: '<%=this.mUser.id.ToString() %>'
                },
               function (json) {
                   getPolicyInfo(json);
               });
            }
        }

        function getPolicyInfo(json) {
            if (json != "") {
                var obj = eval('(' + json + ')')//转换航班JSON
                var i = 0;
                try {
                    for (var tablename in obj) {//循环所有的表
                        i++;
                        var objtablenamelen = obj[tablename].length; //行的数量
                        for (var j = 0; j < objtablenamelen; j++) {
                            if (obj[tablename][j]["GUID"] == undefined) {
                                continue;
                            }
                            //是否高返
                            var HighPolicyFlag = "";
                            if (obj[tablename][j]["HighPolicyFlag"] == 1) {
                                HighPolicyFlag = "<br /><span style='color:Red;font-weight:bolder;'>高返</span>"
                            }
                            var houFanImg = "<br />" + GetString(obj[tablename][j]["LaterPoint"]);
                            //政策佣金
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "zc").html(obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"] + "/" + obj[tablename][j]["Commission"] + HighPolicyFlag + houFanImg);
                            //鼠标移到舱位显示政策(普通)
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "zct").attr("title", obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"] + "/" + obj[tablename][j]["Commission"] + "/" + obj[tablename][j]["SJFee"]);
                            //鼠标移到舱位显示政策(特价)
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "zctt").attr("title", obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"] + "/待计算");

                            //实际支付金额
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "sj").html("￥" + obj[tablename][j]["SJFee"]);
                            //特价舱位政策现返隐藏在属性中,待获取价格后计算
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "sppol").attr("sppol", obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"]);
                            //特价舱位后返隐藏在属性中,待获取价格后显示
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "sppol").attr("later", obj[tablename][j]["LaterPoint"]);

                            //特价舱位政策显示为(政策%+现返/待计算)
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "sppol").html(obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"] + "/待计算");

                            //如果是固定特价类型,则赋值属性固定特价价格,基建,燃油,用于发送到后台计算
                            if (obj[tablename][j]["GenerationType"] == "3") {

                                if (jQueryOne("#" + obj[tablename][j]["GUID"] + "cw").attr("isCache") == 1) {
                                    var jjty = jQueryOne("#" + obj[tablename][j]["GUID"] + "jjry").attr("jjry");
                                    jQueryOne("#" + obj[tablename][j]["GUID"] + "sppol").attr("spaceprice", obj[tablename][j]["SeatPrice"] + "|" + jjty);
                                }
                                if (jQueryOne("#" + obj[tablename][j]["GUID"] + "cw").attr("isCache") == 2) {
                                    jQueryOne("#" + obj[tablename][j]["GUID"] + "cw").html("￥" + obj[tablename][j]["SeatPrice"]);
                                    var f = parseFloat(obj[tablename][j]["SeatPrice"]) + parseFloat(obj[tablename][j]["ABFare"]) + parseFloat(obj[tablename][j]["RQFare"]);
                                    jQueryOne("#" + obj[tablename][j]["GUID"] + "pm").html("￥" + f.toString().split('.')[0]);
                                }

                            }
                        }
                    }
                } catch (e) {
                    showdialogmsg("政策数据解析失败,请重试或联系管理员");
                }
            } else {
                showdialogmsg("政策数据获取失败,请重新尝试");
            }
        }
        function GetString(val) {
            var month = 0;
            var star = 0;
            if (val.indexOf(".") != -1) {
                var temp = val;
                val = val.substr(0, val.indexOf("."));
                var shi = parseInt(temp.substr(temp.indexOf(".") + 1, 1), 10);
                month = parseInt(shi / 5, 10);
                star = parseInt(shi % 5, 10);
            }
            var sun = parseInt(val, 10);
            var LevelArr = [];
            for (var i = 0; i < parseInt(sun, 10); i++) {
                LevelArr.push('<img src=\"../img/Level/sun.jpg\" />');
            }
            for (var i = 0; i < parseInt(month, 10); i++) {
                LevelArr.push('<img src=\"../img/Level/month.jpg\" />');
            }
            for (var i = 0; i < parseInt(star, 10); i++) {
                LevelArr.push('<img src=\"../img/Level/star.jpg\" />');
            }
            return LevelArr.join("");
        }
    </script>
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
    <div id="overlay" style="display: none;">
    </div>
    <div id="loading" runat="server" style="display: none; background: url(../images/mainbg.gif) no-repeat;">
        请稍等，<br />
        您查询的航班正在搜索中……<br />
        <strong style="color: Red;">价格以PATA为准</strong><br />
        <strong style="color: Red;">政策返点以订单确认返点为准</strong><br />
        <img src="../img/loading.gif">
    </div>
    <form id="form1" name="form1" runat="server">
    <div class="listing">
        <div class="normal-content" style="padding: 10px;">
            <div class="titleheight">
                <h3 style="display: none" id="h31" runat="server">
                    航班信息（<asp:Label ID="lblSky" runat="server"></asp:Label>）<font>共<asp:Label ID="lblnumt"
                        runat="server"></asp:Label>个</font>,<strong style="color: Red; font-size: 12px;">价格以PATA为准，政策返点以订单确认返点为准</strong></h3>
                <h3 style="display: block" id="h32" runat="server">
                    <strong>机票快捷预订</strong></h3>
                <ul>
                    <li><em class="current"></em>预订</li>
                    <li><em class="bg"></em>填写订单</li>
                    <li><em class="bg"></em>确定</li>
                    <li><em class="bg"></em>支付</li>
                    <li><em></em>完成</li>
                </ul>
            </div>
            <div id=".tabs">
                <table cellspacing="0" cellpadding="0" border="0" id="gnjd" width="100%" class="kuaijie">
                    <tr>
                        <td style="width: 90px; padding-left: 5px; font-size: 14px; color: #505050; font-weight: bold;">
                            承 运 人 ：
                        </td>
                        <td width="170" id="td_left" style="text-align: left; padding-left: 15px;">
                            <uc1:SelectAirCode ID="ddlCarrier" runat="server" IsDShowName="false" DefaultOptionText="所有航空公司"
                                DefaultOptionValue="" />
                        </td>
                        <td colspan="4" style="text-align: left;">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="padding-left: 5px; border-right: ">
                                        <label for="travelOne">
                                            <input type="radio" checked="true" id="travelOne" value="1" onclick="flyType(1)"
                                                name="travelGroup" />单程</label>
                                        <label for="travelReruen">
                                            <input type="radio" id="travelReruen" value="2" onclick="flyType(2)" name="travelGroup" />往返</label>
                                        <label for="travelTheWay">
                                            <input type="radio" id="travelTheWay" value="3" onclick="flyType(3)" name="travelGroup" />联程</label>
                                    </td>
                                    <td>
                                        <div id="divIsShowShare" style="display: none;" runat="server">
                                            &nbsp;
                                            <asp:CheckBox ID="cbIsShowShare" runat="server" Text="不显示共享航班"></asp:CheckBox>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px; padding-left: 5px; font-size: 14px; color: #505050; font-weight: bold;">
                            出发城市：
                        </td>
                        <td width="170">
                            <%--出发城市开始--%>
                            <input name="txtFromCityCode" class="inputtxtdat" runat="server" type="text" id="txtFromCityCode"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 150px;" mod_address_reference="hidFCityCode" />
                            <input type="hidden" name="hidFCityCode" id="hidFCityCode" runat="server" />
                            <%--出发城市结束--%>
                        </td>
                        <td id="Conntitle" style="width: 90px; padding-left: 5px; font-size: 14px; color: #505050;
                            display: none; font-weight: bold;">
                            中转城市：
                        </td>
                        <td id="midCityContainer" style="display: none;" width="170">
                            <%--中转城市开始--%>
                            <input name="txtMidCityCode" class="inputtxtdat" runat="server" type="text" id="txtMidCityCode"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 150px;" mod_address_reference="hidMCityCode" />
                            <input type="hidden" name="hidMCityCode" id="hidMCityCode" runat="server" />
                            <%--中转城市结束--%>
                        </td>
                        <td colspan="4">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px; padding-left: 5px; font-size: 14px; color: #505050; font-weight: bold;">
                            到达城市：
                        </td>
                        <td width="170">
                            <%--到达城市开始--%>
                            <input name="txtToCityCode" class="inputtxtdat" runat="server" type="text" id="txtToCityCode"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 150px;" mod_address_reference="hidTCityCode" />
                            <input type="hidden" name="hidTCityCode" id="hidTCityCode" runat="server" />
                            <%--到达城市结束--%>
                        </td>
                        <td colspan="4">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px; padding-left: 5px; font-size: 14px; color: #505050; font-weight: bold;">
                            出发日期：
                        </td>
                        <td width="170">
                            <input type="text" readonly="readonly" id="txtFromDate" runat="server" class="Wdate inputtxtdat"
                                style="width: 150px" onchange="setDate(this,0)" onfocus="setDate(this,1)" />
                        </td>
                        <td id="tdRerurnTime1" style="width: 90px; padding-left: 5px; font-size: 14px; display: none;
                            color: #505050; font-weight: bold;">
                            <span id="SecondTime">返回日期</span>：
                        </td>
                        <td id="tdRerurnTime2" width="170">
                            <div id="rtnReturnDate" style="display: none">
                                <input type="text" readonly="readonly" id="txtReturnDate" runat="server" class="Wdate inputtxtdat"
                                    style="width: 150px" onfocus="getDate()" />
                            </div>
                        </td>
                        <td class="Adaptive" style="text-align: left">
                            <asp:Button runat="server" ID="btnQuery" CssClass="btn" Text="查询航班" OnClick="btnQuery_Click"
                                OnClientClick="return queryCheck();"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button runat="server" ID="btnQueryTeam" CssClass="btn" Text="查询散冲团"  style="display: none" OnClientClick="return queryCheck();"
                                OnClick="btnQueryTeam_Click"></asp:Button>
                        </td>
                    </tr>
                </table>
            </div>
            <%-- 星期选择 开始 --%>
            <div class="week-tickets" id="divShowWeek" style="display: none">
                <table class="datetable">
                    <tbody>
                        <tr>
                            <td id="td1">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="listl">
                                        </td>
                                        <td class="listc">
                                            <asp:Label ID="lbl1" runat="server" Text=""></asp:Label>
                                            <br />
                                            <div id="div1_1">
                                                <div id="div1_2">
                                                    <asp:LinkButton ID="lbtn1" runat="server" OnClick="lbtn1_Click" OnClientClick="return queryCheck();"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="listr">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="td2">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="listl">
                                        </td>
                                        <td class="listc">
                                            <asp:Label ID="lbl2" runat="server" Text=""></asp:Label><br />
                                            <div id="div2_1">
                                                <div id="div2_2">
                                                    <asp:LinkButton ID="lbtn2" runat="server" OnClick="lbtn2_Click" OnClientClick="return queryCheck();"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="listr">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="td3">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="listl">
                                        </td>
                                        <td class="listc">
                                            <asp:Label ID="lbl3" runat="server" Text=""></asp:Label><br />
                                            <div id="div3_1">
                                                <div id="div3_2">
                                                    <asp:LinkButton ID="lbtn3" runat="server" OnClick="lbtn3_Click" OnClientClick="return queryCheck();"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="listr">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="td4">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="listl">
                                        </td>
                                        <td class="listc">
                                            <asp:Label ID="lbl4" runat="server" Text=""></asp:Label><br />
                                            <div id="div4_1">
                                                <div id="div4_2">
                                                    <asp:LinkButton ID="lbtn4" runat="server" OnClick="lbtn4_Click" OnClientClick="return queryCheck();"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="listr">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="td5">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="listl">
                                        </td>
                                        <td class="listc">
                                            <asp:Label ID="lbl5" runat="server" Text=""></asp:Label><br />
                                            <div id="div5_1">
                                                <div id="div5_2">
                                                    <asp:LinkButton ID="lbtn5" runat="server" OnClick="lbtn5_Click" OnClientClick="return queryCheck();"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="listr">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="td6">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="listl">
                                        </td>
                                        <td class="listc">
                                            <asp:Label ID="lbl6" runat="server" Text=""></asp:Label><br />
                                            <div id="div6_1">
                                                <div id="div6_2">
                                                    <asp:LinkButton ID="lbtn6" runat="server" OnClick="lbtn6_Click" OnClientClick="return queryCheck();"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="listr">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="td7">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                    <tr>
                                        <td class="listl">
                                        </td>
                                        <td class="listc">
                                            <asp:Label ID="lbl7" runat="server" Text=""></asp:Label><br />
                                            <div id="div7_1">
                                                <div id="div7_2">
                                                    <asp:LinkButton ID="lbtn7" runat="server" OnClick="lbtn7_Click" OnClientClick="return queryCheck();"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="listr">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <%-- 星期选择 结束 --%>
            <%-- 散冲团开始--%>
            <div id="showGroupInfo">
            </div>
            <%-- 散冲团结束--%>
            <%-- 显示航班数据 开始 --%>
            <div id="AirListDiv" class="result" style="margin: 0px;">
                <%--存储隐藏域ID--%>
                <input type="hidden" id="Hid_HidId" />
                <input type="hidden" id="hidTjValue" runat="server" name="hidTjValue" value="" />
                <%--传过来的数据--%>
                <%--行程类型--%>
                <input type="hidden" id="Hid_travel" runat="server" name="Hid_travel" value="1" />
                <%--分组ID--%>
                <input type="hidden" id="Hid_GroupId" runat="server" name="Hid_GroupId" value="" />
                <%--航班信息--%>
                <input type="hidden" id="FlyInfo" runat="server" name="FlyInfo" />
                <%--航班信息--%>
                <input type="hidden" id="FlyInfoBack" runat="server" name="FlyInfoBack" />
                <%--隐藏返点(true隐藏)--%>
                <input type="hidden" id="hidePolicy" runat="server" name="hidePolicy" value="False" />
                <%--隐藏返点(true隐藏)--%>
                <input type="hidden" id="hidRoleType" runat="server" name="hidRoleType" value="1" />
                <%--是否有共享航班1段--%>
                <input type="hidden" id="isGX" runat="server" name="isGX" value="0" />
                <%--是否有共享航班2段--%>
                <input type="hidden" id="isGX2" runat="server" name="isGX2" value="0" />
                <div id="showAirInfo">
                </div>
                <div id="showAirInfoBack">
                </div>
                <div id="backYuDing" style="text-align: center">
                </div>
                <div id="showDiv" runat="server">
                </div>
                <div id="showDivBack" runat="server">
                </div>
            </div>
            <%-- 显示航班数据 开始 --%>
        </div>
    </div>
    </form>
    <form name="Order" method="post" action="Create.aspx">
    <input id="Carryer" type="hidden" name="Carryer" />
    <input id="FlyNo" type="hidden" name="FlyNo" />
    <input id="Aircraft" type="hidden" name="Aircraft" />
    <input id="Time" type="hidden" name="Time" />
    <input id="City" type="hidden" name="City" />
    <input id="ABFee" type="hidden" name="ABFee" />
    <input id="FuelAdultFee" type="hidden" name="FuelAdultFee" />
    <input id="DiscountRate" type="hidden" name="DiscountRate" />
    <input id="TickNum" type="hidden" name="TickNum" />
    <input id="XSFee" type="hidden" name="XSFee" />
    <input id="FareFee" type="hidden" name="FareFee" />
    <input id="Mileage" type="hidden" name="Mileage" />
    <input id="Cabin" type="hidden" name="Cabin" />
    <input id="Reservation" type="hidden" name="Reservation" />
    <input id="SpecialType" type="hidden" name="SpecialType" />
    <input id="TravelType" type="hidden" name="TravelType" />
    <input id="Terminal" type="hidden" name="Terminal" />
    <input id="ReturnTime" type="hidden" name="ReturnTime" />
    <input id="hidcity" type="hidden" name="hidcity" />
    <input id="hidOrderByStr" type="hidden" runat="server" value="desc" />
    <input id="hidIsShowShare" type="hidden" runat="server" />
    <input type="hidden" value="" runat="server" id="hidTime" />
    <input id="currentuserid" type="hidden" name="currentuserid" value="<%=this.mUser.id.ToString() %>" />
    </form>
    <%--排序查询、经停查询、特价查询--%>
    <script type="text/javascript" language="javascript">
        // 创建xmlhttprequest,ajax开始
        function creatReq(url, type) {

            var req; //定义变量，用来创建xmlhttprequest对象
            var valUrl = url + "&random=" + Math.ceil(Math.random() * 35 + "&currentuserid=" + document.getElementById("currentuserid").value);
            if (window.XMLHttpRequest) { //非IE浏览器，用xmlhttprequest对象创建
                req = new XMLHttpRequest();
            }
            else if (window.ActiveXObject) { //IE浏览器用activexobject对象创建
                req = new ActiveXObject("Microsoft.XMLHttp");
            }
            if (req) {//成功创建xmlhttprequest
                req.open("GET", valUrl, true); //与服务端建立连接(请求方式post或get，地址,true表示异步)
                //req.onreadystatechange = callback; //指定回调函数

                req.onreadystatechange = function () {
                    if (req.readyState == 4) {//请求状态为4表示成功
                        if (req.status == 200) {//http状态200表示OK
                            if (type == "1") {
                                printList(req); //显示排序
                            } else if (type == "2") {
                                ReturnTdOnMouseOver(req); //显示经停
                            } else if (type == "3") {
                                ReturnSpecial(req); //显示特价
                            } else if (type == "4") {
                                ReturnSpecialBack(req); //显示往返联成特价
                            }
                        }
                    }
                }
                req.send(null); //发送请求   
            }
        }
        //查询排序
        function jsOrderBy(fname) {
            if (document.getElementById("hidOrderByStr").value != "") {
                var jurl = "../../Ajax/FlightAjax.aspx?&sdate=" + Date().toString() + "&type=" + document.getElementById("hidOrderByStr").value + "&filename=" + fname + "&currentuserid=" + document.getElementById("currentuserid").value;
                if (document.getElementById("hidOrderByStr").value == "desc") {
                    document.getElementById("hidOrderByStr").value = "asc";
                }
                else {
                    document.getElementById("hidOrderByStr").value = "desc";
                }
                creatReq(jurl, 1);
            }
        }
        //返回排序数据
        function printList(req) {
            var temptext = req.responseText; //获取数据
            if (temptext != "") { //查询显示数据
                document.getElementById("showDiv").innerHTML = temptext;
            }
            else { }
        }
        var tdobj = null;
        var tdTjObj = null;
        //查询经停
        function tdonmouseover(guid, sTime, flyNo) {
            var turl = "../Ajax/SelStopsAjax.aspx?guid=" + guid + "&stime=" + sTime + "&flyno=" + flyNo + "&currentuserid=" + document.getElementById("currentuserid").value;
            var obj = document.getElementById(guid + "jt");
            if (obj.issend != undefined) {
                //已有数据
                //alert(tdobj.title);
                //obj.title = obj["issend"].toString();

            } else {
                //tdobj = obj;
                obj.innerHTML = "经停<br/><font style=\"font-size:12px;\">获取中...</font)";
                obj.issend = 1;
                //creatReq(turl + "&sdate=" + Date().toString());

                creatReq(turl + "&sdate=" + Math.random().toString(), 2);

            }
        }
        //返回经停数据
        function ReturnTdOnMouseOver(req) {
            var temps = req.responseText; //获取数据
            var tempInfo = temps.split('|')[0];
            var tempGuid = temps.split('|')[1];
            if (tempGuid != "" && tempInfo != "") {
                document.getElementById(tempGuid + "jt").innerHTML = "经停:<font style=\"font-size:12px;\">" + tempInfo + "</font)";
                //tdobj.title = temp;
                //tdobj["issend"] = temp;
            } else {
                document.getElementById(tempGuid + "jt").innerHTML = "经停<br/><font style=\"font-size:12px;\">获取失败</font)";
            }
        }
        var getSpecialUrl = "";
        var getSpecialVal = "";

        //查询特价
        function GetSpecial(obj, val, guid) {
            if (jQueryOne("#" + guid + "sppol").attr("sppol") != "null") {
                tdTjObj = obj;
                var gurl = "../Ajax/SpPatAjax.aspx?val=" + val + "|" + jQueryOne("#" + guid + "sppol").attr("sppol") + "|" + jQueryOne("#" + guid + "sppol").attr("spaceprice");
                if ((getSpecialUrl != "" && getSpecialUrl == gurl && getSpecialVal == "") || obj.value == "无运价") {
                    obj.value = "无运价";
                    showdialogmsg("无运价！");
                } else {

                    getSpecialUrl = gurl;
                    //显示查询
                    //document.getElementById("loading").innerHTML = "请稍等，<br />特价价格搜索中……<br /><img src='../img/loading.gif'>";
                    document.getElementById("overlay").style.display = "block";
                    document.getElementById("loading").style.display = "block";
                    creatReq(gurl + "&sdate=" + Date().toString() + "&currentuserid=" + jQueryOne("#currentuserid").val(), 3);
                }
            }
            else {
                showdialogmsg("政策未获取成功,请等待政策获取成功再获取价格！");
            }


        }
        //返回特价数据
        function ReturnSpecial(req) {
            document.getElementById("overlay").style.display = "none";
            document.getElementById("loading").style.display = "none";
            //document.getElementById("loading").innerHTML = "请稍等，<br />您查询的航班正在搜索中……<br /><img src='../img/loading.gif'>";
            getSpecialVal = req.responseText;
            if (getSpecialVal != null && getSpecialVal != "") {
                var arr = getSpecialVal.split('@');
                try {
                    jQueryOne("#" + arr[6] + "cw").html(arr[0]); //舱位
                    jQueryOne("#" + arr[6] + "jjry").html(arr[1].split('.')[0] + "<br />" + arr[2].split('.')[0]); //基建燃油
                    jQueryOne("#" + arr[6] + "pm").html(arr[3]); //票面价
                    var tempPolicyAndReturn = jQueryOne("#" + arr[6] + "sppol").attr("sppol"); //政策+现返
                    var tempYJ = arr[5]; //佣金
                    var tempLaterMoney = GetString(jQueryOne("#" + arr[6] + "sppol").attr("later")); //后返
                    jQueryOne("#" + arr[6] + "sppol").html(tempPolicyAndReturn + "/" + tempYJ + "<br />" + tempLaterMoney); //政策
                    jQueryOne("#" + arr[6] + "sjtj").html(arr[4]); //实付金额
                    jQueryOne("#" + arr[6] + "yd").show(); //预订显示
                    jQueryOne("#" + arr[6] + "HidSpecial").val(arr[0] + "|" + arr[1].split('.')[0] + "|" + arr[2].split('.')[0]); //特价的基建,燃油,舱位,票面价,实付金额
                    jQueryOne("#" + arr[6] + "zctt").attr("title", tempPolicyAndReturn + "/" + tempYJ + "/" + arr[4]); //鼠标移动到舱位显示政策现返佣金实付价格
                } catch (e) {

                }
            } else {
                tdTjObj.value = "无运价";
                showdialogmsg("无运价");
            }
        }
        var tempsItemlist = ""; //记录特价时候往返联乘航班信息的临时全局变量
        var tempshowAirInfo = ""; //记录特价时候往返联乘去程还是返程的临时全局变量
        var tempI = "";
        //查询往返联成特价
        function GetSpecialBack(guid, val, YPrice, i, sItemlist, showAirInfo) {
            tdTjObj = jQuery("#" + guid + "_A");
            //因为联成查询页面无法匹配政策,所以不能看到固定特价
            var gurl = "../Ajax/SpPatAjax.aspx?val=" + val + "|0%||||" + YPrice;
            //tdTjObj.isck(0--未点击,1--已经获取,2--无运价
            if ((getSpecialUrl != "" && getSpecialUrl == gurl && getSpecialVal == "") || tdTjObj.attr("isck") == "2") {
                showdialogmsg("无运价！");
            }
            else if (tdTjObj.attr("isck") == "1") {
                showdialogmsg("已经获取此舱位价格,请勿重复点击！");
            } else {

                tempsItemlist = sItemlist;
                tempshowAirInfo = showAirInfo;
                tempI = i;
                getSpecialUrl = gurl;
                //显示查询
                //document.getElementById("loading").innerHTML = "请稍等，<br />特价价格搜索中……<br /><img src='../img/loading.gif'>";
                document.getElementById("overlay").style.display = "block";
                document.getElementById("loading").style.display = "block";
                creatReq(gurl + "&sdate=" + Date().toString() + "&currentuserid=" + jQuery("#currentuserid").val(), 4);
            }
        }
        //返回往返联成的特价
        function ReturnSpecialBack(req) {
            document.getElementById("overlay").style.display = "none";
            document.getElementById("loading").style.display = "none";
            //document.getElementById("loading").innerHTML = "请稍等，<br />您查询的航班正在搜索中……<br /><img src='../img/loading.gif'>";
            getSpecialVal = req.responseText;
            if (getSpecialVal != null && getSpecialVal != "") {
                var arr = getSpecialVal.split('@');
                try {
                    var Againlists = tempsItemlist.split('|');
                    var counttemplists = Againlists.length;
                    tempsItemlist = ""; //清空字符串重新拼接
                    //特价获取基建燃油舱位价后需重新拼接字符串
                    for (var i = 0; i < counttemplists; i++) {
                        //5,6,9
                        if (i == counttemplists) {
                            tempsItemlist += Againlists[i];
                        } else {
                            if (i == 5) {//基建
                                tempsItemlist += arr[1] + "|";
                            }
                            else if (i == 6) {//燃油
                                tempsItemlist += arr[2] + "|";
                            }
                            else if (i == 9) {//舱位价
                                tempsItemlist += arr[0] + "|";
                            } else {
                                tempsItemlist += Againlists[i] + "|";
                            }
                        }
                    }
                    //销售价
                    var xsfee = "￥" + arr[0];
                    var msg2 = "saveBackInfo('" + tempsItemlist + "', '" + tempshowAirInfo + "','0');";
                    //tempDiscount8 += "<input type=\"radio\" title='" + xsfee + "' name=\"backGroup" + showAirInfo + "\" value=\"" + obj[tablename][j]["GUID"] + "\" onclick=\"" + msg + "\" /><span class=\"cang\">" + obj[tablename][j]["Space"].toString() + "</span>" + "<span class=\"red\">/" + obj[tablename][j]["DiscountRate"] + "</span><br />";
                    var SpecialInput = "<input type=\"radio\"   title='" + xsfee + "'  name=\"backGroup" + tempshowAirInfo + "\" value=\"" + arr[6] + "\" onclick=\"" + msg2 + "\" /><span class=\"cang\">" + tdTjObj.text() + "</span><span class=\"red1\">/" + arr[7] + "</span><br />";
                    var flag = discountArea(arr[7]); //折扣
                    switch (flag) {
                        case 1:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_100").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_100").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_100").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_100").append(tempHtml);
                            tdTjObj.attr("isck", "1");
                            break;
                        case 2:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_90").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_90").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_90").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_90").append(tempHtml);
                            tdTjObj.attr("isck", "1");
                            break;
                        case 3:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_80").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_80").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_80").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_80").append(tempHtml);
                            tdTjObj.attr("isck", "1");
                            break;
                        case 4:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_70").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_70").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_70").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_70").append(tempHtml);
                            tdTjObj.attr("isck", "1");
                            break;
                        case 5:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_60").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_60").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_60").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_60").append(tempHtml);
                            tdTjObj.attr("isck", "1");
                            break;
                        case 6:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_50").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_50").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_50").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_50").append(tempHtml);
                            tdTjObj.attr("isck") == "1";
                            break;
                        case 7:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_40").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_40").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_40").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_40").append(tempHtml);
                            tdTjObj.attr("isck", "1");
                            break;
                        case 8:
                            var tempHtml = jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_30").html();
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_30").html("");
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_30").html(SpecialInput);
                            jQueryOne("#tdBack" + tempshowAirInfo + tempI + "_30").append(tempHtml);
                            tdTjObj.attr("isck", "1");
                            break;
                        default:
                            tdTjObj.attr("isck", "2");
                            showdialogmsg("无运价");
                            break;
                    }
                } catch (e) {
                    tdTjObj.attr("isck", "2");
                    showdialogmsg("无运价");
                }
            } else {
                tdTjObj.attr("isck", "2");
                showdialogmsg("无运价");
            }
        }
    </script>
    <%--排序 和 经停--%>
    <script type="text/javascript" language="javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd = ""; //  "?r=" + Math.random();
            var SE = new CtripJsLoader();
            var files = [["../JS/CitySelect/tuna_100324.js" + rd, "GB2312", true, null], ["../AJAX/GetCity.aspx" + rd, "GB2312", true, null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        jQueryOne(function () {
            //OptionSort("ddlCarrier_ddl_AirCode");
            ReLoad();
        });  
    </script>
</body>
</html>
