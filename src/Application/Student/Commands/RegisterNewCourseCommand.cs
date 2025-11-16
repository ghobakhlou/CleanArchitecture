using Application.Student.Inputs;
using CSharpFunctionalExtensions;
using Application.Student;

namespace Application.Student.Commands;

public class RegisterNewCourseCommand(RegisterNewCourseInput input) : ICommand<Guid>
{
    public RegisterNewCourseInput Input { get; } = input;
}

public class RegisterNewCourseCommandHandler(StudentOperationsEntry entry) : ICommandHandler<RegisterNewCourseCommand, Guid>
{
    public Task<Result<Guid>> Handle(RegisterNewCourseCommand request, CancellationToken cancellationToken)
        => entry.RegisterCourseAsync(request.Input, cancellationToken);
}
