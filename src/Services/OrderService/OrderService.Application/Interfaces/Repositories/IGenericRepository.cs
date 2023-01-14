using OrderService.Domain.SeedWork;
using System.Linq.Expressions;

namespace OrderService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAll();

    Task<List<T>> Get(Func<IQueryable<T>, IOrderedQueryable<T>> includes, Expression<Func<T, bool>> filter = null);

    Task<List<T>> Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);

    Task<T> GetByID(Guid id);

    Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

    Task<T> AddAsync(T entity);

    T Update(T entity);
}