namespace Flowsy.Mediation;

public interface IRequestEnvironmentResolver
{
    Task<RequestEnvironment> ResolveAsync(CancellationToken cancellationToken);
}
