namespace ServiceBase
{
    using System;
    using System.Threading;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Represents a wrapper like <see cref="ThreadLocal{T}" /> but instead
    /// being limited to threads its scope is the current http request.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value.
    /// </typeparam>
    public class RequestLocal<TValue> : IDisposable
    {
        #region Private Static Fields

        /// <summary>
        /// Saves the internal id of the last requestlocal instance.
        /// </summary>
        private static long _id;

        #endregion

        #region Private Fields

        /// <summary>
        /// Saves the key used for caching in the http context.
        /// </summary>
        private string _key;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLocal{T}" />
        /// class.
        /// </summary>
        public RequestLocal(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;

            this._key = string.Format(
                "requestlocal_{0}_-_{1}",
                typeof(TValue).FullName,
                Interlocked.Increment(ref RequestLocal<TValue>._id));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the request scoped value.
        /// </summary>
        public TValue Value
        {
            get
            {
                return (TValue)this._httpContextAccessor
                    .HttpContext.Items[this._key];
            }

            set
            {
                this._httpContextAccessor
                    .HttpContext.Items[this._key] = value;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the value was already created.
        /// </summary>
        public bool IsValueCreated
        {
            get
            {
                return this._httpContextAccessor
                    .HttpContext.Items.ContainsKey(this._key);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Releases the current instance and removes the value out of the
        /// http context.
        /// </summary>
        public void Dispose()
        {
            this._httpContextAccessor
                    .HttpContext.Items.Remove(this._key);
        }

        #endregion
    }
}


