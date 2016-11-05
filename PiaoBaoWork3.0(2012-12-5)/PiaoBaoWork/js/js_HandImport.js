var maxPasNum=50;//最大乘机人数
var maxskyNum=9;//最大航段数
var pasArr=[];//乘机人序号
var skyArr=[];//航段序号
//对话框包含处理
function showdialog(t,f) {
    jQuery("select").hide();
    jQuery("#dialog").html(t);
    jQuery("#dialog").dialog({
        title: '提示',
        bgiframe: true,
        height: 180,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            jQuery("select").show();
        },
        buttons: {
            '确定': function (evt) {
                jQuery(this).dialog('close');
                var target=evt.srcElement?evt.srcElement:evt.target;
                jQuery(target).attr("disabled",true);

                //代码
                if(f!=null) {
                    if(f=="go") {
                        location.href='../Order/OrderList.aspx?currentuserid='+jQuery("#currentuserid").val();
                    }
                }
            }
        }
    });
}
//跳到指定Url
function GoUrl(url) {
    location.href=url;
    return false;
}

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
function GetDate(strdate) {
    var year="1900";
    var month="01";
    var day="01";
    var hour="00";
    var min="00";
    var sec="00";
    if(strdate.indexOf('-')!= -1) {
        var sArr=strdate.split('-');
        if(sArr.length>=3) {
            year=sArr[0];
            month=sArr[1];
            day=sArr[2].split(' ')[0];
            if(sArr[2].indexOf(":")!= -1) {
                var s2Arr=jQuery.trim(sArr[2].split(' ')[1]).split(':');
                if(s2Arr.length>=1) {
                    hour=s2Arr[0]
                }
                if(s2Arr.length>=2) {

                    min=s2Arr[1];
                }
                if(s2Arr.length>=3) {
                    sec=s2Arr[2];
                }
            }
        }
    }
    return new Date(year,month-1,day,hour,min,sec);
}
//日期已字符串形式显示
function GetStrDate(date,fg) {
    var d1="";
    if(fg==0) {//yyyy-MM-dd
        d1=(date.getFullYear()+"-"+(date.getMonth()+1).toString().padLeft(2,'0')+"-"+date.getDate().toString().padLeft(2,'0'));
    } else if(fg==1)//yyyy-MM-dd HH
    {
        d1=(date.getFullYear()+"-"+(date.getMonth()+1).toString().padLeft(2,'0')+"-"+date.getDate().toString().padLeft(2,'0')+" "+date.getHours().toString().padLeft(2,'0'));
    }
    else if(fg==2)//yyyy-MM-dd HH:mm
    {
        d1=(date.getFullYear()+"-"+(date.getMonth()+1).toString().padLeft(2,'0')+"-"+date.getDate().toString().padLeft(2,'0')+" "+date.getHours().toString().padLeft(2,'0')+":"+date.getMinutes().toString().padLeft(2,'0'));
    } else if(fg==3)//yyyy-MM-dd HH:mm:ss
    {
        d1=(date.getFullYear()+"-"+(date.getMonth()+1).toString().padLeft(2,'0')+"-"+date.getDate().toString().padLeft(2,'0')+" "+date.getHours().toString().padLeft(2,'0')+":"+date.getMinutes().toString().padLeft(2,'0')+":"+date.getSeconds().toString().padLeft(2,'0'));
    }
    return d1;
}
//将日期转换为json的时间格式
function GetJSONDate(date) {
    return "/Date("+Date.UTC(date.getFullYear(),date.getMonth(),date.getDate(),date.getHours(),date.getMinutes(),date.getSeconds())+")/";
}
//初始化全局数组
function initArr(Arr,num) {
    for(var i=1;i<=num;i++) {
        eval("var obj={_"+i+":0}");
        Arr.push(obj);
    }
    return Arr;
}
//重置序号状态
function resetFg(arr,val) {
    for(var i=0;i<arr.length;i++) {
        for(var m in arr[i]) {
            arr[i][m]=val;
        }
    }
}
//设置序号状态
function setFg(arr,key,val) {
    for(var i=0;i<arr.length;i++) {
        for(var j in arr[i]) {
            if(j=="_"+key) {
                arr[i][j]=val;
            }
        }
    }
}
//获取最小没有使用的序号
function getMinFg(arr) {
    var index="0";
    var istrue=false;
    for(var i=0;i<arr.length;i++) {
        if(istrue) {
            break;
        }
        for(var key in arr[i]) {
            if(arr[i][key]=="0") {
                index=key.replace("_","");
                istrue=true;
                break;
            }
        }
    }
    return index;
}
//--------------------------------------------------------
//添加一行
function addGroup(evt,name) {
    if(evt!=null) {
        var target=evt.srcElement?evt.srcElement:evt.target;
        jQuery(target).attr("disabled",true);
    }

    var num=0;
    //模板
    var trHtml=jQuery("<div></div>").append(jQuery("#tab_"+name+" thead tr[id='tr"+name+"_0']").clone(true)).html();
    var trCnt=jQuery("#tab_"+name+" tbody tr[id='tr"+name+"']").length;
    //maxPasNum
    //maxskyNum
    if(name=="pas") {
        if(trCnt>=maxPasNum) {
            showdialog("乘客数目已超过最大范围,不能继续添加了！");
            return false;
        }
        //获取可用序号从1开始
        num=getMinFg(pasArr);
        //标记为已使用
        setFg(pasArr,num,"1");
    } else if(name=="sky") {
        if(trCnt>=maxskyNum) {
            showdialog("航段数目已超过最大范围,不能继续添加了！");
            return false;
        }
        //获取可用序号从1开始
        num=getMinFg(skyArr);
        //标记为已使用
        setFg(skyArr,num,"1");
    }
    //操作内容
    var opDiv='<div id="'+name+'_opdiv_'+num+'"><a onclick="addGroup(event,\''+name+'\');return false;"  href="#">添加</a><br /><a href="#" onclick="return removeGroup(event,\''+name+'\','+num+')" >删除</a></div>';
    //设置操作内容HTML
    trHtml="<tr id='tr"+name+"_"+num+"'>"+jQuery("<tr>"+trHtml+"</tr>").find("td:last").html(opDiv).parent().html()+"</tr>";
    //替换id
    trHtml=trHtml.NewReplace("_0","_"+num).NewReplace("txtSetSel(this,'ddlFromCity',0)","txtSetSel(this,'ddlFromCity',"+num+")").NewReplace("ddlSetText(this,'txtFromCity',0)","ddlSetText(this,'txtFromCity',"+num+")");
    trHtml=trHtml.NewReplace("txtSetSel(this,'ddlToCity',0)","txtSetSel(this,'ddlToCity',"+num+")").NewReplace("ddlSetText(this,'txtToCity',0)","ddlSetText(this,'txtToCity',"+num+")");
    //添加节点
    jQuery("#tab_"+name+" tbody").append(trHtml);
    //设置初始值
    //setModel(num,name,null);

    //注册事件
    //航段
    if(name=="sky") {
        jQuery("#tab_"+name+" tbody input[id*="+name+"_yfareprice_]").blur(NumVate);
        jQuery("#tab_"+name+" tbody input[id*="+name+"_spaceprice_]").blur(NumVate);
        jQuery("#tab_"+name+" tbody input[id*="+name+"_abfee_]").blur(NumVate);
        jQuery("#tab_"+name+" tbody input[id*="+name+"_rqfee_]").blur(NumVate);
        jQuery("#tab_"+name+" tbody input[id*="+name+"_discount_]").blur(NumVate);

        jQuery("#tab_sky tbody input[id*=sky_carrycode_]").blur(require);
        jQuery("#tab_sky tbody input[id*=sky_flight_]").blur(require);

        jQuery("#tab_sky tbody input[id*=sky_flystartdate_]").blur(require);
        jQuery("#tab_sky tbody input[id*=sky_flyenddate_]").blur(require);
        jQuery("#tab_sky tbody input[id*=sky_space_]").blur(require);
    } else if(name=="pas") {
        //乘机人
        jQuery("#tab_"+name+" tbody input[id*="+name+"_seatprice_]").blur(NumVate);
        jQuery("#tab_"+name+" tbody input[id*="+name+"_abprice_]").blur(NumVate);
        jQuery("#tab_"+name+" tbody input[id*="+name+"_rqprice_]").blur(NumVate);

        jQuery("#tab_pas tbody input[id*=pas_pasname_]").blur(require);
        jQuery("#tab_pas tbody input[id*=pas_cardnum_]").blur(require);
        jQuery("#tab_pas tbody input[id*=txtBirday_]").blur(require);
        //jQuery("#tab_pas tbody input[id*=pas_phone_]").blur(require);手机号码

        jQuery("#tab_pas tbody select[id*=pas_cardtype_]").change(CardChange);
    }

    if(evt!=null) {
        var target=evt.srcElement?evt.srcElement:evt.target;
        jQuery(target).attr("disabled",false);
    }
    return num;
}

