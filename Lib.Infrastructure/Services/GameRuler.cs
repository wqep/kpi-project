using System.Collections.Generic;
using Lib.Core.BaseClasses;
using Lib.Core.Enums;
using Lib.Core.Models.Items;
using Lib.Core.Models.Items.Common;
using Lib.Infrastructure.Database;

namespace Lib.Core.Services;

public class GameRuler
{
    private readonly DatabaseManager _db;

    public GameRuler(DatabaseManager db)
    {
        _db = db;
    }

    public string ProcessLooting(Character hero, int roomId)
    {
        var foundItems = new List<BaseItem> 
        { 
            new RedHeart(), 
            new IronPlate()
        };
        
        string lootMsg = "🎉 You found items:\n\n";

        foreach (var item in foundItems)
        {
            item.AddBonuses(hero);
            _db.AddItemToInventory(hero.Id, item.Name);
            lootMsg += $"**{item.Name}** ({item.Rarity})\n_{item.Description}_\n\n";
        }

        _db.UpdateCharacterStats(hero);
        _db.ChangeRoomType(roomId, RoomType.Empty);

        return lootMsg;
    }
}