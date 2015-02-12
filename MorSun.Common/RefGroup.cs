using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// HOHO18
/// 生成于2009年4月15日
/// </summary>      
namespace MorSun.Common
{
    namespace 类别
    {

        public struct Reference
        {  
          
            #region 邮件配置类别

            public const string 插件配置 = "46bf0bbc-64e5-4e57-af43-cc5910fb3955";

            #region 邮箱栏目
            public const string 邮箱栏目_发件箱 = "fc3785f1-200e-43b4-94a7-d41b0504c690";
            public const string 邮箱栏目_收件箱 = "fb7df431-4dc9-4a17-a059-aad276724d3b";
            public const string 邮箱栏目_草稿箱 = "ca615b28-3219-4483-a5e1-4e191c4d8a32";
            #endregion

            #region 邮件重要性
            public const string 邮件重要性_一般 = "6b60b379-3515-4d3a-9f7b-1843688d3105";
            public const string 邮件重要性_重要 = "714628ea-a026-4d08-9d36-66068b160e05";
            public const string 邮件重要性_非常重要 = "84065604-c831-4f3d-a002-9ec475a5da8e";
            #endregion

            #region 附件类别
            public const string 附件类别_邮件 = "819a5bbd-9a02-42e9-96ae-86f050e52a3a";
            public const string 附件类别_公告 = "0c805c57-8a6c-4d23-8d88-d082998f6cf1";
            public const string 附件类别_项目文档 = "6ef70bd8-9f02-45ea-9e6a-86596b41789b";
            #endregion

            #region 电子邮件类别
            public const string 电子邮件类别_账号注册 = "b40ce493-8abf-42de-8182-c94061882788";
            public const string 电子邮件类别_找回密码 = "6d26c158-a9aa-475b-ba0a-c1ff9742f5c1";
            public const string 电子邮件类别_登录失败 = "147f2fd3-0d69-4b95-b6f2-db9de59eec2c";
            #endregion

            #endregion

            #region 班次类别

            public const string 节假日 = "538d6462-6535-4085-aa29-bf589e157ead";
            public const string 正常班 = "8329d584-7c61-4815-9afc-0094efe6cb5b";
            public const string 周末值班 = "c284b3e9-b7b6-4275-9fa8-d9be7ddf2395";
            public const string 值班 = "d8b55b3e-0cdc-4e74-a9e9-913d7a7da66e";
            public const string 节假日值班 = "aee5d4ef-98e6-40c3-8c7a-8bbf2a2409cc";

            #endregion             

            #region 资源类别
            public const string 资源类别_个人桌面 = "486225d8-4278-44f3-9e85-d703bfabec12";
            public const string 资源类别_资源 = "496533f6-2281-4fd4-904c-4f7b3d264d76";
            #endregion                                   

            #region 邦马
            //马币来源
            public const string 马币来源_充值 = "f4b37e62-b338-46e5-a263-6826cc70d17b";            
            public const string 马币来源_赚取 = "38e43d82-7447-4e4e-8b5a-f183f459d5b1";
            public const string 马币来源_赠送 = "40db0500-1be2-44d3-bb9a-adf146033934";
            public const string 马币来源_归还 = "ee24483b-4f0e-41ef-bf29-b9f63d670a86";
            //以上是正数 以下是添加负数
            public const string 马币来源_消费 = "fdb05cbe-1fd7-4485-a72c-eccfb17ac92a";
            public const string 马币来源_扣取 = "2e898bfe-bd58-4012-9b5b-9d69101adff4";
            public const string 马币来源_取现 = "a190a815-5553-4024-8b39-ecea3798e882";

            //马币类别
            public const string 马币类别_马币 = "b6dda7ba-9cca-4b37-bfe8-5b080161a4b9";
            public const string 马币类别_邦币 = "97554448-28bd-4038-bf4a-8594e05e4bbd";
            public const string 马币类别_绑币 = "22b672fa-6334-4ecc-a7f4-c29c988c3594";

            //卡密用途
            public const string 卡密用途_充值 = "ea1586f3-4bfc-4dbe-9ce9-16e2f2fbd6cf";
            public const string 卡密用途_对照 = "6229237a-c0f7-4c16-b527-b50318245388";

            //卡密充值
            public const string 卡密充值_未充值 = "3799d19f-db33-41f4-87f8-ffac87e68cf8";
            public const string 卡密充值_已充值 = "183cb93d-4b41-46ce-a1ff-8c12141660e4";
            public const string 卡密充值_已退款 = "7f0b1356-a4a5-4871-be72-325abc5ebd17";

