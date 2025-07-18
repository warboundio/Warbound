using System.Threading;
using System.Threading.Tasks;
using Core.Discords;
using Core.ETL;
using Core.Logs;
using ETL.ETLs.Validation;

// creating main branch
Logging.Configure();
_ = ETLRunner.RunLoopAsync();

DiscordBot bot = new();
_ = bot.StartAsync();

SchemaValidationETL.RunAsync();

await Task.Delay(Timeout.Infinite);
