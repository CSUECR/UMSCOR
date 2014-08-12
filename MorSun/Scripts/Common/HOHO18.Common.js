/// <reference path="../jquery-1.8.2.min.js" />

/*******************************AJAX提交后页面控制   防止重复提交**********************************************/
//获取页面正文的尺寸
function getPageSize() {
    if (document.documentElement) {
        return { width: parseInt(document.documentElement.scrollWidth), height: parseInt(document.documentElement.scrollHeight) };
    }
    else {
        return { width: parseInt(document.body.scrollWidth), height: parseInt(document.body.scrollHeight) };
    }
}
//获取浏览器窗口的尺寸
function winDimensions() {
    var winHeight = 0, winWidth = 0;
    //获取窗口宽度
    if (window.innerWidth && window.innerHeight) {
        winWidth = window.innerWidth;
        winHeight = window.innerHeight;
    }
    else if (document.documentElement && document.documentElement.clientHeight && document.documentElement.clientWidth) {
        winHeight = document.documentElement.clientHeight;
        winWidth = document.documentElement.clientWidth;
    }
    else if ((document.body) && (document.body.clientWidth)) {
        winWidth = document.body.clientWidth;
        winHeight = document.body.clientHeight;
    }
    return { winWidth: winWidth, winHeight: winHeight };
}
//把选中的项移动位置,toIndex:移动到的索引,超出范围则没任何效果
function SelectedIndexMoveTo(id, toIndex) {
    var s = $(id)[0];
    if (toIndex >= 0 && toIndex < s.options.length && s != undefined && s != null) {
        var tValue = s.options[s.selectedIndex].value;
        var tText = s.options[s.selectedIndex].innerHTML;
        s.options[s.selectedIndex].value = s.options[toIndex].value;
        s.options[s.selectedIndex].innerHTML = s.options[toIndex].innerHTML;
        s.options[toIndex].value = tValue;
        s.options[toIndex].innerHTML = tText;
        s.selectedIndex = toIndex;
    }
}

//获取滚动条的位置
function getScrollTop() {
    var scTop, scLeft;
    if (typeof window.pageYOffset != 'undefined') {
        scTop = window.pageYOffset;
        scLeft = window.pageXOffset;
    }
    else if (typeof document.compatMode != 'undefined' && document.compatMode != 'BackCompat') {
        scTop = document.documentElement.scrollTop;
        scLeft = document.documentElement.scrollLeft;
    }
    else if (typeof document.body != 'undefined') {
        scTop = document.body.scrollTop;
        scLeft = document.body.scrollLeft;
    }
    return { top: scTop, left: scLeft };
}
//获取元素绝对位置
function getAbsoluteLocation(element) {
    if (arguments.length != 1 || element == null) {
        return null;
    }
    var offsetTop = element.offsetTop;
    var offsetLeft = element.offsetLeft;
    var offsetWidth = element.offsetWidth;
    var offsetHeight = element.offsetHeight;
    while (element = element.offsetParent) {
        offsetTop += element.offsetTop - element.scrollTop;
        offsetLeft += element.offsetLeft - element.scrollLeft;
    }
    return { left: offsetLeft, top: offsetTop };
}


//获取鼠标位置
//调用mouseCoords(event),返回{x,y}
function mouseCoords(ev) {
    if (ev.pageX || ev.pageY) {
        return { x: ev.pageX, y: ev.pageY };
    }
    return {
        x: ev.clientX + document.body.scrollLeft - document.body.clientLeft,
        y: ev.clientY + document.body.scrollTop - document.body.clientTop
    };
}

//更新Loading的位置
function updateLoadingPosition() {
    //winDimensions()获取浏览器窗口的尺寸
    $(".fixedRightDown").css({ "z-index": "900000", left: ((winDimensions().winWidth - $(".fixedRightDown").attr("offsetWidth")) / 2 + getScrollTop().left) + "px", top: ((winDimensions().winHeight - $(".fixedRightDown").attr("offsetHeight")) / 2 + getScrollTop().top) + "px" });
    $("#DivWrap").css({ "width": getPageSize().width + "px", "height": getPageSize().height + "px", "position": "absolute", "top": "0", "left": "0", "z-index": "8999" });
    setTimeout(function () { updateLoadingPosition(); }, 500);
}
//Loading
function Loading(showInfo, loadID) {
    loadID = (loadID != undefined ? loadID : "#Loading");
    var l = $(loadID);

    var htmlstr = "<div id=\"loading\" > ";
    htmlstr += "<div id=\"loader_container\">";
    htmlstr += "<div id=\"loader\">";
    htmlstr += "<div align=\"center\" style=\"font-size:12px; \">" + (showInfo ? showInfo : "使劲运行中...") + "</div>";
    htmlstr += "<div align=\"center\">";
    htmlstr += "</div></div></div></div>";
    l.html(htmlstr);
    //l.html("<div class=\"Loade\"><div class=\"LoadingImg\"><img src=\"/Content/images/loading/loading_detail.gif\" /></div><div class=\"LoadingFont\">" + (showInfo ? showInfo : "载入中。。。</div>")+"</div>");
    l.show();
    var load = $("#DivWrap")[0];
    if (!load) {
        load = document.createElement("Div");
        load.id = "DivWrap";
        document.body.appendChild(load);
        $(load).html('<iframe src="javascript:{void(0)}" style="width:100%;height:100%;border:none;filter:alpha(opacity=0);-moz-opacity:0;opacity:0;" frameborder="0"></iframe>');
    }
    $(load).show();
    updateLoadingPosition();
}
//EndLoading
function EndLoading(loadID) {
    loadID = (loadID != undefined ? loadID : "#Loading");
    var l = $(loadID);
    l.hide();
    var load = $("#DivWrap")[0];
    if (!load) {
        load = document.createElement("Div");
        load.id = "DivWrap";
        $(load).css({ "filter": "alpha(opacity=0)", "-moz-opacity": "0", "opacity": "0", "background": "#000", "width": $(window).width(), "height": $(document.body).height(), "position": "absolute", "top": "0", "left": "0", "z-index": "1" });
        document.body.appendChild(load);
    }
    $(load).hide();
}

