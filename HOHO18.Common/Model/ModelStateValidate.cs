using System;
using System.Text;
using System.Text.RegularExpressions;

namespace HOHO18.Common
{
    /// <summary>
    /// 数据校验类
    /// HOHO18
    /// 2009.4
    /// </summary>
    public sealed class ModelStateValidate
    {
        private static Regex Intege = new Regex("^-?[0-9]\\d*$", RegexOptions.Singleline);					//整数
        private static Regex IntegeZ = new Regex("^[1-9]\\d*$", RegexOptions.Singleline);					//正整数
        private static Regex IntegeF = new Regex("^-[1-9]\\d*$", RegexOptions.Singleline);						//负整数

        private static Regex Num = new Regex("^([+-]?)\\d*\\.?\\d+$", RegexOptions.Singleline);				//数字
        private static Regex NumZ = new Regex(@"^\d+(\.\d+)?$", RegexOptions.Singleline);//正数（正整数 + 0）原"^[1-9]\\d*|0$"
	    private static Regex NumF = new Regex("^-[1-9]\\d*|0$",RegexOptions.Singleline);					//负数（负整数 + 0）

	    private static Regex Decmal = new Regex("^([+-]?)\\d*\\.\\d+$",RegexOptions.Singleline);			//浮点数
	    private static Regex DecmalZ = new Regex("^[1-9]\\d*.\\d*|0.\\d*[1-9]\\d*$",RegexOptions.Singleline);　　	//正浮点数
	    private static Regex DecmalF = new Regex("^-([1-9]\\d*.\\d*|0.\\d*[1-9]\\d*)$",RegexOptions.Singleline);　 //负浮点数
	    private static Regex Decmal1 = new Regex("^-?([1-9]\\d*.\\d*|0.\\d*[1-9]\\d*|0?.0+|0)$",RegexOptions.Singleline);　 //浮点数
	    private static Regex Decmal1Z = new Regex("^[1-9]\\d*.\\d*|0.\\d*[1-9]\\d*|0?.0+|0$",RegexOptions.Singleline);　　 //非负浮点数（正浮点数 + 0）
	    private static Regex Decmal1F = new Regex("^(-([1-9]\\d*.\\d*|0.\\d*[1-9]\\d*))|0?.0+|0$",RegexOptions.Singleline);　　//非正浮点数（负浮点数 + 0）

