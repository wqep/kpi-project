using Lib.Core.Interfaces;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class ThunderStrikeSkill : ISkill
{
    public string Name => "Thunderstrike";
    public int BaseDmg { get; set; }
    public int Multiplier { get; set; }
    
    public ThunderStrikeSkill(int totalDmg)
    {
        BaseDmg = totalDmg;
        Multiplier = 2;
    }

    public void Execute(IBattleUnit caster, IBattleUnit target)
    {
        var existCharm = target.CurrentEffects.Find(e => e.BattleStateEnum == BattleStateEnum.Charmed); 
        if (existCharm != null)
        {
            BaseDmg *= Multiplier;
        }

        target.Hp = Math.Max(0, target.Hp - BaseDmg);
    }
}