using lequeuer.api.Data;

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
}