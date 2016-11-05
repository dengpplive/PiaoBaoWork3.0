<%@ Page Language="C#" AutoEventWireup="True" EnableEventValidation="false" CodeFile="TGQApplication.aspx.cs"
    Inherits="Order_TGQApplication" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>退改签申请</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <style type="text/css">
        .table_info th, .table_info td
        {
            border-color: #CCCCCC;
            border-style: solid;
            border-width: 1px 1px 1px 1px;
            color: #606060;
        }
        table
        {
            border-collapse: collapse;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function showdialog(t,param) {
            jQuery("#showOne").html(t);
            jQuery("#showOne").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    if(jQuery("#showOne").attr("IsClear")!="0") {
                        jQuery("#Hid_CommitType").val("0");
                        jQuery("#Hid_IsCancel").val("0");
                    }
                },
                buttons: {
                    '确定': function () {
                        if(param!=null&&param.type==0) {
                            location.href=param.url;
                        } else {
                            jQuery(this).dialog('close');
                        }
                    }
                }
            });
        }
        function showdialogN2(t,param) {
            jQuery("#showOne").html(t);
            jQuery("#showOne").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    if(jQuery("#showOne").attr("IsClear")!="0") {
                        jQuery("#Hid_CommitType").val("0");
                        jQuery("#Hid_IsCancel").val("0");
                    }
                }
            });
        }
        function isIE(number) {
            if(typeof (number)!=number) {
                return !!document.all;
            }
        }
        //ie
        function copyValue(strValue) {
            if(isIE()) {
                clipboardData.setData("Text",strValue);
            }
            else {
                copy(strValue);
            }
        }
        //非IE
        function copy(text2copy) {
            var flashcopier='flashcopier';
            if(!document.getElementById(flashcopier)) {
                var divholder=document.createElement('div');
                divholder.id=flashcopier;
                document.body.appendChild(divholder);
            }
        }
        //返回Url
        function GoBack() {
            var val=jQuery.trim(jQuery("#Hid_GoUrl").val());
            if(val!="") {
                location.href=val;
            } else {
                location.go(-1);
            }
            return false;
        }
        //日期大小比较 开始时间>结束时间返回true 否则false
        function CompareDate(sdate,edate) {
            var s_year=1900,s_month=1,s_day=1,s_hour=0,s_minute=0,s_second=0;
            var e_year=1900,e_month=1,e_day=1,e_hour=0,e_minute=0,e_second=0;
            //开始日期
            var strDate1=sdate.split(' ')[0].split('-');
            var date1;
            if(strDate1.length==3) {
                s_year=strDate1[0];
                s_month=strDate1[1];
                s_day=strDate1[2];
            }
            if(sdate.split(' ').length==2) {
                var strTime1=sdate.split(' ')[1].split(':');
                if(strTime1.length==2) {
                    s_hour=strTime1[0];
                    s_minute=strTime1[1]
                } else if(strTime1.length==3) {
                    s_hour=strTime1[0];
                    s_minute=strTime1[1];
                    s_second=strTime1[2];
                }
            }
            //结束日期
            var strDate2=edate.split(' ')[0].split('-');
            var date2;
            if(strDate2.length==3) {
                e_year=strDate2[0];
                e_month=strDate2[1];
                e_day=strDate2[2];
            }
            if(edate.split(' ').length==2) {
                var strTime2=edate.split(' ')[1].split(':');
                if(strTime2.length==2) {
                    e_hour=strTime2[0];
                    e_minute=strTime2[1]
                } else if(strTime2.length==3) {
                    e_hour=strTime2[0];
                    e_minute=strTime2[1];
                    e_second=strTime2[2];
                }
            }
            date1=new Date(s_year,s_month,s_day,s_hour,s_minute,s_second);
            date2=new Date(e_year,e_month,e_day,e_hour,e_minute,e_second);
            return (date1>date2);
        }
        //申请类型 设置显示隐藏
        function ApplayType() {
            var selType=jQuery("input[name='rblList']:checked").val();
            if(selType=="3") {
                //退票
                jQuery('#ddlGQApp').hide();
                jQuery('#ddlFPApp').hide();


                //显示退票理由 1自愿、 0非自愿
                if(jQuery("#Hid_ZY").val()=="1") {
                    jQuery('#ddlTPApp1').show();
                    jQuery('#ddlTPApp').hide();
                } else if(jQuery("#Hid_ZY").val()=="0") {
                    jQuery('#ddlTPApp').show();
                    jQuery('#ddlTPApp1').hide();
                }

                jQuery("#trGQ").hide();
                jQuery("#trGqType").hide();
                //改签选项
                jQuery("#TrGQOption").hide();
                //显示  
                jQuery("#trskyinfo").show();
            }
            else if(selType=="4") {
                //废票
                jQuery('#ddlGQApp').hide();
                jQuery('#ddlFPApp').show(); //显示废票理由
                jQuery('#ddlTPApp').hide();
                jQuery('#ddlTPApp1').hide();

                jQuery("#trGQ").hide();
                jQuery("#trGqType").hide();
                //改签选项
                jQuery("#TrGQOption").hide();
                //显示  
                jQuery("#trskyinfo").show();
            } else if(selType=="5") {
                //改签
                jQuery('#ddlGQApp').show(); //显示改签理由
                jQuery('#ddlFPApp').hide();
                jQuery('#ddlTPApp').hide();
                jQuery('#ddlTPApp1').hide();

                jQuery("#trGQ").show();
                jQuery("#trGqType").show();
                //改签选项
                jQuery("#TrGQOption").show();
                //隐藏
                jQuery("#trskyinfo").hide();

                //改签类型
                var selVal=jQuery("input[type='radio'][name='GQOption']:checked").val();
                GQ_Option(selVal);
            }
            //退票 自愿与非自愿显示与隐藏
            if(selType=="3") {
                jQuery("#thIsZy").show();
                jQuery("#tdIsZy").show();
            } else {
                jQuery("#thIsZy").hide();
                jQuery("#tdIsZy").hide();
            }
        }
        //改签选项设置 0同等舱位改期 1升舱并改期
        function GQ_Option(type) {
            jQuery("input[type='radio'][name='GQOption'][value='"+type+"']").attr("checked",true);
            var selType=jQuery("input[name='rblList']:checked").val();
            if(selType=="5") {
                //改签
                if(jQuery("#trGQ").is(":hidden")) {
                    jQuery("#trGQ").show();
                }
                //设置要修改的数据
                jQuery("#sky1_tab tr[id*='trsky_']").each(function (index,tr) {
                    var jTr=jQuery(tr);
                    //出发日期
                    jTr.find("input[type='text'][id*='txtFromDate']").show();
                    jTr.find("span[id*='spanFromDate']").hide();
                    //到达日期
                    jTr.find("input[type='text'][id*='txtToDate']").show();
                    jTr.find("span[id*='spanToDate']").hide();
                    //航班号
                    jTr.find("input[type='text'][id*='txtFlightCode']").show();
                    jTr.find("span[id*='spanFlightCode']").hide();

                    if(type=="0") {
                        //同等舱位改期                                                                    
                        jTr.find("input[type='text'][id*='txtSpace']").hide();
                        jTr.find("span[id*='spanSpace']").show();
                    } else if(type=="1") {
                        // 升舱并改期                                               
                        jTr.find("input[type='text'][id*='txtSpace']").show();
                        jTr.find("span[id*='spanSpace']").hide();
                    }
                });
            }
        }
        //分隔符
        var splitChar="@@@@";
        //开始申请
        function StartApplay() {
            try {
                jQuery("#btnApplay").attr("disabled",true);
                var selType=jQuery("input[name='rblList']:checked").val();
                //选择的成人,儿童,婴儿数目
                var AdultCount=0,CHDCount=0,INFCount=0;
                //乘客和航段数据
                var pasData=[];
                var skyData=[];
                var IsOk=false; //是否验证成功
                var IsSelAll=false; //是否全选乘机人
                var IsSelPas=false; //是否选择了乘客
                //选择的乘机人
                var pastr=jQuery("#pas_tab tr[id*='tr_']");
                pastr.each(function (index,tr) {
                    var jTr=jQuery(tr);
                    //复选框
                    var ck=jTr.find("input[id*='CkTGQ_']");
                    var id=ck.val();
                    //乘客姓名
                    var pasName=jTr.find("input[type='hidden'][id*='pasName_']").val().replace(splitChar,"");
                    //乘客类型
                    var pasPType=jTr.find("input[type='hidden'][id*='pasPType_']").val().replace(splitChar,"");
                    if(pasPType=="1") {
                        AdultCount++;
                    } else if(pasPType=="2") {
                        CHDCount++;
                    } else if(pasPType=="3") {
                        INFCount++;
                    }
                    //证件类型
                    var pasCType=jTr.find("input[type='hidden'][id*='pasCType_']").val().replace(splitChar,"");
                    //证件号码
                    var pasCid=jTr.find("input[type='hidden'][id*='pasCid_']").val().replace(splitChar,"");
                    //行程单号
                    var pasTravelNumber=jTr.find("input[type='hidden'][id*='pasTravelNumber_']").val().replace(splitChar,"");
                    //票号
                    var pasTicketNum=jTr.find("input[type='hidden'][id*='pasTicketNum_']").val().replace(splitChar,"");
                    //IsBack_
                    var pasIsBack=jTr.find("input[type='hidden'][id*='pasIsBack_']").val().replace(splitChar,"");
                    var pArr=[];
                    pArr.push(id);
                    pArr.push(pasName);
                    pArr.push(pasPType);
                    pArr.push(pasCType);
                    pArr.push(pasCid);
                    pArr.push(pasTravelNumber);
                    pArr.push(pasTicketNum);
                    pArr.push(pasIsBack.toLowerCase()=="true"?"1":"0");
                    var flag=ck.is(":checked")?1:0
                    pArr.push(flag); //选中的话
                    if(flag==1&&!IsSelPas) {
                        IsSelPas=true;
                    }
                    //加入
                    pasData.push(pArr.join(splitChar));
                });
                if(!IsSelPas) {
                    showdialog("请选择需要申请的乘客！");
                } else {
                    //是否全选
                    if(pasData.length==pastr.length) {
                        IsSelAll=true; //设为全选
                    } else {
                        IsSelAll=false;
                    }
                    //构造航段数据
                    jQuery("#sky_tab0 tr[id*='trsky0_']").each(function (index,tr) {
                        var jTr=jQuery(tr);
                        var model=[];
                        //航段id
                        var SkyId=jTr.find("input[type='hidden'][id*='SkyId_']").val().replace(splitChar,"");
                        var FromCityName=jTr.find("input[type='hidden'][id*='FromCityName_']").val().replace(splitChar,"");
                        var ToCityName=jTr.find("input[type='hidden'][id*='ToCityName_']").val().replace(splitChar,"");
                        var FromCityCode=jTr.find("input[type='hidden'][id*='FromCityCode_']").val().replace(splitChar,"");
                        var ToCityCode=jTr.find("input[type='hidden'][id*='ToCityCode_']").val().replace(splitChar,"");
                        var FromDate=jTr.find("input[type='hidden'][id*='FromDate_']").val().replace(splitChar,"");
                        var ToDate=jTr.find("input[type='hidden'][id*='ToDate_']").val().replace(splitChar,"");
                        var CarryCode=jTr.find("input[type='hidden'][id*='CarryCode_']").val().replace(splitChar,"");
                        var FlightCode=jTr.find("input[type='hidden'][id*='FlightCode_']").val().replace(splitChar,"");
                        var Space=jTr.find("input[type='hidden'][id*='Space_']").val().replace(splitChar,"");

                        model.push(SkyId);
                        model.push(FromCityName);
                        model.push(ToCityName);
                        model.push(FromCityCode);
                        model.push(ToCityCode);
                        model.push(FromDate);
                        model.push(ToDate);
                        model.push(CarryCode);
                        model.push(FlightCode);
                        model.push(Space);

                        //加入数组
                        skyData.push(model.join(splitChar));
                    });

                    //申请理由判断

                    //退费票需要分离获取取消编码

                    //退票
                    if(selType=="3") {

                        if(jQuery("#Hid_ZY").val()=="1") {

                            var len=jQuery("#ddlTPApp1 option").length;
                            if(len>0) {
                                //可以通过
                                IsOk=true;
                            } else {
                                showdialog("请选择退票申请理由！");
                            }

                        } else if(jQuery("#Hid_ZY").val()=="0") {

                            var len=jQuery("#ddlTPApp option").length;
                            if(len>0) {
                                //可以通过
                                IsOk=true;
                            } else {
                                showdialog("请选择退票申请理由！");
                            }
                        }
                    }
                    //废票
                    else if(selType=="4") {
                        var len=jQuery("#ddlFPApp option").length;
                        if(len>0) {
                            //可以通过
                            IsOk=true;
                        } else {
                            showdialog("请选择废票申请理由！");
                        }
                    }
                    //改签
                    else if(selType=="5") {
                        var len=jQuery("#ddlGQApp option").length;
                        if(len==0) {
                            showdialog("请选择改签申请理由！");
                        } else {
                            if(jQuery("#trGQ").is(":hidden")) {
                                jQuery("#trGQ").show();
                            }
                            if(INFCount>0&&AdultCount==0) {
                                showdialog("不能单独改签婴儿，请选择一个成人!");
                            } else {

                                var err="";


                                //改签类型 0同等舱位改期 1升舱并改期
                                var GQType=jQuery("input[type='radio'][name='GQOption']:checked").val();
                                var GQsky=jQuery("#sky1_tab tr[id*='trsky_']");

                                GQsky.each(function (index,tr) {
                                    var jTr=jQuery(tr);
                                    //出发日期
                                    var startDate=jTr.find("input[type='text'][id*='txtFromDate']").val();
                                    //到达日期
                                    var endDate=jTr.find("input[type='text'][id*='txtToDate']").val();
                                    //航班号
                                    var FlightCode=jTr.find("input[type='text'][id*='txtFlightCode_']").val().replace(splitChar,"");
                                    //舱位
                                    var Space=jTr.find("input[type='hidden'][id*='Space_']").val().replace(splitChar,"");

                                    if(GQType=="1") {
                                        //舱位
                                        Space=jTr.find("input[type='text'][id*='txtSpace_']").val().replace(splitChar,"");
                                    }
                                    if(jQuery.trim(startDate)==""||jQuery.trim(endDate)=="") {
                                        err="航段"+(index+1)+"起飞日期和到达日期不能为空!";
                                        return false;
                                    } else if(CompareDate(startDate,endDate)) {
                                        //不能通过
                                        err="航段"+(index+1)+"起飞日期必须早于到达日期!";
                                        return false;
                                    } else if(jQuery.trim(FlightCode)=="") {
                                        err="航段"+(index+1)+"航班号不能为空!";
                                        return false;
                                    }
                                    else if(jQuery.trim(Space)=="") {
                                        err="航段"+(index+1)+"舱位不能为空!";
                                        return false;
                                    }
                                });

                                //改期 检查日期
                                if(err!="") {
                                    showdialog(err);
                                } else {
                                    //构造航段数据
                                    skyData=[];
                                    var msg="";
                                    var skyLen=0;
                                    GQsky.each(function (index,tr) {
                                        var jTr=jQuery(tr);
                                        var model=[];
                                        //航段id
                                        var SkyId=jTr.find("input[type='hidden'][id*='SkyId_']").val().replace(splitChar,"");
                                        var FromCityName=jTr.find("input[type='hidden'][id*='FromCityName_']").val().replace(splitChar,"");
                                        var ToCityName=jTr.find("input[type='hidden'][id*='ToCityName_']").val().replace(splitChar,"");
                                        var FromCityCode=jTr.find("input[type='hidden'][id*='FromCityCode_']").val().replace(splitChar,"");
                                        var ToCityCode=jTr.find("input[type='hidden'][id*='ToCityCode_']").val().replace(splitChar,"");
                                        var CarryCode=jTr.find("input[type='hidden'][id*='CarryCode_']").val().replace(splitChar,"");
                                        var old_FromDate=jTr.find("input[type='hidden'][id*='FromDate_']").val().replace(splitChar,"");
                                        var old_ToDate=jTr.find("input[type='hidden'][id*='ToDate_']").val().replace(splitChar,"");
                                        var old_FlightCode=jTr.find("input[type='hidden'][id*='FlightCode_']").val().replace(splitChar,"");
                                        var old_Space=jTr.find("input[type='hidden'][id*='Space_']").val().replace(splitChar,"");
                                        var Space=old_Space;
                                        //修改的数据
                                        // 0改期 
                                        var FromDate=jTr.find("input[type='text'][id*='txtFromDate_']").val().replace(splitChar,"");
                                        var ToDate=jTr.find("input[type='text'][id*='txtToDate_']").val().replace(splitChar,"");
                                        var FlightCode=jTr.find("input[type='text'][id*='txtFlightCode_']").val().replace(splitChar,"");
                                        // 0同等舱位改期 1升舱并改期
                                        if(GQType=="0") {
                                            if(old_FromDate==FromDate&&old_ToDate==ToDate&&old_FlightCode==FlightCode) {
                                                msg="同等舱位改期第"+(index+1)+"航段数据不能和原航段数据相同！";
                                                if(GQsky.length==1) {
                                                    return false;
                                                } else {
                                                    skyLen++;
                                                }
                                            }
                                        }
                                        else if(GQType=="1") {
                                            // 1升舱
                                            Space=jTr.find("input[type='text'][id*='txtSpace_']").val().replace(splitChar,"");
                                            if(old_FromDate==FromDate&&old_ToDate==ToDate&&old_FlightCode==FlightCode&&Space==old_Space) {
                                                msg="升舱并改期第"+(index+1)+"航段数据不能和原航段数据相同！";
                                                if(GQsky.length==1) {
                                                    return false;
                                                } else {
                                                    skyLen++;
                                                }
                                            }
                                        }

                                        model.push(SkyId);
                                        model.push(FromCityName);
                                        model.push(ToCityName);
                                        model.push(FromCityCode);
                                        model.push(ToCityCode);
                                        model.push(FromDate);
                                        model.push(ToDate);
                                        model.push(CarryCode);
                                        model.push(FlightCode);
                                        model.push(Space);

                                        //加入数组
                                        skyData.push(model.join(splitChar));
                                    });
                                    if(msg=="") {
                                        //可以通过
                                        IsOk=true;
                                    } else {
                                        if(GQsky.length==1) {
                                            showdialog(msg);
                                            return;
                                        } else if(GQsky.length>1&&skyLen==GQsky.length) {
                                            showdialog(msg);
                                            return;
                                        } else {
                                            IsOk=true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //通过验证后赋值
                    if(IsOk) {
                        //设置乘客和航段数据
                        jQuery("#Hid_pasData").val(pasData.join('^'));
                        jQuery("#Hid_skyData").val(skyData.join('^'));
                        //隐藏按钮
                        jQuery("#tdBtnOk").hide();
                        //执行后台代码
                        jQuery("#btnOk").click();

                    } else {
                        jQuery("#Hid_pasData").val("");
                        jQuery("#Hid_skyData").val("");
                        jQuery("#tdBtnOk").show();
                    }
                }
            } catch(e) {
            } finally {
                jQuery("#btnApplay").attr("disabled",false);
            }
        }
        //取消按钮
        function btnClose() {
            jQuery("#showOne").dialog("close");
            jQuery("#Hid_CommitType").val("0");
            jQuery("#Hid_IsCancel").val("0");
            //alert("请先取消编码后申请退废票！");
        }
        function OK(selType,IsCancel) {
            var checked=jQuery("#cboType").attr("checked");
            jQuery("#Hid_IsCancel").val(checked?"1":"0");
            //StartApplay();
            //隐藏按钮
            jQuery("#tdBtnOk").hide();
            jQuery("#showOne").attr("IsClear","0");
            jQuery("#showOne").dialog('close');
            jQuery("#showOne").removeAttr("IsClear");
            //执行后台代码
            jQuery("#btnOk").click();

            // jQuery("#Hid_CommitType").val("0");
            //  jQuery("#Hid_IsCancel").val("0");
        }
        //自愿和非自愿退票显示隐藏 1自愿 0非自愿
        function ZYTP(type) {
            jQuery("#Hid_ZY").val(type); //1自愿 0非自愿
            if(type==1) {
                jQuery('#ddlTPApp1').show();
                jQuery('#ddlTPApp').hide();
            } else if(type==0) {
                jQuery('#ddlTPApp').show();
                jQuery('#ddlTPApp1').hide();
            }
        }

        //加载。。。
        jQuery(function () {
            ApplayType();
        })
       
    </script>
    <style type="text/css">
        .table_info th, .table_info td
        {
            border-color: #CCCCCC;
            border-style: solid;
            border-width: 1px 1px 1px 1px;
            color: #606060;
        }
        table
        {
            border-collapse: collapse;
        }
        .tdNew
        {
            font-weight: bold;
            background: url("../img/title.png") repeat-x scroll 0 0 transparent;
            text-align: right;
        }
        .td1
        {
            color: Red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="tabs">
        <div id="tabs-1" class="ui-tabs-panel ui-widget-content ui-corner-bottom">
            <table width="100%" align="center" border="0" cellpadding="0" cellspacing="0" style="border: 1px solid #E6E6E6;">
                <tr>
                    <td class="mainl">
                    </td>
                    <td>
                        <table width="100%" align="center" class="detail" border="0" cellpadding="5" cellspacing="0"
                            style="padding: 5px;">
                            <tr>
                                <td>
                                    <div class="ebill-bg-top">
                                        <h1>
                                            退改签申请</h1>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%;">
                                    <div>
                                        <strong style="color: #FC9331;">申请操作</strong>
                                    </div>
                                    <table id="table3" class="else-table" width="100%">
                                        <tr>
                                            <td class="tdNew" style="width: 12%">
                                                申请类型:
                                            </td>
                                            <td style="width: 38%" align="left">
                                                <asp:RadioButtonList ID="rblList" runat="server" RepeatDirection="Horizontal" onclick="ApplayType()">
                                                    <%--    <asp:ListItem Selected="True" Value="3">退票申请</asp:ListItem>
                                                    <asp:ListItem Value="4">废票申请</asp:ListItem>
                                                    <asp:ListItem Value="5">改签申请</asp:ListItem>--%>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td id="thIsZy" class="tdNew" style="width: 12%">
                                                是否自愿:
                                            </td>
                                            <td id="tdIsZy" align="left" style="width: 38%">
                                                <input id="rblSType_1" type="radio" name="rblSType" checked="checked" onclick="ZYTP(1);" /><label
                                                    for="rblSType_1">自愿申请</label>
                                                <input id="rblSType_0" type="radio" name="rblSType" onclick="ZYTP(0);" /><label for="rblSType_0">非自愿申请</label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 12%">
                                            </td>
                                            <td colspan="3" align="left">
                                                <span class="td1"><b>温馨提示:当天出票可进行废票、退票申请；非当天出票只能进行退票申请 </b></span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew" style="width: 12%">
                                                申请理由:
                                            </td>
                                            <td align="left" colspan="3" style="width: 88%">
                                                <%--改签--%>
                                                <asp:DropDownList ID="ddlGQApp" Width="700px" runat="server" class="hide">
                                                </asp:DropDownList>
                                                <%--退票 非自愿--%>
                                                <asp:DropDownList ID="ddlTPApp" Width="700px" runat="server" class="hide">
                                                </asp:DropDownList>
                                                <%--退票 自愿--%>
                                                <asp:DropDownList ID="ddlTPApp1" Width="700px" runat="server">
                                                </asp:DropDownList>
                                                <%--废票--%>
                                                <asp:DropDownList ID="ddlFPApp" Width="700px" runat="server" class="hide">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew" style="width: 12%">
                                                申请备注:
                                            </td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="txtRemark" runat="server" MaxLength="200" TextMode="MultiLine" Width="700px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div>
                                        <strong style="color: #FC9331;">订单信息</strong>
                                    </div>
                                    <table id="table9" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                                        border="0">
                                        <tr>
                                            <td class="tdNew" style="width: 12%">
                                                订单编号:
                                            </td>
                                            <td style="width: 26%" class="td1">
                                                <asp:Label ID="lblOrderId" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew" style="width: 10%">
                                                PNR:
                                            </td>
                                            <td style="width: 20%; font-weight: bold">
                                                <asp:Label ID="lblPNR" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew" style="width: 12%">
                                                订单金额:
                                            </td>
                                            <td style="width: 20%" class=" td1">
                                                <asp:Label ID="lblPayMoney" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                订单来源:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblOrderSourceType" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                订单状态:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblOrderStatusCode" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                政策点数:
                                            </td>
                                            <td class="td1" align="center" valign="middle">
                                                <asp:Label ID="lblPolicyPoint" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                创建时间:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblCreateTime" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                支付时间:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayTime" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                出票时间:
                                            </td>
                                            <td align="center" valign="middle">
                                                <asp:Label ID="lblCPTime" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                支付方式:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayWay" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                内部交易流水号:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblInPayNo" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                支付流水号:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayNo" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                支付状态:
                                            </td>
                                            <td>
                                                <asp:Label ID="lblPayStatus" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td class="tdNew">
                                                机票状态检查:
                                            </td>
                                            <td style="text-align: left">
                                                <asp:DropDownList ID="ddlCheckType" Enabled="false" runat="server">
                                                    <asp:ListItem Text="需要检查" Value="0" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="不需要检查" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                                <span class="btn btn-ok-s" style="display: none;">
                                                    <asp:Button ID="btnUpdate" runat="server" Text="修 改" />
                                                </span>
                                            </td>
                                            <td class="tdNew">
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew">
                                                废、改处理时间:
                                            </td>
                                            <td colspan="5" style="text-align: left">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew" style="background-image: url('../img/title1.png');">
                                                客规参考:
                                            </td>
                                            <td colspan="5" style="text-align: left;">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdNew" style="background-image: url('../img/title1.png');">
                                                政策备注:
                                            </td>
                                            <td colspan="5" style="text-align: left">
                                                <asp:Label ID="lblPolicyRemark" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="trskyinfo">
                                <td>
                                    <div style="height: 22px">
                                        <strong style="color: #FC9331;">行程信息</strong>
                                    </div>
                                    <table id="sky_tab0" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                        width="100%">
                                        <thead>
                                            <tr>
                                                <th>
                                                    起飞城市
                                                </th>
                                                <th>
                                                    到达城市
                                                </th>
                                                <th>
                                                    起飞日期
                                                </th>
                                                <th>
                                                    到达日期
                                                </th>
                                                <th>
                                                    承运人
                                                </th>
                                                <th>
                                                    航班号
                                                </th>
                                                <th>
                                                    舱位
                                                </th>
                                                <th>
                                                    折扣
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepSkyWay" runat="server">
                                            <ItemTemplate>
                                                <tr class="leftliebiao_checi" style="line-height: 30px;" id='trsky0_<%# Eval("id")%>'>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# Eval("FromCityName") %>
                                                        <input id='SkyId_<%# Eval("id")%>' type="hidden" value='<%# Eval("id")%>' />
                                                        <input id='FromCityName_<%# Eval("id")%>' type="hidden" value='<%# Eval("FromCityName")%>' />
                                                        <input id='ToCityName_<%# Eval("id")%>' type="hidden" value='<%# Eval("ToCityName")%>' />
                                                        <input id='FromCityCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("FromCityCode")%>' />
                                                        <input id='ToCityCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("ToCityCode")%>' />
                                                        <input id='FromDate_<%# Eval("id")%>' type="hidden" value='<%# Eval("FromDate")%>' />
                                                        <input id='ToDate_<%# Eval("id")%>' type="hidden" value='<%# Eval("ToDate")%>' />
                                                        <input id='CarryCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("CarryCode")%>' />
                                                        <input id='FlightCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("FlightCode")%>' />
                                                        <input id='Space_<%# Eval("id")%>' type="hidden" value='<%# Eval("Space")%>' />
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# Eval("ToCityName") %>
                                                    </td>
                                                    <td style="width: 20%; text-align: center;">
                                                        <%# Eval("FromDate") %>
                                                    </td>
                                                    <td style="width: 20%; text-align: center;">
                                                        <%# Eval("ToDate") %>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# Eval("CarryCode")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# Eval("FlightCode") %>
                                                    </td>
                                                    <td style="width: 10%; text-align: center; color: Red;">
                                                        <%# Eval("Space") %>
                                                    </td>
                                                    <td style="width: 10%; text-align: center; color: Red;">
                                                        <%# Eval("Discount") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <%--改签设置选项--%>
                            <tr id="trGQ" class="hide">
                                <td>
                                    <div>
                                        <%-- <strong style="color: #FC9331;">改签信息</strong> --%>
                                        <strong class="td1"><b>改签信息</b></strong> <span id="TrGQOption"><b>改签类型:</b></span>
                                        <span id="trGqType">
                                            <label for="ckGQ_Time">
                                                <input id="ckGQ_Time" type="radio" name="GQOption" value="0" checked="checked" onclick="GQ_Option(this.value)" />同等舱位改期</label>
                                            <label for="ckGQ_Space">
                                                <input id="ckGQ_Space" type="radio" name="GQOption" value="1" onclick="GQ_Option(this.value)" />升舱并改期</label>
                                        </span>
                                    </div>
                                    <table id="sky1_tab" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                        width="100%">
                                        <thead>
                                            <tr>
                                                <th style="width: 10%; text-align: center;">
                                                    起飞城市
                                                </th>
                                                <th style="width: 10%; text-align: center;">
                                                    到达城市
                                                </th>
                                                <th style="width: 10%; text-align: center;">
                                                    起飞日期
                                                </th>
                                                <th style="width: 10%; text-align: center;">
                                                    到达日期
                                                </th>
                                                <th style="width: 10%; text-align: center;">
                                                    承运人
                                                </th>
                                                <th style="width: 10%; text-align: center;">
                                                    航班号
                                                </th>
                                                <th style="width: 10%; text-align: center;">
                                                    舱位
                                                </th>
                                                <th>
                                                    折扣
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="repGQ" runat="server">
                                            <ItemTemplate>
                                                <tr class="leftliebiao_checi" style="line-height: 30px;" id='trsky_<%# Eval("id") %>'>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# Eval("FromCityName") %>
                                                        <%--数据--%>
                                                        <input id='SkyId_<%# Eval("id")%>' type="hidden" value='<%# Eval("id")%>' />
                                                        <input id='FromCityName_<%# Eval("id")%>' type="hidden" value='<%# Eval("FromCityName")%>' />
                                                        <input id='ToCityName_<%# Eval("id")%>' type="hidden" value='<%# Eval("ToCityName")%>' />
                                                        <input id='CarryCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("CarryCode")%>' />
                                                        <input id='FromCityCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("FromCityCode")%>' />
                                                        <input id='ToCityCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("ToCityCode")%>' />
                                                        <input id='FromDate_<%# Eval("id")%>' type="hidden" value='<%# Eval("FromDate")%>' />
                                                        <input id='ToDate_<%# Eval("id")%>' type="hidden" value='<%# Eval("ToDate")%>' />
                                                        <input id='FlightCode_<%# Eval("id")%>' type="hidden" value='<%# Eval("FlightCode")%>' />
                                                        <input id='Space_<%# Eval("id")%>' type="hidden" value='<%# Eval("Space")%>' />
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# Eval("ToCityName") %>
                                                    </td>
                                                    <td style="width: 20%; text-align: center;">
                                                        <input type="text" id="txtFromDate_<%# Eval("id") %>" style="width: 150px;" readonly="true"
                                                            onfocus="WdatePicker({isShowClear:false,autoPickDate:true, minDate:'%y-%M-%d %H:%m:%s',dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                                                            value='<%# Eval("FromDate") %>' />
                                                        <span class="hide" id='spanFromDate<%# Eval("id") %>'>
                                                            <%# Eval("FromDate") %></span>
                                                    </td>
                                                    <td style="width: 20%; text-align: center;">
                                                        <input type="text" id='txtToDate_<%# Eval("id") %>' style="width: 150px;" readonly="true"
                                                            onfocus="WdatePicker({isShowClear:false,autoPickDate:true,minDate:'%y-%M-%d %H:%m:%s',dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                                                            value='<%# Eval("ToDate") %>' />
                                                        <span class="hide" id='spanToDate<%# Eval("id") %>'>
                                                            <%# Eval("ToDate")%></span>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <%# Eval("CarryCode")%>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <input id='txtFlightCode_<%# Eval("id") %>' value='<%# Eval("FlightCode")%>' type="text"
                                                            style="width: 50px" maxlength="4" />
                                                        <span class="hide" id="spanFlightCode<%# Eval("id") %>">
                                                            <%# Eval("FlightCode")%></span>
                                                    </td>
                                                    <td style="width: 10%; text-align: center; color: Red;">
                                                        <input id='txtSpace_<%# Eval("id") %>' value='<%# Eval("Space")%>' type="text" maxlength="2"
                                                            style="width: 50px" class="hide" />
                                                        <span class="hide" id='spanSpace<%# Eval("id") %>'>
                                                            <%# Eval("Space")%></span>
                                                    </td>
                                                    <td style="width: 10%; text-align: center; color: Red;">
                                                        <%# Eval("Discount") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div>
                                        <strong style="color: #FC9331;">乘客信息</strong>&nbsp;&nbsp;&nbsp;&nbsp;<span runat="server"
                                            visible="false" id="showSpValue" style="color: Red;">PNR导入订单，申请退废票时，请手动取消乘机人。</span>
                                    </div>
                                    <table id="pas_tab" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                        width="100%">
                                        <thead>
                                            <tr>
                                                <th>
                                                </th>
                                                <th>
                                                    姓名
                                                </th>
                                                <th>
                                                    类型
                                                </th>
                                                <th>
                                                    证件类型
                                                </th>
                                                <th>
                                                    证件号码
                                                </th>
                                                <th>
                                                    票号
                                                </th>
                                                <th>
                                                    状态
                                                </th>
                                                <th>
                                                    舱位价
                                                </th>
                                                <th>
                                                    机建
                                                </th>
                                                <th>
                                                    燃油
                                                </th>
                                            </tr>
                                        </thead>
                                        <asp:Repeater ID="RepPassenger" runat="server">
                                            <ItemTemplate>
                                                <tr class="leftliebiao_checi" style="line-height: 30px;" id="tr_<%#Eval("id")%>">
                                                    <td style="width: 4%; text-align: center;" align="center">
                                                        <input id='CkTGQ_<%# Eval("id") %>' type="checkbox" class='<%# ShowData(0,Eval("IsBack")) %>'
                                                            value='<%# Eval("id") %>' />
                                                    </td>
                                                    <td style="width: 9%; text-align: center;">
                                                        <asp:Label ID="lblPassengerName" runat="server" Text='<%# Eval("PassengerName") %>'></asp:Label>
                                                    </td>
                                                    <td style="width: 9%; text-align: center;">
                                                        <asp:Label ID="lblPType" runat="server" Text=' <%#ShowData(1,Eval("PassengerType"))%>'></asp:Label>
                                                    </td>
                                                    <td style="width: 10%; text-align: center;">
                                                        <asp:Label ID="lblCType" runat="server" Text='<%#ShowData(2,Eval("CType")) %>'></asp:Label>
                                                    </td>
                                                    <td style="width: 17%; text-align: center;">
                                                        <asp:Label ID="txtCid" runat="server" Text='<%# Eval("Cid")%>'></asp:Label>
                                                    </td>
                                                    <td style="width: 15%; text-align: center;">
                                                        <asp:Label ID="lblTicketNumber" runat="server" Text=' <%# Eval("TicketNumber")%>'></asp:Label>
                                                        <input id='btcopytnum' type='button' value='复制' onclick="copyValue('<%# Eval("TicketNumber") %>')"
                                                            class='<%#ShowData(4,Eval("TicketNumber")) %>' />
                                                    </td>
                                                    <td style="width: 9%;">
                                                        <%--机票状态--%>
                                                        <%#ShowData(9, Eval("TicketStatus"))%>
                                                    </td>
                                                    <td style="width: 9%; text-align: center; color: Red;">
                                                        <%# Eval("PMFee")%>
                                                    </td>
                                                    <td style="width: 9%; text-align: center; color: Red;">
                                                        <%# Eval("ABFee")%>
                                                    </td>
                                                    <td style="width: 9%; text-align: center; color: Red;">
                                                        <%# Eval("FuelFee")%>
                                                        <%--数据--%>
                                                        <input type="hidden" id='pasName_<%# Eval("id") %>' value='<%# Eval("PassengerName") %>' />
                                                        <input type="hidden" id='pasPType_<%# Eval("id") %>' value='<%# Eval("PassengerType") %>' />
                                                        <input type="hidden" id='pasCType_<%# Eval("id") %>' value='<%# Eval("CType") %>' />
                                                        <input type="hidden" id='pasCid_<%# Eval("id") %>' value='<%# Eval("Cid") %>' />
                                                        <input type="hidden" id='pasTravelNumber_<%# Eval("id") %>' value='<%# Eval("TravelNumber") %>' />
                                                        <input type="hidden" id='pasTicketNum_<%# Eval("id") %>' value='<%# Eval("TicketNumber") %>' />
                                                        <input type="hidden" id='pasIsBack_<%# Eval("id") %>' value='<%# Eval("IsBack") %>' />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td align="center" id="tdBtnOk">
                                                <span class="btn btn-ok-s" id="spanApplay" runat="server">
                                                    <input id="btnApplay" type="button" value="确认申请" onclick="if(confirm('是否确认要申请')){ StartApplay();}" />
                                                    <asp:Button ID="btnOk" runat="server" Text="" class="hide" OnClick="btnOk_Click" />
                                                </span>&nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnCancel" runat="server" Text="返回" OnClientClick="return GoBack();" /></span>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="ebill">
                                        <div>
                                            <span class="ebill-account"><span class="inner"><span class="inner">基础操作信息</span> </span>
                                            </span>
                                        </div>
                                        <div class="ebill-info-bg-inner">
                                            <table border="0" cellspacing="0" cellpadding="0" width="100%">
                                                <tr>
                                                    <td style="text-align: right; border: 1px">
                                                        <asp:Label ID="lbl1" runat="server" Text="锁定操作员：" Width="100px"></asp:Label>
                                                    </td>
                                                    <td class="tab_in_td_f" style="width: 40%;">
                                                        <asp:Label ID="lblLockId" runat="server" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Label ID="lbl2" runat="server" Text="锁定时间：" Width="100px"></asp:Label>
                                                    </td>
                                                    <td class="tab_in_td_f" style="width: 40%;">
                                                        <asp:Label ID="lblLockTime" runat="server" Text="Label"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="table1" class="else-table" border="0" cellspacing="0" cellpadding="0"
                                                width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>
                                                            操作时间
                                                        </th>
                                                        <th>
                                                            操作员账号
                                                        </th>
                                                        <th>
                                                            操作员姓名
                                                        </th>
                                                        <th>
                                                            操作类型
                                                        </th>
                                                        <th>
                                                            详细记录
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <asp:Repeater ID="RepOrderLog" runat="server">
                                                    <ItemTemplate>
                                                        <tr class="leftliebiao_checi" style="line-height: 30px;">
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperTime")%>
                                                            </td>
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperLoginName")%>
                                                            </td>
                                                            <td style="width: 15%; text-align: center;">
                                                                <%#Eval("OperUserName")%>
                                                            </td>
                                                            <td style="width: 10%; text-align: center;">
                                                                <%#Eval("OperType")%>
                                                            </td>
                                                            <td style="width: 45%; text-align: center;">
                                                                <asp:Label Style="word-break: break-all; white-space: normal" ID="lblLogContent"
                                                                    Width="100%" runat="server" Text=' <%#Eval("OperContent")%>' ToolTip='<%#Eval("OperContent") %>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                            <div class="fn-clear">
                                            </div>
                                        </div>
                                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="ebill-info-bg">
                                            <tr>
                                                <td class="ebill-info-bgl">
                                                </td>
                                                <td class="ebill-info-bgc">
                                                </td>
                                                <td class="ebill-info-bgr">
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="mainr">
                    </td>
                </tr>
            </table>
        </div>
        <div id="showOne">
        </div>
    </div>
    <input type="hidden" runat="server" id="hidType" value="0" />
    <%--取消编码使用--%>
    <input id="hidOrderSource" type="hidden" runat="server" />
    <input id="hidBtnType" type="hidden" runat="server" value="0" />
    <%--提交类型--%>
    <input id="hidSelPerNum" type="hidden" runat="server" value="0" />
    <%--提交人数--%>
    <input id="hidPolicySource" type="hidden" runat="server" />
    <%--返回url--%>
    <input id="Hid_GoUrl" type="hidden" runat="server" />
    <%--参入后台代码使用乘客数据--%>
    <input id="Hid_pasData" type="hidden" runat="server" />
    <%--参入后台代码使用航段数据--%>
    <input id="Hid_skyData" type="hidden" runat="server" />
    <%--编码提交标志0开始 1取消编码 2分离编码 --%>
    <input id="Hid_CommitType" type="hidden" runat="server" value="0" />
    <%--是否取消编码 1取消 0不取消--%>
    <input id="Hid_IsCancel" type="hidden" runat="server" value="0" />
    <%--1自愿 0非自愿--%>
    <input id="Hid_ZY" type="hidden" runat="server" value="1" />
    </form>
</body>
</html>
