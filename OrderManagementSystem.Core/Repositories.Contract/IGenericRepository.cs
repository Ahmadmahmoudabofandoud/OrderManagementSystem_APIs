using OrderManagementSystem.Core.Entities.UserAggregate;
using OrderManagementSystem.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Repositories.Contract
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);

        void Add(T Entity);
        void Update(T Entity);
        void Delete(T Entity);
        Task<bool> GetAnyAsync(string userName);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate); 
    }
}
