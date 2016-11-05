//显示消息
function showMsg(t,param) {

    if(param!=null) {
        if(param.t==1) {
            //更改数据
            try {
                var model=param.m;
                if(model!=null) {
                    if(model._DataFlag!="0") {
                        t=model._Msg;//操作内容
                    }
                    var rtdata=model._PnrContent;
                    var patdata=model._PATContent;
                    var office=model._Office;
                    var rtpat=rtdata+"\r\n"+patdata;
                    jQuery("#Hid_Office").val(office);
                    var IsMerge=jQuery("#Hid_PnrConIsAll").val();
                    if(IsMerge=="1") {
                        jQuery("#txtPNRAndPata").val(rtpat);
                    } else {
                        jQuery("#txtPNRInfo").val(rtdata);
                        jQuery("#txtPATAInfo").val(patdata);
                    }
                }
            } catch(e) {
            }
        } else if(param.t==2) {
            //提示
            t=unescape(t);
            //获取实体
            var model=eval("("+unescape(param.code)+")");
            var SecondPM=model.SecondPM;
            var rtdata=SecondPM._PnrContent;
            var patdata=SecondPM._PATContent;
            // var office=model._Office;
            var rtpat=rtdata+"\r\n"+patdata;
            if(model.IsMerge=="1") {
                jQuery("#txtPNRAndPata").val(rtpat);
            } else {
                jQuery("#txtPNRInfo").val(rtdata);
                jQuery("#txtPATAInfo").val(patdata);
            }
        }
    }
    jQuery("#show").html(t);
    jQuery("#show").dialog({
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

                var orderid=jQuery.trim(jQuery("#adultOrderId").val());
                if(param!=null) {
                    if(param.t==1) {
                        if(param.m!=null) {
                            var model=param.m;
                            if(model._DataFlag=="1") {
                                //证件号
                                var list=[];
                                var isPass=true;
                                //处理
                                jQuery("#PasTab tr").each(function (a,b) {
                                    var tr=jQuery(b);
                                    var paName=tr.find("td:eq(1)").text();
                                    var paCid=jQuery.trim(tr.find("input[name='PasCid']").val().replace('|',''));
                                    //乘客类型
                                    var pasType=jQuery.trim(tr.find("input[name='PasCid']").attr("pasType"));
                                    if(paCid=="") {
                                        isPass=false;
                                        tr.find("span[id*='msg']").html("<font style='color:red'>* 证件号不能为空</font>");
                                        return false;
                                    }
                                    //var p = /^\d{4}\-\d{2}\-\d{2}$/i;
                                    if(paCid.length>20)// || (!(p.test(paCid) && paCid.length == 10))) {
                                    {
                                        isPass=false;
                                        tr.find("span[id*='msg']").html("<font style='color:red'>* 证件号格式错误,长度不能超过20个字符。</font>");
                                        return false;
                                    } else {
                                        if(pasType=="3") {
                                            var p=/^\d{4}\-\d{2}\-\d{2}$/i;
                                            if(!p.test(paCid)) {
                                                tr.find("span[id*='msg']").html("<font style='color:red'>婴儿证件号格式错误,正确格式为:年-月-日</font>");
                                                isPass=false;
                                                return false;
                                            } else {
                                                tr.find("span[id*='msg']").html("");
                                            }
                                        } else {
                                            tr.find("span[id*='msg']").html("");
                                        }
                                    }
                                    list.push(paName+'|'+paCid);
                                });
                                if(list.length>0) {
                                    param.m.pasStr=list.join('@@@');
                                }
                                if(!isPass) {
                                    return;
                                }
                            }
                            else if(model._DataFlag=="3") {
                                if(orderid=="") {
                                    jQuery("#msgOrderId").html("<font class='red'>输入订单号/编码不能为空！</font>");
                                    return;
                                }
                                //儿童关联成人订单号
                                model.orderId=orderid;
                            }
                        }
                        jQuery("#show").dialog('close');
                        ShowDiv(true);
                        //继续导入
                        StartImport(param.TongDao,param);
                    } else if(param.t==2) {
                        if(orderid=="") {
                            if(jQuery("#msgOrderId")[0]!=undefined) {
                                jQuery("#msgOrderId").html("<font class='red'>输入订单号/编码不能为空！</font>");
                            } else {
                                jQuery("#show").dialog('close');
                            }
                            return;
                        }
                        jQuery("#Hid_OrderId").val(orderid);
                        jQuery("#show").dialog('close');
                        if(param.type==0) {
                            //pnr
                            jQuery("#btnH_PNRImport").click();
                        } else if(param.type==1) {
                            //内容
                            jQuery("#btnH_PNRConImport").click();
                        }
                    }
                } else {
                    jQuery("#show").dialog('close');
                }
            },'取消': function () {
                jQuery(this).dialog('close');
            }
        }
    }).css({ "width": "auto","height": "auto" });
}
//显示隐藏遮罩层 true 显示 false隐藏
function ShowDiv(flag) {
    if(flag) {
        jQuery("#overlay").show();
        jQuery("#loading").show();
    } else {
        jQuery("#overlay").hide();
        jQuery("#loading").hide();
    }
}
//类型选择
function SelectImportType() {
    var ImportType=jQuery("input[name='ImportType'][type='radio']:checked").val();
    var CHDOPENAsAdultOrder=jQuery("#Hid_CHDOPENAsAdultOrder").val();
    if(ImportType=="2") {
        if(CHDOPENAsAdultOrder=="1") {
            jQuery("span[id='Span_CHD']").show();
        } else {
            jQuery("span[id='Span_CHD']").hide();
        }
    } else {
        jQuery("span[id='Span_CHD']").hide();
    }
}
//采购分销导入
function BuyImport() {
    var ImportType=jQuery("input[name='ImportType'][type='radio']:checked").val();
    StartImport(ImportType);
    return false;
}


