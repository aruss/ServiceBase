// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Events
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Default implementation of the event service.
    /// Write events raised to the log.
    /// </summary>
    public class DefaultEventService : IEventService
    {
        /// <summary>
        /// The options
        /// </summary>
        private readonly EventOptions eventOptions;

        /// <summary>
        /// The <see cref="IHttpContextAccessor"/>
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// The <see cref="IDateTimeAccessor"/>
        /// </summary>
        private readonly IDateTimeAccessor dateTimeAccessor;

        /// <summary>
        /// The sink
        /// </summary>
        private readonly IEventSink eventSink;

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
            IEventSink eventSink)
        {
            this.eventOptions = eventOptions;
            this.httpContextAccessor = httpContextAccessor;
            this.dateTimeAccessor = dateTimeAccessor;
            this.eventSink = eventSink;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public async Task RaiseAsync(Event evt)
        {
            if (evt == null) throw new ArgumentNullException("evt");

            if (this.CanRaiseEvent(evt))
            {
                await this.PrepareEventAsync(evt);
                await this.eventSink.PersistAsync(evt);
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
                    return this.eventOptions.RaiseFailureEvents;

                case EventTypes.Information:
                    return this.eventOptions.RaiseInformationEvents;

                case EventTypes.Success:
                    return this.eventOptions.RaiseSuccessEvents;

                case EventTypes.Error:
                    return this.eventOptions.RaiseErrorEvents;

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
            HttpContext httpContext = this.httpContextAccessor.HttpContext;

            evt.ActivityId = httpContext.TraceIdentifier;
            evt.TimeStamp = this.dateTimeAccessor.UtcNow;
            evt.ProcessId = Process.GetCurrentProcess().Id;

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