// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Collections
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortDirection
    {
        [EnumMember(Value = "descending")]
        Descending,

        [EnumMember(Value = "ascending")]
        Ascending
    }
}