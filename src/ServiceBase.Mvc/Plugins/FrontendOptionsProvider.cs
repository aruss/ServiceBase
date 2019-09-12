// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System;
    using System.Collections.Generic;

    public class FrontendOptionsProvider : IFrontendOptionsProvider
    {
        private readonly IEnumerable<IFrontendOptions> _frontendOptions;

        public FrontendOptionsProvider(IEnumerable<IFrontendOptions> frontendOptions)
        {
            this._frontendOptions = frontendOptions;
        }

        public IDictionary<string, IDictionary<string, object>> GetOptionsAsDictionary()
        {
            var result = new Dictionary<string, IDictionary<string, object>>();

            foreach (var item in this._frontendOptions)
            {
                Type type = item.GetType();
                string name = type.Name.Replace("FrontendOptions", string.Empty);
                var valueDict = new Dictionary<string, object>();

                foreach (var propertyInfo in type.GetProperties())
                {
                    valueDict.Add(propertyInfo.Name, propertyInfo.GetValue(item));
                }

                result.Add(name, valueDict);
            }

            return result;
        }
    }
}
