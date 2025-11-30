using Microsoft.EntityFrameworkCore;
using project1.Application.Interfaces;
using project1.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace project1.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SchoolDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(SchoolDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            // If entity has UpdatedDate, set it
            var updatedDateProp = typeof(T).GetProperty("UpdatedDate");
            if (updatedDateProp != null && updatedDateProp.PropertyType == typeof(DateTime?))
            {
                updatedDateProp.SetValue(entity, DateTime.UtcNow);
            }
        }

        public void Delete(T entity)
        {
            // Check for Soft Delete (IsActive property)
            var isActiveProp = typeof(T).GetProperty("IsActive");
            if (isActiveProp != null && isActiveProp.PropertyType == typeof(bool))
            {
                isActiveProp.SetValue(entity, false);
                Update(entity); // Will set UpdatedDate via Update method
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null) return await _dbSet.LongCountAsync();
            return await _dbSet.LongCountAsync(predicate);
        }

        public async Task<IReadOnlyList<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else
            {
                return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            }
        }
    }
}
