using Lib.Core.BaseClasses;
using Lib.Core.Interfaces;
using Lib.Core.Models.Skills.SpecialSkills;

namespace Lib.Core.Models.Enemies.ThirdLocation;

public class HeadlessWarden : EnemyBase, IBattleUnit
{
    public HeadlessWarden()
    {
        Hp = 75;
        MaxHp = 75;
        HandDmg = 13;
        Name = "HeadlessWarden";
        PhisDefense = 9;
        CurrentSkills.Add(new Frenzy(2));
        CurrentSkills.Add(new Frighten());
        CurrentSkills.Add(new HalebardStrike(18));
    }
    
    public override void CastSkill(ISkill chosenSkill, IBattleUnit target)
    {
        chosenSkill.Execute(this, target);
    }
}