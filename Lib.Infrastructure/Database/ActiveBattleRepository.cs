using System;
using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database.Repositories;

public class ActiveBattleRepository
{
    private const string ConnectionString = "Data Source=game.db";

    public void StartBattle(int charId, string enemiesJson)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM ActiveBattles WHERE CharId = @charId";
        deleteCmd.Parameters.AddWithValue("@charId", charId);
        deleteCmd.ExecuteNonQuery();

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = @"
            INSERT INTO ActiveBattles (CharId, EnemiesJson, HeroEffectsJson) 
            VALUES (@charId, @enemies, '[]')";
        insertCmd.Parameters.AddWithValue("@charId", charId);
        insertCmd.Parameters.AddWithValue("@enemies", enemiesJson);
        insertCmd.ExecuteNonQuery();
    }

    public (string EnemiesJson, string HeroEffectsJson)? GetBattle(int charId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT EnemiesJson, HeroEffectsJson FROM ActiveBattles WHERE CharId = @charId";
        command.Parameters.AddWithValue("@charId", charId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
            return (reader.GetString(0), reader.GetString(1));

        return null;
    }

    public void SaveBattleState(int charId, string enemiesJson, string heroEffectsJson)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE ActiveBattles 
            SET EnemiesJson = @enemies, HeroEffectsJson = @heroFx 
            WHERE CharId = @charId";
        command.Parameters.AddWithValue("@enemies", enemiesJson);
        command.Parameters.AddWithValue("@heroFx", heroEffectsJson);
        command.Parameters.AddWithValue("@charId", charId);
        command.ExecuteNonQuery();
    }

    public void EndBattle(int charId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM ActiveBattles WHERE CharId = @charId";
        command.Parameters.AddWithValue("@charId", charId);
        command.ExecuteNonQuery();
    }
}