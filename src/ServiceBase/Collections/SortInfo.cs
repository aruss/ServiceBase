// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Collections
{
    public class SortInfo
    {
        public string Field { get; set; }
        public SortDirection Direction { get; set; }

        public SortInfo()
        {
        }

        public SortInfo(string field, SortDirection direction)
        {
            this.Field = field;
            this.Direction = direction;
        }
    }
}