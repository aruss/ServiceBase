namespace PluginB
{
    using System;
    using System.Linq;

    public static class PluginBDbContextInitializer
    {
        public static void Initialize(PluginBDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any foos.
            if (context.Foos.Any())
            {
                // DB has been seeded
                return;
            }

            FooEntity[] foos = new FooEntity[]
            {
               new FooEntity{ Id = Guid.NewGuid(), Name="Carson" },
               new FooEntity{ Id = Guid.NewGuid(), Name="Meredith" },
               new FooEntity{ Id = Guid.NewGuid(), Name="Arturo" },
               new FooEntity{ Id = Guid.NewGuid(), Name="Gytis" },
               new FooEntity{ Id = Guid.NewGuid(), Name="Yan" },
            };

            foreach (FooEntity s in foos)
            {
                context.Foos.Add(s);
            }

            context.SaveChanges();
        }
    }
}
