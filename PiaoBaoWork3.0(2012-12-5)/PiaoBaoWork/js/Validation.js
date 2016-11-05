function Validation(dyName, yzName, tsName) {
    //验证账号--非空
    if (dyName == "LoginName") {
        //debugger;
        $("#" + tsName).html("");
        //var reg = /^\w{4,18}$/;

        var reg = /^([a-zA-Z0-9]|[._-]|[^x00-xff]){2,18}$/;
        if ($("#" + yzName).val() == "") {
            $("#" + tsName).html("<font color=red>必填</font>");
            //$("#" + yzName).focus();
            return false;
        }
        else if (!reg.test($("#" + yzName).val())) {
            $("#" + tsName).html("<font color=red>长度必须在2-18位之间且不能有特殊符号</font>");
            //$("#" + yzName).focus();
            return false;
        }
        else {
            var account = $("#" + yzName).val();

            $.post("../Ajax/RemoteCheck.ashx", { account: escape(account), currentuserid:$("#currentuser").val(),rom:Math.random()},
                                    function (data) {
                                        var r = data;
                                        if (r == "1") {
                                            $("#" + tsName).html("<font color=red>账号存在</font>");
                                            return false;
                                        } else {
                                            $("#" + tsName).html("<font color=green>账号可用</font>");
                                        }
                                    }, "text"
                );
        }
    }
    //验证密码--非空
    else if (dyName == "Password") {
        $("#" + tsName).html("");
        var reg = /^\w{4,18}$/;
        if ($("#" + yzName).val() == "") {
            $("#" + tsName).html("<font color=red>必填</font>");
            //$("#" + yzName).focus();
            return false;
        }
        else if (!reg.test($("#" + yzName).val())) {
            $("#" + tsName).html("<font color=red>只能输入字母数字或字符，且长度必须在4-18位之间</font>");
            //$("#" + yzName).focus();
            return false;
        }
    }
    //验证手机--非空
    else if (dyName == "Mobile") {
        $("#" + tsName).html("");
        var reg = /^\d{11}/;
        if ($("#" + yzName).val() == "") {
            $("#" + tsName).html("<font color=red>必填</font>");
            //$("#" + yzName).focus();
            return false;
        }
        else if (!reg.test($("#" + yzName).val())) {
            $("#" + tsName).html("<font color=red>只能输入数字，且长度必须在11位</font>");
            //$("#" + yzName).focus();
            return false;
        }
    }
    //验证电话--非空
    else if (dyName == "Tel") {
        $("#" + tsName).html("");
        var reg = /^\d{3,5}-?\d{6,8}$/;
        if ($("#" + yzName).val() == "") {
            $("#" + tsName).html("<font color=red>必填</font>");
            //$("#" + yzName).focus();
            return false;
        }
        else if (!reg.test($("#" + yzName).val())) {
            $("#" + tsName).html("<font color=red>电话号码请加区号。区号3-5位，电话6-8位。</font>");
            //$("#" + yzName).focus();
            return false;
        }
    }
    //验证字母--非空
    else if (dyName == "IsABC") {
        $("#" + tsName).html("");
        var reg = /^[A-Za-z]/;
        if ($("#" + yzName).val() == "") {
            $("#" + tsName).html("<font color=red>必填</font>");
            //$("#" + yzName).focus();
            return false;
        }
        else if (!reg.test($("#" + yzName).val())) {
            $("#" + tsName).html("<font color=red>只能输入字母</font>");
            //$("#" + yzName).focus();
            return false;
        }
    }
    //验证汉字--非空
    else if (dyName == "IsChinese") {
        $("#" + tsName).html("");
        var str = $("#"+yzName).val();
        var reg = /[\u4E00-\u9FA5\uF900-\uFA2D]/;
        if ($("#" + yzName).val() == "") {
            $("#" + tsName).html("<font color=red>必填</font>");
            //$("#" + yzName).focus();
            return false;
        }
        else if (!reg.test($("#" + yzName).val())) {
            $("#" + tsName).html("<font color=red>只能输入汉字</font>");
            //$("#" + yzName).focus();
            return false;
        } 
        else if (str.length > 13) {
            $("#" + tsName).html("<font color=red>长度不能超过13个汉字</font>");
            return false;
         }
    }



    //验证邮箱
    else if (dyName == "Email") {
        $("#" + tsName).html("");
        if ($("#" + yzName).val() != "") {
            var reg = /\w@\w*\.\w/;
            if (!reg.test($("#" + yzName).val())) {
                $("#" + tsName).html("<font color=red>邮箱格式不正确</font>");
                //$("#" + yzName).focus();
                return false;
            }
        }
    }
    //验证非空
    else if (dyName == "IsNotNull") {
        $("#" + tsName).html("");
        if ($("#" + yzName).val() == "") {
            $("#" + tsName).html("<font color=red>必填</font>");
            //$("#" + yzName).focus();
            return false;
        }
    }


    return true;
}