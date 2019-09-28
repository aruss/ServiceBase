namespace ServiceBase.Exceptions
{
    using System;

    [Serializable]
    public class CyclicReferenceException : Exception
    {
        #region Public Properties

        public  object Source
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public CyclicReferenceException()
        {
        }

        public CyclicReferenceException(object source)
            : this($"Cyclic reference in object `{ source }`")
        {
            this.Source = source;
        }

        public CyclicReferenceException(string message) : base(message)
        {
        }

        public CyclicReferenceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected CyclicReferenceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
