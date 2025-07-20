using System.Threading;
using System.Threading.Tasks;
using Core.Logs;

Logging.Configure();
//_ = ETLRunner.RunLoopAsync();

//DiscordBot bot = new();
//_ = bot.StartAsync();

await Task.Delay(Timeout.Infinite);
