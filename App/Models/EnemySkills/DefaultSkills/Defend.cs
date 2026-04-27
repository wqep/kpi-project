using KPI_PROJECT.Models.BaseClasses;
using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.EnemySkills.DefaultSkills;

public class Defend : ISkill
{
    public string Name => "Defend";

    public void Execute(IBattleUnit.IBattleUnit caster, IBattleUnit.IBattleUnit target)
    {
        var defendExists =  target.CurrentEffects.Find(e => e.BattleState == BattleState.Defensive);
        if (defendExists != null)
        {
            return;
        }
        
        target.CurrentEffects.Add(new ActiveEffect(BattleState.Defensive, 1));
    }
}