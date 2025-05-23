using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace lequeuer.api.Configuration;

public static class ExceptionHandlerConfigurator
{
    public static void ConfigureProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(opts =>
        {
            opts.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("tradeId", activity?.Id);
            };
        });

        builder.Services.AddExceptionHandler<ProblemExceptionHandler>();
    }
}

[Serializable]
public class ProblemException(string error, string message) : Exception(message)
{
    public string Error { get; } = error;
    public new string Message { get; } = message;
}

[Serializable]
public class ValidationProblemException(string error, string message) : Exception(message)
{
    public string Error { get; } = error;
    public new string Message { get; } = message;
}

public class ProblemExceptionHandler(IProblemDetailsService problemDetailsService): IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ProblemException problemException => new()
            {
                Status = StatusCodes.Status409Conflict,
                Title = problemException.Error,
                Detail = problemException.Message,
                Type = "Conflict"
            },
            ValidationProblemException validationProblemException => new()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = validationProblemException.Error,
                Detail = validationProblemException.Message,
                Type = "Bad Request"
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = exception.Message,
                Type = "Internal Server Error"
            }
        };
        
        httpContext.Response.StatusCode = problemDetails.Status!.Value;
            
        return await problemDetailsService.TryWriteAsync(new()
        {
            HttpContext = httpContext, 
            ProblemDetails = problemDetails
        });
    }
}