//-------------公用函数--------------------------------------
//只用来显示提示信息对话框
function showMsgDg(t) {
    jQuery("#show").html(t);
    jQuery("#show").dialog({
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
                jQuery(this).dialog('close');
                //失败启用
                jQuery("#btnSub").show();
                showBgDiv(false); //隐藏遮罩层
            }
        }
    });
}
function showDD(t,patam) {
    jQuery("#show").html(t);
    jQuery("#show").dialog({
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
                jQuery(this).dialog('close');
                jQuery("#btnSub").attr("commit","1");
                jQuery("#btnSub").click();
                jQuery("#btnSub").attr("commit","0");
            },
            '取消': function () {
                jQuery(this).dialog('close');
            }
        }
    });
}

//显示和控制操作对话框
function showdg(t,param) {
    jQuery("#opshow").html(t);
    jQuery("#opshow").dialog({
        title: '提示',
        bgiframe: true,
        height: 180,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            OPPolicy(param,0);
        },
        buttons: {
            '确定': function () {
                jQuery(this).dialog('close');
                OPPolicy(param,1);
            }
        },
        '取消': function () {
            jQuery(this).dialog('close');
            OPPolicy(param,0);
        }
    });
    if(param!=null&&param.type!=null) {
        jQuery("#opshow").dialog("option","buttons",{
            '确定': function () {
                jQuery(this).dialog('close');
                OPPolicy(param,1);
            }
        });
    }
}
//对话框政策操作
function OPPolicy(param,type) {
    //禁用当前操作的按钮
    var obj=event.srcElement?event.srcElement:event.target;
    jQuery(obj)[0].disabled=true;

    //type 1确定 其他取消
    if(param==1) {
        //政策变动 是否继续预定 
        if(type!=1) {
            //取消
            ClearTr(true);
        }
    } else if(param.type!=null&&param.result!=null) {
        if(param.type=="1") {
            location.href="PayMent.aspx?"+param.result+"&currentuserid="+jQuery("#currentuserid").val();
        }
    }
}
//扩展新方法
String.prototype.NewReplace=function (sourceData,replaceData) {
    sourceData=sourceData.replace("(","\\(").replace(")","\\)");
    var reg=new RegExp(sourceData,"ig");
    var data=this.replace(reg,replaceData);
    return data;
}
//--------公用函数结束-------------------------------------------


//---成人政策---------------------------------------------------------------------------     
function LoadDiv(fg) {
    if(fg) {
        jQuery("#policyCon").hide();
        jQuery("#TempDiv").show();
    } else {
        jQuery("#policyCon").show();
        jQuery("#TempDiv").hide();
    }
}
//设置单击和鼠标滑动政策样式
function setItemStyle(num) {
    var trObj=jQuery('#tabPolicy tr[name="policy_'+num+'"]');
    //单击
    trObj.click(function (evt) {
        trObj.find("input[name='rbtnPolicy']").attr("checked",true);
    })
    //鼠标滑动
    .mouseover(function (evt) {
        trObj.attr("bgColor","#F5F5F5");
    }).mouseout(function (evt) {
        trObj.attr("bgColor","#ffffff");
    });
}
//构造html
function createHtml(num,model) {
    //构造
    jQuery('#tabPolicy tr:lt(3)').show();
    var tr=jQuery('#tabPolicy tr[name="policy_0"]');
    tr.removeClass("hide");
    tr.attr("style","");
    tr.show();
    var cloneTr=tr.clone(true);
    //清空数据
    cloneTr.find("tr").show();
    cloneTr.find("#tdSeatPrice_0").html("");
    cloneTr.find("#tdABFee_0").html("");
    cloneTr.find("#tdfandian_0").html("");
    cloneTr.find("#tdshifu_0").html("");
    cloneTr.find("#tdworktime_0").html("");
    cloneTr.find("#tdfpgq_0").html("");
    cloneTr.find("#tdpolicytype_0").html("");
    cloneTr.find("#beizhu_0").html("");
    cloneTr.find("#tdSeatPrice_0").attr("title","0");
    cloneTr.find("#tabPolicy tr[name='policy_0']").attr("title","");
    jQuery(cloneTr).show();
    if(num==0) {
        //默认选中
        cloneTr.find("#rbtn_0").attr("checked",true);
    }
    //----------------------------------------------
    var tr_html=jQuery("<div></div>").append(cloneTr).html();
    //替换id
    tr_html=tr_html.NewReplace("policy_0","policy_"+num).NewReplace("rbtn_0","rbtn_"+num).NewReplace("tdop_0","tdop_"+num).NewReplace("tdSeatPrice_0","tdSeatPrice_"+num);
    tr_html=tr_html.NewReplace("tdABFee_0","tdABFee_"+num).NewReplace("tdfandian_0","tdfandian_"+num).NewReplace("tdshifu_0","tdshifu_"+num).NewReplace("tdworktime_0","tdworktime_"+num);
    tr_html=tr_html.NewReplace("tdfpgq_0","tdfpgq_"+num).NewReplace("tdpolicytype_0","tdpolicytype_"+num).NewReplace("tdbeizhuHead_0","tdbeizhuHead_"+num).NewReplace("beizhu_0","beizhu_"+num);
    tr_html=tr_html.NewReplace("tdcpxl_0","tdcpxl_"+num).NewReplace("tdCHDcpxl_0","tdCHDcpxl_"+num).NewReplace("tdzhifutype_0","tdzhifutype_"+num).NewReplace("tdPMPrice_0","tdPMPrice_"+num);
    //    tr_html=tr_html.NewReplace("tdzhifutype_","tdzhifutype_"+num)
    //追加到后面
    if(num>0) {
        jQuery("#tabPolicy").append(tr_html);
        //设置样式
        setItemStyle(num);
    }
    //-----------------------------------------------------
    //添加的时候 如果是高返政策默认隐藏
    if(model._HighPolicyFlag=="1") {
        jQuery("#tabPolicy tr[name='policy_"+num+"']").hide();
        //0普通 1高返
        jQuery("#tabPolicy tr[name='policy_"+num+"']").attr("ptype","1");
    } else {
        jQuery("#tabPolicy tr[name='policy_"+num+"']").show();
        //0普通 1高返
        jQuery("#tabPolicy tr[name='policy_"+num+"']").attr("ptype","0");
    }
    //是否隐藏政策返点 0显示，1隐藏
    var IsHidePolicy=jQuery("#Hid_IsPolicy").val();
    //隐藏政策
    if(IsHidePolicy=="1") {
        jQuery("#thfandian_0").hide();
        jQuery("#tdfandian_"+num).hide();
    }
    //是否隐藏价格 0显示，1隐藏
    var IsHidePrice=IsHidePolicy; //jQuery("#hidShowPrice").val();
    //隐藏价格
    if(IsHidePrice=="1") {
        jQuery("#thfandian_0").hide();
        jQuery("#thshifu_0").hide();
        jQuery("#tdfandian_"+num).hide();
        jQuery("#tdshifu_"+num).hide();
    }
    //设置radio 的index
    jQuery("#rbtn_"+num).val(num);
    //jQuery("#rbtn_"+num).attr("title",num);
}
//清空 true清空所有 false清空部分
function ClearTr(isall) {
    jQuery('#tabPolicy tr:gt(2)').remove();
    if(isall) {
        jQuery('#tabPolicy tr:lt(3)').hide();
    }
}

