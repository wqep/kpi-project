using Lib.Core.BaseClasses;
using Lib.Core.Enums;
using Lib.Core.Models.Map;
using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database;

public class DatabaseManager
{
    private readonly string _connectionString = "Data Source=game.db";

    public DatabaseManager()
    {
        InitDatabase();
    }

    public void InitDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                TelegramId INTEGER PRIMARY KEY,
                Username TEXT
            );

            CREATE TABLE IF NOT EXISTS Characters (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TelegramId INTEGER,
                Class TEXT,
                Level INTEGER,
                HP INTEGER,
                MaxHP INTEGER,
                HandDmg INTEGER,
                PhisDefense INTEGER,
                BasePhisDefense INTEGER,
                MagicPower INTEGER,
                CurrentRoomId INTEGER DEFAULT 0,
                State INTEGRAL DEFAULT 0,
                IsAlive INTEGER DEFAULT 1,
                FOREIGN KEY (TelegramId) REFERENCES Users(TelegramId)
            );

            CREATE TABLE IF NOT EXISTS Rooms (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CharacterId INTEGER,
                Type INTEGER
            );

            CREATE TABLE IF NOT EXISTS RoomConnections (
                FromRoomId INTEGER,
                ToRoomId INTEGER,
                Direction TEXT,
                FOREIGN KEY (FromRoomId) REFERENCES Rooms(Id),
                FOREIGN KEY (ToRoomId) REFERENCES Rooms(Id)
            );

            CREATE TABLE IF NOT EXISTS Inventory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CharacterId INTEGER,
                ItemName TEXT,
                FOREIGN KEY (CharacterId) REFERENCES Characters(Id)
            );
        ";
        command.ExecuteNonQuery();
    }

    public void EnsureUserExists(long telegramId, string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Users (TelegramId, Username) VALUES ($id, $username) ON CONFLICT(TelegramId) DO UPDATE SET Username = $username;";
        command.Parameters.AddWithValue("$id", telegramId);
        command.Parameters.AddWithValue("$username", username);
        command.ExecuteNonQuery();
    }

    public int SaveCharacter(Character c)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Characters (TelegramId, Class, Level, HP, MaxHP, HandDmg, PhisDefense, BasePhisDefense, MagicPower) 
            VALUES ($tgId, $class, $lvl, $hp, $mhp, $dmg, $def, $bdef, $mp);
            SELECT last_insert_rowid();";
        
        command.Parameters.AddWithValue("$tgId", c.TelegramId);
        command.Parameters.AddWithValue("$class", c.Class);
        command.Parameters.AddWithValue("$lvl", c.Level);
        command.Parameters.AddWithValue("$hp", c.Hp);
        command.Parameters.AddWithValue("$mhp", c.MaxHp);
        command.Parameters.AddWithValue("$dmg", c.HandDmg);
        command.Parameters.AddWithValue("$def", c.PhisDefense);
        command.Parameters.AddWithValue("$bdef", c.BasePhisDefense);
        command.Parameters.AddWithValue("$mp", c.MagicPower);
        
        return Convert.ToInt32(command.ExecuteScalar());
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

    public void AddItemToInventory(int charId, string itemName)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Inventory (CharacterId, ItemName) VALUES ($charId, $item)";
        command.Parameters.AddWithValue("$charId", charId);
        command.Parameters.AddWithValue("$item", itemName);
        command.ExecuteNonQuery();
    }

    public List<string> GetInventory(int charId)
    {
        var items = new List<string>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT ItemName FROM Inventory WHERE CharacterId = $charId";
        command.Parameters.AddWithValue("$charId", charId);
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(reader.GetString(0));
        return items;
    }

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

    public void UpdateCharacterRoom(long tgId, int roomId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Characters SET CurrentRoomId = $rid WHERE TelegramId = $tgId AND IsAlive = 1";
        command.Parameters.AddWithValue("$rid", roomId);
        command.Parameters.AddWithValue("$tgId", tgId);
        command.ExecuteNonQuery();
    }

    public Character? GetActiveCharacter(long telegramId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Characters WHERE TelegramId = $tgId AND IsAlive = 1 LIMIT 1";
        command.Parameters.AddWithValue("$tgId", telegramId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Character
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TelegramId = reader.GetInt64(reader.GetOrdinal("TelegramId")),
                Class = reader.GetString(reader.GetOrdinal("Class")),
                Hp = reader.GetInt32(reader.GetOrdinal("HP")),
                MaxHp = reader.GetInt32(reader.GetOrdinal("MaxHP")),
                HandDmg = reader.GetInt32(reader.GetOrdinal("HandDmg")),
                PhisDefense = reader.GetInt32(reader.GetOrdinal("PhisDefense")),
                BasePhisDefense = reader.GetInt32(reader.GetOrdinal("BasePhisDefense")),
                MagicPower = reader.GetInt32(reader.GetOrdinal("MagicPower")),
                Level = reader.GetInt32(reader.GetOrdinal("Level")),
                CurrentRoomId = reader.GetInt32(reader.GetOrdinal("CurrentRoomId")),
                State = reader.GetInt32(reader.GetOrdinal("State"))
            };
        }
        return null;
    }
    
    public void UpdateCharacterStats(Character hero)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE Characters 
        SET HP = $hp, MaxHP = $mhp, HandDmg = $dmg, 
            PhisDefense = $def, BasePhisDefense = $bdef, MagicPower = $mp 
        WHERE Id = $id";
    
        command.Parameters.AddWithValue("$hp", hero.Hp);
        command.Parameters.AddWithValue("$mhp", hero.MaxHp);
        command.Parameters.AddWithValue("$dmg", hero.HandDmg);
        command.Parameters.AddWithValue("$def", hero.PhisDefense);
        command.Parameters.AddWithValue("$bdef", hero.BasePhisDefense);
        command.Parameters.AddWithValue("$mp", hero.MagicPower);
        command.Parameters.AddWithValue("$id", hero.Id);
    
        command.ExecuteNonQuery();
    }
    
    public void UpdateCharacterState(int charId, int state)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Characters SET State = $state WHERE Id = $id";
        command.Parameters.AddWithValue("$state", state);
        command.Parameters.AddWithValue("$id", charId);
        command.ExecuteNonQuery();
    }
}