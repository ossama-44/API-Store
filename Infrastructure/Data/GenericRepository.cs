using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext context;

        public GenericRepository(StoreDbContext context)
        {
            this.context = context;
        }
        public void Add(T entity)
        => this.context.Set<T>().Add(entity);

        public void Delete(T entity)
            => this.context.Set<T>().Remove(entity);


        public async Task<T> GetByIdAsync(int id)
          => await this.context.Set<T>().FindAsync(id);

        public async Task<T> GetEntityWithSpecifications(ISpecifications<T> specifications)
           => await ApplySpecifications(specifications).FirstOrDefaultAsync();


        public async Task<IReadOnlyList<T>> ListAllAsync()
            => await this.context.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> ListAsync(ISpecifications<T> specifications)
           => await ApplySpecifications(specifications).ToListAsync();

        private IQueryable<T> ApplySpecifications(ISpecifications<T> specifications) 
            => SpecificationsEvaluator<T>.GetQuery(this.context.Set<T>().AsQueryable(), specifications);
        public void Update(T entity)
          => this.context.Set<T>().Update(entity);

        public async Task<int> CountAsync(ISpecifications<T> specifications)
            => await ApplySpecifications(specifications).CountAsync();
    }

}
