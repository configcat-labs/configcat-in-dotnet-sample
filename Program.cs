using FeatureFlagsInDotNetSample.Components;
using ConfigCat.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var configCatSdkKey = builder.Configuration["ConfigCatSdkKey"];

// Register ConfigCatClient as a singleton service so you can inject it in your controllers, actions, etc.
builder.Services.AddSingleton<IConfigCatClient>(sp =>
{
    return ConfigCatClient.Get(configCatSdkKey, options =>
    {
        options.PollingMode = PollingModes.LazyLoad(cacheTimeToLive: TimeSpan.FromSeconds(10));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
