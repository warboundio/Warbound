using System.Threading;
using System.Threading.Tasks;
using Core.Discords;
using Core.ETL;
using Core.Logs;
using Data.ETLs;

Logging.Configure();
_ = ETLRunner.RunLoopAsync();

DiscordBot bot = new();
_ = bot.StartAsync();

_ = RecipeETL.RunAsync();

await Task.Delay(Timeout.Infinite);
