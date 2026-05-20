using System.Collections.Generic;
using System.Text;
using Lib.Core.Enums;
using Lib.Core.Models.Map;
using Lib.Infrastructure.Database;

namespace Lib.Infrastructure.Services;

public class MapRenderer
{
    private readonly RoomRepository _roomRepo;

    public MapRenderer(RoomRepository roomRepo)
    {
        _roomRepo = roomRepo;
    }

    public string RenderMap(int charId, int currentRoomId, int width, int height)
    {
        var allRooms = _roomRepo.GetAllCharacterRooms(charId);
        var explored = _roomRepo.GetExploredRooms(charId);

        var sb = new StringBuilder();
        sb.AppendLine("```");

        for (int y = height - 1; y >= 0; y--)
        {
            var row = new StringBuilder();
            for (int x = 0; x < width; x++)
            {
                int index = x * height + y;
                if (index >= allRooms.Count) break;

                var room = allRooms[index];
                bool isCurrent = room.Id == currentRoomId;
                bool isExplored = explored.Contains(room.Id);

                row.Append(GetRoomIcon(room, isCurrent, isExplored));

                if (x < width - 1)
                    row.Append("-");
            }
            sb.AppendLine(row.ToString());

            if (y > 0)
            {
                var connRow = new StringBuilder();
                for (int x = 0; x < width; x++)
                    connRow.Append(x < width - 1 ? "|  " : "|");
                sb.AppendLine(connRow.ToString());
            }
        }

        sb.AppendLine("```");
        return sb.ToString();
    }

    private string GetRoomIcon(Room room, bool isCurrent, bool isExplored)
    {
        if (isCurrent) return "📍";
        if (!isExplored) return "⬜";

        return room.Type switch
        {
            RoomType.Enemy => "💀",
            RoomType.Loot  => "🎁",
            RoomType.Exit  => "🚪",
            _              => "🌫️"
        };
    }
}