//重命名解决冲突
var $J=jQuery.noConflict(false);
//显示
function showdg(id,t,param) {
    $J("select").hide();
    $J("#"+id).html(t);
    $J("#"+id).dialog({
        title: '提示',
        bgiframe: true,
        width: 350,
        height: 140,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            $J("select").show();
        },
        buttons: {
            '确定': function () {
                $J(this).dialog('close');
                if(param!=null) {
                    if(param=="0") {
                        location.reload(true);
                    }
                }
            }
        }
    }).css({ "width": "auto","height": "auto" });
}

//退款查询使用
function showdialog(t,p) {
    $J("#showOne").html(t);
    $J("#showOne").dialog({
        title: '标题',
        bgiframe: true,
        height: 140,
        modal: true,
        overlay: {
            backgroundColor: '#red',
            opacity: 0.5
        },
        buttons: {
            '确定': function () {
                $J(this).dialog('close');
                if(p!=null&&t.indexOf("成功")!= -1) {
                    location.reload(true);
                }
            }
        }
    });
}
//-------------------------修改证件号-----------------------------------------------
function _show() {
    $J("#overlay").show();
    $J("#loading").show();
}
function _hide() {
    $J("#overlay").hide();
    $J("#loading").hide();
}
function showUpdate(t,c) {
    $J("#show2").html(c);
    $J("#show2").dialog({
        title: t,
        bgiframe: true,
        width: 380,
        height: 250,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            '修改': function () {
                var obj=event.srcElement?event.srcElement:event.target;
                $J(obj)[0].disabled=true;
                $J(this).dialog("close");
                SsrUpdate(1);
            },
            "取消": function () {
                $J(this).dialog("close");
            }
        },
        close: function () {
            //location.reload(true);
        }
    });
}
//修改乘机人证件号
function ShowUpdateSsr(IsDate,_Remark,_pasName,pasType,ssrId,orderID,PasId,Is3U) {
    var Remark=unescape(_Remark);
    var pasName=unescape(_pasName);
    var ssrHtml="";
    var Isex="0";
    if(Is3U=="0"||Is3U=="1") {
        if(IsDate=="0") {
            ssrHtml="<input id=\"pasSsr\" type=\"text\" value=\""+ssrId+"\" maxlength=\"18\" />";
            if(Is3U=="1") {
                Is3U="0";
                ssrHtml+="<br /><span style='color:red;'> 3U只能修改证件号中任意的3位号码</span>"
            }
        } else {
            ssrHtml="<input id=\"pasSsr\" type=\"text\" value=\""+ssrId+"\" readonly=\"true\" maxlength=\"18\"  onclick=\"WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd',maxDate:'%y-%M-%d'})\" />";
        }
    }
    /*
    else if(Is3U=="1") {
    if(IsDate=="0") {

    var leftssr=ssrId,rightSsr="";
    if(ssrId.length>3) {
    leftssr=ssrId.substring(0,ssrId.length-3);
    rightSsr=ssrId.substring(ssrId.length-3);
    }
    Isex="1";
    ssrHtml="<input id=\"pasSsr\" type=\"text\" value=\""+leftssr+"\" disabled=\"disabled\" /><input id=\"pas_3uex\" type=\"text\" style=\"width:60px;\" maxlength=\"3\" value=\""+rightSsr+"\" />";
    } else {
    ssrHtml="<input id=\"pasSsr\" type=\"text\" value=\""+ssrId+"\" readonly=\"true\"  onclick=\"WdatePicker({isShowClear:false,isShowWeek:false,dateFmt:'yyyy-MM-dd',minDate:'{%y-2}-%M-%d',maxDate:'%y-%M-%d'})\" />";
    }
    }*/
    var html="<input id=\"pasidname\" type=\"hidden\" value=\""+PasId+"##"+pasName+"\" /><input id=\"IsEx\" type=\"hidden\" value=\""+Isex+"\" /><table><tr><th>乘机人:</th><td>"+pasName+"</td><td></td><td></td></tr><tr><th>证件号:</th><td>"+ssrHtml+"</td><td></td><td></td></tr>"+
        "<tr><th>同步编码：</th><td colspan=\"3\"><label for='IsSyncPnr1'><input type='radio' id='IsSyncPnr1' name='IsSyncPnr' value='1'  />是</label><label for='IsSyncPnr2'><input type='radio' id='IsSyncPnr2' name='IsSyncPnr' value='0'  checked='true'  />否</label></td></tr>"+

        "<tr><th>备注：</th><td colspan=\"3\"><textarea id=\"TxtPRemark\" cols=\"35\" rows=\"4\">"+Remark+"</textarea></td></tr>"+
    //"<tr><td colspan=\"4\" align=\"center\"><input type=\"button\" value=\"修改\" id=\"btnOK\" onclick=\"SsrUpdate()\" /><input type=\"button\" value\"取消\" id=\"btnClose\"></td></tr>";
        "</table>";
    showUpdate("证件号修改",html);
    return false;
}
//调用修改证件号
function SsrUpdate(type) {
    if(type==0) {
        $J("#show2").dialog("close");
    } else if(type==1) {
        var pasId=$J("#pasidname").val();
        var pasSsr=$J.trim($J("#pasSsr").val());
        var oldCid=$J("#Hid_CID_"+pasId.split('##')[0]).val();
        var IsEx=$J.trim($J("#IsEx").val());
        var orderId=$J.trim($J("#Hid_OrderId").val());
        if(IsEx=="1") {
            pasSsr=$J.trim($J("#pasSsr").val())+$J.trim($J("#pas_3uex").val());
        }
        var TxtReMark=$J.trim($J("#TxtPRemark").val()).replace("'","");
        if(pasSsr=="") {
            showdialog("输入证件号不能为空！");
            return;
        }
        if(oldCid==pasSsr) {
            showdialog("修改前后证件号一样！");
            return;
        }
        //是否同步到编码 1是 0否
        var IsBlockPnr=$J.trim($J("input[type='radio'][name='IsSyncPnr']:checked").val());
        _show();
        $J.post("OrderDetail.aspx",
                {
                    SsrOpType: escape("ssr"),
                    OrderId: escape(orderId),
                    PassngerSsr: escape(pasSsr),
                    oldCid: escape(oldCid),
                    pRemark: escape(TxtReMark),
                    passengerId: escape(pasId),
                    IsBlockPnr: escape(IsBlockPnr),
                    num: Math.random(),
                    currentuserid: $J("#currentuserid").val()
                },
                function (data) {
                    _hide();
                    $J("#show2").dialog("close");
                    showdialog(data,1);

                },"text");

    }
}
//------------------------------------------------------------------------
//-------行程单---------------------------------------------------
//打印票据
function GoPrint(url) {
    window.open(url);
    return false;
}
//保存设置打印范围
function SaveTripNum(obj) {
    var tStart=$J.trim($J("#txtNumStart").val());
    var tEnd=$J.trim($J("#txtNumEnd").val());
    var CpyNo=$J.trim($J("#Hid_CpyNo").val());
    var ParamId=$J.trim($J("#Hid_ParamId").val());
    var val_RoleType=$J.trim($J("#Hid_RoleType").val());

    var msg='';
    //if(tStart==''||tEnd=='') {
    //    msg="输入数据不能为空";
    //}
    if(msg=='') {
        if($J.isNaN(tStart)||$J.isNaN(tEnd)||tEnd.length!=10||tStart.length!=10) {
            msg="输入数据无效";
        }
    }
    if(msg=='') {
        if(parseFloat(tStart,10)-parseFloat(tEnd,10)>0) {
            msg="输入行程单号段范围有误";
        }
    }
    if(msg!='') {
        showdg('showOne',msg);
    } else {
        var param=
                {
                    optype: "SAVENUM",
                    StartAndEnd: escape(tStart+"-"+tEnd),
                    CpyNo: escape(CpyNo),
                    ParamId: escape(ParamId),
                    RoleType: escape(val_RoleType),
                    num: Math.random(),
                    currentuserid: $J("#currentuserid").val()
                };
        $J.post("OrderDetail.aspx",param,function (data) {
            var strArr=data.split("##");
            if(strArr.length==2) {
                if(strArr[0]=="1") {
                    //成功
                    showdg('showOne',"保存成功",0);
                } else if(strArr[0]=="0") {
                    //失败
                    showdg('showOne',"行程单号段保存失败！");
                }
            }
        },"text");
    }
}

