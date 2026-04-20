using KPI_PROJECT.Models.BaseClasses;
using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.FirstLevelEnemies;

public class StarterMagician : EnemyBase
{
    public StarterMagician()
    {
        HP = 25;
        MaxHp = 25;
        HandDmg = 5;
        Name = "Wise Magician";
        PhisDefence = 1;
        Speed = 4;
    }

    public override void HandAttack( Player player)
    {
        var existDefensive = player.CurrentEffects.Find(e => e.BattleState == BattleState.Defensive);
        int totalDmg = HandDmg - player.Defense;
        if (existDefensive != null)
        {
            totalDmg -= 2;
        }
        
        totalDmg = Math.Max(0, totalDmg);
        player.HP -= totalDmg;
    }

    /// <summary>
    /// Charming. Lowers defense (-3), and lets other skills to hurt harder (x2)
    /// </summary>
    public override void UseSkill(Player player)
    {
        var existCharm = player.CurrentEffects.Find(e => e.BattleState == BattleState.Charmed);
        if (existCharm != null)
        {
            existCharm.TurnsLeft = 3;
        }

        else
        {
            player.CurrentEffects.Add(new ActiveEffect(BattleState.Charmed, 3));
            player.Defense = Math.Max(0, player.Defense - 3);
        }
    }
    /// <summary>
    /// FIRE BALLLL. Base dmg -3, sets burning state
    /// </summary>
    public override void UseSkill2( Player player)
    {
        var existBurning = player.CurrentEffects.Find(e => e.BattleState == BattleState.Burning);
        var existCharm = player.CurrentEffects.Find(e => e.BattleState == BattleState.Charmed);
        if (existBurning != null)
        { 
            existBurning.TurnsLeft = 3;
        }
        
        else
        {
            player.CurrentEffects.Add(new ActiveEffect(BattleState.Burning, 3));
        }

        int totalDmg = 3;

        if (existCharm != null)
        {
            totalDmg *= 2;
        }
            
        player.HP-=totalDmg;
        
        
    }

    public override void Defend()
    {
        this.CurrentEffects.Add(new ActiveEffect(BattleState.Defensive, 3));
    }
}