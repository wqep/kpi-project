using System.Collections.Generic;
using Lib.Core.BaseClasses;
using Lib.Core.Enums;
using Lib.Core.Models.Items;
using Lib.Core.Models.Items.Common;
using Lib.Infrastructure.Database;
using Lib.Infrastructure.Database.Repositories;
using Serilog;

namespace Lib.Infrastructure.Services;

public class GameRuler
{
    private readonly InventoryRepository _invRepo;
    private readonly CharacterRepository _charRepo;
    private readonly RoomRepository _roomRepo;

    public GameRuler(InventoryRepository invRepo, CharacterRepository charRepo, RoomRepository roomRepo)
    {
        _invRepo = invRepo;
        _charRepo = charRepo;
        _roomRepo = roomRepo;
    }

    public string ProcessLooting(Character hero, int roomId)
    {
        Log.Debug("Hero {HeroId} is looting room {RoomId}", hero.Id, roomId);
        
        var foundItems = new List<BaseItem> 
        { 
            new RedHeart(), 
            new IronPlate()
        };
        
        string lootMsg = "🎉 You found items:\n\n";

        foreach (var item in foundItems)
        {
            item.AddBonuses(hero);
            _invRepo.AddItemToInventory(hero.Id, item.Name);
            lootMsg += $"**{item.Name}** ({item.Rarity})\n_{item.Description}_\n\n";
            
            Log.Information("Hero {HeroId} found item {ItemName} in room {RoomId}", hero.Id, item.Name, roomId);
        }

        _charRepo.UpdateCharacterStats(hero);
        _roomRepo.ChangeRoomType(roomId, RoomType.Empty);
        
        Log.Debug("Room {RoomId} marked as Empty after looting by hero {HeroId}", roomId, hero.Id);

        return lootMsg;
    }
}