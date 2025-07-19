using Core.GitHub;

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
        Assert.True(method.IsStatic);
        
        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal("title", parameters[0].Name);
        Assert.Equal("body", parameters[1].Name);
        Assert.Equal(typeof(string), parameters[0].ParameterType);
        Assert.Equal(typeof(string), parameters[1].ParameterType);
    }

    [Fact]
    public void ItShouldBeStaticClass()
    {
        Type serviceType = typeof(GitHubIssueService);
        Assert.True(serviceType.IsAbstract && serviceType.IsSealed);
    }
}