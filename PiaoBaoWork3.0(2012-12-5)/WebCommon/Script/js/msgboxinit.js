function g_showWarn(msg) {
    $.msgbox(msg, {
        type: "alert",
        buttons: [
                  { type: "cancel", value: "确定" }
                ]
    });
}
function g_showError(msg) {
    $.msgbox(msg, {
        type: "error",
        buttons: [
                  { type: "cancel", value: "确定" }
                ]
    });
}
function g_showInfo(msg) {
    $.msgbox(msg, {
        type: "info",
        buttons: [
                  { type: "cancel", value: "确定" }
                ]
    });
}
