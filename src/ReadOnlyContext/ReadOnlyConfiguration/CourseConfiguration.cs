using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReadOnlyContext.QueryModels;

namespace ReadOnlyContext.ReadOnlyConfiguration;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable(nameof(Course).ToSnakeCase());

    }
}
