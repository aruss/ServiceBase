// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Extensions
{
    /// <summary>
    /// Represents different modes for batching.
    /// </summary>
    public enum BatchMode
    {
        /// <summary>
        /// Batches by creating N equal sized batches.
        /// </summary>
        ByBatchCount,

        /// <summary>
        /// Batches by creating an undefined amount of batches where all
        /// contain max N elements.
        /// </summary>
        ByElementCount
    }
}
