namespace Flowsy.Mediation.Pagination;

public class DataPageQuery<TQueryResult, TResult> : Request<TQueryResult>, IDataPageQuery
    where TQueryResult : DataPageQueryResult<TResult>
    where TResult : class
{
    protected DataPageQuery(long pageNumber = 1, long? pageSize = null, bool countTotal = false)
    {
        PageNumber = pageNumber;
        _pageSize = pageSize;
        CountTotal = countTotal;
    }

    private long? _pageSize;

    public long PageNumber { get; set; }

    public long PageSize
    {
        get => _pageSize ?? DataPageConfiguration.Default.PageSize;
        set => _pageSize = Math.Min(value, DataPageConfiguration.Default.MaxPageSize);
    }
    public bool CountTotal { get; set; }
}