            //卡密有效性
            public const string 卡密有效性_有效 = "97afe305-a2b1-44c4-a6d4-e7df1289e13e";
            public const string 卡密有效性_无效 = "7e5b5100-723b-430c-9690-92402d2caedd";

            //取现情况
            public const string 取现情况_已取 = "1fdb37e1-09cf-49d7-902b-0a761f1af3c1";
            public const string 取现情况_未取 = "6fb84b21-12e8-44f4-bdbb-9b6e3eef9b33";

            //卡密类别
            public const string 卡密类别_认证66 = "767a26ea-58ae-4afa-b8a2-eadaf61c8ee4";
            public const string 卡密类别_10 = "87e2e67a-a120-4003-851e-194ff75032bb";
            public const string 卡密类别_20 = "7f3154f2-63e4-4b47-be81-9810746f6bf0";
            public const string 卡密类别_50 = "27271639-0454-4d05-82f7-4a4ef2c62c55";

            public const string 卡密类别_100 = "32c11f96-b154-4357-9231-f8ebcbadcc18";
            public const string 卡密类别_200 = "464d72d7-8895-41a8-8be9-6f721912fbbf";
            public const string 卡密类别_500 = "216f0480-4699-42d6-a355-e83a5e512c7b";
            public const string 卡密类别_1000 = "574fc8e7-e186-4200-b1a3-a0bddd5cce34";

            //新闻类别
            public const string 新闻类别_新闻 = "78fba6d7-ade7-46eb-9662-a9f75ea26b69";
            public const string 新闻类别_维基 = "9e72c2d3-0659-46fa-8cfc-3a0f392e7192";
            public const string 新闻类别_通知 = "5b0bc466-04d8-4ef4-bf1a-7c113108b41f";
            

            //问答类别
            public const string 问答类别_问题 = "14a751b6-c23c-463c-8fdd-dff1839210b9";
            public const string 问答类别_答案 = "0b2f6da1-2f15-424a-9f96-6a43e5927f6d";
            public const string 问答类别_评论 = "590fcb9e-88b5-4f22-bc1f-be04c0d55c84";
            public const string 问答类别_放弃 = "ab2b3779-0a08-410f-8192-b79a48f28985";
            public const string 问答类别_不是问题 = "542d7de5-3465-4c25-9953-8ab0fbc096ba";

            //微信消息类别
            public const string 微信消息类别_文本 = "274a7194-7294-4cb5-9f62-3da299390dae";
            public const string 微信消息类别_图片 = "218e60f3-a822-453d-a829-772307534ded";
            public const string 微信消息类别_声音 = "b71f4cde-c13a-4dbe-a4ef-2ef99f51b426";
            public const string 微信消息类别_链接 = "b57b534e-2363-488c-a008-d02aa95b4f3d";
            public const string 微信消息类别_视频 = "7330de99-74b7-42d3-9f92-04dcd98b25af";
            public const string 微信消息类别_位置 = "b4976da7-d0af-4859-b106-cf3c2a22c081";

            //分配答题操作
            public const string 分配答题操作_待解答 = "dc2ac6b6-2937-4b04-aab8-07591614b1b3";
            public const string 分配答题操作_已解答 = "339efab2-f36c-4e7e-bc50-787730bf7959";
            public const string 分配答题操作_放弃 = "fe38a84c-4fd6-4621-a120-2c69391e4d84";
            public const string 分配答题操作_不是问题 = "ff801b8b-c3a6-4040-af92-12fb33d48f6c";
            public const string 分配答题操作_未处理 = "70be26d3-4b72-40bd-94f0-cd2546a871f4";

            //微信应用
            public const string 微信应用_作业邦 = "b61f5cee-676e-4716-9afa-4cdb44080239";
            public const string 微信应用_Net邦 = "26baaceb-b54d-47f0-accd-55ba88a92997";
            public const string 微信应用_图美美 = "eddd5bb6-4129-4428-acbf-baf5252335e3";
            public const string 微信应用_妆美人 = "4f9c851e-db8f-4965-97c0-3cf9e43c02a7";
            public const string 微信应用_色视界 = "26c99101-7a22-473a-a418-659872174032";
            public const string 微信应用_动感影 = "058e1905-acb6-46f5-aa52-c2c5665fef34";


            //微信认证类别
            public const string 认证类别_未认证 = "91ce82aa-495e-4f58-952c-35f37343ef25";
            public const string 认证类别_认证马可 = "7bedb679-8dc6-4928-81f5-ba06ad23ae70";
            public const string 认证类别_认证邦主 = "61dba173-f4d9-4171-b979-68aa993e786b";

