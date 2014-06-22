using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace MorSun.Common.Privelege
{
    /// <summary>
    /// 操作权限（用于当前用户能够操作范围）
    /// </summary>
    public class OperatePrivilege
    {
        public bool CanRead { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanFlagDelete { get; set; }
        public bool CanDelete { get; set; }
        //安排
        public bool CanArrange { get; set; }
        //撤销
        public bool CanRevoke { get; set; }
        public bool CanConfirm { get; set; }

        public bool CanRecycle { get; set; }
        public bool CanOrder { get; set; }
        //生成
        public bool CanGenerate { get; set; }
        public bool CanImport { get; set; }
        //解锁
        //public bool CanUnLock { get; set; }
        //配置
        public bool CanConfigure { get; set; }
        /// <summary>
        /// 审核 add by timfeng 2013-8-12
        /// </summary>
        public bool CanAudit { get; set; }
        /// <summary>
        ///反馈
        /// </summary>
        public bool CanFeedback { get; set; }
        public bool CanMaintain { get; set; }

        /// <summary>
        /// 打印的权限判定
        /// </summary>
        [DefaultValue(false)]
        public bool CanPrint { get; set; }
        
    }
    /// <summary>
    /// 生成于2011年05月05日
    /// </summary>
    public struct 操作
    {

        public const string 添加 = "332B5605-335A-4308-A755-23C348CBCC5B";
        public const string 修改 = "E9303F56-C2E7-4636-A42A-4973FEE38B1E";
        public const string 删除 = "4A5BEF1A-D2C0-459D-8966-B20446FD5A5A";
        public const string 查看 = "1A5292A5-A119-49B8-A209-A9C14C5D0B3A";
        public const string 回收站 = "78BCE0EE-1DC0-4CD1-8400-4CF6C75220DC";
        public const string 排序 = "3369D2F7-59A8-425E-A988-8D83BC1122FE";
        public const string 彻底删除 = "B9409DE8-4804-492F-9476-E0D1092091FC";
        public const string 生成 = "62e02dd9-9055-4247-b8ed-035d1de4235b";
        //public const string 解锁 = "9A4494DF-FB6D-4D7F-8EAA-56881C39D4C6";
        public const string 审核 = "cd46db87-f15c-4533-a137-6c4109d1978d";
        public const string 配置 = "87ABA619-6AE7-4C66-B32E-B3513310DBA2";
        public const string 清除 = "f25fcee7-3af7-4aa3-b10f-612ebcba48f8";
        public const string 导入 = "5704a73f-c4dc-45c8-ba5f-c619b5089825";
        public const string 安排 = "74dd693f-4959-4ce5-be43-439a6bdf4a17";
        public const string 确认 = "be4c2279-0698-4bb4-93d5-51e646ed2388";
        public const string 测试 = "74dd693f-4959-4ce5-be43-439a6bdf4a17";
        public const string 导出 = "be4c2279-0698-4bb4-93d5-51e646ed2388";
        public const string 反馈 = "6e58cc96-7dd5-4399-8797-bb4b62b74ab6";
        public const string 维修 = "4cac97fa-f122-4ee5-afbc-a40048f2f356";

        public const string 打印 = "a220f670-d426-493e-89b0-89b3e6d309da";

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
        public const string 部门 = "401D4647-9AE4-4AAA-B126-D855D12618EC";
        /// <summary>
        /// 类别组
        /// </summary>
        public const string 类别组 = "D968629F-6E34-4CE0-8C2C-6083E6805665";
        /// <summary>
        /// 类别
        /// </summary>
        public const string 类别 = "FD1F62EA-1BD2-46D0-8A83-AE1947B60893";
        /// <summary>
        /// 操作
        /// </summary>
        public const string 操作 = "F49A80D1-6989-4182-AECC-9D5D3CCDFF55";
        /// <summary>
        /// 岗位
        /// </summary>
        public const string 岗位 = "B7B59163-71C2-429E-8E86-D0ABC9576262";
        /// <summary>
        /// 权限
        /// </summary>
        public const string 权限 = "BD3BC770-2437-4105-9E93-11EABBB3BE75";
        /// <summary>
        /// 日志
        /// </summary>
        public const string 日志 = "F1439F13-9FF4-4CE6-8759-9FAAF3A891F6";
        /// <summary>
        /// 资源管理
        /// </summary>
        public const string 资源管理 = "048F512F-C5D6-47D3-9586-1E1F5131B757";        
        /// <summary>
        /// 系统参数配置
        /// </summary>
        public const string 系统参数配置 = "BA61DF81-F4F0-48D6-9F8B-F8C6B57EBDB7";
        /// <summary>
        /// 员工
        /// </summary>
        public const string 员工 = "76309187-C594-467E-910B-78D5A6D79403";
        /// <summary>
        /// 角色配置
        /// </summary>
        public const string 角色配置 = "AF927396-8D91-4563-A733-B1AFBD84E4A1";
        /// <summary>
        /// 个人资料
        /// </summary>
        public const string 个人资料 = "5BA61F34-0E42-4188-8D48-4E9D7D665F18";
        
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
        public const string 左侧菜单 = "340aaeed-7d57-4a63-ae4d-6a4bcee8ebea";
        
        
        /// <summary>
        /// 数据库备份
        /// </summary>
        public const string 数据库备份 = "D838F110-E6FA-4D2B-9FCB-018312A24AC8";
        ///// <summary>
        ///// 岗位级别
        ///// </summary>
        //public const string 岗位级别 = "37019C92-2411-4CDA-A055-0C77C863D898";


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
    }   
}