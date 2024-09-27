using OrderManagementSystem.Core;
using OrderManagementSystem.Core.Repositories.Contract;
using OrderManagementSystem.Infrastructure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderManagementDbContext _context;

        private Hashtable _repositories;


        public UnitOfWork(OrderManagementDbContext context)
        {
            _context = context;
            _repositories = new Hashtable();
        }
        public IGenericRepository<T> Repository<T>() where T : class
        {
            var key = typeof(T).Name;

            if (!_repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<T>(_context);

                _repositories.Add(key, repository);
            }

            return _repositories[key] as IGenericRepository<T>;
        }

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _context.DisposeAsync();

        
    }
}
