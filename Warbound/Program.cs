using System.Threading;
using System.Threading.Tasks;
using Core.ETL;
using Core.Logs;
using ETL.ETLs;

Logging.Configure();
_ = ETLRunner.RunLoopAsync();

//DiscordBot bot = new();
//_ = bot.StartAsync();

_ = RecipeIndexETL.RunAsync();

await Task.Delay(Timeout.Infinite);
