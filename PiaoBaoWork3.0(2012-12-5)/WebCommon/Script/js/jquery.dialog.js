(function ($) {
    Array.prototype.indexOf = function (obj) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == obj) {
                return i;
            }
        }
        return -1;
    }

    $.extend($, {
        showDialog: function (options) {
            dialog = new $.dialog(options);
            return dialog;
        }
    })

    $.dialog = function (options) {
        this.settings = $.extend(true, {}, $.dialog.defaults, options);
        this.init();
    }

    $.extend($.dialog, {
        defaults: {
            width: 400,
            height: 150,
            type: "alert",
            title: "系统提示",
            //            content: "这是系统提示内容",
            url: "",
            subHeadingTitle: "副标题",
            subHeadingContent: "这是副标题内容",
            showSubHeading: false,
            showButton: true,
            needMask: true,
            okEvent: function () { },
            noEvent: function () { },
            cancelEvent: function () { }
        },
        setDefaults: function (settings) {
            $.extend($.dialog.defaults, settings);
        },
        prototype: {
            init: function () {
            },
            show: function (msg) {
                var id = this.createId();
                var $ui = $(this.createUI(id));

                $ui.width(this.settings.width);
                $ui.height(this.settings.height);

                $ui.find("#_dialog_title_" + id).text(this.settings.title);
                if (!this.settings.showButton)
                    $ui.find("#_dialog_buttonbox_" + id).hide();
                if (this.settings.showSubHeading) {
                    $ui.find("#_dialog_subheading_title_" + id).text(this.settings.subHeadingTitle);
                    $ui.find("#_dialog_subheading_content_" + id).text(this.settings.subHeadingContent);
                    $ui.find("#_dialog_subheading_" + id).show();
                }
                if (this.settings.url != "") {
                    $ui.find("#_dialog_content_" + id).hide();
                    $ui.find("#_dialog_iframe_" + id).show();
                    $ui.find("#_dialog_iframe_" + id).attr("src", this.settings.url);
                }
                else {
                    $ui.find("#_dialog_message_" + id).text(msg);
                }
                if (this.settings.type == "alert") {
                    $ui.find("#_dialog_buttonbox_yes" + id).hide();
                    $ui.find("#_dialog_buttonbox_no" + id).hide();
                    $ui.find("#_dialog_buttonbox_cancel" + id).val("确定");
                }
                if (this.settings.type == "confirm")
                    $ui.find("#_dialog_messageicon_" + id).attr("src", '<%= WebResource("PbProject.WebCommon.Script.images.icon_query.gif")%>');

                var $dialog = this;

                this.bindEvent($ui.find("#_dialog_closebutton_" + id), "click", function () { $dialog.close(); });

                this.bindEvent($ui.find("#_dialog_buttonbox_cancel" + id), "click", function () {
                    $dialog.close();
                });
                this.bindEvent($ui.find("#_dialog_buttonbox_cancel" + id), "click", this.settings.cancelEvent);

                this.bindEvent($ui.find("#_dialog_buttonbox_yes" + id), "click", function () {
                    $dialog.close();
                });
                this.bindEvent($ui.find("#_dialog_buttonbox_yes" + id), "click", this.settings.okEvent);

                this.bindEvent($ui.find("#_dialog_buttonbox_no" + id), "click", function () {
                    $dialog.close();
                });
                this.bindEvent($ui.find("#_dialog_buttonbox_no" + id), "click", this.settings.noEvent);

                var top, left;
                top = ($(window).height() - $ui.height() - 146) / 2;
                left = ($(window).width() - $ui.width()) / 2;
                if (this.idArray) {
                    top += this.idArray.length * 8;
                    left += this.idArray.length * 8;
                }
                $ui.css({ "top": top + "px", "left": left + "px" });

                //                var mw = Math.max(document.body.clientWidth, document.body.scrollWidth);
                //                var mh = Math.max(document.body.clientHeight, document.body.scrollHeight);

                if (this.settings.needMask) {
                    var $mask = $(this.createMask(id));
                    $mask.height($(document.body).height() < $(window).height() ? $(window).height() : $(document.body).height());
                    $mask.width($(document.body).width() < $(window).width() ? $(window).width() : $(document.body).width());
                    $mask.appendTo($(document.body));
                }
                $ui.appendTo($(document.body));
                this.idArray.push(id);

                $.Move("_dialog_top_" + id, "_dialog_box_" + id);
                if (this.settings.needMask)
                    $(window).bind("resize", function () {
                        $("#_dialog_mask_" + id).height($(document.body).height() < $(window).height() ? $(window).height() : $(document.body).height());
                        $("#_dialog_mask_" + id).width($(document.body).width() < $(window).width() ? $(window).width() : $(document.body).width());
                    })
            },
            close: function () {
                var currentId;
                if (this.idArray.length > 0)
                    currentId = this.idArray[this.idArray.length - 1];
                else
                    return;
                $("#_dialog_box_" + currentId).remove();
                $("#_dialog_mask_" + currentId).remove();
                if (this.idArray.indexOf(currentId) >= 0) {
                    this.idArray.splice(this.idArray.indexOf(currentId), 1);
                }
            },
            createId: function () {
                var d = new Date();
                return d.getYear().toString() + (d.getMonth() + 1).toString() + d.getDate().toString() + d.getHours().toString() + d.getMinutes().toString() + d.getSeconds().toString() + d.getMilliseconds().toString();
            },
            idArray: [],
            bindEvent: function ($control, funcName, eventFunc) {
                if ($control && funcName && eventFunc) {
                    $control.bind(funcName, eventFunc);
                }
            },
            createUI: function (id) {
                var ui = [];
                ui.push('<div id="_dialog_box_' + id + '" style="position:absolute;top:0;left:0;">');
                ui.push('<div id="_dialog_top_' + id + '" style="width: 100%; height: 33px;">');
                ui.push('<div style="width: 13px; height: 100%; margin-right: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_lt.png")%>); background-repeat: no-repeat; float: left;">');
                ui.push('</div>');
                ui.push('<div style="width: 13px; height: 100%; margin-left: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_rt.png")%>); background-repeat: no-repeat; float: right;">');
                ui.push('</div>');
                ui.push('<div style="width: auto; height: 100%; margin: auto 13px auto 13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_ct.png")%>); background-repeat: repeat-x;">');
                ui.push('<div style="float: left; font-weight: bold; color: #FFFFFF; padding: 12px 0 0 4px; font-size: 12px;">');
                ui.push('<img style="vertical-align: middle" alt="" src="<%= WebResource("PbProject.WebCommon.Script.images.icon_dialog.gif")%>" />');
                ui.push('<label id="_dialog_title_' + id + '" style="vertical-align: middle; font-size: 12px">');
                ui.push('</label>');
                ui.push('</div>');
                ui.push('<div id="_dialog_closebutton_' + id + '" onmouseout="this.style.background=\'url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_closebtn.gif")%>)\'"');
                ui.push('onmouseover="this.style.background=\'url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_closebtn_over.gif")%>)\'" style="position: relative; cursor: pointer; float: right; margin-top: 12px; height: 17px; width: 28px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_closebtn.gif")%>);">');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('<div style="clear: both;">');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('<div id="_dialog_subheading_' + id + '" style="width: 100%; height: 50px;display: none;">');
                ui.push('<div style="width: 13px; height: 100%; margin-right: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_mlm.png")%>); background-repeat: no-repeat; float: left;">');
                ui.push('</div>');
                ui.push('<div style="width: 13px; height: 100%; margin-left: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_mrm.png")%>); background-repeat: no-repeat; float: right;">');
                ui.push('</div>');
                ui.push('<div style="width: auto; height: 100%; margin: auto 13px auto 13px; background: #EAECE9 url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_bg.jpg")%>) no-repeat right top;">');
                ui.push('<div style="height: 100%; padding: 0 5px 0 8px; vertical-align: middle; float: left;">');
                ui.push('<img style="width: 32px; height: 32px; margin-top: 8px;" src="<%= WebResource("PbProject.WebCommon.Script.images.window.gif")%>" />');
                ui.push('</div>');
                ui.push('<div style="height: 100%; line-height: 16px; padding-left: 6px; float: left; overflow: hidden;">');
                ui.push('<h5 id="_dialog_subheading_title_' + id + '" style="margin: 5px auto 3px 3px; font-size: 12px"></h5>');
                ui.push('<div id="_dialog_subheading_content_' + id + '" style="margin: 5px auto 3px 3px; font-size: 14px"></div>');
                ui.push('</div>');
                ui.push('<div style="clear: both;">');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('<div id="_dialog_middle_' + id + '" style="width: 100%; height: 100%;">');
                ui.push('<div style="width: 13px; height: 100%; margin-right: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_mlm.png")%>); background-repeat: no-repeat; float: left;">');
                ui.push('</div>');
                ui.push('<div style="width: 13px; height: 100%; margin-left: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_mrm.png")%>); background-repeat: no-repeat; float: right;">');
                ui.push('</div>');
                ui.push('<div style="width: auto; margin: auto 13px auto 13px; height: 100%; background-color: White;">');
                ui.push('<table id="_dialog_content_' + id + '" style="width: 100%; height: 100%;">');
                ui.push('<tr>');
                ui.push('<td style="width: 80px; text-align: center;">');
                ui.push('<img id="_dialog_messageicon_' + id + '" src="<%= WebResource("PbProject.WebCommon.Script.images.icon_alert.gif")%>" alt="" style="width: 34px; height: 34px;" /></td>');
                ui.push('<td id="_dialog_message_' + id + '" style="text-align: left; font-size: 12px"></td>');
                ui.push('</tr>');
                ui.push('</table>');
                ui.push('<iframe id="_dialog_iframe_' + id + '" src="" style="width: 100%; height: 100%; background-color: transparent; border: none; display: none;"></iframe>');
                ui.push('</div>');
                ui.push('<div style="clear: both;">');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('<div id="_dialog_buttonbox_' + id + '" style="width: 100%; height: 50px; text-align: right;">');
                ui.push('<div style="width: 13px; height: 100%; margin-right: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_mlm.png")%>); background-repeat: no-repeat; float: left;">');
                ui.push('</div>');
                ui.push('<div style="width: 13px; height: 100%; margin-left: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_mrm.png")%>); background-repeat: no-repeat; float: right;">');
                ui.push('</div>');
                ui.push('<div style="width: auto; height: 100%; margin: auto 13px auto 13px; border-top: #dadee5 1px solid; background-color: #f6f6f6; overflow-y: no-display;">');
                ui.push('<div style="margin:10px;">');
                ui.push('<input id="_dialog_buttonbox_yes' + id + '" type="button" style="width: 50px;" value="是" />');
                ui.push('<input id="_dialog_buttonbox_no' + id + '" type="button" style="width: 50px;" value="否" />');
                ui.push('<input id="_dialog_buttonbox_cancel' + id + '" type="button" style="width: 50px;" value="取消" />');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('<div style="clear: both;">');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('<div id="_dialog_bottom_' + id + '" style="width: 100%; height: 13px;">');
                ui.push('<div style="width: 13px; height: 100%; margin-right: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_lb.png")%>); background-repeat: no-repeat; float: left;">');
                ui.push('</div>');
                ui.push('<div style="width: 13px; height: 100%; margin-left: -13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_rb.png")%>); background-repeat: no-repeat; float: right;">');
                ui.push('</div>');
                ui.push('<div style="width: auto; height: 100%; margin: auto 13px auto 13px; background: url(<%= WebResource("PbProject.WebCommon.Script.images.dialog_cb.png")%>); background-repeat: repeat-x;">');
                ui.push('</div>');
                ui.push('<div style="clear: both;">');
                ui.push('</div>');
                ui.push('</div>');
                ui.push('</div>');
                return ui.join('\n');
            },
            createMask: function (id) {
                var mask = '<div id="_dialog_mask_' + id + '" style="position: absolute; top:0;left:0;opacity: 0.5;background-color: rgb(0, 0, 0);filter: alpha(opacity=50)">';
                return mask;
            }
        }
    })

    $.Move = function (_this, _target) {
        if (typeof (_this) == 'object') {
            _this = _this;
        } else {
            _this = $("#" + _this);
        }
        if (typeof (_target) == 'object') {
            _target = _target;
        } else {
            _target = $("#" + _target);
        }

        var maxX, maxY;
        var activeHeight = 0;
        $.each(_target.children(), function (index, item) {
            if ($(item).is(":visible")) {
                activeHeight += $(item).height();
            }
        })
        maxX = $(window).width() - _target.width();
        maxY = $(window).height() - activeHeight;

        _this.hover(function () { $(this).css("cursor", "move"); }, function () { $(this).css("cursor", "default"); })
        _this.mousedown(function (e) {//e鼠标事件
            var offset = $(this).offset();
            var x = e.pageX - offset.left;
            var y = e.pageY - offset.top;
            if (_target)
                _target.css({ 'opacity': '0.3' });
            else
                _this.css({ 'opacity': '0.3' });
            $(document).bind("mousemove", function (ev) {//绑定鼠标的移动事件，因为光标在DIV元素外面也要有效果，所以要用doucment的事件，而不用DIV元素的事件
                _this.bind('selectstart', function () { return false; });
                var _x = ev.pageX - x; //获得X轴方向移动的值
                var _y = ev.pageY - y; //获得Y轴方向移动的值
                if (_x <= 0)
                    _x = 0;
                else if (_x >= maxX)
                    _x = maxX;

                if (_y <= 0)
                    _y = 0;
                else if (_y >= maxY)
                    _y = maxY;

                if (_target)
                    _target.css({ 'left': _x + "px", 'top': _y + "px" });
                else
                    _this.css({ 'left': _x + "px", 'top': _y + "px" });
            });
        });

        $(document).mouseup(function () {
            $(this).unbind("mousemove");
            if (_target)
                _target.css({ 'opacity': '' });
            else
                _this.css({ 'opacity': '' });
        })
    };


} (jQuery));

function dAlert(msg, func) {
    if (func)
        $.showDialog({ cancelEvent: func }).show(msg);
    else
        $.showDialog().show(msg);
}

function dConfirm(msg, okFunc, cancelFunc) {
    if (okFunc && cancelFunc) {
        $.showDialog({ type: "confirm", okEvent: okFunc, noEvent: cancelFunc }).show(msg);
        return;
    }
    else if (okFunc) {
        $.showDialog({ type: "confirm", okEvent: okFunc }).show(msg);
        return;
    }
    else if (cancelFunc) {
        $.showDialog({ type: "confirm", noEvent: cancelFunc }).show(msg);
        return;
    }
    else
        $.showDialog({ type: "confirm" }).show(msg);
}