//开始导入编码
function StartImport(TongDao,SecondRequest) {
    try {
        var CHDOPENAsAdultOrder=jQuery("#Hid_CHDOPENAsAdultOrder").val();
        //角色类型
        var RoleType=jQuery.trim(jQuery("#Hid_UserRoleType").val());
        var pnr="";
        var bigpnr=jQuery.trim(jQuery("#txtPNR").val());
        var _RTAndPat=jQuery.trim(jQuery("#txtPNRAndPata").val());
        var _RTData=jQuery.trim(jQuery("#txtPNRInfo").val()),_PatData=jQuery.trim(jQuery("#txtPATAInfo").val());
        //RT和Pat是否合并
        var IsMergeVal=jQuery("#Hid_PnrConIsAll").val();
        var _OrderId="";//订单号
        var tempIsSecond=0;
        var PasList="";
        //第二次调用
        if(SecondRequest!=null) {
            tempIsSecond=SecondRequest.t;
            var model=SecondRequest.m;
            if(model._DataFlag=="3"&&model!=null) {
                _OrderId=model.orderId;//关联成人订单号
            } else if(model._DataFlag=="1"&&model!=null) {
                //证件号
                PasList=escape(model.pasStr);
            }
        }
        var pnrPatern=/^[A-Za-z0-9]{6}$/i;
        var errMsg='';
        var _IsSecond=tempIsSecond;
        if(TongDao=="4") {
            //Pnr内容导入
            if(IsMergeVal=="1") {
                _RTAndPat=jQuery.trim(jQuery("#txtPNRAndPata").val());
                if(_RTAndPat=="") {
                    errMsg="输入RT和PAT数据不能为空！";
                }
            } else {
                _RTData=jQuery.trim(jQuery("#txtPNRInfo").val());
                _PatData=jQuery.trim(jQuery("#txtPATAInfo").val());
                if(_RTData=="") {
                    errMsg="输入RT数据不能为空！";
                } else if(_PatData=="") {
                    errMsg="输入PAT数据不能为空！";
                }
            }
            if(errMsg=="") {
                _RTAndPat=_RTAndPat;
                _RTData=_RTData;
                _PatData=_PatData;
            }
        } else if(TongDao=="5") {//升舱换开通道
            pnr=jQuery.trim(jQuery("#txtNewPNR").val());
            _OrderId=jQuery.trim(jQuery("#txtOldOrderNo").val());
            if(pnr=="") {
                errMsg="输入新PNR编码不能为空！";
            }
            else if(_OrderId=="") {
                errMsg="输入原订单号不能为空！";
            }
            if(errMsg=="") {
                if(!pnrPatern.test(pnr)) {
                    errMsg="输入新PNR编码格式有误！";
                }
            }
        } else if(TongDao=="2") {//儿童编码
            pnr=jQuery.trim(jQuery("#txtPNR").val());
            _OrderId=jQuery.trim(jQuery("#txtPernsOrder").val());
            if(pnr=="") {
                errMsg="输入儿童编码不能为空！";
            }
            else if(_OrderId==""||_OrderId=="成人订单号") {
                if(CHDOPENAsAdultOrder=="1") {
                    errMsg="输入成人订单号不能为空！";
                }
            }
            if(errMsg=="") {
                if(!pnrPatern.test(pnr)||pnr=="ooooo"||pnr=="oooooo") {
                    errMsg="输入儿童编码格式有误！";
                }
            }
        }
        else {
            if(TongDao=="0") {
                pnr=jQuery.trim(jQuery("#txtPNR").val());
            } else if(TongDao=="1") {
                pnr=jQuery.trim(jQuery("#txtPNR").val());
            } else if(TongDao=="3") {
                bigpnr=jQuery.trim(jQuery("#txtPNR").val());
            }
            if(TongDao=="3") {
                if(bigpnr=="") {
                    errMsg="输入大编码不能为空！";
                }
                else if(!pnrPatern.test(bigpnr)||bigpnr=="ooooo"||bigpnr=="oooooo") {
                    errMsg="输入大编码格式有误！";
                }
            } else {
                if(pnr=="") {
                    errMsg="输入编码不能为空！";
                }
                else if(!pnrPatern.test(pnr)||pnr=="ooooo"||pnr=="oooooo") {
                    errMsg="输入编码格式有误！";
                }
            }
        }
        //是否允许换编码出票 0允许 1不允许
        var AllowChangePNR_Val=jQuery("#chkChangePnr").attr("checked")?"1":"0";
        if(errMsg=="") {
            //发送请求
            //隐藏
            ShowDiv(true);
            //单击按钮
            var pnrImportBtn=event.srcElement?event.srcElement:event.target;

            var url="../AJAX/PnrImport.ashx";
            var Param={
                OPPage: "Import",
                RoleType: RoleType,
                AllowChangePNR: AllowChangePNR_Val,
                ImportTongDao: TongDao,
                IsMerge: IsMergeVal,
                IsSecond: _IsSecond,
                Pnr: pnr,
                BigPnr: bigpnr,
                OrderId: _OrderId,
                RTAndPAT: encodeURIComponent(_RTAndPat),
                RTData: encodeURIComponent(_RTData),
                PATDATA: encodeURIComponent(_PatData),
                Office: jQuery("#Hid_Office").val(),
                num: Math.random(),
                currentuserid: jQuery("#currentuserid").val()
            };
            if(PasList!="") {
                //添加新参数
                Param.PasList=PasList;
            }
            function Handle(data) {
                //启用按钮
                jQuery(pnrImportBtn).attr("disabled",false);
                try {
                    var model=eval("("+data+")");
                    if(model!=null&&model!=undefined) {
                        try {
                            //处理数据                   
                            if(model._ErrCode=="1") {
                                //成功 加入订单号
                                location.href=model._GoUrl+"&currentuserid="+jQuery("#currentuserid").val();
                            } else {
                                if(model._OpType=="1") {
                                    //提示和操作
                                    showMsg(model._Msg,{ t: 1,TongDao: TongDao,m: model });
                                } else {
                                    //提示
                                    showMsg(model._Msg);
                                }
                            }
                        } catch(e) {
                            showMsg("Pnr导入失败!!");
                        }
                    } else {
                        showMsg("Pnr导入失败!");
                    }
                } catch(e) {
                }
                //显示
                ShowDiv(false);
            }
            //禁用按钮
            jQuery(pnrImportBtn).attr("disabled",true);
            jQuery.post(url,Param,Handle,"text");
        } else {
            //显示错误提示
            showMsg(errMsg);
        }
    } catch(e) {
        alert(e.message);
    }
    return false;
}

