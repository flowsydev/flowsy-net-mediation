namespace Flowsy.Mediation.Test.Simulation;

public abstract class ApplicationRequest<TResponse> : ContextualRequest<OperationContext, TResponse>;

public abstract class ApplicationRequest : ContextualRequest<OperationContext>;