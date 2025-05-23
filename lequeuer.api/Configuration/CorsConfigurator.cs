namespace lequeuer.api.Configuration;

public static class CorsConfigurator
{
    private const string _localCorsKey = "localCorsKey";
    private const string _productionCorsKey = "productionCorsKey";

    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy(_localCorsKey, b =>
            {
                b.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });

            opt.AddPolicy(_productionCorsKey, b =>
            {
                b.WithOrigins("https://dev.azalea.life").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });
        });
    }

    public static void UseConfiguredCors(this WebApplication app)
    {
        var env = app.Environment;
        app.UseCors(env.IsProduction() ? _productionCorsKey : _localCorsKey);
    }
}