<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupPolicyList.aspx.cs"
    Inherits="Policy_GroupPolicyList" %>

<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="tleName" runat="server">散冲团政策列表</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <style type="text/css">
        .NBor
        {
            border: 0px;
        }
        .YBor
        {
            border: 1px solid #999;
        }
        .show
        {
            display: block;
        }
        .hide
        {
            display: none;
        }
        .Operation
        {
            width: 80px;
        }
        .tdWidth100
        {
            width: 100px;
        }
        .tdWidth150
        {
            width: 150px;
        }
        .tdWidth200
        {
            width: 200px;
        }
        .CelueTab
        {
            width: 100%;
            border-collapse: collapse;
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
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
        }
        .SelTr
        {
            background-color: #6593E9;
            border: 1px solid green;
        }
    </style>
    <script language="javascript" type="text/javascript">
        //设置当前选择的行样式 由id控制哦
        function SetCurrSelStyle(id) {
            // jQuery("#tr_item"+id)[0].bgColor="green";
            //设置样式
        }
        //航空公司选择
        function GetCode(text,val,sel) {

        }
        //-------------String扩展新方法-------------------------------------------
        //扩展新方法
        String.prototype.NewReplace=function (sourceData,replaceData) {
            sourceData=sourceData.replace("(","\\(").replace(")","\\)");
            var reg=new RegExp(sourceData,"ig");
            var data=this.replace(reg,replaceData);
            return data;
        }
        //padLeft(10,'0')
        String.prototype.padLeft=function (length,char) {
            var d=this;
            var len=d.length;
            while(len<length) {
                d=char+d;
                len++;
            }
            return d;
        }
        //padRight(10,'0')
        String.prototype.padRight=function (length,char) {
            var d=this;
            var len=d.length;
            while(len<length) {
                d=d+char;
                len++;
            }
            return d;
        }
        //对话框
        function showdialog(t,f) {
            jQuery("#dgShow").html(t);
            jQuery("#dgShow").dialog({
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
                        //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策
                        var PageType=jQuery("#Hid_PageType").val();
                        if(f=="0") {
                            //跳到登录页面                           
                            top.location.href="../Login.aspx";
                        }
                        jQuery(this).dialog('close');
                        if(f=="1") {
                            //处理成功 查询按钮
                            jQuery("#btnQuery").click();
                        }
                    }
                }
            });
        }
        function NumVate() {
            try {
                var value=jQuery.trim(jQuery(this).val());
                if(value==null||value=='') {
                    showdialog("文本框不能为空,请输入正确的数字!");
                    jQuery(this).val("0");
                    return false;
                }
                var ctrlId=jQuery(this).attr("id");
                //数据范围0-100
                var NumFanWei=jQuery(this).attr("FanWei");
                if(!isNaN(value)) {
                    var userreg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
                    if(userreg.test(value)) {
                        //验证通过
                        if(parseFloat(value).toString().length>5) {
                            jQuery(this).val("0");
                            showdialog("输入数据超出范围!");
                            return false;
                        }
                        var idArr=[
                        'txtDownRebate_',
                        'txtLaterRebate_',
                        'txtShareRebate_',
                        'txtAirRebate_',
                        'txtBuildPrice_'
                        ];
                        var isXianZhi=false;
                        for(var i=0;i<ctrlId.length;i++) {
                            if(ctrlId.indexOf(ctrlId[i])!= -1) {
                                isXianZhi=true;
                                break;
                            }
                        }
                        //数据范围有限制
                        if(isXianZhi&&NumFanWei!=null) {
                            var xzArr=NumFanWei.split('-');
                            if(xzArr.length==2) {
                                var min=parseFloat(xzArr[0],10);
                                var max=parseFloat(xzArr[1],10);
                                var curVal=parseFloat(value,10);
                                if(curVal>=max||curVal<min) {
                                    jQuery(this).val("0");
                                    showdialog("输入数据超出范围["+NumFanWei+"]!");
                                    return false;
                                }
                            }
                        }

                    } else {
                        var numindex=parseInt(value.indexOf("."),10);
                        if(numindex==0) {
                            jQuery(this).val("0");
                            showdialog("输入的数字不规范");
                            return false;
                        }
                        var head=value.substring(0,numindex);
                        var bottom=value.substring(numindex,numindex+3);
                        var fianlNum=head+bottom;
                        jQuery(this).val(fianlNum);
                    }
                } else {
                    jQuery(this).val("0");
                    showdialog("请输入数字");
                    return false;
                }
                var s='0';
                if(NumFanWei!=null) {
                    var x=parseFloat(value,10);
                    s=ShowPoint(x,1);//保留一位小数                    
                    jQuery(this).val(s);
                } else {
                    if(!jQuery.isNaN(value)) {
                        if(ctrlId.indexOf("txtAdvanceDays_")!= -1) {
                            jQuery(this).val(parseInt(value,10));
                        }
                        else {
                            jQuery(this).val(parseFloat(value,10));
                        }
                    }
                }
            } catch(e) {
                alert(e.message);
            }
        }
        //数字 保留小数位数
        function ShowPoint(number,PointNum) {
            var tempNum=number.toString();
            if(tempNum.indexOf(".")!= -1)//存在小数点
            {
                var strArr=tempNum.split('.');
                if(strArr[1].length>PointNum) {
                    strArr[1]=strArr[1].substring(0,PointNum);
                } else {
                    strArr[1].padRight(PointNum,'0');
                }
                tempNum=strArr[0]+"."+strArr[1];
            }
            return parseFloat(tempNum,10);
        }
        function SetTabStyle() {
            var PageType=jQuery("#Hid_PageType").val();
            jQuery("#secTable tr td[id*='tdPolicy_']").each(function (index,td) {
                jQuery(td).removeClass("sec2 sec1");
                if(jQuery(td).attr("id")=="tdPolicy_"+PageType) {
                    jQuery(td).addClass("sec2");
                } else {
                    jQuery(td).addClass("sec1");
                }
            });
        }
        jQuery(function () {
            SetTabStyle();

            //验证
            jQuery("input[type='text'][id*='txtDownRebate_']").blur(NumVate);
            jQuery("input[type='text'][id*='txtLaterRebate_']").blur(NumVate);
            jQuery("input[type='text'][id*='txtShareRebate_']").blur(NumVate);
            jQuery("input[type='text'][id*='txtAirRebate_']").blur(NumVate);
            jQuery("input[type='text'][id*='txtBuildPrice_']").blur(NumVate);
            jQuery("input[type='text'][id*='txtOilPrice_']").blur(NumVate);

            jQuery("input[type='text'][id*='txtAdvanceDays_']").blur(NumVate);
            jQuery("input[type='text'][id*='txtSeatCount_']").blur(NumVate);


        });
        //清空空数据
        function EmptyArr(arr) {
            var newArr=[];
            for(var i=0;i<arr.length;i++) {
                if(jQuery.trim(arr[i])!="") {
                    newArr.push(arr[i]);
                }
            }
            return newArr;
        }
        //日期大小比较 返回true false
        function CompareDate(sdate,edate) {
            var strDate1=sdate.split('-');
            var date1;
            if(strDate1.length==3) {
                date1=new Date(strDate1[0],strDate1[1],strDate1[2]);
            }
            var strDate2=edate.split('-');
            var date2;
            if(strDate1.length==3) {
                date2=new Date(strDate2[0],strDate2[1],strDate2[2]);
            }
            return (date1>date2);
        }
        function showUpdate(id) {
            //局部修改
            jQuery("#divContainer_"+id).hide();
            jQuery("#divUpdateCon_"+id).show();

            jQuery("#show_AirCode_"+id).hide();
            jQuery("#hide_AirCode_"+id).show();

            jQuery("#show_FlightNo_"+id).hide();
            jQuery("#hide_FlightNo_"+id).show();

            jQuery("#show_FromCityCode_"+id).hide();
            jQuery("#hide_FromCityCode_"+id).show();

            jQuery("#show_ToCityCode_"+id).hide();
            jQuery("#hide_ToCityCode_"+id).show();

            jQuery("#show_Class_"+id).hide();
            jQuery("#hide_Class_"+id).show();

            jQuery("#show_BuildPrice_"+id).hide();
            jQuery("#hide_BuildPrice_"+id).show();

            jQuery("#show_OilPrice_"+id).hide();
            jQuery("#hide_OilPrice_"+id).show();

            jQuery("#show_SeatCount_"+id).hide();
            jQuery("#hide_SeatCount_"+id).show();


            jQuery("#show_AdvanceDays_"+id).hide();
            jQuery("#hide_AdvanceDays_"+id).show();

            jQuery("#show_OfficeCode_"+id).hide();
            jQuery("#hide_OfficeCode_"+id).show();


            jQuery("#show_youhui_"+id).hide();
            jQuery("#hide_youhui_"+id).show();

            jQuery("#ul_show_"+id).hide();
            jQuery("#ul_hide_"+id).show();

            jQuery("#show_Flight_"+id).hide();
            jQuery("#hide_Flight_"+id).show();

            jQuery("#show_Print_"+id).hide();
            jQuery("#hide_Print_"+id).show();
            return false;
        }

        function hideUpdate(id) {
            //局部修改
            jQuery("#divContainer_"+id).show();
            jQuery("#divUpdateCon_"+id).hide();

            jQuery("#show_AirCode_"+id).show();
            jQuery("#hide_AirCode_"+id).hide();

            jQuery("#show_FlightNo_"+id).show();
            jQuery("#hide_FlightNo_"+id).hide();

            jQuery("#show_FromCityCode_"+id).show();
            jQuery("#hide_FromCityCode_"+id).hide();

            jQuery("#show_ToCityCode_"+id).show();
            jQuery("#hide_ToCityCode_"+id).hide();

            jQuery("#show_Class_"+id).show();
            jQuery("#hide_Class_"+id).hide();

            jQuery("#show_BuildPrice_"+id).show();
            jQuery("#hide_BuildPrice_"+id).hide();

            jQuery("#show_OilPrice_"+id).show();
            jQuery("#hide_OilPrice_"+id).hide();

            jQuery("#show_SeatCount_"+id).show();
            jQuery("#hide_SeatCount_"+id).hide();

            jQuery("#show_AdvanceDays_"+id).show();
            jQuery("#hide_AdvanceDays_"+id).hide();

            jQuery("#show_OfficeCode_"+id).show();
            jQuery("#hide_OfficeCode_"+id).hide();

            jQuery("#show_youhui_"+id).show();
            jQuery("#hide_youhui_"+id).hide();

            jQuery("#ul_show_"+id).show();
            jQuery("#ul_hide_"+id).hide();

            jQuery("#show_Flight_"+id).show();
            jQuery("#hide_Flight_"+id).hide();

            jQuery("#show_Print_"+id).show();
            jQuery("#hide_Print_"+id).hide();

            return false;
        }
        function ajaxUpdate(id,opType) {
            //公司编号
            var val_CpyNo=jQuery("#Hid_CpyNo").val();
            //公司名称
            var val_CpyName=jQuery("#Hid_CpyName").val();
            //登录账号
            var val_LoginName=jQuery("#Hid_LoginName").val();
            //供应商名字
            var val_UserName=jQuery("#Hid_UserName").val();
            //页面类型
            var PageType=jQuery("#Hid_PageType").val();


            var AirCode=jQuery("#txtAirCode_"+id).val();
            var FlightNo=jQuery("#txtFlightNo_"+id).val();
            var fromCode=jQuery("#txtFromCityCode_"+id).val();
            var toCode=jQuery("#txtToCityCode_"+id).val();
            var Seat=jQuery("#txtClass_"+id).val();
            var AircraftFare=jQuery("#txtBuildPrice_"+id).val();
            var RQFare=jQuery("#txtOilPrice_"+id).val();

            var SeatCount=jQuery("#txtSeatCount_"+id).val();
            var AdvanceDays=jQuery("#txtAdvanceDays_"+id).val();
            var OfficeCode=jQuery("#txtOfficeCode_"+id).val();


            var val_DownPoint=jQuery("#txtDownRebate_"+id).val();
            var val_LaterPoint=jQuery("#txtLaterRebate_"+id).val();
            var val_SharePoint=jQuery("#txtShareRebate_"+id).val();
            var val_AirReBate=jQuery("#txtAirRebate_"+id).val();

            var youhui=jQuery("#txtyouhui_"+id).val();
            var Prices=jQuery("#Hid_Prices_"+id).val();
            var Rebate=jQuery("#Hid_Rebate_"+id).val();
            var PriceType=jQuery("#hid_PriceType_"+id).val().toLowerCase();
            if(PriceType=="1"||PriceType=="true") {
                Prices=youhui;
            } else {
                Rebate=youhui;
            }
            var patern=/^\d+$/;
            if(!patern.test(SeatCount)) {
                showdialog('请输舱位个数格式错误,必须为整数！');
                return false;
            }
            if(!patern.test(AdvanceDays)) {
                showdialog('输入提前天数格式错误,必须为整数！');
                return false;
            }
            var carrcode=EmptyArr(AirCode.split('/'));
            if(carrcode.length==0) {
                showdialog('请输入承运人二字码！');
                return false;
            }
            if(carrcode.length>0&&carrcode[0].length>3) {
                showdialog('输入承运人二字码错误！');
                return false;
            }

            var fromCodeArr=EmptyArr(fromCode.split('/'));
            if(fromCodeArr.length==0) {
                showdialog('请输入出发城市三字码！');
                return false;
            } else {
                if(fromCodeArr.length>1) {
                    showdialog('只能输入一个出发城市三字码！');
                    return false;
                }
            }
            var toCodeArr=EmptyArr(toCode.split('/'));
            if(toCodeArr.length==0) {
                showdialog('请输入到达城市三字码！');
                return false;
            } else {
                if(toCodeArr.length>1) {
                    showdialog('只能输入一个到达城市三字码！');
                    return false;
                }
            }
            var SeatArr=EmptyArr(Seat.split('/'));
            if(SeatArr.length==0) {
                showdialog('请输入舱位！');
                return false;
            } else {
                if(SeatArr.length>1||SeatArr[0].length>1) {
                    showdialog('只能输入一个舱位！');
                    return false;
                }
            }


            if(jQuery.isNaN(AircraftFare)) {
                showdialog('输入机建格式错误,必须为数字！');
                return false;
            }
            if(jQuery.isNaN(RQFare)) {
                showdialog('输入燃油格式错误,必须为数字！');
                return false;
            }
            if(jQuery.isNaN(val_AirReBate)) {
                showdialog('输入航空公司返点格式错误,必须为数字！');
                return false;
            }
            if(jQuery.isNaN(val_DownPoint)) {
                showdialog('输入下级分销返点格式错误,必须为数字！');
                return false;
            }
            if(jQuery.isNaN(val_LaterPoint)) {
                showdialog('输入下级分销后返格式错误,必须为数字！');
                return false;
            }
            if(jQuery.isNaN(val_SharePoint)) {
                showdialog('输入共享政策返点格式错误,必须为数字！');
                return false;
            }

            var val_FlightStartDate=jQuery("#txtFlightStartDate_"+id).val();
            var val_FlightEndDate=jQuery("#txtFlightEndDate_"+id).val();

            var val_PrintStartDate=jQuery("#txtPrintStartDate"+id).val();
            var val_PrintEndDate=jQuery("#txtPrintEndDate"+id).val();

            if(jQuery.trim(val_FlightStartDate)=="") {
                showdialog('乘机日期开始时间不能为空！');
                return false;
            }
            if(jQuery.trim(val_FlightEndDate)=="") {
                showdialog('乘机日期结束时间不能为空！');
                return false;
            }
            if(jQuery.trim(val_PrintStartDate)=="") {
                showdialog('出票日期开始时间不能为空！');
                return false;
            }
            if(jQuery.trim(val_PrintEndDate)=="") {
                showdialog('出票日期结束时间不能为空！');
                return false;
            }


            //乘机日期
            var d1=CompareDate(val_FlightStartDate,val_FlightEndDate);
            if(d1) {
                showdialog('政策开始有效日期不能大于结束有效日期！');
                return false;
            }
            //出票日期
            var d1=CompareDate(val_PrintStartDate+':00',val_PrintEndDate+':00');
            if(d1) {
                showdialog('出票开始有效日期不能大于出票结束有效日期！');
                return false;
            }
            //验证
            jQuery("#a_update_"+id).attr("disabled",true);
            jQuery("#a_copy_"+id).attr("disabled",true);
            jQuery("#a_del_"+id).attr("disabled",true);
            jQuery.post("../AJAX/CommonAjAx.ashx",
            {
                //参数区域
                CpyNo: escape(val_CpyNo),
                CpyName: escape(val_CpyName),
                LoginName: escape(val_LoginName),
                UserName: escape(val_UserName),
                AirCode: escape(AirCode),
                FromCode: escape(fromCode),
                ToCode: escape(toCode),
                FlightNo: escape(FlightNo),
                Class: escape(Seat),
                FlightStartDate: escape(val_FlightStartDate),
                FlightEndDate: escape(val_FlightEndDate),
                PrintStartDate: escape(val_PrintStartDate),
                PrintEndDate: escape(val_PrintEndDate),
                AdvanceDays: escape(AdvanceDays),
                SeatCount: escape(SeatCount),
                Prices: escape(Prices),
                Rebate: escape(Rebate),
                OilPrice: escape(RQFare),
                BuildPrice: escape(AircraftFare),
                AirRebate: escape(val_AirReBate),
                DownRebate: escape(val_DownPoint),
                LaterRebate: escape(val_LaterPoint),
                ShareRebate: escape(val_SharePoint),
                OfficeCode: escape(OfficeCode),

                OpFunction: escape("GroupPolicy"),
                OpType: escape(opType),
                Id: escape(id),
                num: Math.random(),
                currentuserid: "<%=this.mUser.id.ToString() %>"
            },
             function (data) {
                 jQuery("#a_update_"+id).attr("disabled",false);
                 jQuery("#a_copy_"+id).attr("disabled",false);
                 jQuery("#a_del_"+id).attr("disabled",false);
                 var strReArr=data.split('##');
                 if(strReArr.length==3) {
                     //错误代码
                     var errCode=strReArr[0];
                     //错误描述
                     var errDes=strReArr[1];
                     //错误结果
                     var result=jQuery.trim(unescape(strReArr[2]));
                     if(errCode=="1") {//处理成功
                         showdialog(errDes,"1");
                         hideUpdate(id);
                     } else {
                         showdialog(errDes);
                     }
                 }

             },"text");

            return false;
        }
        //全选
        function SelectAll(obj) {
            jQueryOne("input[name*='ItemCk'][type='checkbox']").attr("checked",obj.checked);
        }
        function DataValidate(obj,optype,id) {
            var ArrIds=[];
            var IDS='';
            if(id==undefined||id==null) {
                jQuery("input[name*='ItemCk'][type='checkbox']:checked").each(function (i,val) {
                    ArrIds.push("'"+jQuery(val).val()+"'");
                });
                if(ArrIds.length==0) {
                    showdialog("请选择复选框！");
                    return false;
                }
                IDS=ArrIds.join(",");
            } else {
                IDS=id;
            }
            jQuery(obj).attr("disabled",true);
            jQuery.post("../AJAX/CommonAjAx.ashx",
                {
                    OpFunction: escape("GroupPolicy"),
                    OpType: escape(optype),
                    Id: escape(IDS),
                    num: Math.random(),
                    currentuserid: "<%=this.mUser.id.ToString() %>"
                },
                 function (data) {
                     jQuery(obj).attr("disabled",false);
                     var strReArr=data.split('##');
                     if(strReArr.length==3) {
                         //错误代码
                         var errCode=strReArr[0];
                         //错误描述
                         var errDes=strReArr[1];
                         //错误结果
                         var result=jQuery.trim(unescape(strReArr[2]));
                         if(errCode=="1") {
                             if(optype=="2") {
                                 showdialog("删除成功！","1");
                             }
                         } else {
                             if(optype=="2") {
                                 showdialog("删除失败！");
                             }
                         }
                     }
                 },"text");

            return false;
        }
    </script>
