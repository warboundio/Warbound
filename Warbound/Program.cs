using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Core.ETL;
using Core.GitHub;
using Core.Logs;
using Core.Settings;
using ETL.BlizzardAPI.General;
using ETL.ETLs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

// Configure logging
Logging.Configure();

// Create host with dependency injection
HostApplicationBuilder builder = Host.CreateApplicationBuilder();

// Configure HTTP client factory with policies
builder.Services.AddHttpClient("GitHubAPI", client =>
{
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationSettings.Instance.GithubToken);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
    client.DefaultRequestHeaders.UserAgent.ParseAdd("WarboundETL");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("BlizzardAPI", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Register services
builder.Services.AddSingleton<IGitHubIssueService, GitHubIssueService>();
builder.Services.AddSingleton<IBlizzardTokenProvider, BlizzardTokenProvider>();
builder.Services.AddSingleton<IBlizzardAPIRouter, BlizzardAPIRouterImplementation>();

IHost host = builder.Build();

// Configure the service provider for static access
Core.Services.ServiceProvider.Configure(host.Services);

// Get services from DI container
IGitHubIssueService gitHubService = host.Services.GetRequiredService<IGitHubIssueService>();
IBlizzardTokenProvider blizzardTokenProvider = host.Services.GetRequiredService<IBlizzardTokenProvider>();

_ = ETLRunner.RunLoopAsync();

//DiscordBot bot = new();
//_ = bot.StartAsync();

//_ = gitHubService.Create("Another Test", "Another Body");

_ = RecipeETL.RunAsync();

await Task.Delay(Timeout.Infinite);
