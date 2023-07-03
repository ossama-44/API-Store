using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,ISpecifications<TEntity> specifications)
        {
            var query = inputQuery;

            if (specifications.Criteria != null)
            {
                query = query.Where(specifications.Criteria);
            }

            if(specifications.OrderBy != null)
                query = query.OrderBy(specifications.OrderBy);

            if (specifications.OrderByDescending != null)
                query = query.OrderByDescending(specifications.OrderByDescending);

            if (specifications.IsPagingEnabled) 
                query = query.Skip(specifications.Skip).Take(specifications.Take);

            query = specifications.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