</head>
<body>
    <div id="dgShow">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <%--审核选项卡开始--%>
    <div id="tab">
        <table id="secTable" border="0" cellspacing="0" cellpadding="0">
            <tr align="left">
                <td class="sec2" width="97px" id="tdPolicy_1">
                    <asp:LinkButton ID="btn1" runat="server" Height="27px" Style="line-height: 24px;
                        text-decoration: none; color: #666;" OnClick="btn1_Click">普通政策</asp:LinkButton>
                    <asp:Label runat="server" ID="labPolicy_1" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td class="sec1" width="97px" id="tdPolicy_2">
                    <asp:LinkButton ID="btn2" runat="server" Height="27px" Style="line-height: 24px;
                        text-decoration: none; color: #666;" OnClick="btn2_Click">特价政策</asp:LinkButton>
                    <asp:Label runat="server" ID="labPolicy_2" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td class="sec1" width="97px" id="tdPolicy_3">
                    <asp:LinkButton ID="btn3" runat="server" Height="27px" Style="line-height: 24px;
                        text-decoration: none; color: #666;" OnClick="btn3_Click">默认政策</asp:LinkButton>
                    <asp:Label runat="server" ID="labPolicy_3" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td class="sec1" width="97px" id="tdPolicy_4">
                    <asp:LinkButton ID="btn4" runat="server" Height="27px" Style="line-height: 24px;
                        text-decoration: none; color: #666;" OnClick="btn4_Click">散冲团政策</asp:LinkButton>
                    <asp:Label runat="server" ID="labPolicy_4" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div class="title" style="width: 100%">
        <ul style="list-style-type: none; margin: 0 0; padding: 0 0; position: relative;
            width: 100%">
            <li>
                <asp:Label ID="lblShow" Text="散冲团政策列表" runat="server" />
            </li>
        </ul>
    </div>
    <div class="container">
        <table class="Search" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <th>
                    承运人：
                </th>
                <td>
                    <uc1:SelectAirCode ID="ddlAirCode" IsDShowName="false" IsShowAll="true" DefaultOptionValue=""
                        ChangeFunctionName="GetCode" runat="server" />
                </td>
                <th>
                    出发城市：
                </th>
                <td>
                    <uc1:SelectAirCode ID="ddlFromCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--出发城市--"
                        DefaultOptionValue="" InputMaxLength="3" runat="server" />
                </td>
                <th>
                    到达城市：
                </th>
                <td colspan="2">
                    <uc1:SelectAirCode ID="ddlToCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--到达城市--"
                        DefaultOptionValue="" InputMaxLength="3" runat="server" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <th>
                    舱位:
                </th>
                <td>
                    <uc1:SelectAirCode ID="ddlClass" IsDShowName="false" IsShowAll="true" DefaultOptionText="--选择舱位--"
                        DefaultOptionValue="" InputMaxLength="1" runat="server" />
                </td>
                <th>
                    政策类型：
                </th>
                <td>
                    <asp:DropDownList runat="server" ID="ddlPolicyType">
                        <asp:ListItem Value="-1" Text="全部" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="B2B"></asp:ListItem>
                        <asp:ListItem Value="2" Text="BSP"></asp:ListItem>
                        <%--  <asp:ListItem Value="3" Text="B2B/BSP"></asp:ListItem>--%>
                    </asp:DropDownList>
                </td>
                <th>
                    政策有效日期：
                </th>
                <td colspan="2">
                    <input type="text" readonly="readonly" id="txtFlightStartDate" runat="server" onfocus="WdatePicker({isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})"
                        style="width: 150px;" class="Wdate inputtxtdat" />-<input type="text" readonly="readonly"
                            id="txtFlightEndDate" runat="server" onfocus="WdatePicker({isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd  HH:mm'})"
                            style="width: 150px;" class="Wdate inputtxtdat" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <th>
                    发布者：
                </th>
                <td>
                    <uc1:SelectAirCode ID="SelPublic" DefaultOptionText="--发布者--" DefaultOptionValue=""
                        IsDShowName="false" runat="server" InputMaxLength="50" />
                </td>
                <th>
                    优惠方式:
                </th>
                <td>
                    <asp:DropDownList runat="server" ID="ddlPriceType" Style="width: 80px;">
                        <asp:ListItem Value="-1" Text="全部" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="0" Text="折扣"></asp:ListItem>
                        <asp:ListItem Value="1" Text="价格"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <th>
                    出票日期：
                </th>
                <td colspan="2">
                    <input type="text" readonly="readonly" id="txtTicketStartDate" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                        style="width: 150px;" class="Wdate inputtxtdat" />-<input type="text" readonly="readonly"
                            id="txtTicketEndDate" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                            style="width: 150px;" class="Wdate inputtxtdat" />
                </td>
                <td>
                </td>
            </tr>
        </table>
    </div>
    <div class="c-list-filter-go">
        <table cellspacing="0" cellpadding="0" border="0">
            <tbody>
                <tr>
                    <td align="center" colspan="4">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" />
                        </span><span class="btn btn-ok-s">
                            <asp:Button ID="btnClear" runat="server" Text="重置数据" OnClick="btnClear_Click" />
                        </span><span class="btn btn-ok-s" id="btnadd" runat="server">
                            <asp:Button ID="btnInsert" runat="server" Text="新增政策" OnClick="btnInsert_Click" />
                        </span><span class="btn btn-ok-s" id="span_del" runat="server">
                            <asp:Button ID="btnDelete" runat="server" Text="批量删除" OnClientClick="return DataValidate(this,2);" />
                        </span><span class="btn btn-ok-s">
                            <asp:Button ID="btnExpiresQuery" runat="server" Text="过期政策查询" OnClick="btnExpiresQuery_Click" />
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <table id="tb-all-trade" class="tb-all-trade" border="1" cellspacing="0" cellpadding="0"
        width="100%">
        <tr>
            <th>
                <input type="checkbox" id="headerCk" name="headerCk" onclick="SelectAll(this)" />
            </th>
            <th>
                操作
            </th>
            <th>
                承运人
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
                舱位
            </th>
            <th>
                起飞抵达时间
            </th>
            <th>
                机型
            </th>
            <th>
                机建
            </th>
            <th>
                燃油
            </th>
            <th>
                座位数
            </th>
            <th>
                提前天数
            </th>
            <th>
                出票Office
            </th>
            <th>
                类型
            </th>
            <th>
                政策
            </th>
            <th>
                发布者
            </th>
            <th>
                操作者
            </th>
            <th>
                政策适用时间
            </th>
            <th>
                出票有效期
            </th>
        </tr>
        <asp:Repeater ID="Repeater" runat="server" OnPreRender="Repeater_PreRender" OnItemCommand="Repeater_ItemCommand">
            <ItemTemplate>
                <tr id="tr_item<%#Eval("Id") %>" onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                    <td>
                        <input type="checkbox" id="ItemCk" name="ItemCk" value='<%#Eval("Id") %>' runat="server" />
                    </td>
                    <td>
                        <span id='span_edit_<%#Eval("Id") %>'><a href="<%# GetUrl(Eval("Id").ToString(),Eval("CpyNo").ToString())%>">
                            修改</a><br />
                        </span>
                        <div id='divContainer_<%#Eval("Id") %>'>
                            <a id="a_<%#Eval("Id") %>" href="#" onclick="return showUpdate('<%#Eval("Id") %>')">
                                局部修改</a></div>
                        <div id="divUpdateCon_<%#Eval("Id") %>" class="hide">
                            <span id="span_update_<%#Eval("Id") %>"><a id='a_update_<%#Eval("Id") %>' href="#"
                                onclick="return ajaxUpdate('<%#Eval("Id") %>','1')">更新</a></span> <span id="span_cancel_<%#Eval("Id") %>">
                                    <a id='a_cancel_<%#Eval("Id") %>' href="#" onclick="return hideUpdate('<%#Eval("Id") %>')">
                                        取消</a></span>
                            <br />
                        </div>
                        <span id="span_copy_<%#Eval("Id") %>"><a id='a_copy_<%#Eval("Id") %>' href="#" onclick="return ajaxUpdate('<%#Eval("Id") %>','4')">
                            复制</a><br />
                        </span><span id="span_del_<%#Eval("Id") %>"><a id='a_del_<%#Eval("Id") %>' href="#"
                            onclick="return ajaxUpdate('<%#Eval("Id") %>','2')">删除</a><br />
                        </span>
                    </td>
                    <td>
                        <span id="show_AirCode_<%#Eval("Id") %>">
                            <%#Eval("AirCode")%></span> <span id="hide_AirCode_<%#Eval("Id") %>" class="hide">
                                <input type="text" id="txtAirCode_<%#Eval("Id") %>" value='<%#Eval("AirCode")%>'
                                    size="5" />
                            </span>
                    </td>
                    <td>
                        <span id="show_FlightNo_<%#Eval("Id") %>">
                            <%#Eval("FlightNo")%></span> <span id="hide_FlightNo_<%#Eval("Id") %>" class="hide">
                                <input type="text" id="txtFlightNo_<%#Eval("Id") %>" value='<%#Eval("FlightNo")%>'
                                    size="5" />
                            </span>
                    </td>
                    <td>
                        <span id="show_FromCityCode_<%#Eval("Id") %>">
                            <%#Eval("FromCityCode")%></span> <span id="hide_FromCityCode_<%#Eval("Id") %>" class="hide">
                                <input type="text" id="txtFromCityCode_<%#Eval("Id") %>" value='<%#Eval("FromCityCode")%>'
                                    size="5" />
                            </span>
                    </td>
                    <td>
                        <span id="show_ToCityCode_<%#Eval("Id") %>">
                            <%#Eval("ToCityCode")%></span> <span id="hide_ToCityCode_<%#Eval("Id") %>" class="hide">
                                <input type="text" id="txtToCityCode_<%#Eval("Id") %>" value='<%#Eval("ToCityCode")%>'
                                    size="5" />
                            </span>
                    </td>
                    <td>
                        <span id="show_Class_<%#Eval("Id") %>">
                            <%# Eval("Class")%>
                        </span><span id="hide_Class_<%#Eval("Id") %>" class="hide">
                            <input type="text" id="txtClass_<%#Eval("Id") %>" value='<%#Eval("Class")%>' size="5" />
                        </span>
                    </td>
                    <td>
                        <%# ShowItem(1,Eval("FlightTime").ToString(),"","") %>
                    </td>
                    <td>
                        <%# Eval("PlaneType")%>
                    </td>
                    <td>
                        <span id="show_BuildPrice_<%#Eval("Id") %>">
                            <%# Eval("BuildPrice")%>
                        </span><span id='hide_BuildPrice_<%#Eval("Id") %>' class="hide">
                            <input type="text" id='txtBuildPrice_<%#Eval("Id") %>' value='<%# Eval("BuildPrice")%>'
                                size="4" />
                        </span>
                    </td>
                    <td>
                        <span id="show_OilPrice_<%#Eval("Id") %>">
                            <%# Eval("OilPrice")%>
                        </span><span id='hide_OilPrice_<%#Eval("Id") %>' class="hide">
                            <input type="text" id='txtOilPrice_<%#Eval("Id") %>' value='<%# Eval("OilPrice")%>'
                                size="4" />
                        </span>
                    </td>
                    <td>
                        <span id="show_SeatCount_<%#Eval("Id") %>">
                            <%# Eval("SeatCount")%>
                        </span><span id='hide_SeatCount_<%#Eval("Id") %>' class="hide">
                            <input type="text" id='txtSeatCount_<%#Eval("Id") %>' value='<%# Eval("SeatCount")%>'
                                size="4" />
                        </span>
                    </td>
                    <td>
                        <span id="show_AdvanceDays_<%#Eval("Id") %>">
                            <%# Eval("AdvanceDays")%>
                        </span><span id='hide_AdvanceDays_<%#Eval("Id") %>' class="hide">
                            <input type="text" id='txtAdvanceDays_<%#Eval("Id") %>' value='<%# Eval("AdvanceDays")%>'
                                size="4" />
                        </span>
                    </td>
                    <td>
                        <span id="show_OfficeCode_<%#Eval("Id") %>">
                            <%# Eval("OfficeCode")%>
                        </span><span id='hide_OfficeCode_<%#Eval("Id") %>' class="hide">
                            <input type="text" id='txtOfficeCode_<%#Eval("Id") %>' value='<%# Eval("OfficeCode")%>'
                                size="7" />
                        </span>
                    </td>
                    <td>
                        <ul class="ulClass">
                            <input type="hidden" id='hid_PriceType_<%#Eval("Id") %>' value='<%#Eval("PriceType") %>' />
                            <input type="hidden" id='Hid_Prices_<%#Eval("Id") %>' value='<%#Eval("Prices") %>' />
                            <input type="hidden" id='Hid_Rebate_<%#Eval("Id") %>' value='<%#Eval("Rebate") %>' />
                            <li>政策类型:<%# ShowItem(2,Eval("PolicyType").ToString(),"","")%></li>
                            <li><span id='show_youhui_<%#Eval("Id") %>'>优惠<%# ((Eval("PriceType").ToString() == "1" || Eval("PriceType").ToString().ToLower() == "true")?"价格:":"折扣:")+ShowItem(3, Eval("PriceType").ToString(), Eval("Rebate").ToString(), Eval("Prices").ToString())%>
                            </span><span id='hide_youhui_<%#Eval("Id") %>' class="hide">优惠<%# (Eval("PriceType").ToString() == "1" || Eval("PriceType").ToString().ToLower() == "true")?"价格":"折扣"%>
                                <input type="text" id='txtyouhui_<%#Eval("Id") %>' value='<%# ShowItem(3, Eval("PriceType").ToString(), Eval("Rebate").ToString(), Eval("Prices").ToString())%>'
                                    size="4" />
                            </span></li>
                        </ul>
                    </td>
                    <td>
                        <ul class="ulClass" id='ul_show_<%#Eval("Id") %>'>
                            <li><span id="span_DownRebate">下级：
                                <%#Eval("DownRebate")%></span></li>
                            <li><span id="span_LaterRebate">后返：
                                <%#Eval("LaterRebate")%></span></li>
                            <li><span id="span_ShareRebate">共享：
                                <%#Eval("ShareRebate")%></span></li>
                            <li><span id="span_AirRebate">航空：
                                <%#Eval("AirRebate")%></span></li>
                        </ul>
                        <ul class="ulClass hide" id='ul_hide_<%#Eval("Id") %>'>
                            <li>下级：<input type="text" id="txtDownRebate_<%#Eval("Id") %>" value='<%#Eval("DownRebate")%>'
                                size="4" /></li>
                            <li>后返：<input type="text" id='txtLaterRebate_<%#Eval("Id") %>' value='<%#Eval("LaterRebate")%>'
                                size="4" /></li>
                            <li>共享：<input type="text" id='txtShareRebate_<%#Eval("Id") %>' value='<%#Eval("ShareRebate")%>'
                                size="4" /></li>
                            <li>航空：<input type="text" id='txtAirRebate_<%#Eval("Id") %>' value='<%#Eval("AirRebate")%>'
                                size="4" /></li>
                        </ul>
                    </td>
                    <td>
                        <%#Eval("OperLoginName")%>
                    </td>
                    <td>
                        <%#Eval("OperLoginName")%>
                    </td>
                    <td>
                        <span id='show_Flight_<%#Eval("Id") %>'>
                            <%# DataBinder.Eval(Container.DataItem, "FlightStartDate", "{0:yyyy-MM-dd HH:mm}")%><br />
                            <%# DataBinder.Eval(Container.DataItem, "FlightEndDate", "{0:yyyy-MM-dd HH:mm}")%>
                        </span><span id='hide_Flight_<%#Eval("Id") %>' class="hide">
                            <input type="text" id='txtFlightStartDate_<%#Eval("Id")%>' style="width: 80px;" readonly="true"
                                value='<%# DataBinder.Eval(Container.DataItem,"FlightStartDate","{0:yyyy-MM-dd HH:mm}")%>'
                                class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'<%= System.DateTime.Now.ToString("yyyy-MM-dd HH:mm") %>',autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" />
                            <br />
                            <input type="text" id='txtFlightEndDate_<%#Eval("Id")%>' style="width: 80px;" readonly="true"
                                value='<%# DataBinder.Eval(Container.DataItem, "FlightEndDate", "{0:yyyy-MM-dd HH:mm}")%>'
                                class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'<%= System.DateTime.Now.ToString("yyyy-MM-dd HH:mm") %>',autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" />
                        </span>
                    </td>
                    <td>
                        <span id="show_Print_<%#Eval("Id") %>">
                            <%# DataBinder.Eval(Container.DataItem, "PrintStartDate", "{0:yyyy-MM-dd}")%><br />
                            <%# DataBinder.Eval(Container.DataItem, "PrintEndDate", "{0:yyyy-MM-dd}")%>
                        </span><span id="hide_Print_<%#Eval("Id") %>" class="hide">
                            <input type="text" id='txtPrintStartDate<%#Eval("Id") %>' style="width: 80px;" readonly="true"
                                value='<%# DataBinder.Eval(Container.DataItem,"PrintStartDate","{0:yyyy-MM-dd}")%>'
                                class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'<%= System.DateTime.Now.ToString("yyyy-MM-dd") %>',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                            <br />
                            <input type="text" id='txtPrintEndDate<%#Eval("Id") %>' style="width: 80px;" readonly="true"
                                value='<%# DataBinder.Eval(Container.DataItem, "PrintEndDate", "{0:yyyy-MM-dd}")%>'
                                class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'<%= System.DateTime.Now.ToString("yyyy-MM-dd") %>',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                        </span>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    <input type="hidden" id="hid_PageSize" value="20" runat="server" />
    <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
        CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
        PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
        AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
        EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
        ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
        SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
    </webdiyer:AspNetPager>
    <%--公司编号--%>
    <input type="hidden" id="Hid_CpyNo" runat="server" />
    <%--供应商名字--%>
    <input type="hidden" id="Hid_UserName" runat="server" />
    <%--公司名称--%>
    <input type="hidden" id="Hid_CpyName" runat="server" />
    <%--登录账号 --%>
    <input type="hidden" id="Hid_LoginName" runat="server" />
    <%--当前选择的ID--%>
    <input type="hidden" id="Hid_CurrId" runat="server" />
    <%--页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策--%>
    <input type="hidden" id="Hid_PageType" runat="server" value="1" />
    <asp:Literal ID="scriptText" runat="server"></asp:Literal>
    </form>
</body>
</html>
