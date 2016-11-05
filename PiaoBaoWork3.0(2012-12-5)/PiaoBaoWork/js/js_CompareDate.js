//日期大小比较 开始时间>结束时间返回true 否则false
function CompareDate(sdate,edate) {
    var s_year=1900,s_month=1,s_day=1,s_hour=0,s_minute=0,s_second=0;
    var e_year=1900,e_month=1,e_day=1,e_hour=0,e_minute=0,e_second=0;
    //开始日期
    var strDate1=sdate.split(' ')[0].split('-');
    var date1;
    if(strDate1.length==3) {
        s_year=strDate1[0];
        s_month=strDate1[1];
        s_day=strDate1[2];
    }
    if(sdate.split(' ').length==2) {
        var strTime1=sdate.split(' ')[1].split(':');
        if(strTime1.length==2) {
            s_hour=strTime1[0];
            s_minute=strTime1[1]
        } else if(strTime1.length==3) {
            s_hour=strTime1[0];
            s_minute=strTime1[1];
            s_second=strTime1[2];
        }
    }
    //结束日期
    var strDate2=edate.split(' ')[0].split('-');
    var date2;
    if(strDate2.length==3) {
        e_year=strDate2[0];
        e_month=strDate2[1];
        e_day=strDate2[2];
    }
    if(edate.split(' ').length==2) {
        var strTime2=edate.split(' ')[1].split(':');
        if(strTime2.length==2) {
            e_hour=strTime2[0];
            e_minute=strTime2[1]
        } else if(strTime2.length==3) {
            e_hour=strTime2[0];
            e_minute=strTime2[1];
            e_second=strTime2[2];
        }
    }
    date1=new Date(s_year,s_month,s_day,s_hour,s_minute,s_second);
    date2=new Date(e_year,e_month,e_day,e_hour,e_minute,e_second);
    return (date1>date2);
}