//移除一行
function removeGroup(evt,name,num) {
    if(num!=null) {
        jQuery("#tab_"+name+" tbody tr[id='tr"+name+"_"+num+"']").remove();
    } else {
        var trCount=jQuery("#tab_"+name+" tbody tr").length;
        //从后往前删除
        var lastTr=jQuery("#tab_"+name+" tbody tr:last");
        if(lastTr.length>0) {
            num=lastTr.attr("id").NewReplace("tr"+name+"_","");
            lastTr.remove();
        } else {
            showdialog("没有数据可以删除！");
            return false;
        }
    }
    //标记为没有使用
    if(name=="pas") {
        setFg(pasArr,num,"0");
    } else if(name=="sky") {
        setFg(skyArr,num,"0");
    }
    return false;
}

//城市选择
function ddlSetText(ddlObj,flag,num) {
    var ddlVal=jQuery.trim(jQuery(ddlObj).val()).split('-')[0].toUpperCase();
    jQuery("#"+flag+"_"+num).val(ddlVal);
}
function txtSetSel(txtObj,flag,num) {
    var txtVal=jQuery(txtObj).val().toUpperCase();
    if(txtVal!="") {
        jQuery("#"+flag+"_"+num+" option[value*='"+txtVal+"']").attr("selected",true);
    } else {
        jQuery("#"+flag+"_"+num+" option").eq(0).attr("selected",true);
    }
}
//设置数据
function setModel(num,name,model) {
    if(name=="pas") {
        if(model==null) {

            //清空数据
            jQuery("#tab_pas tbody  #tr"+name+"_"+num+" input").val("");
            var txt=jQuery("#tab_pas tbody  #pas_cardtype_"+num+" option:selected").text();
            if(txt.indexOf("日期")!= -1) {
                jQuery("#tab_pas tbody  #pas_cardnum_"+num).hide();
                jQuery("#tab_pas tbody  #txtBirday_"+num).show();
            } else {
                jQuery("#tab_pas tbody  #pas_cardnum_"+num).show();
                jQuery("#tab_pas tbody  #txtBirday_"+num).hide();
            }
            var date=new Date();
            var strDate=GetStrDate(date,0);
            jQuery("#tab_pas tbody  #txtBirday_"+num).val(strDate);
        } else {

            //显示值
            jQuery("#tab_pas tbody #pas_pasname_"+num).val(model._passengername);
            jQuery("#tab_pas tbody  #pas_type_"+num).val(model._passengertype);
            jQuery("#tab_pas tbody  #pas_cardtype_"+num).val(model._ctype);
            var txt=jQuery("#tab_pas tbody  #pas_cardtype_"+num+" option:selected").text();
            if(txt.indexOf("日期")!= -1||/\d{4}\-\d{2}\-\d{2}/.test(model._cid)) {
                jQuery("#tab_pas tbody  #pas_cardnum_"+num).hide();
                jQuery("#tab_pas tbody  #txtBirday_"+num).show();
                //var date=GetStrDate(eval("new "+model._cid.NewReplace("/","")+""),0);
                jQuery("#tab_pas tbody  #txtBirday_"+num).val(model._cid);

            } else {
                jQuery("#tab_pas tbody  #pas_cardnum_"+num).show();
                jQuery("#tab_pas tbody  #txtBirday_"+num).hide();
                jQuery("#tab_pas tbody  #pas_cardnum_"+num).val(model._cid);
                // jQuery("#tab_pas tbody  #txtBirday_"+num).val(model._cid);
            }
            //儿童编码处理
            //if(model._passengertype=="2") {

            if(/\d{4}\-\d{2}\-\d{2}/.test(model._cid)) {
                jQuery("#tab_pas tbody  #pas_cardtype_"+num+" option:contains('出生日期')").attr("selected",true);
            }
            //}

            jQuery("#tab_pas tbody  #pas_phone_"+num).val(model._a10);
            jQuery("#tab_pas tbody  #pas_seatprice_"+num).val(model._pmfee);
            jQuery("#tab_pas tbody  #pas_abprice_"+num).val(model._abfee);
            jQuery("#tab_pas tbody  #pas_rqprice_"+num).val(model._fuelfee);

        }
    } else if(name=="sky") {
        if(model==null) {
            //清空数据//#trsky_1
            jQuery("#tr"+name+"_"+num+" input").val("");
        } else {
            //显示值
            jQuery("#sky_carrycode_"+num).val(model._carrycode);
            jQuery("#sky_flight_"+num).val(model._flightcode);
            jQuery("#sky_aircraft_"+num).val(model._aircraft);

            jQuery("#txtFromCity_"+num).val(model._fromcitycode);
            jQuery("#ddlFromCity_"+num).val(model._fromcitycode);

            jQuery("#txtToCity_"+num).val(model._tocitycode);
            jQuery("#ddlToCity_"+num).val(model._tocitycode);

            var flyStartDate=GetStrDate(eval("new "+model._fromdate.NewReplace("/","")+""),2);
            var flyEndDate=GetStrDate(eval("new "+model._todate.NewReplace("/","")+""),2);

            jQuery("#sky_flystartdate_"+num).val(flyStartDate);
            jQuery("#sky_flyenddate_"+num).val(flyEndDate);
            jQuery("#sky_startterminal_"+num).val(model._terminal);

            jQuery("#sky_space_"+num).val(model._space);
            jQuery("#sky_yfareprice_"+num).val(model._yfarefee);
            jQuery("#sky_spaceprice_"+num).val(model._spaceprice);
            jQuery("#sky_abfee_"+num).val(model._abfee);
            jQuery("#sky_rqfee_"+num).val(model._fuelfee);
            jQuery("#sky_discount_"+num).val(model._discount);
        }
    }
}

