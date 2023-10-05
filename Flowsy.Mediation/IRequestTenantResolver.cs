namespace Flowsy.Mediation;

public interface IRequestTenantResolver
{
    string GetTenantId();
    string GetTenantIdAsync();
}