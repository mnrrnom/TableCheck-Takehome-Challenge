using Microsoft.Extensions.FileProviders;

namespace lequeuer.api.Configuration;

public static class StaticFileServerConfigurator
{
    public static void UseStaticFileServer(this WebApplication app)
    {
        if (app.Environment.IsDevelopment()) return;
        var webRootPath = app.Environment.WebRootPath;
        var browserPath = Path.Combine(webRootPath, "browser");

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(browserPath)
        });

        app.UseDefaultFiles(new DefaultFilesOptions
        {
            FileProvider = new PhysicalFileProvider(browserPath)
        });
    }
}