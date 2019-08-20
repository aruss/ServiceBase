// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Resources
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class InMemoryResourceStore : IResourceStore
    {
        private static BlockingCollection<Resource> _resources
            = new BlockingCollection<Resource>();

        internal ILogger<InMemoryResourceStore> _logger;

        public InMemoryResourceStore(ILogger<InMemoryResourceStore> logger)
        {
            this._logger = logger; 
        }

        public Task<Resource> GetAsync(
            string culture,
            string group,
            string key)
        {
            IEnumerable<Resource> query = this.GetAllAsync(culture, group).Result
                .Where(c =>
                    key.Equals(c.Key,
                        StringComparison.InvariantCultureIgnoreCase));

            Resource result = query
                .OrderBy(c => c.CreatedAt)
                .FirstOrDefault();

            this._logger.LogDebug(
                "Found resource in database {0}, {1} {2}", culture, key, result != null);

            return Task.FromResult(result);
        }

        public Task<IEnumerable<Resource>> GetAllAsync(
            string culture,
            string group)
        {
            IEnumerable<Resource> result = InMemoryResourceStore._resources
                .Where(c =>
                    culture.Equals(c.Culture,
                        StringComparison.InvariantCultureIgnoreCase) &&
                    group.Equals(c.Group,
                        StringComparison.InvariantCultureIgnoreCase));

            // TODO: Create message only if debug log level is active
            this._logger.LogDebug("Found {0} number of resourxes in database",
                result.Count());

            return Task.FromResult(result);
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
                .Select(s => s.Culture)
                .Distinct();

            // TODO: Create message only if debug log level is active
            this._logger.LogDebug("Found {0} number of cultures in database",
                result.Count());

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

                this._logger.LogDebug("Writing resource to database {0} {1} {2} {3} {4}",
                    culture, group, item.Key, item.Value, source);

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

            this._logger.LogDebug("Writing resource to database {0} {1} {2} {3} {4}",
                    culture, group, key, value, source);

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
