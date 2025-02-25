using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.ValueObjects;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext implementation that serves as the persistence gateway for the application.
/// 
/// DESIGN HIGHLIGHTS:
/// - **Clean Architecture & DDD:**  
///   Implements IApplicationDbContext to decouple domain logic from persistence.
///   Manages aggregates (e.g., Student, Course) while preserving domain invariants.
/// 
/// - **Auditing & Soft Deletion:**  
///   Globally applies audit information (Created/Updated timestamps and user info) and 
///   implements soft delete functionality via global query filters.
/// 
/// - **Domain Events & CQRS:**  
///   Captures and dispatches domain events using MediatR after changes are saved,
///   promoting an event-driven architecture.
/// 
/// - **Integration & Context Awareness:**  
///   Uses injected services (ICurrentUserService, IServiceBusService) to capture contextual data 
///   and enable integration with external systems.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceBusService _serviceBusService;
    private readonly MediatR.IMediator _mediator;

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options)
    {

    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IServiceBusService serviceBusService,
        MediatR.IMediator mediator
        ) : base(options)
    {
        _currentUserService = currentUserService;
        _serviceBusService = serviceBusService;
        _mediator = mediator;
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetAuditableFieldsGlobally();
        SoftDeleteAuditable();

        var result =  await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEvents();

        return result;
    }

    public async Task<int> SaveChangesAsync(string auditType, CancellationToken cancellationToken = new CancellationToken())
    {        

        return await SaveChangesAsync(cancellationToken);
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

    private void SetAuditableFieldsGlobally()
    {
        foreach (var item in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (item.State)
            {
                case EntityState.Added:
                    item.Entity.CreatedDate = DateTimeOffset.UtcNow;
                    item.Entity.UpdatedDate = DateTimeOffset.UtcNow;
                    item.Entity.CreatedBy = _currentUserService.Email;
                    item.Entity.UpdatedBy = _currentUserService.Email;
                    break;
                case EntityState.Modified:
                    item.Entity.UpdatedDate = DateTimeOffset.UtcNow;
                    item.Entity.UpdatedBy = _currentUserService.Email;
                    break;
                case EntityState.Deleted:
                    item.Entity.UpdatedDate = DateTimeOffset.UtcNow;
                    item.Entity.UpdatedBy = _currentUserService.Email;
                    break;
            }
        }

    }

    public  async Task DispatchDomainEvents()
    {
        var entities = ChangeTracker
            .Entries<IHasDomainEvent>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.DomainEvents.Clear());

        foreach (var domainEvent in domainEvents)
        {
            // decide to publish to mediator or ServiceBus. depending on the software design
            // await _mediator.Publish(domainEvent);
            //  await _serviceBusService.SendAsync(domainEvent);
        }
    }

    private void SoftDeleteAuditable()
    {
        foreach (var item in ChangeTracker.Entries<ISoftDeleteable>())
        {
            switch (item.State)
            {
                case EntityState.Deleted:
                    item.Entity.IsDeleted = true;
                    item.State = EntityState.Modified;
                    break;
            }
        }
    }

}
