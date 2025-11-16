using Application.Student.Inputs;
using CSharpFunctionalExtensions;
using Application.Student;

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
        public EnrollStudentInput Input { get; } = input;
    }

    public class EnrollStudentCommandHandler(StudentOperationsEntry entry) : ICommandHandler<EnrollStudentCommand, Guid>
    {
        public Task<Result<Guid>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
            => entry.EnrollStudentAsync(request.Input, cancellationToken);
    }
}
