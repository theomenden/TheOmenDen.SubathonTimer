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

// Setup Serilog for structured logging

try
{
    var host = new HostBuilder()
        .ConfigureAppConfiguration((context, config) =>
        {
            var builtConfig = config.Build();
            var keyVaultUri = new Uri("https://clientsecurevault.vault.azure.net/");

            config.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential(), new AzureKeyVaultConfigurationOptions());
        })
        .ConfigureFunctionsWebApplication() // Required for isolated model
        .ConfigureServices((context, services) =>
        {
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();

            var config = context.Configuration;

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
#if DEBUG
                .WriteTo.ApplicationInsights(config["APPINSIGHTS_INSTRUMENTATIONKEY"] ?? string.Empty, TelemetryConverter.Traces)
#endif
                .CreateLogger();

            services.AddLogging(lb => lb.AddSerilog(Log.Logger, true));

            // Strongly typed Twitch config
            services.Configure<TwitchSettings>(options =>
            {
                options.ClientId = config["CorvidOnlineTwitchClientId"] ?? string.Empty;
                options.ClientSecret = config["CorvidOnlineTwitchSecret"] ?? string.Empty;
                options.SigningSecret = config["CorvidOnlineTwitchSigningKey"] ?? string.Empty;
            });

            // Main Twitch API Client
            services.AddHttpClient(TwitchConstants.TwitchHttpClientName, client =>
            {
                client.BaseAddress = new Uri(TwitchConstants.TwitchBaseUri);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
                client.DefaultRequestHeaders.Add(TwitchConstants.ClientIdHeader, config["CorvidOnlineTwitchClientId"]);
            })
            .AddStandardResilienceHandler();

            // OAuth HTTP Client
            services.AddHttpClient(TwitchConstants.TwitchOAuthHttpClientName, client =>
            {
                client.BaseAddress = new Uri(TwitchConstants.TwitchOAuthBaseUri);
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
                options.Retry.MaxRetryAttempts = 2;

                // Must be >= 2x AttemptTimeout
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(70);
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 3;
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://subathon.corvid.online")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        })
        .Build();

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception during host startup.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
