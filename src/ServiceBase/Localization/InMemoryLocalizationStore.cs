// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LocaleDictionary = System.Collections.Concurrent
        .ConcurrentDictionary<string, string>;

    using UberLocaleDictionary = System.Collections.Concurrent
        .ConcurrentDictionary<string, System.Collections.Concurrent
            .ConcurrentDictionary<string, string>>;

    public class InMemoryLocalizationStore : ILocalizationStore
    {
        private static UberLocaleDictionary _dictionaries
            = new UberLocaleDictionary();

        public Task<string> GetAsync(string culture, string name)
        {
            LocaleDictionary dict = InMemoryLocalizationStore
                .GetCultureDictionary(culture);

            if (dict.ContainsKey(name))
            {
                return Task.FromResult(dict[name]);
            }

            return Task.FromResult<string>(null);
        }

        public Task<IEnumerable<string>> GetAllCultures()
        {
            IEnumerable<string> keys = _dictionaries.Keys;
            return Task.FromResult(keys);
        }

        private static LocaleDictionary GetCultureDictionary(
            string culture)
        {
            return InMemoryLocalizationStore
                ._dictionaries.GetOrAdd(culture, (c) =>
                {
                    return new LocaleDictionary();
                });
        }

        public Task WriteAsync(string culture, string key, string value)
        {
            LocaleDictionary dict = InMemoryLocalizationStore
                .GetCultureDictionary(culture);

            dict.TryAdd(key, value);

            return Task.CompletedTask;
        }

        public Task WriteAsync(string culture,
            IDictionary<string, string> dictionary)
        {
            LocaleDictionary dict = InMemoryLocalizationStore
               .GetCultureDictionary(culture);

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                dict.TryAdd(pair.Key, pair.Value);
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(string culture, string key)
        {
            LocaleDictionary dict = InMemoryLocalizationStore
                    .GetCultureDictionary(culture);

            string value = string.Empty;

            dict.TryRemove(key, out value);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(string culture, IEnumerable<string> keys)
        {
            LocaleDictionary dict = InMemoryLocalizationStore
                 .GetCultureDictionary(culture);

            string value = string.Empty;

            foreach (string key in keys)
            {
                dict.TryRemove(key, out value);
            }

            return Task.CompletedTask;
        }

    }
}
