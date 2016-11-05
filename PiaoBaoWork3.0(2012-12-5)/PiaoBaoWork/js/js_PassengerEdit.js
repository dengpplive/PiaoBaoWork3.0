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
            }
        }
    });
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
function initArr(arr,num) {
    for(var i=1;i<=num;i++) {
        eval("var obj={_"+i+":0}");
        arr.push(obj);
    }
    return arr;
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
//最多可以添加航空公司和卡号数
var maxCarryNum=20;
var carryArr=[];
//添加一行
function addGroup(evt,name) {
    if(evt!=null) {
        var target=evt.srcElement?evt.srcElement:evt.target;
        jQuery(target).attr("disabled",true);
    }
    var num=0;
    //模板
    var trHtml=jQuery("<div></div>").append(jQuery("#tab_"+name+" tr[id='tr"+name+"_0'").clone(true)).html();
    var trCnt=jQuery("#tab_"+name+" tr[id*='tr"+name+"_']").length;
    if(name=="carry") {
        if(trCnt>=maxCarryNum) {
            showdialog("已超过最大范围,不能继续添加了！");
            return false;
        }
        //获取可用序号从1开始
        num=getMinFg(carryArr);
        //标记为已使用
        setFg(carryArr,num,"1");
    }
    //操作内容
    var opDiv='<div id="'+name+'_opdiv_'+num+'"> <span class="btn btn-ok-s"><input type="button" value="添加" id="btnAdd_'+num+'" onclick="return addGroup(event,\''+name+'\')"  /></span>'+
             ' <span class="btn btn-ok-s"><input type="button" value="删除" id="btnDel_'+num+'" onclick="return removeGroup(event,\''+name+'\','+num+')"  /></span></div>';
    //设置操作内容HTML
    trHtml="<tr id='tr"+name+"_"+num+"'>"+jQuery("<tr>"+trHtml+"</tr>").find("td:last").html(opDiv).parent().html()+"</tr>";
    //替换id
    trHtml=trHtml.NewReplace("_0","_"+num).NewReplace("txtSetSel(this,'ddlCarryCode',0)","txtSetSel(this,'ddlCarryCode',"+num+")").NewReplace("ddlSetText(this,'txtCarryCode',0)","ddlSetText(this,'txtCarryCode',"+num+")");
    //添加节点
    jQuery("#tab_"+name).append(trHtml);
    //设置初始值
    jQuery("#tab_"+name+" ddlCarryCode_"+num).eq(0).attr("selected",true);
    jQuery("#tab_"+name+" txtCarryCode_"+num).val("");
    jQuery("#tab_"+name+" txtAirNo_"+num).val("");

    if(evt!=null) {
        var target=evt.srcElement?evt.srcElement:evt.target;
        jQuery(target).attr("disabled",false);
    }
    return num;
}

//移除一行
function removeGroup(evt,name,num) {
    if(num!=null) {
        jQuery("#tab_"+name+" tr[id='tr"+name+"_"+num+"'").remove();
    } else {
        var trCount=jQuery("#tab_"+name+" tr").length;
        if(trCount>1) {
            //从后往前删除
            var lastTr=jQuery("#tab_"+name+" tr:last");
            num=lastTr.attr("id").NewReplace("tr"+name+"_","");
            lastTr.remove();
        } else {
            showdialog("该行数据不能删除！");
            return false;
        }
    }
    //标记为没有使用
    setFg(carryArr,num,"0");
    return false;
}
//保存数据
function SaveData() {

    var url="PassengerEdit.aspx";

    var val_IsEdit=jQuery("#Hid_IsEdit").val();
    var val_Name=jQuery.trim(jQuery("#txtUser").val());
    var val_Phone=jQuery.trim(jQuery("#txtPhone").val());

    var val_CardType=jQuery.trim(jQuery("#ddlCardType").val());
    //var text_CardType=jQuery.trim(jQuery("#ddlCardType option:selected").text());

    var val_CardNum=jQuery.trim(jQuery("#txtCardNum").val());
    //var val_Date=jQuery.trim(jQuery("#txtDate").val());
    if(jQuery("#txtDate").is(":visible")) {
        val_CardNum=jQuery.trim(jQuery("#txtDate").val());
    }

    var val_sex=jQuery("input[type='radio'][name='sex']:checked").val();
    var val_pastype=jQuery("input[type='radio'][name='pastype']:checked").val();
    var val_Birthday=jQuery("#txtBirthday").val();
    var val_Remark=jQuery("#txtRemark").val();

    if(val_Name=="") {
        showdialog("旅客姓名不能为空！");
        return false;
    }
    if(val_Phone=="") {
        showdialog("旅客手机号码不能为空！");
        return false;
    }
    if(val_CardNum=="") {
        showdialog("旅客证件号码不能为空！");
        return false;
    }
    if(val_Birthday=="") {
        showdialog("旅客出生日期不能为空！");
        return false;
    }
    //验证航空公司卡号 暂时不验证
    var val_CpyandNo="";

    var msg="";
    var carrNo=[];
    jQuery("#tab_carry tr").each(function (index,tr) {
        var carrCode=jQuery(tr).find("select[id*='ddlCarryCode_']").val();
        var AirNo=jQuery.trim(jQuery(tr).find("input[id*='txtAirNo_']").val());

        if(carrCode!=""&&AirNo=="") {
            msg="航空公司卡号不能为空！";
            return false;
        }
        carrNo.push(carrCode+","+AirNo);
    });
    if(msg!="") {
        showdialog(msg);
        return false;
    }
    if(carrNo!=null&&carrNo.length>0) {
        val_CpyandNo=carrNo.join('|');
    }

    var param={
        IsEdit: escape(val_IsEdit),
        Name: escape(val_Name),
        Phone: escape(val_Phone),
        CardType: escape(val_CardType),
        CardNum: escape(val_CardNum),
        Sex: escape(val_sex),
        Pastype: escape(val_pastype),
        Birthday: escape(val_Birthday),
        Remark: escape(val_Remark),
        CpyandNo: escape(val_CpyandNo),
        save: "save",
        num: Math.random(),
        currentuserid: jQuery("#currentuserid").val()
    };

    if(val_IsEdit=="1") {
        //编辑
        var Id=jQuery("#Hid_id").val();
        param.Id=jQuery("#Hid_id").val();
    }
    jQuery.post(url,param,function (data) {
        if(jQuery.trim(data)!="") {
            var strArr=data.split('@@');
            if(strArr.length==2) {
                if(strArr[0]=="1") {
                    showdialog(strArr[1]);
                } else {
                    showdialog(strArr[1]);
                }
            }
        } else {
            showdialog("操作失败！");
        }
    },"text");
    return false;
}
//选择旅客类型
function PasTypeChange() {
    var text=jQuery(this).attr('txt');
    var val=jQuery(this).val();
    var opData=jQuery.trim(jQuery("#Hid_CardData").val()).split('|');
    var ophtml=[];
    var opArr=[];
    for(var i=0;i<opData.length;i++) {
        opArr=opData[i].split('@@');
        if(text.indexOf('成人')!= -1) {
            ophtml.push('<option value="'+opArr[1]+'">'+opArr[0]+'</option>');
        } else if(text.indexOf('儿童')!= -1) {
            if(opData[i].indexOf('身份证')!= -1||opData[i].indexOf('出生日期')!= -1||opData[i].indexOf('其他有效证件')!= -1) {
                ophtml.push('<option value="'+opArr[1]+'">'+opArr[0]+'</option>');
            }
        } else if(text.indexOf('婴儿')!= -1) {
            if(opData[i].indexOf('其他有效证件')!= -1) {
                ophtml.push('<option value="'+opArr[1]+'">'+opArr[0]+'</option>');
            }
        }
    }
    jQuery("#ddlCardType").html(ophtml.join(''));
    jQuery("#ddlCardType option:visible").eq(0).attr("selected",true);
    CardTypeChange();
}
//选择证件类型
function CardTypeChange() {
    var val=jQuery(this).val();
    var text=jQuery("#ddlCardType option:selected").text();
    var pasType=jQuery("input[type='radio'][name='pastype']:checked").attr("txt");
    if(pasType.indexOf('成人')!= -1) {
        jQuery("#txtCardNum").show();
        jQuery("#txtDate").hide();
    } else if(pasType.indexOf('儿童')!= -1) {
        if(text.indexOf("出生日期")!= -1) {
            jQuery("#txtCardNum").hide();
            jQuery("#txtDate").show();
        } else {
            jQuery("#txtCardNum").show();
            jQuery("#txtDate").hide();
        }
    } else if(pasType.indexOf('婴儿')!= -1) {
        jQuery("#txtCardNum").hide();
        jQuery("#txtDate").show();
    }
}

//加载。。。
jQuery(function () {
    //初始化航空公司和卡号数
    initArr(carryArr,maxCarryNum);
    var IsEdit=jQuery("#Hid_IsEdit").val();
    //单击旅客类型事件
    jQuery("input[type='radio'][name='pastype']").click(PasTypeChange);
    jQuery("#ddlCardType").change(CardTypeChange);


    if(IsEdit=="1") {

        //编辑
        var CpyandNo=jQuery.trim(jQuery("#Hid_CpyandNo").val());
        var Arr=CpyandNo.split('|');
        var num=0;
        var carryCode='',Card='';
        var name='carry';
        for(var i=0;i<Arr.length;i++) {
            var carrArr=Arr[i].split(',');
            if(carrArr.length==2) {
                carryCode=carrArr[0].toUpperCase();
                Card=carrArr[1];
                if(i>0) {
                    //添加
                    num=addGroup(null,name);
                }
                //赋值                       
                jQuery("#tab_"+name+" select[id='ddlCarryCode_"+num+"'] option[value='"+carryCode+"']").attr("selected",true);
                jQuery("#tab_"+name+" #txtCarryCode_"+num).val(carryCode);
                jQuery("#tab_"+name+" #txtAirNo_"+num).val(Card);
            }
        }
        var Flyer=jQuery.trim(jQuery("#Hid_Flyer").val());
        if(Flyer!=null) {
            var model=eval("("+Flyer+")");
            jQuery("#txtUser").val(model._name);
            jQuery("#txtPhone").val(model._tel);
            //证件类型
            jQuery("select[id='ddlCardType']").val(model._certificatetype);
            //乘客类型
            jQuery("input[type='radio'][name='pastype'][value="+model._flyertype+"]").attr("checked",true);
            CardTypeChange();
            if(jQuery("#txtCardNum").is(":visible")) {
                jQuery("#txtCardNum").val(model._certificatenum);
            } else {
                jQuery("#txtDate").val(model._certificatenum);
            }
            jQuery("input[type='radio'][name='sex'][value="+model._sex+"]").attr("checked",true);
            var Birthday=GetStrDate(eval("new "+model._brontime.NewReplace("/","")+""),0);
            jQuery("#txtBirthday").val(Birthday);
            jQuery("#txtRemark").val(model._remark);

        }
    }
})