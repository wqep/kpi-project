using App.Telegram.Handlers;
using Lib.Infrastructure.Database;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace App;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("⏳ BOT LAUNCHING...");

        Console.WriteLine("⚙️ CHECKING DATABASES...");
        var dbInit = new DbInitializer();
        dbInit.Initialize();
        Console.WriteLine("✅ DATA BASES INITED!");

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
                Console.WriteLine($"❌ Error: {exception}");
                return Task.CompletedTask;
            },
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMe();
        Console.WriteLine($"🚀 BOT @{me.Username} STARTED NAD WORKING PROPERLY");
        Console.WriteLine("PRESS ENTER TO STOP....\n");
        
        Console.ReadLine();
        cts.Cancel();
    }
}