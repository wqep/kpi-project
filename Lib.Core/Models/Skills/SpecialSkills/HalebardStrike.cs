using Lib.Core.Interfaces;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class HalebardStrike : ISkill
{
    public string Name => "HalebardStrike";
    public int BaseDmg;
    
    public HalebardStrike(int baseDmg)
    {
        BaseDmg = baseDmg;
    }
    
    public void Execute(IBattleUnit casteer, IBattleUnit target)
    {
        target.Hp = Math.Max(0, target.Hp - BaseDmg);
    }
}