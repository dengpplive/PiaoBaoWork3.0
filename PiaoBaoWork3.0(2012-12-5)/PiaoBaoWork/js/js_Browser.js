//获取浏览器类型
function getBrowser() {
    var a=navigator.userAgent.toLowerCase();
    var c=/opera/.test(a),
            h=/chrome/.test(a),
            b=/webkit/.test(a),
            m=!h&&/safari/.test(a),
            g=!c&&/msie/.test(a),
            e=g&&/msie 7/.test(a),
            f=g&&/msie 8/.test(a),
            i=g&&!e&&!f,
            k=!b&&/gecko/.test(a),
            n=k&&/rv:1\.8/.test(a);
    k&&/rv:1\.9/.test(a);
    return {
        IE: g,
        IE6: i,
        IE7: e,
        IE8: f,
        Moz: k,
        FF2: n,
        Opera: c,
        Safari: m,
        WebKit: b,
        Chrome: h
    }
}