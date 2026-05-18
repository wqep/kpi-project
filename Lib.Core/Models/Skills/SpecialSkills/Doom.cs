using Lib.Core.Interfaces;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class Doom : ISkill
{
    public string Name => "Doom";
    
    public void Execute(IBattleUnit casteer, IBattleUnit target)
    {
        var doomed = target.CurrentEffects.Find(e => e.BattleStateEnum == BattleStateEnum.Doomed);
        
        if (doomed != null)
        {
            return;
        }
        
        target.CurrentEffects.Add(new ActiveEffect(BattleStateEnum.Doomed, 4));
    }
}