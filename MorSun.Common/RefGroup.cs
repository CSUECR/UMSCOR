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
            public const string 马币来源_消费 = "fdb05cbe-1fd7-4485-a72c-eccfb17ac92a";
            public const string 马币来源_赚取 = "38e43d82-7447-4e4e-8b5a-f183f459d5b1";
            public const string 马币来源_赠送 = "40db0500-1be2-44d3-bb9a-adf146033934";

            //马币类别
            public const string 马币类别_马币 = "b6dda7ba-9cca-4b37-bfe8-5b080161a4b9";
            public const string 马币类别_邦币 = "97554448-28bd-4038-bf4a-8594e05e4bbd";
            public const string 马币类别_绑币 = "22b672fa-6334-4ecc-a7f4-c29c988c3594";

            //卡密用途
            public const string 卡密用途_充值 = "ea1586f3-4bfc-4dbe-9ce9-16e2f2fbd6cf";
            public const string 卡密用途_对照 = "6229237a-c0f7-4c16-b527-b50318245388";

            //卡密充值
            public const string 卡密充值_已充值 = "183cb93d-4b41-46ce-a1ff-8c12141660e4";
            public const string 卡密充值_未充值 = "3799d19f-db33-41f4-87f8-ffac87e68cf8";

            //卡密有效性
            public const string 卡密有效性_有效 = "97afe305-a2b1-44c4-a6d4-e7df1289e13e";
            public const string 卡密有效性_失效 = "7e5b5100-723b-430c-9690-92402d2caedd";

            //新闻类别
            public const string 新闻类别_新闻 = "78fba6d7-ade7-46eb-9662-a9f75ea26b69";
            public const string 新闻类别_维基 = "9e72c2d3-0659-46fa-8cfc-3a0f392e7192";
            public const string 新闻类别_通知 = "5b0bc466-04d8-4ef4-bf1a-7c113108b41f";
            
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

            public const string 导航菜单 = "93fe60d1-e4d5-413f-939f-1869cb3be726";
        }
    }    

    namespace 配置
    {
        public struct CFG
        {
            public const string 应用邮箱 = "s5kdm@qq.com";
            public const string 邮箱密码 = "4F2CCCEC077614815761B530E9D9164C";
            public const string 邮箱端口 = "587";


            public const string 有效时间 = "48";
            public const string 邮件通用入口 = "/Home/EL";
            public const string 账号激活路径 = "/Account/ActiveUser";
            public const string 邮件改密路径 = "/Account/ECPW";
        }
    }
}