// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Events.RabbitMQ
{
    public interface IBinarySerializer
    {
        byte[] Serialize(object obj);

        TObject Deserialize<TObject>(byte[] bytes);
    }
}