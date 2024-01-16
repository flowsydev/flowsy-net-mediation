namespace Flowsy.Mediation;

public interface IRequestContextProvider<TContext>
{
    TContext Provide();
    Task<TContext> ProvideAsync(CancellationToken cancellationToken);
}
