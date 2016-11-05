//-----set部分---------------------------------------------------------
//发布类型 1.出港，2.入港,3.全国
function setPublishType(v) {
    jQueryOne("#ReleaseType").val(v);
    //行程类型
    var TravelType=jQueryOne("#TravelType").val();
    if(v=="1") {
        //出巷
        jQueryOne("#tr_to1").hide();
        jQueryOne("#tr_to2").show();
        jQueryOne("#tr_to3").show();

        jQueryOne("#tr_from1").show();
        jQueryOne("#tr_from2").hide();
        jQueryOne("#tr_from2_01").hide();
        jQueryOne("#tr_from3").hide();

        jQueryOne("#SameFrom").show();
        jQueryOne("#SameTo").hide();

        //中转联程
        jQueryOne("#tr_middle2").hide();
        jQueryOne("#tr_middle_02").hide();
        jQueryOne("#tr_middle3").hide();
    } else if(v=="2") {
        //入巷
        jQueryOne("#tr_to1").show();
        jQueryOne("#tr_to2").hide();
        jQueryOne("#tr_to3").hide();
        jQueryOne("#tr_to2_01").hide();

        jQueryOne("#tr_from1").hide();
        jQueryOne("#tr_from2").show();
        jQueryOne("#tr_from2_01").show();
        jQueryOne("#tr_from3").show();

        jQueryOne("#SameFrom").hide();
        jQueryOne("#SameTo").show();

        //中转联程
        jQueryOne("#tr_middle2").hide();
        jQueryOne("#tr_middle_02").hide();
        jQueryOne("#tr_middle3").hide();
    } else {
        //全国                
        jQueryOne("#tr_to1").hide();
        jQueryOne("#tr_to2").show();
        jQueryOne("#tr_to3").show();
        jQueryOne("#tr_to2_01").show();

        jQueryOne("#tr_from1").hide();
        jQueryOne("#tr_from2").show();
        jQueryOne("#tr_from2_01").show();
        jQueryOne("#tr_from3").show();

        jQueryOne("#SameFrom").hide();
        jQueryOne("#SameTo").hide();
    }
    if(TravelType==4) {
        //中转联程
        jQueryOne("#tr_middle2").show();
        jQueryOne("#tr_middle_02").show();
        jQueryOne("#tr_middle3").show();
    } else {
        //中转联程
        jQueryOne("#tr_middle2").hide();
        jQueryOne("#tr_middle_02").hide();
        jQueryOne("#tr_middle3").hide();
    }
    var PublishGroup=document.getElementsByName("PublishGroup");
    for(var i=0;i<PublishGroup.length;i++) {
        if(PublishGroup[i]!=null&&PublishGroup[i].value==v) {
            PublishGroup[i].checked=true;
            break;
        }
    }
}
//设置行程类型 1.单程，2.往返/单程，3.往返，4.中转联程
function setTravelType(v) {
    //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
    var PageType=jQueryOne("#Hid_PageType").val();
    jQueryOne("#TravelType").val(v);
    //1普通政策
    if(PageType=="1") {
        //是否显示低开复选框1,4隐藏
        //if(v==1||v==4) {
        //    jQueryOne("#IsLowerOpenDiv").hide();
        //} else {
        jQueryOne("#IsLowerOpenDiv").show();
        //}
        //联程和往返显示特价复选框
        var PolicyCount=jQueryOne("#Hid_dyncId").val();
        for(var i=0;i<PolicyCount;i++) {
            if(v==3||v==4) {
                jQueryOne("#TJshow_"+i).show();
            } else {
                jQueryOne("#TJshow_"+i).hide();
                jQueryOne("#TJSpace_"+i).hide();
                jQueryOne("#TJCK_"+i).attr("checked",false);
            }
        }
    }

    //行程类型组
    var TravelGroup=document.getElementsByName('TravelGroup');
    for(var i=0;i<TravelGroup.length;i++) {
        if(TravelGroup[i].type&&TravelGroup[i].value==v) {
            TravelGroup[i].checked=true;
            break;
        }
    }
    //发布类型
    var ReleaseType=jQueryOne("#ReleaseType").val();
    if(v=="1"||v=="2"||v=="3") {
        if(ReleaseType=="1"||ReleaseType=="2") {
            jQueryOne("#tr_middle2").hide();
            jQueryOne("#tr_middle_02").hide();
            jQueryOne("#tr_middle3").hide();
        }
    }
    else if(v=="4") {
        if(ReleaseType=="1"||ReleaseType=="2") {
            jQueryOne("#tr_middle2").show();
            jQueryOne("#tr_middle_02").show();
            jQueryOne("#tr_middle3").show();
        }
        else if(ReleaseType=="3") {
            //中转联程
            jQueryOne("#tr_from1").hide();
            jQueryOne("#tr_from2").show();
            jQueryOne("#tr_from2_01").show();
            jQueryOne("#tr_from3").show();

            jQueryOne("#tr_middle1").hide();
            jQueryOne("#tr_middle2").show();
            jQueryOne("#tr_middle_02").show();
            jQueryOne("#tr_middle3").show();

            jQueryOne("#tr_to1").hide();
            jQueryOne("#tr_to2").show();
            jQueryOne("#tr_to2_01").show();
            jQueryOne("#tr_to3").show();
        }
    }
}
//加载
function setETDZType(_val) {
    /*
    var val=_val;
    if(val==undefined||val==null) {
    val=jQueryOne("input[name='ETDZType'][type='radio']:checked").val();
    } else {
    jQueryOne("#divETDZType_0 input[name='ETDZType_0'][type='radio'][value='"+val+"']").attr("checked",true);
    }
    jQueryOne("#Hid_etdz").val(val);
    */
    if(_val!=null) {
        jQueryOne("#divetdztype_0 input[name='etdztype_0'][type='radio'][value='"+_val+"']").attr("checked",true);
    }
}
//适用航班号类型 1.全部2.适用3.不适用
function setFlightNumGroup(obj,FlightType,FlightValue) {
    var FlightNumGroup=jQueryOne("input[name='FlightNumGroup'][type='radio']");
    var _FlightType=FlightType;
    if(FlightType==undefined||FlightType==null) {
        if(obj!=null&&obj.checked) {
            _FlightType=obj.value;
        }
    }
    if(_FlightType=="1") {
        jQueryOne("#txtFlightValue")[0].disabled=true;
        jQueryOne("#txtFlightValue")[0].style.border="1px solid gray";
        jQueryOne("#txtFlightValue")[0].style.backgroundColor="#D4D0C8";
        jQueryOne("#txtFlightValue").val('');
    }
    else {
        jQueryOne("#txtFlightValue")[0].disabled=false;
        jQueryOne("#txtFlightValue")[0].style.border="1px solid gray";
        jQueryOne("#txtFlightValue")[0].style.backgroundColor="white";
        if(FlightValue!=undefined&&FlightValue!=null) {
            jQueryOne("#FlightValue").val(FlightValue);
            jQueryOne("#txtFlightValue").val(FlightValue);
        }
    }
    jQueryOne("input[name='FlightNumGroup'][type='radio'][value='"+_FlightType+"']").attr("checked",true);
    jQueryOne("#FlightType").val(_FlightType);
}
//同城共享设置
function setSameCityShare() {
    var citysame=jQueryOne("#ReleaseType").val();
    if(citysame=="1") {
        jQueryOne("#CityNameSame").val((jQueryOne("#FromSameCitySharePolicy")[0].checked?"1":"2"))
    } else if(citysame=="2") {
        jQueryOne("#CityNameSame").val((jQueryOne("#ToSameCitySharePolicy")[0].checked?"1":"2"))
    } else {
        jQueryOne("#CityNameSame").val("2");
    }
}
//设置适用航班号
function setFlightType() {
    var FlightNumGroup=document.getElementsByName("FlightNumGroup");
    var b='1';
    for(var i=0;i<FlightNumGroup.length;i++) {
        if(FlightNumGroup[i].checked) {
            b=FlightNumGroup[i].value;
            break;
        }
    }
    jQueryOne("#FlightType").val(b);
    jQueryOne("#FlightValue").val(jQueryOne("#txtFlightValue").val());
}
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
//设置特价政策票价生成方式
function setGenerationType(_val) {
    var val=_val;
    var PolicyCount=jQueryOne("#Hid_dyncId").val();
    for(var i=0;i<PolicyCount.length;i++) {
        if(val=="3") {
            jQueryOne("#tjTab_"+i).show();
        } else {
            jQueryOne("#tjTab_"+i).hide();
        }
    }
    jQueryOne("input[name='TJGenerationType'][type='radio'][value='"+val+"']").attr("checked",true);
}

