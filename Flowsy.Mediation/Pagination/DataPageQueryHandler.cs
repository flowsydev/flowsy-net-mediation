namespace Flowsy.Mediation.Pagination;

public class DataPageQueryHandler<TQuery, TResult> : RequestHandler<TQuery, DataPageQueryResult<TResult>>
    where TQuery : DataPageQuery<TResult>, IDataPageQuery
    where TResult : class
{
}