<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddDefaultPolicy.aspx.cs"
    Inherits="Policy_AddDefaultPolicy" %>

<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>默认政策</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
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
    <script type="text/javascript" language="javascript">
    <!--
        //为Jquery重新命名
        var jQueryOne=jQuery.noConflict(false);
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
                        jQueryOne(this).dialog('close');
                    }
                }
            });
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
        //--------------------------------------------------------
        //适用班期 班期限制
        function setSchedule(val) {
            var banqiGroup=document.getElementsByName("banqiGroup");
            var _val=val;
            if(_val==undefined||_val==null) {
                _val="";
                for(var i=0;i<banqiGroup.length;i++) {
                    if(banqiGroup[i].checked) {
                        _val+=banqiGroup[i].value+"/";
                    }
                }
            } else {
                var Arr=_val.split('/');
                for(var i=0;i<Arr.length;i++) {
                    if(jQueryOne.trim(Arr[i])!="") {
                        jQueryOne("input[name='banqiGroup'][type='checkbox'][value='"+jQueryOne.trim(Arr[i])+"']").attr("checked",true);
                    }
                }
            }
            jQueryOne("#Schedule").val(_val);
        }
        function GetStrDate(date) {
            return (date.getFullYear()+"-"+(date.getMonth()+1)+"-"+date.getDate());
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
        //数字验证【0-100.00】
        function NumVate() {
            try {

                var value=jQueryOne.trim(jQueryOne(this).val());
                if(value==null||value=='') {
                    showdialog("文本框不能为空,请输入正确的数字!");
                    jQueryOne(this).val("0");
                    return false;
                }
                var ctrlId=jQueryOne(this).attr("id");
                //数据范围0-100
                var NumFanWei=jQueryOne(this).attr("FanWei");
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
                        'txtAdultDownPoint',
                        'txtAdultLaterPoint',
                        'txtAdultSharePoint',
                        'txtChildDownPoint',
                        'txtChildLaterPoint',
                        'txtChildSharePoint'
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
        //初始化数据
        jQueryOne(function () {
            //返点
            jQueryOne("#txtAdultDownPoint").blur(NumVate);
            jQueryOne("#txtAdultLaterPoint").blur(NumVate);
            jQueryOne("#txtAdultSharePoint").blur(NumVate);

            jQueryOne("#txtChildDownPoint").blur(NumVate);
            jQueryOne("#txtChildLaterPoint").blur(NumVate);
            jQueryOne("#txtChildSharePoint").blur(NumVate);

            //现返
            jQueryOne("#txtAdultDownReturnMoney").blur(NumVate);
            jQueryOne("#txtAdultLaterReturnMoney").blur(NumVate);
            jQueryOne("#txtShareReturnMoney").blur(NumVate);

            jQueryOne("#txtChildDownReturnMoney").blur(NumVate);
            jQueryOne("#txtChildLaterReturnMoney").blur(NumVate);
            jQueryOne("#txtChildShareReturnMoney").blur(NumVate);

            jQueryOne("#txtOffice_0").blur(function () {
                var val=jQueryOne.trim(jQueryOne(this).val());
                var patOffice=/^[A-Za-z]{3}\d{3}$/;
                if(val!=""&&!patOffice.test(val)) {
                    showdialog("输入Office格式错误!");
                    return false;
                }
            });

            //加载
            var adultVal=jQueryOne.trim(jQueryOne("#Hid_AdultPolicy").val());
            if(adultVal!="") {
                var AdultModel=eval("("+unescape(adultVal)+")");
                if(AdultModel!=null&&AdultModel!=undefined) {
                    jQueryOne("#txtOffice_0").val(AdultModel._office);
                    //返点
                    jQueryOne("#txtAdultDownPoint").val(AdultModel._downpoint);
                    jQueryOne("#txtAdultLaterPoint").val(AdultModel._laterpoint);
                    jQueryOne("#txtAdultSharePoint").val(AdultModel._sharepoint);
                    //现返                   
                    jQueryOne("#txtAdultDownReturnMoney").val(AdultModel._downreturnmoney);
                    jQueryOne("#txtAdultLaterReturnMoney").val(AdultModel._laterreturnmoney);
                    jQueryOne("#txtShareReturnMoney").val(AdultModel._sharepointreturnmoney);


                    jQueryOne("input[name='AdultPolicyType'][type='radio'][value="+AdultModel._policytype+"]").attr("checked",true);
                    setSchedule(AdultModel._scheduleconstraints);

                    //乘机开始日期
                    var date=eval("new "+AdultModel._flightstartdate.NewReplace("/","")+"");
                    jQueryOne("#txtFlightStartDate").val(GetStrDate(date));
                    //乘机结束日期
                    date=eval("new "+AdultModel._flightenddate.NewReplace("/","")+"");
                    jQueryOne("#txtFlightEndDate").val(GetStrDate(date));
                    //出票开始日期
                    date=eval("new "+AdultModel._printstartdate.NewReplace("/","")+"");
                    jQueryOne("#txtTicketStartDate").val(GetStrDate(date));
                    //出票结束日期
                    date=eval("new "+AdultModel._printenddate.NewReplace("/","")+"");
                    jQueryOne("#txtTicketEndDate").val(GetStrDate(date));

                    jQueryOne("#txtPolicyRemark").val(AdultModel._remark);
                }
            }
            var childVal=jQueryOne.trim(jQueryOne("#Hid_ChildPolicy").val());
            if(childVal!="") {
                var ChildModel=eval("("+unescape(childVal)+")");
                if(ChildModel!=null&&ChildModel!=undefined) {
                    jQueryOne("#txtOffice_0").val(ChildModel._office);
                    //返点
                    jQueryOne("#txtChildDownPoint").val(ChildModel._downpoint);
                    jQueryOne("#txtChildLaterPoint").val(ChildModel._laterpoint);
                    jQueryOne("#txtChildSharePoint").val(ChildModel._sharepoint);

                    //现返                          
                    jQueryOne("#txtChildDownReturnMoney").val(ChildModel._downreturnmoney);
                    jQueryOne("#txtChildLaterReturnMoney").val(ChildModel._laterreturnmoney);
                    jQueryOne("#txtChildShareReturnMoney").val(ChildModel._sharepointreturnmoney);

                    jQueryOne("input[name='childPolicyType'][type='radio'][value="+ChildModel._policytype+"]").attr("checked",true);
                    setSchedule(ChildModel._scheduleconstraints);

                    //乘机开始日期
                    var date=eval("new "+ChildModel._flightstartdate.NewReplace("/","")+"");
                    jQueryOne("#txtFlightStartDate").val(GetStrDate(date));
                    //乘机结束日期
                    date=eval("new "+ChildModel._flightenddate.NewReplace("/","")+"");
                    jQueryOne("#txtFlightEndDate").val(GetStrDate(date));
                    //出票开始日期
                    date=eval("new "+ChildModel._printstartdate.NewReplace("/","")+"");
                    jQueryOne("#txtTicketStartDate").val(GetStrDate(date));
                    //出票结束日期
                    date=eval("new "+ChildModel._printenddate.NewReplace("/","")+"");
                    jQueryOne("#txtTicketEndDate").val(GetStrDate(date));

                    jQueryOne("#txtPolicyRemark").val(ChildModel._remark);
                }
            }
        });
        var addCount=0;
        //添加政策
        function AddPolicy() {

            var adultId="",childId="";
            var adultVal=jQueryOne.trim(jQueryOne("#Hid_AdultPolicy").val());
            if(adultVal!="") {
                var AdultModel=eval("("+unescape(adultVal)+")");
                if(AdultModel!=null&&AdultModel!=undefined) {
                    adultId=AdultModel._id;
                }
            }
            var childVal=jQueryOne.trim(jQueryOne("#Hid_ChildPolicy").val());
            if(childVal!="") {
                var ChildModel=eval("("+unescape(childVal)+")");
                if(ChildModel!=null&&ChildModel!=undefined) {
                    childId=ChildModel._id;
                }
            }
            var val_CarryCode=jQueryOne("#Hid_AirCode").val();
            //班期
            setSchedule();
            var val_Office=jQueryOne("#txtOffice_0").val();
            var patOffice=/^[A-Za-z]{3}\d{3}$/;
            if(val_Office!=""&&!patOffice.test(val_Office)) {
                showdialog("输入Office格式错误!");
                return false;
            }

            //政策
            var val_DownPoint="0";
            var val_LaterPoint="0";
            var val_SharePoint="0";
            var val_DownReturnMoney="0";
            var val_LaterReturnMoney="0";
            var val_ShareReturnMoney="0";

            var val_PolicyType="1";
            var val_Id="";
            var val_A1="1";
            if(addCount==0) {
                val_A1="1";
                if(adultId=="") {
                    val_OpType="0";//添加
                } else {
                    val_OpType="1";//修改
                    val_Id=adultId;
                }
                //返点
                val_DownPoint=jQueryOne("#txtAdultDownPoint").val();
                val_LaterPoint=jQueryOne("#txtAdultLaterPoint").val();
                val_SharePoint=jQueryOne("#txtAdultSharePoint").val();
                //现返
                val_DownReturnMoney=jQueryOne("#txtAdultDownReturnMoney").val();
                val_LaterReturnMoney=jQueryOne("#txtAdultLaterReturnMoney").val();
                val_ShareReturnMoney=jQueryOne("#txtShareReturnMoney").val();

                val_PolicyType=jQueryOne("input[name='AdultPolicyType'][type='radio']:checked").val();
            } else if(addCount==1) {
                val_A1="2";
                if(childId=="") {
                    val_OpType="0";//添加
                } else {
                    val_OpType="1";//修改
                    val_Id=childId;
                }
                //返点
                val_DownPoint=jQueryOne("#txtChildDownPoint").val();
                val_LaterPoint=jQueryOne("#txtChildLaterPoint").val();
                val_SharePoint=jQueryOne("#txtChildSharePoint").val();
                //现返
                val_DownReturnMoney=jQueryOne("#txtChildDownReturnMoney").val();
                val_LaterReturnMoney=jQueryOne("#txtChildLaterReturnMoney").val();
                val_ShareReturnMoney=jQueryOne("#txtChildShareReturnMoney").val();

                val_PolicyType=jQueryOne("input[name='childPolicyType'][type='radio']:checked").val();
            }
            //日期
            var val_FlightStartDate=jQueryOne("#txtFlightStartDate").val();
            var val_FlightEndDate=jQueryOne("#txtFlightEndDate").val();
            var val_PrintStartDate=jQueryOne("#txtTicketStartDate").val();
            var val_PrintEndDate=jQueryOne("#txtTicketEndDate").val();

            //返点
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
            //现返
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
            //备注
            var val_PolicyRemark=jQueryOne("#txtPolicyRemark").val();
            var val_Schedule=jQueryOne("#Schedule").val();


            jQueryOne("#addAndNext").attr("disabled",true);
            jQueryOne.post("../AJAX/CommonAjAx.ashx",
            {
                CarryCode: escape(val_CarryCode),
                Office: escape(val_Office),
                DownPoint: escape(val_DownPoint),
                LaterPoint: escape(val_LaterPoint),
                SharePoint: escape(val_SharePoint),

                DownReturnMoney: escape(val_DownReturnMoney),
                LaterReturnMoney: escape(val_LaterReturnMoney),
                ShareReturnMoney: escape(val_ShareReturnMoney),



                PolicyType: escape(val_PolicyType),
                Schedule: escape(val_Schedule),
                PolicyRemark: escape(val_PolicyRemark),
                FlightStartDate: escape(val_FlightStartDate),
                FlightEndDate: escape(val_FlightEndDate),
                PrintStartDate: escape(val_PrintStartDate),
                PrintEndDate: escape(val_PrintEndDate),
                AuditType: escape(1),
                A1: escape(val_A1),
                OpFunction: escape("OpPolicy"),
                OpPage: escape("AddDefaultPolicy.aspx"),
                OpType: escape(val_OpType),
                Id: escape(val_Id),
                num: Math.random(),
                currentuserid: jQueryOne("#currentuserid").val()
            },
             function (data) {
                 jQueryOne("#addAndNext").attr("disabled",false);
                 addCount++;
                 if(addCount==2) {
                     //完成
                     addCount=0;
                     showdialog('保存成功');
                 } else {
                     AddPolicy();
                 }
             },
            "text");
        }
        function GetCode(text,val,sel) {

            jQueryOne("#addAndNext").attr("disabled",true);
            jQueryOne("#Hid_AirCode").val(val);
            jQueryOne.post("../AJAX/CommonAjAx.ashx",
            {
                CarryCode: escape(val),
                cpyNO: jQueryOne("#Hid_CpyNO").val(),
                OpFunction: escape("defaultPolicy"),
                OpType: escape(3),
                num: Math.random(),
                currentuserid: jQueryOne("#currentuserid").val()
            },function (data) {

                jQueryOne("#addAndNext").attr("disabled",false);
                var strReArr=data.split('##');
                if(strReArr.length==3) {
                    //错误代码
                    var errCode=strReArr[0];
                    //错误描述
                    var errDes=strReArr[1];
                    //错误结果
                    var result=unescape(strReArr[2]);
                    var Arr=result.split('$@@@@@@@$');
                    if(Arr[0]!="") {
                        //成人
                        jQueryOne("#Hid_AdultPolicy").val(escape(Arr[0]));
                        var AdultModel=eval("("+Arr[0]+")");
                        if(AdultModel!=null&&AdultModel!=undefined) {
                            jQueryOne("#txtOffice_0").val(AdultModel._office);
                            //返点
                            jQueryOne("#txtAdultDownPoint").val(AdultModel._downpoint);
                            jQueryOne("#txtAdultLaterPoint").val(AdultModel._laterpoint);
                            jQueryOne("#txtAdultSharePoint").val(AdultModel._sharepoint);
                            //现返
                            jQueryOne("#txtAdultDownReturnMoney").val(AdultModel._downreturnmoney);
                            jQueryOne("#txtAdultLaterReturnMoney").val(AdultModel._laterreturnmoney);
                            jQueryOne("#txtShareReturnMoney").val(AdultModel._sharepointreturnmoney);


                            jQueryOne("input[name='AdultPolicyType'][type='radio'][value="+AdultModel._policytype+"]").attr("checked",true);
                            setSchedule(AdultModel._scheduleconstraints);
                            //乘机开始日期
                            var date=eval("new "+AdultModel._flightstartdate.NewReplace("/","")+"");
                            jQueryOne("#txtFlightStartDate").val(GetStrDate(date));
                            //乘机结束日期
                            date=eval("new "+AdultModel._flightenddate.NewReplace("/","")+"");
                            jQueryOne("#txtFlightEndDate").val(GetStrDate(date));
                            //出票开始日期
                            date=eval("new "+AdultModel._printstartdate.NewReplace("/","")+"");
                            jQueryOne("#txtTicketStartDate").val(GetStrDate(date));
                            //出票结束日期
                            date=eval("new "+AdultModel._printenddate.NewReplace("/","")+"");
                            jQueryOne("#txtTicketEndDate").val(GetStrDate(date));
                            jQueryOne("#txtPolicyRemark").val(AdultModel._remark);
                        }
                    }
                    if(Arr.length>1&&Arr[1]!="") {
                        //儿童
                        jQueryOne("#Hid_ChildPolicy").val(escape(Arr[1]));
                        var ChildModel=eval("("+Arr[1]+")");
                        if(ChildModel!=null&&ChildModel!=undefined) {
                            jQueryOne("#txtOffice_0").val(ChildModel._office);
                            //返点
                            jQueryOne("#txtChildDownPoint").val(ChildModel._downpoint);
                            jQueryOne("#txtChildLaterPoint").val(ChildModel._laterpoint);
                            jQueryOne("#txtChildSharePoint").val(ChildModel._sharepoint);
                            //现返                          
                            jQueryOne("#txtChildDownReturnMoney").val(ChildModel._downreturnmoney);
                            jQueryOne("#txtChildLaterReturnMoney").val(ChildModel._laterreturnmoney);
                            jQueryOne("#txtChildShareReturnMoney").val(ChildModel._sharepointreturnmoney);

                            jQueryOne("input[name='childPolicyType'][type='radio'][value="+ChildModel._policytype+"]").attr("checked",true);
                            setSchedule(ChildModel._scheduleconstraints);
                            //乘机开始日期
                            var date=eval("new "+ChildModel._flightstartdate.NewReplace("/","")+"");
                            jQueryOne("#txtFlightStartDate").val(GetStrDate(date));
                            //乘机结束日期
                            date=eval("new "+ChildModel._flightenddate.NewReplace("/","")+"");
                            jQueryOne("#txtFlightEndDate").val(GetStrDate(date));
                            //出票开始日期
                            date=eval("new "+ChildModel._printstartdate.NewReplace("/","")+"");
                            jQueryOne("#txtTicketStartDate").val(GetStrDate(date));
                            //出票结束日期
                            date=eval("new "+ChildModel._printenddate.NewReplace("/","")+"");
                            jQueryOne("#txtTicketEndDate").val(GetStrDate(date));
                            jQueryOne("#txtPolicyRemark").val(AdultModel._remark);
                        }
                    }
                }
            },
            "text");
        }
        //返回按钮
        function ReGo() {
            var PageType=jQueryOne("#Hid_PageType").val();
            var IsEdit=jQueryOne("#Hid_IsEdit").val();
            var cid=jQueryOne("#Hid_id").val();
            var pid=jQueryOne("#Hid_currPage").val();
            var where=jQueryOne("#Hid_where").val();//条件
            location.href="PolicyList.aspx?PageType="+PageType+"&edit="+IsEdit+"&cid="+cid+"&pid="+pid+"&where="+where+"&currentuserid=<%=this.mUser.id.ToString() %>";
        }
        //结束乘机日期同步到出票结束日期
        function TicketFocus(n) {
            WdatePicker(
            {
                isShowClear: false,
                isShowWeek: false,
                minDate: '%y-%M-%d',
                autoPickDate: true,
                dateFmt: 'yyyy-MM-dd',
                onpicked: function () {
                    $dp.$('txtTicketEndDate').value=$dp.cal.getP('y')+'-'+$dp.cal.getP('M')+'-'+$dp.cal.getP('d');
                }
            });
        }      
    //-->
    </script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <div id="dgShow">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="title">
        <asp:Label ID="lblShow" Text="默认政策发布" runat="server" />
    </div>
    <div>
        <table style="width: 100%; border-collapse: collapse; border-color: #DFF0FD;" border="1"
            cellpadding="0" cellspacing="50" id="tabDefault">
            <tr>
                <th class="w_td">
                    航空公司:
                </th>
                <td>
                    <uc2:SelectAirCode ID="SelectAirCode1" runat="server" IsDShowName="false" DefaultOptionValue="ALL"
                        ChangeFunctionName="GetCode" />
                </td>
            </tr>
            <tr>
                <th class="w_td">
                    Office:
                </th>
                <td>
                    <input type="text" id="txtOffice_0" class="tdw120 inputBorder" maxlength="6" />
                </td>
            </tr>
            <tr>
                <th class="w_td">
                    默认政策:
                </th>
                <td>
                    <table width="100%" border="1" style="line-height: 50px; text-align: center;">
                        <tr>
                            <td>
                                乘客类型
                            </td>
                            <td>
                                返点
                            </td>
                            <td>
                                现返
                            </td>
                            <td>
                                政策类型
                            </td>
                        </tr>
                        <tr>
                            <td>
                                成人默认政策
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            下级分销返点
                                        </th>
                                        <td>
                                            <input id="txtAdultDownPoint" type="text" value="0" style="width: 90px;" maxlength="9"
                                                fanwei="0-100" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            下级分销后返
                                        </th>
                                        <td>
                                            <input id="txtAdultLaterPoint" type="text" value="0" style="width: 90px;" maxlength="9"
                                                fanwei="0-100" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            共享政策返点
                                        </th>
                                        <td>
                                            <input id="txtAdultSharePoint" type="text" value="0" style="width: 90px;" maxlength="9"
                                                fanwei="0-100" />%
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            下级分销现返金额
                                        </th>
                                        <td>
                                            <input id="txtAdultDownReturnMoney" type="text" value="0" style="width: 90px;" maxlength="9" />￥
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            下级分销后返现返金额
                                        </th>
                                        <td>
                                            <input id="txtAdultLaterReturnMoney" type="text" value="0" style="width: 90px;" maxlength="9" />￥
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            共享政策现返金额
                                        </th>
                                        <td>
                                            <input id="txtShareReturnMoney" type="text" value="0" style="width: 90px;" maxlength="9" />￥
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <label for="AdultPolicyType_1">
                                    <input type="radio" id="AdultPolicyType_1" name="AdultPolicyType" value="1" checked="checked" />B2B
                                </label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <label for="AdultPolicyType_2">
                                    <input type="radio" id="AdultPolicyType_2" name="AdultPolicyType" value="2" />BSP
                                </label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <label for="AdultPolicyType_3" class="hide">
                                    <input type="radio" id="AdultPolicyType_3" name="AdultPolicyType" value="3" />B2B/BSP
                                </label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                儿童默认政策
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            下级分销返点
                                        </th>
                                        <td>
                                            <input id="txtChildDownPoint" type="text" value="0" style="width: 90px;" maxlength="9"
                                                fanwei="0-100" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            下级分销后返
                                        </th>
                                        <td>
                                            <input id="txtChildLaterPoint" type="text" value="0" style="width: 90px;" maxlength="9"
                                                fanwei="0-100" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            共享政策返点
                                        </th>
                                        <td>
                                            <input id="txtChildSharePoint" type="text" value="0" style="width: 90px;" maxlength="9"
                                                fanwei="0-100" />%
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <th>
                                            下级分销现返金额
                                        </th>
                                        <td>
                                            <input id="txtChildDownReturnMoney" type="text" value="0" style="width: 90px;" maxlength="9" />￥
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            下级分销后返现返金额
                                        </th>
                                        <td>
                                            <input id="txtChildLaterReturnMoney" type="text" value="0" style="width: 90px;" maxlength="9" />￥
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            共享政策现返金额
                                        </th>
                                        <td>
                                            <input id="txtChildShareReturnMoney" type="text" value="0" style="width: 90px;" maxlength="9" />￥
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <label for="ChildPolicyType_1">
                                    <input type="radio" id="childPolicyType_1" name="childPolicyType" value="1" checked="checked" />B2B
                                </label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <label for="ChildPolicyType_2">
                                    <input type="radio" id="ChildPolicyType_2" name="childPolicyType" value="2" />BSP
                                </label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <label for="ChildPolicyType_3" class="hide">
                                    <input type="radio" id="ChildPolicyType_3" name="childPolicyType" value="3" />B2B/BSP
                                </label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th class="w_td">
                    <span style="color: #ff0066;">*</span>班期限制：
                </th>
                <td class="r-td">
                    <div style="float: left">
                        <label for="day0">
                            <input type="checkbox" id="day0" name="banqiGroup" value="7" />星期日</label>
                        <label for="day1">
                            <input type="checkbox" id="day1" name="banqiGroup" value="1" />星期一</label>
                        <label for="day2">
                            <input type="checkbox" id="day2" name="banqiGroup" value="2" />星期二</label>
                        <label for="day3">
                            <input type="checkbox" id="day3" name="banqiGroup" value="3" />星期三</label>
                        <label for="day4">
                            <input type="checkbox" id="day4" name="banqiGroup" value="4" />星期四</label>
                        <label for="day5">
                            <input type="checkbox" id="day5" name="banqiGroup" value="5" />星期五</label>
                        <label for="day6">
                            <input type="checkbox" id="day6" name="banqiGroup" value="6" />星期六</label>
                    </div>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <span style="color: Green;">说明:勾选上后限制政策不生效，默认全部生效</span>
                </td>
            </tr>
            <tr id="tr2">
                <th class="w_td">
                    <span style="color: #ff0066;">*</span>乘机日期：
                </th>
                <td class="r-td" style="text-align: left;">
                    <div>
                        <input type="text" id="txtFlightStartDate" style="width: 130px;" readonly="true"
                            runat="server" class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />至
                        <input type="text" id="txtFlightEndDate" style="width: 130px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="TicketFocus(this)" />
                    </div>
                </td>
            </tr>
            <tr id="tr3">
                <th class="w_td">
                    <span style="color: #ff0066;">*</span>出票日期：
                </th>
                <td class="r-td" style="text-align: left;">
                    <div>
                        <input type="text" id="txtTicketStartDate" style="width: 130px;" readonly="true"
                            class="inputBorder" runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />至
                        <input type="text" id="txtTicketEndDate" style="width: 130px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                    </div>
                </td>
            </tr>
            <tr>
                <th class="w_td">
                    政策备注：
                </th>
                <td class="r-td" style="text-align: left;">
                    <textarea id="txtPolicyRemark" cols="60" rows="5" class="inputBorder" maxlength="500"
                        style="width: 600px;"></textarea>
                </td>
            </tr>
            <tr>
                <td class="r-td" align="center" colspan="2">
                    <span class="btn btn-ok-s">
                        <input type="button" value="保存" onclick="AddPolicy()" id="addAndNext" runat="server" /></span>
                    <span class="btn btn-ok-s">
                        <input type="button" value="返回" onclick="ReGo()" /></span>
                </td>
            </tr>
        </table>
    </div>
    <%--成人默认政策--%>
    <input type="hidden" id="Hid_AdultPolicy" runat="server" />
    <%--成人默认政策--%>
    <input type="hidden" id="Hid_ChildPolicy" runat="server" />
    <input type="hidden" id="Schedule" runat="server" />
    <input type="hidden" id="Hid_AirCode" value="ALL" runat="server" />
    <%--列表来源查询条件 用于返回列表--%>
    <%--编辑部分 1编辑 0添加--%>
    <input type="hidden" id="Hid_IsEdit" value="0" runat="server" />
    <input type="hidden" id="Hid_PageType" runat="server" value="3" />
    <input type="hidden" id="Hid_where" runat="server" />
    <input type="hidden" id="Hid_currPage" runat="server" />
    <input type="hidden" id="Hid_id" runat="server" />
    <input type="hidden" id="Hid_CpyNO" value="0" runat="server" />
    </form>
</body>
</html>
