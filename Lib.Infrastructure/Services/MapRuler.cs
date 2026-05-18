using Lib.Core.Enums;
using Lib.Infrastructure.Database;
using Lib.Infrastructure.Database.Repositories;

namespace Lib.Infrastructure.Services;

public class MapRuler
{
    private readonly RoomRepository _roomRepo;
    private readonly CharacterRepository _charRepo;

    public MapRuler(RoomRepository roomRepo, CharacterRepository charRepo)
    {
        _roomRepo = roomRepo;
        _charRepo = charRepo;
    }

    public void GenerateStarterDungeon(int charId, long telegramId)
    {
        int r1 = _roomRepo.CreateRoom(charId, RoomType.Empty);
        int r2 = _roomRepo.CreateRoom(charId, RoomType.Empty);
        int r3 = _roomRepo.CreateRoom(charId, RoomType.Loot);
        int r4 = _roomRepo.CreateRoom(charId, RoomType.Exit);

        _roomRepo.CreateConnection(r1, r2, "North");
        _roomRepo.CreateConnection(r2, r1, "South");
        _roomRepo.CreateConnection(r2, r3, "East");
        _roomRepo.CreateConnection(r3, r2, "West");
        _roomRepo.CreateConnection(r2, r4, "North");

        _charRepo.UpdateCharacterRoom(telegramId, r1);
    }

    public void MovePlayer(long telegramId, int targetRoomId)
    {
        _charRepo.UpdateCharacterRoom(telegramId, targetRoomId);
    }
}