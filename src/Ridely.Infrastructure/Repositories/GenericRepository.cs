﻿using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;

namespace Ridely.Infrastructure.Repositories;
internal abstract class GenericRepository<TEntity>(ApplicationDbContext context) :
    IGenericRepository<TEntity> where TEntity : Entity
{
    private readonly ApplicationDbContext _context = context;

    public async Task AddAsync(TEntity entity) =>
        await _context.Set<TEntity>().AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities) =>
        await _context.Set<TEntity>().AddRangeAsync(entities);

    public void Delete(TEntity entity) =>
        _context.Set<TEntity>().Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) =>
        _context.Set<TEntity>().RemoveRange(entities);

    public async Task<List<TEntity>> GetAllAsync() =>
        await _context.Set<TEntity>().ToListAsync();

    public virtual async Task<TEntity?> GetAsync(long id) =>
        await _context.Set<TEntity>().FindAsync(id);

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().UpdateRange(entities);
    }
}
