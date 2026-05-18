using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database;

public class UserRepository : BaseRepository
{
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
}