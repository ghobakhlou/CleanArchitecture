using Application.Common.Interfaces;   
using Application.Student.Inputs;      
using CSharpFunctionalExtensions;      
using Microsoft.EntityFrameworkCore;   
using Shared.ValueObjects;             
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student.Commands
{
    /*
       WHY WE USE A COMMAND + HANDLER PATTERN HERE (CQRS & MEDIATR):
       ----------------------------------------------------------------
       - We isolate the "enroll student" use case into a dedicated command to adhere to the CQRS principle:
         * "Command" = write operation or intent.
       - The command object carries only the data necessary to fulfill the enrollment, promoting clarity.
       - MediatR (through ICommand<T>) orchestrates dispatching this command to the right handler, 
         decoupling controller endpoints from domain logic.
       - This separation simplifies maintenance, testing, and future changes (i.e., business logic isn't tied to controllers).
     */
    public class EnrollStudentCommand(EnrollStudentInput input) : ICommand<Guid>
    {
        /*
           WHY WE ENCAPSULATE INPUT WITH A VALUE OBJECT:
           - By receiving an 'EnrollStudentInput' DTO, we encapsulate all required data in a single, immutable object.
           - This adheres to domain-driven design (DDD) principles by explicitly naming our intent through a purposeful object.
         */
        public EnrollStudentInput Input { get; } = input;
    }


    public class EnrollStudentCommandHandler(IApplicationDbContext context) : ICommandHandler<EnrollStudentCommand, Guid>
    {
        /*
           WHY FUNCTIONAL APPROACH WITH Result<T>:
           - We return a 'Result<Guid>' to express success or failure in a functional style.
           - This pattern makes error handling explicit rather than relying on exceptions for control flow.
           - It fosters more predictable, testable code and encourages a "happy path" approach with clear branching for failures.
         */
        public async Task<Result<Guid>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the course - a key domain entity the student will be tied to
            var course = await context.Courses
                .SingleOrDefaultAsync(
                    c => c.Id == request.Input.CourseId,
                    cancellationToken: cancellationToken
                );

            if (course is null)
            {
                // Returning a functional failure preserves a single, consistent approach to error handling
                return Result.Failure<Guid>("Course doesn't exist.");
            }

            // Validate email via a domain-specific value object, ensuring consistency & reusability
            var email = Email.Create(request.Input.EmailAddress);
            if (email.IsFailure)
            {
                return Result.Failure<Guid>("Email is invalid.");
            }

            // Check if this student already exists for the given email, abiding by domain constraints
            var emailValue = email.Value;
            var student = await context.Students
                .SingleOrDefaultAsync(
                    s => s.Email == emailValue,
                    cancellationToken: cancellationToken
                );

            if (student is null)
            {
                // The Name value object ensures domain rules like non-empty first/last name 
                // are validated in a single place.
                var name = Name.Create(request.Input.FirstName, request.Input.LastName);
                if (name.IsFailure)
                {
                    return Result.Failure<Guid>("Name is invalid.");
                }

                // Creating a new student entity is part of the domain logic.
                // Using a value object for name & email helps keep domain rules consistent.
                student = new Domain.Entities.Student(Guid.NewGuid(), name.Value, emailValue, course);

                // Persist the new student entity to the database
                await context.Students.AddAsync(student, cancellationToken);
            }
            else
            {
                // If the student already exists, we just enroll them in the course.
                // The Student entity's "EnrollIn" method typically checks domain rules 
                // like whether the student is already enrolled, etc.
                student.EnrollIn(course);
            }

            // Commit changes in a single transaction, consistent with the unit-of-work principle in DDD
            await context.SaveChangesAsync(cancellationToken);

            // Return a success result capturing the student's unique ID
            return Result.Success(student.Id);
        }
    }
}
