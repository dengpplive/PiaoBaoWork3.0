// JavaScript Document
var qs = window.location.search.substring(1);
if (qs.length > 0) {
    var Airways = qs.substring(qs.indexOf('=') + 1).toUpperCase();
    switch (Airways) {
        case "CA": //涓浗鍥借埅
            doClick_price(document.getElementById("price" + 1));
            break;
        case "MU": //涓滄柟鑸┖
            doClick_price(document.getElementById("price" + 2));
            break;
        case "FM": //涓婃捣鑸┖
            doClick_price(document.getElementById("price" + 3));
            break;
        case "CZ": //鍗楁柟鑸┖
            doClick_price(document.getElementById("price" + 4));
            break;
        case "SC": //灞变笢鑸┖
            doClick_price(document.getElementById("price" + 5));
            break;
        case "ZH": //娣卞湷鑸┖
            doClick_price(document.getElementById("price" + 6));
            break;
        case "3U": //鍥涘窛鑸┖
            doClick_price(document.getElementById("price" + 7));
            break;
        case "MF": //鍘﹂棬鑸┖
            doClick_price(document.getElementById("price" + 8));
            break;
        case "HU": //娴疯埅闆嗗洟
        case "GS": //澶ф柊鍗�
        case "CN": //CN 鏂板崕
        case "8L": // 楣忚 8l
        case "PN": //瑗块儴 PN
            doClick_price(document.getElementById("price" + 9));
            break;
        case "EU": //楣拌仈鑸┖
            doClick_price(document.getElementById("price" + 10));
            break;
        case "HO": //鍚夌ゥ鑸┖
            doClick_price(document.getElementById("price" + 11));
            break;
        case "G5": //鍗庡鑸┖
            doClick_price(document.getElementById("price" + 12));
            break;
        case "BK": //濂ュ嚡鑸┖
            doClick_price(document.getElementById("price" + 13));
            break;
        case "NS": //涓滃寳鑸┖
            doClick_price(document.getElementById("price" + 14));
            break;
        case "KN": //涓仈鑸┖
            doClick_price(document.getElementById("price" + 15));
            break;
        case "JD": //閲戦箍鑸┖
            doClick_price(document.getElementById("price" + 16));
            break;
        case "JR": //骞哥鑸┖
            doClick_price(document.getElementById("price" + 17));
            break;
        default:
            doClick_price(document.getElementById("price1"));
            break;
    }
}
// function getQueryString() {
// qs = window.location.search.substring(1);
// if (qs.length > 0) {
// var parts = qs.split('&');
// for (var s in parts) {
// var pair = parts[s].split('=');
// var value = parts[s].substring(parts[s].indexOf('=') + 1);
// this[pair[0]] = value;
// }
// }
// }
function doClick_price(o) {
    o.className = "current";
    var j;
    var id;
    var e;
    for (var i = 1; i <= 20; i++) {
        id = "price" + i;
        j = document.getElementById(id);
        e = document.getElementById("pr_con" + i);
        if (id != o.id) {
            j.className = "";
            e.style.display = "none";
        } else {
            e.style.display = "block";
        }
    }
} 