//添加一行政策
function addTrPolicy(num,model) {
    //构造html            
    createHtml(num,model);
    //设置显示数据
    setModel(num,model);
}
//默认显示 0普通政策 1高返政策 。。。可扩展选项卡
function ShowPoliy(type) {

    var data=jQuery.trim(jQuery("#Hid_Data").val());

    var gf=jQuery("#tabPolicy tr[name*='policy_'][ptype='1']");
    var pt=jQuery("#tabPolicy tr[name*='policy_'][ptype='0']");
    if(type=="1") {
        if(data!="") {
            gf.show();
            pt.hide();
        }
        //jQuery("#btngf").attr("disabled",true);
        //jQuery("#btnpt").attr("disabled",false);
        gf.find("input[name='rbtnPolicy'][type='radio']").attr("checked",false);
        gf.find("input[name='rbtnPolicy'][type='radio']").eq(0).attr("checked",true);
    } else {
        if(data!="") {
            gf.hide();
            pt.show();
        }
        //jQuery("#btngf").attr("disabled",false);
        //jQuery("#btnpt").attr("disabled",true);
        pt.find("input[name='rbtnPolicy'][type='radio']").attr("checked",false);
        pt.find("input[name='rbtnPolicy'][type='radio']").eq(0).attr("checked",true);
    }

}
var TimeQi=null;
//定时显示
function fn_Time_Exec() {
    if(jQuery("#btnAgainGetPolicy").is(":hidden")) {
        jQuery("#btnAgainGetPolicy").show();
    }
}
function hideAll() {
    jQuery("#tabPolicy tr[name*='policy_']").hide();
    jQuery("#div_CHD").hide();
    jQuery("#div_INF").hide();
}
///获取政策 
function getAdultPolicy() {
    //匹配政策是否在落地运营商设置的时间内 1是 0否
    var IsInWorkTime=jQuery("#Hid_IsInWorkTime").val();
    if(IsInWorkTime=="0") {
        jQuery("#btnSub").attr("disabled",true);
        jQuery("#PolicyDiv").show();
        jQuery("#PolicyDiv").html('<p class="red" align="center" style="font-size:20px"><b>供应商已下班</b></p>');
        showMsgDg('该供应商已下班!');
    } else {
        var IsTest=0; //IsTest=1为测试
        //清空政策数据
        jQuery("#Hid_Data").val("");
        //禁用生成按钮
        jQuery("#btnSub").attr("disabled",true);
        jQuery("#btnAgainGetPolicy").hide();
        LoadDiv(true);
        //传递参数                     
        var CacheGUID=jQuery("#Hid_CacheGUID").val();

        //单程 FromCode ToCode
        //往返 FromCode ToCode
        //联程 FromCode  MiddleCode ToCode

        var FromCode=jQuery("#Hid_FromCode").val();
        var MiddleCode=jQuery("#Hid_MiddleCode").val();
        var ToCode=jQuery("#Hid_ToCode").val();

        var TravelType_Val=jQuery("#Hid_TravelType").val();
        var GroupId=jQuery("#Hid_GroupId").val();
        var OrderID=jQuery("#Hid_OrderID").val();
        var FromDate=jQuery("#Hid_FromDate").val();
        var ReturnDate=jQuery("#Hid_ToDate").val();

        //用于航空公司政策b2b
        var Pnr=jQuery.trim(jQuery("#Hid_Pnr").val());
        var BigPnr=jQuery.trim(jQuery("#Hid_BigPnr").val());
        var CarrayCode=jQuery.trim(jQuery("#Hid_CarrayCode").val());
        var Office=jQuery.trim(jQuery("#Hid_Office").val());
        var Space=jQuery.trim(jQuery("#Hid_Space").val());
        //是否中转联程
        var isUnite=false;
        if(TravelType_Val=="3") {
            isUnite=true;
        }
        //是否有婴儿
        var IsInF=jQuery("#Hid_HasINF").val()=="1"?true:false;
        //请求参数
        var Url="../Ajax/GetPolicyInfo.ashx";
        var Param={ StartCityNameCode: FromCode,MiddCityNameCode: MiddleCode,TargetCityNameCode: ToCode,
            FromDate: FromDate,ReturnDate: ReturnDate,
            TravelType: TravelType_Val,OrderID: OrderID,chaceNameByGUID: CacheGUID,GroupId: GroupId,IsOrGetPolicy: false,
            HavaChild: false,
            isUnite: isUnite,
            IsINF: IsInF,
            Pnr: Pnr,
            BigPnr: BigPnr,
            CarrayCode: CarrayCode,
            Space: Space,
            Office: Office,
            num: Math.random(),
            currentuserid: jQuery("#currentuserid").val()
        };
        if(IsTest==1) {
            Url="Confirmation.aspx";
            Param={ test: "1",num: Math.random(),currentuserid: jQuery("#currentuserid").val() };
        }
        //订单标志 0成人订单 1儿童订单 2成人和儿童两个订单
        var OrderFlag=parseInt(jQuery("#Hid_OrderFlag").val(),10);
        //返回数据进行处理
        function Fn_Handle(data) {
            if(OrderFlag<2) {
                LoadDiv(false);
                jQuery("#div_Adult").show();
                jQuery("#btnSub").attr("disabled",false);
            }
            try {
                if(data.indexOf('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">')<=0) {
                    if(jQuery.trim(data)!="") {
                        jQuery("#Hid_Data").val(escape(data));
                        if(OrderFlag>1) {
                            //还有儿童再请求一次
                            Param.HavaChild=true;
                            //是否儿童出成人票
                            var IsCHDETAdultTK=jQuery("#Hid_IsCHDETAdultTK").val();
                            Param.IsChdToAdultTK=IsCHDETAdultTK;
                            jQuery.post(Url,Param,function (data) {
                                //显示
                                LoadDiv(false);
                                jQuery("#div_Adult").show();
                                jQuery("#btnSub").attr("disabled",false);

                                var oldData=unescape(jQuery("#Hid_Data").val());
                                var adult=eval("("+oldData+")");
                                //政策列表
                                var newList=[];
                                for(var i=0;i<adult._OutPutPolicyList.length;i++) {
                                    newList.push(adult._OutPutPolicyList[i]);
                                }
                                data=eval("("+data+")");
                                //政策列表
                                var dataList=data._OutPutPolicyList;
                                //政策内部提示信息
                                var Msg=data._PolicyErrMsg;
                                if(dataList!=null&&dataList.length>0) {
                                    newList.push(dataList[0]);
                                    adult._OutPutPolicyList=newList;
                                    var newdata=JSON.stringify(adult);
                                    jQuery("#Hid_Data").val(escape(newdata));
                                }
                                ShowPolicy();
                            },"text");
                        } else {
                            //没有儿童
                            ShowPolicy();
                        }
                    } else {
                        //政策加载失败
                        showMsgDg('政策加载失败!');
                        //隐藏所有
                        hideAll();
                    }
                } else {
                    //请重新登录
                    showMsgDg('政策加载失败,请重新登录!');
                    //隐藏所有
                    hideAll();
                }
            } catch(e) {
                //政策加载异常
                showMsgDg('政策加载异常!');
                //隐藏所有
                hideAll();
            }
            jQuery("#btnAgainGetPolicy").show();
        };
        //清空列表
        ClearTr(true);
        if(OrderFlag=="1") {
            //只有儿童只请求一次
            Param.HavaChild=true;
        }
        //发送请求
        jQuery.post(Url,Param,Fn_Handle,"text");
        //定时显示重新加载按钮    
        if(TimeQi!=null) {
            window.clearTimeout(TimeQi);
        }
        TimeQi=window.setTimeout(fn_Time_Exec,2*60*1000);
    } //
}
var IsExistAdultPolicy=false; //是否有成人政策
//政策列表
var dataList=null;
function ShowPolicy() {
    //---------start--------------------------------------------------
    var data=unescape(jQuery("#Hid_Data").val());
    data=eval("("+data+")");
    //政策列表
    dataList=data._OutPutPolicyList;
    //政策内部提示信息
    var Msg=data._PolicyErrMsg;
    var errMsg="";
    if(dataList!=null&&dataList.length>0) {
        //-------成人政策-------------------
        var childList=[];
        var AdultDefaultList=[];
        var INFtoModel=null;
        for(var i=0;i<dataList.length;i++) {
            var model=dataList[i];
            model.index=i;
            //添加界面政策数据显示
            if(model._DefaultType!="2") {
                if(INFtoModel==null) {
                    INFtoModel=model; //婴儿所需要的成人实体信息
                }
                IsExistAdultPolicy=true;
                addTrPolicy(i,model);
                if(model.err!=undefined) {
                    errMsg=model.err;
                }
            }
            //成人默认政策
            else if(model._DefaultType=="1") {
                AdultDefaultList.push(model);
            }
            else if(model._DefaultType=="2") {
                //儿童默认政策
                childList.push(model);
            }
        }
        if(AdultDefaultList!=null&&AdultDefaultList.length>0) {
            IsExistAdultPolicy=true;
            for(var i=0;i<AdultDefaultList.length;i++) {
                addTrPolicy(AdultDefaultList[i].index,AdultDefaultList[i]);
                if(AdultDefaultList[i].err!=undefined) {
                    errMsg=AdultDefaultList[i].err;
                }
            }
        }
        if(jQuery.trim(errMsg)=="") {
            ShowPoliy("0");
            //没有成人政策时
            if(!IsExistAdultPolicy) {
                jQuery("#tabPolicy").hide();
            } else {
                jQuery("#tabPolicy").show();
            }
        } else {
            showdg(errMsg,1);
        }
        //----------------------------
        //儿童政策
        if(childList.length>0) {
            var OrderFlag=parseInt(jQuery("#Hid_OrderFlag").val(),10);
            if(OrderFlag>0) {
                childPolicy(childList);
                //显示儿童政策
                jQuery("#div_CHD").show();
                jQuery("#tabChildPolicy input[name='rCHD']").eq(0).attr("checked",true);
            }
        }
        var IsInF=jQuery("#Hid_HasINF").val();
        //婴儿政策
        if(IsInF=="1"&&INFtoModel!=null) {
            InfPolicy(INFtoModel);
            //显示婴儿政策
            jQuery("#div_INF").show();
            jQuery("#tabINFPolicy input[name='rINF']").eq(0).attr("checked",true);
        }
    } else {
        if(jQuery.trim(Msg)!="") {
            //政策加载失败 提示消息 Msg
            showMsgDg(Msg);
        } else {
            showMsgDg('没有获取到政策数据!');
        }
    }
}
//获取出票效率  0-B2B|1-BSP|2-517|3-百拓|4-8000|5-今日|6-票盟|7-51book|8-共享|9-易行
function GetCPXL(CPXL,index) {
    var CPTime='';
    if(CPXL!="") {
        var M_OneArr=CPXL.split('|');
        for(var i=0;i<M_OneArr.length;i++) {
            if(index==i) {
                CPTime=jQuery.trim(M_OneArr[i])
                break;
            }
        }
    }
    if(CPTime!=''&&!jQuery.isNaN(CPTime)) {
        CPTime=ShowLeay(CPTime);
    }
    return CPTime;
}
//显示出票速度
function ShowLeay(CPTime_Minute) {
    var mmsecond=parseFloat(CPTime_Minute,10)*60*1000;
    var result=[60,60,24];
    var flag;
    var result_re="";
    mmsecond=Math.floor(mmsecond/1000);
    //变成秒单位,但是不操作
    var i;
    //下面这个for计算时分秒
    for(i=0;i<3;i++) {
        flag=Math.floor(mmsecond%result[i]);
        mmsecond=Math.floor(mmsecond/result[i]);
        if(flag<10) {
            result_re="0"+flag+":"+result_re;
        } else {
            result_re=flag+":"+result_re;
        }
    }
    //去掉最后的一个冒号
    result_re=result_re.substring(0,result_re.length-1);
    //下面计算年月日
    var year,month,day;
    var everyMonth=[31,28,31,30,31,30,31,31,30,31,30,31];
    //计算年
    flag=Math.floor(mmsecond/365);
    year=1970-0+flag;
    mmsecond=Math.floor(mmsecond%365);
    //计算月和日
    for(i=0;i<12;i++) {
        //判断闰月
        if(((year%4==0)&&(year%100!=0))||(year%400==0)) {
            if(mmsecond==59) {
                month="02";
                day="29";
                break;
            }
        }
        if(mmsecond>everyMonth[i]) {
            mmsecond-=everyMonth[i];
        } else {
            month=i+1;
            day=mmsecond;
            month=month>10?month:"0"+month;
            day=day>10?day:"0"+day;
        }
    }
    //拼起来
    //result_re=year+"-"+month+"-"+day+" "+result_re;   
    var strArr=result_re.split(':');
    var _hour='',_minute='',_senond='';
    if(strArr.length==3) {
        _hour=strArr[0];
        _minute=strArr[1];
        _senond=strArr[2];
    }
    result_re=(day!=''&&parseInt(day,10)!=0?day+'天':'')+(_hour!=''&&parseInt(_hour,10)!=0?_hour+'时':'')+(_minute!=''&&parseInt(_minute,10)!=0?_minute+'分':'')+(_senond!=''&&parseInt(_senond,10)!=0?_senond+'秒':'');
    return result_re;
}
//获取支付方式的图片显示
function getPayTypeImg(payType) {
    var imgArr=[];//支付方式图片路径
    //0-网银|1-支付宝|2-快钱|3-汇付|4-财付通|5-账户支付|6-收银  
    var payArr=payType.split('|');
    for(var i=0;i<payArr.length;i++) {
        if(payArr[i]!="") {
            //if(payArr[i]=="0") {
            //    imgArr.push('<img src="../img/pay/wangying_0.gif" alt="网银" title="网银" />');
            //}
            if(payArr[i]=="1") {
                imgArr.push('<img src="../img/pay/zhifubao_1.gif" alt="支付宝" title="支付宝" />');
            }
            else if(payArr[i]=="2") {
                imgArr.push('<img src="../img/pay/kuaiqian_2.gif" alt="快钱" title="快钱"/>');
            }
            else if(payArr[i]=="3") {
                imgArr.push('<img src="../img/pay/huifu_3.gif" alt="汇付" title="汇付"/>');
            }
            else if(payArr[i]=="4") {
                imgArr.push('<img src="../img/pay/caifutong_4.gif" alt="财付通" title="财付通" />');
            }
            else if(payArr[i]=="5") {
                imgArr.push('<img src="../img/pay/zhanghu_5.gif" alt="账户支付" title="账户支付" />');
            }
            //else if(payArr[i]=="6") {
            //    imgArr.push('<img src="../img/pay/shouying_6.gif" alt="收银"  title="收银" />');
            //}
        }
    }
    var strImg=imgArr.join("");
    var newArr=[];
    if(imgArr.length>4) {
        newArr=imgArr.slice(0,4);
        imgArr=imgArr.slice(4);
        strImg=newArr.join("")+"<br />"+imgArr.join("");
    }
    return strImg;
}
//设置指定tr政策数据值
function setModel(num,model) {
    try {

        var XSfee=model._SeatPrice; //舱位价
        var ABfee=model._ABFare; //机建费
        var Fulefee=model._RQFare; //燃油
        //票面价
        var PMFare=parseFloat(model._SeatPrice,10)+parseFloat(model._ABFare,10)+parseFloat(model._RQFare,10);
        //最终政策返点
        var FxPolicy=model._ReturnPoint;
        //是否为高返政策1是 0否 默认0
        var HighPolicyFlag=model._HighPolicyFlag;
        //政策种类
        var PolicyKind=model._PolicyKind;
        //政策后返
        var LaterPoint=jQuery.trim(model._LaterPoint)==""?"0":jQuery.trim(model._LaterPoint);
        //后返图片显示字符串 星星 月亮 太阳 
        var LaterPoint_img=GetString(LaterPoint);
        //1开启后返 0关闭后返
        var IsHouFanOpen=jQuery("#Hid_IsHouFanOpen").val();
        if(IsHouFanOpen=="0") {
            LaterPoint_img="";
        }
        //政策来源
        var PolicySource=parseInt((jQuery.trim(model._PolicySource)==""?"0":jQuery.trim(model._PolicySource)),10);
        //pat价格数目
        var patCount=jQuery("#Hid_AdultPriceCount").val();
        //自动出票方式 手动(0或者null空)， 半自动1， 全自动2
        var AutoPrintFlag=model._AutoPrintFlag;
        //政策备注
        var strPolicyRemark=jQuery.trim(model._PolicyRemark);
        var strTitlePolicyRemark=strPolicyRemark;
        var strSeatPrice=XSfee; //舱位价
        //显示优惠特价
        if(num>=0&&patCount>1) {
            strSeatPrice=strSeatPrice+'<span style="color: Red;">优惠特价</span>'
        }
        var strJJRQ=ABfee+"/"+Fulefee; //基建燃油
        //佣金
        var strYJ=model._PolicyYongJin;
        //现返
        var strXF=model._PolicyReturnMoney;
        //实付金额
        var strSFPrice=model._PolicyShiFuMoney;
        //上班时间
        var workTime=model._WorkTime;
        //废票改签时间
        var FPGQTime=model._FPGQTime;
        //政策类型
        var strPolicyType="B2B";
        if(model._PolicyType=="1") {
            strPolicyType="B2B";
        } else if(model._PolicyType=="2") {
            strPolicyType="BSP";
        }
        else if(model._PolicyType=="3") {
            strPolicyType="B2B/BSP";
        }       
        //自动出票
        if(AutoPrintFlag=="2") {
            if(strPolicyRemark.length>55) {
                strPolicyRemark=strPolicyRemark.substring(0,55);
            }
            strPolicyRemark+="<span style='color:red;font-weight:bold;'>自动出票</span>";
            strTitlePolicyRemark+="自动出票";
        }

        //设置舱位价显示
        jQuery("#tdSeatPrice_"+num).html(strSeatPrice);
        //政策来源
        jQuery("#tdSeatPrice_"+num).attr("title",PolicySource);

        //设置基建燃油
        jQuery("#tdABFee_"+num).html(strJJRQ);
        //设置票面价
        jQuery("#tdPMPrice_"+num).html(PMFare);

        //返点佣金 还有现返
        //var strPolicy_XF_YJ=FxPolicy+"%"+(strXF>0?"+"+strXF+"/":"/")+strYJ+"<br />"+LaterPoint_img;
        if(parseFloat(strXF)==0) {
            strXF="";
        }
        if(parseFloat(strXF)>0) {
            strXF="+"+strXF;
        }
        var strPolicy_XF_YJ=FxPolicy+"%"+strXF+"/"+strYJ+"<br />"+LaterPoint_img;
        jQuery("#tdfandian_"+num).html(strPolicy_XF_YJ);
        //实付金额
        jQuery("#tdshifu_"+num).html(strSFPrice);
        //政策种类
        jQuery("#tdshifu_"+num).attr("title",PolicyKind);
        //上班时间
        jQuery("#tdworktime_"+num).html(workTime);
        //废票改签时间
        jQuery("#tdfpgq_"+num).html(FPGQTime);
        if(PolicySource=="1"||PolicySource=="2") {
            strPolicyType=strPolicyType+"(本地)";
        }
        //政策类型 
        jQuery("#tdpolicytype_"+num).html(strPolicyType);
        
        jQuery("#tdpolicytype_"+num).attr("title",model._CarryCode+"|"+model._PolicyOffice+"|"+(model._PolicyId.indexOf("b2bpolicy")!= -1?"1":"0"));
        //政策备注                
        jQuery("#tabPolicy tr[name='policy_"+num+"']").attr("title",strTitlePolicyRemark);
        jQuery("#beizhu_"+num).html((HighPolicyFlag=="1"?"<span style='color:red;font-weight:bold;'>高返</span>":"")+strPolicyRemark);

        //是否隐藏政策返点 0显示，1隐藏
        var IsHidePolicy=jQuery("#Hid_IsPolicy").val();
        //隐藏政策
        if(IsHidePolicy=="1") {
            jQuery("#tdfandian_"+num).hide();
            jQuery("#thfandian_0").hide();
        }
        //是否隐藏价格 0显示，1隐藏 暂时与隐藏政策返点
        var IsHidePrice=IsHidePolicy; //jQuery("#hidShowPrice").val();
        //隐藏价格
        if(IsHidePrice=="1") {

            jQuery("#tdshifu_"+num).hide();
            jQuery("#thshifu_0").hide();

            jQuery("#thpmfare_"+num).hide();
            jQuery("#tdPMPrice_0").hide(); //票面价隐藏

            jQuery("#tdfandian_"+num).hide();
            jQuery("#thfandian_0").hide();
        }

        //出票效率 0-B2B|1-BSP|2-517|3-百拓|4-8000|5-今日|6-票盟|7-51book|8-共享|9-易行
        var CPXL=model._chuPiaoShiJian;
        var CpTime=GetCPXL(CPXL,PolicySource-1);
        if(CpTime!="") {
            if(AutoPrintFlag=="1") {
                CpTime+="<br /><span style='color:red;font-weight:bold;'>半自动出票</span>";
            }
            else if(AutoPrintFlag=="2") {
                CpTime+="<br /><span style='color:red;font-weight:bold;'>全自动出票</span>";
            }
        }
        jQuery("#tdcpxl_"+num).html(CpTime);

        //支付方式
        var payType=model._PayType;
        var payImg=getPayTypeImg(payType);
        jQuery("#tdzhifutype_"+num).html(payImg);
    } catch(e) {
        alter(e.message);
    }
}
//---成人政策结束---------------------------------------------------------------------------

