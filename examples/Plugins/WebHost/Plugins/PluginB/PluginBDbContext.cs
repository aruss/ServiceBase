namespace PluginB
{
    using Microsoft.EntityFrameworkCore;

    public class PluginBDbContext : DbContext
    {
        public PluginBDbContext(DbContextOptions<PluginBDbContext> options)
            : base(options)
        {
        }

        public DbSet<FooEntity> Foos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FooEntity>().ToTable("Foos");
        }
    }
}
