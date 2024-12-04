namespace Flowsy.Mediation.Test.Simulation;

public abstract class ApplicationRequestHandler<TRequest, TResult> : ContextualRequestHandler<OperationContext, TRequest, TResult>
    where TRequest : ApplicationRequest<TResult>;
    
public abstract class ApplicationRequestHandler<TRequest> : ContextualRequestHandler<OperationContext, TRequest>
    where TRequest : ApplicationRequest;