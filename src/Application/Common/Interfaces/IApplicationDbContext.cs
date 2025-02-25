using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.ValueObjects;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    public DbSet<Student> Students { get; }
    public DbSet<Course> Courses { get; }
}

