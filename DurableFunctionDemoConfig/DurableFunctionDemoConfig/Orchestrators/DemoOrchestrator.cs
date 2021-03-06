﻿using DurableFunctionDemoConfig.ActivityFunctions;
using DurableFunctionDemoConfig.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionDemoConfig.Orchestrators
{
    public class DemoOrchestrator
    {
        [FunctionName(nameof(DemoOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var repos = await context.CallActivityAsync<List<string>>(nameof(GetUserRepositoryList), null);
            var list = string.Join(',', repos);
            log.LogInformation($"Repository list: {list}");

            var tasks = new List<Task<RepoViewCount>>();

            // fan-out
            foreach (var repo in repos)
            {
                var task = context.CallActivityAsync<RepoViewCount>(nameof(GetRepositoryViewCount), repo);
                tasks.Add(task);
            }

            // fan-in
            var repoViewCounts = await Task.WhenAll(tasks);

            await context.CallActivityAsync(nameof(ReportRepoViewCount), repoViewCounts);
        }
    }
}