	    private static Regex Email = new Regex("^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$",RegexOptions.Singleline); //邮件
	    private static Regex Color = new Regex("^[a-fA-F0-9]{6}$",RegexOptions.Singleline);			//颜色
	    private static Regex Url = new Regex("^http[s]?:\\/\\/([\\w-]+\\.)+[\\w-]+([\\w-./?%&=]*)?$",RegexOptions.Singleline);	//url
	    private static Regex Chinese = new Regex("^[\\u4E00-\\u9FA5\\uF900-\\uFA2D]+$",RegexOptions.Singleline);					//仅中文
	    private static Regex Ascii = new Regex("^[\\x00-\\xFF]+$",RegexOptions.Singleline);				//仅ACSII字符
	    private static Regex Zipcode = new Regex("^\\d{6}$",RegexOptions.Singleline);						//邮编	   
	    private static Regex Ip4 = new Regex("^(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)\\.(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)$",RegexOptions.Singleline);	//ip地址
	    private static Regex NotEmpty = new Regex("^\\S+$",RegexOptions.Singleline);						//非空
	    private static Regex Picture = new Regex("(.*)\\.(jpg|bmp|gif|ico|pcx|jpeg|tif|png|raw|tga)$",RegexOptions.Singleline);	//图片
	    private static Regex Rar = new Regex("(.*)\\.(rar|zip|7zip|tgz)$",RegexOptions.Singleline);								//压缩文件
        private static Regex Date = new Regex("^\\d{4}(\\-|\\/|\\u002E)\\d{1,2}\\1\\d{1,2}$",RegexOptions.Singleline);					//日期
        private static Regex ShortTime = new Regex("^(\\d{1,2})(:)?(\\d{1,2})\\2(\\d{1,2})$",RegexOptions.Singleline);					//短时间
        private static Regex ShortDate = new Regex("^(\\d{1,4})(-|\\/)(\\d{1,2})\\2(\\d{1,2})$",RegexOptions.Singleline);					//短日期
        private static Regex LongTime = new Regex("^(\\d{1,4})(-|\\/)(\\d{1,2})\\2(\\d{1,2}) (\\d{1,2}):(\\d{1,2}):(\\d{1,2})$",RegexOptions.Singleline);					//长时间
	    private static Regex Qq = new Regex("^[1-9]*[1-9][0-9]*$",RegexOptions.Singleline);				//QQ号码
	    private static Regex Tel = new Regex("^(([0\\+]\\d{2,3}-)?(0\\d{2,3})-)?(\\d{7,8})(-(\\d{3,}))?$",RegexOptions.Singleline);	//电话号码的函数(包括验证国内区号,国际区号,分机号)
	    private static Regex Username = new Regex("^\\w+$",RegexOptions.Singleline);						//用来用户注册。匹配由数字、26个英文字母或者下划线组成的字符串
	    private static Regex Letter = new Regex("^[A-Za-z]+$",RegexOptions.Singleline);					//字母
	    private static Regex LetterU = new Regex("^[A-Z]+$",RegexOptions.Singleline);					//大写字母
        private static Regex LetterL = new Regex("^[a-z]+$",RegexOptions.Singleline);					//小写字母
        private static Regex Idcard = new Regex("^[1-9]([0-9]{14}|([0-9]{16}(\\d|x|X)))$",RegexOptions.Singleline);    //new Regex("^[1-9]([0-9]{14}|[0-9]{17})$",RegexOptions.Singleline);	//身份证 
        private static Regex Passport=new Regex("(P\\d{7})|(G\\d{8})",RegexOptions.Singleline);//护照
        private static Regex Guid = new Regex(@"^[a-zA-Z0-9]{8}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{12}$",RegexOptions.Singleline);	//GUID    

        
        private static Regex Required = new Regex(".+",RegexOptions.Singleline);//必须
        private static Regex Empty = new Regex("^[\\s| ]*$",RegexOptions.Singleline);//空
        private static Regex Phone = new Regex("^((\\(\\d{2,3}\\))|(\\d{3}\\-))?(\\(0\\d{2,3}\\)|0\\d{2,3}-)?[1-9]\\d{6,7}(\\-\\d{1,4})?$",RegexOptions.Singleline);
        private static Regex MobileMe = new Regex("^((\\(\\d{2,3}\\))|(\\d{3}\\-))?((1\\d{10})|(1\\d{10}))$",RegexOptions.Singleline);
        private static Regex Number = new Regex("^\\d+$",RegexOptions.Singleline);
        private static Regex QqMe = new Regex("^\\d{5,10}$",RegexOptions.Singleline);
        private static Regex Chinaname = new Regex("^[\u4e00-\u9fa5]{0,5}$",RegexOptions.Singleline);
        private static Regex UsernameMe = new Regex("^.{4,16}$",RegexOptions.Singleline);
        private static Regex EmailMe = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$",RegexOptions.Singleline);
        private static Regex NumShu = new Regex("^[|0-9]+$", RegexOptions.Singleline);              //匹配数字和|
        private static Regex NumLetter = new Regex("^[A-Za-z0-9]+$",RegexOptions.Singleline);              //匹配数字和字母
        private static Regex ChineseLetter = new Regex("^[A-Za-z\\u4E00-\\u9FA5\\uF900-\\uFA2D]+$",RegexOptions.Singleline);                  // 字母和汉字
        private static Regex NumChineseLetter = new Regex("^[0-9A-Za-z\\u4E00-\\u9FA5\\uF900-\\uFA2D]+$",RegexOptions.Singleline);              // 字母和汉字和数字
        private static Regex Address = new Regex("^[\\s#\\u002E0-9A-Za-z\\u4E00-\\u9FA5\\uF900-\\uFA2D\\u002E\\u0028\\u0029#-]+$",RegexOptions.Singleline);      //地址
        private static Regex PropertyNo = new Regex("^[0-9A-Za-z/-]+$", RegexOptions.Singleline);      //房源编号   
        //private static Regex NumChineseLetterSign = new Regex("^[0-9A-Za-z:Pu:Po:Ps:Pe:Pf:Pd:Pc:Zs\\u4E00-\\u9FA5\\uF900-\\uFA2D]+$", RegexOptions.Singleline);              // 字母和汉字和数字和符号
        private static Regex NumChineseLetterSign = new Regex("^[~`!@#%&-_=\\]\\};:',/|0-9A-Za-z\\u002E\\u0024\\u005E\\u007B\\u005B\\u0028\\u007C\\u0029\\u002A\\u002B\\u003F\\u005C\\u4E00-\\u9FA5\\uF900-\\uFA2D]*$", RegexOptions.Singleline);              // 字母和汉字和数字和符号
        //除 .$ ^ { [ ( | ) * + ? \ 外，其他字符与自身匹配。~`!@#%&-_=]};:"',/|
        //点的转义：.  ==> \\u002E
        //美元符号的转义：$  ==> \\u0024
        //乘方符号的转义：^  ==> \\u005E
        //左大括号的转义：{  ==> \\u007B
        //左方括号的转义：[  ==> \\u005B
        //左圆括号的转义：(  ==> \\u0028
        //竖线的转义：| ==> \\u007C
        //右圆括号的转义：) ==> \\u0029
        //星号的转义：*  ==> \\u002A
        //加号的转义：+  ==> \\u002B
        //问号的转义：?  ==> \\u003F
        //反斜杠的转义：\ ==> \\u005C 
#region
        //时间正则表达式
//        string regex = @"^((\d{2}(([02468][048])|([13579][26]))[\-\/\s]?((((0?[13578]
//)|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[4
//69])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\
//s]?((0?[1-9])|([1-2][0-9])))))|(\d{2}(([02468][1235679])|([1
//3579][01345789]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((
//0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((
//0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|(1[0-9]
//)|(2[0-8]))))))"; //日期部分
//  regex += @"(\s(((0?[0-9])|([1-2][0-3]))\:([0-5]?[0-9])((\s)|(\:([0-5]?[0-9])))))?$"; //时间部分
#endregion


        
        private bool test()
        {
            return Regex.IsMatch("00-00-000", @"^[a-zA-Z0-9]{8}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{4}\-[a-zA-Z0-9]{12}$", RegexOptions.Singleline);
        }
      
