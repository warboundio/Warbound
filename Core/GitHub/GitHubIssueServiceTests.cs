using Core.GitHub;

namespace Core.GitHub;

public class GitHubIssueServiceTests
{
    [Fact]
    public void ItShouldHaveCreateTestIssueMethod()
    {
        GitHubIssueService service = new();
        
        // Verify the method exists and returns Task<int>
        Type serviceType = typeof(GitHubIssueService);
        var method = serviceType.GetMethod("CreateTestIssue");
        
        Assert.NotNull(method);
        Assert.Equal(typeof(Task<int>), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    [Fact]
    public void ItShouldHaveCreateMethodWithProperSignature()
    {
        GitHubIssueService service = new();
        
        // Verify the Create method exists with proper signature
        Type serviceType = typeof(GitHubIssueService);
        var method = serviceType.GetMethod("Create", new[] { typeof(string), typeof(string) });
        
        Assert.NotNull(method);
        Assert.Equal(typeof(Task<int>), method.ReturnType);
        
        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal("title", parameters[0].Name);
        Assert.Equal("body", parameters[1].Name);
        Assert.Equal(typeof(string), parameters[0].ParameterType);
        Assert.Equal(typeof(string), parameters[1].ParameterType);
    }
}