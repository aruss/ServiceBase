namespace PluginA
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Shared;

    public class FooMemoryMepository : IFooRepository
    {
        public static ConcurrentDictionary<Guid, Foo> Foos =
            new ConcurrentDictionary<Guid, Foo>();

        public IEnumerable<Foo> Get()
        {
            return FooMemoryMepository.Foos.Values.ToArray();
        }
    }
}
