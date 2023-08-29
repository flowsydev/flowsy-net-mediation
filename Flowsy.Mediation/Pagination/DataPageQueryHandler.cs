namespace Flowsy.Mediation.Pagination;

public class DataPageQueryHandler<TQuery, TQueryResult, TResult> : RequestHandler<TQuery, TQueryResult>
    where TQueryResult : DataPageQueryResult<TResult>
    where TQuery : DataPageQuery<TQueryResult, TResult>, IDataPageQuery
    where TResult : class
{
}