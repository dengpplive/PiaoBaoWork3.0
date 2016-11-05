(function () {
    $.extend($.fn, {
        mask: function (msg, maskDivClass) {
            this.unmask();
            var op = {
                opacity: 0.5,
                z: 10000,
                bgcolor: '#ccc'
            };
            var original = $(document.body);
            var position = { top: 0, left: 0 };
            if (this[0] && this[0] !== window.document) {
                original = this;
                position = original.position();
            }
            var maskDiv = $('<div class="maskdivgen"> </div>');
            maskDiv.appendTo(original);
            var maskWidth = original.outerWidth();
            if (!maskWidth) {
                maskWidth = original.width();
            }
            var maskHeight = original.outerHeight();
            if (!maskHeight) {
                maskHeight = original.height();
            }
            maskDiv.css({
                position: 'absolute',
                top: position.top,
                left: position.left,
                'z-index': op.z,
                width: maskWidth,
                height: maskHeight,
                'background-color': op.bgcolor,
                opacity: 0
            });
            if (maskDivClass) {
                maskDiv.addClass(maskDivClass);
            }
            if (msg) {
                var msgDiv = $('<div style="position:absolute;border:#6593cf 1px solid; padding:2px;background:#ccca">' + '<div style="line-height:24px;border:#a3bad9 1px solid;background:white;padding:2px 10px 2px 10px">' + msg + '</div></div>');
                msgDiv.appendTo(maskDiv);
                var widthspace = (maskDiv.width() - msgDiv.width());
                var heightspace = (maskDiv.height() - msgDiv.height());
                msgDiv.css({
                    cursor: 'wait',
                    top: (heightspace / 2 - 2),
                    left: (widthspace / 2 - 2)
                });
            }
            maskDiv.fadeIn('fast', function () {
                $(this).fadeTo('slow', op.opacity);
            })
            return maskDiv;
        },
        unmask: function () {
            var original = $(document.body);
            if (this[0] && this[0] !== window.document) {
                original = $(this[0]);
            }
            original.find("> div.maskdivgen").fadeOut('slow', 0, function () {
                $(this).remove();
            });
        }
    });
})();