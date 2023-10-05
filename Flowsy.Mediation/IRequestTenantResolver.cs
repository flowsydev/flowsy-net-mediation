namespace Flowsy.Mediation;

public interface IRequestTenantResolver
{
    string? GetTenantId();
    Task<string?> GetTenantIdAsync(CancellationToken cancellationToken);
}