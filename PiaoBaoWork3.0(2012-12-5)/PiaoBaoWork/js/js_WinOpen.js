//form提交打开一个新窗口
function WinOpen(url,method,param) {
    var from=document.createElement("FORM");
    from.id="form_"+Math.random();
    from.target="_blank";
    from.action=url;
    from.method=method;//GET还是POST
    if(param!=null) {
        for(var o in param) {
            var input=document.createElement("input");
            input.id=o;
            input.type="hidden";
            input.name=o;
            input.value=param[o];
            from.appendChild(input);
        }
    }
    document.appendChild(from);
    from.submit();
}