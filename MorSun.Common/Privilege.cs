using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace MorSun.Common.Privelege
{
   
    /// <summary>
    /// 生成于2011年05月05日
    /// </summary>
    public struct 操作
    {
        public const string 添加 = "332b5605-335a-4308-a755-23c348cbcc5b";
        public const string 修改 = "e9303f56-c2e7-4636-a42a-4973fee38b1e";
        public const string 删除 = "4a5bef1a-d2c0-459d-8966-b20446fd5a5a";
        public const string 查看 = "1a5292a5-a119-49b8-a209-a9c14c5d0b3a";
        public const string 彻底删除 = "b9409de8-4804-492f-9476-e0d1092091fc";
        public const string 回收站 = "78bce0ee-1dc0-4cd1-8400-4cf6c75220dc";
        public const string 配置 = "87aba619-6ae7-4c66-b32e-b3513310dba2";

        #region 操作范围
        public const string 系统管理员 = "259ff2fa-66bf-454a-8cc5-babfd9b6cda9";        
        public const string 部门经理 = "4fbfac8b-d704-4226-a751-362ef6ed949b";        
        public const string 普通员工 = "9a5c620d-2c38-4b36-ba24-7bfd3c182efa"; 
        #endregion
    }


    public struct 资源
    {
        /// <summary>
        /// 部门管理
        /// </summary>
        public const string 部门 = "401d4647-9ae4-4aaa-b126-d855d12618ec";
        /// <summary>
        /// 类别组
        /// </summary>
        public const string 类别组 = "d968629f-6e34-4ce0-8c2c-6083e6805665";
        /// <summary>
        /// 类别
        /// </summary>
        public const string 类别 = "fd1f62ea-1bd2-46d0-8a83-ae1947b60893";
        /// <summary>
        /// 操作
        /// </summary>
        public const string 操作 = "f49a80d1-6989-4182-aecc-9d5d3ccdff55";
        /// <summary>
        /// 岗位
        /// </summary>
        public const string 岗位 = "b7b59163-71c2-429e-8e86-d0abc9576262";
        /// <summary>
        /// 权限
        /// </summary>
        public const string 权限 = "bd3bc770-2437-4105-9e93-11eabbb3be75";
        /// <summary>
        /// 日志
        /// </summary>
        public const string 日志 = "f1439f13-9ff4-4ce6-8759-9faaf3a891f6";
        /// <summary>
        /// 资源管理
        /// </summary>
        public const string 资源管理 = "048f512f-c5d6-47d3-9586-1e1f5131b757";
        /// <summary>
        /// 系统参数配置
        /// </summary>
        public const string 系统参数配置 = "ba61df81-f4f0-48d6-9f8b-f8c6b57ebdb7";
        /// <summary>
        /// 员工
        /// </summary>
        public const string 用户 = "76309187-c594-467e-910b-78d5a6d79403";
        /// <summary>
        /// 角色配置
        /// </summary>
        public const string 角色配置 = "af927396-8d91-4563-a733-b1afbd84e4a1";
        /// <summary>
        /// 个人资料
        /// </summary>
        public const string 个人资料 = "5ba61f34-0e42-4188-8d48-4e9d7d665f18";

        /// <summary>
        /// 省
        /// </summary>
        public const string 省 = "36f16cf0-ab19-4e91-8da1-b702fa236cc8";

        /// <summary>
        /// 设区市
        /// </summary>
        public const string 设区市 = "50fa15be-14aa-4e9c-aa78-c7597cfea102";
        /// <summary>
        /// 县市区
        /// </summary>
        public const string 县市区 = "dc55b21e-6a51-41ac-99c1-f6630b09098b";
        /// <summary>
        /// 镇乡
        /// </summary>
        public const string 镇乡 = "5251ed09-cc52-4058-9f67-17f74f9a2714";
        /// <summary>
        /// 建制村
        /// </summary>
        public const string 建制村 = "6317c2ed-0283-4597-917c-421cdd4142ad";
        /// <summary>
        /// 头部菜单
        /// </summary>
        public const string 头部菜单 = "0431df8e-3cc6-493a-aee6-5e126d841735";
        /// <summary>
        /// 左侧菜单
        /// </summary>
        public const string 导航菜单 = "340aaeed-7d57-4a63-ae4d-6a4bcee8ebea";


        /// <summary>
        /// 数据库备份
        /// </summary>
        public const string 数据库备份 = "d838f110-e6fa-4d2b-9fcb-018312a24ac8";
        ///// <summary>
        ///// 岗位级别
        ///// </summary>
        //public const string 岗位级别 = "37019c92-2411-4cda-a055-0c77c863d898";


        /// <summary>
        /// 引导菜单名称配置 
        /// </summary>
        public const string 引导菜单名称配置 = "f6eeb15b-9bcd-4208-b2f6-b02646042e1a";

        /// <summary>
        /// 内部邮件管理
        /// </summary>
        public const string 内部邮件 = "1013c4dd-f5be-4bf7-a781-18df3a22d97d";
        /// <summary>
        /// 考勤机
        /// </summary>
        public const string 考勤机 = "43431ae8-7a06-49ca-9685-026ad36e5013";
        /// <summary>
        /// 我的考勤
        /// </summary>
        public const string 我的考勤 = "046b634b-2006-4198-94b7-8b29004f0794";
        /// <summary>
        /// 排班管理
        /// </summary>
        public const string 排班模板 = "4e514b62-3ab0-4a52-8175-0fe552a162c4";
        /// <summary>
        /// 设置排班
        /// </summary>
        public const string 设置排班 = "9c8ff3bc-b1d4-4fde-8968-7a9a57c635b2";
        /// <summary>
        /// 免签人员管理
        /// </summary>
        public const string 免签人员管理 = "f8142e7f-d350-4490-a941-4d173530e254";
        /// <summary>
        /// 节假日管理
        /// </summary>
        public const string 节假日管理 = "ad51e654-d6c6-4baf-9dae-cc72e7c17151";
        /// <summary>
        /// 班次类型管理
        /// </summary>
        public const string 班次类型管理 = "d7d2442d-e34f-4307-9954-1bb046d80668";
        /// <summary>
        /// 班次管理
        /// </summary>
        public const string 班次管理 = "7802926e-886d-4193-b0b7-728bd1017842";
        /// <summary>
        /// 班次
        /// </summary>
        public const string 班次 = "ddebbe36-9061-489e-9029-e42713bb5485";
        /// <summary>
        /// 默认排班模板
        /// </summary>
        public const string 默认排班模板 = "5441ad74-20bb-4a10-b46a-c695d70643e0";
        /// <summary>
        /// 员工排班
        /// </summary>
        public const string 安排值班 = "0d2c999b-5b48-4fe6-b8e5-88d075867e6a";

        /// <summary>
        /// 考勤申请表
        /// </summary>
        public const string 考勤申请表 = "aad00ee5-faa1-40fe-af71-1d7840938a2b";


        //操作范围
        public const string 操作范围 = "57688bea-7ea6-4473-9c9d-608f1b41bae2";   
     
        //新闻
        public const string 新闻 = "5857b234-388d-4aec-9262-9ed1d3fa02df";

        //取现
        public const string 取现 = "2bdceded-c43e-4576-b1aa-612502086a41";   

        //卡密
        public const string 卡密 = "d51546c6-66a6-42f7-bdb8-2ab5b9bb770a";   
    }   
}