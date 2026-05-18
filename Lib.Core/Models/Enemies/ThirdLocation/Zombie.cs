using Lib.Core.BaseClasses;
using Lib.Core.Interfaces;
using Lib.Core.Models.Skills.SpecialSkills;

namespace Lib.Core.Models.Enemies.ThirdLocation;

public class Zombie : EnemyBase, IBattleUnit
{
    public Zombie()
    {
        Hp = 70;
        MaxHp = 70;
        HandDmg = 12;
        Name = "Zombie";
        PhisDefense = 8;
        CurrentSkills.Add(new Frenzy(2));
        CurrentSkills.Add(new Frighten());
        CurrentSkills.Add(new Bite(15));
    }
    
    public override void CastSkill(ISkill chosenSkill, IBattleUnit target)
    {
        chosenSkill.Execute(this, target);
    }
}