//-------------------------------------------------
function GetKehu(UninCode) {

    jQuery("#btnH_PNRImport").attr("disabled",true);
    jQuery("#btnH_PNRConImport").attr("disabled",true);
    var url="../AJAX/PnrImport.ashx";
    var Param={
        OPPage: "KeHu",
        UninCode: UninCode,
        num: Math.random(),
        currentuserid: jQuery("#currentuserid").val()
    };
    function Handle(data) {

        jQuery("#btnH_PNRImport").attr("disabled",false);
        jQuery("#btnH_PNRConImport").attr("disabled",false);
        var result=jQuery.trim(data);
        jQuery("#selKH_0").html(result);
    }
    jQuery.post(url,Param,Handle,"text");
}
function contains(string,substr,isIgnoreCase) {
    if(isIgnoreCase) {
        string=string.toLowerCase();
        substr=substr.toLowerCase();
    }
    var startChar=substr.substring(0,1);
    var strLen=substr.length;
    for(var j=0;j<string.length-strLen+1;j++) {
        if(string.charAt(j)==startChar)
        //如果匹配起始字符,开始查找 
        {
            if(string.substring(j,j+strLen)==substr)
            //如果从j开始的字符与str匹配那ok 
            {
                return true;
            }
        }
    } return false;
}
//供应和客户选择
function ddlSetText(ddlObj,flag,num) {

    var ddlVal=jQuery.trim(jQuery(ddlObj).val()).split('@')[1];
    ddlObj.a=1;
    if(flag=="txtCompany") {
        jQuery("#"+flag+"_"+num).val(ddlVal);
        var UninCode=jQuery.trim(jQuery(ddlObj).val()).split('@')[0];
        //获取客户
        GetKehu(UninCode);
        jQuery("#Hid_GY").val(jQuery(ddlObj).val());
    } else if(flag=="txtkehu") {
        jQuery("#"+flag+"_"+num).val(ddlVal);
        jQuery("#Hid_KH").val(jQuery(ddlObj).val());
    }
    ddlObj.a=0;
}

