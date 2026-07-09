using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;
using SharedKernel.Persistance.Abstractions;
using ShellMgmt.Persistence.ApplicationDbContext;

namespace ShellMgmt.Persistence.Ef.Repository;

public class Repository<T>(AppDbContext context) : IReadRepository<T>, IRepository<T> where T : class
{
    private readonly AppDbContext _context = context;

    public async Task<T> Add(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<PagedList<T>> Get(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        List<T> list = await SpecificationEvaluator.GetQuery(_context.Set<T>().AsQueryable(), specification, out int totalCount)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

        return new PagedList<T>(list, specification.Skip, specification.Take, totalCount);
    }

    public async Task<T?> GetById(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public Task Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }
}
