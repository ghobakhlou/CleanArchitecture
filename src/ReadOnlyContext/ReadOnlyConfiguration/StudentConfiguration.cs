using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReadOnlyContext.QueryModels;
using Shared.ValueObjects;

namespace ReadOnlyContext.ReadOnlyConfiguration
{

    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable(nameof(Student).ToSnakeCase());


            builder.Property(p => p.Email)
                   .HasConversion(
                       p => p.EmailAddress,
                       p => Email.Create(p).Value
                   );

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