function txtSetSel(txtObj,flag,num) {

    var txtVal=jQuery(txtObj).val();
    var ddlsel=jQuery("#"+flag+"_"+num)[0];
    if(txtVal=="") {
        jQuery("#"+flag+"_"+num+" option").eq(0).attr("selected",true);
    }
    if(ddlsel==null||ddlsel.a!=1) {
        if(txtVal!="") {
            //jQuery("#"+flag+"_"+num+" option[value*='"+txtVal+"']").attr("selected",true);
            //jQuery("#"+flag+"_"+num+" option:contains('"+txtVal+"')").attr("selected",true);
            var opVal='';
            var jop=null;
            jQuery("#"+flag+"_"+num+" option").each(function (index,op) {
                opVal=jQuery(op).text();
                if(contains(opVal,txtVal,true)) {
                    jop=jQuery(op);
                    return false;
                }
            });
            if(jop!=null) {
                jop.attr("selected",true);
            }

        }
        if(flag=="selGY") {
            var UninCode=jQuery.trim(jQuery("#selGY_0").val()).split('@')[0];
            //获取客户
            GetKehu(UninCode);
            jQuery("#Hid_GY").val(jQuery("#selGY_"+num).val());
        } else if(flag=="selKH") {
            jQuery("#Hid_KH").val(jQuery("#selKH_"+num).val());
        }
    }
}
function returnFG(obj) {
    var fg=HImport(obj);
    if(fg) {
        ShowDiv(true);
    }
    return fg;
}
//导入
function HImport(obj) {
    var IsSuc=true;
    try {
        var RoleType=jQuery.trim(jQuery("#Hid_UserRoleType").val());
        var GY=jQuery.trim(jQuery("#Hid_GY").val());
        var KH=jQuery.trim(jQuery("#Hid_KH").val());
        if(RoleType=="1") {
            if(GY=="") {
                showMsg("请选择供应商或者落地运营商");
                IsSuc=false;
                return IsSuc;
            }
        }
        if(KH=="") {
            showMsg("请选择客户");
            IsSuc=false;
            return IsSuc;
        }
        if(obj!=null) {
            if(obj.source=="1"||obj.source=="3") {
                //编码导入
                var pnr=(obj.source=="3")?jQuery.trim(jQuery("#txtH_PNR3").val()):jQuery.trim(jQuery("#txtH_PNR").val());
                if(pnr=="") {
                    showMsg("输入编码不能为空！");
                    IsSuc=false;
                    return IsSuc;
                }
                var pnrPatern=/^[A-Za-z0-9]{6}$/i;
                if(!pnrPatern.test(pnr)||pnr=="ooooo"||pnr=="oooooo") {
                    showMsg("输入编码格式错误！");
                    IsSuc=false;
                    return IsSuc;
                }
                if(obj.source=="1") {
                    //是否为大编码
                    jQuery("#Hid_IsBigCode").val(jQuery("#ckH_Big").is(":checked")?"1":"0");
                } else {
                    jQuery("#Hid_IsBigCode").val("0");
                }
            } else if(obj.source=="2") {
                var IsMerge=jQuery("#Hid_PnrConIsAll").val();
                var pnrCon=jQuery.trim(jQuery("#pnrCon").val());
                var patCon=jQuery.trim(jQuery("#patCon").val())
                //是否合并
                if(IsMerge=="1") {
                    //内容导入 
                    pnrCon=jQuery.trim(jQuery("#txtPNRAndPata").val());
                    patCon=jQuery.trim(jQuery("#txtPNRAndPata").val())
                }
                if(pnrCon=="") {
                    showMsg("输入Pnr内容不能为空！");
                    IsSuc=false;
                    return IsSuc;
                }
            }
        }
    } catch(e) {
        alert(e.message);
    }
    return IsSuc;
}

//加载。。
jQuery(function () {
    var CHDOPENAsAdultOrder=jQuery("#Hid_CHDOPENAsAdultOrder").val();
    jQuery("#txtPernsOrder").focus(function () {
        if(jQuery.trim(jQuery(this).val())=="成人订单号") {
            jQuery(this).val("");
            jQuery(this).css({ "color": "black" });
        }
    }).blur(function () {
        if(jQuery.trim(jQuery(this).val())=="") {
            jQuery(this).val("成人订单号");
            jQuery(this).css({ "color": "gray" });
        }
    });

    var RoleType=jQuery.trim(jQuery("#Hid_UserRoleType").val());
    if(RoleType!="") {
        if(parseInt(RoleType,10)==1) {
            jQuery("#tr_gy").show();
        } else {
            jQuery("#tr_gy").hide();
        }
    }
})
