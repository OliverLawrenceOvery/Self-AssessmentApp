using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_EntityFramework.CRUD_Services
{
    public class GenericDataService<T> : IDataService<T> where T : IdentityObject
    {
        public virtual async Task<T> Get(int id)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                T entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                IEnumerable<T> entities = await context.Set<T>()
                    .ToListAsync();
                return entities;
            }
        }

        public virtual async Task<T> Create(T entity)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                EntityEntry<T> createdResult = await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();
                return createdResult.Entity;
            }
        }

        public virtual async Task<bool> Delete(int id)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                T entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public virtual async Task<T> Update(int id, T entity)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                entity.Id = id;
                context.Set<T>().Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        public virtual async Task<IEnumerable<T>> GetByParameter(IdentityObject parameter)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                IEnumerable<T> entities = await context.Set<T>().Where(t => t.Id == parameter.Id)
                    .ToListAsync();
                return entities;
            }
        }
    }
}
