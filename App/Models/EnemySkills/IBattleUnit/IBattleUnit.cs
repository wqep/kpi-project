using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.EnemySkills.IBattleUnit;

public interface IBattleUnit
{
    public int HP { get; set; }
    public int MaxHp { get; set; }
    public int HandDmg { get; set; }
    public int PhisDefense { get; set; }
    public int MagicPower { get; set; }
    public List<ActiveEffect> CurrentEffects { get; set; }
}