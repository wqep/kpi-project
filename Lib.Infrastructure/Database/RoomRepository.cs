using Lib.Core.Enums;
using Lib.Core.Models.Map;
using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database;

public class RoomRepository : BaseRepository
{
    public Room? GetRoom(int roomId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var roomCmd = connection.CreateCommand();
        roomCmd.CommandText = "SELECT Type FROM Rooms WHERE Id = $id";
        roomCmd.Parameters.AddWithValue("$id", roomId);

        using var reader = roomCmd.ExecuteReader();
        if (!reader.Read()) return null;

        var room = new Room { Id = roomId, Type = (RoomType)reader.GetInt32(0) };

        var connCmd = connection.CreateCommand();
        connCmd.CommandText = "SELECT ToRoomId, Direction FROM RoomConnections WHERE FromRoomId = $id";
        connCmd.Parameters.AddWithValue("$id", roomId);

        using var connReader = connCmd.ExecuteReader();
        while (connReader.Read())
        {
            room.Exits.Add(new RoomConnection { TargetRoomId = connReader.GetInt32(0), Direction = connReader.GetString(1) });
        }
        return room;
    }

    public int CreateRoom(int charId, RoomType type)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Rooms (CharacterId, Type) VALUES ($charId, $type); SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$charId", charId);
        command.Parameters.AddWithValue("$type", (int)type);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void CreateConnection(int from, int to, string dir)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO RoomConnections (FromRoomId, ToRoomId, Direction) VALUES ($from, $to, $dir)";
        command.Parameters.AddWithValue("$from", from);
        command.Parameters.AddWithValue("$to", to);
        command.Parameters.AddWithValue("$dir", dir);
        command.ExecuteNonQuery();
    }
    
    public void ChangeRoomType(int roomId, RoomType newType)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Rooms SET Type = $type WHERE Id = $id";
        command.Parameters.AddWithValue("$type", (int)newType);
        command.Parameters.AddWithValue("$id", roomId);
        command.ExecuteNonQuery();
    }
    
    public void MarkRoomExplored(int charId, int roomId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
        INSERT OR IGNORE INTO ExploredRooms (CharacterId, RoomId) 
        VALUES ($charId, $roomId)";
        command.Parameters.AddWithValue("$charId", charId);
        command.Parameters.AddWithValue("$roomId", roomId);
        command.ExecuteNonQuery();
    }

    public HashSet<int> GetExploredRooms(int charId)
    {
        var result = new HashSet<int>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT RoomId FROM ExploredRooms WHERE CharacterId = $charId";
        command.Parameters.AddWithValue("$charId", charId);
        using var reader = command.ExecuteReader();
        while (reader.Read()) result.Add(reader.GetInt32(0));
        return result;
    }

    public List<Room> GetAllCharacterRooms(int charId)
    {
        var rooms = new List<Room>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Type FROM Rooms WHERE CharacterId = $charId";
        command.Parameters.AddWithValue("$charId", charId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rooms.Add(new Room 
            { 
                Id = reader.GetInt32(0), 
                Type = (RoomType)reader.GetInt32(1) 
            });
        }
        return rooms;
    }
}