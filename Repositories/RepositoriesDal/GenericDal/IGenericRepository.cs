using System.Linq.Expressions;

namespace Repositories.RepositoriesDal.GenericDal;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetAll();
    IQueryable<T> Where(Expression<Func<T, bool>> expression);
    ValueTask<T?> GetByIdAsync(Guid id);
    ValueTask AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}