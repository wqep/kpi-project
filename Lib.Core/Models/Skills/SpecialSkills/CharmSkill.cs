using Lib.Core.Interfaces;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class CharmSkill : ISkill
{
    public string Name => "Charming";
    
    public void Execute(IBattleUnit caster, IBattleUnit target)
    {
        var existCharm = target.CurrentEffects.Find(e => e.BattleStateEnum == BattleStateEnum.Charmed);
        if (existCharm != null)
        {
            existCharm.TurnsLeft = 3;
        }

        else
        {
            target.CurrentEffects.Add(new ActiveEffect(BattleStateEnum.Charmed, 3));
            target.PhisDefense = Math.Max(0, target.PhisDefense - 3);
        }
    }
}