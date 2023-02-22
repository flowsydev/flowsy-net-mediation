namespace Flowsy.Mediation.Pagination;

public class DataPageQueryResult<T> where T : class
{
    public DataPageQueryResult(IDataPageQuery query, IEnumerable<T> items, long? totalItemCount = null)
    {
        Query = query;
        Items = items;
        TotalItemCount = totalItemCount;
    }

    public IDataPageQuery Query { get; }
    
    public IEnumerable<T> Items { get; }
    
    public long ItemCount => Items.Count();
    public long? TotalItemCount { get; }
    
    public long? TotalPageCount => 
        Query is {CountTotal: true, PageSize: > 0} && TotalItemCount is > 0
            ? (long) Math.Ceiling(TotalItemCount.Value / (decimal) Query.PageSize) 
            : default;
    
    public bool HasMore 
        => Query.CountTotal && TotalItemCount.HasValue && TotalPageCount.HasValue 
            ? Query.PageNumber < TotalPageCount
            : Query.PageSize == ItemCount;
}