            //在线状态
            public const string 在线状态_在线 = "da2c77e5-cdf8-413f-a722-f4325a0e2d12";
            public const string 在线状态_退出 = "963cd36d-d1c3-44d0-9499-9a7d37ef3635";

            //异议处理结果
            public const string 异议处理结果_答错 = "d22154ed-40a9-415d-b1eb-36afbb27bd31";
            public const string 异议处理结果_答对 = "4079fb81-952d-4903-9576-2cc5c18f81d9";
            public const string 异议处理结果_无标准答案 = "7f4b016a-c42d-4926-859e-a1774abda418";
            #endregion

        }
    }

    namespace 类别组
    {
        public struct RefGroup
        {
            #region 公共类别组：审批状态
            public const string 审批状态 = "33ccd953-05e8-416f-a388-4020751f5500";
            #endregion

            /// <summary>
            /// 邮箱栏目
            /// </summary>
            public const string 邮箱栏目 = "76766233-c331-4620-8fc8-b4844f6cb667";
            /// <summary>
            /// 邮件重要性
            /// </summary>
            public const string 邮件重要性 = "b4305874-2187-4fc6-967e-ba07cabd2615";
            /// <summary>
            /// 邮件重要性
            /// </summary>
            public const string 电子邮件类别 = "dc87a85f-f965-4e50-9623-6bf5c82f7c04";
            /// <summary>
            /// 上传文件类型
            /// </summary>
            public const string 上传文件类型 = "ef87e944-5877-4d6d-b81f-97af7ef6f246";
            /// <summary>
            /// 考勤机类型
            /// </summary>
            public const string 考勤机类型 = "0860b97c-f364-4bd2-865b-5b003c4b09af";
            /// <summary>
            /// 班次类别
            /// </summary>
            public const string 班次类别 = "ad4ff473-b8c1-4ebf-a3e7-a4ca8fb209f3";
            /// <summary>
            /// 有效月份
            /// </summary>
            public const string 有效月份 = "37999089-e578-40f8-923b-385fe194a878";
            /// <summary>
            /// 新闻类型
            /// </summary>
            public const string 新闻类别 = "e8152433-675e-4d20-a790-7ce571fa8e1f";
            
            /// <summary>
            /// 资源类别
            /// </summary>
            public const string 资源类别 = "03cd2760-026a-4bfb-906f-1af92bec5681";

            /// <summary>
            /// 导航菜单
            /// </summary>
            public const string 导航菜单 = "93fe60d1-e4d5-413f-939f-1869cb3be726";

            /// <summary>
            /// 问答类别
            /// </summary>
            public const string 问答类别 = "9e74d4b7-eef6-416e-9726-2422e7dd618f";

            /// <summary>
            /// 微信消息类别
            /// </summary>
            public const string 微信消息类别 = "d0774341-fbd0-4f7c-b5af-a72e3e8dabc2";

            /// <summary>
            /// 用户认证类别
            /// </summary>
            public const string 用户认证类别 = "3f348b47-1f4f-4328-b042-e4c3db3f30e2";
        }
    }    

    namespace 配置
    {
        public struct CFG
        {
            //应用基础信息配置
            public const string 应用邮箱 = "bungma@bungma.com";
            public const string 邮箱密码 = "9E2F34192E8311E739064CA878056955";
            public const string 邮箱端口 = "587";


            public const string 有效时间 = "48"; //单位 小时
            public const string 邮件通用入口 = "/H/EL";
            public const string 账号激活路径 = "/Account/ActiveUser";
            public const string 邮件改密路径 = "/Account/ECPW";
            public const string 问题查看路径 = "/qa/q";            

            public const string 默认推广代码 = "bungma";

            public const string 注册默认角色 = "98dcfbe9-fa44-4832-8a1b-5a82de8d1abf";
            public const string 作业邦认证默认角色 = "776231d4-d89a-4c1f-99c9-9c0977cc76d3";

            //微信 更改只要改这个地方
            //当前微信应用
            //网站域名  不是通过域名访问网站的情况，不能
            public const string 网站域名 = "http://www.bungma.com";
            public const string 邦马网_当前微信应用 = "b61f5cee-676e-4716-9afa-4cdb44080239";
            public const string 邦马网_微信令牌 = "WXZYBung34fdjs38";
            public const string 邦马网_加密KEY = "dZuSlbu2fMkS8mBNhNH2LXR9UZu4ng8Rg2JgqSC9lbJ";
            public const string 邦马网_应用ID = "wxa71a8636a745274b";
            public const string 邦马网_应用密钥 = "217a0e2bcc11c04f05b767c8eebd296a";
            public const string 邦马网_AT缓存键 = "ZYBAT";
            public const string 邦马网_获取AT网址 = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET";
            public const string 邦马网_TIC缓存键 = "ZYBTIC";
            public const string 邦马网_获取TIC网址 = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=ACCESS_TOKEN&type=jsapi";


