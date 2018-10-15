// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocalizationStore
    {
        Task<string> GetAsync(string culture, string name);

        Task<IEnumerable<string>> GetAllCultures();

        Task WriteAsync(string culture, string key, string value);
        Task WriteAsync(string culture, IDictionary<string, string> dictionary);

        Task DeleteAsync(string culture, string key);
        Task DeleteAsync(string culture, IEnumerable<string> keys);
    }
}
