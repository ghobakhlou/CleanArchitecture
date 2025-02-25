using Application.Common.Interfaces;
using Application.Student.Inputs;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Application.Student.Commands;

public class RegisterNewCourseCommand(
   RegisterNewCourseInput input) : ICommand<Guid>
{
    public RegisterNewCourseInput Input { get; } = input;
}

public class RegisterNewCourseCommandHandler(IApplicationDbContext context) : ICommandHandler<RegisterNewCourseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterNewCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.SingleOrDefaultAsync(c => c.Name == request.Input.Name, cancellationToken: cancellationToken);

        //Assuming course name must be unique
        if (course is not null)
        {
            return Result.Failure<Guid>("course already exists.");
        }

        course = new Domain.Entities.Course() { Id = Guid.NewGuid(), Name = request.Input.Name };
        await context.Courses.AddAsync(course, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(course.Id);
    }
}