//将修改的数据保存起来
function SaveHidden() {
    try {
        ShowDiv(true);
        if(!validate()) return false;

        //所有信息
        var ALLInfo=eval("("+unescape(jQuery("#Hid_ALLInfo").val())+")");

        //订单实体
        var Order=ALLInfo.OrderParam._OrderParamModel[0]._Order;
        //航段列表
        var Sky_List=ALLInfo.OrderParam._OrderParamModel[0]._SkyList;
        //乘机人列表
        var Pas_List=ALLInfo.OrderParam._OrderParamModel[0]._PasList;

        //---------为订单实体赋值------------------------------------------------------------------------------------
        Order._pnr=jQuery("#o_orderpnr").val();
        Order._bigcode=jQuery("#o_orderbigpnr").val();
        if(!Order._ischdflag) {
            ALLInfo.OrderParam._PnrInfo.AdultPnr=Order._pnr;
            ALLInfo.OrderParam._PnrInfo.AdultPnrToBigPNR=Order._bigcode;
        } else {
            ALLInfo.OrderParam._PnrInfo.childPnr=Order._pnr;
            ALLInfo.OrderParam._PnrInfo.childPnrToBigPNR=Order._bigcode;
        }
        Order._office=jQuery("#o_orderoffice").val();
        Order._printoffice=jQuery("#o_orderprintoffice").val();
        ALLInfo.OrderParam._PnrInfo.Office=Order._office;
        ALLInfo.OrderParam._PnrInfo.PrintOffice=Order._printoffice;

        ALLInfo.RTData=jQuery.trim(jQuery("#txtrtdata").val());
        ALLInfo.PATData=jQuery.trim(jQuery("#txtpatdata").val());




        ALLInfo.m_SupCompany._uninallname=jQuery("#o_ordercreatecompany").val();
        ALLInfo.m_CurCompany._uninallname=jQuery("#o_orderowncompany").val();
        Order._orderstatuscode=jQuery("#o_orderstatuscode").val();
        Order._ordersourcetype=jQuery("#o_ordersource").val();
        //创建日期
        var createtime=jQuery("#o_createtime").val();
        var date=new Date(createtime.split('-')[0],parseInt(createtime.split('-')[1]-1,10),createtime.split('-')[2]);
        Order._createtime=GetJSONDate(date);

        Order._pmfee=jQuery("#o_pmfee").val();
        Order._abfee=jQuery("#o_abfee").val();
        Order._fuelfee=jQuery("#o_fuelfee").val();
        Order._babyfee=jQuery("#o_babyfee").val();
        // Order._paymoney=jQuery("#o_paymoney").val();
        Order._policypoint=jQuery("#o_policy").val();
        Order._returnpoint=jQuery("#o_policy").val();
        Order._policytype=jQuery("#o_ddlPolicyType").val();

        //---------------为乘机人赋值------------------------------------------------------------------------------
        //实体字符串
        var strSkyWay=unescape(jQuery("#Hid_SkyModel").val());
        var strPas=unescape(jQuery("#Hid_PasModel").val());

        //处理乘机人列表
        var msg="";
        var newPasList=[];
        var PasList=jQuery("#tab_pas tbody  tr[id*='trpas_']");
        if(PasList.length==0) {
            showdialog("请添加乘客数据项！");
            return false;
        }
        PasList.each(function (index,trpas) {

            var tr=jQuery(trpas);
            var num=tr.attr("id").replace("trpas_","");
            var model=null;
            if(tr.attr("pid")!=null) {
                //修改
                for(var k=0;k<Pas_List.length;k++) {
                    if(Pas_List[k]._id==tr.attr("pid")) {
                        model=Pas_List[k];
                        break;
                    }
                }
            }
            if(model==null) {
                //重新构造
                model=eval("("+strPas+")");
            }
            var pas_pasname=jQuery.trim(jQuery("#tab_pas tbody  #pas_pasname_"+num).val());
            var pas_cardnum=jQuery.trim(jQuery("#tab_pas tbody  #pas_cardnum_"+num).val());
            var phone=jQuery.trim(jQuery("#tab_pas tbody  #pas_phone_"+num).val());

            var textCard=jQuery("#tab_pas tbody  #pas_cardtype_"+num+" option:selected").text();
            if(textCard.indexOf('日期')!= -1) {
                pas_cardnum=jQuery.trim(jQuery("#tab_pas tbody  #txtBirday_"+num).val());
            }

            var pas_seatprice=jQuery.trim(jQuery("#tab_pas tbody  #pas_seatprice_"+num).val());
            var pas_abprice=jQuery.trim(jQuery("#tab_pas tbody  #pas_abprice_"+num).val());
            var pas_rqprice=jQuery.trim(jQuery("#tab_pas tbody  #pas_rqprice_"+num).val());
            if(pas_pasname=="") {
                msg="输入乘客姓名不能为空！";
                return false;
            } if(pas_cardnum=="") {
                msg="输入乘客证件号不能为空！";
                return false;
            }
            /*
            if(phone=="") {
            msg="输入乘客手机号码不能为空！";
            return false;
            }
            if(phone.length!=11) {
            msg="输入乘客手机号码必须为11位！";
            return false;
            }
            var reg=/^0{0,1}(13[0-9]|145|15[7-9]|153|156|18[0-9])[0-9]{8}$/i;
            if(!reg.test(phone)) {
            msg="输入乘客手机号码不存在！";
            return false;
            }*/
            if(jQuery.isNaN(pas_seatprice)) {
                msg="乘客输入舱位价格式错误！";
                return false;
            }
            if(jQuery.isNaN(pas_abprice)) {
                msg="乘客输入机建费价格式错误！";
                return false;
            }
            if(jQuery.isNaN(pas_rqprice)) {
                msg="乘客输入燃油费价格式错误！";
                return false;
            }
            //修改实体值-----------------------------
            model._passengername=pas_pasname;
            model._passengertype=jQuery("#tab_pas tbody  #pas_type_"+num).val();
            model._ctype=jQuery("#tab_pas tbody  #pas_cardtype_"+num).val();
            model._cid=pas_cardnum;
            model._a10=phone;
            model._pmfee=pas_seatprice;
            model._abfee=pas_abprice;
            model._fuelfee=pas_rqprice;
            //------------------------------------------------
            newPasList.push(model);
        })
        if(msg!="") {
            showdialog(msg);
            return false;
        }

        //---------------为航段赋值------------------------------------------------------------------------------

        //处理航段列表
        var newSkyList=[];
        var skyDATAList=jQuery("#tab_sky tbody  tr[id*='trsky_']");
        if(skyDATAList.length==0) {
            showdialog("请添加航段数据项！");
            return false;
        }
        skyDATAList.each(function (index,trpas) {
            //         
            var tr=jQuery(trpas);
            var num=tr.attr("id").replace("trsky_","");
            var model=null;
            if(tr.attr("sid")!=null) {
                //修改
                for(var k=0;k<Sky_List.length;k++) {
                    if(Sky_List[k]._id==tr.attr("sid")) {
                        model=Sky_List[k];
                        break;
                    }
                }
            }
            if(model==null) {
                //重新构造
                model=eval("("+strSkyWay+")");
            }

            var carrycode=jQuery.trim(jQuery("#sky_carrycode_"+num).val());
            if(carrycode=="") {
                msg="输入航空公司二字码不能为空！";
                return false;
            }
            var flightcode=jQuery.trim(jQuery("#sky_flight_"+num).val());
            if(flightcode=="") {
                msg="输入航班号不能为空！";
                return false;
            }

            if(jQuery.trim(jQuery("#sky_flystartdate_"+num).val())=="") {
                msg="航段起飞日期不能为空！";
                return false;
            }
            if(jQuery.trim(jQuery("#sky_flyenddate_"+num).val())=="") {
                msg="航段到达日期不能为空！";
                return false;
            }
            var space=jQuery.trim(jQuery("#sky_space_"+num).val());
            var yfarefee=jQuery.trim(jQuery("#sky_yfareprice_"+num).val());
            var spaceprice=jQuery.trim(jQuery("#sky_spaceprice_"+num).val());
            var abfee=jQuery.trim(jQuery("#sky_abfee_"+num).val());
            var fuelfee=jQuery.trim(jQuery("#sky_rqfee_"+num).val());
            var discount=jQuery.trim(jQuery("#sky_discount_"+num).val());
            if(space=="") {
                msg="航段舱位不能为空！";
                return false;
            }
            if(jQuery.isNaN(yfarefee)) {
                msg="航段Y舱票价格式错误！";
                return false;
            }
            if(jQuery.isNaN(spaceprice)) {
                msg="航段舱位价格式错误！";
                return false;
            }
            if(jQuery.isNaN(abfee)) {
                msg="航段机建费格式错误！";
                return false;
            }
            if(jQuery.isNaN(fuelfee)) {
                msg="航段燃油费格式错误！";
                return false;
            }
            if(jQuery.isNaN(discount)) {
                msg="航段输入折扣格式错误！";
                return false;
            }
            //修改实体值------------------------------------------------
            model._carrycode=carrycode;
            model._flightcode=flightcode;
            model._aircraft=jQuery("#sky_aircraft_"+num).val();
            //出发城市
            var strfromCity=jQuery("#ddlFromCity_"+num+" option:selected").text();
            model._fromcityname=strfromCity.split("-")[1];
            model._fromcitycode=strfromCity.split("-")[0];
            //到达城市
            var strtoCity=jQuery("#ddlToCity_"+num+" option:selected").text();
            model._tocityname=strtoCity.split('-')[1];
            model._tocitycode=strtoCity.split('-')[0];
            //起飞到达日期
            var flyStartDate=GetJSONDate(GetDate(jQuery("#sky_flystartdate_"+num).val()+":00"));
            var flyEndDate=GetJSONDate(GetDate(jQuery("#sky_flyenddate_"+num).val()+":00"));
            model._fromdate=flyStartDate;
            model._todate=flyEndDate;
            //航站楼
            model._terminal=jQuery("#sky_startterminal_"+num).val();
            //舱位
            model._space=space;
            model._yfarefee=yfarefee;
            model._spaceprice=spaceprice;
            model._abfee=abfee;
            model._fuelfee=fuelfee;
            model._discount=discount;
            //------------------------------------------------
            newSkyList.push(model);
        })
        if(msg!="") {
            showdialog(msg);
            return false;
        }

        //赋值
        ALLInfo.OrderParam._OrderParamModel[0]._Order=Order;
        ALLInfo.OrderParam._OrderParamModel[0]._SkyList=newSkyList;
        ALLInfo.OrderParam._OrderParamModel[0]._PasList=newPasList;
        //所有信息
        var strAllVal=JSON.stringify(ALLInfo);
        var txtRemark=escape(jQuery("#txtRemark").val());
        var Url="HandPnrImport.aspx";
        function handle(data) {

            jQuery("#btnCreate").attr("disabled",false);
            data=jQuery.trim(data);
            if(data!="") {
                var strArr=data.split('##');
                if(strArr.length==3) {
                    if(strArr[0]=="1") {
                        showdialog(strArr[1]+"<br /><a href='../Order/OrderList.aspx?currentuserid="+jQuery("#currentuserid").val()+"'>"+strArr[2]+"</a>","go");
                        jQuery("#span_OrderId").html(strArr[2]);
                    } else if(strArr[0]=="0") {
                        showdialog(strArr[1]);
                    }
                }
            } else {
                showdialog("生成订单失败,返回数据为空！");
            }
        }

        var Param={ create: "1",data: strAllVal,Remark: txtRemark,num: Math.random(),currentuserid: jQuery("#currentuserid").val() };
        jQuery.post(Url,Param,handle,"text");

        //jQuery("#Hid_ALLInfo").val("");
        //jQuery("#Hid_ALLInfo").val(escape(strAllVal));
    } catch(e) {
        alert(e.message);
        return false;
    }
    finally {
        ShowDiv(false);
    }
    jQuery("#btnCreate").attr("disabled",true);
    return false;
}

