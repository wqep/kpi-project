using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.BaseClasses;

public abstract class EnemyBase
{
    public int HP { get; set; }
    public int MaxHp { get; set; }
    public int HandDmg { get; set; }
    public string Name { get; set; }
    public int PhisDefence { get; set; }
    public int Speed { get; set; }
    
    public bool IsDead => HP <= 0;
    public List<ActiveEffect> CurrentEffects { get; set; }

    public abstract void HandAttack( Player player);
    public abstract void UseSkill( Player player);
    public abstract void UseSkill2( Player player);
    public abstract void Defend();
}