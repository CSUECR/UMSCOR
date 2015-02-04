using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web.Mvc;
using HOHO18.Common;
using System.Reflection;
using HOHO18.Common.Helper;
using System.IO;

namespace MorSun.Model
{
    //[Bind(Include = "ParentId,Category,Virtual,DeptName,Domain,Description,Sort,Province,City,Town,Area,Address,Tel,WYDTLon,WYDTLat,WYDTZoom,WYDTTitle,WYDTContent,WYDTImage,WYDTImgWide,WYDTImgHigh,WYDTImgTopLeftHorizontal,WYDTImgTopLeftVertical")]
    //[Bind]
    public class systemXML
    {
        //每页显示条数
        public string PageSize { get; set; }

        //是否允许注册,true允许注册false关闭注册
        public string Register { get; set; }

        //是否允许通过邮件找回密码,true允许注册false关闭注册
        public string PassWordRecovery { get; set; }

        //头像图片类别ID
        public string UpLoadReference_Image { get; set; }

        //用户头像上传大小限制
        public string UpLoadUserImageSize { get; set; }

        //注册角色名称
        public string RoleName { get; set; }

        //注册岗位名称
        public string PositionName { get; set; }

        //角色权限范围
        public string RolePrivAgran { get; set; }

        ///邮件设置 属性
        //发件人地址
        public string ServiceMail { get; set; }

        //发件人名称
        public string ServiceMailName { get; set; }

        //邮件密码
        public string ServiceMailPassword { get; set; }

        //邮件标题
        public string ServiceMailGetPasswordTitle { get; set; }

        //邮件发送域名
        public string ServiceDoMain { get; set; }

        //服务器文件路径
        public string ServicePath { get; set; }

        /// <summary>
        /// 是否解锁
        /// </summary>
        public string UnlockingFlag { get; set; }
        /// <summary>
        /// 解锁天数
        /// </summary>
        public string UnlockingDay { get; set; }
        /// <summary>
        /// 考勤最大记录数
        /// </summary>
        public string KqRecordCount { get; set; }

        /// <summary>
        /// 是否开启验证码
        /// </summary>
        public string VerificationCode { get; set; }

        /// <summary>
        /// 添加
        /// </summary>
        public string AddModel { get; set; }

        /// <summary>
        /// 批量删除
        /// </summary>
        public string BatchDel { get; set; }

        /// <summary>
        /// 回收站
        /// </summary>
        public string Recycle { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// 批量离职
        /// </summary>
        public string UserLiZhi { get; set; }

        /// <summary>
        /// 离职管理
        /// </summary>
        public string UserLiZhiManage { get; set; }

        /// <summary>
        /// 彻底删除
        /// </summary>
        public string DelItem { get; set; }

        /// <summary>
        /// 批量还原
        /// </summary>
        public string BatchRecovery { get; set; }

        /// <summary>
        /// 更新地区信息
        /// </summary>
        public string UpdateRegion { get; set; }

        /// <summary>
        /// 面板切换记录
        /// </summary>
        public string Panel { get; set; }

        /// <summary>
        /// 欢迎页面显示的文字
        /// </summary>
        public string DeskTop { get; set; }

        /// <summary>
        /// 工作流虚拟目录
        /// </summary>
        public string 工作流虚拟目录 { get; set; }
        /// <summary>
        /// 工作流虚拟目录
        /// </summary>
        public string 智能表单虚拟目录 { get; set; }
        /// <summary>
        /// 工作流虚拟目录
        /// </summary>
        public string 智能报表虚拟目录 { get; set; }

        /// <summary>
        /// 返回各模块管理文字说明，返回
        /// </summary>
        public string Back { get; set; }

        /// <summary>
        /// 是否多点登录
        /// </summary>
        public string MultiLogOn { get; set; }

        /// <summary>
        /// 生成导航菜单
        /// </summary>
        public string GenerationMenuName { get; set; }

        /// <summary>
        /// 默认保存目录
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// 默认允许上传文件扩展名
        /// </summary>
        public string FileExt { get; set; }


        /// <summary>
        /// 出现在上传对话框中的文件类型描述
        /// </summary>
        public string FileDesc { get; set; }

        /// <summary>
        /// 当允许多文件生成时，设置选择文件的个数
        /// </summary>
        public string QueueSizeLimit { get; set; }

        /// <summary>
        /// 是否允许同时上传多文件，默认false
        /// </summary>
        public string Multi { get; set; }

        /// <summary>
        /// 选定文件后是否自动上传，默认false
        /// </summary>
        public string Auto { get; set; }

        /// <summary>
        /// 多文件上传时，同时上传文件数目限制
        /// </summary>
        public string SimUploadLimit { get; set; }

        /// <summary>
        /// 浏览按钮的文本
        /// </summary>
        public string ButtonImg { get; set; }

        /// <summary>
        /// 控制上传文件的大小，单位KB
        /// </summary>
        public string SizeLimit { get; set; }

        /// <summary>
        /// 导航菜单最多显示个数
        /// </summary>
        public string PopupMenu { get; set; }

        /// <summary>
        /// CC工作流虚拟目录
        /// </summary>
        public string Ccflow { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domail { get; set; }

        /// <summary>
        /// 员工考勤备注是否开启，开启=true，关闭=false
        /// </summary>
        public string KQRemark { get; set; }
        /// <summary>
        /// 公司全称
        /// </summary>
        public string CompanyName { get; set; }
    }
}