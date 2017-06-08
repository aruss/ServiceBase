using System.Collections.Generic;

namespace ServiceBase.Api
{
    public interface IPagedList
    {
        int Total { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
        List<SortInfo> Sort { get; set; }
    }
}