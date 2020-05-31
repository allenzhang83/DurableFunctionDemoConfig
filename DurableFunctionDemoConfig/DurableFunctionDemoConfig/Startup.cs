using DurableFunctionDemoConfig.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: FunctionsStartup(typeof(DurableFunctionDemoConfig.Startup))]
namespace DurableFunctionDemoConfig
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var existingConfig = serviceProvider.GetService<IConfiguration>();

            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(existingConfig)
                .AddEnvironmentVariables();

            var builtConfig = configBuilder.Build();
            var appConfigConnectionString = builtConfig["AppConfigurationConnectionString"];
            var hostingEnv = builtConfig["HostingEnv"];

            configBuilder.AddAzureAppConfiguration(options =>
                options
                    .Connect(appConfigConnectionString)
                    .Select(KeyFilter.Any, hostingEnv)
            );
            builtConfig = configBuilder.Build();
            builder.Services.Replace(new ServiceDescriptor(typeof(IConfiguration), builtConfig));

            builder.Services.AddSingleton<IGitHubApiService, GitHubApiService>();
        }
    }
}
