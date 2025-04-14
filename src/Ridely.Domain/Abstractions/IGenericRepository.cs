namespace Ridely.Domain.Abstractions;
public interface IGenericRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetAsync(long id);
    Task<List<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
}
