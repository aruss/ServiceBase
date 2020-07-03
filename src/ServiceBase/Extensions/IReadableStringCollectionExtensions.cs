﻿namespace ServiceBase.Extensions
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Extensions.Primitives;

    public static class IReadableStringCollectionExtensions
    {
        [DebuggerStepThrough]
        public static NameValueCollection AsNameValueCollection(
            this IEnumerable<KeyValuePair<string, StringValues>> collection)
        {
            var nv = new NameValueCollection();

            foreach (var field in collection)
            {
                nv.Add(field.Key, field.Value.First());
            }

            return nv;
        }

        [DebuggerStepThrough]
        public static NameValueCollection AsNameValueCollection(
            this IDictionary<string, StringValues> collection)
        {
            var nv = new NameValueCollection();

            foreach (var field in collection)
            {
                nv.Add(field.Key, field.Value.First());
            }

            return nv;
        }
    }
}
