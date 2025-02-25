using Application.Common.Exceptions;
using Application.Student.Commands;
using Application.Student.Inputs;
using MediatR;

namespace Api
{
    /// <summary>
    /// GraphQL mutation endpoint demonstrating a clean separation of concerns.
    /// This class leverages CQRS with MediatR to decouple API contracts from application logic,
    /// aligning with Domain-Driven Design (DDD) principles.
    /// </summary>
    [GraphQLDescription("Represents the mutations available.")]
    public class Mutation
    {
        /// <summary>
        /// Enrolls a student by dispatching an <see cref="EnrollStudentCommand"/> through MediatR.
        /// This encapsulates the business logic for enrollment and returns a standardized response.
        /// </summary>
        /// <param name="input">Student enrollment details (name, email, course, etc.).</param>
        /// <param name="sender">MediatR interface for sending commands to the corresponding handler.</param>
        /// <returns>A response message containing the student's unique ID if enrollment succeeds, or Guid.Empty otherwise.</returns>
        public async Task<ResponseMessage<Guid>> EnrollStudent(
            EnrollStudentInput input,
            [Service] ISender sender)
        {
            var result = await sender.Send(new EnrollStudentCommand(input));
            return ResponseMessage<Guid>.ToResponse(
                result,
                result.IsSuccess ? result.Value : Guid.Empty
            );
        }

        /// <summary>
        /// Registers a new course by sending a <see cref="RegisterNewCourseCommand"/> through MediatR.
        /// This method illustrates the CQRS pattern in action by isolating write operations and returning a standardized response.
        /// </summary>
        /// <param name="input">Course registration details (title, description, etc.).</param>
        /// <param name="sender">MediatR interface for command dispatching.</param>
        /// <returns>A response message containing the new course's unique ID if registration is successful, or Guid.Empty otherwise.</returns>
        public async Task<ResponseMessage<Guid>> RegisterNewCourse(
            RegisterNewCourseInput input,
            [Service] ISender sender)
        {
            var result = await sender.Send(new RegisterNewCourseCommand(input));
            return ResponseMessage<Guid>.ToResponse(
                result,
                result.IsSuccess ? result.Value : Guid.Empty
            );
        }
    }
}
