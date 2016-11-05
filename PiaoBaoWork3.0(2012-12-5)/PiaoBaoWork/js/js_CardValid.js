/**  
* 身份证15位编码规则：dddddd yymmdd xx p   
* dddddd：地区码   
* yymmdd: 出生年月日   
* xx: 顺序类编码，无法确定   
* p: 性别，奇数为男，偶数为女  
* 
* 身份证18位编码规则：dddddd yyyymmdd xxx y   
* dddddd：地区码   
* yyyymmdd: 出生年月日   
* xxx:顺序类编码，无法确定，奇数为男，偶数为女   
* y: 校验码，该位数值可通过前17位计算获得   
* 18位号码加权因子为(从右到左) Wi = [ 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2,1 ]  
* 验证位 Y = [ 1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2 ]   
* 校验位计算公式：Y_P = mod( ∑(Ai×Wi),11 )   
* i为身份证号码从右往左数的 2...18 位; Y_P为脚丫校验码所在校验码数组位置  
*   
*/

var Wi=[7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2,1];// 加权因子   
var ValideCode=[1,0,10,9,8,7,6,5,4,3,2];// 身份证验证位值.10代表X   
//验证调用方法
function IdCardValidate(idCard) {
    var Msg=[
    "身份证号码为空",
    "身份证号码地区非法",
    "身份证号码位数不对",
    "身份证号码出生日期超出范围",
    "身份证号码格式错误"
    ];
    idCard=trim(idCard.replace(/ /g,"")).toUpperCase();
    //为空验证
    if(idCard.length==0) {
        return Msg[0];
    }
    //地区验证
    if(!AreaValidity(idCard)) {
        return Msg[1];
    }
    //位数验证
    if(!CardNo_LenValid(idCard)) {
        return Msg[2];
    }

    //出生日期验证
    if(!CardNoBirthDay(idCard)) {
        return Msg[3];
    }
    if(!Validity(idCard)) {
        return Msg[4];
    }
    return "";//通过
}
//校验码验证
function Validity(idCard) {
    var isValidity=true;
    if(idCard.length==15) {
        isValidity=isValidityBrithBy15IdCard(idCard);
    } else if(idCard.length==18) {
        var a_idCard=idCard.split("");// 得到身份证数组 
        var lastChar=a_idCard[a_idCard.length-1];
        if(isValidityBrithBy18IdCard(idCard)&&isTrueValidateCodeBy18IdCard(idCard,a_idCard)) {
            isValidity=true;
        } else {
            isValidity=false;
        }
    } else {
        isValidity=false;
    }
    return isValidity;
}
/**  
* 判断身份证号码为18位时最后的验证位是否正确  
* a_idCard 身份证号码数组    
*/
function isTrueValidateCodeBy18IdCard(idCard,a_idCard) {
    var sum=0; // 声明加权求和变量   
    if(a_idCard[17].toLowerCase()=='x') {
        a_idCard[17]=10;// 将最后位为x的验证码替换为10方便后续操作   
    }
    for(var i=0;i<17;i++) {
        sum+=Wi[i]*a_idCard[i];// 加权求和   
    }
    valCodePosition=sum%11;// 得到验证码所位置    
    if(a_idCard[17]==ValideCode[valCodePosition]||isTrueValidateCodeBy18IdCard_A(idCard)) {
        return true;
    } else {
        return false;
    }
}
//另一种18位身份证号码校验
function isTrueValidateCodeBy18IdCard_A(idCard) {
    //计算校验位 
    var idcard_array=idCard.split("");
    var S=(parseInt(idcard_array[0])+parseInt(idcard_array[10]))*7
                 +(parseInt(idcard_array[1])+parseInt(idcard_array[11]))*9
                 +(parseInt(idcard_array[2])+parseInt(idcard_array[12]))*10
                 +(parseInt(idcard_array[3])+parseInt(idcard_array[13]))*5
                 +(parseInt(idcard_array[4])+parseInt(idcard_array[14]))*8
                 +(parseInt(idcard_array[5])+parseInt(idcard_array[15]))*4
                 +(parseInt(idcard_array[6])+parseInt(idcard_array[16]))*2
                 +parseInt(idcard_array[7])*1
                 +parseInt(idcard_array[8])*6
                 +parseInt(idcard_array[9])*3;
    var Y=S%11;
    var M="F";
    var JYM="10X98765432";
    /*判断校验位*/
    M=JYM.substr(Y,1);
    //比较最后一位号码
    if(M.toUpperCase()==idcard_array[17].toUpperCase()) {
        return true;
    } else {
        return false;
    }
}
/**  
* 通过身份证判断是男是女  
*  idCard 15/18位身份证号码   
* 'female'-女、'male'-男  
*/
function maleOrFemalByIdCard(idCard) {
    idCard=trim(idCard.replace(/ /g,""));// 对身份证号码做处理。包括字符间有空格。   
    if(idCard.length==15) {
        if(idCard.substring(14,15)%2==0) {
            return 'female';
        } else {
            return 'male';
        }
    } else if(idCard.length==18) {
        if(idCard.substring(14,17)%2==0) {
            return 'female';
        } else {
            return 'male';
        }
    } else {
        return null;
    }
}
/**  
* 验证18位数身份证号码中的生日是否是有效生日  
* idCard 18位书身份证字符串    
*/
function isValidityBrithBy18IdCard(idCard18) {
    var year=idCard18.substring(6,10);
    var month=idCard18.substring(10,12);
    var day=idCard18.substring(12,14);
    var temp_date=new Date(year,parseFloat(month)-1,parseFloat(day));
    // 这里用getFullYear()获取年份，避免千年虫问题   
    if(temp_date.getFullYear()!=parseFloat(year)
          ||temp_date.getMonth()!=parseFloat(month)-1
          ||temp_date.getDate()!=parseFloat(day)) {
        return false;
    } else {
        return true;
    }
}
/**  
* 验证15位数身份证号码中的生日是否是有效生日  
* idCard15 15位书身份证字符串
*/
function isValidityBrithBy15IdCard(idCard15) {
    var year=idCard15.substring(6,8);
    var month=idCard15.substring(8,10);
    var day=idCard15.substring(10,12);
    var temp_date=new Date(year,parseFloat(month)-1,parseFloat(day));
    // 对于老身份证中的你年龄则不需考虑千年虫问题而使用getYear()方法   
    if(temp_date.getYear()!=parseFloat(year)
              ||temp_date.getMonth()!=parseFloat(month)-1
              ||temp_date.getDate()!=parseFloat(day)) {
        return false;
    } else {
        return true;
    }
}
//身份证长度验证
function CardNo_LenValid(idCard) {
    var IsSuc=true;
    idCard=trim(idCard.replace(/ /g,""));
    if(!/^\d{17}(\d|[a-zA-Z])$/i.test(idCard)&&!/^\d{15}$/i.test(idCard)) {
        IsSuc=false;
    }
    return IsSuc;
}

