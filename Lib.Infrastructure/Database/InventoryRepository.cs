using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database;

public class InventoryRepository : BaseRepository
{
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
}