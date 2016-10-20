// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ServiceBase.Events
{
    /// <summary>
    /// Default implementation of the event service. Write events raised to the log.
    /// </summary>
    public class DefaultEventService : IEventService
    {
        private readonly EventServiceHelper _helper;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        private readonly EventOptions _options;

        public DefaultEventService(ILogger<DefaultEventService> logger, IOptions<EventOptions> options, IHttpContextAccessor context)
        {
            _logger = logger;
            _options = options.Value;
            _helper = new EventServiceHelper(_options, context);
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public virtual Task RaiseAsync<T>(Event<T> evt)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));

            if (_helper.CanRaiseEvent(evt))
            {
                _logger.LogInformation(_helper.PrepareEvent(evt));
            }

            return Task.FromResult(0);
        }
    }
}