using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApplication.BLL.Specifications
{
    public static class SpecificationsEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery,
            ISpecifications<T> spec)
        {
            var query = inputQuery;
            if(spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);     
            }
            query = spec.Includes.Aggregate
                (query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));
            return query;
        }
    }
}