//创建或者作废行程单
function UpdateTrip(param,obj) {

    param=eval("("+unescape(param)+")");
    var msg="";
    if($J.trim(param.TKnum)=="") {
        msg="票号不能为空！";
    } else if($J.trim(param.TPNum)=="") {
        msg="行程单号不能为空！";
    } else if(param.type=="0"&&param.TPId=="0") {
        msg="行程单号不能为空!!";
    }
    if(msg=="") {
        $J(obj).attr("disabled",true);
        var CpyNo=$J("#Hid_CpyNo").val();
        var GYCpyNo=$J("#Hid_GYCpyNo").val();
        var RoleType=$J("#Hid_RoleType").val();
        var OrderId=$J("#Hid_OrderId").val();//订单号

        param.optype=param.type=="0"?"create":"void";
        param.type=escape(param.type);
        param.CpyNo=escape(CpyNo);
        param.GYCpyNo=escape(GYCpyNo);
        param.RoleType=escape(RoleType);
        param.pid=escape(param.pid);
        param.TPId=escape(param.TPId);
        param.pName=escape(param.pName);
        param.TKnum=escape(param.TKnum);
        param.TPNum=escape(param.TPNum);
        param.Office=escape(param.Office);
        param.OrderId=escape(OrderId);//订单号


        param.num=Math.random();
        param.currentuserid=$J("#currentuserid").val();
        $J.post("OrderDetail.aspx",param,function (data) {
            $J(obj).attr("disabled",false);
            var strArr=data.split("##");
            if(strArr.length==2) {
                if(strArr[0]=="1") {
                    //成功
                    showdg('showOne',strArr[1],0);
                } else if(strArr[0]=="0") {
                    //失败
                    showdg('showOne',strArr[1]);
                }
            }
        },"text");
    } else {
        showdg('showOne',msg);
    }
}
//关闭对话框
function cancelTrip(id) {
    $J("#"+id).dialog("close");
}
//显示HTML
function ShowHTML(param) {
    var msg="";
    var IsValid=$J("#Hid_IsValid").val();
    var RoleType=parseInt($J("#Hid_RoleType").val(),10);
    var model=null;
    if(RoleType>3) {
        if(param.type=="0") {
            //-------create start---------------------------------------------------------
            if(IsValid=="0") {
                //无可用行程单时需申请
                var valArr=$J("#Hid_ApplyApram").val().split('@@');
                var param="";
                if(RoleType==1) {
                    if(valArr.length==4) {
                        var v=[];
                        v.push("LoginName="+valArr[0]);
                        v.push("UserName="+valArr[1]);
                        v.push("CpyNo="+valArr[2]);
                        v.push("CpyName="+valArr[3]);
                        v.push("currentuserid="+$J("#currentuserid").val());
                        param=v.join('&');
                    }
                }
                msg='创建行程单需先申请后,才可创建！<br />&nbsp;&nbsp;&nbsp;<a href="../TravelNumManage/ApplyTravel.aspx?'+param+'">进入申请页面</a>';
            } else {
                //检查票号是否为空
                if($J.trim(param.TKnum)=="") {
                    msg='乘客票号为空,无法创建行程单！';
                }
                if(msg=="") {
                    var tkPatern=/\d{3,4}(\-?|\s+)\d{10}/;
                    if(!tkPatern.test($J.trim(param.TKnum))) {
                        msg='乘客票号格式不和规范,无法创建行程单！';
                    }
                }
                if(msg=="") {
                    //有可用行程单
                    var data=$J("#Hid_TripData").val();
                    if(data!="") {
                        try {
                            data=eval("("+data+")");
                            if(data!=null&&data.length>0) {
                                model=data[0];
                                //行程单号
                                param.TPNum=model._tripnum;
                                //行程单id
                                param.TPId=model._id;
                                param.Office=model._createoffice;
                            }
                        } catch(e) {
                            msg="设置的行程单号段内无可用行程单，请重新设置打印号段或者重新申请行程单！";
                        }
                    }
                }
            }
            //-------create end---------------------------------------------------------
        } else if(param.type=="1") {
            //------void start--------------------------------
            //检查票号是否为空
            if($J.trim(param.TKnum)=="") {
                msg='乘客票号为空,无法创建行程单！';
            }
            if(msg=="") {
                var tkPatern=/\d{3,4}(\-?|\s+)\d{10}/;
                if(!tkPatern.test($J.trim(param.TKnum))) {
                    msg='乘客票号格式不和规范,无法创建行程单！';
                }
            }
            if(msg=="") {
                param.TPId="0";
                param.Office="";
            }
            //------void end  --------------------------------                
        }
        if(msg!="") {
            showdg("showOne",msg);
        } else {

            var btnCon="",pm=escape(JSON.stringify(param));
            var savaHtml='',btnSaveNum='',msgInfo="";
            if(param.type=="0") {
                //可用行程单范围   
                var tripFanWei=$J("#Hid_StartEndNum").val();
                var tripUseFanWei=$J("#Hid_SetStartEndNum").val().split('-');
                var txtStart='',txtEnd='';
                if(tripUseFanWei.length==2) {
                    txtStart=tripUseFanWei[0].replace("|","");
                    txtEnd=tripUseFanWei[1].replace("|","");
                }

                var printNum='<input type="text" id="txtNumStart" style="width:100px;" value="'+txtStart+'" maxlength="10" />-<input type="text" id="txtNumEnd"  style="width:100px;" value="'+txtEnd+'" maxlength="10" />';
                btnSaveNum='<span class="btn btn-ok-s"><input type="button" id="saveNum" value="保存打印号段" onclick="SaveTripNum(this)" /></span>&nbsp;&nbsp;&nbsp;&nbsp;';
                savaHtml='<tr><th class=\"tdNew\">打印号段：</th><td colspan="3">'+printNum+'</td><th class=\"tdNew\">可用号段范围</th><td><font class="red">'+tripFanWei+'</font></td></tr>';
                btnCon='<span class="btn btn-ok-s"><input type="button" id="btnCreate" value="创建行程单" onclick=UpdateTrip("'+pm+'",this) /></span>&nbsp;&nbsp';
                if(param.TPNum=="") {
                    msgInfo="<font class='red'>行程单打印号段范围不在可用号段范围内，请重新设置！</font>";
                }
            } else {
                btnCon='<span class="btn btn-ok-s"><input type="button" id="btnVoid" value="作废行程单" onclick=UpdateTrip("'+pm+'",this) /></span>&nbsp;&nbsp';
            }

            //显示对话框
            var html="<table width=\"760px\" cellspacing=\"0\" cellpadding=\"2\" border=\"1\" id=\"tableTrip\">";
            html+="<tr><th class=\"tdNew\">姓名：</th><td>"+param.pName+"</td><th class=\"tdNew\">票号：</th><td><input type=\"text\" id=\"txtticket_1\" value=\""+param.TKnum+"\" disabled=\"disabled\" /></td><th class=\"tdNew\">行程单号：</th><td><input type=\"text\" id=\"txtTripNum_1\" value=\""+param.TPNum+"\" disabled=\"disabled\" /></td></tr>";
            html+=savaHtml;
            html+='<tr><td colspan="2"></td><td colspan="2" align="center">'+btnSaveNum+"&nbsp;&nbsp"+btnCon+'<span class="btn btn-ok-s"><input type="button" id="btnCancel" value="关  闭" onclick=cancelTrip("html") /></span></td><td colspan="2">'+msgInfo+'</td></tr>';
            html+="</table>";
            showdg("html",html);
            $J("#html").dialog("option","buttons",{});
            $J("#html").dialog("option",{ "width": "800px","height": "auto" });
            $J("#html").dialog("option",{ "position": [150,200] });
        }
    }
    return false;
}

