using System.Threading.Tasks;

namespace Data.ETLs;

public class ItemExpansionETLTests
{
    [Fact]
    public void ItShouldHaveStaticRunAsyncMethod()
    {
        // Arrange
        var method = typeof(ItemExpansionETL).GetMethod("RunAsync");
        
        // Assert
        Assert.NotNull(method);
        Assert.True(method.IsStatic);
        Assert.Equal(typeof(Task), method.ReturnType);
    }
}