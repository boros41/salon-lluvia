using Microsoft.EntityFrameworkCore;

namespace Mvc.Data.Repository;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected DbContext Context { get; init; }
    private DbSet<TEntity> DbSet { get; }

    public Repository(DbContext ctx)
    {
        Context = ctx;
        DbSet = ctx.Set<TEntity>();
    }

    public virtual void Insert(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public virtual IEnumerable<TEntity> List(QueryOptions<TEntity> options)
    {
        IQueryable<TEntity> query = DbSet;

        foreach (string include in options.GetIncludes())
        {
            query = query.Include(include);
        }

        if (options.HasWhere)
        {
            query = query.Where(options.Where);
        }

        if (options.HasOrderBy)
        {
            query = query.OrderBy(options.OrderBy);
        }

        return query.ToList();
    }

    public virtual TEntity? Get(int id)
    {
        return DbSet.Find(id);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void Save()
    {
        Context.SaveChanges();
    }
}