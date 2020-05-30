// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Events.RabbitMQ
{
    using System;

    public class RabbitMqOptions
    {
        public Uri Uri { get; set; }
        public string ExchangeName { get; internal set; } = "logs";
    }
}