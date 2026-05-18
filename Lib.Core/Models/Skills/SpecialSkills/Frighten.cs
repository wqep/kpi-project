using Lib.Core.Interfaces;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class Frighten : ISkill
{
    public string Name => "Frighten";
    public void Execute(IBattleUnit casteer, IBattleUnit target)
    {
        var frightened = target.CurrentEffects.Find(e => e.BattleStateEnum == BattleStateEnum.Frightened);
        
        if (frightened != null)
        {
            int duration = 1;
            target.CurrentEffects.Add(new ActiveEffect(BattleStateEnum.Frightened, duration));
        }
    }
}