/* 
* 身份证号码出生日期验证   
*/
function CardNoBirthDay(idCard) {
    var IsSuc=true;
    var ereg=null;
    if(idCard.length==15) {
        if((parseInt(idCard.substr(6,2))+1900)%4==0||((parseInt(idCard.substr(6,2))+1900)%100==0&&(parseInt(idCard.substr(6,2))+1900)%4==0)) {
            ereg=/^[1-9][0-9]{5}[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}$/;//测试出生日期的合法性   
        } else {
            ereg=/^[1-9][0-9]{5}[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}$/;//测试出生日期的合法性   
        }
        if(!ereg.test(idCard)) {
            //没有通过
            IsSuc=false;
        }
    } else if(idCard.length==18) {
        if(parseInt(idCard.substr(6,4))%4==0||(parseInt(idCard.substr(6,4))%100==0&&parseInt(idCard.substr(6,4))%4==0)) {
            ereg=/^[1-9][0-9]{5}19[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9A-Za-z]$/;//闰年出生日期的合法性正则表达式   
        } else {
            ereg=/^[1-9][0-9]{5}19[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9A-Za-z]$/;//平年出生日期的合法性正则表达式   
        }
        if(!ereg.test(idCard)) {
            //没有通过
            IsSuc=false;
        }
    }
    return IsSuc;
}
/* 
 * 身份证号码区域验证 
 */
function AreaValidity(idCard) {
    var area=true;
    var aCity={ 11: "北京",12: "天津",13: "河北",14: "山西",15: "内蒙古",21: "辽宁",22: "吉林",23: "黑龙 江",31: "上海",32: "江苏",33: "浙江",34: "安徽",35: "福建",36: "江西",37: "山东",41: "河南",42: "湖 北",43: "湖南",44: "广东",45: "广西",46: "海南",50: "重庆",51: "四川",52: "贵州",53: "云南",54: "西 藏",61: "陕西",62: "甘肃",63: "青海",64: "宁夏",65: "新疆",71: "台湾",81: "香港",82: "澳门",91: "国 外" };
    if(aCity[parseInt(idCard.substr(0,2))]==null) {
        area=false;
    }
    return area;
}
//去掉字符串头尾空格   
function trim(str) {
    return str.replace(/(^\s*)|(\s*$)/g,"");
}  

