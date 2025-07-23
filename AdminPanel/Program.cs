using Addon;
using AdminPanel.Components;
using Core;
using Core.Discords;
using Core.ETL;
using Core.GitHub;
using Core.Logs;
using Core.Services;
using Core.Tools;
using Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddDbContext<CoreContext>();

builder.Services.AddSingleton<GitHubIssueMonitor>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<GitHubIssueMonitor>());

builder.Services.AddSingleton<DraftService>();

builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});


WebApplication app = builder.Build();
GitHubIssueService.Monitor = app.Services.GetRequiredService<GitHubIssueMonitor>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

WarcraftData.Instance.Load();
if (!BuildConfig.IsDebug)
{
    LUAPublisher.Publish();
    Logging.Configure();

    DiscordBot bot = new();
    _ = bot.StartAsync();

    _ = ETLRunner.RunLoopAsync();
    AutoPublisher.Boot();
    AutoPublisher.BootSimulatedClient();
}

app.Run();
