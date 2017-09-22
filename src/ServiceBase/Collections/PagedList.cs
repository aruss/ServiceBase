namespace ServiceBase.Collections
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class PagedList<TItem> : IPagedList
    {
        /// <summary>
        /// Initializes a new instance of the <ee cref="PagedList{TItem}"/>
        /// class.
        /// </summary>
        public PagedList()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <ee cref="PagedList{TItem}"/>
        /// class.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="total"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sort"></param>
        public PagedList(
           IEnumerable<TItem> items,
           int total,
           int skip = 0,
           int? take = null,
           IEnumerable<SortInfo> sort = null)
        {
            this.Items = items;
            this.Total = total;
            this.Skip = skip;

            this.Take = take.HasValue ? take.Value : items.Count();
            this.Sort = sort; 
        }
        
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<SortInfo> Sort { get; set; }
        public IEnumerable<TItem> Items { get; set; }
    }
}