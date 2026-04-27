using KPI_PROJECT.Models.BaseClasses;
using KPI_PROJECT.Models.EnemySkills;
using KPI_PROJECT.Models.EnemySkills.IBattleUnit;
using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.FirstLevelEnemies;

public class StarterMagician : EnemyBase, IBattleUnit
{
    public StarterMagician()
    {
        HP = 25;
        MaxHp = 25;
        HandDmg = 5;
        Name = "Wise Magician";
        MagicPower = 5;
        PhisDefense = 1;
        Speed = 4;
        CurrentSkills.Add(new FireballSkill(10));
        CurrentSkills.Add(new CharmSkill());
    }


    public void CastSkill(ISkill chosenSkill, IBattleUnit target)
    {
        chosenSkill.Execute(this, target);
    }
}