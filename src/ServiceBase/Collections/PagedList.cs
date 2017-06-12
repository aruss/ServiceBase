using System.Collections.Generic;

namespace ServiceBase.Collections
{
    public class PagedList<TItem> : IPagedList
    {
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<SortInfo> Sort { get; set; }
        public IEnumerable<TItem> Items { get; set; }
    }
}