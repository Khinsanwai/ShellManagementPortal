using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel.Domain;
using SharedKernel.Persistance.Abstractions;
using ShellMgmt.Persistence.ApplicationDbContext;

namespace ShellMgmt.Persistence.Ef.Repository;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

    public async Task Begin()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
        }
    }

    public void Dispose() => _context.Dispose();

    public async Task Rollback()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return _context.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<IAuditableEntity>> entries =
            _context.ChangeTracker.Entries<IAuditableEntity>();

        foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedDate).CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.UpdatedDate).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
