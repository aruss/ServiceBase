using System.Collections.Generic;

namespace ServiceBase.Collections
{
    public interface IPagedList
    {
        int Total { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
        IEnumerable<SortInfo> Sort { get; set; }
    }
}