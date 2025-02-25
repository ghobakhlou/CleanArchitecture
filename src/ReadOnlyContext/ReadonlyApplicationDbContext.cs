using Microsoft.EntityFrameworkCore;
using ReadOnlyContext.QueryModels;
using Shared.Common;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ReadOnlyContext;

public class ReadonlyApplicationDbContext : DbContext, IReadonlyApplicationDbContext
{
    public ReadonlyApplicationDbContext(
        DbContextOptions<ReadonlyApplicationDbContext> options
        ) : base(options)
    {

    }

    public IQueryable<Student> Students => Set<Student>().AsNoTracking();
    public IQueryable<Course> Courses => Set<Course>().AsNoTracking();


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new InvalidOperationException("This context is read-only.");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplyGlobalQueryFilter(builder);
        base.OnModelCreating(builder);
    }
    private void ApplyGlobalQueryFilter(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeleteable).IsAssignableFrom(entityType.ClrType))
            {
                builder.SetSoftDeleteFilter(entityType.ClrType);
            }
        }
    }
}
