namespace Flowsy.Mediation;

public interface IRequestContextProvider
{
    object ProvideContext();
    Task<object> ProvideContextAsync(CancellationToken cancellationToken);
}
