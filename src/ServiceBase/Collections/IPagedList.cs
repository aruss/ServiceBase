// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Collections
{
    using System.Collections.Generic;

    public interface IPagedList
    {
        int Total { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
        IEnumerable<SortInfo> Sort { get; set; }
    }
}