using Microsoft.Data.Sqlite;

namespace MyRoguelikeBot.Database;

public class DatabaseManager
{
    private string _connectionString = "Data Source=game.db";

    public void InitDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Players (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TelegramId INTEGER UNIQUE,
                Nickname TEXT,
                Class TEXT,
                Level INTEGER DEFAULT 1,
                MaxHp INTEGER,
                HP INTEGER,
                HandDmg INTEGER,
                PhisDefense INTEGER,
                BasePhisDefense INTEGER,
                MagicPower INTEGER
            )";
        command.ExecuteNonQuery();
    }
}