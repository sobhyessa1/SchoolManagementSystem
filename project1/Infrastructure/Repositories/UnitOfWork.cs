using project1.Application.Interfaces;
using project1.Infrastructure.Data;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace project1.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchoolDbContext _context;
        private readonly Hashtable _repositories;

        public UnitOfWork(SchoolDbContext context)
        {
            _context = context;
            _repositories = new Hashtable();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
            {
                // Should not happen as initialized in ctor, but for safety
                return new GenericRepository<T>(_context);
            }

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type]!;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
