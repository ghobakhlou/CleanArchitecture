using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.ValueObjects;
using System.Diagnostics.Metrics;

namespace Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the Student aggregate for persistence using Entity Framework Core.
    /// 
    /// DESIGN RATIONALE:
    /// - **Domain-Driven Design (DDD):**
    ///   * Maps the Student aggregate while preserving domain integrity.
    ///   * Value objects (Email, Name) are explicitly mapped to ensure invariants and encapsulate business rules.
    ///
    /// - **Clean Architecture:**
    ///   * The configuration decouples domain logic from persistence concerns.
    ///   * Domain events are ignored to avoid persisting non-persistent artifacts.
    ///
    /// - **ORM Best Practices:**
    ///   * Uses property conversions to map complex types (e.g., Email) to a simple database type.
    ///   * Configures relationships with cascade delete to maintain referential integrity.
    ///   * Naming conventions (snake case for table names) ensure consistency across the database schema.
    /// </summary>
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Set the primary key and configure the table naming convention.
            builder.HasKey(x => x.Id);
            builder.ToTable(nameof(Student).ToSnakeCase());

            // Exclude domain events from persistence, as they are used for business logic only.
            builder.Ignore(c => c.DomainEvents);

            // Map the Email value object to its underlying string representation.
            builder.Property(p => p.Email)
                   .HasConversion(
                       p => p.EmailAddress,
                       p => Email.Create(p).Value
                   );

            // Configure the owned type for Name to persist its properties.
            builder.OwnsOne(c => c.Name, name =>
            {
                name.Property(e => e.FirstName);
                name.Property(e => e.LastName);
            });

            // Define the one-to-many relationship between Student and Enrollments.
            // Cascade delete is configured to ensure that orphaned enrollments are removed when a student is deleted.
            // The property access mode is set to Field to encapsulate the enrollments collection.
            builder.HasMany(p => p.Enrollments)
                   .WithOne(p => p.Student)
                   .OnDelete(DeleteBehavior.Cascade)
                   .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
