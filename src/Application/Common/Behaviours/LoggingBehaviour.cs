using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;

namespace Application.Common.Behaviors;

public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest>
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
        //_currentUserService = currentUserService;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        var userName = _currentUserService?.Email;

        _logger.LogInformation("Services Request: {Name} {@UserName} {@Request}",
            requestName, userName, request);

        return Task.CompletedTask;
    }
}
