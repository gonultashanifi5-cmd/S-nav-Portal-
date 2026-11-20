using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Models;

namespace OnlineSinavPortali.Repositories;

public class GenericRepository<T>(AppDbContext context) where T : BaseEntity
{
    protected DbSet<T> Entities { get; } = context.Set<T>();

    public IQueryable<T> Query() => Entities.AsNoTracking();

    public ValueTask<T?> GetByIdAsync(int id) => Entities.FindAsync(id);

    public async Task AddAsync(T entity)
    {
        Entities.Add(entity);
        await SaveAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        Entities.Remove(entity);
        await SaveAsync();
    }

    public Task<int> SaveAsync() => context.SaveChangesAsync();
}
