<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DiscountSet.aspx.cs" Inherits="DiscountSet_DiscountSet" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>扣点组设置</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script id="script_k0" src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
        body
        {
            font-size: 12px;
        }
        a:link, a:active
        {
            color: #6D69EE;
        }
        a:visited
        {
            color: blue;
        }
        a:hover
        {
            color: Red;
        }
        
        .input
        {
            width: 100px;
            color: #14B4E3;
            font-size: 12px;
            font-weight: bold;
        }
        .Content
        {
            margin-left: 5%;
            margin-right: 5%;
        }
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
        }
        .TextBorder
        {
            border: 1px solid #999;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .textLeft
        {
            text-align: left;
        }
        .textCenter
        {
            text-align: center;
        }
        .textRight
        {
            text-align: right;
        }
        .inputBorder
        {
            border: 1px solid #999;
        }
        .FontStyle
        {
            color: Green;
        }
        .tdWidth200
        {
            width: 200px;
        }
        .tdWidth100
        {
            width: 100px;
        }
        .tdWidth350
        {
            width: 350px;
        }
        .w_td
        {
            width: 12%;
            text-align: right;
            border: 1px solid #EEEEEE;
            font-weight: normal;
        }
        .r-td
        {
            width: 86%;
            border: 1px solid #EEEEEE;
        }
        #spanSun
        {
            color: Red;
        }
        #spanMonth
        {
            color: Red;
        }
        #spanStar
        {
            color: Red;
        }
        #LevelMsg
        {
            color: Red;
        }
        .tdw120
        {
            width: 120px;
        }
        .textAreaFlight
        {
            width: 549px;
            border: 1px solid #999;
        }
        .tdBorder
        {
            border: 1px solid #C5DBF2;
        }
    </style>
    <script type="text/javascript">
        //------------------------控件部分js----------------------------------------------
        var jQueryOne = jQuery.noConflict(false);
        //重新加载城市控件
        window.onload = function ReLoad() {
            try {
                //                  var rd = "?r=" + Math.random();
                //                  var SE = new CtripJsLoader();
                //                  var dataUrl = "../JS/CitySelect/fltdomestic_gb2312.js" + rd;

                //                  var files = [["../JS/CitySelect/tuna_100324.js" + rd, "GB2312", true, null], [dataUrl, "GB2312", true, null]];
                //                  SE.scriptAll(files);

                //注册添加和排除按钮回车事件      
                jQueryOne("#txtFromCode").keydown(function (event) {
                    if (event.keyCode == 13) {
                        KeyBoardInput('txtFromCode', 'From_RightBox', 'From_LeftBox');
                    }
                });
                jQueryOne("#txtFromCode_01").keydown(function (event) {
                    if (event.keyCode == 13) {
                        KeyBoardInput('txtFromCode_01', 'From_LeftBox', 'From_RightBox');
                    }
                });

                jQueryOne("#txtToCode").keydown(function (event) {
                    if (event.keyCode == 13) {
                        KeyBoardInput('txtToCode', 'To_RightBox', 'To_LeftBox');
                    }
                });
                jQueryOne("#txtToCode_02").keydown(function (event) {
                    if (event.keyCode == 13) {
                        KeyBoardInput('txtToCode_02', 'To_LeftBox', 'To_RightBox');
                    }
                });
                //列表框键盘事件 4个
                jQueryOne("#From_LeftBox").keydown(function (event) {
                    if (event.keyCode == 39) {//右
                        dblclick('From_RightBox', 'From_LeftBox', '1', 'rl');
                    }
                    else if (event.keyCode == 40) {//下
                        dblclick('From_RightBox', 'From_LeftBox', '2', 'rl')
                    }
                });
                jQueryOne("#From_RightBox").keydown(function (event) {
                    if (event.keyCode == 37) {//左
                        dblclick('From_RightBox', 'From_LeftBox', '1', 'lr');
                    }
                    else if (event.keyCode == 38) {//上
                        dblclick('From_RightBox', 'From_LeftBox', '2', 'lr');
                    }
                });

                jQueryOne("#To_LeftBox").keydown(function (event) {
                    if (event.keyCode == 39) {//右
                        dblclick('To_RightBox', 'To_LeftBox', '1', 'rl');
                    }
                    else if (event.keyCode == 40) {//下
                        dblclick('To_RightBox', 'To_LeftBox', '2', 'rl');
                    }
                });
                jQueryOne("#To_RightBox").keydown(function (event) {
                    if (event.keyCode == 37) {//左
                        dblclick('To_RightBox', 'To_LeftBox', '1', 'lr');
                    }
                    else if (event.keyCode == 38) {//上
                        dblclick('To_RightBox', 'To_LeftBox', '2', 'lr');
                    }
                });

                var IsEdit = jQueryOne("#Hid_IsEdit").val();
                if (IsEdit == "1") {
                    KeyBoardInput('txtFromCode', 'From_RightBox', 'From_LeftBox')
                    KeyBoardInput('txtToCode', 'To_RightBox', 'To_LeftBox')
                }
            }
            catch (e) {
                alert(e.message);
            }
        }
        //双击或者添加listBox 
        function dblclick(sourceId, targetId, Only_Mul, Direct) {
            var SourceBox = document.getElementById(sourceId); // jQueryOne("#" + sourceId); 
            var TargetBox = document.getElementById(targetId); //jQueryOne("#" + targetId);
            if (Direct == "lr") {
                if (Only_Mul == "0") {
                    //单个
                    if (SourceBox.selectedIndex < 0) {
                        return;
                    }
                    var val = SourceBox.options[SourceBox.selectedIndex].value;
                    var text = SourceBox.options[SourceBox.selectedIndex].text;
                    //移除源
                    SourceBox.removeChild(SourceBox.options[SourceBox.selectedIndex]);
                    //添加目的
                    TargetBox.options.add(new Option(text, val));
                } else if (Only_Mul == "1") {
                    //多个
                    var len = SourceBox.options.length;
                    var selArr = [];
                    for (var i = 0; i < len; i++) {
                        if (SourceBox.options[i].selected) {
                            selArr.push(SourceBox.options[i]);
                        }
                    }
                    for (var j = 0; j < selArr.length; j++) {
                        TargetBox.options.add(new Option(selArr[j].text, selArr[j].value));
                        SourceBox.removeChild(selArr[j]);
                    }
                }
                else if (Only_Mul == "2") {
                    //所有
                    for (var i = 0; i < SourceBox.options.length; i++) {
                        TargetBox.options.add(new Option(SourceBox.options[i].text, SourceBox.options[i].value));
                    }
                    SourceBox.options.length = 0;
                }
            } else if (Direct == "rl") {
                if (Only_Mul == "0") {
                    //单个
                    if (TargetBox.selectedIndex < 0) {
                        return;
                    }
                    var val = TargetBox.options[TargetBox.selectedIndex].value;
                    var text = TargetBox.options[TargetBox.selectedIndex].text;
                    //移除源
                    TargetBox.removeChild(TargetBox.options[TargetBox.selectedIndex]);
                    //添加目的
                    SourceBox.options.add(new Option(text, val));
                } else if (Only_Mul == "1") {
                    //多个
                    var len = TargetBox.options.length;
                    var selArr = [];
                    for (var i = 0; i < len; i++) {
                        if (TargetBox.options[i].selected) {
                            selArr.push(TargetBox.options[i]);
                        }
                    }
                    for (var j = 0; j < selArr.length; j++) {
                        SourceBox.options.add(new Option(selArr[j].text, selArr[j].value));
                        TargetBox.removeChild(selArr[j]);
                    }
                }
                else if (Only_Mul == "2") {
                    //所有                   
                    for (var i = 0; i < TargetBox.options.length; i++) {
                        SourceBox.options.add(new Option(TargetBox.options[i].text, TargetBox.options[i].value));
                    }
                    TargetBox.options.length = 0;
                }
            }
            GetString(sourceId);
        }
        //键盘输入文本框
        function KeyBoardInput(SourceId, sBox, tBox, Content) {
            var sVal = '';
            if (Content != undefined && Content != null) {
                sVal = Content;
            } else {
                sVal = jQueryOne("#" + SourceId).val().toUpperCase();
            }
            if (sVal.length < 2) {
                return;
            }
            var LBox = document.getElementById(sBox);
            var RBox = document.getElementById(tBox);
            if (LBox.options.length > 0) {
                var IsValite = false;
                var len = LBox.options.length;
                var val = "";
                var cityName = '';
                if (sVal.indexOf("/") != -1) {
                    var vArr = sVal.split("/");
                    for (var c = 0; c < vArr.length; c++) {
                        for (var i = 0; i < LBox.options.length; i++) {
                            val = LBox.options[i].text.split('_')[0].toUpperCase();
                            cityName = LBox.options[i].text.split('_')[1].toUpperCase(); //城市名
                            if (vArr[c] == val || vArr[c] == cityName) {
                                //找到                                        
                                RBox.options.add(new Option(LBox.options[i].text, val));
                                LBox.removeChild(LBox.options[i]);
                                IsValite = true;
                                break;
                            }
                        }
                    }
                    //jQueryOne("#" + SourceId).val(""); //不清空
                } else {
                    var cityName = '';
                    for (var i = 0; i < len; i++) {
                        val = LBox.options[i].text.split('_')[0].toUpperCase();
                        cityName = LBox.options[i].text.split('_')[1].toUpperCase(); //城市名
                        if (sVal == val || sVal == cityName) {
                            //找到
                            RBox.options.add(new Option(LBox.options[i].text, val));
                            LBox.removeChild(LBox.options[i]);
                            IsValite = true;
                            break;
                        }
                    }
                }
            }
            GetString(SourceId);
        }
        //用于排序
        function sortBy(arr, prop, desc) {
            var props = [], ret = [], i = 0, len = arr.length;
            if (typeof prop == 'string') {
                for (; i < len; i++) {
                    var oI = arr[i];
                    (props[i] = new String(oI && oI[prop] || ''))._obj = oI;
                }
            } else if (typeof prop == 'function') {
                for (; i < len; i++) {
                    var oI = arr[i];
                    (props[i] = new String(oI && prop(oI) || ''))._obj = oI;
                }
            } else if (typeof prop == 'number') {
                for (; i < len; i++) {
                    var oI = arr[i];
                    (props[i] = new String(oI && oI[prop] || ''))._obj = oI;
                }
            }
            else {
                throw '参数类型错误';
            }
            props.sort();
            for (i = 0; i < len; i++) {
                ret[i] = props[i]._obj;
            }
            if (desc) ret.reverse();
            return ret;
        };
        //对下拉列表排序
        function selOptionSort(selId) {
            var RData = [];
            var SelObjArr = jQueryOne("#" + selId)[0].options;
            //排序
            RData = sortBy(SelObjArr, function (op) {
                return op.value;
            });
            SelObjArr.length = 0;
            for (var i = 0; i < RData.length; i++) {
                SelObjArr.add(RData[i]);
            }
            return SelObjArr;
        }
        //赋值
        function GetString(SourceId) {
            var val = '';
            if (SourceId == "From_LeftBox" || SourceId == "From_RightBox" || SourceId == "txtFromCode" || SourceId == "txtFromCode_01") {
                //出发城市列表
                var FromSel = document.getElementById("From_RightBox");
                var LData = [], RData = [];
                //对左右两个列表排序
                LData = selOptionSort('From_LeftBox');
                RData = selOptionSort('From_RightBox');

                for (var i = 0; i < LData.length; i++) {
                    val += LData[i].value + "/";
                }

                jQueryOne("#FromCityCode").val(val);
                //设置文本框值txtFromCode
                jQueryOne("#txtFromCode").val(val);



            }
            else if (SourceId == "To_LeftBox" || SourceId == "To_RightBox" || SourceId == "txtToCode" || SourceId == "txtToCode_02") {
                //到达城市列表
                var FromSel = document.getElementById("To_LeftBox");
                var LData = [], RData = [];
                //对左右两个列表排序
                LData = selOptionSort('To_LeftBox');
                RData = selOptionSort('To_RightBox');

                for (var i = 0; i < LData.length; i++) {
                    val += LData[i].value + "/";
                }

                //设置隐藏域值
                jQueryOne("#ToCityCode").val(val);
                //设置文本框值
                jQueryOne("#txtToCode").val(val);


            }
        }
         
       
    </script>
    <script type="text/javascript">
        function showdialog(t) {
            jQueryOne("#showOne").html(t);
            jQueryOne("#showOne").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                width: 250,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        jQueryOne(this).dialog('close');

                    }
                }
            });
        }
        ////////////////////////////////扣点范围
        function addTR(i) {
            //debugger;
            document.getElementById("hidtxtCount").value = parseInt(document.getElementById("hidtxtCount").value) + 1;
            //隐藏当前tr中的添加，删除按钮
            if (i < 4) {
                document.getElementById("sAdd" + i).style.display = "none";
            }
            if (i > 0) {
                document.getElementById("sDelete" + i).style.display = "none";
            }
            //显示下一个tr
            //document.getElementById("tr" + (i + 1)).style.display = "block";
            jQueryOne("#tr" + (i + 1)).removeClass("hide");
        }

        //删除一个
        function delTR(i) {
            document.getElementById("hidtxtCount").value = parseInt(document.getElementById("hidtxtCount").value) - 1;
            //显示上一个tr中的添加，删除按钮
            document.getElementById("sAdd" + (i - 1)).style.display = "block";
            if (i > 1) {
                document.getElementById("sDelete" + (i - 1)).style.display = "block";
            }
            //隐藏当前tr
            //document.getElementById("tr" + i).style.display = "none";
            jQueryOne("#tr" + i).addClass("hide");
            
        }
        //接口隐藏显示
        function showjk(str) {
            if (str == 2) {
                document.getElementById("showjk").style.display = "block";
            } else {
                document.getElementById("showjk").style.display = "none";
            }
        }

        function disabledl(str) {
            var rblvlaue = "";
            var vRbtid = document.getElementById("rblSelectType");
            //得到所有radio
            var vRbtidList = vRbtid.getElementsByTagName("INPUT");
            for (var i = 0; i < vRbtidList.length; i++) {
                if (vRbtidList[i].checked) {
                    var text = vRbtid.cells[i].innerText;
                    rblvlaue = vRbtidList[i].value;
                }
            }
            if (parseInt(rblvlaue) == 3) {
                document.getElementById("ddlbasetype").disabled = true;
            } else {
                document.getElementById("ddlbasetype").disabled = false;
            }
            
        }
        function Pause(obj, iMinSecond) {
            if (window.eventList == null) window.eventList = new Array();
            var ind = -1;
            for (var i = 0; i < window.eventList.length; i++) {
                if (window.eventList[i] == null) {
                    window.eventList[i] = obj;
                    ind = i;
                    break;
                }
            }
            if (ind == -1) {
                ind = window.eventList.length;
                window.eventList[ind] = obj;
            }
            setTimeout("GoOn(" + ind + ")", iMinSecond);
        }

        //js继续函数    
        function GoOn(ind) {
            var obj = window.eventList[ind];
            window.eventList[ind] = null;
            if (obj.NextStep) obj.NextStep();
            else obj();
        }
        function showErr(c1, c2) {
            Pause(this, 500);
            this.NextStep = function () {
                if (jQueryOne("#" + c1).val() == "") {
                    jQueryOne("#" + c2).html("不能为空！");
                }
                else {
                    jQueryOne("#" + c2).html("<b>*</b>");
                }
                if (parseFloat(jQueryOne("#txtA0").val()) >= parseFloat(jQueryOne("#txtB0").val())) {
                    bools++;
                    jQueryOne("#spanB" + i).html("值必须大于上一个值!");
                }
            }
        }
        function showAllErr() {
            var bools = 0;
            if (jQueryOne("#txtGroupName").val() == "") {
                bools++;
                jQueryOne("#spanGroupName").html("不能为空!");
            }
            else {
                jQueryOne("#spanGroupName").html("<b>*</b>");
            }
            if (jQueryOne("#hid_showgroupdtl").val != "0") {
                if (jQueryOne("#txtA0").val() == "") {
                    bools++;
                    jQueryOne("#spanA0").html("不能为空!");
                }
                else {
                    jQueryOne("#spanA0").html("<b>*</b>");
                }
                var hidtxtCountTemp = document.getElementById("hidtxtCount").value;
                for (var i = 0; i < hidtxtCountTemp; i++) {
                    if (parseFloat(jQueryOne("#txtA" + i).val()) >= parseFloat(jQueryOne("#txtB" + i).val()) || jQueryOne("#txtB" + i).val()=="") {
                        bools++;
                        jQueryOne("#spanB" + i).html("值必须大于上一个值!");
                    } 
                    if (i > 0) {
                        if (parseFloat(jQueryOne("#txtA" + i).val()) <= parseFloat(jQueryOne("#txtB" + (i - 1)).val()) || jQueryOne("#txtA" + i).val()=="") {
                            bools++;
                            jQueryOne("#spanA" + i).html("值必须大于上一个值!");
                        }
                    }
                    if (jQueryOne("#txtpoint" + i).val() =="" || parseFloat(jQueryOne("#txtpoint" + i).val())<0.1) {
                        bools++;
                        jQueryOne("#spanpoint" + i).html("点数不能小于0.1");
                    }
                }
            }

            if (bools > 0) {
                return false;
            }
            else {
                return true;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="top: 0px;">
        <div class="title">
            <asp:Label ID="lblShow" Text="扣点组设置" runat="server" />
        </div>
        <div class="c-list-filter">
            <div class="container">
                <table class="Search" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td colspan="2">
                            <table class="Search" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <th>
                                        扣点组名称：
                                    </th>
                                    <td>
                                        <asp:TextBox ID="txtGroupName" Width="115px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                                        <span id="spanGroupName" style="color: Red;"><b>*</b></span>
                                    </td>
                                    <td>
                                        扣点组标志
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblDefaultFlag" runat="server" RepeatColumns="2">
                                            <asp:ListItem Value="false" Text="非默认" Selected="True"></asp:ListItem>
                                            <asp:ListItem Value="true" Text="默认"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td>
                                        协调标志
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblUniteFlag" runat="server" RepeatColumns="2">
                                            <asp:ListItem Value="0" Text="不协调" Selected="True"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="协调"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td>
                                        协调返点值
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnitePoint" Width="115px" CssClass="inputtxtdat" runat="server"
                                            onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        (格式： 3 表示值： 3% 返点）
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table runat="server" id="showgroupdtl" class="Search" cellspacing="0" cellpadding="0"
                    border="0">
                    <tr>
                        <td>
                            <h4>
                                扣点明细设置</h4>
                        </td>
                    </tr>
                    <tr>
                        <th class="w_td">
                            
                        </th>
                        <td>
                            <uc1:SelectAirCode ID="SelectAirCode1" DefaultOptionValue="ALL" DefaultOptionText="ALL"
                                runat="server" />
                        </td>
                    </tr>
                    <tr id="tr_from2">
                        <th class="w_td">
                            <font style="color: red;">*</font>出发城市：
                        </th>
                        <td class="r-td" align="left">
                            <input type="text" id="txtFromCode" runat="server" style="width: 300px;" autocomplete="off"
                                class="inputBorder" />
                            <span class="btn btn-ok-s">
                                <input type="button" id="btnAdd1" runat="server" value="加入" autocomplete="off" onclick="KeyBoardInput('txtFromCode','From_RightBox','From_LeftBox')" /></span>
                            <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="msg0"></span></span>
                        </td>
                    </tr>
                    <tr id="tr_from2_01">
                        <th class="w_td">
                        </th>
                        <td class="r-td" align="left">
                            <input type="text" id="txtFromCode_01" runat="server" style="width: 300px;" autocomplete="off"
                                class="inputBorder" />
                            <span class="btn btn-ok-s">
                                <input type="button" id="btnfromPaiChu" runat="server" value="排除" autocomplete="off"
                                    onclick="KeyBoardInput('txtFromCode_01','From_LeftBox','From_RightBox')" /></span>
                            <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="Span1"></span></span>
                        </td>
                    </tr>
                    <tr id="tr_from3">
                        <th class="w_td">
                        </th>
                        <td class="r-td">
                            <table cellspacing="0" cellpadding="0" border="0" id="fromtab">
                                <tr>
                                    <td align="center">
                                        <font style="color: green; font-size: 15px;">已选城市</font>
                                    </td>
                                    <td>
                                    </td>
                                    <td align="center">
                                        <font style="color: green; font-size: 15px;">待选城市</font>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:ListBox ID="From_LeftBox" size="15" runat="server" Style="width: 150px; height: 210px;"
                                            ondblclick="dblclick('From_RightBox','From_LeftBox','0','rl')" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                    <td valign="middle" align="center" width="150px" style="padding-right: 5px; padding-left: 5px;
                                        text-align: center; line-height: 20px;">
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="from_btnAdd" runat="server" value="<<添加" class="input" onclick="dblclick('From_RightBox','From_LeftBox','1','lr')" /></span><br />
                                        <br />
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="from_btnAddAll" runat="server" value="<<全部添加" class="input"
                                                onclick="dblclick('From_RightBox','From_LeftBox','2','lr')" /></span><br />
                                        <br />
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="from_btnDelAll" runat="server" value="全部删除>>" class="input"
                                                onclick="dblclick('From_RightBox','From_LeftBox','2','rl')" /></span><br />
                                        <br />
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="from_btnDel" runat="server" value="删除>>" class="input" onclick="dblclick('From_RightBox','From_LeftBox','1','rl')" /></span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox ID="From_RightBox" size="15" runat="server" Style="width: 150px; height: 210px;"
                                            ondblclick="dblclick('From_RightBox','From_LeftBox','0','lr')" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="tr_to2">
                        <th class="w_td">
                            <font style="color: red;">*</font>到达城市：
                        </th>
                        <td class="r-td">
                            <input type="text" id="txtToCode" runat="server" style="width: 300px;" autocomplete="off"
                                class="inputBorder" />
                            <span class="btn btn-ok-s">
                                <input type="button" id="btnAdd3" runat="server" value="加入" autocomplete="off" onclick="KeyBoardInput('txtToCode','To_RightBox','To_LeftBox')" /></span>
                            <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="msg1"></span></span>
                        </td>
                    </tr>
                    <tr id="tr_to2_01">
                        <th class="w_td">
                        </th>
                        <td class="r-td">
                            <input type="text" id="txtToCode_02" runat="server" style="width: 300px;" autocomplete="off"
                                class="inputBorder" />
                            <span class="btn btn-ok-s">
                                <input type="button" id="btnToPaiChu" runat="server" value="排除" autocomplete="off"
                                    onclick="KeyBoardInput('txtToCode_02','To_LeftBox','To_RightBox')" /></span>
                            <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="Span2"></span></span>
                        </td>
                    </tr>
                    <tr id="tr_to3">
                        <th class="w_td">
                        </th>
                        <td class="r-td">
                            <table cellspacing="0" cellpadding="0" border="0" id="totab" style="display: block;">
                                <tr>
                                    <td align="center">
                                        <font style="color: green; font-size: 15px;">已选城市</font>
                                    </td>
                                    <td>
                                    </td>
                                    <td align="center">
                                        <font style="color: green; font-size: 15px;">待选城市</font>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:ListBox ID="To_LeftBox" size="15" runat="server" Style="width: 150px; height: 210px;"
                                            ondblclick="dblclick('To_RightBox','To_LeftBox','0','rl')" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                    <td valign="middle" align="center" width="150px" style="padding-right: 5px; padding-left: 5px;
                                        text-align: center; line-height: 20px;">
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="to_btnAdd" runat="server" value="<<添加" class="input" onclick="dblclick('To_RightBox','To_LeftBox','1','lr')" /></span><br />
                                        <br />
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="to_btnAddAll" runat="server" value="<<全部添加" class="input"
                                                onclick="dblclick('To_RightBox','To_LeftBox','2','lr')" /></span><br />
                                        <br />
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="to_btnDelAll" runat="server" value="全部删除>>" class="input"
                                                onclick="dblclick('To_RightBox','To_LeftBox','2','rl')" /></span><br />
                                        <br />
                                        <span class="btn btn-ok-s">
                                            <input type="button" id="to_btnDel" runat="server" value="删除>>" class="input" onclick="dblclick('To_RightBox','To_LeftBox','1','rl')" /></span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox ID="To_RightBox" size="15" runat="server" Style="width: 150px; height: 210px;"
                                            ondblclick="dblclick('To_RightBox','To_LeftBox','0','lr')" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <th class="w_td">
                            调整类型
                        </th>
                        <td>
                            <asp:RadioButtonList ID="rblSelectType" runat="server" RepeatColumns="3" onclick="disabledl()">
                                <%--<asp:ListItem Value="1" Text="扣点" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="2" Text="留点"></asp:ListItem>
                                <asp:ListItem Value="3" Text="补点"></asp:ListItem>--%>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr id="tr0" runat="server">
                        <th class="w_td">
                            点数范围
                        </th>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA0" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanA0" style="color: Red;"><b>*</b></span> -
                                        <asp:TextBox ID="txtB0" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanB0" style="color: Red;"><b>*</b></span> 点数:<asp:TextBox ID="txtpoint0"
                                            runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                            <span id="spanpoint0" style="color: Red;"></span>
                                        现返:<asp:TextBox ID="txtMoney0" Width="115px" CssClass="inputtxtdat" runat="server"
                                            onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sAdd0" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd0" onclick="addTR(0)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="tr1" runat="server" class="hide">
                        <th class="w_td">
                        </th>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA1" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanA1" style="color: Red;"><b>*</b></span> -
                                        <asp:TextBox ID="txtB1" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanB1" style="color: Red;"><b>*</b></span> 点数:<asp:TextBox ID="txtpoint1"
                                            runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                            <span id="spanpoint1" style="color: Red;"></span>
                                        现返:<asp:TextBox ID="txtMoney1" Width="115px" CssClass="inputtxtdat" runat="server"
                                            onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sAdd1" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd1" onclick="addTR(1)" /></span>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sDelete1" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete1" onclick="delTR(1)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="tr2" runat="server" class="hide">
                        <th class="w_td">
                        </th>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA2" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanA2" style="color: Red;"><b>*</b></span> -
                                        <asp:TextBox ID="txtB2" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanB2" style="color: Red;"><b>*</b></span> 点数:<asp:TextBox ID="txtpoint2"
                                            runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                            <span id="spanpoint2" style="color: Red;"></span>
                                        现返:<asp:TextBox ID="txtMoney2" Width="115px" CssClass="inputtxtdat" runat="server"
                                            onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sAdd2" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd2" onclick="addTR(2)" /></span>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sDelete2" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete2" onclick="delTR(2)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="tr3" runat="server" class="hide">
                        <th class="w_td">
                        </th>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA3" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanA3" style="color: Red;"><b>*</b></span> -
                                        <asp:TextBox ID="txtB3" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanB3" style="color: Red;"><b>*</b></span> 点数:<asp:TextBox ID="txtpoint3"
                                            runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                            <span id="spanpoint3" style="color: Red;"></span>
                                        现返:<asp:TextBox ID="txtMoney3" Width="115px" CssClass="inputtxtdat" runat="server"
                                            onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sAdd3" runat="server">
                                            <input type="button" value=" 添  加 " id="btAdd3" onclick="addTR(3)" /></span>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sDelete3" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete3" onclick="delTR(3)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="tr4" runat="server" class="hide">
                        <th class="w_td">
                        </th>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtA4" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanA4" style="color: Red;"><b>*</b></span> -
                                        <asp:TextBox ID="txtB4" runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                        <span id="spanB4" style="color: Red;"><b>*</b></span> 点数:<asp:TextBox ID="txtpoint4"
                                            runat="server" CssClass="inputtxtdat" onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                            <span id="spanpoint4" style="color: Red;"></span>
                                        现返:<asp:TextBox ID="txtMoney4" Width="115px" CssClass="inputtxtdat" runat="server"
                                            onkeyup="value=value.replace(/[^0-9.]/g,'')"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s" id="sDelete4" runat="server">
                                            <input type="button" value=" 删  除 " id="btDelete4" onclick="delTR(4)" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <th class="w_td">
                            类型
                        </th>
                        <td class="alignLeft">
                            <table>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlbasetype" runat="server" CssClass="txt" onchange="showjk(this.value)">
                                            <asp:ListItem Value="1">本地</asp:ListItem>
                                            <asp:ListItem Value="2">接口</asp:ListItem>
                                            <asp:ListItem Value="3">共享</asp:ListItem>
                                        </asp:DropDownList>
                                        <span id="spanbasetype" style="color: Red;"><b>*</b></span>
                                    </td>
                                    <td>
                                        <span id="showjk" runat="server" class="hide">&nbsp;&nbsp;&nbsp;
                                            <%--<asp:DropDownList ID="ddljk" CssClass="txt" runat="server" AppendDataBoundItems="true">
                                                <asp:ListItem Value="0" Selected="True">--选择接口--</asp:ListItem>
                                            </asp:DropDownList>--%>
                                            <asp:CheckBoxList ID="cbljk" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="3">517</asp:ListItem>
                                            <asp:ListItem Value="4">百拓</asp:ListItem>
                                            <asp:ListItem Value="5">8000翼</asp:ListItem>
                                            <asp:ListItem Value="6">今日</asp:ListItem>
                                            <asp:ListItem Value="7">票盟</asp:ListItem>
                                            <asp:ListItem Value="8">51book</asp:ListItem>
                                            <asp:ListItem Value="10">易行</asp:ListItem>
                                            </asp:CheckBoxList>
                                            <span id="spanddljk" style="color: Red;"><b>*</b></span></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <th class="w_td">
                            时间范围
                        </th>
                        <td>
                            <input type="text" id="txtStartDate" style="width: 130px;" readonly="true" runat="server"
                                class="inputtxtdat" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />至
                             <span id="span3" style="color: Red;"><b>*</b></span>
                            <input type="text" id="txtEndDate" style="width: 130px;" readonly="true" runat="server"
                                class="inputtxtdat" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                        <span id="span4" style="color: Red;"><b>*</b></span>
                        </td>
                    </tr>
                </table>
                <table align="center">
                    <tr>
                        <td align="center" class="btni">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnAdd" runat="server" Text="保存" OnClick="btnAdd_Click" OnClientClick="return showAllErr()" />
                            </span>&nbsp; &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                                <asp:Button ID="btnBack" runat="server" Text="返 回" />
                            </span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <%--国内城市数据 ctu-成都--%>
    <input type="hidden" id="Hid_InnerCityData" runat="server" />
    <input type="hidden" id="FromCityCode" runat="server" />
    <input type="hidden" id="ToCityCode" runat="server" />
    <input type="hidden" runat="server" id="hidtxtCount" value="1" />
    <%--编辑部分 1编辑 0添加--%>
    <input type="hidden" id="Hid_IsEdit" value="0" runat="server" />
    <%--扣点组id--%>
    <input type="hidden" id="hid_showgroupdtl" runat="server" />
    <div id="showOne">
    </div>
    </form>
</body>
</html>
