// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project
// root for license information.

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
            IEnumerable<Resource> query =
                this.GetAllAsync(culture, group).Result
                    .Where(c =>
                        key.Equals(c.Key,
                            StringComparison.InvariantCultureIgnoreCase));

            Resource result = query
                .OrderBy(c => c.CreatedAt)
                .FirstOrDefault();

            this._logger.LogDebug(
                "Found resource in database {0}, {1} {2}",
                culture, key, result != null);

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

            this._logger.LogDebug(() =>
            {
                return String.Format(
                    "Found {0} resources in resource store",
                    result.Count()
                );
            });

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

            this._logger.LogDebug(() =>
            {
                return String.Format(
                    "Found {0} cultures in resource store",
                    result.Count()
                );
            });

            return Task.FromResult(result);
        }

        public Task WriteAsync(
            string culture,
            string group,
            IDictionary<string, string> dictionary,
            string source = null)
        {
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                this.WriteAsync(culture, group, item.Key, item.Value, source);
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
            DateTime now = DateTime.UtcNow;

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

            this._logger.LogDebug(() =>
            {
                return String.Format("Adding resource to resource store: {0}",
                    Newtonsoft.Json.JsonConvert.SerializeObject(
                        resource,
                        Newtonsoft.Json.Formatting.None)
                );
            });

            try
            {
                InMemoryResourceStore._resources.TryAdd(resource);
            }
            catch (InvalidOperationException ex)
            {
                // In case dupplicate key is added just log it instead of
                // exiting the app 
                this._logger.LogError(ex.Message);
            }

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
