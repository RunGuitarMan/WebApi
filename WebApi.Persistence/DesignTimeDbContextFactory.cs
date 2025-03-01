using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(ApplicationDbContext.ConnectionString, builder =>
        {
            builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ApplicationDbContext.SchemaName);
        });
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}