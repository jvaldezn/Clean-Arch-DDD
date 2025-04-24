using CleanArch.Domain.Products;
using CleanArch.Domain.Users;
using CleanArch.Infrastructure.EntityType;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Product { get; set; } = default!;
    public DbSet<User> User { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
