using Lib.Core.BaseClasses;
using Microsoft.Data.Sqlite;
using System;
using Lib.Infrastructure.Services;

namespace Lib.Infrastructure.Database.Repositories;

public class CharacterRepository : BaseRepository
{
    public int SaveCharacter(Character c)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        
        command.CommandText = @"
            INSERT INTO Characters (TelegramId, Class, Level, HP, MaxHP, HandDmg, PhisDefense, BasePhisDefense, MagicPower, Location, Floor) 
            VALUES ($tgId, $class, $lvl, $hp, $mhp, $dmg, $def, $bdef, $mp, $loc, $floor);
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
        command.Parameters.AddWithValue("$loc", c.Location);
        command.Parameters.AddWithValue("$floor", c.Floor);

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
                State = reader.GetInt32(reader.GetOrdinal("State")),
                Location = reader.GetInt32(reader.GetOrdinal("Location")),
                Floor = reader.GetInt32(reader.GetOrdinal("Floor")),
                TurnsLeft = reader.GetInt32(reader.GetOrdinal("TurnsLeft")),
                MapWidth = reader.GetInt32(reader.GetOrdinal("MapWidth")),
                MapHeight = reader.GetInt32(reader.GetOrdinal("MapHeight"))
                
            };
        }
        return null;
    }
    
    public void UpdateMapSize(int charId, int width, int height)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Characters SET MapWidth = $w, MapHeight = $h WHERE Id = $id";
        command.Parameters.AddWithValue("$w", width);
        command.Parameters.AddWithValue("$h", height);
        command.Parameters.AddWithValue("$id", charId);
        command.ExecuteNonQuery();
    }
    
    public void UpdateLocationAndFloor(int charId, int location, int floor)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Characters SET Location = $loc, Floor = $floor WHERE Id = $id";
        command.Parameters.AddWithValue("$loc", location);
        command.Parameters.AddWithValue("$floor", floor);
        command.Parameters.AddWithValue("$id", charId);
        command.ExecuteNonQuery();
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
    
    public void UpdateTurnsLeft(int charId, int turnsLeft)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Characters SET TurnsLeft = $turns WHERE Id = $id";
        command.Parameters.AddWithValue("$turns", turnsLeft);
        command.Parameters.AddWithValue("$id", charId);
        command.ExecuteNonQuery();
    }

    public void ResetTurnsForFloor(int charId, int location, int floor)
    {
        int turns = TurnsHelper.GetTurnsForFloor(location, floor);
        UpdateTurnsLeft(charId, turns);
    }
    
    public void KillCharacter(int charId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Characters SET IsAlive = 0 WHERE Id = $id";
        command.Parameters.AddWithValue("$id", charId);
        command.ExecuteNonQuery();
    }
}