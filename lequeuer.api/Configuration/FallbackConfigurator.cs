using Microsoft.Extensions.FileProviders.Physical;

namespace lequeuer.api.Configuration;

public static class FallbackConfigurator
{
    public static void MapCustomFallbackToFile(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (!context.Request.Path.StartsWithSegments("/api") && !context.Request.Path.StartsWithSegments("/hubs"))
            {
                var fileInfo = new PhysicalFileInfo(new(Path.Combine(app.Environment.WebRootPath, "browser", "index.html")));
                await context.Response.SendFileAsync(fileInfo);
                return;
            }
            await next();
        });
    }
}