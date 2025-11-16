using Application.Common.Interfaces;
using Application.Student.Inputs;
using CSharpFunctionalExtensions;
using Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student;

/// <summary>
/// The motion-first seam for student commands. All mutations flow through here so the edit path is short
/// and failure messages stay local to the seam.
/// </summary>
public class StudentOperationsEntry(IApplicationDbContext context, StudentFieldRegistry registry)
{
    private readonly IApplicationDbContext _context = context;
    private readonly StudentFieldRegistry _registry = registry;

    public async Task<Result<Guid>> EnrollStudentAsync(EnrollStudentInput input, CancellationToken cancellationToken)
    {
        var course = await _registry.GetCourseAsync(input.CourseId, cancellationToken);
        if (course.IsFailure)
        {
            return FailureWithLocation<Guid>(course.Error, nameof(EnrollStudentAsync));
        }

        var email = _registry.CreateEmail(input.EmailAddress);
        if (email.IsFailure)
        {
            return FailureWithLocation<Guid>(email.Error, nameof(EnrollStudentAsync));
        }

        var existingStudent = await _registry.FindStudentByEmailAsync(email.Value, cancellationToken);
        Student student;

        if (existingStudent is null)
        {
            var name = _registry.CreateName(input.FirstName, input.LastName);
            if (name.IsFailure)
            {
                return FailureWithLocation<Guid>(name.Error, nameof(EnrollStudentAsync));
            }

            student = new Student(Guid.NewGuid(), name.Value, email.Value, course.Value);
            await _context.Students.AddAsync(student, cancellationToken);
        }
        else
        {
            student = existingStudent;
            student.EnrollIn(course.Value);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(student.Id);
    }

    public async Task<Result<Guid>> RegisterCourseAsync(RegisterNewCourseInput input, CancellationToken cancellationToken)
    {
        var uniqueness = await _registry.EnsureCourseNameIsUniqueAsync(input.Name, cancellationToken);
        if (uniqueness.IsFailure)
        {
            return FailureWithLocation<Guid>(uniqueness.Error, nameof(RegisterCourseAsync));
        }

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = input.Name
        };

        await _context.Courses.AddAsync(course, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(course.Id);
    }

    private static Result<T> FailureWithLocation<T>(string message, string member)
    {
        return Result.Failure<T>($"[{nameof(StudentOperationsEntry)}::{member}] {message}");
    }
}