//---儿童政策开始---------------------------------------------------------------------------
function childPolicy(list) {
    for(var i=0;i<list.length;i++) {
        //显示儿童政策
        childShowHTML(list[i]);
    }
}
//显示儿童政策
function childShowHTML(model) {
    var XSfee=model._SeatPrice; //舱位价
    var ABfee=model._ABFare; //机建费
    var Fulefee=model._RQFare; //燃油
    var PMFare=parseFloat(model._SeatPrice,10)+parseFloat(model._ABFare,10)+parseFloat(model._RQFare,10); //票面价
    var childPolicy=model._ReturnPoint; //儿童默认政策
    var childLaterPoint=model._LaterPoint; //儿童政策后返
    //儿童政策后返图片显示
    var childLaterPoint_img=GetString(childLaterPoint);
    //1开启后返 0关闭后返
    var IsHouFanOpen=jQuery("#Hid_IsHouFanOpen").val();
    if(IsHouFanOpen=="0") {
        childLaterPoint_img="";
    }

    var childYJ=model._PolicyYongJin; //儿童佣金
    var childShiFu=model._PolicyShiFuMoney; //儿童实付金额
    //上班时间
    var worktime=model._WorkTime;
    //废票改签时间
    var fpgqtime=model._FPGQTime;


    //政策类型
    var strPolicyType="B2B";
    if(model._PolicyType=="1") {
        strPolicyType="B2B";
    } else if(model._PolicyType=="2") {
        strPolicyType="BSP";
    }
    else if(model._PolicyType=="3") {
        strPolicyType="B2B/BSP";
    }

    if(model._PolicySource=="1"||model._PolicySource=="2") {
        strPolicyType=strPolicyType+"(本地)";
    }

    //是否隐藏政策返点 0显示，1隐藏
    var IsHidePolicy=jQuery("#Hid_IsPolicy").val();
    //隐藏政策
    if(IsHidePolicy=="1") {
        jQuery("#thCHDfandian").hide();
        jQuery("#tdCHDfandian_0").hide();
    }
    //是否隐藏价格 0显示，1隐藏 暂时与隐藏政策返点一样
    var IsHidePrice=IsHidePolicy; //jQuery("#hidShowPrice").val();    
    //隐藏价格
    if(IsHidePrice=="1") {
        jQuery("#thCHDfandian").hide();
        jQuery("#thCHDshifu").hide();

        jQuery("#tdCHDfandian_0").hide();
        jQuery("#tdCHDshifu_0").hide();
        jQuery("#tdCHDPMPrice_0").hide(); //票面价隐藏
    }
    //赋值
    jQuery("#tdCHDSeatPrice_0").html(XSfee);
    jQuery("#tdCHDPMPrice_0").html(PMFare);  //票面价          
    jQuery("#tdCHDABfare_0").html(ABfee+'/'+Fulefee);
    jQuery("#tdCHDfandian_0").html(childPolicy+'%/'+childYJ);
    jQuery("#tdCHDshifu_0").html(childShiFu);
    jQuery("#tdCHDworktime_0").html(worktime);
    jQuery("#tdCHDdpgqtime_0").html(fpgqtime);
    jQuery("#tdCHDPolicyType_0").html(strPolicyType);
    //儿童选择的索引
    jQuery("#Hid_SelChildIndex").val(model.index);

    //出票效率 0-B2B|1-BSP|2-517|3-百拓|4-8000|5-今日|6-票盟|7-51book|8-共享|9-易行
    var CPXL=model._chuPiaoShiJian;
    var CpTime=GetCPXL(CPXL,model._PolicySource-1);
    jQuery("#tdCHDcpxl_0").html(CpTime);

    //支付方式
    var payType=model._PayType;
    var payImg=getPayTypeImg(payType);
    jQuery("#tdCHDzffs_0").html(payImg);
}
//---儿童政策结束---------------------------------------------------------------------------

