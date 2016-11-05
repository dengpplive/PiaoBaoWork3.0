//扩展新方法
String.prototype.NewReplace=function (sourceData,replaceData) {
    sourceData=sourceData.replace("(","\\(").replace(")","\\)");
    var reg=new RegExp(sourceData,"ig");
    var data=this.replace(reg,replaceData);
    return data;
}
//初始化全局数组
var gArr=[];//值为1表示序号已使用 0表示未使用
function initArr(num) {
    for(var i=1;i<=num;i++) {
        eval("var obj={_"+i+":0}");
        gArr.push(obj);
    }
    return gArr;
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
//添加
function AddRob(evt) {
    if(evt!=null) {
        var target=evt.srcElement?evt.srcElement:evt.target;
        $(target).attr("disabled",true);
    }
    //用于序号显示
    var TrLen=$("#robTab tbody tr[id*='trrob_']").length;
    if((TrLen+1)<=maxNum) {
        var TrCount=getMinFg(gArr);
        //标记为已使用
        setFg(gArr,TrCount,"1");
        //克隆
        var cloneTr=$("#robTab thead tr[id='trrob_0']").clone(true);
        var tr_html=$("<div></div>").append(cloneTr).html();

        tr_html=tr_html.NewReplace("trrob_0","trrob_"+TrCount).NewReplace("txtscanTime_0","txtscanTime_"+TrCount).NewReplace("robscancount_0","robscancount_"+TrCount);
        var opDiv='<div id="op_div_'+TrCount+'"><a onclick="AddRob();return false;" href="#" >添加</a>&nbsp;&nbsp;&nbsp;<a  href="#" onclick="return removeGroup('+TrCount+');">移除</a></div>';
        var newTr='<tr id="trrob_'+TrCount+'">'+$(tr_html).find("td:last").html(opDiv).parent().html()+'</tr>';
        //添加
        $("#robTab tbody").append(newTr);
        //重置序号
        $("#robTab tbody tr[id*='trrob_']").each(function (index,tr) {
            $(tr).find("span[id *='robscancount_']").html("第"+(index+1)+"次");
        });
        $("#robTab tbody td").css({ "text-align": "center" });

        if(evt!=null) {
            var target=evt.srcElement?evt.srcElement:evt.target;
            $(target).attr("disabled",false);
        }
        return TrCount;
    } else {
        showdialog("最大可以扫描"+maxNum+"次");
    }
}
//移除一行
function removeGroup(num) {
    var TrLen=$("#robTab tbody tr[id*='trrob_']").length;
    if(TrLen<=1) {
        showdialog("必须有一组扫描时间！");
    } else {
        if(num!=null) {
            //具体删除
            $('#robTab tbody tr[id="trrob_'+num+'"]').remove();
        }
        //标记为没有使用
        setFg(gArr,num,"0");
        //重置序号  
        $("#robTab tbody tr[id*='trrob_']").each(function (index,tr) {
            $(tr).find("span[id *='robscancount_']").html("第"+(index+1)+"次");
        });
    }
    return false;
}
//处理
function handle() {
    //原来验证
    var isuc=valall();
    if(isuc) {
        //验证通过 在验证抢票设置
        try {
            var resultArr=[];
            var RobInnerTime=$.trim($("#txtRobMinuteInner").val());
            if($.isNaN(RobInnerTime)) {
                showdialog("抢票持续时间设置格式错误！");
                isuc=false;
            } else {
                var t=0;
                $("#robTab tbody tr[id*='trrob_']").each(function (index,tr) {
                    var time=$(tr).find("input[id*='txtscanTime_']").val();
                    if($.isNaN(time)) {
                        isuc=false;
                        return false;
                    }
                    t+=parseInt(time,10);
                    resultArr.push((index+1)+"^"+time);
                });
                if(!isuc) {
                    showdialog("抢票扫描时间格式错误！");
                    isuc=false;
                } else {
                    if(t>parseInt(RobInnerTime,10)) {
                        showdialog("抢票次数设置时间之和超出抢票时间范围！");
                        isuc=false;
                    }
                    if(resultArr.length>0) {
                        $("#Hid_RobSetting").val(resultArr.join('|'));
                    }
                }
            }
        } catch(e) {
            isuc=false;
        }
    }
    return isuc;
}
var maxNum=10;
$(function () {
    //最大可以添加几组
    initArr(maxNum);
    var m_RobSetting=$.trim($("#Hid_RobSetting").val());
    if(m_RobSetting!="") {
        var strArr=m_RobSetting.split('|');
        for(var i=0;i<strArr.length;i++) {
            if($.trim(strArr[i])!="") {
                var secArr=$.trim(strArr[i]).split('^');
                if(secArr.length==2) {
                    var num=AddRob();
                    $("#txtscanTime_"+num).val(secArr[1]);
                }
            }
        }
    } else {
        AddRob();
    }
});