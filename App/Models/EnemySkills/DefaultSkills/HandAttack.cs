using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.EnemySkills.DefaultSkills;

public class HandAttack : ISkill
{
    public string Name => "Hand Attack";
    

    public void Execute(IBattleUnit.IBattleUnit caster, IBattleUnit.IBattleUnit target)
    {
        var existDefensive = target.CurrentEffects.Find(e => e.BattleState == BattleState.Defensive);
        int totalDmg = caster.HandDmg - target.PhisDefense;
        if (existDefensive != null)
        {
            totalDmg -= 2;
        }
        
        totalDmg = Math.Max(0, totalDmg);
        target.HP -= totalDmg;
    }
}