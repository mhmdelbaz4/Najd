using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NajdAPI.Data;
using NajdAPI.IRepos;
using NajdAPI.Models;

namespace NajdAPI.Repos
{
    public class GenericRepo<T>(NajdDBContext context) : IGenericRepo<T> where T : BaseEntity
    {
        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            context.Attach(entity);
            entity.IsDeleted = true;
            context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<IReadOnlyList<T>?> GetEntitiesAsync()
        {
            return await context.Set<T>().Where(entity => entity.IsDeleted == false).ToListAsync();
        }

        public async Task<T?> GetEntityAsync(int id)
        {
            return await context.Set<T>().FirstOrDefaultAsync(entity => entity.Id == id && entity.IsDeleted == false);
        }

        public bool IsExist(int id)
        {
            return context.Set<T>().Any(x => x.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }
    }
}
