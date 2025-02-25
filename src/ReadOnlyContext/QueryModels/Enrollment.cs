using Shared.Common;
using System;

namespace ReadOnlyContext.QueryModels;

public class Enrollment : Entity<Guid>
{
    public virtual Course Course { get; }
    public virtual Student Student { get; }

    protected Enrollment()
    {
    }

    public Enrollment(Course course, Student student)
        : this()
    {
        Course = course;
        Student = student;
    }
}