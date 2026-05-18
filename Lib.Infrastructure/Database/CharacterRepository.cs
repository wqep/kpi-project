using Lib.Core.BaseClasses;
using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database.Repositories;

public class CharacterRepository : BaseRepository
{
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
}