// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Resources
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InMemoryResourceStore : IResourceStore
    {
        private static BlockingCollection<Resource> _resources
            = new BlockingCollection<Resource>();

        public Task<Resource> GetAsync(
            string culture,
            string group,
            string key)
        {
            IEnumerable<Resource> query = InMemoryResourceStore._resources
                .Where(c =>
                    culture.Equals(c.Culture,
                        StringComparison.InvariantCultureIgnoreCase) &&
                    group.Equals(c.Group,
                        StringComparison.InvariantCultureIgnoreCase) &&
                    key.Equals(c.Key,
                        StringComparison.InvariantCultureIgnoreCase));

            Resource result = query
                .OrderBy(c => c.CreatedAt)
                .FirstOrDefault();

            return Task.FromResult(result);
        }

        public Task<IEnumerable<Resource>> GetAllAsync(
            string culture,
            string group)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetAllCulturesAsync(string group)
        {
            IEnumerable<string> result = InMemoryResourceStore._resources
                .Where(c => group
                    .Equals(
                        c.Group,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
                .Select(s => s.Culture);

            return Task.FromResult(result);
        }

        public Task WriteAsync(
            string culture,
            string group,
            IDictionary<string, string> dictionary,
            string source = null)
        {
            DateTime now = DateTime.Now;

            foreach (KeyValuePair<string, string> item in dictionary)
            {
                Resource resource = new Resource
                {
                    Id = Guid.NewGuid(),
                    Culture = culture,
                    Group = group,
                    Key = item.Key,
                    Value = item.Value,
                    Source = source,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                InMemoryResourceStore._resources.TryAdd(resource);
            }

            return Task.CompletedTask;
        }

        public Task WriteAsync(
            string culture,
            string group,
            string key,
            string value,
            string source = null)
        {
            DateTime now = DateTime.Now;

            Resource resource = new Resource
            {
                Id = Guid.NewGuid(),
                Culture = culture,
                Group = group,
                Key = key,
                Value = value,
                Source = source,
                CreatedAt = now,
                UpdatedAt = now
            };

            InMemoryResourceStore._resources.TryAdd(resource);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(
            string culture,
            string group,
            IEnumerable<string> keys,
            string source = null)
        {
            throw new NotImplementedException();
        }
    }
}