//是否审核
function IsAudit() {
    jQueryOne("#IsAuditVal").val(jQueryOne("#CkIsAudit").is(":checked")?"1":"2");
}
//是否低开
function IsLowerOpen() {
    jQueryOne("#IsLowerOpen").val(jQueryOne("#cklowerOpen").is(":checked")?"1":"0");
}
//是否适用共享航班
function ApplyCarry(v) {
    if(v==1) {
        jQueryOne("#liApplyTo").show();
    }
    if(v==0) {
        jQueryOne("#liApplyTo").hide();
        jQueryOne("#txtApplyToFlightNo").val("");
    }
    jQueryOne("#IsApplyToShareFlight").val(v);
    jQueryOne("input[name='Flight'][type='radio'][value='"+v+"']").attr("checked",true);
}
//公用函数--------------------------------------------------
function EmptyArr(arr) {
    var newArr=[];
    for(var i=0;i<arr.length;i++) {
        if(jQueryOne.trim(arr[i])!="") {
            newArr.push(arr[i]);
        }
    }
    return newArr;
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
//-------------验证-----------------------
//数据,保留小数位数最大位数
function numberPoint(data) {
    if(data==null||data=='') {
        return false;
    }
    var reg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
    if(jQueryOne.isNaN(data)) {
        return false;
    }
    if(!reg.test(data)) {
        return false;
    }
    return true;
}
//适用航班号验证
function FlightNumberVate() {
    var ValidateSuc=true;
    //航班号验证
    var FlightType=jQueryOne("#FlightType").val();
    var val=jQueryOne.trim(jQueryOne("#txtFlightValue").val());
    var valBool=[];
    if(val!=""&&FlightType!="1") {
        val=val.replace(" ","/").replace("，","/").replace(",","/");
        var arr=val.split("/");
        var pattern=/^\d[0-9\*]{2,3}$/;
        var IsSuc=false;
        var count=0,count1=0;
        var err=[];
        for(var i=0;i<arr.length;i++) {
            arr[i]=arr[i].replace("/","");
            arr[i]=jQueryOne.trim(arr[i]);
            if(arr[i]!=""&&arr[i]!="/") {
                if((arr[i].length==4||arr[i].length==3)) {// && pattern.test(arr[i])) {
                    IsSuc=true;
                    count++;
                } else {
                    err.push(arr[i]);
                }
                count1++;
            }
        }
        if(IsSuc&&count==count1) {
        } else {
            showdialog("输入航班号"+err.join("/")+"格式错误");
            ValidateSuc=false;
            return;
        }
    }
    return ValidateSuc;
}

//输入数据验证
function VateData() {
    var ValidateSuc=true;
    //验证城市       
    var TravelType=jQueryOne("#TravelType").val();
    var FromCityCode=jQueryOne.trim(jQueryOne("#FromCityCode").val());
    var MiddleCityCode=jQueryOne.trim(jQueryOne("#MiddleCityCode").val());
    var ToCityCode=jQueryOne.trim(jQueryOne("#ToCityCode").val());
    if(FromCityCode=="") {
        showdialog("出发城市不能为空!");
        ValidateSuc=false;
        return;
    }
    if(ToCityCode=="") {
        showdialog("到达城市不能为空!");
        ValidateSuc=false;
        return;
    }
    if(TravelType=="4"&&MiddleCityCode=="") {
        showdialog("中转城市不能为空!");
        ValidateSuc=false;
        return;
    }
    //航班号验证
    if(!FlightNumberVate()) {
        ValidateSuc=false;
        return;
    }
    //验证政策
    var PageType=jQueryOne("#Hid_PageType").val();
    var objlength=jQueryOne('#tabAll tr[id*="policy_tr"]').length;
    var strArr;
    for(var i=0;i<objlength;i++) {
        if(PageType=="1"||PageType=="2") {
            strArr=GetPolicy(PageType,i);
            if(strArr[0]=="0") {
                showdialog(strArr[1]);
                ValidateSuc=false;
                return;
            }
        }
    }
    //提前天数
    var FlightValue=jQueryOne("#txtAdvanceDay").val();
    if(jQueryOne.isNaN(FlightValue)||FlightValue.indexOf(".")!= -1) {
        showdialog("提前天数输入格式错误");
        ValidateSuc=false;
        return;
    }
    //返点 保留两位小数
    var AirReBate=jQueryOne("#txtAirReBate").val();
    if(!numberPoint(AirReBate)) {
        showdialog("航空公司返点输入数据格式错误或者不合乎规范！");
        ValidateSuc=false;
        return;
    }
    //现返 保留两位小数
    var AirReturnMoney=jQueryOne("#txtAirReturnMoney").val();
    if(jQueryOne.isNaN(AirReturnMoney)) {
        showdialog("航空公司现返金额输入数据格式错误或者不合乎规范！");
        ValidateSuc=false;
        return;
    }

    //乘机日期
    var d1=CompareDate(jQueryOne("#txtFlightStartDate").val(),jQueryOne("#txtFlightEndDate").val());
    if(d1) {
        showdialog('乘机开始有效日期不能大于乘机结束有效日期！');
        ValidateSuc=false;
        return;
    }
    //出票日期
    var d1=CompareDate(jQueryOne("#txtTicketStartDate").val(),jQueryOne("#txtTicketEndDate").val());
    if(d1) {
        showdialog('出票开始有效日期不能大于出票结束有效日期！');
        ValidateSuc=false;
        return;
    }
    return ValidateSuc;
}
//-----get部分---------------------------------------------------------

//----添加政策---------------------------------------------------------
//政策组编号 返回内容数组
function GetPolicy(PageType,num) {
    //数据结构长度为3 1:验证数据状态0失败1成功 2:描述 3:数据
    var outArr=["0","",[]];
    if(PageType=="1"||PageType=="2") {
        //选中的仓位值
        var checkedObj=jQueryOne('#seat_'+num+' input[name="seat'+num+'"][type="checkbox"]:checked,#TJSpace_'+num+' input[name="seat'+num+'"][type="checkbox"]:checked');
        var vals=[];
        for(var i=0;i<checkedObj.length;i++) {
            vals.push(checkedObj[i].value);
        }

        //承运人二字代码  CA^中国国际航空公司^999
        var AirCode=jQueryOne.trim(jQueryOne('#Select_'+num).val());
        //office
        var office=jQueryOne('#txtOffice_'+num).val();
        //政策类型
        var PolicyType=jQueryOne('#SelPolicyType_'+num).val();
        //下级分销返点
        var DownPoint=jQueryOne("#DownPoint_"+num).val();
        //下级分销后返
        var LaterPoint=jQueryOne("#LaterPoint_"+num).val();
        //共享政策返点
        var SharePoint=jQueryOne("#SharePoint_"+num).val();
        //下级分销现返金额
        var DownReturnMoney=jQueryOne("#DownReturnMoney_"+num).val();
        //下级分销后返现返金额
        var LaterReturnMoney=jQueryOne("#LaterReturnMoney_"+num).val();
        //共享政策现返金额
        var ShareReturnMoney=jQueryOne("#ShareReturnMoney_"+num).val();

        //乘机日期
        var FlightStartDate=jQueryOne("#txtFlightStartDate_"+num).val();
        var FlightEndDate=jQueryOne("#txtFlightEndDate_"+num).val();
        //出票日期
        var TicketStartDate=jQueryOne("#txtTicketStartDate_"+num).val();
        var TicketEndDate=jQueryOne("#txtTicketEndDate_"+num).val();
        //提前天数
        var AdvanceDay=jQueryOne("#txtAdvanceDay_"+num).val();
        //选择的自动出票方式 0=手动，1=半自动，2=全自动
        var AutoTicketNum=jQueryOne("#divetdztype_"+num+" input[name='etdztype_"+num+"'][type='radio']:checked").val();
        if(PolicyType=="3")//政策类型为B2B/BSP 修改自动出票为手动
        {
            AutoTicketNum="0";
        }

        //固定价格
        var SpacePrice=jQueryOne("#txtTJGuding_"+num).val();
        //参考价格 
        var TJcankao=jQueryOne("#txtTJcankao_"+num).val();

        //提前天数
        if(jQueryOne.isNaN(AdvanceDay)) {
            outArr[1]="输入提前天数格式错误！";
        }
        //出票日期
        var d1=CompareDate(TicketStartDate,TicketEndDate);
        if(d1) {
            outArr[1]='出票开始有效日期不能大于出票结束有效日期！';
        }
        //乘机日期
        var d1=CompareDate(FlightStartDate,FlightEndDate);
        if(d1) {
            outArr[1]='乘机开始有效日期不能大于乘机结束有效日期！';
        }
        if(AirCode==""||AirCode=="0") {
            outArr[1]="请选择航空公司！";
        } else {
            AirCode=AirCode.split('^')[0];
        }
        var patOffice=/^[A-Za-z]{3}\d{3}$/;
        if(office!=""&&!patOffice.test(office)) {
            outArr[1]="输入Office格式错误!";
        }
        if(vals.length==0) {
            if(PageType=="1") {
                outArr[1]="请选择航空公司基本仓位！";
            } else if(PageType=="2") {
                outArr[1]="请选择航空公司特价仓位！";
            }
        }
        //点数
        if(jQueryOne.isNaN(DownPoint)) {
            outArr[1]="输入下级分销返点格式错误！";
        }
        if(jQueryOne.isNaN(LaterPoint)) {
            outArr[1]="输入下级分销后返格式错误！";
        }
        if(jQueryOne.isNaN(SharePoint)) {
            outArr[1]="输入共享政策返点格式错误！";
        }
        //现返
        if(jQueryOne.isNaN(DownReturnMoney)) {
            outArr[1]="输入下级分销现返金额格式错误！";
        }
        if(jQueryOne.isNaN(LaterReturnMoney)) {
            outArr[1]="输入下级分销后返现返金额格式错误！";
        }
        if(jQueryOne.isNaN(ShareReturnMoney)) {
            outArr[1]="输入共享政策现返金额格式错误！";
        }

        if(PageType=="2") {
            if(jQueryOne.isNaN(TJcankao)) {
                outArr[1]="输入参考价格格式错误！";
            }
            if(jQueryOne.isNaN(SpacePrice)) {
                outArr[1]="输入固定特价格式错误！";
            }
        }
        if(outArr[1]!="") {
            return outArr;
        }
        else {
            outArr[0]="1";
            outArr[2].push(AirCode);
            outArr[2].push(vals.join('/'));
            outArr[2].push(office);
            outArr[2].push(PolicyType);
            //返点
            outArr[2].push(DownPoint);
            outArr[2].push(LaterPoint);
            outArr[2].push(SharePoint);
            //现返
            outArr[2].push(DownReturnMoney);
            outArr[2].push(LaterReturnMoney);
            outArr[2].push(ShareReturnMoney);

            if(PageType=="2") {
                //固定特价和参考价格
                outArr[2].push(SpacePrice);
                outArr[2].push(TJcankao);
            } else {
                outArr[2].push("0");
                outArr[2].push("0");
            }
            outArr[2].push(FlightStartDate);
            outArr[2].push(FlightEndDate);
            outArr[2].push(TicketStartDate);
            outArr[2].push(TicketEndDate);
            outArr[2].push(AdvanceDay);
            outArr[2].push(AutoTicketNum);//自动出票方式
        }
    }
    return outArr;
}
//获取操作类型 0添加 1修改 2删除 3查询
function GetOpType() {
    return jQueryOne("#Hid_IsEdit").val();
}
var AddNum=0;
function AddPolicy() {
    //[控制部分]
    //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
    var PageType=jQueryOne("#Hid_PageType").val();
    //操作类型
    var val_OpType=GetOpType();
    ////操作政策的功能
    var val_OpFunction="OpPolicy";

    //[数据部分]

    //添加几条政策
    var MaxAddNum=parseInt(jQueryOne("#Hid_dyncId").val());
    //出票方式自动出票
    setETDZType();
    //获取同城共享
    setSameCityShare();
    //审核
    IsAudit();
    //低开
    IsLowerOpen();
    //适用航班号
    setFlightType();

    //设置班期
    setSchedule();
    //数据验证
    if(!VateData()) {
        return;
    }


    //公司编号
    var val_CpyNo=jQueryOne("#Hid_CpyNo").val();
    //供应商名字
    var val_CpyName=jQueryOne("#Hid_CpyName").val();
    //登录账号
    var val_LoginName=jQueryOne("#Hid_LoginName").val();
    //政策id
    var val_Id=jQueryOne("#Hid_id").val();

    //政策种类
    var val_PolicyKind=jQueryOne("#Hid_PolicyKind").val();
    //票价生成方式 1.正常价格，2.动态特价，3.固定特价
    var val_GenerationType=jQueryOne("input[name='TJGenerationType'][type='radio']:checked").val();

    //发布类型 1.出港，2.入港,3.全国
    var val_ReleaseType=jQueryOne("#ReleaseType").val();
    //行程类型 1.单程，2.往返/单程，3.往返，4.中转联程
    var val_TravelType=jQueryOne("#TravelType").val();
    //政策类型 1=B2B，2=BSP，3=B2B/BSP
    var val_PolicyType=jQueryOne("#PolicyType").val();//
    //团队标志 0.普通，1.团队 
    var val_TeamFlag=jQueryOne("#txtTeamFlagCK").is(":checked")?"1":"0";
    //出发城市三字码（全国政策填：ALL）
    var val_FromCityCode=jQueryOne("#FromCityCode").val();
    //中转城市三字码（全国政策填：ALL）
    var val_MiddleCityCode=jQueryOne("#MiddleCityCode").val();
    //到达城市三字码（全国政策填：ALL）
    var val_ToCityCode=jQueryOne("#ToCityCode").val();
    //是否同城机场共享政策 1.是，2.否
    var val_CityNameSame=jQueryOne("#CityNameSame").val();
    //适用航班号类型 1.全部2.适用3.不适用
    var val_FlightType=jQueryOne("#FlightType").val();
    //适用或者不适用航班号
    var val_FlightValue=jQueryOne("#FlightValue").val();
    //班期限制 周一到周日 1-7
    var val_Schedule=jQueryOne("#Schedule").val();

    //航空公司返点
    var val_AirReBate=jQueryOne("#txtAirReBate").val();
    //航空公司现返金额
    var val_AirReturnMoney=jQueryOne("#txtAirReturnMoney").val();

    //提前天数
    var val_AdvanceDay=jQueryOne("#txtAdvanceDay").val();

    //乘机生效日期
    var val_FlightStartDate=jQueryOne("#txtFlightStartDate").val();
    //乘机失效日期
    var val_FlightEndDate=jQueryOne("#txtFlightEndDate").val();
    //出票生效日期
    var val_PrintStartDate=jQueryOne("#txtTicketStartDate").val();
    //出票失效日期
    var val_PrintEndDate=jQueryOne("#txtTicketEndDate").val();

    //承运人 航空公司编号
    //var val_CarryCode=jQueryOne("#Hid_CarryCode").val();

    //固定价格
    //var val_SpacePrice=jQueryOne("#txtTJGuding_"+AddNum).val();
    //参考价格 未加字段
    //var val_TJcankao=jQueryOne("#txtTJcankao_"+AddNum).val();

    //点数..//获取政策数据       
    var PolicyArr=GetPolicy(PageType,AddNum);
    var val_CarryCode="",val_ShippingSpace="",val_Office="",val_PolicyType="1",val_DownPoint="0",val_LaterPoint="0",val_SharePoint="0",val_SpacePrice="0",val_ReferencePrice="0";
    var val_DownReturnMoney="0",val_LaterReturnMoney="0",val_ShareReturnMoney="0",val_AutoPrintFlag="0";
    if(PageType=="1"||PageType=="2") {
        //航空公司二字码
        val_CarryCode=PolicyArr[2][0];
        //舱位
        val_ShippingSpace=PolicyArr[2][1];
        //出票Office号
        val_Office=PolicyArr[2][2];
        val_PolicyType=PolicyArr[2][3];
        //点数
        val_DownPoint=PolicyArr[2][4];
        val_LaterPoint=PolicyArr[2][5];
        val_SharePoint=PolicyArr[2][6];
        //现返
        val_DownReturnMoney=PolicyArr[2][7];
        val_LaterReturnMoney=PolicyArr[2][8];
        val_ShareReturnMoney=PolicyArr[2][9];

        val_SpacePrice=PolicyArr[2][10];
        val_ReferencePrice=PolicyArr[2][11];

        val_FlightStartDate=PolicyArr[2][12];
        val_FlightEndDate=PolicyArr[2][13];
        val_PrintStartDate=PolicyArr[2][14];
        val_PrintEndDate=PolicyArr[2][15];
        val_AdvanceDay=PolicyArr[2][16];
        val_AutoPrintFlag=PolicyArr[2][17];
    }
    //自动出票方式 手动(0或者null空)， 半自动1， 自动2
    //var val_AutoPrintFlag=jQueryOne("#Hid_etdz").val();

    //审核状态 1.已审，2.未审
    var val_AuditType=jQueryOne("#ck_AuditType").is(":checked")?"1":"2";


    //这条政策是否适用于共享航班 1适用 0不适用
    var val_IsApplyToShareFlight=jQueryOne("#IsApplyToShareFlight").val();
    //适用共享航空公司二字码 如:CA/CZ/ZH/HU
    var val_ShareAirCode=jQueryOne("#txtApplyToFlightNo").val();
    //是否往返低开 0不低开 1低开
    var val_IsLowerOpen=jQueryOne("#IsLowerOpen").val();
    //是否高返政策 1是 其他不是
    var val_HighPolicyFlag=jQueryOne("#ckIsGaoFan").is(":checked")?"1":"0";

    //专属扣点组ID
    var val_GroupId=jQueryOne("#Hid_GroupId").val();
    //政策备注
    var val_PolicyRemark=jQueryOne("#txtPolicyRemark").val();

    //禁用按钮
    jQueryOne("#addAndNext").attr("disabled",true);
    //发送请求
    jQueryOne.post("../AJAX/CommonAjAx.ashx",
    {
        //参数区域
        CpyNo: escape(val_CpyNo),
        CpyName: escape(val_CpyName),
        LoginName: escape(val_LoginName),

        PolicyKind: escape(val_PolicyKind),
        GenerationType: escape(val_GenerationType),
        ReleaseType: escape(val_ReleaseType),
        TravelType: escape(val_TravelType),
        PolicyType: escape(val_PolicyType),
        TeamFlag: escape(val_TeamFlag),
        FromCityCode: escape(val_FromCityCode),
        MiddleCityCode: escape(val_MiddleCityCode),
        ToCityCode: escape(val_ToCityCode),
        CityNameSame: escape(val_CityNameSame),
        FlightType: escape(val_FlightType),
        FlightValue: escape(val_FlightValue),
        Schedule: escape(val_Schedule),

        CarryCode: escape(val_CarryCode),
        Office: escape(val_Office),
        PolicyType: escape(val_PolicyType),
        ShippingSpace: escape(val_ShippingSpace),
        SpacePrice: escape(val_SpacePrice),
        ReferencePrice: escape(val_ReferencePrice),
        DownPoint: escape(val_DownPoint),
        LaterPoint: escape(val_LaterPoint),
        SharePoint: escape(val_SharePoint),

        DownReturnMoney: escape(val_DownReturnMoney),
        LaterReturnMoney: escape(val_LaterReturnMoney),
        ShareReturnMoney: escape(val_ShareReturnMoney),

        AdvanceDay: escape(val_AdvanceDay),
        AirReBate: escape(val_AirReBate),
        AirReturnMoney: escape(val_AirReturnMoney),
        FlightStartDate: escape(val_FlightStartDate),
        FlightEndDate: escape(val_FlightEndDate),
        PrintStartDate: escape(val_PrintStartDate),
        PrintEndDate: escape(val_PrintEndDate),
        AuditType: escape(val_AuditType),

        IsApplyToShareFlight: escape(val_IsApplyToShareFlight),
        ShareAirCode: escape(val_ShareAirCode),
        IsLowerOpen: escape(val_IsLowerOpen),
        HighPolicyFlag: escape(val_HighPolicyFlag),
        AutoPrintFlag: escape(val_AutoPrintFlag),
        GroupId: escape(val_GroupId),
        PolicyRemark: escape(val_PolicyRemark),
        OpPage: escape("NewAddPolicy.aspx"),
        OpFunction: escape(val_OpFunction),
        OpType: escape(val_OpType),
        Id: escape(val_Id),
        num: Math.random(),
        currentuserid: jQueryOne("#currentuserid").val()
    },
    function (data) {
        //启用按钮
        jQueryOne("#addAndNext").attr("disabled",false);
        //判断网页是否失效
        var isgo=IsGoLoginPage(data);
        if(isgo) {
            dialogTip("网页已失效请重新登录！","0");
            return;
        }
        if(PageType=="1"||PageType=="2") {
            //添加普通政策           
            if((AddNum+1)==MaxAddNum) {
                AddNum=0;//恢复初始值
                //结果处理  
                var strReArr=data.split('##');
                if(strReArr.length==3) {
                    //错误代码
                    var errCode=strReArr[0];
                    //错误描述
                    var errDes=strReArr[1];
                    //错误结果
                    var result=jQueryOne.trim(unescape(strReArr[2]));
                    if(errCode=="1") {//处理成功
                        var html='';
                        if(val_OpType=="1") {
                            html='是否返回列表:<ul class="ulClass"><li><label for="rbYes"><input type="radio" id="rbYes" value="1" name="IsGoList" />是</label></li><li><label for="rbNO"><input type="radio" id="rbNO"  value="2" checked="checked" name="IsGoList" />否</label></li></ul>';
                        }
                        //处理
                        showdialog(errDes+"<br>"+html);
                    } else {//处理失败
                        showdialog(errDes);
                    }
                }
            } else {
                //添加政策
                AddNum++;
                AddPolicy();
            }
        }
    },
    "text");
}
//--------------------------------------------
function CreateDiv(id) {
    var div=document.getElementById(id);
    if(div==null) {
        div=document.createElement("div");
        div.id=id;
        if(document.all) {
            document.body.appendChild(div);
        }
        else {
            document.insertBefore(div,document.body);
        }
    }
    return div;
}

function showApp(id,c,p) {
    var div=CreateDiv(id);
    jQueryOne("#"+id).html(c);
    jQueryOne("#"+id).dialog({
        title: '提示',
        bgiframe: true,
        height: 180,
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
function GetTravel(type) {
    var result="单程";
    if(type=="1") {
        result="单程";
    } else if(type=="2") {
        result="往返/单程";
    } else if(type=="3") {
        result="往返";
    } else if(type=="4") {
        result="中转联程";
    }
    return result;
}

function GetDycPolicy(num) {

    //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
    var PageType=jQueryOne("#Hid_PageType").val();
    //行程类型 1.单程，2.往返/单程，3.往返，4.中转联程
    var val_TravelType=jQueryOne("#TravelType").val();
    //出发城市三字码
    var val_FromCityCode=jQueryOne("#FromCityCode").val();
    //中转城市三字码
    var val_MiddleCityCode=jQueryOne("#MiddleCityCode").val();
    //到达城市三字码
    var val_ToCityCode=jQueryOne("#ToCityCode").val();
    //承运人二字代码  CA^中国国际航空公司^999
    var AirCode=jQueryOne.trim(jQueryOne('#Select_'+num).val()).split('^')[0];
    //政策类型  1=B2B，2=BSP，3=B2B/BSP
    var PolicyType=jQueryOne('#SelPolicyType_'+num).val();
    //下级分销返点
    var DownPoint=jQueryOne("#DownPoint_"+num).val();
    if(jQueryOne.isNaN(DownPoint)) {
        DownPoint=0;
    }

    //发送请求
    var url="../AJAX/GetHandler.ashx";
    var param={
        PageType: escape(PageType),
        FC: escape(val_FromCityCode),
        MC: escape(val_MiddleCityCode),
        TC: escape(val_ToCityCode),
        TravelType: escape(val_TravelType),
        PolicyType: escape(PolicyType),
        CarryCode: escape(AirCode),
        DownPoint: escape(DownPoint),

        OpName: escape("GetTopPoint"),
        num: Math.random(),
        currentuserid: jQueryOne("#currentuserid").val()
    };
    var GetTopPoint=function (data) {
        try {
            var strArr=unescape(data).split('#######');
            if(strArr.length==2&&strArr[1]!="") {
                var pMode=eval("("+strArr[1]+")");

                var html='<ul class="ulClass">';
                if(AirCode!='') {
                    html+='<li>航空公司二字码:'+AirCode+'</li>';
                }
                if(val_TravelType!=""&&val_TravelType!="0") {
                    html+='<li>行程类型:'+GetTravel(val_TravelType)+'</li>';
                }
                if(PolicyType!=""&&PolicyType!="0") {
                    html+='<li>政策类型:'+(PolicyType=="1"?"B2B":"BSP")+'</li>';
                }
                if(val_FromCityCode!='') {
                    html+='<li>出发城市三字码:'+val_FromCityCode+'</li>';
                }
                if(val_MiddleCityCode!='') {
                    html+='<li>中转城市三字码:'+val_MiddleCityCode+'</li>';
                }
                if(val_ToCityCode!='') {
                    html+='<li>到达城市三字码:'+val_ToCityCode+'</li>';
                }
                if(pMode!=null) {
                    html+='<li>最优政策返点:<b class="red">'+pMode._downpoint+'</b></li>';
                }
                else {
                    html+='<li>最优政策返点:没有获取到数据!</li>';
                }
                html+='</ul>';
                showApp("ShowPoint",html);
            } else {
                showApp("ShowPoint","没有获取到数据!!");
            }
        } catch(e) {
        }
    }
    //请求
    jQueryOne.post(url,param,GetTopPoint,"text");
}