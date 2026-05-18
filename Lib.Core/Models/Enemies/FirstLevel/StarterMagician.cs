using Lib.Core.BaseClasses;
using Lib.Core.Interfaces;
using Lib.Core.Models.Skills.SpecialSkills;

namespace Lib.Core.Models.Enemies.FirstLevel;

public class StarterMagician : EnemyBase, IBattleUnit
{
    public StarterMagician()
    {
        Hp = 25;
        MaxHp = 25;
        HandDmg = 5;
        Name = "Wise Magician";
        MagicPower = 5;
        PhisDefense = 1;
        CurrentSkills.Add(new FireballSkill(10));
        CurrentSkills.Add(new CharmSkill());
    }
    
    public override void CastSkill(ISkill chosenSkill, IBattleUnit target)
    {
        chosenSkill.Execute(this, target);
    }
}