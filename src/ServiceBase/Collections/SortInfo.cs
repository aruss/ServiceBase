// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Collections
{
    public class SortInfo
    {
        public string Field { get; set; }
        public bool IsAsc { get; set; }

        public SortInfo()
        {
        }

        public SortInfo(string field, bool isAsc)
        {
            this.Field = field;
            this.IsAsc = isAsc;
        }
    }
}
