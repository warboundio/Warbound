using Core.GitHub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Moq;

namespace Core.GitHub;

public class GitHubIssueServiceTests
{
    [Fact]
    public void ItShouldHaveCreateMethodWithProperSignature()
    {
        // Verify the Create method exists with proper signature
        Type serviceType = typeof(GitHubIssueService);
        var method = serviceType.GetMethod("Create", new[] { typeof(string), typeof(string) });
        
        Assert.NotNull(method);
        Assert.Equal(typeof(Task<int>), method.ReturnType);
        Assert.False(method.IsStatic);
        
        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal("title", parameters[0].Name);
        Assert.Equal("body", parameters[1].Name);
        Assert.Equal(typeof(string), parameters[0].ParameterType);
        Assert.Equal(typeof(string), parameters[1].ParameterType);
    }

    [Fact]
    public void ItShouldImplementInterface()
    {
        Type serviceType = typeof(GitHubIssueService);
        Type interfaceType = typeof(IGitHubIssueService);
        
        Assert.True(interfaceType.IsAssignableFrom(serviceType));
    }
    
    [Fact]
    public void ItShouldRequireHttpClientFactory()
    {
        // Verify constructor requires IHttpClientFactory
        var constructors = typeof(GitHubIssueService).GetConstructors();
        var constructor = constructors.FirstOrDefault(c => c.GetParameters().Length == 1);
        
        Assert.NotNull(constructor);
        var parameter = constructor.GetParameters()[0];
        Assert.Equal(typeof(IHttpClientFactory), parameter.ParameterType);
    }
}