//-----婴儿政策----------------------------------------------------
function InfPolicy(model) {
    var XSfee=0.00; //舱位价
    var ABfee=0.00; //机建费
    var Fulefee=0.00; //燃油
    var PMFare=0.00; //票面价
    //价格字符串
    var strPrice=jQuery.trim(jQuery("#Hid_INFGDPrice").val());
    var DStrPrice=strPrice.split('@')[0]; //低价格
    var GStrPrice=strPrice.split('@')[1]; //高价格                              
    XSfee=parseFloat(GStrPrice.split('|')[0],10);
    ABfee=parseFloat(GStrPrice.split('|')[1],10);
    Fulefee=parseFloat(GStrPrice.split('|')[2],10);

    PMFare=parseFloat(XSfee,10)+parseFloat(ABfee,10)+parseFloat(Fulefee,10); //票面价

    //是否隐藏政策返点 0显示，1隐藏
    var IsHidePolicy=jQuery("#Hid_IsPolicy").val();
    //隐藏政策
    if(IsHidePolicy=="1") {
        jQuery("#thINFfandian").hide();
        jQuery("#tdINFfandian").hide();
    }
    //是否隐藏价格 0显示，1隐藏 暂时与隐藏政策返点一样
    var IsHidePrice=IsHidePolicy; //jQuery("#hidShowPrice").val();    
    //隐藏价格
    if(IsHidePrice=="1") {
        jQuery("#thINFfandian").hide(); //返点佣金
        jQuery("#tdINFPMPrice").hide(); //票面价
        jQuery("#thINFSF").hide(); //实付金额
        jQuery("#thINFfandian").hide();
        jQuery("#tdINFshifu").hide();
    }
    var shifu=XSfee+ABfee+Fulefee;
    jQuery("#tdINFSeatPrice").html(XSfee);
    jQuery("#tdINFPMPrice").html(PMFare); //票面价
    jQuery("#tdINFABFare").html(ABfee+"/"+Fulefee);
    jQuery("#tdINFworktime").html(model._WorkTime);
    jQuery("#tdINFfpgqtime").html(model._FPGQTime);
    jQuery("#tdINFPolicyType").html("B2B");
    jQuery("#tdINFfandian").html("0/0");
    jQuery("#tdINFshifu").html(shifu);
}

