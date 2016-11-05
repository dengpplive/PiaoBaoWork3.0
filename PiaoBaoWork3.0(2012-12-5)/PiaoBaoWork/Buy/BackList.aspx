<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BackList.aspx.cs" Inherits="Buy_BackList" %>

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
    <script src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"  id="script_k0"  charset="utf-8"></script>
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
            font-size: 12px;
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
           #ReturnDiv
        {
            border: 1px solid #d7d7d7;
            background-color: #f6f6f6;
            margin-bottom: 10px;
            padding: 6px;
        }
        #ReturnDiv table
        {
            width: 100%;
            background-color: White;
        }
        #ReturnDiv .td1
        {
            width: 80px;
            height: 30px;
        }
        #ReturnDiv .td2
        {
            width: 15%;
            height: 30px;
        }
        #ReturnDiv .td3
        {
            width: 15%;
            height: 30px;
            color: #DF681F;
            font-weight: bold;
        }
        #ReturnDiv h5
        {
            background: url("../img/bg_32x32.gif") no-repeat scroll 0 3px transparent;
            color: #FF6600;
            float: left;
            font-size: 15px;
            padding-left: 35px;
            line-height: 40px;
            height: 40px;
        }
        #ReturnDiv .td1
        {
        }
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
                        location.href = "OrderChangeList.aspx?currentuserid=" + jQueryOne("#currentuserid").val();
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
        function FI(str) {
            jQueryOne("#ReturnDiv").html(str);
        }
        function showMoreInfo(m) {
            jQueryOne("#div_" + m).toggle(100);
        }
        function InsertHtmls(hmt) {
            jQueryOne("#AirListDiv").html(hmt);
        }
        //为预订按钮后面隐藏域复制
        function setHidId(hid) {
            jQueryOne("#Hid_HidId").val(hid);
        }
        function InValue(s) {
            var sInput = s.split('|');
            var info = jQueryOne("#FlyInfo").val().toString().split('|');
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

            //动态 特价时
            if (sInput[14] == 1) {
                //获取价格
                var tjValue = jQueryOne("#" + jQueryOne("#Hid_HidId").val()).val();
                //jQueryOne("#hidTjValue").val();
                if (tjValue != "") {
                    var valStrS = tjValue.split('|');
                    // tjValue: 舱位价|基建|燃油|票价|实付价格
                    jQueryOne("#ABFee").val(valStrS[1]); //基建
                    jQueryOne("#FuelAdultFee").val(valStrS[2]); //燃油
                    jQueryOne("#XSFee").val(valStrS[0]); //舱位价
                  
                }
            }
            jQueryOne("#TravelType").val(info[15]);
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
      
 

     
       
     
        function getColumnTitle() {
            var outstring = "";
            outstring += "<table class=\"tab1\">";
            outstring += "<tr>";
            outstring += " <td style=\"width: 1%;\">";
            outstring += " </td>";
            outstring += "<td style=\"width: 8%; cursor: pointer; text-decoration: underline; color: Blue;\"";
            outstring += " onclick=\"jsOrderBy('CarrCode')\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">航班信息</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;\"";
            outstring += " onclick=\"jsOrderBy('StartTime')\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">起抵时间/机场</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 5%;\">";
            outstring += "机型";
            outstring += "</td>";
            outstring += "<td style=\"width: 8%;\">";
            outstring += "机建/燃油";
            outstring += "</td>";
            outstring += "<td style=\"width: 8%; cursor: pointer; text-decoration: underline; color: Blue;\"";
            outstring += " onclick=\"jsOrderBy('DiscountRate')\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">舱位/折扣</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 5%;\">";
            outstring += "座位数";
            outstring += "</td>";
            outstring += "<td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;\"";
            outstring += " onclick=\"jsOrderBy('XSFee')\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">舱位价</a>";
            outstring += "</td>";
            outstring += "<td style=\"width: 10%;\">";
            outstring += "票面价";
            outstring += "</td>";
            //                        if (0 == 0)
            //                          {
            //                        if (1 == 1)
            //                          { 
            outstring += "<td style=\"width: 10%;\">";
            outstring += "政策/佣金";
            outstring += "</td>";
            outstring += "<td style=\"width: 10%; cursor: pointer; text-decoration: underline; color: Blue;\"";
            outstring += " onclick=\"jsOrderBy('SJFee')\">";
            outstring += "<a href=\"#\" style=\"color: Blue; text-decoration: underline;\">实付金额</a>";
            outstring += "</td>";
            //      } 
            //                        }
            // else
            //  {
            // <td style="width: 10%; cursor: pointer; text-decoration: underline; color: Blue;"
            //     onclick="jsOrderBy('XSFee')">
            //     <a href="#" style="color: Blue; text-decoration: underline;">票面价格</a>
            // </td>
            // } 
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
        function getAirInfo(json) {
            var airInfoIsOk = false; //航班数据是否正确
            var chaceNameByGUID = ""; //基础数据缓存ID
            if (json.split('infoSplit')[2] != '') {
                jQueryOne("#showAirInfo").html(json.split('infoSplit')[2]); //输出错误信息

            } else {
                try {
                    chaceNameByGUID = json.split('infoSplit')[1];
                    var obj = eval('(' + json.split('infoSplit')[0] + ')')//转换航班JSON
                    var outstring = ""; //拼接的表格字符串
                    var strTitle = "";
                    var price = "0";
                    jQueryOne("#showAirInfo").html("");
                    //拼接列头
                    outstring += getColumnTitle();
                    var i = 0;
                    for (var tablename in obj) {//循环所有的表
                        i++;
                        var objtablenamelen = obj[tablename].length; //行的数量
                        if (objtablenamelen == 1 && obj[tablename][0]["TickNum"] == "--") {
                            outstring += "<table class='tab2'>";
                            outstring += "<tr onmouseover=\"this.className='leftliebiao_checi2'\" onmouseout=\"this.className='leftliebiao_checi'\"class='leftliebiao_checi'>";
                            outstring += "<td style='width: 1%;'></td>";
                            outstring += "<td style='width: 8%;'><img src=\"../img/air/" + obj[tablename][0]["CarrCode"].toString().ToUpper() + ".gif\" />" + obj[tablename][0]["Carrier"].toString() + "<br /><b>" + obj[tablename][0]["CarrCode"].toString() + obj[tablename][0]["FlightNo"].toString() + "</b></td>"; //航班信息
                            outstring += "<td style='width: 10%;'><b>" + obj[tablename][0]["StartTime"].toString() + "</b> " + obj[tablename][0]["FromCity"].toString() + "<br /><b>" + obj[tablename][0]["EndTime"].toString() + "</b> " + obj[tablename][0]["ToCity"].toString() + "</td>"; //起抵时间/机场
                            outstring += "<td style='width: 5%;'>" + obj[tablename][0]["Model"].toString() + "<br/>" + obj[tablename][0]["IsStop"].toString() + "</td>"; //机型
                            outstring += "<td style='width: 8%;'>" + obj[tablename][0]["ABFee"].toString() + "<br />" + obj[tablename][0]["FuelAdultFee"].toString() + "</td>"; //机建/燃油
                            outstring += "<td style='width: 8%;'><strong style=\"color:#6666AD\">" + obj[tablename][0]["Space"].toString() + "</strong>(--折)</td>"; //舱位/折扣
                            outstring += "<td style='width: 5%;'>" + obj[tablename][0]["TickNum"].toString() + "</td>"; //座位数
                            outstring += "<td style='width: 10%;' class='FXCss'>--</td>"; //
                            outstring += "<td style='width: 10%;' class='FXCss'>--</td>"; //
                            //                            if (NowCompany.A3 == 0) {
                            //                                // 显示政策
                            //                                //当前用户为采购商时不显示佣金放点和实际价格
                            //                                if ((NowCompany.RoleType == 1 || NowCompany.IsPolicy == 1) && NowCompany.RoleType != 3) {
                            outstring += "<td style='width:10%;' class='FXCss'>--</td>"; //优惠
                            outstring += "<td style='width:10%;' class='FXCss'>--</td>"; //实际支付金额
                            //                                }
                            //                            }
                            //                            else {
                            //                                // 隐藏政策
                            //outstring += "<td style='width:10%;'>--</td>"; //票面价格 ：舱位价+基建+燃油
                            //                            }

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
                                //佣金
                                var yjfee = "";
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

                                var IsGaoFan = "";

                                //                            if (obj[tablename][j]["A21"] != null && obj[tablename][j]["A21"] != "" && obj[tablename][j]["A21"] == "1") {
                                //                                //IsGaoFan = "<span style='color:Red;font-weight:bolder;'>高返</span>";
                                //                                IsGaoFan = "高返";
                                //                            }

                                //动态特价的显示
                                if (obj[tablename][j]["SpecialType"].toString() == "1") {
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

                                    //xsfee = "<span id=\"span_" + i + "_" + j + "\" class=\"btnen btnen-ok-s\"><input id='btnSpecial' type='button' value='获取价格' onclick=\"GetSpecial(this,'" + Specialstr + "@@HidSpecial_" + i + "_" + j + "')\" /></span>";
                                    xsfee = "<span class=\"btnen btnen-ok-s\"><input id='btnSpecial' type='button' value='获取价格' onclick=\"GetSpecial(this,'" + Specialstr + "','" + obj[tablename][j]["GUID"].toString() + "')\" /></span>";
                                    yjfee = "特价";
                                    sjf = "特价";
                                }
                                //非特价折扣的显示
                                if (zk.indexOf("特价") == -1) {
                                    zk += "折";
                                }
                                //                            xf = (decimal.Parse(ds.Tables[i].Rows[j]["ReturnMoney"].ToString()) * 1).ToString();
                                //                            xf = xf.Contains("-") ? xf : "+" + xf;
                                //                            xf = xf == "0" || xf == "-0" || xf == "+0" ? "" : xf;
                                try {
                                    //price = (decimal.Parse(ds.Tables[i].Rows[j]["XSFee"].ToString()) + decimal.Parse(ds.Tables[i].Rows[j]["ABFee"].ToString()) + decimal.Parse(ds.Tables[i].Rows[j]["FuelAdultFee"].ToString())).ToString("f2");
                                }
                                catch (e) {
                                    //price = "";
                                }

                                if (yjfee != "特价") {
                                    //yjfee = (decimal.Parse(yjfee) + (decimal.Parse(xf == "" ? "0" : xf))).ToString();
                                }
                                else {
                                    price = "特价";
                                }

                                //扣点显示隐藏判断
                                //if (NowCompany.A3 == 0)
                                if (1 == 1) {
                                    strTitle = "";
                                }
                                else {
                                    if (yjfee == "特价") {
                                        //strTitle = "title='【舱位价】：特价 \n【政策/佣金】：" + plic + "%" + xf + IsGaoFan + "/特价\n【票面价格】：特价\n【实付金额】：特价' ";
                                    }
                                    else {
                                        //隐藏政策
                                        // strTitle = "title='【舱位价】：" + xsfee + "\n【政策/佣金】：" + plic + "%" + xf + "/" + yjfee + IsGaoFan + "\n【票面价格】：" + price + "\n【实付金额】：" + sjf + " ' ";
                                    }
                                }
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

                                // if (obj[tablename][j]["DiscountRate"].toString().indexOf("-") != -1) {
                                if (obj[tablename][j]["SpecialType"].toString() == "1") {//特价

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
                                    outstring += "<td style='width: 8%;'><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"] + "</strong>(" + zk + ")"; //舱位/折扣
                                    outstring += "<br/><a href='javascript:void(0)' " + titlestr + " >退改签</a>";
                                    outstring += "</td>";
                                    //座位数非>9时显示为红色
                                    if (SpaceNum != ">9") {
                                        outstring += "<td style='width:5%;color:red;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                    }
                                    else {
                                        outstring += "<td style='width:5%;'>" + obj[tablename][j]["TickNum"] + "</td>"; //座位数
                                    }
                                    //outstring += "<td id=\"tdTitleTj_" + i + "_" + j + "\" " + strTitle + " style='width: 10%;' class='FXCss'>" + xsfee + "</td>"; //舱位价
                                    //outstring += "<td style='width:10%;' id=\"pmtd_" + i + "_" + j + "\" class='FXCss'>￥特价</td>";
                                    outstring += "<td id = '" + obj[tablename][j]["GUID"] + "cw'  style='width: 10%;' class='FXCss'>" + xsfee + "</td>"; //舱位价
                                    outstring += "<td id = '" + obj[tablename][j]["GUID"] + "pm'  style='width:10%;'class='FXCss'>￥特价</td>";
                                    //                                    if (NowCompany.A3 == 0) {
                                    //                                        //显示政策
                                    //                                        //当前用户为采购商时不显示佣金放点和实际价格
                                    //                                        if ((NowCompany.RoleType == 1 || NowCompany.IsPolicy == 1) && NowCompany.RoleType != 3) {
                                    //                                            if (NowCompany.A20.Contains("|37|")) {
                                    outstring += "<td id='" + obj[tablename][j]["GUID"] + "sppol' style='width:10%;' class='FXCss' sppol='null' spaceprice='null'>政策获取中<span style='color:Red;font-weight:bolder;'>" + IsGaoFan + "</span></BR>" + GetString("") + "</td>"; //优惠
                                    outstring += "<td id='" + obj[tablename][j]["GUID"] + "sjtj' style='width:10%;' class='FXCss'>￥" + sjf + "</td>"; //实付价格

                                    //outstring += "<td id=\"yjtd_" + i + "_" + j + "\" style='width:10%;' class='FXCss'>特价<span style='color:Red;font-weight:bolder;'>" + IsGaoFan + "</span></BR>" + GetString("") + "</td>"; //优惠
                                    //outstring += "<td id=\"sjtd_" + i + "_" + j + "\" style='width:10%;' class='FXCss'>￥" + sjf + "</td>"; //实付价格

                                    //                                            }
                                    //                                            else {
                                    //                                                outstring += "<td id=\"yjtd_" + i + "_" + j + "\" style='width:10%;' class='FXCss'>" + plic + "%" + xf + "/" + yjfee + "<span style='color:Red;font-weight:bolder;'>" + IsGaoFan + "</span></td>"; //优惠
                                    //                                                outstring += "<td id=\"sjtd_" + i + "_" + j + "\" style='width:10%;' class='FXCss'>￥" + sjf + "</td>"; //实付价格
                                    //                                            }
                                    //                                        }
                                    //                                    }
                                    //                                    else {
                                    //                                        //隐藏政策
                                    //outstring += "<td id=\"sjtd_" + i + "_" + j + "_2\" style='width:10%;' class='FXCss'>￥" + price + "</td>"; //票面价格 ：舱位价+基建+燃油
                                    //                                    }

                                    if (bz == "经停") {
                                        var flryNo = obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"];
                                        outstring += "<td style='width: 7%;' onmouseover=\"tdonmouseover(this,'" + obj[tablename][j]["StartDate"] + "','" + flryNo + "')\">" + bz + "</td>"; //经停
                                    }
                                    else {
                                        outstring += "<td style='width: 7%;'>" + bz + "</td>";  //经停
                                    }
                                    //if (NowCompany.RoleType != 1) {
                                    if (2 != 1) {
                                        var IsShowBtn = "display:none;";
                                        if (obj[tablename][j]["SpecialType"].toString() == "2") {
                                            IsShowBtn = "display:block;";
                                        }

                                        //outstring += "<td style='width: 8%;'><span id='" + obj[tablename][j]["GUID"] + "yd' style='" + IsShowBtn + "' ><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='预订' onclick='setHidId(\"HidSpecial_" + i + "_" + j + "\");" + msg + "' /></span></span><input type=\"hidden\" id=\"HidSpecial_" + i + "_" + j + "\"  /></td>";
                                        outstring += "<td style='width: 8%;'><span id='" + obj[tablename][j]["GUID"] + "yd' style='" + IsShowBtn + "' ><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='预订' onclick='setHidId(\"" + obj[tablename][j]["GUID"] + "HidSpecial\");" + msg + "' /></span></span><input type=\"hidden\" id=\"" + obj[tablename][j]["GUID"] + "HidSpecial\"  /></td>";
                                    }
                                    else {

                                        var IsShowBtn = "display:none;";
                                        if (obj[tablename][j]["SpecialType"].toString() == "2") {
                                            IsShowBtn = "display:block;";
                                        }
                                        outstring += "<td style='width: 8%;' id='R1Btn'><span id='" + obj[tablename][j]["GUID"] + "yd' style='" + IsShowBtn + "' ><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='第二程' onclick='setHidId(\"" + obj[tablename][j]["GUID"] + "HidSpecial\");" + msg + "' /></span></span><input type=\"hidden\" id=\"" + obj[tablename][j]["GUID"] + "HidSpecial\"  /></td>";
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
                                        outstring += "<td style='width: 8%;'><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"] + "</strong>(" + zk + ")"; //舱位/折扣
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
                                        //                                        if (NowCompany.A3 == 0) {
                                        //                                            //显示政策
                                        //                                            //当前用户为采购商时不显示佣金放点和实际价格
                                        //                                            if ((NowCompany.RoleType == 1 || NowCompany.IsPolicy == 1) && NowCompany.RoleType != 3) {
                                        //                                                if (NowCompany.A20.Contains("|37|")) {
                                        outstring += "<td id='" + obj[tablename][j]["GUID"] + "zc' style='width:10%;' class='FXCss'>政策获取中<span style='color:Red;font-weight:bolder;'>" + IsGaoFan + "</span></BR>" + GetString("") + "</td>"; //优惠
                                        outstring += "<td id='" + obj[tablename][j]["GUID"] + "sj' style='width:10%;' class='FXCss'>￥" + sjf + "</td>"; //实付价格
                                        //                                                }
                                        //                                                else {
                                        //                                                    outstring += "<td style='width:10%;' class='FXCss'>" + plic + "%" + xf + "/" + yjfee + "<span style='color:Red;font-weight:bolder;'>" + IsGaoFan + "</span></td>"; //优惠
                                        //                                                    outstring += "<td style='width:10%;' class='FXCss'>￥" + sjf + "</td>"; //实付价格
                                        //                                                }
                                        //                                            }
                                        //                                        }
                                        //                                        else {
                                        //                                            //隐藏政策
                                        //outstring += "<td style='width:10%;' class='FXCss'>￥" + price + "</td>"; //票面价格 ：舱位价+基建+燃油
                                        //                                        }
                                        if (bz == "经停") {
                                            var flryNo = obj[tablename][j]["CarrCode"] + obj[tablename][j]["FlightNo"];
                                            outstring += "<td style='width: 7%;' onmouseover=\"tdonmouseover(this,'" + obj[tablename][j]["StartDate"] + "','" + flryNo + "')\">" + bz + "</td>"; //经停
                                        }
                                        else {
                                            outstring += "<td style='width: 7%;'>" + bz + "</td>";  //经停
                                        }



                                        //                                        if (NowCompany.RoleType != 1) {
                                        outstring += "<td style='width: 8%;'><span class=\"btnen btnen-ok-s\" id='spCZ' ><input id='btnCreate' type='button' value='预订' onclick='" + msg + "' /></span></td>";
                                        //                                        }
                                        //                                        else {
                                        //outstring += "<td style='width: 8%;' id='R1Btn'><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='第二程' onclick='" + msg + "' /></span></td>";
                                        //                                        }
                                        outstring += "</tr></table>";
                                        NoTJIndex++;

                                        outstring += " <div id='div_" + i + "' class='TempDiv'>";
                                        outstring += " <table>";



                                    }

                                    else {
                                        outstring += "<tr><td style='width: 1%;'></td><td style='width: 8%;'></td><td style='width: 10%;'></td><td style='width: 5%;'></td><td style='width: 8%;'></td>";
                                        outstring += "<td style='width: 8%;'><strong style=\"color:#6666AD\">" + obj[tablename][j]["Space"] + "</strong>（" + zk + "）";

                                        outstring += "<br/><a href='javascript:void(0)' " + titlestr + " >退改签</a></td>";

                                        if (SpaceNum != ">9") {
                                            outstring += "<td style='width: 5%;color:red;'>" + obj[tablename][j]["TickNum"] + "</td>";
                                        }
                                        else {
                                            outstring += "<td style='width: 5%;'>" + obj[tablename][j]["TickNum"] + "</td>";
                                        }
                                        outstring += "<td " + strTitle + " style='width: 10%;' class='FXCss'>￥" + obj[tablename][j]["XSFee"].split('.')[0] + "</td>";
                                        outstring += "<td style='width:10%;' class='FXCss'>" + PMFee + "</td>";
                                        //                                        if (NowCompany.A3 == 0) {
                                        //                                            //当前用户为采购商时不显示佣金放点和实际价格
                                        //                                            if ((NowCompany.RoleType == 1 || NowCompany.IsPolicy == 1) && NowCompany.RoleType != 3) {
                                        //                                                if (NowCompany.A20.Contains("|37|")) {
                                        outstring += "<td id='" + obj[tablename][j]["GUID"] + "zc' style='width:10%;' class='FXCss'>政策获取中<span style='color:Red;font-weight:bolder;'>" + IsGaoFan + "</span></BR>" + GetString("") + "</td>";
                                        outstring += "<td id='" + obj[tablename][j]["GUID"] + "sj' style='width:10%;' class='FXCss'>￥" + sjf + "</td>";
                                        //                                                }
                                        //                                                else {

                                        //                                                    outstring += "<td style='width:10%;' class='FXCss'> " + plic + "%" + xf + "/" + yjfee + "<span style='color:Red;font-weight:bolder;'>" + IsGaoFan + "</span></td>";
                                        //                                                    outstring += "<td style='width:10%;' class='FXCss'>￥" + sjf + "</td>";
                                        //                                                }
                                        //                                            }
                                        //                                        }
                                        //                                        else {
                                        //                                            //隐藏政策
                                        //outstring += "<td style='width:10%;' class='FXCss'>￥" + price + "</td>"; //票面价格 ：舱位价+基建+燃油
                                        //                                        }

                                        //                                        if (NowCompany.RoleType != 1) {
                                        outstring += "<td style='width: 7%;'></td><td style='width: 8%;'><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='预订' onclick='InValue(\"" + sItemlist + "\")' /></span></td></tr>";
                                        //                                        }
                                        //                                        else {
                                        //outstring += "<td style='width: 7%;'></td><td style='width: 8%;' id='R1Btn'><span class=\"btnen btnen-ok-s\" id='spCZ'><input id='btnCreate' type='button' value='第二程' onclick='InValue(\"" + sItemlist + "\")' /></span></td></tr>";

                                        //                                        }
                                    }
                                }
                            }
                            outstring += "</table></div>";
                            jQueryOne("#showAirInfo").append(outstring);
                            outstring = "";
                        }
                    }

                    airInfoIsOk = true;
                }
                catch (e) {
                    airInfoIsOk = false;
                    jQueryOne("#showAirInfo").html("<font style=\"font-size:18px;font-weight:bold;color:red;\">航班数据解析失败,请重试或联系管理员</font>");
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
                    FromDate: jQueryOne("#txtBeginTime").val(),
                    ReturnDate: jQueryOne("#txtReturnTime").val(),
                    TravelType: jQueryOne("#Hid_travel").val(),
                    chaceNameByGUID: chaceNameByGUID,
                    IsOrGetPolicy: "true",
                    GroupId: jQueryOne("#Hid_GroupId").val(),
                    OrderID: "",
                    HavaChild: "false",
                    random: Math.random(),
                    currentuserid:jQueryOne("#currentuserid").val()
                },
               function (json) { getPolicyInfo(json) });
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
                            //政策佣金
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "zc").html(obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"] + "/" + obj[tablename][j]["Commission"]);
                            //实际支付金额
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "sj").html("￥" + obj[tablename][j]["SJFee"]);
                            //特价舱位政策现返隐藏在属性中,待获取价格后计算
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "sppol").attr("sppol", obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"]);
                            //特价舱位政策显示为(政策%+现返/待计算)
                            jQueryOne("#" + obj[tablename][j]["GUID"] + "sppol").html(obj[tablename][j]["Policy"] + "%" + obj[tablename][j]["ReturnMoney"] + "/待计算");

                            //如果是固定特价类型,则赋值属性固定特价价格,基建,燃油,用于发送到后台计算
                            if (obj[tablename][j]["GenerationType"] == "3") {
                                var jjty = jQueryOne("#" + obj[tablename][j]["GUID"] + "jjry").attr("jjry");
                                jQueryOne("#" + obj[tablename][j]["GUID"] + "sppol").attr("spaceprice", obj[tablename][j]["SpacePrice"] + "|" + jjty);
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

        function GetString(s) {
            return "";
        }
    </script>
</head>
<body>
    <%--城市控件使用容器开始--%>
    <div id="jsContainer">
        <div id="jsHistoryDiv" class="hide">
            <iframe id="jsHistoryFrame" name="jsHistoryFrame" src="about:blank"></iframe>
        </div>
        <textarea  id="jsSaveStatus" class="hide"></textarea>
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
    <div id="loading" style="display: none;">
        请稍等，<br />
        您查询的航班正在搜索中……<br />
        <img src="../img/loading.gif"></div>
    <form id="form1" name="form1" runat="server">
    <div class="listing">
        <div class="normal-content" style="padding: 10px;">
 
               <h2 class="normal-titile">
            <strong>返程机票预订</strong></h2>
           
            <div id=".tabs">
            <div id="ReturnDiv">
            </div>
              <div id="Div1" style="background: url(../img/bg_search.gif) repeat-x; border: 1px solid #D7D7D7;
                padding: 5px; border-radius: 5px; padding: 6px;">
                <table height="60px;" width="100%" id="gnjd">
                   <tr>
                        <td style="width: 80px; font-size: 14px; font-weight: bold;">
                            出发城市：
                        </td>
                        <td>
                            <%--出发城市开始--%>
                            <input name="txtStart" class="inputtxtdat" runat="server" type="text" id="txtStart"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 150px;" mod_address_reference="hiStart" disabled="disabled" />
                            <input type="hidden" name="hiStart" id="hiStart" runat="server" />
                            <%--出发城市结束--%>
                        </td>
                        <td id="Conntitle" style="width: 80px; font-size: 14px; font-weight: bold; display: none;"  runat="server">
                            中转城市：
                        </td>
                        <td id="ConnTxt" style="width: 170px;display:none" runat="server">
                            <%--中转城市开始--%>
                            <input name="txtConn" class="inputtxtdat" runat="server" type="text" id="txtConn"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 150px;" mod_address_reference="hiConn" disabled="disabled" />
                            <input type="hidden" name="hiConn" id="hiConn" runat="server" />
                            <%--中转城市结束--%>
                        </td>
                        <td style="width: 80px; font-size: 14px; font-weight: bold;">
                            到达城市：
                        </td>
                        <td>
                            <%--到达城市开始--%>
                            <input name="txtTarget" class="inputtxtdat" runat="server" type="text" id="txtTarget"
                                mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                                mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                                style="width: 150px;" mod_address_reference="hiTarget" disabled="disabled" />
                            <input type="hidden" name="hiTarget" id="hiTarget" runat="server" />
                            <%--到达城市结束--%>
                        </td>
                        <td rowspan="2">
                            <asp:Button runat="server" ID="btnQuery" CssClass="btn big1 cp" Text="查询" 
                                OnClientClick="showts();" onclick="btnQuery_Click"
                               ></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 80px; font-size: 14px; font-weight: bold;">
                            出发时间：
                        </td>
                        <td>
                            <input type="text" readonly="readonly" id="txtBeginTime" runat="server" class="Wdate inputtxtdat"
                                style="width: 150px" onfocus="WdatePicker({isShowClear:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',doubleCalendar:true})" disabled="disabled" />
                        </td>
                        <td style="width: 80px; font-size: 14px; font-weight: bold;" >
                        <span id="spBack" runat="server">返回时间：</span>
                           
                        </td>
                        <td>
                            <input type="text" readonly="readonly" id="txtReturnTime" runat="server" class="Wdate inputtxtdat"
                                style="width: 150px" onfocus="WdatePicker({isShowClear:false,minDate:'%y-%M-{%d+1}',dateFmt:'yyyy-MM-dd',doubleCalendar:true})" />
                        </td>
                    </tr>
                </table>
            </div>
            </div>
          
        
            <%-- 显示航班数据 开始 --%>
            <div id="AirListDiv" class="result" style="margin: 0px;">
               
                    <%--存储隐藏域ID--%>
        <input type="hidden" id="Hid_HidId" />
        <input type="hidden" id="hidTjValue" value="" runat="server" />
        <%--传过来的数据--%>
        <%--行程类型--%>
        <input type="hidden" id="Hid_travel" value="1" runat="server" />
        <%--分组ID--%>
        <input type="hidden" id="Hid_GroupId" value="" runat="server" />
        <%--共享航班标示--%>
        <input id="hidIsShowShare2" type="hidden" runat="server" />
        <%--航班信息--%>
        <input id="FlyInfo" type="hidden" runat="server" name="FlyInfo" />
        <%--匹配政策用的出发城市--%>
        <input id="hidFCityCode" type="hidden" runat="server" name="hidFCityCode" />
         <%--匹配政策用的中转城市--%>
        <input id="hidMCityCode" type="hidden" runat="server" name="hidMCityCode" />
        <%--匹配政策用的到达城市--%>
        <input id="hidTCityCode" type="hidden" runat="server" name="hidTCityCode" />
                  <div id="showAirInfo">
                  </div>
                <div id="showDiv" runat="server">
                   
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
    <asp:HiddenField ClientIDMode="Static" ID="currentuserid" runat="server" />
    </form>
    <%--排序查询、经停查询、特价查询--%>
    <script type="text/javascript" language="javascript">

        var req; //定义变量，用来创建xmlhttprequest对象
        var type = "1";
        // 创建xmlhttprequest,ajax开始
        function creatReq(url) {
            var valUrl = url + "&random=" + Math.ceil(Math.random() * 35);
            if (window.XMLHttpRequest) { //非IE浏览器，用xmlhttprequest对象创建
                req = new XMLHttpRequest();
            }
            else if (window.ActiveXObject) { //IE浏览器用activexobject对象创建
                req = new ActiveXObject("Microsoft.XMLHttp");
            }
            if (req) {//成功创建xmlhttprequest
                req.open("GET", valUrl, true); //与服务端建立连接(请求方式post或get，地址,true表示异步)
                req.onreadystatechange = callback; //指定回调函数
                req.send(null); //发送请求   
            }
        }
        //回调函数，对服务端的响应处理，监视response状态
        function callback() {
            if (req.readyState == 4) {//请求状态为4表示成功
                if (req.status == 200) {//http状态200表示OK
                    if (type == "1") {
                        printList(); //显示排序
                    } else if (type == "2") {
                        ReturnTdOnMouseOver(); //显示经停
                    } else if (type == "3") {
                        ReturnSpecial(); //显示特价
                    }
                }
                else {
                    if (type == "2") {
                        tdobj.innerHTML = "经停<br/><font style=\"font-size:12px;\">获取中...</font)";
                    }
                } //http返回状态失败
            } else {
                if (type == "2") {
                    tdobj.innerHTML = "经停<br/><font style=\"font-size:12px;\">获取中...</font)";
                }
            }
        }
        //查询排序
        function jsOrderBy(fname) {
            if (document.getElementById("hidOrderByStr").value != "") {
                var jurl = "../../Ajax/FlightAjax.aspx?&sdate=" + Date().toString() + "&type=" + document.getElementById("hidOrderByStr").value + "&filename=" + fname+"&currentuserid=<%=this.mUser.id.ToString() %>";
                if (document.getElementById("hidOrderByStr").value == "desc") {
                    document.getElementById("hidOrderByStr").value = "asc";
                }
                else {
                    document.getElementById("hidOrderByStr").value = "desc";
                }
                type = "1";
                creatReq(jurl);
            }
        }
        //返回排序数据
        function printList() {
            var temptext = req.responseText; //获取数据
            if (temptext != "") { //查询显示数据
                document.getElementById("showDiv").innerHTML = temptext;
            }
            else { }
        }
        var tdobj = null;
        var tdTjObj = null;
        //查询经停
        function tdonmouseover(obj, sTime, flyNo) {
            var turl = "../../Ajax/SelStopsAjax.aspx?&stime=" + sTime + "&flyno=" + flyNo + "&currentuserid=" + jQueryOne("#currentuserid").val();  ;
            if (obj["issend"] != undefined) {
                //已有数据
                //alert(tdobj.title);
                //obj.title = obj["issend"].toString();

            } else {
                type = "2";
                tdobj = obj;
                obj["issend"] = "1";
                creatReq(turl + "&sdate=" + Date().toString());
            }
        }
        //返回经停数据
        function ReturnTdOnMouseOver() {
            var temp = req.responseText; //获取数据
            if (temp != "") {
                tdobj.innerHTML = "经停:<font style=\"font-size:12px;\">" + temp + "</font)";
                //tdobj.title = temp;
                //tdobj["issend"] = temp;
            }
        }
        var getSpecialUrl = "";
        var getSpecialVal = "";
        //查询特价
        function GetSpecial(obj, val, guid) {
            if (jQueryOne("#" + guid + "sppol").attr("sppol") != "null") {
                tdTjObj = obj;
                var gurl = "../Ajax/SpPatAjax.aspx?currentuserid="+jQueryOne("#currentuserid").val()+"&val=" + val + "|" + jQueryOne("#" + guid + "sppol").attr("sppol") + "|" + jQueryOne("#" + guid + "sppol").attr("spaceprice");
                if ((getSpecialUrl != "" && getSpecialUrl == gurl && getSpecialVal == "") || obj.value == "无运价") {
                    obj.value = "无运价";
                    showdialogmsg("无运价！");
                } else {
                    type = "3";
                    getSpecialUrl = gurl;
                    //显示查询
                    document.getElementById("loading").innerHTML = "请稍等，<br />特价价格搜索中……<br /><img src='../img/loading.gif'>";
                    document.getElementById("overlay").style.display = "block";
                    document.getElementById("loading").style.display = "block";
                    creatReq(gurl + "&sdate=" + Date().toString());
                }
            }
            else {
                showdialogmsg("政策未获取成功,请等待政策获取成功再获取价格！");
            }


        }
        //返回特价数据
        function ReturnSpecial() {
            document.getElementById("overlay").style.display = "none";
            document.getElementById("loading").style.display = "none";
            document.getElementById("loading").innerHTML = "请稍等，<br />您查询的航班正在搜索中……<br /><img src='../img/loading.gif'>";
            getSpecialVal = req.responseText;
            if (getSpecialVal != null && getSpecialVal != "") {
                var arr = getSpecialVal.split('@');
                try {
                    jQueryOne("#" + arr[6] + "cw").html(arr[0]); //舱位
                    jQueryOne("#" + arr[6] + "jjry").html(arr[1] + "<br />" + arr[2]); //基建燃油
                    jQueryOne("#" + arr[6] + "pm").html(arr[3]); //票面价
                    jQueryOne("#" + arr[6] + "sppol").html(jQueryOne("#" + arr[6] + "sppol").attr("sppol") + "/" + arr[5]); //政策
                    jQueryOne("#" + arr[6] + "sjtj").html(arr[4]); //实付金额
                    jQueryOne("#" + arr[6] + "yd").show(); //预订显示
                    jQueryOne("#" + arr[6] + "HidSpecial").val(arr[0] + "|" + arr[1] + "|" + arr[2]); //特价的基建,燃油,舱位,票面价,实付金额
                } catch (e) {

                }
            } else {
                tdTjObj.value = "无运价";
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
    <script type="text/javascript" language="javascript">
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
                LevelArr.push('<img src=\"../img/sun.jpg\" />');
            }
            for (var i = 0; i < parseInt(month, 10); i++) {
                LevelArr.push('<img src=\"../img/month.jpg\" />');
            }
            for (var i = 0; i < parseInt(star, 10); i++) {
                LevelArr.push('<img src=\"../img/star.jpg\" />');
            }
            return LevelArr.join("");
        }
    </script>
</body>
</html>

