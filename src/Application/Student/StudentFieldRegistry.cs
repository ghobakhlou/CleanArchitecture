using Application.Common.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student;

/// <summary>
/// Central seam for the most common field rules (validation, mapping, and uniqueness).
/// Editing student policies now happens here so the motion path stays short and failures are local.
/// </summary>
public class StudentFieldRegistry(IApplicationDbContext context)
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Course>> GetCourseAsync(Guid id, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken: cancellationToken);

        return course is null
            ? Result.Failure<Course>("Course doesn't exist.")
            : Result.Success(course);
    }

    public Result<Email> CreateEmail(string emailAddress) => Email.Create(emailAddress);

    public Result<Name> CreateName(string firstName, string lastName) => Name.Create(firstName, lastName);

    public async Task<Student?> FindStudentByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return await _context.Students
            .SingleOrDefaultAsync(student => student.Email == email, cancellationToken: cancellationToken);
    }

    public async Task<Result> EnsureCourseNameIsUniqueAsync(string name, CancellationToken cancellationToken)
    {
        var exists = await _context.Courses
            .AnyAsync(c => c.Name == name, cancellationToken: cancellationToken);

        return exists
            ? Result.Failure("course already exists.")
            : Result.Success();
    }
}
