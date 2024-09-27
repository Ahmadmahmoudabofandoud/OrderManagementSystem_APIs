using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.UserAggregate;
using OrderManagementSystem.Core.Repositories.Contract;
using OrderManagementSystem.Core.Specifications;
using OrderManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly OrderManagementDbContext _context;

        public GenericRepository(OrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);


        public void Add(T Entity)
            => _context.Add(Entity);

        public void Delete(T Entity)
            => _context.Remove(Entity);

        public void Update(T Entity)
            => _context.Update(Entity);

        public async Task<bool> GetAnyAsync(string userName)
            => await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().SingleOrDefaultAsync(predicate);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
             => await _context.Set<T>().AnyAsync(predicate);

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        => await ApplySpecifications(spec).ToListAsync();

        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
            => SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec);
    }
}
