using Abp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data.Common;

namespace MDM.CatalogService.EntityFrameworkCore;

public static class CatalogDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<CatalogDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<CatalogDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }

    public static void ConfigureBaseService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        var valueComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());
    }
}
