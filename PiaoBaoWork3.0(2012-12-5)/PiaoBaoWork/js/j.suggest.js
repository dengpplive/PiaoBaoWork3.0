	/// <reference path="jquery-1.3.2-vsdoc.js" />
	
	(function ($) {
	    var pingyin = '中文/拼音', ping1 = '请输入中文/拼音或者↑↓选择', sort = ",按拼音排序", notan = '找不到,';
	    $.suggest = function (input, options) {

	        var $input = $(input).attr("autocomplete", "off");
	        var $results;

	        var timeout = false; 	// hold timeout ID for suggestion results to appear	
	        var prevLength = 0; 		// last recorded length of $input.val()
	        var cache = []; 			// cache MRU list
	        var cacheSize = 0; 		// size of cache in chars (bytes?)

	        if ($.trim($input.val()) == '' || $.trim($input.val()) == pingyin) $input.val(pingyin).css('color', '#aaa');
	        if (!options.attachObject)
	            options.attachObject = $(document.createElement("ul")).appendTo('body');

	        $results = $(options.attachObject);
	        $results.addClass(options.resultsClass);

	        resetPosition();
	        $(window)
				.load(resetPosition)		// just in case user is changing size of page while loading
				.resize(resetPosition);

	        $input.blur(function () {
	            setTimeout(function () { $results.hide() }, 200);
	            if ($.trim($(this).val()) == '') {
	                $(this).val(pingyin);
	            }
	            else if ($results.css("display") == "block") {
	                selectCurrentResult();
	            }

	        });

	        $input.focus(function () {
	            if ($.trim($(this).val()) == pingyin) {
	                $(this).val('').css('color', '#000');
	            }
	            if ($.trim($(this).val()) == '') {
	                displayItems(''); //显示热门城市列表
	            }
	        });
	        $input.click(function () {
	            var q = $.trim($(this).val());
	            displayItems(q);
	            $(this).select();
	        });

	        // help IE users if possible
	        try {
	            $results.bgiframe();
	        } catch (e) { }

	        $input.keyup(processKey); //

	        function resetPosition() {
	            // requires jquery.dimension plugin
	            var offset = $input.offset();
	            $results.css({
	                top: (offset.top + input.offsetHeight) + 'px',
	                left: offset.left + 'px'
	            });
	        }


	        function processKey(e) {

	            // handling up/down/escape requires results to be visible
	            // handling enter/tab requires that AND a result to be selected

	            if ((/27$|38$|40$/.test(e.keyCode) && $results.is(':visible')) ||
					(/^13$|^9$/.test(e.keyCode) && getCurrentResult())) {

	                if (e.preventDefault)
	                    e.preventDefault();
	                if (e.stopPropagation)
	                    e.stopPropagation();

	                e.cancelBubble = true;
	                e.returnValue = false;

	                switch (e.keyCode) {

	                    case 38: // up
	                        prevResult();
	                        break;

	                    case 40: // down
	                        nextResult();
	                        break;
	                    case 13: // return
	                        selectCurrentResult();
	                        break;

	                    case 27: //	escape
	                        $results.hide();
	                        break;

	                }

	            } else if ($input.val().length != prevLength) {

	                if (timeout)
	                    clearTimeout(timeout);
	                timeout = setTimeout(suggest, options.delay);
	                prevLength = $input.val().length;

	            }
	        }

	        function suggest() {

	            var q = $.trim($input.val());
	            displayItems(q);
	        }
	        function displayItems(items) {            
	            //var topPx = location.search != '' ? location.search : '-8';	          
	            var html = '';
	            var htnum = 0;
	            if (items == '') {//热门城市遍历
	                for (h in options.hot_list) {
	                    html += '<li rel="' + options.hot_list[h][0] + '"><a href="#' + h + '"><span>' + options.hot_list[h][2] + '</span>' + options.hot_list[h][1] + '</a></li>';
	                    htnum += 19;
	                }
	                html = '<div id="showcity" style="height:20px;width:200px;float:left;" class="gray ac_result_tip">' + ping1 + '</div><br /><ul>' + html + '</ul>';
	                htnum += 24;
	            }
	            else {
	                /*if (!items)
	                return;
	                if (!items.length) {
	                $results.hide();
	                return;
	                }*/
	                for (var i = 0; i < options.source.length; i++) {//国内城市匹配
	                    var reg = new RegExp('^' + items + '.*$', 'im');
	                    if (reg.test(options.source[i][0]) || reg.test(options.source[i][1]) || reg.test(options.source[i][2]) || reg.test(options.source[i][3])) {
	                        html += '<li style="height:20px;" rel="' + options.source[i][0] + '"><a href="#' + i + '"><span>' + options.source[i][2] + '</span>' + options.source[i][1] + '</a></li>';
	                        htnum += 19;
	                    }
	                }
	                if (html == '') {

	                    suggest_tip = '<div class="gray ac_result_tip">' + notan + items + '</div>';
	                    htnum += 24;
	                }
	                else {

	                    suggest_tip = '<div class="gray ac_result_tip">' + items + sort + '</div>';
	                    htnum += 24;
	                }
	                html = suggest_tip + '<ul>' + html + '</ul>';
	            }
	            html = html + "<iframe id='DivShim' src='" + options.iframeUrl + "' scrolling='no' frameborder='0' style='position:absolute;top:0px; left:0px;z-index:-10' width='200px'></iframe>";


	            //alert(html);
	            $results.html(html).show();
	            resetPosition();
	            //$results.css("height",300);
	            //alert($results.children("#showcity").css("height"));


	            $results.children("#DivShim").css("height", "" + ((htnum * 1)) + "")
	            //              alert($results.children("#DivShim").css("top")+","+$results.children("#DivShim").css("left")+","+$results.children("#DivShim").css("zIndex"));

	            $results.children('ul').children('li:first-child').addClass(options.selectClass);

	            $results.children('ul')
					.children('li')
					.mouseover(function () {
					    $results.children('ul').children('li').removeClass(options.selectClass);
					    $(this).addClass(options.selectClass);
					})
					.click(function (e) {
					    e.preventDefault();
					    e.stopPropagation();
					    selectCurrentResult();
					});
	        }

	        function getCurrentResult() {

	            if (!$results.is(':visible'))
	                return false;

	            var $currentResult = $results.children('ul').children('li.' + options.selectClass);
	            if (!$currentResult.length)
	                $currentResult = false;
	            return $currentResult;

	        }

	        function selectCurrentResult() {
	            $currentResult = getCurrentResult();
	            if ($currentResult) {
	                $input.val($currentResult.children('a').html().replace(/<span>.+?<\/span>/i, ''));
	                if (document.getElementById(options.HidId) != undefined) {
	                    document.getElementById(options.HidId).value = $currentResult[0].rel;
	                }
	                //$input.val($currentResult.children('a').html().replace(/<span>.+?<\/span>/i,''));
	                $results.hide();
	                if ($(options.dataContainer)) {
	                    $(options.dataContainer).val($currentResult.attr('rel'));
	                }
	                if (options.onSelect) {

	                    options.onSelect.apply($input[0]);
	                }
	            }
	            else {
	                if ($results.css("display") == "block") {
	                    $(options.dataContainer).val("");
	                }
	            }

	        }

	        function nextResult() {

	            $currentResult = getCurrentResult();

	            if ($currentResult)
	                $currentResult
						.removeClass(options.selectClass)
						.next()
							.addClass(options.selectClass);
	            else
	                $results.children('ul').children('li:first-child').addClass(options.selectClass);

	        }

	        function prevResult() {

	            $currentResult = getCurrentResult();

	            if ($currentResult)
	                $currentResult
						.removeClass(options.selectClass)
						.prev()
							.addClass(options.selectClass);
	            else
	                $results.children('ul').children('li:last-child').addClass(options.selectClass);

	        }

	    }

	    $.fn.suggest = function (source, options) {

	        if (!source)
	            return;

	        options = options || {};
	        options.source = source;
	        options.hot_list = options.hot_list || [];
	        options.delay = options.delay || 0;
	        options.resultsClass = options.resultsClass || 'ac_results';
	        options.selectClass = options.selectClass || 'ac_over';
	        options.matchClass = options.matchClass || 'ac_match';
	        options.minchars = options.minchars || 1;
	        options.delimiter = options.delimiter || '\n';
	        options.onSelect = options.onSelect || false;
	        options.dataDelimiter = options.dataDelimiter || '\t';
	        options.dataContainer = options.dataContainer || '#ac_results';
	        options.attachObject = options.attachObject || null;
	        options.iframeUrl = options.iframeUrl || "blank.htm";

	        this.each(function () {
	            new $.suggest(this, options);
	        });

	        return this;

	    };

	})(jQuery);