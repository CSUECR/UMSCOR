
$.validator.addMethod("email", function (value, element) {
    if (value == false) {
        return true;
    }
    if (this.optional(element) || /^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]*$/i.test(value)) {
        return true;
    }
    //console.log('email');
});
//电子邮件扩展方法注册
$.validator.unobtrusive.adapters.addBool("email");

$.validator.addMethod("num", function (value, element) {
    if (value == false) {
        return true;
    }
    if (this.optional(element) || /^[0-9]*$/i.test(value)) {
        return true;
    }
});
//数字扩展方法注册
$.validator.unobtrusive.adapters.addBool("num");

$.validator.addMethod("numletter", function (value, element) {
    if (value == false) {
        return true;
    }
    if (this.optional(element) || /^[A-Za-z0-9]*$/i.test(value)) {
        return true;
    }
    
});
//数字字母扩展方法注册
$.validator.unobtrusive.adapters.addBool("numletter");

//$.validator.addMethod("numletterchinese", function (value, element) {
//    if (value == false) {
//        return true;
//    }
//    if (this.optional(element) || /^[0-9A-Za-z\\u4e00-\\u9fa5]*$/i.test(value)) {        
//        return true;
//    }
//    console.log(/^[0-9A-Za-z\\u4E00-\\u9FA5]*$/i.test(value));
//});
////数字字母汉字扩展方法注册
//$.validator.unobtrusive.adapters.addBool("numletterchinese");

//$.validator.addMethod("numlettersign", function (value, element) {
//    if (value == false) {
//        return true;
//    }
//    if (this.optional(element) || /^[A-Za-z0-9!@#$%*()_+^&}{:;?.]*$/i.test(value)) {
//        return true;
//    }
//});
////数字字母符号扩展方法注册
//$.validator.unobtrusive.adapters.addBool("numlettersign");

//$.validator.addMethod("numlettersignchinese", function (value, element) {
//    if (value == false) {
//        return true;
//    }
//    if (this.optional(element) || /^[~`!@#%&-_=\\]\\};:',|0-9A-Za-z\\u002E\\u0024\\u005E\\u007B\\u005B\\u0028\\u007C\\u0029\\u002A\\u002B\\u003F\\u005C\\u4E00-\\u9FA5\\uF900-\\uFA2D]*$/i.test(value)) {
//        return true;
//    }

//});
////扩展方法注册
//$.validator.unobtrusive.adapters.addBool("numlettersignchinese");

//$.validator.addMethod("numlettersignchinesespace", function (value, element) {
//    if (value == false) {
//        return true;
//    }
//    if (this.optional(element) || /^[\s*~`!@#%&-_=\\]\\};:',|0-9A-Za-z\\u002E\\u0024\\u005E\\u007B\\u005B\\u0028\\u007C\\u0029\\u002A\\u002B\\u003F\\u005C\\u4E00-\\u9FA5\\uF900-\\uFA2D]*$/i.test(value)) {
//        return true;
//    }

//});
////扩展方法注册
//$.validator.unobtrusive.adapters.addBool("numlettersignchinesespace");



