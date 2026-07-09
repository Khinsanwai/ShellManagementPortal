using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;
using SharedKernel.Persistance.Abstractions;
using ShellMgmt.Persistence.ApplicationDbContext;

namespace ShellMgmt.Persistence.Ef.Repository;

public class ReadRepository<T>(ReadDbContext dbContext) : IReadRepository<T> where T : class
{
    private readonly ReadDbContext _dbContext = dbContext;

    public async Task<PagedList<T>> Get(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        List<T> list = await SpecificationEvaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification, out int totalCount)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

        return new PagedList<T>(list, specification.Skip, specification.Take, totalCount);
    }

    public async Task<T?> GetById(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }
}
