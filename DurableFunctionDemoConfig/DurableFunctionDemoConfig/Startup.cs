using DurableFunctionDemoConfig.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(DurableFunctionDemoConfig.Startup))]
namespace DurableFunctionDemoConfig
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IGitHubApiService, GitHubApiService>();
        }
    }
}