//------------------------------------------
//显示修改票号
function ShowTicketNumber(OrderId,pasId,pasName,TicketNumber,TravelNumber) {
    var html="<table  cellspacing=\"0\" cellpadding=\"2\" border=\"1\" width=\"300px\">";
    html+="<tr><th>乘客姓名:</th><td>"+pasName+"</td></tr>";
    html+="<tr><th>票号:</th><td><input type='text' id='txtTicketNumber' value='"+TicketNumber+"' maxlength='14' /></td></tr>";
    html+='<tr><td colspan=\"2\" align=\"center\"><span class="btn btn-ok-s"><input type="button" id="btnUpdateTK" value="修改票号" onclick=UpdateTK("'+OrderId+'","'+pasId+'","'+pasName+'","'+TicketNumber+'","'+TravelNumber+'")  /></span>&nbsp;&nbsp;<span class="btn btn-ok-s"><input type="button" id="btnCancel" value="取消" onclick="closeTK()"  /></span></td></tr>';

    html+="</table>";
    showCon("修改票号",320,150,html);
    $J("#ICon").dialog("option","buttons",{});
}
//修改票号
function UpdateTK(OrderId,Pid,pasName,oldNumber,TravelNumber) {
    var patern=/^\d{3,4}\-?\d{10}$/i;
    var TicketNumber=$J.trim($J("#txtTicketNumber").val());
    if(TicketNumber=="") {
        showdialog("输入票号不能为空！");
        return;
    } else if(TicketNumber==oldNumber) {
        showdialog("输入票号不能和原来相同！");
        return;
    } else if(OrderId=="") {
        showdialog("输入订单号不能为空！");
        return;
    }
    if(!patern.test(TicketNumber)) {
        showdialog("输入票号格式错误！");
        return;
    }
    _show();
    $J.post("OrderDetail.aspx",
                {
                    SsrOpType: escape("UPTK"),
                    OrderId: escape(OrderId),
                    PasID: escape(Pid),
                    PassengerName: escape(pasName),
                    OldNumber: escape(oldNumber),
                    TicketNumber: escape(TicketNumber),
                    TravelNumber: escape(TravelNumber),
                    num: Math.random(),
                    currentuserid: $J("#currentuserid").val()
                },
                function (data) {
                    _hide();
                    $J("#html").dialog("close");
                    showdialog(data,1);
                },"text");
}
//关闭或者取消
function closeTK() {
    $J("#ICon").dialog('close');
}
//验证出票备注
function CPValidate() {
    var flag=true;
    var CPRemark=$J.trim($J("#txtCPRemark").val());
    if(CPRemark=="") {
        showdialog("输入出票备注不能为空！");
        flag=false;
    }
    return flag;
}
//提取票号
function GetDetr(TicketNumber) {
    TicketNumber=$J.trim(TicketNumber);
    var CpyNo=$J.trim($J("#Hid_CpyNo").val());
    var Office=$J.trim($J("#Hid_Office").val());
    var url="../AJAX/GetHandler.ashx";
    var SendIns="DETR:TN/"+TicketNumber;
    var param=
    {
        OpName: escape("Send"),
        CpyNo: escape(CpyNo),
        Office: escape(Office),
        SendIns: escape(SendIns),
        InsType: escape("1"),
        num: Math.random(),
        currentuserid: $J("#currentuserid").val()
    };
    $J.post(url,param,RecvData,"text");
    return false;
}
//提取编码
function GetPNR(pnr) {
    var CpyNo=$J.trim($J("#Hid_CpyNo").val());
    var Office=$J.trim($J("#Hid_Office").val());
    var url="../AJAX/GetHandler.ashx";
    var SendIns="RT"+pnr;
    var param=
    {
        OpName: escape("Send"),
        CpyNo: escape(CpyNo),
        Office: escape(Office),
        SendIns: escape(SendIns),
        InsType: escape("2"),
        num: Math.random(),
        currentuserid: $J("#currentuserid").val()
    };

    $J.post(url,param,RecvData,"text");
    return false;
}
//创建div
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
function showCon(t,w,h,html) {
    var id="ICon";
    CreateDiv(id);
    $J("select").hide();
    $J("#"+id).html(html);
    $J("#"+id).dialog({
        title: t,
        bgiframe: true,
        modal: true,
        width: w,
        height: h,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            $J("select").show();
        },
        buttons: {
            '确定': function () {
                $J(this).dialog('close');
            }
        }
    });
}
//接收数据
function RecvData(data,type) {
    var showData='';
    if(data!="") {
        var strArr=unescape(data).split('#######');
        showData=strArr[1];
        if(strArr==null||strArr.length==1) {
            showData='无数据';
        }
    }
    if(type!=null) {
        if(type==1) {
            var strSkyModel=$J.trim($J("#Hid_SkyWay").val());
            if(strSkyModel!="") {
                var list=eval("("+unescape(strSkyModel)+")")
                if(list!=null&&list.length>0) {
                    var M=list[0];
                    data=M._pnrcontent;
                    showData=data;
                }
            }
        }
    }
    var _html='<textarea id="txtRecvData" rows="25" cols="300" style="color: green; background-color: black;  width: 100%!important; min-width: 600px!important; height: auto; min-height: 400px;" >'+showData+'</textarea>';
    showCon('黑屏数据',735,500,_html);
}
//显示PNR内容
function showPnrCon() {
    var strSkyModel=$J.trim($J("#Hid_SkyWay").val());
    if(strSkyModel!="") {
        var list=eval("("+unescape(strSkyModel)+")")
        if(list!=null&&list.length>0) {
            var M=list[0];
            var html='<table><tr><td>PNR内容:<td><td><textarea id="txtRecvData" rows="15" cols="300" style="color: green; background-color: black;  width: 100%!important; min-width: 150px!important; height: auto; min-height: 150px;" >'+M._pnrcontent+'</textarea><td></tr>';
            html+='<tr><td>PAT内容:<td><td><textarea id="txtRecvData" rows="15" cols="300" style="color: green; background-color: black;  width: 100%!important; min-width: 150px!important; height: auto; min-height: 150px;" >'+M._pat+'</textarea><td></tr>';
            html+='</table>';
            showCon("编码信息",700,500,html);
        }
    }
    return false;
}