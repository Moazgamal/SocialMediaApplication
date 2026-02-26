using Microsoft.EntityFrameworkCore;
using SocialMediaApplication.BLL.Interfaces;
using SocialMediaApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<T> GetAsync(int id)
        {
            return await _dbContext.FindAsync<T>(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            //if (typeof(T) == typeof(Employee))
            //    return (IEnumerable<T>)await _dbContext.Set<Employee>().Include(E => E.Department).AsNoTracking().ToListAsync();
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }
    }
}
