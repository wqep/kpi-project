using Lib.Core.BaseClasses;
using Lib.Core.Interfaces;
using Lib.Core.Models.Skills.SpecialSkills;

namespace Lib.Core.Models.Enemies.ThirdLocation;

public class Skeleton : EnemyBase, IBattleUnit
{
    public Skeleton()
    {
        Hp = 65;
        MaxHp = 65;
        HandDmg = 11;
        Name = "Skeleton";
        PhisDefense = 7;
        CurrentSkills.Add(new Frenzy(2));
        CurrentSkills.Add(new Frighten());
    }
    
    public override void CastSkill(ISkill chosenSkill, IBattleUnit target)
    {
        chosenSkill.Execute(this, target);
    }
}