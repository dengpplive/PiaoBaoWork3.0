function btnOk(num) {
    if(num=="5") {
        $J("#btnQuery").click();
    }
}
//对话框包含处理
function showdialog(t,f) {
    $J("select").hide();
    $J("#show").html(t);
    $J("#show").dialog({
        title: '提示',
        bgiframe: true,
        height: 180,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            $J("select").show();
        },
        buttons: {
            '确定': function (evt) {
                $J(this).dialog('close');
                var target=evt.srcElement?evt.srcElement:evt.target;
                $J(target).attr("disabled",true);
                if(f=="0") {
                    location.go(-1);
                } else if(f=="1"||f=="2")//修改订单状态成功 审核订单
                {
                    $J("#btnQuery").click();
                }
            }
        }
    });
}

//出发城市选择
function GetFromCode(text,val,sel) {
    var Arr=text.split('-');
    if(Arr.length==2) {
        $J("#Hid_fromCode").val(Arr[0]);
        $J("#Hid_FromCity").val(Arr[1]);
    } else {
        $J("#Hid_fromCode").val("");
        $J("#Hid_FromCity").val("");
    }
}
//到达城市选择
function GetToCode(text,val,sel) {
    var Arr=text.split('-');
    if(Arr.length==2) {
        $J("#Hid_toCode").val(Arr[0]);
        $J("#Hid_ToCity").val(Arr[1]);
    } else {
        $J("#Hid_toCode").val("");
        $J("#Hid_ToCity").val("");
    }
}


//-----验证--------------------------------------------------------------------
//清空数据
function ClearData() {
    $J("input[type='text']").attr("value","");
    $J("select option").eq(0).attr("selected",true);
}

///---------------------------------------------------
function ShowDiv(id) {

    $J("#processDiv_"+id).show();
    $J("#showOP_"+id).hide();

    return false;
}

function HideDiv(id) {
    $J("#processDiv_"+id).hide();
    $J("#showOP_"+id).show();
    return false;
}
//数字验证【0-100.00】
function NumVate() {
    try {
        var value=$J.trim($J(this).val());
        if(value==null||value=='') {
            showdialog("文本框不能为空,请输入正确的数字!");
            $J(this).val("0");
            return false;
        }
        if(!isNaN(value)) {
            var userreg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
            if(userreg.test(value)) {
                //验证通过
                if(parseFloat(value).toString().length>5) {
                    $J(this).val("0");
                    showdialog("输入数据超出范围!");
                    return false;
                }
            } else {
                var numindex=parseInt(value.indexOf("."),10);
                if(numindex==0) {
                    $J(this).val("0");
                    showdialog("输入的数字不规范");
                    return false;
                }
                var head=value.substring(0,numindex);
                var bottom=value.substring(numindex,numindex+3);
                var fianlNum=head+bottom;
                $J(this).val(fianlNum);
            }
        } else {
            $J(this).val("0");
            showdialog("请输入数字");
            return false;
        }
    } catch(e) {
        alert(e.message);
    }
}
function validate() {
    var isRe=true;
    //角色
    var val_RoleType=$J.trim($J("#Hid_RoleType").val());
    if(val_RoleType=="1") {
        var selVal=$J.trim($J("#ddlGY").val());
        if(selVal=="") {
            showdialog("请选择供应商或者落地运营商，在查询!");
            isRe=false;
        }
    }
    return isRe;
}

