using App.Telegram.Handlers;
using Lib.Infrastructure.Database;
using Serilog;
using Serilog.Formatting.Compact;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace App;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: "logs/app-log.clef",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("⏳ BOT LAUNCHING...");

            Log.Information("⚙️ CHECKING DATABASES...");
            var dbInit = new DbInitializer();
            dbInit.Initialize();

            string token = await File.ReadAllTextAsync("Token.txt");
            var botClient = new TelegramBotClient(token.Trim());

            var botManager = new BotManager(botClient);

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: botManager.HandleUpdateAsync,
                errorHandler: (ITelegramBotClient client, Exception exception, CancellationToken ct) => 
                {
                    Console.WriteLine();
                    Log.Error($"❌ Error: {exception}");
                    Console.Write("\nPRESS ENTER TO STOP...");
                    return Task.CompletedTask;
                },
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMe();
            Log.Information($"🚀 BOT @{me.Username} STARTED AND WORKING PROPERLY");
            Console.Write("\nPRESS ENTER TO STOP...");
        
            Console.ReadLine();
            cts.Cancel();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Fatal exception have occured");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}