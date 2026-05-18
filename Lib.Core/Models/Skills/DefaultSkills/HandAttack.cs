using Lib.Core.Interfaces;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models.Skills.DefaultSkills;

public class HandAttack : ISkill
{
    public string Name => "Hand Attack";
    
    public void Execute(IBattleUnit caster, IBattleUnit target)
    {
        var existDefensive = target.CurrentEffects.Find(e => e.BattleStateEnum == BattleStateEnum.Defensive);
        int totalDmg = caster.HandDmg - target.PhisDefense;
        if (existDefensive != null)
        {
            totalDmg -= 2;
        }
        
        totalDmg = Math.Max(0, totalDmg);
        target.Hp -= totalDmg;
    }
}