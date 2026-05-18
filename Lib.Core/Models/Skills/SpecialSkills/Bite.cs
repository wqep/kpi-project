using Lib.Core.Interfaces;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class Bite : ISkill
{
    public string Name => "Bite";
    public int BaseDmg;
    
    public Bite(int baseDmg)
    {
        BaseDmg = baseDmg;
    }
    
    public void Execute(IBattleUnit casteer, IBattleUnit target)
    {
        target.Hp = Math.Max(0, target.Hp - BaseDmg);
    }
}