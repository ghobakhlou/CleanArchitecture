using HotChocolate.Execution.Instrumentation;
using HotChocolate.Execution;

namespace Api;

public class ExecutionEventListener(ILogger<ExecutionEventListener> logger) : ExecutionDiagnosticEventListener
{
    public override void RequestError(IRequestContext context,
        Exception exception)
    {
        logger.LogError(exception, "A request error occurred!");
    }
}
