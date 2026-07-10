using Microsoft.EntityFrameworkCore;
using ShellMgmt.Domain.AppModels;
using ShellMgmt.Domain.ClaimModels;
using ShellMgmt.Domain.InstitutionModels;
using ShellMgmt.Domain.MenuItemModels;
using ShellMgmt.Domain.OrgUnitModels;
using ShellMgmt.Domain.ResourceModels;
using ShellMgmt.Domain.TenantModels;

namespace ShellMgmt.Persistence.ApplicationDbContext;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MenuItem> MenuItem { get; set; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<Resource> Resource { get; set; }
    public DbSet<Claim> Claim { get; set; }
    public DbSet<Institution> Institution { get; set; }
    public DbSet<OrgUnit> OrgUnit { get; set; }
    public DbSet<App> App { get; set; }
    public DbSet<UserMenuAssignment> UserMenuAssignment { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItem>().ToTable("MenuItem", "appshell");
        modelBuilder.Entity<Tenant>().ToTable("Tenant", "keycloak");
        modelBuilder.Entity<Resource>().ToTable("Resource", "keycloak");
        modelBuilder.Entity<Claim>().ToTable("Claim", "appshell");
        modelBuilder.Entity<Institution>().ToTable("Institution", "appshell");
        modelBuilder.Entity<OrgUnit>().ToTable("OrgUnit", "appshell");
        modelBuilder.Entity<App>().ToTable("App", "appshell");
        modelBuilder.Entity<UserMenuAssignment>().ToTable("UserMenuAssignment", "appshell");

        modelBuilder.Entity<UserMenuAssignment>()
            .HasOne(ua => ua.MenuItem)
            .WithMany()
            .HasForeignKey(ua => ua.MenuItemId);
    }
}
