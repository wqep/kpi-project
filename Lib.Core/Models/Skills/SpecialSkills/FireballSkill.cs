using Lib.Core.Interfaces;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models.Skills.SpecialSkills;

public class FireballSkill : ISkill
{
    public string Name => "Fireball";
    public int BaseDmg { get; set; }
    public int Multiplier { get; set; }

    public FireballSkill(int dmg)
    {
        BaseDmg = dmg;
        Multiplier = 2;
    }

    public void Execute(IBattleUnit caster, IBattleUnit target)
    {
        var existBurning = target.CurrentEffects.Find(e => e.BattleStateEnum == BattleStateEnum.Burning);
        var existCharm = target.CurrentEffects.Find(e => e.BattleStateEnum == BattleStateEnum.Charmed);
        int resDamage = BaseDmg + caster.MagicPower;
        if (existBurning != null)
        { 
            existBurning.TurnsLeft = 3;
        }
        else
        {
            target.CurrentEffects.Add(new ActiveEffect(BattleStateEnum.Burning, 3));
        }
        
        if (existCharm != null)
        {
            resDamage *= Multiplier;
        }
        
        target.Hp =  Math.Max(0, target.Hp - resDamage);
        
    }
}