<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PolicyList.aspx.cs" Inherits="Policy_PolicyList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="tleName" runat="server">政策管理</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <script id="script_k0" src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
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
        .Search th, td
        {
            padding: 3px;
        }
        .input_50
        {
            width: 50px;
        }
        .input_70
        {
            width: 70px;
        }
    </style>
    <script type="text/javascript">
        //为Jquery重新命名
        var jQueryOne=jQuery.noConflict(false);
        //对话框
        function showdialog(t,f) {
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
                        //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策
                        var PageType=jQueryOne("#Hid_PageType").val();
                        if(f=="0") {
                            //跳到登录页面                           
                            top.location.href="../Login.aspx";
                        }
                        jQueryOne(this).dialog('close');
                        if(f=="1") {
                            //处理成功 查询按钮
                            jQueryOne("#btnQuery").click();
                        }
                    }
                }
            });
        }
        function showHTML(title,html,w,h) {
            jQueryOne("#divPause").html(html);
            jQueryOne("#divPause").dialog({
                title: title,
                bgiframe: true,
                height: h,
                width: w,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                }
            });//.css({ "width": "auto","height": "auto" });
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
        //清空空数据
        function EmptyArr(arr) {
            var newArr=[];
            for(var i=0;i<arr.length;i++) {
                if(jQueryOne.trim(arr[i])!="") {
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
        //根据行程类型显示隐藏
        function setCity(val) {
            if(val=="4"||val=="0") {
                jQueryOne("#td_middle0,#td_middle1").show();
                jQueryOne("#Hid_IsShowMiddle").val("1");
            } else {
                jQueryOne("#td_middle0,#td_middle1").hide();
                jQueryOne("#Hid_IsShowMiddle").val("0");
            }
        }

        //全选
        function SelectAll(obj) {
            jQueryOne("input[name*='ItemCk'][type='checkbox']").attr("checked",obj.checked);
        }
        //设置当前选择的行样式 由id控制哦
        function SetCurrSelStyle(id) {
            // jQueryOne("#tr_item"+id)[0].bgColor="green";
            //设置样式
        }
        function NumVate() {
            try {
                var value=jQueryOne.trim(jQueryOne(this).val());
                var ctrlId=jQueryOne(this).attr("id");
                //数据范围0-100
                var NumFanWei=jQueryOne(this).attr("FanWei");
                if(value==null||value=='') {
                    showdialog("文本框不能为空,请输入正确的数字!");
                    jQueryOne(this).val("0");
                    return false;
                }
                if(!isNaN(value)) {
                    var userreg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
                    if(userreg.test(value)) {
                        //验证通过
                        if(parseFloat(value).toString().length>5) {
                            jQueryOne(this).val("0");
                            showdialog("输入数据超出范围!");
                            return false;
                        }
                        var idArr=[
                        'txtDownPoint_',
                        'txtLaterPoint_',
                        'txtSharePoint_',
                        'txtAirReBate_'
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
                                    jQueryOne(this).val("0");
                                    showdialog("输入数据超出范围["+NumFanWei+"]!");
                                    return false;
                                }
                            }
                        }
                    } else {
                        var numindex=parseInt(value.indexOf("."),10);
                        if(numindex==0) {
                            jQueryOne(this).val("0");
                            showdialog("输入的数字不规范");
                            return false;
                        }
                        var head=value.substring(0,numindex);
                        var bottom=value.substring(numindex,numindex+3);
                        var fianlNum=head+bottom;
                        jQueryOne(this).val(fianlNum);
                    }
                } else {
                    jQueryOne(this).val("0");
                    showdialog("请输入数字");
                    return false;
                }
                var s='0';
                if(NumFanWei!=null) {
                    var x=parseFloat(value,10);
                    s=ShowPoint(x,1);//保留一位小数
                } else {
                    s=parseFloat(value,10);
                }
                jQueryOne(this).val(s);
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
        function showUpdate(id) {
            //局部修改
            jQueryOne("#divContainer_"+id).hide();
            jQueryOne("#divUpdateCon_"+id).show();

            jQueryOne("#show_CarryCode_"+id).hide();
            jQueryOne("#hide_CarryCode_"+id).show();

            jQueryOne("#show_StartCityNameCode_"+id).hide();
            jQueryOne("#hide_StartCityNameCode_"+id).show();

            jQueryOne("#show_MiddleCityNameCode_"+id).hide();
            jQueryOne("#hide_MiddleCityNameCode_"+id).show();

            jQueryOne("#show_TargetCityNameCode_"+id).hide();
            jQueryOne("#hide_TargetCityNameCode_"+id).show();

            jQueryOne("#show_ShippingSpace_"+id).hide();
            jQueryOne("#hide_ShippingSpace_"+id).show();

            jQueryOne("#ul_show_"+id).hide();
            jQueryOne("#ul_hide_"+id).show();

            jQueryOne("#show_Flight_"+id).hide();
            jQueryOne("#hide_Flight_"+id).show();

            jQueryOne("#show_Print_"+id).hide();
            jQueryOne("#hide_Print_"+id).show();

            return false;
        }

        function hideUpdate(id) {
            //局部修改
            jQueryOne("#divContainer_"+id).show();
            jQueryOne("#divUpdateCon_"+id).hide();

            jQueryOne("#show_CarryCode_"+id).show();
            jQueryOne("#hide_CarryCode_"+id).hide();

            jQueryOne("#show_StartCityNameCode_"+id).show();
            jQueryOne("#hide_StartCityNameCode_"+id).hide();

            jQueryOne("#show_MiddleCityNameCode_"+id).show();
            jQueryOne("#hide_MiddleCityNameCode_"+id).hide();


            jQueryOne("#show_TargetCityNameCode_"+id).show();
            jQueryOne("#hide_TargetCityNameCode_"+id).hide();


            jQueryOne("#show_ShippingSpace_"+id).show();
            jQueryOne("#hide_ShippingSpace_"+id).hide();

            jQueryOne("#ul_show_"+id).show();
            jQueryOne("#ul_hide_"+id).hide();

            jQueryOne("#show_Flight_"+id).show();
            jQueryOne("#hide_Flight_"+id).hide();

            jQueryOne("#show_Print_"+id).show();
            jQueryOne("#hide_Print_"+id).hide();

            return false;
        }
        function ajaxUpdate(id,opType) {
            //公司编号
            var val_CpyNo=jQueryOne("#Hid_CpyNo").val();
            //供应商名字
            var val_CpyName=jQueryOne("#Hid_CpyName").val();
            //登录账号
            var val_LoginName=jQueryOne("#Hid_LoginName").val();
            var PageType=jQueryOne("#Hid_PageType").val();
            //行程类型
            var travelType=jQueryOne("#hid_travelType_"+id).val();
            var val_CarryCode=jQueryOne("#txtCarryCode_"+id).val();
            //返点
            var val_DownPoint=jQueryOne("#txtDownPoint_"+id).val();
            var val_LaterPoint=jQueryOne("#txtLaterPoint_"+id).val();
            var val_SharePoint=jQueryOne("#txtSharePoint_"+id).val();
            //现返
            var val_DownReturnMoney=jQueryOne("#txtDownReturnMoney_"+id).val();
            var val_LaterReturnMoney=jQueryOne("#txtLaterReturnMoney_"+id).val();
            var val_ShareReturnMoney=jQueryOne("#txtShareReturnMoney_"+id).val();

            //航空公司返点 现返
            var val_AirReBate=jQueryOne("#hid_AirReBate_"+id).val();
            var val_AirReBateReturnMoney=jQueryOne("#hid_AirReBateReturnMoney_"+id).val();

            var carrcode=EmptyArr(val_CarryCode.split('/'));
            if(carrcode.length==0) {
                showdialog('请输入承运人二字码！');
                return false;
            }
            var val_FromCityCode="//";
            var val_MiddleCityCode="//";
            var val_ToCityCode="//";
            var val_ShippingSpace="//";
            if(PageType!="3") {
                val_FromCityCode=jQueryOne("#txtStartCityNameCode_"+id).val();
                val_MiddleCityCode=jQueryOne("#txtMiddleCityNameCode_"+id).val();
                val_ToCityCode=jQueryOne("#txtTargetCityNameCode_"+id).val();
                val_ShippingSpace=jQueryOne("#txtShippingSpace_"+id).val();
                if(EmptyArr(val_FromCityCode.split('/')).length==0) {
                    showdialog('请输入出发城市三字码！');
                    return false;
                }
                if(travelType=="4"&&EmptyArr(val_MiddleCityCode.split('/')).length==0) {
                    showdialog('请输入中转城市三字码！');
                    return false;
                }

                if(EmptyArr(val_ToCityCode.split('/')).length==0) {
                    showdialog('请输入到达城市三字码！');
                    return false;
                }
                if(EmptyArr(val_ShippingSpace.split('/')).length==0) {
                    showdialog('请输入航空公司舱位！');
                    return false;
                }
                val_AirReBate=jQueryOne("#txtAirReBate_"+id).val();
                val_AirReBateReturnMoney=jQueryOne("#txtAirReBateReturnMoney_"+id).val();
                if(jQueryOne.isNaN(val_AirReBate)) {
                    showdialog('输入航空公司返点格式错误,必须为数字！');
                    return false;
                }
                if(jQueryOne.isNaN(val_AirReBateReturnMoney)) {
                    showdialog('输入航空公司现返金额格式错误,必须为数字！');
                    return false;
                }
            } else {
                if(carrcode.length>0&&carrcode[0]!="ALL"&&carrcode.length>=2&&carrcode[0].length>2) {
                    showdialog('默认政策最多输入一个航空公司二字码！');
                    return false;
                }
            }

            if(jQueryOne.isNaN(val_DownPoint)) {
                showdialog('输入下级分销返点格式错误,必须为数字！');
                return false;
            }
            if(jQueryOne.isNaN(val_LaterPoint)) {
                showdialog('输入下级分销后返格式错误,必须为数字！');
                return false;
            }
            if(jQueryOne.isNaN(val_SharePoint)) {
                showdialog('输入共享政策返点格式错误,必须为数字！');
                return false;
            }

            if(jQueryOne.isNaN(val_DownReturnMoney)) {
                showdialog('输入下级分销现返金额格式错误,必须为数字！');
                return false;
            }
            if(jQueryOne.isNaN(val_LaterReturnMoney)) {
                showdialog('输入下级分销后返现返金额格式错误,必须为数字！');
                return false;
            }
            if(jQueryOne.isNaN(val_ShareReturnMoney)) {
                showdialog('输入共享政策现返金额格式错误,必须为数字！');
                return false;
            }

            var val_FlightStartDate=jQueryOne("#txtFlightStartDate_"+id).val();
            var val_FlightEndDate=jQueryOne("#txtFlightEndDate_"+id).val();

            var val_PrintStartDate=jQueryOne("#txtPrintStartDate"+id).val();
            var val_PrintEndDate=jQueryOne("#txtPrintEndDate"+id).val();

            if(jQueryOne.trim(val_FlightStartDate)=="") {
                showdialog('乘机日期开始时间不能为空！');
                return false;
            }
            if(jQueryOne.trim(val_FlightEndDate)=="") {
                showdialog('乘机日期结束时间不能为空！');
                return false;
            }
            if(jQueryOne.trim(val_PrintStartDate)=="") {
                showdialog('出票日期开始时间不能为空！');
                return false;
            }
            if(jQueryOne.trim(val_PrintEndDate)=="") {
                showdialog('出票日期结束时间不能为空！');
                return false;
            }

            //乘机日期
            var d1=CompareDate(val_FlightStartDate,val_FlightEndDate);
            if(d1) {
                showdialog('乘机开始有效日期不能大于乘机结束有效日期！');
                return false;
            }
            //出票日期
            var d1=CompareDate(val_PrintStartDate,val_PrintEndDate);
            if(d1) {
                showdialog('出票开始有效日期不能大于出票结束有效日期！');
                return false;
            }

            //验证
            jQueryOne("#a_update_"+id).attr("disabled",true);
            jQueryOne("#a_copy_"+id).attr("disabled",true);
            jQueryOne("#a_del_"+id).attr("disabled",true);
            jQueryOne.post("../AJAX/CommonAjAx.ashx",
            {
                //参数区域
                CpyNo: escape(val_CpyNo),
                CpyName: escape(val_CpyName),
                LoginName: escape(val_LoginName),

                FromCityCode: escape(val_FromCityCode),
                MiddleCityCode: escape(val_MiddleCityCode),
                ToCityCode: escape(val_ToCityCode),
                CarryCode: escape(val_CarryCode),
                ShippingSpace: escape(val_ShippingSpace),
                //SpacePrice: escape(val_SpacePrice),
                //ReferencePrice: escape(val_ReferencePrice),
                DownPoint: escape(val_DownPoint),
                LaterPoint: escape(val_LaterPoint),
                SharePoint: escape(val_SharePoint),
                AirReBate: escape(val_AirReBate),

                DownReturnMoney: escape(val_DownReturnMoney),
                LaterReturnMoney: escape(val_LaterReturnMoney),
                ShareReturnMoney: escape(val_ShareReturnMoney),
                AirReturnMoney: escape(val_AirReBateReturnMoney),


                FlightStartDate: escape(val_FlightStartDate),
                FlightEndDate: escape(val_FlightEndDate),
                PrintStartDate: escape(val_PrintStartDate),
                PrintEndDate: escape(val_PrintEndDate),

                OpFunction: escape("OpPolicy"),
                OpType: escape(opType),
                //Copy: escape(IsCopy),
                OpPage: escape("PolicyList.aspx"),
                Id: escape(id),
                num: Math.random(),
                currentuserid: jQueryOne("#currentuserid").val()
            },
             function (data) {
                 jQueryOne("#a_update_"+id).attr("disabled",false);
                 jQueryOne("#a_copy_"+id).attr("disabled",false);
                 jQueryOne("#a_del_"+id).attr("disabled",false);
                 var strReArr=data.split('##');
                 if(strReArr.length==3) {
                     //错误代码
                     var errCode=strReArr[0];
                     //错误描述
                     var errDes=strReArr[1];
                     //错误结果
                     var result=jQueryOne.trim(unescape(strReArr[2]));
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
        function DataValidate(obj,optype,id) {
            var ArrIds=[];
            var IDS='';
            if(id==undefined||id==null) {
                jQueryOne("input[name*='ItemCk'][type='checkbox']:checked").each(function (i,val) {
                    ArrIds.push("'"+jQueryOne(val).val()+"'");
                });
                if(ArrIds.length==0) {
                    showdialog("请选择复选框！");
                    return false;
                }
                IDS=ArrIds.join(",");
            } else {
                IDS=id;
            }
            var audittype="";
            if(optype=="2") {
                audittype="";
            }
            else if(optype=="5") {
                audittype="1";
            }
            else if(optype=="6") {
                audittype="2";
            }
            jQueryOne(obj).attr("disabled",true);
            jQueryOne.post("../AJAX/CommonAjAx.ashx",
                {
                    OpFunction: escape("OpPolicy"),
                    OpType: escape(optype),
                    AuditType: audittype,
                    Id: escape(IDS),
                    num: Math.random(),
                    currentuserid: jQueryOne("#currentuserid").val()
                },
                 function (data) {
                     jQueryOne(obj).attr("disabled",false);
                     var strReArr=data.split('##');
                     if(strReArr.length==3) {
                         //错误代码
                         var errCode=strReArr[0];
                         //错误描述
                         var errDes=strReArr[1];
                         //错误结果
                         var result=jQueryOne.trim(unescape(strReArr[2]));
                         if(errCode=="1") {
                             if(optype=="2") {
                                 showdialog("删除成功！","1");
                             } else if(optype=="5") {
                                 showdialog("政策已审核成功！","1");
                             }
                             else if(optype=="6") {
                                 showdialog("政策未审核成功！","1");
                             }
                         } else {
                             if(optype=="2") {
                                 showdialog("删除失败！");
                             } else if(optype=="5") {
                                 showdialog("政策已审核失败！");
                             }
                             else if(optype=="6") {
                                 showdialog("政策未审核失败！");
                             }
                         }
                     }
                 },"text");

            return false;
        }

        function SetTabStyle() {

            var PageType=jQueryOne("#Hid_PageType").val();
            jQueryOne("#secTable tr td[id*='tdPolicy_']").each(function (index,td) {
                jQueryOne(td).removeClass("sec2 sec1");
                if(jQueryOne(td).attr("id")=="tdPolicy_"+PageType) {
                    jQueryOne(td).addClass("sec2");
                } else {
                    jQueryOne(td).addClass("sec1");
                }
            });
        }

        //------------------------------------------------------------------
        //显示政策挂起解挂批量操作条件
        function ShowPause() {
            var html='<iframe frameborder="no" border="0" marginwidth="0" marginheight="0" scrolling="no" allowtransparency="yes" width="100%" style="margin:0px;padding:0px;" height="100%" src="PolicyFilter.aspx?currentuserid=<%=this.mUser.id.ToString() %>"/>';
            showHTML("政策挂起解挂-筛选条件",html,1000,300);
            return false;
        }
        //关闭
        function closeHTML() {
            jQueryOne("#divPause").dialog('close');
        }
        //修改
        function PatchUpdatePause(param) {
            try {
                var url="../AJAX/CommonAjAx.ashx?currentuserid="+jQueryOne("#currentuserid").val();
                var val_CpyNo=jQueryOne.trim(jQueryOne("#Hid_CpyNo").val());
                var val_UserName=jQueryOne.trim(jQueryOne("#Hid_CpyName").val());
                var val_LoginName=jQueryOne.trim(jQueryOne("#Hid_LoginName").val());
                var val_UserRoleType=jQueryOne.trim(jQueryOne("#Hid_RoleType").val());

                param.CpyNo=escape(val_CpyNo);
                param.LoginName=escape(val_LoginName);
                param.UserName=escape(val_UserName);
                param.RoleType=escape(val_UserRoleType);
                param.OpType=escape("2");
                param.OpPage=escape("PolicyList.aspx?currentuserid=<%=this.mUser.id.ToString() %>");
                param.OpFunction=escape("Suppend");
                param.IsWhere=escape("1");//根据条件挂起解挂
                //禁用操作按钮
                //jQueryOne(param.OpBtnObj).attr("disabled",true);

                jQueryOne.post(url,param,function (data) {
                    // jQueryOne(param.OpBtnObj).attr("disabled",false);
                    var strReArr=data.split('##');
                    if(strReArr.length==3) {
                        //错误代码
                        var errCode=strReArr[0];
                        //错误描述
                        var errDes=strReArr[1];
                        //错误结果
                        var result=jQueryOne.trim(unescape(strReArr[2]));
                        if(errCode=="1") {
                            showdialog(errDes,1);
                        } else {
                            showdialog(errDes);
                        }
                    } else {
                        showdialog("页面出错了，请重新登录！");
                    }
                },"text");
            } catch(e) {
                //alert(e.message);
            }
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
    <div id="dgShow">
    </div>
    <div id="divPause">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <%--政策类型选项卡开始--%>
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
    <%--政策类型选项卡结束--%>
    <div class="title" style="width: 99%">
        <ul style="list-style-type: none; margin: 0 0; padding: 0 0; position: relative;
            width: 99%">
            <li>
                <asp:Label ID="lblShow" Text="政策管理" runat="server" />
            </li>
        </ul>
    </div>
    <div id="div_tab1" runat="server">
        <div class="container">
            <table class="Search" cellspacing="0" cellpadding="0" border="0">
                <tr class='<%=PageType=="3"?"hide":"show" %>'>
                    <th id="dd1">
                        出发城市：
                    </th>
                    <td colspan="2">
                        <%-- 出发城市--%>
                        <input name="txtFromCityName" class="inputtxtdat" runat="server" type="text" id="txtFromCityName"
                            mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                            mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                            style="width: 150px;" mod_address_reference="FromCityCode" />
                    </td>
                    <th id="td_middle0">
                        中转城市：
                    </th>
                    <td colspan="2" id="td_middle1">
                        <%-- 出发城市--%>
                        <input name="txtMiddleCityName" class="inputtxtdat" runat="server" type="text" id="txtMiddleCityName"
                            mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                            mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                            style="width: 230px;" mod_address_reference="MiddleCityCode" />
                    </td>
                    <th id="Th1">
                        到达城市：
                    </th>
                    <td colspan="2">
                        <%-- 到达城市--%>
                        <input name="txtToCityName" class="inputtxtdat" runat="server" type="text" id="txtToCityName"
                            mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                            mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                            style="width: 230px;" mod_address_reference="ToCityCode" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="show">
                    <th>
                        承运人：
                    </th>
                    <td colspan="2">
                        <uc1:SelectAirCode ID="ddlCarry" IsDShowName="false" runat="server" DefaultOptionValue="" />
                    </td>
                    <th>
                        乘机日期：
                    </th>
                    <td colspan="2">
                        <input type="text" readonly="readonly" id="txtFlightStartDate" runat="server" onfocus="WdatePicker({isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                            style="width: 110px;" class="Wdate inputtxtdat" />-<input type="text" readonly="readonly"
                                id="txtFlightEndDate" runat="server" onfocus="WdatePicker({isShowWeek:false,autoPickDate:true,dateFmt:'yyyy-MM-dd'})"
                                style="width: 110px;" class="Wdate inputtxtdat" />
                    </td>
                    <th>
                        出票日期：
                    </th>
                    <td colspan="2">
                        <input type="text" readonly="readonly" id="txtTicketStartDate" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                            style="width: 110px;" class="Wdate inputtxtdat" />-<input type="text" readonly="readonly"
                                id="txtTicketEndDate" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                                style="width: 110px;" class="Wdate inputtxtdat" />
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="show">
                    <th>
                        政策类型：
                    </th>
                    <td colspan="2">
                        <asp:DropDownList runat="server" ID="ddlPolicyType" Style="width: 150px;">
                            <asp:ListItem Value="-1" Text="全部" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="B2B"></asp:ListItem>
                            <asp:ListItem Value="2" Text="BSP"></asp:ListItem>
                            <%--<asp:ListItem Value="3" Text="B2B/BSP"></asp:ListItem>--%>
                        </asp:DropDownList>
                    </td>
                    <th id="Th2">
                        发布者：
                    </th>
                    <td>
                        <uc1:SelectAirCode ID="SelPublic" DefaultOptionText="--发布者--" DefaultOptionValue=""
                            IsDShowName="false" runat="server" InputMaxLength="50" />
                    </td>
                    <th id="Td1" colspan="2" align="right">
                        <span class='<%=PageType!="3"?"show":"hide" %>'>审核状态：</span>
                    </th>
                    <td style="width: 150px;">
                        <span class='<%=PageType!="3"?"show":"hide" %>'>
                            <asp:DropDownList runat="server" ID="rdAuditState" Style="width: 150px;">
                                <asp:ListItem Value="0" Text="全部" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="1" Text="审核"></asp:ListItem>
                                <asp:ListItem Value="2" Text="未审核"></asp:ListItem>
                            </asp:DropDownList>
                        </span>
                    </td>
                    <th class='<%=PageType=="3"?"show":"hide" %>'>
                        乘客类型：
                    </th>
                    <td class='<%=PageType=="3"?"show":"hide" %>'>
                        <asp:DropDownList runat="server" ID="ddlPasType" Width="114px">
                            <asp:ListItem Value="0" Text="全部" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="成人"></asp:ListItem>
                            <asp:ListItem Value="2" Text="儿童"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr class='<%=PageType=="3"?"hide":"show" %>'>
                    <th>
                        行程类型：
                    </th>
                    <td colspan="2">
                        <asp:DropDownList runat="server" ID="rblTravelType" onchange="setCity(this.value)"
                            Style="width: 150px;">
                            <asp:ListItem Value="0" Text="全部" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="单程"></asp:ListItem>
                            <asp:ListItem Value="2" Text="单程/往返"></asp:ListItem>
                            <asp:ListItem Value="3" Text="往返"></asp:ListItem>
                            <asp:ListItem Value="4" Text="联程"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <th>
                        出票方式：
                    </th>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlETDZType" onchange="setCity(this.value)"
                            Style="width: 155px;">
                            <asp:ListItem Value="" Text="全部" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="0" Text="手动"></asp:ListItem>
                            <asp:ListItem Value="1" Text="半自动"></asp:ListItem>
                            <asp:ListItem Value="2" Text="自动"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <th>
                    </th>
                    <td align="left">
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                    </td>
                    <td>
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
                            </span>
                            <% if (PageType != "3")
                               {%>
                            <span class="btn btn-ok-s" id="span_del" runat="server">
                                <asp:Button ID="btnDelete" runat="server" Text="批量删除" OnClientClick="return DataValidate(this,2);" />
                            </span><span class="btn btn-ok-s" runat="server" id="spanpatch">
                                <asp:Button ID="btnReview" runat="server" Text="批量审核" OnClientClick="return DataValidate(this,'5');" />
                            </span><span class="btn btn-ok-s" runat="server" id="spanpatch0">
                                <asp:Button ID="btnNotReview" runat="server" Text="批量未审核" OnClientClick="return DataValidate(this,'6');" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnExpiresQuery" runat="server" Text="过期政策查询" OnClick="btnExpiresQuery_Click" />
                            </span><span class="btn btn-ok-s">
                                <asp:Button ID="btnPatchPause" runat="server" Text="批量政策挂起解挂" OnClientClick="return ShowPause();" />
                            </span>
                            <%} %>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <table id="tb-all-trade" class="tb-all-trade" border="1" cellspacing="0" cellpadding="0"
        width="100%">
        <tr>
            <% if (PageType != "3")
               {%>
            <th>
                <input type="checkbox" id="headerCk" name="headerCk" onclick="SelectAll(this)" />
            </th>
            <%} %>
            <th>
                操作
            </th>
            <th>
                承运人
            </th>
            <% if (PageType != "3")
               {%>
            <th>
                出发城市
            </th>
            <th class='<%=IsShowMiddle?"show":"hide" %>'>
                中转城市
            </th>
            <th>
                到达城市
            </th>
            <th>
                舱位
            </th>
            <th>
                适用航班号
            </th>
            <th>
                排除航班号
            </th>
            <th>
                政策状态
            </th>
            <th>
                审核状态
            </th>
            <%} %>
            <th>
                班期限制
            </th>
            <% if (PageType == "3")
               { %>
            <th>
                乘客类型
            </th>
            <th>
                政策类型
            </th>
            <%}
               else
               {%>
            <th>
                类型
            </th>
            <%} %>
            <th>
                政策
            </th>
            <th>
                乘机有效期
            </th>
            <th>
                出票有效期
            </th>
            <th>
                发布者
            </th>
            <th>
                操作者
            </th>
        </tr>
        <asp:Repeater ID="Repeater" runat="server" OnPreRender="Repeater_PreRender" OnItemCommand="Repeater_ItemCommand">
            <ItemTemplate>
                <tr id="tr_item<%#Eval("Id") %>" onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                    <% if (PageType != "3")
                       {%>
                    <td>
                        <input type="checkbox" id="ItemCk" name="ItemCk" value='<%#Eval("Id") %>' runat="server" />
                        <input type="hidden" id='hid_travelType_<%#Eval("Id") %>' value='<%#Eval("TravelType") %>' />
                    </td>
                    <%} %>
                    <td>
                        <span id='span_edit_<%#Eval("Id") %>'><a href="<%# GetUrl(Eval("Id").ToString(),Eval("CpyNo").ToString(),Eval("CarryCode").ToString())%>">
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
                        <% if (PageType != "3")
                           {%>
                        <span id="span_copy_<%#Eval("Id") %>"><a id='a_copy_<%#Eval("Id") %>' href="#" onclick="return ajaxUpdate('<%#Eval("Id") %>','4')">
                            复制</a><br />
                        </span><span id="span_del_<%#Eval("Id") %>"><a id='a_del_<%#Eval("Id") %>' href="#"
                            onclick="return ajaxUpdate('<%#Eval("Id") %>','2')">删除</a><br />
                        </span><span id='span_aduit_<%#Eval("Id") %>' class='<%# Eval("AuditType").ToString()=="1"?"hide":"show" %>'>
                            <a href="#" onclick="return DataValidate(this,'5','<%#Eval("Id") %>');">审核</a><br />
                        </span><span id='span_canceladuit_<%#Eval("Id") %>' class='<%# Eval("AuditType").ToString()=="1"?"show":"hide" %>'>
                            <a href="#" onclick="return DataValidate(this,'6','<%#Eval("Id") %>');">取消审核</a><br />
                        </span>
                        <%} %>
                    </td>
                    <td>
                        <span id="show_CarryCode_<%#Eval("Id") %>">
                            <%#Eval("CarryCode")%></span> <span id="hide_CarryCode_<%#Eval("Id") %>" class="hide">
                                <input type="text" id="txtCarryCode_<%#Eval("Id") %>" value='<%#Eval("CarryCode")%>'
                                    class="input_50" />
                            </span>
                    </td>
                    <% if (PageType != "3")
                       {%>
                    <td title='<%# Eval("StartCityNameCode") %>'>
                        <span id="show_StartCityNameCode_<%#Eval("Id") %>">
                            <%# SubChar( Eval("StartCityNameCode"),8,"...")%></span> <span id="hide_StartCityNameCode_<%#Eval("Id") %>"
                                class="hide">
                                <input type="text" id="txtStartCityNameCode_<%#Eval("Id") %>" value='<%#Eval("StartCityNameCode")%>'
                                    class="input_50" />
                            </span>
                    </td>
                    <td title='<%# Eval("MiddleCityNameCode") %>' class='<%=IsShowMiddle?"show":"hide" %>'>
                        <span id="show_MiddleCityNameCode_<%#Eval("Id") %>">
                            <%# SubChar(Eval("MiddleCityNameCode"),8,"...")%></span> <span id="hide_MiddleCityNameCode_<%#Eval("Id") %>"
                                class="hide">
                                <input type="text" id="txtMiddleCityNameCode_<%#Eval("Id") %>" value='<%#Eval("MiddleCityNameCode")%>'
                                    class="input_50" />
                            </span>
                    </td>
                    <td title='<%# Eval("TargetCityNameCode") %>'>
                        <span id="show_TargetCityNameCode_<%#Eval("Id") %>">
                            <%# SubChar(Eval("TargetCityNameCode"),8,"...")%></span> <span id="hide_TargetCityNameCode_<%#Eval("Id") %>"
                                class="hide">
                                <input type="text" id="txtTargetCityNameCode_<%#Eval("Id") %>" value='<%#Eval("TargetCityNameCode")%>'
                                    class="input_50" />
                            </span>
                    </td>
                    <td title='<%# Eval("ShippingSpace") %>'>
                        <span id="show_ShippingSpace_<%#Eval("Id") %>">
                            <%# SubChar(Eval("ShippingSpace"),8,"...")%>
                        </span><span id="hide_ShippingSpace_<%#Eval("Id") %>" class="hide">
                            <input type="text" id="txtShippingSpace_<%#Eval("Id") %>" value='<%#Eval("ShippingSpace")%>'
                                class="input_70" />
                        </span>
                    </td>
                    <td>
                        <span id="show_ApplianceFlight_<%#Eval("Id") %>">
                            <%#SubChar( ShowItem(0, 1, Eval("ApplianceFlightType"), Eval("ApplianceFlight"), Eval("UnApplianceFlight")),15,"...")%></span>
                        <%--<span id="hide_ApplianceFlight_<%#Eval("Id") %>" class="hide">
                            <input type="text" id="txtApplianceFlight_<%#Eval("Id") %>" value='<%#ShowItem(0, 1, Eval("ApplianceFlightType"), Eval("ApplianceFlight"), Eval("UnApplianceFlight"))%>' />
                      </span>   --%>
                    </td>
                    <td>
                        <%# SubChar(ShowItem(0, 2, Eval("ApplianceFlightType"), Eval("ApplianceFlight"), Eval("UnApplianceFlight")),15,"...")%>
                    </td>
                    <td>
                        <%#  ShowItem(8, Eval("IsPause"))%>
                    </td>
                    <td>
                        <%#  ShowItem(4,Eval("AuditType"))%>
                    </td>
                    <%} %>
                    <td>
                        <%# Eval("ScheduleConstraints")%>
                    </td>
                    <% if (PageType == "3")
                       { %>
                    <td>
                        <%# Eval("A1").ToString()=="1"?"成人":"儿童" %>
                    </td>
                    <td>
                        <%# ShowItem(3,Eval("PolicyType"))%>
                    </td>
                    <%
                        }
                       else
                       {
                    %>
                    <td>
                        <ul class="ulClass" id="ul_type">
                            <li>
                                <%# ShowItem(1, Eval("ReleaseType"))%><br />
                                <%# ShowItem(2, Eval("TravelType"))%><br />
                                <%# ShowItem(3,Eval("PolicyType"))%><br />
                                <% if (IsOpenGF)
                                   { %>
                                <%# ShowItem(9, Eval("HighPolicyFlag"))%><br />
                                <%} %>
                                <% if (PageType != "2")
                                   {%>
                                <%# ShowItem(5, Eval("TeamFlag"))%><br />
                                <%}
                                   else
                                   { %>
                                <%# ShowItem(7, Eval("GenerationType"))%><br />
                                <%} %>
                                <%# ShowItem(6, Eval("AutoPrintFlag"))%></li>
                        </ul>
                    </td>
                    <%} %>
                    <td>
                        <ul class="ulClass" id='ul_show_<%#Eval("Id") %>'>
                            <li><span id="span_DownPoint">下级：
                                <%#Eval("DownPoint")%>/<%#Eval("DownReturnMoney")%></span></li>
                            <li><span id="span_LaterPoint">后返：
                                <%#Eval("LaterPoint")%>/<%#Eval("LaterReturnMoney")%></span></li>
                            <li><span id="span_SharePoint">共享：
                                <%#Eval("SharePoint")%>/<%#Eval("SharePointReturnMoney")%></span></li>
                            <% if (PageType != "3")
                               { %>
                            <li><span id="span_AirReBate">航空：<%#Eval("AirReBate")%></span>/<span id="span_AirReturnMonry"><%#Eval("AirReBateReturnMoney")%></span></li>
                            <%} %>
                        </ul>
                        <ul class="ulClass hide" id='ul_hide_<%#Eval("Id") %>'>
                            <li>下级：<input type="text" id="txtDownPoint_<%#Eval("Id") %>" value='<%#Eval("DownPoint")%>'
                                size="4" fanwei="0-100" />/<input type="text" id='txtDownReturnMoney_<%#Eval("Id") %>'
                                    value='<%#Eval("DownReturnMoney")%>' size="4" /></li>
                            <li>后返：<input type="text" id='txtLaterPoint_<%#Eval("Id") %>' value='<%#Eval("LaterPoint")%>'
                                size="4" fanwei="0-100" />/<input type="text" id='txtLaterReturnMoney_<%#Eval("Id") %>'
                                    value='<%#Eval("LaterReturnMoney")%>' size="4" /></li>
                            <li>共享：<input type="text" id='txtSharePoint_<%#Eval("Id") %>' value='<%#Eval("SharePoint")%>'
                                size="4" fanwei="0-100" />/<input type="text" id='txtShareReturnMoney_<%#Eval("Id") %>'
                                    value='<%#Eval("SharePointReturnMoney")%>' size="4" /></li>
                            <input type="hidden" id='hid_AirReBate_<%#Eval("Id") %>' value='<%#Eval("AirReBate")%>' />
                            <input type="hidden" id='hid_AirReBateReturnMoney_<%#Eval("Id") %>' value='<%#Eval("AirReBateReturnMoney")%>' />
                            <% if (PageType != "3")
                               { %>
                            <li>航空：<input type="text" id='txtAirReBate_<%#Eval("Id") %>' value='<%#Eval("AirReBate")%>'
                                size="4" fanwei="0-100" />/<input type="text" id='txtAirReBateReturnMoney_<%#Eval("Id") %>'
                                    value='<%#Eval("AirReBateReturnMoney")%>' size="4" /></li>
                            <%} %>
                        </ul>
                    </td>
                    <td>
                        <span id='show_Flight_<%#Eval("Id") %>'>
                            <%# DataBinder.Eval(Container.DataItem, "FlightStartDate", "{0:yyyy-MM-dd}")%><br />
                            <%# DataBinder.Eval(Container.DataItem, "FlightEndDate", "{0:yyyy-MM-dd}")%>
                        </span><span id='hide_Flight_<%#Eval("Id") %>' class="hide">
                            <input type="text" id='txtFlightStartDate_<%#Eval("Id")%>' style="width: 80px;" readonly="true"
                                value='<%# DataBinder.Eval(Container.DataItem,"FlightStartDate","{0:yyyy-MM-dd}")%>'
                                class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'<%= System.DateTime.Now.ToString("yyyy-MM-dd") %>',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                            <br />
                            <input type="text" id='txtFlightEndDate_<%#Eval("Id")%>' style="width: 80px;" readonly="true"
                                value='<%# DataBinder.Eval(Container.DataItem, "FlightEndDate", "{0:yyyy-MM-dd}")%>'
                                class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'<%= System.DateTime.Now.ToString("yyyy-MM-dd") %>',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
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
                    <td>
                        <%#Eval("CreateLoginName")%>
                    </td>
                    <td>
                        <%#Eval("UpdateLoginName")%>
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
    <script type="text/javascript" src="../js/Tooltip/ToolTip.js"> </script>
    <script type="text/javascript">
        <!--
        initToolTip("td");
        //重新加载城市控件
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../JS/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        //初始化城市控件
        jQueryOne(function () {
            //选项卡样式
            SetTabStyle();
            var PageType=jQueryOne("#Hid_PageType").val();
            if(PageType!="3") {
                //加载控件
                ReLoad();
            }
            //验证
            jQueryOne("input[type='text'][id*='txtDownPoint_']").blur(NumVate);
            jQueryOne("input[type='text'][id*='txtLaterPoint_']").blur(NumVate);
            jQueryOne("input[type='text'][id*='txtSharePoint_']").blur(NumVate);
            jQueryOne("input[type='text'][id*='txtAirReBate_']").blur(NumVate);

            //金额
            jQueryOne("input[type='text'][id*='txtDownReturnMoney_']").blur(NumVate);
            jQueryOne("input[type='text'][id*='txtLaterReturnMoney_']").blur(NumVate);
            jQueryOne("input[type='text'][id*='txtShareReturnMoney_']").blur(NumVate);
            jQueryOne("input[type='text'][id*='txtAirReBateReturnMoney_']").blur(NumVate);

        });  
        //-->
    </script>
    <%--隐藏域--%>
    <input type="hidden" id="FromCityCode" runat="server" />
    <input type="hidden" id="MiddleCityCode" runat="server" />
    <input type="hidden" id="ToCityCode" runat="server" />
    <input type="hidden" id="Hid_IsShowMiddle" runat="server" value="1" />
    <%--公司编号--%>
    <input type="hidden" id="Hid_CpyNo" runat="server" />
    <%--用户角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" />
    <%--供应商名字--%>
    <input type="hidden" id="Hid_CpyName" runat="server" />
    <%--登录账号 --%>
    <input type="hidden" id="Hid_LoginName" runat="server" />
    <%--是否开启高返权限 1是 0否--%>
    <input type="hidden" id="Hid_IsOpenGF" runat="server" value="0" />
    <%--当前选择的ID--%>
    <input type="hidden" id="Hid_CurrId" runat="server" />
    <%--页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策--%>
    <input type="hidden" id="Hid_PageType" runat="server" value="1" />
    <asp:Literal ID="scriptText" runat="server"></asp:Literal>
    </form>
</body>
</html>
