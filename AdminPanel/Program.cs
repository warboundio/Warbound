using AdminPanel.Components;
using Core;
using Core.GitHub;
using Core.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register CoreContext for dependency injection
builder.Services.AddDbContext<CoreContext>();

// Register GitHubIssueMonitor as hosted service and singleton for access
builder.Services.AddSingleton<GitHubIssueMonitor>();
builder.Services.AddHostedService<GitHubIssueMonitor>(provider => provider.GetRequiredService<GitHubIssueMonitor>());

// Register DraftService as singleton
builder.Services.AddSingleton<DraftService>();

WebApplication app = builder.Build();

// Set up the monitor reference for GitHubIssueService
GitHubIssueService.Monitor = app.Services.GetRequiredService<GitHubIssueMonitor>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
