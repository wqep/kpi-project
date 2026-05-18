using Lib.Core.Interfaces;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class Frenzy : ISkill
{
    public string Name => "Frenzy";
    public int FrenzyDuration { get; set; }

    public Frenzy(int frenzyDuration)
    {
        FrenzyDuration = frenzyDuration;
    }
    
    public void Execute(IBattleUnit casteer, IBattleUnit target)
    {
        casteer.CurrentEffects.Add(new ActiveEffect(BattleStateEnum.Frenzy, FrenzyDuration));
    }
}