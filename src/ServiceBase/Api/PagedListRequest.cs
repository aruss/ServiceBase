using System.Collections.Generic;
using ServiceBase.Collections; 

namespace ServiceBase.Api
{
    public class PagedListRequest
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
        public IEnumerable<SortInfo> Sort { get; set; }
    }
}