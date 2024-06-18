// Configuration/ConfigCatExtensions.cs
using ConfigCat.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigCatInDotnetSample.Configuration
{
    public static class ConfigCatExtensions
    {
        public static IConfigurationBuilder AddConfigCat(this IConfigurationBuilder builder, string sdkKey, TimeSpan pollInterval)
        {
            var configSource = new ConfigCatConfigurationSource(sdkKey, pollInterval);
            builder.Add(configSource);
            return builder;
        }

        public static IServiceCollection ConfigureConfigCat(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FeatureSet>(configuration.GetSection("FeatureSet"));
            return services;
        }
    }

    public class ConfigCatConfigurationSource : IConfigurationSource
    {
        private readonly string _sdkKey;
        private readonly TimeSpan _pollInterval;

        public ConfigCatConfigurationSource(string sdkKey, TimeSpan pollInterval)
        {
            _sdkKey = sdkKey;
            _pollInterval = pollInterval;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new ConfigCatConfigurationProvider(_sdkKey, _pollInterval);
    }

    public class ConfigCatConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IConfigCatClient _client;
        private readonly Timer _timer;
        private readonly ILogger<ConfigCatConfigurationProvider> _logger;

        public ConfigCatConfigurationProvider(string sdkKey, TimeSpan pollInterval)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<ConfigCatConfigurationProvider>();

            _client = ConfigCatClient.Get(sdkKey, options =>
            {
                options.PollingMode = PollingModes.ManualPoll;
                options.Logger = new ConsoleLogger(ConfigCat.Client.LogLevel.Warning);
            });

            // Initial data load
            LoadAsync().Wait(); // Block until initial load completes
            _timer = new Timer(async _ => await RefreshConfigAsync(), null, pollInterval, pollInterval);
        }

        private async Task RefreshConfigAsync()
        {
            await LoadAsync();
        }

        public override void Load()
        {
            LoadAsync().Wait(); // Block until load completes
        }

        public async Task LoadAsync()
        {
            var config = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                await _client.ForceRefreshAsync(); // Ensure we fetch the latest config

                var allKeys = await _client.GetAllKeysAsync();
                _logger.LogInformation($"All keys from ConfigCat: {string.Join(", ", allKeys)}");

                foreach (var key in allKeys)
                {
                    // Fetching the value with correct default type
                    if (key.Equals("GrandFeature", StringComparison.OrdinalIgnoreCase))
                    {
                        var value = await _client.GetValueAsync(key, false); // Correct boolean default value
                        _logger.LogInformation($"Fetched raw value for {key}: {value}");
                        config[$"FeatureSet:{key}"] = value.ToString();
                    }
                    else
                    {
                        var value = await _client.GetValueAsync(key, string.Empty); // Default for non-boolean
                        _logger.LogInformation($"Fetched raw value for {key}: {value}");
                        config[$"FeatureSet:{key}"] = value;
                    }
                }
                Data = config;
                OnReload();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading configuration from ConfigCat.");
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _client.Dispose();
        }
    }
}
