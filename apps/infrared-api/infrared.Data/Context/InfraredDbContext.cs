using infrared.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace infrared.Data;

public class InfraredDbContext : DbContext
{
    public InfraredDbContext(DbContextOptions<InfraredDbContext> options) : base(options)
    {
    }

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}