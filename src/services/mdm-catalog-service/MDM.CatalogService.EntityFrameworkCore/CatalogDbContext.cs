using Abp.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MDM.CatalogService.EntityFrameworkCore;

public class CatalogDbContext : AbpDbContext
{
    public CatalogDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureBaseService();
    }
}