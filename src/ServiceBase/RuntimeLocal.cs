// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase
{
    using System.Threading;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Local data wraps <see cref="RequestLocal{T}"/> and
    /// <see cref="ThreadLocal{T}"/> and makes it easier to use them both.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class RuntimeLocal<TValue>
    {
        /// <summary>
        /// Saves the data if we're in a web runtime.
        /// </summary>
        private RequestLocal<TValue> _requestLocal;

        /// <summary>
        /// Saves the data if we'rnt in a web runtime.
        /// </summary>
        private ThreadLocal<TValue> _threadLocal;

        public RuntimeLocal(IHttpContextAccessor httpContextAccessor)
        {
            this._requestLocal = new RequestLocal<TValue>(httpContextAccessor);
        }

        public RuntimeLocal()
        {
            this._threadLocal = new ThreadLocal<TValue>();
        }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public TValue Value
        {
            get
            {
                if (this._requestLocal == null)
                {
                    return this._threadLocal.IsValueCreated
                            ? this._threadLocal.Value : default(TValue);
                }
                else
                {
                    return this._requestLocal.IsValueCreated
                           ? this._requestLocal.Value : default(TValue);
                }
            }
            set
            {
                if (this._requestLocal == null)
                {
                    this._threadLocal.Value = value;
                }
                else
                {
                    this._requestLocal.Value = value;
                }
            }
        }
    }
}