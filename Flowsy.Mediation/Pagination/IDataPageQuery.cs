namespace Flowsy.Mediation.Pagination;

public interface IDataPageQuery
{
    long PageNumber { get; set; }
    long PageSize { get; set; }
    bool CountTotal { get; set; }
}