using Microsoft.Extensions.Options;
using ConfigCatInDotnetSample.Configuration;
using ConfigCat.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add ConfigCat configuration
builder.Configuration.AddConfigCat(builder.Configuration["ConfigCat:Key"],
    TimeSpan.Parse(builder.Configuration["ConfigCat:PollInterval"]));

// Configure services to use ConfigCat settings
builder.Services.ConfigureConfigCat(builder.Configuration);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseHttpsRedirection();

// Define a route that handles GET requests to /api/feature
app.MapGet("/api/feature", async (HttpContext context, IOptionsSnapshot<FeatureSet> features) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    try
    {
        var featureValue = features.Value.GrandFeature;
        logger.LogInformation($"GrandFeature is set to: {featureValue}");

        if (featureValue)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("feature flag's on");
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("feature flag's off");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while retrieving the feature flag value.");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Internal Server Error");
    }
});

app.Run();

public class FeatureSet
{
    public bool GrandFeature { get; set; }
}