/*******************************分页所需的脚本 start********************************************/
//分页的重新刷新，需要优化
function refePage(_this,pIndex) {
//    var clickFuncStr = $(_this).attr('onclick');
//    var clickFuncArr = clickFuncStr.split(',');
//    var pIndex = clickFuncArr[1].substr(0, 1)
//    //主要为了跳页而兼容的
//    if (isNaN(pIndex)) {
//        pIndex = parseInt($('#AjaxPageSkipPageIndex').val())
//    }
    var thisUrl = window.location.href;
    //列表页中的筛选条件
    var searchFormSerialize = $('form').serialize();
    var requestUrl = "";
    //去除筛选条件的值
    var addrArr = thisUrl.split('?');
    var baseUrl = addrArr[0]; //基本地址
    if (addrArr.length > 1) {
        var queryObj = getQuerysObjByQueryStr(addrArr[1] + '&' + searchFormSerialize);
        //返回对象为空的情况
        if (isEmptyObject(queryObj)) {
            requestUrl = baseUrl + "?PIndex=" + pIndex;
        }
        else {
            queryObj["PIndex"] = pIndex;
            var queryStr = formatStringByQuerysObj(queryObj);
            requestUrl = baseUrl + '?' + formatStringByQuerysObj(queryObj);
        }
    }
    else {
        requestUrl = baseUrl + "?PIndex=" + pIndex + '&' + searchFormSerialize;
    }
    window.location.href = requestUrl;
}
///通过url获取query对象
//queryStr:url的问号后方的键值对字符串
//return:querys的键值对的对象(无论key还是value，都是小写的）
function getQuerysObjByQueryStr(queryStr) {
    queryStr = queryStr || "";
    var querysObj = {};
    //有多个关键字，
    var queryArr = queryStr.split('&');
    for (var i = 0; i < queryArr.length; i++) {
        var itemArr = queryArr[i].split('=');
        if (itemArr.length >= 2) {
            querysObj[itemArr[0]] = itemArr[1];
        }
    }
    return querysObj;
}
//通过query对象格式化query字符串,
//obj:传入query对象
//return:返回格式化query字符串
function formatStringByQuerysObj(obj) {
    var querysArr = [];
    var formatQueryStr = "";
    for (var key in obj) {
        if (typeof (obj[key]) != "function") {
            var str = key + '=' + obj[key];
            querysArr.push(str);
        }
    }
    formatQueryStr = querysArr.join('&');
    return formatQueryStr
}
//判断对象为空对象
//obj:对象
//return : true表示为空对象，false表示不为空
function isEmptyObject(obj) {
    for (var name in obj) {
        return false;
    }
    return true;

}
//某输入框,只能输入数字,在输入框的onkeydown中调用
/*
同时禁用输入法
style="ime-mode:disabled" onkeydown="return InputInteger(event)"
eg: onkeydown="return InputInteger(event)"
*/
function InputInteger(event) {
    var e = event || window.event;
    var k = e.keyCode;
    //启用删除,复制,粘贴,剪切,方向键左右
    if ((k >= 48 && k <= 57) || (k >= 96 && k <= 105) || k == 8 || k == 46 || k == 37 || k == 39 || (e.ctrlKey && (k == 67 || k == 86 || k == 88))) return true;
    else return false;
}
/*******************************分页所需的脚本 end********************************************/

/*******************************小工具********************************************************/
function AddFavorite(sURL, sTitle) {
    try { window.external.addFavorite(sURL, sTitle) } catch (e) { try { window.sidebar.addPanel(sTitle, sURL, "") } catch (e) { alert("收藏失败，Chromes浏览器请使用快捷键Ctrl+D进行添加！") } }
}
/*******************************小工具  end***************************************************/

/*******************************获取验证码****************************************************/
function getVerifyCode(text, imgControl) {
    var imgSrc = $(imgControl).attr("src");    
    if ($(text).val() != '') {        
        imgSrc += '&';        
        console.log(imgSrc);
        $(imgControl).attr("src", imgSrc);
    }
}


/*****************************返回顶部方法******************************************************/
function backtop() {
    //当滚动条的位置处于距顶部100像素以下时，跳转链接出现，否则消失
    $(function () {
        $(window).scroll(function () {
            if ($(window).scrollTop() > 200) {
                $("#back-to-top").fadeIn(1500);
            }
            else {
                $("#back-to-top").fadeOut(1500);
            }
        });
        //当点击跳转链接后，回到页面顶部位置
        $("#back-to-top").click(function () {
            $('body,html').animate({ scrollTop: 0 }, 1000);
            return false;
        });
    });
}

/*********************************一些操作********************************************************/
//跳转页面
function jumptourl(url)
{
    window.location.href = url;
}
//显示隐藏元素
function showElement(e) {
    $(e).show();
}

function hideElement(e) {
    $(e).hide();
}
