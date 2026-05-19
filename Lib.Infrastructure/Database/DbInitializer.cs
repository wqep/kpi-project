using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database;

public class DbInitializer : BaseRepository
{
    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var tables = new[]
        {
            @"CREATE TABLE IF NOT EXISTS Users (
                TelegramId INTEGER PRIMARY KEY,
                Username TEXT
            )",

            @"CREATE TABLE IF NOT EXISTS Characters (
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
                State INTEGER DEFAULT 0,
                IsAlive INTEGER DEFAULT 1,
                Location INTEGER DEFAULT 1,
                Floor INTEGER DEFAULT 1,
                TurnsLeft INTEGER DEFAULT 25,
                FOREIGN KEY (TelegramId) REFERENCES Users(TelegramId)
            )",

            @"CREATE TABLE IF NOT EXISTS Rooms (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CharacterId INTEGER,
                Type INTEGER,
                FOREIGN KEY (CharacterId) REFERENCES Characters(Id)
            )",

            @"CREATE TABLE IF NOT EXISTS RoomConnections (
                FromRoomId INTEGER,
                ToRoomId INTEGER,
                Direction TEXT,
                FOREIGN KEY (FromRoomId) REFERENCES Rooms(Id),
                FOREIGN KEY (ToRoomId) REFERENCES Rooms(Id)
            )",

            @"CREATE TABLE IF NOT EXISTS Inventory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CharacterId INTEGER,
                ItemName TEXT,
                FOREIGN KEY (CharacterId) REFERENCES Characters(Id)
            )",

            @"CREATE TABLE IF NOT EXISTS ActiveBattles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CharId INTEGER UNIQUE NOT NULL,
                EnemiesJson TEXT NOT NULL DEFAULT '[]',
                HeroEffectsJson TEXT NOT NULL DEFAULT '[]'
            )"
        };

        foreach (var sql in tables)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}