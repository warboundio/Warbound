using System.Threading;
using System.Threading.Tasks;
using Core.Discords;
using Core.ETL;
using Core.Logs;
using ETL.BlizzardAPI.Endpoints;
using ETL.ETLs;

Logging.Configure();
_ = ETLRunner.RunLoopAsync();

//DiscordBot bot = new();
//_ = bot.StartAsync();

//_ = GitHubIssueService.Create("Another Test", "Another Body");

_ = RecipeETL.RunAsync();

await Task.Delay(Timeout.Infinite);
