using App.Telegram.Handlers;
using Lib.Infrastructure.Database;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Serilog;
using Serilog.Formatting.Compact;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: "logs/bot-log.clef",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Ініціалізація бази даних та токена...");
            var dbManager = new DatabaseManager();

            string tokenPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Telegram", "Token.txt");
            string botToken = await File.ReadAllTextAsync(tokenPath);

            var botClient = new TelegramBotClient(botToken.Trim());
            BotManager botManager = new BotManager(botClient);

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: botManager.HandleUpdateAsync,
                errorHandler: ErrorHandler,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMe();

            Log.Information("Bot started successfully: @{Username}", me.Username);
            Console.WriteLine("Press enter to stop...");
            Console.ReadLine();

            Log.Information("Stoping bot...");
            await cts.CancelAsync();
        }
        catch (FileNotFoundException ex)
        {
            Log.Fatal(ex, "Fatal error while loading a token!");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Fatal error while program have been working!");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Log.Error(exception, "Telegram API error: {Message}", exception.Message);
        return Task.CompletedTask;
    }
}