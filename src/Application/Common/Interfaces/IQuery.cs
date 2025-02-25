using MediatR;

namespace Application.Common.Interfaces;

/// <summary>
///  Marker interface to represent a command with a result type response
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse>
{
}


/// <summary>
/// Defines a handler for a request with Result type response
/// </summary>
public interface IQueryHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
{

}
