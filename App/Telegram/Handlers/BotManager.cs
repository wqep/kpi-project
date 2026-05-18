using Lib.Core.BaseClasses;
using Lib.Core.Enums;
using Lib.Core.Services;
using Lib.Infrastructure.CharacterFactory;
using Lib.Infrastructure.Database;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace App.Telegram.Handlers;

public class BotManager
{
    private readonly DatabaseManager _dbManager;
    private readonly MapRuler _mapRuler;
    private readonly GameRuler _gameRuler;
    private readonly ITelegramBotClient _botClient;

    public BotManager(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _dbManager = new DatabaseManager();
        _mapRuler = new MapRuler(_dbManager);
        _gameRuler = new GameRuler(_dbManager);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            long id = update.CallbackQuery.From.Id;
            string data = update.CallbackQuery.Data;

            Character? hero = _dbManager.GetActiveCharacter(id);

            if (hero != null && hero.State == 1)
            {
                if (data.StartsWith("move_to:") || data.StartsWith("action_loot:"))
                {
                    await botClient.AnswerCallbackQuery(
                        callbackQueryId: update.CallbackQuery.Id, 
                        text: "⚔️ You're in fight, you cannot run", 
                        showAlert: true);
                    return; 
                }
            }

            await botClient.AnswerCallbackQuery(update.CallbackQuery.Id);

            if (data.StartsWith("class_"))
            {
                _dbManager.EnsureUserExists(id, update.CallbackQuery.From.Username ?? "Unknown");
                
                hero = CharacterFactory.CreateFromClass(id, data);
                hero.Id = _dbManager.SaveCharacter(hero); 
                
                _mapRuler.GenerateStarterDungeon(hero.Id, id);
                
                hero = _dbManager.GetActiveCharacter(id); 
                if (hero != null) await ShowRoom(id, hero);
            }
            else if (data.StartsWith("move_to:"))
            {
                int targetId = int.Parse(data.Split(':')[1]);
                _mapRuler.MovePlayer(id, targetId);
                
                hero = _dbManager.GetActiveCharacter(id);
                if (hero != null) await ShowRoom(id, hero);
            }
            else if (data.StartsWith("action_loot:"))
            {
                int roomId = int.Parse(data.Split(':')[1]);
                if (hero != null)
                {
                    string lootMsg = _gameRuler.ProcessLooting(hero, roomId);
                    await botClient.SendMessage(id, lootMsg, parseMode: ParseMode.Markdown);
                    await ShowRoom(id, hero);
                }
            }
            return;
        }

        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            long id = update.Message.From.Id;
            if (update.Message.Text == "/start")
            {
                var menu = new InlineKeyboardMarkup(new[] {
                    new [] { InlineKeyboardButton.WithCallbackData("Warrior", "class_warrior"), 
                             InlineKeyboardButton.WithCallbackData("Thief", "class_thief"),
                             InlineKeyboardButton.WithCallbackData("Paladin",  "class_Paladin")
                    }
                });
                await botClient.SendMessage(id, "Choose class:", replyMarkup: menu);
            }
            else
            {
                Character? hero = _dbManager.GetActiveCharacter(id);
                if (hero != null) await ShowRoom(id, hero);
            }
        }
    }

    private async Task ShowRoom(long chatId, Character hero)
    {
        var room = _dbManager.GetRoom(hero.CurrentRoomId);
        if (room == null) return;

        var inv = _dbManager.GetInventory(hero.Id);
        string bag = inv.Count > 0 ? string.Join(", ", inv) : "Empty";

        string icon = room.Type switch { RoomType.Loot => "🎁", RoomType.Exit => "🚪", _ => "🌫️" };
        
        string msg = $"{icon} **Room Type: {room.Type}**\n" +
                     $"❤️ HP: {hero.Hp}/{hero.MaxHp} | 🪄 MP: {hero.MagicPower}\n" +
                     $"🛡 Def: {hero.PhisDefense} | ⚔️ Dmg: {hero.HandDmg}\n" +
                     $"🎒 Bag: {bag}\n\nWhere to?";

        var buttons = new List<InlineKeyboardButton[]>();
        
        if (room.Type == RoomType.Loot)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("🔍 Search the Room", $"action_loot:{room.Id}") });
        }

        foreach (var ex in room.Exits)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData($"Go {ex.Direction}", $"move_to:{ex.TargetRoomId}") });
        }

        await _botClient.SendMessage(chatId, msg, replyMarkup: new InlineKeyboardMarkup(buttons), parseMode: ParseMode.Markdown);
    }
}