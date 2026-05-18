using Lib.Core.Enums;
using Lib.Infrastructure.Database;

namespace Lib.Core.Services;

public class MapRuler
{
    private readonly DatabaseManager _db;

    public MapRuler(DatabaseManager db)
    {
        _db = db;
    }

    public void GenerateStarterDungeon(int charId, long telegramId)
    {
        int r1 = _db.CreateRoom(charId, RoomType.Empty);
        int r2 = _db.CreateRoom(charId, RoomType.Empty);
        int r3 = _db.CreateRoom(charId, RoomType.Loot);
        int r4 = _db.CreateRoom(charId, RoomType.Exit);

        _db.CreateConnection(r1, r2, "North");
        _db.CreateConnection(r2, r1, "South");
        _db.CreateConnection(r2, r3, "East");
        _db.CreateConnection(r3, r2, "West");
        _db.CreateConnection(r2, r4, "North");

        _db.UpdateCharacterRoom(telegramId, r1);
    }

    public void MovePlayer(long telegramId, int targetRoomId)
    {
        _db.UpdateCharacterRoom(telegramId, targetRoomId);
    }
}