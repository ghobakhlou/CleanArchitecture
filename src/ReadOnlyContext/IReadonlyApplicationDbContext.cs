using ReadOnlyContext.QueryModels;
using System.Linq;

namespace ReadOnlyContext;

public interface IReadonlyApplicationDbContext
{
    IQueryable<Student> Students { get; }
    IQueryable<Course> Courses { get; }
}