//---------------------------------------------------------
//数据验证和更改隐藏域值
function DataValidate(obj,param) {
    var IsVate=false;
    var index= -1;
    jQuery("input[name='rbtnPolicy'][id*='rbtn_']").each(function (ins,obj) {
        if(jQuery(obj).is(":checked")) {
            index=ins;
            return false;
        }
    });
    var data=jQuery.trim(jQuery("#Hid_Data").val());
    if(data=="") {
        showMsgDg("请选择政策数据!！");
    } else {
        if(index== -1) {
            showMsgDg("请选择政策数据！");
        } else {
            jQuery("#Hid_SelIndex").val(index);
            IsVate=true;
        }
    }
    var commit=jQuery("#btnSub").attr("commit");
    if(dataList!=null&&(commit==null||commit=="0")) {
        var len=dataList.length;
        if(index>=0&&index<len) {
            var model=dataList[index];
            if(model!=null) {
                //如果是共享
                if(model._PolicySource=="9") {
                    var zhanghu=jQuery("#Hid_zhanghu").val();
                    var shouying=jQuery("#Hid_shouying").val();
                    var msg="";
                    if(zhanghu=="1"||shouying=="1") {
                        msg="此政策不支持账户支付、收银支付";
                    }
                    if(msg!="") {
                        showDD(msg,{ opType: 1 });
                        IsVate=false;
                    }
                }
            }
        }
    }

    if(IsVate) {
        //标识是否单击确认按钮
        jQuery("#Hid_Isbtn").val("1");
        jQuery("#Hid_ViewState").val(escape(jQuery("#PolicyDiv").html()));
        jQuery(obj).hide();
        showBgDiv(true); //显示遮罩层
    }

    return IsVate;
}
//刷新页面
function Refresh() {
    jQuery("#Hid_Isbtn").val("0");
    location.reload(true);
}
//显示或者隐藏div val=true显示遮罩层 false隐藏
function showBgDiv(val) {
    if(val) {
        jQuery("#overlay").show();
        jQuery("#loading").show();
    } else {
        jQuery("#overlay").hide();
        jQuery("#loading").hide();
    }
}
//显示隐藏机型
function showHideJX(val) {
    if(val=="2"||val=="3") {
        jQuery("#JX_Header").hide();
        jQuery("#JX_Body").hide()
    } else {
        jQuery("#JX_Header").show();
        jQuery("#JX_Body").show()
    }
}
//----选项卡单击事件样式开始---------------------------------------------------------
function MenuClick(e) {
    var e=window.event?window.event.srcElement:e.target;
    if(/a/i.test(e.tagName)) {
        e.parentNode.className=e.className;
        e.parentNode.nextSibling.style.cssText='border-top:none';
        e.blur();
    }
}
//切换选项卡
function switchTab(obj,type) {
    setSelStyle(obj);
    ShowPoliy(type);
    return false;
}
//设置选中选项卡样式
function setSelStyle(e) {
    jQuery("#header a").removeClass("nosel");
    jQuery("#header a").addClass("nosel");
    jQuery(e).removeClass("nosel").addClass("sel");
}
//后返范围[0-10]图片显示 太阳表示1个点 月亮表示0.5个点 星星表示0.1个点
function GetString(val) {
    var month=0;
    var star=0;
    val=val+'';
    if(val.indexOf(".")!= -1) {
        var temp=val;
        val=val.substr(0,val.indexOf("."));
        var shi=parseInt(temp.substr(temp.indexOf(".")+1,1),10);
        month=parseInt(shi/5,10);
        star=parseInt(shi%5,10);
    }
    var sun=parseInt(val,10);
    var LevelArr=[];
    for(var i=0;i<parseInt(sun,10);i++) {
        LevelArr.push('<img src=\"../img/Level/sun.jpg\" />');
    }
    for(var i=0;i<parseInt(month,10);i++) {
        LevelArr.push('<img src=\"../img/Level/month.jpg\" />');
    }
    for(var i=0;i<parseInt(star,10);i++) {
        LevelArr.push('<img src=\"../img/Level/star.jpg\" />');
    }
    return LevelArr.join("");
}
//-------选项卡单击事件样式结束------------------------------------------------------

//页面加载
jQuery(function () {

    jQuery("#PolicyDiv input[id*='rbtn_']").click(function () {
        if(jQuery(this).is(":checked")) {
            var index=jQuery(this).val();
            jQuery("#Hid_SelIndex").val(index);
        }
    });
    //设置默认政策项样式
    setItemStyle(0);
    //隐藏机型
    var OrderSource=jQuery("#Hid_OrderSourceType").val();
    //显示隐藏机型
    showHideJX(OrderSource);

    var Isbtn=jQuery("#Hid_Isbtn").val();
    if(Isbtn=="0") {
        //获取政策
        getAdultPolicy();
    } else {
        jQuery("#PolicyDiv").html(unescape(jQuery("#Hid_ViewState").val()));
    }

});