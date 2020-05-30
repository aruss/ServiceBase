// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc
{
    using System.Collections.Generic;

    public class ModelStateTransferValue
    {
        public ModelStateTransferValue()
        {
            this.ErrorMessages = new List<string>();
        }

        public string Key { get; set; }
        public string AttemptedValue { get; set; }
        public object RawValue { get; set; }
        public ICollection<string> ErrorMessages { get; set; }
    }
}
