<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PolicyFilter.aspx.cs" Inherits="Policy_PolicyFilter" %>

<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script id="script_k0" src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <script type="text/javascript" src="../js/js_CompareDate.js"></script>
    <style type="text/css">
        .style1
        {
            width: 89px;
        }
        
        .style3
        {
            width: 96px;
        }
        .style4
        {
            width: 173px;
        }
        .style6
        {
            width: 175px;
        }
        .style7
        {
            width: 97px;
        }
        .show
        {
            display: block;
        }
        .hide
        {
            display: none;
        }
    </style>
    <script language="javascript" type="text/javascript">
        //为Jquery重新命名
        var jQueryOne=jQuery.noConflict(false);
        //对话框
        function showdialog(t) {
            jQueryOne("#dgShow").html(t);
            jQueryOne("#dgShow").dialog({
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
                        jQueryOne(this).dialog('close');
                    }
                }
            });
        }
        //航空公司选择
        function GetCode(text,val,sel) {
            jQueryOne("#Hid_AirCode").val(val);
        }
        //出发城市选择
        function GetFromCode(text,val,sel) {
            jQuery("#FromCityCode").val(val);
        }
        //到达城市选择
        function GetMiddleCode(text,val,sel) {
            jQuery("#MiddleCityCode").val(val);
        }
        //到达城市选择
        function GetToCode(text,val,sel) {
            jQuery("#ToCityCode").val(val);
        }
        //调用
        function Call(btnObj,type) {
            if(type<=1) {
                var val_CarrayCode=jQueryOne.trim(jQueryOne("#Hid_AirCode").val());
                var val_FromCityCode=jQueryOne.trim(jQueryOne("#FromCityCode").val());
                var val_MiddleCityCode=jQueryOne.trim(jQueryOne("#MiddleCityCode").val());
                var val_ToCityCode=jQueryOne.trim(jQueryOne("#ToCityCode").val());
                //var val_CpyNo=jQueryOne.trim(jQueryOne("#Hid_CpyNo").val());
                var val_PolicyType=jQueryOne("input[name='PType'][type='radio']:checked").val();

                var val_StartDate=jQueryOne.trim(jQueryOne("#txtStartDate").val());
                var val_EndDate=jQueryOne.trim(jQueryOne("#txtEndDate").val());

                if(val_CarrayCode=="") {
                    showdialog("请选择航空公司！");
                } else if(val_StartDate!=""&&(val_StartDate==val_EndDate||CompareDate(val_StartDate,val_EndDate))) {
                    showdialog("乘机开始日期不能大于结束日期！");
                } else {
                    //传出参数
                    var param={
                        CarrayCode: escape(val_CarrayCode),
                        FromCityCode: escape(val_FromCityCode),
                        MiddleCityCode: escape(val_MiddleCityCode),
                        ToCityCode: escape(val_ToCityCode),
                        PolicyType: escape(val_PolicyType),
                        // CpyNo: escape(val_CpyNo),
                        StartDate: escape(val_StartDate),
                        EndDate: escape(val_EndDate),
                        SupType: escape(type),
                        // OpBtnObj: btnObj,//操作按钮对象                   
                        num: Math.random()
                    };
                    jQueryOne(btnObj).attr("disabled",true);
                    window.parent.PatchUpdatePause(param);
                    jQueryOne(btnObj).attr("disabled",false);
                }
            } else {
                //关闭
                window.parent.closeHTML();
            }
        }
    </script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
</head>
<body style="margin: 0px; padding: 0px;">
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
    <div id="dgShow">
    </div>
    <div id="divPause">
    </div>
    <form id="form1" runat="server">
    <table style="border-collapse: collapse;" border="1" cellspacing="0" cellpadding="0">
        <tr>
            <th class="style1" align="right">
                航空公司：
            </th>
            <td>
                <uc1:SelectAirCode ID="ddlAirCode" IsDShowName="false" IsShowAll="true" DefaultOptionValue=""
                    ChangeFunctionName="GetCode" runat="server" />
            </td>
            <th class="style3" align="right">
                出发城市：
            </th>
            <td class="style4">
                <%-- 出发城市--%>
                <uc1:SelectAirCode ID="ddlFromCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--出发城市--"
                    ChangeFunctionName="GetFromCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />
            </td>
            <th align="right">
                中转城市：
            </th>
            <td class="style6" align="left">
                <%-- 中转城市--%>
                <uc1:SelectAirCode ID="ddlMiddleCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--到达城市--"
                    ChangeFunctionName="GetMiddleCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />
            </td>
            <td class="style7">
            </td>
        </tr>
        <tr>
            <th class="style1" align="right">
                政策类型：
            </th>
            <td>
                <span>
                    <label for="rdPType1" style="text-align: left">
                        <input id="rdPType1" type="radio" name="PType" value="1" checked="checked" />B2B
                    </label>
                    <label for="rdPType2">
                        <input id="rdPType2" type="radio" name="PType" value="2" />BSP
                    </label>
                    <label for="rdPType3">
                        <input id="rdPType3" type="radio" name="PType" value="3" />全部
                    </label>
                </span>
            </td>
            <th class="style4" align="right">
                到达城市：
            </th>
            <td class="style4">
                <%-- 到达城市--%>
                <uc1:SelectAirCode ID="ddlToCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--到达城市--"
                    ChangeFunctionName="GetToCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />
            </td>
            <th align="right">
                乘机日期：
            </th>
            <td colspan="3" align="left">
                <div>
                    <input id="txtStartDate" runat="server" class="inputBorder" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                        readonly="true" style="width: 130px;" type="text" />至
                    <input id="txtEndDate" runat="server" class="inputBorder" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd',onpicked:function() {$dp.$('txtTicketEndDate').value=$dp.cal.getP('y')+'-'+$dp.cal.getP('M')+'-'+$dp.cal.getP('d');}})"
                        readonly="true" style="width: 130px;" type="text" />
                </div>
            </td>
        </tr>
        <tr>
            <td class="style1">
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td class="style3">
                &nbsp;
            </td>
            <td class="style4">
                <span class="btn btn-ok-s">
                    <input id="btnGuaQi" type="button" value="挂起" onclick="Call(this,1)" />
                </span><span class="btn btn-ok-s">
                    <input id="Button1" type="button" value="解挂" onclick="Call(this,0)" />
                </span><span class="btn btn-ok-s">
                    <input id="Button2" type="button" value="关闭" onclick="Call(this,2)" />
                </span>
            </td>
            <td>
                &nbsp;
            </td>
            <td class="style6">
                &nbsp;
            </td>
            <td class="style7">
                &nbsp;
            </td>
        </tr>
    </table>
    <input type="hidden" id="Hid_AirCode" runat="server" />
    <input type="hidden" id="FromCityCode" runat="server" />
    <input type="hidden" id="MiddleCityCode" runat="server" />
    <input type="hidden" id="ToCityCode" runat="server" />
    <script language="javascript" type="text/javascript">
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../JS/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../JS/CitySelect/fltdomestic_gb2312.js"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        jQueryOne(function () {
            ReLoad();
        });
    </script>
    </form>
</body>
</html>
