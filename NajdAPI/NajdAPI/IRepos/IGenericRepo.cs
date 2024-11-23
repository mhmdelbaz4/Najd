using NajdAPI.Models;

namespace NajdAPI.IRepos;

public interface IGenericRepo<T> where T : BaseEntity
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T?> GetEntityAsync(int id);
    Task<IReadOnlyList<T>?> GetEntitiesAsync();
    bool IsExist(int id);
    Task<bool> SaveChangesAsync();
}
