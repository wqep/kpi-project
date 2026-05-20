using System;
using System.Collections.Generic;
using Lib.Core.BaseClasses;
using Lib.Core.Models.Enemies.FirstLevel;
using Lib.Core.Models.Enemies.SecondLocation;

namespace Lib.Core.Factories;

public static class EnemyFactory
{
    private static readonly Random _rand = new Random();

    public static List<EnemyBase> GenerateEnemiesForLocation(int locationId)
    {
        int count = _rand.Next(1, 4); 
        var enemies = new List<EnemyBase>();
        
        for (int i = 0; i < count; i++)
        {
            enemies.Add(locationId switch
            {
                1 => GetFirstLocationEnemy(),
                2 => GetSecondLocationEnemy(),
                _ => GetFirstLocationEnemy()
            });
        }
        
        return enemies;
    }

    private static EnemyBase GetFirstLocationEnemy()
    {
        return _rand.Next(1, 4) switch
        {
            1 => new StarterMagician(),
            2 => new WiseMagician(),
            _ => new MagicianShishian()
        };
    }

    private static EnemyBase GetSecondLocationEnemy()
    {
        return _rand.Next(1, 4) switch
        {
            1 => new Squire(),
            2 => new RoyalKnight(),
            _ => new Warlord()
        };
    }
    
    public static EnemyBase CreateByClassName(string className)
    {
        return className switch
        {
            "StarterMagician" => new StarterMagician(),
            "WiseMagician" => new WiseMagician(),
            "MagicianShishian" => new MagicianShishian(),
            "Squire" => new Squire(),
            "RoyalKnight" => new RoyalKnight(),
            "Warlord" => new Warlord(),
            _ => new StarterMagician()
        };
    }
}