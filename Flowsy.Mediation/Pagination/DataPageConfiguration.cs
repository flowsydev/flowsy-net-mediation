namespace Flowsy.Mediation.Pagination;

public class DataPageConfiguration
{
    public static DataPageConfiguration Default { get; set; } = new();

    public DataPageConfiguration(long pageSize = long.MaxValue, long maxPageSize = long.MaxValue)
    {
        PageSize = pageSize;
        MaxPageSize = maxPageSize;
    }

    public long PageSize { get; set; }
    public long MaxPageSize { get; set; }
}