        #region 整数
        /// <summary>
        /// 用于服务端验证整数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsIntege(string data)
        {
            Match m = Intege.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证正整数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsIntegeZ(string data)
        {
            Match m = IntegeZ.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验负整数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsIntegeF(string data)
        {
            Match m = IntegeF.Match(data);
            return m.Success;
        }

        #endregion

        #region 数字
        /// <summary>
        /// 用于服务端验证数字
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNum(string data)
        {
            Match m = Num.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证正数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNumZ(string data)
        {
            Match m = NumZ.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证负数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNumF(string data)
        {
            Match m = NumF.Match(data);
            return m.Success;
        }
        #endregion

        #region 浮点数
        /// <summary>
        /// 用于服务端验证浮点数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsDecmal(string data)
        {
            Match m = Decmal.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证正浮点数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsDecmalZ(string data)
        {
            Match m = DecmalZ.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证负浮点数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsDecmalF(string data)
        {
            Match m = DecmalF.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证浮点数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsDecmal1(string data)
        {
            Match m = Decmal1.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证非负浮点数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsDecmal1Z(string data)
        {
            Match m = Decmal1Z.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证非正浮点数
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsDecmal1F(string data)
        {
            Match m = Decmal1F.Match(data);
            return m.Success;
        }
        #endregion

        #region 其他
        
        /// <summary>
        /// 验证是否是GUID
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsGuid(string data)
        {
            Match m = Guid.Match(data);
            return m.Success;
        }
        /// <summary>
        /// 验证是否是字母或汉字
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsChineseLetter(string data)
        {
            Match m = ChineseLetter.Match(data);
            return m.Success;
        }
        /// <summary>
        /// 验证是否是数字字母汉字
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNumChineseLetter(string data)
        {
            Match m = NumChineseLetter.Match(data);
            return m.Success;
        }
        /// <summary>
        /// 验证是否是数字字母汉字符号
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNumChineseLetterSign(string data)
        {
            Match m = NumChineseLetterSign.Match(data);
            return m.Success;
        }
        /// <summary>
        /// 用于服务端验证邮箱
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsEmail(string data)
        {
            Match m = Email.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证地址
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsAddress(string data)
        {
            Match m = Address.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证房产证号
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsPropertyNo(string data)
        {
            Match m = PropertyNo.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 验证是否是数字或字母
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNumLetter(string data)
        {
            Match m = NumLetter.Match(data);
            return m.Success;
        }
        
        /// <summary>
        /// 用于服务端验证颜色
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsColor(string data)
        {
            Match m = Color.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证url
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsUrl(string data)
        {
            Match m = Url.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证中文
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsChinese(string data)
        {
            Match m = Chinese.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证Ascii
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsAscii(string data)
        {
            Match m = Ascii.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证邮编
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsZipcode(string data)
        {
            Match m = Zipcode.Match(data);
            return m.Success;
        }        

        /// <summary>
        /// 用于服务端验证IP地址
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsIp4(string data)
        {
            Match m = Ip4.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证非空
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNotEmpty(string data)
        {
            Match m = NotEmpty.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证图片
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsPicture(string data)
        {
            Match m = Picture.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证压缩文件
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsRar(string data)
        {
            Match m = Rar.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证日期
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsDate(string data)
        {
            Match m = Date.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证短时间
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsShortTime(string data)
        {
            Match m = ShortTime.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证短日期
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsShortDate(string data)
        {
            Match m = ShortDate.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证长时间
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsLongTime(string data)
        {
            Match m = LongTime.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证QQ
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsQq(string data)
        {
            Match m = Qq.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证电话号码
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsTel(string data)
        {
            Match m = Tel.Match(data);
            return m.Success;
        }        

        /// <summary>
        /// 用于服务端验证用来用户注册。匹配由数字、26个英文字母或者下划线组成的字符串
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsUsername(string data)
        {
            Match m = Username.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证字母
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsLetter(string data)
        {
            Match m = Letter.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证大写字母
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsLetterU(string data)
        {
            Match m = LetterU.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证小写字母
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsLetterL(string data)
        {
            Match m = LetterL.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证身份证
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsIdcard(string data)
        {
            Match m = Idcard.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证非空值
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsRequired(string data)
        {
            Match m = Required.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证空值
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsEmpty(string data)
        {
            Match m = Empty.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证电话，固定电话及手机都可以
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsPhone(string data)
        {
            Match m = Phone.Match(data);
            return m.Success;
        }
        

        /// <summary>
        /// 用于服务端验证数字
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNumber(string data)
        {
            Match m = Number.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证手机Me
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsMobileMe(string data)
        {
            Match m = MobileMe.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证QQMe
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsQqMe(string data)
        {
            Match m = QqMe.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证中文名
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsChinaname(string data)
        {
            Match m = Chinaname.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证用户名Me
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsUsernameMe(string data)
        {
            Match m = UsernameMe.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证电子邮件Me
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsEmailMe(string data)
        {
            Match m = EmailMe.Match(data);
            return m.Success;
        }

        /// <summary>
        /// 用于服务端验证数字和|
        /// </summary>
        /// <param name="data">待验证数据</param>
        /// <returns></returns>
        public static bool IsNumShu(string data)
        {
            Match m = NumShu.Match(data);
            return m.Success;
        }

        #endregion 
    }
}