//管理员设置
function SetHid(obj) {
    try {
        var val=$J.trim($J(obj).val());
        var text=$J(obj).find("option:selected").text();
        if(val!=="") {
            var CpyNo=val.split('@')[0];
            var LoginName=val.split('@')[1];
            $J("#Hid_CPCpyNo").val(CpyNo)
            $J("#Hid_LoginName").val(LoginName);
        }
    } catch(e) {
        alert(e.message);
    }
}
//拒绝和审核提交 订单
function Update(id,flag) {
    try {
        var url="../AJAX/CommonAjAx.ashx";
        var val_OpFunction="lineOrderProcess";//线下订单处理
        var val_OpType="2";//修改操作   
        var val_OpPage="LineOrderProcess.aspx";
        //订单号
        var OrderId=$J.trim($J("#hid_OrderId_"+id).val());
        //出票公司编号
        var CPCpyNo=$J.trim($J("#CPCpyNo_"+id).val());
        //登录账号
        var LoginName=$J.trim($J("#Hid_LoginName").val());
        //角色
        var val_RoleType=$J.trim($J("#Hid_RoleType").val());
        var param={
            OpFunction: escape(val_OpFunction),
            OpType: escape(val_OpType),
            OpPage: escape(val_OpPage),
            num: Math.random(),
            currentuserid: $J("#currentuserid").val()
        };
        if(flag=="0") {
            //拒绝审核
            var val_Flag="1";//拒绝审核修改订单状态
            var val_orderStatusCode='28';
            param.Id=escape(id);
            param.Status=escape(val_orderStatusCode);
            param.Flag=escape(val_Flag);
        }
        else if(flag=="1") {
            //提交审核
            if(CPCpyNo=="") {
                showdialog("出票公司编号不能为空!");
                return false;
            }
            var val_Flag="2";//提交审核
            //手动录入
            var IsHand=$J("input[type='checkbox'][id*='ckIsHandInput_"+id+"']").is(":checked")?"1":"0";
            //不算成人
            var NotAdult=$J("input[type='checkbox'][id*='ck_NotAdult_"+id+"']").is(":checked")?"1":"0";
            var Pnr=$J.trim($J("#txtPnr_"+id).val());
            //Office
            var Office=$J.trim($J("#txtOffice_"+id).val());
            if(val_RoleType=="1") {
                if(CPCpyNo==""||LoginName=="") {
                    showdialog("请选择供应商或者落地运营商!");
                    return false;
                }
            }
            //PNR
            if(Pnr=="") {
                showdialog("输入PNR不能为空!");
                return false;
            } if(Pnr=="ooooo"||Pnr=="oooooo") {
                showdialog("输入PNR格式错误!");
                return false;
            }
            if(IsHand=="0") {
                if(Office!="") {
                    var pOffice=/^[A-Za-z]{3}[0-9]{3}$/;
                    if(!pOffice.test(Office)) {
                        showdialog("输入Office格式错误!");
                        return false;
                    }
                }
            }
            //政策点数
            var PolicyPoint=0;
            PolicyPoint=$J.trim($J("#txtPolicy_"+id).val());
            if($J.isNaN(PolicyPoint)) {
                showdialog("输入政策点数格式错误!");
                return false;
            }
            if(parseFloat(PolicyPoint,10)>=100||parseFloat(PolicyPoint,10)<0) {
                showdialog("输入政策点数超出范围[0-100]!");
                return false;
            }
            //政策类型
            var PolicyType=$J("select[id='selPolicyType_"+id+"']").val();
            //手动录价           
            var SeatPrice=$J.trim($J("#txtSeatPrice_"+id).val());
            var JJPrice=$J.trim($J("#txtJJPrice_"+id).val());
            var RQPrice=$J.trim($J("#txtRQPrice_"+id).val());
            if($J.isNaN(SeatPrice)) {
                showdialog("输入舱位价必须为数字!");
                return false;
            }
            if($J.isNaN(JJPrice)) {
                showdialog("输入机建费必须为数字!");
                return false;
            }
            if($J.isNaN(RQPrice)) {
                showdialog("输入燃油费必须为数字!");
                return false;
            }
            //--------------添加参数------------
            param.Id=escape(id);//订单id 
            param.RoleType=escape(val_RoleType);
            param.CPCpyNo=escape(CPCpyNo);
            param.LoginName=escape(LoginName);
            param.OrderId=escape(OrderId);
            param.Flag=escape(val_Flag);
            param.IsHand=escape(IsHand);
            param.NotAdult=escape(NotAdult);
            param.Office=escape(Office);
            param.Pnr=escape(Pnr);
            param.PolicyPoint=escape(PolicyPoint);
            param.PolicyType=escape(PolicyType);
            param.SeatPrice=escape(SeatPrice);
            param.JJPrice=escape(JJPrice);
            param.RQPrice=escape(RQPrice);
            //--------------------------
        }
        //禁用按钮
        $J("#a_jujue_"+id).attr("disabled",true);
        var target=event.srcElement?event.srcElement:event.target;
        $J(target).attr("disabled",true);
        $J.post(url,param,function Handle(data) {
            $J("#a_jujue_"+id).attr("disabled",false);
            //启用           
            $J(target).attr("disabled",false);

            //处理数据
            var strReArr=data.split('##');
            if(strReArr.length==3) {
                //错误代码
                var errCode=strReArr[0];
                //错误描述
                var errDes=strReArr[1];
                //错误结果
                var result=$J.trim(unescape(strReArr[2]));
                if(errCode=="1") {
                    //处理成功
                    if(flag=="0") {
                        showdialog("拒绝成功！","1");
                    } else if(flag=="1") {
                        showdialog(errDes,"2");
                    }
                } else {
                    //失败
                    if(flag=="0"||flag=="1") {
                        showdialog(errDes);
                    }
                }
            }
        },"text");
    } catch(e) {
        alert(e.message);
    }
    return false;
}

//加载。。。
$J(function () {
    //手动录价
    $J("input[type='checkbox'][id*='ckIsHandInput_']").click(function () {
        var id=$J(this).attr("id").replace("ckIsHandInput_","");
        if($J(this).is(":checked")) {
            $J("#tab_pnr_"+id+" tr[name='price']").show();
        } else {
            $J("#tab_pnr_"+id+" tr[name='price']").hide();
        }
    });
    //提交
    $J("a[id*='btnCommit_']").click(function () {
        var id=$J(this).attr("id").replace("btnCommit_","");
        Update(id,1);
        return false;
    });
    //取消
    $J("a[id*='btnCancel_']").click(function () {
        var id=$J(this).attr("id").replace("btnCancel_","");
        HideDiv(id);
        return false;
    });
    //编码验证
    $J("input[type='text'][id*='txtPnr_'").blur(function () {
        var val=$J.trim($J(this).val());
        if(val!="") {
            var p=/^[A-Za-z0-9]{5,6}$/;
            if(!p.test(val)||val=="ooooo"||val=="oooooo") {
                showdialog("编码格式错误！");
            }
        }
    });
    //政策点数
    $J("input[type='text'][id*='txtPolicy_'").blur(NumVate);
    //舱位价
    $J("input[type='text'][id*='txtSeatPrice_'").blur(NumVate);
    //机建费
    $J("input[type='text'][id*='txtJJPrice_'").blur(NumVate);
    //燃油费
    $J("input[type='text'][id*='txtRQPrice_'").blur(NumVate);
})