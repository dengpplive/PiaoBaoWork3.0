function copy(evt) {
    var obj = evt.srcElement ? evt.srcElement : evt.target;
    if (obj.tagName.toLowerCase() == "td") {
        if (obj.tle != undefined && obj.tle != "") {
            copyToClipboard(obj.tle);
        } else {
            copyToClipboard(obj.innerText);
        }
    }
}
function copyToClipboard(copytext) {
    if (window.clipboardData) {
        window.clipboardData.clearData();
        window.clipboardData.setData("Text", copytext);
    } else if (navigator.userAgent.indexOf("Opera") != -1) {
        window.location = copytext;
    } else if (window.netscape) {
        try {
            netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
        } catch (e) {
            alert("您的firefox安全限制限制您进行剪贴板操作，请打开’about:config’将signed.applets.codebase_principal_support’设置为true’之后重试，相对路径为firefox根目录/greprefs/all.js");
            return false;
        }

        var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
        if (!clip) {
            return;
        }

        var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
        if (!trans) {
            return;
        }

        trans.addDataFlavor('text/unicode');

        var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
        str.data = copytext;
        trans.setTransferData("text/unicode", str, copytext.length * 2);
        var clipid = Components.interfaces.nsIClipboard;
        if (!clip) {
            return false;
        }

        clip.setData(trans, null, clipid.kGlobalClipboard);
    }
}

