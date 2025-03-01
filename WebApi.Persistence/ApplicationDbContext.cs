using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain;

namespace WebApi.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public const string ConnectionString =
        "Host=localhost;Port=5432;Database=example_db;Username=example_user;Password=example_password";
    public const string SchemaName = "example_schema";
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(ConnectionString);
    
    public DbSet<TestEntity> TestEntities { get; internal set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        modelBuilder.HasDefaultSchema(SchemaName);
    }
}
