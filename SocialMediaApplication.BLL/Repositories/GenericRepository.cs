using Microsoft.EntityFrameworkCore;
using SocialMediaApplication.BLL.Interfaces;
using SocialMediaApplication.BLL.Specifications;
using SocialMediaApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private protected readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(T entity)
            => _dbContext.Set<T>().Add(entity);
        public void Update(T entity)
            => _dbContext.Set<T>().Update(entity);

        public void Delete(T entity)
            => _dbContext.Set<T>().Remove(entity);

        public async Task<T?> GetAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).AsNoTracking().ToListAsync();  
        }
        public async Task<IQueryable<T>> GetAllQueryableAsync()
        {
            return  _dbContext.Set<T>().AsNoTracking();
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }
        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        //public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        //{
        //    return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        //}

        public async Task<bool> AnyWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).AnyAsync();
        }
        public bool AnyWithSpec(ISpecifications<T> spec)
        {
            var dummySpec = new BaseSpecifications<T>();
            return  ApplySpecifications(dummySpec).AsNoTracking().Any(spec.Criteria);
        }
    }
}
