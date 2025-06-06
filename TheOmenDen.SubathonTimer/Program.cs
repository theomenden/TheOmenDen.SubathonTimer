using BlazorDexie.Extensions;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using TheOmenDen.SubathonTimer;
using TheOmenDen.SubathonTimer.Models;
using TheOmenDen.SubathonTimer.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProcessName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Async(a =>
    {
        a.Console(theme: AnsiConsoleTheme.Code);
        a.Debug(new CompactJsonFormatter());
    })
    .CreateLogger();
try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    builder.Services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddSerilog(new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithMemoryUsage()
            .WriteTo.Async(a =>
            {
                a.Console(theme: AnsiConsoleTheme.Code);
                a.Debug(new CompactJsonFormatter());
            })
            .CreateLogger(), dispose: true);
    });
    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
    builder.Services.AddSingleton<ITimerService, TimerService>();

    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    builder.Services.AddFluentUIComponents();
    builder.Services.AddBlazoredLocalStorage();
    builder.Services.AddBlazorDexie();


    builder.Services.AddScoped<TwitchUserState>();

    builder.Services.AddHttpClient(TwitchConstants.TwitchBackend, client =>
        {
            client.BaseAddress = new Uri("https://twitchinterop-g9h9b6fyh8f3f0ga.eastus2-01.azurewebsites.net/api/");
        })
    .AddStandardResilienceHandler();
    builder.Services.AddScoped<ITwitchAuthService, TwitchAuthService>();

    builder.Services.AddMsalAuthentication(options =>
    {
        builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
        options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
    });

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    await Log.CloseAndFlushAsync();
}
