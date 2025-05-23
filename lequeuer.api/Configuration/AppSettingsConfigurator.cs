namespace lequeuer.api.Configuration;

public static class AppSettingsConfigurator
{
    public static AppSettings ConfigureAppSettings(this WebApplicationBuilder builder)
    {
        AppSettings settings = new();
        var configSection = builder.Configuration.GetSection(nameof(AppSettings));
        configSection.Bind(settings);
        builder.Services.Configure<AppSettings>(configSection);
        return settings;
    }
}