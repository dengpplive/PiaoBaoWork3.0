var HL = HL || {};    
HL.Cookie = {    
/* 
函数名称：HL.Cookie.Get([string name]) 
函数功能：得到Cookie 
参数：name 可选项，要取得的Cookie名称 
说明：name为空时将通过数组形式返回全部Cookie，name不为空时返回此Cookie名称的值，没有任何值时返回undefined 
*/   
     Get : function(name){    
        var cv = document.cookie.split("; ");//使用"; "分割Cookie    
        var cva = [], cvat = [], cvam = [], temp;    
        /*循环的得到Cookie名称与值*/   
        for(i=0; i<cv.length; i++){    
             temp = cv[i].split("=");//用"="分割Cookie的名称与值    
            if(temp[0].indexOf("_divide_") > 0){    
                 cvam[temp[0]] = temp[1];    
             }else{    
                if(temp[0] != "") cvat[i] = [temp[0], temp[1]];    
             }    
         }    
        for(i=0; i<cvat.length; i++){    
            if(cvat[i]){    
                if(cvat[i][1].substr(0,8) != "^divide|"){    
                    /*小于4K的Cookie处理*/   
                     cva[cvat[i][0]] = unescape(cvat[i][1]);    
                 }else{    
                    /*大于4K的Cookie处理*/   
                    var sta = cvat[i][1].indexOf("$"), tot = cvat[i][1].substring(8,sta);    
                     cva[cvat[i][0]] = cvat[i][1].substring(sta+1);    
                    for(j=1; j<tot; j++){    
                         cva[cvat[i][0]] += cvam[cvat[i][0]+"_divide_"+j];    
                     }    
                     cva[cvat[i][0]] = unescape(cva[cvat[i][0]]);    
                 }    
             }    
         }    
        if(name) return cva[name];//如果有name则输出这个name的Cookie值    
        else return cva;//如果没有name则输出以名称为key，值为Value的数组    
     },    
/* 
函数名称：HL.Cookie.Set(string name, string   value[, int expires[, string path[, string domain[, string secure]]]]) 
函数功能：存入Cookie 
参数：name 必要项，要存入的Cookie名称 
       value 必要项，要存入的Cookie名称对应的值 
       expires 可选项，Cookie的过期时间，可以填入以秒为单位的保存时间，也可以填入日期格式（wdy, DD-Mon-YYYY HH:MM:SS GMT）的到期时间 
       path 可选项，Cookie在服务器端的有效路径 
       domain 可选项，该Cookie的有效域名 
       secure 可选项， 指明Cookie 是否仅通过安全的 HTTPS 连接传送，0或false或空时为假 
说明：保存成功则返回true，保存失败返回false 
*/   
     Set : function(name, value, expires, path, domain, secure, divide){    
        if(!divide) var value = escape(value);    
        if(!name || !value) return false;//如果没有name和value则返回false    
        if(name == "" || value == "") return false;//如果name和value为空则返回false    
        /*对于过期时间的处理*/   
        if(expires){    
            /*如果是数字则换算成GMT时间，当前时间加上以秒为单位的expires*/   
            if(/^[0-9]+$/.test(expires)){    
                var today = new Date();    
                 expires = new Date(today.getTime()+expires*1000).toGMTString();    
            /*判断expires格式是否正确，不正确则赋值为undefined*/   
             }else if(!/^wed, \d{2} \w{3} \d{4} \d{2}\:\d{2}\:\d{2} GMT$/.test(expires)){    
                 expires = undefined;    
             }    
         }    
        if(name.indexOf("_divide_")< 1 && !divide){    
            this.Del(name, path, domain);//删除前一次存入的Cookie    
         }    
        /*合并cookie的相关值*/   
        var cv = name+"="+value+";"   
                + ((expires) ? " expires="+expires+";" : "")    
                + ((path) ? "path="+path+";" : "")    
                + ((domain) ? "domain="+domain+";" : "")    
                + ((secure && secure != 0) ? "secure" : "");    
        /*判断Cookie总长度是否大于4K*/   
        if(cv.length < 4096){    
             document.cookie = cv;//写入cookie    
         }else{    
            /*对于大于4K的Cookie的操作*/   
            var max = Math.floor(value.length/3800)+1;    
            for(i=0; i<max; i++){    
                if(i == 0){    
                    this.Set(name, '^divide|'+max+'$'+value.substr(0,3800), expires, path, domain, secure, true);    
                 }else{    
                    this.Set(name+"_divide_"+i, value.substr(i*3800,3800), expires, path, domain, secure, true);    
                 }    
             }    
         }    
        return true;    
     },    
/* 
函数名称：HL.Cookie.Del(string name[, string path[, string domain]]) 
函数功能：删除Cookie 
参数：name 必要项，要删除的Cookie名称 
       path 可选项，要删除的Cookie在服务器端的有效路径 
       domain 可选项，要删除的Cookie的有效域名 
说明：删除成功返回true，删除失败返回false 
*/   
     Del : function(name, path, domain){    
        if(!name) return false;//如果没有name则返回false    
        if(name == "") return false;//如果name为空则返回false    
        if(!this.Get(name)) return false;//如果要删除的name值不存在则返回false    
        /*对于大于4K的Cookie进行处理*/   
        if(escape(this.Get(name)).length > 3800){    
            var max = Math.floor(escape(this.Get(name)).length/3800)+1;    
            for(i=1; i<max; i++){    
                /*合并Cookie的相关值，并删除*/   
                 document.cookie = name+"_divide_"+i+"=;"   
                               + ((path) ? "path="+path+";" : "")    
                               + ((domain) ? "domain="+domain+";" : "")    
                               + "expires=Thu, 01-Jan-1970 00:00:01 GMT;";    
             }    
         }    
        /*合并Cookie的相关值，并删除*/   
         document.cookie = name+"=;"   
                           + ((path) ? "path="+path+";" : "")    
                           + ((domain) ? "domain="+domain+";" : "")    
                           + "expires=Thu, 01-Jan-1970 00:00:01 GMT;";    
        return true;    
     }    
}   
