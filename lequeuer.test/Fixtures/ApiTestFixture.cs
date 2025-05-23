using lequeuer.api;
using lequeuer.api.Data;
using lequeuer.test.Fixtures;
using lequeuer.test.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;

[assembly:AssemblyFixture(typeof(ApiTestFixture))]

namespace lequeuer.test.Fixtures;

public class ApiTestFixture : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    private readonly MySqlContainer _mysqlContainer =
        new MySqlBuilder()
            .WithDatabase("lequeuer-test")
            .WithPassword("root")
            .WithUsername("root")
            .Build();

    public HttpClient HttpClient { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DataContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseMySql(
                    _mysqlContainer.GetConnectionString(),
                    ServerVersion.AutoDetect(_mysqlContainer.GetConnectionString()
                    ));
            });

            var db = services.BuildServiceProvider().GetRequiredService<DataContext>();
            db.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }
    
    public async ValueTask InitializeAsync()
    {
        await _mysqlContainer.StartAsync();
        HttpClient = CreateClient();
        await new DatabaseSeeder(Services).SeedAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await _mysqlContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}