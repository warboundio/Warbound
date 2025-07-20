# LuaPublisher Usage Examples

The `LuaPublisher` utility can be called from anywhere in the codebase to publish LUA addon files.

## Basic Usage

```csharp
using Addon;

// Simple publish - returns true if successful, false otherwise
bool success = LuaPublisher.Publish();

if (success)
{
    Console.WriteLine("LUA files published successfully!");
}
else
{
    Console.WriteLine("Failed to publish LUA files.");
}
```

## In Web Controllers

```csharp
[HttpPost("publish-addon")]
public IActionResult PublishAddon()
{
    bool result = LuaPublisher.Publish();
    return result ? Ok("Published successfully") : BadRequest("Publish failed");
}
```

## In Background Services

```csharp
public class AddonPublishService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Publish addon files every hour
            LuaPublisher.Publish();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
```

## During Build Process

```csharp
// Can be called from MSBuild targets or build scripts
public static void Main(string[] args)
{
    Console.WriteLine("Building project...");
    // ... build logic ...
    
    Console.WriteLine("Publishing LUA files...");
    LuaPublisher.Publish();
}
```

The utility automatically:
- Finds the `Addon/LUA` source directory
- Creates target directories if they don't exist
- Copies all files preserving directory structure
- Handles errors gracefully
- Provides detailed logging output