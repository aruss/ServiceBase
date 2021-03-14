// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.Extensions.Logging
{
    using System;
    using System.Diagnostics;
    using ServiceBase.Logging;

    /// <summary>
    /// <see cref="ILogger"/> extension methods.
    /// </summary>
    public static partial class ILoggerExtensions
    {
        [DebuggerStepThrough]
        public static void LogCritical(this ILogger logger, object obj)
        {
            logger.LogCritical(LogSerializer.Serialize(obj));
        }

        [DebuggerStepThrough]
        public static void LogDebug(this ILogger logger, object obj)
        {
            logger.LogDebug(LogSerializer.Serialize(obj));
        }

        [DebuggerStepThrough]
        public static void LogError(this ILogger logger, object obj)
        {
            logger.LogError(LogSerializer.Serialize(obj));
        }

        [DebuggerStepThrough]
        public static void LogInformation(this ILogger logger, object obj)
        {
            logger.LogInformation(LogSerializer.Serialize(obj));
        }

        [DebuggerStepThrough]
        public static void LogTrace(this ILogger logger, object obj)
        {
            logger.LogTrace(LogSerializer.Serialize(obj));
        }

        [DebuggerStepThrough]
        public static void LogWarning(this ILogger logger, object obj)
        {
            logger.LogWarning(LogSerializer.Serialize(obj));
        }

        [DebuggerStepThrough]
        public static void LogCritical(this ILogger logger, Func<object> func)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                logger.LogCritical(func()); 
            }
        }
        
        [DebuggerStepThrough]
        public static void LogDebug(this ILogger logger, Func<object> func)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogCritical(func());
            }
        }

        [DebuggerStepThrough]
        public static void LogError(this ILogger logger, Func<object> func)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogCritical(func());
            }
        }

        [DebuggerStepThrough]
        public static void LogInformation(this ILogger logger, Func<object> func)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(func());
            }
        }

        [DebuggerStepThrough]
        public static void LogTrace(this ILogger logger, Func<object> func)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace(func());
            }
        }

        [DebuggerStepThrough]
        public static void LogWarning(this ILogger logger, Func<object> func)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning(func());
            }
        }
    }
}