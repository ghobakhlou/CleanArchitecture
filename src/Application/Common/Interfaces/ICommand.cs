using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Common.Interfaces;

/// <summary>
///  Marker interface to represent a command with a result type response
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Defines a handler for a command with Result type response
/// </summary>
public interface ICommandHandler<TRequest>
    : IRequestHandler<TRequest, Result> where TRequest : ICommand
{

}

/// <summary>
///  Marker interface to represent a command with a result type response
/// </summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

/// <summary>
/// Defines a handler for a request with Result type response
/// </summary>
public interface ICommandHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, Result<TResponse>> where TRequest : ICommand<TResponse>
{
}
