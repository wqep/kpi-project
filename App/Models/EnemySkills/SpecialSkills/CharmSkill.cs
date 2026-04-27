using KPI_PROJECT.Models;
using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.EnemySkills;

public class CharmSkill : ISkill
{
    public string Name => "Charming";
    
    
    public void Execute(IBattleUnit.IBattleUnit caster, IBattleUnit.IBattleUnit target)
    {
        var existCharm = target.CurrentEffects.Find(e => e.BattleState == BattleState.Charmed);
        if (existCharm != null)
        {
            existCharm.TurnsLeft = 3;
        }

        else
        {
            target.CurrentEffects.Add(new ActiveEffect(BattleState.Charmed, 3));
            target.PhisDefense = Math.Max(0, target.PhisDefense - 3);
        }
    }
}