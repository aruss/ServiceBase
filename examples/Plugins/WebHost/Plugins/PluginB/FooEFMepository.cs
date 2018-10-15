namespace PluginB
{
    using System.Collections.Generic;
    using System.Linq;
    using Shared;

    public class FooEFMepository : IFooRepository
    {
        private readonly PluginBDbContext _dbContext;

        public FooEFMepository(PluginBDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IEnumerable<Foo> Get()
        {
            return this._dbContext.Foos
                .Select(s => new Foo
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToList();
        }
    }
}
