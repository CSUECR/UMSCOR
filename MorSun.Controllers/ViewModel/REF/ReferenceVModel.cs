using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using MorSun.Bll;

namespace MorSun.Controllers.ViewModel
{
    public class ReferenceVModel : BaseVModel<wmfReference>
    {
        /// <summary>
        /// 被选中的编号
        /// </summary>
        //public virtual string CheckedId { get; set; }

        public virtual Guid? CheckedGId { get; set; }

        public override IQueryable<wmfReference> List
        {
            get
            {
                var l = All;
                if (RefGroupId != null)
                {
                    if (!Guid.Equals(RefGroupId, null))
                    {
                        l = l.Where(p => p.RefGroupId == RefGroupId);
                    }
                    if (ItemValue != null)
                    {
                        l = l.Where(p => p.ItemInfo == ItemValue);
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
                }
                else if (RefGroupId == null && FlagTrashed == "1")
                {
                    l = l.Where(p => p.FlagTrashed == true);
                }
                else
                    l = l.Take(0);
                return l.OrderBy(p => p.Sort);
            }
        }
        ///// <summary>
        ///// 获取被选中的类型组
        ///// </summary>
        //public virtual wmfReference Reference
        //{
        //    get
        //    {
        //        var refGroup = All.FirstOrDefault(r => r.ID == CheckedId);
        //        return refGroup ?? First;
        //    }
        //}

        BaseBll<wmfRefGroup> refGroupBll;

        public virtual BaseBll<wmfRefGroup> RefGroupBll
        {
            get
            {
                refGroupBll = refGroupBll.Load();
                return refGroupBll;
            }
            set
            {
                refGroupBll = value;
            }
        }

        public virtual IQueryable<wmfRefGroup> RefList
        {
            get
            {
                return RefGroupBll.All.OrderBy(p => p.RegTime);
            }
        }

        public virtual Guid? RefGroupId { get; set; }

        public virtual string ItemValue { get; set; }

        /// <summary>
        /// 通过id获取其itemValue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetReferenceValue(string id)
        {
            var resultValue = string.Empty;
            var guid = Guid.Empty;
            if (Guid.TryParse(id, out guid))
            {
                resultValue = GetReferenceValue(guid);
            }
            return resultValue;

        }
        /// <summary>
        /// 通过id数组获取其ItemValue
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="splitChar">格式化输出的字符</param>
        /// <returns></returns>
        public string GetReferenceValue(string[] ids, char splitChar = ',')
        {
            var resultBuilder = new StringBuilder();
            foreach (var id in ids)
            {
                var itemValue = GetReferenceValue(id);
                if (!string.IsNullOrEmpty(itemValue))
                {
                    resultBuilder.Append(itemValue);
                    resultBuilder.Append(splitChar);
                }
            }
            return resultBuilder.ToString().TrimEnd(splitChar);
        }
        /// <summary>
        /// 通过id获取其itemValue ，效率没有item.wmfreference来的高
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetReferenceValue(Guid? guid)
        {
            var resultValue = string.Empty;
            if (guid != null && guid != Guid.Empty)
            {
                var referenceModel = this.Dao.GetModel(guid);
                if (referenceModel != null)
                {
                    resultValue = referenceModel.ItemValue;
                }
            }
            return resultValue;
        }
        /// <summary>
        ///  通过refGroupId获取Reference集合
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IQueryable<wmfReference> GetReferencesByRefGroupId(string groupId)
        {
            var groupGuid = Guid.Empty;
            if (Guid.TryParse(groupId, out groupGuid))
            {
                this.RefGroupId = groupGuid;
                return List;
            }
            throw new ArgumentException("groupId无效");
        }

        /// <summary>
        /// 通过itemValue取iteminfo主要是取配置用的。如户表产值系数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetReferenceValueByName(string ItemValue)
        {            
            var resultValue = string.Empty;
            if (!String.IsNullOrEmpty(ItemValue))
            {
                ItemValue = ItemValue.Trim();
                var referenceModel = this.Dao.All.Where(p => p.ItemValue == ItemValue).FirstOrDefault();
                if (referenceModel != null)
                {
                    resultValue = referenceModel.ItemInfo;
                }
            }
            return resultValue;
        }
    }
}
