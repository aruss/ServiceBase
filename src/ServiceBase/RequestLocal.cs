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
        private static long id;

        #endregion Private Static Fields

        #region Private Fields

        /// <summary>
        /// Saves the key used for caching in the http context.
        /// </summary>
        private string key;

        private readonly IHttpContextAccessor httpContextAccessor;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLocal{T}" />
        /// class.
        /// </summary>
        public RequestLocal(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;

            this.key = string.Format(
                "requestlocal_{0}_-_{1}",
                typeof(TValue).FullName,
                Interlocked.Increment(ref RequestLocal<TValue>.id));
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the request scoped value.
        /// </summary>
        public TValue Value
        {
            get
            {
                return (TValue)this.httpContextAccessor
                    .HttpContext.Items[this.key];
            }

            set
            {
                this.httpContextAccessor
                    .HttpContext.Items[this.key] = value;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the value was already created.
        /// </summary>
        public bool IsValueCreated
        {
            get
            {
                return this.httpContextAccessor
                    .HttpContext.Items.ContainsKey(this._key);
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Releases the current instance and removes the value out of the
        /// http context.
        /// </summary>
        public void Dispose()
        {
            this.httpContextAccessor
                    .HttpContext.Items.Remove(this.key);
        }

        #endregion Public Methods
    }
}