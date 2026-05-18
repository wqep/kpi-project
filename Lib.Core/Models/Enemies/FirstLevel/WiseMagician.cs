using Lib.Core.BaseClasses;
using Lib.Core.Interfaces;
using Lib.Core.Models.Skills.SpecialSkills;

namespace Lib.Core.Models.Enemies.FirstLevel;

public class WiseMagician : EnemyBase, IBattleUnit
{
    public WiseMagician()
    {
        Hp = 35;
        MaxHp = 35;
        HandDmg = 6;
        Name = "Wise Magician";
        PhisDefense = 2;
        CurrentSkills.Add(new FireballSkill(12));
        CurrentSkills.Add(new CharmSkill());
        CurrentSkills.Add(new ThunderStrikeSkill(5));
    }
    
    public override void CastSkill(ISkill chosenSkill, IBattleUnit target)
    {
        chosenSkill.Execute(this, target);
    }
}