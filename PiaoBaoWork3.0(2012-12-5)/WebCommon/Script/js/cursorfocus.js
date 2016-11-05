$(function() {
    $(".input,.textarea").focus(function() {
        $(this).addClass("focus");
    }).blur(function() {
        $(this).removeClass("focus");
    });

    //输入框提示,获取拥有HintTitle,HintInfo属性的对象
    $("[HintTitle],[HintInfo]").focus(function(event) {
        $("*").stop(); //停止所有正在运行的动画
		$("#HintMsg").remove(); //先清除，防止重复出错
        var HintHtml = "<ul id=\"HintMsg\"><li class=\"HintTop\"></li><li class=\"HintInfo\"><b>" + $(this).attr("HintTitle") + "</b>" + $(this).attr("HintInfo") + "</li><li class=\"HintFooter\"></li></ul>"; //设置显示的内容
        var offset = $(this).offset(); //取得事件对象的位置
        $("body").append(HintHtml); //添加节点
        $("#HintMsg").fadeTo(1000, 0.85); //对象的透明度
        var HintHeight = $("#HintMsg").height(); //取得容器高度
        $("#HintMsg").css({ "top": offset.top - HintHeight + "px", "left": offset.left + "px" }).fadeIn(500);
    }).blur(function(event) {
        $("#HintMsg").remove(); //删除UL
    });
});