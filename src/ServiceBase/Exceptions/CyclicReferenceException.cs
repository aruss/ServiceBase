// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Exceptions
{
    using System;

    [Serializable]
    public class CyclicReferenceException : Exception
    {
        #region Public Properties

#pragma warning disable CS0114
        public object Source
        {
            get;
            set;
        }
#pragma warning restore CS0114

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
