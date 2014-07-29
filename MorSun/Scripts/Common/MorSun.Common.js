/// <reference path="../jquery-1.8.2.min.js" />
/// <reference path="../../Content/JQueryQtipCustom/jquery.qtip.min.js" />
/*
*处理页面的相关脚本
*/
//通用form提交事件处理
//btn:点击按钮
//formId:当前FormId
//errMessage:提交出错处理
//topErrDiv：顶部DIVId,可不传
function ajaxSubmitFormHandle(btn, formId, errMessage, topErrDiv, jumpUrl) {
    if (!topErrDiv)
        topErrDiv = '#divInfo';
    if (!errMessage)
        errMessage = '操作失败';
    if (!jumpUrl)
        jumpUrl = true;
    $(btn).click(function () {        
        var $ajaxSubmitForm = $(formId);
        if ($ajaxSubmitForm.valid()) {
            Loading();
            $.ajax({
                url: $ajaxSubmitForm.attr("action"),
                data: $ajaxSubmitForm.serialize(),
                type: 'POST',
                success: function (data) {
                    EndLoading();
                    //操作成功的提示信息并且跳转页面
                    if (data.ResultType == 0) {
                        $(topErrDiv).qtip({
                            content: {
                                text: data.Message,
                                title: {
                                    button: true
                                }
                            }
                            , position: {
                                target: [$('body').width() / 2, 20],
                                my: 'center center',
                                at: 'center center'
                            }
                            , show: {
                                ready: true
                            }
                             , hide: false
                        });
                        if (jumpUrl)
                        {
                            setTimeout(function () { $(topErrDiv).qtip('destroy'); window.location.href = data.AppendData; }, 2000);
                        }
                        else
                        {
                            setTimeout(function () { $(topErrDiv).qtip('destroy'); }, 2000);
                        }
                        
                    }
                    else {
                        //强制刷新验证码
                        $('#Verifycode').focus();
                        $(topErrDiv).qtip({
                            content: {
                                text: data.Message,
                                title: {
                                    button: true
                                }
                            }
                            , position: {
                                target: [$('body').width() / 2, 20],
                                my: 'center center',
                                at: 'center center'
                            }
                            , style: {
                                classes: 'qtip-red'
                            }
                            , show: {
                                ready: true
                            }
                             , hide: false
                        });
                        console.log("data" + data);
                        console.log("data.AppendData" + data.AppendData);
                        $.each(data.AppendData, function (index, valOfElement) {
                            var inputElem = "#" + valOfElement.Key;
                            var errorText = valOfElement.ErrorMessages.join(',');
                            console.log("inputElem" + inputElem + ',' + errorText);
                            console.log("qtip" + inputElem);
                            $(inputElem).qtip({
                                content: { text: errorText },
                                position: {
                                    my: 'left center',
                                    at: 'right center',
                                    viewport: $(window)
                                },
                                show: { ready: true },
                                hide: false,
                                style: {
                                    classes: 'qtip-red'
                                }
                            });
                        });
                    }
                },
                error: function (data) {
                    EndLoading();
                    alert(errMessage);
                }
            });            
        }        
    });
}
/*操作成功调用方法
*data参数与OperationResult相对应的json数据[ResultType:"xxx",Message:"xxx",AppendData:"xxx",LogMessage:"xxxx"]
*/
function onSuccess(data, topErrDiv) {
    if (!topErrDiv)
        topErrDiv = '#divInfo';
    //操作成功的提示信息并且跳转页面
    if (data.ResultType == 0) {
        $(topErrDiv).qtip({
            content: {
                text: data.Message,
                title: {
                    button: true
                }
            }
            , position: {
                target: [$('body').width() / 2, 20],
                my: 'center center',
                at: 'center center'
            }
            , show: {
                ready: true
            }
             , hide: false
        });
        setTimeout(function () { $(topErrDiv).qtip('destroy'); window.location.href = data.AppendData; }, 2000);
    }
    else {
        $(topErrDiv).qtip({
            content: {
                text: data.Message,
                title: {
                    button: true
                }
            }
            , position: {
                target: [$('body').width() / 2, 20],
                my: 'center center',
                at: 'center center'
            }
            , style: {
                classes: 'qtip-red'
            }
            , show: {
                ready: true
            }
             , hide: false
        });

        $.each(data.AppendData, function (index, valOfElement) {
            var inputElem = "#" + valOfElement.Key;

            var errorText = valOfElement.ErrorMessages.join(',');
            console.log(inputElem + ',' + errorText);
            $(inputElem).qtip({
                content: { text: errorText },
                position: {
                    my: 'left center',
                    at: 'right center',
                    viewport: $(window)
                },
                show: { ready: true },
                hide: false,
                style: {
                    classes: 'qtip-red'
                }
            });
        });
    }
}


