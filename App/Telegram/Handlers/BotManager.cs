using Lib.Core.BaseClasses;
using Lib.Core.Enums;
using Lib.Infrastructure.CharacterFactory;
using Lib.Infrastructure.Database.Repositories;
using Lib.Infrastructure.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lib.Infrastructure.Database;

namespace App.Telegram.Handlers;

public class BotManager
{
    private readonly CharacterRepository _charRepo;
    private readonly RoomRepository _roomRepo;
    private readonly InventoryRepository _invRepo;
    private readonly UserRepository _userRepo;
    private readonly ActiveBattleRepository _battleRepo;
    private readonly MapRuler _mapRuler;
    private readonly GameRuler _gameRuler;
    private readonly BattleRuler _battleRuler;
    private readonly ITelegramBotClient _botClient;

    public BotManager(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        
        _charRepo = new CharacterRepository();
        _roomRepo = new RoomRepository();
        _invRepo = new InventoryRepository();
        _userRepo = new UserRepository();
        
        _battleRepo = new ActiveBattleRepository();
        _mapRuler = new MapRuler(_roomRepo, _charRepo, _battleRepo);
        _gameRuler = new GameRuler(_invRepo, _charRepo, _roomRepo);
        _battleRuler = new BattleRuler(_charRepo, _battleRepo);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            long id = update.CallbackQuery.From.Id;
            string data = update.CallbackQuery.Data;

            Character? hero = _charRepo.GetActiveCharacter(id);

            if (hero != null && hero.State == 1)
            {
                if (data.StartsWith("move_to:") || data.StartsWith("action_loot:"))
                {
                    await botClient.AnswerCallbackQuery(
                        callbackQueryId: update.CallbackQuery.Id,
                        text: "⚔️ You're in a fight, you cannot run!",
                        showAlert: true);
                    return;
                }
            }

            await botClient.AnswerCallbackQuery(update.CallbackQuery.Id);

            if (data.StartsWith("class_"))
            {
                _userRepo.EnsureUserExists(id, update.CallbackQuery.From.Username ?? "Unknown");

                var oldHero = _charRepo.GetActiveCharacter(id);
                if (oldHero != null)
                    _charRepo.KillCharacter(oldHero.Id);

                hero = CharacterFactory.CreateFromClass(id, data);
                hero.Id = _charRepo.SaveCharacter(hero);
                _mapRuler.GenerateLocationOne(hero.Id, id, floor: 1);
                hero = _charRepo.GetActiveCharacter(id);
                if (hero != null) await ShowRoom(id, hero);
            }
            else if (data.StartsWith("move_to:"))
            {
                int targetId = int.Parse(data.Split(':')[1]);
                string moveResult = _mapRuler.ProcessRoomEntry(id, targetId);

                hero = _charRepo.GetActiveCharacter(id);
                if (hero == null) return;

                if (moveResult == "no_turns")
                {
                    await botClient.SendMessage(id, "⌛ *You ran out of turns! Darkness consumed you.*", parseMode: ParseMode.Markdown);
                    return;
                }

                if (moveResult == "victory")
                {
                    await botClient.SendMessage(id, "🏆 *You conquered all locations! You are victorious!*", parseMode: ParseMode.Markdown);
                    return;
                }

                if (moveResult.StartsWith("next_floor:"))
                {
                    var parts = moveResult.Split(':');
                    int loc = int.Parse(parts[1]);
                    int floor = int.Parse(parts[2]);
                    await botClient.SendMessage(id, $"🚪 *You descend deeper...*\n📍 Location {loc}, Floor {floor}", parseMode: ParseMode.Markdown);
                    hero = _charRepo.GetActiveCharacter(id);
                    if (hero != null) await ShowRoom(id, hero);
                    return;
                }

                if (hero.State == 1)
                    await ShowBattle(id, hero);
                else
                    await ShowRoom(id, hero);
            }
            
            else if (data.StartsWith("action_loot:"))
            {
                int roomId = int.Parse(data.Split(':')[1]);
                if (hero != null)
                {
                    string lootMsg = _gameRuler.ProcessLooting(hero, roomId);
                    await botClient.SendMessage(id, lootMsg, parseMode: ParseMode.Markdown);
                    hero = _charRepo.GetActiveCharacter(id);
                    if (hero != null) await ShowRoom(id, hero);
                }
            }
            else if (data == "battle_action:attack")
            {
                if (hero == null) return;
                var (msg, enemies) = _battleRuler.GetBattleState(id);

                var buttons = new List<InlineKeyboardButton[]>();
                for (int i = 0; i < enemies.Count; i++)
                {
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"⚔️ {enemies[i].Name} (HP: {enemies[i].Hp})",
                            $"battle_target:{i}")
                    });
                }
                buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("« Back", "battle_back") });

                await botClient.SendMessage(id, "Choose your target:", replyMarkup: new InlineKeyboardMarkup(buttons));
            }
            else if (data == "battle_action:defend")
            {
                if (hero == null) return;
                string defendResult = _battleRuler.ProcessDefend(id);
                await botClient.SendMessage(id, defendResult, parseMode: ParseMode.Markdown);
                hero = _charRepo.GetActiveCharacter(id);
                if (hero != null) await ShowBattle(id, hero);
            }
            else if (data == "battle_back")
            {
                if (hero != null) await ShowBattle(id, hero);
            }
            else if (data.StartsWith("battle_target:"))
            {
                int enemyIndex = int.Parse(data.Split(':')[1]);
                if (hero == null) return;

                string attackResult = _battleRuler.ProcessAttack(id, enemyIndex);
                await botClient.SendMessage(id, attackResult, parseMode: ParseMode.Markdown);

                if (attackResult.Contains("DARKNESS TOOK YOU"))
                    return;

                hero = _charRepo.GetActiveCharacter(id);
                if (hero == null) return;

                if (hero.State == 1)
                    await ShowBattle(id, hero);
                else
                    await ShowRoom(id, hero);
            }

            return;
        }

        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            long id = update.Message.From.Id;
            if (update.Message.Text == "/start")
            {
                var menu = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Warrior", "class_warrior"),
                        InlineKeyboardButton.WithCallbackData("Thief", "class_thief"),
                        InlineKeyboardButton.WithCallbackData("Paladin", "class_paladin")
                    }
                });
                await botClient.SendMessage(id, "Choose your class:", replyMarkup: menu);
            }
            else
            {
                Character? hero = _charRepo.GetActiveCharacter(id);
                if (hero == null) return;

                if (hero.State == 1)
                    await ShowBattle(id, hero);
                else
                    await ShowRoom(id, hero);
            }
        }
    }

    private async Task ShowBattle(long chatId, Character hero)
    {
        var (msg, enemies) = _battleRuler.GetBattleState(chatId);

        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData("⚔️ Attack", "battle_action:attack") },
            new[] { InlineKeyboardButton.WithCallbackData("🛡 Defend", "battle_action:defend") }
        };

        await _botClient.SendMessage(chatId, msg, replyMarkup: new InlineKeyboardMarkup(buttons), parseMode: ParseMode.Markdown);
    }

    private async Task ShowRoom(long chatId, Character hero)
    {
        var room = _roomRepo.GetRoom(hero.CurrentRoomId);
        if (room == null) return;

        var inv = _invRepo.GetInventory(hero.Id);
        string bag = inv.Count > 0 ? string.Join(", ", inv) : "Empty";

        string icon = room.Type switch { RoomType.Loot => "🎁", RoomType.Exit => "🚪", _ => "🌫️" };

        string msg = $"{icon} **Room: {room.Type}**\n" +
                     $"❤️ HP: {hero.Hp}/{hero.MaxHp} | 🪄 MP: {hero.MagicPower}\n" +
                     $"🛡 Def: {hero.PhisDefense} | ⚔️ Dmg: {hero.HandDmg}\n" +
                     $"⌛ Turns left: {hero.TurnsLeft}\n" +
                     $"🎒 Bag: {bag}\n\nWhere to?";

        var buttons = new List<InlineKeyboardButton[]>();

        if (room.Type == RoomType.Loot)
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("🔍 Search the Room", $"action_loot:{room.Id}") });

        foreach (var ex in room.Exits)
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData($"Go {ex.Direction}", $"move_to:{ex.TargetRoomId}") });

        await _botClient.SendMessage(chatId, msg, replyMarkup: new InlineKeyboardMarkup(buttons), parseMode: ParseMode.Markdown);
    }
}