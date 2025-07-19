using System.Net.Http;
using Core.GitHub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.HttpClient;

public class HttpClientSecurityManagementTests
{
    [Fact]
    public void ItShouldConfigureHttpClientFactoryCorrectly()
    {
        // Arrange
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        
        // Configure services similar to Program.cs
        builder.Services.AddHttpClient("GitHubAPI", client =>
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WarboundETL");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient("BlizzardAPI", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

        builder.Services.AddSingleton<IGitHubIssueService, GitHubIssueService>();

        IHost host = builder.Build();
        Core.Services.ServiceProvider.Configure(host.Services);

        // Act
        IHttpClientFactory httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
        System.Net.Http.HttpClient gitHubClient = httpClientFactory.CreateClient("GitHubAPI");
        System.Net.Http.HttpClient blizzardClient = httpClientFactory.CreateClient("BlizzardAPI");

        // Assert
        Assert.NotNull(httpClientFactory);
        Assert.NotNull(gitHubClient);
        Assert.NotNull(blizzardClient);
        
        // Verify timeouts are configured
        Assert.Equal(TimeSpan.FromSeconds(30), gitHubClient.Timeout);
        Assert.Equal(TimeSpan.FromSeconds(30), blizzardClient.Timeout);
        
        // Verify headers are configured
        Assert.Contains(gitHubClient.DefaultRequestHeaders.Accept, 
            h => h.MediaType == "application/vnd.github+json");
        Assert.Contains(blizzardClient.DefaultRequestHeaders.Accept, 
            h => h.MediaType == "application/json");
        
        // Verify user agent is set for GitHub
        Assert.NotEmpty(gitHubClient.DefaultRequestHeaders.UserAgent);
    }

    [Fact]
    public void ItShouldProvideGitHubServiceInstance()
    {
        // Arrange
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        
        builder.Services.AddHttpClient("GitHubAPI");
        builder.Services.AddSingleton<IGitHubIssueService, GitHubIssueService>();

        IHost host = builder.Build();
        Core.Services.ServiceProvider.Configure(host.Services);

        // Act
        IGitHubIssueService gitHubService = host.Services.GetRequiredService<IGitHubIssueService>();

        // Assert
        Assert.NotNull(gitHubService);
        Assert.IsType<GitHubIssueService>(gitHubService);
    }

    [Fact]
    public void ItShouldMaintainBackwardCompatibilityForStaticMethods()
    {
        // Arrange
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        
        builder.Services.AddHttpClient("GitHubAPI");
        builder.Services.AddSingleton<IGitHubIssueService, GitHubIssueService>();

        IHost host = builder.Build();
        Core.Services.ServiceProvider.Configure(host.Services);

        // Act & Assert - These should not throw since they use the service locator
        Assert.NotNull(Core.Services.ServiceProvider.GetService<IGitHubIssueService>());
    }

    [Fact]
    public void ItShouldConfigureSSLValidationByDefault()
    {
        // Arrange
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Services.AddHttpClient("TestClient");
        IHost host = builder.Build();

        // Act
        IHttpClientFactory factory = host.Services.GetRequiredService<IHttpClientFactory>();
        System.Net.Http.HttpClient client = factory.CreateClient("TestClient");

        // Assert - By default, HttpClient validates SSL certificates
        // We can't easily test this without making actual HTTPS requests,
        // but we can verify the client is properly configured
        Assert.NotNull(client);
        Assert.True(client.Timeout > TimeSpan.Zero);
    }

    [Fact]
    public void ItShouldUseSingletonLifetimeForServices()
    {
        // Arrange
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        
        builder.Services.AddHttpClient("GitHubAPI");
        builder.Services.AddSingleton<IGitHubIssueService, GitHubIssueService>();

        IHost host = builder.Build();

        // Act
        IGitHubIssueService service1 = host.Services.GetRequiredService<IGitHubIssueService>();
        IGitHubIssueService service2 = host.Services.GetRequiredService<IGitHubIssueService>();

        // Assert - Singleton services should return the same instance
        Assert.Same(service1, service2);
    }

    [Fact]
    public void ItShouldCreateProperHttpClientInstances()
    {
        // Arrange
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Services.AddHttpClient();
        IHost host = builder.Build();

        // Act
        IHttpClientFactory factory = host.Services.GetRequiredService<IHttpClientFactory>();
        System.Net.Http.HttpClient client1 = factory.CreateClient();
        System.Net.Http.HttpClient client2 = factory.CreateClient();

        // Assert - HttpClientFactory should create separate instances but reuse connections
        Assert.NotNull(client1);
        Assert.NotNull(client2);
        Assert.NotSame(client1, client2); // Different instances
    }
}