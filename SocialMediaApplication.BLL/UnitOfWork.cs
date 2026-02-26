using SocialMediaApplication.BLL.Interfaces;
using SocialMediaApplication.BLL.Repositories;
using SocialMediaApplication.DAL.Data;
using SocialMediaApplication.DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.BLL
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private Hashtable _repos;

        public UnitOfWork(ApplicationDbContext DbContext)
        {
            _dbContext = DbContext;
            _repos = new Hashtable();
        }
        public IGenericRepository<T> Repository<T>() where T : class
        {
            var key = typeof(T).Name;
            if (!_repos.ContainsKey(key))
            {
                var repo = new GenericRepository<T>(_dbContext);
                _repos.Add(key, repo);
            }
            return _repos[key] as IGenericRepository<T>;
        }
        public async Task<int> Complete()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
