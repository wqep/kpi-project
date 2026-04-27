using KPI_PROJECT.Models.BaseClasses;
using KPI_PROJECT.Models.EnemySkills;
using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.FirstLevelEnemys;

public class WiseMagician : EnemyBase
{
    public WiseMagician()
    {
        HP = 35;
        MaxHp = 35;
        HandDmg = 6;
        Name = "Wise Magician";
        PhisDefense = 2;
        Speed = 5;
    }
    public List<ISkill> CurrentSkills { get; set; } = new List<ISkill>()
    {
        new CharmSkill(),
        new FireballSkill(15),
    };
    public  void HandAttack( Player player)
    {
       
    }
    
    /// <summary>
    /// Strong Charming. Lowers defense (-4), and lets other skills to hurt harder (x2,5)
    /// </summary>
    public  void UseSkill1(Player player)
    {
        
    }
    
    public  void UseSkill2(Player player)
    {
        
        
    }
    
    public  void Defend()
    {
        this.CurrentEffects.Add(new ActiveEffect(BattleState.Defensive, 3));
    }
}