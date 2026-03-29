using System.Linq.Expressions;

namespace Mvc.Data.Repository;

public sealed class QueryOptions<TEntity> where TEntity : class
{
    public Expression<Func<TEntity, bool>> Where { get; set; } = null!;
    public Expression<Func<TEntity, object>> OrderBy { get; set; } = null!;

    private string[] _includes = [];
    public string Includes
    {
        set => _includes = value.Replace(" ", "").Split(",");
    }

    public string[] GetIncludes()
    {
        return _includes;
    }

    private string[] _thenIncludes = [];
    public string ThenIncludes
    {
        set => _thenIncludes = value.Replace(" ", "").Split(",");
    }

    public string[] GetThenIncludes()
    {
        return _thenIncludes;
    }

    public bool HasWhere => Where is not null;
    public bool HasOrderBy => OrderBy is not null;
}