using HotChocolate.Data;
using ReadOnlyContext;
using ReadOnlyContext.QueryModels;

namespace Api;

public class Query
{
    [GraphQLDescription("Get students")]
    [UseFiltering]
    public IQueryable<Student> GetStudents([Service] IReadonlyApplicationDbContext context)
    {
        return context.Students;
    }

    [GraphQLDescription("Get courses")]
    [UseFiltering]
    public IQueryable<Course> GetCourses([Service] IReadonlyApplicationDbContext context)
    {
        return context.Courses;
    }
}
