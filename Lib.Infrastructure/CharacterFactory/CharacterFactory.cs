using Lib.Core.BaseClasses;
using Serilog;

namespace Lib.Infrastructure.CharacterFactory;

public static class CharacterFactory
{
    public static Character CreateFromClass(long tgId, string callbackData)
    {
        Log.Debug("Creating a new character for TelegramId {TelegramId} with class {ClassCallback}", tgId, callbackData);
        
        var character = new Character 
        { 
            TelegramId = tgId, 
            Level = 1,
            CurrentEffects = new(),
            Items = new()
        };

        switch (callbackData)
        {
            case "class_thief":
                SetStats(character, "Thief", 42, 23, 2, 5);
                break;
                
            case "class_warrior":
                SetStats(character, "Warrior", 55, 14, 4, 4);
                break;
                
            case "class_paladin":
                SetStats(character, "Paladin", 75, 12, 6, 2);
                break;
            
            default:
                Log.Warning("Received unknown callbackData when creating class: {CallbackData}. TelegramId: {TelegramId}", callbackData, tgId);
                break;
        }
        
        return character;
    }

    private static void SetStats(Character c, string className, int hp, int dmg, int def, int mp)
    {
        c.Class = className;
        c.MaxHp = hp;
        c.Hp = hp; 
        c.HandDmg = dmg;
        c.BasePhisDefense = def;
        c.PhisDefense = def;
        c.MagicPower = mp;
    }
}