            public const string 看答案指令 = "DA==>";



            //提问默认消费马币值
            public const string 提问默认收费马币值 = "500";
            //提交异议要扣取的压金值
            public const string 提交异议扣取压金值 = "500";

            //默认微信账号
            public const string 默认收费问题微信号 = "ocpytjv3sfsfsfsfsfsfsf-sfweixin-fjeiw-vneiwq";//"ocpytjv3hZIxb-GgNB0Q9-l3yWaI";//11
            public const string 默认免费问题微信号 = "ocpytjv3mfmfmfmfmfmfmf-mfweisin-vnre-qnqvink";//"ocpytjqu5aLMNmuSoJPaNfD-O2UE";//12


            //答题用户配置

            //在线答题用户缓存Key
            public const string 在线答题用户缓存键 = "olineqausers"; //默认小写
            public const string 用户待答题缓存键前缀 = "dt";
            public const string 用户马币缓存键前缀 = "mb";
            public const string 限制用户并发缓存键前缀 = "lrq";
            public const string 查看问题缓存键前缀 = "ckqa";

            //在线答题用户缓存更新时间
            public const string 在线答题用户缓存更新时间 = "1";//单位 分钟

            public const string 疑似退出时间 = "5"; //单位 分钟  超过这个数值，5分钟内不再分配新答题
            public const string 强制退出时间 = "15"; //单位 分钟  系统将该用户的答题分配给其他活跃用户，并更改该用户状态为退出 试运行先设置15分钟，否则太短，用户答题很困难，因为一张图有多道题目的存在
            public const string 用户待答题保有量 = "7"; //单位 个 系统根据总的待答题数量，除以用户待答题的保有量，等于应该取出的用户量
            public const string 未处理问题激活时间 = "5"; //单位 分钟 超过配置时间的未处理问题，系统主动激活
            
            public const string 用户连续请求时间间隔 = "15"; //单位秒 。

            public const string 用户提交异议有效时间间隔 = "72"; //

            //微信答题命令            
            public const string 开始答题 = "dt";
            public const string 放弃本题 = "fq";
            public const string 不是问题 = "bs";
            public const string 退出答题 = "tc";
            public const string 回答问题 = "hd";//文字回答问题 后面必须跟上答案
            public const string 查看问题 = "ck";
            public const string 详细信息 = "xx";//答题时获取详细信息

            //独立或当前缀命令
            public const string 我的问题前缀 = "wt";  //后面跟上数字，按提问时间逆序取问题，默认取最新的提问

            //有前缀的处理
            public const string 看答案前缀 = "kda";
            //public const string 文字答题前缀 = "hd";
            public const string 微信绑定前缀 = "bd";

            //帮助指令
            public const string 帮助指令英文 = "help";
            public const string 帮助指令中文 = "bz";

            public const string 连续答退时间间隔 = "1";//单位 小时 限制一小时内
            public const string 连续答退次数间隔 = "5";//限制5次以内

            //卡密检测
            public const string 卡密检测结果_已充值 = "已充值";
            public const string 卡密检测结果_未充值 = "未充值";

            //卡密退款
            public const string 卡密退款_退款操作失败 = "退款操作失败";
            public const string 卡密退款_请录入卡密 = "请录入卡密";
            public const string 卡密退款_该卡密已退款 = "该卡密已退款";
            public const string 卡密退款_卡密退款操作成功 = "卡密退款操作成功";

            //邦马网对接
            public const string 邦马网_对接统一码 = "F1W%I^v2&n^eJiw3F*J$V*N29E3I%W@vn#d1a@ie^4k!0";
            public const string 邦马网_字符串分隔符 = ",";

            //用户表数据同步时间
            public const string 邦马网_用户数据同步时间范围 = "1.1"; //单位 小时
            public const string 邦马网_问题数据同步时间范围 = "1.1"; //单位 小时
            public const string 邦马网_JSON数据间隔 = "||====||";            
        }
    }

    namespace 常量集
    {
        public static class ConstList
        {
            /// <summary>
            /// 可答题的认证类别
            /// </summary>
            public static List<Guid?> DTCertificationLevel = new List<Guid?>() { Guid.Parse(类别.Reference.认证类别_认证邦主) };

            /// <summary>
            /// 默认答题分配用户
            /// </summary>
            public static List<string> DefaultDISUser = new List<string>() { 配置.CFG.默认收费问题微信号, 配置.CFG.默认免费问题微信号 };
            
        }
    }
}