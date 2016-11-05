var DClass = function (properties) {
    var _class = function () { return (arguments[0] !== null && this.initialize && typeof (this.initialize) == 'function') ? this.initialize.apply(this, arguments) : this; };
    _class.prototype = properties;
    return _class;
};
var Dialog = new DClass({
    options: {
        Width: 400,
        Height: 400,
        Left: 100,
        Top: 150,
        Titleheight: 26,
        Minwidth: 200,
        Minheight: 200,
        CancelIco: true,
        ResizeIco: false,
        IsDrag: true,
        Info: "",
        maskColor: "red",
        Content: null,
        Zindex: 200000,
        RandomId: null,
        BeforeClose: null,
        AfterClose: null,
        ToolbarHTML: null,
        MenubarHTML: null,
        MiddlePanelHTML: null,
        BottomPanelHTML: null,
        StatusPanelHTML: null,
        BottomBtnType: 0
    },
    initialize: function (options) {
        try {
            this._dragobj = null;
            this._resize = null;
            this._cancel = null;
            this._body = null;
            this._maskColor = options.maskColor == null ? this.options.maskColor : options.maskColor;
            this._x = 0;
            this._y = 0;
            this._fM = this.BindAsEventListener(this, this.Move);
            this._fS = this.Bind(this, this.Stop);
            this._isdrag = options.IsDrag;
            this._Css = null;
            this.Width = options.Width == null ? this.options.Width : options.Width;
            this.Height = options.Hight == null ? this.options.Height : options.Hight;
            this.Left = options.Left == null ? this.options.Left : options.Left;
            this.Top = options.Top == null ? this.options.Top : options.Top;
            this.CancelIco = options.CancelIco == null ? this.options.CancelIco : options.CancelIco;
            this.Info = options.Info == null ? this.options.Info : options.Info;
            this.Content = options.Content == null ? this.options.Content : options.Content;
            this.Minwidth = options.Minwidth == null ? this.options.Minwidth : options.Minwidth;
            this.Minheight = options.Minheight == null ? this.options.Minheight : options.Minheight;
            this.Titleheight = options.Titleheight == null ? this.options.Titleheight : options.Titleheight;
            this.Zindex = (options.Zindex == null || isNaN(options.Zindex)) ? this.options.Zindex : options.Zindex;
            this._RandomId = options.RandomId == null ? function () { return parseInt(Math.random() * 1000); } () : options.RandomId;
            this._BeforeClose = options.BeforeClose == null ? null : options.BeforeClose;
            this._AfterClose = options.AfterClose == null ? null : options.AfterClose;
            this._BottomBtnType = options.BottomBtnType == null ? 0 : options.BottomBtnType;
            //自动扩展添加的属性
            this.Extend(this, options);
            Dialog.Zindex = this.Zindex;            
            this.Jquery = options.JqueryObj;
            //构造dialog
            var obj = ['dialogcontainter', 'dialogtitle', 'dialogtitleinfo', 'dialogtitleico', 'dialogbody', 'dialogbottom'];
            var _index = this.RandomId;
            this.dialogDiv = this.create('div', null, function (elm) { elm.id = 'dialog_' + _index });
            var dgId = ['dgcontainter_' + _index, 'dgtitle_' + _index, 'dgtitleinfo_' + _index, 'dgtitleico_' + _index, 'dgbody_' + _index, 'dgbottom_' + _index];
            for (var i = 0; i < obj.length; i++) {
                obj[i] = this.create('div', null, function (elm) { elm.className = obj[i]; });
                obj[i].id = dgId[i];
            }
            obj[2].innerHTML = options.Info;
            if (options.Content != null && options.Content != undefined) {
                obj[4].innerHTML = options.Content;
            }
            obj[1].appendChild(obj[2]);
            obj[1].appendChild(obj[3]);
            obj[0].appendChild(obj[1]);
            obj[0].appendChild(obj[4]);
            obj[0].appendChild(obj[5]);
            this.dialogDiv.appendChild(obj[0]);
            document.body.appendChild(this.dialogDiv);
            this._dragobj = obj[0];
            this._resize = obj[5];
            this._cancel = obj[3];
            this._body = obj[4];
            this._clientHeight = 0

            ///o,x1,x2
            ////设置Dialog的长 宽 ,left ,top
            with (this._dragobj.style) {
                height = this.Height + "px"; top = this.Top + "px"; width = this.Width + "px"; left = this.Left + "px"; zIndex = this.Zindex;
                this.dialogDiv.style.zIndex = this.Zindex;
            }
            var _bodyPx = this.Height - this.Titleheight - parseInt(this.CurrentStyle(this._body).paddingLeft) * 2;
            this._body.style.height = _bodyPx + 'px';

            if (this.Content == null) {
                //内容区域划分------------
                var _bodyObj = ['dgbody_toolbar', 'dgbody_menubar', 'dgbody_middlepanel', 'dgbody_bottompanel', 'dgbody_statuspanel'];
                var _bodyId = ['toolbar_' + _index, 'menubar_' + _index, 'middlePanel_' + _index, 'bottomPanel_' + _index, 'statusPanel_' + _index];
                for (var i = 0; i < _bodyObj.length; i++) {
                    _bodyObj[i] = this.create('div', null, function (elm) { elm.className = _bodyObj[i]; });
                    _bodyObj[i].id = _bodyId[i];
                    this.addListener(_bodyObj[i], 'mousedown', this.BindAsEventListener(this, this.Cancelbubble));
                }

                if (options.ToolbarHTML != null) {
                    this.ToolbarHTML = _bodyObj[0];
                    _bodyObj[0].innerHTML = options.ToolbarHTML;
                    obj[4].appendChild(this.ToolbarHTML);
                    this._clientHeight += this.ToolbarHTML.offsetHeight;
                }
                if (options.MenubarHTML != null) {
                    this.MenubarHTML = _bodyObj[1];
                    _bodyObj[1].innerHTML = options.MenubarHTML;
                    obj[4].appendChild(_bodyObj[1]);
                    this._clientHeight += _bodyObj[1].offsetHeight;
                }
                if (options.MiddlePanelHTML != null) {
                    this.MiddlePanelHTML = _bodyObj[2];
                    _bodyObj[2].innerHTML = options.MiddlePanelHTML;
                    obj[4].appendChild(_bodyObj[2]);
                }
                //按钮区域
                this.BottomHTML = _bodyObj[3];
                obj[4].appendChild(_bodyObj[3]);
                if (this._BottomBtnType != "-2") {
                    //-----------------------------------------------------------------------------                    
                    if (this._BottomBtnType == "0") {
                        // yes
                        var html0 = '<input type="button" id="btnYes_' + _index + '" value="确定" />';
                        _bodyObj[3].innerHTML = html0;
                        this.btn_Yes = this.GId("btnYes_" + _index);
                        this.addListener(this.btn_Yes, 'mousedown', this.BindAsEventListener(this, this.Close));

                    } else if (this._BottomBtnType == "-1") {
                        // nothing 自定义
                        _bodyObj[3].innerHTML = options.BottomPanelHTML;
                    }
                    else if (this._BottomBtnType == "1") {
                        var html1 = '<input type="button" id="btnYes_' + _index + '" value="确定" class="btnYes" />&nbsp;&nbsp;&nbsp;&nbsp;<input type="button" id="btnClose_' + _index + '" value="关闭"  class="btnClose"/>';
                        _bodyObj[3].innerHTML = html1;
                        this.btn_Yes = this.GId("btnYes_" + _index);
                        this.btn_Close = this.GId("btnClose_" + _index);
                        this.addListener(this.btn_Yes, 'mousedown', this.BindAsEventListener(this, this.Close));
                        this.addListener(this.btn_Close, 'mousedown', this.BindAsEventListener(this, this.Close));

                    }
                    else if (this._BottomBtnType == "2") {
                        var html2 = '<input type="button" id="btnClose_' + _index + '" value="关闭" class="btnClose" />';
                        _bodyObj[3].innerHTML = html2;
                        this.btn_Close = this.GId("btnClose_" + _index);
                        this.addListener(this.btn_Close, 'mousedown', this.BindAsEventListener(this, this.Close));

                    }
                    //-----------------------------------------------------------------------------                 
                    this._clientHeight += _bodyObj[3].offsetHeight;
                }

                if (options.StatusPanelHTML != null) {
                    _bodyObj[4].innerHTML = options.StatusPanelHTML;
                    obj[4].appendChild(_bodyObj[4]);
                    this._clientHeight += _bodyObj[4].offsetHeight;
                }

                with (_bodyObj[2].style) {
                    height = ((_bodyPx - this._clientHeight) < 0 ? 0 : (_bodyPx - this._clientHeight)) + 'px';
                }
                this.middleBody = _bodyObj[2];
                this.HMiddle();
            }
            ///  添加事件	
            if (this._isdrag) {
                this.addListener(this._dragobj, 'mousedown', this.BindAsEventListener(this, this.Start, true));
            }
            //this.addListener(this._cancel, 'mouseover', this.Bind(this, this.Changebg, [this._cancel, '0px 0px', '-21px 0px']));
            //this.addListener(this._cancel, 'mouseout', this.Bind(this, this.Changebg, [this._cancel, '0px 0px', '-21px 0px']));
            this.addListener(this._cancel, 'mouseover', this.BindAsEventListener(this, this.Over));
            this.addListener(this._cancel, 'mouseout', this.BindAsEventListener(this, this.Out));
            this.addListener(this._cancel, 'mousedown', this.BindAsEventListener(this, this.Close));
            this.addListener(this._body, 'mousedown', this.BindAsEventListener(this, this.Cancelbubble));
            this.addListener(this._resize, 'mousedown', this.BindAsEventListener(this, this.Start, false));
            if (this.Jquery != null) {
                this.Jquery(document).mask();
            }

        } catch (e) {
            //alert(e.Message);
        }
    },
    Over: function (e) {
        this._cancel.style.cursor = "default";
    },
    Out: function (e) {
        this._cancel.style.cursor = "default";
    },
    Close: function (e) {
        try {
            //关闭按钮
            if (this._BeforeClose) {
                this._BeforeClose(e);
            }
            if (e != undefined) {
                this.Cancelbubble(e);
            }
            if (this.dialogDiv != null) {
                document.body.removeChild(this.dialogDiv); //this._dragobj);
            }
            if (this.maskDiv != null) {
                document.body.removeChild(this.maskDiv);
            }
            if (this._AfterClose) {
                this._AfterClose(e);
            }
            if (this.Jquery != null) {
                this.Jquery(document).unmask();
            }
        } catch (e) {
            //alert(e.message);
        }
        return this;
    },
    HMiddle: function (offsetH) {
        var v_left = (document.body.clientWidth - this.Width) / 2;
        v_left += document.body.scrollLeft;
        if (offsetH != null) {
            v_left += offsetH;
        }
        this.dialogDiv.style.left = v_left + "px";
        this._dragobj.style.left = v_left + "px";
        return this;
    },
    VMiddle: function (offsetV) {
        var v_top = (document.body.clientHeight - this.Height) / 2;
        v_top += document.body.scrollTop;
        if (offsetV != null) {
            v_top += offsetV;
        }
        this.dialogDiv.style.top = v_top + "px";
        this._dragobj.style.top = v_top + "px";
        return this;
    },
    Cancelbubble: function (e) {
        try {
            this._dragobj.style.zIndex = ++Dialog.Zindex;
            document.all ? (e.cancelBubble = true) : (e.stopPropagation())
        } catch (e) {
            alert(e.message);
        }
        return this;
    },
    middleHtml: function (html) {
        if (html != null && html != undefined) {
            return this.MiddlePanelHTML.innerHTML;
        } else {
            this.MiddlePanelHTML.innerHTML = html;
        }

    },
    Changebg: function (o, x1, x2) {
        o.style.backgroundPosition = (o.style.backgroundPosition == x1) ? x2 : x1;
        return this;
    },
    Start: function (e, isdrag) {
        try {
            if (!isdrag) { this.Cancelbubble(e); }
            this._Css = isdrag ? { x: "left", y: "top"} : { x: "width", y: "height" }
            this._dragobj.style.zIndex = ++Dialog.Zindex;
            this._isdrag = isdrag;
            this._x = isdrag ? (e.clientX - this._dragobj.offsetLeft || 0) : (this._dragobj.offsetLeft || 0);
            this._y = isdrag ? (e.clientY - this._dragobj.offsetTop || 0) : (this._dragobj.offsetTop || 0);
            if (this.isIE) {
                this.addListener(this._dragobj, "losecapture", this._fS);
                this._dragobj.setCapture();
            }
            else {
                e.preventDefault();
                this.addListener(window, "blur", this._fS);
            }
            this.addListener(document, 'mousemove', this._fM);
            this.addListener(document, 'mouseup', this._fS);
        } catch (e) {
            alert(e.message);
        }
        return this;
    },
    Move: function (e) {
        try {
            window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
            var i_x = e.clientX - this._x, i_y = e.clientY - this._y;
            this._dragobj.style[this._Css.x] = (this._isdrag ? Math.max(i_x, 0) : Math.max(i_x, this.Minwidth)) + 'px';
            this._dragobj.style[this._Css.y] = (this._isdrag ? Math.max(i_y, 0) : Math.max(i_y, this.Minheight)) + 'px'
            if (!this._isdrag) {
                var h = Math.max(i_y - this.Titleheight, this.Minheight - this.Titleheight) - 2 * parseInt(this.CurrentStyle(this._body).paddingLeft);
                this._body.style.height = h + 'px';
                if (this.middleBody) {
                    with (this.middleBody.style) {
                        height = ((h - this._clientHeight) < 0 ? 0 : (h - this._clientHeight)) + 'px';
                    }
                }
            }
        } catch (e) {
            alert(e.message);
        }
        return this;
    },
    Stop: function () {
        try {
            this.removeListener(document, 'mousemove', this._fM);
            this.removeListener(document, 'mouseup', this._fS);
            if (this.isIE) {
                this.removeListener(this._dragobj, "losecapture", this._fS);
                this._dragobj.releaseCapture();
            }
            else {
                this.removeListener(window, "blur", this._fS);
            };
        } catch (e) {
            alert(e.message);
        }
        return this;
    },
    ToString: function () {
        return this.GId('dialog_' + this.RandomId).innerHTML;
    },
    GId: function (id) {
        return document.getElementById(id);
    },
    isIE: function () {
        return (document.all) ? true : false;
    },
    Extend: function (destination, source) {
        for (var property in source) {
            destination[property] = source[property];
        }
    },
    Bind: function (object, fun, args) {
        return function () {
            return fun.apply(object, args || []);
        }
    },
    BindAsEventListener: function (object, fun) {
        var args = Array.prototype.slice.call(arguments).slice(2);
        return function (event) {
            return fun.apply(object, [event || window.event].concat(args));
        }
    },
    CurrentStyle: function (element) {
        return element.currentStyle || document.defaultView.getComputedStyle(element, null);
    },
    create: function (elm, parent, fn) {
        var element = document.createElement(elm);
        fn && fn(element);
        parent && parent.appendChild(element);
        return element;
    },
    addListener: function (element, e, fn) {
        element.addEventListener ? element.addEventListener(e, fn, false) : element.attachEvent("on" + e, fn);
    },
    removeListener: function (element, e, fn) {
        element.removeEventListener ? element.removeEventListener(e, fn, false) : element.detachEvent("on" + e, fn);
    }
})