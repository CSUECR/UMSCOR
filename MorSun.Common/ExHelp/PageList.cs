using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;

namespace System
{
    public class PageList<T> : List<T>
        where T : class
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public T model { get; set; }

        public PageList(IEnumerable<T> source, int index, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageSize = PageSize;
            this.PageIndex = index;
            base.AddRange(source.Select(u => new { Sort = 1, TObject = u }).OrderBy(u => u.Sort).Select(u => u.TObject).Skip<T>(((index - 1) * pageSize)).Take<T>(pageSize).ToList<T>());
            this.TotalPages = ((this.TotalCount - 1) / pageSize) + 1;
        }
        /// <summary>
        /// 需要排序完成后的集合
        /// </summary>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        public PageList(IOrderedQueryable<T> source, int index, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageSize = PageSize;
            this.PageIndex = index;
            //base.AddRange(source.Select(u => new { Sort = 1, TObject = u }).OrderBy(u => u.Sort).Select(u => u.TObject).Skip<T>(((index - 1) * pageSize)).Take<T>(pageSize).ToList<T>());
            base.AddRange(source.Skip<T>(((index - 1) * pageSize)).Take<T>(pageSize).ToList<T>());
            this.TotalPages = ((this.TotalCount - 1) / pageSize) + 1;
        }

        public PageList(ObjectQuery<T> source, int index, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageSize = PageSize;
            this.PageIndex = index;
            base.AddRange(source.Select(u => new { Sort = 1, TObject = u }).OrderBy(u => u.Sort).Select(u => u.TObject).Skip<T>(((index - 1) * pageSize)).Take<T>(pageSize).ToList<T>());
            this.TotalPages = ((this.TotalCount - 1) / pageSize) + 1;
        }
    }
    /// <summary>
    /// list的帮助函数
    /// </summary>
    public static class ListHelper
    {
        public static PageList<T> ToPageList<T>(this IEnumerable<T> source, int? pageIndex, int pageSize)
            where T : class
        {
            int index = Convert.ToInt32(pageIndex);

            if (index < 1)
            {
                index = 1;
            }

            if (source is IOrderedQueryable<T>)
                return new PageList<T>(source as IOrderedQueryable<T> , index, pageSize);

            if (source is IQueryable<T>)
                return new PageList<T>(source as ObjectQuery<T>, index, pageSize);

            if (source is ObjectQuery<T>)
                return new PageList<T>(source as ObjectQuery<T>, index, pageSize);


            return new PageList<T>(source, index, pageSize);
        }
    }


}