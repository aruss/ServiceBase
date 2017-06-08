using System.Collections.Generic;

namespace ServiceBase.Api
{
    public class PagedListRequest
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
        public List<SortInfo> Sort { get; set; }
    }
}