﻿/// <reference path="../jquery-1.8.2.min.js" />

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