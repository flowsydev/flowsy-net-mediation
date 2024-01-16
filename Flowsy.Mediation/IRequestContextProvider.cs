namespace Flowsy.Mediation;

public interface IRequestContextProvider<TContext>
{
    TContext ProvideContext();
    Task<TContext> ProvideContextAsync(CancellationToken cancellationToken);
}
