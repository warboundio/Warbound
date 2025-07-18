using System.Threading;
using System.Threading.Tasks;
using Core.Discords;
using Core.ETL;
using Core.Logs;

// creating main branch
Logging.Configure();
_ = ETLRunner.RunLoopAsync();

DiscordBot bot = new();
_ = bot.StartAsync();

await Task.Delay(Timeout.Infinite);
