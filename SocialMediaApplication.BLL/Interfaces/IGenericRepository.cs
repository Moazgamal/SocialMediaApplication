using SocialMediaApplication.BLL.Specifications;
using SocialMediaApplication.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.BLL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
        Task<T?> GetAsync(int id);
        Task<T?> GetWithSpecAsync(ISpecifications<T> spec);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
