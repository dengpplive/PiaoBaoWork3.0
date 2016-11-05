//显示HTML的对话框
function showHtml(html,t,w,h) {
    $J("select").hide();
    $J("#divDG").html(html);
    $J("#divDG").dialog({
        title: t==null?'提示':t,
        bgiframe: true,
        height: h,
        width: w,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            $J("select").show();
            $J("divDG").remove();
        }
    }).css({ "width": "auto","height": "auto" });
    //防止出现按钮
    $J("#divDG").dialog("option","buttons",{});
}
//获取该订单的公司和用户信息
function GetUserInfo(orderId) {
    var div=document.getElementById("divDG");
    if(div==null) {
        div=document.createElement("div");
        div.id="divDG";
        if(document.all) {
            document.body.appendChild(div);
        }
        else {
            document.insertBefore(div,document.body);
        }
    }
    if($J.trim(orderId)!="") {
        var url="../AJAX/GetHandler.ashx";
        var param={
            OpName: escape("GetUser"),
            OrderId: escape(orderId),
            num: Math.random(),
            currentuserid: $J("#currentuserid").val()
        };
        $J.post(url,param,function (data) {
            data=unescape(data);
            if(data!="") {
                var dataArr=data.split('#######');
                if(dataArr.length>1) {
                    var model=eval("("+dataArr[1]+")");
                    if(model!=null) {
                        var Employees=model.m_Employees;
                        var Company=model.m_Company;
                        var YeWuYan=model.m_YeWuYan;
                        //显示具体信息
                        var html='<table cellpadding="5" cellspacing="1" border="1" bordercolor="#000000" style="border-collapse:collapse">';
                        html+='<tr style="height:30px;"><th style="width:40%;">公司名称：</th><td style="width:60%;">'+Company._uninallname+'</td></tr>';
                       // html+='<tr style="height:30px;"><th style="width:40%;">公司编号：</th><td style="width:60%;">'+Company._unincode+'</td></tr>';
                        html+='<tr style="height:30px;"><th style="width:40%;">客户账号：</th><td style="width:60%;">'+Employees._loginname+'</td></tr>';
                        html+='<tr style="height:30px;"><th style="width:40%;">公司电话：</th><td style="width:60%;">'+Company._tel+'</td></tr>';
                        html+='<tr style="height:30px;"><th style="width:40%;">联系人：</th><td style="width:60%;">'+Company._contactuser+'</td></tr>';
                        html+='<tr style="height:30px;"><th style="width:40%;">联系人电话：</th><td style="width:60%;">'+Company._contacttel+'</td></tr>';
                        html+='<tr style="height:30px;"><th style="width:40%;">所属业务员：</th><td style="width:60%;">'+(YeWuYan!=null?YeWuYan._username:"")+'</td></tr>';
                        html+='</table>';
                        showHtml(html,"客户信息",250,300);
                    }
                } else {
                    showdialog("获取客户信息失败！");
                }
            } else {
                showdialog("获取客户信息失败!!");
            }
        },"text");
    }
    return false;
}