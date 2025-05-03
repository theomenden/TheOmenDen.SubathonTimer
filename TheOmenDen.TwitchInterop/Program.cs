using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System.Net.Http.Headers;
using System.Net.Mime;
using TheOmenDen.TwitchInterop.Models;
using TheOmenDen.TwitchInterop.Services;

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
    var host = new HostBuilder()
        .ConfigureFunctionsWebApplication()
        .ConfigureAppConfiguration(config =>
        {
            var keyVaultUri = new Uri(Environment.GetEnvironmentVariable("AzureKeyVault__VaultUri"));
            config.AddAzureKeyVault(
                keyVaultUri,
                new DefaultAzureCredential(),
                new AzureKeyVaultConfigurationOptions());

        })
        .ConfigureFunctionsWorkerDefaults()
        .UseSerilog(Log.Logger)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();
            services.AddOptions();

            var configuration = hostContext.Configuration;

            services.Configure<TwitchSettings>(options =>
            {
                options.ClientId = configuration["CorvidOnlineTwitchClientId"] ?? string.Empty;
                options.ClientSecret = configuration["CorvidOnlineTwitchSecret"] ?? string.Empty;
                options.SigningSecret = configuration["CorvidOnlineTwitchSigningKey"] ?? string.Empty;
            });


            services.AddHttpClient(TwitchConstants.TwitchHttpClientName, client =>
            {
                client.BaseAddress = new Uri(TwitchConstants.TwitchBaseUri);
                client.DefaultRequestHeaders.Add(TwitchConstants.ClientIdHeader, configuration["CorvidOnlineTwitchClientId"]);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
            })
            .AddStandardResilienceHandler();

            services.AddHttpClient(TwitchConstants.TwitchOAuthHttpClientName, client =>
                {
                    client.BaseAddress = new Uri(TwitchConstants.TwitchOAuthBaseUri);
                    client.DefaultRequestHeaders.Accept.Add(
                        MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
                    client.Timeout = TimeSpan.FromSeconds(30);
                })
                .AddStandardResilienceHandler(options =>
                {
                    // Controls timeout for each attempt
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);

                    // Optional: controls cumulative max time across retries
                    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);

                    options.Retry.MaxRetryAttempts = 2;

                    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(35);
                    options.CircuitBreaker.FailureRatio = 0.5;
                    options.CircuitBreaker.MinimumThroughput = 3;
                    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
                });

            services.AddSingleton<ITwitchUserMappingService, TwitchUserMappingService>();
        })
        .Build();

    host.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred while starting the host.");
}
finally
{
    Log.CloseAndFlush();
}