// Alert/Confirm/Prompt
function dialogue(content, title) {
    $('<div />').qtip({
        content: {
            text: content,
            title: title
        },
        position: {
            my: 'center', at: 'center',
            target: $(window)
        },
        show: {
            ready: true,
            modal: {
                on: true,
                blur: false
            }
        },
        hide: false,
        style: 'dialogue',
        events: {
            render: function (event, api) {
                $('button', api.elements.content).click(function (e) {
                    if (e.currentTarget.id == "qtip_bttom_ok") {
                        if (e.result) {
                            api.hide(e);
                        }
                    }
                    else {
                        api.hide(e);
                    }
                });
            },
            hide: function (event, api) {
                api.destroy();
            }
        }
    });
}

//提示信息
//msg:提示内容
function AlertMessage(msg) {
    var message = $('<p />', { text: msg }),
      ok = $('<button />', { text: '确定', 'class': 'full' });
    dialogue(message.add(ok), '提示');
}

///提示信息有包含多行文本控件填写值
///inputId: 把多行文本的所填的值赋值到inputId的控件， 传值如：‘AuditMessage’
//func:回调函数
function PromptTextAreaMessage(inputId, func) {
    var message = $('<p />', { text: '审核不通过的原因' }),
   span=$('<span />'),
      input = $('<textarea />', { id: 'qtip_textarea_message', rows: '5', cols: "10", style: " width: 255px; height: 100px;" }),
       ok = $('<button />', {
          id: 'qtip_bttom_ok',
          text: '确定',
          click: function (event) {
              
              if ($('#qtip_textarea_message').val().length > 0) {
                  $('#' + inputId).val($('#qtip_textarea_message').val());
                 
                  func();
                  return true;
              } else {
                  alert('请输入内容');
                  //$('#qtip_textarea_message').qtip({ content: "请输入内容", show: { ready: true }, hide: false, style: { classes: 'qtip-red'} });
                  //setTimeout(function () { $('#qtip_textarea_message').qtip('destroy'); }, 2000); //2秒后消失  
                  return false;
              }
          }
      }),
      cancel = $('<button />', {
          text: '取消', click: function () {
              $('#' + inputId).val("");
          }
    
      });
      span.append(ok).append(cancel);
    dialogue(message.add(input).add(span), '提示!');


}

///提示信息有包含多行文本控件填写值
//msg：提示内容
///inputId: 把多行文本的所填的值赋值到inputId的控件， 传值如：‘AuditMessage’
function PromptTextAreaMessage2(msg, inputId) {
    var message = $('<p />', { text: msg }),
      input = $('<textarea />', { id: 'qtip_textarea_message', rows: '5', cols: "10", style: " width: 255px; height: 100px;" }),
      ok = $('<button />', {
          text: '确定',
          click: function () {
              $('#' + inputId).val($('#qtip_textarea_message').val());
          }
      }),
      cancel = $('<button />', {
          text: '取消', click: function () {
              $('#' + inputId).val("");
          }
      });

    dialogue(message.add(input).add(ok).add(cancel), '提示!');
}
//window.Confirm = function () {
//    var message = $('<p />', { text: 'Click a button to exit the custom dialogue' }),
//      ok = $('<button />', {
//          text: '确定'
//      }),
//      cancel = $('<button />', {
//          text: '取消'
//      });

//    dialogue(message.add(ok).add(cancel), 'Do you agree?');
//}
//$(document).ready(function () {
//    $('.field-validation-error').each(function () {
//        var inputElem = '#' + $(this).attr('data-valmsg-for').replace('.', '_').replace('[', '_').replace(']', '_');
//        $(this).css("display", "none");
//        $(inputElem).filter(':not(.valid)').qtip({
//            content: { text: $(this).text() },
//            position: {
//                //my: 'top left',
//                //at: 'bottom right',
//                my: 'left center',
//                at: 'right center',
//                viewport: $(window)
//            },
//            show: { ready: true },
//            hide: false,
//            style: {
//                classes: 'qtip-red'
//            }
//        });
//    });
//});