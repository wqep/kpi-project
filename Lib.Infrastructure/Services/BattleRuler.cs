using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Lib.Core.BaseClasses;
using Lib.Core.Factories;
using Lib.Core.Interfaces;
using Lib.Core.Models;
using Lib.Core.Models.StatesAndEffects;
using Lib.Infrastructure.Database.Repositories;

namespace Lib.Infrastructure.Services;

public class BattleRuler
{
    private readonly CharacterRepository _charRepo;
    private readonly ActiveBattleRepository _battleRepo;

    public BattleRuler(CharacterRepository charRepo, ActiveBattleRepository battleRepo)
    {
        _charRepo = charRepo;
        _battleRepo = battleRepo;
    }

    public (string Message, List<EnemyBattleData> Enemies) GetBattleState(long telegramId)
    {
        var hero = _charRepo.GetActiveCharacter(telegramId);
        if (hero == null) return ("Character not found.", new());

        var battleData = _battleRepo.GetBattle(hero.Id);
        if (battleData == null) return ("Battle not found.", new());

        var enemies = JsonSerializer.Deserialize<List<EnemyBattleData>>(battleData.Value.EnemiesJson) ?? new();
        hero.CurrentEffects = JsonSerializer.Deserialize<List<ActiveEffect>>(battleData.Value.HeroEffectsJson) ?? new();

        string effectsInfo = hero.CurrentEffects.Count > 0
            ? "\n⚡ Effects: " + string.Join(", ", hero.CurrentEffects.Select(e => $"{e.BattleStateEnum}({e.TurnsLeft})"))
            : "";

        string enemyList = string.Join("\n", enemies.Select((e, i) => $"{i + 1}. {e.Name} ❤️ {e.Hp}/{e.MaxHp}"));

        string msg = $"⚔️ Battle!\n" +
                     $"❤️ HP: {hero.Hp}/{hero.MaxHp}{effectsInfo}\n\n" +
                     $"{enemyList}\n\n" +
                     $"Choose your action:";

        return (msg, enemies);
    }

    public string ProcessAttack(long telegramId, int enemyIndex)
    {
        var hero = _charRepo.GetActiveCharacter(telegramId);
        if (hero == null) return "Character not found.";
        if (hero.State != 1) return "You are not in battle.";

        var battleData = _battleRepo.GetBattle(hero.Id);
        if (battleData == null)
        {
            _charRepo.UpdateCharacterState(hero.Id, 0);
            return "Error: battle not found.";
        }

        var enemies = JsonSerializer.Deserialize<List<EnemyBattleData>>(battleData.Value.EnemiesJson) ?? new();
        hero.CurrentEffects = JsonSerializer.Deserialize<List<ActiveEffect>>(battleData.Value.HeroEffectsJson) ?? new();

        if (enemyIndex < 0 || enemyIndex >= enemies.Count)
            return "Invalid target.";

        var target = enemies[enemyIndex];

        int dmgToEnemy = Math.Max(0, hero.HandDmg - GetEnemyDefense(target.ClassType));
        target.Hp = Math.Max(0, target.Hp - dmgToEnemy);

        string result = $"⚔️ You attacked {target.Name} for {dmgToEnemy} damage.";

        if (target.Hp <= 0)
        {
            enemies.RemoveAt(enemyIndex);
            result += $"\n💀 {target.Name} has been defeated!";
        }

        if (enemies.Count == 0)
        {
            _battleRepo.EndBattle(hero.Id);
            _charRepo.UpdateCharacterState(hero.Id, 0);
            _charRepo.UpdateTurnsLeft(hero.Id, hero.TurnsLeft - 1);
            return result + "\n\n✅ All enemies defeated! You may move on.";
        }

        var rand = new Random();
        foreach (var enemy in enemies)
        {
            var enemyObj = EnemyFactory.CreateByClassName(enemy.ClassType);
            enemyObj.CurrentEffects = enemy.Effects;
            var skill = enemyObj.CurrentSkills[rand.Next(enemyObj.CurrentSkills.Count)];
            skill.Execute(enemyObj, hero);
            result += $"\n🗡 {enemy.Name} used {skill.Name}";
        }

        TickEffects(hero);
        foreach (var enemy in enemies)
            enemy.Effects = TickEffectsList(enemy.Effects);

        _charRepo.UpdateTurnsLeft(hero.Id, hero.TurnsLeft - 1);
        _charRepo.UpdateCharacterStats(hero);
        _battleRepo.SaveBattleState(
            hero.Id,
            JsonSerializer.Serialize(enemies),
            JsonSerializer.Serialize(hero.CurrentEffects)
        );

        if (hero.Hp <= 0)
        {
            _battleRepo.EndBattle(hero.Id);
            _charRepo.UpdateCharacterState(hero.Id, 0);
            _charRepo.KillCharacter(hero.Id);
            return result + "\n\n💀 You died. DARKNESS TOOK YOU.";
        }

        result += $"\n\n❤️ Your HP: {hero.Hp}/{hero.MaxHp}";
        return result;
    }

    public string ProcessDefend(long telegramId)
    {
        var hero = _charRepo.GetActiveCharacter(telegramId);
        if (hero == null) return "Character not found.";

        hero.CurrentEffects.Add(new ActiveEffect(BattleStateEnum.Defensive, 1));
        _charRepo.UpdateTurnsLeft(hero.Id, hero.TurnsLeft - 1);

        var battleData = _battleRepo.GetBattle(hero.Id);
        if (battleData == null) return "Battle not found.";

        _battleRepo.SaveBattleState(
            hero.Id,
            battleData.Value.EnemiesJson,
            JsonSerializer.Serialize(hero.CurrentEffects)
        );

        return $"🛡 You take a defensive stance. Incoming damage reduced this turn.";
    }

    private int GetEnemyDefense(string classType)
    {
        return classType switch
        {
            "StarterMagician" => 1,
            "WiseMagician" => 2,
            "MagicianShishian" => 3,
            "Squire" => 4,
            "RoyalKnight" => 5,
            "Warlord" => 6,
            _ => 0
        };
    }

    private void TickEffects(Character hero)
    {
        for (int i = hero.CurrentEffects.Count - 1; i >= 0; i--)
        {
            hero.CurrentEffects[i].TurnsLeft--;
            if (hero.CurrentEffects[i].TurnsLeft <= 0)
                hero.CurrentEffects.RemoveAt(i);
        }
    }

    private List<ActiveEffect> TickEffectsList(List<ActiveEffect> effects)
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            effects[i].TurnsLeft--;
            if (effects[i].TurnsLeft <= 0)
                effects.RemoveAt(i);
        }
        return effects;
    }
}