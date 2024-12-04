using Flowsy.Mediation.Test.Simulation.Commands.AddNumbers;

namespace Flowsy.Mediation.Test.Simulation;

public class OperationContextProvider : IRequestContextProvider
{
    public object ProvideContext<TRequest>(TRequest request) where TRequest : IContextualRequest
    {
        return request is AddNumbersCommand ? new OperationContext("john.doe@example.com") : OperationContext.Empty;
    }

    public Task<object> ProvideContextAsync<TRequest>(TRequest request, CancellationToken cancellationToken)  where TRequest : IContextualRequest
        => Task.Run(() => ProvideContext(request), cancellationToken);
}