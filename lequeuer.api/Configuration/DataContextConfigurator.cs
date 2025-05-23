using lequeuer.api.Data;
using lequeuer.api.Utils;

using Microsoft.EntityFrameworkCore;

namespace lequeuer.api.Configuration;

public static class DataContextConfigurator
{
    public static void ConfigureDataContext(this WebApplicationBuilder builder, AppSettings settings)
    {
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options
                .UseMySql(settings.ConnectionString, ServerVersion.AutoDetect(settings.ConnectionString))
                .UseSnakeCaseNamingConvention();
        });
    }
    
    public static void MigrateAndSeedDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.Migrate();
        new Seeder(context).Seed();
    }
}