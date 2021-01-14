// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Events
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using System.Linq;

    /// <summary>
    /// Default implementation of the event service.
    /// Write events raised to the log.
    /// </summary>
    public class DefaultEventService : IEventService
    {
        /// <summary>
        /// The options
        /// </summary>
        private readonly EventOptions _eventOptions;

        /// <summary>
        /// The <see cref="IHttpContextAccessor"/>
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// The <see cref="IDateTimeAccessor"/>
        /// </summary>
        private readonly IDateTimeAccessor _dateTimeAccessor;

        /// <summary>
        /// The sink
        /// </summary>
        private readonly IEnumerable<IEventSink> _eventSinks;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DefaultEventService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="context">The context.</param>
        /// <param name="sink">The sink.</param>
        public DefaultEventService(
            EventOptions eventOptions,
            IHttpContextAccessor httpContextAccessor,
            IDateTimeAccessor dateTimeAccessor,
            IEnumerable<IEventSink> eventSinks)
        {
            this._eventOptions = eventOptions ??
                throw new ArgumentNullException(nameof(eventOptions));

            this._httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            this._dateTimeAccessor = dateTimeAccessor ??
                throw new ArgumentNullException(nameof(dateTimeAccessor));

            this._eventSinks = eventSinks ??
                throw new ArgumentNullException(nameof(eventSinks)); 

            if (!this._eventSinks.Any())
            {
                throw new ApplicationException("Requres at least one event sink");
            }
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public async Task RaiseAsync(Event evt)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            if (this.CanRaiseEvent(evt))
            {
                await this.PrepareEventAsync(evt);

                // TODO: run it parallel 
                foreach (var sink in this._eventSinks)
                {
                    await sink.PersistAsync(evt);
                }
            }
        }

        /// <summary>
        /// Indicates if the type of event will be persisted.
        /// </summary>
        /// <param name="evtType"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public bool CanRaiseEventType(EventTypes evtType)
        {
            switch (evtType)
            {
                case EventTypes.Failure:
                    return this._eventOptions.RaiseFailureEvents;

                case EventTypes.Information:
                    return this._eventOptions.RaiseInformationEvents;

                case EventTypes.Success:
                    return this._eventOptions.RaiseSuccessEvents;

                case EventTypes.Error:
                    return this._eventOptions.RaiseErrorEvents;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Determines whether this event would be persisted.
        /// </summary>
        /// <param name="evt">The evt.</param>
        /// <returns>
        ///   <c>true</c> if this event would be persisted; otherwise,
        ///   <c>false</c>.
        /// </returns>
        protected virtual bool CanRaiseEvent(Event evt)
        {
            return this.CanRaiseEventType(evt.EventType);
        }

        /// <summary>
        /// Prepares the event.
        /// </summary>
        /// <param name="evt">The evt.</param>
        /// <returns></returns>
        protected virtual async Task PrepareEventAsync(Event evt)
        {
            HttpContext httpContext = this._httpContextAccessor.HttpContext;

            evt.ActivityId = httpContext.TraceIdentifier;
            evt.TimeStamp = this._dateTimeAccessor.UtcNow;
            evt.ProcessId = Environment.ProcessId;

            if (httpContext.Connection.LocalIpAddress != null)
            {
                evt.LocalIpAddress = httpContext.Connection
                    .LocalIpAddress.ToString() + ":" +
                    httpContext.Connection.LocalPort;
            }
            else
            {
                evt.LocalIpAddress = "unknown";
            }

            if (httpContext.Connection.RemoteIpAddress != null)
            {
                evt.RemoteIpAddress = httpContext.Connection
                    .RemoteIpAddress.ToString();
            }
            else
            {
                evt.RemoteIpAddress = "unknown";
            }

            await evt.PrepareAsync();
        }
    }
}