//------验证系列------------------------------------------------------------
//必须有数据输入验证
function require() {
    try {
        var value=jQuery.trim(jQuery(this).val());
        if(value==null||value=='') {
            //showdialog("文本框不能为空!");
            return false;
        }
    } catch(e) {
        alert(e.message);
    }
}
//输入数字框验证
function NumVate() {
    try {
        var value=jQuery.trim(jQuery(this).val());
        if(value==null||value=='') {
            showdialog("文本框不能为空,请输入正确的数字!");
            jQuery(this).val("0");
            return false;
        }
        if(!isNaN(value)) {
            var userreg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
            if(userreg.test(value)) {
                //验证通过
                if(parseFloat(value).toString().length>5) {
                    jQuery(this).val("0");
                    showdialog("输入数据超出范围!");
                    return false;
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

    } catch(e) {
        alert(e.message);
    }
}
function CardChange() {

    var num=jQuery(this).attr("id").NewReplace("pas_cardtype_","");
    var txt=jQuery("#pas_cardtype_"+num+" option:selected").text();
    if(txt.indexOf("日期")!= -1) {
        jQuery("#pas_cardnum_"+num).hide();
        jQuery("#txtBirday_"+num).show();
    } else {
        jQuery("#pas_cardnum_"+num).show();
        jQuery("#txtBirday_"+num).hide();
    }
}
function validate() {
    var IsSuc=true;
    var o_orderpnr=jQuery.trim(jQuery("#o_orderpnr").val());
    var o_orderbigpnr=jQuery.trim(jQuery("#o_orderbigpnr").val());
    var o_orderprintoffice=jQuery.trim(jQuery("#o_orderprintoffice").val());
    var o_orderoffice=jQuery.trim(jQuery("#o_orderoffice").val());
    if(o_orderpnr=="") {
        showdialog("PNR不能为空");
        IsSuc=false;
    }
    var pnrPat=/^[A-Za-z0-9]{6}$/;
    if(!pnrPat.test(o_orderpnr)) {
        showdialog("输入PNR长度错误！");
        IsSuc=false;
    }
    if(o_orderbigpnr=="") {
        showdialog("大编码不能为空！");
        IsSuc=false;
    }
    var pnrPat=/^[A-Za-z0-9]{6}$/;
    if(!pnrPat.test(o_orderbigpnr)) {
        showdialog("输入大编码长度错误！");
        IsSuc=false;
    }
    if(o_orderoffice=="") {
        showdialog("预订Office不能为空！");
        IsSuc=false;
    }
    var officePat=/^[A-Za-z]{3}\d{3}$/;
    if(!officePat.test(o_orderoffice)) {
        showdialog("输入Office号格式错误！");
        IsSuc=false;
    }
    if(o_orderprintoffice=="") {
        showdialog("出票Office不能为空！");
        IsSuc=false;
    }
    var officePat=/^[A-Za-z]{3}\d{3}$/;
    if(!officePat.test(o_orderprintoffice)) {
        showdialog("输入出票Office号格式错误！");
        IsSuc=false;
    }

    var o_pmfee=jQuery.trim(jQuery("#o_pmfee").val());
    var o_abfee=jQuery.trim(jQuery("#o_abfee").val());
    var o_fuelfee=jQuery.trim(jQuery("#o_fuelfee").val());
    var o_babyfee=jQuery.trim(jQuery("#o_babyfee").val());
    //var o_paymoney=jQuery.trim(jQuery("#o_paymoney").val());
    var o_policy=jQuery.trim(jQuery("#o_policy").val());

    if(jQuery.isNaN(o_pmfee)) {
        showdialog("输入订单票价格式错误！");
        IsSuc=false;
    }
    if(jQuery.isNaN(o_abfee)) {
        showdialog("输入订单机建费格式错误！");
        IsSuc=false;
    }
    if(jQuery.isNaN(o_fuelfee)) {
        showdialog("输入订单燃油费格式错误！");
        IsSuc=false;
    }
    if(jQuery.isNaN(o_babyfee)) {
        showdialog("输入订单婴儿票面价格格式错误！");
        IsSuc=false;
    }
    if(jQuery.isNaN(o_policy)) {
        showdialog("输入订单政策格式错误！");
        IsSuc=false;
    }
    var policy=ShowPoint(o_policy,1);
    if(policy<0||policy>=100) {
        showdialog("输入订单政策数据超出范围[0-100]！");
        IsSuc=false;
    }
    return IsSuc;
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
//显示隐藏遮罩层 true 显示 false隐藏
function ShowDiv(flag) {
    if(flag) {
        jQuery("#overlay").show();
        jQuery("#loading").show();
        jQuery("#btnCreate").hide();
    } else {
        jQuery("#overlay").hide();
        jQuery("#loading").hide();
        jQuery("#btnCreate").show();
    }
}

//加载读取------------------------------------------------------------------------------------------------
jQuery(function () {

    try {
        ShowDiv(true);


        //初始化乘客序号
        initArr(pasArr,maxPasNum);
        //初始化航段序号
        initArr(skyArr,maxskyNum);


        //所有信息
        var ALLInfo=eval("("+unescape(jQuery("#Hid_ALLInfo").val())+")");
        try {
            if(ALLInfo==null) {
                alert('加载失败');
                return;
            }
            //是否为记账订单1是 0否
            var isJZ=ALLInfo.IsImportJZ;
            if(isJZ=="1") {
                jQuery("#JZ").html("<b class='red'>该订单为记账订单</b>");
            }
        } catch(e) {
        }
        //订单实体
        var Order=ALLInfo.OrderParam._OrderParamModel[0]._Order;
        //航段列表
        var Sky_List=ALLInfo.OrderParam._OrderParamModel[0]._SkyList;
        //乘机人列表
        var Pas_List=ALLInfo.OrderParam._OrderParamModel[0]._PasList;


        //订单赋值
        jQuery("#o_orderpnr").val(Order._pnr);
        jQuery("#o_orderbigpnr").val(Order._bigcode);
        jQuery("#o_orderoffice").val(Order._office);
        jQuery("#o_orderprintoffice").val(Order._printoffice);
        jQuery("#o_ordercreatecompany").val(ALLInfo.m_SupCompany._uninallname);
        jQuery("#o_orderowncompany").val(ALLInfo.m_CurCompany._uninallname);

        jQuery("#o_orderstatuscode").val(Order._orderstatuscode);
        jQuery("#o_ordersource").val(Order._ordersourcetype);

        //禁用
        jQuery("#o_orderstatuscode").attr("disabled",true);
        jQuery("#o_ordersource").attr("disabled",true);



        //创建日期
        var date=eval("new "+Order._createtime.NewReplace("/","")+"");
        jQuery("#o_createtime").val(GetStrDate(date,0));

        jQuery("#o_pmfee").val(Order._pmfee);
        jQuery("#o_abfee").val(Order._abfee);
        jQuery("#o_fuelfee").val(Order._fuelfee);
        jQuery("#o_babyfee").val(Order._babyfee);
        //jQuery("#o_paymoney").val(Order._paymoney);
        jQuery("#o_policy").val(Order._returnpoint);
        //乘机人
        for(var i=0;i<Pas_List.length;i++) {
            /*
            if(i==0) {
            //初始化乘客默认项
            var num=addGroup(window.event,'pas');
            setModel(num,"pas",Pas_List[i]);
            jQuery("#trpas_0").attr("pid",Pas_List[i]._id);
            } else {*/
            var num=addGroup(null,"pas");
            setModel(num,"pas",Pas_List[i]);
            jQuery("#trpas_"+num).attr("pid",Pas_List[i]._id);
            // }
        }
        //航段
        for(var i=0;i<Sky_List.length;i++) {
            /*if(i==0) {
            setModel(0,"sky",Sky_List[i]);
            jQuery("#trsky_0").attr("sid",Sky_List[i]._id);
            } else {*/
            var num=addGroup(null,"sky");
            setModel(num,"sky",Sky_List[i]);
            jQuery("#trsky_"+num).attr("sid",Sky_List[i]._id);
            // }
        }

        //编码信息
        jQuery("#txtrtdata").val(ALLInfo.RTData);
        jQuery("#txtpatdata").val(ALLInfo.PATData);



        //注册事件    
        jQuery("#o_pmfee,#o_abfee,#o_fuelfee,#o_babyfee,#o_policy,#o_paymoney").blur(NumVate);
        jQuery("#tab_sky input[id*=sky_yfareprice_]").blur(NumVate);
        jQuery("#tab_sky input[id*=sky_spaceprice_]").blur(NumVate);
        jQuery("#tab_sky input[id*=sky_abfee_]").blur(NumVate);
        jQuery("#tab_sky input[id*=sky_rqfee_]").blur(NumVate);
        jQuery("#tab_sky input[id*=sky_discount_]").blur(NumVate);

        jQuery("#tab_pas input[id*=pas_seatprice_]").blur(NumVate);
        jQuery("#tab_pas input[id*=pas_abprice_]").blur(NumVate);
        jQuery("#tab_pas input[id*=pas_rqprice_]").blur(NumVate);
        //必须
        jQuery("#o_orderpnr,#o_orderbigpnr,#o_createtime").blur(require);
        jQuery("#o_orderpnr,#o_orderbigpnr,#o_createtime").blur(require);
        //
        jQuery("#tab_sky input[id*=sky_carrycode_]").blur(require);
        jQuery("#tab_sky input[id*=sky_flight_]").blur(require);

        jQuery("#tab_sky input[id*=sky_flystartdate_]").blur(require);
        jQuery("#tab_sky input[id*=sky_flyenddate_]").blur(require);
        jQuery("#tab_sky input[id*=sky_space_]").blur(require);
        //
        jQuery("#tab_pas input[id*=pas_pasname_]").blur(require);
        jQuery("#tab_pas input[id*=pas_cardnum_]").blur(require);
        jQuery("#tab_pas input[id*=txtBirday_]").blur(require);
        //jQuery("#tab_pas input[id*=pas_phone_]").blur(require);//手机号码
        jQuery("#tab_pas select[id*=pas_cardtype_]").change(CardChange);
        ShowDiv(false);
    } catch(e) {
        //alert(e.message);
        ShowDiv(false);
    } finally {
        ShowDiv(false);
    }
});
