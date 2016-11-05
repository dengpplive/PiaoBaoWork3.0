//静态表格分页
var tabId="gvflyer";//需要分页的表格ID
var PagerContainer="pagerDiv";//分页显示的位置
var trsObj;//tr集合
var trLen;//tr长度
var page=0;
var nowPage=0;//当前页
var listNum=9;//每页显示tr数 每页大小
var PagesLen;//总页数
var PageNum=4;//分页链接接数(5个)
//加载
jQuery(function () {
    trsObj=jQuery("#"+tabId+" tbody tr");
    trLen=trsObj.length;
    PagesLen=Math.ceil(trLen/listNum);
    upPage(0);//初始化
});
//单击分页显示
function upPage(p) {
    nowPage=p;
    //内容变换
    for(var i=0;i<trLen;i++) {
        trsObj[i].style.display="none";
    }
    for(var i=p*listNum;i<(p+1)*listNum;i++) {
        if(trsObj[i]) trsObj[i].style.display="block";
    }
    //分页链接变换
    strS='<a href="###" onclick="upPage(0)">首页</a>&nbsp;&nbsp;';
    var PageNum_2=PageNum%2==0?Math.ceil(PageNum/2)+1:Math.ceil(PageNum/2);
    var PageNum_3=PageNum%2==0?Math.ceil(PageNum/2):Math.ceil(PageNum/2)+1;
    var strC="",startPage,endPage;
    if(PageNum>=PagesLen) { startPage=0;endPage=PagesLen-1; }
    else if(nowPage<PageNum_2) { startPage=0;endPage=PagesLen-1>PageNum?PageNum:PagesLen-1; } //首页
    else { startPage=nowPage+PageNum_3>=PagesLen?PagesLen-PageNum-1:nowPage-PageNum_2+1;var t=startPage+PageNum;endPage=t>PagesLen?PagesLen-1:t; }
    for(var i=startPage;i<=endPage;i++) {
        if(i==nowPage) strC+='<a href="###" style="color:red;font-weight:700;" onclick="upPage('+i+')">'+(i+1)+'</a>&nbsp;'
        else strC+='<a href="###" onclick="upPage('+i+')">'+(i+1)+'</a>&nbsp;'
    }
    strE='&nbsp;<a href="###" onclick="upPage('+(PagesLen-1)+')">尾页</a>&nbsp;&nbsp;';
    strE2=nowPage+1+"/"+PagesLen+"页"+"&nbsp;&nbsp;共"+trLen+"条";
    jQuery("#"+PagerContainer).html(strS+strC+strE+strE2);
    jQuery("#trHead").show();
}