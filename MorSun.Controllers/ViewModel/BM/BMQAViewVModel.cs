using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;
using MorSun.Common.类别;
using MorSun.Common.配置;
using HOHO18.Common;
using HOHO18.Common.DEncrypt;

namespace MorSun.Controllers.ViewModel
{
    public class BMQAViewVModel:BaseVModel<bmQAView>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        //public virtual Guid? CheckedId { get; set; }

        /// <summary>
        /// 获取跟目录
        /// </summary>
        public virtual IQueryable<bmQAView> Roots
        {
            get
            {
                var l = base.All;

                if (FlagTrashed == "0")
                {//回收站不能只取根节点
                    l = l.Where(p => p.ParentId == Guid.Empty || p.ParentId == null);
                }
                if (String.IsNullOrEmpty(FlagTrashed) || (!FlagTrashed.Eql("0") && !FlagTrashed.Eql("1")))
                    FlagTrashed = "0";
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }

                return l.OrderBy(p => p.RegTime);
            }
        }

        public virtual IQueryable<bmQAView> Others
        {
            get
            {
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                var l = base.All.Where(p => p.ParentId == sId && p.QARef != refAId && p.QARef != refBSId);
                return l.OrderBy(p => p.RegTime);
            }
        }

        /// <summary>
        /// 答案
        /// </summary>
        public virtual bmQAView A
        {
            get
            {
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                return base.All.FirstOrDefault(p => p.ParentId == sId && (p.QARef == refAId || p.QARef == refBSId));
            }
        }


        /// <summary>
        /// 问题
        /// </summary>
        public virtual bmQAView Q
        {
            get
            {
                return base.All.FirstOrDefault(p => p.ID == sId);
            }
        }

        /// <summary>
        /// 所有的追问
        /// </summary>
        public virtual IQueryable<bmQAView> ChirldQS
        {
            get
            {
                var refAId = Guid.Parse(Reference.问答类别_问题);
                return base.All.Where(p => p.ParentId == sId && p.QARef == refAId).OrderBy(p => p.RegTime);
            }
        }
        /// <summary>
        /// 所有追问的答案
        /// </summary>
        public virtual IQueryable<bmQAView> ChrildAS
        {
            get
            {
                var refAId = Guid.Parse(Reference.问答类别_答案);
                var refBSId = Guid.Parse(Reference.问答类别_不是问题);
                var qids = ChirldQS.Select(p => p.ID);
                return base.All.Where(p => p.ParentId != null && qids.Contains(p.ParentId.Value) && (p.QARef == refAId || p.QARef == refBSId));
            }
        }

        public virtual IQueryable<bmOBView> Objecs
        {
            get
            {
                return new BaseBll<bmOBView>().All.Where(p => p.QAId == sId);
            }
        }

        public override IQueryable<bmQAView> List
        {
            get
            {
                var l = All;
                if (String.IsNullOrEmpty(FlagTrashed))
                    FlagTrashed = "0";
                if (FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                if (FlagTrashed == "0")
                {
                    l = l.Where(p => p.FlagTrashed == false);
                }
                if(sIsSort != null && sIsSort.Value == true)
                {
                    if (sId != null)
                        l = l.Where(p => p.ParentId == sId);
                    else
                        l = l.Where(p => p.ParentId == null);
                }
                return l.OrderBy(p => p.RegTime);
            }
        }
        

        public Guid? sId { get; set; }

        /// <summary>
        /// 微信接口需要原始路径
        /// </summary>
        public Guid? urlId { get; set; }

        public bool? sIsSort { get; set; }

        /// <summary>
        /// 微信APP
        /// </summary>
        public string WeiXinAPP { get { return CFG.邦马网_应用ID; } }
        
        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { get { return Convert.ToString(ChangeDateTime.ConvertDateTimeInt(DateTime.Now)); } }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string NonceStr { get { return TxtHelp.CreateNonceStr(); } }

        /// <summary>
        /// 当前URL
        /// </summary>
        public string ThisUrl { get { return CFG.网站域名 + CFG.问题查看路径 + "/" + urlId.ToString(); } }

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { 
            get {
                // 这里参数的顺序要按照 key 值 ASCII 码升序排序  
                string rawstring = "jsapi_ticket=" + new BasisController().GetWXTICCache() + "&noncestr=" + NonceStr + "&timestamp=" + TimeStamp + "&url=" + ThisUrl + "";
                return HashEncode.SHA1_Hash(rawstring);  
                } 
        }        
    }
}
